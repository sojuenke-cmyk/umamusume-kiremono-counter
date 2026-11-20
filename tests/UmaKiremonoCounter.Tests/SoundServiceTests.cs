using System.Text;
using UmaKiremonoCounter.Views;

namespace UmaKiremonoCounter.Tests;

public class SoundServiceTests
{
    [Fact]
    public void チャイムは正しいPCM_WAV形式()
    {
        var wav = SoundService.CreateChimeWav();

        Assert.Equal("RIFF", Encoding.ASCII.GetString(wav, 0, 4));
        Assert.Equal(wav.Length - 8, BitConverter.ToInt32(wav, 4));
        Assert.Equal("WAVE", Encoding.ASCII.GetString(wav, 8, 4));
        Assert.Equal("fmt ", Encoding.ASCII.GetString(wav, 12, 4));
        Assert.Equal(1, BitConverter.ToInt16(wav, 20));      // PCM
        Assert.Equal(1, BitConverter.ToInt16(wav, 22));      // モノラル
        Assert.Equal(44100, BitConverter.ToInt32(wav, 24));  // サンプリングレート
        Assert.Equal(16, BitConverter.ToInt16(wav, 34));     // 16bit
        Assert.Equal("data", Encoding.ASCII.GetString(wav, 36, 4));
        Assert.Equal(wav.Length - 44, BitConverter.ToInt32(wav, 40));

        // 2〜3秒程度の長さ。
        var seconds = (wav.Length - 44) / 2.0 / 44100;
        Assert.InRange(seconds, 2.0, 3.0);
    }
}
