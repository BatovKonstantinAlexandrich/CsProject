using MaterialSkin;
using MaterialSkin.Controls;

using System.Reflection;

using WinFormsApp.Console.Win_MaterialModule.ThemeModule;

namespace WinFormsApp.AppComponents.AppControls
{
    /// <summary>
    /// Поддерживает управление стилями Material
    /// </summary>
    public class ColorStyleForm : MaterialForm
    {
        private readonly MaterialSkinManager materialSkinManager;

        public ColorStyleForm()
        {
            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
        }


        /// <summary>
        /// Связывание параметров стилизации
        /// </summary>        
        public void BindColorStyle([NotNullNotEmpty] MatColorStyle style)
        {
            var p = this;
            MethodBase.GetCurrentMethod().EnsureIsValid(new object[] { style });
            style.OnChange += (evt) =>
            {
                p.UpdateColorStyle(style);
            };
            UpdateColorStyle(style);
        }


        /// <summary>
        /// Применение параметров стилизации
        /// </summary>        
        private void UpdateColorStyle([NotNullNotEmpty] MatColorStyle style)
        {
            MethodBase.GetCurrentMethod().EnsureIsValid(new object[] { style });

            materialSkinManager.Theme = materialSkinManager.Theme == MaterialSkinManager.Themes.LIGHT ? MaterialSkinManager.Themes.DARK : MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(
                Enum.Parse<Primary>(style.primary),
                Enum.Parse<Primary>(style.darkPrimary),
                Enum.Parse<Primary>(style.lightPrimary),
                Enum.Parse<Accent>(style.accent),
                Enum.Parse<TextShade>(style.textShade)
            );
            materialSkinManager.Theme = materialSkinManager.Theme == MaterialSkinManager.Themes.LIGHT ? MaterialSkinManager.Themes.DARK : MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.Theme = Enum.Parse<MaterialSkinManager.Themes>(style.theme);
        }


        /// <summary>
        /// Выполнение проверочного кода
        /// </summary>
        public static void Run()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var style = new MatColorStyle();
            var form = new ColorStyleForm();
            form.BindColorStyle(style);
            var factory = new ControlsFactory();
            form.LayoutFill(factory.CreateButton("Свойства", () =>
            {
                var ctrl = new InputPanel(style, style.GetType(), (s, e) =>
                {
                    if (s)
                    {
                        style.SetTextValues(e.ToTextDictionary());
                    }
                }, style.GetType().GetPropertyNames<MatColorStyle>());
                form.LayoutRight(ctrl);
            }));
            Application.Run(form);

        }
    }
}
