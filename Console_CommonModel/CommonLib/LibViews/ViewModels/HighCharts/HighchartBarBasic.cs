using System.Collections.Generic;

using Newtonsoft.Json;
using System;

namespace Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts
{
    public class HighchartBarBasicChart : Highchart
    {

        public HighchartBarBasicChart() : base()
        {
            Type = "bar";

        }

        [InputText("type")]
        [JsonProperty("type")]
        [Label("Type")]
        public string Type { get; set; }
    }


    public class HighchartBarBasicTitle : Highchart
    {

        public HighchartBarBasicTitle() : base()
        {
            Text = "Historic World Population by Region";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }


    public class HighchartBarBasicSubtitle : Highchart
    {

        public HighchartBarBasicSubtitle() : base()
        {
            Text = "Source: <a>Wikipedia.org</a>";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }


    public class HighchartBarBasicXAxisTitle : Highchart
    {

        public HighchartBarBasicXAxisTitle() : base()
        {
            Text = null;

        }

        [JsonProperty("text")]
        [Label("Text")]
        public object Text { get; set; }
    }


    public class HighchartBarBasicXAxis : Highchart
    {

        public HighchartBarBasicXAxis() : base()
        {
            Categories = new List<string> { "Africa", "America", "Asia", "Europe", "Oceania" };
            Title = new HighchartBarBasicXAxisTitle();

        }

        [JsonProperty("categories")]
        [Label("Categories")]
        public List<string> Categories { get; set; }
        [JsonProperty("title")]
        [Label("Title")]
        public HighchartBarBasicXAxisTitle Title { get; set; }
    }


    public class HighchartBarBasicYAxisTitle : Highchart
    {

        public HighchartBarBasicYAxisTitle() : base()
        {
            Text = "Population (millions)";
            Align = "high";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
        [InputText("align")]
        [JsonProperty("align")]
        [Label("Align")]
        public string Align { get; set; }
    }


    public class HighchartBarBasicYAxisLabels : Highchart
    {

        public HighchartBarBasicYAxisLabels() : base()
        {
            Overflow = "justify";

        }

        [InputText("overflow")]
        [JsonProperty("overflow")]
        [Label("Overflow")]
        public string Overflow { get; set; }
    }


    public class HighchartBarBasicYAxis : Highchart
    {

        public HighchartBarBasicYAxis() : base()
        {
            Min = null;
            Title = new HighchartBarBasicYAxisTitle();
            Labels = new HighchartBarBasicYAxisLabels();

        }

        [JsonProperty("min")]
        [Label("Min")]
        public object Min { get; set; }
        [JsonProperty("title")]
        [Label("Title")]
        public HighchartBarBasicYAxisTitle Title { get; set; }
        [JsonProperty("labels")]
        [Label("Labels")]
        public HighchartBarBasicYAxisLabels Labels { get; set; }
    }


    public class HighchartBarBasicTooltip : Highchart
    {

        public HighchartBarBasicTooltip() : base()
        {
            ValueSuffix = " millions";

        }

        [InputText("valueSuffix")]
        [JsonProperty("valueSuffix")]
        [Label("Value Suffix")]
        public string ValueSuffix { get; set; }
    }


    public class HighchartBarBasicPlotOptionsBarDataLabelsEnabled : Highchart
    {

        public HighchartBarBasicPlotOptionsBarDataLabelsEnabled() : base()
        {

        }


    }


    public class HighchartBarBasicPlotOptionsBarDataLabels : Highchart
    {

        public HighchartBarBasicPlotOptionsBarDataLabels() : base()
        {
            Enabled = new HighchartBarBasicPlotOptionsBarDataLabelsEnabled();

        }

        [JsonProperty("enabled")]
        [Label("Enabled")]
        public HighchartBarBasicPlotOptionsBarDataLabelsEnabled Enabled { get; set; }
    }


    public class HighchartBarBasicPlotOptionsBar : Highchart
    {

        public HighchartBarBasicPlotOptionsBar() : base()
        {
            DataLabels = new HighchartBarBasicPlotOptionsBarDataLabels();

        }

        [JsonProperty("dataLabels")]
        [Label("Data Labels")]
        public HighchartBarBasicPlotOptionsBarDataLabels DataLabels { get; set; }
    }


    public class HighchartBarBasicPlotOptions : Highchart
    {

        public HighchartBarBasicPlotOptions() : base()
        {
            Bar = new HighchartBarBasicPlotOptionsBar();

        }

        [JsonProperty("bar")]
        [Label("Bar")]
        public HighchartBarBasicPlotOptionsBar Bar { get; set; }
    }


    public class HighchartBarBasicLegendFloating : Highchart
    {

        public HighchartBarBasicLegendFloating() : base()
        {

        }


    }


    public class HighchartBarBasicLegendShadow : Highchart
    {

        public HighchartBarBasicLegendShadow() : base()
        {

        }


    }


    public class HighchartBarBasicLegend : Highchart
    {

        public HighchartBarBasicLegend() : base()
        {
            Layout = "vertical";
            Align = "right";
            VerticalAlign = "top";
            X = -40f;
            Y = 80f;
            Floating = new HighchartBarBasicLegendFloating();
            BorderWidth = 1f;
            BackgroundColor = "#FFFFFF";
            Shadow = new HighchartBarBasicLegendShadow();

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
        [InputNumber("x")]
        [JsonProperty("x")]
        [Label("X")]
        public float X { get; set; }
        [InputNumber("y")]
        [JsonProperty("y")]
        [Label("Y")]
        public float Y { get; set; }
        [JsonProperty("floating")]
        [Label("Floating")]
        public HighchartBarBasicLegendFloating Floating { get; set; }


        [JsonProperty("borderWidth")]
        [Label("Border Width")]
        [InputNumber]
        public float BorderWidth { get; set; }

        [InputText("backgroundColor")]
        [JsonProperty("backgroundColor")]
        [Label("Background Color")]
        public string BackgroundColor { get; set; }
        [JsonProperty("shadow")]
        [Label("Shadow")]
        public HighchartBarBasicLegendShadow Shadow { get; set; }
    }


    public class HighchartBarBasicCredits : Highchart
    {

        public HighchartBarBasicCredits() : base()
        {
            Enabled = null;

        }

        [JsonProperty("enabled")]
        [Label("Enabled")]
        public object Enabled { get; set; }
    }


    public class HighchartBarBasicSeriesItem : Highchart
    {

        public HighchartBarBasicSeriesItem() : base()
        {
            Name = "Year 1800";
            Data = new List<float?> { 107f, 31f, 635f, 203f, 2f };

        }

        [InputText("name")]
        [JsonProperty("name")]
        [Label("Name")]
        public string Name { get; set; }
        [JsonProperty("data")]
        [Label("Data")]
        public List<float?> Data { get; set; }
    }


    public class HighchartBarBasic : Highchart
    {

        public HighchartBarBasic() : base()
        {
            Chart = new HighchartBarBasicChart();
            Title = new HighchartBarBasicTitle();
            Subtitle = new HighchartBarBasicSubtitle();
            XAxis = new HighchartBarBasicXAxis();
            YAxis = new HighchartBarBasicYAxis();
            Tooltip = new HighchartBarBasicTooltip();
            PlotOptions = new HighchartBarBasicPlotOptions();
            Legend = new HighchartBarBasicLegend();
            Credits = new HighchartBarBasicCredits();
            Series = new List<HighchartBarBasicSeriesItem> { new HighchartBarBasicSeriesItem(), new HighchartBarBasicSeriesItem(), new HighchartBarBasicSeriesItem(), new HighchartBarBasicSeriesItem() };

        }

        [JsonProperty("chart")]
        public HighchartBarBasicChart Chart { get; set; }
        [JsonProperty("title")]
        public HighchartBarBasicTitle Title { get; set; }
        [JsonProperty("subtitle")]
        public HighchartBarBasicSubtitle Subtitle { get; set; }
        [JsonProperty("xAxis")]
        public HighchartBarBasicXAxis XAxis { get; set; }
        [JsonProperty("yAxis")]
        public HighchartBarBasicYAxis YAxis { get; set; }
        [JsonProperty("tooltip")]
        public HighchartBarBasicTooltip Tooltip { get; set; }
        [JsonProperty("plotOptions")]
        public HighchartBarBasicPlotOptions PlotOptions { get; set; }
        [JsonProperty("legend")]
        public HighchartBarBasicLegend Legend { get; set; }
        [JsonProperty("credits")]
        public HighchartBarBasicCredits Credits { get; set; }
        [JsonProperty("series")]
        public List<HighchartBarBasicSeriesItem> Series { get; set; }
    }


}