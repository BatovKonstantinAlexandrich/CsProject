using Google.Protobuf.WellKnownTypes;

[Label("Проектирование")]
public class ActiveItem : ActionExecuter
{

    /// <summary>
    /// Признак активной фазы использования объекта
    /// </summary>
    public bool Alive { get => Get<bool>("Alive"); set => Set("Alive", value); }

    /// <summary>
    /// Режим разработчика
    /// </summary>
    public bool DesignMode { get => Get<bool>("DesignMode"); set => Set("DesignMode", value); }


    public ActiveItem() : base()
    {
        this.DesignMode = false;
        this.Alive = false;
        this.EnableChangeSupport = true;
        this.Changed = false;
        this.AddPropertyChangeListener(nameof(DesignMode), OnDesignModeChanged);
    }


    public void BindPropertiesChangeSupport( IChangeSupport support )
    {
        foreach(var key in this.DataState.Keys.ToList())
        {
            this.AddPropertyChangeListener(key, evt => support.SetValue(key, evt.After) );
        }
    }


    


    /// <summary>
    /// Метод инициализации жизненного цикла
    /// </summary>
    public virtual void Init()
    {
        this.Info(GetType().GetTypeName() + " инициализация " + GetHashCode());
        this.Alive = true;
    }

    /// <summary>
    /// Метод инициализации жизненного цикла
    /// </summary>
    public virtual void Destroy()
    {
        this.Info(GetType().GetTypeName() + " освобождение " + GetHashCode());
        this.Alive = false;
    }

    protected virtual void OnDesignModeChanged(PropertyChangedMessage message)
    {
        this.Info("OnDesignModeChanged");
    }

    public virtual void OnInit()
    {
        this.Info($"Init");
    }
    public virtual void OnDestroy()
    {
        this.Info($"Destroy");
    }
}


