using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using DynamicData;
using ReactiveUI;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using SquadGUI.Assets;
using Avalonia.Media;

namespace SquadGUI.ViewModels;

public class DashboardViewModel : ViewModelBase
{
    private const int ChartItemsCount = 6;
    private Dictionary<FloatNumTextBox, double> _ppList;
    private Dictionary<FloatNumTextBox, double> _cpList;

    private ObservableCollection<FloatNumTextBox> _numVelNumVelTextBoxes;
    private ObservableCollection<CustomComboBoxImplement> _pcpComboBoxes;
    private ObservableCollection<Border> _textBlocks;
    private ObservableCollection<Border> _usedBlocks;
    private ObservableCollection<NumTextBox> _loadNumTextBoxes;
    private ObservableCollection<ComboBox> _posComboBoxes;
    private ObservableCollection<TextBox> _msTextBoxes;
    private ObservableCollection<Button> _deleteButtons;

    private ISeries[] _series;
    private readonly List<Axis> _axisList;
    private readonly LineSeries<double> _lineSeries;
    private readonly LineSeries<double> _v50Series;
    private readonly LineSeries<double> _v50MinSeries;
    private bool _isPaneOpenAttribute;

    private string _meanValueMs;
    private string _meanValueFt;
    private string _debugChartMsg;
    private string _highPartialMs;
    private string _highPartialFt;
    private string _lowCompleteMs;
    private string _lowCompleteFt;
    private string _mixedResultsFt;
    private string _mixedResultsMs;
    private string _gapMs;
    private string _gapFt;
    private string _rangeResultsFt;
    private string _rangeResultsMs;
    private string _v50MinFt;
    private string _v50MinMs;
    private string _deltaVMs;
    private string _deltaVFt;
    private string _percentageFt;
    private string _percentageMs;
    private string _expandedUncertainty;
    private string _decisionRule;
    private string _fahrenheitText;
    private string _celsiusText;
    private string _humidityText;
    private string _selectedProjectile;
    private string _selectedPowder;
    private string _selectedBarrel;

    private DateTimeOffset? _formDate = DateTimeOffset.Now;


    //constructor
    public DashboardViewModel()
    {
        IsPaneOpenAttribute = false;

        var yAxis = new Axis()
        {
            MinLimit = 0,
            MaxLimit = 2700,
        };
        _axisList = new List<Axis> { yAxis };

        _lineSeries = new LineSeries<double>
        {
            Stroke = new SolidColorPaint(SKColors.MediumPurple) { StrokeThickness = 6 },
            GeometrySize = 10,
            GeometryStroke = new SolidColorPaint(SKColors.MediumPurple),
            GeometryFill = new SolidColorPaint(SKColors.WhiteSmoke),
            LineSmoothness = 0,
            Name = "Shot",

            Fill = null,
        };
        _v50Series = new LineSeries<double>()
        {
            Stroke = new SolidColorPaint(SKColors.Purple) { StrokeThickness = 6 },
            GeometrySize = 0,
            GeometryStroke = new SolidColorPaint(SKColors.Purple) { StrokeThickness = 0 },
            Fill = null,
            Name = "V50"
        };
        _v50MinSeries = new LineSeries<double>()
        {
            Stroke = new SolidColorPaint(SKColors.Indigo) { StrokeThickness = 6 },
            GeometrySize = 0,
            GeometryStroke = new SolidColorPaint(SKColors.Indigo) { StrokeThickness = 0 },
            Fill = null,
            Name = "V50Min"
        };

        Series = new ISeries[] { _lineSeries, _v50Series, _v50MinSeries };

        TogglePaneCommand = ReactiveCommand.Create(TogglePane);

        _ppList = new Dictionary<FloatNumTextBox, double>();
        _cpList = new Dictionary<FloatNumTextBox, double>();

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
        PcpComboBoxes = new ObservableCollection<CustomComboBoxImplement>();
        TextBlocks = new ObservableCollection<Border>();
        LoadNumTextBoxes = new ObservableCollection<NumTextBox>();
        PosComboBoxes = new ObservableCollection<ComboBox>();
        UsedBlocks = new ObservableCollection<Border>();
        MsTextBoxes = new ObservableCollection<TextBox>();
        DeleteButtons = new ObservableCollection<Button>();

        for (var i = 0; i < ChartItemsCount + 1; ++i) // +1 bc of the empty box
        {
            AddVelocityNumTextBox();
            AddPcpComboBox();
            AddTextBlock();
            AddLoadNumTextBox();
            AddUsedBlock();
            AddPosComboBox();
            AddMsTextBox();
            AddDeleteButton();
        }
    }

    // <summary>
    // Updates the chart, the used column and calculates the V50 based on the value of the input fields. 
    // </summary
    private void UpdateChart()
    {
        if ((PcpComboBoxes.Count - 1) % 2 != 0 && PcpComboBoxes.Count != 6)
        {
            _lineSeries.Values = new double[ChartItemsCount];
            DebugChartMsg = $"even number of shots needed";
            return;
        }

        foreach (var box in PcpComboBoxes)
        {
            var b = box.SelectedItem?.ToString();
            var c = NumVelTextBoxes[PcpComboBoxes.IndexOf(box)];
            if (b == "PP" && double.TryParse(c.Text, out var tempPP))
            {
                _cpList.Remove(c);
                if (!_ppList.ContainsKey(
                        c)) // the warning is wrong, if you fix it and type in a box the program will crash
                    _ppList.Add(c, tempPP);
                else _ppList[c] = tempPP;
            }
            else if (b == "CP" && double.TryParse(c.Text, out var tempCP))
            {
                _ppList.Remove(c);
                if (!_cpList.ContainsKey(
                        c)) // the warning is wrong, if you fix it and type in a box the program will crash
                    _cpList.Add(c, tempCP);
                else _cpList[c] = tempCP;
            }
            else
            {
                _ppList.Remove(c);
                _cpList.Remove(c);
            }
        }

        if (_ppList.Count >= 3 && _cpList.Count >= 3)
        {
            var count = 3;
            var lowestCpList = _cpList.OrderBy(x => x.Value).ToList();
            var highestPpList = _ppList.OrderByDescending(x => x.Value).ToList();

            switch (DifferenceBetweenLists(lowestCpList.Take(count).ToList(), highestPpList.Take(count).ToList()))
            {
                case <= 40:
                    DebugChartMsg = $"Worked";
                    MeanValueMs =
                        CalculateV50Mean(lowestCpList.Take(count).ToList(), highestPpList.Take(count).ToList())
                            .ToString("F3");
                    break;
                case <= 50:
                    if (_ppList.Count <= 5 && _cpList.Count <= 5)
                    {
                        DebugChartMsg =
                            $"%50m/s difference between the 3 highest pp shots and 3 lowest cp shot so {5 - _ppList.Count} pp shots are needed and {5 - _cpList.Count} cp shots are needed";
                        _v50Series.Values = null;
                        break;
                    }

                    count = 5;

                    DebugChartMsg = "Worked";
                    MeanValueMs =
                        CalculateV50Mean(lowestCpList.Take(count).ToList(), highestPpList.Take(count).ToList())
                            .ToString("F3");
                    break;
                case <= 60:
                    if (_ppList.Count <= 7 && _cpList.Count <= 7)
                    {
                        DebugChartMsg =
                            $"60m/s difference between the highest pp shot and lowest cp shot so {7 - _ppList.Count} pp shots are needed and {7 - _cpList.Count} cp shots are needed";
                        _v50Series.Values = null;
                        break;
                    }

                    count = 7;

                    DebugChartMsg = "Worked";
                    MeanValueMs =
                        CalculateV50Mean(lowestCpList.Take(count).ToList(), highestPpList.Take(count).ToList())
                            .ToString("F3");
                    break;
                default:
                    DebugChartMsg = $"How Did We Get Here...";
                    MeanValueMs = "";
                    _v50Series.Values = null;
                    break;
            }
        }
        else
        {
            DebugChartMsg = $"At least 3 pp and 3 cp shots are needed";
            _v50Series.Values = null;
            MeanValueMs = "";
        }

        _lineSeries.Values = NumVelTextBoxes.Where((box) => box != NumVelTextBoxes.Last())
            .Select(box => (double.TryParse(box.Text, out var result) ? result : 0))
            .ToArray();

        if (double.TryParse(V50MinFt, out double result))
        {
            _v50MinSeries.Values = Enumerable
                .Repeat(result, NumVelTextBoxes.Count - 1).ToArray();
        }
        else
        {
            _v50MinSeries.Values = null;
        }
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

        AddPcpComboBox();
        AddVelocityNumTextBox();
        AddTextBlock();
        AddLoadNumTextBox();
        AddUsedBlock();
        AddPosComboBox();
        AddMsTextBox();
        AddDeleteButton();
    }

    private void AddVelocityNumTextBox()
    {
        var numTextBox = new FloatNumTextBox()
        {
            Opacity = 0.3
        };

        numTextBox.KeyDown += AddButtons;
        numTextBox.PropertyChanged += ((o, args) =>
        {

            if (args.Property != TextBox.TextProperty)
            {
                return;
            }

            if (double.TryParse(numTextBox.Text, out var feet))
            {
                var meters = ConvertFeetToMeters(feet);
                MsTextBoxes[NumVelTextBoxes.IndexOf(numTextBox)].Text = meters.ToString("F3");
            }
            else
            {
                MsTextBoxes[NumVelTextBoxes.IndexOf(numTextBox)].Text = "";
            }

            UpdateChart();
        });
        if (NumVelTextBoxes.Count > 0)
        {
            NumVelTextBoxes.Last().Opacity = 1;
        }

        NumVelTextBoxes.Add(numTextBox);
    }

    private void AddPcpComboBox()
    {
        var box = new CustomComboBoxImplement()
        {
            Width = 100,
            Height = 30,
            Margin = Thickness.Parse("10"),
            Items = { "PP", "CP", "N/A" },
            Opacity = 0.3
        };

        box.PointerPressed += (sender, e) =>
        {
            if (sender is ComboBox cb)
            {
                cb.IsDropDownOpen = true;
                AddButtons(sender, e);
            }
        };
        box.SelectionChanged += (sender, e) => { UpdateChart(); };
        if (PcpComboBoxes.Count > 0)
        {
            PcpComboBoxes.Last().Opacity = 1;
        }

        PcpComboBoxes.Add(box);
    }

    private void AddTextBlock()
    {
        var border = new Border
        {
            Opacity = 0.3,
            Child = new TextBlock
            {
                Text = (TextBlocks.Count + 1).ToString(),
                Opacity = 0.3
            }
        };
        border.Classes.Add("TextBlockBorder");
        if (TextBlocks.Count > 0)
        {
            TextBlocks.Last().Opacity = 1;
            TextBlocks.Last().Child.Opacity = 1;
        }

        TextBlocks.Add(border);
    }

    private void AddPosComboBox()
    {
        var posComboBox = new ComboBox()
        {
            Width = 100,
            Height = 30,
            Margin = Thickness.Parse("10"),
            Items = { "Crown", "Back", "Left", "Right", "Front" },
            Opacity = 0.3
        };
        posComboBox.PointerPressed += (sender, e) =>
        {
            if (sender is ComboBox cb)
            {
                cb.IsDropDownOpen = true;
                AddButtons(sender, e);
            }
        };
        if (PosComboBoxes.Count > 0)
        {
            PosComboBoxes.Last().Opacity = 1;
        }

        PosComboBoxes.Add(posComboBox);
    }

    private void AddLoadNumTextBox()
    {
        var textBox = new NumTextBox()
        {
            Opacity = 0.3
        };
        textBox.KeyDown += AddButtons;
        if (LoadNumTextBoxes.Count > 0)
        {
            LoadNumTextBoxes.Last().Opacity = 1;
        }

        LoadNumTextBoxes.Add(textBox);
    }

    private void AddUsedBlock()
    {
        var border = new Border()
        {
            Opacity = 0.3,
            Child = new TextBlock()
            {
                Text = "",
                Opacity = 0.3
            }
        };
        border.Classes.Add("TextBlockBorder");
        if (UsedBlocks.Count > 0)
        {
            UsedBlocks.Last().Opacity = 1;
            UsedBlocks.Last().Child.Opacity = 1;
        }

        UsedBlocks.Add(border);
    }

    private void AddMsTextBox()
    {
        var textBox = new TextBox()
        {
            IsEnabled = false,
            Watermark = "",
            Opacity = 0.3,
        };
        if (MsTextBoxes.Count > 0)
        {
            MsTextBoxes.Last().Opacity = 1;
        }

        MsTextBoxes.Add(textBox);
    }

    private void AddDeleteButton()
    {
        var pathIcon = new PathIcon()
        {
            Data = (Geometry)Application.Current.FindResource("CloseRegular"),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        var button = new Button()
        {
            Content = pathIcon,
            Opacity = 0.3,
            IsEnabled = false
        };

        button.Click += (sender, e) =>
        {
            var index = DeleteButtons.IndexOf(button);
            if (DeleteButtons.Count - 1 < ChartItemsCount + 1)
            {
                NumVelTextBoxes[index].Text = string.Empty;
                PcpComboBoxes[index].SelectedIndex = 10;
                LoadNumTextBoxes[index].Text = string.Empty;
                PosComboBoxes[index].SelectedIndex = 10;
                return;
            }

            NumVelTextBoxes.RemoveAt(index);
            PcpComboBoxes.RemoveAt(index);
            TextBlocks.RemoveAt(index);
            LoadNumTextBoxes.RemoveAt(index);
            PosComboBoxes.RemoveAt(index);
            UsedBlocks.RemoveAt(index);
            MsTextBoxes.RemoveAt(index);
            DeleteButtons.RemoveAt(index);
            UpdateChart();

            for (var i = index; i < TextBlocks.Count; i++)
            {
                ((TextBlock)TextBlocks[i].Child).Text = (i + 1).ToString();
            }
        };
        if (DeleteButtons.Count > 0)
        {
            DeleteButtons.Last().Opacity = 1;
            DeleteButtons.Last().IsEnabled = true;
        }

        DeleteButtons.Add(button);
    }

    private void UpdateUsedBlock(List<KeyValuePair<FloatNumTextBox, double>> cp,
        List<KeyValuePair<FloatNumTextBox, double>> pp)
    {
        for (var i = 0; i < NumVelTextBoxes.Count - 1; ++i)
        {
            if (pp.Any((pair) => { return pair.Key == NumVelTextBoxes[i]; }) || cp.Any((pair) =>
                {
                    return pair.Key == NumVelTextBoxes[i];
                }))
            {
                ((TextBlock)UsedBlocks[i].Child).Text = "Y";
            }
            else
            {
                ((TextBlock)UsedBlocks[i].Child).Text = "N";
            }
        }
    }

    //TO DO: fix when deleting from the box the avg is not updated acordingly
    // <summary>
    // Calculates the V50Mean based on the values of the cp and pc lists. It also updates the text of the Used column.
    // </summary>
    // <param name="size"> The first N (size based on the rules of the V50) numbers. 
    // <param name="pp"> The first N NumVelTextBoxes from the pp list and their values.
    // <param name="cp"> The first N NumVelTextBoxes from the cp list and their values.
    // <returns> A double containing the 
    private double CalculateV50Mean(List<KeyValuePair<FloatNumTextBox, double>> cp,
        List<KeyValuePair<FloatNumTextBox, double>> pp)
    {
        double sum = 0;
        for (var i = 0; i < cp.Count; ++i)
        {
            sum += cp[i].Value;
            sum += pp[i].Value;
        }

        UpdateUsedBlock(cp, pp);
        HighPartialMs = ConvertFeetToMeters(pp.First().Value).ToString("F3");
        HighPartialFt = pp.First().Value.ToString("F3");
        LowCompleteMs = ConvertFeetToMeters(cp.First().Value).ToString("F3");
        LowCompleteFt = cp.First().Value.ToString("F3");

        if (pp[0].Value - cp[0].Value >= 0)
        {
            MixedResultsFt = (pp[0].Value - cp[0].Value).ToString("f3");
            MixedResultsMs = ConvertFeetToMeters(pp[0].Value - cp[0].Value).ToString("f3");
            GapMs = "";
            GapFt = "";
        }
        else
        {
            MixedResultsFt = "";
            MixedResultsMs = "";
            GapMs = (cp[0].Value - pp[0].Value).ToString("f3");
            GapFt = ConvertFeetToMeters(cp[0].Value - pp[0].Value).ToString("f3");
        }

        var V50Ft = sum / (pp.Count + cp.Count);
        var V50Ms = ConvertFeetToMeters(V50Ft);

        _v50Series.Values = Enumerable.Repeat(V50Ft, NumVelTextBoxes.Count - 1).ToArray();

        if (!double.TryParse(V50MinMs, out double V50MinMsValue))
        {
            return V50Ms;
        }

        var deltaMs = V50Ms - V50MinMsValue;
        DeltaVMs = (V50Ms - double.Parse(V50MinMs)).ToString("F3");
        DeltaVFt = (V50Ft - double.Parse(V50MinFt)).ToString("F3");

        var percentageMs = deltaMs / V50MinMsValue;
        PercentageMs = percentageMs.ToString("F2") + "%";
        PercentageFt = PercentageMs;
        return V50Ms;
    }

    private static double DifferenceBetweenLists(List<KeyValuePair<FloatNumTextBox, double>> cp,
        List<KeyValuePair<FloatNumTextBox, double>> pp)
    {
        double max = 0, temp;
        for (int i = 0; i < cp.Count; i++)
        {
            temp = ConvertFeetToMeters(double.Abs(cp[i].Value - pp[i].Value));
            if (temp > max)
                max = temp;
        }

        return max;
    }

    private static double ConvertFeetToMeters(double feet)
    {
        return feet * 0.3048;
    }

    private static string ConvertFeetToMetersString(string feet)
    {
        if (double.TryParse(feet, out double feetValue))
        {
            return (feetValue * 0.3048).ToString("F3");
        }

        return feet;
    }

    private static double ConvertMetersToFeet(double meters)
    {
        return meters / 0.3048;
    }

    private static string ConvertMetersToFeetString(string feet)
    {
        if (double.TryParse(feet, out double feetValue))
        {
            return (feetValue / 0.3048).ToString("F3");
        }

        return feet;
    }

    private string DateFormater(string input)
    {
        if (DateTime.TryParse(input, out DateTime date))
        {
            return date.ToString("dd-MM-yyyy");
        }

        return input;
    }

    private static string FahrenheitToCelsiusString(string fahrenheit)
    {
        if (fahrenheit.EndsWith("\u00b0F"))
        {
            return int.TryParse(fahrenheit.AsSpan(0, fahrenheit.Length - 3), out var value)
                ? ((value - 32) * 5 / 9).ToString("N0")
                : "";
        }
        else
        {
            return int.TryParse(fahrenheit, out var value)
                ? ((value - 32) * 5 / 9).ToString("N0")
                : "";
        }
    }

    private static string CelsiusToFahrenheitString(string celsius)
    {
        if (celsius.EndsWith("\u00b0C"))
        {
            return int.TryParse(celsius.AsSpan(0, celsius.Length - 3), out var value)
                ? (value * 9 / 5 + 32).ToString("N0")
                : "";
        }
        else
        {
            return int.TryParse(celsius, out var value)
                ? (value * 9 / 5 + 32).ToString("N0")
                : "";
        }
    }

    //Button Commands
    public ReactiveCommand<Unit, Unit> TogglePaneCommand { get; }

    //getters and setters

    public DateTimeOffset? FormDate
    {
        get => _formDate;
        set => this.RaiseAndSetIfChanged(ref _formDate, value);
    }

    public string MeanValueMs
    {
        get => _meanValueMs;
        set
        {
            this.RaiseAndSetIfChanged(ref _meanValueMs, value);
            Console.WriteLine(value);
            MeanValueFt = ConvertMetersToFeetString(value);
        }
    }

    public string MeanValueFt
    {
        get => _meanValueFt;
        set => this.RaiseAndSetIfChanged(ref _meanValueFt, value);
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

    public ObservableCollection<ComboBox> PosComboBoxes
    {
        get => _posComboBoxes;
        set => this.RaiseAndSetIfChanged(ref _posComboBoxes, value);
    }

    public ObservableCollection<FloatNumTextBox> NumVelTextBoxes
    {
        get => _numVelNumVelTextBoxes;
        set => this.RaiseAndSetIfChanged(ref _numVelNumVelTextBoxes, value);
    }

    public ObservableCollection<CustomComboBoxImplement> PcpComboBoxes
    {
        get => _pcpComboBoxes;
        set => this.RaiseAndSetIfChanged(ref _pcpComboBoxes, value);
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

    public ObservableCollection<Border> UsedBlocks
    {
        get => _usedBlocks;
        set => this.RaiseAndSetIfChanged(ref _usedBlocks, value);
    }

    public ISeries[] Series
    {
        get => _series;
        set => this.RaiseAndSetIfChanged(ref _series, value);
    }

    public ObservableCollection<TextBox> MsTextBoxes
    {
        get => _msTextBoxes;
        set => this.RaiseAndSetIfChanged(ref _msTextBoxes, value);
    }

    public ObservableCollection<Button> DeleteButtons
    {
        get => _deleteButtons;
        set => this.RaiseAndSetIfChanged(ref _deleteButtons, value);
    }

    public List<Axis> AxisList
    {
        get => _axisList;
    }

    //V50 Summary accesors and setters
    public string HighPartialMs
    {
        get => _highPartialMs;
        set => this.RaiseAndSetIfChanged(ref _highPartialMs, value);
    }

    public string HighPartialFt
    {
        get => _highPartialFt;
        set => this.RaiseAndSetIfChanged(ref _highPartialFt, value);
    }

    public string LowCompleteMs
    {
        get => _lowCompleteMs;
        set => this.RaiseAndSetIfChanged(ref _lowCompleteMs, value);
    }

    public string LowCompleteFt
    {
        get => _lowCompleteFt;
        set => this.RaiseAndSetIfChanged(ref _lowCompleteFt, value);
    }

    public string MixedResultsFt
    {
        get => _mixedResultsFt;
        set => this.RaiseAndSetIfChanged(ref _mixedResultsFt, value);
    }

    public string MixedResultsMs
    {
        get => _mixedResultsMs;
        set => this.RaiseAndSetIfChanged(ref _mixedResultsMs, value);
    }

    public string GapMs
    {
        get => _gapMs;
        set => this.RaiseAndSetIfChanged(ref _gapMs, value);
    }

    public string GapFt
    {
        get => _gapFt;
        set => this.RaiseAndSetIfChanged(ref _gapFt, value);
    }

    public string RangeResultsFt
    {
        get => _rangeResultsFt;
        set => this.RaiseAndSetIfChanged(ref _rangeResultsFt, value);
    }

    public string RangeResultsMs
    {
        get => _rangeResultsMs;
        set => this.RaiseAndSetIfChanged(ref _rangeResultsMs, value);
    }

    public string V50MinFt
    {
        get => _v50MinFt;
        set
        {
            this.RaiseAndSetIfChanged(ref _v50MinFt, value);
            this.RaiseAndSetIfChanged(ref _v50MinMs, ConvertFeetToMetersString(value));
            this.RaisePropertyChanged(nameof(V50MinMs));
            UpdateChart();
            if (double.TryParse(value, out var feetValue))
            {
                _v50MinSeries.Values = Enumerable
                    .Repeat(feetValue, NumVelTextBoxes.Count - 1).ToArray();
                return;
            }

            _v50MinSeries.Values = null;

        }
    }

    public string V50MinMs
    {
        get => _v50MinMs;
        set
        {
            this.RaiseAndSetIfChanged(ref _v50MinMs, value);
            this.RaiseAndSetIfChanged(ref _v50MinFt, ConvertMetersToFeetString(value));
            _v50MinSeries.Values =
                Enumerable.Repeat(ConvertMetersToFeetString(value), NumVelTextBoxes.Count - 1).ToArray();
            this.RaisePropertyChanged(nameof(V50MinFt));
            UpdateChart();
            if (double.TryParse(value, out var meters))
            {
                _v50MinSeries.Values = Enumerable
                    .Repeat(ConvertMetersToFeet(meters), NumVelTextBoxes.Count - 1).ToArray();
                return;
            }

            _v50MinSeries.Values = null;
        }
    }

    public string DeltaVMs
    {
        get => _deltaVMs;
        set => this.RaiseAndSetIfChanged(ref _deltaVMs, value);
    }

    public string DeltaVFt
    {
        get => _deltaVFt;
        set => this.RaiseAndSetIfChanged(ref _deltaVFt, value);
    }

    public string PercentageFt
    {
        get => _percentageFt;
        set => this.RaiseAndSetIfChanged(ref _percentageFt, value);
    }

    public string PercentageMs
    {
        get => _percentageMs;
        set => this.RaiseAndSetIfChanged(ref _percentageMs, value);
    }

    public string ExpandedUncertainty
    {
        get => _expandedUncertainty;
        set => this.RaiseAndSetIfChanged(ref _expandedUncertainty, value);
    }

    public string DecisionRule
    {
        get => _decisionRule;
        set => this.RaiseAndSetIfChanged(ref _decisionRule, value);
    }

    public string CelsiusText
    {
        get => _celsiusText;
        set
        {
            this.RaiseAndSetIfChanged(ref _celsiusText, value);
            if (!string.IsNullOrEmpty(value))
            {
                this.RaiseAndSetIfChanged(ref _fahrenheitText, CelsiusToFahrenheitString(value) + " \u00b0F");
            }
            else
            {
                this.RaiseAndSetIfChanged(ref _fahrenheitText, CelsiusToFahrenheitString(value));
            }

            this.RaisePropertyChanged(nameof(FahrenheitText));
        }
    }

    public string FahrenheitText
    {
        get => _fahrenheitText;
        set
        {
            this.RaiseAndSetIfChanged(ref _fahrenheitText, value);
            if (!string.IsNullOrEmpty(value))
            {
                this.RaiseAndSetIfChanged(ref _celsiusText, FahrenheitToCelsiusString(value) + " \u00b0C");
            }
            else
            {
                this.RaiseAndSetIfChanged(ref _celsiusText, FahrenheitToCelsiusString(value));
            }

            this.RaisePropertyChanged(nameof(CelsiusText));
        }
    }

    public string HumidityText
    {
        get => _humidityText;
        set => this.RaiseAndSetIfChanged(ref _humidityText, value);
    }

    public string SelectedProjectile
    {
        get => _selectedProjectile;
        set => this.RaiseAndSetIfChanged(ref _selectedProjectile, value);
    }

    public string SelectedPowder
    {
        get => _selectedPowder;
        set => this.RaiseAndSetIfChanged(ref _selectedPowder, value);
    }

    public string SelectedBarrel
    {
        get => _selectedBarrel;
        set => this.RaiseAndSetIfChanged(ref _selectedBarrel, value);
    }
}
    