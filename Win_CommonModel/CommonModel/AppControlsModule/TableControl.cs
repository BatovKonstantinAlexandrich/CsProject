using AngleSharp.Attributes;

using MaterialSkin.Controls;

using Mysqlx.Crud;

using System.Collections;
using System.Windows.Forms;

using WinFormsApp.AppComponents.AppControls;

namespace WinFormsApp.AppComponents.AppControlsModule
{
    /// <summary>
    /// Управление таблицей
    /// </summary>
    public partial class TableControl : ControlBase
    {
        private readonly BindingSource bindingSource;

        public MaterialCard materialCard;
        public DataGridView dataGridView;
        KeyEventHandler add, remove;
        PreviewKeyDownEventHandler preview;
        int editRowIndex, editColumnIndex;

        public TableControl()
        {
            string key = "";
            InitializeComponent();

            // Чтобы форма не растягивала грид
            dataGridView.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            // (или вообще AnchorStyles.None, если нужно)
            // Только вертикальная прокрутка
            dataGridView.ScrollBars = ScrollBars.Vertical;
            // Шапка колонок видна и фиксирована сверху
            dataGridView.ColumnHeadersVisible = true;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            // Опционально: запретить изменение размеров пользователем
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.AllowUserToResizeColumns = false;

            this.BindMouseEvents(this.dataGridView);
            bindingSource = new BindingSource();
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.DataSource = bindingSource;
            dataGridView.CellValueChanged += OnCellValueChanged;
            dataGridView.RowsAdded += OnRowsAdded;
            dataGridView.RowsRemoved += OnRowsRemoved;
            dataGridView.KeyPress += OnKeyPress;
            dataGridView.EditingControlShowing += OnEditingControlShowing;
            dataGridView.CellBeginEdit += (s, e) => { editRowIndex = e.RowIndex; editColumnIndex = e.ColumnIndex; };
            dataGridView.CellEndEdit += (s, e) => { dataGridView.Rows[editRowIndex].Cells[editColumnIndex].Value = key; };
            bool delete = false;

            add = (sender, e) => { PressedKeys.Add(Enum.GetName(typeof(Keys), e.KeyCode)); this.Info(PressedKeys.ToJson()); delete = false; };
            remove = (sender, e) =>
            {
                if (!delete)
                {
                    key = "";
                    foreach (var pkey in PressedKeys)
                        key += $"{pkey}+";
                    if (key.EndsWith("+"))
                        key = key.Substring(0, key.Length - 1);
                    this.Info($"Rows[{editRowIndex}].Cells[{editColumnIndex}]");
                    if (editColumnIndex == 0)
                    {
                        dataGridView.EndEdit();
                        dataGridView.Rows[editRowIndex].Cells[editColumnIndex].Value = key;
                    }
                }
                PressedKeys.Remove(Enum.GetName(typeof(Keys), e.KeyCode));
                this.Info(PressedKeys.ToJson());
                delete = true;
                this.Info(key);
            };

            preview = (sender, e) => { this.Info(e.ToJsonOnScreen()); PressedKeys.Remove(Enum.GetName(typeof(Keys), e.KeyCode)); this.Info(PressedKeys.ToJson()); };

        }

        private void EditingControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // Некоторые клавиши требуют предварительной обработки

            this.Info(e.KeyCode);
            if (e.KeyCode == Keys.Tab || e.KeyCode == Keys.Enter)
            {
                e.IsInputKey = true;
            }
        }

        private List<string> PressedKeys = new();
        private void EditingControl_KeyDown(object sender, KeyEventArgs e)
        {

            var editingControl = sender as Control;

            switch (e.KeyCode)
            {
                case Keys.Enter:
                    //dataGridView1.EndEdit();
                    // Переходим к следующей ячейке
                    //MoveToNextCell();
                    e.Handled = true;
                    break;

                case Keys.Escape:
                    //dataGridView1.CancelEdit();
                    e.Handled = true;
                    break;

                case Keys.Tab:
                    if (e.Shift)
                    {
                        // Shift+Tab - предыдущая ячейка
                        //MoveToPreviousCell();
                    }
                    else
                    {
                        // Tab - следующая ячейка
                        //MoveToNextCell();
                    }
                    e.Handled = true;
                    break;
            }
        }
        private void OnEditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            PressedKeys.Clear();

            // Убираем предыдущие обработчики
            e.Control.KeyDown -= add;
            e.Control.KeyUp -= remove;
            e.Control.PreviewKeyDown -= preview;

            // Добавляем новые обработчики
            e.Control.KeyDown += add;
            e.Control.KeyUp += remove;
            e.Control.PreviewKeyDown += preview;
        }

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            System.Console.WriteLine(e.KeyChar);
        }

        public Action<int> OnRowRemoved = (index) => { };
        public Action<int> OnRowAdded = (index) => { };
        public Action<int, object> OnRowChanged = (index, p) => { };

        private void OnRowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            this.Info(bindingSource.DataSource);
            OnRowRemoved(e.RowIndex);
        }

        private void OnRowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            this.Info(bindingSource.DataSource);
            OnRowAdded(e.RowIndex);
        }

        private void OnCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            this.Info(bindingSource.DataSource);

            var enumerator = ((IEnumerable)bindingSource.DataSource).GetEnumerator();
            int i = 0;
            object p = null;
            while (enumerator.MoveNext())
            {

                if (e.RowIndex == i)
                {
                    var item = enumerator.Current;
                    p = item;
                    break;
                }
                i++;
            }
            OnRowChanged(e.RowIndex, p);
        }

        public void SetModel(IEnumerable model)
        {
            bindingSource.DataSource = model;
        }

        public void SetColumns(IEnumerable<string> columns)
        {
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.Visible = columns.Contains(column.Name);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        private void InitializeComponent()
        {
            materialCard = new MaterialCard();
            dataGridView = new DataGridView() { Dock = DockStyle.Fill };
            materialCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView).BeginInit();
            SuspendLayout();

            // Применяем современные стили к контейнеру
            BorderRadius = 8;
            BorderWidth = 0;
            ShowShadow = true;
            BackColor = Color.FromArgb(250, 250, 250);
            Padding = new Padding(8);

            // 
            // materialCard1 - современный стиль
            // 
            materialCard.BackColor = Color.FromArgb(255, 255, 255);
            materialCard.Controls.Add(dataGridView);
            materialCard.Depth = 0;
            materialCard.Dock = DockStyle.Fill;
            materialCard.ForeColor = Color.FromArgb(33, 33, 33);
            materialCard.Location = new Point(0, 0);
            materialCard.Margin = new Padding(8);
            materialCard.MouseState = MaterialSkin.MouseState.HOVER;
            materialCard.Name = "materialCard";
            materialCard.Padding = new Padding(12);
            materialCard.Size = new Size(280, 239);
            materialCard.TabIndex = 0;

            // 
            // dataGridView1 - современный стиль таблицы
            // 
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Location = new Point(12, 12);
            dataGridView.Name = "dataGridView";
            dataGridView.Size = new Size(256, 215);
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.TabIndex = 0;

            // Современные стили для DataGridView
            dataGridView.BackgroundColor = Color.White;
            dataGridView.BorderStyle = BorderStyle.None;
            dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridView.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridView.EnableHeadersVisualStyles = false;
            dataGridView.GridColor = Color.FromArgb(240, 240, 240);
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.MultiSelect = false;
            dataGridView.ReadOnly = true;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridView.DefaultCellStyle.BackColor = Color.White;
            dataGridView.DefaultCellStyle.ForeColor = Color.FromArgb(33, 33, 33);
            dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 240, 255);
            dataGridView.DefaultCellStyle.SelectionForeColor = Color.FromArgb(33, 33, 33);
            dataGridView.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            dataGridView.DefaultCellStyle.Padding = new Padding(8, 4, 8, 4);

            // Стили для заголовков
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(66, 66, 66);
            //dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.SemiBold);
            dataGridView.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 8, 8, 8);
            dataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            // Стили для строк заголовков
            dataGridView.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);
            dataGridView.RowHeadersDefaultCellStyle.ForeColor = Color.FromArgb(66, 66, 66);

            // Альтернативные цвета строк
            dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(252, 252, 252);
            dataGridView.AlternatingRowsDefaultCellStyle.ForeColor = Color.FromArgb(33, 33, 33);

            // 
            // TableControl
            // 
            Controls.Add(materialCard);
            Name = "TableControl";
            Size = new Size(280, 239);
            materialCard.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
            ResumeLayout(false);
        }


    }
}
