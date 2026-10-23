using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataApi;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataODBC.API;

using Microsoft.Data.SqlClient;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.Data.ADOWebApiService
{
    public class ProcedureExecuter : IProcedureExecuter
    {
        private readonly ProcedureMetadata metadata;
        private readonly IWebApi api;

        public ProcedureExecuter(IWebApi api, ProcedureMetadata pmd)
        {
            metadata = pmd;
            this.api = api;
        }

        public override string ToString()
        {
            return metadata.ProcedureName;
        }

        public string GetName() => metadata.FullName;
        public string GetDescription() => metadata.FullName;
        public ProcedureMetadata GetMetaData() => metadata;


        public Dictionary<string, ParameterMetadata> GetInputMetaData() =>
            new Dictionary<string, ParameterMetadata>(metadata.ParametersMetadata.Where(p => p.Value.ParameterMode == "In").ToList());
        public Dictionary<string, ParameterMetadata> GetOutputMetaData() =>
            new Dictionary<string, ParameterMetadata>(metadata.ParametersMetadata.Where(p => p.Value.ParameterMode == "Out").ToList());



        /// <summary>
        /// Возвращает функцию для выполнения процедуры 
        /// </summary>    
        public Func<Dictionary<string, object>, object> GetExecFunc<TResult>() where TResult : class
        {
            return (input) =>
            {
                lock (this)
                {
                    /*string message = "";
                    if (input is not null) foreach (var kv in input) message += $" {kv.Key}={kv.Value}";
                    this.Info(GetName() + message);
                    var con = api.GetConnection();
                    Thread.Sleep(1000);
                    SqlCommand command = new SqlCommand(metadata.FullName, con);
                    command.CommandType = CommandType.StoredProcedure;

                    var target = metadata.ParametersMetadata.Keys.ToHashSet();
                    var current = input.Keys.ToHashSet();
                    var required = target.Except(current);
                    if (required.Count() > 0)
                        throw new ArgumentException("Необходимо передать значения в параметры " + required.ToJson(), required.ToJson().Replace("[", "").Replace("]", "").Replace("\"", ""));
                    foreach (var kv in input)
                    {
                        command.Parameters.Add(new SqlParameter(kv.Key, kv.Value));
                    }
                    var dataTable = new DataTable();
                    var adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);
                    return GetResultSet<TResult>(dataTable);*/
                    return null;

                }

            };
        }



    }
}