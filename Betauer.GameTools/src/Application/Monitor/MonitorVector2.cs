using System;
using Betauer.Core.Nodes;
using Betauer.UI;
using Godot;

namespace Betauer.Application.Monitor;
public partial class MonitorVector2 : BaseMonitor<MonitorVector2> {

    public static readonly Color DefaultLineXColor = Colors.Red;
    public static readonly Color DefaultLineYColor = Colors.Blue;
    public static readonly Color DefaultLineLengthColor = new (0.9f,0,0.9f);
    
    private readonly HBoxContainer _legend = new();
    private readonly Control _chartSpacer = new();
    private bool _dirty = true;
    private Func<Vector2>? _loadValue;
    private Func<Vector2, string>? _formatValue;
    private Action<Line2D> _borderConfig = _ => { };

    public readonly Line2D LineX = new();
    public readonly Line2D LineY = new();
    public readonly Line2D LineLength = new();
    public Line2D BorderLine { get; } = new();
    public int ChartSize { get; private set; } = 100;
    public Label Label { get; } = new();
    public Label CurrentValue { get; } = new();
    public float MaxValue { get; private set; }

    public MonitorVector2 SetLabel(string? label) {
        if (label == null) {
            Label.Visible = false;
        } else {
            Label.Text = label;
            Label.Visible = true;
        }
        return this;
    }

    public MonitorVector2 Load(Func<Vector2> action, float maxValue) {
        _loadValue = action;
        MaxValue = maxValue;
        return this;
    }

    public MonitorVector2 LoadNormalized(Func<Vector2> action) {
        _loadValue = () => action().Normalized();
        MaxValue = 1f;
        return this;
    }

    public MonitorVector2 LoadAngle(Func<float> action) {
        _loadValue = () => {
            var angle = action();
            return Vector2.FromAngle(angle);
        };
        _formatValue = v2 => {
            var angle = action();
            return Mathf.RadToDeg(angle).ToString("0.0");
        };
        MaxValue = 1f;
        return this;
    }

    public MonitorVector2 Format(string format) {
        _formatValue = (v) => v.ToString(format);
        return this;
    }

    public MonitorVector2 Format(Func<Vector2, string> action) {
        _formatValue = action;
        return this;
    }

    public MonitorVector2 SetChartWidth(int width) {
        if (width == ChartSize) return this; // ignore if there is no change
        ChartSize = width;
        _dirty = true;
        return this;
    }

    public MonitorVector2 ConfigureBorderLine(Action<Line2D> conf) {
        _borderConfig = conf;
        _dirty = true;
        return this;
    }

    public override void _Ready() {
        LineX.AddPoint(Vector2.Zero);
        LineX.AddPoint(Vector2.Zero);
        LineX.DefaultColor = DefaultLineXColor;
        LineX.Width = 3;
        LineY.AddPoint(Vector2.Zero);
        LineY.AddPoint(Vector2.Zero);
        LineY.DefaultColor = DefaultLineYColor;
        LineY.Width = 3;
        LineLength.AddPoint(Vector2.Zero);
        LineLength.AddPoint(Vector2.Zero);
        LineLength.DefaultColor = DefaultLineLengthColor;
        LineLength.Width = 1;

        this.NodeBuilder()
            .Child(_chartSpacer).End()
            .Child(LineLength).End()
            .Child(LineX).End()
            .Child(LineY).End()
            .Child(BorderLine).End()
            .Child<Label>()
                .Child(_legend)
                    .Child(Label, label => {
                        label.HorizontalAlignment = HorizontalAlignment.Right;
                        label.SetFontColor(DefaultLabelColor);
                    })
                    .End()
                    .Child(CurrentValue, label => {
                        label.HorizontalAlignment = HorizontalAlignment.Left;
                    })
                    .End()
                .End()
            .End()
        .End();
    }

    public override void UpdateMonitor(double delta) {
        if (_dirty) {
            ConfigureChart();
        }
        var center = ChartSize / 2f;
        var value = _loadValue.Invoke();
        var valuePercentX = center * (value.X / MaxValue);
        var valuePercentY = center * (value.Y / MaxValue);
        LineX.SetPointPosition(1, new Vector2(center + valuePercentX, center));
        LineY.SetPointPosition(1, new Vector2(center, center + valuePercentY));
        LineLength.SetPointPosition(1, new Vector2(center + valuePercentX, center + valuePercentY));
        CurrentValue.Text = _formatValue != null ? _formatValue.Invoke(value) : value.ToString("000.00");
    }

    private void ConfigureChart() {
        SizeFlagsHorizontal = 0; // Don't expand, so the size will be the CustomMinimumSize
        CustomMinimumSize = new Vector2(ChartSize, 0);
        _chartSpacer.CustomMinimumSize = new Vector2(ChartSize, ChartSize);

        var center = ChartSize / 2;
        LineX.SetPointPosition(0, new Vector2(center, center));
        LineY.SetPointPosition(0, new Vector2(center, center));
        LineLength.SetPointPosition(0, new Vector2(center, center));

        _legend.GrowHorizontal = GrowDirection.Begin;
        _legend.SetAnchorsAndOffsetsPreset(LayoutPreset.RightWide);

        BorderLine.ClearPoints();
        BorderLine.AddPoint(new Vector2(0, 0));
        BorderLine.AddPoint(new Vector2(ChartSize, 0));
        BorderLine.AddPoint(new Vector2(ChartSize, ChartSize));
        BorderLine.AddPoint(new Vector2(0, ChartSize));
        BorderLine.AddPoint(new Vector2(0, 0));
        BorderLine.Width = 2f;
        BorderLine.DefaultColor = DefaultBorderColor;
        _borderConfig.Invoke(BorderLine);
    }
}