using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using UmaKiremonoCounter.Models;

namespace UmaKiremonoCounter.Services;

/// <summary>保存データを JSON ファイルに読み書きする。</summary>
public sealed class DataStore : IDataStore
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
    };

    public DataStore(string? directory = null)
    {
        directory ??= Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "UmaKiremonoCounter");
        FilePath = Path.Combine(directory, "data.json");
    }

    public string FilePath { get; }

    public AppData Load()
    {
        if (!File.Exists(FilePath))
        {
            return new AppData();
        }

        try
        {
            return JsonSerializer.Deserialize<AppData>(File.ReadAllText(FilePath), Options) ?? new AppData();
        }
        catch (JsonException)
        {
            // 壊れたファイルは退避して新規データで起動する（上書きで消さない）。
            File.Copy(FilePath, $"{FilePath}.broken-{DateTime.Now:yyyyMMddHHmmss}", overwrite: true);
            return new AppData();
        }
    }

    public void Save(AppData data)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);

        // 書き込み途中で落ちてもデータが壊れないよう、一時ファイルに書いてから置き換える。
        var temp = FilePath + ".tmp";
        File.WriteAllText(temp, JsonSerializer.Serialize(data, Options));
        File.Move(temp, FilePath, overwrite: true);
    }
}
