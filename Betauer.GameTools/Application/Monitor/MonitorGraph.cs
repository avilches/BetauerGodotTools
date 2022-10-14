using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Object = Godot.Object;

namespace Betauer.Application.Monitor {
    public class MonitorGraph : BaseMonitor {

        public static readonly Color DefaultSeparatorColor = new(1,1,1,0.05f);
        public static readonly Color DefaultBorderColor = new(1,1,1,0.1f);
        
        private const int Fps = 60;
        private readonly Node2D _separatorHolder = new();
        private readonly Control _chartSpacer = new();
        private readonly List<Line2D> _separators = new();
        private readonly HBoxContainer _legendContainer = new();
        private Action<Line2D> _chartLineConfig = line2d => { };
        private Action<Line2D> _borderConfig = line2d => { };
        private Action<Line2D> _separatorConfig = line2d => { };
        private Color _color = Colors.YellowGreen;
        private bool _dirty = true;
        private int _secondsHistory = 5;
        private int _frameCount = 0;
        private int DataSize => _secondsHistory * Fps;
        private Func<float> _loadValue;
        private Func<float, string>? _formatValue;

        public Label Label { get; } = new();
        public Label CurrentValue { get; } = new();
        public LinkedList<float> Data { get; } = new();
        public Line2D BorderLine { get; } = new();
        public Line2D ChartLine { get; } = new();
        
        public int ChartHeight { get; private set; } = 100;
        public int ChartWidth { get; private set; } = 300;
        public bool IsAutoRange { get; private set; } = true;
        public float MaxValue { get; private set; } = 0;
        public float MinValue { get; private set; } = 0;
        
        public MonitorGraph RemoveIfInvalid(Object target) {
            Watch = target;
            return this;
        }

        public MonitorGraph SetColor(Color color) {
            _color = color;
            return this;
        }

        public MonitorGraph SetLabel(string? prefix) {
            if (prefix == null) {
                Label.Visible = false;
            } else {
                Label.Text = prefix;
                Label.Visible = true;
            }
            return this;
        }

        public MonitorGraph Load(Func<float> action) {
            _loadValue = action;
            return this;
        }

        public MonitorGraph Load(Func<bool> action) {
            _loadValue = () => action() ? 1 : 0;
            _formatValue ??= (v) => v > 0f ? "True" : "False";
            return this;
        }

        public MonitorGraph Format(Func<float, string> action) {
            _formatValue = action;
            return this;
        }

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

        public override void _Ready() {
            Label.Modulate = MonitorText.DefaultLabelModulateColor;
            _legendContainer.AddChild(Label);
            _legendContainer.AddChild(CurrentValue);
            AddChild(_chartSpacer);
            AddChild(_separatorHolder);
            AddChild(ChartLine);
            AddChild(BorderLine);
            AddChild(_legendContainer);
            _dirty = true;
        }

        public MonitorGraph ConfigureChartLine(Action<Line2D> conf) {
            _chartLineConfig = conf;
            _dirty = true;
            return this;
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

        private void ConfigureBorder() {
            BorderLine.ClearPoints();
            BorderLine.AddPoint(new Vector2(0, 0));
            BorderLine.AddPoint(new Vector2(ChartWidth, 0));
            BorderLine.AddPoint(new Vector2(ChartWidth, ChartHeight));
            BorderLine.AddPoint(new Vector2(0, ChartHeight));
            BorderLine.AddPoint(new Vector2(0, 0));
            BorderLine.Width = 2f;
            BorderLine.DefaultColor = DefaultBorderColor;
            _borderConfig(BorderLine);
        }

        private void ConfigureSeparators() {
            var pending = _secondsHistory - _separators.Count;
            if (pending > 0) {
                while (pending-- > 0) {
                    var sep = new Line2D();
                    sep.Width = 1f;
                    sep.DefaultColor = DefaultSeparatorColor;
                    _separatorConfig(sep);
                    _separators.Add(sep);
                    _separatorHolder.AddChild(sep);
                    sep.AddPoint(Vector2.Zero);
                    sep.AddPoint(Vector2.Zero);
                }
            } else if (pending < 0) {
                while (pending++ < 0) {
                    _separators[^1].QueueFree(); // Delete last separator
                    _separators.RemoveAt(_separators.Count-1);
                }
            }
        }

        private void ConfigureChartData() {
            var pending = (_secondsHistory * Fps) - Data.Count;
            if (pending > 0) {
                var zero = Mathf.Lerp(MinValue, MaxValue, 0.5f);
                while (pending-- > 0) Data.AddFirst(zero);
            } else if (pending < 0) {
                while (pending++ < 0) Data.RemoveFirst();
            }
        }

        private void Add(float v) {
            Data.AddLast(v);
            if (IsAutoRange) {
                if (v > MaxValue) MaxValue = v;
                else if (v < MinValue) MinValue = v;
            }
            while (Data.Count > DataSize) Data.RemoveFirst();
        }

        private void UpdateSeparators() {
            var sepSize = (float)ChartWidth / _secondsHistory;
            var sepStep = sepSize / Fps;
            var discount = sepStep * _frameCount;
            for (var i = 0; i < _secondsHistory; i++) {
                var sep = _separators[i];
                var x = sepSize * (i + 1);
                sep.SetPointPosition(0, new Vector2(x - discount, 0));
                sep.SetPointPosition(1, new Vector2(x - discount, ChartHeight));
            }
        }

        private void ConfigureChartLine() {
            ChartLine.Width = 2f;
            ChartLine.DefaultColor = _color;
            _chartLineConfig(ChartLine);
            CurrentValue.Modulate = _color;
        }

        private void DumpDataToChartLine() {
            ChartLine.ClearPoints();
            var elements = _secondsHistory * 60;
            var range = MaxValue - MinValue;
            var i = 0;
            Data.ForEach(v => {
                var percentWidth = (float)i / elements;
                var percentHeight = (v - MinValue) / range;
                var x = Mathf.Lerp(0, ChartWidth, percentWidth);
                var y = Mathf.Lerp(0, ChartHeight, Math.Clamp(percentHeight, 0f, 1f));
                ChartLine.AddPoint(new Vector2(x, y));
                i++;
            });
        }

        public override void Process(float delta) {
            if (_dirty) {
                ConfigureChartLine();
                ConfigureChartData();
                ConfigureSeparators();
                ConfigureBorder();
                _chartSpacer.RectMinSize = new Vector2(ChartWidth, ChartHeight);
                _dirty = false;
            }
            var value = _loadValue.Invoke();
            CurrentValue.Text = _formatValue != null? _formatValue.Invoke(value) : value.ToString("F");
            Add(value);
            DumpDataToChartLine();
            UpdateSeparators();
            _frameCount ++;
            if (_frameCount == Fps) {
                if (IsAutoRange) {
                    MinValue = Data.Min();
                    MaxValue = Data.Max();
                }
                _frameCount = 0;
            }
        }
    }
}