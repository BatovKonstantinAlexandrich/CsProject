using System.Collections.Generic;

using Newtonsoft.Json;
using System;

namespace Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts
{
    [Label("Конфигурация Highcharts: Bar Negative Stack Chart")]
    [Description("Часть конфигурации диаграммы Highcharts: Bar Negative Stack Chart.")]
    public class HighchartBarNegativeStackChart : Highchart
    {

        public HighchartBarNegativeStackChart() : base()
        {
            Type = "bar";

        }

        [InputText("type")]
        [JsonProperty("type")]
        [Label("Type")]
        public string Type { get; set; }
    }
    [Label("Конфигурация Highcharts: Bar Negative Stack Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Bar Negative Stack Title.")]
    public class HighchartBarNegativeStackTitle : Highchart
    {

        public HighchartBarNegativeStackTitle() : base()
        {
            Text = "Population pyramid for Germany, 2018";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Bar Negative Stack Subtitle")]
    [Description("Часть конфигурации диаграммы Highcharts: Bar Negative Stack Subtitle.")]
    public class HighchartBarNegativeStackSubtitle : Highchart
    {

        public HighchartBarNegativeStackSubtitle() : base()
        {
            Text = "Source: <a href='http://populationpyramid.net/germany/2018/'>Population Pyramids of the World from 1950 to 2100</a>";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Bar Negative Stack Accessibility Point")]
    [Description("Часть конфигурации диаграммы Highcharts: Bar Negative Stack Accessibility Point.")]
    public class HighchartBarNegativeStackAccessibilityPoint : Highchart
    {

        public HighchartBarNegativeStackAccessibilityPoint() : base()
        {
            ValueDescriptionFormat = "{index}. Age {xDescription}, {value}%.";

        }

        [InputText("valueDescriptionFormat")]
        [JsonProperty("valueDescriptionFormat")]
        [Label("Value Description Format")]
        public string ValueDescriptionFormat { get; set; }
    }
    [Label("Конфигурация Highcharts: Bar Negative Stack Accessibility")]
    [Description("Часть конфигурации диаграммы Highcharts: Bar Negative Stack Accessibility.")]
    public class HighchartBarNegativeStackAccessibility : Highchart
    {

        public HighchartBarNegativeStackAccessibility() : base()
        {
            Point = new HighchartBarNegativeStackAccessibilityPoint();

        }


        [JsonProperty("point")]
        [Label("Point")]
        public HighchartBarNegativeStackAccessibilityPoint Point { get; set; }
    }
    [Label("Конфигурация Highcharts: Bar Negative Stack X Axis Item Labels")]
    [Description("Часть конфигурации диаграммы Highcharts: Bar Negative Stack X Axis Item Labels.")]
    public class HighchartBarNegativeStackXAxisItemLabels : Highchart
    {

        public HighchartBarNegativeStackXAxisItemLabels() : base()
        {
            Step = 1f;

        }

        [InputNumber("step")]
        [JsonProperty("step")]
        [Label("Step")]
        public float Step { get; set; }
    }
    [Label("Конфигурация Highcharts: Bar Negative Stack X Axis Item Accessibility")]
    [Description("Часть конфигурации диаграммы Highcharts: Bar Negative Stack X Axis Item Accessibility.")]
    public class HighchartBarNegativeStackXAxisItemAccessibility : Highchart
    {

        public HighchartBarNegativeStackXAxisItemAccessibility() : base()
        {
            Description = "Age (male)";

        }

        [InputText("description")]
        [JsonProperty("description")]
        [Label("Description")]
        public string Description { get; set; }
    }
    [Label("Конфигурация Highcharts: Bar Negative Stack X Axis Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Bar Negative Stack X Axis Item.")]
    public class HighchartBarNegativeStackXAxisItem : Highchart
    {

        public HighchartBarNegativeStackXAxisItem() : base()
        {
            Categories = new List<string> { "0-4", "5-9", "10-14", "15-19", "20-24", "25-29", "30-34", "35-39", "40-44", "45-49", "50-54", "55-59", "60-64", "65-69", "70-74", "75-79", "80-84", "85-89", "90-94", "95-99", "100 + " };
            Reversed = null;
            Labels = new HighchartBarNegativeStackXAxisItemLabels();
            Accessibility = new HighchartBarNegativeStackXAxisItemAccessibility();

        }


        [JsonProperty("categories")]
        [Label("Categories")]
        public List<string> Categories { get; set; }

        [JsonProperty("reversed")]
        [Label("Reversed")]
        public object Reversed { get; set; }

        [JsonProperty("labels")]
        [Label("Labels")]
        public HighchartBarNegativeStackXAxisItemLabels Labels { get; set; }

        [JsonProperty("accessibility")]
        [Label("Accessibility")]
        public HighchartBarNegativeStackXAxisItemAccessibility Accessibility { get; set; }
    }
    [Label("Конфигурация Highcharts: Bar Negative Stack Y Axis Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Bar Negative Stack Y Axis Title.")]
    public class HighchartBarNegativeStackYAxisTitle : Highchart
    {

        public HighchartBarNegativeStackYAxisTitle() : base()
        {
            Text = null;

        }


        [JsonProperty("text")]
        [Label("Text")]
        public object Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Bar Negative Stack Y Axis Labels")]
    [Description("Часть конфигурации диаграммы Highcharts: Bar Negative Stack Y Axis Labels.")]
    public class HighchartBarNegativeStackYAxisLabels : Highchart
    {

        public HighchartBarNegativeStackYAxisLabels() : base()
        {
            Formatter = () => { };

        }


        [JsonProperty("formatter")]
        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [Label("Formatter")]
        public Action Formatter { get; set; }
    }
    [Label("Конфигурация Highcharts: Bar Negative Stack Y Axis Accessibility")]
    [Description("Часть конфигурации диаграммы Highcharts: Bar Negative Stack Y Axis Accessibility.")]
    public class HighchartBarNegativeStackYAxisAccessibility : Highchart
    {

        public HighchartBarNegativeStackYAxisAccessibility() : base()
        {
            Description = "Percentage population";
            RangeDescription = "Range: 0 to 5%";

        }

        [InputText("description")]
        [JsonProperty("description")]
        [Label("Description")]
        public string Description { get; set; }
        [InputText("rangeDescription")]
        [JsonProperty("rangeDescription")]
        [Label("Range Description")]
        public string RangeDescription { get; set; }
    }
    [Label("Конфигурация Highcharts: Bar Negative Stack Y Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: Bar Negative Stack Y Axis.")]
    public class HighchartBarNegativeStackYAxis : Highchart
    {

        public HighchartBarNegativeStackYAxis() : base()
        {
            Title = new HighchartBarNegativeStackYAxisTitle();
            Labels = new HighchartBarNegativeStackYAxisLabels();
            Accessibility = new HighchartBarNegativeStackYAxisAccessibility();

        }


        [JsonProperty("title")]
        [Label("Title")]
        public HighchartBarNegativeStackYAxisTitle Title { get; set; }

        [JsonProperty("labels")]
        [Label("Labels")]
        public HighchartBarNegativeStackYAxisLabels Labels { get; set; }

        [JsonProperty("accessibility")]
        [Label("Accessibility")]
        public HighchartBarNegativeStackYAxisAccessibility Accessibility { get; set; }
    }
    [Label("Конфигурация Highcharts: Bar Negative Stack Plot Options Series")]
    [Description("Часть конфигурации диаграммы Highcharts: Bar Negative Stack Plot Options Series.")]
    public class HighchartBarNegativeStackPlotOptionsSeries : Highchart
    {

        public HighchartBarNegativeStackPlotOptionsSeries() : base()
        {
            Stacking = "normal";

        }

        [InputText("stacking")]
        [JsonProperty("stacking")]
        [Label("Stacking")]
        public string Stacking { get; set; }
    }
    [Label("Конфигурация Highcharts: Bar Negative Stack Plot Options")]
    [Description("Часть конфигурации диаграммы Highcharts: Bar Negative Stack Plot Options.")]
    public class HighchartBarNegativeStackPlotOptions : Highchart
    {

        public HighchartBarNegativeStackPlotOptions() : base()
        {
            Series = new HighchartBarNegativeStackPlotOptionsSeries();

        }


        [JsonProperty("series")]
        [Label("Series")]
        public HighchartBarNegativeStackPlotOptionsSeries Series { get; set; }
    }
    [Label("Конфигурация Highcharts: Bar Negative Stack Tooltip")]
    [Description("Часть конфигурации диаграммы Highcharts: Bar Negative Stack Tooltip.")]
    public class HighchartBarNegativeStackTooltip : Highchart
    {

        public HighchartBarNegativeStackTooltip() : base()
        {
            Formatter = () => { };

        }


        [JsonProperty("formatter")]
        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [Label("Formatter")]
        public Action Formatter { get; set; }
    }
    [Label("Конфигурация Highcharts: Bar Negative Stack Series Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Bar Negative Stack Series Item.")]
    public class HighchartBarNegativeStackSeriesItem : Highchart
    {

        public HighchartBarNegativeStackSeriesItem() : base()
        {
            Name = "Male";
            Data = new List<float?> { -2.2f, -2.1f, -2.2f, -2.4f, -2.7f, -3f, -3.3f, -3.2f, -2.9f, -3.5f, -4.4f, -4.1f, -3.4f, -2.7f, -2.3f, -2.2f, -1.6f, -0.6f, -0.3f, 0, 0 };

        }

        [InputText("name")]
        [JsonProperty("name")]
        [Label("Name")]
        public string Name { get; set; }

        [JsonProperty("data")]
        [Label("Data")]
        public List<float?> Data { get; set; }
    }
    [Label("Диаграмма Highcharts: Bar Negative Stack")]
    [Description("Модель диаграммы Highcharts: Bar Negative Stack.")]
    public class HighchartBarNegativeStack : Highchart
    {

        public HighchartBarNegativeStack() : base()
        {
            Chart = new HighchartBarNegativeStackChart();
            Title = new HighchartBarNegativeStackTitle();
            Subtitle = new HighchartBarNegativeStackSubtitle();
            Accessibility = new HighchartBarNegativeStackAccessibility();
            XAxis = new List<HighchartBarNegativeStackXAxisItem> { new HighchartBarNegativeStackXAxisItem(), new HighchartBarNegativeStackXAxisItem() };
            YAxis = new HighchartBarNegativeStackYAxis();
            PlotOptions = new HighchartBarNegativeStackPlotOptions();
            Tooltip = new HighchartBarNegativeStackTooltip();
            Series = new List<HighchartBarNegativeStackSeriesItem> { new HighchartBarNegativeStackSeriesItem(), new HighchartBarNegativeStackSeriesItem() };

        }


        [JsonProperty("chart")]
        public HighchartBarNegativeStackChart Chart { get; set; }

        [JsonProperty("title")]
        public HighchartBarNegativeStackTitle Title { get; set; }

        [JsonProperty("subtitle")]
        public HighchartBarNegativeStackSubtitle Subtitle { get; set; }

        [JsonProperty("accessibility")]
        public HighchartBarNegativeStackAccessibility Accessibility { get; set; }

        [JsonProperty("xAxis")]
        public List<HighchartBarNegativeStackXAxisItem> XAxis { get; set; }

        [JsonProperty("yAxis")]
        public HighchartBarNegativeStackYAxis YAxis { get; set; }

        [JsonProperty("plotOptions")]
        public HighchartBarNegativeStackPlotOptions PlotOptions { get; set; }

        [JsonProperty("tooltip")]
        public HighchartBarNegativeStackTooltip Tooltip { get; set; }

        [JsonProperty("series")]
        public List<HighchartBarNegativeStackSeriesItem> Series { get; set; }
    }


}