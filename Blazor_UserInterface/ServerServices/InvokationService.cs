using NetCoreConstructorAngular.Data.DataConverter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NetCoreConstructorAngular.Data.DataServices
{
    public class InvokationService: BaseService
    {
        private HashSet<string> PrimitiveTypeNames = new HashSet<string>() {
            "String", "Boolean", "Double", "Int16", "Int32", "Int64", "UInt16", "UInt32", "UInt64" };


        public bool IsPrimitive( object subject)
        {
            Type type = subject.GetType();
            return type.IsPrimitive || PrimitiveTypeNames.Contains(subject.GetType().Name);
        }


        public object Invoke(MethodInfo method, object target, string json)
        {
            string state = "Поиск обьекта: ";
            Dictionary<string, object> pars = new Dictionary<string, object>();
            List<object> invArgs = null;
            try
            {
                SeriallizationTool bind = new SeriallizationTool();
                Dictionary<string, object> parametersMap = 
                    JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                foreach(string key in parametersMap.Keys)
                {
                    if(IsPrimitive(parametersMap[key]) == false)
                    {
                        string propertyJson = JObject.FromObject(parametersMap[key]).ToString();
                        object value = bind.Deserrialize(propertyJson);
                        pars[key] = value;

                    }
                    else
                    {
                        if (parametersMap[key].GetType().Name == "Int64")
                        {                             
                            pars[key] = Int32.Parse(parametersMap[key].ToString());
                        }
                        else
                        {
                            pars[key] = parametersMap[key];
                        }
                        
                    }
                }                                 
                invArgs = new List<object>();
                foreach (ParameterInfo pinfo in method.GetParameters())
                {
                    if (pinfo.IsOptional == false && pars.ContainsKey(pinfo.Name) == false)
                    {
                        throw new Exception("require argument " + pinfo.Name);
                    }
                    string parameterName = pinfo.ParameterType.Name;

                    if (parameterName.StartsWith("Dictionary"))
                    {
                        Dictionary<string, object> dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(pars[pinfo.Name].ToString());
                        invArgs.Add(dictionary);
                    }
                    else
                    {
                        invArgs.Add(pars[pinfo.Name]);
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception("ArgumentsException: " + ex.Message, ex);
            }


            try
            {
                object result = method.Invoke(target, invArgs.ToArray());
                state = state.Substring(0, state.Length - 7) + "успех;";
                return result;
            }
            catch (Exception ex)
            {
                string message = "";
                while (ex.InnerException!=null)
                {
                    message += ex.InnerException.Message;
                    ex = ex.InnerException;
                }
                throw new Exception(message,ex);
            }
        }
    }
}
