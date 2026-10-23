using System.Collections.Generic;

using Newtonsoft.Json;
using System;

namespace Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts
{
    [Label("Конфигурация Highcharts: Area Inverted Chart Inverted")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Inverted Chart Inverted.")]
    public class HighchartAreaInvertedChartInverted : Highchart
    {

        public HighchartAreaInvertedChartInverted() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: Area Inverted Chart")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Inverted Chart.")]
    public class HighchartAreaInvertedChart : Highchart
    {

        public HighchartAreaInvertedChart() : base()
        {
            Type = "area";
            Inverted = new HighchartAreaInvertedChartInverted();

        }

        [InputText("type")]
        [JsonProperty("type")]
        [Label("Type")]
        public string Type { get; set; }

        [JsonProperty("inverted")]
        [Label("Inverted")]
        public HighchartAreaInvertedChartInverted Inverted { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Inverted Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Inverted Title.")]
    public class HighchartAreaInvertedTitle : Highchart
    {

        public HighchartAreaInvertedTitle() : base()
        {
            Text = "Average fruit consumption during one week";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Inverted Accessibility Keyboard Navigation Series Navigation")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Inverted Accessibility Keyboard Navigation Series Navigation.")]
    public class HighchartAreaInvertedAccessibilityKeyboardNavigationSeriesNavigation : Highchart
    {

        public HighchartAreaInvertedAccessibilityKeyboardNavigationSeriesNavigation() : base()
        {
            Mode = "serialize";

        }

        [InputText("mode")]
        [JsonProperty("mode")]
        [Label("Mode")]
        public string Mode { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Inverted Accessibility Keyboard Navigation")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Inverted Accessibility Keyboard Navigation.")]
    public class HighchartAreaInvertedAccessibilityKeyboardNavigation : Highchart
    {

        public HighchartAreaInvertedAccessibilityKeyboardNavigation() : base()
        {
            SeriesNavigation = new HighchartAreaInvertedAccessibilityKeyboardNavigationSeriesNavigation();

        }


        [JsonProperty("seriesNavigation")]
        [Label("Series Navigation")]
        public HighchartAreaInvertedAccessibilityKeyboardNavigationSeriesNavigation SeriesNavigation { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Inverted Accessibility")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Inverted Accessibility.")]
    public class HighchartAreaInvertedAccessibility : Highchart
    {

        public HighchartAreaInvertedAccessibility() : base()
        {
            KeyboardNavigation = new HighchartAreaInvertedAccessibilityKeyboardNavigation();

        }


        [JsonProperty("keyboardNavigation")]
        [Label("Keyboard Navigation")]
        public HighchartAreaInvertedAccessibilityKeyboardNavigation KeyboardNavigation { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Inverted Legend Floating")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Inverted Legend Floating.")]
    public class HighchartAreaInvertedLegendFloating : Highchart
    {

        public HighchartAreaInvertedLegendFloating() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: Area Inverted Legend")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Inverted Legend.")]
    public class HighchartAreaInvertedLegend : Highchart
    {

        public HighchartAreaInvertedLegend() : base()
        {
            Layout = "vertical";
            Align = "right";
            VerticalAlign = "top";
            X = -150f;
            Y = 100f;
            Floating = new HighchartAreaInvertedLegendFloating();
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
        public HighchartAreaInvertedLegendFloating Floating { get; set; }
        [InputNumber("borderWidth")]
        [JsonProperty("borderWidth")]
        [Label("Border Width")]
        public float BorderWidth { get; set; }
        [InputText("backgroundColor")]
        [JsonProperty("backgroundColor")]
        [Label("Background Color")]
        public string BackgroundColor { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Inverted X Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Inverted X Axis.")]
    public class HighchartAreaInvertedXAxis : Highchart
    {

        public HighchartAreaInvertedXAxis() : base()
        {
            Categories = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

        }


        [JsonProperty("categories")]
        [Label("Categories")]
        public List<string> Categories { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Inverted Y Axis Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Inverted Y Axis Title.")]
    public class HighchartAreaInvertedYAxisTitle : Highchart
    {

        public HighchartAreaInvertedYAxisTitle() : base()
        {
            Text = "Number of units";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Inverted Y Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Inverted Y Axis.")]
    public class HighchartAreaInvertedYAxis : Highchart
    {

        public HighchartAreaInvertedYAxis() : base()
        {
            Title = new HighchartAreaInvertedYAxisTitle();
            AllowDecimals = null;
            Min = null;

        }


        [JsonProperty("title")]
        [Label("Title")]
        public HighchartAreaInvertedYAxisTitle Title { get; set; }

        [JsonProperty("allowDecimals")]
        [Label("Allow Decimals")]
        public object AllowDecimals { get; set; }

        [JsonProperty("min")]
        [Label("Min")]
        public object Min { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Inverted Plot Options Area")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Inverted Plot Options Area.")]
    public class HighchartAreaInvertedPlotOptionsArea : Highchart
    {

        public HighchartAreaInvertedPlotOptionsArea() : base()
        {
            FillOpacity = 0.5f;

        }

        [InputNumber("fillOpacity")]
        [JsonProperty("fillOpacity")]
        [Label("Fill Opacity")]
        public float FillOpacity { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Inverted Plot Options")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Inverted Plot Options.")]
    public class HighchartAreaInvertedPlotOptions : Highchart
    {

        public HighchartAreaInvertedPlotOptions() : base()
        {
            Area = new HighchartAreaInvertedPlotOptionsArea();

        }


        [JsonProperty("area")]
        [Label("Area")]
        public HighchartAreaInvertedPlotOptionsArea Area { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Inverted Series Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Inverted Series Item.")]
    public class HighchartAreaInvertedSeriesItem : Highchart
    {

        public HighchartAreaInvertedSeriesItem() : base()
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
    [Label("Диаграмма Highcharts: Area Inverted")]
    [Description("Модель диаграммы Highcharts: Area Inverted.")]
    public class HighchartAreaInverted : Highchart
    {

        public HighchartAreaInverted() : base()
        {
            Chart = new HighchartAreaInvertedChart();
            Title = new HighchartAreaInvertedTitle();
            Accessibility = new HighchartAreaInvertedAccessibility();
            Legend = new HighchartAreaInvertedLegend();
            XAxis = new HighchartAreaInvertedXAxis();
            YAxis = new HighchartAreaInvertedYAxis();
            PlotOptions = new HighchartAreaInvertedPlotOptions();
            Series = new List<HighchartAreaInvertedSeriesItem> { new HighchartAreaInvertedSeriesItem(), new HighchartAreaInvertedSeriesItem() };

        }


        [JsonProperty("chart")]
        public HighchartAreaInvertedChart Chart { get; set; }

        [JsonProperty("title")]
        public HighchartAreaInvertedTitle Title { get; set; }

        [JsonProperty("accessibility")]
        public HighchartAreaInvertedAccessibility Accessibility { get; set; }

        [JsonProperty("legend")]
        public HighchartAreaInvertedLegend Legend { get; set; }

        [JsonProperty("xAxis")]
        public HighchartAreaInvertedXAxis XAxis { get; set; }

        [JsonProperty("yAxis")]
        public HighchartAreaInvertedYAxis YAxis { get; set; }

        [JsonProperty("plotOptions")]
        public HighchartAreaInvertedPlotOptions PlotOptions { get; set; }

        [JsonProperty("series")]
        public List<HighchartAreaInvertedSeriesItem> Series { get; set; }
    }


}