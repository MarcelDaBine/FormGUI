using System;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Input;

namespace SquadGUI.Assets;

/// <summary>
/// Custom TextBox that only accepts numeric input with a maximum value of 1200
/// </summary>
public class NumTextBox : TextBox
{
    protected override Type StyleKeyOverride => typeof(TextBox);

    public NumTextBox()
    {
        Text = "";
    }
    
    /// <summary>
    /// Handles text input events and validates if the input is numeric
    /// </summary>
    protected override void OnTextInput(TextInputEventArgs e)
    {
        if (!IsNumeric(e.Text))
        {
            e.Handled = true;
            return;
        }
        base.OnTextInput(e);
    }

    /// <summary>
    /// Handles key down events and validates if the pressed key is allowed for numeric input
    /// </summary>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (!IsNumericKey(e))
        {
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
    }

    /// <summary>
    /// Checks if the input text would result in a valid number less than or equal to 1200
    /// </summary>
    private bool IsNumeric(string text)
    { 
        var selectionStart = SelectionStart;
        var selectionLength = SelectionEnd - SelectionStart;

        var before = Text.Substring(0, selectionStart);

        var result = before.Remove(selectionStart + selectionLength) + text;

        if (!int.TryParse(result, out var num))
        {
            return false;
        }
        return (num <= 2700 && result != "00" && !result.Contains(".00"));
    }

    /// <summary>
    /// Validates if the pressed key is a numeric key, decimal point, minus sign, backspace, or enter
    /// </summary>
    private bool IsNumericKey(KeyEventArgs e)
    {
        var key = e.Key;
        var modifiers = e.KeyModifiers;

        // Allow Ctrl combinations (like Ctrl+V, Ctrl+C, Ctrl+X, Ctrl+A, Ctrl+Z, Ctrl+Y)
        if (modifiers.HasFlag(KeyModifiers.Control))
        {
            if (e.Key == Key.V)
            {
                var topLevel = TopLevel.GetTopLevel(this);
                if (topLevel?.Clipboard is { } clipboard)
                {
                    var pasteText = clipboard.GetTextAsync();

                    if (!IsNumeric(pasteText.Result))
                    {
                        e.Handled = true;
                        return false;
                    }
                }
            }
            return key == Key.V || key == Key.C || key == Key.X ||
                   key == Key.A || key == Key.Z || key == Key.Y;
        }
        
        if ((key >= Key.D0 && key <= Key.D9) || (key >= Key.NumPad0 && key <= Key.NumPad9))
            return true;
        
        if (key == Key.Decimal || key == Key.OemPeriod)
            return true;
        
        return (key == Key.Back || key == Key.Enter || key == Key.Tab || key == Key.Delete ||
                key == Key.Left || key == Key.Right || key == Key.Up || key == Key.Down ||
                key == Key.Home || key == Key.End);
    }
}