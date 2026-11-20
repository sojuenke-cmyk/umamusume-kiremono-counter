using System.Collections.ObjectModel;
using UmaKiremonoCounter.Models;
using UmaKiremonoCounter.Services;

namespace UmaKiremonoCounter.ViewModels;

/// <summary>統計表の1行。該当しない項目は "-"。</summary>
public sealed record StatsRow(string Title, string Occurrences, string Successes, string RateText, string MissStreak, bool IsTotal = false);

public sealed record LogEntry(string Text);

public sealed class MainViewModel : ObservableObject
{
    private const int MaxLogEntries = 200;

    /// <summary>未取得連続・合計の対象になる切れ者イベント。</summary>
    private static readonly EventKind[] StreakKinds = [EventKind.SignatureEvent, EventKind.CommonEvent, EventKind.RaceEvent];

    private readonly IDataStore _store;
    private readonly IDialogService _dialog;
    private readonly AppData _data;
    private int _currentCareer;
    private string _undoDescription = string.Empty;
    private double _windowLeft = double.NaN;
    private double _windowTop = double.NaN;

    public MainViewModel(IDataStore store, IDialogService dialog, IScreenService screen, TrainerAbilityTimerViewModel trainerAbilityTimer)
    {
        TrainerAbilityTimer = trainerAbilityTimer;
        _store = store;
        _dialog = dialog;
        _data = store.Load();
        RestoreWindowPosition(screen.VirtualScreen);

        Signature = new CategoryViewModel("勝負服 / 固有 切れ者イベント", true, s => Record(EventKind.SignatureEvent, s), CanRecord);
        Common = new CategoryViewModel("共通 切れ者イベント", true, s => Record(EventKind.CommonEvent, s), CanRecord);
        Race = new CategoryViewModel("レース後 切れ者イベント", true, s => Record(EventKind.RaceEvent, s), CanRecord);
        Romance = new CategoryViewModel("恋愛相談", true, s => Record(EventKind.RomanceConsultation, s), CanRecord);
        Awakening = new CategoryViewModel("覚醒Lv7 切れ者イベント", false, s => Record(EventKind.AwakeningLv7, s), CanRecord);

        StartCareerCommand = new RelayCommand(StartCareer);
        UndoCommand = new RelayCommand(Undo, () => _data.Records.Count > 0);
        ResetCommand = new RelayCommand(Reset, () => _data.Records.Count > 0);

        Refresh();
    }

    public TrainerAbilityTimerViewModel TrainerAbilityTimer { get; }

    public CategoryViewModel Signature { get; }

    public CategoryViewModel Common { get; }

    public CategoryViewModel Race { get; }

    public CategoryViewModel Romance { get; }

    public CategoryViewModel Awakening { get; }

    public ObservableCollection<StatsRow> Stats { get; } = [];

    public ObservableCollection<LogEntry> Log { get; } = [];

    public RelayCommand StartCareerCommand { get; }

    public RelayCommand UndoCommand { get; }

    public RelayCommand ResetCommand { get; }

    /// <summary>ウィンドウの左端位置（NaN なら OS 既定の位置）。</summary>
    public double WindowLeft
    {
        get => _windowLeft;
        set
        {
            if (SetProperty(ref _windowLeft, value) && IsValidPosition(value))
            {
                _data.Window.Left = value;
            }
        }
    }

    /// <summary>ウィンドウの上端位置（NaN なら OS 既定の位置）。</summary>
    public double WindowTop
    {
        get => _windowTop;
        set
        {
            if (SetProperty(ref _windowTop, value) && IsValidPosition(value))
            {
                _data.Window.Top = value;
            }
        }
    }

    public int CurrentCareer
    {
        get => _currentCareer;
        private set => SetProperty(ref _currentCareer, value);
    }

    public string UndoDescription
    {
        get => _undoDescription;
        private set => SetProperty(ref _undoDescription, value);
    }

    public bool Topmost
    {
        get => _data.Window.Topmost;
        set
        {
            if (_data.Window.Topmost == value)
            {
                return;
            }

            _data.Window.Topmost = value;
            OnPropertyChanged();
            Save();
        }
    }

    public void Save() => _store.Save(_data);

    private bool CanRecord() => CurrentCareer > 0;

    private void StartCareer() => Add(new CounterRecord(DateTime.Now, EventKind.CareerStart, false, CurrentCareer + 1));

    private void Record(EventKind kind, bool success) => Add(new CounterRecord(DateTime.Now, kind, success, CurrentCareer));

    private void Add(CounterRecord record)
    {
        _data.Records.Add(record);
        Save();
        Refresh();
    }

    private void Undo()
    {
        if (_data.Records.Count == 0)
        {
            return;
        }

        _data.Records.RemoveAt(_data.Records.Count - 1);
        Save();
        Refresh();
    }

    private void Reset()
    {
        if (!_dialog.Confirm("すべての記録を削除します。元に戻せません。よろしいですか？"))
        {
            return;
        }

        _data.Records.Clear();
        Save();
        Refresh();
    }

    private void Refresh()
    {
        var records = _data.Records;
        CurrentCareer = records.Count(r => r.Kind == EventKind.CareerStart);

        Stats.Clear();
        Stats.Add(ToRow("勝負服/固有", Statistics.Compute(records, EventKind.SignatureEvent)));
        Stats.Add(ToRow("共通", Statistics.Compute(records, EventKind.CommonEvent)));
        Stats.Add(ToRow("レース後", Statistics.Compute(records, EventKind.RaceEvent)));
        Stats.Add(ToRow("合計", Statistics.Compute(records, StreakKinds), isTotal: true));

        var romance = Statistics.Compute(records, EventKind.RomanceConsultation);
        Stats.Add(ToRow("恋愛相談", romance) with { MissStreak = "-" });

        var awakening = Statistics.Compute(records, EventKind.AwakeningLv7);
        Stats.Add(new StatsRow("覚醒Lv7", $"{awakening.Occurrences}", "-", "-", "-"));

        Log.Clear();
        foreach (var entry in BuildLog(records).Take(MaxLogEntries))
        {
            Log.Add(entry);
        }

        UndoDescription = records.Count == 0 ? "取り消す記録はありません" : $"取り消し対象: {Describe(records[^1])}";
        Signature.RaiseCanExecuteChanged();
        Common.RaiseCanExecuteChanged();
        Race.RaiseCanExecuteChanged();
        Romance.RaiseCanExecuteChanged();
        Awakening.RaiseCanExecuteChanged();
        UndoCommand.RaiseCanExecuteChanged();
        ResetCommand.RaiseCanExecuteChanged();
    }

    /// <summary>最小化中の -32000 などの異常値は保存しない。</summary>
    private static bool IsValidPosition(double value) => double.IsFinite(value) && value > -10000;

    /// <summary>前回の位置に戻す。タイトルバーが画面外になる場合は OS 既定の位置にする。</summary>
    private void RestoreWindowPosition(ScreenBounds screen)
    {
        if (_data.Window.Left is not { } left || _data.Window.Top is not { } top)
        {
            return;
        }

        var visible = left + 100 > screen.Left && left < screen.Right - 50
                      && top >= screen.Top && top < screen.Bottom - 30;
        if (visible)
        {
            _windowLeft = left;
            _windowTop = top;
        }
    }

    private static StatsRow ToRow(string title, CategoryStats stats, bool isTotal = false) => new(
        title,
        $"{stats.Occurrences}",
        $"{stats.Successes}",
        stats.SuccessRate is { } rate ? $"{rate:P1}" : "-",
        $"{stats.MissStreak}",
        isTotal);

    /// <summary>切れ者を取得した記録を新しい順に返す。</summary>
    private static IEnumerable<LogEntry> BuildLog(List<CounterRecord> records)
    {
        var entries = new List<LogEntry>();
        var attempts = new Dictionary<EventKind, int>();
        foreach (var record in records)
        {
            if (record.Kind == EventKind.CareerStart)
            {
                continue;
            }

            attempts[record.Kind] = attempts.GetValueOrDefault(record.Kind) + 1;
            if (!record.Success)
            {
                continue;
            }

            var text = $"{record.Timestamp:MM/dd HH:mm}　#{record.CareerNumber}　{KindLabel(record.Kind)}";
            if (StreakKinds.Contains(record.Kind))
            {
                text += $"（{attempts[record.Kind]}回目で取得）";
            }

            entries.Add(new LogEntry(text));
            attempts[record.Kind] = 0;
        }

        entries.Reverse();
        return entries;
    }

    private static string Describe(CounterRecord record) => record.Kind switch
    {
        EventKind.CareerStart => $"育成開始（#{record.CareerNumber}）",
        EventKind.AwakeningLv7 => KindLabel(record.Kind),
        _ => $"{KindLabel(record.Kind)} {(record.Success ? "成功" : "失敗")}",
    };

    private static string KindLabel(EventKind kind) => kind switch
    {
        EventKind.CareerStart => "育成開始",
        EventKind.SignatureEvent => "勝負服/固有",
        EventKind.CommonEvent => "共通",
        EventKind.RomanceConsultation => "恋愛相談",
        EventKind.AwakeningLv7 => "覚醒Lv7",
        EventKind.RaceEvent => "レース後",
        _ => kind.ToString(),
    };
}
