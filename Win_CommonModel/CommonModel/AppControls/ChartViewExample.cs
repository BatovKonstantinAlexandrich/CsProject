using System;
using System.Drawing;
using System.Windows.Forms;

using ScottPlot;
using ScottPlot.WinForms;

using WinFormsApp.AppViewModules.ChartModule;

namespace WinFormsApp.AppComponents.AppControls
{
    /// <summary>
    /// Пример использования ScottPlot для создания диаграмм
    /// </summary>
    public class ChartViewExample : Form
    {
        private FormsPlot _plot;

        public ChartViewExample()
        {
            InitializeComponent();
            CreateSampleCharts();
        }

        private void InitializeComponent()
        {
            Text = "Пример диаграмм ScottPlot";
            Size = new Size(1200, 800);
            StartPosition = FormStartPosition.CenterScreen;

            // Создаем диаграмму
            _plot = ChartViewHelper.CreateChart("Пример визуализации данных", "Ось X", "Ось Y");
            Controls.Add(_plot);
        }

        private void CreateSampleCharts()
        {
            // Пример 1: Линейный график
            double[] xData = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            double[] yData1 = { 1, 4, 9, 16, 25, 36, 49, 64, 81, 100 };
            double[] yData2 = { 2, 5, 10, 17, 26, 37, 50, 65, 82, 101 };

            ChartViewHelper.AddLineChart(_plot, xData, yData1, "Квадратичная функция", Color.Blue, 2, 5);
            ChartViewHelper.AddLineChart(_plot, xData, yData2, "Квадратичная + 1", Color.Red, 2, 5);

            // Пример 2: Столбчатая диаграмма
            double[] barValues = { 10, 25, 15, 30, 20 };
            string[] barLabels = { "Янв", "Фев", "Мар", "Апр", "Май" };
            ChartViewHelper.AddBarChart(_plot, barValues, barLabels, "Продажи", Color.Green);

            // Настройка сетки и легенды
            ChartViewHelper.SetGrid(_plot, true, Color.LightGray);
            ChartViewHelper.SetLegend(_plot, true);
        }

        /// <summary>
        /// Статический метод для быстрого создания и отображения примера
        /// </summary>
        public static void ShowExample()
        {
            Application.Run(new ChartViewExample());
        }
    }
}

