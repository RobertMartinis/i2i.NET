using System.Windows.Input;
using Microsoft.Win32;
using i2i_dotnet.Helpers;
using i2i_dotnet.Services;
using System;
using System.IO;
using System.Windows.Forms;

namespace i2i_dotnet.ViewModels
{
    public class MainViewModel
    {
        private readonly ThermoRawFileService _rawService;

        public ICommand LoadRawCommand { get; }

        public MainViewModel()
        {
            _rawService = new ThermoRawFileService();
            LoadRawCommand = new RelayCommand(LoadRawFilesFromFolder);
        }

        private void LoadRawFilesFromFolder()
        {
            using var dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string folderPath = dialog.SelectedPath;

                var spectraGroups = _rawService.LoadRawFilesFromFolder(folderPath);

               
               System.Diagnostics.Debug.WriteLine("Finished");
             
            }
        }

    }
}
