using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using WeatherPresenter.App.Models;

namespace WeatherPresenter.App;

public static class OlieCommon
{
    public static async Task<List<ImageModel>> LoadImages(IStorageProvider storageProvider)
    {
        var folders = await storageProvider.OpenFolderPickerAsync(
            new FolderPickerOpenOptions
            {
                Title = "Open an image folder",
                AllowMultiple = false
            });
        if (folders.Count == 0) return [];

        var items = await folders[0].GetItemsAsync().ToListAsync();
        var result = new List<ImageModel>();

        foreach (var storageItem in items)
        {
            using var fileItem = await storageProvider.TryGetFileFromPathAsync(storageItem.Path.AbsolutePath);

            if (fileItem is not null)
            {
                await using var stream = await fileItem.OpenReadAsync();
                var bitmap = new Bitmap(stream);
                result.Add(new ImageModel
                {
                    Bitmap = bitmap,
                    Filename = fileItem.Name
                });
            }

            storageItem.Dispose();
        }

        folders[0].Dispose();

        return result;
    }
}