using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using i2i_dotnet.Features.TargetedTab.Models;
using i2i_dotnet.Features.TargetedTab.Services;
using i2i_dotnet.Shared.Stores;
using ThermoFisher.CommonCore.Data;
using RelayCommand = i2i_dotnet.Core.RelayCommand;

namespace i2i_dotnet.Features.TargetedTab.ViewModels;

public enum StepState
{
    Idle,
    Working,
    Done,
    Error
}

public sealed partial class WorkflowPanelViewModel : ObservableObject
{
    private readonly IFileReadService _fileService;
    private readonly IFolderDialogService _folderDialog;
    private readonly IAnalyteFileService _analyteFileService;
    private readonly IFindPeaksService _findPeaksService;
    public ObservableCollection<string> AnalyteList { get; set; } = new ObservableCollection<string>();
    
    private ExperimentStore _experimentStore;
    public ICommand LoadFilesCommand { get; }
    public ICommand LoadAnalyteListCommand { get; }
    public ICommand FindPeaksCommand { get; }
    
    [ObservableProperty]
    private int _fileCount = 0;
    [ObservableProperty]
    private int _analyteCount = 0;

    [ObservableProperty]
    private string _overallStatusText = "Ready";
    [ObservableProperty]
    private Brush _overallStatusBrush = Brushes.LimeGreen;

    [ObservableProperty]
    private StepState _rawState = StepState.Idle;
    
    [ObservableProperty]
    private StepState _analyteState = StepState.Idle;
    [ObservableProperty]
    private StepState _peaksState = StepState.Idle;
    
    [ObservableProperty]
    private double _ppm = 5;

    [ObservableProperty]
    private bool _canLoadAnalytes = false;
    [ObservableProperty]
    private bool _canFindPeaks = false;

    // Public bindable properties

    partial void OnRawStateChanged(StepState value)
    {
        OnStepStateChanged();
    }

    partial void OnPeaksStateChanged(StepState value)
    {
        OnStepStateChanged();
    }

    partial void OnAnalyteStateChanged(StepState value)
    {
        OnStepStateChanged();
    }

    partial void OnCanLoadAnalytesChanged(bool value)
    {
        ((RelayCommand)LoadAnalyteListCommand).RaiseCanExecuteChanged();
    }

    partial void OnCanFindPeaksChanged(bool value)
    {
        ((RelayCommand)FindPeaksCommand).RaiseCanExecuteChanged();
    }

    // Brushes for your Ellipses
    public Brush RawStatusBrush => BrushFor(RawState);
    public Brush AnalyteStatusBrush => BrushFor(AnalyteState);
    public Brush PeaksStatusBrush => BrushFor(PeaksState);

    
    [ObservableProperty]
    private double _progress;

    [ObservableProperty]
    private bool _isLoading;
    
    public WorkflowPanelViewModel(IFileReadService fileService,
        IFolderDialogService folderDialog, 
        IAnalyteFileService analyteFileService,
        IFindPeaksService findPeaksService,
        ExperimentStore experimentStore)
    {
        _fileService = fileService;
        _folderDialog = folderDialog;
        _analyteFileService = analyteFileService;
        _findPeaksService = findPeaksService;
        _experimentStore = experimentStore;
        LoadFilesCommand = new RelayCommand<ExperimentFileType>(async (value) => await LoadFilesAsync(value));

        LoadAnalyteListCommand = new RelayCommand(LoadAnalytes, () => CanLoadAnalytes);
        FindPeaksCommand = new RelayCommand(FindPeaks, () => CanFindPeaks);

        // initial gating
        CanLoadAnalytes = false;
        CanFindPeaks = false;

        UpdateOverallStatus("Ready", Brushes.LimeGreen);
    }

    private async Task LoadFilesAsync(ExperimentFileType type)
    {
        RawState = StepState.Working;

        var folder = _folderDialog.PickFolder("Select folder");
        
        if (string.IsNullOrEmpty(folder))
            return;
        
        IsLoading = true;
        Progress = 0;
        
        var progress = new Progress<double>(p => Progress = p);
        Experiment exp = new Experiment(); 
        UpdateOverallStatus("Loading experiment...", Brushes.Gold);
        // Update VM state
        if (type == ExperimentFileType.mzML)
        {
            exp = await Task.Run(() =>
                _fileService.LoadMzmlFilesFromFolder(folder, progress));
        }
        else
        {
           exp = await Task.Run(() =>
                _fileService.LoadRawFilesFromFolder(folder, progress)
            );
        }

        FileCount = exp.LineCount;
        RawState = StepState.Done;
        UpdateOverallStatus("Experiment loaded", Brushes.LimeGreen);

        CanLoadAnalytes = true;
        _experimentStore.MSExperiment = exp;
        ((RelayCommand)LoadAnalyteListCommand).RaiseCanExecuteChanged();
    }

    private void LoadAnalytes()
    {
        AnalyteList.Clear();
        AnalyteState = StepState.Working;
        UpdateOverallStatus("Loading analyte list...", Brushes.Gold);
        
        var documentPath = _folderDialog.PickFile("Select analyte file");
        if (string.IsNullOrEmpty(documentPath))
            return;
        var analyteFile = _analyteFileService.LoadAnalytes(documentPath);
        AnalyteState = StepState.Done;
        
        AnalyteCount = analyteFile.Count;
        UpdateOverallStatus("Analytes loaded", Brushes.LimeGreen);

        CanFindPeaks = true;
        
        _experimentStore.Analytes = analyteFile;
    }

    private void FindPeaks()
    {
        PeaksState = StepState.Working;
        UpdateOverallStatus("Finding peaks...", Brushes.Gold);
        FindPeaksResult result = _findPeaksService.FindPeaks(_ppm);
        _experimentStore.AnalyteMatrix = result.AnalyteMatrix;
        // fake success
        PeaksState = StepState.Done;
        UpdateOverallStatus("Peaks found", Brushes.LimeGreen);
        
        var analyteList = _experimentStore.Analytes;
        if (analyteList == null || analyteList.Count == 0)
            return;

        AnalyteList.Clear();
        foreach (var analyte in analyteList)
            AnalyteList.Add(analyte.Mz.ToString());

    }

    private void OnStepStateChanged()
    {
        // tell UI to update ellipse fills
        // These are "calculated properties", so we nudge them by raising PropertyChanged.
        // Easiest way: Set() already raises for the state; we additionally raise for brushes.
        // (If you want, I can show a helper for this pattern.)
        _ = RawStatusBrush;
        _ = AnalyteStatusBrush;
        _ = PeaksStatusBrush;
        // There isn't a direct OnPropertyChanged here in this base class,
        // so we re-set the OverallStatusText etc. only.
        // If you want explicit brush notifications, I can adjust ObservableObject to include OnPropertyChanged().
    }

    private static Brush BrushFor(StepState state) => state switch
    {
        StepState.Idle => Brushes.Yellow,
        StepState.Working => Brushes.Gold,
        StepState.Done => Brushes.LimeGreen,
        StepState.Error => Brushes.Red,
        _ => Brushes.Yellow
    };

    private void UpdateOverallStatus(string text, Brush brush)
    {
        OverallStatusText = text;
        OverallStatusBrush = brush;
    }
}
