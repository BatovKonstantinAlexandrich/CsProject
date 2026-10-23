using System.Collections.Generic;

using Newtonsoft.Json;
using System;

namespace Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts
{
    [Label("Конфигурация Highcharts: Pie Donut Chart")]
    [Description("Часть конфигурации диаграммы Highcharts: Pie Donut Chart.")]
    public class HighchartPieDonutChart : Highchart
    {

        public HighchartPieDonutChart() : base()
        {
            Type = "pie";

        }

        [InputText("type")]
        [JsonProperty("type")]
        [Label("Type")]
        public string Type { get; set; }
    }
    [Label("Конфигурация Highcharts: Pie Donut Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Pie Donut Title.")]
    public class HighchartPieDonutTitle : Highchart
    {

        public HighchartPieDonutTitle() : base()
        {
            Text = "Browser market share, January, 2018";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Pie Donut Subtitle")]
    [Description("Часть конфигурации диаграммы Highcharts: Pie Donut Subtitle.")]
    public class HighchartPieDonutSubtitle : Highchart
    {

        public HighchartPieDonutSubtitle() : base()
        {
            Text = "Source: <a href='http://statcounter.com' target='_blank'>statcounter.com</a>";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Pie Donut Plot Options Pie")]
    [Description("Часть конфигурации диаграммы Highcharts: Pie Donut Plot Options Pie.")]
    public class HighchartPieDonutPlotOptionsPie : Highchart
    {

        public HighchartPieDonutPlotOptionsPie() : base()
        {
            Shadow = null;
            Center = new List<string> { "50%", "50%" };

        }


        [JsonProperty("shadow")]
        [Label("Shadow")]
        public object Shadow { get; set; }

        [JsonProperty("center")]
        [Label("Center")]
        public List<string> Center { get; set; }
    }
    [Label("Конфигурация Highcharts: Pie Donut Plot Options")]
    [Description("Часть конфигурации диаграммы Highcharts: Pie Donut Plot Options.")]
    public class HighchartPieDonutPlotOptions : Highchart
    {

        public HighchartPieDonutPlotOptions() : base()
        {
            Pie = new HighchartPieDonutPlotOptionsPie();

        }


        [JsonProperty("pie")]
        [Label("Pie")]
        public HighchartPieDonutPlotOptionsPie Pie { get; set; }
    }
    [Label("Конфигурация Highcharts: Pie Donut Tooltip")]
    [Description("Часть конфигурации диаграммы Highcharts: Pie Donut Tooltip.")]
    public class HighchartPieDonutTooltip : Highchart
    {

        public HighchartPieDonutTooltip() : base()
        {
            ValueSuffix = "%";

        }

        [InputText("valueSuffix")]
        [JsonProperty("valueSuffix")]
        [Label("Value Suffix")]
        public string ValueSuffix { get; set; }
    }
    [Label("Конфигурация Highcharts: Pie Donut Series Item Data Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Pie Donut Series Item Data Item.")]
    public class HighchartPieDonutSeriesItemDataItem : Highchart
    {

        public HighchartPieDonutSeriesItemDataItem() : base()
        {
            Name = "Chrome";
            Y = 62.74f;
            Color = "#90ed7d";

        }

        [InputText("name")]
        [JsonProperty("name")]
        [Label("Name")]
        public string Name { get; set; }
        [InputNumber("y")]
        [JsonProperty("y")]
        [Label("Y")]
        public float Y { get; set; }
        [InputText("color")]
        [JsonProperty("color")]
        [Label("Color")]
        public string Color { get; set; }
    }
    [Label("Конфигурация Highcharts: Pie Donut Series Item Data Labels")]
    [Description("Часть конфигурации диаграммы Highcharts: Pie Donut Series Item Data Labels.")]
    public class HighchartPieDonutSeriesItemDataLabels : Highchart
    {

        public HighchartPieDonutSeriesItemDataLabels() : base()
        {
            Formatter = () => { };
            Color = "#ffffff";
            Distance = -30f;

        }


        [JsonProperty("formatter")]
        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [Label("Formatter")]
        public Action Formatter { get; set; }


        [InputText("color")]
        [JsonProperty("color")]
        [Label("Color")]
        public string Color { get; set; }


        [InputNumber("distance")]
        [JsonProperty("distance")]
        [Label("Distance")]
        public float Distance { get; set; }
    }
    [Label("Конфигурация Highcharts: Pie Donut Series Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Pie Donut Series Item.")]
    public class HighchartPieDonutSeriesItem : Highchart
    {

        public HighchartPieDonutSeriesItem() : base()
        {
            Name = "Browsers";
            Data = new List<HighchartPieDonutSeriesItemDataItem> { new HighchartPieDonutSeriesItemDataItem(), new HighchartPieDonutSeriesItemDataItem(), new HighchartPieDonutSeriesItemDataItem(), new HighchartPieDonutSeriesItemDataItem(), new HighchartPieDonutSeriesItemDataItem(), new HighchartPieDonutSeriesItemDataItem(), new HighchartPieDonutSeriesItemDataItem() };
            Size = "60%";
            DataLabels = new HighchartPieDonutSeriesItemDataLabels();

        }

        [InputText("name")]
        [JsonProperty("name")]
        [Label("Name")]
        public string Name { get; set; }

        [JsonProperty("data")]
        [Label("Data")]
        public List<HighchartPieDonutSeriesItemDataItem> Data { get; set; }
        [InputText("size")]
        [JsonProperty("size")]
        [Label("Size")]
        public string Size { get; set; }

        [JsonProperty("dataLabels")]
        [Label("Data Labels")]
        public HighchartPieDonutSeriesItemDataLabels DataLabels { get; set; }
    }
    [Label("Конфигурация Highcharts: Pie Donut Responsive Rules Item Condition")]
    [Description("Часть конфигурации диаграммы Highcharts: Pie Donut Responsive Rules Item Condition.")]
    public class HighchartPieDonutResponsiveRulesItemCondition : Highchart
    {

        public HighchartPieDonutResponsiveRulesItemCondition() : base()
        {
            MaxWidth = 400f;

        }

        [InputNumber("maxWidth")]
        [JsonProperty("maxWidth")]
        [Label("Max Width")]
        public float MaxWidth { get; set; }
    }
    [Label("Конфигурация Highcharts: Pie Donut Responsive Rules Item Chart Options Series Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Pie Donut Responsive Rules Item Chart Options Series Item.")]
    public class HighchartPieDonutResponsiveRulesItemChartOptionsSeriesItem : Highchart
    {

        public HighchartPieDonutResponsiveRulesItemChartOptionsSeriesItem() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: Pie Donut Responsive Rules Item Chart Options")]
    [Description("Часть конфигурации диаграммы Highcharts: Pie Donut Responsive Rules Item Chart Options.")]
    public class HighchartPieDonutResponsiveRulesItemChartOptions : Highchart
    {

        public HighchartPieDonutResponsiveRulesItemChartOptions() : base()
        {
            Series = new List<HighchartPieDonutResponsiveRulesItemChartOptionsSeriesItem> { new HighchartPieDonutResponsiveRulesItemChartOptionsSeriesItem(), new HighchartPieDonutResponsiveRulesItemChartOptionsSeriesItem() };

        }


        [JsonProperty("series")]
        [Label("Series")]
        public List<HighchartPieDonutResponsiveRulesItemChartOptionsSeriesItem> Series { get; set; }
    }
    [Label("Конфигурация Highcharts: Pie Donut Responsive Rules Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Pie Donut Responsive Rules Item.")]
    public class HighchartPieDonutResponsiveRulesItem : Highchart
    {

        public HighchartPieDonutResponsiveRulesItem() : base()
        {
            Condition = new HighchartPieDonutResponsiveRulesItemCondition();
            ChartOptions = new HighchartPieDonutResponsiveRulesItemChartOptions();

        }


        [JsonProperty("condition")]
        [Label("Condition")]
        public HighchartPieDonutResponsiveRulesItemCondition Condition { get; set; }

        [JsonProperty("chartOptions")]
        [Label("Chart Options")]
        public HighchartPieDonutResponsiveRulesItemChartOptions ChartOptions { get; set; }
    }
    [Label("Конфигурация Highcharts: Pie Donut Responsive")]
    [Description("Часть конфигурации диаграммы Highcharts: Pie Donut Responsive.")]
    public class HighchartPieDonutResponsive : Highchart
    {

        public HighchartPieDonutResponsive() : base()
        {
            Rules = new List<HighchartPieDonutResponsiveRulesItem> { new HighchartPieDonutResponsiveRulesItem() };

        }


        [JsonProperty("rules")]
        [Label("Rules")]
        public List<HighchartPieDonutResponsiveRulesItem> Rules { get; set; }
    }
    [Label("Диаграмма Highcharts: Pie Donut")]
    [Description("Модель диаграммы Highcharts: Pie Donut.")]
    public class HighchartPieDonut : Highchart
    {

        public HighchartPieDonut() : base()
        {
            Chart = new HighchartPieDonutChart();
            Title = new HighchartPieDonutTitle();
            Subtitle = new HighchartPieDonutSubtitle();
            PlotOptions = new HighchartPieDonutPlotOptions();
            Tooltip = new HighchartPieDonutTooltip();
            Series = new List<HighchartPieDonutSeriesItem> { new HighchartPieDonutSeriesItem(), new HighchartPieDonutSeriesItem() };
            Responsive = new HighchartPieDonutResponsive();

        }


        [JsonProperty("chart")]
        public HighchartPieDonutChart Chart { get; set; }

        [JsonProperty("title")]
        public HighchartPieDonutTitle Title { get; set; }

        [JsonProperty("subtitle")]
        public HighchartPieDonutSubtitle Subtitle { get; set; }

        [JsonProperty("plotOptions")]
        public HighchartPieDonutPlotOptions PlotOptions { get; set; }

        [JsonProperty("tooltip")]
        public HighchartPieDonutTooltip Tooltip { get; set; }

        [JsonProperty("series")]
        public List<HighchartPieDonutSeriesItem> Series { get; set; }

        [JsonProperty("responsive")]
        public HighchartPieDonutResponsive Responsive { get; set; }
    }


}