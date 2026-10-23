using System.Collections.Generic;

using Newtonsoft.Json;
using System;

namespace Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts
{
    [Label("Конфигурация Highcharts: Error Bar Chart")]
    [Description("Часть конфигурации диаграммы Highcharts: Error Bar Chart.")]
    public class HighchartErrorBarChart : Highchart
    {

        public HighchartErrorBarChart() : base()
        {
            ZoomType = "xy";

        }

        [InputText("zoomType")]
        [JsonProperty("zoomType")]
        [Label("Zoom Type")]
        public string ZoomType { get; set; }
    }
    [Label("Конфигурация Highcharts: Error Bar Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Error Bar Title.")]
    public class HighchartErrorBarTitle : Highchart
    {

        public HighchartErrorBarTitle() : base()
        {
            Text = "Temperature vs Rainfall";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Error Bar X Axis Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Error Bar X Axis Item.")]
    public class HighchartErrorBarXAxisItem : Highchart
    {

        public HighchartErrorBarXAxisItem() : base()
        {
            Categories = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        }


        [JsonProperty("categories")]
        [Label("Categories")]
        public List<string> Categories { get; set; }
    }
    [Label("Конфигурация Highcharts: Error Bar Y Axis Item Labels Style")]
    [Description("Часть конфигурации диаграммы Highcharts: Error Bar Y Axis Item Labels Style.")]
    public class HighchartErrorBarYAxisItemLabelsStyle : Highchart
    {

        public HighchartErrorBarYAxisItemLabelsStyle() : base()
        {
            Color = "#434348";

        }

        [InputText("color")]
        [JsonProperty("color")]
        [Label("Color")]
        public string Color { get; set; }
    }
    [Label("Конфигурация Highcharts: Error Bar Y Axis Item Labels")]
    [Description("Часть конфигурации диаграммы Highcharts: Error Bar Y Axis Item Labels.")]
    public class HighchartErrorBarYAxisItemLabels : Highchart
    {

        public HighchartErrorBarYAxisItemLabels() : base()
        {
            Format = "{value} °C";
            Style = new HighchartErrorBarYAxisItemLabelsStyle();

        }

        [InputText("format")]
        [JsonProperty("format")]
        [Label("Format")]
        public string Format { get; set; }

        [JsonProperty("style")]
        [Label("Style")]
        public HighchartErrorBarYAxisItemLabelsStyle Style { get; set; }
    }
    [Label("Конфигурация Highcharts: Error Bar Y Axis Item Title Style")]
    [Description("Часть конфигурации диаграммы Highcharts: Error Bar Y Axis Item Title Style.")]
    public class HighchartErrorBarYAxisItemTitleStyle : Highchart
    {

        public HighchartErrorBarYAxisItemTitleStyle() : base()
        {
            Color = "#434348";

        }

        [InputText("color")]
        [JsonProperty("color")]
        [Label("Color")]
        public string Color { get; set; }
    }
    [Label("Конфигурация Highcharts: Error Bar Y Axis Item Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Error Bar Y Axis Item Title.")]
    public class HighchartErrorBarYAxisItemTitle : Highchart
    {

        public HighchartErrorBarYAxisItemTitle() : base()
        {
            Text = "Temperature";
            Style = new HighchartErrorBarYAxisItemTitleStyle();

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }

        [JsonProperty("style")]
        [Label("Style")]
        public HighchartErrorBarYAxisItemTitleStyle Style { get; set; }
    }
    [Label("Конфигурация Highcharts: Error Bar Y Axis Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Error Bar Y Axis Item.")]
    public class HighchartErrorBarYAxisItem : Highchart
    {

        public HighchartErrorBarYAxisItem() : base()
        {
            Labels = new HighchartErrorBarYAxisItemLabels();
            Title = new HighchartErrorBarYAxisItemTitle();

        }


        [JsonProperty("labels")]
        [Label("Labels")]
        public HighchartErrorBarYAxisItemLabels Labels { get; set; }

        [JsonProperty("title")]
        [Label("Title")]
        public HighchartErrorBarYAxisItemTitle Title { get; set; }
    }
    [Label("Конфигурация Highcharts: Error Bar Tooltip Shared")]
    [Description("Часть конфигурации диаграммы Highcharts: Error Bar Tooltip Shared.")]
    public class HighchartErrorBarTooltipShared : Highchart
    {

        public HighchartErrorBarTooltipShared() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: Error Bar Tooltip")]
    [Description("Часть конфигурации диаграммы Highcharts: Error Bar Tooltip.")]
    public class HighchartErrorBarTooltip : Highchart
    {

        public HighchartErrorBarTooltip() : base()
        {
            Shared = new HighchartErrorBarTooltipShared();

        }


        [JsonProperty("shared")]
        [Label("Shared")]
        public HighchartErrorBarTooltipShared Shared { get; set; }
    }
    [Label("Конфигурация Highcharts: Error Bar Series Item Tooltip")]
    [Description("Часть конфигурации диаграммы Highcharts: Error Bar Series Item Tooltip.")]
    public class HighchartErrorBarSeriesItemTooltip : Highchart
    {

        public HighchartErrorBarSeriesItemTooltip() : base()
        {
            PointFormat = "<span style='font-weight: bold; color: {series.color}'>{series.name}</span>: <b>{point.y:.1f} mm</b> ";

        }

        [InputText("pointFormat")]
        [JsonProperty("pointFormat")]
        [Label("Point Format")]
        public string PointFormat { get; set; }
    }
    [Label("Конфигурация Highcharts: Error Bar Series Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Error Bar Series Item.")]
    public class HighchartErrorBarSeriesItem : Highchart
    {

        public HighchartErrorBarSeriesItem() : base()
        {
            Name = "Rainfall";
            Type = "column";
            YAxis = 1f;
            Data = new List<float?> { 49.9f, 71.5f, 106.4f, 129.2f, 144f, 176f, 135.6f, 148.5f, 216.4f, 194.1f, 95.6f, 54.4f };
            Tooltip = new HighchartErrorBarSeriesItemTooltip();

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
        public HighchartErrorBarSeriesItemTooltip Tooltip { get; set; }
    }
    [Label("Диаграмма Highcharts: Error Bar")]
    [Description("Модель диаграммы Highcharts: Error Bar.")]
    public class HighchartErrorBar : Highchart
    {

        public HighchartErrorBar() : base()
        {
            Chart = new HighchartErrorBarChart();
            Title = new HighchartErrorBarTitle();
            XAxis = new List<HighchartErrorBarXAxisItem> { new HighchartErrorBarXAxisItem() };
            YAxis = new List<HighchartErrorBarYAxisItem> { new HighchartErrorBarYAxisItem(), new HighchartErrorBarYAxisItem() };
            Tooltip = new HighchartErrorBarTooltip();
            Series = new List<HighchartErrorBarSeriesItem> { new HighchartErrorBarSeriesItem(), new HighchartErrorBarSeriesItem(), new HighchartErrorBarSeriesItem(), new HighchartErrorBarSeriesItem() };

        }


        [JsonProperty("chart")]
        public HighchartErrorBarChart Chart { get; set; }

        [JsonProperty("title")]
        public HighchartErrorBarTitle Title { get; set; }

        [JsonProperty("xAxis")]
        public List<HighchartErrorBarXAxisItem> XAxis { get; set; }

        [JsonProperty("yAxis")]
        public List<HighchartErrorBarYAxisItem> YAxis { get; set; }

        [JsonProperty("tooltip")]
        public HighchartErrorBarTooltip Tooltip { get; set; }

        [JsonProperty("series")]
        public List<HighchartErrorBarSeriesItem> Series { get; set; }
    }


}