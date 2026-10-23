using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibData.Console_DataModule;

using System;
using System.Collections.Generic;
using System.Linq;


namespace Console_CommonModel.CommonLib.LibViews.DataGrid
{
    /// <summary>
    /// Пример использования EditableGridView
    /// </summary>
    public class RepositoryGridViewExample
    {
        /// <summary>
        /// Пример создания EditableGridView с настройкой колонок
        /// </summary>
        public static RepositoryGridView CreateExample()
        {
            var gridView = new RepositoryGridView
            {
                Title = "Управление продуктами"
            };

            // Настраиваем типы колонок
            gridView.AddColumnConfig("Name", GridViewCellType.TextBox, "Название", readOnly: false);
            gridView.AddColumnConfig("Price", GridViewCellType.Numeric, "Цена", readOnly: false);

            gridView.AddColumnConfig("Category", GridViewCellType.ComboBox, "Категория", readOnly: false);
            gridView.AddColumnConfig("InStock", GridViewCellType.CheckBox, "В наличии", readOnly: false);
            gridView.AddColumnConfig("DateCreated", GridViewCellType.DateTimePicker, "Дата создания", readOnly: false);
            gridView.AddColumnConfig("Link", GridViewCellType.Link, "Ссылка", readOnly: true);
            gridView.AddColumnConfig("Action", GridViewCellType.Button, "Действие", readOnly: true);

            // Настраиваем ComboBox для категории
            var categoryConfig = gridView.GetColumnConfig("Category");
            if (categoryConfig != null)
            {
                categoryConfig.ComboBoxItems = new List<string> { "Электроника", "Одежда", "Продукты", "Книги" };
            }

            // Настраиваем Button
            var actionConfig = gridView.GetColumnConfig("Action");
            if (actionConfig != null)
            {
                actionConfig.ButtonText = "Редактировать";
            }

            // Настраиваем Numeric для цены
            var priceConfig = gridView.GetColumnConfig("Price");
            if (priceConfig != null)
            {
                priceConfig.NumericMin = 0;
                priceConfig.NumericMax = 100000;
                priceConfig.NumericIncrement = 0.01m;
            }

            // Подписываемся на события
            gridView.OnRowAdded += (index, rowData) =>
            {
                gridView.Info($"Добавлена строка {index}: {rowData}");
            };

            gridView.OnRowRemoved += (index, rowData) =>
            {
                gridView.Info($"Удалена строка {index}: {rowData}");
            };

            gridView.OnCellValueChanged += (rowIndex, columnIndex, columnName, oldValue, newValue) =>
            {
                gridView.Info($"Изменена ячейка [{rowIndex}, {columnName}]: {oldValue} -> {newValue}");
            };

            gridView.OnRowChanged += (rowIndex, columnIndex, oldData, newData) =>
            {
                gridView.Info($"Изменена строка {rowIndex}");
            };

            return gridView;
        }

        /// <summary>
        /// Пример связывания с Repository
        /// </summary>
        public static RepositoryGridView CreateWithRepository<TDbContext, TEntity>()
            where TDbContext : DbContextBase
            where TEntity : BaseEntity
        {
            var gridView = new RepositoryGridView
            {
                Title = $"Управление {typeof(TEntity).Name}",
                AutoSave = true
            };

            // Автоматически определяем колонки из свойств сущности
            var properties = typeof(TEntity).GetProperties()
                .Where(p => p.CanRead && p.CanWrite && !p.Name.Equals("Id"))
                .ToList();

            foreach (var property in properties)
            {
                GridViewCellType cellType = DetermineCellType(property.PropertyType);
                gridView.AddColumnConfig(property.Name, cellType, property.Name, readOnly: false);
            }

            // Связываем с Repository
            gridView.BindToRepository<TDbContext, TEntity>();

            return gridView;
        }

        /// <summary>
        /// Пример связывания с Repository
        /// </summary>
        public static RepositoryGridView CreateWithRepositoryBase(Type TDbContext, Type TEntity)
        {
            var gridView = new RepositoryGridView
            {
                Title = $"Управление {TEntity.Name}",
                AutoSave = true
            };

            // Автоматически определяем колонки из свойств сущности
            var properties = TEntity.GetProperties()
                .Where(p => p.CanRead && p.CanWrite && !p.Name.Equals("Id"))
                .ToList();

            foreach (var property in properties)
            {
                GridViewCellType cellType = DetermineCellType(property.PropertyType);
                gridView.AddColumnConfig(property.Name, cellType, property.Name, readOnly: false);
            }

            // Связываем с Repository
            //gridView.BindToRepository<TDbContext, TEntity>();
            gridView.InvokeGenericMethod(nameof(gridView.BindToRepository), new List<Type>() { TDbContext, TEntity }, new List<object>());

            return gridView;
        }


        /// <summary>
        /// Определяет тип ячейки на основе типа свойства
        /// </summary>
        private static GridViewCellType DetermineCellType(Type propertyType)
        {
            if (propertyType == typeof(bool) || propertyType == typeof(bool?))
                return GridViewCellType.CheckBox;

            if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
                return GridViewCellType.DateTimePicker;

            if (propertyType == typeof(int) || propertyType == typeof(int?) ||
                propertyType == typeof(decimal) || propertyType == typeof(decimal?) ||
                propertyType == typeof(double) || propertyType == typeof(double?) ||
                propertyType == typeof(float) || propertyType == typeof(float?))
                return GridViewCellType.Numeric;

            //if (propertyType == typeof(byte[]) || propertyType == typeof(Image))
            //return GridViewCellType.Image;

            return GridViewCellType.TextBox;
        }

        /// <summary>
        /// Пример использования с данными
        /// </summary>
        public static void ExampleWithData()
        {
            var gridView = CreateExample();

            // Добавляем данные
            var product1 = new { Name = "Товар 1", Price = 100.50m, Category = "Электроника", InStock = true, DateCreated = DateTime.Now, Link = "http://example.com" };
            var product2 = new { Name = "Товар 2", Price = 200.75m, Category = "Одежда", InStock = false, DateCreated = DateTime.Now.AddDays(-1), Link = "http://example2.com" };

            gridView.AddRow(product1);
            gridView.AddRow(product2);

            // Обновляем ячейку
            gridView.UpdateCell(0, "Price", 150.00m);

            // Удаляем строку
            // gridView.RemoveRow(1);
        }
    }
}



