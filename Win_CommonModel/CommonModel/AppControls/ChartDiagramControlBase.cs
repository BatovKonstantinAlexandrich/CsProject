using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using WinFormsApp.AppViews.BaseComponent;

namespace WinFormsApp.AppComponents.AppControls;

/// <summary>
/// Панель с вложенным контролом диаграммы LiveCharts.
/// </summary>
public class ChartDiagramControlBase : ControlBase
{
    public Control diagram { get; set; }

    private readonly List<Action> _designMouseUnwire = new();


   
    public void SetDiagram(Control chart)
    {
        if (diagram is not null)
            this.Controls.Remove(diagram);
        diagram = chart;
        
        ClearDesignMouseForwarders();
        if (chart is null)
            return;
        
        this.AddFill(chart);        

        BindMouseEventsDeep(chart);
        ApplyDiagramDesignInteractivity();
    }

    /// <summary>
    /// В режиме конструктора перехватывает WPF-мышь (диаграмма остаётся видимой) и пробрасывает на представление.
    /// </summary>
    public void ApplyDiagramDesignInteractivity()
    {
        ClearDesignMouseForwarders();
        if (diagram is null)
            return;

        var designMode = viewModel?.DesignMode == true;
        if (!designMode || viewModel is not AppViewBase view)
            return;

        WireDesignModeMouseForward(diagram, view);
    }

    public ChartDiagramControlBase()
    {
        
        Load += (_, _) => ApplyDiagramDesignInteractivity();        
    }

    private void BindMouseEventsDeep(Control node)
    {
        if (node is null)
            return;
        BindMouseEvents(node);
        foreach (Control child in node.Controls)
            BindMouseEventsDeep(child);
    }

    private void ClearDesignMouseForwarders()
    {
        foreach (var unwire in _designMouseUnwire)
            unwire();
        _designMouseUnwire.Clear();
    }

    private void WireDesignModeMouseForward(Control root, AppViewBase view)
    {
        void WalkWinForms(Control node)
        {
            if (node is ElementHost host)
                TryWireElementHost(host, view);

            foreach (Control child in node.Controls)
                WalkWinForms(child);
        }

        WalkWinForms(root);
    }

    private void TryWireElementHost(ElementHost host, AppViewBase view)
    {
        if (TryWireWpfRoot(host.Child, view))
            return;

        void OnHostReady(object _, EventArgs __)
        {
            if (!TryWireWpfRoot(host.Child, view))
                return;
            host.HandleCreated -= OnHostReady;
            host.Layout -= OnHostReady;
        }

        host.HandleCreated += OnHostReady;
        host.Layout += OnHostReady;
    }

    private bool TryWireWpfRoot(System.Windows.UIElement wpfRoot, AppViewBase view)
    {
        if (wpfRoot is null)
            return false;

        void Attach(System.Windows.UIElement element)
        {
            void OnEnter(object _, System.Windows.Input.MouseEventArgs e)
            {
                view.Focused = true;
                view.OnMouseEnter(this, EventArgs.Empty);
            }

            void OnLeave(object _, System.Windows.Input.MouseEventArgs e)
                => view.OnMouseLeave(this, EventArgs.Empty);

            void OnDown(object _, System.Windows.Input.MouseButtonEventArgs e)
            {
                if (e.ChangedButton != System.Windows.Input.MouseButton.Left)
                    return;
                EnsureDesignerFocusBeforeClick(view);
                view.Focused = true;
                view.OnMouseDown(this, ToWinFormsMouseArgs(e));
                e.Handled = true;
            }

            void OnMove(object _, System.Windows.Input.MouseEventArgs e)
                => view.OnMouseMove(this, ToWinFormsMoveArgs(e));

            void OnUp(object _, System.Windows.Input.MouseButtonEventArgs e)
            {
                view.OnMouseUp(this, ToWinFormsMouseArgs(e));
                if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
                    e.Handled = true;
            }

            element.MouseEnter += OnEnter;
            element.MouseLeave += OnLeave;
            element.PreviewMouseDown += OnDown;
            element.PreviewMouseMove += OnMove;
            element.PreviewMouseUp += OnUp;

            _designMouseUnwire.Add(() => element.MouseEnter -= OnEnter);
            _designMouseUnwire.Add(() => element.MouseLeave -= OnLeave);
            _designMouseUnwire.Add(() => element.PreviewMouseDown -= OnDown);
            _designMouseUnwire.Add(() => element.PreviewMouseMove -= OnMove);
            _designMouseUnwire.Add(() => element.PreviewMouseUp -= OnUp);
        }

        Attach(wpfRoot);
        return true;
    }

    private Point ClientPointFromScreen()
        => PointToClient(System.Windows.Forms.Cursor.Position);

    private MouseEventArgs ToWinFormsMouseArgs(System.Windows.Input.MouseButtonEventArgs e)
    {
        var pt = ClientPointFromScreen();
        return new MouseEventArgs(ToWinFormsButton(e.ChangedButton), e.ClickCount, pt.X, pt.Y, 0);
    }

    private MouseEventArgs ToWinFormsMoveArgs(System.Windows.Input.MouseEventArgs e)
    {
        var pt = ClientPointFromScreen();
        var buttons = MouseButtons.None;
        if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            buttons |= MouseButtons.Left;
        if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed)
            buttons |= MouseButtons.Right;
        if (e.MiddleButton == System.Windows.Input.MouseButtonState.Pressed)
            buttons |= MouseButtons.Middle;
        return new MouseEventArgs(buttons, 0, pt.X, pt.Y, 0);
    }

    private static MouseButtons ToWinFormsButton(System.Windows.Input.MouseButton button) => button switch
    {
        System.Windows.Input.MouseButton.Left => MouseButtons.Left,
        System.Windows.Input.MouseButton.Right => MouseButtons.Right,
        System.Windows.Input.MouseButton.Middle => MouseButtons.Middle,
        System.Windows.Input.MouseButton.XButton1 => MouseButtons.XButton1,
        System.Windows.Input.MouseButton.XButton2 => MouseButtons.XButton2,
        _ => MouseButtons.None
    };
}
