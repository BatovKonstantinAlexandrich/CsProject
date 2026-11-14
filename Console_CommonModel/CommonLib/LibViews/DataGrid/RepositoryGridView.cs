using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json.Serialization;
using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibData.RepositoryPattern;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibViews.DataGrid
{



    /// <summary>
    /// Модель AppViewBase для редактирования GridView с поддержкой различных типов ячеек и связыванием с Repository
    /// </summary>
    [Label("Редактируемая таблица")]
    [Description("Модель представления для редактирования данных в табличном формате с поддержкой различных типов ячеек и связыванием с Repository")]
    public class RepositoryGridView : AppViewBase
    {
        /// <summary>
        /// Заголовок таблицы
        /// </summary>

        [Label("Title")]
        public string Title { get => Get<string>("Title"); set => Set("Title", value); }

        /// <summary>
        /// Конфигурации колонок (имя колонки -> конфигурация)
        /// </summary>

        [Label("Column Configs")]
        public Dictionary<string, GridViewColumnConfig> ColumnConfigs { get; set; } = new Dictionary<string, GridViewColumnConfig>();

        /// <summary>
        /// Видимые колонки
        /// </summary>

        [Label("Visible Columns")]
        public ChangingList<string> VisibleColumns { get; set; } = new ChangingList<string>();

        /// <summary>
        /// Данные таблицы
        /// </summary>

        [Label("Data Rows")]
        public ChangingList<object> DataRows { get; set; } = new ChangingList<object>();

        /// <summary>
        /// Тип сущности (для связывания с Repository)
        /// </summary>

        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [Label("Entity Type")]
        public Type EntityType { get; set; }

        /// <summary>
        /// Тип контекста базы данных (для связывания с Repository)
        /// </summary>
        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]


        [Label("Db Context Type")]
        public Type DbContextType { get; set; }

        /// <summary>
        /// Репозиторий для работы с данными
        /// </summary>
        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]


        [Label("Repository")]
        public object Repository { get; set; }

        /// <summary>
        /// Событие добавления строки
        /// </summary>

        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [Label("On Row Added")]
        public Action<int, object> OnRowAdded { get; set; }

        /// <summary>
        /// Событие удаления строки
        /// </summary>

        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [Label("On Row Removed")]
        public Action<int, object> OnRowRemoved { get; set; }

        /// <summary>
        /// Событие изменения строки
        /// </summary>

        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [Label("On Row Changed")]
        public Action<int, int, object, object> OnRowChanged { get; set; }

        /// <summary>
        /// Событие изменения значения ячейки
        /// </summary>

        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [Label("On Cell Value Changed")]
        public Action<int, int, string, object, object> OnCellValueChanged { get; set; }

        /// <summary>
        /// Событие выбора строки
        /// </summary>
        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]


        [Label("On Row Selected")]
        public Action<int, object> OnRowSelected { get; set; }

        /// <summary>
        /// Автоматически сохранять изменения в Repository
        /// </summary>
        [InputBool]
        [Label("Auto Save")]
        public bool AutoSave { get => Get<bool>("AutoSave"); set => Set("AutoSave", value); }

        /// <summary>
        /// Показывать кнопки добавления/удаления
        /// </summary>
        [InputBool]
        [Label("Show Add Remove Buttons")]
        public bool ShowAddRemoveButtons { get => Get<bool>("ShowAddRemoveButtons"); set => Set("ShowAddRemoveButtons", value); }

        public RepositoryGridView() : base()
        {
            Init();
            Title = "Редактируемая таблица";
            AutoSave = true;
            ShowAddRemoveButtons = true;
            DataRows.OnChanged += OnDataRowsChanged;
            VisibleColumns.OnChanged += OnVisibleColumnsChanged;
        }

        /// <summary>
        /// Добавить конфигурацию колонки
        /// </summary>
        public void AddColumnConfig(string columnName, GridViewCellType cellType, string headerText = null, bool readOnly = false)
        {
            var config = new GridViewColumnConfig
            {
                Name = columnName,
                HeaderText = headerText ?? columnName,
                CellType = cellType,
                ReadOnly = readOnly
            };

            ColumnConfigs[columnName] = config;

            if (!VisibleColumns.Contains(columnName))
            {
                VisibleColumns.Add(columnName);
            }
        }

        /// <summary>
        /// Добавить конфигурацию колонки с полной настройкой
        /// </summary>
        public void AddColumnConfig(GridViewColumnConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            ColumnConfigs[config.Name] = config;

            if (config.Visible && !VisibleColumns.Contains(config.Name))
            {
                VisibleColumns.Add(config.Name);
            }
        }

        /// <summary>
        /// Получить конфигурацию колонки
        /// </summary>
        public GridViewColumnConfig GetColumnConfig(string columnName)
        {
            return ColumnConfigs.ContainsKey(columnName) ? ColumnConfigs[columnName] : null;
        }

        /// <summary>
        /// Добавить строку в таблицу
        /// </summary>
        public void AddRow(object rowData = null)
        {
            if (rowData == null && EntityType != null)
            {
                // Создаем новую сущность через рефлексию
                rowData = Activator.CreateInstance(EntityType);
            }

            if (rowData != null)
            {
                int index = DataRows.Count;
                DataRows.Add(rowData);
                OnRowAdded?.Invoke(index, rowData);

                // Автоматически сохраняем в Repository, если включено
                if (AutoSave && Repository != null && EntityType != null)
                {
                    SaveRowToRepository(rowData);
                }
            }
        }

        /// <summary>
        /// Удалить строку из таблицы
        /// </summary>
        public void RemoveRow(int rowIndex)
        {
            if (rowIndex >= 0 && rowIndex < DataRows.Count)
            {
                var rowData = DataRows[rowIndex];
                DataRows.RemoveAt(rowIndex);
                OnRowRemoved?.Invoke(rowIndex, rowData);

                // Автоматически удаляем из Repository, если включено
                if (AutoSave && Repository != null && EntityType != null)
                {
                    RemoveRowFromRepository(rowData);
                }
            }
        }

        /// <summary>
        /// Удалить строку по данным
        /// </summary>
        public void RemoveRow(object rowData)
        {
            int index = DataRows.IndexOf(rowData);
            if (index >= 0)
            {
                RemoveRow(index);
            }
        }

        /// <summary>
        /// Обновить строку в таблице
        /// </summary>
        public void UpdateRow(int rowIndex, object rowData)
        {
            if (rowIndex >= 0 && rowIndex < DataRows.Count && rowData != null)
            {
                var oldData = DataRows[rowIndex];
                DataRows[rowIndex] = rowData;
                OnRowChanged?.Invoke(rowIndex, -1, oldData, rowData);

                // Автоматически обновляем в Repository, если включено
                if (AutoSave && Repository != null && EntityType != null)
                {
                    UpdateRowInRepository(rowData);
                }
            }
        }

        /// <summary>
        /// Обновить значение ячейки
        /// </summary>
        public void UpdateCell(int rowIndex, int columnIndex, object value)
        {
            if (rowIndex >= 0 && rowIndex < DataRows.Count)
            {
                var rowData = DataRows[rowIndex];
                var columnName = VisibleColumns[columnIndex];
                var oldValue = GetCellValue(rowData, columnName);

                SetCellValue(rowData, columnName, value);

                OnCellValueChanged?.Invoke(rowIndex, columnIndex, columnName, oldValue, value);
                OnRowChanged?.Invoke(rowIndex, columnIndex, rowData, rowData);

                // Автоматически обновляем в Repository, если включено
                if (AutoSave && Repository != null && EntityType != null)
                {
                    UpdateRowInRepository(rowData);
                }
            }
        }

        /// <summary>
        /// Обновить значение ячейки по имени колонки
        /// </summary>
        public void UpdateCell(int rowIndex, string columnName, object value)
        {
            int columnIndex = VisibleColumns.IndexOf(columnName);
            if (columnIndex >= 0)
            {
                UpdateCell(rowIndex, columnIndex, value);
            }
        }

        /// <summary>
        /// Получить значение ячейки
        /// </summary>
        public object GetCellValue(object rowData, string columnName)
        {
            if (rowData == null || string.IsNullOrEmpty(columnName))
                return null;

            var property = rowData.GetType().GetProperty(columnName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (property != null)
            {
                return property.GetValue(rowData);
            }

            return null;
        }

        /// <summary>
        /// Установить значение ячейки
        /// </summary>
        public void SetCellValue(object rowData, string columnName, object value)
        {
            if (rowData == null || string.IsNullOrEmpty(columnName))
                return;

            var property = rowData.GetType().GetProperty(columnName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (property != null && property.CanWrite)
            {
                try
                {
                    // Преобразуем значение к типу свойства
                    if (value != null && property.PropertyType != value.GetType())
                    {
                        if (property.PropertyType.IsEnum)
                        {
                            value = Enum.Parse(property.PropertyType, value.ToString());
                        }
                        else if (property.PropertyType == typeof(bool) && value is string)
                        {
                            value = bool.Parse(value.ToString());
                        }
                        else if (property.PropertyType == typeof(int) && value is string)
                        {
                            value = int.Parse(value.ToString());
                        }
                        else if (property.PropertyType == typeof(decimal) && value is string)
                        {
                            value = decimal.Parse(value.ToString());
                        }
                        else if (property.PropertyType == typeof(DateTime) && value is string)
                        {
                            value = DateTime.Parse(value.ToString());
                        }
                        else
                        {
                            value = System.Convert.ChangeType(value, property.PropertyType);
                        }
                    }

                    property.SetValue(rowData, value);
                }
                catch (Exception ex)
                {
                    this.Error($"Ошибка установки значения {columnName}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Связать с Repository
        /// </summary>
        public void BindToRepository<TDbContext, TEntity>(IRepository<TEntity> repository = null)
            where TDbContext : DbContextBase
            where TEntity : BaseEntity
        {
            EntityType = typeof(TEntity);
            DbContextType = typeof(TDbContext);

            if (repository == null)
            {
                // Создаем репозиторий через рефлексию
                var repositoryType = typeof(Repository<,>).MakeGenericType(typeof(TEntity), typeof(TDbContext));
                Repository = Activator.CreateInstance(repositoryType);
            }
            else
            {
                Repository = repository;
            }

            // Загружаем данные из Repository
            LoadDataFromRepository();
        }

        /// <summary>
        /// Загрузить данные из Repository
        /// </summary>
        public void LoadDataFromRepository()
        {
            if (Repository == null || EntityType == null)
                return;

            try
            {
                // Вызываем метод GetAll через рефлексию
                var getAllMethod = Repository.GetType().GetMethod("GetAll");
                if (getAllMethod != null)
                {
                    var data = getAllMethod.Invoke(Repository, null) as System.Collections.IEnumerable;
                    if (data != null)
                    {
                        DataRows.Clear();
                        foreach (var item in data)
                        {
                            DataRows.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.Error($"Ошибка загрузки данных из Repository: {ex.Message}");
            }
        }

        /// <summary>
        /// Сохранить строку в Repository
        /// </summary>
        private void SaveRowToRepository(object rowData)
        {
            if (Repository == null || rowData == null)
                return;

            try
            {
                var addMethod = Repository.GetType().GetMethod("Add");
                if (addMethod != null)
                {
                    addMethod.Invoke(Repository, new[] { rowData });

                    var saveMethod = Repository.GetType().GetMethod("Save");
                    saveMethod?.Invoke(Repository, null);
                }
            }
            catch (Exception ex)
            {
                this.Error($"Ошибка сохранения строки в Repository: {ex.Message}");
            }
        }

        /// <summary>
        /// Обновить строку в Repository
        /// </summary>
        private void UpdateRowInRepository(object rowData)
        {
            if (Repository == null || rowData == null)
                return;

            try
            {
                var updateMethod = Repository.GetType().GetMethod("Update");
                if (updateMethod != null)
                {
                    updateMethod.Invoke(Repository, new[] { rowData });

                    var saveMethod = Repository.GetType().GetMethod("Save");
                    saveMethod?.Invoke(Repository, null);
                }
            }
            catch (Exception ex)
            {
                this.Error($"Ошибка обновления строки в Repository: {ex.Message}");
            }
        }

        /// <summary>
        /// Удалить строку из Repository
        /// </summary>
        private void RemoveRowFromRepository(object rowData)
        {
            if (Repository == null || rowData == null)
                return;

            try
            {
                var removeMethod = Repository.GetType().GetMethod("Remove");
                if (removeMethod != null)
                {
                    removeMethod.Invoke(Repository, new[] { rowData });

                    var saveMethod = Repository.GetType().GetMethod("Save");
                    saveMethod?.Invoke(Repository, null);
                }
            }
            catch (Exception ex)
            {
                this.Error($"Ошибка удаления строки из Repository: {ex.Message}");
            }
        }

        /// <summary>
        /// Сохранить все изменения в Repository
        /// </summary>
        public void SaveAll()
        {
            if (Repository == null)
                return;

            try
            {
                var saveMethod = Repository.GetType().GetMethod("Save");
                saveMethod?.Invoke(Repository, null);
            }
            catch (Exception ex)
            {
                this.Error($"Ошибка сохранения всех изменений: {ex.Message}");
            }
        }

        /// <summary>
        /// Обновить данные из Repository
        /// </summary>
        public void Refresh()
        {
            LoadDataFromRepository();
        }

        private void OnDataRowsChanged(object sender, ListChangedEvent e)
        {
            // Обработка изменения данных
        }

        private void OnVisibleColumnsChanged(object sender, ListChangedEvent e)
        {
            // Обработка изменения видимых колонок
        }
    }
}



