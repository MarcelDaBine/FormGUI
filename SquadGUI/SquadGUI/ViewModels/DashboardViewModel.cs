using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;
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
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore.Measure;
using SquadGUI.Interfaces;
using SquadGUI.Services;

namespace SquadGUI.ViewModels;

public class DashboardViewModel : ViewModelBase
{
    private const int ChartItemsCount = 6;
    private Dictionary<FloatNumTextBox, double> _ppList;
    private Dictionary<FloatNumTextBox, double> _cpList;
    private Dictionary<ComboBox, string> _selectedItems;

    private ObservableCollection<Border> _textBlocks;
    private ObservableCollection<Border> _usedBlocks;
    private ObservableCollection<FloatNumTextBox> _numVelNumVelTextBoxes;
    private ObservableCollection<ComboBox> _pcpComboBoxes;
    private ObservableCollection<FloatNumTextBox> _loadNumTextBoxes;
    private ObservableCollection<ComboBox> _posComboBoxes;
    private ObservableCollection<TextBox> _trackIdTextBoxes;
    private ObservableCollection<Button> _notesTextBoxes;
    private ObservableCollection<TextBox> _msTextBoxes;
    private ObservableCollection<Button> _deleteButtons;
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


    private string _meanValueMs = "";
    private string _meanValueFt = "";
    private string _debugChartMsg = "";
    private string _highPartialMs = "";
    private string _highPartialFt = "";
    private string _lowCompleteMs = "";
    private string _lowCompleteFt = "";
    private string _mixedResultsFt = "";
    private string _mixedResultsMs = "";
    private string _gapMs = "";
    private string _gapFt = "";
    private string _rangeResultsFt = "";
    private string _rangeResultsMs = "";
    private string _v50MinFt = "";
    private string _v50MinMs = "";
    private string _deltaVMs = "";
    private string _deltaVFt = "";
    private string _percentageFt = "";
    private string _percentageMs = "";
    private string _expandedUncertainty = "";
    private string _decisionRule = "";
    private string _fahrenheitText = "";
    private string _celsiusText = "";
    private string _humidityText = "";
    private string _selectedProjectile = "";
    private string _selectedPowder = "";
    private string _selectedBarrel = "";
    private string _selectedModel = "";
    private string _selectedSize = "";
    private string _selectedMass = "";
    private string _selectedCondition = "";
    private string _massGrams = "";
    private string _massPounds = "";
    private string _selectedRangeConfig = "Doppler radar";
    private string _rangeConfigText = "";
    private string _velText = "";
    private string _trackIdText = "";
    private string _lotNo = "";
    private string _sampleNumberText = "";
    private string _reportNumberText = "";
    private string _client = "";
    private string _description = "";
    private string _optionalInfoText = "";
    private string _shotSpacing = "";
    private string _witnessPanel = "";
    private string _obliquity = "";
    private string _backingMaterial = "";
    private string _shooter = "";
    private string _recorder = "";


    private DateTimeOffset? _formDate = DateTimeOffset.Now;
    private TimeSpan? _formTime = TimeSpan.Parse(DateTime.Now.ToString("HH:mm:ss"));
    
    private IFileIo _fileIo;
    public ReactiveCommand<Grid,Unit> ValidateAllCommand { get; }
    public ReactiveCommand<Unit, Task> DeserializeCommand { get;}
    
    public ReactiveCommand<Unit,Unit> SaveCommand { get; }
    
    public ICommand TogglePaneCommand { get; }
    


    //constructor
    public DashboardViewModel(IFileIo fileIo)
    {
        _fileIo = fileIo;
        
        IsPaneOpenAttribute = false;
        TogglePaneCommand = new RelayCommand(() =>
        {
            IsPaneOpenAttribute = !IsPaneOpenAttribute;
        });

        var yAxis = new Axis()
        {
            MinLimit = 0,
            MaxLimit = 2700,
        };
        _axisList = new List<Axis> { yAxis };

        _lineSeries = new LineSeries<double>
        {
            Stroke = new SolidColorPaint(SKColors.SlateGray, 6),
            GeometrySize = 10,
            GeometryStroke = new SolidColorPaint(SKColors.SlateGray),
            GeometryFill = new SolidColorPaint(SKColors.WhiteSmoke),
            LineSmoothness = 0,
            Name = "Shot",

            Fill = null,
        };
        _v50Series = new LineSeries<double>()
        {
            Stroke = new SolidColorPaint(SKColor.Parse("#3d9ce1"), 6),
            GeometrySize = 0,
            GeometryStroke = new SolidColorPaint(SKColor.Parse("#3d9ce1"), 0),
            Fill = null,
            Name = "V50"
        };
        _v50MinSeries = new LineSeries<double>()
        {
            Stroke = new SolidColorPaint(SKColor.Parse("#3db9e6"), 6),
            GeometrySize = 0,
            GeometryStroke = new SolidColorPaint(SKColor.Parse("#3d9ce1"), 0),
            Fill = null,
            Name = "V50Min"
        };

        ValidateAllCommand = ReactiveCommand.Create<Grid>(ValidateForm);
        DeserializeCommand = ReactiveCommand.Create(Deserialize);
        SaveCommand = ReactiveCommand.Create(SaveForm);

        Series = new ISeries[] { _v50MinSeries, _v50Series, _lineSeries };

        TogglePaneCommand = ReactiveCommand.Create(TogglePane);

        _ppList = new Dictionary<FloatNumTextBox, double>();
        _cpList = new Dictionary<FloatNumTextBox, double>();


        SampleInfoBoxes = new ObservableCollection<TextBox>();
        SampleInfoBoxesIndexes = new ObservableCollection<Border>();
        SampleDeleteButtons = new ObservableCollection<Button>();
        AddSampleInfoBox("");
        AddSampleInfoIndexBlock();
        AddSampleDeleteButton();

        StandardsDeleteButtons = new ObservableCollection<Button>();
        StandardsTextBoxes = new ObservableCollection<TextBox>();
        StandardsTextBoxesIndexes = new ObservableCollection<Border>();
        AddStandardsDeleteButton();
        AddStandardsTextBox("");
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
        LoadNumTextBoxes = new ObservableCollection<FloatNumTextBox>();
        PosComboBoxes = new ObservableCollection<ComboBox>();
        UsedBlocks = new ObservableCollection<Border>();
        MsTextBoxes = new ObservableCollection<TextBox>();
        DeleteButtons = new ObservableCollection<Button>();
        TrackIdTextBoxes = new ObservableCollection<TextBox>();
        NotesTextBoxes = new ObservableCollection<Button>();

        for (var i = 0; i < ChartItemsCount + 1; ++i) // +1 bc of the empty box
        {
            AddVelocityNumTextBox("");
            AddPcpComboBox("");
            AddTextBlock();
            AddLoadNumTextBox("");
            AddUsedBlock();
            AddPosComboBox("");
            AddMsTextBox();
            AddDeleteButton();
            AddTrackIdBox("");
            AddNotesBoxes("");
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
                if (!_ppList.ContainsKey(c)) // the warning is wrong, if you fix it and type in a box the program will crash
                    _ppList.Add(c, tempPP);
                else _ppList[c] = tempPP;
            }
            else if (b == "CP" && double.TryParse(c.Text, out var tempCP))
            {
                _ppList.Remove(c);
                if (!_cpList.ContainsKey(c)) // the warning is wrong, if you fix it and type in a box the program will crash
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
                    DebugChartMsg = "Worked";
                    MeanValueMs = CalculateV50Mean(lowestCpList.Take(count).ToList(), highestPpList.Take(count).ToList()).ToString("F3");
                    UpdateChartValues();
                    break;
                case <= 50:
                    count = 5;
                    if (_ppList.Count <= count && _cpList.Count <= count)
                    {
                        DebugChartMsg = $"%50m/s difference between the 3 highest pp shots and 3 lowest cp shot so {count - _ppList.Count} pp shots are needed and {count - _cpList.Count} cp shots are needed";
                        _v50Series.Values = null;
                        break;
                    }

                    DebugChartMsg = "Worked";
                    MeanValueMs = CalculateV50Mean(lowestCpList.Take(count).ToList(), highestPpList.Take(count).ToList()).ToString("F3");
                    UpdateChartValues();
                    break;
                case <= 60:
                    count = 7;
                    if (_ppList.Count <= count && _cpList.Count <= count)
                    {
                        DebugChartMsg =
                            $"60m/s difference between the highest pp shot and lowest cp shot so {count - _ppList.Count} pp shots are needed and {count - _cpList.Count} cp shots are needed";
                        _v50Series.Values = null;
                        break;
                    }

                    DebugChartMsg = "Worked";
                    MeanValueMs = CalculateV50Mean(lowestCpList.Take(count).ToList(), highestPpList.Take(count).ToList()).ToString("F3");
                    UpdateChartValues();
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
    }

    private void UpdateChartValues()
    {
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
    private void AddButtonsSender(object? sender, EventArgs e)
    {
        
        switch (sender)
        {
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

        AddPcpComboBox("");
        AddVelocityNumTextBox("");
        AddTextBlock();
        AddLoadNumTextBox("");
        AddUsedBlock();
        AddPosComboBox("");
        AddMsTextBox();
        AddDeleteButton();
        AddTrackIdBox("");
        AddNotesBoxes("");
    }
    
    private void AddButtonsStandardsInfo(object? sender, EventArgs e)
    {
        if (sender as TextBox != StandardsTextBoxes.Last())
        {
            return;
        }
        AddStandardsTextBox("");
        AddStandardsTextIndexBlock();
        AddStandardsDeleteButton();
    }
    
    private void AddButtonsSampleInfo(object? sender, EventArgs e)
    {
        if (sender as TextBox != SampleInfoBoxes.Last())
        {
            return;
        }
        AddSampleInfoBox("");
        AddSampleInfoIndexBlock();
        AddSampleDeleteButton();
    }

    private void AddVelocityNumTextBox(string value)
    {
        var numTextBox = new FloatNumTextBox()
        {
            Opacity = 0.3
        };
        Behaviors.ValidationBehavior.SetEnableValidation(numTextBox, false);
        numTextBox.KeyDown += AddButtonsSender;
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
            Behaviors.ValidationBehavior.SetEnableValidation(NumVelTextBoxes.Last(), true);
        }

        NumVelTextBoxes.Add(numTextBox);
    }

    private void AddPcpComboBox(string value)
    {
        var box = new ComboBox()
        {
            Width = 100,
            Height = 30,
            Margin = Thickness.Parse("10"),
            Items = { "PP", "CP", "NA" },
            SelectedItem = value,
            Opacity = 0.3
        };

        /*
        box.PointerPressed += (sender, e) =>
        {
            if (sender is ComboBox cb)
            {
                cb.IsDropDownOpen = true;
                AddButtonsSender(sender, e);
            }
        };
        */
        box.SelectionChanged += (sender, e) => { UpdateChart(); };
        Behaviors.ValidationBehavior.SetEnableValidation(box, false);
        
        if (PcpComboBoxes.Count > 0)
        {
            PcpComboBoxes.Last().Opacity = 1;
            Behaviors.ValidationBehavior.SetEnableValidation(PcpComboBoxes.Last(), true);
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

    private void AddPosComboBox(string value)
    {
        var posComboBox = new ComboBox()
        {
            Width = 100,
            Height = 30,
            Margin = Thickness.Parse("10"),
            Items = { "Crown", "Back", "Left", "Right", "Front" },
            SelectedItem = value,
            Opacity = 0.3
        };
        /*
        posComboBox.PointerPressed += (sender, e) =>
        {
            if (sender is ComboBox cb)
            {
                cb.IsDropDownOpen = true;
                AddButtonsSender(sender, e);
            }
        };
        */
        Behaviors.ValidationBehavior.SetEnableValidation(posComboBox, false);
        if (PosComboBoxes.Count > 0)
        {
            PosComboBoxes.Last().Opacity = 1;
            Behaviors.ValidationBehavior.SetEnableValidation(PosComboBoxes.Last(), true);
        }

        PosComboBoxes.Add(posComboBox);
    }

    private void AddTrackIdBox(string value)
    {
        var textBox = new TextBox()
        {
            Opacity = 0.3,
            Text = value
        };
        Behaviors.ValidationBehavior.SetEnableValidation(textBox, false);
        if (TrackIdTextBoxes.Count > 0)
        {
            TrackIdTextBoxes.Last().Opacity = 1;
            Behaviors.ValidationBehavior.SetEnableValidation(TrackIdTextBoxes.Last(), true);
        }
        TrackIdTextBoxes.Add(textBox);
    }

    private void AddNotesBoxes(string value)
    {
        var button = new Button()
        {
            Opacity = 0.3,
            IsEnabled = false
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
                        Text = value
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
        if (NotesTextBoxes.Count > 0)
        {
            NotesTextBoxes.Last().IsEnabled = true;
        }
        NotesTextBoxes.Add(button);
    }

    private void AddLoadNumTextBox(string value)
    {
        var textBox = new FloatNumTextBox()
        {
            Opacity = 0.3,
            Text = value
        };
        textBox.KeyDown += AddButtonsSender;
        Behaviors.ValidationBehavior.SetEnableValidation(textBox, false);
        if (LoadNumTextBoxes.Count > 0)
        {
            LoadNumTextBoxes.Last().Opacity = 1;
            Behaviors.ValidationBehavior.SetEnableValidation(LoadNumTextBoxes.Last(), true);
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
    
    private void AddSampleInfoBox(string value)
    {
        var textBox = new TextBox()
        {
            Opacity = 0.3,
            Text = value
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
    private void AddStandardsTextBox(string value)
    {
        var textBox = new TextBox()
        {
            Opacity = 0.3,
            Text = value
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
            return double.TryParse(fahrenheit.AsSpan(0, fahrenheit.Length - 3), out var value)
                ? ((value - 32) * 5 / 9).ToString("F1")
                : "";
        }
        else
        {
            return double.TryParse(fahrenheit, out var value)
                ? ((value - 32) * 5 / 9).ToString("F1")
                : "";
        }
    }

    private static string CelsiusToFahrenheitString(string celsius)
    {
        if (celsius.EndsWith("\u00b0C"))
        {
            return double.TryParse(celsius.AsSpan(0, celsius.Length - 3), out var value)
                ? (value * 9 / 5 + 32).ToString("F1")
                : "";
        }
        else
        {
            return double.TryParse(celsius, out var value)
                ? (value * 9 / 5 + 32).ToString("F1")
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
        if (mainGrid == null)
        {
            return;
        }

        if (!Behaviors.ValidationBehavior.ValidateAll(mainGrid) || String.IsNullOrEmpty(MeanValueFt))
        {
            return;
        }

        var shots = new List<object>();
        
        for (var i = 0; i < NumVelTextBoxes.Count - 1; i++)
        {
            var noteText = NotesTextBoxes[i].Content?.ToString();
            var row = new
            {
                load = double.Parse(LoadNumTextBoxes[i].Text),
                trackID = TrackIdTextBoxes[i].Text,
                strVelFt = double.Parse(NumVelTextBoxes[i].Text),
                strVelMs = double.Parse(MsTextBoxes[i].Text),
                ppCp = PcpComboBoxes[i].SelectedItem?.ToString(),
                used = ((TextBlock)UsedBlocks[i].Child).Text == "Y" ? true : false,
                notes = string.IsNullOrEmpty(noteText) ? "" : noteText,
                position = PosComboBoxes[i].SelectedItem?.ToString()
            };

            shots.Add(row);
        }
        
        var info = new List<object>();
        if (_sampleInfoBoxes.Count == 1)
        {
            info = null;
        }
        else
        {
            foreach (var box in _sampleInfoBoxes) 
            {
                info.Add(box.Text);
            }
        }
        
        List<object> proc = new List<object>();
        if (_standardsTextBoxes.Count == 1)
        {
            proc = null;
        }
        else
        {
            foreach (var box in _standardsTextBoxes) 
            {
                proc.Add(box.Text);
            }
        }

        var data = new
        {
            date = _formDate?.ToString("yyyy-MM-dd"),
            time = _formTime?.ToString(@"hh\:mm"),
            temperatureC = double.Parse(_celsiusText.Substring(0, _celsiusText.Length - 2).Trim()),
            temperatureF = double.Parse(_fahrenheitText.Substring(0, _fahrenheitText.Length - 2).Trim()),
            humidity = double.Parse(_humidityText.Substring(0, _humidityText.Length - 1).Trim()),
            lotNo = LotNo, 
            client = Client,
            reportNumber = SampleReportNumber1 + " " + SampleReportNumberText + " " + SelectedProjectile + " " + SampleReportNumber2 + " " + SelectedCondition,
            sampleNumber = _sampleNumberText,
            description = Description,
            model = _selectedModel,
            size = _selectedSize,
            mass = _selectedMass,
            grams = double.Parse(_massGrams.Substring(0, _massGrams.Length - 1).Trim()),
            pounds = double.Parse(_massPounds.Substring(0, _massPounds.Length - 2).Trim()),
            condition = _selectedCondition,
            optionalInfoText = OptionalInfoText,
            inputRowsInfo = info,
            projectile = _selectedProjectile,
            powder = _selectedPowder,
            barrel = _selectedBarrel,
            sensor = _selectedRangeConfig,
            shotSpacing = _shotSpacing,
            witnessPanel = _witnessPanel,
            obliquity = _obliquity,
            backingMaterial = _backingMaterial,
            inputFieldsProc = proc,

            // Chart fields
            v50ValueM = double.Parse(_meanValueMs),
            v50ValueFt = _meanValueFt,
            highPartialM = _highPartialMs,
            highPartialFt = _highPartialFt,
            lowCompleteM = _lowCompleteMs,
            lowCompleteFt = _lowCompleteFt,
            mixedResultsM = _mixedResultsMs,
            mixedResultsFt = _mixedResultsFt,
            gapM = _gapMs,
            gapFt = _gapFt,
            rangeResultsM = _rangeResultsMs,
            rangeResultsFt = _rangeResultsFt,
            v50MinFt = _v50MinFt,
            v50MinM = _v50MinMs,
            deltaVM = _deltaVMs,
            deltaVFt = _deltaVFt,
            percentageM = _percentageMs,
            percentageFt = _percentageFt,
            expandedUncertainty = _expandedUncertainty,
            decisionRule = _decisionRule,
            data = shots
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
        };

        string json = JsonSerializer.Serialize(data, options);

        // You can write it to a file or just debug output
        
        _fileIo.SubmitJsonAsync(json);
        
        Console.WriteLine(json); // or Debug.WriteLine(json) if in Avalonia GUI
    }

    private void SaveForm()
    {

        var shots = new List<object>();

        for (var i = 0; i < NumVelTextBoxes.Count; i++)
        {
            var noteText = NotesTextBoxes[i].Content?.ToString();

            shots.Add(new
            {
                load = TryParseDouble(LoadNumTextBoxes[i]?.Text),
                trackID = TryParseString(TrackIdTextBoxes[i]?.Text),
                strVelFt = TryParseDouble(NumVelTextBoxes[i]?.Text),
                ppCp = TryParseString(PcpComboBoxes[i]?.SelectedItem?.ToString()),
                notes = TryParseString(noteText),
                position = TryParseString(PosComboBoxes[i]?.SelectedItem?.ToString())
            });
        }

        var info = _sampleInfoBoxes.Count > 1 ? _sampleInfoBoxes.Select(b => b.Text).ToList() : null;
        var proc = _standardsTextBoxes.Count > 1 ? _standardsTextBoxes.Select(b => b.Text).ToList() : null;

        var data = new
        {
            date = _formDate?.ToString("yyyy-MM-dd"),
            time = _formTime?.ToString(@"hh\:mm"),
            temperatureC = TryParseDouble(_celsiusText),
            temperatureF = TryParseDouble(_fahrenheitText),
            humidity = TryParseDouble(_humidityText),
            lotNo = TryParseString(LotNo),
            client = TryParseString(Client),
            reportNumber = TryParseString(SampleReportNumber1 + " " + SampleReportNumberText + " " +
                                          SelectedProjectile + " " + SampleReportNumber2 + " " + SelectedCondition),
            sampleNumber = TryParseString(_sampleNumberText),
            description = TryParseString(Description),
            model = TryParseString(_selectedModel),
            size = TryParseString(_selectedSize),
            mass = TryParseString(_selectedMass),
            grams = TryParseDouble(_massGrams),
            pounds = TryParseDouble(_massPounds),
            condition = TryParseString(_selectedCondition),
            optionalInfoText = TryParseString(OptionalInfoText),
            inputRowsInfo = info,
            projectile = TryParseString(_selectedProjectile),
            powder = TryParseString(_selectedPowder),
            barrel = TryParseString(_selectedBarrel),
            sensor = TryParseString(_selectedRangeConfig),
            shotSpacing = TryParseString(_shotSpacing),
            witnessPanel = TryParseString(_witnessPanel),
            obliquity = TryParseString(_obliquity),
            backingMaterial = TryParseString(_backingMaterial),
            inputRowsProc = proc,
            v50ValueM = TryParseDouble(_meanValueMs),
            v50ValueFt = TryParseDouble(_meanValueFt),
            highPartialM = TryParseDouble(_highPartialMs),
            highPartialFt = TryParseDouble(_highPartialFt),
            lowCompleteM = TryParseDouble(_lowCompleteMs),
            lowCompleteFt = TryParseDouble(_lowCompleteFt),
            mixedResultsM = TryParseString(_mixedResultsMs),
            mixedResultsFt = TryParseString(_mixedResultsFt),
            gapM = TryParseString(_gapMs),
            gapFt = TryParseString(_gapFt),
            rangeResultsM = TryParseDouble(_rangeResultsMs),
            rangeResultsFt = TryParseDouble(_rangeResultsFt),
            v50MinFt = TryParseDouble(_v50MinFt),
            v50MinM = TryParseDouble(_v50MinMs),
            deltaVM = TryParseDouble(_deltaVMs),
            deltaVFt = TryParseDouble(_deltaVFt),
            percentageM = TryParseDouble(_percentageMs),
            percentageFt = TryParseDouble(_percentageFt),
            expandedUncertainty = TryParseString(_expandedUncertainty),
            decisionRule = TryParseString(_decisionRule),
            data = shots
        };

        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(data, options);
        _fileIo.SaveJsonAsync(json);
    }


    private async Task Deserialize()
    {
        Console.WriteLine(Directory.GetCurrentDirectory());
        var report = await _fileIo.OpenJsonAsync<ReportModel>();

        if (report != null)
        {
            LoadFromModel(report);
        }
    }

    private void LoadFromModel(ReportModel report)
    {
        FormDate = DateTimeOffset.TryParse(report.Date, out var dt) ? dt : DateTimeOffset.Now;
        FormTime = TimeSpan.TryParse(report.Time, out var ts) ? ts : TimeSpan.Parse(DateTime.Now.ToString("HH:mm"));

        Shooter = TryParseFromString(report.Shooter);
        Recorder = TryParseFromString(report.Recorder);
        CelsiusText = report.TemperatureC != null ? report.TemperatureC + " °C" : "";
        HumidityText = report.Humidity != null ? report.Humidity + "%" : "";

        LotNo = TryParseFromDouble(report.LotNo);
        Client = TryParseFromString(report.Client);
        SampleNumberText = TryParseFromString(report.SampleNumber);
        Description = TryParseFromString(report.Description);

        SelectedModel = TryParseFromString(report.Model);
        SelectedSize = TryParseFromString(report.Size);
        SelectedMass = TryParseFromString(report.Mass);
        MassGrams = report.Grams != null ? report.Grams + " g" : "";
        SelectedCondition = TryParseFromString(report.Condition);
        OptionalInfoText = TryParseFromString(report.OptionalInfoText);

        SelectedProjectile = TryParseFromString(report.Projectile);
        SelectedPowder = TryParseFromString(report.Powder);
        SelectedBarrel = TryParseFromString(report.Barrel);
        SelectedRangeConfig = TryParseFromString(report.Sensor);
        ShotSpacing = TryParseFromString(report.ShotSpacing);
        WitnessPanel = TryParseFromString(report.WitnessPanel);
        Obliquity = TryParseFromString(report.Obliquity);
        BackingMaterial = TryParseFromString(report.BackingMaterial);
        
        V50MinMs = TryParseFromDouble(report.V50MinM);
        V50MinFt = TryParseFromDouble(report.V50MinFt);
        ExpandedUncertainty = TryParseFromDouble(report.ExpandedUncertainty);
        DecisionRule = TryParseFromString(report.DecisionRule);

        // Rows
        if (report.Data != null)
        {
            for (var i = 0; i < report.Data.Count; i++)
            {
                var row = report.Data[i];
                if (NumVelTextBoxes.Count > i)
                {
                    LoadNumTextBoxes[i].Text = TryParseFromString(row.Load);
                    TrackIdTextBoxes[i].Text = TryParseFromString(row.TrackId);
                    NumVelTextBoxes[i].Text = TryParseFromDouble(row.StrVelFt);
                    PcpComboBoxes[i].SelectedItem = TryParseFromString(row.PpCp);
                    PosComboBoxes[i].SelectedItem = TryParseFromString(row.Position);
                    NotesTextBoxes[i].Content = TryParseFromString(row.Notes);
                }
                else
                {
                    AddPcpComboBox(TryParseFromString(row.PpCp));
                    AddVelocityNumTextBox(TryParseFromDouble(row.StrVelFt));
                    AddTextBlock();
                    AddLoadNumTextBox(TryParseFromString(row.Load));
                    AddUsedBlock();
                    AddPosComboBox(TryParseFromString(row.Position));
                    AddMsTextBox();
                    AddDeleteButton();
                    AddTrackIdBox(TryParseFromString(row.TrackId));
                    AddNotesBoxes(TryParseFromString(row.Notes));
                }
            }
        }


        // Lists
        SampleInfoBoxes.Clear();
        if (report.InputRowsInfo != null)
        {
            foreach (var item in report.InputRowsInfo)
            {
                AddSampleInfoBox(item);
                AddSampleInfoIndexBlock();
                AddSampleDeleteButton();
            }
            AddSampleInfoBox("");
        }

        StandardsTextBoxes.Clear();
        if (report.InputRowsProc != null)
        {
            foreach (var item in report.InputRowsProc)
            {
                AddStandardsTextBox(item);
                AddStandardsTextIndexBlock();
                AddStandardsDeleteButton();
            }
            AddStandardsTextBox("");
        }
    }

    private static string TryParseFromString(string? input)
    {
        return string.IsNullOrWhiteSpace(input) ? "" : input;
    }

    private static string TryParseFromDouble(double? input)
    {
        return input.HasValue ? input.Value.ToString("0.##") : "";
    }

    
    private static double? TryParseDouble(string? input)
    {
        return double.TryParse(input, out var result) ? result : null;
    }

    private static string? TryParseString(string? input)
    {
        return string.IsNullOrWhiteSpace(input) ? null : input;
    }


    
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

    public ObservableCollection<FloatNumTextBox> LoadNumTextBoxes
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

    public string SelectedModel
    {
        get => _selectedModel;
        set => this.RaiseAndSetIfChanged(ref _selectedModel, value);
    }

    public string SelectedSize
    {
        get => _selectedSize;
        set => this.RaiseAndSetIfChanged(ref _selectedSize, value);
    }

    public string SelectedMass
    {
        get => _selectedMass;
        set => this.RaiseAndSetIfChanged(ref _selectedMass, value);
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

    public string LotNo
    {
        get => _lotNo;
        set => this.RaiseAndSetIfChanged(ref _lotNo, value);
    }
    public string Client
    {
        get => _client;
        set => this.RaiseAndSetIfChanged(ref _client, value);
    }

    public string Description
    {
        get => _description;
        set => this.RaiseAndSetIfChanged(ref _description, value);
    }

    public string OptionalInfoText
    {
        get => _optionalInfoText;
        set => this.RaiseAndSetIfChanged(ref _optionalInfoText, value);
    }

    public string ShotSpacing
    {
        get => _shotSpacing;
        set => this.RaiseAndSetIfChanged(ref _shotSpacing, value);
    }

    public string WitnessPanel
    {
        get => _witnessPanel;
        set => this.RaiseAndSetIfChanged(ref _witnessPanel, value);
    }

    public string Obliquity
    {
        get => _obliquity;
        set => this.RaiseAndSetIfChanged(ref _obliquity, value);
    }

    public string BackingMaterial
    {
        get => _backingMaterial;
        set => this.RaiseAndSetIfChanged(ref _backingMaterial, value);
    }

    public string Shooter
    {
        get => _shooter;
        set => this.RaiseAndSetIfChanged(ref _shooter, value);
    }

    public string Recorder
    {
        get => _recorder;
        set => this.RaiseAndSetIfChanged(ref _recorder, value);
    }

    public string SampleReportNumberText { get => _sampleNumberText.ToUpper().Trim(); }

    public static string SampleReportNumber1 { get => "GLV INC"; }

    public static string SampleReportNumber2 { get => "V50"; }
}
