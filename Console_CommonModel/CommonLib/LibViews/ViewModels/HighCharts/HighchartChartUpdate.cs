using System.Collections.Generic;

using Newtonsoft.Json;
using System;

namespace Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts
{
    [Label("Конфигурация Highcharts: Chart Update Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Chart Update Title.")]
    public class HighchartChartUpdateTitle : Highchart
    {

        public HighchartChartUpdateTitle() : base()
        {
            Text = "Chart.update";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Chart Update Subtitle")]
    [Description("Часть конфигурации диаграммы Highcharts: Chart Update Subtitle.")]
    public class HighchartChartUpdateSubtitle : Highchart
    {

        public HighchartChartUpdateSubtitle() : base()
        {
            Text = "Plain";

        }

        [Label("Заголовок")]
        [InputText("text")]
        [JsonProperty("text")]
        public string Text { get; set; }
    }

    [Label("Конфигурация Highcharts: Chart Update X Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: Chart Update X Axis.")]
    public class HighchartChartUpdateXAxis : Highchart
    {

        public HighchartChartUpdateXAxis() : base()
        {
            Categories = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        }


        [JsonProperty("categories")]
        [Label("Categories")]
        public List<string> Categories { get; set; }
    }
    [Label("Конфигурация Highcharts: Chart Update Series Item Color By Point")]
    [Description("Часть конфигурации диаграммы Highcharts: Chart Update Series Item Color By Point.")]
    public class HighchartChartUpdateSeriesItemColorByPoint : Highchart
    {

        public HighchartChartUpdateSeriesItemColorByPoint() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: Chart Update Series Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Chart Update Series Item.")]
    public class HighchartChartUpdateSeriesItem : Highchart
    {

        public HighchartChartUpdateSeriesItem() : base()
        {
            Type = "column";
            ColorByPoint = new HighchartChartUpdateSeriesItemColorByPoint();
            Data = new List<float?> { 29.9f, 71.5f, 106.4f, 129.2f, 144f, 176f, 135.6f, 148.5f, 216.4f, 194.1f, 95.6f, 54.4f };
            ShowInLegend = null;

        }

        [InputText("type")]
        [JsonProperty("type")]
        [Label("Type")]
        public string Type { get; set; }

        [JsonProperty("colorByPoint")]
        [Label("Color By Point")]
        public HighchartChartUpdateSeriesItemColorByPoint ColorByPoint { get; set; }

        [JsonProperty("data")]
        [Label("Data")]
        public List<float?> Data { get; set; }

        [JsonProperty("showInLegend")]
        [Label("Show In Legend")]
        public object ShowInLegend { get; set; }
    }
    [Label("Диаграмма Highcharts: Chart Update")]
    [Description("Модель диаграммы Highcharts: Chart Update.")]
    public class HighchartChartUpdate : Highchart
    {

        public HighchartChartUpdate() : base()
        {
            Title = new HighchartChartUpdateTitle();
            Subtitle = new HighchartChartUpdateSubtitle();
            XAxis = new HighchartChartUpdateXAxis();
            Series = new List<HighchartChartUpdateSeriesItem> { new HighchartChartUpdateSeriesItem() };

        }


        [Label("Заголовок")]
        [JsonProperty("title")]
        public HighchartChartUpdateTitle Title { get; set; }


        [Label("Подзаголовок")]
        [JsonProperty("subtitle")]
        public HighchartChartUpdateSubtitle Subtitle { get; set; }


        [Label("Ось X")]
        [JsonProperty("xAxis")]
        public HighchartChartUpdateXAxis XAxis { get; set; }


        [Label("Данные")]
        [JsonProperty("series")]
        public List<HighchartChartUpdateSeriesItem> Series { get; set; }
    }


}