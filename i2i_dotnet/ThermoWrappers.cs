using System.Collections;
using System;
using MathNet.Numerics.LinearAlgebra;
using ThermoFisher.CommonCore.Data;
using ThermoFisher.CommonCore.Data.Business;
using ThermoFisher.CommonCore.Data.Interfaces;
using ThermoFisher.CommonCore.RawFileReader;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics;
using System.Collections.Concurrent;
using System.Formats.Tar;
using Range = ThermoFisher.CommonCore.Data.Business.Range;

namespace i2i_dotnet
{
    internal class ThermoWrappers
    {

        public static (double[] intensities, double[] time) LoadFileSingle(string fileNameInput, string selectedFilter)
        {

            var rawFile = RawFileReaderFactory.ReadFile(fileNameInput);


            rawFile.SelectInstrument(Device.MS, 1);

            var options = rawFile.DefaultMassOptions();
            options.ToleranceUnits = ToleranceUnits.ppm;

            int firstScanNumber = rawFile.RunHeaderEx.FirstSpectrum;
            int lastScanNumber = rawFile.RunHeaderEx.LastSpectrum;

            ChromatogramTraceSettings settings = new ChromatogramTraceSettings(TraceType.MassRange)
            {
                Filter = selectedFilter,
                MassRanges = new[] { ThermoFisher.CommonCore.Data.Business.Range.Create(866.4800, 866.4830) }

            };



            settings.Filter = selectedFilter;

            var extracted_chrom = rawFile.GetChromatogramData(new IChromatogramSettings[] { settings }, firstScanNumber, lastScanNumber);
            var trace = ChromatogramSignal.FromChromatogramData(extracted_chrom);

            double[] time = (double[])trace[0].Times;
            double[] intensities = (double[])trace[0].Intensities;
            return (intensities, time);

        }

        public static (ArrayList massesOneScan, ArrayList intensitiesOneScan, double[] timeArray) LoadAndStoreFile(string fileNameInput, string selectedFilter)
        {
            var basecase = RawFileReaderAdapter.ThreadedFileFactory(fileNameInput);
            var rawFile = basecase.CreateThreadAccessor();

            rawFile.SelectInstrument(Device.MS, 1);

            int firstScanNumber = rawFile.RunHeaderEx.FirstSpectrum;
            int lastScanNumber = rawFile.RunHeaderEx.LastSpectrum;
            List<int> filteredScansList = rawFile.GetFilteredScansListByScanRange(selectedFilter, firstScanNumber, lastScanNumber);

            ArrayList massesOneScan = new ArrayList(filteredScansList.Count);
            ArrayList intensitiesOneScan = new ArrayList(filteredScansList.Count);
            double[] timeArray = new double[filteredScansList.Count];

            for (int i = 0; i < filteredScansList.Count; i++)

            {

                timeArray[i] = rawFile.RetentionTimeFromScanNumber(filteredScansList[i]);
                massesOneScan.Add(rawFile.GetAdvancedPacketData(filteredScansList[i]).CentroidData.Masses);
                intensitiesOneScan.Add(rawFile.GetAdvancedPacketData(filteredScansList[i]).CentroidData.Intensities);


            }
            rawFile.Dispose();
            return (massesOneScan, intensitiesOneScan, timeArray);
        }

        public static (ArrayList massesOneScan, ArrayList intensitiesOneScan, double[] timeArray) LoadAndStoreFileParallel(string fileNameInput, string selectedFilter)
        {
            var basecase = RawFileReaderAdapter.ThreadedFileFactory(fileNameInput);
            var rawFile = basecase.CreateThreadAccessor();

            rawFile.SelectInstrument(Device.MS, 1);

            int firstScanNumber = rawFile.RunHeaderEx.FirstSpectrum;
            int lastScanNumber = rawFile.RunHeaderEx.LastSpectrum;
            List<int> filteredScansList = rawFile.GetFilteredScansListByScanRange(selectedFilter, firstScanNumber, lastScanNumber);

            ConcurrentBag<double[]> massesOneScan = new ConcurrentBag<double[]>();
            ConcurrentBag<double[]> intensitiesOneScan = new ConcurrentBag<double[]>();
            double[] timeArray = new double[filteredScansList.Count];

            Parallel.For(0, filteredScansList.Count, i =>
            {
                var rawAccess = basecase.CreateThreadAccessor();
                rawAccess.SelectInstrument(Device.MS, 1);

                timeArray[i] = rawAccess.RetentionTimeFromScanNumber(filteredScansList[i]);
                massesOneScan.Add(rawAccess.GetAdvancedPacketData(filteredScansList[i]).CentroidData.Masses);
                intensitiesOneScan.Add(rawAccess.GetAdvancedPacketData(filteredScansList[i]).CentroidData.Intensities);
            });

            // Convert ConcurrentBag to ArrayList
            ArrayList massesArrayList = new ArrayList(massesOneScan);
            ArrayList intensitiesArrayList = new ArrayList(intensitiesOneScan);

            rawFile.Dispose();
            return (massesArrayList, intensitiesArrayList, timeArray);
        }

        public static Array ppmArray(double comparitor, double[] arrayMz)
        {
            double[] tmp = arrayMz.Select(x => x - comparitor).ToArray();
            tmp.Select(x => (x / comparitor) * 1000000).ToArray();
            return (tmp);

        }
        public static double[] PpmArrayAvx(double comparitor, double[] arrayMz)
        {
            var comparitorVector = Vector256.Create(comparitor);
            var result = new double[arrayMz.Length];
            var millionfactor = Vector256.Create<double>(1000000);
            var ppmFactor = Avx.Divide(millionfactor, comparitorVector);
            // Iterate through the array in blocks of 4, processing with AVX256

            for (int i = 0; i <= arrayMz.Length - 4; i += 4)
            {
                var mzVector = Vector256.Create(arrayMz[i], arrayMz[i + 1], arrayMz[i + 2], arrayMz[i + 3]);
                var diffVector = Avx.Subtract(mzVector, comparitorVector);
                var divideVector = Avx.Divide(diffVector, comparitorVector);
                var ppmVector = Avx.Multiply(divideVector, millionfactor);


                // Store the results directly into the result array using Avx.Store
                //Avx.StoreLow(result, i, ppmVector);
                ppmVector.CopyTo(result, i);
            }

            // Handle any remaining elements with regular scalar operations
            for (int i = arrayMz.Length - (arrayMz.Length % 4); i < arrayMz.Length; i++)
            {
                result[i] = ((arrayMz[i] - comparitor) / comparitor) * 1000000.0;
            }

            return result;
        }

        public static IRawDataExtended LoadAndStoreFile_fullfile(string fileNameInput)
        {
            var basecase = RawFileReaderAdapter.ThreadedFileFactory(fileNameInput);
            IRawDataExtended rawFile = basecase.CreateThreadAccessor();

            //rawFile.SelectInstrument(Device.MS, 1);


            return (rawFile);
        }

        public static (double[], double[]) GetEIC(double mzval, IRawDataPlus file, string filterApp, double tolerance_num, string unit)
        {

            MassOptions tolerance = new MassOptions() { Tolerance = tolerance_num, ToleranceUnits = ToleranceUnits.ppm };

            switch (unit)
            {
                case "ppm":
                    tolerance.ToleranceUnits = ToleranceUnits.ppm;
                    break;
                case "mmu":
                    tolerance.ToleranceUnits = ToleranceUnits.mmu;
                    break;
                case "amu":
                    tolerance.ToleranceUnits = ToleranceUnits.amu;
                    break;
            }



            file.SelectInstrument(Device.MS, 1);
            // Define the settings for getting the Base Peak chromatogram
            ChromatogramTraceSettings settings = new ChromatogramTraceSettings(TraceType.MassRange)
            {
                Filter = filterApp,
                MassRanges = new[] { new Range(mzval, mzval) }
            };


            // Get the chromatogram from the RAW file. 

            var data = file.GetChromatogramData(new IChromatogramSettings[] { settings }, -1, -1, tolerance);



            double[] intensitiy_vector = data.IntensitiesArray[0];
            double[] time_vector = data.PositionsArray[0];
            file.Dispose();
            return (intensitiy_vector, time_vector);
        }

        private static ChromatogramSignal[] GetUnfilteredTic(IRawFileThreadManager manager)
        {
            ChromatogramSignal[] chroTrace;
            using (IRawDataPlus file = manager.CreateThreadAccessor())
            {
                // open MS data
                file.SelectInstrument(Device.MS, 1);

                // Define settings for Tic
                var settingsTic = new ChromatogramTraceSettings(TraceType.TIC);

                // read the chromatogram
                var data = file.GetChromatogramData(new IChromatogramSettings[] { settingsTic }, -1, -1);

                // split the data into chromatograms
                chroTrace = ChromatogramSignal.FromChromatogramData(data);
            }
            return chroTrace;
        }

    }
}
