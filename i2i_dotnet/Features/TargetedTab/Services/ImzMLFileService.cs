using i2i_dotnet.Features.TargetedTab.Models;

namespace i2i_dotnet.Features.TargetedTab.Services;

public interface ImzMLFileService
{
   Experiment LoadMzmlFilesFromFolder(string folderPath, IProgress<double>? progress = null);
}