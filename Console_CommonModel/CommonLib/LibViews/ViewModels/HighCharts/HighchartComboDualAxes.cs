using System.Collections.Generic;

using Newtonsoft.Json;
using System;

namespace Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts
{
    [Label("Конфигурация Highcharts: Combo Dual Axes Chart")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Dual Axes Chart.")]
    public class HighchartComboDualAxesChart : Highchart
    {

        public HighchartComboDualAxesChart() : base()
        {
            ZoomType = "xy";

        }

        [InputText("zoomType")]
        [JsonProperty("zoomType")]
        [Label("Zoom Type")]
        public string ZoomType { get; set; }
    }
    [Label("Конфигурация Highcharts: Combo Dual Axes Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Dual Axes Title.")]
    public class HighchartComboDualAxesTitle : Highchart
    {

        public HighchartComboDualAxesTitle() : base()
        {
            Text = "Average Monthly Temperature and Rainfall in Tokyo";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Combo Dual Axes Subtitle")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Dual Axes Subtitle.")]
    public class HighchartComboDualAxesSubtitle : Highchart
    {

        public HighchartComboDualAxesSubtitle() : base()
        {
            Text = "Source: WorldClimate.com";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Combo Dual Axes X Axis Item Crosshair")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Dual Axes X Axis Item Crosshair.")]
    public class HighchartComboDualAxesXAxisItemCrosshair : Highchart
    {

        public HighchartComboDualAxesXAxisItemCrosshair() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: Combo Dual Axes X Axis Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Dual Axes X Axis Item.")]
    public class HighchartComboDualAxesXAxisItem : Highchart
    {

        public HighchartComboDualAxesXAxisItem() : base()
        {
            Categories = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            Crosshair = new HighchartComboDualAxesXAxisItemCrosshair();

        }


        [JsonProperty("categories")]
        [Label("Categories")]
        public List<string> Categories { get; set; }

        [JsonProperty("crosshair")]
        [Label("Crosshair")]
        public HighchartComboDualAxesXAxisItemCrosshair Crosshair { get; set; }
    }
    [Label("Конфигурация Highcharts: Combo Dual Axes Y Axis Item Labels Style")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Dual Axes Y Axis Item Labels Style.")]
    public class HighchartComboDualAxesYAxisItemLabelsStyle : Highchart
    {

        public HighchartComboDualAxesYAxisItemLabelsStyle() : base()
        {
            Color = "#434348";

        }

        [InputText("color")]
        [JsonProperty("color")]
        [Label("Color")]
        public string Color { get; set; }
    }
    [Label("Конфигурация Highcharts: Combo Dual Axes Y Axis Item Labels")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Dual Axes Y Axis Item Labels.")]
    public class HighchartComboDualAxesYAxisItemLabels : Highchart
    {

        public HighchartComboDualAxesYAxisItemLabels() : base()
        {
            Format = "{value}°C";
            Style = new HighchartComboDualAxesYAxisItemLabelsStyle();

        }

        [InputText("format")]
        [JsonProperty("format")]
        [Label("Format")]
        public string Format { get; set; }

        [JsonProperty("style")]
        [Label("Style")]
        public HighchartComboDualAxesYAxisItemLabelsStyle Style { get; set; }
    }
    [Label("Конфигурация Highcharts: Combo Dual Axes Y Axis Item Title Style")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Dual Axes Y Axis Item Title Style.")]
    public class HighchartComboDualAxesYAxisItemTitleStyle : Highchart
    {

        public HighchartComboDualAxesYAxisItemTitleStyle() : base()
        {
            Color = "#434348";

        }

        [InputText("color")]
        [JsonProperty("color")]
        [Label("Color")]
        public string Color { get; set; }
    }
    [Label("Конфигурация Highcharts: Combo Dual Axes Y Axis Item Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Dual Axes Y Axis Item Title.")]
    public class HighchartComboDualAxesYAxisItemTitle : Highchart
    {

        public HighchartComboDualAxesYAxisItemTitle() : base()
        {
            Text = "Temperature";
            Style = new HighchartComboDualAxesYAxisItemTitleStyle();

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }

        [JsonProperty("style")]
        [Label("Style")]
        public HighchartComboDualAxesYAxisItemTitleStyle Style { get; set; }
    }
    [Label("Конфигурация Highcharts: Combo Dual Axes Y Axis Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Dual Axes Y Axis Item.")]
    public class HighchartComboDualAxesYAxisItem : Highchart
    {

        public HighchartComboDualAxesYAxisItem() : base()
        {
            Labels = new HighchartComboDualAxesYAxisItemLabels();
            Title = new HighchartComboDualAxesYAxisItemTitle();

        }


        [JsonProperty("labels")]
        [Label("Labels")]
        public HighchartComboDualAxesYAxisItemLabels Labels { get; set; }

        [JsonProperty("title")]
        [Label("Title")]
        public HighchartComboDualAxesYAxisItemTitle Title { get; set; }
    }
    [Label("Конфигурация Highcharts: Combo Dual Axes Tooltip Shared")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Dual Axes Tooltip Shared.")]
    public class HighchartComboDualAxesTooltipShared : Highchart
    {

        public HighchartComboDualAxesTooltipShared() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: Combo Dual Axes Tooltip")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Dual Axes Tooltip.")]
    public class HighchartComboDualAxesTooltip : Highchart
    {

        public HighchartComboDualAxesTooltip() : base()
        {
            Shared = new HighchartComboDualAxesTooltipShared();

        }


        [JsonProperty("shared")]
        [Label("Shared")]
        public HighchartComboDualAxesTooltipShared Shared { get; set; }
    }
    [Label("Конфигурация Highcharts: Combo Dual Axes Legend Floating")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Dual Axes Legend Floating.")]
    public class HighchartComboDualAxesLegendFloating : Highchart
    {

        public HighchartComboDualAxesLegendFloating() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: Combo Dual Axes Legend")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Dual Axes Legend.")]
    public class HighchartComboDualAxesLegend : Highchart
    {

        public HighchartComboDualAxesLegend() : base()
        {
            Layout = "vertical";
            Align = "left";
            X = 120f;
            VerticalAlign = "top";
            Y = 100f;
            Floating = new HighchartComboDualAxesLegendFloating();
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
        public HighchartComboDualAxesLegendFloating Floating { get; set; }
        [InputText("backgroundColor")]
        [JsonProperty("backgroundColor")]
        [Label("Background Color")]
        public string BackgroundColor { get; set; }
    }
    [Label("Конфигурация Highcharts: Combo Dual Axes Series Item Tooltip")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Dual Axes Series Item Tooltip.")]
    public class HighchartComboDualAxesSeriesItemTooltip : Highchart
    {

        public HighchartComboDualAxesSeriesItemTooltip() : base()
        {
            ValueSuffix = " mm";

        }

        [InputText("valueSuffix")]
        [JsonProperty("valueSuffix")]
        [Label("Value Suffix")]
        public string ValueSuffix { get; set; }
    }
    [Label("Конфигурация Highcharts: Combo Dual Axes Series Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Combo Dual Axes Series Item.")]
    public class HighchartComboDualAxesSeriesItem : Highchart
    {

        public HighchartComboDualAxesSeriesItem() : base()
        {
            Name = "Rainfall";
            Type = "column";
            YAxis = 1f;
            Data = new List<float?> { 49.9f, 71.5f, 106.4f, 129.2f, 144f, 176f, 135.6f, 148.5f, 216.4f, 194.1f, 95.6f, 54.4f };
            Tooltip = new HighchartComboDualAxesSeriesItemTooltip();

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
        public HighchartComboDualAxesSeriesItemTooltip Tooltip { get; set; }
    }
    [Label("Диаграмма Highcharts: Combo Dual Axes")]
    [Description("Модель диаграммы Highcharts: Combo Dual Axes.")]
    public class HighchartComboDualAxes : Highchart
    {

        public HighchartComboDualAxes() : base()
        {
            Chart = new HighchartComboDualAxesChart();
            Title = new HighchartComboDualAxesTitle();
            Subtitle = new HighchartComboDualAxesSubtitle();
            XAxis = new List<HighchartComboDualAxesXAxisItem> { new HighchartComboDualAxesXAxisItem() };
            YAxis = new List<HighchartComboDualAxesYAxisItem> { new HighchartComboDualAxesYAxisItem(), new HighchartComboDualAxesYAxisItem() };
            Tooltip = new HighchartComboDualAxesTooltip();
            Legend = new HighchartComboDualAxesLegend();
            Series = new List<HighchartComboDualAxesSeriesItem> { new HighchartComboDualAxesSeriesItem(), new HighchartComboDualAxesSeriesItem() };

        }


        [JsonProperty("chart")]
        public HighchartComboDualAxesChart Chart { get; set; }

        [JsonProperty("title")]
        public HighchartComboDualAxesTitle Title { get; set; }

        [JsonProperty("subtitle")]
        public HighchartComboDualAxesSubtitle Subtitle { get; set; }

        [JsonProperty("xAxis")]
        public List<HighchartComboDualAxesXAxisItem> XAxis { get; set; }

        [JsonProperty("yAxis")]
        public List<HighchartComboDualAxesYAxisItem> YAxis { get; set; }

        [JsonProperty("tooltip")]
        public HighchartComboDualAxesTooltip Tooltip { get; set; }

        [JsonProperty("legend")]
        public HighchartComboDualAxesLegend Legend { get; set; }

        [JsonProperty("series")]
        public List<HighchartComboDualAxesSeriesItem> Series { get; set; }
    }


}