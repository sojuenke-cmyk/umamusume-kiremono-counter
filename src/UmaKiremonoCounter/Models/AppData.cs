namespace UmaKiremonoCounter.Models;

/// <summary>保存データ全体。</summary>
public sealed class AppData
{
    public int Version { get; set; } = 1;

    /// <summary>記録（古い順）。</summary>
    public List<CounterRecord> Records { get; set; } = [];

    public WindowSettings Window { get; set; } = new();
}

/// <summary>ウィンドウの状態。</summary>
public sealed class WindowSettings
{
    public double? Left { get; set; }

    public double? Top { get; set; }

    public bool Topmost { get; set; } = true;

    /// <summary>省スペースモードか。</summary>
    public bool IsCompact { get; set; }
}
