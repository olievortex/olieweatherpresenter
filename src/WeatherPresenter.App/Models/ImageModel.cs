using System;
using Avalonia.Media.Imaging;

namespace WeatherPresenter.App.Models;

public class ImageModel : IDisposable
{
    public required Bitmap Bitmap { get; init; }
    public string Filename { get; init; } = string.Empty;

    public void Dispose()
    {
        Bitmap.Dispose();
        GC.SuppressFinalize(this);
    }
}