namespace i2i_dotnet.Features.TargetedTab.Services;

public interface IFolderDialogService
{
    /// <summary>
    /// Shows a folder picker dialog and returns the selected folder,
    /// or null if the user cancelled.
    /// </summary>
    string? PickFolder(string title);
}