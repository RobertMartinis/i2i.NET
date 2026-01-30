using Microsoft.Win32;

namespace i2i_dotnet.Features.TargetedTab.Services;

public class MahAppsFolderDialogService : IFolderDialogService
{
    public string? PickFolder(string title)
    {
        var dlg = new OpenFolderDialog
        {
            Title = title
        };
        
        return dlg.ShowDialog() == true ? dlg.FolderName : null;
    }

    public string? PickFile(string title)
    {
        var dlg = new OpenFileDialog
        {
            Title = title
        };
            
        return dlg.ShowDialog() == true? dlg.FileName : null;
    }
}