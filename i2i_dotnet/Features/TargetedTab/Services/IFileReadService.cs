using i2i_dotnet.Features.TargetedTab.Models;

namespace i2i_dotnet.Features.TargetedTab.Services;

public interface IFileReadService
{
    Experiment LoadMzmlFilesFromFolder(string folderPath,  IProgress<double>? progress = null);

    
    (Experiment, HashSet<string>) LoadRawFilesFromFolder(string folderPath, IProgress<double>? progress = null);
}