using Console_CommonModel.ActionModels;
using Console_CommonModel.CommonLib.LibData.AbstractionModel;

using System.Reflection;
using System.Text.Json.Serialization;
using System.Windows;

namespace ActionModels
{

    [Label("Модель событий")]
    public class EventSupport : BaseEntity, IDisposable, IEventSupport, IViewEventSupport
    {

        [NotInput]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Action<object, MouseEventArgs> OnMouseMove { get; set; } = (s, e) => { };
        public void AddOnMouseMove(Action<object, EventArgs> todo) => OnMouseMove += todo;
        public void RemoveOnMouseMove(Action<object, EventArgs> todo) => OnMouseMove -= todo;


        [NotInput]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Action<object, MouseEventArgs> OnMouseDown { get; set; } = (s, e) => { };
        public void AddOnMouseDown(Action<object, EventArgs> todo) => OnMouseDown += todo;
        public void RemoveOnMouseDown(Action<object, EventArgs> todo) => OnMouseDown -= todo;


        [NotInput]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Action<object, MouseEventArgs> OnMouseUp { get; set; } = (s, e) => { };
        public void AddOnMouseUp(Action<object, EventArgs> todo) => OnMouseUp += todo;
        public void RemoveOnMouseUp(Action<object, EventArgs> todo) => OnMouseUp -= todo;


        [NotInput]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Action<object, EventArgs> OnMouseEnter { get; set; } = (s, e) => { };
        public void AddOnMouseEnter(Action<object, EventArgs> todo) => OnMouseEnter += todo;
        public void RemoveOnMouseEnter(Action<object, EventArgs> todo) => OnMouseEnter -= todo;



        [NotInput]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Action<object, EventArgs> OnClick { get; set; } = (s, e) => { };
        public void AddOnClick(Action<object, EventArgs> todo) => OnClick += todo;
        public void RemoveOnClick(Action<object, EventArgs> todo) => OnClick -= todo;


        [NotInput]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Action<object, MouseEventArgs> OnMouseDoubleClick { get; set; } = (s, e) => { };
        public void AddOnMouseDoubleClick(Action<object, MouseEventArgs> todo) => OnMouseDoubleClick += todo;
        public void RemoveOnMouseDoubleClick(Action<object, MouseEventArgs> todo) => OnMouseDoubleClick -= todo;


        [NotInput]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Action<object, EventArgs> OnLoad { get; set; } = (s, e) => { };
        public void AddOnLoad(Action<object, EventArgs> todo) => OnLoad += todo;
        public void RemoveOnLoad(Action<object, EventArgs> todo) => OnLoad -= todo;


        [NotInput]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Action<object, EventArgs> OnMouseLeave { get; set; } = (s, e) => { };
        public void AddOnMouseLeave(Action<object, EventArgs> todo) => OnMouseLeave += todo;
        public void RemoveOnMouseLeave(Action<object, EventArgs> todo) => OnMouseLeave -= todo;

        [NotInput]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Action<object, KeyPressEventArgs> OnKeyPressed { get; set; } = (s, e) => { };
        public void AddOnKeyPressed(Action<object, KeyPressEventArgs> todo) => OnKeyPressed += todo;
        public void RemoveOnKeyPressed(Action<object, KeyPressEventArgs> todo) => OnKeyPressed -= todo;

        [NotInput]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Action<object, KeyEventArgs> OnKeyDown { get; set; } = (s, e) => { };
        public void AddOnKeyDown(Action<object, EventArgs> todo) => OnKeyDown += todo;
        public void RemoveOnKeyDown(Action<object, EventArgs> todo) => OnKeyDown -= todo;

        [NotInput]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Action<object, KeyEventArgs> OnKeyUp { get; set; } = (s, e) => { };
        public void AddOnKeyUp(Action<object, EventArgs> todo) => OnKeyUp += todo;
        public void RemoveOnKeyUp(Action<object, EventArgs> todo) => OnKeyUp -= todo;

        [NotInput]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Action<object, EventArgs> OnKeyClicked { get; set; } = (s, e) => { };
        public void AddOnKeyClicked(Action<object, EventArgs> todo) => OnKeyClicked += todo;
        public void RemoveOnKeyClicked(Action<object, EventArgs> todo) => OnKeyClicked -= todo;

        [NotInput]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Action<object, EventArgs> OnResized { get; set; } = (s, e) => { };
        public void AddOnResized(Action<object, EventArgs> todo) => OnResized += todo;
        public void RemoveOnResized(Action<object, EventArgs> todo) => OnResized -= todo;

        [NotInput]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Action<object, EventArgs> OnLocationChanged { get; set; } = (s, e) => { };

        [NotInput]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Action<object, EventArgs> OnValidated { get; set; } = (s, e) => { };

        [NotInput]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Action<object, ScrollEventArgs> OnScrollChanged { get; set; } = (s, e) => { };



        [NotInput]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Action<object, EventArgs> OnDragged { get; set; } = (s, e) => { };

        [NotInput]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Action<object, DragEventArgs> OnDragEnter { get; set; } = (s, e) => { };

        [NotInput]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Action<object, DragEventArgs> OnDragOver { get; set; } = (s, e) => { };

        [NotInput]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Action<object, EventArgs> OnDragLeave { get; set; } = (s, e) => { };

        [NotInput]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Action<object, EventArgs> OnDropped { get; set; } = (s, e) => { };


       
        public EventSupport() : base()
        {
            
        }

        public List<string> GetBaseEventProperties()
        {
            return this.GetType().GetProperties().Where(p => p.PropertyType.IsExtends(typeof(Action<object, EventArgs>))).Select(p => p.Name).ToList();
        }



        /// <summary>
        /// ViewItem переопределяет метод и передаёт событие родительскому представлению.        
        /// </summary>
        /// <param name="messageEvent">сообщение</param>
        public virtual void SendEvent(object messageEvent)
        {
            BeforeSendEvent(messageEvent);
        }

        public virtual void BeforeSendEvent(object messageEvent)
        {
            //this.Info($"{messageEvent.GetHashCode()}: SendEvent");
        }

        public void SendChanges()
        {
            SendEvent(new PropertyChangedMessage()
            {
                Type = "change",
                Before = null,
                After = this,
                Property = "this",
                Source = this
            });
        }

        public void FireEvent(PropertyChangedMessage propertyChangedMessage)
        {
            this.SendEvent(propertyChangedMessage);
        }

        public void FireEvent(CommonEventMessage<string> message)
        {
            this.SendEvent(message);
        }

        public void FireEvent<T>(CommonEventMessage<T> message)
        {
            this.SendEvent(message);
        }

        public void FireEvent<T>(T message)
        {
            this.FireEvent(new CommonEventMessage<T>(message));
        }

        /// <summary>
        /// Получение наименований событий, объявленных в объекте
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public List<string> GetEventListeners()
        {
            object target = this;
            List<string> listeners = new List<string>();
            foreach (EventInfo evt in target.GetType().GetEvents())
            {
                listeners.Add(evt.Name);
            }
            return listeners;
        }

        public void Dispose()
        {
            this.FireEvent(new CommonEventMessage<object>(this));
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
                   OnChange.Invoke(this, new PropertyChangeEvent<string>(new PropertyChangedMessage<string>()
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