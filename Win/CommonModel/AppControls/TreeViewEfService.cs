using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

using WinFormsApp.AppViews.TreeViewServices;

using WinFormsApp.AppViewModules.TreeModule;

namespace WinFormsApp.AppComponents.AppControls
{
    /// <summary>
    /// Control для последовательной настройки иерархического представления данных из DbContext
    /// </summary>
    [Label("Сервис EF")]
    [Description("Пользовательский контрол для последовательной настройки иерархического представления данных из DbContext с пошаговым выбором сущностей и их связей")]
    public partial class TreeViewEfService<TDbContext> : UserControl where TDbContext : DbContext
    {
        private Type _dbContextType;
        private IModel _model;
        private List<Type> _selectedEntityTypes = new List<Type>();
        private Panel _contentPanel;
        private Button _doneButton;
        private Button _resetButton;
        private Label _currentPathLabel;
        private FlowLayoutPanel _selectedEntitiesPanel;
        private ListBox _availableEntitiesListBox;
        private Label _availableEntitiesLabel;


        private readonly Action<MethodResult<TreeControlView>> oncompleted;

        public static Control CreateTreeView(Action<MethodResult<TreeControlView>> oncompleted)
            => new TreeViewEfService<TDbContext>(oncompleted);


        /// <summary>
        /// Событие, возникающее при завершении настройки
        /// </summary>
        public event EventHandler<EntityTypesSelectedEventArgs> ConfigurationCompleted;

        /// <summary>
        /// Список выбранных типов сущностей
        /// </summary>
        public List<Type> SelectedEntityTypes => new List<Type>(_selectedEntityTypes);

        public TreeViewEfService([NotNullNotEmpty] Action<MethodResult<TreeControlView>> oncompleted)
        {
            MethodBase.GetCurrentMethod().EnsureAgumentsIsValid(oncompleted);
            InitializeComponent();
            SetDbContextType(typeof(TDbContext));
            this.oncompleted = oncompleted;
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            Size = new Size(600, 500);
            BorderStyle = BorderStyle.FixedSingle;

            // Панель для отображения выбранных сущностей
            _selectedEntitiesPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 100,
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(5)
            };

            // Метка для текущего пути
            _currentPathLabel = new Label
            {
                Dock = DockStyle.Top,
                Height = 30,
                Text = "Выберите начальную сущность:",
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5),
                BackColor = Color.LightBlue
            };

            // Метка для доступных сущностей
            _availableEntitiesLabel = new Label
            {
                Dock = DockStyle.Top,
                Height = 25,
                Text = "Доступные сущности:",
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5)
            };

            // Список доступных сущностей
            _availableEntitiesListBox = new ListBox
            {
                Dock = DockStyle.Fill,
                SelectionMode = SelectionMode.One
            };
            _availableEntitiesListBox.DoubleClick += AvailableEntitiesListBox_DoubleClick;

            // Кнопка выбора
            Button selectButton = new Button
            {
                Text = "Выбрать",
                Dock = DockStyle.Bottom,
                Height = 35
            };
            selectButton.Click += SelectButton_Click;

            // Панель для кнопок
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = Color.WhiteSmoke,
                BorderStyle = BorderStyle.FixedSingle
            };

            _resetButton = new Button
            {
                Text = "Сбросить",
                Location = new Point(10, 10),
                Size = new Size(100, 30),
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };
            _resetButton.Click += ResetButton_Click;

            _doneButton = new Button
            {
                Text = "ГОТОВО",
                Location = new Point(Width - 110, 10),
                Size = new Size(100, 30),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                Enabled = false,
                BackColor = Color.LightGreen,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            _doneButton.Click += DoneButton_Click;

            buttonPanel.Controls.Add(_resetButton);
            buttonPanel.Controls.Add(_doneButton);

            // Обработчик изменения размера для корректного позиционирования кнопки ГОТОВО
            Resize += (s, e) =>
            {
                if (_doneButton != null && buttonPanel != null)
                {
                    _doneButton.Location = new Point(buttonPanel.Width - 110, 10);
                }
            };

            // Панель содержимого
            _contentPanel = new Panel
            {
                Dock = DockStyle.Fill
            };

            _contentPanel.Controls.Add(_availableEntitiesListBox);
            _contentPanel.Controls.Add(selectButton);
            _contentPanel.Controls.Add(_availableEntitiesLabel);
            _contentPanel.Controls.Add(_currentPathLabel);

            // Важно: порядок добавления определяет порядок отрисовки
            // Сначала добавляем панель кнопок (нижняя граница)
            Controls.Add(buttonPanel);
            // Затем панель содержимого (заполняет оставшееся пространство)
            Controls.Add(_contentPanel);
            // И панель выбранных сущностей (верхняя граница)
            Controls.Add(_selectedEntitiesPanel);

            ResumeLayout(false);
        }

        /// <summary>
        /// Устанавливает тип DbContext и инициализирует модель
        /// </summary>
        public void SetDbContextType(Type dbContextType)
        {
            if (!typeof(DbContext).IsAssignableFrom(dbContextType))
            {
                throw new ArgumentException("Тип должен наследоваться от DbContext", nameof(dbContextType));
            }

            _dbContextType = dbContextType;
            _selectedEntityTypes.Clear();
            UpdateSelectedEntitiesPanel();
            LoadModel();
            RefreshAvailableEntities();
        }

        /// <summary>
        /// Загружает модель Entity Framework из DbContext
        /// </summary>
        private void LoadModel()
        {
            try
            {
                // Создаем экземпляр DbContext для получения модели
                using (DbContext dbContext = Activator.CreateInstance(_dbContextType!) as DbContext)
                {
                    if (dbContext != null)
                    {
                        _model = dbContext.Model;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке модели: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Обновляет список доступных сущностей
        /// </summary>
        private void RefreshAvailableEntities()
        {
            _availableEntitiesListBox.Items.Clear();

            if (_model == null)
            {
                _currentPathLabel.Text = "Модель не загружена";
                return;
            }

            if (_selectedEntityTypes.Count == 0)
            {
                // Показываем все сущности из DbContext
                _currentPathLabel.Text = "Выберите начальную сущность:";
                var allEntities = _model.GetEntityTypes()
                    .Select(et => et.ClrType)
                    .Where(t => t != null)
                    .OrderBy(t => t.Name)
                    .ToList();

                foreach (var entityType in allEntities)
                {
                    _availableEntitiesListBox.Items.Add(new EntityTypeItem(entityType));
                }
            }
            else
            {
                // Показываем связанные сущности
                Type lastSelectedType = _selectedEntityTypes.Last();
                _currentPathLabel.Text = $"Выберите связанную сущность для: {GetEntityPath()}";

                var relatedEntities = GetRelatedEntityTypes(lastSelectedType);
                foreach (var entityType in relatedEntities)
                {
                    _availableEntitiesListBox.Items.Add(new EntityTypeItem(entityType));
                }

                if (relatedEntities.Count == 0)
                {
                    _availableEntitiesListBox.Items.Add("Нет связанных сущностей");
                    _availableEntitiesListBox.Enabled = false;
                }
                else
                {
                    _availableEntitiesListBox.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Получает связанные типы сущностей для указанного типа
        /// </summary>
        private List<Type> GetRelatedEntityTypes(Type entityType)
        {
            var relatedTypes = new List<Type>();

            if (_model == null)
                return relatedTypes;

            IEntityType entityTypeMetadata = _model.FindEntityType(entityType);
            if (entityTypeMetadata == null)
                return relatedTypes;

            // Получаем навигационные свойства
            var navigationProperties = entityTypeMetadata.GetNavigations();

            foreach (var navigation in navigationProperties)
            {
                Type targetType = navigation.TargetEntityType.ClrType;
                if (targetType != null && !_selectedEntityTypes.Contains(targetType))
                {
                    relatedTypes.Add(targetType);
                }
            }

            // Также проверяем обратные связи
            var allEntityTypes = _model.GetEntityTypes();
            foreach (var et in allEntityTypes)
            {
                if (et.ClrType == entityType || et.ClrType == null)
                    continue;

                var navigations = et.GetNavigations();
                foreach (var nav in navigations)
                {
                    if (nav.TargetEntityType.ClrType == entityType && !_selectedEntityTypes.Contains(et.ClrType))
                    {
                        relatedTypes.Add(et.ClrType);
                    }
                }
            }

            return relatedTypes.Distinct().OrderBy(t => t.Name).ToList();
        }

        /// <summary>
        /// Получает путь выбранных сущностей
        /// </summary>
        private string GetEntityPath()
        {
            return string.Join(" -> ", _selectedEntityTypes.Select(t => t.Name));
        }

        /// <summary>
        /// Обновляет панель выбранных сущностей
        /// </summary>
        private void UpdateSelectedEntitiesPanel()
        {
            _selectedEntitiesPanel.Controls.Clear();

            foreach (var entityType in _selectedEntityTypes)
            {
                Panel entityPanel = new Panel
                {
                    Size = new Size(150, 80),
                    BorderStyle = BorderStyle.FixedSingle,
                    Margin = new Padding(5),
                    BackColor = Color.LightGray
                };

                Label nameLabel = new Label
                {
                    Text = entityType.Name,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Arial", 9, FontStyle.Bold)
                };

                Button removeButton = new Button
                {
                    Text = "×",
                    Dock = DockStyle.Top,
                    Height = 20,
                    BackColor = Color.LightCoral,
                    FlatStyle = FlatStyle.Flat
                };

                int index = _selectedEntityTypes.IndexOf(entityType);
                removeButton.Click += (s, e) => RemoveEntityAt(index);

                entityPanel.Controls.Add(nameLabel);
                entityPanel.Controls.Add(removeButton);
                _selectedEntitiesPanel.Controls.Add(entityPanel);
            }

            _doneButton.Enabled = _selectedEntityTypes.Count > 0;
        }

        /// <summary>
        /// Удаляет сущность по индексу
        /// </summary>
        private void RemoveEntityAt(int index)
        {
            if (index >= 0 && index < _selectedEntityTypes.Count)
            {
                _selectedEntityTypes.RemoveAt(index);
                UpdateSelectedEntitiesPanel();
                RefreshAvailableEntities();
            }
        }

        /// <summary>
        /// Обработчик двойного клика по списку доступных сущностей
        /// </summary>
        private void AvailableEntitiesListBox_DoubleClick(object sender, EventArgs e)
        {
            SelectCurrentEntity();
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Выбрать"
        /// </summary>
        private void SelectButton_Click(object sender, EventArgs e)
        {
            SelectCurrentEntity();
        }

        /// <summary>
        /// Выбирает текущую сущность из списка
        /// </summary>
        private void SelectCurrentEntity()
        {
            if (_availableEntitiesListBox.SelectedItem is EntityTypeItem item)
            {
                _selectedEntityTypes.Add(item.EntityType);
                UpdateSelectedEntitiesPanel();
                RefreshAvailableEntities();
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Сбросить"
        /// </summary>
        private void ResetButton_Click(object sender, EventArgs e)
        {
            _selectedEntityTypes.Clear();
            UpdateSelectedEntitiesPanel();
            RefreshAvailableEntities();
        }

        /// <summary>
        /// Обработчик нажатия кнопки "ГОТОВО"
        /// </summary>
        private void DoneButton_Click(object sender, EventArgs e)
        {
            if (_selectedEntityTypes.Count > 0)
            {
                // Создаем экземпляр TreeViewEntityBuilder с типом DbContext из контрола
                try
                {
                    // Получаем generic-тип DbContext из типа контрола
                    Type dbContextType = typeof(TDbContext);

                    // Создаем экземпляр DbContext
                    TDbContext dbContext = Activator.CreateInstance<TDbContext>();
                    if (dbContext == null)
                    {
                        MessageBox.Show($"Не удалось создать экземпляр {dbContextType.Name}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Создаем экземпляр TreeViewEntityBuilder
                    var builderType = typeof(TreeViewBuilder<>).MakeGenericType(dbContextType);
                    var builder = Activator.CreateInstance(builderType, dbContext);

                    if (builder == null)
                    {
                        MessageBox.Show("Не удалось создать экземпляр TreeViewEntityBuilder", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Получаем метод Join
                    var joinMethod = builderType.GetMethod("Join");
                    if (joinMethod == null)
                    {
                        MessageBox.Show("Метод Join не найден в TreeViewEntityBuilder", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Последовательно вызываем Join для каждого типа из списка
                    foreach (Type entityType in _selectedEntityTypes)
                    {
                        try
                        {
                            builder = joinMethod.Invoke(builder, new object[] { entityType });
                            if (builder == null)
                            {
                                MessageBox.Show($"Ошибка при добавлении сущности {entityType.Name}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка при добавлении сущности {entityType.Name}: {ex.InnerException?.Message ?? ex.Message}",
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    // Вызываем событие с результатом
                    ConfigurationCompleted?.Invoke(this, new EntityTypesSelectedEventArgs(_selectedEntityTypes));

                    var treeView = (TreeView)builder.Invoke("Build", new object[] { });



                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при создании построителя: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }

    /// <summary>
    /// Класс для хранения информации о типе сущности в списке
    /// </summary>
    internal class EntityTypeItem
    {
        public Type EntityType { get; }

        public EntityTypeItem(Type entityType)
        {
            EntityType = entityType;
        }

        public override string ToString()
        {
            return EntityType.Name;
        }
    }

    /// <summary>
    /// Аргументы события выбора типов сущностей
    /// </summary>
    public class EntityTypesSelectedEventArgs : EventArgs
    {
        public List<Type> EntityTypes { get; }

        public EntityTypesSelectedEventArgs(List<Type> entityTypes)
        {
            EntityTypes = new List<Type>(entityTypes);
        }
    }
}

