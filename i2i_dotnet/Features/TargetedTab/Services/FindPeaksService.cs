using i2i_dotnet.Features.TargetedTab.Models;
using i2i_dotnet.Shared.Stores;
using ThermoFisher.CommonCore.Data.Business;

namespace i2i_dotnet.Features.TargetedTab.Services;



public class FindPeaksService

{
    private IExperimentStore _store;
    
    public FindPeaksService(IExperimentStore store)
    {
        _store = store;
    }
    
    
}