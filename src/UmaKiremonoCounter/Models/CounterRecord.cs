namespace UmaKiremonoCounter.Models;

/// <summary>記録の種類。</summary>
public enum EventKind
{
    /// <summary>育成開始。</summary>
    CareerStart,

    /// <summary>勝負服 or 固有の切れ者イベント。</summary>
    SignatureEvent,

    /// <summary>共通の切れ者イベント。</summary>
    CommonEvent,

    /// <summary>恋愛相談。</summary>
    RomanceConsultation,

    /// <summary>覚醒Lv7の切れ者イベント。</summary>
    AwakeningLv7,
}

/// <summary>1件の記録。すべてのカウント・統計はこの記録の並びから算出する。</summary>
/// <param name="Timestamp">記録日時。</param>
/// <param name="Kind">記録の種類。</param>
/// <param name="Success">切れ者を取得できたか（育成開始では未使用）。</param>
/// <param name="CareerNumber">何回目の育成での記録か。</param>
public sealed record CounterRecord(DateTime Timestamp, EventKind Kind, bool Success, int CareerNumber);
