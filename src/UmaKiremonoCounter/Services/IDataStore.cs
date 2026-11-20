using UmaKiremonoCounter.Models;

namespace UmaKiremonoCounter.Services;

/// <summary>保存データの読み書き。</summary>
public interface IDataStore
{
    AppData Load();

    void Save(AppData data);
}
