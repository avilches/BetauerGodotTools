using System;
using System.Collections.Generic;
using Godot;
using Object = Godot.Object;

namespace Betauer.Application.Monitor {
    public class MonitorGraph : BaseMonitor {
        private const int Fps = 60;
        private int _secondsHistory = 5;
        private int DataSize => _secondsHistory * Fps;
        private int _frameCount = 0;

        private readonly Node2D _separatorHolder = new();
        private readonly Control _chartSpacer = new();
        private readonly List<Line2D> _separators = new();
        private Action<Line2D> _chartLineConfig = (line2d) => { };
        private Action<Line2D> _borderConfig = (line2d) => { };
        private Action<Line2D> _separatorConfig = (line2d) => { };
        private readonly HBoxContainer _legendContainer = new();
        private Color _color = Colors.YellowGreen;
        private bool _dirty = true;

        public readonly Label Label = new();
        public readonly Label CurrentValue = new();
        private Func<float> _loadValue;
        private Func<float, string> _formatValue = (f) => f.ToString("F");
        public readonly LinkedList<float> Data = new();
        private readonly Line2D _borderLine = new();
        private readonly Line2D _chartLine = new();

        public int ChartHeight { get; private set; } = 10;
        public int ChartWidth { get; private set; } = 300;
        public float MaxValue { get; private set; }
        public float MinValue { get; private set; }
        
        public MonitorGraph RemoveIfInvalid(Object target) {
            Target = target;
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

        public MonitorGraph ChartSize(int width, int height) {
            if (width == ChartWidth && height == ChartHeight) return this; // ignore if there is no change
            ChartWidth = width;
            ChartHeight = height;
            _dirty = true;
            return this;
        }

        public MonitorGraph Range(float min, float max) {
            MinValue = Mathf.Min(min, max);
            MaxValue = Mathf.Max(min, max);
            return this;
        }

        public override void _Ready() {
            Label.Modulate = new Color(0.584314f, 0.584314f, 0.584314f, 1);
            _legendContainer.AddChild(Label);
            _legendContainer.AddChild(CurrentValue);
            AddChild(_chartSpacer);
            AddChild(_separatorHolder);
            AddChild(_chartLine);
            AddChild(_borderLine);
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
            _borderLine.ClearPoints();
            _borderLine.AddPoint(new Vector2(0, 0));
            _borderLine.AddPoint(new Vector2(ChartWidth, 0));
            _borderLine.AddPoint(new Vector2(ChartWidth, ChartHeight));
            _borderLine.AddPoint(new Vector2(0, ChartHeight));
            _borderLine.AddPoint(new Vector2(0, 0));
            _borderLine.Width = 1f;
            _borderLine.DefaultColor = Colors.DimGray;
            _borderConfig(_borderLine);
        }

        private void ConfigureSeparators() {
            var pending = _secondsHistory - _separators.Count;
            if (pending > 0) {
                while (pending-- > 0) {
                    var sep = new Line2D();
                    sep.Width = 2f;
                    sep.DefaultColor = Colors.DarkGray;
                    sep.Modulate = new Color(1f, 1f, 1f, 0.3f);
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
            _chartLine.Width = 2f;
            _chartLine.DefaultColor = _color;
            _chartLineConfig(_chartLine);
            CurrentValue.Modulate = _color;
        }

        private void DumpDataToChartLine() {
            _chartLine.ClearPoints();
            var elements = _secondsHistory * 60;
            var range = MaxValue - MinValue;
            var i = 0;
            Data.ForEach(v => {
                var percentWidth = (float)i / elements;
                var percentHeight = (v - MinValue) / range;
                var x = Mathf.Lerp(0, ChartWidth, percentWidth);
                var y = Mathf.Lerp(0, ChartHeight, Math.Clamp(percentHeight, 0f, 1f));
                _chartLine.AddPoint(new Vector2(x, y));
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
            CurrentValue.Text = _formatValue.Invoke(value);
            Add(value);
            DumpDataToChartLine();
            UpdateSeparators();
            _frameCount ++;
            if (_frameCount == Fps) _frameCount = 0;
        }
    }
}