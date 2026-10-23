using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class AppViewsExtensions
{
    /// <summary>
    /// Создание формы ввода
    /// </summary>
    /// <param name="controller">тип</param>
    /// <param name="action">метод</param>
    /// <returns>модел формы</returns>
    public static InputFormModel GetInputFormModel(this Type controller, string action)
    {
#pragma warning disable CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
        /*var validationResults = MethodInfo.GetCurrentMethod().Validate(new object[] {
            controller, action
        });
        if(validationResults.Where(kv => kv.Value.Count()>0).Count() > 0)
        {
            throw new Exception($"Аргументы вызова невалидны: {validationResults.ToJson()}");
        }*/
#pragma warning restore CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
        var methodInfo = controller.GetMethods().FirstOrDefault(m => m.Name == action);
        var actionModel = new MyActionModel();
        actionModel.Name = action;
        actionModel.Type = methodInfo.ReturnType.GetTypeName();
        actionModel.Method = "POST";
        actionModel.Attributes = controller.GetMethodAttributes(action);
        //actionModel.EnsureIsValide();

        var formModel = new InputFormModel();
        formModel.Title = actionModel.Name;
        formModel.Description = controller.GetMethodDescription(action);
        formModel.FormFields = new();

        formModel.Item = new Dictionary<string, object>();
        formModel.ItemType = formModel.Item.GetTypeName();
        foreach (var param in controller.GetActionParameters(action))
        {
            try
            {
                var paramModel = new MyParameterDeclarationModel();
                paramModel.Name = param.Name;
                paramModel.Type = param.ParameterType.GetTypeName(); ;
                paramModel.IsOptional = param.IsOptional;
                paramModel.Position = param.Position;
                //paramModel.Attributes = controller.GetArgumentAttributes(action, param.Name);
                actionModel.Parameters[param.Name] = paramModel;
                paramModel.Attributes = new(controller.GetMethodParameterAttributes(action, param.Name));
                var factory = new FormFieldFactory();
                ///var field = formModel.CreateFormField(paramModel.Name, paramModel.Type, paramModel.Attributes);
                var field = factory.CreateField(paramModel.Name, paramModel.Type, paramModel.Attributes, changed => {
                });
                formModel.FormFields.Add(field);
            }
            catch (Exception ex)
            {
                if (param.IsOptional == false)
                    throw;
                formModel.Error($"{ex}");
            }
        }
        formModel.Item = new Dictionary<string, object>();
        formModel.Json = "{}";
        formModel.Container = "v-group";
        formModel.IsValid = false;
        //formModel.EnsureIsValide();
        //formModel.Info(formModel.ToJsonOnScreen());
        return formModel;

    }


    public static InputFormModel GetInputFormModel(this ActionModel model)
    {
#pragma warning disable CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
        /*var validationResults = MethodInfo.GetCurrentMethod().Validate(new object[] {
            controller, action
        });
        if(validationResults.Where(kv => kv.Value.Count()>0).Count() > 0)
        {
            throw new Exception($"Аргументы вызова невалидны: {validationResults.ToJson()}");
        }*/
#pragma warning restore CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.

        var actionModel = new MyActionModel();
        actionModel.Name = model.Name;
        actionModel.Type = model.Type;
        actionModel.Method = "POST";
        actionModel.Attributes = new Dictionary<string, string>(model.Attributes.Select(p => new KeyValuePair<string, string>(p.Name, p.Value)));
        //actionModel.EnsureIsValide();

        return CreateFormModel(model);

    }

    private static InputFormModel CreateFormModel(ActionModel actionModel)
    {
        var formModel = new InputFormModel();
        formModel.Title = actionModel.Name;
        formModel.Description = actionModel.Description;
        formModel.FormFields = new();

        formModel.Item = new Dictionary<string, object>();
        formModel.ItemType = formModel.Item.GetTypeName();
        foreach (var param in actionModel.Parameters)
        {
            try
            {
                var field = formModel.CreateFormField(param.Name, param.Type,
                    new Dictionary<string, string>(param.Attributes.Select(a => new KeyValuePair<string, string>(a.Name, a.Value))));
                formModel.FormFields.Add(field);
            }
            catch (Exception ex)
            {

                formModel.Error($"{ex}");
            }
        }
        formModel.Item = new Dictionary<string, object>();
        formModel.Json = "{}";
        formModel.Container = "v-group";
        formModel.IsValid = false;
        //formModel.EnsureIsValide();
        //formModel.Info(formModel.ToJsonOnScreen());
        return formModel;
    }
    public static EditView CreateEditView(this object target, Action<bool, object> OnEditCompleted = null)
    {
        return EditView.CreateEditViewBase(target, OnEditCompleted);
    }
}

