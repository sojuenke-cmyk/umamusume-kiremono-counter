using System.Windows;
using UmaKiremonoCounter.Services;
using UmaKiremonoCounter.ViewModels;
using UmaKiremonoCounter.Views;

namespace UmaKiremonoCounter;

/// <summary>アプリの起点。View と ViewModel の組み立てはここだけで行う。</summary>
public partial class App : Application
{
    private MainViewModel? _mainViewModel;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _mainViewModel = new MainViewModel(new DataStore(), new DialogService(), new ScreenService());
        MainWindow = new MainWindow { DataContext = _mainViewModel };
        MainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        // ウィンドウ位置などを保存する（記録そのものは操作のたびに保存済み）。
        _mainViewModel?.Save();
        base.OnExit(e);
    }
}
