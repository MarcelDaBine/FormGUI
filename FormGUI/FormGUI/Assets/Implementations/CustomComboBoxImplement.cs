using System;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Input;

namespace FormGUI.Assets;

/// <summary>
/// Custom ComboBox that tracks the previous selection
/// </summary>
public class CustomComboBoxImplement: ComboBox
{
    protected override Type StyleKeyOverride => typeof(ComboBox);

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        IsDropDownOpen = true;
        base.OnPointerPressed(e);
    }
}