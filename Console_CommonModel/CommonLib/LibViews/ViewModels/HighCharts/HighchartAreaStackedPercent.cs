using System.Collections.Generic;

using Newtonsoft.Json;
using System;

namespace Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts
{
    [Label("Конфигурация Highcharts: Area Stacked Percent Chart")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Stacked Percent Chart.")]
    public class HighchartAreaStackedPercentChart : Highchart
    {

        public HighchartAreaStackedPercentChart() : base()
        {
            Type = "area";

        }

        [InputText("type")]
        [JsonProperty("type")]
        [Label("Type")]
        public string Type { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Stacked Percent Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Stacked Percent Title.")]
    public class HighchartAreaStackedPercentTitle : Highchart
    {

        public HighchartAreaStackedPercentTitle() : base()
        {
            Text = "Historic and Estimated Worldwide Population Distribution by Region";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Stacked Percent Subtitle")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Stacked Percent Subtitle.")]
    public class HighchartAreaStackedPercentSubtitle : Highchart
    {

        public HighchartAreaStackedPercentSubtitle() : base()
        {
            Text = "Source: Wikipedia.org";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Stacked Percent X Axis Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Stacked Percent X Axis Title.")]
    public class HighchartAreaStackedPercentXAxisTitle : Highchart
    {

        public HighchartAreaStackedPercentXAxisTitle() : base()
        {
            Enabled = null;

        }


        [JsonProperty("enabled")]
        [Label("Enabled")]
        public object Enabled { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Stacked Percent X Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Stacked Percent X Axis.")]
    public class HighchartAreaStackedPercentXAxis : Highchart
    {

        public HighchartAreaStackedPercentXAxis() : base()
        {
            Categories = new List<string> { "1750", "1800", "1850", "1900", "1950", "1999", "2050" };
            TickmarkPlacement = "on";
            Title = new HighchartAreaStackedPercentXAxisTitle();

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
        public HighchartAreaStackedPercentXAxisTitle Title { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Stacked Percent Y Axis Labels")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Stacked Percent Y Axis Labels.")]
    public class HighchartAreaStackedPercentYAxisLabels : Highchart
    {

        public HighchartAreaStackedPercentYAxisLabels() : base()
        {
            Format = "{value}%";

        }

        [InputText("format")]
        [JsonProperty("format")]
        [Label("Format")]
        public string Format { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Stacked Percent Y Axis Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Stacked Percent Y Axis Title.")]
    public class HighchartAreaStackedPercentYAxisTitle : Highchart
    {

        public HighchartAreaStackedPercentYAxisTitle() : base()
        {
            Enabled = null;

        }


        [JsonProperty("enabled")]
        [Label("Enabled")]
        public object Enabled { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Stacked Percent Y Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Stacked Percent Y Axis.")]
    public class HighchartAreaStackedPercentYAxis : Highchart
    {

        public HighchartAreaStackedPercentYAxis() : base()
        {
            Labels = new HighchartAreaStackedPercentYAxisLabels();
            Title = new HighchartAreaStackedPercentYAxisTitle();

        }


        [JsonProperty("labels")]
        [Label("Labels")]
        public HighchartAreaStackedPercentYAxisLabels Labels { get; set; }

        [JsonProperty("title")]
        [Label("Title")]
        public HighchartAreaStackedPercentYAxisTitle Title { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Stacked Percent Tooltip Split")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Stacked Percent Tooltip Split.")]
    public class HighchartAreaStackedPercentTooltipSplit : Highchart
    {

        public HighchartAreaStackedPercentTooltipSplit() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: Area Stacked Percent Tooltip")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Stacked Percent Tooltip.")]
    public class HighchartAreaStackedPercentTooltip : Highchart
    {

        public HighchartAreaStackedPercentTooltip() : base()
        {
            PointFormat = "<span style='color:{series.color}'>{series.name}</span>: <b>{point.percentage:.1f}%</b> ({point.y:,.0f} millions)<br/>";
            Split = new HighchartAreaStackedPercentTooltipSplit();

        }

        [InputText("pointFormat")]
        [JsonProperty("pointFormat")]
        [Label("Point Format")]
        public string PointFormat { get; set; }

        [JsonProperty("split")]
        [Label("Split")]
        public HighchartAreaStackedPercentTooltipSplit Split { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Stacked Percent Plot Options Area Marker")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Stacked Percent Plot Options Area Marker.")]
    public class HighchartAreaStackedPercentPlotOptionsAreaMarker : Highchart
    {

        public HighchartAreaStackedPercentPlotOptionsAreaMarker() : base()
        {
            LineWidth = 1f;
            LineColor = "#ffffff";

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
    [Label("Конфигурация Highcharts: Area Stacked Percent Plot Options Area Accessibility")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Stacked Percent Plot Options Area Accessibility.")]
    public class HighchartAreaStackedPercentPlotOptionsAreaAccessibility : Highchart
    {

        public HighchartAreaStackedPercentPlotOptionsAreaAccessibility() : base()
        {
            PointDescriptionFormatter = () => { };

        }


        [JsonProperty("pointDescriptionFormatter")]
        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [Label("Point Description Formatter")]
        public Action PointDescriptionFormatter { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Stacked Percent Plot Options Area")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Stacked Percent Plot Options Area.")]
    public class HighchartAreaStackedPercentPlotOptionsArea : Highchart
    {

        public HighchartAreaStackedPercentPlotOptionsArea() : base()
        {
            Stacking = "percent";
            LineColor = "#ffffff";
            LineWidth = 1f;
            Marker = new HighchartAreaStackedPercentPlotOptionsAreaMarker();
            Accessibility = new HighchartAreaStackedPercentPlotOptionsAreaAccessibility();

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
        public HighchartAreaStackedPercentPlotOptionsAreaMarker Marker { get; set; }

        [JsonProperty("accessibility")]
        [Label("Accessibility")]
        public HighchartAreaStackedPercentPlotOptionsAreaAccessibility Accessibility { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Stacked Percent Plot Options")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Stacked Percent Plot Options.")]
    public class HighchartAreaStackedPercentPlotOptions : Highchart
    {

        public HighchartAreaStackedPercentPlotOptions() : base()
        {
            Area = new HighchartAreaStackedPercentPlotOptionsArea();

        }


        [JsonProperty("area")]
        [Label("Area")]
        public HighchartAreaStackedPercentPlotOptionsArea Area { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Stacked Percent Series Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Stacked Percent Series Item.")]
    public class HighchartAreaStackedPercentSeriesItem : Highchart
    {

        public HighchartAreaStackedPercentSeriesItem() : base()
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
    [Label("Диаграмма Highcharts: Area Stacked Percent")]
    [Description("Модель диаграммы Highcharts: Area Stacked Percent.")]
    public class HighchartAreaStackedPercent : Highchart
    {

        public HighchartAreaStackedPercent() : base()
        {
            Chart = new HighchartAreaStackedPercentChart();
            Title = new HighchartAreaStackedPercentTitle();
            Subtitle = new HighchartAreaStackedPercentSubtitle();
            XAxis = new HighchartAreaStackedPercentXAxis();
            YAxis = new HighchartAreaStackedPercentYAxis();
            Tooltip = new HighchartAreaStackedPercentTooltip();
            PlotOptions = new HighchartAreaStackedPercentPlotOptions();
            Series = new List<HighchartAreaStackedPercentSeriesItem> { new HighchartAreaStackedPercentSeriesItem(), new HighchartAreaStackedPercentSeriesItem(), new HighchartAreaStackedPercentSeriesItem(), new HighchartAreaStackedPercentSeriesItem(), new HighchartAreaStackedPercentSeriesItem() };

        }


        [JsonProperty("chart")]
        public HighchartAreaStackedPercentChart Chart { get; set; }

        [JsonProperty("title")]
        public HighchartAreaStackedPercentTitle Title { get; set; }

        [JsonProperty("subtitle")]
        public HighchartAreaStackedPercentSubtitle Subtitle { get; set; }

        [JsonProperty("xAxis")]
        public HighchartAreaStackedPercentXAxis XAxis { get; set; }

        [JsonProperty("yAxis")]
        public HighchartAreaStackedPercentYAxis YAxis { get; set; }

        [JsonProperty("tooltip")]
        public HighchartAreaStackedPercentTooltip Tooltip { get; set; }

        [JsonProperty("plotOptions")]
        public HighchartAreaStackedPercentPlotOptions PlotOptions { get; set; }

        [JsonProperty("series")]
        public List<HighchartAreaStackedPercentSeriesItem> Series { get; set; }
    }


}