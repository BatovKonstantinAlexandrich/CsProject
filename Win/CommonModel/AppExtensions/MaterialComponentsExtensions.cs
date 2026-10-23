using MaterialSkin;
using MaterialSkin.Controls;
using MaterialSurface;

/// <summary>
/// Методы расширения использующие библиотку MaterialSkin
/// </summary>
public static class MaterialComponentsExtensions
{


    /// <summary>
    /// Показать панель выдвигающуюся панель снизу
    /// </summary>
    public static void ShowSnakBar(this System.Windows.Forms.Form control, string text, string button = "ок", int timeout=5000)
    {
        Snackbar.MakeSnackbar(control, text, button, timeout);
    }


    /// <summary>
    /// Включение режима перемещения элементов списка
    /// </summary>
    public static void EnableDragAndDropPosition(this MaterialCheckedListBox checkList, Action<int,int> onchanged )
    {
        checkList.AllowDrop = true;
        object SelectedItem = null;
        checkList.MouseDown += (s, e) => {
            checkList.DoDragDrop(SelectedItem, DragDropEffects.Move);
        };

        foreach (var checkbox in checkList.Items)
        {
            var pcheckbox = checkbox;
            checkbox.AllowDrop = true;
            checkbox.MouseDown += (s, e) => {

                SelectedItem = checkbox;
                Clipboard.SetText(checkbox.Text, TextDataFormat.UnicodeText);
                pcheckbox.SkinManager.Theme = MaterialSkinManager.Themes.LIGHT;

                // Configure color schema
                pcheckbox.SkinManager.ColorScheme = new ColorScheme(
                    Primary.Blue400, Primary.Blue500,
                    Primary.Blue500, Accent.LightBlue200,
                    TextShade.WHITE
                );
            };
            checkbox.DragEnter += (s, e) =>
            {
                checkbox.BackColor = Color.Black;
            };
            checkbox.DragDrop += (s, e) =>
            {
                var text=Clipboard.GetText();
                var cur = checkList.Items.FirstOrDefault(p => p.Text == text);
                var items = checkList.Items.Where(item => item != cur).ToList();
                checkList.Items.Clear();
                foreach (var next in items)
                {
                    if(next == pcheckbox)
                    {
                        checkList.Items.Add(cur);
                    }
                    checkList.Items.Add(next);

                }
            };
            checkbox.MouseUp += (s, e) => { 
            };
        }
    }
}