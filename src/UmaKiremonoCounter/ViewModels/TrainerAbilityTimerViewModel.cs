using UmaKiremonoCounter.Services;

namespace UmaKiremonoCounter.ViewModels;

public enum TimerState
{
    Idle,
    Running,
    Finished,
}

/// <summary>トレーナーアビリティの12分タイマー。</summary>
public sealed class TrainerAbilityTimerViewModel : ObservableObject
{
    public static readonly TimeSpan Duration = TimeSpan.FromMinutes(12);

    private readonly ITickTimer _ticker;
    private readonly ISoundService _sound;
    private readonly TimeProvider _time;
    private DateTimeOffset _endsAt;
    private TimeSpan _remaining = Duration;
    private TimerState _state = TimerState.Idle;

    public TrainerAbilityTimerViewModel(ITickTimer ticker, ISoundService sound, TimeProvider time)
    {
        _ticker = ticker;
        _sound = sound;
        _time = time;
        _ticker.Tick += (_, _) => Update();

        StartCommand = new RelayCommand(Start);
        ResetCommand = new RelayCommand(Reset, () => State != TimerState.Idle);
    }

    /// <summary>開始（動作中・終了後に押すと12分から再スタート）。</summary>
    public RelayCommand StartCommand { get; }

    /// <summary>停止して12分に戻す。</summary>
    public RelayCommand ResetCommand { get; }

    public TimerState State
    {
        get => _state;
        private set
        {
            if (SetProperty(ref _state, value))
            {
                OnPropertyChanged(nameof(StartLabel));
                ResetCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string RemainingText
    {
        get
        {
            // 残り 0.3 秒なら「00:01」と表示されるよう切り上げる。
            var seconds = (int)Math.Ceiling(_remaining.TotalSeconds);
            return $"{seconds / 60:00}:{seconds % 60:00}";
        }
    }

    public string StartLabel => State == TimerState.Idle ? "開始" : "再開始";

    private void Start()
    {
        _endsAt = _time.GetUtcNow() + Duration;
        SetRemaining(Duration);
        State = TimerState.Running;
        _ticker.Start();
    }

    private void Reset()
    {
        _ticker.Stop();
        SetRemaining(Duration);
        State = TimerState.Idle;
    }

    private void Update()
    {
        if (State != TimerState.Running)
        {
            return;
        }

        var remaining = _endsAt - _time.GetUtcNow();
        if (remaining > TimeSpan.Zero)
        {
            SetRemaining(remaining);
            return;
        }

        _ticker.Stop();
        SetRemaining(TimeSpan.Zero);
        State = TimerState.Finished;
        _sound.PlayAlarm();
    }

    private void SetRemaining(TimeSpan value)
    {
        _remaining = value;
        OnPropertyChanged(nameof(RemainingText));
    }
}
