using System.Collections.Generic;

using Newtonsoft.Json;
using System;

namespace Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts
{
    [Label("Конфигурация Highcharts: Area Negative Chart")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Negative Chart.")]
    public class HighchartAreaNegativeChart : Highchart
    {

        public HighchartAreaNegativeChart() : base()
        {
            Type = "area";

        }

        [InputText("type")]
        [JsonProperty("type")]
        [Label("Type")]
        public string Type { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Negative Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Negative Title.")]
    public class HighchartAreaNegativeTitle : Highchart
    {

        public HighchartAreaNegativeTitle() : base()
        {
            Text = "Area chart with negative values";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Negative X Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Negative X Axis.")]
    public class HighchartAreaNegativeXAxis : Highchart
    {

        public HighchartAreaNegativeXAxis() : base()
        {
            Categories = new List<string> { "Apples", "Oranges", "Pears", "Grapes", "Bananas" };

        }


        [JsonProperty("categories")]
        [Label("Categories")]
        public List<string> Categories { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Negative Credits")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Negative Credits.")]
    public class HighchartAreaNegativeCredits : Highchart
    {

        public HighchartAreaNegativeCredits() : base()
        {
            Enabled = null;

        }


        [JsonProperty("enabled")]
        [Label("Enabled")]
        public object Enabled { get; set; }
    }
    [Label("Конфигурация Highcharts: Area Negative Series Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Area Negative Series Item.")]
    public class HighchartAreaNegativeSeriesItem : Highchart
    {

        public HighchartAreaNegativeSeriesItem() : base()
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
    [Label("Диаграмма Highcharts: Area Negative")]
    [Description("Модель диаграммы Highcharts: Area Negative.")]
    public class HighchartAreaNegative : Highchart
    {

        public HighchartAreaNegative() : base()
        {
            Chart = new HighchartAreaNegativeChart();
            Title = new HighchartAreaNegativeTitle();
            XAxis = new HighchartAreaNegativeXAxis();
            Credits = new HighchartAreaNegativeCredits();
            Series = new List<HighchartAreaNegativeSeriesItem> { new HighchartAreaNegativeSeriesItem(), new HighchartAreaNegativeSeriesItem(), new HighchartAreaNegativeSeriesItem() };

        }


        [JsonProperty("chart")]
        [Label("Chart")]
        public HighchartAreaNegativeChart Chart { get; set; }

        [JsonProperty("title")]
        [Label("Title")]
        public HighchartAreaNegativeTitle Title { get; set; }

        [JsonProperty("xAxis")]
        public HighchartAreaNegativeXAxis XAxis { get; set; }

        [JsonProperty("credits")]
        public HighchartAreaNegativeCredits Credits { get; set; }

        [JsonProperty("series")]
        public List<HighchartAreaNegativeSeriesItem> Series { get; set; }
    }


}