using System;
using System.ComponentModel;

namespace Console_CommonModel.CommonLib.LibViews.DataComponents
{
    /// <summary>
    /// Базовый класс для сервисных компонентов
    /// </summary>
    [Label("Базовый сервис")]
    [Description("Базовый класс для всех сервисных компонентов приложения, обеспечивающий стандартную" +
        " функциональность управления жизненным циклом компонента")]
    public class ServiceComponentBase : Component
    {
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public ServiceComponentBase()
        {
        }

        /// <summary>
        /// Конструктор с контейнером
        /// </summary>
        /// <param name="container">Контейнер для компонента</param>
        public ServiceComponentBase(IContainer container)
        {
            if (container != null)
            {
                container.Add(this);
            }
        }

        /// <summary>
        /// Освобождает ресурсы, используемые компонентом
        /// </summary>
        /// <param name="disposing">True, если освобождаются управляемые ресурсы</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Освобождаем управляемые ресурсы здесь
            }
            base.Dispose(disposing);
        }
    }
}

