namespace i2i_dotnet.Features.TargetedTab.Models;

public class LineScan
{
   
    private readonly List<Spectrum> _spectra = new();
    
    public int spectraCount => _spectra.Count;
    
    
    public void AddSpectra(Spectrum spectra)
    {
        _spectra.Add(spectra);
    }

    public void AddSpectras(IEnumerable<Spectrum> spectras)
    {
        _spectra.AddRange(spectras);
    }
    
    public List<Spectrum> GetSpectras()
    {
        return _spectra;
    }

    public Spectrum GetSpectra(int index)
    {
        return _spectra[index];
    }
    
    public Dictionary<string, Spectrum[]> GetSpectrasByScanFilter(StringComparer? comparer = null)
    {
        comparer ??= StringComparer.Ordinal;

        return _spectra
            .GroupBy(s => s.ScanFilter ?? string.Empty, comparer)
            .ToDictionary(g => g.Key, g => g.ToArray(), comparer);
    }
}