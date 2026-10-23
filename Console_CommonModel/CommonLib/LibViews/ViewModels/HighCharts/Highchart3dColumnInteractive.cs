using System.Collections.Generic;

using Newtonsoft.Json;
using System;

namespace Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts
{
    [Label("Конфигурация Highcharts: 3 d Column Interactive Chart Options 3 d Enabled")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Column Interactive Chart Options 3 d Enabled.")]
    public class Highchart3dColumnInteractiveChartOptions3dEnabled : Highchart
    {

        public Highchart3dColumnInteractiveChartOptions3dEnabled() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: 3 d Column Interactive Chart Options 3 d")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Column Interactive Chart Options 3 d.")]
    public class Highchart3dColumnInteractiveChartOptions3d : Highchart
    {

        public Highchart3dColumnInteractiveChartOptions3d() : base()
        {
            Enabled = new Highchart3dColumnInteractiveChartOptions3dEnabled();
            Alpha = 15f;
            Beta = 15f;
            Depth = 50f;
            ViewDistance = 25f;

        }


        [JsonProperty("enabled")]
        [Label("Enabled")]
        public Highchart3dColumnInteractiveChartOptions3dEnabled Enabled { get; set; }
        [InputNumber("alpha")]
        [JsonProperty("alpha")]
        [Label("Alpha")]
        public float Alpha { get; set; }
        [InputNumber("beta")]
        [JsonProperty("beta")]
        [Label("Beta")]
        public float Beta { get; set; }
        [InputNumber("depth")]
        [JsonProperty("depth")]
        [Label("Depth")]
        public float Depth { get; set; }
        [InputNumber("viewDistance")]
        [JsonProperty("viewDistance")]
        [Label("View Distance")]
        public float ViewDistance { get; set; }
    }
    [Label("Конфигурация Highcharts: 3 d Column Interactive Chart")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Column Interactive Chart.")]
    public class Highchart3dColumnInteractiveChart : Highchart
    {

        public Highchart3dColumnInteractiveChart() : base()
        {
            RenderTo = "container";
            Type = "column";
            Options3d = new Highchart3dColumnInteractiveChartOptions3d();

        }

        [InputText("renderTo")]
        [JsonProperty("renderTo")]
        [Label("Render To")]
        public string RenderTo { get; set; }

        [InputText("type")]
        [JsonProperty("type")]
        [Label("Type")]
        public string Type { get; set; }


        [JsonProperty("options3d")]
        [Label("Options 3 d")]
        public Highchart3dColumnInteractiveChartOptions3d Options3d { get; set; }
    }
    [Label("Конфигурация Highcharts: 3 d Column Interactive Title")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Column Interactive Title.")]
    public class Highchart3dColumnInteractiveTitle : Highchart
    {

        public Highchart3dColumnInteractiveTitle() : base()
        {
            Text = "Chart rotation demo";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: 3 d Column Interactive Subtitle")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Column Interactive Subtitle.")]
    public class Highchart3dColumnInteractiveSubtitle : Highchart
    {

        public Highchart3dColumnInteractiveSubtitle() : base()
        {
            Text = "Test options by dragging the sliders below";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: 3 d Column Interactive Plot Options Column")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Column Interactive Plot Options Column.")]
    public class Highchart3dColumnInteractivePlotOptionsColumn : Highchart
    {

        public Highchart3dColumnInteractivePlotOptionsColumn() : base()
        {
            Depth = 25f;

        }

        [InputNumber("depth")]
        [JsonProperty("depth")]
        [Label("Depth")]
        public float Depth { get; set; }
    }
    [Label("Конфигурация Highcharts: 3 d Column Interactive Plot Options")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Column Interactive Plot Options.")]
    public class Highchart3dColumnInteractivePlotOptions : Highchart
    {

        public Highchart3dColumnInteractivePlotOptions() : base()
        {
            Column = new Highchart3dColumnInteractivePlotOptionsColumn();

        }


        [JsonProperty("column")]
        [Label("Column")]
        public Highchart3dColumnInteractivePlotOptionsColumn Column { get; set; }
    }
    [Label("Конфигурация Highcharts: 3 d Column Interactive Series Item")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Column Interactive Series Item.")]
    public class Highchart3dColumnInteractiveSeriesItem : Highchart
    {

        public Highchart3dColumnInteractiveSeriesItem() : base()
        {
            Data = new List<float?> { 29.9f, 71.5f, 106.4f, 129.2f, 144f, 176f, 135.6f, 148.5f, 216.4f, 194.1f, 95.6f, 54.4f };

        }


        [JsonProperty("data")]
        [Label("Data")]
        public List<float?> Data { get; set; }
    }
    [Label("Диаграмма Highcharts: 3 d Column Interactive")]
    [Description("Модель диаграммы Highcharts: 3 d Column Interactive.")]
    public class Highchart3dColumnInteractive : Highchart
    {

        public Highchart3dColumnInteractive() : base()
        {
            Chart = new Highchart3dColumnInteractiveChart();
            Title = new Highchart3dColumnInteractiveTitle();
            Subtitle = new Highchart3dColumnInteractiveSubtitle();
            PlotOptions = new Highchart3dColumnInteractivePlotOptions();
            Series = new List<Highchart3dColumnInteractiveSeriesItem> { new Highchart3dColumnInteractiveSeriesItem() };

        }


        [JsonProperty("chart")]
        [Label("Chart")]
        public Highchart3dColumnInteractiveChart Chart { get; set; }


        [Label("Заголовок")]
        [JsonProperty("title")]
        public Highchart3dColumnInteractiveTitle Title { get; set; }


        [Label("Подзаголовок")]
        [JsonProperty("subtitle")]
        public Highchart3dColumnInteractiveSubtitle Subtitle { get; set; }

        [JsonProperty("plotOptions")]
        public Highchart3dColumnInteractivePlotOptions PlotOptions { get; set; }


        [Label("Данные")]
        [JsonProperty("series")]
        public List<Highchart3dColumnInteractiveSeriesItem> Series { get; set; }
    }


}