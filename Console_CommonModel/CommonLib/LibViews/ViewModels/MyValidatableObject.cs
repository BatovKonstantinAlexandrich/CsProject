using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ActionModels
{

    /// <summary>
    /// Объект, состояние которого проверяется функциями валидации
    /// </summary>
    [Label("Модель валидации")]
    public partial class MyValidatableObject : IValidatableObject
    {

        /// <summary>
        /// Результаты валидации сохраняются при вызове метода Validate
        /// </summary>
        [NotMapped]
        [NotInput]
        public Dictionary<string, List<string>> ValidationResults;

        /// <summary>
        /// Признак успешной проверки функциями валидации
        /// </summary>
        [NotInput]
        [NotMapped]
        public virtual bool ValidationSuccess { get; set; }


        /// <summary>
        /// Событие изменения состояния валидности
        /// </summary>
        [NotInput]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public Action<bool> OnValidationStateChanged = val => { };


        public MyValidatableObject()
        {            
            InitializeComponent();
        }

        public MyValidatableObject(IContainer container)
        {
            InitializeComponent();
        }


        public void InitializeComponent()
        {
            ValidationResults = new();
            ValidationSuccess = false;
        }     


        /// <summary>
        /// Выполнение проверки функциями валидации
        /// </summary>
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = this.Validate();
            var returning = new List<ValidationResult>();
            foreach (var kv in results)
            {
                var property = kv.Key;
                foreach (var value in kv.Value)
                {
                    returning.Add(new ValidationResult(value, new List<string>() { property }));
                }
            }
            return returning;
        }
    }

}
 