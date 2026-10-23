using System;
using System.Diagnostics;

namespace Console_CommonModel.CommonLib.LibModels.Console_InputModule.ViewModels
{
    /// <summary>
    /// Объект определяет признак активной фазы жизненного цикла
    /// </summary>
    public class AliveItem : ChangeSupport
    {

        /// <summary>
        /// Признак активной фазы использования объекта
        /// </summary>
        public bool Alive { get => Get<bool>("Alive"); set => Set("Alive", value); }


        /*public Action<AliveItem> OnInitPort = (item) =>
        {
            Writing.ToConsole("Initiallized "+item.GetType().Name +" "+item.GetHashCode());
        };*/


        public AliveItem() : base()
        {
            InitAliveItem();

        }


        /// <summary>
        /// Установка значений по-умолчанию
        /// </summary>
        private void InitAliveItem()
        {
            Alive = false;
        }


        /// <summary>
        /// Метод инициализации жизненного цикла
        /// </summary>
        public virtual void Init()
        {
            Debug.WriteLine(GetType().Name + " Init " + GetHashCode());
            Alive = true;
            //OnInitPort(this);
        }


        /// <summary>
        /// Метод инициализации жизненного цикла
        /// </summary>
        public virtual void Destroy()
        {
            Debug.WriteLine(GetType().Name + " Destroy " + GetHashCode());
            Alive = false;
        }
    }
}
