namespace UmaKiremonoCounter.Services;

/// <summary>効果音の再生。</summary>
public interface ISoundService
{
    /// <summary>タイマー終了の通知音を鳴らす。</summary>
    void PlayAlarm();
}
