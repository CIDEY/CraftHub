using System.Collections.Generic;
using System.Threading.Tasks;

namespace CraftHub.Core;

public interface IFileDialogService
{
    Task<string?> OpenFileAsync(string title, IReadOnlyList<FileFilter> filters);
    Task<IReadOnlyList<string>> OpenMultipleFilesAsync(string title, IReadOnlyList<FileFilter> filters);
    Task<string?> SaveFileAsync(string title, IReadOnlyList<FileFilter> filters, string suggestedFileName, string? suggestedDirectory = null);

    /// <summary>Show a folder picker and return the selected folder path, or null if cancelled.</summary>
    Task<string?> OpenFolderAsync(string title, string? startLocation = null);
}
