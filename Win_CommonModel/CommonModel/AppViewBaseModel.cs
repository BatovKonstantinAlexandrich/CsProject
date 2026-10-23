using System;
using System.ComponentModel;

namespace WinFormsApp.AppViews
{
    /// <summary>
    /// Базовая модель представления с заголовком, описанием и событиями сохранения/отмены
    /// </summary>
    [Label("Модель представления")]
    [Description("Базовая модель для представлений с поддержкой заголовка, описания и событий сохранения/отмены")]
    public class AppViewBaseModel : INotifyPropertyChanged
    {
        private string _title;
        private string _description;

        /// <summary>
        /// Заголовок представления
        /// </summary>
        [Label("Заголовок")]
        [Description("Заголовок представления")]
        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        /// <summary>
        /// Описание представления
        /// </summary>
        [Label("Описание")]
        [Description("Подробное описание представления")]
        public string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        /// <summary>
        /// Событие отмены изменений
        /// </summary>
        public event EventHandler OnCancel;

        /// <summary>
        /// Событие сохранения изменений
        /// </summary>
        public event EventHandler OnSave;

        /// <summary>
        /// Событие изменения свойства
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Вызывает событие отмены
        /// </summary>
        public void RaiseCancel()
        {
            OnCancel?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Вызывает событие сохранения
        /// </summary>
        public void RaiseSave()
        {
            OnSave?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Вызывает событие изменения свойства
        /// </summary>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public AppViewBaseModel()
        {
            _title = string.Empty;
            _description = string.Empty;
        }

        public AppViewBaseModel(string title, string description = "")
        {
            _title = title ?? string.Empty;
            _description = description ?? string.Empty;
        }
    }
}





