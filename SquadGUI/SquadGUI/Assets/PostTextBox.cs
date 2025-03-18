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
        Celsius,
        Fahrenheit,
        Percent
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
        if (!IsNumericKey(e.Key))
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
    private bool IsNumeric(string? text)
    {
        var numString = RemoveUnitFromText(Text) + text;

        if (!int.TryParse(numString, out var num))
        {
            return false;
        }
        
        return Unit != PostUnit.Percent ? (num <= 300 && numString != "00") : (num <= 100 && numString != "00");
    }

    /// <summary>
    /// Checks if the pressed key is a valid numeric input key
    /// Allows numbers, decimal point, subtract, backspace and enter keys
    /// </summary>
    private bool IsNumericKey(Key key)
    {
        return (key >= Key.D0 && key <= Key.D9) || (key >= Key.NumPad0 && key <= Key.NumPad9) || key == Key.Decimal ||
               key == Key.Back || key == Key.Enter;
    }
    
    /// <summary>
    /// Handles text input events to ensure only numeric values that are smaller than 2700 are entered.
    /// </summary>
    protected override void OnTextInput(TextInputEventArgs e)
    {
        // We remove the unit if it is present so we validate the numeric part.
        if (Unit != PostUnit.None)
            e.Text = e.Text?.Replace("°C", "").Replace("°F", "").Replace("%", "");

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
            PostUnit.Celsius => text + " °C",
            PostUnit.Fahrenheit => text + " °F",
            PostUnit.Percent => text + "%",
            _ => text,
        };
    }

    private string RemoveUnitFromText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;
        text = text.Trim();
        if (text.EndsWith("°C") || text.EndsWith("°F"))
            return text.Substring(0, text.Length - 2).Trim();
        if (text.EndsWith("%"))
            return text.Substring(0, text.Length - 1).Trim();
        return text;
    }
}