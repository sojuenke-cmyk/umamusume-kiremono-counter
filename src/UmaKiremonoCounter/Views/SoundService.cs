using System.IO;
using System.Media;
using UmaKiremonoCounter.Services;

namespace UmaKiremonoCounter.Views;

/// <summary>
/// アプリ内で生成したチャイム音を鳴らす <see cref="ISoundService"/>。
/// 外部の音声ファイルに依存しないよう、起動時に WAV データを作っておく。
/// </summary>
public sealed class SoundService : ISoundService
{
    private const int SampleRate = 44100;

    private readonly SoundPlayer _player;

    public SoundService()
    {
        _player = new SoundPlayer(new MemoryStream(CreateChimeWav()));
        _player.Load();
    }

    /// <summary>ド・ミ・ソ・ド（上）のチャイムを2回鳴らす WAV データを作る。</summary>
    public static byte[] CreateChimeWav()
    {
        double[] notes = [1046.5, 1318.5, 1568.0, 2093.0];
        var samples = new List<short>();
        for (var repeat = 0; repeat < 2; repeat++)
        {
            foreach (var frequency in notes)
            {
                AppendTone(samples, frequency, seconds: 0.22);
            }

            AppendSilence(samples, seconds: 0.25);
        }

        return CreateWav(samples);
    }

    public void PlayAlarm()
    {
        try
        {
            _player.Play();
        }
        catch (InvalidOperationException)
        {
            SystemSounds.Exclamation.Play();
        }
    }

    private static void AppendTone(List<short> samples, double frequency, double seconds)
    {
        var count = (int)(SampleRate * seconds);
        for (var i = 0; i < count; i++)
        {
            var t = (double)i / SampleRate;
            var attack = Math.Min(1.0, i / (SampleRate * 0.005)); // クリックノイズ防止
            var decay = Math.Exp(-4.0 * t / seconds);
            var value = Math.Sin(2 * Math.PI * frequency * t) * attack * decay * 0.6;
            samples.Add((short)(value * short.MaxValue));
        }
    }

    private static void AppendSilence(List<short> samples, double seconds)
        => samples.AddRange(Enumerable.Repeat((short)0, (int)(SampleRate * seconds)));

    /// <summary>16bit モノラル PCM の WAV データを作る。</summary>
    private static byte[] CreateWav(List<short> samples)
    {
        const short channels = 1;
        const short bitsPerSample = 16;
        const short blockAlign = channels * bitsPerSample / 8;
        var dataSize = samples.Count * blockAlign;

        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);
        writer.Write("RIFF"u8);
        writer.Write(36 + dataSize);
        writer.Write("WAVE"u8);
        writer.Write("fmt "u8);
        writer.Write(16);
        writer.Write((short)1); // PCM
        writer.Write(channels);
        writer.Write(SampleRate);
        writer.Write(SampleRate * blockAlign);
        writer.Write(blockAlign);
        writer.Write(bitsPerSample);
        writer.Write("data"u8);
        writer.Write(dataSize);
        foreach (var sample in samples)
        {
            writer.Write(sample);
        }

        writer.Flush();
        return stream.ToArray();
    }
}
