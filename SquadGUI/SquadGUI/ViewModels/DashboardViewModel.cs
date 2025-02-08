using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactions.Custom;
using DynamicData;
using ReactiveUI;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using SquadGUI.Assets;

namespace SquadGUI.ViewModels;

public class DashboardViewModel : ViewModelBase
{
    private int _numBoxCount, _tailIndex;
    private readonly int _originalNumBoxCount, _originalTailIndex;
    
    private ObservableCollection<NumTextBox> _numNumTextBoxes;
    private ISeries[] _series;
    private Axis _yAxis;
    private List<Axis> _axisList;
    
    public int NumBoxCount
    {
        get => _numBoxCount;
        private set
        {
            _numBoxCount = value;
            _tailIndex = value - 1;
        }
    }

    public int OriginalNumBoxCount
    {
        get => _originalNumBoxCount;
    }
    

    public ObservableCollection<NumTextBox> NumTextBoxes
    {
        get => _numNumTextBoxes;
        set => this.RaiseAndSetIfChanged(ref _numNumTextBoxes, value);
    }
    
    public ISeries[] Series
    {
        get => _series;
        set => this.RaiseAndSetIfChanged(ref _series, value);
    }

    public List<Axis> AxisList
    {
        get => _axisList;
    }
    public ReactiveCommand<Unit, Unit> ResetButtonCommand { get; }

    public DashboardViewModel()
    {
        _yAxis = new Axis()
        {
            MinLimit = 0,
            MaxLimit = 1250,
        };
        _axisList = new List<Axis> { _yAxis };
        
        ResetButtonCommand = ReactiveCommand.Create(ResetButton_OnClick);
        
        NumTextBoxesSetup();
        
        _originalNumBoxCount = NumTextBoxes.Count;
        _originalTailIndex = _originalNumBoxCount - 1;
        NumBoxCount = OriginalNumBoxCount;
        
        UpdateSeries();
    }

    private void NumTextBoxesSetup()
    {
        NumTextBoxes = new ObservableCollection<NumTextBox>()
        {
            new NumTextBox() { Name = "NumTextBox0" },
            new NumTextBox() { Name = "NumTextBox1" },
            new NumTextBox() { Name = "NumTextBox2" },
            new NumTextBox() { Name = "NumTextBox3" },
            new NumTextBox() { Name = "NumTextBox4" },
            new NumTextBox() { Name = "NumTextBox5" },
        };
        
        foreach (var numTextBox in NumTextBoxes)
        {
            numTextBox.KeyDown += (s, args) => AddTextBox(s, args);
        }

        this.WhenAnyValue(x => x.NumTextBoxes).Subscribe( _ =>
        {
            foreach (var numTextBox in NumTextBoxes)
            {
                numTextBox.PropertyChanged += (sender, args) =>
                {
                    if (args.Property == NumTextBox.TextProperty)
                    {
                        UpdateSeries();
                    }
                };
            }
        });
    }
    
    private void UpdateSeries()
    {
        Series = new ISeries[]
        {
            new LineSeries<double>
            {
                Values = NumTextBoxes.Where((box, index) => index != _tailIndex)
                    .Select(box => (double.TryParse(box.Text, out var result)? result : 0))
                    .ToArray(),
                Stroke = new SolidColorPaint(SKColors.MediumPurple){ StrokeThickness = 6 },
                GeometrySize = 10,
                GeometryStroke = new SolidColorPaint(SKColors.MediumPurple),
                GeometryFill = new SolidColorPaint(SKColors.WhiteSmoke),
                LineSmoothness = 0,
                Fill = null,
            },
        };
    }
    
    private void AddTextBox(object sender, KeyEventArgs e)
    {
        if (sender as NumTextBox != NumTextBoxes[_tailIndex])
        {
            if (e.Key == Key.Enter)
            {
                NumTextBoxes[NumTextBoxes.IndexOf(sender as NumTextBox) + 1].Focus();
            }
            
            return;
        }
        
        ++NumBoxCount;
        
        var numTextBox = new NumTextBox()
        {
            Name = $"NumTextBox{_tailIndex}"
        };
        numTextBox.KeyDown += ((s, e) => AddTextBox(s, e));
        numTextBox.PropertyChanged += ((o, args) =>
        {
            if (args.Property == NumTextBox.TextProperty)
            { 
                UpdateSeries();
            }
        });
        
        NumTextBoxes.Add(numTextBox);
    }

    private void ResetButton_OnClick()
    {
        for (int i = OriginalNumBoxCount; i < NumBoxCount; i++)
        { 
            NumTextBoxes.RemoveAt(_originalTailIndex);
        }
        
        NumBoxCount = NumTextBoxes.Count; 
        
        UpdateSeries();
    }
}