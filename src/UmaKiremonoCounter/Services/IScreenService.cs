namespace UmaKiremonoCounter.Services;

/// <summary>全モニターを合わせた仮想画面の範囲。</summary>
public readonly record struct ScreenBounds(double Left, double Top, double Width, double Height)
{
    public double Right => Left + Width;

    public double Bottom => Top + Height;
}

/// <summary>画面情報の取得。</summary>
public interface IScreenService
{
    ScreenBounds VirtualScreen { get; }
}
