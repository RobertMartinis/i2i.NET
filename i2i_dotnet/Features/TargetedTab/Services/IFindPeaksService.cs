using ThermoFisher.CommonCore.Data.Business;

namespace i2i_dotnet.Features.TargetedTab.Services;

public interface IFindPeaksService
{
    FindPeaksResult FindPeaks(double ppm, string scanFilter);
}