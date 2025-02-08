using System;
using Avalonia.Controls;
using Avalonia.Input;

namespace SquadGUI.Assets;

public class NumTextBox : TextBox
{
    protected override Type StyleKeyOverride => typeof(TextBox);
    
    protected override void OnTextInput(TextInputEventArgs e)
    {
        if (!IsNumeric(e.Text))
        {
            e.Handled = true;
            return;
        }

        base.OnTextInput(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (!IsNumericKey(e.Key))
        {
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
    }

    private bool IsNumeric(string text)
    {
        return double.TryParse(text, out _);
    }

    private bool IsNumericKey(Key key)
    {
        return (key >= Key.D0 && key <= Key.D9) || (key >= Key.NumPad0 && key <= Key.NumPad9) || key == Key.Decimal || key == Key.OemPeriod || key == Key.Subtract || key == Key.OemMinus || key == Key.Back || key == Key.Enter;
    }
}