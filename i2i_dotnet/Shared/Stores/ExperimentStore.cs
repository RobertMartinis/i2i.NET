using CommunityToolkit.Mvvm.ComponentModel;
using i2i_dotnet.Features.TargetedTab.Models;

namespace i2i_dotnet.Shared.Stores;

public partial class ExperimentStore : ObservableObject, IExperimentStore
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasExperiment))]
    private Experiment? _mSExperiment = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasAnalytes))]
    private IReadOnlyList<Analyte>? _analytes;

    [ObservableProperty]
    private List<double[,]> _analyteMatrix = new();
    
    [ObservableProperty]
    private List<string> _scanFilters = new();

    public bool HasExperiment => MSExperiment != null;
    public bool HasAnalytes => Analytes is { Count: > 0 };
}