using i2i_dotnet.Core;
using i2i_dotnet.Features.TargetedTab.Models;

namespace i2i_dotnet.Shared.Stores;
public class ExperimentStore : ObservableObject
{
    private Experiment? _msExperiment;
    private IReadOnlyList<Analyte>?  _analytes;

    public Experiment MSExperiment
    {
        get {return _msExperiment;}
        set => Set(ref _msExperiment, value);
    }
    
    public IReadOnlyList<Analyte> Analytes
    {
        get { return _analytes; }
        set => Set(ref _analytes, value);
    }
    public bool HasExperiment => MSExperiment != null;
    public bool HasAnalytes => Analytes != null && Analytes.Count > 0;

    public void ClearAll()
    {
        MSExperiment = null;
        Analytes = null;
    } 
}