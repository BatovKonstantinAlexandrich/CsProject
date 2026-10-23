using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.Others;

namespace Console_CommonModel.CommonLib.LibModels.Console_XmlModule
{
    /// <summary>
    /// Функции для интеграции XML-представлений
    /// </summary>
    [Label("Представление отчёта XML")]
    public partial class ReportView : AppViewBase
    {
        //protected Control control;

        private ProgramBackground background = new ProgramBackground();
        protected string filename;
        protected ReportComponent document;

        public ReportView()
        {
            //MinimumSize = new(640, 480);
        }


        ///// <summary>
        ///// Выполнить XML документ из файла
        ///// </summary>
        ///// <param name="filename">имя файла</param>        
        //public static ReportView Open(Control control, string filename)
        //{
        //    var form = new ReportView();
        //    control.Size = new(1200, 968);

        //    form.OnFileOpen(control, filename);
        //    return form;
        //}

        private void BindFile(string filename)
        {
            string dir;
            if (!filename.Contains(Path.PathSeparator))
            {
                dir = Directory.GetCurrentDirectory();
            }
            else
            {
                dir = filename.Substring(0, filename.LastIndexOf(Path.PathSeparator));
            }

            Task.Run(() =>
            {


                string path = filename.Substring(0, 3);//@"D:\";

                FileSystemWatcher watcher = new FileSystemWatcher();
                watcher.Path = path;
                watcher.NotifyFilter = NotifyFilters.LastWrite
                                     | NotifyFilters.FileName
                                     | NotifyFilters.DirectoryName;

                watcher.Filter = "*.xml";
                watcher.IncludeSubdirectories = true;

                watcher.Created += OnFileChanged;
                watcher.Changed += OnFileChanged;
                watcher.Deleted += OnFileChanged;
                watcher.Renamed += OnFileRenamed;

                watcher.EnableRaisingEvents = true;

                Console.WriteLine("Monitoring " + path + " for changes...");
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            });
        }

        public void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            this.Info(e.Name);
            this.Info(filename);
            Console.WriteLine($"{e.ChangeType}: {e.Name}");
            background.PutAction(() =>
            {
                UpdateView(document = XmlNodeCompiler.CompileXmlFile(filename));
            });
        }

        public void OnFileSave(string filename)
        {
            if (document is null)
                throw new ArgumentNullException(nameof(document));
            var text = document.ToJsonOnScreen();
            File.WriteAllText(filename, text);
        }

        public void OnFileSaveAs()
        {
            if (document is null)
                throw new ArgumentNullException(nameof(document));
            var text = document.ToJsonOnScreen();

            BindFile(filename = SaveFile(text));
        }

        private string SaveFile(string text)
        {
            throw new NotImplementedException();
        }

        public void OnFileClose()
        {

            document = null;
            UpdateView(document);
            BindFile(filename = null);
        }

        /// <summary>
        /// Реализация должна отобразить document
        /// </summary>
        public virtual void UpdateViewRep(ReportComponent document)
        {
            if (control is not null)
            {
                UpdateView(document);
            }
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"Renamed: {e.OldName} to {e.Name}");
        }


        public void OnFileOpen(string filename)
        {
            this.filename = filename;
            if (!File.Exists(filename))
                throw new Exception($"Файл не найден: {filename}");
            document = XmlNodeCompiler.CompileXmlFile(filename);
            this.Info(document);
            BindFile(this.filename = filename);

            UpdateView(document);
        }

        /// <summary>
        /// Обновление представления
        /// </summary>
        /// <param name="document"></param>
        public void UpdateView(ReportComponent document)
        {
            //this.control = control;
            //control.Controls.Clear();
            //if (document is null)
            //    return;
            //var designerForm = new Form() { Dock = DockStyle.Fill };
            //control.Controls.Add(designerForm);

            //designerForm.Controls.Clear();
            //var body = document.GetBody();
            //foreach (var kv in body.HierElements)
            //{
            //    try
            //    {
            //        this.Info(kv.Key);
            //        var view = (AppViewBase)kv.Value.Item.Target;
            //        designerForm.Controls.Add(view.ToControl());
            //    }
            //    catch (Exception ex)
            //    {
            //        this.Error(ex);
            //    }
            //}
        }

    }
}
