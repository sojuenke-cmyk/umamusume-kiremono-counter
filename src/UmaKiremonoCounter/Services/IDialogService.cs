namespace UmaKiremonoCounter.Services;

/// <summary>ViewModel から View のダイアログを使うための窓口。</summary>
public interface IDialogService
{
    /// <summary>はい/いいえの確認を出し、「はい」なら true を返す。</summary>
    bool Confirm(string message);
}
