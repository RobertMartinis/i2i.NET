using System.Windows.Input;

namespace i2i_dotnet.Core;

/// <summary>
/// A simple ICommand implementation.
/// Lets you bind Buttons/MenuItems/KeyBindings to ViewModel methods without code-behind.
///
/// WPF calls:
/// - CanExecute() to decide if the button should be enabled
/// - Execute() when the user clicks
/// </summary>
public sealed class RelayCommand : ICommand
{
    // What should happen when the command runs
    private readonly Action _execute;

    // Optional: whether the command is allowed right now (controls enabled/disabled)
    private readonly Func<bool>? _canExecute;

    /// <summary>
    /// Create a command that runs execute().
    /// Optionally provide canExecute() to enable/disable the command.
    /// </summary>
    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    /// <summary>
    /// WPF subscribes to this event.
    /// When it fires, WPF re-asks CanExecute() and updates button enabled/disabled state.
    /// </summary>
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Called by WPF to ask: "Should this command be enabled?"
    /// If no _canExecute was provided, we default to true (enabled).
    /// </summary>
    public bool CanExecute(object? parameter)
        => _canExecute?.Invoke() ?? true;

    /// <summary>
    /// Called by WPF when the user triggers the command (clicks a button etc.)
    /// </summary>
    public void Execute(object? parameter)
        => _execute();

    /// <summary>
    /// Call this when something changed that affects CanExecute().
    /// Example: after loading RAW data, you want "Load analyte list" to become enabled.
    /// </summary>
    public void RaiseCanExecuteChanged()
        => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}