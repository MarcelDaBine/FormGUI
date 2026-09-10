using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;

namespace FormGUI.Assets;

/// <summary>
/// A TextBox that only accepts numeric input for floating point numbers
/// </summary>
public class FloatNumTextBox : TextBox
{
    protected override Type StyleKeyOverride => typeof(TextBox);
    private int _dotPosition = -1;
    
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

        e.Text = FormatNumber(e.Text);
        
        base.OnTextInput(e);
    }

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
    /// Validates if the input text results in a valid numeric value
    /// Maximum value allowed is 2700 and leading zeros are not allowed
    /// </summary>
    private bool IsNumeric(string? text)
    { 
        var numString = Text + text;
        double num;
        
        if (!double.TryParse(numString, out num))
        {
            return false;
        }

        return (num <= 2700 && numString != "00");
    }

    /// <summary>
    /// Checks if the pressed key is a valid numeric input key
    /// Allows numbers, decimal point, subtract, backspace and enter keys
    /// </summary>
    private bool IsNumericKey(Key key)
    {
        return (key >= Key.D0 && key <= Key.D9) || (key >= Key.NumPad0 && key <= Key.NumPad9) || key == Key.OemPeriod || key == Key.Decimal || key == Key.Subtract || key == Key.Back || key == Key.Enter;
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