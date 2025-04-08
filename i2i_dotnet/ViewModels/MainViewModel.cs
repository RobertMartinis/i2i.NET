using System.Windows.Input;
using Microsoft.Win32;
using i2i_dotnet.Helpers;
using i2i_dotnet.Services;
using System;
using System.IO;

namespace i2i_dotnet.ViewModels
{
    public class MainViewModel
    {
        private readonly ThermoRawFileService _rawService;

        public ICommand LoadRawCommand { get; }

        public MainViewModel()
        {
            _rawService = new ThermoRawFileService();
            LoadRawCommand = new RelayCommand(LoadRawFile);
        }

        private void LoadRawFile()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Thermo RAW files (*.raw)|*.raw",
                Multiselect = false
            };
            if (dialog.ShowDialog() == true)
            {
                string filePath = dialog.FileName;
                System.Diagnostics.Debug.WriteLine(File.Exists(filePath));
                try
                {
                    _rawService.LoadFileToMSSpectra(filePath);

                    // Optional: Notify UI or bind to Spectra result
                    var first = _rawService.Spectra.FirstOrDefault();
                    if (first != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Loaded scan #{first.ScanNumber} with {first.MzCount} peaks.");
                    }

                    else {
                        System.Diagnostics.Debug.WriteLine("Error");
                            }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading RAW file: {ex.Message}");
                }
            }
        }
    }
}
