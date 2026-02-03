using System.ComponentModel;
using i2i_dotnet.Features.TargetedTab.Models;

namespace i2i_dotnet.Shared.Stores;

public interface IExperimentStore : INotifyPropertyChanged
{
    Experiment? MSExperiment { get; set; }
    IReadOnlyList<Analyte>? Analytes { get; set; }
    
    bool HasExperiment { get; }
    bool HasAnalytes { get; }
    double[,]? AnalyteMatrix {get; set;}
   
}