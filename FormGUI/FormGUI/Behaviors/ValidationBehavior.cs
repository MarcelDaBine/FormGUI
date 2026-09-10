using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using System.Linq;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using ReactiveUI;

namespace FormGUI.Behaviors;

public class ValidationBehavior : AvaloniaObject
{
    // Register attached property
    public static readonly AttachedProperty<bool> EnableValidationProperty =
        AvaloniaProperty.RegisterAttached<ValidationBehavior, Control, bool>("EnableValidation");
    
    public static readonly AttachedProperty<bool> IsControlProperty =
        AvaloniaProperty.RegisterAttached<ValidationBehavior, Control, bool>("IsControl");

    // Register the property changed callback
    static ValidationBehavior()
    {
        EnableValidationProperty.Changed.AddClassHandler<Control>((control, e) => 
            EnableValidationChanged(control, (bool)e.NewValue));
    }

    // Get and set methods for the attached property
    public static void SetEnableValidation(AvaloniaObject element, bool value) => element.SetValue(EnableValidationProperty, value);
    public static bool GetEnableValidation(AvaloniaObject element) => element.GetValue(EnableValidationProperty);
    
    public static void SetIsControl(AvaloniaObject element, bool value) => element.SetValue(IsControlProperty, value);
    public static bool GetIsControl(AvaloniaObject element) => element.GetValue(IsControlProperty);

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
                if (args.Property.Name == nameof(TextBox.IsFocused) || args.Property.Name == nameof(TextBox.Text))
                    ValidateControl(textBox);
            };
        }
        else if (control is ComboBox comboBox)
        {
            comboBox.PropertyChanged += (sender, args) =>
            {
                if (args.Property.Name == nameof(ComboBox.IsDropDownOpen) || args.Property.Name == nameof(ComboBox.SelectedItem))
                    ValidateControl(comboBox);
            };
        }
    }

    // Validate all controls inside the parent panel
    public static bool ValidateAll(Panel parent)
    {
        var boxes = parent.GetVisualDescendants()
            .OfType<TemplatedControl>()
            .Where(GetEnableValidation);
        var controls = parent.GetVisualDescendants()
            .OfType<TemplatedControl>()
            .Where(GetIsControl);
        
        var isGood = true;

        foreach (var box in boxes)
        {
            if (!ValidateControl(box))
            {
                isGood = false;
            }
        }
        
        var itemsControls = parent.GetVisualDescendants()
            .OfType<ItemsControl>()
            .Where(GetIsControl); // Use this to filter only the ItemsControls you want

        foreach (var ic in itemsControls)
        {
            foreach (var control in ic.Items.OfType<TemplatedControl>())
            {
                if (GetEnableValidation(control) && !ValidateControl(control))
                {
                    isGood = false;
                }
            }
        }


        return isGood;
    }

    // Perform validation on a single control
    
    private static bool ValidateControl(TemplatedControl control)
    {
        var isValid = control switch
        {
            TextBox textBox => !string.IsNullOrEmpty(textBox.Text),
            ComboBox comboBox => comboBox.IsDropDownOpen || comboBox.SelectedItem != null,
            _ => true
        };
        control.BorderBrush = isValid ? Brushes.Gray : Brushes.Red;
        return isValid;
    }
}
