using System.Windows.Controls;
using i2i_dotnet.Features.TargetedTab.ViewModels;
using ScottPlot;

namespace i2i_dotnet.Features.TargetedTab.Views;

public partial class PlotView : Page
{
    public PlotView()
    {
        InitializeComponent();
        DataContext = new PlotViewModel();
    }
}