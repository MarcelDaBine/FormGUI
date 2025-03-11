using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Xaml.Interactions.Custom;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using SquadGUI.Assets;
using Avalonia.Media;

namespace SquadGUI.ViewModels;

public class DashboardViewModel : ViewModelBase
{
    private const int ChartItemsCount = 6;
    private Dictionary<FloatNumTextBox, double> ppSortedList;
    private Dictionary<FloatNumTextBox, double> cpDeSortedList;
    
    private ObservableCollection<FloatNumTextBox> _numVelNumVelTextBoxes;
    private ObservableCollection<PpcpComboBox> _comboBoxes;
    private ObservableCollection<Border> _textBlocks;
    private ObservableCollection<TextBlock> _usedBlocks;
    private ObservableCollection<NumTextBox> _loadNumTextBoxes;
    
    private ISeries[] _series;
    private Axis _yAxis;
    private readonly List<Axis> _axisList;
    private readonly LineSeries<double> _lineSeries;
    private bool _isPaneOpenAttribute;

    private string _meanValue;
    private string _debugChartMsg;
    
    //Button Commands
    public ReactiveCommand<Unit, Unit> ResetButtonCommand { get; }
    public ReactiveCommand<Unit, Unit> TogglePaneCommand { get; }

    //getters and setters
    public string MeanValue
    {
        get => _meanValue;
        set => this.RaiseAndSetIfChanged(ref _meanValue, value);
    }

    public string DebugChartMsg
    {
        get => _debugChartMsg;
        set => this.RaiseAndSetIfChanged(ref _debugChartMsg, value);
    }
    public bool IsPaneOpenAttribute 
    {
        get => _isPaneOpenAttribute;
        set => this.RaiseAndSetIfChanged(ref _isPaneOpenAttribute, value); 
    }
    
    public ObservableCollection<FloatNumTextBox> NumVelTextBoxes
    {
        get => _numVelNumVelTextBoxes;
        set => this.RaiseAndSetIfChanged(ref _numVelNumVelTextBoxes, value);
    }

    public ObservableCollection<PpcpComboBox> ComboBoxes
    {
        get => _comboBoxes;
        set => this.RaiseAndSetIfChanged(ref _comboBoxes, value);
    }

    public ObservableCollection<Border> TextBlocks
    {
        get => _textBlocks;
        set => this.RaiseAndSetIfChanged(ref _textBlocks, value);
    }

    public ObservableCollection<NumTextBox> LoadNumTextBoxes
    {
        get => _loadNumTextBoxes;
        set => this.RaiseAndSetIfChanged(ref _loadNumTextBoxes, value);
    }

    public ObservableCollection<TextBlock> UsedBlocks
    {
        get => _usedBlocks;
        set => this.RaiseAndSetIfChanged(ref _usedBlocks, value);
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

    //constructor
    public DashboardViewModel()
    {
        IsPaneOpenAttribute = false;
        
        _yAxis = new Axis()
        {
            MinLimit = 0,
            MaxLimit = 2700,
        };
        _axisList = new List<Axis> { _yAxis };

        _lineSeries = new LineSeries<double>
        {
            Stroke = new SolidColorPaint(SKColors.MediumPurple) { StrokeThickness = 6 },
            GeometrySize = 10,
            GeometryStroke = new SolidColorPaint(SKColors.MediumPurple),
            GeometryFill = new SolidColorPaint(SKColors.WhiteSmoke),
            LineSmoothness = 0,

            Fill = null,
        };
        
        Series = new ISeries[] { _lineSeries };
        
        ResetButtonCommand = ReactiveCommand.Create(ResetButton_OnClick);
        TogglePaneCommand = ReactiveCommand.Create(TogglePane);

        ppSortedList = new Dictionary<FloatNumTextBox, double>();
        cpDeSortedList = new Dictionary<FloatNumTextBox, double>();
        
        ChartInputFieldSetup();
        UpdateChart();
    }

    // <summary>
    // initializes the input fields
    // </summary>
    private void ChartInputFieldSetup()
    {
        //setup number input field
        NumVelTextBoxes = new ObservableCollection<FloatNumTextBox>();
        for (var i = 0; i < ChartItemsCount; ++i)
        {
            AddVelocityNumTextBox();
        }
        
        //setup the PP/CP combo boxes
        ComboBoxes = new ObservableCollection<PpcpComboBox>();

        for (var i = 0; i < ChartItemsCount; ++i)
        {
            AddComboBox();
        }
        
        //setup the shot number in the graph
        TextBlocks = new ObservableCollection<Border>();
        for (var i = 0; i < ChartItemsCount; ++i)
        {
            AddTextBlock();
        }
        
        LoadNumTextBoxes = new ObservableCollection<NumTextBox>();
        for (var i = 0; i < ChartItemsCount; i++)
        {
            AddLoadNumTextBox();
        }

        UsedBlocks = new ObservableCollection<TextBlock>();
        for (var i = 0; i < ChartItemsCount; i++)
        {
            AddUsedBlock();
        }
    }
    
    // <summary>
    // Updates the chart, the used column and calculates the V50 based on the value of the input fields. 
    // </summary
    private void UpdateChart()
    {
        if ((ComboBoxes.Count-1) % 2 != 0 && ComboBoxes.Count != 6)
        {
            _lineSeries.Values = new double[ChartItemsCount];
            DebugChartMsg = $"even number of shots needed";
            return;
        }
        
        foreach (var box in ComboBoxes)
        {
            var b = box.SelectedItem?.ToString();
            var c = NumVelTextBoxes[ComboBoxes.IndexOf(box)];
            if (b == "PP" && double.TryParse(c.Text, out var tempPP))
            {
                cpDeSortedList.Remove(c);
                if(!ppSortedList.ContainsKey(c)) // the warning is wrong, if you fix it and type in a box the program will crash
                    ppSortedList.Add(c, tempPP);
                else ppSortedList[c] = tempPP;
            }
            else if (b == "CP" && double.TryParse(c.Text, out var tempCP))
            {
                ppSortedList.Remove(c);
                if(!cpDeSortedList.ContainsKey(c)) // the warning is wrong, if you fix it and type in a box the program will crash
                    cpDeSortedList.Add(c, tempCP);
                else cpDeSortedList[c] = tempCP;
            }
        }

        if (ppSortedList.Count >= 3 && cpDeSortedList.Count >= 3)
        {
            var lowestCpList = cpDeSortedList.OrderBy(x => x.Value).ToList();
            var highestPpList = ppSortedList.OrderByDescending(x => x.Value).ToList();

            switch (double.Abs(highestPpList.First().Value - lowestCpList.First().Value))
            {
                case <= 40:
                    DebugChartMsg = $"ye";
                    MeanValue = CalculateV50Mean(lowestCpList.Take(3).ToList(), highestPpList.Take(3).ToList()).ToString();
                    break;
                case <= 50:
                    if (ppSortedList.Count <= 5 && cpDeSortedList.Count <= 5)
                    {
                        DebugChartMsg = $"%50m/s difference between the highest pp shot and lowest cp shot so {5 - ppSortedList.Count} pp shots are needed and {5 - cpDeSortedList.Count}cp shots are needed";
                        break;
                    }
                    MeanValue = CalculateV50Mean(lowestCpList.Take(5).ToList(), highestPpList.Take(5).ToList()).ToString();
                    break;
                case <= 60:
                    if (ppSortedList.Count <= 7 && cpDeSortedList.Count <= 7)
                    {
                        DebugChartMsg = $"60m/s difference between the highest pp shot and lowest cp shot so{7 - ppSortedList.Count} pp shots are needed and {7 - cpDeSortedList.Count}cp shots are needed";
                        break;
                    }
                    MeanValue = CalculateV50Mean(lowestCpList.Take(7).ToList(), highestPpList.Take(7).ToList()).ToString();
                    break;
                default:
                    DebugChartMsg = $"tf";
                    break;
            }
        }
        else
        {
            DebugChartMsg = $"no";
        }
        _lineSeries.Values = NumVelTextBoxes.Where((box) => box!=NumVelTextBoxes.Last())
            .Select(box => (double.TryParse(box.Text, out var result) ? result : 0))
            .ToArray();
    }

    private void ResetButton_OnClick()
    {
        for (var i = ChartItemsCount; i < NumVelTextBoxes.Count - 1; i++)
        { 
            NumVelTextBoxes.RemoveAt(ChartItemsCount - 1);
        }
        
        UpdateChart();
    }

    private void TogglePane()
    {
        IsPaneOpenAttribute = !IsPaneOpenAttribute;
    }
    
    // <summary>
    // Adds Buttons and does some checks to focus the next input field in the same column.
    // </summary>
    private void AddButtons(object? sender, EventArgs e)
    {
        switch (sender)
        {
            case NumTextBox numTextBox:
                if (numTextBox == LoadNumTextBoxes.Last())
                {
                    break;
                }
                if ((e as KeyEventArgs)?.Key == Key.Enter)
                {

                    LoadNumTextBoxes[LoadNumTextBoxes.IndexOf(numTextBox) + 1].Focus();
                }
                return;
            
            case ComboBox comboBox:
                if (comboBox == ComboBoxes.Last())
                {
                    break;
                }
                if ((e as SelectionChangedEventArgs)?.AddedItems.Count > 0)
                {
                    ComboBoxes[ComboBoxes.IndexOf(comboBox) + 1].Focus();
                }
                return;
            case FloatNumTextBox floatNumTextBox:
                if (floatNumTextBox == NumVelTextBoxes.Last())
                {
                    break;
                }
                if ((e as SelectionChangedEventArgs)?.AddedItems.Count > 0)
                {
                    NumVelTextBoxes[NumVelTextBoxes.IndexOf(floatNumTextBox) + 1].Focus();
                }
                return;
            case TextBox textBox:
                if (textBox == LoadNumTextBoxes.Last())
                {
                    break;
                }
                if ((e as KeyEventArgs)?.Key == Key.Enter)
                {
                    LoadNumTextBoxes[LoadNumTextBoxes.IndexOf(textBox) + 1].Focus();
                }

                return;
            default:
                return;
                
        }
        AddComboBox();
        AddVelocityNumTextBox();
        AddTextBlock();
        AddLoadNumTextBox();
        AddUsedBlock();
    }

    private void AddVelocityNumTextBox()
    {
        var numTextBox = new FloatNumTextBox();
        
        numTextBox.KeyDown += AddButtons;
        numTextBox.PropertyChanged += ((o, args) =>
        {
            if (args.Property != TextBox.TextProperty)
            {
                return;
            }
            UpdateChart();
        });
        NumVelTextBoxes.Add(numTextBox);
    }
    private void AddComboBox()
    {
        var box = new PpcpComboBox()
        {
            Width = 100,
            Height = 30,
            Margin = Avalonia.Thickness.Parse("10"),
            Items = { "PP", "CP", "N/A" }
        };
        
        box.PointerPressed += (sender, e) =>
        {
            if (sender is ComboBox cb)
            {
                cb.IsDropDownOpen = true;
                AddButtons(sender, e);
            }
        };
        box.SelectionChanged += (sender, e) =>
        {
            UpdateChart();
        };
        ComboBoxes.Add(box);
    }

    private void AddTextBlock()
    {
        var border = new Border
        {
            BorderThickness = new Avalonia.Thickness(2),
            BorderBrush = Brushes.LightGray, 
            Padding = new Avalonia.Thickness(1),
            Margin = new Avalonia.Thickness(0,10,0,10),
            Width = 100,
            Height = 30, 
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
            Child = new TextBlock
            {
                Margin = new Avalonia.Thickness(90,10,0,0),
                Width = 100,
                Height = 30,
                Text = (TextBlocks.Count + 1).ToString(),
            }
        };
        TextBlocks.Add(border);
    }

    private void AddLoadNumTextBox()
    {
        var textBox = new NumTextBox(); 
        textBox.KeyDown += AddButtons;
        LoadNumTextBoxes.Add(textBox);
    }
    
    private void AddUsedBlock()
    { 
        var textBox = new TextBlock()
        {
            Margin = new Avalonia.Thickness(90,10,0,0),
            Text = "",
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        UsedBlocks.Add(textBox);
    }

    private void UpdateUsedBlock()
    {
        for (var i = 0; i < NumVelTextBoxes.Count; ++i)
        {
            if (ppSortedList.ContainsKey(NumVelTextBoxes[i]) || cpDeSortedList.ContainsKey(NumVelTextBoxes[i]))
            {
                UsedBlocks[i].Text = "Y";
            }
            else
            {
                UsedBlocks[i].Text = "N";
            }
        }
    }
    //TO DO: fix when deleting from the box the avg is not updated acordingly
    // <summary>
    // Calculates the V50Mean based on the values of the cp and pc lists. It also updates the text of the Used column.
    // </summary>
    // <param name="pp">The first N (size based on the rules of the V50) NumVelTextBoxes from the pp list and their values.
    // <param name="cp">The first N (size based on the rules of the V50) NumVelTextBoxes from the cp list and their values.
    // <returns> A double containing the 
    private double CalculateV50Mean(List<KeyValuePair<FloatNumTextBox,double>> cp, List<KeyValuePair<FloatNumTextBox,double>> pp)
    {
        double sum = 0;
        for (var i = 0; i < cp.Count; ++i)
        {
            sum += cp[i].Value;
            sum += pp[i].Value;
        }
        UpdateUsedBlock();
        return sum / (pp.Count + cp.Count);
    }
}    //}