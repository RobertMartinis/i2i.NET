using i2i_learn.Data;
using ThermoFisher.CommonCore.Data;
using ThermoFisher.CommonCore.Data.Business;
using ThermoFisher.CommonCore.Data.Interfaces;
using ThermoFisher.CommonCore.RawFileReader;
using System.Diagnostics;
using Emgu.CV.Features2D;
using System.Xml.Linq;
using System.IO;
namespace i2i_dotnet.Services
{
    /// <summary>
    /// Class for loading and retrieving spectral information from raw Thermo Fisher files.
    /// </summary>
    /// 
    class ThermoRawFileService
    {

        public List<MSSpectrum> Spectra { get; private set; } = new List<MSSpectrum>();

     
        public void LoadFileToMSSpectra(string filePath)
        {

            var rawFile = RawFileReaderAdapter.FileFactory(filePath);

        }
    }
}

