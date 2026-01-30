using i2i_dotnet.Features.TargetedTab.Model;

namespace i2i_dotnet.Features.TargetedTab.Services;

public interface IRawFileService
{
    List<List<MSExperiment>> LoadRawFilesFromFolder(string folderPath);
}