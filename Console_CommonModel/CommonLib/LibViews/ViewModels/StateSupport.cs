using System.ComponentModel.DataAnnotations.Schema;


[Label("Управление состоянием")]
public class StateSupport: ChangeSupport
{

    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [NotMapped]
    private Dictionary<string, object> _fixedState = new();

    public StateSupport()
    {        
    }

    /// <summary>
    /// Сохранение состояния объекта
    /// </summary>    
    public string FixState()
    {
        _fixedState.Clear();        
        foreach (var key in DataState.Keys)
        {
            _fixedState[key] = DataState[key];
        }
        return _fixedState.ToJsonOnScreen().GetHashSha256();
    }


    /// <summary>
    /// Применение изменений для зафиксированного состояния
    /// </summary>
    public void Commit()
    {
        this.EnsureIsValide();
        _fixedState.Clear();
        foreach (var key in DataState.Keys)
        {
            _fixedState[key] = DataState[key];
        }
    }


    /// <summary>
    /// Применение сохраннёного состояния
    /// </summary>
    public void Rollback()
    {        
        foreach (var key in _fixedState.Keys)
        {
            DataState[key] = _fixedState[key];
        }
        _fixedState.Clear();
    }
}


[Label("Проверка управления состоянием")]
public class StateSupportTest: TestingElement
{
    public StateSupportTest() : this(AppProviderService.GetInstance())
    {
    }
    public StateSupportTest(IServiceProvider provider) : base(provider)
    {
    }

    public override void OnTest()
    {
        this.Assert(p => {
            var viewBase = new AppViewBase();
            viewBase.Dock = "Top";
            viewBase.FixState();
            viewBase.Dock = "Bottom";
            viewBase.Rollback();
            return viewBase.Dock == "Top";
        },
        "Реализована поддержка управления данными объекта",
        "Ошибки при реализации поддержки управления данными объекта ");
    }
}




/*
           List<string> errors = this.Validate(key);                 
           if (errors.Count > 0) {
               string message = "";
               errors.ForEach((s) => {
                   message += "\n" + s;
               });
               throw new PropertyValidationException(key,message);
           }
           if((value == null && before != null)|| (value != null && before == null))
           {
               Changed = true;
           }else if(value!=null && before != null)
           {
               if (value.Equals(before) == false)
               {
                   Changed = true;
               }
           }


           switch (typeof(T).Name.ToLower())
           {
               default:
                   OnChange.Invoke(this, new PropertyChangeEvent<T>(new PropertyChangedMessage<T>()
                   {
                       Before = (T)before,
                       After = (T)Properties[key],
                       Property = key,
                       Source = this
                   }));
                   break;
                   throw new UnsupportedException($"В настоящий момент тип данных {typeof(T).Name} неподдерживется");
               case "boolean":
               case "bool":
                   OnChange.Invoke(this, new PropertyChangeEvent<Nullable<bool>>(new PropertyChangedMessage<Nullable<bool>>()
                   {
                       Before = (bool?)before,
                       After = (bool?)Properties[key],
                       Property = key,
                       Source = this
                   }));
                   break;
               case "datetime":
                   OnChange.Invoke(this, new PropertyChangeEvent<Nullable<DateTime>>(new PropertyChangedMessage<Nullable<DateTime>>()
                   {
                       Before = (DateTime?)before,
                       After = (DateTime?)Properties[key],
                       Property = key,
                       Source = this
                   }));
                   break;
               case "string":
                   OnChange.Invoke(this, new PropertyChangeEvent<string>(new Proper


tyChangedMessage<string>()
                   {
                       Before = (string)before,
                       After = (string)Properties[key],
                       Property = key,
                       Source = this
                   }));
                   break;
               case "object":
                   OnChange.Invoke(this, new PropertyChangeEvent<object>(new PropertyChangedMessage<object>()
                   {
                       Before = (object)before,
                       After = (object)Properties[key],
                       Property = key,
                       Source = this
                   }));
                   break;
               case "int":
                   OnChange.Invoke(this, new PropertyChangeEvent<int?>(new PropertyChangedMessage<int?>()
                   {
                       Before = (int?)before,
                       After = (int?)Properties[key],
                       Property = key,
                       Source = this
                   }));
                   break;
               case "long":
                   OnChange.Invoke(this, new PropertyChangeEvent<long?>(new PropertyChangedMessage<long?>()
                   {
                       Before = (long?)before,
                       After = (long?)Properties[key],
                       Property = key,
                       Source = this
                   }));
                   break;
               case "float":
                   OnChange.Invoke(this, new PropertyChangeEvent<float?>(new PropertyChangedMessage<float?>()
                   {
                       Before = (float?)before,
                       After = (float?)Properties[key],
                       Property = key,
                       Source = this
                   }));
                   break;

           }*/