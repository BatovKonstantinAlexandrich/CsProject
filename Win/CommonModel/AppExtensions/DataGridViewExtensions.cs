public static class DataGridViewExtensions
{


    public static void AddTextbox(this DataGridViewRow grid, string value )
    {
        DataGridViewTextBoxCell textbox = new DataGridViewTextBoxCell()
        {
            Value = value
        };        
        grid.Cells.Add(textbox);
    }


    public static void AddCheckbox(this DataGridViewRow grid, bool value)
    {
        DataGridViewCheckBoxCell textbox = new DataGridViewCheckBoxCell()
        {
            Value = value
        };
        grid.Cells.Add(textbox);
    }



    public static void AddImage(this DataGridViewRow grid, byte[] data)
    {
        DataGridViewImageCell textbox = new DataGridViewImageCell()
        {
            Value = data
        };
        grid.Cells.Add(textbox);
    }


    public static void AddCombobox(this DataGridViewRow grid, string value, List<string> options)
    {
        DataGridViewComboBoxCell textbox = new DataGridViewComboBoxCell()
        {
            Value = value
        };
        foreach (string option in options)
        {
            textbox.Items.Add(option);
        } 
        grid.Cells.Add(textbox);
    }


    public static void SetInputRows(this DataGridView grid, IEnumerable<InputFormField> fields, List<string> columns)
    {
        grid.SetupDefaults();
        grid.Info(fields.ToJsonOnScreen());
        grid.ColumnCount = columns.Count();
        for (int i = 0; i < columns.Count(); i++)
        {
            grid.Columns[i].Name = columns.ToList()[i];
        }

        grid.RowCount = 1;// fields.Count();
        foreach (var field in fields)
        {
            var row = new DataGridViewRow();
            for (int i = 0; i < columns.Count(); i++)
            {
                switch(field.GetType().GetPropertyInputType(columns[i]))
                {
                    case "bool": 
                    case "checkbox": row.AddCheckbox((bool)field.GetValue(columns[i])); break; 
                    case "select":
                        var inputSelect = field.GetType().GetPropertyAttributes(columns[i]).FirstOrDefault(kv => kv.Key.ToType().IsExtends(typeof(InputSelectAttribute)));
                        var selectOptions = inputSelect.Key.ToType().New<InputSelectAttribute>(new object[] { inputSelect.Value }).Options;
                        row.AddCombobox(""+field.Value, selectOptions);
                        break;
                    default:  row.AddTextbox("" + field.GetValue(columns[i])); break;
                }
                
            }                
            grid.Rows.Add(row);           
        }
        grid.ReadOnly = false;
        grid.Update();
    }


    public static void SetupDefaults(this DataGridView grid)
    {
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.Location = new Point(8, 8);
        grid.Size = new Size(500, 250);
        grid.AutoSizeRowsMode =
            DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
        grid.ColumnHeadersBorderStyle =
            DataGridViewHeaderBorderStyle.Single;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
        grid.GridColor = Color.Black;
        grid.RowHeadersVisible = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = false;
        grid.Dock = DockStyle.Fill;
        grid.ReadOnly = true;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;

        grid.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        grid.ColumnHeadersDefaultCellStyle.Font = new Font(grid.Font, FontStyle.Bold);
    }


    public static IEnumerable<string> SetHeaders (this DataGridView grid, List<string> labels, IEnumerable<string> columns)
    {
     
        var columnNames = new List<string>();
        grid.ColumnCount = columns.Count();
        var columnsNames = columns.ToList();
        for (int i = 0; i < columns.Count(); i++)
        {
            grid.Columns[i].Name = labels[i];
            grid.Columns[i].Tag = columnsNames[i];
            columnNames.Add(grid.Columns[i].Name);
        }
        return columnNames;
    }


    public static IEnumerable<string> SetHeaders<TModel>(this DataGridView grid, IEnumerable<string> columns)
    {
        var labels = columns.Select(c => typeof(TModel).GetPropertyLabel(c)).ToList();
        return grid.SetHeaders(labels, columns);
    }


    public static IEnumerable<string> SetHeaders(this DataGridView grid, IEnumerable<string> columns)
    {
        var columnNames = new List<string>();
        grid.ColumnCount = columns.Count();
        var columnsNames = columns.ToList();
        for (int i = 0; i < columns.Count(); i++)
        {
            grid.Columns[i].Name = columnsNames[i];
            grid.Columns[i].Tag = columnsNames[i];
            columnNames.Add(grid.Columns[i].Name);
        }
        return columnNames;
    }

    public static void SetDataRows<TModel>(this DataGridView grid, IEnumerable<TModel> values)
    {
        if(values.Count()==0)
        {
            
        }
        else
        {
            var columns = values.First().GetType().GetOwnPropertyNames();
            grid.SetDataRows<TModel>(columns, values);
        }
        
    }
    public static void SetObjectsDataRows(this DataGridView grid, IEnumerable<object> values)
    {
        var p = values.FirstOrDefault();

        var columns = p is null? new List<string>(): p.GetType().GetOwnPropertyNames();
        grid.SetDataRows(columns, values);
    }
    public static void SetDataRows(this DataGridView grid, IEnumerable<string> columnsLabels, IEnumerable<string> columnsKeys, IEnumerable<Dictionary<string, object>> values, Action<DataGridView, int, int, object, object, string> onchanged = null)
    {
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
            return;

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
        grid.ReadOnly = true;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;

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
    }
    public static void SetDataRows<TModel>(this DataGridView grid, IEnumerable<string> columns, IEnumerable<TModel> values)
    {
        var columnsList = columns?.ToList() ?? new List<string>();
        if (columnsList.Count == 0)
            return;

        var valuesList = values?.ToList() ?? new List<TModel>();
        grid.SetupDefaults();

        var sample = valuesList.FirstOrDefault();
        var ptype = sample?.GetType() ?? typeof(TModel);
        var labels = columnsList.Select(c => ptype.GetPropertyLabel(c)).ToList();
        grid.SetHeaders(labels, columnsList);

        if (valuesList.Count == 0)
        {
            grid.RowCount = 1;
            for (var col = 0; col < columnsList.Count; col++)
                grid.Rows[0].Cells[col].Value = "";
            grid.Update();
            return;
        }

        grid.RowCount = valuesList.Count;
        var row = 0;
        foreach (var item in valuesList)
        {
            var col = 0;
            foreach (var column in columnsList)
            {
                grid.Rows[row].Cells[col++].Value = item is null ? "" : $"{item.GetValue(column)}";
            }
            row++;
        }

        grid.Update();
    }

    public static void SetDataRows(this DataGridView grid, IEnumerable<string> columns, IEnumerable<object> values)
    {
        var columnsList = columns?.ToList() ?? new List<string>();
        var valuesList = values?.ToList() ?? new List<object>();
        if (columnsList.Count == 0 && valuesList.Count > 0)
        {
            var first = valuesList.FirstOrDefault();
            if (first is not null)
                columnsList = first.GetType().GetOwnInputSimplePropertyNames();
        }

        if (columnsList.Count == 0)
        {
            grid.ColumnCount = 0;
            //grid.RowCount = 0;
            return;
        }

        grid.SetupDefaults();

        var sample = valuesList.FirstOrDefault();
        if(sample is not null)
        {
            var ptype = sample?.GetType() ?? typeof(object);
            var labels = columnsList.Select(c => ptype.GetPropertyLabel(c)).ToList();
            grid.SetHeaders(labels, columnsList);

        }

        if (valuesList.Count == 0)
        {
            grid.RowCount = 1;

            for (var col = 0; col < columnsList.Count; col++)
            {
                if (grid.Rows[0].Cells.Count > col)
                {
                    grid.Rows[0].Cells[col].Value = "";
                }
            }
            grid.Update();
            return;
        }

        grid.RowCount = valuesList.Count;
        for (var row = 0; row < valuesList.Count; row++)
        {
            var item = valuesList[row];
            for (var col = 0; col < columnsList.Count; col++)
                grid.Rows[row].Cells[col].Value = item is null ? "" : $"{item.GetValue(columnsList[col])}";
        }

        grid.Update();
    }

}

