using System.Globalization;
using System.IO;
using System.Xml.Linq;
using i2i_dotnet.Features.TargetedTab.Models;
using System.Linq;
using System.Threading.Tasks;
namespace i2i_dotnet.Features.TargetedTab.Services;

public class MzMLFileService : ImzMLFileService
{
    private HashSet<string> _scanFilters = new();
    public LineScan LoadFileToMsSpectra(string filePath)
    {
        var doc = XDocument.Load(filePath);
        XNamespace ns = doc.Root!.Name.Namespace;
        

        var lineScan = new LineScan();

        var spectraEls = doc.Descendants(ns + "spectrum").ToList();

        foreach (var spec in spectraEls)
        {
            // ---- get mz + intensity (assume binaryDataArray(1)=mz, (2)=intensity) ----
            var bdas = spec.Descendants(ns + "binaryDataArray").ToList();
            if (bdas.Count < 2) continue;

            var mzBda = bdas[0];
            var itBda = bdas[1];

            bool is64 = itBda.Elements(ns + "cvParam").Any(x => (string?)x.Attribute("name") == "64-bit float");
            // if not 64-bit assume 32-bit float
            string mzBase64 = mzBda.Descendants(ns + "binary").First().Value.Trim();
            string itBase64 = itBda.Descendants(ns + "binary").First().Value.Trim();

            byte[] mzBytes = Convert.FromBase64String(mzBase64);
            byte[] itBytes = Convert.FromBase64String(itBase64);

            double[] mz = is64 ? BytesToDoubles(mzBytes) : BytesToFloatsAsDoubles(mzBytes);
            double[] it = is64 ? BytesToDoubles(itBytes) : BytesToFloatsAsDoubles(itBytes);

            // ---- TIC from spectrum cvParam ----
            var cv = spec.Elements(ns + "cvParam").ToList();
            double tic = ReadCvDouble(cv, "total ion current") ?? 0.0;

            // ---- scan filter + retention time from scan cvParam ----
            var scanEl = spec.Descendants(ns + "scan").FirstOrDefault();
            var scanCv = scanEl?.Elements(ns + "cvParam").ToList() ?? new List<XElement>();

            // MATLAB used scan.cvParam(1).valueAttribute (not stable); here’s the common one:
            double rt = ReadCvDouble(scanCv, "scan start time") ?? 0.0;

            string scanFilter = ReadCvString(scanCv, "filter string") ?? "";
            
            _scanFilters.Add(scanFilter);

            // Adapt to your Spectrum constructor
            var spectrum = new Spectrum(
                
                mzlist: mz.ToList(),
                intensities: it.ToList(),
                scanfilter: scanFilter,
                retentionTime: rt,
                scanNumber:1
            );

            lineScan.AddSpectra(spectrum);
        }

        return lineScan;
    }

    private static string? ReadCvString(IEnumerable<XElement> cvParams, string name)
        => (string?)cvParams.FirstOrDefault(x => (string?)x.Attribute("name") == name)?.Attribute("value");

    private static double? ReadCvDouble(IEnumerable<XElement> cvParams, string name)
    {
        var s = ReadCvString(cvParams, name);
        if (s is null) return null;
        return double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var v) ? v : null;
    }

    private static double[] BytesToDoubles(byte[] bytes)
    {
        int n = bytes.Length / 8;
        var arr = new double[n];
        Buffer.BlockCopy(bytes, 0, arr, 0, bytes.Length);
        return arr;
    }

    private static double[] BytesToFloatsAsDoubles(byte[] bytes)
    {
        int n = bytes.Length / 4;
        var floats = new float[n];
        Buffer.BlockCopy(bytes, 0, floats, 0, bytes.Length);
        var doubles = new double[n];
        for (int i = 0; i < n; i++) doubles[i] = floats[i];
        return doubles;
    }

    public (Experiment, string[]) LoadMzmlFilesFromFolder(string folderPath, IProgress<double>? progress = null)
    {
        Experiment exp = new Experiment();
        var mzmlfiles = Directory.GetFiles(folderPath, "*.mzml");
        int numFiles = mzmlfiles.Length;
        int done = 0;
        LineScan[] linescans = new LineScan[numFiles];
        Parallel.For(0, numFiles, i =>
        {
            try
            {
                LineScan spectrumFromFile = LoadFileToMsSpectra(mzmlfiles[i]);
                linescans[i] = spectrumFromFile;
                int step = Interlocked.Increment(ref done);
                progress?.Report((step) * 100.0 / mzmlfiles.Length);
            }

            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        });

        exp.AddLineScans(linescans);
        return (exp, _scanFilters.ToArray());
    }
}
