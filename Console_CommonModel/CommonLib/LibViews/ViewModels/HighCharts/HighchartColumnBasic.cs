using System.Collections.Generic;

using Newtonsoft.Json;
using System;

namespace Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts
{
    [Label("Конфигурация Highcharts: Column Basic Chart")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Basic Chart.")]
    public class HighchartColumnBasicChart : Highchart
    {

        public HighchartColumnBasicChart() : base()
        {
            Type = "column";

        }

        [InputText("type")]
        [JsonProperty("type")]
        [Label("Type")]
        public string Type { get; set; }
    }
    [Label("Конфигурация Highcharts: Column Basic Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Basic Title.")]
    public class HighchartColumnBasicTitle : Highchart
    {

        public HighchartColumnBasicTitle() : base()
        {
            Text = "Monthly Average Rainfall";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Column Basic Subtitle")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Basic Subtitle.")]
    public class HighchartColumnBasicSubtitle : Highchart
    {

        public HighchartColumnBasicSubtitle() : base()
        {
            Text = "Source: WorldClimate.com";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Column Basic X Axis Crosshair")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Basic X Axis Crosshair.")]
    public class HighchartColumnBasicXAxisCrosshair : Highchart
    {

        public HighchartColumnBasicXAxisCrosshair() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: Column Basic X Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Basic X Axis.")]
    public class HighchartColumnBasicXAxis : Highchart
    {

        public HighchartColumnBasicXAxis() : base()
        {
            Categories = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            Crosshair = new HighchartColumnBasicXAxisCrosshair();

        }


        [JsonProperty("categories")]
        [Label("Categories")]
        public List<string> Categories { get; set; }

        [JsonProperty("crosshair")]
        [Label("Crosshair")]
        public HighchartColumnBasicXAxisCrosshair Crosshair { get; set; }
    }
    [Label("Конфигурация Highcharts: Column Basic Y Axis Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Basic Y Axis Title.")]
    public class HighchartColumnBasicYAxisTitle : Highchart
    {

        public HighchartColumnBasicYAxisTitle() : base()
        {
            Text = "Rainfall (mm)";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Column Basic Y Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Basic Y Axis.")]
    public class HighchartColumnBasicYAxis : Highchart
    {

        public HighchartColumnBasicYAxis() : base()
        {
            Min = null;
            Title = new HighchartColumnBasicYAxisTitle();

        }


        [JsonProperty("min")]
        [Label("Min")]
        public object Min { get; set; }

        [JsonProperty("title")]
        [Label("Title")]
        public HighchartColumnBasicYAxisTitle Title { get; set; }
    }
    [Label("Конфигурация Highcharts: Column Basic Tooltip Shared")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Basic Tooltip Shared.")]
    public class HighchartColumnBasicTooltipShared : Highchart
    {

        public HighchartColumnBasicTooltipShared() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: Column Basic Tooltip Use HTML")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Basic Tooltip Use HTML.")]
    public class HighchartColumnBasicTooltipUseHTML : Highchart
    {

        public HighchartColumnBasicTooltipUseHTML() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: Column Basic Tooltip")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Basic Tooltip.")]
    public class HighchartColumnBasicTooltip : Highchart
    {

        public HighchartColumnBasicTooltip() : base()
        {
            HeaderFormat = "<span style='font-size:10px'>{point.key}</span><table>";
            PointFormat = "<tr><td style='color:{series.color};padding:0'>{series.name}: </td><td style='padding:0'><b>{point.y:.1f} mm</b></td></tr>";
            FooterFormat = "</table>";
            Shared = new HighchartColumnBasicTooltipShared();
            UseHTML = new HighchartColumnBasicTooltipUseHTML();

        }

        [InputText("headerFormat")]
        [JsonProperty("headerFormat")]
        [Label("Header Format")]
        public string HeaderFormat { get; set; }
        [InputText("pointFormat")]
        [JsonProperty("pointFormat")]
        [Label("Point Format")]
        public string PointFormat { get; set; }
        [InputText("footerFormat")]
        [JsonProperty("footerFormat")]
        [Label("Footer Format")]
        public string FooterFormat { get; set; }

        [JsonProperty("shared")]
        [Label("Shared")]
        public HighchartColumnBasicTooltipShared Shared { get; set; }

        [JsonProperty("useHTML")]
        [Label("Use HTML")]
        public HighchartColumnBasicTooltipUseHTML UseHTML { get; set; }
    }
    [Label("Конфигурация Highcharts: Column Basic Plot Options Column")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Basic Plot Options Column.")]
    public class HighchartColumnBasicPlotOptionsColumn : Highchart
    {

        public HighchartColumnBasicPlotOptionsColumn() : base()
        {
            PointPadding = 0.2f;
            BorderWidth = null;

        }

        [InputNumber("pointPadding")]
        [JsonProperty("pointPadding")]
        [Label("Point Padding")]
        public float PointPadding { get; set; }

        [JsonProperty("borderWidth")]
        [Label("Border Width")]
        public object BorderWidth { get; set; }
    }
    [Label("Конфигурация Highcharts: Column Basic Plot Options")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Basic Plot Options.")]
    public class HighchartColumnBasicPlotOptions : Highchart
    {

        public HighchartColumnBasicPlotOptions() : base()
        {
            Column = new HighchartColumnBasicPlotOptionsColumn();

        }


        [JsonProperty("column")]
        [Label("Column")]
        public HighchartColumnBasicPlotOptionsColumn Column { get; set; }
    }
    [Label("Конфигурация Highcharts: Column Basic Series Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Column Basic Series Item.")]
    public class HighchartColumnBasicSeriesItem : Highchart
    {

        public HighchartColumnBasicSeriesItem() : base()
        {
            Name = "Tokyo";
            Data = new List<float?> { 49.9f, 71.5f, 106.4f, 129.2f, 144f, 176f, 135.6f, 148.5f, 216.4f, 194.1f, 95.6f, 54.4f };

        }

        [InputText("name")]
        [JsonProperty("name")]
        [Label("Name")]
        public string Name { get; set; }

        [JsonProperty("data")]
        [Label("Data")]
        public List<float?> Data { get; set; }
    }
    [Label("Диаграмма Highcharts: Column Basic")]
    [Description("Модель диаграммы Highcharts: Column Basic.")]
    public class HighchartColumnBasic : Highchart
    {

        public HighchartColumnBasic() : base()
        {
            Chart = new HighchartColumnBasicChart();
            Title = new HighchartColumnBasicTitle();
            Subtitle = new HighchartColumnBasicSubtitle();
            XAxis = new HighchartColumnBasicXAxis();
            YAxis = new HighchartColumnBasicYAxis();
            Tooltip = new HighchartColumnBasicTooltip();
            PlotOptions = new HighchartColumnBasicPlotOptions();
            Series = new List<HighchartColumnBasicSeriesItem> { new HighchartColumnBasicSeriesItem(), new HighchartColumnBasicSeriesItem(), new HighchartColumnBasicSeriesItem(), new HighchartColumnBasicSeriesItem() };

        }


        [JsonProperty("chart")]
        public HighchartColumnBasicChart Chart { get; set; }

        [JsonProperty("title")]
        public HighchartColumnBasicTitle Title { get; set; }

        [JsonProperty("subtitle")]
        public HighchartColumnBasicSubtitle Subtitle { get; set; }

        [JsonProperty("xAxis")]
        public HighchartColumnBasicXAxis XAxis { get; set; }

        [JsonProperty("yAxis")]
        public HighchartColumnBasicYAxis YAxis { get; set; }

        [JsonProperty("tooltip")]
        public HighchartColumnBasicTooltip Tooltip { get; set; }

        [JsonProperty("plotOptions")]
        public HighchartColumnBasicPlotOptions PlotOptions { get; set; }

        [JsonProperty("series")]
        public List<HighchartColumnBasicSeriesItem> Series { get; set; }
    }


}