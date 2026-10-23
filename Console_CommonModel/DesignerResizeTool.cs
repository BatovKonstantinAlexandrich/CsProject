using Console_CommonModel.API;

using MySqlX.XDevAPI;

using Parlot;

using System.Drawing;

using WinFormsApp.AppViews.AppRootModule;

namespace WinFormsApp.API
{
    /// <summary>
    /// Инструмент изменения размера выделенного представления (края и углы).
    /// Обновляет <see cref="AppViewBase.Width"/> / <see cref="AppViewBase.Height"/>
    /// (и X/Y при ресайзе с левой/верхней стороны).
    /// </summary>
    public class DesignerResizeTool : ViewTool
    {
        private const int HandleMargin = 12;
        private const int MinWidth = 40;
        private const int MinHeight = 25;

        private AppViewBase viewBase;
        private bool resizing;
        private ResizeEdge edge = ResizeEdge.None;
        private Point resizeAnchorParent;
        private int startX;
        private int startY;
        private int startWidth;
        private int startHeight;
        private bool savedAutoSize;
        private DesignerViewSubtreeEvents.Subscription _mouseSubscription;

        private enum ResizeEdge
        {
            None,
            N, S, E, W,
            NE, NW, SE, SW
        }

        public DesignerResizeTool() : base(new DesignerView()) { }

        public DesignerResizeTool(DesignerView control) : base(control)
        {
        }

        /// <summary>Идёт жест изменения размера (Capture активен) — не снимать фокус по MouseLeave.</summary>
        public bool IsResizing => resizing;

        protected override void Activating(ViewNode view)
        {
            viewBase = AppViewBase.ResolveDesignerFocusTarget((AppViewBase)view);

            // Синхронизация модели с фактическим размером контрола (без подмены Height на Width).
            var existing = viewBase.ToControl();
            if (existing is not null)
            {
                if (viewBase.Width != existing.Width)
                    viewBase.Width = existing.Width;
                if (viewBase.Height != existing.Height)
                    viewBase.Height = existing.Height;
            }

            _mouseSubscription = new DesignerViewSubtreeEvents.Subscription();
            _mouseSubscription.Wire(viewBase, OnMouseDown, OnMouseMove, OnMouseUp, OnMouseLeave);
            viewBase.OnMouseDoubleClick += this.DoOnDoubleClick;

            rollback.Add(() =>
            {
                _mouseSubscription?.Unwire(OnMouseDown, OnMouseMove, OnMouseUp, OnMouseLeave);
                _mouseSubscription = null;
                EndResize(restoreAutoSize: true);
            });
        }

        private void DoOnDoubleClick(object sender, MouseEventArgs evt)
        {
            var control = viewBase.ToControl();
            var client = viewBase.Client.PointToClient(new(evt.X, evt.Y));
            edge = GetResizeEdge(client.X, client.Y, control.Width, control.Height, HandleMargin);
            if(edge!= ResizeEdge.None)
            {
                switch(edge)
                {
                    case ResizeEdge.N:
                        view.AnchorTop = view.AnchorTop ? false : true;
                        break;
                    case ResizeEdge.W:
                        view.AnchorLeft = view.AnchorLeft ? false : true;
                        break;
                    case ResizeEdge.E:
                        view.AnchorRight = view.AnchorRight ? false : true;
                        break;
                    case ResizeEdge.S:
                        view.AnchorBottom = view.AnchorBottom ? false : true;
                        break;
                    default: throw new NotImplementedException();
                }
            }
        }
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || viewBase is null || resizing)
                return;

            var control = viewBase.ToControl();
            var parent = control?.Parent;
            if (control is null || parent is null)
                return;

            var client = control.PointToClient(new(e.X,e.Y));
            edge = GetResizeEdge(client.X, client.Y, control.Width, control.Height, HandleMargin);
            if (edge == ResizeEdge.None)
                return;

            resizing = true;
            viewBase.Dock = "None";
            savedAutoSize = viewBase.AutoSize;
            viewBase.AutoSize = false;

            // Стартовая геометрия — фактические Bounds контрола (модель X/Y часто рассинхрон).
             
            startX = control.X;
            startY = control.Y;
            startWidth = Math.Max(1, control.Width);
            startHeight = Math.Max(1, control.Height);
            viewBase.X = startX;
            viewBase.Y = startY;
            viewBase.Width = startWidth;
            viewBase.Height = startHeight;
            resizeAnchorParent = parent.PointToClient(new(e.X, e.Y));

            control.Capture = true;
            control.Cursor = CursorForEdge(edge);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var control = viewBase?.ToControl();
            if (control is null)
                return;

            if (!resizing)
            {
                var client = control.PointToClient(new(e.X, e.Y));
                var hover = GetResizeEdge(client.X, client.Y, control.Width, control.Height, HandleMargin);
                control.Cursor = CursorForEdge(hover);
                return;
            }

            // SetBounds при ресайзе слева/сверху может сбросить Capture — удерживаем.
            if (!control.Capture)
                control.Capture = true;

            // Пока Capture=true, события приходят даже за пределами контрола —
            // считаем дельту только в координатах родителя, не из e.X/e.Y.
            var parent = control.Parent;
            if (parent is null)
                return;

            var cur = parent.PointToClient(new(e.X, e.Y));
            int dx = cur.X - resizeAnchorParent.X;
            int dy = cur.Y - resizeAnchorParent.Y;

            int x = startX;
            int y = startY;
            int w = startWidth;
            int h = startHeight;
            //this.Info(edge);
            switch (edge)
            {
                //Право
                case ResizeEdge.E:

                    w = startWidth + dx;
                    break;

                //Лево
                case ResizeEdge.W:
                    x = startX + dx;
                    w = startX + startWidth - x;                    
                    break;

                //НИЗ
                case ResizeEdge.S:
                    h = startHeight + dy;
                    break;
                case ResizeEdge.N:
                    y = startY + dy;
                    h = startY + startHeight-y;
                    //h = startHeight + dy;
                    //y = startY + dy;
                    break;
                case ResizeEdge.SE:
                    w = startWidth + dx;
                    h = startHeight + dy;
                    break;
                case ResizeEdge.SW:
                    w = startWidth - dx;
                    x = startX + dx;
                    h = startHeight + dy;
                    break;
                case ResizeEdge.NE:
                    w = startWidth + dx;
                    h = startHeight - dy;
                    y = startY + dy;
                    break;
                case ResizeEdge.NW:
                    w = startWidth - dx;
                    x = startX + dx;
                    h = startHeight - dy;
                    y = startY + dy;
                    break;
            }

            var minW = Math.Max(MinWidth, viewBase.MinWidth > 0 ? viewBase.MinWidth : MinWidth);
            var minH = Math.Max(MinHeight, viewBase.MinHeight > 0 ? viewBase.MinHeight : MinHeight);

            if (w < minW)
            {
                if (edge is ResizeEdge.W or ResizeEdge.NW or ResizeEdge.SW)
                    x = startX + startWidth - minW;
                w = minW;
            }

            if (h < minH)
            {
                if (edge is ResizeEdge.N or ResizeEdge.NW or ResizeEdge.NE)
                    y = startY + startHeight - minH;
                h = minH;
            }

            viewBase.X = x;
            viewBase.Y = y;
            viewBase.Width = w;
            viewBase.Height = h;

            control.AutoSize = false;
            if (control is IControlBase cb)
                cb.AutoSizeMode = "GrowAndShrink";
            control.SetBounds(x, y, w, h);
            control.Cursor = CursorForEdge(edge);
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            EndResize(restoreAutoSize: true);
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            // Во время ресайза курсор часто уходит за левую/нижнюю границу —
            // не сбрасываем курсор и не трогаем состояние жеста.
            if (resizing || isMouseLeftButton())
            {
                var ctrl = viewBase?.ToControl();
                if (ctrl is not null && !ctrl.Capture)
                    ctrl.Capture = true;
                return;
            }
            var c = viewBase?.ToControl();
            if (c is not null)
                c.Cursor = Cursors.Default;
        }
        public enum Cursors
        {
            Default,
            SizeNS,
            SizeWE,
            SizeNWSE,
            SizeNESW
        }
        private bool isMouseLeftButton()
        {
            return false;
        }

        private void EndResize(bool restoreAutoSize)
        {
            if (!resizing && edge == ResizeEdge.None)
                return;

            resizing = false;
            edge = ResizeEdge.None;
            if (restoreAutoSize && viewBase is not null)
                viewBase.AutoSize = savedAutoSize;

            var control = viewBase?.ToControl();
            if (control is not null)
            {
                control.Capture = false;
                control.Cursor = Cursors.Default;
            }
        }

        internal static Cursors GetResizeCursorForClientPoint(int x, int y, int w, int h, int margin)
            => CursorForEdge(GetResizeEdge(x, y, w, h, margin));

        private static Cursors CursorForEdge(ResizeEdge edge) => edge switch
        {
            ResizeEdge.N or ResizeEdge.S => Cursors.SizeNS,
            ResizeEdge.E or ResizeEdge.W => Cursors.SizeWE,
            ResizeEdge.NE or ResizeEdge.SW => Cursors.SizeNESW,
            ResizeEdge.NW or ResizeEdge.SE => Cursors.SizeNWSE,
            _ => Cursors.Default,
        };

        private static ResizeEdge GetResizeEdge(int x, int y, int w, int h, int margin)
        {
            w = Math.Max(1, w);
            h = Math.Max(1, h);
            int dLeft = x;
            int dRight = w - x;
            int dTop = y;
            int dBottom = h - y;

            bool zLeft = dLeft <= margin;
            bool zRight = dRight <= margin;
            bool zTop = dTop <= margin;
            bool zBottom = dBottom <= margin;

            if (!zLeft && !zRight && !zTop && !zBottom)
                return ResizeEdge.None;

            if (zLeft && zTop)
                return ResizeEdge.NW;
            if (zRight && zTop)
                return ResizeEdge.NE;
            if (zLeft && zBottom)
                return ResizeEdge.SW;
            if (zRight && zBottom)
                return ResizeEdge.SE;

            int best = Math.Min(Math.Min(dLeft, dRight), Math.Min(dTop, dBottom));
            if (best == dLeft)
                return ResizeEdge.W;
            if (best == dRight)
                return ResizeEdge.E;
            if (best == dTop)
                return ResizeEdge.N;
            return ResizeEdge.S;
        }
    }
}
