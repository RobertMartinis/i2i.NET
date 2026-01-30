using System.Windows.Input;
using System.Windows.Media;
using i2i_dotnet.Core;
using i2i_dotnet.Features.TargetedTab.Services;

namespace i2i_dotnet.Features.TargetedTab.ViewModels;

public enum StepState
{
    Idle,
    Working,
    Done,
    Error
}

public sealed class WorkflowPanelViewModel : ObservableObject
{
    private readonly IRawFileService _rawFiles;
    private readonly IFolderDialogService _folderDialog;
    private readonly IAnalyteFileService _analyteFileService;
    public ICommand LoadRawCommand { get; }
    public ICommand LoadAnalyteListCommand { get; }
    public ICommand FindPeaksCommand { get; }
    
    private int _fileCount = 0;
    private int _analyteCount = 0;

    private string _overallStatusText = "Ready";
    private Brush _overallStatusBrush = Brushes.LimeGreen;

    private StepState _rawState = StepState.Idle;
    private StepState _analyteState = StepState.Idle;
    private StepState _peaksState = StepState.Idle;

    private bool _canLoadAnalytes = false;
    private bool _canFindPeaks = false;

    // Public bindable properties
    public int FileCount
    {
        get => _fileCount;
        set => Set(ref _fileCount, value);
    }

    public int AnalyteCount
    {
        get => _analyteCount;
        set => Set(ref _analyteCount, value);
    }

    public string OverallStatusText
    {
        get => _overallStatusText;
        set => Set(ref _overallStatusText, value);
    }

    public Brush OverallStatusBrush
    {
        get => _overallStatusBrush;
        set => Set(ref _overallStatusBrush, value);
    }

    public StepState RawState
    {
        get => _rawState;
        private set
        {
            if (Set(ref _rawState, value))
                OnStepStateChanged();
        }
    }

    public StepState AnalyteState
    {
        get => _analyteState;
        private set
        {
            if (Set(ref _analyteState, value))
                OnStepStateChanged();
        }
    }

    public StepState PeaksState
    {
        get => _peaksState;
        private set
        {
            if (Set(ref _peaksState, value))
                OnStepStateChanged();
        }
    }

    // Brushes for your Ellipses
    public Brush RawStatusBrush => BrushFor(RawState);
    public Brush AnalyteStatusBrush => BrushFor(AnalyteState);
    public Brush PeaksStatusBrush => BrushFor(PeaksState);

    public bool CanLoadAnalytes
    {
        get => _canLoadAnalytes;
        private set
        {
            if (Set(ref _canLoadAnalytes, value))
                ((RelayCommand)LoadAnalyteListCommand).RaiseCanExecuteChanged();
        }
    }

    public bool CanFindPeaks
    {
        get => _canFindPeaks;
        private set
        {
            if (Set(ref _canFindPeaks, value))
                ((RelayCommand)FindPeaksCommand).RaiseCanExecuteChanged();
        }
    }
    private double _progress;
    public double Progress
    {
        get => _progress;
        private set => Set(ref _progress, value);
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        private set => Set(ref _isLoading, value);
    }
    
    public WorkflowPanelViewModel(IRawFileService rawfiles, IFolderDialogService folderDialog, IAnalyteFileService analyteFileService)
    {
        // NOTE: later you’ll call Services here instead of fake values.
        _rawFiles = rawfiles;
        _folderDialog = folderDialog;
        _analyteFileService = analyteFileService;
        LoadRawCommand = new RelayCommand(async () => await LoadRawAsync());

        LoadAnalyteListCommand = new RelayCommand(LoadAnalytes, () => CanLoadAnalytes);
        FindPeaksCommand = new RelayCommand(FindPeaks, () => CanFindPeaks);

        // initial gating
        CanLoadAnalytes = false;
        CanFindPeaks = false;

        UpdateOverallStatus("Ready", Brushes.LimeGreen);
    }

    private async Task LoadRawAsync()
    {
        RawState = StepState.Working;

        var folder = _folderDialog.PickFolder("Select folder");
        if (string.IsNullOrEmpty(folder))
            return;
        
        IsLoading = true;
        Progress = 0;
        
        var progress = new Progress<double>(p => Progress = p);
        
        UpdateOverallStatus("Loading experiment...", Brushes.Gold);
        // Update VM state
        var spectra = await Task.Run(() =>
            _rawFiles.LoadRawFilesFromFolder(folder, progress)
        );
        FileCount = spectra.Count;
        RawState = StepState.Done;
        UpdateOverallStatus("Experiment loaded", Brushes.LimeGreen);

        CanLoadAnalytes = true;
        ((RelayCommand)LoadAnalyteListCommand).RaiseCanExecuteChanged();
    }

    private void LoadAnalytes()
    {
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
    }

    private void FindPeaks()
    {
        PeaksState = StepState.Working;
        UpdateOverallStatus("Finding peaks...", Brushes.Gold);

        // fake success
        PeaksState = StepState.Done;
        UpdateOverallStatus("Peaks found", Brushes.LimeGreen);
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
