using System.Collections.Generic;

using Newtonsoft.Json;
using System;

namespace Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts
{
    [Label("Конфигурация Highcharts: Line Basic Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Line Basic Title.")]
    public class HighchartLineBasicTitle : Highchart
    {

        public HighchartLineBasicTitle() : base()
        {
            Text = "Solar Employment Growth by Sector, 2010-2016";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Line Basic Subtitle")]
    [Description("Часть конфигурации диаграммы Highcharts: Line Basic Subtitle.")]
    public class HighchartLineBasicSubtitle : Highchart
    {

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }

        public HighchartLineBasicSubtitle() : base()
        {
            Text = "Source: thesolarfoundation.com";

        }
    }
    [Label("Конфигурация Highcharts: Line Basic Y Axis Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Line Basic Y Axis Title.")]
    public class HighchartLineBasicYAxisTitle : Highchart
    {

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }

        public HighchartLineBasicYAxisTitle() : base()
        {
            Text = "Number of Employees";

        }
    }
    [Label("Конфигурация Highcharts: Line Basic Y Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: Line Basic Y Axis.")]
    public class HighchartLineBasicYAxis : Highchart
    {


        [JsonProperty("title")]
        [Label("Title")]
        public HighchartLineBasicYAxisTitle Title { get; set; }


        public HighchartLineBasicYAxis() : base()
        {
            Title = new HighchartLineBasicYAxisTitle();

        }


    }
    [Label("Конфигурация Highcharts: Line Basic X Axis Accessibility")]
    [Description("Часть конфигурации диаграммы Highcharts: Line Basic X Axis Accessibility.")]
    public class HighchartLineBasicXAxisAccessibility : Highchart
    {

        public HighchartLineBasicXAxisAccessibility() : base()
        {
            RangeDescription = "Range: 2010 to 2017";

        }

        [InputText("rangeDescription")]
        [JsonProperty("rangeDescription")]
        [Label("Range Description")]
        public string RangeDescription { get; set; }
    }
    [Label("Конфигурация Highcharts: Line Basic X Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: Line Basic X Axis.")]
    public class HighchartLineBasicXAxis : Highchart
    {

        public HighchartLineBasicXAxis() : base()
        {
            Accessibility = new HighchartLineBasicXAxisAccessibility();

        }


        [JsonProperty("accessibility")]
        [Label("Accessibility")]
        public HighchartLineBasicXAxisAccessibility Accessibility { get; set; }
    }
    [Label("Конфигурация Highcharts: Line Basic Legend")]
    [Description("Часть конфигурации диаграммы Highcharts: Line Basic Legend.")]
    public class HighchartLineBasicLegend : Highchart
    {

        public HighchartLineBasicLegend() : base()
        {
            Layout = "vertical";
            Align = "right";
            VerticalAlign = "middle";

        }

        [InputText("layout")]
        [JsonProperty("layout")]
        [Label("Layout")]
        public string Layout { get; set; }
        [InputText("align")]
        [JsonProperty("align")]
        [Label("Align")]
        public string Align { get; set; }
        [InputText("verticalAlign")]
        [JsonProperty("verticalAlign")]
        [Label("Vertical Align")]
        public string VerticalAlign { get; set; }
    }
    [Label("Конфигурация Highcharts: Line Basic Plot Options Series Label")]
    [Description("Часть конфигурации диаграммы Highcharts: Line Basic Plot Options Series Label.")]
    public class HighchartLineBasicPlotOptionsSeriesLabel : Highchart
    {

        public HighchartLineBasicPlotOptionsSeriesLabel() : base()
        {
            ConnectorAllowed = null;

        }


        [JsonProperty("connectorAllowed")]
        [Label("Connector Allowed")]
        public object ConnectorAllowed { get; set; }
    }
    [Label("Конфигурация Highcharts: Line Basic Plot Options Series")]
    [Description("Часть конфигурации диаграммы Highcharts: Line Basic Plot Options Series.")]
    public class HighchartLineBasicPlotOptionsSeries : Highchart
    {

        public HighchartLineBasicPlotOptionsSeries() : base()
        {
            Label = new HighchartLineBasicPlotOptionsSeriesLabel();
            PointStart = 2010f;

        }


        [JsonProperty("label")]
        [Label("Label")]
        public HighchartLineBasicPlotOptionsSeriesLabel Label { get; set; }
        [InputNumber("pointStart")]
        [JsonProperty("pointStart")]
        [Label("Point Start")]
        public float PointStart { get; set; }
    }
    [Label("Конфигурация Highcharts: Line Basic Plot Options")]
    [Description("Часть конфигурации диаграммы Highcharts: Line Basic Plot Options.")]
    public class HighchartLineBasicPlotOptions : Highchart
    {

        public HighchartLineBasicPlotOptions() : base()
        {
            Series = new HighchartLineBasicPlotOptionsSeries();

        }


        [JsonProperty("series")]
        [Label("Series")]
        public HighchartLineBasicPlotOptionsSeries Series { get; set; }
    }
    [Label("Конфигурация Highcharts: Line Basic Series Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Line Basic Series Item.")]
    public class HighchartLineBasicSeriesItem : Highchart
    {

        public HighchartLineBasicSeriesItem() : base()
        {
            Name = "Installation";
            Data = new List<float?> { 43934f, 52503f, 57177f, 69658f, 97031f, 119931f, 137133f, 154175f };

        }

        [InputText("name")]
        [JsonProperty("name")]
        [Label("Name")]
        public string Name { get; set; }

        [JsonProperty("data")]
        [Label("Data")]
        public List<float?> Data { get; set; }
    }
    [Label("Конфигурация Highcharts: Line Basic Responsive Rules Item Condition")]
    [Description("Часть конфигурации диаграммы Highcharts: Line Basic Responsive Rules Item Condition.")]
    public class HighchartLineBasicResponsiveRulesItemCondition : Highchart
    {

        public HighchartLineBasicResponsiveRulesItemCondition() : base()
        {
            MaxWidth = 500f;

        }

        [InputNumber("maxWidth")]
        [JsonProperty("maxWidth")]
        [Label("Max Width")]
        public float MaxWidth { get; set; }
    }
    [Label("Конфигурация Highcharts: Line Basic Responsive Rules Item Chart Options Legend")]
    [Description("Часть конфигурации диаграммы Highcharts: Line Basic Responsive Rules Item Chart Options Legend.")]
    public class HighchartLineBasicResponsiveRulesItemChartOptionsLegend : Highchart
    {

        public HighchartLineBasicResponsiveRulesItemChartOptionsLegend() : base()
        {
            Layout = "horizontal";
            Align = "center";
            VerticalAlign = "bottom";

        }

        [InputText("layout")]
        [JsonProperty("layout")]
        [Label("Layout")]
        public string Layout { get; set; }
        [InputText("align")]
        [JsonProperty("align")]
        [Label("Align")]
        public string Align { get; set; }
        [InputText("verticalAlign")]
        [JsonProperty("verticalAlign")]
        [Label("Vertical Align")]
        public string VerticalAlign { get; set; }
    }
    [Label("Конфигурация Highcharts: Line Basic Responsive Rules Item Chart Options")]
    [Description("Часть конфигурации диаграммы Highcharts: Line Basic Responsive Rules Item Chart Options.")]
    public class HighchartLineBasicResponsiveRulesItemChartOptions : Highchart
    {

        public HighchartLineBasicResponsiveRulesItemChartOptions() : base()
        {
            Legend = new HighchartLineBasicResponsiveRulesItemChartOptionsLegend();

        }


        [JsonProperty("legend")]
        [Label("Legend")]
        public HighchartLineBasicResponsiveRulesItemChartOptionsLegend Legend { get; set; }
    }
    [Label("Конфигурация Highcharts: Line Basic Responsive Rules Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Line Basic Responsive Rules Item.")]
    public class HighchartLineBasicResponsiveRulesItem : Highchart
    {

        public HighchartLineBasicResponsiveRulesItem() : base()
        {
            Condition = new HighchartLineBasicResponsiveRulesItemCondition();
            ChartOptions = new HighchartLineBasicResponsiveRulesItemChartOptions();

        }


        [JsonProperty("condition")]
        [Label("Condition")]
        public HighchartLineBasicResponsiveRulesItemCondition Condition { get; set; }

        [JsonProperty("chartOptions")]
        [Label("Chart Options")]
        public HighchartLineBasicResponsiveRulesItemChartOptions ChartOptions { get; set; }
    }
    [Label("Конфигурация Highcharts: Line Basic Responsive")]
    [Description("Часть конфигурации диаграммы Highcharts: Line Basic Responsive.")]
    public class HighchartLineBasicResponsive : Highchart
    {

        public HighchartLineBasicResponsive() : base()
        {
            Rules = new List<HighchartLineBasicResponsiveRulesItem> { new HighchartLineBasicResponsiveRulesItem() };

        }


        [JsonProperty("rules")]
        [Label("Rules")]
        public List<HighchartLineBasicResponsiveRulesItem> Rules { get; set; }
    }
    [Label("Диаграмма Highcharts: Line Basic")]
    [Description("Модель диаграммы Highcharts: Line Basic.")]
    public class HighchartLineBasic : Highchart
    {

        public HighchartLineBasic() : base()
        {
            Title = new HighchartLineBasicTitle();
            Subtitle = new HighchartLineBasicSubtitle();
            YAxis = new HighchartLineBasicYAxis();
            XAxis = new HighchartLineBasicXAxis();
            Legend = new HighchartLineBasicLegend();
            PlotOptions = new HighchartLineBasicPlotOptions();
            Series = new List<HighchartLineBasicSeriesItem> { new HighchartLineBasicSeriesItem(), new HighchartLineBasicSeriesItem(), new HighchartLineBasicSeriesItem(), new HighchartLineBasicSeriesItem(), new HighchartLineBasicSeriesItem() };
            Responsive = new HighchartLineBasicResponsive();

        }


        [JsonProperty("title")]
        public HighchartLineBasicTitle Title { get; set; }

        [JsonProperty("subtitle")]
        public HighchartLineBasicSubtitle Subtitle { get; set; }

        [JsonProperty("yAxis")]
        public HighchartLineBasicYAxis YAxis { get; set; }

        [JsonProperty("xAxis")]
        public HighchartLineBasicXAxis XAxis { get; set; }

        [JsonProperty("legend")]
        public HighchartLineBasicLegend Legend { get; set; }

        [JsonProperty("plotOptions")]
        public HighchartLineBasicPlotOptions PlotOptions { get; set; }

        [JsonProperty("series")]
        public List<HighchartLineBasicSeriesItem> Series { get; set; }

        [JsonProperty("responsive")]
        public HighchartLineBasicResponsive Responsive { get; set; }
    }


}