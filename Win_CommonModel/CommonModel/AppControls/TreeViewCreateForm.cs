using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Windows.Forms;
using MaterialSkin.Controls;
using WinFormsApp.AppViews.FormModule;
using WinFormsDesignerApp.Console_InputModule.ViewEvents.List;
using WinFormsApp.AppViewModules.TreeModule;
using WinFormsApp.AppViews.TreeViewServices;

namespace WinFormsApp.AppComponents.AppControls
{
    /// <summary>
    /// Форма для создания TreeView с выбором BusinessDatasource и настройкой запросов
    /// </summary>
    [Label("Форма создания TreeView")]
    public class TreeViewCreateForm : MaterialForm
    {
        private TreeViewCreateFormModel model;
        private InputPanel inputPanel;
        private Panel queriesPanel;
        private FlowLayoutPanel queriesFlowPanel;
        private MaterialButton btnAddQuery;
        private MaterialButton btnCreateTreeView;
        private MaterialButton btnCancel;
        private TreeView resultTreeView;

        public TreeView ResultTreeView => resultTreeView;

        public TreeViewCreateForm()
        {
            InitializeComponent();
            LoadBusinessDatasources();
        }

        private void InitializeComponent()
        {
            Text = "Создание TreeView";
            Width = 1000;
            Height = 700;
            StartPosition = FormStartPosition.CenterScreen;

            var mainPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            Controls.Add(mainPanel);



            // Создаем модель
            model = new TreeViewCreateFormModel();

            // Создаем InputPanel для редактирования модели
            inputPanel = new InputPanel(model, typeof(TreeViewCreateFormModel), (success, obj) =>
            {
                if (success)
                {
                    var model = (TreeViewCreateFormModel)obj;
                    UpdateConnectionFromDatasource();
                }
            });
            inputPanel.RunAtForm();
            inputPanel.Dock = DockStyle.Fill;

            this.LayoutFill(inputPanel);

            // Панель для запросов
            queriesPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(5)
            };
            mainPanel.Controls.Add(queriesPanel);

            var queriesLabel = new MaterialLabel
            {
                Text = "SQL запросы:",
                Dock = DockStyle.Top,
                Height = 30
            };
            queriesPanel.Controls.Add(queriesLabel);

            queriesFlowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };
            queriesPanel.Controls.Add(queriesFlowPanel);

            // Кнопка добавления запроса
            btnAddQuery = new MaterialButton
            {
                Text = "Добавить запрос",
                Dock = DockStyle.Top,
                Height = 35
            };
            btnAddQuery.Click += BtnAddQuery_Click;
            queriesPanel.Controls.Add(btnAddQuery);

            // Панель кнопок
            var buttonsPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                Padding = new Padding(10)
            };
            mainPanel.Controls.Add(buttonsPanel);

            btnCreateTreeView = new MaterialButton
            {
                Text = "Создать TreeView",
                Dock = DockStyle.Right,
                Width = 150,
                Height = 35
            };
            btnCreateTreeView.Click += BtnCreateTreeView_Click;
            buttonsPanel.Controls.Add(btnCreateTreeView);

            btnCancel = new MaterialButton
            {
                Text = "Отмена",
                Dock = DockStyle.Right,
                Width = 100,
                Height = 35,
                Margin = new Padding(0, 0, 10, 0)
            };
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;
            buttonsPanel.Controls.Add(btnCancel);

            // Подписка на изменение BusinessDatasourceId
            if (model != null)
            {
                model.AddPropertyChangeListener(nameof(TreeViewCreateFormModel.BusinessDatasourceId), evt =>
                {
                    UpdateConnectionFromDatasource();
                });
            }
        }

        private void LoadBusinessDatasources()
        {
            try
            {
                using (var db = new BusinessDbContext())
                {
                    var datasources = db.BusinessDatasources.ToList();
                    this.Info($"Загружено источников данных: {datasources.Count}");
                }
            }
            catch (Exception ex)
            {
                this.Warn($"Ошибка при загрузке источников данных: {ex.Message}");
            }
        }

        private void UpdateConnectionFromDatasource()
        {
            if (model.BusinessDatasourceId > 0)
            {
                try
                {
                    using (var db = new BusinessDbContext())
                    {
                        var datasource = db.BusinessDatasources.FirstOrDefault(d => d.Id == model.BusinessDatasourceId);
                        if (datasource != null)
                        {
                            model.BusinessDatasource = datasource;
                            this.Info($"Выбран источник данных: {datasource.Name}, Connection: {datasource.ConnectionString}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.Warn($"Ошибка при получении строки подключения: {ex.Message}");
                }
            }
        }

        private void BtnAddQuery_Click(object sender, EventArgs e)
        {
            var query = new SequentialQuery
            {
                SqlQuery = "SELECT * FROM TableName",
                KeyColumn = "Id",
                TextColumn = "Name",
                ParentKeyColumn = "ParentId",
                AddToRoot = true
            };

            model.Queries.Add(query);
            AddQueryControl(query);
        }

        private void AddQueryControl(SequentialQuery query)
        {
            var queryCard = new MaterialCard
            {
                Width = queriesFlowPanel.Width - 30,
                Height = 200,
                Margin = new Padding(5),
                Padding = new Padding(10)
            };

            var queryInputPanel = new InputPanel(query, typeof(SequentialQuery), (success, obj) =>
            {
                if (success)
                {
                    this.Info("Запрос обновлен");
                }
            });
            queryInputPanel.Dock = DockStyle.Fill;

            var deleteButton = new MaterialButton
            {
                Text = "Удалить",
                Dock = DockStyle.Bottom,
                Height = 30
            };
            deleteButton.Click += (s, e) =>
            {
                model.Queries.Remove(query);
                queriesFlowPanel.Controls.Remove(queryCard);
            };

            queryCard.Controls.Add(queryInputPanel);
            queryCard.Controls.Add(deleteButton);
            queriesFlowPanel.Controls.Add(queryCard);
        }

        private void BtnCreateTreeView_Click(object sender, EventArgs e)
        {
            try
            {
                // Валидация
                if (string.IsNullOrWhiteSpace(model.Title))
                {
                    MessageBox.Show("Необходимо указать заголовок", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (model.BusinessDatasourceId <= 0)
                {
                    MessageBox.Show("Необходимо выбрать источник данных", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(model.Connection))
                {
                    MessageBox.Show("Строка подключения не задана", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (model.Queries == null || model.Queries.Count == 0)
                {
                    MessageBox.Show("Необходимо добавить хотя бы один SQL запрос", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Создаем TreeView
                resultTreeView = Program.Create(model.Title, model.Connection, model.Queries.ToList());

                if (resultTreeView != null)
                {
                    DialogResult = DialogResult.OK;
                    this.Info("TreeView успешно создан");
                }
                else
                {
                    MessageBox.Show("Не удалось создать TreeView", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании TreeView: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Error($"Ошибка при создании TreeView: {ex}");
            }
        }
    }
}

