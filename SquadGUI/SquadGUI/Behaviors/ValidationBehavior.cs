using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using System.Linq;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;

namespace SquadGUI.Behaviors;

public class ValidationBehavior : AvaloniaObject
{
    // Register attached property
    public static readonly AttachedProperty<bool> EnableValidationProperty =
        AvaloniaProperty.RegisterAttached<ValidationBehavior, Control, bool>("EnableValidation");

    // Register the property changed callback
    static ValidationBehavior()
    {
        EnableValidationProperty.Changed.AddClassHandler<Control>((control, e) => 
            EnableValidationChanged(control, (bool)e.NewValue));
    }

    // Get and set methods for the attached property
    public static void SetEnableValidation(AvaloniaObject element, bool value) => element.SetValue(EnableValidationProperty, value);
    public static bool GetEnableValidation(AvaloniaObject element) => element.GetValue(EnableValidationProperty);

    // Handle when the attached property changes
    private static void EnableValidationChanged(AvaloniaObject element, bool isEnabled)
    {
        if (element is not TemplatedControl control) return;

        if (isEnabled)
        {
            // Attach validation logic when the control is added to the visual tree
            control.AttachedToVisualTree += (_, _) => AttachValidation(control);
        }
    }

    // Method to attach validation to the control
    private static void AttachValidation(TemplatedControl control)
    {
        if (control is TextBox textBox)
        {
            textBox.PropertyChanged += (sender, args) =>
            {
                if (args.Property.Name == nameof(TextBox.Text))
                    ValidateControl(textBox);
            };
        }
        else if (control is ComboBox comboBox)
        {
            comboBox.PropertyChanged += (sender, args) =>
            {
                if (args.Property.Name == nameof(ComboBox.IsDropDownOpen))
                    ValidateControl(comboBox);
            };
        }
    }

    // Validate all controls inside the parent panel
    public static void ValidateAll(Panel parent)
    {
        var controls = parent.GetVisualDescendants()
                             .OfType<TemplatedControl>()
                             .Where(GetEnableValidation);

        foreach (var control in controls)
        {
            ValidateControl(control);
        }
    }

    // Perform validation on a single control
    private static void ValidateControl(TemplatedControl control)
    {
        bool isValid = control switch
        {
            TextBox textBox => !string.IsNullOrWhiteSpace(textBox.Text),
            ComboBox comboBox => comboBox.IsDropDownOpen || comboBox.SelectedItem != null,
            _ => true
        };
        control.BorderBrush = isValid ? Brushes.Gray : Brushes.Red;
    }
}
