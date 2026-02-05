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
            
            var rawService = new ThermoRawFileService();
            var folderDialog = new MahAppsFolderDialogService();
            var analyteFileService = new AnalyteFileService();
            var findPeaksService = new FindPeaksService(experimentStore);

            TargetedTab = new TargetedTabViewModel(rawService,
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
