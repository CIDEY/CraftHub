using System;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Data.Core;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;
using CraftHub.Domain.Models;

namespace CraftHub.Helpers;

/// <summary>
/// Builds bindings to <see cref="DynamicDataRow"/>'s string indexer.
/// </summary>
public static class DynamicRowCellBinding
{
    public static CompiledBindingExtension ForKey(
        string key,
        BindingMode mode = BindingMode.Default,
        IValueConverter? converter = null)
    {
        var propertyInfo = new ClrPropertyInfo(
            key,
            row => ((DynamicDataRow)row)[key],
            (row, value) => ((DynamicDataRow)row)[key] = value as string ?? string.Empty,
            typeof(string));

        var path = new CompiledBindingPathBuilder()
            .Property(propertyInfo, PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
            .Build();

        return new CompiledBindingExtension(path)
        {
            Mode = mode,
            Converter = converter
        };
    }
}
