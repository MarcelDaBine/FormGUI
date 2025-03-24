using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;

namespace SquadGUI.Assets;

/// <summary>
/// A TextBox that only accepts numeric input for floating point numbers
/// </summary>
public class FloatNumTextBox : TextBox
{
    protected override Type StyleKeyOverride => typeof(TextBox);
    private int _dotPosition = -1;

    public FloatNumTextBox()
    {
        Text = "";
    }
    
    /// <summary>
    /// Handles text input events to ensure only numeric values that are smaller than 2700 are entered
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
    /// Handles key down events to ensure only numeric keys are accepted
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
    /// Validates if the input text results in a valid numeric value
    /// Maximum value allowed is 2700 and leading zeros are not allowed
    /// </summary>
    private bool IsNumeric(string? text)
    {
        var start = Math.Min(SelectionStart, SelectionEnd);
        var length = Math.Abs(SelectionEnd - SelectionStart);

        var result = Text.Remove(start, length) + text;
        

        if (!double.TryParse(result, out var num))
        {
            return false;
        }

        if (num <= 10000 && result != "00" && !result.Contains(".00"))
        {
            var temp = result.Split('.');
            if (text == "." || temp.Length == 1)
            {
                return true;
            }
        
            if (temp[1].Length < 4)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks if the pressed key is a valid numeric input key
    /// Allows numbers, decimal point, subtract, backspace and enter keys
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