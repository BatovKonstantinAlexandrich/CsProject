using System.Collections.Generic;

using Newtonsoft.Json;
using System;

namespace Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts
{
    [Label("Конфигурация Highcharts: Areaspline Chart")]
    [Description("Часть конфигурации диаграммы Highcharts: Areaspline Chart.")]
    public class HighchartAreasplineChart : Highchart
    {

        public HighchartAreasplineChart() : base()
        {
            Type = "areaspline";

        }

        [InputText("type")]
        [JsonProperty("type")]
        [Label("Type")]
        public string Type { get; set; }
    }
    [Label("Конфигурация Highcharts: Areaspline Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Areaspline Title.")]
    public class HighchartAreasplineTitle : Highchart
    {

        public HighchartAreasplineTitle() : base()
        {
            Text = "Average fruit consumption during one week";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Areaspline Legend Floating")]
    [Description("Часть конфигурации диаграммы Highcharts: Areaspline Legend Floating.")]
    public class HighchartAreasplineLegendFloating : Highchart
    {

        public HighchartAreasplineLegendFloating() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: Areaspline Legend")]
    [Description("Часть конфигурации диаграммы Highcharts: Areaspline Legend.")]
    public class HighchartAreasplineLegend : Highchart
    {

        public HighchartAreasplineLegend() : base()
        {
            Layout = "vertical";
            Align = "left";
            VerticalAlign = "top";
            X = 150f;
            Y = 100f;
            Floating = new HighchartAreasplineLegendFloating();
            BorderWidth = 1f;
            BackgroundColor = "#FFFFFF";

        }

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
        [InputNumber("x")]
        [JsonProperty("x")]
        [Label("X")]
        public float X { get; set; }
        [InputNumber("y")]
        [JsonProperty("y")]
        [Label("Y")]
        public float Y { get; set; }

        [JsonProperty("floating")]
        [Label("Floating")]
        public HighchartAreasplineLegendFloating Floating { get; set; }
        [InputNumber("borderWidth")]
        [JsonProperty("borderWidth")]
        [Label("Border Width")]
        public float BorderWidth { get; set; }
        [InputText("backgroundColor")]
        [JsonProperty("backgroundColor")]
        [Label("Background Color")]
        public string BackgroundColor { get; set; }
    }
    [Label("Конфигурация Highcharts: Areaspline X Axis Plot Bands Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Areaspline X Axis Plot Bands Item.")]
    public class HighchartAreasplineXAxisPlotBandsItem : Highchart
    {

        public HighchartAreasplineXAxisPlotBandsItem() : base()
        {
            From = 4.5f;
            To = 6.5f;
            Color = "rgba(68, 170, 213, .2)";

        }

        [InputNumber("from")]
        [JsonProperty("from")]
        [Label("From")]
        public float From { get; set; }
        [InputNumber("to")]
        [JsonProperty("to")]
        [Label("To")]
        public float To { get; set; }
        [InputText("color")]
        [JsonProperty("color")]
        [Label("Color")]
        public string Color { get; set; }
    }
    [Label("Конфигурация Highcharts: Areaspline X Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: Areaspline X Axis.")]
    public class HighchartAreasplineXAxis : Highchart
    {

        public HighchartAreasplineXAxis() : base()
        {
            Categories = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            PlotBands = new List<HighchartAreasplineXAxisPlotBandsItem> { new HighchartAreasplineXAxisPlotBandsItem() };

        }


        [JsonProperty("categories")]
        [Label("Categories")]
        public List<string> Categories { get; set; }

        [JsonProperty("plotBands")]
        [Label("Plot Bands")]
        public List<HighchartAreasplineXAxisPlotBandsItem> PlotBands { get; set; }
    }
    [Label("Конфигурация Highcharts: Areaspline Y Axis Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Areaspline Y Axis Title.")]
    public class HighchartAreasplineYAxisTitle : Highchart
    {

        public HighchartAreasplineYAxisTitle() : base()
        {
            Text = "Fruit units";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Areaspline Y Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: Areaspline Y Axis.")]
    public class HighchartAreasplineYAxis : Highchart
    {

        public HighchartAreasplineYAxis() : base()
        {
            Title = new HighchartAreasplineYAxisTitle();

        }


        [JsonProperty("title")]
        [Label("Title")]
        public HighchartAreasplineYAxisTitle Title { get; set; }
    }
    [Label("Конфигурация Highcharts: Areaspline Tooltip Shared")]
    [Description("Часть конфигурации диаграммы Highcharts: Areaspline Tooltip Shared.")]
    public class HighchartAreasplineTooltipShared : Highchart
    {

        public HighchartAreasplineTooltipShared() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: Areaspline Tooltip")]
    [Description("Часть конфигурации диаграммы Highcharts: Areaspline Tooltip.")]
    public class HighchartAreasplineTooltip : Highchart
    {

        public HighchartAreasplineTooltip() : base()
        {
            Shared = new HighchartAreasplineTooltipShared();
            ValueSuffix = " units";

        }


        [JsonProperty("shared")]
        [Label("Shared")]
        public HighchartAreasplineTooltipShared Shared { get; set; }
        [InputText("valueSuffix")]
        [JsonProperty("valueSuffix")]
        [Label("Value Suffix")]
        public string ValueSuffix { get; set; }
    }
    [Label("Конфигурация Highcharts: Areaspline Credits")]
    [Description("Часть конфигурации диаграммы Highcharts: Areaspline Credits.")]
    public class HighchartAreasplineCredits : Highchart
    {

        public HighchartAreasplineCredits() : base()
        {
            Enabled = null;

        }


        [JsonProperty("enabled")]
        [Label("Enabled")]
        public object Enabled { get; set; }
    }
    [Label("Конфигурация Highcharts: Areaspline Plot Options Areaspline")]
    [Description("Часть конфигурации диаграммы Highcharts: Areaspline Plot Options Areaspline.")]
    public class HighchartAreasplinePlotOptionsAreaspline : Highchart
    {

        public HighchartAreasplinePlotOptionsAreaspline() : base()
        {
            FillOpacity = 0.5f;

        }

        [InputNumber("fillOpacity")]
        [JsonProperty("fillOpacity")]
        [Label("Fill Opacity")]
        public float FillOpacity { get; set; }
    }
    [Label("Конфигурация Highcharts: Areaspline Plot Options")]
    [Description("Часть конфигурации диаграммы Highcharts: Areaspline Plot Options.")]
    public class HighchartAreasplinePlotOptions : Highchart
    {

        public HighchartAreasplinePlotOptions() : base()
        {
            Areaspline = new HighchartAreasplinePlotOptionsAreaspline();

        }


        [JsonProperty("areaspline")]
        [Label("Areaspline")]
        public HighchartAreasplinePlotOptionsAreaspline Areaspline { get; set; }
    }
    [Label("Конфигурация Highcharts: Areaspline Series Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Areaspline Series Item.")]
    public class HighchartAreasplineSeriesItem : Highchart
    {

        public HighchartAreasplineSeriesItem() : base()
        {
            Name = "John";
            Data = new List<float?> { 3f, 4f, 3f, 5f, 4f, 10f, 12f };

        }

        [InputText("name")]
        [JsonProperty("name")]
        [Label("Name")]
        public string Name { get; set; }

        [JsonProperty("data")]
        [Label("Data")]
        public List<float?> Data { get; set; }
    }
    [Label("Диаграмма Highcharts: Areaspline")]
    [Description("Модель диаграммы Highcharts: Areaspline.")]
    public class HighchartAreaspline : Highchart
    {

        public HighchartAreaspline() : base()
        {
            Chart = new HighchartAreasplineChart();
            Title = new HighchartAreasplineTitle();
            Legend = new HighchartAreasplineLegend();
            XAxis = new HighchartAreasplineXAxis();
            YAxis = new HighchartAreasplineYAxis();
            Tooltip = new HighchartAreasplineTooltip();
            Credits = new HighchartAreasplineCredits();
            PlotOptions = new HighchartAreasplinePlotOptions();
            Series = new List<HighchartAreasplineSeriesItem> { new HighchartAreasplineSeriesItem(), new HighchartAreasplineSeriesItem() };

        }


        [JsonProperty("chart")]
        public HighchartAreasplineChart Chart { get; set; }

        [JsonProperty("title")]
        public HighchartAreasplineTitle Title { get; set; }

        [JsonProperty("legend")]
        public HighchartAreasplineLegend Legend { get; set; }

        [JsonProperty("xAxis")]
        public HighchartAreasplineXAxis XAxis { get; set; }

        [JsonProperty("yAxis")]
        public HighchartAreasplineYAxis YAxis { get; set; }

        [JsonProperty("tooltip")]
        public HighchartAreasplineTooltip Tooltip { get; set; }

        [JsonProperty("credits")]
        public HighchartAreasplineCredits Credits { get; set; }

        [JsonProperty("plotOptions")]
        public HighchartAreasplinePlotOptions PlotOptions { get; set; }

        [JsonProperty("series")]
        public List<HighchartAreasplineSeriesItem> Series { get; set; }
    }


}