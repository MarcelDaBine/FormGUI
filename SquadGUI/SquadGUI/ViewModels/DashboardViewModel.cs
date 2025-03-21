using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using DynamicData;
using ReactiveUI;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using SquadGUI.Assets;
using Avalonia.Media;
using LiveChartsCore.Measure;

namespace SquadGUI.ViewModels;

public class DashboardViewModel : ViewModelBase
{
    private const int ChartItemsCount = 6;
    private Dictionary<FloatNumTextBox, double> _ppList;
    private Dictionary<FloatNumTextBox, double> _cpList;
    private Dictionary<ComboBox, string> _selectedItems;

    private ObservableCollection<FloatNumTextBox> _numVelNumVelTextBoxes;
    private ObservableCollection<ComboBox> _pcpComboBoxes;
    private ObservableCollection<Border> _textBlocks;
    private ObservableCollection<Border> _usedBlocks;
    private ObservableCollection<NumTextBox> _loadNumTextBoxes;
    private ObservableCollection<ComboBox> _posComboBoxes;
    private ObservableCollection<TextBox> _msTextBoxes;
    private ObservableCollection<Button> _deleteButtons;
    private ObservableCollection<TextBox> _trackIdTextBoxes;
    private ObservableCollection<Button> _notesTextBoxes;
    private ObservableCollection<TextBox> _sampleInfoBoxes;
    private ObservableCollection<Border> _sampleInfoBoxesIndexes;
    private ObservableCollection<Button> _sampleDeleteButtons;
    private ObservableCollection<TextBox> _standardsTextBoxes;
    private ObservableCollection<Border> _standardsTextBoxesIndexes;
    private ObservableCollection<Button> _standardsDeleteButtons;

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
    private string _selectedModel;
    private string _selectedSize;
    private string _selectedMass;
    private string _selectedCondition;
    private string _massGrams;
    private string _massPounds;
    private string _selectedRangeConfig = "Doppler radar";
    private string _notes;
    private string _rangeConfigText;
    private string _velText; 
    private string _trackIdText;
    private string _sampleNumberText;
    private string _reportNumberText;

    private DateTimeOffset? _formDate = DateTimeOffset.Now;
    private TimeSpan? _formTime = TimeSpan.Parse(DateTime.Now.ToString("HH:mm:ss"));

    public ReactiveCommand<object, Unit> ComboPointerPressed { get; }
    public ReactiveCommand<Grid,Unit> ValidateAllCommand { get; }


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

        ValidateAllCommand = ReactiveCommand.Create<Grid>(ValidateForm);

        Series = new ISeries[] { _lineSeries, _v50Series, _v50MinSeries };

        TogglePaneCommand = ReactiveCommand.Create(TogglePane);

        _ppList = new Dictionary<FloatNumTextBox, double>();
        _cpList = new Dictionary<FloatNumTextBox, double>();


        SampleInfoBoxes = new ObservableCollection<TextBox>();
        SampleInfoBoxesIndexes = new ObservableCollection<Border>();
        SampleDeleteButtons = new ObservableCollection<Button>();
        AddSampleInfoBox();
        AddSampleInfoIndexBlock();
        AddSampleDeleteButton();

        StandardsDeleteButtons = new ObservableCollection<Button>();
        StandardsTextBoxes = new ObservableCollection<TextBox>();
        StandardsTextBoxesIndexes = new ObservableCollection<Border>();
        AddStandardsDeleteButton();
        AddStandardsTextBox();
        AddStandardsTextIndexBlock();

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
        PcpComboBoxes = new ObservableCollection<ComboBox>();
        TextBlocks = new ObservableCollection<Border>();
        LoadNumTextBoxes = new ObservableCollection<NumTextBox>();
        PosComboBoxes = new ObservableCollection<ComboBox>();
        UsedBlocks = new ObservableCollection<Border>();
        MsTextBoxes = new ObservableCollection<TextBox>();
        DeleteButtons = new ObservableCollection<Button>();
        TrackIdTextBoxes = new ObservableCollection<TextBox>();
        NotesTextBoxes = new ObservableCollection<Button>();

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
            AddTrackIdBox();
            AddNotesBoxes();
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
        AddTrackIdBox();
        AddNotesBoxes();
    }

    private void AddButtonsSampleInfo(object? sender, EventArgs e)
    {
        if (sender as TextBox != SampleInfoBoxes.Last())
        {
            return;
        }
        AddSampleInfoBox();
        AddSampleInfoIndexBlock();
        AddSampleDeleteButton();
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
        var box = new ComboBox()
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

    private void AddTrackIdBox()
    {
        var textBox = new TextBox()
        {
            Opacity = 0.3
        };
        if (TrackIdTextBoxes.Count > 0)
        {
            TrackIdTextBoxes.Last().Opacity = 1;
        }

        TrackIdTextBoxes.Add(textBox);
    }

    private void AddNotesBoxes()
    {
        var button = new Button()
        {
            Opacity = 0.3,
        };
        if (NotesTextBoxes.Count > 0)
        {
            NotesTextBoxes.Last().Opacity = 1;
        }

        var flyout = new Flyout()
        {
            Content = new Grid()
            {
                Width = 800,
                Height = 800,
                Children =
                {
                    new TextBox()
                    {
                        Width = 750,
                        Height = 750,
                        TextWrapping = TextWrapping.Wrap,
                        Watermark = "Input...",
                    },
                }
            },
            ShowMode = FlyoutShowMode.Standard,
            Placement = PlacementMode.Bottom,
        };
        flyout.FlyoutPresenterClasses.Add("Bigger");

        FlyoutBase.SetAttachedFlyout(button, flyout);
        
        button.Click += (o, e) =>
        {
            var sender = o as Control;
            if (sender != null)
            {
                Flyout.ShowAttachedFlyout(sender);
            }
        };
        flyout.Closed += (o, e) =>
        {
            button.Content = !string.IsNullOrWhiteSpace(((TextBox)((Grid)flyout.Content).Children[0]).Text)
                ? "Has notes"
                : "";
        };

        NotesTextBoxes.Add(button);
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
    
    private void AddSampleInfoIndexBlock()
    {
        var border = new Border()
        {
            Opacity = 0.3,
            Child = new TextBlock()
            {
                Text = (SampleInfoBoxesIndexes.Count + 1).ToString(),
                Opacity = 0.3
            }
        };
        border.Classes.Add("TextBlockBorder");
        if (SampleInfoBoxesIndexes.Count > 0)
        {
            SampleInfoBoxesIndexes.Last().Opacity = 1;
            SampleInfoBoxesIndexes.Last().Child.Opacity = 1;
        }

        SampleInfoBoxesIndexes.Add(border);
    }
    
    private void AddSampleInfoBox()
    {
        var textBox = new TextBox()
        {
            Opacity = 0.3,
        };
        if (SampleInfoBoxes.Count > 0)
        {
            SampleInfoBoxes.Last().Opacity = 1;
        }
        textBox.KeyDown += AddButtonsSampleInfo;
        SampleInfoBoxes.Add(textBox);
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
                
                NotesTextBoxes[index].Content = string.Empty;
                var attachedFlyout = FlyoutBase.GetAttachedFlyout(NotesTextBoxes[index]);
                if (attachedFlyout is Flyout flyout && flyout.Content is Grid grid)
                {
                    var textBox = grid.Children.OfType<TextBox>().FirstOrDefault();
                    if (textBox != null)
                    {
                        textBox.Text = string.Empty;
                    }
                }
                
                TrackIdTextBoxes[index].Text = string.Empty;
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
            NotesTextBoxes.RemoveAt(index);
            TrackIdTextBoxes.RemoveAt(index);
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
    
    private void AddSampleDeleteButton()
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
            var index = SampleDeleteButtons.IndexOf(button);

            SampleInfoBoxes.RemoveAt(index);
            SampleInfoBoxesIndexes.RemoveAt(index);
            SampleDeleteButtons.RemoveAt(index);

            for (var i = index; i < SampleInfoBoxesIndexes.Count; i++)
            {
                ((TextBlock)SampleInfoBoxesIndexes[i].Child).Text = (i + 1).ToString();
            }
        };
        if (SampleDeleteButtons.Count > 0)
        {
            SampleDeleteButtons.Last().Opacity = 1;
            SampleDeleteButtons.Last().IsEnabled = true;
        }

        SampleDeleteButtons.Add(button);
    }

    private void AddStandardsDeleteButton()
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
            var index = StandardsDeleteButtons.IndexOf(button);
            if (index >= 0)
            {
                // Remove the corresponding standards items
                StandardsTextBoxes.RemoveAt(index);
                StandardsTextBoxesIndexes.RemoveAt(index);
                StandardsDeleteButtons.RemoveAt(index);
            
                // Update indexes for remaining items
                for (var i = index; i < StandardsTextBoxesIndexes.Count; i++)
                {
                    ((TextBlock)StandardsTextBoxesIndexes[i].Child).Text = (i + 1).ToString();
                }
            }
        };

        if (StandardsDeleteButtons.Count > 0)
        {
            StandardsDeleteButtons.Last().Opacity = 1;
            StandardsDeleteButtons.Last().IsEnabled = true;
        }

        StandardsDeleteButtons.Add(button);
    }
    private void AddStandardsTextBox()
    {
        var textBox = new TextBox()
        {
            Opacity = 0.3,
        };
        if (StandardsTextBoxes.Count > 0)
        {
            StandardsTextBoxes.Last().Opacity = 1;
        }
        textBox.KeyDown += AddButtonsStandardsInfo;
        StandardsTextBoxes.Add(textBox);
    }

    private void AddStandardsTextIndexBlock()
    {
        var border = new Border()
        {
            Opacity = 0.3,
            Child = new TextBlock()
            {
                Text = (StandardsTextBoxesIndexes.Count + 1).ToString(),
                Opacity = 0.3
            }
        };
        border.Classes.Add("TextBlockBorder");
        if (StandardsTextBoxesIndexes.Count > 0)
        {
            StandardsTextBoxesIndexes.Last().Opacity = 1;
            StandardsTextBoxesIndexes.Last().Child.Opacity = 1;
        }

        StandardsTextBoxesIndexes.Add(border);
    }

    private void AddButtonsStandardsInfo(object? sender, EventArgs e)
    {
        if (sender as TextBox != StandardsTextBoxes.Last())
        {
            return;
        }
        AddStandardsTextBox();
        AddStandardsTextIndexBlock();
        AddStandardsDeleteButton();
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

    private static string PoundsToGramsString(string pounds)
    {
        if (pounds.EndsWith("lb"))
        {
            return double.TryParse(pounds.AsSpan(0, pounds.Length - 2), out var valueLb) ? (valueLb * 453.59237).ToString("F3") : "";
        }
        return double.TryParse(pounds, out var value) ? (value * 453.59237).ToString("F3") : "";
    }
    
    private static string GramsToPoundsString(string grams)
    {
        if (grams.EndsWith('g'))
        {
            return double.TryParse(grams.AsSpan(0, grams.Length - 1), out var valueG) ? (valueG / 453.59237).ToString("F3") : "";
        }
        return double.TryParse(grams, out var value) ? (value / 453.59237).ToString("F3") : "";
    }

    private void ValidateForm(Grid mainGrid)
    {
        if (mainGrid != null)
        {
            Behaviors.ValidationBehavior.ValidateAll(mainGrid);
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

    public TimeSpan? FormTime
    {
        get => _formTime;
        set => this.RaiseAndSetIfChanged(ref _formTime, value);
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

    public ObservableCollection<ComboBox> PcpComboBoxes
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

    public ObservableCollection<Button> SampleDeleteButtons
    {
        get => _sampleDeleteButtons;
        set => this.RaiseAndSetIfChanged(ref _sampleDeleteButtons, value);
    }

    public ObservableCollection<TextBox> SampleInfoBoxes
    {
        get => _sampleInfoBoxes;
        set => this.RaiseAndSetIfChanged(ref _sampleInfoBoxes, value);
    }

    public ObservableCollection<Border> SampleInfoBoxesIndexes
    {
        get => _sampleInfoBoxesIndexes;
        set => this.RaiseAndSetIfChanged(ref _sampleInfoBoxesIndexes, value);
    }
    
    public ObservableCollection<Button> StandardsDeleteButtons
    {
        get => _standardsDeleteButtons;
        set => this.RaiseAndSetIfChanged(ref _standardsDeleteButtons, value);
    }

    public ObservableCollection<Border> StandardsTextBoxesIndexes
    {
        get => _standardsTextBoxesIndexes;
        set => this.RaiseAndSetIfChanged(ref _standardsTextBoxesIndexes, value);
    }

    public ObservableCollection<TextBox> StandardsTextBoxes
    {
        get => _standardsTextBoxes;
        set => this.RaiseAndSetIfChanged(ref _standardsTextBoxes, value);
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

    public ObservableCollection<TextBox> TrackIdTextBoxes
    {
        get => _trackIdTextBoxes;
        set => this.RaiseAndSetIfChanged(ref _trackIdTextBoxes, value);
    }

    public ObservableCollection<Button> NotesTextBoxes
    {
        get => _notesTextBoxes;
        set => this.RaiseAndSetIfChanged(ref _notesTextBoxes, value);
    }

    public string Notes
    {
        get => _notes;
        set => this.RaiseAndSetIfChanged(ref _notes, value);
    }

    public string SelectedModel
    {
        get => _selectedModel;
        set => _selectedModel = value;
    }

    public string SelectedSize
    {
        get => _selectedSize;
        set => _selectedSize = value;
    }

    public string SelectedMass
    {
        get => _selectedMass;
        set => _selectedMass = value;
    }

    public string SelectedCondition
    {
        get => _selectedCondition;
        set => this.RaiseAndSetIfChanged(ref _selectedCondition, value);
    }

    public string MassGrams
    {
        get => _massGrams;
        set
        {
            this.RaiseAndSetIfChanged(ref _massGrams, value);
            if (!string.IsNullOrEmpty(value))
            {
                this.RaiseAndSetIfChanged(ref _massPounds, GramsToPoundsString(value) + " lb");
            }
            else
            {
                this.RaiseAndSetIfChanged(ref _massPounds, GramsToPoundsString(value));
            }

            this.RaisePropertyChanged(nameof(MassPounds));  
        } 
    }


    public string MassPounds
    {
        get => _massPounds;
        set
        {
            this.RaiseAndSetIfChanged(ref _massPounds, value);
            if (!string.IsNullOrEmpty(value))
            {
                this.RaiseAndSetIfChanged(ref _massGrams, PoundsToGramsString(value) + " g");
            }
            else
            {
                this.RaiseAndSetIfChanged(ref _massGrams, PoundsToGramsString(value));
            } 
            this.RaisePropertyChanged(nameof(MassGrams));
        }
    }

    public string SelectedRangeConfig
    {
        get => _selectedRangeConfig;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedRangeConfig, value);
            if (SelectedRangeConfig == "Doppler radar")
            {
                RangeConfigText = "Infinition inc. JB-6e Junction box , BR-3502 Doppler Radar Tx: 35.505 GHz ";
                TrackIdText = "Track ID";
                VelText = "Str.Vel. Ft/s";
            }
            else
            {
                RangeConfigText = "Muzzle to  Primary Screens: 5 feet\nDistance Between Primary Screens: 5 feet\nDistance Between Secondary Screens: 5 feet\nDistance from Primary Screens to the Target: 5 feet\nTotal Range Distance: 15 feet";
                TrackIdText = "Vel.1 ft/s";
                VelText = "Vel.2 ft/s";
            }
        }
    }

    public Collection<string> HelmetTypes { get; } = new Collection<string> 
    { 
        "Viper", "Cobra", "CAMALUS", "Virtus", "Visor", "Mandible"
    };
    
    public Collection<string> HelmetSize { get; } = new Collection<string> 
    { 
        "Small", "Medium", "Large", "X-Large", "Small MidCut", "Medium MidCut", "Large MidCut", "X-Large MidCut", "Small HiCut", "Medium HiCut", "Large HiCut", "X-Large HiCut"
    };

    public Collection<string> ShellDescriptions { get; } = new Collection<string>
    {
        "Raw  (no edge trim or paint)",
        "Unpainted (edge trim, no paint)",
        "Finished (trimmed and painted)"
    };

    public Collection<string> Conditions { get; } = new Collection<string>()
    {
        "Hot", "Cold", "Temp Shock", "Wet", "Seawater", "WM", "Ambient"
    };

    public Collection<string> Projectiles { get; } = new Collection<string>()
    {
        "2 gr RCC",
        "4 gr RCC",
        "16 gr RCC",
        "17 gr FSP",
        "44 gr FSP",
        "64 gr RCC",
        "9 mm FMJ 124 gr",
        "7.62x39 mm FMJ 123 gr",
        "7.62 mm M80 BALL (C21)",
        "7.62 mm M61 (P80)",
        "7.62 mm APM 2",
        "5.56x45 mm M855",
        "5.56x45 mm M193",
        "7.62x39 mm PS",
        ".44 Mag HSP 240 gr",
        ".357 Sig",
        "17 gr FSP Untumbled"
    };

    public ObservableCollection<string> PowderTypes { get; } = new ObservableCollection<string>
    {
        "N310",
        "H-110",
        "Varget",
        "Red Dot",
        "The Group",
        "N140",
        "IMR 4198",
        "HS-6",
        "IMR 4350",
        "H380",
        "IMR 4227",
        "IMR 4064"
    };
    
    public Collection<string> BarrelType { get; } = new Collection<string>()
    {
        "5.56 mm",
        "5.56 Sabot",
        "7.62x39mm",
        ".308 Win",
        ".308 Win Sabot",
        "30-06 Springfield",
        "30-06 Springfield Sabot",
        ".300 Win Mag",
        ".300 Win Mag Sabot",
        ".220 Swift",
        ".22 Hornet",
        "9 mm",
        ".44 Mag",
        ".357 Mag",
        ".50 BMG Sabot"
    };

    public Collection<string> RangeTypes { get; } = new Collection<string>()
    {
        "Doppler radar",
        "Light Screens"
    };

    public string RangeConfigText
    {
        get => _rangeConfigText;
        set => this.RaiseAndSetIfChanged(ref _rangeConfigText, value);
    }

    public string VelText
    {
        get => _velText;
        set => this.RaiseAndSetIfChanged(ref _velText, value);
    }

    public string TrackIdText
    {
        get => _trackIdText;
        set => this.RaiseAndSetIfChanged(ref _trackIdText, value);
    }

    public string SampleNumberText
    {
        get => _sampleNumberText;
        set
        {
            this.RaiseAndSetIfChanged(ref _sampleNumberText, value);
            this.RaisePropertyChanged(nameof(SampleReportNumberText));
        }
    }

    public string SampleReportNumberText { get => _sampleNumberText.ToUpper().Trim(); }
}
    