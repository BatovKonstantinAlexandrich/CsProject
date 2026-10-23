using System.Collections.Generic;

using Newtonsoft.Json;
using System;

namespace Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts
{
    [Label("Конфигурация Highcharts: Accessible Line Chart")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Line Chart.")]
    public class HighchartAccessibleLineChart : Highchart
    {

        public HighchartAccessibleLineChart() : base()
        {
            Type = "spline";

        }

        [InputText("type")]
        [JsonProperty("type")]
        [Label("Type")]
        public string Type { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Line Legend")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Line Legend.")]
    public class HighchartAccessibleLineLegend : Highchart
    {

        public HighchartAccessibleLineLegend() : base()
        {
            SymbolWidth = 40f;

        }

        [InputNumber("symbolWidth")]
        [JsonProperty("symbolWidth")]
        [Label("Symbol Width")]
        public float SymbolWidth { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Line Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Line Title.")]
    public class HighchartAccessibleLineTitle : Highchart
    {

        public HighchartAccessibleLineTitle() : base()
        {
            Text = "Most common desktop screen readers";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Line Subtitle")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Line Subtitle.")]
    public class HighchartAccessibleLineSubtitle : Highchart
    {

        public HighchartAccessibleLineSubtitle() : base()
        {
            Text = "Source: WebAIM. Click on points to visit official screen reader website";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Line Y Axis Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Line Y Axis Title.")]
    public class HighchartAccessibleLineYAxisTitle : Highchart
    {

        public HighchartAccessibleLineYAxisTitle() : base()
        {
            Text = "Percentage usage";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Line Y Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Line Y Axis.")]
    public class HighchartAccessibleLineYAxis : Highchart
    {

        public HighchartAccessibleLineYAxis() : base()
        {
            Title = new HighchartAccessibleLineYAxisTitle();

        }


        [JsonProperty("title")]
        [Label("Title")]
        public HighchartAccessibleLineYAxisTitle Title { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Line X Axis Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Line X Axis Title.")]
    public class HighchartAccessibleLineXAxisTitle : Highchart
    {

        public HighchartAccessibleLineXAxisTitle() : base()
        {
            Text = "Time";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Line X Axis Accessibility")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Line X Axis Accessibility.")]
    public class HighchartAccessibleLineXAxisAccessibility : Highchart
    {

        public HighchartAccessibleLineXAxisAccessibility() : base()
        {
            Description = "Time from December 2010 to September 2019";

        }

        [InputText("description")]
        [JsonProperty("description")]
        [Label("Description")]
        public string Description { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Line X Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Line X Axis.")]
    public class HighchartAccessibleLineXAxis : Highchart
    {

        public HighchartAccessibleLineXAxis() : base()
        {
            Title = new HighchartAccessibleLineXAxisTitle();
            Accessibility = new HighchartAccessibleLineXAxisAccessibility();
            Categories = new List<string> { "December 2010", "May 2012", "January 2014", "July 2015", "October 2017", "September 2019" };

        }


        [JsonProperty("title")]
        [Label("Title")]
        public HighchartAccessibleLineXAxisTitle Title { get; set; }

        [JsonProperty("accessibility")]
        [Label("Accessibility")]
        public HighchartAccessibleLineXAxisAccessibility Accessibility { get; set; }

        [JsonProperty("categories")]
        [Label("Categories")]
        public List<string> Categories { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Line Tooltip")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Line Tooltip.")]
    public class HighchartAccessibleLineTooltip : Highchart
    {

        public HighchartAccessibleLineTooltip() : base()
        {
            ValueSuffix = "%";

        }

        [InputText("valueSuffix")]
        [JsonProperty("valueSuffix")]
        [Label("Value Suffix")]
        public string ValueSuffix { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Line Plot Options Series Point Events")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Line Plot Options Series Point Events.")]
    public class HighchartAccessibleLinePlotOptionsSeriesPointEvents : Highchart
    {

        public HighchartAccessibleLinePlotOptionsSeriesPointEvents() : base()
        {
            Click = () => { };

        }


        [JsonProperty("click")]
        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [Label("Click")]
        public Action Click { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Line Plot Options Series Point")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Line Plot Options Series Point.")]
    public class HighchartAccessibleLinePlotOptionsSeriesPoint : Highchart
    {

        public HighchartAccessibleLinePlotOptionsSeriesPoint() : base()
        {
            Events = new HighchartAccessibleLinePlotOptionsSeriesPointEvents();

        }


        [JsonProperty("events")]
        [Label("Events")]
        public HighchartAccessibleLinePlotOptionsSeriesPointEvents Events { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Line Plot Options Series")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Line Plot Options Series.")]
    public class HighchartAccessibleLinePlotOptionsSeries : Highchart
    {

        public HighchartAccessibleLinePlotOptionsSeries() : base()
        {
            Point = new HighchartAccessibleLinePlotOptionsSeriesPoint();
            Cursor = "pointer";

        }


        [JsonProperty("point")]
        [Label("Point")]
        public HighchartAccessibleLinePlotOptionsSeriesPoint Point { get; set; }
        [InputText("cursor")]
        [JsonProperty("cursor")]
        [Label("Cursor")]
        public string Cursor { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Line Plot Options")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Line Plot Options.")]
    public class HighchartAccessibleLinePlotOptions : Highchart
    {

        public HighchartAccessibleLinePlotOptions() : base()
        {
            Series = new HighchartAccessibleLinePlotOptionsSeries();

        }


        [JsonProperty("series")]
        [Label("Series")]
        public HighchartAccessibleLinePlotOptionsSeries Series { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Line Series Item Accessibility")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Line Series Item Accessibility.")]
    public class HighchartAccessibleLineSeriesItemAccessibility : Highchart
    {

        public HighchartAccessibleLineSeriesItemAccessibility() : base()
        {
            Description = "This is the most used screen reader in 2019";

        }

        [InputText("description")]
        [JsonProperty("description")]
        [Label("Description")]
        public string Description { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Line Series Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Line Series Item.")]
    public class HighchartAccessibleLineSeriesItem : Highchart
    {

        public HighchartAccessibleLineSeriesItem() : base()
        {
            Name = "NVDA";
            Data = new List<float?> { 34.8f, 43f, 51.2f, 41.4f, 64.9f, 72.4f };
            Website = "";
            Color = "#49a65e";
            Accessibility = new HighchartAccessibleLineSeriesItemAccessibility();

        }

        [InputText("name")]
        [JsonProperty("name")]
        [Label("Name")]
        public string Name { get; set; }

        [JsonProperty("data")]
        [Label("Data")]
        public List<float?> Data { get; set; }
        [InputText("website")]
        [JsonProperty("website")]
        [Label("Website")]
        public string Website { get; set; }
        [InputText("color")]
        [JsonProperty("color")]
        [Label("Color")]
        public string Color { get; set; }

        [JsonProperty("accessibility")]
        [Label("Accessibility")]
        public HighchartAccessibleLineSeriesItemAccessibility Accessibility { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Line Responsive Rules Item Condition")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Line Responsive Rules Item Condition.")]
    public class HighchartAccessibleLineResponsiveRulesItemCondition : Highchart
    {

        public HighchartAccessibleLineResponsiveRulesItemCondition() : base()
        {
            MaxWidth = 550f;

        }

        [InputNumber("maxWidth")]
        [JsonProperty("maxWidth")]
        [Label("Max Width")]
        public float MaxWidth { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Line Responsive Rules Item Chart Options Legend")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Line Responsive Rules Item Chart Options Legend.")]
    public class HighchartAccessibleLineResponsiveRulesItemChartOptionsLegend : Highchart
    {

        public HighchartAccessibleLineResponsiveRulesItemChartOptionsLegend() : base()
        {
            ItemWidth = 150f;

        }

        [InputNumber("itemWidth")]
        [JsonProperty("itemWidth")]
        [Label("Item Width")]
        public float ItemWidth { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Line Responsive Rules Item Chart Options X Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Line Responsive Rules Item Chart Options X Axis.")]
    public class HighchartAccessibleLineResponsiveRulesItemChartOptionsXAxis : Highchart
    {

        public HighchartAccessibleLineResponsiveRulesItemChartOptionsXAxis() : base()
        {
            Categories = new List<string> { "Dec. 2010", "May 2012", "Jan. 2014", "July 2015", "Oct. 2017", "Sep. 2019" };

        }


        [JsonProperty("categories")]
        [Label("Categories")]
        public List<string> Categories { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Line Responsive Rules Item Chart Options Y Axis Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Line Responsive Rules Item Chart Options Y Axis Title.")]
    public class HighchartAccessibleLineResponsiveRulesItemChartOptionsYAxisTitle : Highchart
    {

        public HighchartAccessibleLineResponsiveRulesItemChartOptionsYAxisTitle() : base()
        {
            Enabled = null;

        }


        [JsonProperty("enabled")]
        [Label("Enabled")]
        public object Enabled { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Line Responsive Rules Item Chart Options Y Axis Labels")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Line Responsive Rules Item Chart Options Y Axis Labels.")]
    public class HighchartAccessibleLineResponsiveRulesItemChartOptionsYAxisLabels : Highchart
    {

        public HighchartAccessibleLineResponsiveRulesItemChartOptionsYAxisLabels() : base()
        {
            Format = "{value}%";

        }

        [InputText("format")]
        [JsonProperty("format")]
        [Label("Format")]
        public string Format { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Line Responsive Rules Item Chart Options Y Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Line Responsive Rules Item Chart Options Y Axis.")]
    public class HighchartAccessibleLineResponsiveRulesItemChartOptionsYAxis : Highchart
    {

        public HighchartAccessibleLineResponsiveRulesItemChartOptionsYAxis() : base()
        {
            Title = new HighchartAccessibleLineResponsiveRulesItemChartOptionsYAxisTitle();
            Labels = new HighchartAccessibleLineResponsiveRulesItemChartOptionsYAxisLabels();

        }


        [JsonProperty("title")]
        [Label("Title")]
        public HighchartAccessibleLineResponsiveRulesItemChartOptionsYAxisTitle Title { get; set; }

        [JsonProperty("labels")]
        [Label("Labels")]
        public HighchartAccessibleLineResponsiveRulesItemChartOptionsYAxisLabels Labels { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Line Responsive Rules Item Chart Options")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Line Responsive Rules Item Chart Options.")]
    public class HighchartAccessibleLineResponsiveRulesItemChartOptions : Highchart
    {

        public HighchartAccessibleLineResponsiveRulesItemChartOptions() : base()
        {
            Legend = new HighchartAccessibleLineResponsiveRulesItemChartOptionsLegend();
            XAxis = new HighchartAccessibleLineResponsiveRulesItemChartOptionsXAxis();
            YAxis = new HighchartAccessibleLineResponsiveRulesItemChartOptionsYAxis();

        }


        [JsonProperty("legend")]
        [Label("Legend")]
        public HighchartAccessibleLineResponsiveRulesItemChartOptionsLegend Legend { get; set; }

        [JsonProperty("xAxis")]
        [Label("X Axis")]
        public HighchartAccessibleLineResponsiveRulesItemChartOptionsXAxis XAxis { get; set; }

        [JsonProperty("yAxis")]
        [Label("Y Axis")]
        public HighchartAccessibleLineResponsiveRulesItemChartOptionsYAxis YAxis { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Line Responsive Rules Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Line Responsive Rules Item.")]
    public class HighchartAccessibleLineResponsiveRulesItem : Highchart
    {

        public HighchartAccessibleLineResponsiveRulesItem() : base()
        {
            Condition = new HighchartAccessibleLineResponsiveRulesItemCondition();
            ChartOptions = new HighchartAccessibleLineResponsiveRulesItemChartOptions();

        }


        [JsonProperty("condition")]
        [Label("Condition")]
        public HighchartAccessibleLineResponsiveRulesItemCondition Condition { get; set; }

        [JsonProperty("chartOptions")]
        [Label("Chart Options")]
        public HighchartAccessibleLineResponsiveRulesItemChartOptions ChartOptions { get; set; }
    }
    [Label("Конфигурация Highcharts: Accessible Line Responsive")]
    [Description("Часть конфигурации диаграммы Highcharts: Accessible Line Responsive.")]
    public class HighchartAccessibleLineResponsive : Highchart
    {

        public HighchartAccessibleLineResponsive() : base()
        {
            Rules = new List<HighchartAccessibleLineResponsiveRulesItem> { new HighchartAccessibleLineResponsiveRulesItem() };

        }


        [JsonProperty("rules")]
        [Label("Rules")]
        public List<HighchartAccessibleLineResponsiveRulesItem> Rules { get; set; }
    }
    [Label("Диаграмма Highcharts: Accessible Line")]
    [Description("Модель диаграммы Highcharts: Accessible Line.")]
    public class HighchartAccessibleLine : Highchart
    {

        public HighchartAccessibleLine() : base()
        {
            Chart = new HighchartAccessibleLineChart();
            Legend = new HighchartAccessibleLineLegend();
            Title = new HighchartAccessibleLineTitle();
            Subtitle = new HighchartAccessibleLineSubtitle();
            YAxis = new HighchartAccessibleLineYAxis();
            XAxis = new HighchartAccessibleLineXAxis();
            Tooltip = new HighchartAccessibleLineTooltip();
            PlotOptions = new HighchartAccessibleLinePlotOptions();
            Series = new List<HighchartAccessibleLineSeriesItem> { new HighchartAccessibleLineSeriesItem(), new HighchartAccessibleLineSeriesItem(), new HighchartAccessibleLineSeriesItem(), new HighchartAccessibleLineSeriesItem(), new HighchartAccessibleLineSeriesItem(), new HighchartAccessibleLineSeriesItem() };
            Responsive = new HighchartAccessibleLineResponsive();

        }


        [JsonProperty("chart")]
        public HighchartAccessibleLineChart Chart { get; set; }


        [JsonProperty("legend")]
        public HighchartAccessibleLineLegend Legend { get; set; }


        [JsonProperty("title")]
        public HighchartAccessibleLineTitle Title { get; set; }


        [JsonProperty("subtitle")]
        public HighchartAccessibleLineSubtitle Subtitle { get; set; }



        [JsonProperty("yAxis")]
        public HighchartAccessibleLineYAxis YAxis { get; set; }

        [JsonProperty("xAxis")]
        public HighchartAccessibleLineXAxis XAxis { get; set; }

        [JsonProperty("tooltip")]
        public HighchartAccessibleLineTooltip Tooltip { get; set; }

        [JsonProperty("plotOptions")]
        public HighchartAccessibleLinePlotOptions PlotOptions { get; set; }

        [JsonProperty("series")]
        public List<HighchartAccessibleLineSeriesItem> Series { get; set; }

        [JsonProperty("responsive")]
        public HighchartAccessibleLineResponsive Responsive { get; set; }
    }


}