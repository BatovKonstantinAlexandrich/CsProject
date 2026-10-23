using System.Collections.Generic;

using Newtonsoft.Json;
using System;

namespace Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts
{
    [Label("Конфигурация Highcharts: Line Labels Chart")]
    [Description("Часть конфигурации диаграммы Highcharts: Line Labels Chart.")]
    public class HighchartLineLabelsChart : Highchart
    {

        public HighchartLineLabelsChart() : base()
        {
            Type = "line";

        }

        [InputText("type")]
        [JsonProperty("type")]
        [Label("Type")]
        public string Type { get; set; }
    }
    [Label("Конфигурация Highcharts: Line Labels Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Line Labels Title.")]
    public class HighchartLineLabelsTitle : Highchart
    {

        public HighchartLineLabelsTitle() : base()
        {
            Text = "Monthly Average Temperature";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Line Labels Subtitle")]
    [Description("Часть конфигурации диаграммы Highcharts: Line Labels Subtitle.")]
    public class HighchartLineLabelsSubtitle : Highchart
    {

        public HighchartLineLabelsSubtitle() : base()
        {
            Text = "Source: WorldClimate.com";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Line Labels X Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: Line Labels X Axis.")]
    public class HighchartLineLabelsXAxis : Highchart
    {

        public HighchartLineLabelsXAxis() : base()
        {
            Categories = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        }


        [JsonProperty("categories")]
        [Label("Categories")]
        public List<string> Categories { get; set; }
    }
    [Label("Конфигурация Highcharts: Line Labels Y Axis Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Line Labels Y Axis Title.")]
    public class HighchartLineLabelsYAxisTitle : Highchart
    {

        public HighchartLineLabelsYAxisTitle() : base()
        {
            Text = "Temperature (°C)";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Line Labels Y Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: Line Labels Y Axis.")]
    public class HighchartLineLabelsYAxis : Highchart
    {

        public HighchartLineLabelsYAxis() : base()
        {
            Title = new HighchartLineLabelsYAxisTitle();

        }


        [JsonProperty("title")]
        [Label("Title")]
        public HighchartLineLabelsYAxisTitle Title { get; set; }
    }
    [Label("Конфигурация Highcharts: Line Labels Plot Options Line Data Labels Enabled")]
    [Description("Часть конфигурации диаграммы Highcharts: Line Labels Plot Options Line Data Labels Enabled.")]
    public class HighchartLineLabelsPlotOptionsLineDataLabelsEnabled : Highchart
    {

        public HighchartLineLabelsPlotOptionsLineDataLabelsEnabled() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: Line Labels Plot Options Line Data Labels")]
    [Description("Часть конфигурации диаграммы Highcharts: Line Labels Plot Options Line Data Labels.")]
    public class HighchartLineLabelsPlotOptionsLineDataLabels : Highchart
    {

        public HighchartLineLabelsPlotOptionsLineDataLabels() : base()
        {
            Enabled = new HighchartLineLabelsPlotOptionsLineDataLabelsEnabled();

        }


        [JsonProperty("enabled")]
        [Label("Enabled")]
        public HighchartLineLabelsPlotOptionsLineDataLabelsEnabled Enabled { get; set; }
    }
    [Label("Конфигурация Highcharts: Line Labels Plot Options Line")]
    [Description("Часть конфигурации диаграммы Highcharts: Line Labels Plot Options Line.")]
    public class HighchartLineLabelsPlotOptionsLine : Highchart
    {

        public HighchartLineLabelsPlotOptionsLine() : base()
        {
            DataLabels = new HighchartLineLabelsPlotOptionsLineDataLabels();
            EnableMouseTracking = null;

        }


        [JsonProperty("dataLabels")]
        [Label("Data Labels")]
        public HighchartLineLabelsPlotOptionsLineDataLabels DataLabels { get; set; }

        [JsonProperty("enableMouseTracking")]
        [Label("Enable Mouse Tracking")]
        public object EnableMouseTracking { get; set; }
    }
    [Label("Конфигурация Highcharts: Line Labels Plot Options")]
    [Description("Часть конфигурации диаграммы Highcharts: Line Labels Plot Options.")]
    public class HighchartLineLabelsPlotOptions : Highchart
    {

        public HighchartLineLabelsPlotOptions() : base()
        {
            Line = new HighchartLineLabelsPlotOptionsLine();

        }


        [JsonProperty("line")]
        [Label("Line")]
        public HighchartLineLabelsPlotOptionsLine Line { get; set; }
    }
    [Label("Конфигурация Highcharts: Line Labels Series Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Line Labels Series Item.")]
    public class HighchartLineLabelsSeriesItem : Highchart
    {

        public HighchartLineLabelsSeriesItem() : base()
        {
            Name = "Tokyo";
            Data = new List<float?> { 7f, 6.9f, 9.5f, 14.5f, 18.4f, 21.5f, 25.2f, 26.5f, 23.3f, 18.3f, 13.9f, 9.6f };

        }

        [InputText("name")]
        [JsonProperty("name")]
        [Label("Name")]
        public string Name { get; set; }

        [JsonProperty("data")]
        [Label("Data")]
        public List<float?> Data { get; set; }
    }
    [Label("Диаграмма Highcharts: Line Labels")]
    [Description("Модель диаграммы Highcharts: Line Labels.")]
    public class HighchartLineLabels : Highchart
    {

        public HighchartLineLabels() : base()
        {
            Chart = new HighchartLineLabelsChart();
            Title = new HighchartLineLabelsTitle();
            Subtitle = new HighchartLineLabelsSubtitle();
            XAxis = new HighchartLineLabelsXAxis();
            YAxis = new HighchartLineLabelsYAxis();
            PlotOptions = new HighchartLineLabelsPlotOptions();
            Series = new List<HighchartLineLabelsSeriesItem> { new HighchartLineLabelsSeriesItem(), new HighchartLineLabelsSeriesItem() };

        }

        [JsonProperty("chart")]
        [Label("Chart")]
        public HighchartLineLabelsChart Chart { get; set; }


        [JsonProperty("title")]
        public HighchartLineLabelsTitle Title { get; set; }


        [JsonProperty("subtitle")]
        public HighchartLineLabelsSubtitle Subtitle { get; set; }


        [JsonProperty("xAxis")]
        public HighchartLineLabelsXAxis XAxis { get; set; }


        [JsonProperty("yAxis")]
        public HighchartLineLabelsYAxis YAxis { get; set; }

        [JsonProperty("plotOptions")]
        public HighchartLineLabelsPlotOptions PlotOptions { get; set; }

        [JsonProperty("series")]
        public List<HighchartLineLabelsSeriesItem> Series { get; set; }
    }


}