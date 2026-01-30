
using i2i_dotnet.Features.TargetedTab.Model;
using ThermoFisher.CommonCore.RawFileReader;
using ThermoFisher.CommonCore.Data.Business;
using System.IO;
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
    private List<MSExperiment> LoadFileToMsSpectra(string filePath)
    {
        List<MSExperiment> rawFileSpectrums = new List<MSExperiment>();

        var rawFile = RawFileReaderAdapter.FileFactory(filePath);

        rawFile.SelectInstrument(Device.MS, 1);

        for (int i = 1; i < rawFile.RunHeaderEx.SpectraCount; i++)
        {

            var scanStatistics = rawFile.GetScanStatsForScanNumber(i);
            string scanFilter = rawFile.GetFilterForScanNumber(i).ToString();
            if (scanStatistics.IsCentroidScan && scanStatistics.SpectrumPacketType == SpectrumPacketType.FtCentroid)
            {
                var centroidStream = rawFile.GetCentroidStream(i, false);

                MSExperiment spectra = new MSExperiment(i, centroidStream.Masses, centroidStream.Intensities, scanFilter);

                rawFileSpectrums.Add(spectra);
            }

            else
            {
                var segmentedScan = rawFile.GetSegmentedScanFromScanNumber(i, scanStatistics);

                MSExperiment spectra = new MSExperiment(i, segmentedScan.Positions, segmentedScan.Intensities, scanFilter);

                rawFileSpectrums.Add(spectra);
            }

        }

        return rawFileSpectrums;
    }

    /// <summary>
    /// Function to load .raw files from a folder. 
    /// </summary>
    /// <param name="folderPath">Path to the folder containing .raw files.</param>
    /// <returns>A 2D-list of MSSpectrum objects, where the first index is a row and second index a spectra in that row.</returns>
    public List<List<MSExperiment>> LoadRawFilesFromFolder(string folderPath)
    {
        var allSpectra = new List<List<MSExperiment>>();

        var rawDirectories = Directory.GetFiles(folderPath, "*.raw");

        foreach (var rawDir in rawDirectories)
        {
            try
            {
                List<MSExperiment> spectrumFromFile = LoadFileToMsSpectra(rawDir);
                allSpectra.Add(spectrumFromFile);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load {rawDir}: {ex.Message}");
            }
        }

        return allSpectra;
    }


    }

}
