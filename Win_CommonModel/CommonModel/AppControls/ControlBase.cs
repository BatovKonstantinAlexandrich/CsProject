using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;

using System.Text.Json.Serialization;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using System.ComponentModel;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;
using WinFormsApp.API;
using ActionModels;
using Console_CommonModel.CommonLib.Others;

namespace WinFormsApp.AppComponents.AppControls
{
    /// <summary>
    /// Базовый класс элементов управления Windows с современным дизайном.
    /// </summary>
    public class ControlBase : UserControl, IEventSupport, IChangeSupport, IControlBase, IPanelsControl
    {
        [NotInput]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public Action<object, EventArgs> OnLoad { get; set; } = (s, e) => { };

        [Label("Левая панель")]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [InputConstructor<Control>()]
        public IControlBase Left { get => Get<IControlBase>("Left"); set => Set<IControlBase>("Left", value); }

        [Label("Правая панель")]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [InputConstructor<Control>()]
        public IControlBase Right { get => Get<IControlBase>("Right"); set => Set<IControlBase>("Right", value); }

        [Label("Верхняя панель")]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [InputConstructor<Control>()]
        public IControlBase Top { get => Get<IControlBase>("Top"); set => Set<IControlBase>("Top", value); }

        [Label("Нижняя панель")]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [InputConstructor<Control>()]
        public IControlBase Bottom { get => Get<IControlBase>("Bottom"); set => Set<IControlBase>("Bottom", value); }



        public readonly ChangingList<object> contextMenuServices = new();
        public AppViewBase viewModel;
        public object component;
        public ChangeSupport changeSupport = new();

        // Современные стилистические свойства
        private bool _useModernStyle = true;
        private int _borderRadius = 8;
        private Color _borderColor = System.Drawing.Color.FromArgb(220, 220, 220);
        private int _borderWidth = 1;
        private bool _showShadow = true;
        private Color _shadowColor = System.Drawing.Color.FromArgb(30, 0, 0, 0);
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

            control.MouseMove += (s, e) => ForwardChildMouseMove((Control)s, new()
            {
                X = e.X,
                Y = e.Y,
                Button = Enum.GetName(e.Button).CreateEnumValue<Console_CommonModel.API.MouseButtons>()
            });
            control.MouseUp += (s, e) => ForwardChildMouseUp((Control)s, new()
            {
                X = e.X,
                Y = e.Y,
                Button = Enum.GetName(e.Button).CreateEnumValue<Console_CommonModel.API.MouseButtons>()
            });
            control.MouseDown += (s, e) => ForwardChildMouseDown((Control)s, new()
            {
                X = e.X,
                Y = e.Y,
                Button = Enum.GetName(e.Button).CreateEnumValue<Console_CommonModel.API.MouseButtons>()
            });
            control.MouseLeave += (s, e) => ForwardChildMouseLeave(e);
            control.MouseEnter += (s, e) => ForwardChildMouseEnter(e);
            control.MouseHover += (s, e) => { this.OnMouseHover(e); };
        }

        private System.Windows.Forms.MouseEventArgs ToHostMouseArgs(Control src, ActionModels.MouseEventArgs e)
        {
            var client = PointToClient(src.PointToScreen(new Point(e.X, e.Y)));
            var b = Enum.GetName(e.Button).ToString().CreateEnumValue<MouseButtons>();
            return new System.Windows.Forms.MouseEventArgs(b, e.Clicks, client.X, client.Y, e.Delta);
        }

        private void ForwardChildMouseDown(Control src, ActionModels.MouseEventArgs e)
        {
            var hostArgs = ToHostMouseArgs(src, e);
            if (viewModel is AppViewBase view)
            {
                EnsureDesignerFocusBeforeClick(view);
                view.OnMouseDown(this, new() { 
                    X = e.X,
                    Y = e.Y,
                    Button = e.Button
                });
                return;
            }
            OnMouseDown(hostArgs);
        }

        private void ForwardChildMouseMove(Control src, ActionModels.MouseEventArgs e)
        {
            var hostArgs = ToHostMouseArgs(src, e);
            if (viewModel is AppViewBase view)
                view.OnMouseMove(this, new()
                {
                    X = e.X,
                    Y = e.Y,
                    Button = e.Button
                });
            else
                OnMouseMove(hostArgs);
        }

        private void ForwardChildMouseUp(Control src, ActionModels.MouseEventArgs e)
        {
            var hostArgs = ToHostMouseArgs(src, e);
            if (viewModel is AppViewBase view)
                view.OnMouseUp(this, new()
                {
                    X = e.X,
                    Y = e.Y,
                    Button = e.Button
                });
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
            var target = ResolveDesignerFocusTarget(view);
            var root = target.GetAppRootView();
            if (root is null || root.FocusedViews.Contains(target))
                return;
            target.OnMouseEnter(target, EventArgs.Empty);
        }

        private static AppViewBase ResolveDesignerFocusTarget(AppViewBase view)
        {
            throw new NotImplementedException();
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
            BackColor = System.Drawing.Color.White;
            ForeColor = System.Drawing.Color.FromArgb(33, 33, 33);
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

            PropagateMouseEventsForAddedControl(this, e.Control);
        }

        private void PropagateMouseEventsForAddedControl(ControlBase controlBase, Control control)
        {
            throw new NotImplementedException();
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
            ContextMenuStrip.Items.AddRange(CreateContextMenu(
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
                        var args = GetUserInputParameters(method);
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

        private ContextMenuStrip CreateContextMenu(List<object> list, Func<object, object> value1, Func<object, List<string>> value2, Func<object, object, object> value3, Func<object, object, object> value4)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<object> GetUserInputParameters(MethodInfo method)
        {
            throw new NotImplementedException();
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
                Color shadowColor = System.Drawing.Color.FromArgb(alpha, _shadowColor.R, _shadowColor.G, _shadowColor.B);

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

        public void SetLeft(IControlBase leftButton) => Left = leftButton;
        public void SetRight(IControlBase leftButton) => Right = leftButton;
        public void SetTop(IControlBase leftButton) => Top = leftButton;
        public void SetBottom(IControlBase leftButton) => Bottom = leftButton;


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

        public AppViewBase ViewModel { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        Console_CommonModel.CommonLib.Others.ComponentBase IControlBase.component { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool KeyPreview { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        IControlBase IControlBase.Left { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        IControlBase IControlBase.Right { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        IControlBase IControlBase.Top { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        IControlBase IControlBase.Bottom { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        object IControlBase.AutoSizeMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        Size IControlBase.PreferredSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        Action<object, object> IControlBase.HandleCreated { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
 
        public IControlBase RunAtForm(string title = "")
        {
            throw new NotImplementedException();
        }

        public IControlBase ShowDialog(string title = "")
        {
            throw new NotImplementedException();
        }

        public IControlBase GetAppForm()
        {
            throw new NotImplementedException();
        }

        public void SetSize(int v1, int v2)
        {
            throw new NotImplementedException();
        }

        public void Add(IComponent? component)
        {
            throw new NotImplementedException();
        }

        public void Add(IComponent? component, string? name)
        {
            throw new NotImplementedException();
        }

        public void Remove(IComponent? component)
        {
            throw new NotImplementedException();
        }

        public ComponentCollection Components => throw new NotImplementedException();

        public string Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        IControlBase ITreeContainer<IControlBase>.Parent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        ChangingList<IControlBase> ITreeContainer<IControlBase>.Controls { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Point GetMousePosition()
        {
            throw new NotImplementedException();
        }

        public void SubmitDialog(string title, object control, Action<object> value)
        {
            throw new NotImplementedException();
        }

        public void ShowSnakBar(string title)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ShowView<TAppView>(string title, TAppView model, Action<bool, TAppView> oncompleted) where TAppView : AppViewBase
        {
            throw new NotImplementedException();
        }

        public bool InfoDialog(string Title, string Text, string Button)
        {
            throw new NotImplementedException();
        }

        public void ShowHelp(string Text)
        {
            throw new NotImplementedException();
        }

        public bool RemoteDialog(string Title, string Url)
        {
            throw new NotImplementedException();
        }

        public bool ConfirmDialog(string Title, string Text)
        {
            throw new NotImplementedException();
        }

        public bool CreateEntityDialog(string Title, string Entity)
        {
            throw new NotImplementedException();
        }

        public object InputDialog(string Title, object Properties)
        {
            throw new NotImplementedException();
        }

        public string Eval(string js)
        {
            throw new NotImplementedException();
        }

        public string HandleEvalResult(Func<object, object> handle, string js)
        {
            throw new NotImplementedException();
        }

        public string Callback(string action, params string[] args)
        {
            throw new NotImplementedException();
        }

        public bool AddEventListener(string id, string type, string js)
        {
            throw new NotImplementedException();
        }

        public bool DispatchEvent(string id, string type, object message)
        {
            throw new NotImplementedException();
        }

        public string OnConnected(string token)
        {
            throw new NotImplementedException();
        }

        public bool AnchorBottom { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool AnchorLeft { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool AnchorRight { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool AnchorTop { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string BackgroundColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        string IStyledItem.BorderColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        float IStyledItem.BorderWidth { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Color { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        DesignerResizeTool.Cursors IStyledItem.Cursor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        string IStyledItem.Dock { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public float FontSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        int IStyledItem.Margin { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int MinHeight { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int MinWidth { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        int IStyledItem.Padding { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string TextAlign { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int TextIndent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int X { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int Y { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string GetCssText()
        {
            throw new NotImplementedException();
        }

        public void SetBaseTextStyle()
        {
            throw new NotImplementedException();
        }

        Action<object, EventArgs> ActionModels.IViewEventSupport.OnClick { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        Action<object, ActionModels.DragEventArgs> ActionModels.IViewEventSupport.OnDragEnter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<object, EventArgs> OnDragged { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        Action<object, EventArgs> ActionModels.IViewEventSupport.OnDragLeave { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        Action<object, ActionModels.DragEventArgs> ActionModels.IViewEventSupport.OnDragOver { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<object, EventArgs> OnDropped { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<object, EventArgs> OnKeyClicked { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        Action<object, Console_CommonModel.ActionModels.KeyEventArgs> ActionModels.IViewEventSupport.OnKeyDown { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<object, ActionModels.KeyPressEventArgs> OnKeyPressed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        Action<object, Console_CommonModel.ActionModels.KeyEventArgs> ActionModels.IViewEventSupport.OnKeyUp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        Action<object, EventArgs> ActionModels.IViewEventSupport.OnLocationChanged { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        Action<object, ActionModels.MouseEventArgs> ActionModels.IViewEventSupport.OnMouseDoubleClick { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        Action<object, ActionModels.MouseEventArgs> ActionModels.IViewEventSupport.OnMouseDown { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        Action<object, EventArgs> ActionModels.IViewEventSupport.OnMouseEnter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        Action<object, EventArgs> ActionModels.IViewEventSupport.OnMouseLeave { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        Action<object, ActionModels.MouseEventArgs> ActionModels.IViewEventSupport.OnMouseMove { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        Action<object, ActionModels.MouseEventArgs> ActionModels.IViewEventSupport.OnMouseUp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<object, EventArgs> OnResized { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<object, ActionModels.ScrollEventArgs> OnScrollChanged { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        Action<object, EventArgs> ActionModels.IViewEventSupport.OnValidated { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void AddOnClick(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void AddOnKeyClicked(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void AddOnKeyDown(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void AddOnKeyPressed(Action<object, ActionModels.KeyPressEventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void AddOnKeyUp(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void AddOnLoad(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void AddOnMouseDoubleClick(Action<object, ActionModels.MouseEventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void AddOnMouseDown(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void AddOnMouseEnter(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void AddOnMouseLeave(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void AddOnMouseMove(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void AddOnMouseUp(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void AddOnResized(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public List<string> GetBaseEventProperties()
        {
            throw new NotImplementedException();
        }

        public void RemoveOnClick(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void RemoveOnKeyClicked(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void RemoveOnKeyDown(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void RemoveOnKeyPressed(Action<object, ActionModels.KeyPressEventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void RemoveOnKeyUp(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void RemoveOnLoad(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void RemoveOnMouseDoubleClick(Action<object, ActionModels.MouseEventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void RemoveOnMouseDown(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void RemoveOnMouseEnter(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void RemoveOnMouseLeave(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void RemoveOnMouseMove(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void RemoveOnMouseUp(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void RemoveOnResized(Action<object, EventArgs> todo)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public IControlBase ToBaseForm(string title = "")
        {
            throw new NotImplementedException();
        }

        bool IControlRoot.IsHandleCreated { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        bool IControlRoot.InvokeRequired { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        bool IControlRoot.IsDisposed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }

    public abstract class ControlBase<TView> : ControlBase, IBind<TView> where TView : AppViewBase
    {
        public abstract void Bind(TView view);
    }


}
