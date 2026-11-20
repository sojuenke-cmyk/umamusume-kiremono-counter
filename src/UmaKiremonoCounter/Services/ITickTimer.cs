namespace UmaKiremonoCounter.Services;

/// <summary>UI スレッドで一定間隔ごとに Tick を発生させるタイマー。</summary>
public interface ITickTimer
{
    event EventHandler? Tick;

    void Start();

    void Stop();
}
