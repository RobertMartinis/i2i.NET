using i2i_dotnet.Features.TargetedTab.Models;

namespace i2i_dotnet.Features.TargetedTab.Services;

public interface IFileService
{
    Experiment LoadRawFilesFromFolder(string folderPath,
        IProgress<(int done, int total, string msg)>? progress = null,
        CancellationToken ct = default);

    IReadOnlyList<Analyte> LoadAnalytes(string xlsxPath,
        CancellationToken ct = default);
}