using MaterialSkin.Controls;

using Namotion.Reflection;


/// <summary>
/// Методы расширения упрощающие работу с WinForms.
/// </summary>
public static class WinFormsExtensions
{
    /// <summary>
    /// Делает Panel принимающей перетаскиваемые объекты
    /// </summary>
    /// <param name="panel">Панель, которую нужно сделать Droppable</param>
    /// <param name="onDropHandler">Обработчик события Drop (возвращает bool - успех операции)</param>
    public static void SetDroppable(this Control _dropSurface, Action<string> doDrop)
    {


        _dropSurface.DragEnter += (_, e) =>
        {
            e.Effect = DragDropEffects.Copy;
        };

        _dropSurface.DragOver += (_, e) =>
        {
            e.Effect = DragDropEffects.Copy;
        };

        _dropSurface.DragDrop += (_, e) =>
        {
            if (!e.Data.GetDataPresent(Enum.GetName<DataFormats>(DataFormats.Text)))
                return;

            var text = e.Data.GetData(Enum.GetName< DataFormats > (DataFormats.Text)) as string;
            if (string.IsNullOrWhiteSpace(text))
                return;
            doDrop(text);

        };
    }


    public static void Trace(this Control control)
        => control.Trace(1);
    public static void Trace(this Control control, int level)
    {
        if (control is null)
            return;
        for (int i = 0; i < level; i++)
            System.Console.Write("   ");
        System.Console.WriteLine(control.GetType().GetTypeName() +
            $"{(control.HasProperty("Text") ? control.GetProperty("Text") : "")}" + " " + control.Dock);
        foreach (Control ctrl in control.Controls)
        {
            ctrl.Trace(level + 1);
        }
    }

    public static Control SetDock(this Control control, [NotNullNotEmpty] string value)
    {
        if (String.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(nameof(value));
        value = value.ToLower();
        switch (value)
        {
            case "top": control.Dock = DockStyle.Top; break;
            case "left": control.Dock = DockStyle.Left; break;
            case "bottom": control.Dock = DockStyle.Bottom; break;
            case "right": control.Dock = DockStyle.Right; break;
            case "fill": control.Dock = DockStyle.Fill; break;
            case "none": control.Dock = DockStyle.None; break;
            default: throw new NotImplementedException($"Dock не поддерживает значение {value}");
        }
        return control;
    }


    /// <summary>
    /// Cursors.SizeNS - вверх-вниз
    /// </summary>
    /// <param name="ctrl"></param>
    /// <param name="Cursor"></param>
    public static void SetCursor(this Control ctrl, string Cursor)
    {
        switch (Cursor)
        {
            case "wait": ctrl.Cursor = Cursors.WaitCursor; break;
            case "w-resize": ctrl.Cursor = Cursors.SizeWE; break;
            case "col-resize": ctrl.Cursor = Cursors.SizeNS; break;
            case "grabbing": ctrl.Cursor = Cursors.SizeNS; break;
            case "sw-resize": ctrl.Cursor = Cursors.SizeWE; break;
            case "nw-resize": ctrl.Cursor = Cursors.SizeNS; break;
            case "ne-resize": ctrl.Cursor = Cursors.SizeNS; break;
            case "se-resize": ctrl.Cursor = Cursors.SizeNS; break;
            case "e-resize": ctrl.Cursor = Cursors.SizeNS; break;
            case "s-resize": ctrl.Cursor = Cursors.SizeNS; break;
            case "n-resize": ctrl.Cursor = Cursors.SizeNS; break;
            case "inherit": ctrl.Cursor = Cursors.Default; break;
            default: ctrl.Cursor = Cursors.Default; break;
        }
    }


    public static void ReplaceControl(this Control container, Control removing, Control adding)
    {
        List<Control> ctrls = new();
        foreach (Control control in container.Controls)
        {
            if (control == removing)
            {
                ctrls.Add(adding);
            }
            else
            {
                ctrls.Add(control);
            }
        }
        container.Controls.Clear();
        container.Controls.AddRange(ctrls.ToArray());
    }

    /// <summary>
    /// Запуск элемента на WinForm
    /// </summary>    
    public static string RunAtTestForm(this Control ctrl)
    {
        try
        {
            var form = ctrl.ToBaseForm();
            form.Load += (s, e) =>
            {
                form.Close();
            };
            form.Run();
            return null;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    public static void RunAtForm(this Control ctrl, string title) =>
        ctrl.ToBaseForm(title).Run();

    public static Form RunAtForm(this Control ctrl)
    {
        if (ctrl is Form)
        {
            ((Form)ctrl).Run();
            return ((Form)ctrl);
        }
        else
        {
            var form = ctrl.ToBaseForm();
            form.Width = ctrl.Width > 0 ? ctrl.Width : 640;
            form.Height = ctrl.Width > 0 ? ctrl.Height : 480;
            form.Run();
            return form;
        }

    }

    public static Form Main;

    public static Form Run(this Form form)
    {
        form.KeyPreview = true;

        System.Windows.Forms.Application.Run(form);
        return form;
    }


    /// <summary>
    /// Получение элементов управления списком
    /// </summary>
    public static List<Control> GetControlList(this Control p)
    {
        List<Control> controls = new();
        foreach (Control c in p.Controls)
        {
            controls.Add(c);
        }
        return controls;
    }

    public static Button GetButtont(this Control p, string text)
    {
        return (Button)p.GetControlList().FirstOrDefault(c => ((c is Button) || (c is MaterialButton)) && c.Text.ToLower() == $"{text}".ToLower());
    }


    /// <summary>
    /// Создание панели элементов
    /// </summary>
    public static Panel ToPanel(this IEnumerable<Control> list)
    {
        var p = new Panel();
        list.ToList().ForEach(p.Controls.Add);
        return p;
    }
}
 