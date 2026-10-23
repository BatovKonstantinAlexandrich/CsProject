using System.Collections.Generic;

using Newtonsoft.Json;
using System;

namespace Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts
{
    [Label("Конфигурация Highcharts: Column Stacked And Grouped Chart")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Stacked And Grouped Chart.")]
    public class HighchartColumnStackedAndGroupedChart : Highchart
    {

        public HighchartColumnStackedAndGroupedChart() : base()
        {
            Type = "column";

        }

        [InputText("type")]
        [JsonProperty("type")]
        [Label("Type")]
        public string Type { get; set; }
    }
    [Label("Конфигурация Highcharts: Column Stacked And Grouped Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Stacked And Grouped Title.")]
    public class HighchartColumnStackedAndGroupedTitle : Highchart
    {

        public HighchartColumnStackedAndGroupedTitle() : base()
        {
            Text = "Total fruit consumption, grouped by gender";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Column Stacked And Grouped X Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Stacked And Grouped X Axis.")]
    public class HighchartColumnStackedAndGroupedXAxis : Highchart
    {

        public HighchartColumnStackedAndGroupedXAxis() : base()
        {
            Categories = new List<string> { "Apples", "Oranges", "Pears", "Grapes", "Bananas" };

        }


        [JsonProperty("categories")]
        [Label("Categories")]
        public List<string> Categories { get; set; }
    }
    [Label("Конфигурация Highcharts: Column Stacked And Grouped Y Axis Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Stacked And Grouped Y Axis Title.")]
    public class HighchartColumnStackedAndGroupedYAxisTitle : Highchart
    {

        public HighchartColumnStackedAndGroupedYAxisTitle() : base()
        {
            Text = "Number of fruits";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Column Stacked And Grouped Y Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Stacked And Grouped Y Axis.")]
    public class HighchartColumnStackedAndGroupedYAxis : Highchart
    {

        public HighchartColumnStackedAndGroupedYAxis() : base()
        {
            AllowDecimals = null;
            Min = null;
            Title = new HighchartColumnStackedAndGroupedYAxisTitle();

        }


        [JsonProperty("allowDecimals")]
        [Label("Allow Decimals")]
        public object AllowDecimals { get; set; }

        [JsonProperty("min")]
        [Label("Min")]
        public object Min { get; set; }

        [JsonProperty("title")]
        [Label("Title")]
        public HighchartColumnStackedAndGroupedYAxisTitle Title { get; set; }
    }
    [Label("Конфигурация Highcharts: Column Stacked And Grouped Tooltip")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Stacked And Grouped Tooltip.")]
    public class HighchartColumnStackedAndGroupedTooltip : Highchart
    {

        public HighchartColumnStackedAndGroupedTooltip() : base()
        {
            Formatter = () => { };

        }


        [JsonProperty("formatter")]
        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [Label("Formatter")]
        public Action Formatter { get; set; }
    }
    [Label("Конфигурация Highcharts: Column Stacked And Grouped Plot Options Column")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Stacked And Grouped Plot Options Column.")]
    public class HighchartColumnStackedAndGroupedPlotOptionsColumn : Highchart
    {

        public HighchartColumnStackedAndGroupedPlotOptionsColumn() : base()
        {
            Stacking = "normal";

        }

        [InputText("stacking")]
        [JsonProperty("stacking")]
        [Label("Stacking")]
        public string Stacking { get; set; }
    }
    [Label("Конфигурация Highcharts: Column Stacked And Grouped Plot Options")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Stacked And Grouped Plot Options.")]
    public class HighchartColumnStackedAndGroupedPlotOptions : Highchart
    {

        public HighchartColumnStackedAndGroupedPlotOptions() : base()
        {
            Column = new HighchartColumnStackedAndGroupedPlotOptionsColumn();

        }


        [JsonProperty("column")]
        [Label("Column")]
        public HighchartColumnStackedAndGroupedPlotOptionsColumn Column { get; set; }
    }
    [Label("Конфигурация Highcharts: Column Stacked And Grouped Series Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Stacked And Grouped Series Item.")]
    public class HighchartColumnStackedAndGroupedSeriesItem : Highchart
    {

        public HighchartColumnStackedAndGroupedSeriesItem() : base()
        {
            Name = "John";
            Data = new List<float?> { 5f, 3f, 4f, 7f, 2f };
            Stack = "male";

        }

        [InputText("name")]
        [JsonProperty("name")]
        [Label("Name")]
        public string Name { get; set; }

        [JsonProperty("data")]
        [Label("Data")]
        public List<float?> Data { get; set; }
        [InputText("stack")]
        [JsonProperty("stack")]
        [Label("Stack")]
        public string Stack { get; set; }
    }
    [Label("Диаграмма Highcharts: Column Stacked And Grouped")]
    [Description("Модель диаграммы Highcharts: Column Stacked And Grouped.")]
    public class HighchartColumnStackedAndGrouped : Highchart
    {

        public HighchartColumnStackedAndGrouped() : base()
        {
            Chart = new HighchartColumnStackedAndGroupedChart();
            Title = new HighchartColumnStackedAndGroupedTitle();
            XAxis = new HighchartColumnStackedAndGroupedXAxis();
            YAxis = new HighchartColumnStackedAndGroupedYAxis();
            Tooltip = new HighchartColumnStackedAndGroupedTooltip();
            PlotOptions = new HighchartColumnStackedAndGroupedPlotOptions();
            Series = new List<HighchartColumnStackedAndGroupedSeriesItem> { new HighchartColumnStackedAndGroupedSeriesItem(), new HighchartColumnStackedAndGroupedSeriesItem(), new HighchartColumnStackedAndGroupedSeriesItem(), new HighchartColumnStackedAndGroupedSeriesItem() };

        }


        [JsonProperty("chart")]
        [Label("Chart")]
        public HighchartColumnStackedAndGroupedChart Chart { get; set; }

        [JsonProperty("title")]
        public HighchartColumnStackedAndGroupedTitle Title { get; set; }

        [JsonProperty("xAxis")]
        public HighchartColumnStackedAndGroupedXAxis XAxis { get; set; }

        [JsonProperty("yAxis")]
        public HighchartColumnStackedAndGroupedYAxis YAxis { get; set; }

        [JsonProperty("tooltip")]
        public HighchartColumnStackedAndGroupedTooltip Tooltip { get; set; }

        [JsonProperty("plotOptions")]
        public HighchartColumnStackedAndGroupedPlotOptions PlotOptions { get; set; }

        [JsonProperty("series")]
        public List<HighchartColumnStackedAndGroupedSeriesItem> Series { get; set; }
    }


}