using System;
using Avalonia.Controls;

namespace SquadGUI.Assets;

/// <summary>
/// Custom ComboBox that tracks the previous selection
/// </summary>
public class PpcpComboBox: ComboBox
{
    protected override Type StyleKeyOverride => typeof(ComboBox);
    private string _pastSelection = String.Empty;

    /// <summary>
    /// Gets or sets the previous selection value
    /// </summary>
    public string PastSelection
    {
        get => _pastSelection;
        set => _pastSelection = value;
    }
}