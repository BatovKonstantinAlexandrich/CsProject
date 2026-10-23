using Console_CommonModel.CommonLib.LibViews.AppViewModules.MenuModule;

using System.Text;


namespace Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent
{


    [NotInput]
    public class AppProgramView : AppViewContent<NavView>, INavView
    {
        protected MenuView fileMenuView { get; } = new MenuView() { Text = "Файл" };
        protected MenuView navMenuView { get; } = new MenuView() { Text = "Навигация" };
        protected MenuView designMenuView { get; } = new MenuView() { Text = "Проектирование" };

        [Label("filename")]

        public string filename { get; set; }
        [Label("xml Text")]

        public string xmlText { get; set; }
        [Label("xml View")]
        public AppViewBase xmlView { get; } = new();

        private FileSystemWatcher _xmlFileWatcher;
        private Timer _fileReloadDebounceTimer;
        private Timer _fileWatchPollTimer;
        private readonly object _fileWatchLock = new();

        private string _watchDirectory;
        private string _watchFilter;
        private string _watchedTargetFullPath;
        private Action<string, string> _watchFileCallback;
        private DateTime _lastKnownFileWriteUtc;
        private volatile int _watcherRecoveryGate;

        public AppProgramView() : base()
        {
            BindFileMenu();
            BindNavMenu();
            BindDesignerMenu();
            AddNavOption("Приложение", xmlView.ToCard());
            OnNavSelected("Приложение");
            var designer = this;
            OnLoad += (s, e) =>
            {
                designer.ToControl().GetAppForm().SetSize(1024, 800);
                designer.ToControl().GetAppForm().KeyPreview = true;
            };

            BindKeyboardConfig();
        }


        /// <summary>
        /// Связывание конфигурации клавиатуры с файлом json
        /// </summary>
        public void BindKeyboardConfig()
        {
            keyboardConfig.OnChanged += (s, evt) =>
            {
                //this.SaveKeyboardConfig();
            };
        }

        /// <summary>
        /// Дублирование чтения файла не нужно: сохраняем текст для «Сохранить» и загружаем в <see cref="LoadXmlText"/>.
        /// </summary>
        public override void LoadXmlFile(string filename)
        {
            var xml = File.ReadAllText(filename);
            this.filename = filename;
            xmlText = xml;
            LoadXmlText(xml);
        }

        /// <summary>
        /// Перезагрузка XML в <see cref="xmlView"/> при изменении файла на диске (как при «Открыть»).
        /// </summary>
        public virtual void OnFileChanged(string changedPath, string text)
        {
            xmlText = text;
            if (!string.IsNullOrEmpty(changedPath))
                filename = changedPath;
            xmlView.FromXml(xmlText);
        }

        /// <summary>
        /// Отслеживание изменений: можно передать путь к файлу или к каталогу (тогда фильтр <c>*.xml</c>).
        /// </summary>
        public void WatchFileChanges(string fileOrDirectoryPath, Action<string, string> onChanged)
        {
            StopWatchingFileChanges();

            if (string.IsNullOrWhiteSpace(fileOrDirectoryPath))
                return;

            string watchDir;
            string filter;
            if (File.Exists(fileOrDirectoryPath))
            {
                watchDir = Path.GetDirectoryName(Path.GetFullPath(fileOrDirectoryPath)) ?? ".";
                filter = Path.GetFileName(fileOrDirectoryPath);
            }
            else if (Directory.Exists(fileOrDirectoryPath))
            {
                watchDir = Path.GetFullPath(fileOrDirectoryPath);
                filter = "*.xml";
            }
            else
            {
                watchDir = Path.GetDirectoryName(Path.GetFullPath(fileOrDirectoryPath)) ?? ".";
                filter = Path.GetFileName(fileOrDirectoryPath);
                if (string.IsNullOrEmpty(filter) || filter is "." or "..")
                    filter = "*.xml";
            }

            _watchDirectory = watchDir;
            _watchFilter = filter;
            try
            {
                _watchedTargetFullPath = Path.GetFullPath(filename ?? fileOrDirectoryPath);
            }
            catch (ArgumentException)
            {
                _watchedTargetFullPath = filename ?? fileOrDirectoryPath;
            }
            _watchFileCallback = onChanged;
            try
            {
                if (File.Exists(_watchedTargetFullPath))
                    _lastKnownFileWriteUtc = File.GetLastWriteTimeUtc(_watchedTargetFullPath);
            }
            catch
            {
                _lastKnownFileWriteUtc = DateTime.MinValue;
            }

            AttachFileSystemWatcher();
            StartFileWatchPolling();
        }

        private void AttachFileSystemWatcher()
        {
            lock (_fileWatchLock)
            {
                if (_xmlFileWatcher is not null)
                {
                    try
                    {
                        _xmlFileWatcher.EnableRaisingEvents = false;
                        _xmlFileWatcher.Dispose();
                    }
                    catch
                    {
                    }
                    _xmlFileWatcher = null;
                }

                if (string.IsNullOrEmpty(_watchDirectory) || !Directory.Exists(_watchDirectory))
                    return;

                _xmlFileWatcher = new FileSystemWatcher(_watchDirectory, _watchFilter)
                {
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.CreationTime,
                    IncludeSubdirectories = false,
                    InternalBufferSize = 65536,
                    EnableRaisingEvents = true
                };

                void scheduleReload(string fullPath)
                {
                    if (string.IsNullOrEmpty(_watchedTargetFullPath))
                        return;
                    try
                    {
                        if (!string.Equals(Path.GetFullPath(fullPath), Path.GetFullPath(_watchedTargetFullPath), StringComparison.OrdinalIgnoreCase))
                            return;
                    }
                    catch (ArgumentException)
                    {
                        return;
                    }

                    lock (_fileWatchLock)
                    {
                        _fileReloadDebounceTimer?.Dispose();
                        _fileReloadDebounceTimer = new Timer(_ => TryReloadWatchedFileFromDisk(), null, 400, Timeout.Infinite);
                    }
                }

                _xmlFileWatcher.Changed += (_, e) =>
                {
                    if (e.ChangeType == WatcherChangeTypes.Changed)
                        scheduleReload(e.FullPath);
                };
                _xmlFileWatcher.Created += (_, e) => scheduleReload(e.FullPath);
                _xmlFileWatcher.Renamed += (_, e) => scheduleReload(e.FullPath);
                _xmlFileWatcher.Error += OnFileSystemWatcherError;
            }
        }

        private void OnFileSystemWatcherError(object sender, ErrorEventArgs e)
        {
            this.Info($"Ошибка FileSystemWatcher (восстановление): {e.GetException().Message}");
            if (Interlocked.CompareExchange(ref _watcherRecoveryGate, 1, 0) != 0)
                return;
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    Thread.Sleep(500);
                    lock (_fileWatchLock)
                    {
                        if (_xmlFileWatcher is not null)
                        {
                            try
                            {
                                _xmlFileWatcher.EnableRaisingEvents = false;
                                _xmlFileWatcher.Dispose();
                            }
                            catch
                            {
                            }
                            _xmlFileWatcher = null;
                        }
                    }
                    AttachFileSystemWatcher();
                }
                finally
                {
                    Interlocked.Exchange(ref _watcherRecoveryGate, 0);
                }
            });
        }

        private void StartFileWatchPolling()
        {
            lock (_fileWatchLock)
            {
                _fileWatchPollTimer?.Dispose();
                _fileWatchPollTimer = new Timer(_ => PollWatchedFileForChanges(), null, 2500, 2500);
            }
        }

        private void PollWatchedFileForChanges()
        {
            if (string.IsNullOrEmpty(_watchedTargetFullPath) || _watchFileCallback is null)
                return;
            try
            {
                if (!File.Exists(_watchedTargetFullPath))
                    return;
                var utc = File.GetLastWriteTimeUtc(_watchedTargetFullPath);
                if (utc <= _lastKnownFileWriteUtc)
                    return;
                lock (_fileWatchLock)
                {
                    _fileReloadDebounceTimer?.Dispose();
                    _fileReloadDebounceTimer = new Timer(_ => TryReloadWatchedFileFromDisk(), null, 150, Timeout.Infinite);
                }
            }
            catch
            {
            }
        }

        private void TryReloadWatchedFileFromDisk()
        {
            try
            {
                var path = _watchedTargetFullPath ?? filename;
                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                    return;

                string xml = null;
                for (var attempt = 0; attempt < 8; attempt++)
                {
                    try
                    {
                        xml = File.ReadAllText(path);
                        break;
                    }
                    catch (IOException)
                    {
                        Thread.Sleep(60);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Thread.Sleep(60);
                    }
                }
                if (xml is null)
                    return;

                try
                {
                    _lastKnownFileWriteUtc = File.GetLastWriteTimeUtc(path);
                }
                catch
                {
                }

                RunOnUiThread(() => _watchFileCallback?.Invoke(path, xml));
            }
            finally
            {
                lock (_fileWatchLock)
                {
                    _fileReloadDebounceTimer?.Dispose();
                    _fileReloadDebounceTimer = null;
                }
            }
        }

        public void StopWatchingFileChanges()
        {
            lock (_fileWatchLock)
            {
                _fileReloadDebounceTimer?.Dispose();
                _fileReloadDebounceTimer = null;
                _fileWatchPollTimer?.Dispose();
                _fileWatchPollTimer = null;
            }
            _watchDirectory = null;
            _watchFilter = null;
            _watchedTargetFullPath = null;
            _watchFileCallback = null;
            if (_xmlFileWatcher is not null)
            {
                try
                {
                    _xmlFileWatcher.EnableRaisingEvents = false;
                    _xmlFileWatcher.Dispose();
                }
                catch
                {
                }
                _xmlFileWatcher = null;
            }
        }

        private void BindDesignerMenu()
        {
            var fileDialog = new NewFileDialogService();
            var designModeMenuItem = new MenuView()
            {
                Text = DesignMode ? "Выключить режим конструктора" : "Включить режим конструктора"
            };
            designModeMenuItem.OnClick += (s, e) =>
            {

                DesignMode = DesignMode ? false : true;
                designModeMenuItem.Text = DesignMode ? "Выключить режим конструктора" : "Включить режим конструктора";
            };
            designMenuView.MenuItems.Add(designModeMenuItem);
            MenuView.MenuItems.Add(designMenuView);
        }

        private void BindFileMenu()
        {
            
            var fileDialog = new NewFileDialogService();
            fileMenuView.AddMenuActions(new() {
                { "Открыть",       async ()=>{
                    
                    //fileDialog.OpenFile("Открыть", "*.xml|Xml files", Directory.GetCurrentDirectory(), OnFileOpened);

                    var p = ((string, string, byte[]) filedata) => {
                        try
                        {
                            var xml = Encoding.Default.GetString(filedata.Item3);
                            xmlText = xml;
                            LoadXmlText(xml);
                        }
                        catch(Exception ex)
                        {
                            this.Error(ex);
                        }
                    };
                    ((dynamic)(this.GetAppRootView().control)).OpenFileDialog( p );

                 }},
                { "Сохранить",      ()=>{ OnSaveFile();
                } },
                { "Сохранить как",  ()=>{ 
                    fileDialog.SaveFile(xmlText=layoutView.ToXmlText()); 
                } }
            });
            MenuView.MenuItems.Add(fileMenuView);
        }

        public virtual void OnSaveFile()
        {
            PutAction($"Сохранение файла {filename}", () =>
            {
                xmlText = layoutView.ToXmlText();
                this.Info(xmlText);
                File.WriteAllText(filename, xmlText);
            });
        }

        protected virtual void OnFileOpened(string filename)
        {
            LoadXmlFile(this.filename = filename);
            WatchFileChanges(this.filename, OnFileChanged);
            var name = this.filename.Substring(Math.Max(this.filename.LastIndexOf("\\"), this.filename.LastIndexOf("/")) + 1);
            var form = GetAppRootView().ToControl().GetAppForm();
            if (form is not null)
            {
                form.Text = name;
            }
            else
            {
                OnLoad += (s, e) => GetAppRootView().ToControl().GetAppForm().Text = name;
            }
        }

        private void BindNavMenu()
        {
            controlView.Config.OnChanged += (s, e) =>
            {
                switch (e.Target.Action)
                {
                    case "added":
                        navMenuView.MenuItems.Add(new MenuView()
                        {
                            Text = e.Target.Key,
                            OnClick = (s, evt) =>
                            {
                                OnNavSelected(e.Target.Key);
                            }
                        });
                        break;
                    case "removed":
                        navMenuView.MenuItems.Remove(navMenuView.MenuItems.FirstOrDefault(m => m.Text == e.Target.Key));
                        break;
                }
            };
            MenuView.MenuItems.Add(navMenuView);
        }

        public List<string> GetNavOptions()
        {
            return ((INavView)controlView).GetNavOptions();
        }

        public void OnNavSelected(string selected)
        {
            ((INavView)controlView).OnNavSelected(selected);
        }

        public void AddNavOption(string selected, AppViewBase view)
        {
            ((INavView)controlView).AddNavOption(selected, view);
        }

        public void RemoveNavOption(string selected)
        {
            ((INavView)controlView).RemoveNavOption(selected);
        }
    }


    [Label("Представление сервиса управления контентом")]
    public class AppViewContent<TContentView> : AppViewRoot
        where TContentView : AppViewBase
    {
        [Label("control View")]
        public virtual TContentView controlView { get; }

        public AppViewContent()
        {
            LayoutFill(controlView = typeof(TContentView).New<TContentView>());
        }
    }



    /*public ViewItem SelectedViewItem { get => Get<ViewItem>(nameof(SelectedViewItem)); set => Set(nameof(SelectedViewItem), value); }

    

    public FlexView FlexView = new();

    private KeyboardService keyboardService = new();

    protected ChangingList<IViewTool> tools = new() { };
    protected IViewTool tool;

    public AppViewRoot()
    {
        this.SelectedViewItem = new ButtonView();
        this.Top = MenuView;
        this.LayoutFill( FlexView);

        this.Bottom = StatusView;
        this.BottomHeight = 30;
        this.StatusView.StartAsync();
        this.InitFocusModel();
        this.InitTools();
        this.InitMainMenu();
        this.InitKeyboardConfig();


        this.TopHeight = 30;
    }


    public void ValidateViews()
    {

    }



    private void InitKeyboardConfig()
    {
        keyboardService.Setup(this, new());
    }

    private void InitFocusModel()
    {
        focusModel.NowInFocus.OnChanged += OnFocusChanged;
        FocusedViews.OnChanged += (s, e) =>
        {
            StatusView.Text = $"Выделено ({FocusedViews.Count}) элемента";
        };
        this.AddPropertyChangeListener(nameof(SelectedViewItem), evt =>
        {
            if (evt.Before is not null)
            {
                var p = (AppViewBase)evt.Before;
                var box = new Panel();
                p.control.Parent.ReplaceControl(p.control, box);
            }
            if (evt.After is not null)
            {
                var p = (AppViewBase)evt.After;
            }
        });
    }

   







    


    private void InitTools()
    {
        tools.Add(new SelectViewItemTool(this));
    }

    private void OnControlsChanged(object sender, IChangingListEvent e)
    {


    }



    private void OnViewItemClicked(object sender, EventArgs args)
    {
        this.Info(sender);
    }



    private void OnFocusChanged(object sender, ChangingDictionaryEvent<int, ViewItem> e)
    {
        switch (e.Target.Action)
        {
            case "added": FocusedViews.Add(e.Target.Value); break;
            case "removed": FocusedViews.Remove(e.Target.Value); break;
            case "updated": FocusedViews.Remove(e.Target.Value); FocusedViews.Add(e.Target.Value); break;
            default: throw new Exception();
        }
        StatusView.Text = $"{FocusedViews.Count()}";
    }

    public void Add(ViewNode item)        
        => ((ILayoutView)FlexView).Add(item);        

    public void Remove(ViewNode item)
        => ((ILayoutView)FlexView).Remove(item);

}



/// <summary>
/// Базовый интерфейс инструментов управления элементами.
/// </summary>
public interface IViewTool
{
    Action<object, EventArgs> OnClick(ViewItem view);
}


/// <summary>
/// Инструмент выбора элемента для редактирования.
/// </summary>
public class SelectViewItemTool : IViewTool
{
    private readonly AppViewRoot root;

    public SelectViewItemTool(AppViewRoot root)
    {
        this.root = root;
    }



    public Action<object, EventArgs> OnClick(ViewItem view)
    {
        return (s, e) =>
        {
            root.SelectedViewItem = view;
        };
    }
}*/
}
