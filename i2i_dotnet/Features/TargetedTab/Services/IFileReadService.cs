using i2i_dotnet.Features.TargetedTab.Models;

namespace i2i_dotnet.Features.TargetedTab.Services;

public interface IFileReadService
{
    (Experiment, string[]) LoadMzmlFilesFromFolder(string folderPath,  IProgress<double>? progress = null);

    
    (Experiment, string[]) LoadRawFilesFromFolder(string folderPath, IProgress<double>? progress = null);
}