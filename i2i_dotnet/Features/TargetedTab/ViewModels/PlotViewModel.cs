using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using i2i_dotnet.Shared.Stores;
using ScottPlot.WPF;

namespace i2i_dotnet.Features.TargetedTab.ViewModels;

public sealed partial class PlotViewModel : ObservableObject, IDisposable
{
    public WpfPlot PlotControl { get; } = new();

    private readonly ExperimentStore _experimentStore;

    [ObservableProperty]
    private int _selectedIndex;

    [ObservableProperty] private string _selectedAnalyte;

    public PlotViewModel(ExperimentStore experimentStore)
    {
        _experimentStore = experimentStore;

        _experimentStore.PropertyChanged += ExperimentStoreOnPropertyChanged;

        ReplotFromStore();
    }

    private void ExperimentStoreOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ExperimentStore.AnalyteMatrix))
        {
            // If FindPeaks sets AnalyteMatrix, we replot.
            ReplotFromStore();
        }
    }

    private void ReplotFromStore()
    {
        var matrices = _experimentStore.AnalyteMatrix;
        if (matrices == null || matrices.Count == 0)
            return;

        var idx = Math.Clamp(SelectedIndex, 0, matrices.Count - 1);
        if (idx != SelectedIndex)
            SelectedIndex = idx;

        // Ensure plotting happens on the UI thread (ScottPlot/WPF control)
        if (PlotControl.Dispatcher.CheckAccess())
        {
            PlotMz(matrices[idx]);
        }
        else
        {
            PlotControl.Dispatcher.Invoke(() => PlotMz(matrices[idx]));
        }
    }

    public void PlotMz(double[,] analyteMatrix)
    {
        var plt = PlotControl.Plot;
        plt.Clear();
        plt.Add.Heatmap(analyteMatrix);
        plt.Legend.IsVisible = false;
        plt.HideAxesAndGrid();
        plt.Axes.Margins(0, 0);
        plt.Axes.AutoScale();
        PlotControl.Refresh();
    }

    partial void OnSelectedIndexChanged(int value)
    {
        var matrices = _experimentStore.AnalyteMatrix;
        if (matrices == null || matrices.Count == 0)
            return;

        if (value < 0 || value >= matrices.Count)
            return;

        if (PlotControl.Dispatcher.CheckAccess())
            PlotMz(matrices[value]);
        else
            PlotControl.Dispatcher.Invoke(() => PlotMz(matrices[value]));
    }

    public void Dispose()
    {
        _experimentStore.PropertyChanged -= ExperimentStoreOnPropertyChanged;
    }
}
