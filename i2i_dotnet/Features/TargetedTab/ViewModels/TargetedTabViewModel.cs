using i2i_dotnet.Features.TargetedTab.Services;
using i2i_dotnet.Shared.Stores;
using MahApps.Metro.Controls.Dialogs;
using ScottPlot;

namespace i2i_dotnet.Features.TargetedTab.ViewModels;
public class TargetedTabViewModel
{
    public WorkflowPanelViewModel WorkflowPanel { get; }
    public PlotViewModel PlotViewPanel { get; }

    public TargetedTabViewModel(IFileReadService fileService,
        IFolderDialogService dialog,
        IAnalyteFileService analyteFileService,
        IFindPeaksService findPeaksService,
        ExperimentStore experimentStore,
        IDialogCoordinator dialogCoordinator)
    {
        WorkflowPanel = new WorkflowPanelViewModel(fileService, dialog, analyteFileService, findPeaksService, experimentStore, dialogCoordinator);
        PlotViewPanel = new PlotViewModel(experimentStore);
    }
}
