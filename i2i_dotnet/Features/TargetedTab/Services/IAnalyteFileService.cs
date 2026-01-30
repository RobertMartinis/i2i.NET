using i2i_dotnet.Features.TargetedTab.Model;

namespace i2i_dotnet.Features.TargetedTab.Services;

public interface IAnalyteFileService
{
    IReadOnlyList<Analyte> LoadAnalytes(string path);
}