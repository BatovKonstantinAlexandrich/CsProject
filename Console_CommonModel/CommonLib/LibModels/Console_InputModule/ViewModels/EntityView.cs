using Console_CommonModel.CommonLib.LibData.AbstractionModel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibModels.Console_InputModule.ViewModels
{
    public class EntityView : ViewItem
    {
        public BaseEntity Data { get; set; }

        public object Update(Dictionary<string, string> binding)
        {
            foreach (var p in binding)
            {

                object value = GetBindedValue(p.Value, Data);
                SetValue(p.Key, value);
            }
            return this;
        }

        private void SetValue(string key, object value)
        {
            PropertyInfo prop = GetType().GetProperty(key);
            if (prop != null)
            {
                prop.SetValue(this, value);
            }
            FieldInfo field = GetType().GetField(key);
            if (field != null)
            {
                field.SetValue(this, value);
            }
        }

        private object GetBindedValue(string value, BaseEntity data)
        {
            if (data.GetType().GetPropertyNames().Contains(value))
            {
                return data.GetType().GetProperty(value).GetValue(data);
            }
            else
            {
                return Expressions.Interpolate(value, data);
            }

        }
    }
}
