using i2i_dotnet.Features.TargetedTab.Services;
namespace i2i_dotnet.Features.TargetedTab.ViewModels;
public class TargetedTabViewModel
{
    public WorkflowPanelViewModel WorkflowPanel { get; }

    public TargetedTabViewModel(IRawFileService raw, IFolderDialogService dialog, IAnalyteFileService analyteFileService)
    {
        WorkflowPanel = new WorkflowPanelViewModel(raw, dialog, analyteFileService);
    }
}
