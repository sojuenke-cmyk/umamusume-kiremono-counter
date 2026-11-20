using UmaKiremonoCounter.Services;

namespace UmaKiremonoCounter.ViewModels;

/// <summary>イベント1種類分の記録操作と、省スペース表示用の回数。</summary>
public sealed class CategoryViewModel : ObservableObject
{
    private string _summary = string.Empty;

    /// <param name="title">表示名。</param>
    /// <param name="shortTitle">省スペース表示用の短い名前。</param>
    /// <param name="hasOutcome">成功/失敗を記録するか（false なら発生回数のみ）。</param>
    /// <param name="showStreak">未取得連続回数を表示するか。</param>
    /// <param name="record">記録処理（引数は成功したか）。</param>
    /// <param name="canRecord">記録できる状態か。</param>
    public CategoryViewModel(
        string title, string shortTitle, bool hasOutcome, bool showStreak, Action<bool> record, Func<bool> canRecord)
    {
        Title = title;
        ShortTitle = shortTitle;
        HasOutcome = hasOutcome;
        ShowStreak = showStreak;
        SuccessCommand = new RelayCommand(() => record(true), canRecord);
        FailureCommand = new RelayCommand(() => record(false), canRecord);
        AddCommand = new RelayCommand(() => record(true), canRecord);
    }

    public string Title { get; }

    public string ShortTitle { get; }

    public bool HasOutcome { get; }

    public bool IsCountOnly => !HasOutcome;

    public bool ShowStreak { get; }

    public RelayCommand SuccessCommand { get; }

    public RelayCommand FailureCommand { get; }

    public RelayCommand AddCommand { get; }

    /// <summary>累計の回数（例: 「成功 3/10　未取得連続 2」）。</summary>
    public string Summary
    {
        get => _summary;
        private set => SetProperty(ref _summary, value);
    }

    /// <param name="total">累計の集計。</param>
    public void Update(CategoryStats total)
    {
        if (!HasOutcome)
        {
            Summary = $"発生 {total.Occurrences}";
            return;
        }

        Summary = ShowStreak
            ? $"成功 {total.Successes}/{total.Occurrences}　未取得連続 {total.MissStreak}"
            : $"成功 {total.Successes}/{total.Occurrences}";
    }

    public void RaiseCanExecuteChanged()
    {
        SuccessCommand.RaiseCanExecuteChanged();
        FailureCommand.RaiseCanExecuteChanged();
        AddCommand.RaiseCanExecuteChanged();
    }
}
