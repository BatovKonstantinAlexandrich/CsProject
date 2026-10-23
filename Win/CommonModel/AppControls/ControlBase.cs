using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;


using System.Text.Json.Serialization;

namespace WinFormsApp.AppComponents.AppControls
{
    /// <summary>
    /// Базовый класс элементов управления Windows с современным дизайном.
    /// </summary>
    public class ControlBase : UserControl, IEventSupport, IChangeSupport, IPanelsControl
    {
        [NotInput]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public Action<object, EventArgs> OnLoad { get; set; } = (s, e) => { };
        public void FormatMarginsAndPaddings()
        {
            //TODO:
        }

        [Label("Левая панель")]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [InputConstructor<Control>()]
        public Control Left { get => Get<Control>("Left"); set => Set<Control>("Left", value); }

        [Label("Правая панель")]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [InputConstructor<Control>()]
        public Control Right { get => Get<Control>("Right"); set => Set<Control>("Right", value); }

        [Label("Верхняя панель")]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [InputConstructor<Control>()]
        public Control Top { get => Get<Control>("Top"); set => Set<Control>("Top", value); }

        [Label("Нижняя панель")]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [InputConstructor<Control>()]
        public Control Bottom { get => Get<Control>("Bottom"); set => Set<Control>("Bottom", value); }



        public readonly ChangingList<object> contextMenuServices = new();
        public AppViewBase viewModel;
        public ComponentBase component;
        public ChangeSupport changeSupport = new();

        // Современные стилистические свойства
        private bool _useModernStyle = true;
        private int _borderRadius = 8;
        private Color _borderColor = Color.FromArgb(220, 220, 220);
        private int _borderWidth = 1;
        private bool _showShadow = true;
        private Color _shadowColor = Color.FromArgb(30, 0, 0, 0);
        private int _shadowOffset = 2;
        private int _shadowBlur = 8;
        internal Control LeftControl;


        public bool IsContentOverflowing()
        {
            // "Виртуальный" размер всего контента (включая то, что вне видимой части)
            var content = this.DisplayRectangle.Size;
            // Видимая клиентская область
            var client = this.ClientSize;
            return content.Width > client.Width || content.Height > client.Height;
        }

        private readonly HashSet<Control> _mouseBoundControls = new();

        public void BindMouseEvents(Control control)
        {
            if (control is null || !_mouseBoundControls.Add(control))
                return;

            control.MouseMove += (s, e) => ForwardChildMouseMove((Control)s, e);
            control.MouseUp += (s, e) => ForwardChildMouseUp((Control)s, e);
            control.MouseDown += (s, e) => ForwardChildMouseDown((Control)s, e);
            control.MouseLeave += (s, e) => ForwardChildMouseLeave(e);
            control.MouseEnter += (s, e) => ForwardChildMouseEnter(e);
            control.MouseHover += (s, e) => { this.OnMouseHover(e); };
        }

        private ActionModels.MouseEventArgs ToHostMouseArgs(Control src, System.Windows.Forms.MouseEventArgs e)
        {
            var client = PointToClient(src.PointToScreen(new Point(e.X, e.Y)));
            return new ActionModels.MouseEventArgs(null, e.Clicks, client.X, client.Y, e.Delta);
        }

        private void ForwardChildMouseDown(Control src, System.Windows.Forms.MouseEventArgs e)
        {
            var hostArgs = ToHostMouseArgs(src, e);
            if (viewModel is AppViewBase view)
            {
                EnsureDesignerFocusBeforeClick(view);
                view.OnMouseDown(this, hostArgs);
                return;
            }
            OnMouseDown(hostArgs);
        }

        private void ForwardChildMouseMove(Control src, System.Windows.Forms.MouseEventArgs e)
        {
            var hostArgs = ToHostMouseArgs(src, e);
            if (viewModel is AppViewBase view)
                view.OnMouseMove(this, hostArgs);
            else
                OnMouseMove(hostArgs);
        }

        private void ForwardChildMouseUp(Control src, System.Windows.Forms.MouseEventArgs e)
        {
            var hostArgs = ToHostMouseArgs(src, e);
            if (viewModel is AppViewBase view)
                view.OnMouseUp(this, hostArgs);
            else
                OnMouseUp(hostArgs);
        }

        private void ForwardChildMouseEnter(EventArgs e)
        {
            if (viewModel is AppViewBase view)
                view.OnMouseEnter(this, e);
            else
                OnMouseEnter(e);
        }

        private void ForwardChildMouseLeave(EventArgs e)
        {
            if (Capture || System.Windows.Forms.Control.MouseButtons == MouseButtons.Left)
                return;
            if (viewModel is AppViewBase view)
                view.OnMouseLeave(this, e);
            else
                OnMouseLeave(e);
        }

        /// <summary>
        /// Перед первым кликом в конструкторе добавляет представление в <see cref="AppViewRoot.FocusedViews"/>,
        /// чтобы успел подключиться инструмент «Перемещение».
        /// </summary>
        protected static void EnsureDesignerFocusBeforeClick(AppViewBase view)
        {
            if (!view.DesignMode)
                return;
            var target = AppViewBase.ResolveDesignerFocusTarget(view);
            var root = target.GetAppRootView();
            if (root is null || root.FocusedViews.Contains(target))
                return;
            target.OnMouseEnter(target, EventArgs.Empty);
        }

        public void SetMinContentSize()
        {
            foreach(Control control in Controls)
            {
                SetMinContentSize(control);
            }
            // "Виртуальный" размер всего контента (включая то, что вне видимой части)
            var content = this.DisplayRectangle.Size;
            // Видимая клиентская область
            var client = this.ClientSize;
            while (content.Width > client.Width)
                client.Width++;
            while (content.Height > client.Height)
                client.Height++;   
            
        }

        public void SetMinContentSize(Control ctrl)
        {
            foreach (Control control in ctrl.Controls)
            {
                SetMinContentSize(control);
            }
            // "Виртуальный" размер всего контента (включая то, что вне видимой части)
            var content = ctrl.DisplayRectangle.Size;
            // Видимая клиентская область
            var client = ctrl.ClientSize;
            while (content.Width > client.Width)
                client.Width++;
            while (content.Height > client.Height)
                client.Height++;
        }

        public ControlBase()
        {
            this.SetVerticalScroll();
            OnInit();
            this.AllowDrop = true;
            BorderRadius = 6;
            BorderWidth = 0;
            ShowShadow = true;
            changeSupport.EventHandler = this;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.UserPaint |
                         ControlStyles.DoubleBuffer |
                         ControlStyles.ResizeRedraw |
                         ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.White;
            ForeColor = Color.FromArgb(33, 33, 33);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            // Инициализируем регион для скругленных углов
            if (_useModernStyle && _borderRadius > 0)
            {
                Load += (s, e) =>
                {
                    using (GraphicsPath path = CreateRoundedRectanglePath(ClientRectangle, _borderRadius))
                    {
                        Region = new Region(path);
                    }
                };
            }
            AddPropertyChangeListener(OnPropertyChanged);

        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            if (viewModel is null || e.Control is null)
                return;

            WinFormsApp.AppViewServices.ViewEventsBinder.PropagateMouseEventsForAddedControl(this, e.Control);
        }

        private void OnPropertyChanged(PropertyChangedMessage message)
        {
            switch (message.Property)
            {
                case nameof(Top):
                    {
                        Control b = message.Before is null ? null : (Control)message.Before;
                        Control a = message.After is null ? null : (Control)message.After;
                        if (b is not null)
                        {
                            this.Controls.Remove(b);
                        }
                        if (a is not null)
                        {
                            this.LayoutTop(a);
                        }
                        break;
                    }
                case nameof(Bottom):
                    {
                        Control b = message.Before is null ? null : (Control)message.Before;
                        Control a = message.After is null ? null : (Control)message.After;
                        if (b is not null)
                        {
                            this.Controls.Remove(b);
                        }
                        if (a is not null)
                        {
                            this.LayoutBottom(a);
                        }
                        break;
                    }
                case nameof(Left):
                    {
                        Control b = message.Before is null ? null : (Control)message.Before;
                        Control a = message.After is null ? null : (Control)message.After;
                        if (b is not null)
                        {
                            this.Controls.Remove(b);
                        }
                        if (a is not null)
                        {
                            this.LayoutLeft(a);
                        }
                        break;
                    }
                case nameof(Right):
                    {
                        Control b = message.Before is null ? null : (Control)message.Before;
                        Control a = message.After is null ? null : (Control)message.After;
                        if (b is not null)
                        {
                            this.Controls.Remove(b);
                        }
                        if (a is not null)
                        {
                            this.LayoutRight(a);
                        }
                        break;
                    }
                case nameof(View):
                    {
                        Control b = message.Before is null ? null : (Control)message.Before;
                        Control a = message.After is null ? null : (Control)message.After;
                        if (b is not null)
                        {
                            this.Controls.Remove(b);
                        }
                        if (a is not null)
                        {
                            this.LayoutFill(a);
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// Включить/выключить современный стиль
        /// </summary>
        public bool UseModernStyle
        {
            get => _useModernStyle;
            set
            {
                _useModernStyle = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Радиус скругления углов
        /// </summary>
        public int BorderRadius
        {
            get => _borderRadius;
            set
            {
                _borderRadius = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Цвет границы
        /// </summary>
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Толщина границы
        /// </summary>
        public int BorderWidth
        {
            get => _borderWidth;
            set
            {
                _borderWidth = value;
                Invalidate();
            }
        }

        public List<Control> GetControls()
        {
            List<Control> ctrls = new();
            foreach (Control control in Controls)
            {
                ctrls.Add(control);
            }
            return ctrls;
        }

        /// <summary>
        /// Показывать тень
        /// </summary>
        public bool ShowShadow
        {
            get => _showShadow;
            set
            {
                _showShadow = value;
                Invalidate();
            }
        }



        private void OnInit()
        {
            contextMenuServices.OnChanged += OnContextMenuChanged;
        }

        private void OnContextMenuChanged(object sender, IChangingListEvent e)
        {
            var factory = new ContextMenuFactory();
            ContextMenuStrip.Items.AddRange(factory.CreateContextMenu(
                contextMenuServices.ToList(),
                target => target.GetType().Name,
                target => target.GetType().GetMethods().Where(m => m.Name != "SetControl" && !m.Name.StartsWith("set_") && !m.Name.StartsWith("get_")).Select(m => target.GetType().GetMethodLabel(m.Name)).ToList(),
                (target, action) => action,
                (target, action) =>
                {
                    try
                    {
                        MethodInfo method = target.GetType().GetMethods().FirstOrDefault(m => m.Name == action);
                        if (method is null)
                            throw new ArgumentException(nameof(action), $"Не определён метод {action}");
                        var args = method.GetUserInputParameters();
                        var result = method.Invoke(target, args.ToArray());
                        return result;
                    }
                    catch (Exception ex)
                    {
                        this.Error($"Исключение при рефлексивном вызове метода {target.GetTypeName()}.{action} => {ex.Message}");
                        throw;
                    }
                }
            ).Items);
        }

        public void BeforeSendEvent(object messageEvent)
        {
            ((IEventSupport)changeSupport).BeforeSendEvent(messageEvent);
        }

        public void FireEvent(CommonEventMessage<string> message)
        {
            ((IEventSupport)changeSupport).FireEvent(message);
        }

        public void FireEvent(PropertyChangedMessage propertyChangedMessage)
        {
            ((IEventSupport)changeSupport).FireEvent(propertyChangedMessage);
        }

        public void FireEvent<T>(CommonEventMessage<T> message)
        {
            ((IEventSupport)changeSupport).FireEvent(message);
        }

        public void FireEvent<T>(T message)
        {
            ((IEventSupport)changeSupport).FireEvent(message);
        }

        public List<string> GetEventListeners()
        {
            return ((IEventSupport)changeSupport).GetEventListeners();
        }

        public void SendChanges()
        {
            ((IEventSupport)changeSupport).SendChanges();
        }

        public void SendEvent(object messageEvent)
        {
            ((IEventSupport)changeSupport).SendEvent(messageEvent);
        }

        [NotInput]
        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public bool Changed { get => ((IChangeSupport)changeSupport).Changed; set => ((IChangeSupport)changeSupport).Changed = value; }

        [NotInput]
        public ConcurrentDictionary<string, object> DataState { get => ((IChangeSupport)changeSupport).DataState; set => ((IChangeSupport)changeSupport).DataState = value; }

        [NotInput]
        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public bool ValidateOnInput { get => ((IChangeSupport)changeSupport).ValidateOnInput; set => ((IChangeSupport)changeSupport).ValidateOnInput = value; }

        public void AddPropertyChangeListener(string property, Action<PropertyChangedMessage> listener)
        {
            ((IChangeSupport)changeSupport).AddPropertyChangeListener(property, listener);
        }

        public T Get<T>(string key)
        {
            return ((IChangeSupport)changeSupport).Get<T>(key);
        }

        public void OnGet<T>(string key, T value)
        {
            ((IChangeSupport)changeSupport).OnGet(key, value);
        }

        public void Set<T>(string key, T value)
        {
            ((IChangeSupport)changeSupport).Set(key, value);
        }

        public bool WasChanged()
        {
            return ((IChangeSupport)changeSupport).WasChanged();
        }

        public void ApplyPropertyChangedEvent(PropertyChangedMessage evt)
        {
            ((IChangeSupport)changeSupport).ApplyPropertyChangedEvent(evt);
        }

        public void AddPropertyChangeListener(Action<PropertyChangedMessage> listener)
        {
            ((IChangeSupport)changeSupport).AddPropertyChangeListener(listener);
        }

        public void RemovePropertyChangeListener(Action<PropertyChangedMessage> listener)
        {
            ((IChangeSupport)changeSupport).RemovePropertyChangeListener(listener);
        }


        [NotInput]
        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public Action<PropertyChangedMessage> OnChange { get => ((IChangeSupport)changeSupport).OnChange; set => ((IChangeSupport)changeSupport).OnChange = value; }
        public Control RightControl { get; internal set; }
        public Control TopControl { get; internal set; }
        public Control BottomControl { get; internal set; }


        [Label("Заполнить пространство")]
        public bool FillContent { get; set; }

        /// <summary>
        /// Отрисовка современного стиля с скругленными углами и тенями
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (!_useModernStyle)
            {
                base.OnPaint(e);
                return;
            }

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            Rectangle rect = ClientRectangle;
            rect.Width -= 1;
            rect.Height -= 1;

            // Рисуем тень
            if (_showShadow && _borderRadius > 0)
            {
                DrawShadow(g, rect);
            }

            // Рисуем фон с скругленными углами
            using (GraphicsPath path = CreateRoundedRectanglePath(rect, _borderRadius))
            {
                using (SolidBrush brush = new SolidBrush(BackColor))
                {
                    g.FillPath(brush, path);
                }

                // Рисуем границу
                if (_borderWidth > 0)
                {
                    using (Pen pen = new Pen(_borderColor, _borderWidth))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }

            // Рисуем дочерние элементы
            base.OnPaint(e);
        }

        /// <summary>
        /// Создает путь для скругленного прямоугольника
        /// </summary>
        private GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90); // Верхний левый угол
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90); // Верхний правый угол
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90); // Нижний правый угол
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90); // Нижний левый угол
            path.CloseFigure();

            return path;
        }

        /// <summary>
        /// Рисует тень вокруг элемента
        /// </summary>
        private void DrawShadow(Graphics g, Rectangle rect)
        {
            int shadowSize = _shadowBlur;
            Rectangle shadowRect = new Rectangle(
                rect.X + _shadowOffset,
                rect.Y + _shadowOffset,
                rect.Width,
                rect.Height
            );

            for (int i = 0; i < shadowSize; i++)
            {
                int alpha = _shadowColor.A / shadowSize * (shadowSize - i) / shadowSize;
                Color shadowColor = Color.FromArgb(alpha, _shadowColor.R, _shadowColor.G, _shadowColor.B);

                using (GraphicsPath shadowPath = CreateRoundedRectanglePath(shadowRect, _borderRadius))
                using (SolidBrush brush = new SolidBrush(shadowColor))
                {
                    g.FillPath(brush, shadowPath);
                }
            }
        }

        /// <summary>
        /// Создает область для клиппинга дочерних элементов
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_useModernStyle && _borderRadius > 0)
            {
                // Обновляем регион при изменении размера
                using (GraphicsPath path = CreateRoundedRectanglePath(ClientRectangle, _borderRadius))
                {
                    Region = new Region(path);
                }
            }
        }

        public void AddPropertyChangeListener<T>(string property, Action<T> changed)
        {
            ((IChangeSupport)changeSupport).AddPropertyChangeListener(property, changed);
        }

        public void Bind(string propertyName, IChangeSupport target, string relativePropertyName)
        {
            ((IChangeSupport)changeSupport).Bind(propertyName, target, relativePropertyName);
        }

        public void SetLeft(Control leftButton) => Left = leftButton;
        public void SetRight(Control leftButton) => Right = leftButton;
        public void SetTop(Control leftButton) => Top = leftButton;
        public void SetBottom(Control leftButton) => Bottom = leftButton;


        public void HideSides()
        {
        }

        public void SetBottomHeight(int h)
        {
        }

        public void SetLeftWidth(int h)
        {
        }

        public void SetRightWidth(int h)
        {
        }

        public void SetTopHeight(int h)
        {
        }

        public void SetAutoSizes()
        {
            throw new NotImplementedException();
        }

        public int GetTopHeight()
        {
            throw new NotImplementedException();
        }

        public Control GetRight()
        {
            throw new NotImplementedException();
        }

        public void FillLeft(Button button)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class ControlBase<TView> : ControlBase, IBind<TView> where TView : AppViewBase
    {
        public abstract void Bind(TView view);
    }


}
