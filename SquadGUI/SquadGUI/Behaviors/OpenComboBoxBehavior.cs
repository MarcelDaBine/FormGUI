using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Xaml.Interactivity;

namespace SquadGUI.Behaviors
{
    public class OpenComboBoxBehavior : Behavior<ComboBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject != null)
            {
                AssociatedObject.PointerPressed += OnPointerPressed;
                //AssociatedObject.LostFocus += OnLostFocus;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (AssociatedObject != null)
            {
                AssociatedObject.PointerPressed -= OnPointerPressed;
                //AssociatedObject.LostFocus -= OnLostFocus;
            }
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.IsDropDownOpen = true;
            }
        }

        private void OnLostFocus(object? sender, RoutedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                comboBox.Classes.Remove(":pressed");
                //comboBox.Classes.Add("normal");
            }
        }
    }
}
