namespace UmaKiremonoCounter.ViewModels;

/// <summary>イベント1種類分の記録操作。集計の表示は統計側で行う。</summary>
public sealed class CategoryViewModel
{
    /// <param name="title">表示名。</param>
    /// <param name="hasOutcome">成功/失敗を記録するか（false なら発生回数のみ）。</param>
    /// <param name="record">記録処理（引数は成功したか）。</param>
    /// <param name="canRecord">記録できる状態か。</param>
    public CategoryViewModel(string title, bool hasOutcome, Action<bool> record, Func<bool> canRecord)
    {
        Title = title;
        HasOutcome = hasOutcome;
        SuccessCommand = new RelayCommand(() => record(true), canRecord);
        FailureCommand = new RelayCommand(() => record(false), canRecord);
        AddCommand = new RelayCommand(() => record(true), canRecord);
    }

    public string Title { get; }

    public bool HasOutcome { get; }

    public bool IsCountOnly => !HasOutcome;

    public RelayCommand SuccessCommand { get; }

    public RelayCommand FailureCommand { get; }

    public RelayCommand AddCommand { get; }

    public void RaiseCanExecuteChanged()
    {
        SuccessCommand.RaiseCanExecuteChanged();
        FailureCommand.RaiseCanExecuteChanged();
        AddCommand.RaiseCanExecuteChanged();
    }
}
