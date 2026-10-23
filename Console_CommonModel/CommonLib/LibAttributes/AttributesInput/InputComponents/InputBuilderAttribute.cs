using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbConnectionStrings;
using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibAttributes.AttributesInput.InputComponents
{

    public interface IInputBuilder<TResult>
    {
        /// <summary>
        /// ИСп. для обновления данных
        /// </summary>
        public Action<TResult> UpdateResult { get; set; }
        public void SetTargetValue(TResult value);
        public void OnInit();
        public TResult OnBuild();
        public void OnDestroy();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class EditViewBuilder<TResult> : IInputBuilder<TResult> where TResult : class
    {
        public TResult Model { get; private set; }
        public EditView EditView { get; private set; }


        /// <summary>
        /// ИСп. для обновления данных
        /// </summary>

        public Action<TResult> UpdateResult { get; set; } = r => { };


        public EditViewBuilder()
        {
        }

        public void OnInit()
        {
            EditView = EditView.CreateEditViewBase(Model = typeof(TResult).New<TResult>(), OnEditCompleted);
        }

        private void OnEditCompleted(bool success, object result)
        {
            if (success)
            {
                UpdateResult(OnBuild());
            }
        }

        public TResult OnBuild()
        {
            EditView.ShowDialog();
            return Model;
        }

        public void OnDestroy()
        {

        }

        public void SetTargetValue(TResult value)
        {
            //this.EditView.SetTargetValue(value, OnEditCompleted);
        }
    }




    public interface IPropertyBuilder<TBuildResult> : IInputBuilder<TBuildResult>
    {
        public void BuildPropertyEditor(object taget, string property);
    }


    public class SqlServerConnectionStringBuilder : IInputBuilder<string>
    {
        public Action<string> UpdateResult { get; set; }
        public EditView EditView { get; private set; }

        public string Model { get; private set; }
        public SqlServerConnectionString ConnectionString { get; private set; }

        public void OnEditCompleted(bool success, dynamic data)
        {

        }

        public void OnInit()
        {
            EditView = EditView.CreateEditView(ConnectionString = new SqlServerConnectionString(), OnEditCompleted);
        }

        public string OnBuild()
        {
            EditView.ShowDialog();
            return ConnectionString.ToString();
        }

        public void OnDestroy()
        {
        }

        public void SetTargetValue(string value)
        {
            ConnectionString.SetFromString(value);
            foreach (var kv in ConnectionString.ToTextDictionary())
            {
                var field = EditView.FormFields.FirstOrDefault(f => f.Name == kv.Key);
                if (field is not null)
                {
                    field.OnInput(kv.Value);
                }
            }
        }
    }



    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TBuildResult"></typeparam>
    /// <typeparam name="TBuilder"></typeparam>
    public class InputBuilderAttribute<TBuildResult, TBuilder> : BaseInputAttribute
        where TBuilder : class, IInputBuilder<TBuildResult>
    {
        public InputBuilderAttribute()
        {
        }

        public InputBuilderAttribute(string InputType) : base(InputType)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taget"></param>
        /// <param name="property"></param>
        public void BuildPropertyEditor(object target, string property)
        {
            var builder = typeof(TBuilder).New<TBuilder>();
            builder.UpdateResult += res => target.SetValue(property, res);
            builder.OnInit();
            builder.SetTargetValue((TBuildResult)target.GetValue(property));

            var result = builder.OnBuild();
        }

    }
}
