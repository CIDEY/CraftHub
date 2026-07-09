using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CraftHub.Core;

namespace CraftHub.Services;

public class FileDialogService : IFileDialogService
{
    private IStorageProvider? GetStorageProvider()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            return desktop.MainWindow?.StorageProvider;
        return null;
    }

    public async Task<string?> OpenFileAsync(string title, IReadOnlyList<FileFilter> filters)
    {
        var sp = GetStorageProvider();
        if (sp == null) return null;

        var avaloniaFilters = filters.Select(f => new FilePickerFileType(f.Name) { Patterns = f.Patterns }).ToList();

        var result = await sp.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = false,
            FileTypeFilter = avaloniaFilters
        });

        return result.FirstOrDefault()?.Path.LocalPath;
    }

    public async Task<IReadOnlyList<string>> OpenMultipleFilesAsync(string title, IReadOnlyList<FileFilter> filters)
    {
        var sp = GetStorageProvider();
        if (sp == null) return Array.Empty<string>();

        var avaloniaFilters = filters.Select(f => new FilePickerFileType(f.Name) { Patterns = f.Patterns }).ToList();

        var result = await sp.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = true,
            FileTypeFilter = avaloniaFilters
        });

        return result.Select(f => f.Path.LocalPath).ToList();
    }

    public async Task<string?> SaveFileAsync(string title, IReadOnlyList<FileFilter> filters, string suggestedFileName, string? suggestedDirectory = null)
    {
        var sp = GetStorageProvider();
        if (sp == null) return null;

        var avaloniaFilters = filters.Select(f => new FilePickerFileType(f.Name) { Patterns = f.Patterns }).ToList();

        if (string.IsNullOrWhiteSpace(suggestedFileName))
            suggestedFileName = "newFile";

        IStorageFolder? startLocation = null;
        if (!string.IsNullOrWhiteSpace(suggestedDirectory))
        {
            try
            {
                startLocation = await sp.TryGetFolderFromPathAsync(suggestedDirectory);
            }
            catch
            {
                startLocation = null;
            }
        }

        var result = await sp.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = title,
            SuggestedFileName = suggestedFileName,
            SuggestedStartLocation = startLocation,
            FileTypeChoices = avaloniaFilters
        });

        return result?.Path.LocalPath;
    }

    public async Task<string?> OpenFolderAsync(string title, string? startLocation = null)
    {
        var sp = GetStorageProvider();
        if (sp == null) return null;

        IStorageFolder? suggestedStart = null;
        if (!string.IsNullOrWhiteSpace(startLocation))
        {
            try
            {
                suggestedStart = await sp.TryGetFolderFromPathAsync(startLocation);
            }
            catch
            {
                suggestedStart = null;
            }
        }

        var result = await sp.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = title,
            AllowMultiple = false,
            SuggestedStartLocation = suggestedStart
        });

        return result.FirstOrDefault()?.Path.LocalPath;
    }
}
