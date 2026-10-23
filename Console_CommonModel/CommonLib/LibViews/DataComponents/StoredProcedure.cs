using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;

using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibViews.DataComponents
{
    /// <summary>
    /// Компонент для выполнения хранимых процедур
    /// </summary>
    [Label("Хранимая процедура")]
    [Description("Компонент для выполнения хранимых процедур базы данных с поддержкой параметров и динамического редактирования параметров через PropertyGrid")]
    public class StoredProcedure : DataSourceComponent
    {
        private string _name = string.Empty;
        private ChangingDictionary<string, object> _parameters = new ChangingDictionary<string, object>();

        /// <summary>
        /// Имя хранимой процедуры
        /// </summary>
        [Category("Stored Procedure")]
        [Description("Имя хранимой процедуры для выполнения")]
        [DisplayName("Имя процедуры")]
        //[Editor(typeof(StoredProcedureNameEditor), typeof(UITypeEditor))]
        public string Name
        {
            get => _name;
            set => _name = value ?? string.Empty;
        }

        /// <summary>
        /// Параметры хранимой процедуры
        /// </summary>
        [Category("Stored Procedure")]
        [Description("Параметры хранимой процедуры (имя параметра, значение)")]
        [DisplayName("Параметры")]
        [Browsable(false)] // Скрываем из PropertyGrid, так как Dictionary сложно редактировать напрямую
        public ChangingDictionary<string, object> Parameters
        {
            get => _parameters;
            set => _parameters = value ?? new ChangingDictionary<string, object>();
        }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public StoredProcedure()
        {
            _parameters = new ChangingDictionary<string, object>();
        }

        /// <summary>
        /// Конструктор с контейнером
        /// </summary>
        /// <param name="container">Контейнер для компонента</param>
        public StoredProcedure(IContainer container)
        {
            if (container != null)
            {
                container.Add(this);
            }

            _parameters = new ChangingDictionary<string, object>();
        }

        public void OnProcedureSelected()
        {
            _parameters.Clear();

            if (string.IsNullOrWhiteSpace(Name))
            {
                return;
            }

            try
            {
                var proceduresMetadata = GetDbMetadata().GetProceduresMetadata();
                var procedure = proceduresMetadata.FirstOrDefault(p =>
                    p.Key == Name ||
                    p.Value.ProcedureName == Name ||
                    p.Value.FullName == Name).Value;

                if (procedure != null && procedure.ParametersMetadata != null)
                {
                    foreach (var key in procedure.ParametersMetadata.Keys)
                    {
                        _parameters[key] = key;
                    }
                }
            }
            catch (Exception ex)
            {
                // Логируем ошибку, но не прерываем выполнение
                System.Diagnostics.Debug.WriteLine($"Ошибка в OnProcedureSelected: {ex.Message}");
            }
        }

        /// <summary>
        /// Выполняет хранимую процедуру с использованием свойств Name и Parameters
        /// </summary>
        /// <returns>Результат выполнения процедуры в виде DataTable</returns>
        public DataTable Execute()
        {
            if (string.IsNullOrWhiteSpace(_name))
            {
                throw new InvalidOperationException("Имя хранимой процедуры не задано. Установите свойство Name перед выполнением.");
            }

            if (!IsConnected)
            {
                throw new InvalidOperationException("Подключение к базе данных не установлено. Вызовите метод Connect() перед выполнением процедуры.");
            }

            // Используем метод базового класса для выполнения процедуры
            return ExecuteProcedure(_name, _parameters);
        }



        /// <summary>
        /// Выполняет хранимую процедуру с переданными параметрами (переопределяет параметры из свойства Parameters)
        /// </summary>
        /// <param name="parameters">Параметры процедуры (имя параметра, значение). Если null, используются параметры из свойства Parameters</param>
        /// <returns>Результат выполнения процедуры в виде DataTable</returns>
        public DataTable Execute(Dictionary<string, object> parameters)
        {
            if (string.IsNullOrWhiteSpace(_name))
            {
                throw new InvalidOperationException("Имя хранимой процедуры не задано. Установите свойство Name перед выполнением.");
            }

            if (!IsConnected)
            {
                throw new InvalidOperationException("Подключение к базе данных не установлено. Вызовите метод Connect() перед выполнением процедуры.");
            }

            // Используем переданные параметры или параметры из свойства
            Dictionary<string, object> paramsToUse;
            if (parameters != null)
            {
                paramsToUse = parameters;
            }
            else
            {
                // Преобразуем ChangingDictionary в Dictionary
                paramsToUse = new Dictionary<string, object>();
                foreach (var kvp in _parameters)
                {
                    paramsToUse[kvp.Key] = kvp.Value;
                }
            }

            // Используем метод базового класса для выполнения процедуры
            return ExecuteProcedure(_name, paramsToUse);
        }

        /// <summary>
        /// Добавляет или обновляет параметр процедуры
        /// </summary>
        /// <param name="parameterName">Имя параметра</param>
        /// <param name="value">Значение параметра</param>
        public void SetParameter(string parameterName, string value)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                throw new ArgumentException("Имя параметра не может быть пустым.", nameof(parameterName));
            }

            if (_parameters.ContainsKey(parameterName))
            {
                _parameters[parameterName] = value;
            }
            else
            {
                _parameters.Add(parameterName, value);
            }
        }

        /// <summary>
        /// Удаляет параметр процедуры
        /// </summary>
        /// <param name="parameterName">Имя параметра</param>
        /// <returns>True, если параметр был удален, иначе False</returns>
        public bool RemoveParameter(string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                return false;
            }

            return _parameters.Remove(parameterName);
        }

        /// <summary>
        /// Очищает все параметры процедуры
        /// </summary>
        public void ClearParameters()
        {
            _parameters.Clear();
        }

        /// <summary>
        /// Получает значение параметра
        /// </summary>
        /// <param name="parameterName">Имя параметра</param>
        /// <returns>Значение параметра или null, если параметр не найден</returns>
        public object GetParameter(string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                return null;
            }

            return _parameters.ContainsKey(parameterName) ? _parameters[parameterName] : null;
        }

    }
}

