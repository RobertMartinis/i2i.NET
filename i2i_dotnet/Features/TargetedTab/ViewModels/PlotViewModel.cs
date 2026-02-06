using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using i2i_dotnet.Shared.Stores;
using ScottPlot;

namespace i2i_dotnet.Features.TargetedTab.ViewModels;

using ScottPlot.WPF;

public sealed partial class PlotViewModel : ObservableObject
{
    public WpfPlot PlotControl { get; } = new WpfPlot();
    private ExperimentStore _experimentstore;
    
    [ObservableProperty]
    private string? selectedAnalyte;
    [ObservableProperty]
    private int selectedIndex;
    
    // TODO: Add peak data instead of test data. Switch to heatmap.
    public PlotViewModel(ExperimentStore experimentStore)
    {
        _experimentstore = experimentStore;
        double[,] values =
        {
            { 1,  2,  3,  4,  5 },
            { 6,  7,  8,  9, 10 },
            { 11, 12, 13, 14, 15 },
        };

        var plt = PlotControl.Plot;

        plt.Add.Heatmap(values);
        plt.Legend.IsVisible = false;
        plt.HideAxesAndGrid();
        plt.Axes.SetLimits();
        plt.Axes.Margins(0,0);
        
       
        plt.HideGrid(); 

        PlotControl.Refresh();
    }
    
    public void PlotMz(double [,] analyteMatrix)
    {
        /*
        int rows = 50;
        int cols = 80;

        var rng = new Random();

        double[,] values = new double[rows, cols];
        for (int r = 0; r < rows; r++)
        for (int c = 0; c < cols; c++)
            values[r, c] = rng.NextDouble(); // 0..1
            */

        var plt = PlotControl.Plot;
        plt.Clear();
        plt.Add.Heatmap(analyteMatrix);
        PlotControl.Refresh();
    }


   partial void OnSelectedIndexChanged(int value)
{
    System.Diagnostics.Debug.WriteLine($"SelectedAnalyte: {value}");
    List<double[,]> analytem = _experimentstore.AnalyteMatrix;
    
    PlotMz(analytem[value]);
    
}

    
}