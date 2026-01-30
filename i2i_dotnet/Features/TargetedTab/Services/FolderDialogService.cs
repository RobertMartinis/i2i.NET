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

        // ShowDialog returns bool?
        return dlg.ShowDialog() == true ? dlg.FolderName : null;
    }
}