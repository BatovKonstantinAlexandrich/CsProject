using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAuthorization.Data.ActionEvent.CoreMessaging
{
    public class MyPropertyDescriptor  
    {
        private Type _model;
        private string _property;

        public MyPropertyDescriptor(Type model, string property)
        {
            this._model = model;
            this._property = property;

        }
 
    }
}
