using MaterialSkin.Controls;

public static class ContentExtensions
{
    public static MaterialLabel AddTitle(this Control control, string text)
    {
        var factory = new ControlsFactory();
        var result = factory.CreateLabel(text, "Roboto", 16);
        control.LayoutTop(result);
        return result;
    }

    public static MaterialLabel AddTitle(this Control control, Type entityType)
    {
        var factory = new ControlsFactory();
        var result = factory.CreateLabel(entityType.GetLabel(), "Roboto", 16);
        control.LayoutTop(result);
        return result;
    }


    public static Control Edit(this Control control, object target)
    {
        return new ControlsFactory().Edit(target);
    }
    public static Panel AddSubmitButtons(this Control control, Action<bool> oncomplete)
    {
        var factory = new ControlsFactory();
        var panel = new Panel();
        panel.LayoutLeft(factory.CreateButton("Выполнить", () => {
            oncomplete(true);
        }));
        panel.AddFill(new Panel());
        panel.LayoutLeft(factory.CreateButton("Отмена", () => {
            oncomplete(false);
        }));
        control.LayoutBottom(panel);
        return panel;
    }

    public static Dictionary<string, Button> AddButtons(this Control control, IEnumerable<string> labels, Action<string> oncomplete)
    {
        var factory = new ControlsFactory();
        var panel = new FlowLayoutPanel() { FlowDirection = FlowDirection.LeftToRight, Height = 50 };
        var result = new Dictionary<string, Button>();
        foreach (var label in labels)
        {
            var message = label;
            result[label] = factory.CreateButton(label, () =>
            {
                oncomplete(message);
            });
            result[label].Margin = new(2);
            panel.Controls.Add(result[label]);
        }         
        control.LayoutTop(panel);
        panel.Height = 50;
        return result;
    }

    public static Panel AddSideButtons(this Control control, List<string> labels, Action<string> oncomplete)
    {
        var factory = new ControlsFactory();
        var panel = new Panel();
        panel.AddFill(new Panel());
        foreach (var label in labels)
        {
            var message = label;
            panel.LayoutBottom(factory.CreateButton(label, () => {
                oncomplete(message);
            }));
        }
        control.LayoutLeft(panel);
        return panel;
    }

}
