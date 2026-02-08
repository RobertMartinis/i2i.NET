using i2i_dotnet.Features.TargetedTab.Models;

namespace i2i_dotnet.Features.TargetedTab.Services;

public class FileReadService : IFileReadService
{
    private readonly IRawFileService _raw;
    private readonly ImzMLFileService _mzml;
    public FileReadService(IRawFileService raw, ImzMLFileService mzml)
    {
        _raw = raw;
        _mzml = mzml;
    } 
    
    public (Experiment, string[]) LoadRawFilesFromFolder(string folder, IProgress<double>? progress = null)
        => _raw.LoadRawFilesFromFolder(folder, progress);

    public (Experiment, string[]) LoadMzmlFilesFromFolder(string folder, IProgress<double>? progress = null)
        => _mzml.LoadMzmlFilesFromFolder(folder, progress);
}