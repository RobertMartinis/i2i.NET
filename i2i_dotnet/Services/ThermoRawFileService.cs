using i2i_learn.Data;
using ThermoFisher.CommonCore.RawFileReader;

namespace i2i_dotnet.Services
{
    /// <summary>
    /// Class for loading and retrieving spectral information from raw Thermo Fisher files.
    /// </summary>
    /// 
    class ThermoRawFileService
    {
        /// <summary>
        /// Reads a Thermo raw file, and converts the contents to a MSSpectrum data type. 
        /// </summary>
        /// <param name="filePath">String to the path of a .raw file.</param>
        /// <returns>A populated MSSpectrum.</returns>
        public List<MSSpectrum> LoadFileToMSSpectra(string filePath)
        {
            List<MSSpectrum> rawFileSpectrums = new List<MSSpectrum>();
           
            var rawFile = RawFileReaderAdapter.FileFactory(filePath);

            for (int i = 0; i < rawFile.RunHeaderEx.SpectraCount; i++)
            { 
                
                var centroidStream = rawFile.GetCentroidStream(i, false);

                string scanFilter = rawFile.GetFilterForScanNumber(i).ToString();

                MSSpectrum spectra = new MSSpectrum(i, centroidStream.Masses, centroidStream.Intensities, scanFilter);

                rawFileSpectrums.Add(spectra);
            
            }

            return rawFileSpectrums;
        }
    }

}
