using System.Collections.Generic;

using Newtonsoft.Json;
using System;

namespace Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts
{
    [Label("Конфигурация Highcharts: Column Stacked Percent Chart")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Stacked Percent Chart.")]
    public class HighchartColumnStackedPercentChart : Highchart
    {

        public HighchartColumnStackedPercentChart() : base()
        {
            Type = "column";

        }

        [InputText("type")]
        [JsonProperty("type")]
        [Label("Type")]
        public string Type { get; set; }
    }
    [Label("Конфигурация Highcharts: Column Stacked Percent Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Stacked Percent Title.")]
    public class HighchartColumnStackedPercentTitle : Highchart
    {

        public HighchartColumnStackedPercentTitle() : base()
        {
            Text = "Stacked column chart";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Column Stacked Percent X Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Stacked Percent X Axis.")]
    public class HighchartColumnStackedPercentXAxis : Highchart
    {

        public HighchartColumnStackedPercentXAxis() : base()
        {
            Categories = new List<string> { "Apples", "Oranges", "Pears", "Grapes", "Bananas" };

        }


        [JsonProperty("categories")]
        [Label("Categories")]
        public List<string> Categories { get; set; }
    }
    [Label("Конфигурация Highcharts: Column Stacked Percent Y Axis Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Stacked Percent Y Axis Title.")]
    public class HighchartColumnStackedPercentYAxisTitle : Highchart
    {

        public HighchartColumnStackedPercentYAxisTitle() : base()
        {
            Text = "Total fruit consumption";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Column Stacked Percent Y Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Stacked Percent Y Axis.")]
    public class HighchartColumnStackedPercentYAxis : Highchart
    {

        public HighchartColumnStackedPercentYAxis() : base()
        {
            Min = null;
            Title = new HighchartColumnStackedPercentYAxisTitle();

        }


        [JsonProperty("min")]
        [Label("Min")]
        public object Min { get; set; }

        [JsonProperty("title")]
        [Label("Title")]
        public HighchartColumnStackedPercentYAxisTitle Title { get; set; }
    }
    [Label("Конфигурация Highcharts: Column Stacked Percent Tooltip Shared")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Stacked Percent Tooltip Shared.")]
    public class HighchartColumnStackedPercentTooltipShared : Highchart
    {

        public HighchartColumnStackedPercentTooltipShared() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: Column Stacked Percent Tooltip")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Stacked Percent Tooltip.")]
    public class HighchartColumnStackedPercentTooltip : Highchart
    {

        public HighchartColumnStackedPercentTooltip() : base()
        {
            PointFormat = "<span style='color:{series.color}'>{series.name}</span>: <b>{point.y}</b> ({point.percentage:.0f}%)<br/>";
            Shared = new HighchartColumnStackedPercentTooltipShared();

        }

        [InputText("pointFormat")]
        [JsonProperty("pointFormat")]
        [Label("Point Format")]
        public string PointFormat { get; set; }

        [JsonProperty("shared")]
        [Label("Shared")]
        public HighchartColumnStackedPercentTooltipShared Shared { get; set; }
    }
    [Label("Конфигурация Highcharts: Column Stacked Percent Plot Options Column")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Stacked Percent Plot Options Column.")]
    public class HighchartColumnStackedPercentPlotOptionsColumn : Highchart
    {

        public HighchartColumnStackedPercentPlotOptionsColumn() : base()
        {
            Stacking = "percent";

        }

        [InputText("stacking")]
        [JsonProperty("stacking")]
        [Label("Stacking")]
        public string Stacking { get; set; }
    }
    [Label("Конфигурация Highcharts: Column Stacked Percent Plot Options")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Stacked Percent Plot Options.")]
    public class HighchartColumnStackedPercentPlotOptions : Highchart
    {

        public HighchartColumnStackedPercentPlotOptions() : base()
        {
            Column = new HighchartColumnStackedPercentPlotOptionsColumn();

        }


        [JsonProperty("column")]
        [Label("Column")]
        public HighchartColumnStackedPercentPlotOptionsColumn Column { get; set; }
    }
    [Label("Конфигурация Highcharts: Column Stacked Percent Series Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Stacked Percent Series Item.")]
    public class HighchartColumnStackedPercentSeriesItem : Highchart
    {

        public HighchartColumnStackedPercentSeriesItem() : base()
        {
            Name = "John";
            Data = new List<float?> { 5f, 3f, 4f, 7f, 2f };

        }

        [InputText("name")]
        [JsonProperty("name")]
        [Label("Name")]
        public string Name { get; set; }

        [JsonProperty("data")]
        [Label("Data")]
        public List<float?> Data { get; set; }
    }
    [Label("Диаграмма Highcharts: Column Stacked Percent")]
    [Description("Модель диаграммы Highcharts: Column Stacked Percent.")]
    public class HighchartColumnStackedPercent : Highchart
    {

        public HighchartColumnStackedPercent() : base()
        {
            Chart = new HighchartColumnStackedPercentChart();
            Title = new HighchartColumnStackedPercentTitle();
            XAxis = new HighchartColumnStackedPercentXAxis();
            YAxis = new HighchartColumnStackedPercentYAxis();
            Tooltip = new HighchartColumnStackedPercentTooltip();
            PlotOptions = new HighchartColumnStackedPercentPlotOptions();
            Series = new List<HighchartColumnStackedPercentSeriesItem> { new HighchartColumnStackedPercentSeriesItem(), new HighchartColumnStackedPercentSeriesItem(), new HighchartColumnStackedPercentSeriesItem() };

        }


        [JsonProperty("chart")]
        [Label("Chart")]
        public HighchartColumnStackedPercentChart Chart { get; set; }

        [JsonProperty("title")]
        public HighchartColumnStackedPercentTitle Title { get; set; }

        [JsonProperty("xAxis")]
        public HighchartColumnStackedPercentXAxis XAxis { get; set; }

        [JsonProperty("yAxis")]
        public HighchartColumnStackedPercentYAxis YAxis { get; set; }

        [JsonProperty("tooltip")]
        public HighchartColumnStackedPercentTooltip Tooltip { get; set; }

        [JsonProperty("plotOptions")]
        public HighchartColumnStackedPercentPlotOptions PlotOptions { get; set; }

        [JsonProperty("series")]
        public List<HighchartColumnStackedPercentSeriesItem> Series { get; set; }
    }


}