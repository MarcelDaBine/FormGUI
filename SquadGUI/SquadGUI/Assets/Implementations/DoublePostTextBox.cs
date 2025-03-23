using System;
using System.Linq;
using System.Net.Mime;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace SquadGUI.Assets;

/// <summary>
/// A TextBox that only accepts numeric input for floating point numbers
/// </summary>
public class DoublePostTextBox : TextBox
{
    protected override Type StyleKeyOverride => typeof(TextBox);
    public enum PostUnit
    {
        None,
        Grams,
        Pounds,
        Celsius,
        Fahrenheit,
    }

    /// <summary>
    /// Set this property to Celsius or Fahrenheit to have the text box always
    /// display the corresponding unit when not editing.
    /// </summary>
    public PostUnit Unit { get; set; } = PostUnit.None;

    
    public DoublePostTextBox()
    {
        Text = "";
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
    /// Validates if the input text results in a valid numeric value.
    /// Maximum value allowed is 10000 and leading zeros are not allowed.
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


    
    /// <summary>
    /// Handles text input events to ensure only numeric values that are smaller than 2700 are entered.
    /// </summary>
    protected override void OnTextInput(TextInputEventArgs e)
    {
        // We remove the unit if it is present so we validate the numeric part.
        if (Unit != PostUnit.None)
            e.Text = e.Text?.Replace(" g","").Replace(" lb","").Replace(" \u00b0C°","").Replace(" \u00b0F","");
        
        if (!IsNumeric(e.Text))
        {
            e.Handled = true;
            return;
        }
        
        base.OnTextInput(e);
    }
    
    /// <summary>
    /// When the text box receives focus, remove any appended unit so the user
    /// can edit only the number.
    /// </summary>
    protected override void OnGotFocus(GotFocusEventArgs e)
    {
        base.OnGotFocus(e);
        if (Unit != PostUnit.None)
        {
            Text = RemoveUnitFromText(Text);
        }
    }

    /// <summary>
    /// When the text box loses focus, append the proper unit if applicable.
    /// </summary>
    protected override void OnLostFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);
        if (!string.IsNullOrEmpty(Text))
        {
            Text = AppendUnit(Text);
        }
    }

    private string AppendUnit(string text)
    {
        return Unit switch
        {
            PostUnit.Grams => text + " g",
            PostUnit.Pounds => text + " lb",
            PostUnit.Celsius => text + " \u00b0C",
            PostUnit.Fahrenheit => text + " \u00b0F",
            _ => text,
        };
    }

    private string RemoveUnitFromText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;
        text = text.Trim();
        if (text.EndsWith("lb") || text.EndsWith("\u00b0C") || text.EndsWith("\u00b0F"))
            return text.Substring(0, text.Length - 2).Trim();
        if (text.EndsWith('g'))
            return text.Substring(0, text.Length - 1).Trim();
        return text;
    }
    
    private string FormatNumber(string number)
    {
        var temp = (Text + number).Split('.');
        if (number == "." || temp.Length == 1)
        {
            return number;
        }
        return temp[1].Length > 3 ? "" : number;
    }
}