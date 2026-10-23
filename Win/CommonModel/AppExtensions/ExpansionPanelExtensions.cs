using MaterialSkin.Controls;

namespace WinFormsApp.AppExtensions
{
    /// <summary>
    /// Вспомогательный класс для работы с MaterialExpansionPanel
    /// </summary>
    public static class ExpansionPanelExtensions
    {
        /// <summary>
        /// Раскрывает MaterialExpansionPanel программно
        /// </summary>
        /// <param name="panel">Панель для раскрытия</param>
        public static void Expand(this MaterialExpansionPanel panel)
        {
            if (panel == null) return;

            // В MaterialSkin используется свойство Expanded или IsExpanded
            // Пробуем оба варианта через рефлексию для совместимости
            var expandedProperty = panel.GetType().GetProperty("Expanded");
            if (expandedProperty != null && expandedProperty.PropertyType == typeof(bool))
            {
                expandedProperty.SetValue(panel, true);
                return;
            }

            var isExpandedProperty = panel.GetType().GetProperty("IsExpanded");
            if (isExpandedProperty != null && isExpandedProperty.PropertyType == typeof(bool))
            {
                isExpandedProperty.SetValue(panel, true);
                return;
            }

            // Если свойства не найдены, пробуем вызвать метод Expand
            var expandMethod = panel.GetType().GetMethod("Expand");
            if (expandMethod != null)
            {
                expandMethod.Invoke(panel, null);
            }
        }

        /// <summary>
        /// Сворачивает MaterialExpansionPanel программно
        /// </summary>
        /// <param name="panel">Панель для сворачивания</param>
        public static void Collapse(this MaterialExpansionPanel panel)
        {
            if (panel == null) return;

            var expandedProperty = panel.GetType().GetProperty("Expanded");
            if (expandedProperty != null && expandedProperty.PropertyType == typeof(bool))
            {
                expandedProperty.SetValue(panel, false);
                return;
            }

            var isExpandedProperty = panel.GetType().GetProperty("IsExpanded");
            if (isExpandedProperty != null && isExpandedProperty.PropertyType == typeof(bool))
            {
                isExpandedProperty.SetValue(panel, false);
                return;
            }

            var collapseMethod = panel.GetType().GetMethod("Collapse");
            if (collapseMethod != null)
            {
                collapseMethod.Invoke(panel, null);
            }
        }

        /// <summary>
        /// Переключает состояние MaterialExpansionPanel (раскрыть/свернуть)
        /// </summary>
        /// <param name="panel">Панель для переключения</param>
        public static void Toggle(this MaterialExpansionPanel panel)
        {
            if (panel == null) return;

            bool isExpanded = panel.IsExpanded();
            if (isExpanded)
            {
                panel.Collapse();
            }
            else
            {
                panel.Expand();
            }
        }

        /// <summary>
        /// Проверяет, раскрыта ли панель
        /// </summary>
        /// <param name="panel">Панель для проверки</param>
        /// <returns>true, если панель раскрыта, иначе false</returns>
        public static bool IsExpanded(this MaterialExpansionPanel panel)
        {
            if (panel == null) return false;

            var expandedProperty = panel.GetType().GetProperty("Expanded");
            if (expandedProperty != null && expandedProperty.PropertyType == typeof(bool))
            {
                return (bool)expandedProperty.GetValue(panel);
            }

            var isExpandedProperty = panel.GetType().GetProperty("IsExpanded");
            if (isExpandedProperty != null && isExpandedProperty.PropertyType == typeof(bool))
            {
                return (bool)isExpandedProperty.GetValue(panel);
            }

            return false;
        }
    }
}

