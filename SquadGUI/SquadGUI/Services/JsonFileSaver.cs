using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using SquadGUI.Interfaces;

namespace SquadGUI.Services;

public class JsonFileSaver : IFileSaver
{
    private readonly Window _window;

    public JsonFileSaver(Window window)
    {
        _window = window;
    }

    public async Task SubmitJsonAsync(string json)
    {
        var file = await _window.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save Report As",
            SuggestedFileName = "report",
            DefaultExtension = "json",
            ShowOverwritePrompt = true,
            SuggestedStartLocation = await _window.StorageProvider.TryGetWellKnownFolderAsync(WellKnownFolder.Documents),
            FileTypeChoices = new List<FilePickerFileType>
            {
                new("JSON Files")
                {
                    Patterns = new[] { "*.json" },
                    MimeTypes = new[] { "application/json" }
                }
            }
        });

        if (file != null)
        {
            await using var stream = await file.OpenWriteAsync();
            using var writer = new StreamWriter(stream);
            await writer.WriteAsync(json);
        }
    }
    public async Task SaveJsonAsync(string json)
    {
        var file = await _window.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save Report As",
            SuggestedFileName = "report",
            DefaultExtension = "json",
            ShowOverwritePrompt = true,
            SuggestedStartLocation = await _window.StorageProvider.TryGetWellKnownFolderAsync(WellKnownFolder.Documents),
            FileTypeChoices = new List<FilePickerFileType>
            {
                new("JSON Files")
                {
                    Patterns = new[] { "*.json" },
                    MimeTypes = new[] { "application/json" }
                }
            }
        });

        if (file != null)
        {
            await using var stream = await file.OpenWriteAsync();
            using var writer = new StreamWriter(stream);
            await writer.WriteAsync(json);
        }
    }
}