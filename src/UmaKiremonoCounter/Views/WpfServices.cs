using System.Windows;
using UmaKiremonoCounter.Services;

namespace UmaKiremonoCounter.Views;

/// <summary>WPF の MessageBox を使う <see cref="IDialogService"/>。</summary>
public sealed class DialogService : IDialogService
{
    public bool Confirm(string message)
    {
        var owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive)
                    ?? Application.Current.MainWindow;
        const string caption = "ウマ娘 切れ者カウンター";
        var result = owner is null
            ? MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning)
            : MessageBox.Show(owner, message, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning);
        return result == MessageBoxResult.Yes;
    }
}

/// <summary>WPF の SystemParameters を使う <see cref="IScreenService"/>。</summary>
public sealed class ScreenService : IScreenService
{
    public ScreenBounds VirtualScreen => new(
        SystemParameters.VirtualScreenLeft,
        SystemParameters.VirtualScreenTop,
        SystemParameters.VirtualScreenWidth,
        SystemParameters.VirtualScreenHeight);
}
