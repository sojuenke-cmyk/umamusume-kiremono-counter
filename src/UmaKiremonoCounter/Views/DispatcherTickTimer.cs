using System.Windows.Threading;
using UmaKiremonoCounter.Services;

namespace UmaKiremonoCounter.Views;

/// <summary><see cref="DispatcherTimer"/> を使う <see cref="ITickTimer"/>。</summary>
public sealed class DispatcherTickTimer : ITickTimer
{
    private readonly DispatcherTimer _timer;

    public DispatcherTickTimer(TimeSpan interval)
    {
        _timer = new DispatcherTimer { Interval = interval };
        _timer.Tick += (_, e) => Tick?.Invoke(this, e);
    }

    public event EventHandler? Tick;

    public void Start() => _timer.Start();

    public void Stop() => _timer.Stop();
}
