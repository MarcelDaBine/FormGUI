using System;
using Avalonia.Controls;
using Avalonia.Input;

namespace SquadGUI.Assets;

/// <summary>
/// Custom TextBox that only accepts numeric input with a maximum value of 1200
/// </summary>
public class NumTextBox : TextBox
{
    protected override Type StyleKeyOverride => typeof(TextBox);
    
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
        if (!IsNumericKey(e.Key))
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
        double num;
        double.TryParse(this.Text + text, out num);

        if (num <= 1200)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Validates if the pressed key is a numeric key, decimal point, minus sign, backspace, or enter
    /// </summary>
    private bool IsNumericKey(Key key)
    {
        return (key >= Key.D0 && key <= Key.D9) || (key >= Key.NumPad0 && key <= Key.NumPad9) || key == Key.Decimal || key == Key.OemPeriod || key == Key.Subtract || key == Key.Back || key == Key.Enter;
    }
}