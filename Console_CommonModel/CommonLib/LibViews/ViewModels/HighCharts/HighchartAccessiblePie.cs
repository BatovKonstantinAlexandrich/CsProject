using System.Collections.Generic;

using Newtonsoft.Json;
using System;

namespace Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts
{
    [Label("Конфигурация Highcharts: Accessible Pie Chart")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Pie Chart.")]
    public class HighchartAccessiblePieChart : Highchart
    {

        public HighchartAccessiblePieChart() : base()
        {
            Type = "pie";

        }

        [InputText("type")]
        [JsonProperty("type")]
        [Label("Type")]
        public string Type { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Pie Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Pie Title.")]
    public class HighchartAccessiblePieTitle : Highchart
    {

        public HighchartAccessiblePieTitle() : base()
        {
            Text = "Primary desktop/laptop screen readers";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Pie Subtitle")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Pie Subtitle.")]
    public class HighchartAccessiblePieSubtitle : Highchart
    {

        public HighchartAccessiblePieSubtitle() : base()
        {
            Text = "Source: WebAIM. Click on point to visit official website";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Pie Tooltip")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Pie Tooltip.")]
    public class HighchartAccessiblePieTooltip : Highchart
    {

        public HighchartAccessiblePieTooltip() : base()
        {
            ValueSuffix = "%";
            BorderColor = "#8ae";

        }

        [InputText("valueSuffix")]
        [JsonProperty("valueSuffix")]
        [Label("Value Suffix")]
        public string ValueSuffix { get; set; }
        [InputText("borderColor")]
        [JsonProperty("borderColor")]
        [Label("Border Color")]
        public string BorderColor { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Pie Plot Options Series Data Labels Enabled")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Pie Plot Options Series Data Labels Enabled.")]
    public class HighchartAccessiblePiePlotOptionsSeriesDataLabelsEnabled : Highchart
    {

        public HighchartAccessiblePiePlotOptionsSeriesDataLabelsEnabled() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: Accessible Pie Plot Options Series Data Labels")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Pie Plot Options Series Data Labels.")]
    public class HighchartAccessiblePiePlotOptionsSeriesDataLabels : Highchart
    {

        public HighchartAccessiblePiePlotOptionsSeriesDataLabels() : base()
        {
            Enabled = new HighchartAccessiblePiePlotOptionsSeriesDataLabelsEnabled();
            ConnectorColor = "#777";
            Format = "<b>{point.name}</b>: {point.percentage:.1f} %";

        }


        [JsonProperty("enabled")]
        [Label("Enabled")]
        public HighchartAccessiblePiePlotOptionsSeriesDataLabelsEnabled Enabled { get; set; }
        [InputText("connectorColor")]
        [JsonProperty("connectorColor")]
        [Label("Connector Color")]
        public string ConnectorColor { get; set; }
        [InputText("format")]
        [JsonProperty("format")]
        [Label("Format")]
        public string Format { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Pie Plot Options Series Point Events")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Pie Plot Options Series Point Events.")]
    public class HighchartAccessiblePiePlotOptionsSeriesPointEvents : Highchart
    {

        public HighchartAccessiblePiePlotOptionsSeriesPointEvents() : base()
        {
            Click = () => { };

        }


        [JsonProperty("click")]
        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [Label("Click")]
        public Action Click { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Pie Plot Options Series Point")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Pie Plot Options Series Point.")]
    public class HighchartAccessiblePiePlotOptionsSeriesPoint : Highchart
    {

        public HighchartAccessiblePiePlotOptionsSeriesPoint() : base()
        {
            Events = new HighchartAccessiblePiePlotOptionsSeriesPointEvents();

        }


        [JsonProperty("events")]
        [Label("Events")]
        public HighchartAccessiblePiePlotOptionsSeriesPointEvents Events { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Pie Plot Options Series")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Pie Plot Options Series.")]
    public class HighchartAccessiblePiePlotOptionsSeries : Highchart
    {

        public HighchartAccessiblePiePlotOptionsSeries() : base()
        {
            DataLabels = new HighchartAccessiblePiePlotOptionsSeriesDataLabels();
            Point = new HighchartAccessiblePiePlotOptionsSeriesPoint();
            Cursor = "pointer";
            BorderWidth = 3f;

        }


        [JsonProperty("dataLabels")]
        [Label("Data Labels")]
        public HighchartAccessiblePiePlotOptionsSeriesDataLabels DataLabels { get; set; }

        [JsonProperty("point")]
        [Label("Point")]
        public HighchartAccessiblePiePlotOptionsSeriesPoint Point { get; set; }
        [InputText("cursor")]
        [JsonProperty("cursor")]
        [Label("Cursor")]
        public string Cursor { get; set; }
        [InputNumber("borderWidth")]
        [JsonProperty("borderWidth")]
        [Label("Border Width")]
        public float BorderWidth { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Pie Plot Options")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Pie Plot Options.")]
    public class HighchartAccessiblePiePlotOptions : Highchart
    {

        public HighchartAccessiblePiePlotOptions() : base()
        {
            Series = new HighchartAccessiblePiePlotOptionsSeries();

        }


        [JsonProperty("series")]
        [Label("Series")]
        public HighchartAccessiblePiePlotOptionsSeries Series { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Pie Series Item Data Item Color Pattern")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Pie Series Item Data Item Color Pattern.")]
    public class HighchartAccessiblePieSeriesItemDataItemColorPattern : Highchart
    {

        public HighchartAccessiblePieSeriesItemDataItemColorPattern() : base()
        {
            Path = "M 0 0 L 5 5 M 4.5 -0.5 L 5.5 0.5 M -0.5 4.5 L 0.5 5.5";
            Color = "#49a65e";
            Width = 5f;
            Height = 5f;

        }

        [InputText("path")]
        [JsonProperty("path")]
        [Label("Path")]
        public string Path { get; set; }
        [InputText("color")]
        [JsonProperty("color")]
        [Label("Color")]
        public string Color { get; set; }
        [InputNumber("width")]
        [JsonProperty("width")]
        [Label("Width")]
        public float Width { get; set; }
        [InputNumber("height")]
        [JsonProperty("height")]
        [Label("Height")]
        public float Height { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Pie Series Item Data Item Color")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Pie Series Item Data Item Color.")]
    public class HighchartAccessiblePieSeriesItemDataItemColor : Highchart
    {

        public HighchartAccessiblePieSeriesItemDataItemColor() : base()
        {
            Pattern = new HighchartAccessiblePieSeriesItemDataItemColorPattern();

        }


        [JsonProperty("pattern")]
        [Label("Pattern")]
        public HighchartAccessiblePieSeriesItemDataItemColorPattern Pattern { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Pie Series Item Data Item Accessibility")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Pie Series Item Data Item Accessibility.")]
    public class HighchartAccessiblePieSeriesItemDataItemAccessibility : Highchart
    {

        public HighchartAccessiblePieSeriesItemDataItemAccessibility() : base()
        {
            Description = "This is the most used desktop screen reader";

        }

        [InputText("description")]
        [JsonProperty("description")]
        [Label("Description")]
        public string Description { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Pie Series Item Data Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Pie Series Item Data Item.")]
    public class HighchartAccessiblePieSeriesItemDataItem : Highchart
    {

        public HighchartAccessiblePieSeriesItemDataItem() : base()
        {
            Name = "NVDA";
            Y = 40.6f;
            Color = new HighchartAccessiblePieSeriesItemDataItemColor();
            Website = " ";
            Accessibility = new HighchartAccessiblePieSeriesItemDataItemAccessibility();

        }

        [InputText("name")]
        [JsonProperty("name")]
        [Label("Name")]
        public string Name { get; set; }
        [InputNumber("y")]
        [JsonProperty("y")]
        [Label("Y")]
        public float Y { get; set; }

        [JsonProperty("color")]
        [Label("Color")]
        public HighchartAccessiblePieSeriesItemDataItemColor Color { get; set; }
        [InputText("website")]
        [JsonProperty("website")]
        [Label("Website")]
        public string Website { get; set; }

        [JsonProperty("accessibility")]
        [Label("Accessibility")]
        public HighchartAccessiblePieSeriesItemDataItemAccessibility Accessibility { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Pie Series Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Pie Series Item.")]
    public class HighchartAccessiblePieSeriesItem : Highchart
    {

        public HighchartAccessiblePieSeriesItem() : base()
        {
            Name = "Screen reader usage";
            Data = new List<HighchartAccessiblePieSeriesItemDataItem> { new HighchartAccessiblePieSeriesItemDataItem(), new HighchartAccessiblePieSeriesItemDataItem(), new HighchartAccessiblePieSeriesItemDataItem(), new HighchartAccessiblePieSeriesItemDataItem(), new HighchartAccessiblePieSeriesItemDataItem() };

        }

        [InputText("name")]
        [JsonProperty("name")]
        [Label("Name")]
        public string Name { get; set; }

        [JsonProperty("data")]
        [Label("Data")]
        public List<HighchartAccessiblePieSeriesItemDataItem> Data { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Pie Responsive Rules Item Condition")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Pie Responsive Rules Item Condition.")]
    public class HighchartAccessiblePieResponsiveRulesItemCondition : Highchart
    {

        public HighchartAccessiblePieResponsiveRulesItemCondition() : base()
        {
            MaxWidth = 500f;

        }

        [InputNumber("maxWidth")]
        [JsonProperty("maxWidth")]
        [Label("Max Width")]
        public float MaxWidth { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Pie Responsive Rules Item Chart Options Plot Options Series Data Labels")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Pie Responsive Rules Item Chart Options Plot Options Series Data Labels.")]
    public class HighchartAccessiblePieResponsiveRulesItemChartOptionsPlotOptionsSeriesDataLabels : Highchart
    {

        public HighchartAccessiblePieResponsiveRulesItemChartOptionsPlotOptionsSeriesDataLabels() : base()
        {
            Format = "<b>{point.name}</b>";

        }

        [InputText("format")]
        [JsonProperty("format")]
        [Label("Format")]
        public string Format { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Pie Responsive Rules Item Chart Options Plot Options Series")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Pie Responsive Rules Item Chart Options Plot Options Series.")]
    public class HighchartAccessiblePieResponsiveRulesItemChartOptionsPlotOptionsSeries : Highchart
    {

        public HighchartAccessiblePieResponsiveRulesItemChartOptionsPlotOptionsSeries() : base()
        {
            DataLabels = new HighchartAccessiblePieResponsiveRulesItemChartOptionsPlotOptionsSeriesDataLabels();

        }


        [JsonProperty("dataLabels")]
        [Label("Data Labels")]
        public HighchartAccessiblePieResponsiveRulesItemChartOptionsPlotOptionsSeriesDataLabels DataLabels { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Pie Responsive Rules Item Chart Options Plot Options")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Pie Responsive Rules Item Chart Options Plot Options.")]
    public class HighchartAccessiblePieResponsiveRulesItemChartOptionsPlotOptions : Highchart
    {

        public HighchartAccessiblePieResponsiveRulesItemChartOptionsPlotOptions() : base()
        {
            Series = new HighchartAccessiblePieResponsiveRulesItemChartOptionsPlotOptionsSeries();

        }


        [JsonProperty("series")]
        [Label("Series")]
        public HighchartAccessiblePieResponsiveRulesItemChartOptionsPlotOptionsSeries Series { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Pie Responsive Rules Item Chart Options")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Pie Responsive Rules Item Chart Options.")]
    public class HighchartAccessiblePieResponsiveRulesItemChartOptions : Highchart
    {

        public HighchartAccessiblePieResponsiveRulesItemChartOptions() : base()
        {
            PlotOptions = new HighchartAccessiblePieResponsiveRulesItemChartOptionsPlotOptions();

        }


        [JsonProperty("plotOptions")]
        [Label("Plot Options")]
        public HighchartAccessiblePieResponsiveRulesItemChartOptionsPlotOptions PlotOptions { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Pie Responsive Rules Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Pie Responsive Rules Item.")]
    public class HighchartAccessiblePieResponsiveRulesItem : Highchart
    {

        public HighchartAccessiblePieResponsiveRulesItem() : base()
        {
            Condition = new HighchartAccessiblePieResponsiveRulesItemCondition();
            ChartOptions = new HighchartAccessiblePieResponsiveRulesItemChartOptions();

        }


        [JsonProperty("condition")]
        [Label("Condition")]
        public HighchartAccessiblePieResponsiveRulesItemCondition Condition { get; set; }

        [JsonProperty("chartOptions")]
        [Label("Chart Options")]
        public HighchartAccessiblePieResponsiveRulesItemChartOptions ChartOptions { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Pie Responsive")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Pie Responsive.")]
    public class HighchartAccessiblePieResponsive : Highchart
    {

        public HighchartAccessiblePieResponsive() : base()
        {
            Rules = new List<HighchartAccessiblePieResponsiveRulesItem> { new HighchartAccessiblePieResponsiveRulesItem() };

        }


        [JsonProperty("rules")]
        public List<HighchartAccessiblePieResponsiveRulesItem> Rules { get; set; }
    }
    [Label("Диаграмма Highcharts: Accessible Pie")]
    [Description("Модель диаграммы Highcharts: Accessible Pie.")]
    public class HighchartAccessiblePie : Highchart
    {

        public HighchartAccessiblePie() : base()
        {
            Chart = new HighchartAccessiblePieChart();
            Title = new HighchartAccessiblePieTitle();
            Subtitle = new HighchartAccessiblePieSubtitle();
            Tooltip = new HighchartAccessiblePieTooltip();
            PlotOptions = new HighchartAccessiblePiePlotOptions();
            Series = new List<HighchartAccessiblePieSeriesItem> { new HighchartAccessiblePieSeriesItem() };
            Responsive = new HighchartAccessiblePieResponsive();

        }


        [JsonProperty("chart")]
        public HighchartAccessiblePieChart Chart { get; set; }

        [JsonProperty("title")]
        public HighchartAccessiblePieTitle Title { get; set; }

        [JsonProperty("subtitle")]
        public HighchartAccessiblePieSubtitle Subtitle { get; set; }

        [JsonProperty("tooltip")]
        public HighchartAccessiblePieTooltip Tooltip { get; set; }

        [JsonProperty("plotOptions")]
        public HighchartAccessiblePiePlotOptions PlotOptions { get; set; }

        [JsonProperty("series")]
        public List<HighchartAccessiblePieSeriesItem> Series { get; set; }

        [JsonProperty("responsive")]
        public HighchartAccessiblePieResponsive Responsive { get; set; }
    }


}