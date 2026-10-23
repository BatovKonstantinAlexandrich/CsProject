using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using MaterialApp.Win_MaterialModule.DialogModule;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Win_BusinessApp.CommonModel;

namespace WinFormsApp.Console.Win_MaterialModule
{
    public class DialogService: ControlsFactory, IClientAPI
    {

        public void ShowText(Form form, string title, string message)
        {
            var dialog = new MaterialSkin.Controls.MaterialDialog(form, title, message);
            dialog.ShowDialog(form);
            dialog.Location = form.Location;

        }

        //большая задержка при вызове, лучше н еиспользовать

        public string SelectSingle(Form form, string title, IEnumerable<string> options)
        {
            form.Hide();
            string result = null;
            bool completed = false;
            Task.Run(() =>
            {
                var checkList = CreateListBoxView(
                    options.ToList(),
                    selected =>
                    {
                        result = selected;
                    });
                SubmitDialogControl dialogContentForm = null;
                dialogContentForm = new SubmitDialogForm(title, checkList, (success) =>
                {
                    completed = true;
                    dialogContentForm.Close();
                });
                var dialogResult = dialogContentForm.ShowDialog();
            });
            while (!completed)
                Thread.Sleep(100);
            form.Show();
            return result;
        }

        public SubmitDialogControl CreateSubmitDialog(string title, Control view, Action<bool> oncompleted = null)
        {
            return new SubmitDialogForm(title, view, (success) =>
            {
                if (oncompleted is not null)
                    oncompleted(success);
            });
        }
        public List<string> SelectMany(Form form, string title, IEnumerable<string> options)
        {
            form.Hide();
            List<string> result = null;
            bool completed = false;
            Task.Run(() =>
            {
                var checkList = CreateCheckedListBox(options.ToList());
                //checkList.EnableDragAndDropPosition((i1, i2) =>
                //{

                //});
                SubmitDialogControl dialogContentForm = null;
                dialogContentForm = new SubmitDialogForm(title, checkList, (success) =>
                {
                    result = checkList.Items.Where(p => p.Checked).Select(p => p.Text).ToList();
                    completed = true;
                    dialogContentForm.Close();
                });
                var dialogResult = dialogContentForm.ShowDialog();
            });
            while (!completed)
                Thread.Sleep(100);
            form.Show();
            return result;
        }
        public List<string> SelectMany(Form form, IEnumerable<string> options)
        {
            form.Hide();
            List<string> result = null;
            bool completed = false;
            Task.Run(() =>
            {
                var checkList = CreateCheckedListBox(new List<string>() { "iphone 11", "sampsung gaglaxy" });
                SubmitDialogControl dialogContentForm = null;
                dialogContentForm = new SubmitDialogForm("test", checkList, (success) =>
                {
                    result = checkList.Items.Where(p => p.Checked).Select(p => p.Text).ToList();
                    completed = true;
                    dialogContentForm.Close();
                });
                var dialogResult = dialogContentForm.ShowDialog();
            });
            while (!completed)
                Thread.Sleep(100);
            form.Show();
            return result;
        }
        public List<string> SelectMany(Form form, IEnumerable<string> options, Action<bool, List<string>> oncompleted)
        {
            form.Hide();
            List<string> result = null;
            bool completed = false;
            Task.Run(() =>
            {
                var checkList = CreateCheckedListBox(new List<string>() { "iphone 11", "sampsung gaglaxy" });
                SubmitDialogControl dialogContentForm = null;
                dialogContentForm = new SubmitDialogForm("test", checkList, (success) =>
                {
                    result = checkList.Items.Where(p => p.Checked).Select(p => p.Text).ToList();
                    completed = true;
                    if (oncompleted is not null)
                    {
                        oncompleted(success, result.ToList());
                    }
                    dialogContentForm.Close();
                });
                var dialogResult = dialogContentForm.ShowDialog();
            });
            while (!completed)
                Thread.Sleep(100);
            form.Show();
            return result;
        }
        public List<string> SelectMany(Form form, Dictionary<string, bool> options)
        {
            return SelectMany(form, options, (s, e) => { });
        }
        public List<string> SelectMany(Form form, Dictionary<string, bool> options, Action<bool, List<string>> oncompleted)
        {
            form.Hide();
            List<string> result = null;
            bool completed = false;
            Task.Run(() =>
            {
                var checkList = CreateCheckedListBox(options.Keys.ToList());
                SubmitDialogControl dialogContentForm = null;
                dialogContentForm = new SubmitDialogForm("Выберите несколько элементов", checkList, (success) =>
                {
                    result = checkList.Items.Where(p => p.Checked).Select(p => p.Text).ToList();
                    completed = true;
                    if (oncompleted is not null)
                    {
                        oncompleted(success, result.ToList());
                    }
                    dialogContentForm.Close();
                });
                var dialogResult = dialogContentForm.ShowDialog();
            });
            while (!completed)
                Thread.Sleep(100);
            form.Show();
            return result;
        }

        public async Task<List<string>> SelectMany(Form form, string title, Dictionary<string, bool> options)
        {
            form.Hide();
            List<string> result = null;
            bool completed = false;

            var checkList = CreateCheckedListBox(options);
            //checkList.EnableDragAndDropPosition((i1, i2) =>
            //{

            //});
            SubmitDialogControl dialogContentForm = null;
            dialogContentForm = new SubmitDialogForm(title, checkList, (success) =>
            {
                result = checkList.Items.Where(p => p.Checked).Select(p => p.Text).ToList();
                completed = true;
                dialogContentForm.Close();
            });
            var dialogResult = dialogContentForm.ShowDialog();

            form.Show();
            await Task.CompletedTask;
            return result;
        }

        public TModel Create<TModel>(Form form) where TModel : class
        {
            TModel result = null;
            bool completed = false;
            Task.Run(() =>
            {
                var contentPane = CreateInputForm<TModel>();
                SubmitDialogControl dialogContentForm = null;
                dialogContentForm = new SubmitDialogForm("test", contentPane, (success) =>
                {
                    var dictionary = contentPane.GetValue("target");

                    result = (TModel)typeof(TModel).New();
                    foreach (var property in typeof(TModel).GetInputProperties())
                    {
                        TextDataSetter.SetValue(result, property, dictionary.GetProperty(property));
                    }

                    dialogContentForm.Close();
                    completed = true;
                });
                var dialogResult = dialogContentForm.ShowDialog();
            });
            while (!completed)
                Thread.Sleep(100);
            return result;
        }

        private ControlBase CreateInputForm<TModel>() where TModel : class
        {
            throw new NotImplementedException();
        }

        public TModel Update<TModel>(Form formController, TModel target) where TModel : class
        {
            TModel result = null;
            bool completed = false;
            Task.Run(() =>
            {
                ControlBase contentPane = Create<TModel>();
                SubmitDialogControl dialogContentForm = null;
                dialogContentForm = new SubmitDialogForm("test", contentPane, (success) =>
                {
                    var dictionary = contentPane.GetValue("target");
                    contentPane.OnInput += (string key, string value) =>
                    {

                        TextDataSetter.SetValue(target, key, value);

                    };

                    result = (TModel)typeof(TModel).New();
                    foreach (var property in typeof(TModel).GetInputProperties())
                    {
                        TextDataSetter.SetValue(result, property, dictionary.GetProperty(property));
                    }

                    dialogContentForm.Close();
                    completed = true;
                });
                var dialogResult = dialogContentForm.ShowDialog();
            });
            while (!completed)
                Thread.Sleep(100);
            return result;
        }

        private ControlBase Create<TModel>() where TModel : class
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Отображает модальное окно с зданным компонентом и кнопками Ок, Отмена
        /// </summary>
        /// <returns>true если ок false если cancel</returns>
        public bool Dialog(Form formController, string title, Control control)
        {
            bool result = false;
            bool completed = false;
            Task.Run(() =>
            {

                SubmitDialogControl dialogContentForm = null;
                dialogContentForm = new SubmitDialogForm(title, control, (success) =>
                {
                    result = success;
                    dialogContentForm.Close();

                    completed = true;
                });
                var dialogResult = dialogContentForm.ShowDialog();
            });
            formController.Hide();
            while (!completed)
                Thread.Sleep(100);
            formController.Show();
            return result;
        }

        public void SubmitDialog(string title, object control, Action<object> value)
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

        public Point GetMousePosition()
        {
            throw new NotImplementedException();
        }

        public Point PointToClient(Point point)
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
    }
}
