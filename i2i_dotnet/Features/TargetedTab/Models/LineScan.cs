namespace i2i_dotnet.Features.TargetedTab.Models;

public class LineScan
{
   
    private readonly List<Spectrum> _spectra = new();
    
    public int spectraCount => _spectra.Count;
    

    public LineScan()
    {
    }
    
    public void AddSpectra(Spectrum spectra)
    {
        _spectra.Add(spectra);
    }

    public void AddSpectras(IEnumerable<Spectrum> spectras)
    {
        _spectra.AddRange(spectras);
    }
}