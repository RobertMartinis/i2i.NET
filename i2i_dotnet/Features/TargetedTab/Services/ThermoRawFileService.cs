using ThermoFisher.CommonCore.RawFileReader;
using ThermoFisher.CommonCore.Data.Business;
using System.IO;
using System.Windows.Shapes;
using i2i_dotnet.Features.TargetedTab.Models;
using ThermoFisher.CommonCore.Data;

namespace i2i_dotnet.Features.TargetedTab.Services
{
    /// <summary>
    /// Class for loading and retrieving spectral information from raw Thermo Fisher files.
    /// </summary>
    /// 
    public class ThermoRawFileService : IRawFileService

    {
    /// <summary>
    /// Reads a Thermo raw file, and converts the contents to a MSSpectrum data type. 
    /// </summary>
    /// <param name="filePath">String to the path of a .raw file.</param>
    /// <returns>A populated MSSpectrum.</returns>
    private LineScan LoadFileToMsSpectra(string filePath)
    {
        LineScan rawFileSpectrums = new LineScan();

        var rawFile = RawFileReaderAdapter.FileFactory(filePath);
        rawFile.SelectInstrument(Device.MS, 1);

        for (int i = 1; i < rawFile.RunHeaderEx.SpectraCount; i++)
        {

            var scanStatistics = rawFile.GetScanStatsForScanNumber(i);
            string scanFilter = rawFile.GetFilterForScanNumber(i).ToString();
            
            if (scanStatistics.IsCentroidScan && scanStatistics.SpectrumPacketType == SpectrumPacketType.FtCentroid)
            {
                var centroidStream = rawFile.GetCentroidStream(i, false);

                Spectrum spectra = new Spectrum(i, centroidStream.Masses, centroidStream.Intensities, scanFilter);

                rawFileSpectrums.AddSpectra(spectra);
            }

            else
            {
                var segmentedScan = rawFile.GetSegmentedScanFromScanNumber(i, scanStatistics);

                Spectrum spectra = new Spectrum(i, segmentedScan.Positions, segmentedScan.Intensities, scanFilter);

                rawFileSpectrums.AddSpectra(spectra);
            }

        }
        
        return rawFileSpectrums;
    }

    /// <summary>
    /// Function to load .raw files from a folder. 
    /// </summary>
    /// <param name="folderPath">Path to the folder containing .raw files.</param>
    /// <returns>A experiment object, containing LineScans representing the rows in the experiment.</returns>
    public Experiment LoadRawFilesFromFolder(string folderPath, IProgress<double>? progress = null)
    {
        Experiment exp = new Experiment();

        var rawDirectories = Directory.GetFiles(folderPath, "*.raw");

        for (int i = 0; i < rawDirectories.Length; i++)
        {
            try
            {
                var rawFile = rawDirectories[i];
                LineScan spectrumFromFile = LoadFileToMsSpectra(rawFile);
                exp.AddLineScan(spectrumFromFile);
                progress?.Report((i + 1) * 100.0 / rawDirectories.Length);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load {rawDirectories[i]}: {ex.Message}");
            }
        }

        return exp;
    }


    }

}
