using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using CraftHub.ViewModels;

namespace CraftHub.Views;

public partial class FileExplorerView : UserControl
{
    private const string FileDragFormat = "crafthub/file-path";

    private FileSystemItemViewModel? _dragCandidate;
    private Point _dragStart;

    public FileExplorerView()
    {
        InitializeComponent();
        FileTree.KeyDown += OnTreeKeyDown;
        FileTree.AddHandler(DragDrop.DragOverEvent, OnTreeDragOver);
        FileTree.AddHandler(DragDrop.DropEvent, OnTreeDrop);
    }

    private void OnItemDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (sender is Control { DataContext: FileSystemItemViewModel item } &&
            DataContext is FileExplorerViewModel vm)
        {
            vm.ActivateItem(item);
            e.Handled = true;
        }
    }

    // Copy / cut / paste / delete on the selected node — like a file manager.
    private void OnTreeKeyDown(object? sender, KeyEventArgs e)
    {
        if (DataContext is not FileExplorerViewModel vm) return;

        if (e.KeyModifiers == KeyModifiers.Control)
        {
            switch (e.Key)
            {
                case Key.C: vm.CopySelectedCommand.Execute(null); e.Handled = true; break;
                case Key.X: vm.CutSelectedCommand.Execute(null); e.Handled = true; break;
                case Key.V: vm.PasteSelectedCommand.Execute(null); e.Handled = true; break;
            }
        }
        else if (e.Key == Key.Delete)
        {
            vm.DeleteSelectedCommand.Execute(null);
            e.Handled = true;
        }
    }

    //  Drag & drop: move files/folders between folders

    private void OnItemPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Visual visual && visual.DataContext is FileSystemItemViewModel item &&
            e.GetCurrentPoint(visual).Properties.IsLeftButtonPressed)
        {
            _dragCandidate = item;
            _dragStart = e.GetPosition(null);
        }
    }

    private async void OnItemPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_dragCandidate is null || sender is not Visual visual) return;

        if (!e.GetCurrentPoint(visual).Properties.IsLeftButtonPressed)
        {
            _dragCandidate = null;
            return;
        }

        var pos = e.GetPosition(null);
        if (Math.Abs(pos.X - _dragStart.X) < 5 && Math.Abs(pos.Y - _dragStart.Y) < 5) return;

        var item = _dragCandidate;
        _dragCandidate = null;

        var data = new DataObject();
        data.Set(FileDragFormat, item.FullPath);
        try
        {
            await DragDrop.DoDragDrop(e, data, DragDropEffects.Move);
        }
        catch
        {
            // drag cancelled — ignore
        }
    }

    private void OnTreeDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = ResolveDropTarget(e) is not null ? DragDropEffects.Move : DragDropEffects.None;
        e.Handled = true;
    }

    private void OnTreeDrop(object? sender, DragEventArgs e)
    {
        if (DataContext is FileExplorerViewModel vm &&
            e.Data.Get(FileDragFormat) is string source &&
            ResolveDropTarget(e) is { } target)
        {
            _ = vm.MoveItemAsync(source, target);
        }
        e.Handled = true;
    }

    /// <summary>Finds the node under the cursor and validates it as a drop target.</summary>
    private static FileSystemItemViewModel? ResolveDropTarget(DragEventArgs e)
    {
        if (e.Data.Get(FileDragFormat) is not string source || string.IsNullOrEmpty(source))
            return null;

        var visual = e.Source as Visual;
        while (visual != null)
        {
            if (visual is Control { DataContext: FileSystemItemViewModel item })
            {
                // Dropping onto the dragged item itself does nothing.
                return string.Equals(source, item.FullPath, StringComparison.OrdinalIgnoreCase) ? null : item;
            }
            visual = visual.GetVisualParent();
        }
        return null;
    }
}
