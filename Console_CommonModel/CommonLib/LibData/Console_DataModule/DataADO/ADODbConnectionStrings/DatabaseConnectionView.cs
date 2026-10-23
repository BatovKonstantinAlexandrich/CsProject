using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.TypedModule;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbConnectionStrings
{

    /// <summary>
    /// Представление для редактирования настроек подключения
    /// </summary>
    [Label("Представление связанное с данными")]
    public class DatabaseConnectionView : TypedView
    {
        [NotInput]
        public List<string> Types = new() {
            nameof(SqlServerConnectionString),
            nameof(PostgresConnectionString),
            nameof(MySqlConnectionString)
        };

        public DatabaseConnectionView()
        {
            Type = nameof(SqlServerConnectionString);
        }

        [InputSelectExpression(nameof(Types))]
        public string Type { get => Get<string>(nameof(Type)); set => Set(nameof(Type), value); }


        [OnChange(nameof(Type))]
        public void OnTypeChaned()
        {
            if (Type is null)
            {
                ViewModel = null;
            }
            else
            {
                ViewModel = Type.ToType().New<AppViewBase>();
                ViewModel.Parent = this;
            }
        }

        //public override Control ToControl()
        //{
        //    return base.ToControl<TypedView>();
        //}
    }
}
