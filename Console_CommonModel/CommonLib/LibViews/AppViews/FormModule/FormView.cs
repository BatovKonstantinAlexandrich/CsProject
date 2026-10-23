using Mysqlx.Crud;

using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text.Json.Serialization;

using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using Console_CommonModel.CommonLib.LibViews.AppViewModules.MenuModule;

using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.FormModule
{


    //[Label("Модель представления формы")]
    //[Description("Содержит представления главного меню, панель статуса, панель инструментов")]
    //public class FormView : AppViewBase
    //{
    //    public ChangingList<string> properties = new();

    //    [JsonIgnore]
    //    [Newtonsoft.Json.JsonIgnore]
    //    [NotMapped]
    //    public ChangingList<InputFormField> formFields = new();

    //    [JsonIgnore]
    //    [Newtonsoft.Json.JsonIgnore]
    //    public object target = null;



    //    [JsonIgnore]
    //    [Newtonsoft.Json.JsonIgnore]
    //    private FormFieldService service = new();
    //    public FormView() : this(new UserAccount()) { }
    //    public FormView(object entity)
    //    {
    //        target = entity;
    //        this.properties.OnChanged += (s, e) =>
    //        {
    //            switch (e.Target.Action)
    //            {
    //                case "added":
    //                    {
    //                        var property = e.Target.Item.ToString();
    //                        var field = service.CreateFormField2(target, property);
    //                        formFields.Add(field);
    //                        break;
    //                    }
    //                case "removed":
    //                    {
    //                        var property = e.Target.Item.ToString();
    //                        var field = formFields.FirstOrDefault(field => field.Name == property);
    //                        if (field is not null)
    //                        {
    //                            formFields.Remove(field);
    //                        }
    //                        break;
    //                    }
    //            }
    //        };
    //        var properties = target.GetType().GetOwnPropertyNames();

    //        foreach (var property in properties)
    //        {
    //            int sleep = 0;
    //            sleep += 500;
    //            //Task.Run(() => {

    //            //    Thread.Sleep(sleep);

    //            this.properties.Add(property);
    //            //});
    //        }


    //        this.Info(formFields.ToJsonOnScreen());
    //        //this.UpdateViewModel(entity);
    //    }



    //    protected void UpdateViewModel([NotNullNotEmpty] object target)
    //    {
    //        MethodBase.GetCurrentMethod().EnsureAgumentsIsValid(target);
    //        var properties = target.GetOwnInputProperties();
    //        List<Task> tasks = new List<Task>();
    //        formFields.Clear();
    //        foreach (var property in properties)
    //        {
    //            var pproperty = property;
    //            var pvalue = target.GetValue(property);
    //            var pattrs = target.GetType().GetPropertyAttributes(property);
    //            var pinfo = target.GetType().GetProperty(property);
    //            //tasks.Add(Task.Run(() => {
    //            var factory = new FormFieldFactory();

    //            formFields.Add(factory.CreateField(target, pproperty, pvalue, (text) =>
    //            {
    //                TextDataSetter.SetValue(target, property, text);
    //            }, pinfo, pattrs, text =>
    //            {
    //                /*if(this.OnFormFieldValueChanged is not null)
    //                {
    //                    this.OnFormFieldValueChanged(property, text);
    //                }*/
    //            }));
    //            //}));
    //        }

    //        Task.WaitAll(tasks.ToArray());

    //        var fieldsList = formFields.ToList();
    //        fieldsList.Sort((p1, p2) =>
    //        {
    //            return
    //            p1.Order != p2.Order ? p1.Order - p2.Order :
    //            p1.Order == p2.Order ?
    //            p1.IsCollection && !p2.IsCollection ? -1 :
    //            !p1.IsCollection && p2.IsCollection ? 1 :
    //                0 : 0;
    //        });
    //    }


    //}

}
