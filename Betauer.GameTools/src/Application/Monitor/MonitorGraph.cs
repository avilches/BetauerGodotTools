using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Betauer.Core.Nodes;
using Betauer.UI;
using Godot;

namespace Betauer.Application.Monitor;
public partial class MonitorGraph : BaseMonitor<MonitorGraph> {

    public class Serie {
        private readonly MonitorGraph _graph;
        private Action<Line2D> _chartLineConfig = _ => { };
        private Color _color = Colors.YellowGreen;
        private Func<float> _loadValue;
        private Func<float, string>? _formatValue;

        public Line2D ChartLine { get; } = new();
        public Label Label { get; } = new();
        public Label CurrentValue { get; } = new();
        public LinkedList<float> Data { get; } = new();

        internal Serie(MonitorGraph graph) {
            _graph = graph;
        }

        public Serie SetColor(Color? color) {
            _color = color ?? _graph.NextColor();
            _graph._dirty = true;
            return this;
        }

        public Serie SetLabel(string? label) {
            if (label == null) {
                Label.Visible = false;
            } else {
                Label.Text = label;
                Label.Visible = true;
            }
            return this;
        }

        public Serie Load(Func<float> action) {
            _loadValue = action;
            return this;
        }

        public Serie Load(Func<bool> action) {
            _loadValue = () => action() ? MaxValue : MinValue;
            _formatValue ??= (v) => v > 0f ? "True" : "False";
            return this;
        }

        public Serie Format(string format) {
            _formatValue = (v) => v.ToString(format);
            return this;
        }

        public Serie Format(Func<float, string> action) {
            _formatValue = action;
            return this;
        }
        
        public Serie ConfigureChartLine(Action<Line2D> conf) {
            _chartLineConfig = conf;
            _graph._dirty = true;
            return this;
        }

        internal void ConfigureChartLine() {
            ChartLine.Width = 2f;
            ChartLine.DefaultColor = _color;
            _chartLineConfig.Invoke(ChartLine);
            CurrentValue.SetFontColor(_color);
        }

        public int SecondsHistory => _graph._secondsHistory;
        public int DataSize => _graph.DataSize;
        public int ChartWidth => _graph.ChartWidth;
        public int ChartHeight => _graph.ChartHeight;
        public float MinValue => _graph.MinValue;
        public float MaxValue => _graph.MaxValue;
        public bool IsAutoRange => _graph.IsAutoRange;

        internal void ConfigureChartData() {
            var pending = (SecondsHistory * Fps) - Data.Count;
            if (pending > 0) {
                var zero = Mathf.Lerp(MinValue, MaxValue, 0.5f);
                while (pending-- > 0) Data.AddFirst(zero);
            } else if (pending < 0) {
                while (pending++ < 0) Data.RemoveFirst();
            }
        }

        internal void Process(double delta) {
            var value = _loadValue.Invoke();
            CurrentValue.Text = _formatValue != null? _formatValue.Invoke(value) : value.ToString("000.00");
            Add(value);
            DumpDataToChartLine();
        }

        internal void Add(float v) {
            Data.AddLast(v);
            if (IsAutoRange) {
                if (v > MaxValue) _graph.MaxValue = v;
                else if (v < MinValue) _graph.MinValue = v;
            }
            while (Data.Count > DataSize) Data.RemoveFirst();
        }

        public MonitorGraph EndSerie() {
            return _graph;
        }

        internal void DumpDataToChartLine() {
            ChartLine.ClearPoints();
            var elements = SecondsHistory * 60;
            var range = MaxValue - MinValue;
            var i = 0;
            Data.ForEach(v => {
                var percentWidth = (float)i / elements;
                var percentHeight = (v - MinValue) / range;
                var x = Mathf.Lerp(0, ChartWidth, percentWidth);
                var y = Mathf.Lerp(0, ChartHeight, Math.Clamp(percentHeight, 0f, 1f));
                ChartLine.AddPoint(new Vector2(x, ChartHeight - y));
                i++;
            });
        }
    }

    public class Separator {
        internal readonly float Value;
        internal readonly Line2D Line2D;

        public Separator(float value) {
            Value = value;
            Line2D = new Line2D {
                DefaultColor = DefaultSeparatorColor,
                Width = 1,
                Visible = false
            };
            Line2D.AddPoint(Vector2.Zero);
            Line2D.AddPoint(Vector2.Zero);
        }
    }

    private const int Fps = 60;
    private readonly Node2D _timeSeparatorsHolder = new();
    private readonly Node2D _separatorsHolder = new();
    private readonly Node2D _lineChartHolder = new();
    private readonly Control _chartSpacer = new();
    private readonly HBoxContainer _legend = new();
    private readonly List<Line2D> _timeSeparators = new();
    private readonly List<Separator> _separators = new();
    private Action<Line2D> _borderConfig = _ => { };
    private Action<Line2D> _separatorConfig = _ => { };
    private bool _dirty = true;
    private int _secondsHistory = 10;
    private int _frameCount = 0;
    private int DataSize => _secondsHistory * Fps;
    private List<Serie> _series = new();

    public Line2D BorderLine { get; } = new();
    public int ChartHeight { get; private set; } = 100;
    public int ChartWidth { get; private set; } = 300;
    public bool IsAutoRange { get; private set; } = true;
    public float MaxValue { get; private set; } = 0;
    public float MinValue { get; private set; } = 0;
    
    public MonitorGraph Keep(int seconds) {
        if (seconds == _secondsHistory) return this; // ignore if there is no change
        _secondsHistory = Math.Max(1, seconds);
        _dirty = true;
        return this;
    }

    public MonitorGraph SetChartSize(int width, int height) {
        SetChartHeight(height);
        SetChartWidth(width);
        return this;
    }

    public MonitorGraph SetChartWidth(int width) {
        if (width == ChartWidth) return this; // ignore if there is no change
        ChartWidth = width;
        _dirty = true;
        return this;
    }

    public MonitorGraph SetChartHeight(int height) {
        if (height == ChartHeight) return this; // ignore if there is no change
        ChartHeight = height;
        _dirty = true;
        return this;
    }

    public MonitorGraph AutoRange(bool autoRange = true) {
        IsAutoRange = autoRange;
        return this;
    }

    public MonitorGraph Range(float min, float max) {
        MinValue = Mathf.Min(min, max);
        MaxValue = Mathf.Max(min, max);
        IsAutoRange = false;
        return this;
    }

    public MonitorGraph AddSeparator(float value) {
        var separator = new Separator(value);
        _separators.Add(separator);
        _separatorsHolder.AddChild(separator.Line2D);
        return this;
    }

    public Serie AddSerie(string? label = null) {
        var serie = new Serie(this);
        serie.SetLabel(label);
        _series.Add(serie);
        _lineChartHolder.AddChild(serie.ChartLine);
        _legend.NodeBuilder()
            .Child(serie.Label)
                .Config(label => {
                label.HorizontalAlignment = HorizontalAlignment.Right;
                    label.SetFontColor(DefaultLabelColor);
                })
            .End()
            .Child(serie.CurrentValue)
                .Config(label => {
                    label.HorizontalAlignment = HorizontalAlignment.Right;
                })
            .End();
        _dirty = true;
        return serie;
    }

    public override void _Ready() {
        this.NodeBuilder()
            .Child(_chartSpacer).End()
            .Child(_timeSeparatorsHolder).End()
            .Child(_separatorsHolder).End()
            .Child(_lineChartHolder).End()
            .Child(BorderLine).End()
            .Child<Label>()
                .Child(_legend).End()
            .End();

        _legend.GrowHorizontal = GrowDirection.Begin;
        _legend.SetAnchorsAndOffsetsPreset(LayoutPreset.RightWide);
        _dirty = true;
    }

    public MonitorGraph ConfigureBorderLine(Action<Line2D> conf) {
        _borderConfig = conf;
        _dirty = true;
        return this;
    }

    public MonitorGraph ConfigureSeparatorLine(Action<Line2D> conf) {
        _separatorConfig = conf;
        _dirty = true;
        return this;
    }

    private void ConfigureChartSpaceAndBorder() {
        SizeFlagsHorizontal = 0; // Don't expand, so the size will be the CustomMinimumSize
        CustomMinimumSize = new Vector2(ChartWidth, 0);

        BorderLine.ClearPoints();
        BorderLine.AddPoint(new Vector2(0, 0));
        BorderLine.AddPoint(new Vector2(ChartWidth, 0));
        BorderLine.AddPoint(new Vector2(ChartWidth, ChartHeight));
        BorderLine.AddPoint(new Vector2(0, ChartHeight));
        BorderLine.AddPoint(new Vector2(0, 0));
        BorderLine.Width = 2f;
        BorderLine.DefaultColor = DefaultBorderColor;
        _borderConfig.Invoke(BorderLine);
        
        _chartSpacer.CustomMinimumSize = new Vector2(ChartWidth, ChartHeight);

    }

    private void ConfigureValueSeparators() {
        var range = MaxValue - MinValue;
        foreach (var separator in _separators) {
            var visible = separator.Value > MinValue && separator.Value < MaxValue;
            separator.Line2D.Visible = visible;
            if (visible) {
                var percentHeight = (separator.Value - MinValue) / range;
                var y = ChartHeight - Mathf.Lerp(0, ChartHeight, percentHeight);
                separator.Line2D.SetPointPosition(0, new Vector2(0, y));
                separator.Line2D.SetPointPosition(1, new Vector2(ChartWidth, y));
            }
        }
    }

    private void ConfigureTimeSeparators() {
        var pending = _secondsHistory - _timeSeparators.Count;
        if (pending > 0) {
            while (pending-- > 0) {
                var sep = new Line2D();
                sep.Width = 1f;
                sep.DefaultColor = DefaultSeparatorColor;
                _separatorConfig.Invoke(sep);
                _timeSeparators.Add(sep);
                _timeSeparatorsHolder.AddChild(sep);
                sep.AddPoint(Vector2.Zero);
                sep.AddPoint(Vector2.Zero);
            }
        } else if (pending < 0) {
            while (pending++ < 0) {
                _timeSeparators[^1].QueueFree(); // Delete last separator
                _timeSeparators.RemoveAt(_timeSeparators.Count-1);
            }
        }
    }

    private void UpdateSeparators() {
        var sepSize = (float)ChartWidth / _secondsHistory;
        var sepStep = sepSize / Fps;
        var discount = sepStep * _frameCount;
        for (var i = 0; i < _secondsHistory; i++) {
            var sep = _timeSeparators[i];
            var x = sepSize * (i + 1);
            sep.SetPointPosition(0, new Vector2(x - discount, 0));
            sep.SetPointPosition(1, new Vector2(x - discount, ChartHeight));
        }
    }

    public override void UpdateMonitor(double delta) {
        if (_dirty) {
            foreach (var serie in _series) {
                serie.ConfigureChartLine();
                serie.ConfigureChartData();
            }
            ConfigureValueSeparators();
            ConfigureTimeSeparators();
            ConfigureChartSpaceAndBorder();
            _dirty = false;
        }
        for (var i = 0; i < _series.Count; i++) {
            _series[i].Process(delta);
        }
        UpdateSeparators();
        _frameCount ++;
        if (_frameCount == Fps) {
            if (IsAutoRange) {
                MinValue = _series.SelectMany(s => s.Data).Min();
                MaxValue = _series.SelectMany(s => s.Data).Max();
            }
            _frameCount = 0;
        }
    }
}