using System.Collections.Generic;

using Newtonsoft.Json;
using System;

namespace Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts
{
    [Label("Конфигурация Highcharts: Bar Stacked Chart")]
    [Description("Часть конфигурации диаграммы Highcharts: Bar Stacked Chart.")]
    public class HighchartBarStackedChart : Highchart
    {

        public HighchartBarStackedChart() : base()
        {
            Type = "bar";

        }

        [InputText("type")]
        [JsonProperty("type")]
        [Label("Type")]
        public string Type { get; set; }
    }
    [Label("Конфигурация Highcharts: Bar Stacked Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Bar Stacked Title.")]
    public class HighchartBarStackedTitle : Highchart
    {

        public HighchartBarStackedTitle() : base()
        {
            Text = "Stacked bar chart";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Bar Stacked X Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: Bar Stacked X Axis.")]
    public class HighchartBarStackedXAxis : Highchart
    {

        public HighchartBarStackedXAxis() : base()
        {
            Categories = new List<string> { "Apples", "Oranges", "Pears", "Grapes", "Bananas" };

        }


        [JsonProperty("categories")]
        [Label("Categories")]
        public List<string> Categories { get; set; }
    }
    [Label("Конфигурация Highcharts: Bar Stacked Y Axis Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Bar Stacked Y Axis Title.")]
    public class HighchartBarStackedYAxisTitle : Highchart
    {

        public HighchartBarStackedYAxisTitle() : base()
        {
            Text = "Total fruit consumption";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Bar Stacked Y Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: Bar Stacked Y Axis.")]
    public class HighchartBarStackedYAxis : Highchart
    {

        public HighchartBarStackedYAxis() : base()
        {
            Min = null;
            Title = new HighchartBarStackedYAxisTitle();

        }


        [JsonProperty("min")]
        [Label("Min")]
        public object Min { get; set; }

        [JsonProperty("title")]
        [Label("Title")]
        public HighchartBarStackedYAxisTitle Title { get; set; }
    }
    [Label("Конфигурация Highcharts: Bar Stacked Legend Reversed")]
    [Description("Часть конфигурации диаграммы Highcharts: Bar Stacked Legend Reversed.")]
    public class HighchartBarStackedLegendReversed : Highchart
    {

        public HighchartBarStackedLegendReversed() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: Bar Stacked Legend")]
    [Description("Часть конфигурации диаграммы Highcharts: Bar Stacked Legend.")]
    public class HighchartBarStackedLegend : Highchart
    {

        public HighchartBarStackedLegend() : base()
        {
            Reversed = new HighchartBarStackedLegendReversed();

        }


        [JsonProperty("reversed")]
        [Label("Reversed")]
        public HighchartBarStackedLegendReversed Reversed { get; set; }
    }
    [Label("Конфигурация Highcharts: Bar Stacked Plot Options Series")]
    [Description("Часть конфигурации диаграммы Highcharts: Bar Stacked Plot Options Series.")]
    public class HighchartBarStackedPlotOptionsSeries : Highchart
    {

        public HighchartBarStackedPlotOptionsSeries() : base()
        {
            Stacking = "normal";

        }

        [InputText("stacking")]
        [JsonProperty("stacking")]
        [Label("Stacking")]
        public string Stacking { get; set; }
    }
    [Label("Конфигурация Highcharts: Bar Stacked Plot Options")]
    [Description("Часть конфигурации диаграммы Highcharts: Bar Stacked Plot Options.")]
    public class HighchartBarStackedPlotOptions : Highchart
    {

        public HighchartBarStackedPlotOptions() : base()
        {
            Series = new HighchartBarStackedPlotOptionsSeries();

        }


        [JsonProperty("series")]
        [Label("Series")]
        public HighchartBarStackedPlotOptionsSeries Series { get; set; }
    }
    [Label("Конфигурация Highcharts: Bar Stacked Series Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Bar Stacked Series Item.")]
    public class HighchartBarStackedSeriesItem : Highchart
    {

        public HighchartBarStackedSeriesItem() : base()
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
    [Label("Диаграмма Highcharts: Bar Stacked")]
    [Description("Модель диаграммы Highcharts: Bar Stacked.")]
    public class HighchartBarStacked : Highchart
    {

        public HighchartBarStacked() : base()
        {
            Chart = new HighchartBarStackedChart();
            Title = new HighchartBarStackedTitle();
            XAxis = new HighchartBarStackedXAxis();
            YAxis = new HighchartBarStackedYAxis();
            Legend = new HighchartBarStackedLegend();
            PlotOptions = new HighchartBarStackedPlotOptions();
            Series = new List<HighchartBarStackedSeriesItem> { new HighchartBarStackedSeriesItem(), new HighchartBarStackedSeriesItem(), new HighchartBarStackedSeriesItem() };

        }


        [JsonProperty("chart")]
        [Label("Chart")]
        public HighchartBarStackedChart Chart { get; set; }

        [JsonProperty("title")]
        public HighchartBarStackedTitle Title { get; set; }

        [JsonProperty("xAxis")]
        public HighchartBarStackedXAxis XAxis { get; set; }

        [JsonProperty("yAxis")]
        public HighchartBarStackedYAxis YAxis { get; set; }

        [JsonProperty("legend")]
        public HighchartBarStackedLegend Legend { get; set; }

        [JsonProperty("plotOptions")]
        public HighchartBarStackedPlotOptions PlotOptions { get; set; }

        [JsonProperty("series")]
        public List<HighchartBarStackedSeriesItem> Series { get; set; }
    }


}