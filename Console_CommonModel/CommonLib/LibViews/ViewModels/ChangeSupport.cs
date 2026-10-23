using Microsoft.EntityFrameworkCore.Metadata.Internal;

using Namotion.Reflection;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace ActionModels
{

    [Label("Поддержка событий изменения свойств")]
    public partial class ChangeSupport : EventSupport, IChangeSupport
    {
        /// <summary>
        /// Когда установлен этот флаг работает правило обратного наследования областей
        /// </summary>
        [NotMapped]
        public bool EnableVerticalBinding = true;

        /// <summary>
        /// Включение/отключение прослушивание изменений
        /// </summary>
        [Label("Включено")]
        [NotMapped]
        public bool EnableChangeSupport = true;

        [NotMapped]
        [NotInput]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public object EventHandler;


        public void ApplyPropertyChangedEvent(PropertyChangedMessage evt)
        {
            this.SetValue(evt.Property, evt.After);
        }

        /// <summary>
        /// Конструктор объекта 
        /// </summary>
        public ChangeSupport() : base()
        {
            this.EventHandler = this;
            OnChange = (message) => { };
            Changed = false;
        }

        public ChangeSupport(IContainer container) : base()
        {
            //container.Add(this);
            this.EventHandler = this;
            OnChange = (message) => { };
            Changed = false;
        }

        /// <summary>
        /// Структура физического хранения свойств
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [Label("Область видимости")]
        [NotInput]
        [NotMapped]
        public ConcurrentDictionary<string, object> DataState { get; set; } =
            new ConcurrentDictionary<string, object>();

        public void BindTwoWays(string propertyName, IChangeSupport target)
        {
            this.AddPropertyChangeListener(nameof(propertyName),
                evt => {
                    if (target.GetValue(propertyName) != evt.After)
                    {
                        target.SetValue(propertyName, evt.After);
                    }
                });
            target.AddPropertyChangeListener(nameof(propertyName),
                evt => {
                    if (this.GetValue(propertyName) != evt.After)
                    {
                        this.SetValue(propertyName, evt.After);
                    }
                });
        }


        public void Bind(string propertyName, ViewItem target, string relativePropertyName)
        {
            var ctrl = this;
            target.OnEvent += (message) =>
            {
                if (message is PropertyChangedMessage)
                {

                    PropertyChangedMessage changed = (PropertyChangedMessage)message;


                    if (changed.Property == relativePropertyName)
                    {
                        var value = ctrl.GetValue(propertyName);
                        if (value != changed.After)
                        {
                            Setter.SetValue(ctrl, propertyName, changed.After);
                            ctrl.Changed = true;
                        }
                    }
                }
            };
        }

        public void Bind(string propertyName, IChangeSupport target, string relativePropertyName)
        {
            var ctrl = this;
            target.OnChange += (message) =>
            {
                if (message is PropertyChangedMessage)
                {
                    PropertyChangedMessage changed = (PropertyChangedMessage)message;
                    if (changed.Property == relativePropertyName)
                    {
                        Setter.SetValue(ctrl, propertyName, changed.After);
                        ctrl.Changed = true;
                    }
                }
            };
        }
        /**
         * Регистрация обработчиков событий изменения свойств
         */
        public Action AddPropertyChangeListener<T>(string property, Action<T> onchanged)
        {
            return AddPropertyChangeListener(property, evt => {
                var message = new PropertyChangedMessage<T>();
                var data = (T)evt.After;
                onchanged(data);
            });
        }
        public Action AddPropertyChangeListener(string property, Action<PropertyChangedMessage> listener)
        {            
            Action<PropertyChangedMessage> onchanged = (PropertyChangedMessage evt) =>
            {
                if (evt is not null && evt.Property == property)
                {
                    if (listener is not null)
                    {
                        try
                        {
                            listener(evt);
                        }
                        catch (TargetException tex)
                        {

                            this.Error($"Исключение при обработки события " +
                                $"изменения свойств: {evt.ToJsonOnScreen()} => {tex.InnerException.Message}");

                        }
                        catch (Exception ex)
                        {
                            this.Error($"Исключение при обработки события " +
                                $"изменения свойств: {evt.ToJsonOnScreen()} => {ex.Message}");
                        }

                    }

                }
            };
            OnChange += onchanged;
            return () => {
                this.OnChange -= onchanged;
            };
        }





        /// <summary>
        /// Признак изменения свойств после последнего обновления
        /// </summary>
        [NotMapped]
        [NotInput]
        private bool _Changed = false;

        [NotMapped]
        [NotInput]
        public bool Changed
        {
            get
            {
                return _Changed;
            }
            set
            {
                _Changed = value;
            }
        }


        /// <summary>
        /// Сброс признака изменения данных, предназначен для применения в процедурах обновления данных
        /// </summary>
        public virtual bool WasChanged()
        {
            if (Changed)
            {
                Changed = false;
                return true;
            }
            return false;
        }






        /// <summary>
        /// Событие изменения значения свойства
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [NotMapped]
        [NotInput]
        public Action<PropertyChangedMessage> OnChange { get; set; }









        /// <summary>
        /// Создание события при извлечении значения из свойства
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>

        public void OnGet<T>(string key, T value)
        {
            //this.Info($"GET: /{GetType().GetTypeName()}/{GetHashCode()}/{key}/{value}");        
        }



        /// <summary>
        /// Метод получения значения свойства
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            if (DataState.ContainsKey(key))
            {
                T value = (T)DataState[key];
                OnGet<T>(key, value);
                return value;
            }
            else
            {

                if (EnableVerticalBinding)
                {
                    if (DataState.ContainsKey("Parent"))
                    {
                        ChangeSupport parentSupport = (ChangeSupport)DataState["Parent"];
                        T value = parentSupport.Get<T>(key);
                        OnGet<T>(key, value);
                        return value;
                    }
                    else
                    {
                        throw new Exception($"PropertyNotInitiallized {key}");
                    }
                }
                else
                {
                    throw new Exception($"PropertyNotInitiallized {key}");
                }

            }
        }



        /// <summary>
        /// Ссылки на методы действий которые вызываются при смене значения свойств
        /// </summary>
        private static ConcurrentDictionary<string, ConcurrentDictionary<string, List<MethodInfo>>> MethodsOnPropertyChanged = new();

        [InputBool()]
        [NotMapped]
        [NotInput]
        public bool ValidateOnInput { get; set; } = false;


        /// <summary>
        /// Установка значения свойства
        /// </summary>
        /// <typeparam name="T">тип свойства</typeparam>
        /// <param name="key">наименование свойства</param>
        /// <param name="value">значение свойства</param>
        public void Set<T>(string key, T value)

        {
            object before = DataState.ContainsKey(key) ? DataState[key] : null;
            if (DataState.ContainsKey(key) && DataState[key] is T && (Typing.IsPrimitive(typeof(T)) ? $"{DataState[key]}" == $"{value}" : DataState[key] == (object)value))
                return;
            DataState[key] = value;
            if (ValidateOnInput)
            {
                var validationResult = this.Validate(key);
                if (validationResult.Count > 0)
                {
                    string message = "";
                    foreach (var error in validationResult)
                        message += $"  {error}";
                    this.Error($"Свойство {key} невалидно: \n" + message);
                    throw new Exception($"{key} {validationResult}");
                }
            }
            if (this.GetType().HasProperty(key))
            {
                var attrs = CommonUtils.ForProperty(this.GetType(), key);
                if (attrs.ContainsKey("UpdateWhenChangedAttribute"))
                {
                    bool needChange = CommonUtils.IsTrueValue(attrs["UpdateWhenChangedAttribute"]);
                    if (needChange)
                    {
                        if (before != null && value != null && (before.ToString() != value.ToString()))
                        {
                            Changed = true;
                        }
                        if ((before != null && value == null) || (before == null && value != null))
                        {
                            Changed = true;
                        }

                    }
                    else
                    {
                    }

                }
                else
                {
                    if (before != null && value != null && (before.ToString() != value.ToString()))
                    {
                        Changed = true;
                    }
                    if ((before != null && value == null) || (before == null && value != null))
                    {
                        Changed = true;
                    }
                }
            }
            //if(key==nameof(ViewItem.DatasourceId))
            //{
            //    Console.WriteLine(1+1);
            //    Console.WriteLine(1+1);
            //}
            if (EnableChangeSupport)
            {
                if (this.OnChange is null)
                {
                    this.OnChange = evt => { };
                }
                this.OnChange(new PropertyChangedMessage()
                {
                    Source = this,
                    Property = key,
                    Before = before,
                    After = value
                });
            }

            this.SendEvent(new PropertyChangedMessage()
            {
                Source = this,
                Property = key,
                Before = before,
                After = value
            });

            //this.Info($"on changed {GetType().Name} {key}");
            if (EventHandler is null)
                EventHandler = this;

            if(!MethodsOnPropertyChanged.ContainsKey(this.GetTypeName()))
            {
                lock(MethodsOnPropertyChanged)
                {
                    MethodsOnPropertyChanged[this.GetTypeName()] = new();


                    foreach (var method in EventHandler.GetType().GetMethods().Where(m => m.DeclaringType != typeof(object) && !m.Name.StartsWith("set_") && !m.Name.StartsWith("get_")))
                    {
                        Dictionary<string, string> methodAttrs = EventHandler.GetType().GetMethodAttributes(method.Name);
                        if (methodAttrs.ContainsKey(nameof(OnChangeAttribute)))
                        {
                            var propertyName = methodAttrs[nameof(OnChangeAttribute)].Replace("\"", "");
                            if(!MethodsOnPropertyChanged[this.GetTypeName()].ContainsKey(propertyName))
                            {
                                MethodsOnPropertyChanged[this.GetTypeName()][propertyName] = new();
                            }
                            MethodsOnPropertyChanged[this.GetTypeName()][propertyName].Add(method);
                        }
                    }
                }
            }

            if(true)
            {
                

                    if (MethodsOnPropertyChanged[this.GetTypeName()].ContainsKey(key))
                    {
                        foreach(var method in MethodsOnPropertyChanged[this.GetTypeName()][key])
                        {
                            try
                            {
                                //this.Info(method.Name);
                                if (method.GetParameters().Count() == 0)
                                {
                                    EventHandler.Invoke(method.Name, new object[] { });
                                }
                                else
                                {
                                    EventHandler.Invoke(method.Name, new object[] { new PropertyChangedMessage() {
                                            Source = this,
                                            Property = key,
                                            Before = before,
                                            After = value
                                        }});
                                }

                                //this.Invoke(method.Name, new Dictionary<string, object>());
                            }
                            catch (Exception ex)
                            {
                                this.Error($"Ошибка рефлексивного вызова {method.Name} {method.GetParameters().Select(p => p.Name).ToJson()}=> {ex.Message}");
                            }
                        }
                    }
                    /*foreach (var method in EventHandler.GetType().GetMethods().Where(m => m.DeclaringType != typeof(object) && !m.Name.StartsWith("set_") && !m.Name.StartsWith("get_")))
                    {
                        if (method.DeclaringType == typeof(object))
                            continue;
                        Dictionary<string, string> methodAttrs = EventHandler.GetType().GetMethodAttributes(method.Name);
                        if (methodAttrs.ContainsKey(nameof(OnChangeAttribute)))
                        {
                            var k1 = methodAttrs[nameof(OnChangeAttribute)].Replace("\"", "");
                            var k2 = key;
                            if (k1 == k2)
                            {
                                try
                                {
                                    this.Info(method.Name);
                                    if (method.GetParameters().Count() == 0)
                                    {
                                        EventHandler.Invoke(method.Name, new object[] { });
                                    }
                                    else
                                    {
                                        EventHandler.Invoke(method.Name, new object[] { new PropertyChangedMessage() {
                                            Source = this,
                                            Property = key,
                                            Before = before,
                                            After = value
                                        }});
                                    }

                                    //this.Invoke(method.Name, new Dictionary<string, object>());
                                }
                                catch (Exception ex)
                                {
                                    this.Error($"Ошибка рефлексивного вызова {method.Name} {method.GetParameters().Select(p => p.Name).ToJson()}=> {ex.Message}");
                                }
                            }
                        }

                    }*/
                

            }

        }

        public void AddPropertyChangeListener(Action<PropertyChangedMessage> listener)
        {
            this.OnChange += listener;
        }

        public void RemovePropertyChangeListener(Action<PropertyChangedMessage> listener)
        {
            this.OnChange -= listener;
        }


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