using System.Collections.Generic;

using Newtonsoft.Json;
using System;

namespace Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts
{
    [Label("Конфигурация Highcharts: Area Stacked Chart")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Stacked Chart.")]
    public class HighchartAreaStackedChart : Highchart
    {

        public HighchartAreaStackedChart() : base()
        {
            Type = "area";

        }

        [InputText("type")]
        [JsonProperty("type")]
        [Label("Type")]
        public string Type { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Stacked Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Stacked Title.")]
    public class HighchartAreaStackedTitle : Highchart
    {

        public HighchartAreaStackedTitle() : base()
        {
            Text = "Historic and Estimated Worldwide Population Growth by Region";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Stacked Subtitle")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Stacked Subtitle.")]
    public class HighchartAreaStackedSubtitle : Highchart
    {

        public HighchartAreaStackedSubtitle() : base()
        {
            Text = "Source: Wikipedia.org";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Stacked X Axis Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Stacked X Axis Title.")]
    public class HighchartAreaStackedXAxisTitle : Highchart
    {

        public HighchartAreaStackedXAxisTitle() : base()
        {
            Enabled = null;

        }


        [JsonProperty("enabled")]
        [Label("Enabled")]
        public object Enabled { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Stacked X Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Stacked X Axis.")]
    public class HighchartAreaStackedXAxis : Highchart
    {

        public HighchartAreaStackedXAxis() : base()
        {
            Categories = new List<string> { "1750", "1800", "1850", "1900", "1950", "1999", "2050" };
            TickmarkPlacement = "on";
            Title = new HighchartAreaStackedXAxisTitle();

        }


        [JsonProperty("categories")]
        [Label("Categories")]
        public List<string> Categories { get; set; }
        [InputText("tickmarkPlacement")]
        [JsonProperty("tickmarkPlacement")]
        [Label("Tickmark Placement")]
        public string TickmarkPlacement { get; set; }

        [JsonProperty("title")]
        [Label("Title")]
        public HighchartAreaStackedXAxisTitle Title { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Stacked Y Axis Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Stacked Y Axis Title.")]
    public class HighchartAreaStackedYAxisTitle : Highchart
    {

        public HighchartAreaStackedYAxisTitle() : base()
        {
            Text = "Billions";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Stacked Y Axis Labels")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Stacked Y Axis Labels.")]
    public class HighchartAreaStackedYAxisLabels : Highchart
    {

        public HighchartAreaStackedYAxisLabels() : base()
        {
            Formatter = () => { };

        }


        [JsonProperty("formatter")]
        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [Label("Formatter")]
        public Action Formatter { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Stacked Y Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Stacked Y Axis.")]
    public class HighchartAreaStackedYAxis : Highchart
    {

        public HighchartAreaStackedYAxis() : base()
        {
            Title = new HighchartAreaStackedYAxisTitle();
            Labels = new HighchartAreaStackedYAxisLabels();

        }


        [JsonProperty("title")]
        [Label("Title")]
        public HighchartAreaStackedYAxisTitle Title { get; set; }

        [JsonProperty("labels")]
        [Label("Labels")]
        public HighchartAreaStackedYAxisLabels Labels { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Stacked Tooltip Split")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Stacked Tooltip Split.")]
    public class HighchartAreaStackedTooltipSplit : Highchart
    {

        public HighchartAreaStackedTooltipSplit() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: Area Stacked Tooltip")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Stacked Tooltip.")]
    public class HighchartAreaStackedTooltip : Highchart
    {

        public HighchartAreaStackedTooltip() : base()
        {
            Split = new HighchartAreaStackedTooltipSplit();
            ValueSuffix = " millions";

        }


        [JsonProperty("split")]
        [Label("Split")]
        public HighchartAreaStackedTooltipSplit Split { get; set; }
        [InputText("valueSuffix")]
        [JsonProperty("valueSuffix")]
        [Label("Value Suffix")]
        public string ValueSuffix { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Stacked Plot Options Area Marker")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Stacked Plot Options Area Marker.")]
    public class HighchartAreaStackedPlotOptionsAreaMarker : Highchart
    {

        public HighchartAreaStackedPlotOptionsAreaMarker() : base()
        {
            LineWidth = 1f;
            LineColor = "#666666";

        }

        [InputNumber("lineWidth")]
        [JsonProperty("lineWidth")]
        [Label("Line Width")]
        public float LineWidth { get; set; }
        [InputText("lineColor")]
        [JsonProperty("lineColor")]
        [Label("Line Color")]
        public string LineColor { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Stacked Plot Options Area")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Stacked Plot Options Area.")]
    public class HighchartAreaStackedPlotOptionsArea : Highchart
    {

        public HighchartAreaStackedPlotOptionsArea() : base()
        {
            Stacking = "normal";
            LineColor = "#666666";
            LineWidth = 1f;
            Marker = new HighchartAreaStackedPlotOptionsAreaMarker();

        }

        [InputText("stacking")]
        [JsonProperty("stacking")]
        [Label("Stacking")]
        public string Stacking { get; set; }
        [InputText("lineColor")]
        [JsonProperty("lineColor")]
        [Label("Line Color")]
        public string LineColor { get; set; }
        [InputNumber("lineWidth")]
        [JsonProperty("lineWidth")]
        [Label("Line Width")]
        public float LineWidth { get; set; }

        [JsonProperty("marker")]
        [Label("Marker")]
        public HighchartAreaStackedPlotOptionsAreaMarker Marker { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Stacked Plot Options")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Stacked Plot Options.")]
    public class HighchartAreaStackedPlotOptions : Highchart
    {

        public HighchartAreaStackedPlotOptions() : base()
        {
            Area = new HighchartAreaStackedPlotOptionsArea();

        }


        [JsonProperty("area")]
        [Label("Area")]
        public HighchartAreaStackedPlotOptionsArea Area { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Stacked Series Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Stacked Series Item.")]
    public class HighchartAreaStackedSeriesItem : Highchart
    {

        public HighchartAreaStackedSeriesItem() : base()
        {
            Name = "Asia";
            Data = new List<float?> { 502f, 635f, 809f, 947f, 1402f, 3634f, 5268f };

        }

        [InputText("name")]
        [JsonProperty("name")]
        [Label("Name")]
        public string Name { get; set; }

        [JsonProperty("data")]
        [Label("Data")]
        public List<float?> Data { get; set; }
    }
    [Label("Диаграмма Highcharts: Area Stacked")]
    [Description("Модель диаграммы Highcharts: Area Stacked.")]
    public class HighchartAreaStacked : Highchart
    {

        public HighchartAreaStacked() : base()
        {
            Chart = new HighchartAreaStackedChart();
            Title = new HighchartAreaStackedTitle();
            Subtitle = new HighchartAreaStackedSubtitle();
            XAxis = new HighchartAreaStackedXAxis();
            YAxis = new HighchartAreaStackedYAxis();
            Tooltip = new HighchartAreaStackedTooltip();
            PlotOptions = new HighchartAreaStackedPlotOptions();
            Series = new List<HighchartAreaStackedSeriesItem> { new HighchartAreaStackedSeriesItem(), new HighchartAreaStackedSeriesItem(), new HighchartAreaStackedSeriesItem(), new HighchartAreaStackedSeriesItem(), new HighchartAreaStackedSeriesItem() };

        }


        [JsonProperty("chart")]
        public HighchartAreaStackedChart Chart { get; set; }

        [JsonProperty("title")]
        public HighchartAreaStackedTitle Title { get; set; }

        [JsonProperty("subtitle")]
        public HighchartAreaStackedSubtitle Subtitle { get; set; }

        [JsonProperty("xAxis")]
        public HighchartAreaStackedXAxis XAxis { get; set; }

        [JsonProperty("yAxis")]
        public HighchartAreaStackedYAxis YAxis { get; set; }

        [JsonProperty("tooltip")]
        public HighchartAreaStackedTooltip Tooltip { get; set; }

        [JsonProperty("plotOptions")]
        public HighchartAreaStackedPlotOptions PlotOptions { get; set; }

        [JsonProperty("series")]
        public List<HighchartAreaStackedSeriesItem> Series { get; set; }
    }


}