using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace i2i_learn
{
    internal class wrappers
    {
        public static double[,] TimeAlignment(ConcurrentBag<double[]> timeBag, ArrayList intensityList, ConcurrentBag<int> indexBag, double tmax)
        {


            var t0 = 0;
            var timeVector = MathNet.Numerics.Generate.LinearSpaced(20000, t0, tmax);
            var indexList = indexBag.ToList();
            ArrayList timeList = new ArrayList(timeBag.ToArray());
            //ArrayList intensityList = new ArrayList(intensityBag.ToArray());

            double[,] matris = new double[indexList.Count, timeVector.Length];

            for (int i = 0; i < indexList.Count; i++)
            {
                double[] currentTimeList = (double[])timeList[i];
                double[] currentIntensityList = (double[])intensityList[i];

                for (int j = 0; j < timeVector.Length; j++)
                {
                    var result = currentTimeList.Select(x => Math.Abs(x - timeVector[j])).ToArray();
                    int minIndex = Array.IndexOf(result, result.Min());
                    matris[indexList[i], j] = currentIntensityList[minIndex];
                }

            }
            return matris;
        }

        public static double[,] timeAlignment_single(List<double[]> timeBag, ArrayList intensityList, double tmax)
        {


            var t0 = 0;
            var timeVector = MathNet.Numerics.Generate.LinearSpaced(5000, t0, tmax);

            ArrayList timeList = new ArrayList(timeBag.ToArray());
            //ArrayList intensityList = new ArrayList(intensityBag.ToArray());

            double[,] matris = new double[timeBag.Count, timeVector.Length];

            for (int i = 0; i < timeBag.Count; i++)
            {
                double[] currentTimeList = (double[])timeList[i];
                double[] currentIntensityList = (double[])intensityList[i];

                for (int j = 0; j < timeVector.Length; j++)
                {
                    var result = currentTimeList.Select(x => Math.Abs(x - timeVector[j])).ToArray();
                    int minIndex = Array.IndexOf(result, result.Min());
                    matris[i, j] = currentIntensityList[minIndex];
                }

            }
            return matris;
        }

        public static double[,] timeAlignment_parallel(List<double[]> timeBag, ArrayList intensityList, double tmax)
        {
            var t0 = 0;
            var timeVector = MathNet.Numerics.Generate.LinearSpaced(5000, t0, tmax);

            ArrayList timeList = new ArrayList(timeBag.ToArray());

            double[,] matris = new double[timeBag.Count, timeVector.Length];

            Parallel.For(0, timeBag.Count, i =>
            {
                double[] currentTimeList = (double[])timeList[i];
                double[] currentIntensityList = (double[])intensityList[i];

                for (int j = 0; j < timeVector.Length; j++)
                {
                    var result = currentTimeList.Select(x => Math.Abs(x - timeVector[j])).ToArray();
                    int minIndex = Array.IndexOf(result, result.Min());
                    matris[i, j] = currentIntensityList[minIndex];
                }
            });

            return matris;
        }

        public static double[,] timeAlignment_parallel_concurrent(ConcurrentStack<double[]> timeBag, ConcurrentStack<double[]> intensityList, double tmax)
        {
            var t0 = 0;
            var timeVector = MathNet.Numerics.Generate.LinearSpaced(5000, t0, tmax);

            
            ArrayList timeList = new ArrayList(timeBag.ToArray());
            double[][] inten =  intensityList.ToArray();
            double[,] matris = new double[timeBag.Count, timeVector.Length];

            Parallel.For(0, timeBag.Count, i =>
            {
                
                double[] currentTimeList = (double[])timeList[i];
                double[] currentIntensityList = inten[i];

                for (int j = 0; j < timeVector.Length; j++)
                {
                    var result = currentTimeList.Select(x => Math.Abs(x - timeVector[j])).ToArray();
                    int minIndex = Array.IndexOf(result, result.Min());
                    matris[i, j] = currentIntensityList[minIndex];
                }
            });

            return matris;
        }


        public static ArrayList extractMassToCharge(ArrayList mzBag, ArrayList intensityBag, double numericMzValue)
        {
            ArrayList extractedDat = new ArrayList();
            for (int j = 0; j < mzBag.Count; j++)
            {
                ArrayList mzvals = (ArrayList)mzBag[j];
                ArrayList intvals = (ArrayList)intensityBag[j];
                double[] intenstyForExtractedMz = new double[mzvals.Count];

                for (int i = 0; i < mzvals.Count; i++)
                {
                    double[] mzvals2 = (double[])mzvals[i];
                    double[] intvals2 = (double[])intvals[i];

                    double[] ppmdiff = (double[])ThermoWrappers.PpmArrayAvx(numericMzValue, mzvals2);

                    ppmdiff = ppmdiff.Select(Math.Abs).ToArray();
                    int minIndex = Array.IndexOf(ppmdiff, ppmdiff.Min());
                    if (ppmdiff[minIndex] < 5)
                    {
                        intenstyForExtractedMz[i] = intvals2[minIndex];
                    }
                    else
                    {
                        intenstyForExtractedMz[i] = 0;
                    }


                }
                extractedDat.Add(intenstyForExtractedMz);
            }
            return extractedDat;
        }





        public static ArrayList extractMassToChargeParallel(ArrayList mzBag, ArrayList intensityBag, double numericMzValue)
            {
                ArrayList extractedDat = new ArrayList();
                Dictionary<int, double[]> resultDict = new Dictionary<int, double[]>();

                Parallel.ForEach(Enumerable.Range(0, mzBag.Count), j =>
                {
                    ArrayList mzvals = (ArrayList)mzBag[j];
                    ArrayList intvals = (ArrayList)intensityBag[j];
                    double[] intenstyForExtractedMz = new double[mzvals.Count];

                    for (int i = 0; i < mzvals.Count; i++)
                    {
                        double[] mzvals2 = (double[])mzvals[i];
                        double[] intvals2 = (double[])intvals[i];

                        double[] ppmdiff = (double[])ThermoWrappers.ppmArray(numericMzValue, mzvals2);

                        ppmdiff = ppmdiff.Select(Math.Abs).ToArray();
                        int minIndex = Array.IndexOf(ppmdiff, ppmdiff.Min());
                        if (ppmdiff[minIndex] < 5)
                        {
                            intenstyForExtractedMz[i] = intvals2[minIndex];
                        }
                        else
                        {
                            intenstyForExtractedMz[i] = 0;
                        }
                    }

                    lock (resultDict)
                    {
                        resultDict[j] = intenstyForExtractedMz;
                    }
                });

                var orderedResults = resultDict.OrderBy(item => item.Key).Select(item => item.Value).ToList();
                extractedDat.AddRange(orderedResults);

                return extractedDat;
            }

       public static PlotModel makeHeatMapFig(double[,] matris)
            {
                var modelout = new PlotModel { Title = "Heatmap" };

                // Color axis (the X and Y axes are generated automatically)
                modelout.Axes.Add(new LinearColorAxis
                {
                    Palette = OxyPalettes.Viridis(256)
                }
                );


                var heatMapSeries = new HeatMapSeries
                {
                    X0 = 0,
                    X1 = matris.GetLength(1),
                    Y0 = 0,
                    Y1 = matris.GetLength(0),
                    Interpolate = false,
                    RenderMethod = HeatMapRenderMethod.Bitmap,
                    Data = matris
                };

                LinearAxis xAxis = new LinearAxis
                {
                    Position = AxisPosition.Bottom,
                    Minimum = 0,
                    Maximum = matris.GetLength(1),
                    IsAxisVisible = false,
                    IsZoomEnabled = false
                };

                LinearAxis yAxis = new LinearAxis
                {
                    Position = AxisPosition.Left,
                    Minimum = 0,
                    Maximum = matris.GetLength(0),
                    IsAxisVisible = false,
                    IsZoomEnabled = false
                };



            
                modelout.Axes.Add(yAxis);
                modelout.Axes.Add(xAxis);
                modelout.Series.Add(heatMapSeries);


            return modelout;
            }

            public static double aspectratfunc(double velocity, double spacing, double tmax, int numFiles)
            {
                var x_dim = velocity * tmax * 60;
                var y_dim = spacing * numFiles;
                var aspectRatio = x_dim / y_dim;
                return aspectRatio;
            }
        static int FindClosestIndexWithAvx2(Span<double> dataSpan, double targetNumber)
        {
            if (dataSpan.Length == 0)
                throw new ArgumentException("The span is empty.");

            const int VectorSize = 4; // AVX2 uses 256 bits, which can store 4 doubles
            int remainder = dataSpan.Length % VectorSize;
            int vectorizedLength = dataSpan.Length - remainder;

            Vector<double> targetVector = new Vector<double>(targetNumber);

            int closestIndex = 0;
            double minDifference = Math.Abs(targetNumber - dataSpan[0]);

            for (int i = 0; i < vectorizedLength; i += VectorSize)
            {
                Vector<double> dataVector = new Vector<double>(dataSpan.Slice(i, VectorSize));

                // Calculate absolute differences using SIMD
                Vector<double> differenceVector = Vector.Abs(Vector.Subtract(dataVector, targetVector));

                // Find the minimum difference and corresponding index
                double[] differences = new double[VectorSize];
                differenceVector.CopyTo(differences);

                for (int j = 0; j < VectorSize; j++)
                {
                    if (differences[j] < minDifference)
                    {
                        minDifference = differences[j];
                        closestIndex = i + j;
                    }
                }
            }

            // Handle the remaining elements
            for (int i = vectorizedLength; i < dataSpan.Length; i++)
            {
                double currentDifference = Math.Abs(targetNumber - dataSpan[i]);

                if (currentDifference < minDifference)
                {
                    minDifference = currentDifference;
                    closestIndex = i;
                }
            }

            return closestIndex;
        }


          

    }

}
