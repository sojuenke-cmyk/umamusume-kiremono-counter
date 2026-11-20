using Microsoft.Extensions.Time.Testing;
using UmaKiremonoCounter.Services;
using UmaKiremonoCounter.ViewModels;

namespace UmaKiremonoCounter.Tests;

public class TrainerAbilityTimerViewModelTests
{
    private readonly FakeTickTimer _ticker = new();
    private readonly FakeSoundService _sound = new();
    private readonly FakeTimeProvider _time = new();
    private readonly TrainerAbilityTimerViewModel _timer;

    public TrainerAbilityTimerViewModelTests()
    {
        _timer = new TrainerAbilityTimerViewModel(_ticker, _sound, _time);
    }

    [Fact]
    public void 初期状態は12分で待機中()
    {
        Assert.Equal(TimerState.Idle, _timer.State);
        Assert.Equal("12:00", _timer.RemainingText);
        Assert.Equal("開始", _timer.StartLabel);
        Assert.False(_timer.ResetCommand.CanExecute(null));
    }

    [Fact]
    public void 開始すると残り時間が減っていく()
    {
        _timer.StartCommand.Execute(null);
        Assert.Equal(TimerState.Running, _timer.State);
        Assert.True(_ticker.IsRunning);

        Advance(TimeSpan.FromSeconds(90));
        Assert.Equal("10:30", _timer.RemainingText);

        Advance(TimeSpan.FromMilliseconds(300));
        Assert.Equal("10:30", _timer.RemainingText); // 端数は切り上げ
        Assert.Equal(0, _sound.PlayCount);
    }

    [Fact]
    public void 十二分経過で音が鳴り終了状態になる()
    {
        _timer.StartCommand.Execute(null);

        Advance(TimeSpan.FromMinutes(12) - TimeSpan.FromMilliseconds(1));
        Assert.Equal(TimerState.Running, _timer.State);
        Assert.Equal("00:01", _timer.RemainingText);
        Assert.Equal(0, _sound.PlayCount);

        Advance(TimeSpan.FromMilliseconds(1));
        Assert.Equal(TimerState.Finished, _timer.State);
        Assert.Equal("00:00", _timer.RemainingText);
        Assert.Equal(1, _sound.PlayCount);
        Assert.False(_ticker.IsRunning);

        // 終了後に Tick が来ても二重に鳴らない。
        _ticker.RaiseTick();
        Assert.Equal(1, _sound.PlayCount);
    }

    [Fact]
    public void 動作中に再開始すると12分に戻る()
    {
        _timer.StartCommand.Execute(null);
        Advance(TimeSpan.FromMinutes(5));
        Assert.Equal("再開始", _timer.StartLabel);

        _timer.StartCommand.Execute(null);
        Assert.Equal("12:00", _timer.RemainingText);

        Advance(TimeSpan.FromMinutes(11));
        Assert.Equal(TimerState.Running, _timer.State);
        Assert.Equal(0, _sound.PlayCount);
    }

    [Fact]
    public void リセットすると停止して12分に戻り音は鳴らない()
    {
        _timer.StartCommand.Execute(null);
        Advance(TimeSpan.FromMinutes(3));

        _timer.ResetCommand.Execute(null);
        Assert.Equal(TimerState.Idle, _timer.State);
        Assert.Equal("12:00", _timer.RemainingText);
        Assert.False(_ticker.IsRunning);

        Advance(TimeSpan.FromMinutes(20));
        Assert.Equal(0, _sound.PlayCount);
    }

    private void Advance(TimeSpan by)
    {
        _time.Advance(by);
        _ticker.RaiseTick();
    }

    private sealed class FakeTickTimer : ITickTimer
    {
        public event EventHandler? Tick;

        public bool IsRunning { get; private set; }

        public void Start() => IsRunning = true;

        public void Stop() => IsRunning = false;

        public void RaiseTick() => Tick?.Invoke(this, EventArgs.Empty);
    }

    private sealed class FakeSoundService : ISoundService
    {
        public int PlayCount { get; private set; }

        public void PlayAlarm() => PlayCount++;
    }
}
