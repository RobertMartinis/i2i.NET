using i2i_dotnet.Features.TargetedTab.Services;
using i2i_dotnet.Shared.Stores;
using System.ComponentModel;
using i2i_dotnet.Features.TargetedTab.ViewModels;


namespace i2i_dotnet.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {

        public TargetedTabViewModel TargetedTab { get; }
      
        public MainViewModel()
        {
            var experimentStore = new ExperimentStore();
            var mzmlFileService = new MzMLFileService();
            var rawFileService = new ThermoRawFileService();
            
            var fileReadService = new FileReadService(rawFileService, mzmlFileService);
            var folderDialog = new MahAppsFolderDialogService();
            var analyteFileService = new AnalyteFileService();
            var findPeaksService = new FindPeaksService(experimentStore);

            TargetedTab = new TargetedTabViewModel(fileReadService,
                folderDialog,
                analyteFileService,
                findPeaksService,
                experimentStore);

        }
        
        

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
