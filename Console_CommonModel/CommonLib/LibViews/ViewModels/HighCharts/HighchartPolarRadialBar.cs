using System.Collections.Generic;

using Newtonsoft.Json;
using System;

namespace Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts
{
    [Label("Конфигурация Highcharts: Polar Radial Bar Chart Inverted")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Radial Bar Chart Inverted.")]
    public class HighchartPolarRadialBarChartInverted : Highchart
    {

        public HighchartPolarRadialBarChartInverted() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: Polar Radial Bar Chart Polar")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Radial Bar Chart Polar.")]
    public class HighchartPolarRadialBarChartPolar : Highchart
    {

        public HighchartPolarRadialBarChartPolar() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: Polar Radial Bar Chart")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Radial Bar Chart.")]
    public class HighchartPolarRadialBarChart : Highchart
    {

        public HighchartPolarRadialBarChart() : base()
        {
            Type = "column";
            Inverted = new HighchartPolarRadialBarChartInverted();
            Polar = new HighchartPolarRadialBarChartPolar();

        }

        [InputText("type")]
        [JsonProperty("type")]
        [Label("Type")]
        public string Type { get; set; }

        [JsonProperty("inverted")]
        [Label("Inverted")]
        public HighchartPolarRadialBarChartInverted Inverted { get; set; }

        [JsonProperty("polar")]
        [Label("Polar")]
        public HighchartPolarRadialBarChartPolar Polar { get; set; }
    }
    [Label("Конфигурация Highcharts: Polar Radial Bar Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Radial Bar Title.")]
    public class HighchartPolarRadialBarTitle : Highchart
    {

        public HighchartPolarRadialBarTitle() : base()
        {
            Text = "Winter Olympic medals per existing country (TOP 5)";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Polar Radial Bar Tooltip Outside")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Radial Bar Tooltip Outside.")]
    public class HighchartPolarRadialBarTooltipOutside : Highchart
    {

        public HighchartPolarRadialBarTooltipOutside() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: Polar Radial Bar Tooltip")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Radial Bar Tooltip.")]
    public class HighchartPolarRadialBarTooltip : Highchart
    {

        public HighchartPolarRadialBarTooltip() : base()
        {
            Outside = new HighchartPolarRadialBarTooltipOutside();

        }


        [JsonProperty("outside")]
        [Label("Outside")]
        public HighchartPolarRadialBarTooltipOutside Outside { get; set; }
    }
    [Label("Конфигурация Highcharts: Polar Radial Bar Pane")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Radial Bar Pane.")]
    public class HighchartPolarRadialBarPane : Highchart
    {

        public HighchartPolarRadialBarPane() : base()
        {
            Size = "85%";
            InnerSize = "20%";
            EndAngle = 270f;

        }

        [InputText("size")]
        [JsonProperty("size")]
        [Label("Size")]
        public string Size { get; set; }
        [InputText("innerSize")]
        [JsonProperty("innerSize")]
        [Label("Inner Size")]
        public string InnerSize { get; set; }
        [InputNumber("endAngle")]
        [JsonProperty("endAngle")]
        [Label("End Angle")]
        public float EndAngle { get; set; }
    }
    [Label("Конфигурация Highcharts: Polar Radial Bar X Axis Labels Use HTML")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Radial Bar X Axis Labels Use HTML.")]
    public class HighchartPolarRadialBarXAxisLabelsUseHTML : Highchart
    {

        public HighchartPolarRadialBarXAxisLabelsUseHTML() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: Polar Radial Bar X Axis Labels Allow Overlap")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Radial Bar X Axis Labels Allow Overlap.")]
    public class HighchartPolarRadialBarXAxisLabelsAllowOverlap : Highchart
    {

        public HighchartPolarRadialBarXAxisLabelsAllowOverlap() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: Polar Radial Bar X Axis Labels Style")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Radial Bar X Axis Labels Style.")]
    public class HighchartPolarRadialBarXAxisLabelsStyle : Highchart
    {

        public HighchartPolarRadialBarXAxisLabelsStyle() : base()
        {
            FontSize = "13px";

        }

        [InputText("fontSize")]
        [JsonProperty("fontSize")]
        [Label("Font Size")]
        public string FontSize { get; set; }
    }
    [Label("Конфигурация Highcharts: Polar Radial Bar X Axis Labels")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Radial Bar X Axis Labels.")]
    public class HighchartPolarRadialBarXAxisLabels : Highchart
    {

        public HighchartPolarRadialBarXAxisLabels() : base()
        {
            Align = "right";
            UseHTML = new HighchartPolarRadialBarXAxisLabelsUseHTML();
            AllowOverlap = new HighchartPolarRadialBarXAxisLabelsAllowOverlap();
            Step = 1f;
            Y = 3f;
            Style = new HighchartPolarRadialBarXAxisLabelsStyle();

        }

        [InputText("align")]
        [JsonProperty("align")]
        [Label("Align")]
        public string Align { get; set; }

        [JsonProperty("useHTML")]
        [Label("Use HTML")]
        public HighchartPolarRadialBarXAxisLabelsUseHTML UseHTML { get; set; }

        [JsonProperty("allowOverlap")]
        [Label("Allow Overlap")]
        public HighchartPolarRadialBarXAxisLabelsAllowOverlap AllowOverlap { get; set; }
        [InputNumber("step")]
        [JsonProperty("step")]
        [Label("Step")]
        public float Step { get; set; }
        [InputNumber("y")]
        [JsonProperty("y")]
        [Label("Y")]
        public float Y { get; set; }

        [JsonProperty("style")]
        [Label("Style")]
        public HighchartPolarRadialBarXAxisLabelsStyle Style { get; set; }
    }
    [Label("Конфигурация Highcharts: Polar Radial Bar X Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Radial Bar X Axis.")]
    public class HighchartPolarRadialBarXAxis : Highchart
    {

        public HighchartPolarRadialBarXAxis() : base()
        {
            TickInterval = 1f;
            Labels = new HighchartPolarRadialBarXAxisLabels();
            LineWidth = null;
            Categories = new List<string> { "Norway <span class='f16'><span id='flag' class='flag no'></span></span>", "United States <span class='f16'><span id='flag' class='flag us'></span></span>", "Germany <span class='f16'><span id='flag' class='flag de'></span></span>", "Canada <span class='f16'><span id='flag' class='flag ca'></span></span>", "Austria <span class='f16'><span id='flag' class='flag at'></span></span>" };

        }

        [InputNumber("tickInterval")]
        [JsonProperty("tickInterval")]
        [Label("Tick Interval")]
        public float TickInterval { get; set; }

        [JsonProperty("labels")]
        [Label("Labels")]
        public HighchartPolarRadialBarXAxisLabels Labels { get; set; }

        [JsonProperty("lineWidth")]
        [Label("Line Width")]
        public object LineWidth { get; set; }

        [JsonProperty("categories")]
        [Label("Categories")]
        public List<string> Categories { get; set; }
    }
    [Label("Конфигурация Highcharts: Polar Radial Bar Y Axis Crosshair Enabled")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Radial Bar Y Axis Crosshair Enabled.")]
    public class HighchartPolarRadialBarYAxisCrosshairEnabled : Highchart
    {

        public HighchartPolarRadialBarYAxisCrosshairEnabled() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: Polar Radial Bar Y Axis Crosshair")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Radial Bar Y Axis Crosshair.")]
    public class HighchartPolarRadialBarYAxisCrosshair : Highchart
    {

        public HighchartPolarRadialBarYAxisCrosshair() : base()
        {
            Enabled = new HighchartPolarRadialBarYAxisCrosshairEnabled();
            Color = "#333";

        }


        [JsonProperty("enabled")]
        [Label("Enabled")]
        public HighchartPolarRadialBarYAxisCrosshairEnabled Enabled { get; set; }
        [InputText("color")]
        [JsonProperty("color")]
        [Label("Color")]
        public string Color { get; set; }
    }
    [Label("Конфигурация Highcharts: Polar Radial Bar Y Axis End On Tick")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Radial Bar Y Axis End On Tick.")]
    public class HighchartPolarRadialBarYAxisEndOnTick : Highchart
    {

        public HighchartPolarRadialBarYAxisEndOnTick() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: Polar Radial Bar Y Axis Show Last Label")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Radial Bar Y Axis Show Last Label.")]
    public class HighchartPolarRadialBarYAxisShowLastLabel : Highchart
    {

        public HighchartPolarRadialBarYAxisShowLastLabel() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: Polar Radial Bar Y Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Radial Bar Y Axis.")]
    public class HighchartPolarRadialBarYAxis : Highchart
    {

        public HighchartPolarRadialBarYAxis() : base()
        {
            Crosshair = new HighchartPolarRadialBarYAxisCrosshair();
            LineWidth = null;
            TickInterval = 25f;
            ReversedStacks = null;
            EndOnTick = new HighchartPolarRadialBarYAxisEndOnTick();
            ShowLastLabel = new HighchartPolarRadialBarYAxisShowLastLabel();

        }


        [JsonProperty("crosshair")]
        [Label("Crosshair")]
        public HighchartPolarRadialBarYAxisCrosshair Crosshair { get; set; }

        [JsonProperty("lineWidth")]
        [Label("Line Width")]
        public object LineWidth { get; set; }
        [InputNumber("tickInterval")]
        [JsonProperty("tickInterval")]
        [Label("Tick Interval")]
        public float TickInterval { get; set; }

        [JsonProperty("reversedStacks")]
        [Label("Reversed Stacks")]
        public object ReversedStacks { get; set; }

        [JsonProperty("endOnTick")]
        [Label("End On Tick")]
        public HighchartPolarRadialBarYAxisEndOnTick EndOnTick { get; set; }

        [JsonProperty("showLastLabel")]
        [Label("Show Last Label")]
        public HighchartPolarRadialBarYAxisShowLastLabel ShowLastLabel { get; set; }
    }
    [Label("Конфигурация Highcharts: Polar Radial Bar Plot Options Column")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Radial Bar Plot Options Column.")]
    public class HighchartPolarRadialBarPlotOptionsColumn : Highchart
    {

        public HighchartPolarRadialBarPlotOptionsColumn() : base()
        {
            Stacking = "normal";
            BorderWidth = null;
            PointPadding = null;
            GroupPadding = 0.15f;

        }

        [InputText("stacking")]
        [JsonProperty("stacking")]
        [Label("Stacking")]
        public string Stacking { get; set; }

        [JsonProperty("borderWidth")]
        [Label("Border Width")]
        public object BorderWidth { get; set; }

        [JsonProperty("pointPadding")]
        [Label("Point Padding")]
        public object PointPadding { get; set; }
        [InputNumber("groupPadding")]
        [JsonProperty("groupPadding")]
        [Label("Group Padding")]
        public float GroupPadding { get; set; }
    }
    [Label("Конфигурация Highcharts: Polar Radial Bar Plot Options")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Radial Bar Plot Options.")]
    public class HighchartPolarRadialBarPlotOptions : Highchart
    {

        public HighchartPolarRadialBarPlotOptions() : base()
        {
            Column = new HighchartPolarRadialBarPlotOptionsColumn();

        }


        [JsonProperty("column")]
        [Label("Column")]
        public HighchartPolarRadialBarPlotOptionsColumn Column { get; set; }
    }
    [Label("Конфигурация Highcharts: Polar Radial Bar Series Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Radial Bar Series Item.")]
    public class HighchartPolarRadialBarSeriesItem : Highchart
    {

        public HighchartPolarRadialBarSeriesItem() : base()
        {
            Name = "Gold medals";
            Data = new List<float?> { 132f, 105f, 92f, 73f, 64f };

        }

        [InputText("name")]
        [JsonProperty("name")]
        [Label("Name")]
        public string Name { get; set; }

        [JsonProperty("data")]
        [Label("Data")]
        public List<float?> Data { get; set; }
    }
    [Label("Диаграмма Highcharts: Polar Radial Bar")]
    [Description("Модель диаграммы Highcharts: Polar Radial Bar.")]
    public class HighchartPolarRadialBar : Highchart
    {

        public HighchartPolarRadialBar() : base()
        {

            Colors = new List<string> { "#FFD700", "#C0C0C0", "#CD7F32" };
            Chart = new HighchartPolarRadialBarChart();
            Title = new HighchartPolarRadialBarTitle();
            Tooltip = new HighchartPolarRadialBarTooltip();
            Pane = new HighchartPolarRadialBarPane();
            XAxis = new HighchartPolarRadialBarXAxis();
            YAxis = new HighchartPolarRadialBarYAxis();
            PlotOptions = new HighchartPolarRadialBarPlotOptions();
            Series = new List<HighchartPolarRadialBarSeriesItem> { new HighchartPolarRadialBarSeriesItem(), new HighchartPolarRadialBarSeriesItem(), new HighchartPolarRadialBarSeriesItem() };

        }


        [JsonProperty("colors")]
        public List<string> Colors { get; set; }

        [JsonProperty("chart")]
        public HighchartPolarRadialBarChart Chart { get; set; }

        [JsonProperty("title")]
        public HighchartPolarRadialBarTitle Title { get; set; }

        [JsonProperty("tooltip")]
        public HighchartPolarRadialBarTooltip Tooltip { get; set; }

        [JsonProperty("pane")]
        public HighchartPolarRadialBarPane Pane { get; set; }

        [JsonProperty("xAxis")]
        public HighchartPolarRadialBarXAxis XAxis { get; set; }

        [JsonProperty("yAxis")]
        public HighchartPolarRadialBarYAxis YAxis { get; set; }

        [JsonProperty("plotOptions")]
        public HighchartPolarRadialBarPlotOptions PlotOptions { get; set; }

        [JsonProperty("series")]
        public List<HighchartPolarRadialBarSeriesItem> Series { get; set; }
    }


}