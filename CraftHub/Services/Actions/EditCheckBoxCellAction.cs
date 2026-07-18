using Avalonia.Controls;
using CraftHub.Core;
using CraftHub.Domain.Models;
using CraftHub.Helpers;

namespace CraftHub.Services.Actions;

public sealed class EditCheckBoxCellAction : IUndoableAction
{
    private readonly DynamicDataRow _row;
    private readonly string _propName;
    private readonly bool? _oldValue;
    private readonly bool? _newValue;
    private readonly DataGrid? _dataGrid;

    public EditCheckBoxCellAction(DynamicDataRow row, string propName, bool? oldValue, bool? newValue, DataGrid? dataGrid = null)
    {
        _row = row;
        _propName = propName;
        _oldValue = oldValue;
        _newValue = newValue;
        _dataGrid = dataGrid;
    }
    
    public string Description => Localizer.Get("UndoDescEditCell", _propName);
    public void Undo()
    {
        _row[_propName] = _oldValue?.ToString().ToLower();
        ForceDataGridUpdate();
    }

    public void Redo()
    {
        _row[_propName] = _newValue?.ToString().ToLower();
        ForceDataGridUpdate();
    }
    
    private void ForceDataGridUpdate()
    {
        if (_dataGrid?.ItemsSource is System.Collections.IList list)
        {
            var itemsSource = _dataGrid.ItemsSource;
            _dataGrid.ItemsSource = null;
            _dataGrid.ItemsSource = itemsSource;
        }
    }
}