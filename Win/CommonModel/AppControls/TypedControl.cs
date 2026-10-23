using Google.Protobuf.Reflection;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

using WinFormsApp.AppComponents.AppControls;
using WinFormsApp.AppViews.BaseComponent;

using WinFormsDesignerApp.Console_InputModule.ViewEvents.List;
using WinFormsApp.AppViews.InputModule;

namespace WinFormsDesignerApp
{
    public partial class TypedControlDefault : ControlBase
    {
        private System.Windows.Forms.Control Content;

        public void SetControl(System.Windows.Forms.Control Content)
            =>this.LayoutFill(this.Content = Content);

        public TypedControlDefault()
        {
            this.AutoSize = true;
        }
    }


    public partial class TypedControl : ControlBase 
    {
        public ChangingList<string> Options = new();
        private ChangeSupport Settings = new();
        private object Target;

        public TypedControl( )
        {
            InitializeComponent();
            this.Options.OnChanged += this.OnOptionsChanged;
            /*this.searchControl1.OnOptionSelected += (selected) => {                
                OnSelected(selected);
            };*/
            this.LayoutLeft(new ComponentsFactory().controlsFactory.CreateButton("add", () => {
                this.searchControl1.SearchOptions.Add("1");
                this.Options.Add("1");
                this.Bind<ViewItem>();
                this.searchControl1.SearchOptions.Add("1");
                this.Options.Add("1");

            }));

            var control = this;
           /* int c = control.searchControl1.SelectList.Items.Count;
            System.Console.WriteLine(control.searchControl1.SelectList.Items.Count);
            control.searchControl1.SearchOptions.Add("123");
            System.Console.WriteLine(control.searchControl1.SelectList.Items.Count);
            if (control.searchControl1.SelectList.Items.Count == c)
                throw new Exception("Свойcтва выбора не добавились");*/
        }

        private void OnOptionsChanged(object sender, IChangingListEvent e)
        {
            switch(e.Target.Action)
            {
                case "added": OnOptionAdded(e.Target.Item); break;
                case "removed": OnOptionDelete(e.Target.Item); break;
                default: throw new NotImplementedException();
            }
        }

        private void OnOptionDelete(object item)
        {
            this.searchControl1.SearchOptions.Remove(item.ToString());
            //this.searchControl1.Items.Remove(item);
        }

        private void OnOptionAdded(object item)
        {
            this.searchControl1.SearchOptions.Add(item.ToString());
            //this.materialComboBox1.Items.Add(item);
        }

        public void OnSelected(string typeName)
        {
            try
            {                
                var control = new TableFieldsView();
                this.materialCard1.LayoutFill(control);
                control.GetType().GetMethod("Configure").MakeGenericMethod(typeName.ToType()).Invoke(control, new object[] { });
            }
            catch(Exception ex)
            {
                this.OnError(nameof(OnSelected), new() { typeName}, ex);
            }
        }

        private void OnError(string action, List<string> args, Exception ex)
        {
            this.Error($"Ошибка при выполнении: {action} {args.ToJson()}: {ex.ToTextDocument()}");
        }

        public void Bind<GenericType>()
        {
            this.Options.Clear();
            this.Target = null;
            this.materialCard1.Controls.Clear();
            var typeNames = GetExtendedTypesNames<GenericType>().ToList();
            if (typeNames is null || typeNames.Count() == 0)
                throw new Exception();

            /*System.Console.WriteLine(this.searchControl1.SelectList.Items);
            //this.searchControl1.SelectList.Items.AddRange(typeNames);
            this.searchControl1.SearchOptions.AddRange(typeNames);
            System.Console.WriteLine(this.searchControl1.SelectList.Items.Count);
            System.Console.WriteLine(this.searchControl1.SearchOptions.Count());
            this.Options.AddRange(typeNames);*/
        }


        /// <summary>
        /// Список типов реализующий базовый
        /// </summary>        
        private IEnumerable<Type> GetExtendedTypes<GenericType>() =>
            ServiceFactory.Get().GetTypesExtended<GenericType>().Where(p => !p.IsAbstract);
        private IEnumerable<string> GetExtendedTypesNames<GenericType>() =>
            GetExtendedTypes<GenericType>().Select(p => p.GetTypeName());


        public static void Run()
        {
            var control = new TypedControl();
            control.Bind<AppViewBase>();
            control.RunAtForm();
        }
         
    }
}
