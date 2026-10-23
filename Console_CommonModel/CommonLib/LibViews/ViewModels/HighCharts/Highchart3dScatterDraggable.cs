using System.Collections.Generic;

using Newtonsoft.Json;
using System;

namespace Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts
{
    [Label("Конфигурация Highcharts: 3 d Scatter Draggable Chart Options 3 d Enabled")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Scatter Draggable Chart Options 3 d Enabled.")]
    public class Highchart3dScatterDraggableChartOptions3dEnabled : Highchart
    {

        public Highchart3dScatterDraggableChartOptions3dEnabled() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: 3 d Scatter Draggable Chart Options 3 d Frame Bottom")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Scatter Draggable Chart Options 3 d Frame Bottom.")]
    public class Highchart3dScatterDraggableChartOptions3dFrameBottom : Highchart
    {

        public Highchart3dScatterDraggableChartOptions3dFrameBottom() : base()
        {
            Size = 1f;
            Color = "rgba(0,0,0,0.02)";

        }

        [InputNumber("size")]
        [JsonProperty("size")]
        [Label("Size")]
        public float Size { get; set; }
        [InputText("color")]
        [JsonProperty("color")]
        [Label("Color")]
        public string Color { get; set; }
    }
    [Label("Конфигурация Highcharts: 3 d Scatter Draggable Chart Options 3 d Frame Back")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Scatter Draggable Chart Options 3 d Frame Back.")]
    public class Highchart3dScatterDraggableChartOptions3dFrameBack : Highchart
    {

        public Highchart3dScatterDraggableChartOptions3dFrameBack() : base()
        {
            Size = 1f;
            Color = "rgba(0,0,0,0.04)";

        }

        [InputNumber("size")]
        [JsonProperty("size")]
        [Label("Size")]
        public float Size { get; set; }
        [InputText("color")]
        [JsonProperty("color")]
        [Label("Color")]
        public string Color { get; set; }
    }
    [Label("Конфигурация Highcharts: 3 d Scatter Draggable Chart Options 3 d Frame Side")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Scatter Draggable Chart Options 3 d Frame Side.")]
    public class Highchart3dScatterDraggableChartOptions3dFrameSide : Highchart
    {

        public Highchart3dScatterDraggableChartOptions3dFrameSide() : base()
        {
            Size = 1f;
            Color = "rgba(0,0,0,0.06)";

        }

        [InputNumber("size")]
        [JsonProperty("size")]
        [Label("Size")]
        public float Size { get; set; }
        [InputText("color")]
        [JsonProperty("color")]
        [Label("Color")]
        public string Color { get; set; }
    }
    [Label("Конфигурация Highcharts: 3 d Scatter Draggable Chart Options 3 d Frame")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Scatter Draggable Chart Options 3 d Frame.")]
    public class Highchart3dScatterDraggableChartOptions3dFrame : Highchart
    {

        public Highchart3dScatterDraggableChartOptions3dFrame() : base()
        {
            Bottom = new Highchart3dScatterDraggableChartOptions3dFrameBottom();
            Back = new Highchart3dScatterDraggableChartOptions3dFrameBack();
            Side = new Highchart3dScatterDraggableChartOptions3dFrameSide();

        }


        [JsonProperty("bottom")]
        [Label("Bottom")]
        public Highchart3dScatterDraggableChartOptions3dFrameBottom Bottom { get; set; }

        [JsonProperty("back")]
        [Label("Back")]
        public Highchart3dScatterDraggableChartOptions3dFrameBack Back { get; set; }

        [JsonProperty("side")]
        [Label("Side")]
        public Highchart3dScatterDraggableChartOptions3dFrameSide Side { get; set; }
    }
    [Label("Конфигурация Highcharts: 3 d Scatter Draggable Chart Options 3 d")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Scatter Draggable Chart Options 3 d.")]
    public class Highchart3dScatterDraggableChartOptions3d : Highchart
    {

        public Highchart3dScatterDraggableChartOptions3d() : base()
        {
            Enabled = new Highchart3dScatterDraggableChartOptions3dEnabled();
            Alpha = 10f;
            Beta = 30f;
            Depth = 250f;
            ViewDistance = 5f;
            FitToPlot = null;
            Frame = new Highchart3dScatterDraggableChartOptions3dFrame();

        }


        [JsonProperty("enabled")]
        [Label("Enabled")]
        public Highchart3dScatterDraggableChartOptions3dEnabled Enabled { get; set; }
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
        [InputNumber("viewDistance")]
        [JsonProperty("viewDistance")]
        [Label("View Distance")]
        public float ViewDistance { get; set; }

        [JsonProperty("fitToPlot")]
        [Label("Fit To Plot")]
        public object FitToPlot { get; set; }

        [JsonProperty("frame")]
        [Label("Frame")]
        public Highchart3dScatterDraggableChartOptions3dFrame Frame { get; set; }
    }
    [Label("Конфигурация Highcharts: 3 d Scatter Draggable Chart")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Scatter Draggable Chart.")]
    public class Highchart3dScatterDraggableChart : Highchart
    {

        public Highchart3dScatterDraggableChart() : base()
        {
            RenderTo = "container";
            Margin = 100f;
            Type = "scatter3d";
            Animation = null;
            Options3d = new Highchart3dScatterDraggableChartOptions3d();

        }

        [InputText("renderTo")]
        [JsonProperty("renderTo")]
        [Label("Render To")]
        public string RenderTo { get; set; }
        [InputNumber("margin")]
        [JsonProperty("margin")]
        [Label("Margin")]
        public float Margin { get; set; }
        [InputText("type")]
        [JsonProperty("type")]
        [Label("Type")]
        public string Type { get; set; }

        [JsonProperty("animation")]
        [Label("Animation")]
        public object Animation { get; set; }

        [JsonProperty("options3d")]
        [Label("Options 3 d")]
        public Highchart3dScatterDraggableChartOptions3d Options3d { get; set; }
    }
    [Label("Конфигурация Highcharts: 3 d Scatter Draggable Title")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Scatter Draggable Title.")]
    public class Highchart3dScatterDraggableTitle : Highchart
    {

        public Highchart3dScatterDraggableTitle() : base()
        {
            Text = "Draggable box";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: 3 d Scatter Draggable Subtitle")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Scatter Draggable Subtitle.")]
    public class Highchart3dScatterDraggableSubtitle : Highchart
    {

        public Highchart3dScatterDraggableSubtitle() : base()
        {
            Text = "Click and drag the plot area to rotate in space";

        }

        [InputText("text")]
        [JsonProperty("text")]
        [Label("Text")]
        public string Text { get; set; }
    }
    [Label("Конфигурация Highcharts: 3 d Scatter Draggable Plot Options Scatter")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Scatter Draggable Plot Options Scatter.")]
    public class Highchart3dScatterDraggablePlotOptionsScatter : Highchart
    {

        public Highchart3dScatterDraggablePlotOptionsScatter() : base()
        {
            ChartWidth = 10f;
            ChartHeight = 10f;
            Depth = 10f;

        }

        [InputNumber("width")]
        [JsonProperty("width")]
        [Label("Chart Width")]
        public float ChartWidth { get; set; }

        [InputNumber("height")]
        [JsonProperty("height")]
        [Label("Chart Height")]
        public float ChartHeight { get; set; }

        [InputNumber("depth")]
        [JsonProperty("depth")]
        [Label("Depth")]
        public float Depth { get; set; }
    }
    [Label("Конфигурация Highcharts: 3 d Scatter Draggable Plot Options")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Scatter Draggable Plot Options.")]
    public class Highchart3dScatterDraggablePlotOptions : Highchart
    {

        public Highchart3dScatterDraggablePlotOptions() : base()
        {
            Scatter = new Highchart3dScatterDraggablePlotOptionsScatter();

        }


        [JsonProperty("scatter")]
        [Label("Scatter")]
        public Highchart3dScatterDraggablePlotOptionsScatter Scatter { get; set; }
    }
    [Label("Конфигурация Highcharts: 3 d Scatter Draggable Y Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Scatter Draggable Y Axis.")]
    public class Highchart3dScatterDraggableYAxis : Highchart
    {

        public Highchart3dScatterDraggableYAxis() : base()
        {
            Min = null;
            Max = 10f;
            Title = null;

        }


        [JsonProperty("min")]
        [Label("Min")]
        public object Min { get; set; }
        [InputNumber("max")]
        [JsonProperty("max")]
        [Label("Max")]
        public float Max { get; set; }

        [JsonProperty("title")]
        [Label("Title")]
        public object Title { get; set; }
    }
    [Label("Конфигурация Highcharts: 3 d Scatter Draggable X Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Scatter Draggable X Axis.")]
    public class Highchart3dScatterDraggableXAxis : Highchart
    {

        public Highchart3dScatterDraggableXAxis() : base()
        {
            Min = null;
            Max = 10f;
            GridLineWidth = 1f;

        }


        [JsonProperty("min")]
        [Label("Min")]
        public object Min { get; set; }
        [InputNumber("max")]
        [JsonProperty("max")]
        [Label("Max")]
        public float Max { get; set; }
        [InputNumber("gridLineWidth")]
        [JsonProperty("gridLineWidth")]
        [Label("Grid Line Width")]
        public float GridLineWidth { get; set; }
    }
    [Label("Конфигурация Highcharts: 3 d Scatter Draggable Z Axis")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Scatter Draggable Z Axis.")]
    public class Highchart3dScatterDraggableZAxis : Highchart
    {

        public Highchart3dScatterDraggableZAxis() : base()
        {
            Min = null;
            Max = 10f;
            ShowFirstLabel = null;

        }


        [JsonProperty("min")]
        [Label("Min")]
        public object Min { get; set; }
        [InputNumber("max")]
        [JsonProperty("max")]
        [Label("Max")]
        public float Max { get; set; }

        [JsonProperty("showFirstLabel")]
        [Label("Show First Label")]
        public object ShowFirstLabel { get; set; }
    }
    [Label("Конфигурация Highcharts: 3 d Scatter Draggable Legend")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Scatter Draggable Legend.")]
    public class Highchart3dScatterDraggableLegend : Highchart
    {

        public Highchart3dScatterDraggableLegend() : base()
        {
            Enabled = null;

        }


        [JsonProperty("enabled")]
        [Label("Enabled")]
        public object Enabled { get; set; }
    }
    [Label("Конфигурация Highcharts: 3 d Scatter Draggable Series Item Color By Point")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Scatter Draggable Series Item Color By Point.")]
    public class Highchart3dScatterDraggableSeriesItemColorByPoint : Highchart
    {

        public Highchart3dScatterDraggableSeriesItemColorByPoint() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: 3 d Scatter Draggable Series Item Accessibility Expose As Group Only")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Scatter Draggable Series Item Accessibility Expose As Group Only.")]
    public class Highchart3dScatterDraggableSeriesItemAccessibilityExposeAsGroupOnly : Highchart
    {

        public Highchart3dScatterDraggableSeriesItemAccessibilityExposeAsGroupOnly() : base()
        {

        }


    }
    [Label("Конфигурация Highcharts: 3 d Scatter Draggable Series Item Accessibility")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Scatter Draggable Series Item Accessibility.")]
    public class Highchart3dScatterDraggableSeriesItemAccessibility : Highchart
    {

        public Highchart3dScatterDraggableSeriesItemAccessibility() : base()
        {
            ExposeAsGroupOnly = new Highchart3dScatterDraggableSeriesItemAccessibilityExposeAsGroupOnly();

        }


        [JsonProperty("exposeAsGroupOnly")]
        [Label("Expose As Group Only")]
        public Highchart3dScatterDraggableSeriesItemAccessibilityExposeAsGroupOnly ExposeAsGroupOnly { get; set; }
    }
    [Label("Конфигурация Highcharts: 3 d Scatter Draggable Series Item")]
    [Description("Часть конфигурации диаграммы Highcharts: 3 d Scatter Draggable Series Item.")]
    public class Highchart3dScatterDraggableSeriesItem : Highchart
    {

        public Highchart3dScatterDraggableSeriesItem() : base()
        {
            Name = "Data";
            ColorByPoint = new Highchart3dScatterDraggableSeriesItemColorByPoint();
            Accessibility = new Highchart3dScatterDraggableSeriesItemAccessibility();
            Data = new List<List<float?>> { new List<float?> { 1f, 6f, 5f }, new List<float?> { 8f, 7f, 9f }, new List<float?> { 1f, 3f, 4f }, new List<float?> { 4f, 6f, 8f }, new List<float?> { 5f, 7f, 7f }, new List<float?> { 6f, 9f, 6f }, new List<float?> { 7f, 0, 5f }, new List<float?> { 2f, 3f, 3f }, new List<float?> { 3f, 9f, 8f }, new List<float?> { 3f, 6f, 5f }, new List<float?> { 4f, 9f, 4f }, new List<float?> { 2f, 3f, 3f }, new List<float?> { 6f, 9f, 9f }, new List<float?> { 0, 7f, 0 }, new List<float?> { 7f, 7f, 9f }, new List<float?> { 7f, 2f, 9f }, new List<float?> { 0, 6f, 2f }, new List<float?> { 4f, 6f, 7f }, new List<float?> { 3f, 7f, 7f }, new List<float?> { 0, 1f, 7f }, new List<float?> { 2f, 8f, 6f }, new List<float?> { 2f, 3f, 7f }, new List<float?> { 6f, 4f, 8f }, new List<float?> { 3f, 5f, 9f }, new List<float?> { 7f, 9f, 5f }, new List<float?> { 3f, 1f, 7f }, new List<float?> { 4f, 4f, 2f }, new List<float?> { 3f, 6f, 2f }, new List<float?> { 3f, 1f, 6f }, new List<float?> { 6f, 8f, 5f }, new List<float?> { 6f, 6f, 7f }, new List<float?> { 4f, 1f, 1f }, new List<float?> { 7f, 2f, 7f }, new List<float?> { 7f, 7f, 0 }, new List<float?> { 8f, 8f, 9f }, new List<float?> { 9f, 4f, 1f }, new List<float?> { 8f, 3f, 4f }, new List<float?> { 9f, 8f, 9f }, new List<float?> { 3f, 5f, 3f }, new List<float?> { 0, 2f, 4f }, new List<float?> { 6f, 0, 2f }, new List<float?> { 2f, 1f, 3f }, new List<float?> { 5f, 8f, 9f }, new List<float?> { 2f, 1f, 1f }, new List<float?> { 9f, 7f, 6f }, new List<float?> { 3f, 0, 2f }, new List<float?> { 9f, 9f, 0 }, new List<float?> { 3f, 4f, 8f }, new List<float?> { 2f, 6f, 1f }, new List<float?> { 8f, 9f, 2f }, new List<float?> { 7f, 6f, 5f }, new List<float?> { 6f, 3f, 1f }, new List<float?> { 9f, 3f, 1f }, new List<float?> { 8f, 9f, 3f }, new List<float?> { 9f, 1f, 0 }, new List<float?> { 3f, 8f, 7f }, new List<float?> { 8f, 0, 0 }, new List<float?> { 4f, 9f, 7f }, new List<float?> { 8f, 6f, 2f }, new List<float?> { 4f, 3f, 0 }, new List<float?> { 2f, 3f, 5f }, new List<float?> { 9f, 1f, 4f }, new List<float?> { 1f, 1f, 4f }, new List<float?> { 6f, 0, 2f }, new List<float?> { 6f, 1f, 6f }, new List<float?> { 3f, 8f, 8f }, new List<float?> { 8f, 8f, 7f }, new List<float?> { 5f, 5f, 0 }, new List<float?> { 3f, 9f, 6f }, new List<float?> { 5f, 4f, 3f }, new List<float?> { 6f, 8f, 3f }, new List<float?> { 0, 1f, 5f }, new List<float?> { 6f, 7f, 3f }, new List<float?> { 8f, 3f, 2f }, new List<float?> { 3f, 8f, 3f }, new List<float?> { 2f, 1f, 6f }, new List<float?> { 4f, 6f, 7f }, new List<float?> { 8f, 9f, 9f }, new List<float?> { 5f, 4f, 2f }, new List<float?> { 6f, 1f, 3f }, new List<float?> { 6f, 9f, 5f }, new List<float?> { 4f, 8f, 2f }, new List<float?> { 9f, 7f, 4f }, new List<float?> { 5f, 4f, 2f }, new List<float?> { 9f, 6f, 1f }, new List<float?> { 2f, 7f, 3f }, new List<float?> { 4f, 5f, 4f }, new List<float?> { 6f, 8f, 1f }, new List<float?> { 3f, 4f, 0 }, new List<float?> { 2f, 2f, 6f }, new List<float?> { 5f, 1f, 2f }, new List<float?> { 9f, 9f, 7f }, new List<float?> { 6f, 9f, 9f }, new List<float?> { 8f, 4f, 3f }, new List<float?> { 4f, 1f, 7f }, new List<float?> { 6f, 2f, 5f }, new List<float?> { 0, 4f, 9f }, new List<float?> { 3f, 5f, 9f }, new List<float?> { 6f, 9f, 1f }, new List<float?> { 1f, 9f, 2f } };

        }

        [InputText("name")]
        [JsonProperty("name")]
        [Label("Name")]
        public string Name { get; set; }

        [JsonProperty("colorByPoint")]
        [Label("Color By Point")]
        public Highchart3dScatterDraggableSeriesItemColorByPoint ColorByPoint { get; set; }

        [JsonProperty("accessibility")]
        [Label("Accessibility")]
        public Highchart3dScatterDraggableSeriesItemAccessibility Accessibility { get; set; }

        [JsonProperty("data")]
        public List<List<float?>> Data { get; set; }
    }
    [Label("Диаграмма Highcharts: 3 d Scatter Draggable")]
    [Description("Модель диаграммы Highcharts: 3 d Scatter Draggable.")]
    public class Highchart3dScatterDraggable : Highchart
    {

        public Highchart3dScatterDraggable() : base()
        {
            Chart = new Highchart3dScatterDraggableChart();
            Title = new Highchart3dScatterDraggableTitle();
            Subtitle = new Highchart3dScatterDraggableSubtitle();
            PlotOptions = new Highchart3dScatterDraggablePlotOptions();
            YAxis = new Highchart3dScatterDraggableYAxis();
            XAxis = new Highchart3dScatterDraggableXAxis();
            ZAxis = new Highchart3dScatterDraggableZAxis();
            Legend = new Highchart3dScatterDraggableLegend();
            Series = new List<Highchart3dScatterDraggableSeriesItem> { new Highchart3dScatterDraggableSeriesItem() };

        }


        [JsonProperty("chart")]
        public Highchart3dScatterDraggableChart Chart { get; set; }

        [JsonProperty("title")]
        public Highchart3dScatterDraggableTitle Title { get; set; }

        [JsonProperty("subtitle")]
        public Highchart3dScatterDraggableSubtitle Subtitle { get; set; }

        [JsonProperty("plotOptions")]
        public Highchart3dScatterDraggablePlotOptions PlotOptions { get; set; }

        [JsonProperty("yAxis")]
        public Highchart3dScatterDraggableYAxis YAxis { get; set; }

        [JsonProperty("xAxis")]
        public Highchart3dScatterDraggableXAxis XAxis { get; set; }

        [JsonProperty("zAxis")]
        public Highchart3dScatterDraggableZAxis ZAxis { get; set; }

        [JsonProperty("legend")]
        public Highchart3dScatterDraggableLegend Legend { get; set; }

        [JsonProperty("series")]
        public List<Highchart3dScatterDraggableSeriesItem> Series { get; set; }
    }


}