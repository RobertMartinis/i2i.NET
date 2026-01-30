using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace i2i_dotnet.Core;

/// <summary>
/// Base class for ViewModels (and sometimes bindable models).
/// Implements INotifyPropertyChanged so WPF updates the UI when properties change.
/// </summary>
public abstract class ObservableObject : INotifyPropertyChanged
{
    /// <summary>
    /// WPF listens to this event. When we raise it, any bindings to that property refresh.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Helper method for property setters.
    ///
    /// - Compares the new value to the old value.
    /// - If it changed: store it + raise PropertyChanged automatically.
    ///
    /// The [CallerMemberName] attribute means you don't have to type the property name string.
    /// The compiler fills it in for you (safer than "magic strings").
    /// </summary>
    protected bool Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        // If no change, do nothing. This avoids unnecessary UI redraws.
        if (Equals(field, value))
            return false;

        // Update the backing field
        field = value;

        // Notify WPF: "the property called 'name' changed"
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        return true;
    }
}