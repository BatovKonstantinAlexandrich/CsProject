public class GridControl : ControlBase, ILayoutControl
{
    private readonly FlowLayoutPanel flowPanel;

    public GridControl()
    {
        // Настройка панели с автоматическим переносом элементов слева направо
        this.flowPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true  // Автоматический перенос на следующую строку
        };
    
        // Добавьте flowPanel на форму
        this.Controls.Add(flowPanel);

        this.LayoutFill(this.flowPanel);
    }

    public void Add(Control item)
    {
        this.flowPanel.Controls.Add(item);
    }

    public void Remove(Control item)
    {
        this.flowPanel.Controls.Remove(item);
    }
}
