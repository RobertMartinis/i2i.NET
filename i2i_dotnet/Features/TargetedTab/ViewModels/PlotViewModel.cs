namespace i2i_dotnet.Features.TargetedTab.ViewModels;

using ScottPlot.WPF;

public class PlotViewModel
{
    public WpfPlot PlotControl { get; } = new WpfPlot();

    // TODO: Add peak data instead of test data. Switch to heatmap.
    public PlotViewModel()
    {
        double[] dataX = { 1, 2, 3, 4, 5 };
        double[] dataY = { 1, 4, 9, 16, 25 };

        var plt = PlotControl.Plot;

        plt.Add.Scatter(dataX, dataY);
        plt.Legend.IsVisible = false;
        plt.HideAxesAndGrid();
       
        plt.HideGrid(); 

        PlotControl.Refresh();
    }
    
 

}