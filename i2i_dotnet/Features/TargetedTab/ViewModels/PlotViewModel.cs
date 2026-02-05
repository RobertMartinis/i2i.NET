using System.ComponentModel;
using i2i_dotnet.Shared.Stores;
using ScottPlot;

namespace i2i_dotnet.Features.TargetedTab.ViewModels;

using ScottPlot.WPF;

public class PlotViewModel : INotifyPropertyChanged
{
    public WpfPlot PlotControl { get; } = new WpfPlot();
    private ExperimentStore _experimentstore;
    
    private string? _selectedAnalyte;
    public string? SelectedAnalyte
    {
        get => _selectedAnalyte;
        set
        {
            if (_selectedAnalyte == value) return;
            _selectedAnalyte = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedAnalyte)));
            PlotMz();
        }
    }
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
       
        plt.HideGrid(); 

        PlotControl.Refresh();
    }
    
    public void PlotMz()
    {
        int rows = 50;
        int cols = 80;

        var rng = new Random();

        double[,] values = new double[rows, cols];
        for (int r = 0; r < rows; r++)
        for (int c = 0; c < cols; c++)
            values[r, c] = rng.NextDouble(); // 0..1

        var plt = PlotControl.Plot;
        plt.Clear();
        plt.Add.Heatmap(values);
        PlotControl.Refresh();
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    
}