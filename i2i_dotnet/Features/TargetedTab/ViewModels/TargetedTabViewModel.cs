using i2i_dotnet.Features.TargetedTab.Services;
using i2i_dotnet.Shared.Stores;

namespace i2i_dotnet.Features.TargetedTab.ViewModels;
public class TargetedTabViewModel
{
    public WorkflowPanelViewModel WorkflowPanel { get; }

    public TargetedTabViewModel(IRawFileService raw,
        IFolderDialogService dialog,
        IAnalyteFileService analyteFileService, 
        ExperimentStore experimentStore)
    {
        WorkflowPanel = new WorkflowPanelViewModel(raw, dialog, analyteFileService, experimentStore);
    }
}
