using System.Windows.Input;
using Microsoft.Win32;
using i2i_dotnet.Features.TargetedTab.Services;
using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.ComponentModel;
using i2i_dotnet.Core;
using i2i_dotnet.Features.TargetedTab.ViewModels;


namespace i2i_dotnet.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {

        public TargetedTabViewModel TargetedTab { get; }
      
        public MainViewModel()
        {
            var rawService = new ThermoRawFileService();
            var folderDialog = new MahAppsFolderDialogService();
            var analyteFileService = new AnalyteFileService();

            TargetedTab = new TargetedTabViewModel(rawService, folderDialog, analyteFileService);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
