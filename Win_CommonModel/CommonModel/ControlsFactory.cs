using MaterialSkin.Controls;
using MaterialSkin;
using System.Windows.Forms;
using System.Reflection;
using MaterialApp.Win_MaterialModule.DialogModule;

using LiveCharts;
using System.Collections.Concurrent;
using System.Windows.Shapes;
using Namotion.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Windows.Forms;

using WinFormsApp;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using LiveCharts.WinForms;
using LiveCharts.Wpf;


/// <summary>
/// Создаёт элементы управления WinForms
/// </summary>
[Label("Фабрика элементов")]
[Description("Фабрика для создания стандартизированных элементов управления WinForms, включая кнопки, текстовые поля, списки, меню и другие UI-компоненты")]
public class ControlsFactory : MenuBuilder 
{

    /// <summary>
    /// Растягивает элемент stretch по всей ширине панели, left и right по бокам
    /// При этом размерами управляет созданная панель
    /// </summary>    
    public Panel CreateFlexRow(Control left, Control stretch, Control right)
    {
        
        if (left is null) left = new Panel();
        if (stretch is null) stretch = new Panel();
        if (right is null) right = new Panel();
        var p = new Panel() { BackColor = Color.AliceBlue };
        p.LayoutFill(stretch);
        p.LayoutLeft(left);
        p.LayoutRight(right);
        p.Resize += (s, e) =>
        {
            stretch.Width = p.Width - left.Width - right.Width;
        };
        return p;
    }


    


    private ContextMenuStrip? CreateContextMenu3(ContextView view, Action<object> value)
    {
        var contextMenu = new ContextMenuStrip();
        if (view.Items is not null)
        {
            foreach (var item in view.Items)
            {


            }
        }

        return contextMenu;
    }
    //public ContextMenuStrip CreateContextMenu(ContextView model, Action<string> onclick)
    //{
    //    try
    //    {
    //        if(model is null)
    //        {
    //            //throw new ArgumentException("Установите значение для аргумента model",nameof(model));
    //            return null;
    //        }    
    //        var contextMenu = new ContextMenuStrip();
    //        contextMenu.Click += (s, e) =>
    //        {
    //            model.OnClick(contextMenu, new EventArgs());
    //            if (onclick is not null)
    //            {
    //                onclick("");
    //            }
    //        };
    //        List<string> path = new();
    //        path.Add(model.Label);

    //        if (model.Items is not null)
    //        {
    //            foreach (var item in model.Items)
    //            {
    //                string prefix = "";
    //                foreach (var id in path)
    //                {
    //                    prefix += $"{id}.";
    //                }
    //                var menuItem = CreateContextMenu(path, item, onclick);

    //                contextMenu.Items.Add(menuItem);
    //            }
    //            model.Items.OnChanged += (s, evt) =>
    //            {
    //                switch (evt.Target.Action)
    //                {
    //                    case "added":
    //                        {
    //                            string prefix = "";
    //                            foreach (var id in path)
    //                            {
    //                                prefix += $"{id}.";
    //                            }
    //                            var item = (ContextView)evt.Target.Item;
    //                            var menuItem = CreateContextMenu(path, item, onclick);
    //                            contextMenu.Items.Add(menuItem);
    //                            break;
    //                        }

    //                    case "removed":
    //                        {
    //                            contextMenu.Items.RemoveAt(evt.Target.Index);
    //                        }
    //                        break;
    //                }
    //            };
    //        }
    //        return contextMenu;
    //    }
    //    catch (Exception ex)
    //    {
    //        this.Error($"Исключение при формировании контекстного меню => {ex.Message}\n{ex.StackTrace}");
    //        throw new Exception($"Исключение при формировании контекстного меню => {ex.Message}");
    //    }
    //}


    public ContextMenuStrip CreateContextMenu2(IEnumerable<string> model, Action<string> onclick)
    {
        try
        {
            var contextMenu = new ContextMenuStrip();
            List<string> path = new();


            if (model is not null)
            {
                foreach (var item in model)
                {
                    string prefix = "";
                    foreach (var id in path)
                    {
                        prefix += $"{id}.";
                    }
                    string cmd = item;
                    var menuItem = CreateContextMenuItem(item, () =>
                    {
                        onclick(cmd);
                    });

                    contextMenu.Items.Add(menuItem);
                }

            }
            return contextMenu;
        }
        catch (Exception ex)
        {
            this.Error($"Исключение при формировании контекстного меню => {ex.Message}\n{ex.StackTrace}");
            throw new Exception($"Исключение при формировании контекстного меню => {ex.Message}");
        }
    }

    public Panel CreateScrollPanel(Action<int> scrollChanged)
    {
        var panel = new Panel();
        panel.SetVerticalScroll();
        panel.Scroll += (s, e) =>
        {
            if (scrollChanged is not null)
            {
                scrollChanged(e.NewValue);
            }
        };
        return panel;
    }

    public MaterialCard CreateCard(Action<int> scrollChanged)
    {
        var panel = new Panel();
        panel.SetVerticalScroll();
        panel.Scroll += (s, e) =>
        {
            if (scrollChanged is not null)
            {
                scrollChanged(e.NewValue);
            }
        };
        return panel.ToCard();
    }

    public DateTimePicker CreateDateTimePicker(DateTime value, Action<DateTime> changed)
    {
        var picker = new DateTimePicker();
        picker.Value = value;
        picker.ValueChanged += (s, e) =>
        {
            changed(picker.Value);
        };
        return picker;
    }


    public LiveCharts.WinForms.CartesianChart CreateLineChart(string oxlabel, Dictionary<string, int> data)
    {
        ChartValues<int> chartValues = new ChartValues<int>();
        var chart = new LiveCharts.WinForms.CartesianChart()
        {
        };
        var oxlabels = data.Keys.ToList();
        oxlabels.Sort();

        chart.AxisX.Clear();
        chart.AxisX.Add(new Axis
        {
            Labels = oxlabels
        });
        foreach (var label in oxlabels)
        {
            chartValues.Add(data[label]);
        }
        LineSeries line = new LineSeries();
        line.Title = "";
        line.Values = chartValues;
        var seriesCollection = new SeriesCollection();
        seriesCollection.Add(line);
        chart.Series = seriesCollection;
        return chart;
    }

    public LiveCharts.WinForms.CartesianChart CreateLineChart(string oxlabel, Dictionary<string, List<float>> data)
    {
        ChartValues<int> chartValues = new ChartValues<int>();
        var chart = new LiveCharts.WinForms.CartesianChart()
        {
        };
        var oxlabels = data.Keys.ToList();
        oxlabels.Sort();

        chart.AxisX.Clear();
        chart.AxisX.Add(new Axis
        {
            Labels = oxlabels
        });
        foreach (var label in oxlabels)
        {
            chartValues.AddRange(data[label].Select(v => (int)Math.Floor(v)));
        }
        LineSeries line = new LineSeries();
        line.Title = "";
        line.Values = chartValues;
        var seriesCollection = new SeriesCollection();
        seriesCollection.Add(line);
        chart.Series = seriesCollection;
        return chart;
    }

    static List<string> ResolveCategoryLabels(string xLabel, int pointCount)
    {
        if (pointCount <= 0)
            pointCount = 1;
        if (!string.IsNullOrWhiteSpace(xLabel))
        {
            var parsed = xLabel.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();
            if (parsed.Count > 0)
            {
                while (parsed.Count < pointCount)
                    parsed.Add((parsed.Count + 1).ToString());
                return parsed.Take(pointCount).ToList();
            }
        }
        return Enumerable.Range(1, pointCount).Select(i => i.ToString()).ToList();
    }

    static int GetMaxPointCount(IEnumerable<NamedSeriesArray> series)
        => series?.Select(s => s.data?.Count ?? 0).DefaultIfEmpty(0).Max() ?? 0;

    static ChartValues<int> ToChartValues(IList<float> data, int pointCount)
    {
        var values = new ChartValues<int>();
        for (var i = 0; i < pointCount; i++)
            values.Add(i < (data?.Count ?? 0) ? (int)Math.Floor(data[i]) : 0);
        return values;
    }

    static LiveCharts.WinForms.CartesianChart CreateCartesianChartWithCategories(string xLabel, string yLabel, List<string> categoryLabels, List<string> valueLabels=null)
    {
        if(valueLabels is null)
        {
            valueLabels = new List<string>() { "1", "2", "3" } ;
        }
        var chart = new LiveCharts.WinForms.CartesianChart();
        chart.BackColor = Color.White;
        chart.AxisX.Clear();
        chart.AxisX.Add(new Axis
        {
            Title = xLabel,
            Labels = categoryLabels
        });
        if (!string.IsNullOrWhiteSpace(yLabel))
        {
            chart.AxisY.Clear();
            chart.AxisY.Add(new Axis { Title = yLabel, Labels = valueLabels } );
        }
        return chart;
    }

    public LiveCharts.WinForms.CartesianChart CreateLineChart(string xLabel, List<NamedSeriesArray> series, string yLabel = "", List<string> xLabels=null)
    {
        series ??= new List<NamedSeriesArray>();
        var pointCount = Math.Max(GetMaxPointCount(series), 1);
        var labels = ResolveCategoryLabels(xLabel, pointCount);
        var chart = CreateCartesianChartWithCategories(xLabel, yLabel, labels);
        var seriesCollection = new SeriesCollection();
        foreach (var item in series)
        {
            seriesCollection.Add(new LineSeries
            {
                Title = item.name ?? "",
                Values = ToChartValues(item.data, pointCount)
            });
        }
     
        chart.AxisY.Clear();
        chart.AxisY.Add(new Axis { Title = yLabel, Labels = new List<string>() { } });
        chart.AxisX.Clear();
        chart.AxisX.Add(new Axis { Title = xLabel, Labels = xLabels is null? new List<string>() { }: xLabels });
        chart.Series = seriesCollection;
        return chart;
    }

    public LiveCharts.WinForms.CartesianChart CreateColumnChart(string xLabel, List<NamedSeriesArray> series, string yLabel = "")
    {
        series ??= new List<NamedSeriesArray>();
        var pointCount = Math.Max(GetMaxPointCount(series), 1);
        var labels = ResolveCategoryLabels(xLabel, pointCount);
        var chart = CreateCartesianChartWithCategories(xLabel, yLabel, labels);
        var seriesCollection = new SeriesCollection();
        foreach (var item in series)
        {
            seriesCollection.Add(new ColumnSeries
            {
                Title = item.name ?? "",
                Values = ToChartValues(item.data, pointCount)
            });
        }
        chart.Series = seriesCollection;
        return chart;
    }

    public LiveCharts.WinForms.CartesianChart CreateBarChart(string xLabel, List<NamedSeriesArray> series, string yLabel = "")
    {
        series ??= new List<NamedSeriesArray>();
        var pointCount = Math.Max(GetMaxPointCount(series), 1);
        var labels = ResolveCategoryLabels(xLabel, pointCount);
        var chart = CreateCartesianChartWithCategories("", yLabel, labels);
        chart.AxisX[0].Title = xLabel;
        var seriesCollection = new SeriesCollection();
        foreach (var item in series)
        {
            seriesCollection.Add(new CandleSeries
            {
                Title = item.name ?? "",
                Values = ToChartValues(item.data, pointCount)
            });
        }
        chart.Series = seriesCollection;
        return chart;
    }

    public Control CreatePieChart(List<NamedSeriesArray> series, string chartTitle = "")
    {
        series ??= new List<NamedSeriesArray>();
        var pieChart = new LiveCharts.WinForms.PieChart
        {
            LegendLocation = LegendLocation.Right,
            Dock = DockStyle.Fill
        };
        Func<ChartPoint, string> labelPoint = chartPoint =>
            string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);
        var seriesCollection = new SeriesCollection();
        foreach (var item in series)
        {
            var value = item.data is { Count: > 0 }
                ? item.data.Sum(v => (double)v)
                : 0d;
            seriesCollection.Add(new PieSeries
            {
                Title = item.name ?? "",
                Values = new ChartValues<double> { value },
                DataLabels = true,
                LabelPoint = labelPoint
            });
        }
        if (seriesCollection.Count == 0)
        {
            seriesCollection.Add(new PieSeries
            {
                Title = "",
                Values = new ChartValues<double> { 0 },
                DataLabels = true,
                LabelPoint = labelPoint
            });
        }
        pieChart.Series = seriesCollection;

        if (string.IsNullOrWhiteSpace(chartTitle))
            return pieChart;

        var panel = new Panel { Dock = DockStyle.Fill };
        var titleLabel = CreateTextLabel(chartTitle, "Roboto", 14);
        titleLabel.Dock = DockStyle.Top;
        titleLabel.Height = 32;
        titleLabel.TextAlign = ContentAlignment.MiddleCenter;
        panel.Controls.Add(titleLabel);
        panel.Controls.Add(pieChart);
        return panel;
    }

    public LiveCharts.WinForms.CartesianChart CreateOrgChart(string xLabel, List<NamedSeriesArray> series, string yLabel = "")
    {
        series ??= new List<NamedSeriesArray>();
        var chart = new LiveCharts.WinForms.CartesianChart();
        var orgLabels = series.Select(s => s.name ?? "").ToList();
        if (orgLabels.Count == 0)
            orgLabels.Add("");
        chart.AxisY.Clear();
        chart.AxisY.Add(new Axis
        {
            Title = yLabel,
            Labels = orgLabels
        });
        chart.AxisX.Clear();
        chart.AxisX.Add(new Axis { Title = xLabel, MinValue = 0 });
        var values = new ChartValues<int>();
        foreach (var item in series)
            values.Add(item.data is { Count: > 0 } ? (int)Math.Floor(item.data.Sum(v => v)) : 0);
        chart.Series = new SeriesCollection
        {
            new RowSeries
            {
                Title = xLabel,
                Values = values
            }
        };
        return chart;
    }

    public DateTimePicker CreateInputDate(DateTime? value, Action<DateTime> changed)
    {
        var picker = new DateTimePicker();
        if (value is not null)
            picker.Value = (DateTime)value;
        picker.ValueChanged += (s, e) =>
        {
            changed(picker.Value);
        };
        return picker;
    }

    public MaterialMultiLineTextBox CreateMultilineTextBox(string text, Action<string> changed)
    {
        var box = new MaterialMultiLineTextBox();
        box.BorderStyle = BorderStyle.Fixed3D;
        box.Text = text;
        box.TextChanged += (sender, e) =>
        {

            if (changed is not null)
                changed(box.Text);

        };
        return box;
    }


    /*public ListEditorPanel CreateListEditor(Type type, List<Object> items, Action<object> onselect, Action<object> oncreated, Action<object> ondeleted  )
    {
        var factory = new ComponentsFactory();
        var panel = new Panel();
        panel.AddTitle(type);
        panel.AddFill(
            factory.CreateSelectBox(items.Select(item => item.ToString()).ToList(), 
            selected => { 

        
            }));
        var buttonsPanel = panel.AddSubmitButtons((success) => { 

        });

        return new ListEditorPanel();
    }*/



    public SubmitDialogControl CreateSubmitDialog(string title, Control target, Action<bool> complete)
    {
        return new SubmitDialogForm(title, target, complete);
    }


    public Label CreateTextLabel(string text, string fontName = "Roboto", int fontSize = 12)
    {
        var labelComponent = new Label();
        labelComponent.Font = new Font("Roboto", fontSize);
        labelComponent.Text = text;
        return labelComponent;
    }

    public ToolStripMenuItem CreateContextMenuItem(string title, Action onclick)
    {
        ToolStripMenuItem result = new(title);

        result.Click += (object? sender, EventArgs e) => { if (onclick is not null) onclick(); };
        return result;
    }


    public ContextMenuStrip CreateContextMenu(params ToolStripMenuItem[] items)
    {
        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.AddRange(items);
        return contextMenu;
    }


    public ContextMenuStrip CreateContextMenu(IEnumerable<string> items, Action<string> onclick)
    {
        var contextMenu = new ContextMenuStrip();
        var contextMenuItems = items.Select(item => this.CreateContextMenuItem(item, () => { var command = item; if (onclick is not null) onclick(command); })).ToArray();
        contextMenu.Items.AddRange(contextMenuItems);
        return contextMenu;
    }

    public IEnumerable<ToolStripMenuItem> CreateContextMenuItems(IEnumerable<string> items, Action<string> onclick)
    {
        var contextMenuItems = items.Select(item => this.CreateContextMenuItem(item, () => { var command = item; if (onclick is not null) onclick(command); })).ToArray();
        return contextMenuItems;
    }

    public ToolStripMenuItem CreateContextDropDownMenu(string text, IEnumerable<string> items, Action<string> onclick)
    {
        var contextMenuItems = items.Select(item => this.CreateContextMenuItem(item, () => { var command = item; if (onclick is not null) onclick(command); })).ToArray();
        var menu = CreateContextMenu(text, contextMenuItems);
        return menu;
    }

    public ToolStripMenuItem CreateContextMenu(string text, IEnumerable<ToolStripMenuItem> items)
    {
        var menu = new ToolStripMenuItem() { Text = text };
        foreach (var item in items)
        {
            menu.DropDownItems.Add(item);
        }
        return menu;
    }



    public DataGridView CreateGrid(object data)
    {
        var bindingSource = new BindingSource();
        bindingSource.DataSource = data;
        var factory = new global::ControlsFactory();
        var grid = new DataGridView();
        grid.DataSource = bindingSource;
        return grid;
    }

    /// <summary>
    /// Компонент табличного представления
    /// </summary>
    public DataGridView CreateTable<TModel>(
        string title,
        IEnumerable<string> columns,
        IEnumerable<TModel> values,

        //grid, rowindex, columnIndex, value, targetEntity, propertyName
        Action<DataGridView, int, int, object, object, string> onchanged = null)
    {
        if (columns is null)
            throw new ArgumentNullException("columns");
        if (values is null)
            throw new ArgumentNullException("values");

        var grid = new DataGridView() { SelectionMode = DataGridViewSelectionMode.FullRowSelect };
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;


        grid.CellValueChanged += (sender, evt) =>
        {
            if (onchanged is not null)
            {
                try
                {
                    var target = (evt.RowIndex < values.Count()) ? (object)(values.ToList()[evt.RowIndex]) : null;
                    var property = (evt.ColumnIndex < columns.Count()) ? columns.ToList()[evt.ColumnIndex] : null;
                    if (onchanged is not null)
                    {
                        try
                        {
                            onchanged(grid, evt.RowIndex, evt.ColumnIndex, grid.Rows[evt.RowIndex].Cells[evt.ColumnIndex].Value, target, property);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Ошибка при выполнении обработчика событий изменения", ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

            }
        };
        grid.ColumnCount = columns.Count();
        grid.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        grid.ColumnHeadersDefaultCellStyle.Font =
            new Font(grid.Font, FontStyle.Bold);
        grid.Name = "songsDataGridView";
        grid.Location = new Point(8, 8);
        grid.Size = new Size(500, 250);
        grid.AutoSizeRowsMode =
            DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
        grid.ColumnHeadersBorderStyle =
            DataGridViewHeaderBorderStyle.Single;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
        grid.GridColor = Color.Black;
        grid.RowHeadersVisible = false;
        var columnsNames = columns.ToList();
        for (int i = 0; i < columns.Count(); i++)
        {
            grid.Columns[i].Name = typeof(TModel).HasProperty(columnsNames[i]) ? typeof(TModel).GetPropertyLabel(columnsNames[i]) : "";
        }
        if (values.Count() == 0)
        {
            grid.RowCount = 1;
            var col = 0;
            foreach (var column in columns)
            {
                grid.Rows[0].Cells[col++].Value = $"";
            }
            return grid;

        }
        grid.RowCount = values.Count();
        int row = 0;
        foreach (var item in values)
        {
            int col = 0;
            foreach (var column in columns)
            {
                grid.Rows[row].Cells[col++].Value = $"{item.GetValue(column)}";
            }
            row++;
        }
        grid.SelectionMode =
            DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = false;
        grid.Dock = DockStyle.Fill;

        grid.CellFormatting += (s, e) =>
        {
            if (e.ColumnIndex < 0 || e.ColumnIndex >= columnsNames.Count())
            {
                return;
            }
            var row = e.RowIndex;
            if (row < 0 || row >= values.ToList().Count())
            {
                return;
            }
            var column = columnsNames[e.ColumnIndex];

            var target = values.ToList()[row];
            if (onchanged is not null)
                onchanged(grid, e.RowIndex, e.ColumnIndex, e.Value, target, column);
        };
        grid.AutoResizeColumns();
        return grid;
    }


    public DataGridView CreateTableDictionary(
        string title,
        IEnumerable<string> columnsLabels,
        IEnumerable<string> columnsKeys,
        IEnumerable<Dictionary<string, object>> values,

        //grid, rowindex, columnIndex, value, targetEntity, propertyName
        Action<DataGridView, int, int, object, object, string> onchanged = null)
    {
        if (columnsLabels is null)
            throw new ArgumentNullException("columnsLabels");
        if (columnsKeys is null)
            throw new ArgumentNullException("columnsKeys");
        if (columnsKeys.Count() != columnsKeys.Count())
            throw new ArgumentException("columnsKeys", "Кол-во значений аргумента columnsLabels должно совпадать с кол-вом columnsKeys");
        if (values is null)
            throw new ArgumentNullException("values");

        var grid = new DataGridView() { SelectionMode = DataGridViewSelectionMode.FullRowSelect };
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;


        grid.CellValueChanged += (sender, evt) =>
        {
            /*if (onchanged is not null)
            {
                try
                {
                    var target = (evt.RowIndex < values.Count()) ? (object)(values.ToList()[evt.RowIndex]) : null;
                    var property = (evt.ColumnIndex < columnsKeys.Count()) ? columnsKeys.ToList()[evt.ColumnIndex] : null;
                    if (onchanged is not null)
                    {
                        try
                        {
                            onchanged(grid, evt.RowIndex, evt.ColumnIndex, grid.Rows[evt.RowIndex].Cells[evt.ColumnIndex].Value, target, property);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Ошибка при выполнении обработчика событий изменения", ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

            }*/
        };
        grid.ColumnCount = columnsKeys.Count();
        grid.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        grid.ColumnHeadersDefaultCellStyle.Font =
            new Font(grid.Font, FontStyle.Bold);
        grid.Name = "songsDataGridView";
        grid.Location = new Point(8, 8);
        grid.Size = new Size(500, 250);
        grid.AutoSizeRowsMode =
            DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
        grid.ColumnHeadersBorderStyle =
            DataGridViewHeaderBorderStyle.Single;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
        grid.GridColor = Color.Black;
        grid.RowHeadersVisible = false;
        var columnsNames = columnsLabels.ToList();
        for (int i = 0; i < columnsLabels.Count(); i++)
        {
            grid.Columns[i].Name = columnsNames[i];
        }
        if (values.Count() == 0)
        {
            grid.RowCount = 1;
            var col = 0;
            foreach (var column in columnsKeys)
            {
                grid.Rows[0].Cells[col++].Value = $"";
            }
            return grid;

        }
        grid.RowCount = values.Count();
        int row = 0;
        foreach (var item in values)
        {
            int col = 0;
            foreach (var column in columnsKeys)
            {
                grid.Rows[row].Cells[col++].Value = item.ContainsKey(column) == false ? "Ошибка" : $"{item[column]}";
            }
            row++;
        }
        grid.SelectionMode =
            DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = false;
        grid.Dock = DockStyle.Fill;

        grid.CellFormatting += (s, e) =>
        {
            if (e.ColumnIndex < 0 || e.ColumnIndex >= columnsNames.Count())
            {
                return;
            }
            var row = e.RowIndex;
            if (row < 0 || row >= values.ToList().Count())
            {
                return;
            }
            var column = columnsNames[e.ColumnIndex];

            var target = values.ToList()[row];
            if (onchanged is not null)
                onchanged(grid, e.RowIndex, e.ColumnIndex, e.Value, target, column);
        };
        grid.AutoResizeColumns();
        return grid;
    }


    public ComboBox CreateSelectBox(List<string> Options)
    {
        var select = new ComboBox();
        select.Items.AddRange(Options.Select(o => (object)o).ToArray());
        return select;
    }

    public MaterialComboBox CreateSelectObjectbyIdBox(Dictionary<int, string> Options)
    {
        var select = new MaterialComboBox();
        select.Items.AddRange(Options.Select(kv => $"{kv.Key}.{kv.Value}").ToArray());
        return select;
    }




    public System.Windows.Forms.TreeNode CreateTreeSelectByNameView(TreeViewNodeModel root, Action<string> selected)
    {
        var winFormNode = new TreeNode()
        {
            Text = root.Title,
            Checked = true
        };
        winFormNode.Expand();
        foreach (TreeViewNodeModel pnode in root.Children)
        {
            TreeNode pchildNode = CreateTreeSelectByNameView(pnode, selected);
            winFormNode.Nodes.Add(pchildNode.Text);
        }

        return winFormNode;
    }




    public System.Windows.Forms.TreeView CreateTreeTypedView<T>(TypeNode<T> root, Action<T> selected)
    {
        var view = new System.Windows.Forms.TreeView();
        Func<TypeNode<T>, TreeNode> convert = null;
        convert = (tnode) =>
        {
            var node = new TreeNode();
            node.Text = tnode.NodeName;
            foreach (var pchild in tnode.ChildNodes)
            {
                var nodeChild = convert(pchild.Value);
                node.Nodes.Add(nodeChild);
            }
            return node;
        };
        Func<TreeNode, T> find = (pnode) =>
        {
            var p = pnode;
            var path = new List<string>();
            while (p is not null)
            {
                path.Add(p.Text);
                p = p.Parent;
            }
            path.Reverse();
            var tp = root;
            path.RemoveAt(0);
            foreach (var name in path)
            {
                tp = tp.ChildNodes[name];
            }
            return tp.NodeItem;
        };
        view.Nodes.Add(convert(root));
        view.AfterSelect += (sender, e) =>
        {
            T item = find(e.Node);
            selected(item);
        };
        return view;
    }


    public ListBox CreateListBoxView(IEnumerable<string> items, Action<string> onselected)
    {
        var view = new ListBox();
        view.MouseDown += (sender, e) =>
        {
            var p = view?.SelectedItem?.ToString();
            if (p is not null)
                onselected(p);
        };
        return view;
    }


    public Control CreateInputText(string text, Action<string> onchanged)
    {
        var textbox = new MaterialTextBox2();
        textbox.Text = text;
        textbox.TextChanged += (s, e) =>
        {
            if (onchanged is not null)
                onchanged(textbox.Text);
        };
        return textbox;
    }
    public Control CreateTextBox(string value, Action<string> onChange)
    {
        var box = new TextBox();
        box.Text = value;
        box.TextChanged += (sender, e) =>
        {
            onChange(box.Text);
        };
        return box;
    }


    public void InfoTime(string text, Action todo)
    {
        var started = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        todo();
        var ended = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        this.Info($"{text}: {ended - started} мс.");
    }



    public void LayoutFill(System.Windows.Forms.Control panel, System.Windows.Forms.Control control)
    {
        panel.Controls.Clear();

        control.Anchor = AnchorStyles.Right;
        control.Dock = DockStyle.Fill;
        control.Location = new Point(0, 0);
        control.AutoSize = true;
        control.Dock = DockStyle.Fill;
        panel.Controls.Add(control);
    }



    public void SingleSelect<TModel>(MaterialForm form, IEnumerable<TModel> items, Func<TModel, MaterialListBoxItem> convert, Action<TModel> onselected) where TModel : class
    {
        throw new Exception("form.LayoutFill(items.CreateSelectList(phone => convert(phone), phone => onselected(phone)));");
        //form.LayoutFill(items.CreateSelectList(phone => convert(phone), phone => onselected(phone)));
    }

    public void Edit<TModel>(MaterialForm form, TModel model, Action<TModel> oncomplete) where TModel : class
    {
        throw new Exception("form.LayoutFill(model.CreateEditFormView<TModel>(oncomplete));");
        //form.LayoutFill(model.CreateEditFormView<TModel>(oncomplete));
    }



    /// <summary>
    /// Модель поля ввода на форме
    /// </summary>
    public class FormFieldModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public bool Valid { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<Func<string, string>> Filters { get; set; } = new();
        public List<Func<string, string>> Validators { get; set; } = new();
    }


    public List<FormFieldModel> GetInputFields(object target)
    {

        var list = target.GetType().GetInputProperties().Select(prop => new FormFieldModel()
        {
            Name = prop,
            Value = $"{target.GetType().GetProperty(prop).GetValue(target)}",
            Label = prop,
            Type = target.GetType().GetProperty(prop).PropertyType.IsValueType ? target.GetType().GetProperty(prop).PropertyType.Name : "object"
        }).ToList();
        list.Sort((f1, f2) => f1.Name == "Id" ? 1 : 0); ;
        return list;
    }


    public System.Windows.Forms.Panel CreatePanel(params System.Windows.Forms.Control[] controls)
    {
        var panel = new System.Windows.Forms.Panel();
        foreach (var control in controls)
            panel.Controls.Add(control);
        return panel;
    }


    public MaterialExpansionPanel CreateExpansionPanel(string text, Action<bool, object> onsubmit)
    {
        var panel = new MaterialExpansionPanel();
        panel.Title = text;
        return panel;
    }


    public MaterialButton CreateButton(string text = "", Action onclick = null)
    {
        var button = new MaterialButton();
        button.Text = text;
        button.Height = 20;
        button.Click += (sender, evt) =>
        {
            if (onclick is not null)
                onclick();
        };
        return button;
    }

    public MaterialButton CreateMaterialButton(string text = "", Action onclick = null)
    {
        var button = new MaterialButton();
        button.Text = text;
        button.Margin = new(10);
        button.Height = 20;
        button.Click += (sender, evt) =>
        {
            if (onclick is not null)
                onclick();
        };
        return button;
    }


    public MaterialButton CreateDangerButton(string text, Action onclick)
    {
        var button = new MaterialButton();
        button.Text = text;
        button.Margin = new(10);
        button.Height = 20;
        button.Click += (sender, evt) =>
        {
            onclick();
        };
        button.SkinManager.Theme = MaterialSkinManager.Themes.DARK;
        return button;
    }


    public Label CreateLink(string text, Action onclick)
    {
        var label = new Label();
        label.Font = new System.Drawing.Font("Roboto", 12);
        label.Text = text;
        label.ForeColor = Color.DarkGray;
        label.MouseEnter += (s, e) =>
        {
            label.ForeColor = Color.Blue;
        };
        label.MouseHover += (s, e) =>
        {
            label.ForeColor = Color.Blue;
        };
        label.MouseLeave += (s, e) =>
        {
            label.ForeColor = Color.DarkGray;
        };
        label.Click += (s, e) =>
        {
            onclick();
        };

        label.TextAlign = ContentAlignment.MiddleCenter;
        return label;
    }


    public MaterialTextBox CreateMaterialTextBox(string text, Action<string> oninput)
    {
        var textBox = new MaterialTextBox();
        textBox.Text = text;
        textBox.TextChanged += (sender, evt) =>
        {
            Task.Run(() => oninput(textBox.Text));
        };
        return textBox;
    }


    public MaterialTextBox CreateTextbox(InputFormField field)
    {
        var textBox = new MaterialTextBox();
        textBox.Text = $"{field.Value}";
        textBox.TextChanged += (sender, evt) =>
        {
            field.OnInput(textBox.Text);
        };
        return textBox;
    }


    public MaterialCard CreateEditFormView<TModel>(TModel model, Action<TModel> oncompleted)
    {
        var form = new MaterialSkin.Controls.MaterialCard();
        int p = -25;
        form.Controls.Add(CreateButton("Сохранить", () =>
        {
            oncompleted(model);
        }));
        foreach (var field in GetInputFields(model))
        {
            var pfield = field;
            form.Controls.Add(new MaterialLabel() { Text = "", Width = form.Width, Anchor = AnchorStyles.Left | AnchorStyles.Right, Dock = DockStyle.Top });
            MaterialTextBox input = CreateMaterialTextBox(field.Value, (changed) =>
            {
                //model.GetType().GetProperty(pfield.Name).SetValue(model, changed);
            });
            input.Width = form.Width;
            input.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            input.Dock = DockStyle.Top;


            form.Controls.Add(input);
            form.Controls.Add(new MaterialLabel() { Text = field.Label, Width = form.Width, Anchor = AnchorStyles.Left | AnchorStyles.Right, Dock = DockStyle.Top });
        }

        form.BackColor = Color.Red;
        return form;
    }


    public MaterialListBox CreateMaterialList<TModel>(
            IList<TModel> items,
            Func<TModel, MaterialListBoxItem> convert,
            Action<TModel> onselect) where TModel : class
    {
        var listBox = new MaterialListBox();
        Dictionary<object, MaterialListBoxItem> map = new();
        items.ToList().ForEach(phone =>
        {
            var item = convert(phone);
            item.Tag = phone;
            map[item.Tag] = item;
            listBox.Items.Add(item);
        });
        listBox.MouseDown += (sender, evt) =>
        {
            if (listBox.SelectedItem is not null)
            {
                onselect((TModel)listBox.SelectedItem.Tag);
            }
            else
            {
                onselect(null);
            }

        };
        listBox.SelectedValueChanged += (sender, evt) =>
        {
            onselect((TModel)evt.Tag);
        };

        return listBox;
    }

    public MaterialListBox CreateMaterialList(
       IList<string> options,
        Action<string> onselect)
    {
        var listBox = new MaterialListBox() { Dock = DockStyle.Fill };
        Dictionary<object, MaterialListBoxItem> map = new();
        options.ToList().ForEach(phone =>
        {
            var item = new MaterialListBoxItem();
            item.Tag = phone;
            item.Text = phone;
            map[item.Tag] = item;
            listBox.Items.Add(item);
        });
        string temp = null;
        listBox.MouseDown += (sender, evt) =>
        {
            string val = listBox.SelectedItem is not null ? listBox.SelectedItem.Text : null;
            if (val != temp)
            {
                temp = val;
                onselect(val);
            }

        };
        listBox.SelectedValueChanged += (sender, evt) =>
        {
            string val = listBox.SelectedItem is not null ? listBox.SelectedItem.Text : null;
            if (val != temp)
            {
                temp = val;
                onselect(val);
            }
        };
        return listBox;
    }


    public MaterialListBox CreateSelectListWithTitle(
        string title,
        List<string> options,
        Action<string> onselect)
    {
        var listBox = new MaterialListBox() { };
        Dictionary<object, MaterialListBoxItem> map = new();
        options.ToList().ForEach(phone =>
        {
            var item = new MaterialListBoxItem();
            item.Tag = phone;
            item.Text = phone;
            map[item.Tag] = item;
            listBox.Items.Add(item);
        });
        listBox.MouseDown += (sender, evt) =>
        {
            onselect(listBox.SelectedItem is not null ? listBox.SelectedItem.Text : null);
        };
        listBox.SelectedValueChanged += (sender, evt) =>
        {
            onselect(listBox.SelectedItem is not null ? listBox.SelectedItem.Text : null);
        };
        var panel = new Panel();
        panel.AddFill(listBox);
        panel.LayoutTop(this.CreateLabel(title, "Roboto", 16));
        return listBox;
    }



    public MaterialCheckedListBox CreateCheckedListBox(IEnumerable<string> options, Action<int, bool> onchanged = null)
    {
        var result = new MaterialCheckedListBox();
        int i = 0;
        foreach (var option in options.OrderByDescending(option => option))
        {
            int so = i;
            var item = new MaterialCheckbox()
            {
                Text = option
            };

            result.Items.Add(item);
            item.CheckedChanged += (s, e) =>
            {
                if (onchanged is not null)
                    onchanged(so, item.Checked);
            };

            i++;
        }

        return result;
    }


    public MaterialCheckedListBox CreateCheckedListBox(Dictionary<string, bool> options, Action<string, bool> onchanged = null)
    {
        try
        {
            var result = new MaterialCheckedListBox();
            foreach (var key in options.Keys.ToList().OrderByDescending(option => option))
            {

                var option = new
                {
                    Key = key,
                    Value = options[key]
                };
                MaterialCheckbox p = null;
                result.Items.Add(p = new MaterialCheckbox()
                {
                    Text = option.Key,
                    Checked = option.Value
                });

                p.CheckedChanged += (s, e) =>
                {

                    if (onchanged is not null)
                    {
                        var checkbox = result.Items.FirstOrDefault(p => p.Text == key);
                        onchanged(key, checkbox.Checked);
                    }
                };

            }

            return result;
        }
        catch (Exception ex)
        {
            this.Error(ex);
            throw;
        }
    }


    /// <summary>
    /// Чек-лист с кнопками ОК,ОТМЕНА
    /// </summary>
    public System.Windows.Forms.Panel CreateCheckedListView(Action<MethodResult<IEnumerable<string>>> oncompleted, IEnumerable<string> options)
    {
        var panel = new System.Windows.Forms.Panel();
        var list = CreateCheckedListBox(options);
        var ok = CreateButton("Готово", () =>
        {
            oncompleted(MethodResult<IEnumerable<string>>.OnResult(list.Items.Where(p => p.Checked).Select(p => p.Text)));
        });
        var cancel = CreateButton("Отмена", () =>
        {
            oncompleted(MethodResult<IEnumerable<string>>.OnError("Отменена"));
        });
        var buttons = new FlowLayoutPanel();
        buttons.FlowDirection = FlowDirection.LeftToRight;
        buttons.Controls.Add(ok);
        buttons.Controls.Add(cancel);
        buttons.Dock = DockStyle.Bottom;
        list.Width = panel.Width - panel.Padding.Left - panel.Padding.Right;
        buttons.Width = panel.Width - panel.Padding.Left - panel.Padding.Right;
        list.Height = panel.Height - panel.Padding.Top - panel.Padding.Bottom - buttons.Height;
        panel.Resize += (s, e) =>
        {
            list.Width = panel.Width - panel.Padding.Left - panel.Padding.Right;
            buttons.Width = panel.Width - panel.Padding.Left - panel.Padding.Right;
            list.Height = panel.Height - panel.Padding.Top - panel.Padding.Bottom - buttons.Height;
        };
        buttons.Controls.Add(list);
        return panel;
    }


    public System.Windows.Forms.Panel CreateFormField(PropertyInfo property, Action<object> onChange)
    {
        int OffsetTop = 0;
        int LineHeight = 20;
        var field = property.GetFormFieldModel(property.Name);
        var panel = new FlowLayoutPanel();
        MaterialLabel label = this.CreateLabel(property.Name);
        var labelComponent = new System.Windows.Forms.Label();
        labelComponent.Font = new System.Drawing.Font("Roboto", 10);

        labelComponent.Top = OffsetTop;
        labelComponent.Text = field.Label;

        labelComponent.Height = LineHeight;
        labelComponent.Width = panel.Width;
        panel.Controls.Add(labelComponent);
        OffsetTop += 20;

        // textbox
        var textBox = new System.Windows.Forms.TextBox();
        textBox.Name = field.Name;

        textBox.TextChanged += (sender, evt) =>
        {
            onChange(textBox.Text);
        };
        var Validators = new List<Func<object, string>>();
        textBox.ModifiedChanged += (s, e) =>
        {
            field.Errors.Clear();
            foreach (var validator in Validators)
            {
                string mes = validator(textBox.Text);
                if (!String.IsNullOrWhiteSpace(mes))
                {
                    field.Errors.Add(mes);
                }
            }
            onChange(textBox.Text);
        };
        textBox.Text = "";
        textBox.Top = OffsetTop;
        textBox.Height = LineHeight;

        textBox.Width = panel.Width;
        panel.Resize += (sender, evt) =>
        {
            textBox.Width = panel.Width;
        };
        panel.Controls.Add(textBox);
        OffsetTop += 20;

        // info
        var infolabelComponent = new System.Windows.Forms.Label();
        infolabelComponent.Text = field.Help;
        infolabelComponent.Height = LineHeight;
        infolabelComponent.Height = 20;

        infolabelComponent.Top = OffsetTop;
        infolabelComponent.Width = panel.Width;
        panel.Controls.Add(infolabelComponent);
        OffsetTop += 25;
        return panel;
    }

    public Label CreateBaseLabel(string title, string fontName = "Roboto", int fontSize = 12)
    {
        var label = new Label();
        label.Font = new System.Drawing.Font(fontName, fontSize);
        label.Text = title;
        label.AutoSize = false;
        return label;
    }
    public MaterialLabel CreateLabel(string title, string fontName = "Roboto", int fontSize = 12)
    {
        var label = new MaterialLabel();
        label.Font = new System.Drawing.Font(fontName, fontSize);
        label.Text = title;

        return label;
    }


    public MaterialLabel CreateLabel(string title, string fontName, int fontSize, Color color)
    {
        var label = new MaterialLabel();

        label.Font = new System.Drawing.Font(fontName, fontSize);
        label.Text = title;
        label.ForeColor = color;
        return label;
    }


    public System.Windows.Forms.Control CreateInputTextControl(InputFormField formField, List<Func<string, string>> filters = null)
    {
        var textBox = new MaterialTextBox();
        textBox.Text = $"{formField.Value}";
        string temp = "";
        textBox.TextChanged += (sender, evt) =>
        {
            formField.Warnings.Clear();
            formField.Errors.Clear();
            if (filters is not null)
            {
                foreach (var filter in filters)
                {
                    var error = filter(textBox.Text);
                    if (!String.IsNullOrWhiteSpace(error))
                        formField.Warnings.Add(error);
                }
            }

            if (formField.Warnings.Count() > 0)
            {
                textBox.Text = temp;
            }
            else
            {
                temp = textBox.Text;
                TextDataSetter.SetValue(formField.Target, formField.Name, textBox.Text);

                formField.Errors.AddRange(formField.Target.Validate(formField.Name));
                formField.State = formField.Errors.Count() > 0 ? "invalid" : "valid";
            };

            formField.OnInput(textBox.Text);
        };

        return textBox;
    }



    public MaterialCard FixTop(MaterialLabel label, MaterialCheckedListBox checkList)
    {
        var card = new MaterialCard();
        var flow = new FlowLayoutPanel() { Dock = DockStyle.Fill };
        checkList.Margin = label.Margin = new(0);
        label.Width = flow.Width - flow.Padding.Left - flow.Padding.Right;
        checkList.Width = flow.Width - flow.Padding.Left - flow.Padding.Right;
        checkList.Location = new(0, flow.Padding.Top + label.Height);
        checkList.Height = flow.Height - flow.Padding.Top - flow.Padding.Bottom - label.Height;
        flow.Controls.Add(label);
        label.Anchor = AnchorStyles.Left | AnchorStyles.Right;

        flow.Controls.Add(checkList);
        card.Controls.Add(flow);
        return card;
    }


    /// <summary>
    /// Обернуть в карточку
    /// </summary>
    public MaterialCard LayoutIntoCard(System.Windows.Forms.Control control)
    {
        var card = new MaterialCard();
        control.Margin = new(0);
        control.Dock = DockStyle.Fill;
        card.Controls.Add(control);
        return card;
    }


    /// <summary>
    /// Карточка с 2-я панелями
    /// </summary>
    public MaterialCard CreateSplitCard(System.Windows.Forms.Control left, System.Windows.Forms.Control right)
    {
        var card = new MaterialCard();
        card.Split(left, right);
        return card;
    }


    public System.Windows.Forms.TabControl CreateTabbedPane(Dictionary<string, Control> model)
    {
        var tabs = new TabControl();
        foreach (var kv in model)
        {
            kv.Value.Dock = DockStyle.Fill;
            tabs.AddTab(kv.Key, kv.Value);
        }
        return tabs;
    }

    public System.Windows.Forms.TabControl CreateTabbedPaneProvider(Dictionary<string, Func<object, Control>> model)
    {
        var tabs = new TabControl();
        foreach (var kv in model)
        {
            var p = new Panel();
            p.SetVerticalScroll();
            p.Dock = DockStyle.Fill;
            tabs.AddTab(kv.Key, p);
        }
        tabs.SelectedIndexChanged += (s, e) =>
        {
            tabs.SelectedTab.Controls[0].Controls.Clear();
            tabs.SelectedTab.Controls[0].Controls.Add(model[tabs.SelectedTab.Text](s));
        };
        tabs.SelectedTab.Controls[0].Controls.Clear();
        tabs.SelectedTab.Controls[0].Controls.Add(model[tabs.SelectedTab.Text](this));
        return tabs;
    }

    public System.Windows.Forms.DataGridView CreateGridView<TModel>(IEnumerable<TModel> items, IEnumerable<string> columns)
    {
        var grid = new DataGridView();
        var columnsList = columns.ToList();
        grid.ColumnCount = columns.Count();
        for (int i = 0; i < columns.Count(); i++)
        {
            grid.Columns[i].Name = columnsList[i];

        }
        int j = 0;
        grid.RowCount = items.Count() > 1 ? items.Count() : 1;
        foreach (var row in items)
        {
            for (int i = 0; i < columns.Count(); i++)
            {
                grid.Rows[j].Cells[i].Value = row.GetValue(columnsList[i]);
            }
            j++;
        }
        return grid;
    }


    public System.Windows.Forms.DataGridView CreateGridView(IEnumerable<object> items, IEnumerable<string> columns)
    {
        var grid = new DataGridView();
        var columnsList = columns.ToList();
        grid.ColumnCount = columns.Count();
        for (int i = 0; i < columns.Count(); i++)
        {
            grid.Columns[i].Name = columnsList[i];
            grid.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }
        int j = 0;
        if (items is not null && items.Count() > 0)
        {
            grid.RowCount = items.Count();
            foreach (var row in items)
            {
                for (int i = 0; i < columns.Count(); i++)
                {
                    grid.Rows[j].Cells[i].Value = row.GetValue(columnsList[i]);
                }
                j++;
            }
        }

        return grid;
    }


    public MaterialCheckbox CreateCheckBox(string text, Action<bool> onchanged)
    {
        var checkbox = new MaterialCheckbox()
        {
            Text = text
        };
        checkbox.CheckedChanged += (s, e) =>
        {
            if (onchanged is not null)
                onchanged(checkbox.Checked);
        };
        return checkbox;
    }

    public Panel LayoutFlexVertical(Control top, Control body, Control bottom)
    {
        var panel = new Panel();
        panel.LayoutTop(top);
        panel.AddFill(body);
        panel.LayoutBottom(bottom);
        return panel;
    }

    public Panel LayoutFlexHorizontal(Control left, Control body, Control right)
    {
        var panel = new Panel();
        panel.LayoutLeft(left);
        panel.AddFill(body);
        panel.LayoutRight(right);
        panel.Height = new int[] { body.Height, left.Height, right.Height }.Max();
        return panel;
    }


    /// <summary>
    /// Панель с диалогом подтверждения выполнения
    /// </summary>
    public MaterialCard CreateSubmitDialog(string title, string text, Action<bool> value)
    {
        var card = new MaterialCard();
        var top = this.CreateLabel(title, "Roboto", 16);
        var bottom = this.LayoutFlexHorizontal(
            this.CreateButton("подтвердить", () =>
            {
                value(true);
            }), new Panel(),
            this.CreateButton("отменить", () =>
            {
                value(false);
            })
        );
        bottom.Height = 20;
        var body = this.CreateLabel(text);
        card.LayoutTop(body);

        card.LayoutTop(top);
        card.AddFill(new Panel());
        card.LayoutBottom(bottom);
        return card;
    }

    /// <summary>
    /// Панель с диалогом подтверждения выполнения
    /// </summary>
    public MaterialCard CreateSubmitPanel(string title, Control content, Action<bool> value)
    {
        var card = new MaterialCard();
        var top = this.CreateLabel(title, "Roboto", 16);
        var bottom = this.LayoutFlexHorizontal(
            this.CreateButton("подтвердить", () =>
            {
                value(true);
            }), new Panel(),
            this.CreateButton("отменить", () =>
            {
                value(false);
            })
        );
        bottom.Height = 20;
        var body = content;
        card.LayoutFill(body);

        card.LayoutTop(top);
        //card.AddFill(new Panel());
        card.LayoutBottom(bottom);
        return card;
    }


    /// <summary>
    /// 
    /// </summary>
    public Control CreateImage(string file)
    {
        var box = new PictureBox();
        // Replace the image named below with your own icon.
        // Note the escape character used (@) when specifying the path.
        box.Image = System.Drawing.Image.FromFile(file);
        box.SizeMode = PictureBoxSizeMode.StretchImage;
        box.Size = box.Image.Size;
        return box;
    }


    /// <summary>
    /// Создает PictureBox с изображением из массива байтов
    /// PictureBox имеет размер изображения, изображение растягивается по всей плоскости
    /// </summary>
    public PictureBox CreateImage(byte[] data)
    {
        var box = new PictureBox();
        box.SizeMode = PictureBoxSizeMode.StretchImage;

        if (data is null || data.Length == 0)
            return box;
        
        using (var stream = new MemoryStream(data))
        {
            box.Image = System.Drawing.Image.FromStream(stream);
        }
        
        // Устанавливаем размер PictureBox равным размеру изображения
        box.Size = box.Image.Size;
        
        return box;
    }



    internal Control CreateButtons(string[] strings, Action<string> oncomplete)
    {
        var p = new Panel();
        foreach (var label in strings.Reverse())
        {
            var message = label;
            var button = CreateButton(label, () =>
            {
                oncomplete(message);
            });
            button.Margin = new(10);
            p.LayoutLeft(button);
        }
        p.AddFill(new Panel());
        p.Height = 40;
        return p;
    }


    public MaterialTextBox2 CreateTextbox(object value, Action<string> oninput)
    {
        var textbox = new MaterialTextBox2();
        textbox.Text = $"{value}";
        textbox.TextChanged += (s, e) => { oninput(textbox.Text); };
        return textbox;
    }

    public MaterialTextBox2 CreateTextbox(object value, Action<string> oninput, Action<List<string>> onerrors, params Func<string, string>[] filters)
    {
        var textbox = new MaterialTextBox2();
        textbox.Text = $"{value}";
        string temp = null;
        textbox.TextChanged += (s, e) =>
        {
            this.Info(textbox.Text);
            if (filters is not null)
            {
                try
                {
                    var errors = filters.Select(filter => filter(textbox.Text)).Where(err => !String.IsNullOrWhiteSpace(err)).ToList();
                    if (errors.Count() > 0 && onerrors is not null)
                    {
                        onerrors(errors);
                    }
                    else
                    {
                        if (oninput is not null)
                            oninput(textbox.Text);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                if (oninput is not null)
                    oninput(textbox.Text);
            }
        };
        return textbox;
    }


    public MaterialTextBox2 CreateInputNumber(object value, Action<string> oninput)
    {
        var textbox = new MaterialTextBox2();
        textbox.Text = $"{value}";
        string temp = "";
        textbox.TextChanged += (s, e) =>
        {
            if (!textbox.Text.IsNumber())
            {
                textbox.Text = temp;
            }
            else
            {
                temp = textbox.Text;
            }
            oninput(textbox.Text);
        };
        return textbox;
    }
    public Control CreateInputColor(object value, Action<string> onInput)
    {
        List<string> colorNames = new();
        foreach (Color color in Enum.GetValues(typeof(Color)))
        {
            colorNames.Add(color.Name);
        }
        return this.CreateSelectbox("black", colorNames, onInput);
    }

    public Control CreateInputUrl(object value, Action<string> onInput)
    {
        return CreateTextbox(value, onInput, errors => { }, (text) =>
        {
            return text is not null && text.IsUrl() == false ? "На является URL даресом" : null;
        });
    }

    public Control CreateInputDateTime(object value, Action<string> onInput)
    {
        return this.CreateInputDate(null, value, onInput);
    }

    public Control CreateInputPhone(object value, Action<string> onInput)
    {
        MaterialTextBox2 box = null;
        box = CreateTextbox(value, onInput,
            errors =>
            {
                foreach (var error in errors)
                    System.Console.WriteLine(error);
            },
            (text) =>
            {
                if (onInput is not null)
                {
                    onInput($"{value}");
                }
                return null;
            });

        return box;
    }

    internal Control CreateSelectIcon(object value, Action<string> onInput)
    {
        throw new NotImplementedException();
    }
    internal Control CreateInputDate(InputFormField field, object value, Action<string> onInput)
    {
        throw new NotImplementedException();
    }

    internal Control CreateSelectbox(string value, IEnumerable<string> options, Action<string> oninput)
    {
        var control = new MaterialComboBox();
        int index = options.ToList().IndexOf(value);
        //if (index == -1)
        //    throw new ArgumentException(nameof(value), "Значение не содержиться в списке допустимых");
        foreach (var option in options)
        {
            control.Items.Add(option);
        }
        control.SelectedIndex = index;
        control.SelectedIndexChanged += (s, e) =>
        {
            oninput(control.SelectedItem?.ToString());
        };
        return control;
    }
    internal Control CreateSelectbox(IEnumerable<string> options, Action<string> oninput)
    {
        var control = new MaterialComboBox();
        foreach (var option in options)
        {
            control.Items.Add(option);
        }
        control.SelectedIndexChanged += (s, e) =>
        {
            oninput(control.SelectedItem?.ToString());
        };
        return control;
    }

    public Label CreateBaseLabel(string text)
    {
        var label = new Label();
        label.Text = text;
        label.ForeColor = Color.Red;
        return label;
    }

}
