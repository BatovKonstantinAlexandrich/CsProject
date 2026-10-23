using System.Collections.Generic;

using Newtonsoft.Json;
using System;

namespace Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts
{
    [Label("Конфигурация Highcharts: 3 d Column Null Values Chart Options 3 d Enabled")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Column Null Values Chart Options 3 d Enabled.")]
    public class Highchart3dColumnNullValuesChartOptions3dEnabled : Highchart
    {

        public Highchart3dColumnNullValuesChartOptions3dEnabled() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: 3 d Column Null Values Chart Options 3 d")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Column Null Values Chart Options 3 d.")]
    public class Highchart3dColumnNullValuesChartOptions3d : Highchart
    {

        public Highchart3dColumnNullValuesChartOptions3d() : base()
        {
            Enabled = new Highchart3dColumnNullValuesChartOptions3dEnabled();
            Alpha = 10f;
            Beta = 25f;
            Depth = 70f;

        }


        [JsonProperty("enabled")]
        [Label("Enabled")]
        public Highchart3dColumnNullValuesChartOptions3dEnabled Enabled { get; set; }
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
    }
    [Label("Конфигурация Highcharts: 3 d Column Null Values Chart")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Column Null Values Chart.")]
    public class Highchart3dColumnNullValuesChart : Highchart
    {

        public Highchart3dColumnNullValuesChart() : base()
        {
            Type = "column";
            Options3d = new Highchart3dColumnNullValuesChartOptions3d();

        }

        [InputText("type")]
        [JsonProperty("type")]
        [Label("Type")]
        public string Type { get; set; }

        [JsonProperty("options3d")]
        [Label("Options 3 d")]
        public Highchart3dColumnNullValuesChartOptions3d Options3d { get; set; }
    }
    [Label("Конфигурация Highcharts: 3 d Column Null Values Title")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Column Null Values Title.")]
    public class Highchart3dColumnNullValuesTitle : Highchart
    {

        public Highchart3dColumnNullValuesTitle() : base()
        {
            Text = "3D chart with null values";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: 3 d Column Null Values Subtitle")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Column Null Values Subtitle.")]
    public class Highchart3dColumnNullValuesSubtitle : Highchart
    {

        public Highchart3dColumnNullValuesSubtitle() : base()
        {
            Text = "Notice the difference between a 0 value and a null point";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: 3 d Column Null Values Plot Options Column")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Column Null Values Plot Options Column.")]
    public class Highchart3dColumnNullValuesPlotOptionsColumn : Highchart
    {

        public Highchart3dColumnNullValuesPlotOptionsColumn() : base()
        {
            Depth = 25f;

        }

        [InputNumber("depth")]
        [JsonProperty("depth")]
        [Label("Depth")]
        public float Depth { get; set; }
    }
    [Label("Конфигурация Highcharts: 3 d Column Null Values Plot Options")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Column Null Values Plot Options.")]
    public class Highchart3dColumnNullValuesPlotOptions : Highchart
    {

        public Highchart3dColumnNullValuesPlotOptions() : base()
        {
            Column = new Highchart3dColumnNullValuesPlotOptionsColumn();

        }


        [JsonProperty("column")]
        [Label("Column")]
        public Highchart3dColumnNullValuesPlotOptionsColumn Column { get; set; }
    }
    [Label("Конфигурация Highcharts: 3 d Column Null Values X Axis Labels Skew 3 d")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Column Null Values X Axis Labels Skew 3 d.")]
    public class Highchart3dColumnNullValuesXAxisLabelsSkew3d : Highchart
    {

        public Highchart3dColumnNullValuesXAxisLabelsSkew3d() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: 3 d Column Null Values X Axis Labels Style")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Column Null Values X Axis Labels Style.")]
    public class Highchart3dColumnNullValuesXAxisLabelsStyle : Highchart
    {

        public Highchart3dColumnNullValuesXAxisLabelsStyle() : base()
        {
            FontSize = "16px";
            //EnableChangeSupport = false;
        }

        [InputText("fontSize")]
        [JsonProperty("fontSize")]
        [Label("Font Size")]
        public string FontSize { get; set; }
    }
    [Label("Конфигурация Highcharts: 3 d Column Null Values X Axis Labels")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Column Null Values X Axis Labels.")]
    public class Highchart3dColumnNullValuesXAxisLabels : Highchart
    {

        public Highchart3dColumnNullValuesXAxisLabels() : base()
        {
            Skew3d = new Highchart3dColumnNullValuesXAxisLabelsSkew3d();
            Style = new Highchart3dColumnNullValuesXAxisLabelsStyle();

        }


        [JsonProperty("skew3d")]
        [Label("Skew 3 d")]
        public Highchart3dColumnNullValuesXAxisLabelsSkew3d Skew3d { get; set; }

        [JsonProperty("style")]
        [Label("Style")]
        public Highchart3dColumnNullValuesXAxisLabelsStyle Style { get; set; }
    }
    [Label("Конфигурация Highcharts: 3 d Column Null Values X Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Column Null Values X Axis.")]
    public class Highchart3dColumnNullValuesXAxis : Highchart
    {

        public Highchart3dColumnNullValuesXAxis() : base()
        {
            Categories = new List<string> { "Январь", "Февраль", "март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь" };
            Labels = new Highchart3dColumnNullValuesXAxisLabels();

        }


        [JsonProperty("categories")]
        [Label("Categories")]
        public List<string> Categories { get; set; }

        [JsonProperty("labels")]
        [Label("Labels")]
        public Highchart3dColumnNullValuesXAxisLabels Labels { get; set; }
    }
    [Label("Конфигурация Highcharts: 3 d Column Null Values Y Axis Title")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Column Null Values Y Axis Title.")]
    public class Highchart3dColumnNullValuesYAxisTitle : Highchart
    {

        public Highchart3dColumnNullValuesYAxisTitle() : base()
        {
            Text = null;

        }


        [JsonProperty("text")]
        [Label("Text")]
        public object Text { get; set; }
    }
    [Label("Конфигурация Highcharts: 3 d Column Null Values Y Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Column Null Values Y Axis.")]
    public class Highchart3dColumnNullValuesYAxis : Highchart
    {

        public Highchart3dColumnNullValuesYAxis() : base()
        {
            Title = new Highchart3dColumnNullValuesYAxisTitle();

        }


        [JsonProperty("title")]
        [Label("Title")]
        public Highchart3dColumnNullValuesYAxisTitle Title { get; set; }
    }
    [Label("Конфигурация Highcharts: 3 d Column Null Values Series Item")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Column Null Values Series Item.")]
    public class Highchart3dColumnNullValuesSeriesItem : Highchart
    {

        public Highchart3dColumnNullValuesSeriesItem() : base()
        {
            Name = "Sales";
            Data = new List<float?> { 2f, 3f, null, 4f, 0, 5f, 1f, 4f, 6f, 3f };

        }

        [InputText("name")]
        [JsonProperty("name")]
        [Label("Name")]
        public string Name { get; set; }


        [JsonProperty("data")]
        [Label("Data")]
        public List<float?> Data { get; set; }
    }
    [Label("Диаграмма Highcharts: 3 d Column Null Values")]
    [Description("Модель диаграммы Highcharts: 3 d Column Null Values.")]
    public class Highchart3dColumnNullValues : Highchart
    {

        public Highchart3dColumnNullValues() : base()
        {
            Chart = new Highchart3dColumnNullValuesChart();
            Title = new Highchart3dColumnNullValuesTitle();
            Subtitle = new Highchart3dColumnNullValuesSubtitle();
            PlotOptions = new Highchart3dColumnNullValuesPlotOptions();
            XAxis = new Highchart3dColumnNullValuesXAxis();
            YAxis = new Highchart3dColumnNullValuesYAxis();
            Series = new List<Highchart3dColumnNullValuesSeriesItem> { new Highchart3dColumnNullValuesSeriesItem() };

        }


        [JsonProperty("chart")]
        public Highchart3dColumnNullValuesChart Chart { get; set; }

        [JsonProperty("title")]
        public Highchart3dColumnNullValuesTitle Title { get; set; }

        [JsonProperty("subtitle")]
        public Highchart3dColumnNullValuesSubtitle Subtitle { get; set; }

        [JsonProperty("plotOptions")]
        public Highchart3dColumnNullValuesPlotOptions PlotOptions { get; set; }

        [JsonProperty("xAxis")]
        public Highchart3dColumnNullValuesXAxis XAxis { get; set; }

        [JsonProperty("yAxis")]
        public Highchart3dColumnNullValuesYAxis YAxis { get; set; }

        [JsonProperty("series")]
        public List<Highchart3dColumnNullValuesSeriesItem> Series { get; set; }
    }


}