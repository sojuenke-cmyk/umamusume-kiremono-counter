using UmaKiremonoCounter.Services;

namespace UmaKiremonoCounter.ViewModels;

/// <summary>イベント1種類分の表示と操作。</summary>
public sealed class CategoryViewModel : ObservableObject
{
    private string _summary = string.Empty;

    /// <param name="title">表示名。</param>
    /// <param name="hasOutcome">成功/失敗を記録するか（false なら発生回数のみ）。</param>
    /// <param name="showStreak">未取得連続回数を表示するか。</param>
    /// <param name="record">記録処理（引数は成功したか）。</param>
    /// <param name="canRecord">記録できる状態か。</param>
    public CategoryViewModel(string title, bool hasOutcome, bool showStreak, Action<bool> record, Func<bool> canRecord)
    {
        Title = title;
        HasOutcome = hasOutcome;
        ShowStreak = showStreak;
        SuccessCommand = new RelayCommand(() => record(true), canRecord);
        FailureCommand = new RelayCommand(() => record(false), canRecord);
        AddCommand = new RelayCommand(() => record(true), canRecord);
    }

    public string Title { get; }

    public bool HasOutcome { get; }

    public bool IsCountOnly => !HasOutcome;

    public bool ShowStreak { get; }

    public RelayCommand SuccessCommand { get; }

    public RelayCommand FailureCommand { get; }

    public RelayCommand AddCommand { get; }

    public void RaiseCanExecuteChanged()
    {
        SuccessCommand.RaiseCanExecuteChanged();
        FailureCommand.RaiseCanExecuteChanged();
        AddCommand.RaiseCanExecuteChanged();
    }

    public string Summary
    {
        get => _summary;
        private set => SetProperty(ref _summary, value);
    }

    /// <param name="current">今回の育成での集計。</param>
    /// <param name="missStreak">育成をまたいだ未取得連続回数。</param>
    public void Update(CategoryStats current, int missStreak)
    {
        var parts = new List<string> { $"発生 {current.Occurrences}" };
        if (HasOutcome)
        {
            parts.Add($"成功 {current.Successes}");
        }

        if (ShowStreak)
        {
            parts.Add($"未取得連続 {missStreak}");
        }

        Summary = string.Join("　", parts);
    }
}
