using System.Collections.Generic;

using Newtonsoft.Json;
using System;

namespace Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts
{
    [Label("Конфигурация Highcharts: Combo Multi Axes Chart")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Multi Axes Chart.")]
    public class HighchartComboMultiAxesChart : Highchart
    {

        public HighchartComboMultiAxesChart() : base()
        {
            ZoomType = "xy";

        }

        [InputText("zoomType")]
        [JsonProperty("zoomType")]
        [Label("Zoom Type")]
        public string ZoomType { get; set; }
    }
    [Label("Конфигурация Highcharts: Combo Multi Axes Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Multi Axes Title.")]
    public class HighchartComboMultiAxesTitle : Highchart
    {

        public HighchartComboMultiAxesTitle() : base()
        {
            Text = "Average Monthly Weather Data for Tokyo";
            Align = "left";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
        [InputText("align")]
        [JsonProperty("align")]
        [Label("Align")]
        public string Align { get; set; }
    }
    [Label("Конфигурация Highcharts: Combo Multi Axes Subtitle")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Multi Axes Subtitle.")]
    public class HighchartComboMultiAxesSubtitle : Highchart
    {

        public HighchartComboMultiAxesSubtitle() : base()
        {
            Text = "Source: WorldClimate.com";
            Align = "left";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
        [InputText("align")]
        [JsonProperty("align")]
        [Label("Align")]
        public string Align { get; set; }
    }
    [Label("Конфигурация Highcharts: Combo Multi Axes X Axis Item Crosshair")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Multi Axes X Axis Item Crosshair.")]
    public class HighchartComboMultiAxesXAxisItemCrosshair : Highchart
    {

        public HighchartComboMultiAxesXAxisItemCrosshair() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: Combo Multi Axes X Axis Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Multi Axes X Axis Item.")]
    public class HighchartComboMultiAxesXAxisItem : Highchart
    {

        public HighchartComboMultiAxesXAxisItem() : base()
        {
            Categories = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            Crosshair = new HighchartComboMultiAxesXAxisItemCrosshair();

        }


        [JsonProperty("categories")]
        [Label("Categories")]
        public List<string> Categories { get; set; }

        [JsonProperty("crosshair")]
        [Label("Crosshair")]
        public HighchartComboMultiAxesXAxisItemCrosshair Crosshair { get; set; }
    }
    [Label("Конфигурация Highcharts: Combo Multi Axes Y Axis Item Labels Style")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Multi Axes Y Axis Item Labels Style.")]
    public class HighchartComboMultiAxesYAxisItemLabelsStyle : Highchart
    {

        public HighchartComboMultiAxesYAxisItemLabelsStyle() : base()
        {
            Color = "#90ed7d";

        }

        [InputText("color")]
        [JsonProperty("color")]
        [Label("Color")]
        public string Color { get; set; }
    }
    [Label("Конфигурация Highcharts: Combo Multi Axes Y Axis Item Labels")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Multi Axes Y Axis Item Labels.")]
    public class HighchartComboMultiAxesYAxisItemLabels : Highchart
    {

        public HighchartComboMultiAxesYAxisItemLabels() : base()
        {
            Format = "{value}°C";
            Style = new HighchartComboMultiAxesYAxisItemLabelsStyle();

        }

        [InputText("format")]
        [JsonProperty("format")]
        [Label("Format")]
        public string Format { get; set; }

        [JsonProperty("style")]
        [Label("Style")]
        public HighchartComboMultiAxesYAxisItemLabelsStyle Style { get; set; }
    }
    [Label("Конфигурация Highcharts: Combo Multi Axes Y Axis Item Title Style")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Multi Axes Y Axis Item Title Style.")]
    public class HighchartComboMultiAxesYAxisItemTitleStyle : Highchart
    {

        public HighchartComboMultiAxesYAxisItemTitleStyle() : base()
        {
            Color = "#90ed7d";

        }

        [InputText("color")]
        [JsonProperty("color")]
        [Label("Color")]
        public string Color { get; set; }
    }
    [Label("Конфигурация Highcharts: Combo Multi Axes Y Axis Item Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Multi Axes Y Axis Item Title.")]
    public class HighchartComboMultiAxesYAxisItemTitle : Highchart
    {

        public HighchartComboMultiAxesYAxisItemTitle() : base()
        {
            Text = "Temperature";
            Style = new HighchartComboMultiAxesYAxisItemTitleStyle();

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }

        [JsonProperty("style")]
        [Label("Style")]
        public HighchartComboMultiAxesYAxisItemTitleStyle Style { get; set; }
    }
    [Label("Конфигурация Highcharts: Combo Multi Axes Y Axis Item Opposite")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Multi Axes Y Axis Item Opposite.")]
    public class HighchartComboMultiAxesYAxisItemOpposite : Highchart
    {

        public HighchartComboMultiAxesYAxisItemOpposite() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: Combo Multi Axes Y Axis Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Multi Axes Y Axis Item.")]
    public class HighchartComboMultiAxesYAxisItem : Highchart
    {

        public HighchartComboMultiAxesYAxisItem() : base()
        {
            Labels = new HighchartComboMultiAxesYAxisItemLabels();
            Title = new HighchartComboMultiAxesYAxisItemTitle();
            Opposite = new HighchartComboMultiAxesYAxisItemOpposite();

        }


        [JsonProperty("labels")]
        [Label("Labels")]
        public HighchartComboMultiAxesYAxisItemLabels Labels { get; set; }

        [JsonProperty("title")]
        [Label("Title")]
        public HighchartComboMultiAxesYAxisItemTitle Title { get; set; }

        [JsonProperty("opposite")]
        [Label("Opposite")]
        public HighchartComboMultiAxesYAxisItemOpposite Opposite { get; set; }
    }
    [Label("Конфигурация Highcharts: Combo Multi Axes Tooltip Shared")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Multi Axes Tooltip Shared.")]
    public class HighchartComboMultiAxesTooltipShared : Highchart
    {

        public HighchartComboMultiAxesTooltipShared() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: Combo Multi Axes Tooltip")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Multi Axes Tooltip.")]
    public class HighchartComboMultiAxesTooltip : Highchart
    {

        public HighchartComboMultiAxesTooltip() : base()
        {
            Shared = new HighchartComboMultiAxesTooltipShared();

        }


        [JsonProperty("shared")]
        [Label("Shared")]
        public HighchartComboMultiAxesTooltipShared Shared { get; set; }
    }
    [Label("Конфигурация Highcharts: Combo Multi Axes Legend Floating")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Multi Axes Legend Floating.")]
    public class HighchartComboMultiAxesLegendFloating : Highchart
    {

        public HighchartComboMultiAxesLegendFloating() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: Combo Multi Axes Legend")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Multi Axes Legend.")]
    public class HighchartComboMultiAxesLegend : Highchart
    {

        public HighchartComboMultiAxesLegend() : base()
        {
            Layout = "vertical";
            Align = "left";
            X = 80f;
            VerticalAlign = "top";
            Y = 55f;
            Floating = new HighchartComboMultiAxesLegendFloating();
            BackgroundColor = "rgba(255,255,255,0.25)";

        }

        [InputText("layout")]
        [JsonProperty("layout")]
        [Label("Layout")]
        public string Layout { get; set; }
        [InputText("align")]
        [JsonProperty("align")]
        [Label("Align")]
        public string Align { get; set; }
        [InputNumber("x")]
        [JsonProperty("x")]
        [Label("X")]
        public float X { get; set; }
        [InputText("verticalAlign")]
        [JsonProperty("verticalAlign")]
        [Label("Vertical Align")]
        public string VerticalAlign { get; set; }
        [InputNumber("y")]
        [JsonProperty("y")]
        [Label("Y")]
        public float Y { get; set; }

        [JsonProperty("floating")]
        [Label("Floating")]
        public HighchartComboMultiAxesLegendFloating Floating { get; set; }
        [InputText("backgroundColor")]
        [JsonProperty("backgroundColor")]
        [Label("Background Color")]
        public string BackgroundColor { get; set; }
    }
    [Label("Конфигурация Highcharts: Combo Multi Axes Series Item Tooltip")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Multi Axes Series Item Tooltip.")]
    public class HighchartComboMultiAxesSeriesItemTooltip : Highchart
    {

        public HighchartComboMultiAxesSeriesItemTooltip() : base()
        {
            ValueSuffix = " mm";

        }

        [InputText("valueSuffix")]
        [JsonProperty("valueSuffix")]
        [Label("Value Suffix")]
        public string ValueSuffix { get; set; }
    }
    [Label("Конфигурация Highcharts: Combo Multi Axes Series Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Multi Axes Series Item.")]
    public class HighchartComboMultiAxesSeriesItem : Highchart
    {

        public HighchartComboMultiAxesSeriesItem() : base()
        {
            Name = "Rainfall";
            Type = "column";
            YAxis = 1f;
            Data = new List<float?> { 49.9f, 71.5f, 106.4f, 129.2f, 144f, 176f, 135.6f, 148.5f, 216.4f, 194.1f, 95.6f, 54.4f };
            Tooltip = new HighchartComboMultiAxesSeriesItemTooltip();

        }

        [InputText("name")]
        [JsonProperty("name")]
        [Label("Name")]
        public string Name { get; set; }
        [InputText("type")]
        [JsonProperty("type")]
        [Label("Type")]
        public string Type { get; set; }
        [InputNumber("yAxis")]
        [JsonProperty("yAxis")]
        [Label("Y Axis")]
        public float YAxis { get; set; }

        [JsonProperty("data")]
        [Label("Data")]
        public List<float?> Data { get; set; }

        [JsonProperty("tooltip")]
        [Label("Tooltip")]
        public HighchartComboMultiAxesSeriesItemTooltip Tooltip { get; set; }
    }
    [Label("Конфигурация Highcharts: Combo Multi Axes Responsive Rules Item Condition")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Multi Axes Responsive Rules Item Condition.")]
    public class HighchartComboMultiAxesResponsiveRulesItemCondition : Highchart
    {

        public HighchartComboMultiAxesResponsiveRulesItemCondition() : base()
        {
            MaxWidth = 500f;

        }

        [InputNumber("maxWidth")]
        [JsonProperty("maxWidth")]
        [Label("Max Width")]
        public float MaxWidth { get; set; }
    }
    [Label("Конфигурация Highcharts: Combo Multi Axes Responsive Rules Item Chart Options Legend")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Multi Axes Responsive Rules Item Chart Options Legend.")]
    public class HighchartComboMultiAxesResponsiveRulesItemChartOptionsLegend : Highchart
    {

        public HighchartComboMultiAxesResponsiveRulesItemChartOptionsLegend() : base()
        {
            Floating = null;
            Layout = "horizontal";
            Align = "center";
            VerticalAlign = "bottom";
            X = null;
            Y = null;

        }


        [JsonProperty("floating")]
        [Label("Floating")]
        public object Floating { get; set; }
        [InputText("layout")]
        [JsonProperty("layout")]
        [Label("Layout")]
        public string Layout { get; set; }
        [InputText("align")]
        [JsonProperty("align")]
        [Label("Align")]
        public string Align { get; set; }
        [InputText("verticalAlign")]
        [JsonProperty("verticalAlign")]
        [Label("Vertical Align")]
        public string VerticalAlign { get; set; }

        [JsonProperty("x")]
        [Label("X")]
        public object X { get; set; }

        [JsonProperty("y")]
        [Label("Y")]
        public object Y { get; set; }
    }
    [Label("Конфигурация Highcharts: Combo Multi Axes Responsive Rules Item Chart Options Y Axis Item Labels")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Multi Axes Responsive Rules Item Chart Options Y Axis Item Labels.")]
    public class HighchartComboMultiAxesResponsiveRulesItemChartOptionsYAxisItemLabels : Highchart
    {

        public HighchartComboMultiAxesResponsiveRulesItemChartOptionsYAxisItemLabels() : base()
        {
            Align = "right";
            X = null;
            Y = -6f;

        }

        [InputText("align")]
        [JsonProperty("align")]
        [Label("Align")]
        public string Align { get; set; }

        [JsonProperty("x")]
        [Label("X")]
        public object X { get; set; }
        [InputNumber("y")]
        [JsonProperty("y")]
        [Label("Y")]
        public float Y { get; set; }
    }
    [Label("Конфигурация Highcharts: Combo Multi Axes Responsive Rules Item Chart Options Y Axis Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Multi Axes Responsive Rules Item Chart Options Y Axis Item.")]
    public class HighchartComboMultiAxesResponsiveRulesItemChartOptionsYAxisItem : Highchart
    {

        public HighchartComboMultiAxesResponsiveRulesItemChartOptionsYAxisItem() : base()
        {
            Labels = new HighchartComboMultiAxesResponsiveRulesItemChartOptionsYAxisItemLabels();
            ShowLastLabel = null;

        }


        [JsonProperty("labels")]
        [Label("Labels")]
        public HighchartComboMultiAxesResponsiveRulesItemChartOptionsYAxisItemLabels Labels { get; set; }

        [JsonProperty("showLastLabel")]
        [Label("Show Last Label")]
        public object ShowLastLabel { get; set; }
    }
    [Label("Конфигурация Highcharts: Combo Multi Axes Responsive Rules Item Chart Options")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Multi Axes Responsive Rules Item Chart Options.")]
    public class HighchartComboMultiAxesResponsiveRulesItemChartOptions : Highchart
    {

        public HighchartComboMultiAxesResponsiveRulesItemChartOptions() : base()
        {
            Legend = new HighchartComboMultiAxesResponsiveRulesItemChartOptionsLegend();
            YAxis = new List<HighchartComboMultiAxesResponsiveRulesItemChartOptionsYAxisItem> { new HighchartComboMultiAxesResponsiveRulesItemChartOptionsYAxisItem(), new HighchartComboMultiAxesResponsiveRulesItemChartOptionsYAxisItem(), new HighchartComboMultiAxesResponsiveRulesItemChartOptionsYAxisItem() };

        }


        [JsonProperty("legend")]
        [Label("Legend")]
        public HighchartComboMultiAxesResponsiveRulesItemChartOptionsLegend Legend { get; set; }

        [JsonProperty("yAxis")]
        [Label("Y Axis")]
        public List<HighchartComboMultiAxesResponsiveRulesItemChartOptionsYAxisItem> YAxis { get; set; }
    }
    [Label("Конфигурация Highcharts: Combo Multi Axes Responsive Rules Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Multi Axes Responsive Rules Item.")]
    public class HighchartComboMultiAxesResponsiveRulesItem : Highchart
    {

        public HighchartComboMultiAxesResponsiveRulesItem() : base()
        {
            Condition = new HighchartComboMultiAxesResponsiveRulesItemCondition();
            ChartOptions = new HighchartComboMultiAxesResponsiveRulesItemChartOptions();

        }


        [JsonProperty("condition")]
        [Label("Condition")]
        public HighchartComboMultiAxesResponsiveRulesItemCondition Condition { get; set; }

        [JsonProperty("chartOptions")]
        [Label("Chart Options")]
        public HighchartComboMultiAxesResponsiveRulesItemChartOptions ChartOptions { get; set; }
    }
    [Label("Конфигурация Highcharts: Combo Multi Axes Responsive")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Multi Axes Responsive.")]
    public class HighchartComboMultiAxesResponsive : Highchart
    {

        public HighchartComboMultiAxesResponsive() : base()
        {
            Rules = new List<HighchartComboMultiAxesResponsiveRulesItem> { new HighchartComboMultiAxesResponsiveRulesItem() };

        }


        [JsonProperty("rules")]
        public List<HighchartComboMultiAxesResponsiveRulesItem> Rules { get; set; }
    }
    [Label("Диаграмма Highcharts: Combo Multi Axes")]
    [Description("Модель диаграммы Highcharts: Combo Multi Axes.")]
    public class HighchartComboMultiAxes : Highchart
    {

        public HighchartComboMultiAxes() : base()
        {
            Chart = new HighchartComboMultiAxesChart();
            Title = new HighchartComboMultiAxesTitle();
            Subtitle = new HighchartComboMultiAxesSubtitle();
            XAxis = new List<HighchartComboMultiAxesXAxisItem> { new HighchartComboMultiAxesXAxisItem() };
            YAxis = new List<HighchartComboMultiAxesYAxisItem> { new HighchartComboMultiAxesYAxisItem(), new HighchartComboMultiAxesYAxisItem(), new HighchartComboMultiAxesYAxisItem() };
            Tooltip = new HighchartComboMultiAxesTooltip();
            Legend = new HighchartComboMultiAxesLegend();
            Series = new List<HighchartComboMultiAxesSeriesItem> { new HighchartComboMultiAxesSeriesItem(), new HighchartComboMultiAxesSeriesItem(), new HighchartComboMultiAxesSeriesItem() };
            Responsive = new HighchartComboMultiAxesResponsive();

        }


        [JsonProperty("chart")]
        public HighchartComboMultiAxesChart Chart { get; set; }

        [JsonProperty("title")]
        public HighchartComboMultiAxesTitle Title { get; set; }

        [JsonProperty("subtitle")]
        public HighchartComboMultiAxesSubtitle Subtitle { get; set; }

        [JsonProperty("xAxis")]
        public List<HighchartComboMultiAxesXAxisItem> XAxis { get; set; }

        [JsonProperty("yAxis")]
        public List<HighchartComboMultiAxesYAxisItem> YAxis { get; set; }

        [JsonProperty("tooltip")]
        public HighchartComboMultiAxesTooltip Tooltip { get; set; }

        [JsonProperty("legend")]
        public HighchartComboMultiAxesLegend Legend { get; set; }

        [JsonProperty("series")]
        public List<HighchartComboMultiAxesSeriesItem> Series { get; set; }

        [JsonProperty("responsive")]
        public HighchartComboMultiAxesResponsive Responsive { get; set; }
    }


}