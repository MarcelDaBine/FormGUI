using System;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using FormGUI.Interfaces;

namespace FormGUI.Services;

public class JsonFileIo : IFileIo
{
    private readonly Window _window;

    public JsonFileIo(Window window)
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
    public async Task<T?> OpenJsonAsync<T>()
    {
        try
        {
            var files = await _window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open JSON Report",
                AllowMultiple = false,
                SuggestedStartLocation =
                    await _window.StorageProvider.TryGetWellKnownFolderAsync(WellKnownFolder.Documents),
                FileTypeFilter = new List<FilePickerFileType>
                {
                    new("JSON Files")
                    {
                        Patterns = new[] { "*.json" },
                        MimeTypes = new[] { "application/json" }
                    }
                }
            });

            if (files.Count > 0)
            {
                await using var stream = await files[0].OpenReadAsync();
                using var reader = new StreamReader(stream);
                var json = await reader.ReadToEndAsync();
                return JsonSerializer.Deserialize<T>(json);
            }

            return default;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return default;
        }
    }
}