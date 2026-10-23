using System.Collections.Generic;

using Newtonsoft.Json;
using System;

namespace Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts
{
    [Label("Конфигурация Highcharts: Line Log Axis Title")]
    [Description("Часть конфигурации диаграммы Highcharts: Line Log Axis Title.")]
    public class HighchartLineLogAxisTitle : Highchart
    {

        public HighchartLineLogAxisTitle() : base()
        {
            Text = "Logarithmic axis demo";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: Line Log Axis X Axis Accessibility")]
    [Description("Часть конфигурации диаграммы Highcharts: Line Log Axis X Axis Accessibility.")]
    public class HighchartLineLogAxisXAxisAccessibility : Highchart
    {

        public HighchartLineLogAxisXAxisAccessibility() : base()
        {
            RangeDescription = "Range: 1 to 10";

        }

        [InputText("rangeDescription")]
        [JsonProperty("rangeDescription")]
        [Label("Range Description")]
        public string RangeDescription { get; set; }
    }
    [Label("Конфигурация Highcharts: Line Log Axis X Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: Line Log Axis X Axis.")]
    public class HighchartLineLogAxisXAxis : Highchart
    {

        public HighchartLineLogAxisXAxis() : base()
        {
            TickInterval = 1f;
            Type = "logarithmic";
            Accessibility = new HighchartLineLogAxisXAxisAccessibility();

        }

        [InputNumber("tickInterval")]
        [JsonProperty("tickInterval")]
        [Label("Tick Interval")]
        public float TickInterval { get; set; }
        [InputText("type")]
        [JsonProperty("type")]
        [Label("Type")]
        public string Type { get; set; }

        [JsonProperty("accessibility")]
        [Label("Accessibility")]
        public HighchartLineLogAxisXAxisAccessibility Accessibility { get; set; }
    }
    [Label("Конфигурация Highcharts: Line Log Axis Y Axis Accessibility")]
    [Description("Часть конфигурации диаграммы Highcharts: Line Log Axis Y Axis Accessibility.")]
    public class HighchartLineLogAxisYAxisAccessibility : Highchart
    {

        public HighchartLineLogAxisYAxisAccessibility() : base()
        {
            RangeDescription = "Range: 0.1 to 1000";

        }

        [InputText("rangeDescription")]
        [JsonProperty("rangeDescription")]
        [Label("Range Description")]
        public string RangeDescription { get; set; }
    }
    [Label("Конфигурация Highcharts: Line Log Axis Y Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: Line Log Axis Y Axis.")]
    public class HighchartLineLogAxisYAxis : Highchart
    {

        public HighchartLineLogAxisYAxis() : base()
        {
            Type = "logarithmic";
            MinorTickInterval = 0.1f;
            Accessibility = new HighchartLineLogAxisYAxisAccessibility();

        }

        [InputText("type")]
        [JsonProperty("type")]
        [Label("Type")]
        public string Type { get; set; }
        [InputNumber("minorTickInterval")]
        [JsonProperty("minorTickInterval")]
        [Label("Minor Tick Interval")]
        public float MinorTickInterval { get; set; }

        [JsonProperty("accessibility")]
        [Label("Accessibility")]
        public HighchartLineLogAxisYAxisAccessibility Accessibility { get; set; }
    }
    [Label("Конфигурация Highcharts: Line Log Axis Tooltip")]
    [Description("Часть конфигурации диаграммы Highcharts: Line Log Axis Tooltip.")]
    public class HighchartLineLogAxisTooltip : Highchart
    {

        public HighchartLineLogAxisTooltip() : base()
        {
            HeaderFormat = "<b>{series.name}</b><br />";
            PointFormat = "x = {point.x}, y = {point.y}";

        }

        [InputText("headerFormat")]
        [JsonProperty("headerFormat")]
        [Label("Header Format")]
        public string HeaderFormat { get; set; }
        [InputText("pointFormat")]
        [JsonProperty("pointFormat")]
        [Label("Point Format")]
        public string PointFormat { get; set; }
    }
    [Label("Конфигурация Highcharts: Line Log Axis Series Item")]
    [Description("Часть конфигурации диаграммы Highcharts: Line Log Axis Series Item.")]
    public class HighchartLineLogAxisSeriesItem : Highchart
    {

        public HighchartLineLogAxisSeriesItem() : base()
        {
            Data = new List<float?> { 1f, 2f, 4f, 8f, 16f, 32f, 64f, 128f, 256f, 512f };
            PointStart = 1f;

        }


        [JsonProperty("data")]
        [Label("Data")]
        public List<float?> Data { get; set; }
        [InputNumber("pointStart")]
        [JsonProperty("pointStart")]
        [Label("Point Start")]
        public float PointStart { get; set; }
    }
    [Label("Диаграмма Highcharts: Line Log Axis")]
    [Description("Модель диаграммы Highcharts: Line Log Axis.")]
    public class HighchartLineLogAxis : Highchart
    {

        public HighchartLineLogAxis() : base()
        {
            Title = new HighchartLineLogAxisTitle();
            XAxis = new HighchartLineLogAxisXAxis();
            YAxis = new HighchartLineLogAxisYAxis();
            Tooltip = new HighchartLineLogAxisTooltip();
            Series = new List<HighchartLineLogAxisSeriesItem> { new HighchartLineLogAxisSeriesItem() };

        }


        [JsonProperty("title")]
        public HighchartLineLogAxisTitle Title { get; set; }

        [JsonProperty("xAxis")]
        public HighchartLineLogAxisXAxis XAxis { get; set; }

        [JsonProperty("yAxis")]
        public HighchartLineLogAxisYAxis YAxis { get; set; }

        [JsonProperty("tooltip")]
        public HighchartLineLogAxisTooltip Tooltip { get; set; }

        [JsonProperty("series")]
        public List<HighchartLineLogAxisSeriesItem> Series { get; set; }
    }


}