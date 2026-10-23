using System.Collections.Generic;

using Newtonsoft.Json;
using System;

namespace Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts
{
    [Label("Конфигурация Highcharts: Column Negative Chart")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Negative Chart.")]
    public class HighchartColumnNegativeChart : Highchart
    {

        public HighchartColumnNegativeChart() : base()
        {
            Type = "column";

        }

        [InputText("type")]
        [JsonProperty("type")]
        [Label("Type")]
        public string Type { get; set; }
    }
    [Label("Конфигурация Highcharts: Column Negative Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Negative Title.")]
    public class HighchartColumnNegativeTitle : Highchart
    {

        public HighchartColumnNegativeTitle() : base()
        {
            Text = "Column chart with negative values";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Column Negative X Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Negative X Axis.")]
    public class HighchartColumnNegativeXAxis : Highchart
    {

        public HighchartColumnNegativeXAxis() : base()
        {
            Categories = new List<string> { "Apples", "Oranges", "Pears", "Grapes", "Bananas" };

        }


        [JsonProperty("categories")]
        [Label("Categories")]
        public List<string> Categories { get; set; }
    }
    [Label("Конфигурация Highcharts: Column Negative Credits")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Negative Credits.")]
    public class HighchartColumnNegativeCredits : Highchart
    {

        public HighchartColumnNegativeCredits() : base()
        {
            Enabled = null;

        }


        [JsonProperty("enabled")]
        [Label("Enabled")]
        public object Enabled { get; set; }
    }
    [Label("Конфигурация Highcharts: Column Negative Series Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Negative Series Item.")]
    public class HighchartColumnNegativeSeriesItem : Highchart
    {

        public HighchartColumnNegativeSeriesItem() : base()
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
    [Label("Диаграмма Highcharts: Column Negative")]
    [Description("Модель диаграммы Highcharts: Column Negative.")]
    public class HighchartColumnNegative : Highchart
    {

        public HighchartColumnNegative() : base()
        {
            Chart = new HighchartColumnNegativeChart();
            Title = new HighchartColumnNegativeTitle();
            XAxis = new HighchartColumnNegativeXAxis();
            Credits = new HighchartColumnNegativeCredits();
            Series = new List<HighchartColumnNegativeSeriesItem> { new HighchartColumnNegativeSeriesItem(), new HighchartColumnNegativeSeriesItem(), new HighchartColumnNegativeSeriesItem() };

        }


        [JsonProperty("chart")]
        [Label("Chart")]
        public HighchartColumnNegativeChart Chart { get; set; }

        [JsonProperty("title")]
        [Label("Title")]
        public HighchartColumnNegativeTitle Title { get; set; }

        [JsonProperty("xAxis")]
        public HighchartColumnNegativeXAxis XAxis { get; set; }

        [JsonProperty("credits")]
        public HighchartColumnNegativeCredits Credits { get; set; }

        [JsonProperty("series")]
        public List<HighchartColumnNegativeSeriesItem> Series { get; set; }
    }


}