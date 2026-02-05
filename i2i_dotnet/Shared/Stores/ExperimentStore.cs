using System.ComponentModel;
using i2i_dotnet.Core;
using i2i_dotnet.Features.TargetedTab.Models;

namespace i2i_dotnet.Shared.Stores;
public class ExperimentStore : ObservableObject, IExperimentStore, INotifyPropertyChanged
{
    private Experiment? _msExperiment = new();
    private IReadOnlyList<Analyte>?  _analytes;
    // TODO: Add model for an Analyte Matrix
    private List<double[,]> _analyteMatrix;

    public Experiment? MSExperiment
    {
        get {return _msExperiment;}
        set => Set(ref _msExperiment, value);
    }
    
    public IReadOnlyList<Analyte>? Analytes
    {
        get { return _analytes; }
        set => Set(ref _analytes, value);
    }
    public bool HasExperiment => MSExperiment != null;
    public bool HasAnalytes => Analytes is { Count: > 0 };

    //TODO: Fix type specification for an Analyte Matrix
    public List<double[,]> AnalyteMatrix
    {
        get => _analyteMatrix;
        set => Set(ref _analyteMatrix, value);
    }
    
    public void ClearAll()
    {
        MSExperiment = null;
        Analytes = null;
    } 
    
    
}