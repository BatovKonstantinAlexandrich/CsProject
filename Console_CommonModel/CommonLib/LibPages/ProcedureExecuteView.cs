using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbExecutorService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata;
using Console_CommonModel.CommonLib.LibModels.Console_ModelsModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.TextModule;

using Microsoft.EntityFrameworkCore;

using System.Data;

namespace Console_CommonModel.CommonLib.LibPages
{
    [Label("Представление вызова хранимой процедуры")]
    public partial class ProcedureExecuteView : AppViewBase
    {
        public ProcedureMetadata model { get; set; }
        public Dictionary<string, List<string>> validationResults { get; private set; }
        public bool isValid { get; set; }

        public Action<DataTable> oncompleted;
        public TextView errorsLabel;

        public ProcedureExecuteView()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Создание формы управления параметрами выполнения процедуры
        /// </summary>
        public void SetModel(ISqlExecutor dataSource, ProcedureMetadata model, Action<DataTable> oncompleted, Dictionary<string,object> values=null)
        {
            try
            {
                this.model = model;
                this.oncompleted += (dataTable) =>
                {
                };


                var procedureModel = GetProcedureModel(model);
                if (procedureModel is null)
                    throw new NullReferenceException("Не был получен ProcedureModel");

                ProcedureMetadata selectedProcedureMetadata = this.model;
                var panel = new AppViewBase();


                Dictionary<string, object> args = new();
                var inputParameters = selectedProcedureMetadata.ParametersMetadata
                        .Where(kv => kv.Value.ParameterMode.ToLower() == "in").ToList();

                inputParameters.Sort((p2, p1) => p2.Value.OrdinalPosition - p1.Value.OrdinalPosition);

                Dictionary<string, TextView> errorLabels = new();
                var submitButton = CreateButton("Выполнить", () =>
                {
                    try
                    {
                        var results = dataSource.ExecuteProcedure(selectedProcedureMetadata.FullName, args);
                        if (oncompleted is not null)
                        {
                            oncompleted(results);
                        }
                        errorsLabel.Text = "";
                    }
                    catch (Exception ex)
                    {
                        errorsLabel.Text = ex.Message;
                    }
                });
                Action validate = () =>
                {
                    validationResults = Validate(args);
                    isValid = !validationResults.Any(kv => kv.Value.Count() > 0);
                    foreach (var kv in validationResults)
                    {
                        string text = "";
                        foreach (var message in kv.Value)
                        {
                            text += $"{message}\n";
                        }
                        if (errorLabels.ContainsKey(kv.Key))
                            errorLabels[kv.Key].Text = text;
                    }
                    submitButton.Enabled = isValid;
                };
                var data = new Dictionary<string, object>();

                EditView editView = new();
                
                foreach (var kv in inputParameters)
                {
                    var parameterName = kv.Key;
                    var pname = parameterName.Replace("@", "").ToLower();
                    if (values is not null && values.ContainsKey(pname))
                    {
                        args[parameterName] = values[pname];
                        continue;
                    }
                    args[parameterName] = null;
                    ParameterMetadata parameterMetadata = kv.Value;
                    var parameterPanel = new AppViewBase();
                    var parameterModel = procedureModel.Parameters.FirstOrDefault(p => p.Name == parameterName);                                        
                    var formField = new InputFormField();
                    formField.Name = parameterName;
                    formField.Label = parameterModel.Label;
                    formField.Description = parameterModel.Description;
                    formField.Help = parameterModel.Help;
                    formField.Icon = parameterModel.Icon;
                    formField.Attributes = new();
                    if (parameterModel.Attributes is not null)
                        foreach (var a in parameterModel.Attributes)
                        {
                            formField.Attributes[a.AttributeType] = a.AttributeValue;
                        }
                    formField.Value = null;
                    editView.FormFields.Add(formField);
                    var factory = new FormFieldFactory();
                    formField.OnInput += text => {
                        data[formField.Name] = text;
                    };
                    switch (parameterMetadata.DataType)
                    {
                        case "int":
                            {
                                formField.DataType = typeof(System.Int32).GetTypeName();
                                formField.Type = factory.GetInputType(typeof(System.Int32), formField.Attributes);


                                /*var label = CreateText(parameterModel is not null ? parameterModel.Label : parameterName);
                                TextView errors = CreateText("");
                                var input = CreateInputNumber(null, (value) =>
                                {
                                    args[parameterName] = value;
                                    validate();
                                });
                                errorLabels[parameterName] = errors;
                                parameterPanel.LayoutTop(label);
                                parameterPanel.LayoutTop(input);
                                parameterPanel.LayoutTop(errors);
                                panel.LayoutTop(parameterPanel);*/
                                break;
                            }
                        case "nvarchar":
                            {
                                formField.DataType = typeof(System.String).GetTypeName();
                                formField.Type = factory.GetInputType(typeof(System.String), formField.Attributes);
                                /*var parameterPanel = new AppViewBase();
                                var parameterModel = procedureModel.Parameters.FirstOrDefault(p => p.Name == parameterName);
                                var label = CreateText(parameterModel is not null ? parameterModel.Label : parameterName);

                                var input = CreateTextbox(null, (value) =>
                                {
                                    args[parameterName] = value;
                                    validate();
                                });
                                var errors = CreateText("");

                                errorLabels[parameterName] = errors;
                                parameterPanel.LayoutTop(label);
                                parameterPanel.LayoutTop(input);
                                parameterPanel.LayoutTop(errors);
                                panel.LayoutTop(parameterPanel);*/
                                break;
                            }
                        default: throw new NotSupportedException($"Тип данных не поддерживается: {parameterMetadata.DataType}");
                    }
                }
                editView.OnEditCompleted += (s, e) => {


                    try
                    {
                        dataSource.ExecuteProcedure(model.FullName,
                            data);
                        dataSource.ExecuteProcedure(model.FullName,
                            new Dictionary<string, object>(
                                editView.FormFields.Select(f => new KeyValuePair<string, object>(f.Name, $"{f.Value}"))));
                    }
                    catch (Exception ex)
                    {
                        new TextView() { Text = ex.Message }.ShowDialog();
                    }
                };
                panel.Add(submitButton);
                validate();
                editView.UpdateEditView();

                editView.LayoutTop(panel.CreateTitleCentral(model.ProcedureLabel));

            

                errorsLabel = CreateText("");
                editView.LayoutTop(errorsLabel);

                LayoutFill(editView);
            }
            catch (Exception ex)
            {
                throw new Exception($"Исключение при применении модели процедуры: {ex.Message}");
            }
        }

        private ProcedureModel GetProcedureModel(ProcedureMetadata metadata)
        {

            using (var db = new ModelsDbContext())
            {
                db.Info(db.GetConnectionString());
                ProcedureModel model = db.Procedures.Include(p => p.Parameters).FirstOrDefault(p => p.Name == metadata.ProcedureName);
                if (model is null)
                {
                    db.Procedures.Add(model = new()
                    {
                        Name = metadata.FullName,
                        Label = metadata.FullName,
                        Code = metadata.ProcedureName.Replace("[", "").Replace("]", "")
                    });
                    db.SaveChanges();
                }
                return model;
            }
        }

        public ProcedureModel GetProcedureModel(string fullname)
        {

            using (var db = new ModelsDbContext())
            {
                ProcedureModel model = db.Procedures.Include(p => p.Parameters).FirstOrDefault(p => p.Name == fullname);
                return model;
            }
        }

        public Dictionary<string, List<string>> Validate(Dictionary<string, object> input)
        {
            Dictionary<string, List<string>> results = new();
            foreach (ParameterMetadata par in model.ParametersMetadata.Values)
            {
                results[par.ParameterName] = new();
                //обязательный параметр
                if ((!input.ContainsKey(par.ParameterName) || input[par.ParameterName] is null) && !par.HasDefaultValue)
                {
                    results[par.ParameterName].Add("Необходимо задать значение");
                }
            }
            return results;
        }

    }
}
