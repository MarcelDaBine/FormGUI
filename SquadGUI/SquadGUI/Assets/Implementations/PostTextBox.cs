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
public class PostTextBox : TextBox
{
    protected override Type StyleKeyOverride => typeof(TextBox);
    public enum PostUnit
    {
        None,
        Percent,
    }

    /// <summary>
    /// Set this property to Celsius or Fahrenheit to have the text box always
    /// display the corresponding unit when not editing.
    /// </summary>
    public PostUnit Unit { get; set; } = PostUnit.None;


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
    /// Maximum value allowed is 2700 and leading zeros are not allowed.
    /// </summary>
    private bool IsNumeric(string? inputText)
    {
        var originalText = RemoveUnitFromText(Text);

        var selectionStart = SelectionStart;
        var selectionLength = SelectionEnd - SelectionStart;

        var before = originalText.Substring(0, selectionStart);

        var result = before.Remove(selectionStart + selectionLength) + inputText;
        if (!int.TryParse(result, out var num))
            return false;
        return num <= 100;
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
            e.Text = e.Text?.Replace("%","");

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
            PostUnit.Percent => text + "%",
            _ => text,
        };
    }

    private string RemoveUnitFromText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;
        text = text.Trim();
        return text.EndsWith('%') ? text.AsSpan(0, text.Length - 1).Trim().ToString() : text;
    }
}