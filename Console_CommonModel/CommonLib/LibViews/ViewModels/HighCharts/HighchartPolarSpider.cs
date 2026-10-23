using System.Collections.Generic;

using Newtonsoft.Json;
using System;

namespace Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts
{
    [Label("Конфигурация Highcharts: Polar Spider Chart Polar")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Spider Chart Polar.")]
    public class HighchartPolarSpiderChartPolar : Highchart
    {
        public HighchartPolarSpiderChartPolar() : base()
        {
        }
    }


    [Label("Конфигурация Highcharts: Polar Spider Chart")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Spider Chart.")]
    public class HighchartPolarSpiderChart : Highchart
    {

        public HighchartPolarSpiderChart() : base()
        {
            Polar = new HighchartPolarSpiderChartPolar();
            Type = "line";

        }


        [JsonProperty("polar")]
        [Label("Polar")]
        public HighchartPolarSpiderChartPolar Polar { get; set; }


        [InputText("type")]
        [JsonProperty("type")]
        [Label("Type")]
        public string Type { get; set; }
    }


    [Label("Конфигурация Highcharts: Polar Spider Accessibility")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Spider Accessibility.")]
    public class HighchartPolarSpiderAccessibility : Highchart
    {

        public HighchartPolarSpiderAccessibility() : base()
        {
            Description = "A spiderweb chart compares the allocated budget against actual spending within an organization. The spider chart has six spokes. Each spoke represents one of the 6 departments within the organization: sales, marketing, development, customer support, information technology and administration. The chart is interactive, and each data point is displayed upon hovering. The chart clearly shows that 4 of the 6 departments have overspent their budget with Marketing responsible for the greatest overspend of $20,000. The allocated budget and actual spending data points for each department are as follows: Sales. Budget equals $43,000; spending equals $50,000. Marketing. Budget equals $19,000; spending equals $39,000. Development. Budget equals $60,000; spending equals $42,000. Customer support. Budget equals $35,000; spending equals $31,000. Information technology. Budget equals $17,000; spending equals $26,000. Administration. Budget equals $10,000; spending equals $14,000.";

        }

        [InputText("description")]
        [JsonProperty("description")]
        [Label("Description")]
        public string Description { get; set; }
    }
    [Label("Конфигурация Highcharts: Polar Spider Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Spider Title.")]
    public class HighchartPolarSpiderTitle : Highchart
    {

        public HighchartPolarSpiderTitle() : base()
        {
            Text = "Budget vs spending";
            X = -80;

        }


        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }


        [InputNumber("x")]
        [JsonProperty("x")]
        [Label("X")]
        public override int X { get; set; }
    }
    [Label("Конфигурация Highcharts: Polar Spider Pane")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Spider Pane.")]
    public class HighchartPolarSpiderPane : Highchart
    {

        public HighchartPolarSpiderPane() : base()
        {
            Size = "80%";

        }

        [InputText("size")]
        [JsonProperty("size")]
        [Label("Size")]
        public string Size { get; set; }
    }
    [Label("Конфигурация Highcharts: Polar Spider X Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Spider X Axis.")]
    public class HighchartPolarSpiderXAxis : Highchart
    {

        public HighchartPolarSpiderXAxis() : base()
        {
            Categories = new List<string> { "Sales", "Marketing", "Development", "Customer Support", "Information Technology", "Administration" };
            TickmarkPlacement = "on";
            LineWidth = null;

        }


        [JsonProperty("categories")]
        [Label("Categories")]
        public List<string> Categories { get; set; }
        [InputText("tickmarkPlacement")]
        [JsonProperty("tickmarkPlacement")]
        [Label("Tickmark Placement")]
        public string TickmarkPlacement { get; set; }

        [JsonProperty("lineWidth")]
        [Label("Line Width")]
        public object LineWidth { get; set; }
    }
    [Label("Конфигурация Highcharts: Polar Spider Y Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Spider Y Axis.")]
    public class HighchartPolarSpiderYAxis : Highchart
    {

        public HighchartPolarSpiderYAxis() : base()
        {
            GridLineInterpolation = "polygon";
            LineWidth = null;
            Min = null;

        }

        [InputText("gridLineInterpolation")]
        [JsonProperty("gridLineInterpolation")]
        [Label("Grid Line Interpolation")]
        public string GridLineInterpolation { get; set; }

        [JsonProperty("lineWidth")]
        [Label("Line Width")]
        public object LineWidth { get; set; }

        [JsonProperty("min")]
        [Label("Min")]
        public object Min { get; set; }
    }
    [Label("Конфигурация Highcharts: Polar Spider Tooltip Shared")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Spider Tooltip Shared.")]
    public class HighchartPolarSpiderTooltipShared : Highchart
    {

        public HighchartPolarSpiderTooltipShared() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: Polar Spider Tooltip")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Spider Tooltip.")]
    public class HighchartPolarSpiderTooltip : Highchart
    {

        public HighchartPolarSpiderTooltip() : base()
        {
            Shared = new HighchartPolarSpiderTooltipShared();
            PointFormat = "<span style='color:{series.color}'>{series.name}: <b>${point.y:,.0f}</b><br/>";

        }


        [JsonProperty("shared")]
        [Label("Shared")]
        public HighchartPolarSpiderTooltipShared Shared { get; set; }
        [InputText("pointFormat")]
        [JsonProperty("pointFormat")]
        [Label("Point Format")]
        public string PointFormat { get; set; }
    }
    [Label("Конфигурация Highcharts: Polar Spider Legend")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Spider Legend.")]
    public class HighchartPolarSpiderLegend : Highchart
    {

        public HighchartPolarSpiderLegend() : base()
        {
            Align = "right";
            VerticalAlign = "middle";
            Layout = "vertical";

        }

        [InputText("align")]
        [JsonProperty("align")]
        [Label("Align")]
        public string Align { get; set; }
        [InputText("verticalAlign")]
        [JsonProperty("verticalAlign")]
        [Label("Vertical Align")]
        public string VerticalAlign { get; set; }
        [InputText("layout")]
        [JsonProperty("layout")]
        [Label("Layout")]
        public string Layout { get; set; }
    }
    [Label("Конфигурация Highcharts: Polar Spider Series Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Spider Series Item.")]
    public class HighchartPolarSpiderSeriesItem : Highchart
    {

        public HighchartPolarSpiderSeriesItem() : base()
        {
            Name = "Allocated Budget";
            Data = new List<float?> { 43000f, 19000f, 60000f, 35000f, 17000f, 10000f };
            PointPlacement = "on";

        }

        [InputText("name")]
        [JsonProperty("name")]
        [Label("Name")]
        public string Name { get; set; }

        [JsonProperty("data")]
        [Label("Data")]
        public List<float?> Data { get; set; }
        [InputText("pointPlacement")]
        [JsonProperty("pointPlacement")]
        [Label("Point Placement")]
        public string PointPlacement { get; set; }
    }
    [Label("Конфигурация Highcharts: Polar Spider Responsive Rules Item Condition")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Spider Responsive Rules Item Condition.")]
    public class HighchartPolarSpiderResponsiveRulesItemCondition : Highchart
    {

        public HighchartPolarSpiderResponsiveRulesItemCondition() : base()
        {
            MaxWidth = 500f;

        }

        [InputNumber("maxWidth")]
        [JsonProperty("maxWidth")]
        [Label("Max Width")]
        public float MaxWidth { get; set; }
    }
    [Label("Конфигурация Highcharts: Polar Spider Responsive Rules Item Chart Options Legend")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Spider Responsive Rules Item Chart Options Legend.")]
    public class HighchartPolarSpiderResponsiveRulesItemChartOptionsLegend : Highchart
    {

        public HighchartPolarSpiderResponsiveRulesItemChartOptionsLegend() : base()
        {
            Align = "center";
            VerticalAlign = "bottom";
            Layout = "horizontal";

        }

        [InputText("align")]
        [JsonProperty("align")]
        [Label("Align")]
        public string Align { get; set; }
        [InputText("verticalAlign")]
        [JsonProperty("verticalAlign")]
        [Label("Vertical Align")]
        public string VerticalAlign { get; set; }
        [InputText("layout")]
        [JsonProperty("layout")]
        [Label("Layout")]
        public string Layout { get; set; }
    }
    [Label("Конфигурация Highcharts: Polar Spider Responsive Rules Item Chart Options Pane")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Spider Responsive Rules Item Chart Options Pane.")]
    public class HighchartPolarSpiderResponsiveRulesItemChartOptionsPane : Highchart
    {

        public HighchartPolarSpiderResponsiveRulesItemChartOptionsPane() : base()
        {
            Size = "70%";

        }

        [InputText("size")]
        [JsonProperty("size")]
        [Label("Size")]
        public string Size { get; set; }
    }
    [Label("Конфигурация Highcharts: Polar Spider Responsive Rules Item Chart Options")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Spider Responsive Rules Item Chart Options.")]
    public class HighchartPolarSpiderResponsiveRulesItemChartOptions : Highchart
    {

        public HighchartPolarSpiderResponsiveRulesItemChartOptions() : base()
        {
            Legend = new HighchartPolarSpiderResponsiveRulesItemChartOptionsLegend();
            Pane = new HighchartPolarSpiderResponsiveRulesItemChartOptionsPane();

        }


        [JsonProperty("legend")]
        [Label("Legend")]
        public HighchartPolarSpiderResponsiveRulesItemChartOptionsLegend Legend { get; set; }

        [JsonProperty("pane")]
        [Label("Pane")]
        public HighchartPolarSpiderResponsiveRulesItemChartOptionsPane Pane { get; set; }
    }
    [Label("Конфигурация Highcharts: Polar Spider Responsive Rules Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Spider Responsive Rules Item.")]
    public class HighchartPolarSpiderResponsiveRulesItem : Highchart
    {

        public HighchartPolarSpiderResponsiveRulesItem() : base()
        {
            Condition = new HighchartPolarSpiderResponsiveRulesItemCondition();
            ChartOptions = new HighchartPolarSpiderResponsiveRulesItemChartOptions();

        }


        [JsonProperty("condition")]
        [Label("Condition")]
        public HighchartPolarSpiderResponsiveRulesItemCondition Condition { get; set; }

        [JsonProperty("chartOptions")]
        [Label("Chart Options")]
        public HighchartPolarSpiderResponsiveRulesItemChartOptions ChartOptions { get; set; }
    }
    [Label("Конфигурация Highcharts: Polar Spider Responsive")]
    [Description("Часть конфигурации диаграммы Highcharts: Polar Spider Responsive.")]
    public class HighchartPolarSpiderResponsive : Highchart
    {

        public HighchartPolarSpiderResponsive() : base()
        {
            Rules = new List<HighchartPolarSpiderResponsiveRulesItem> { new HighchartPolarSpiderResponsiveRulesItem() };

        }


        [JsonProperty("rules")]
        [Label("Rules")]
        public List<HighchartPolarSpiderResponsiveRulesItem> Rules { get; set; }
    }
    [Label("Диаграмма Highcharts: Polar Spider")]
    [Description("Модель диаграммы Highcharts: Polar Spider.")]
    public class HighchartPolarSpider : Highchart
    {

        public HighchartPolarSpider() : base()
        {
            Chart = new HighchartPolarSpiderChart();
            Accessibility = new HighchartPolarSpiderAccessibility();
            Title = new HighchartPolarSpiderTitle();
            Pane = new HighchartPolarSpiderPane();
            XAxis = new HighchartPolarSpiderXAxis();
            YAxis = new HighchartPolarSpiderYAxis();
            Tooltip = new HighchartPolarSpiderTooltip();
            Legend = new HighchartPolarSpiderLegend();
            Series = new List<HighchartPolarSpiderSeriesItem> { new HighchartPolarSpiderSeriesItem(), new HighchartPolarSpiderSeriesItem() };
            Responsive = new HighchartPolarSpiderResponsive();

        }


        [JsonProperty("chart")]
        public HighchartPolarSpiderChart Chart { get; set; }

        [JsonProperty("accessibility")]
        public HighchartPolarSpiderAccessibility Accessibility { get; set; }

        [JsonProperty("title")]
        public HighchartPolarSpiderTitle Title { get; set; }

        [JsonProperty("pane")]
        public HighchartPolarSpiderPane Pane { get; set; }

        [JsonProperty("xAxis")]
        public HighchartPolarSpiderXAxis XAxis { get; set; }

        [JsonProperty("yAxis")]
        public HighchartPolarSpiderYAxis YAxis { get; set; }

        [JsonProperty("tooltip")]
        public HighchartPolarSpiderTooltip Tooltip { get; set; }

        [JsonProperty("legend")]
        public HighchartPolarSpiderLegend Legend { get; set; }

        [JsonProperty("series")]
        public List<HighchartPolarSpiderSeriesItem> Series { get; set; }

        [JsonProperty("responsive")]
        public HighchartPolarSpiderResponsive Responsive { get; set; }
    }


}