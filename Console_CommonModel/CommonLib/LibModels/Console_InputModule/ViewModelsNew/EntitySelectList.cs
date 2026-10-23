using System.Collections;

[Label("Список сущности")]
public class EntitySelectList : ListView
{
    private string ModelStatus;

    
    [Label("Надпись")]
    [NotNullNotEmpty()]
    public override string Text { get => Get<string>("Text"); set => Set<string>("Text", value); }


    [Label("Набор данных")]
    [InputEntityType]
    [NotNullNotEmpty]
    public string EntityName { get => Get<string>("EntityName"); set => Set<string>("EntityName", value); }

    [Label("Свойство отображения")]
    [NotNullNotEmpty]
    [InputSelectExpression("EntityProperties")]
    [UpdateWhenChanged( )]
    public string TextExpression { get => Get<string>("TextExpression"); set => Set<string>("TextExpression", value); }

    [NotInput]
    [InputPrimitiveCollection()]
    [Label("Entity Properties")]
    public List<string> EntityProperties { get; set; }

    public EntitySelectList()
    {
        this.TextExpression = "Id";
        this.EntityProperties = new();
        this.EntityName = nameof(UserPerson);

        this.AddPropertyChangeListener("EntityName", changes => {            
            try
            {
                if(changes.After is not null)
                {
                    var ptype = changes.After.ToString().ToType();
                    var props = ptype.GetPropertyNames();
                    this.EntityProperties.Clear();
                    this.EntityProperties.AddRange(props);
                    this.TextExpression = props.First();
                    if (!props.Contains(TextExpression))
                        this.TextExpression = props.Contains("Name")? "Name": props.First();
                }
                this.UpdateModel();
            }
            catch (Exception ex)
            {
                this.Error($"Ошибка при обновлении свойства TextExpression = {TextExpression} => {ex}");
            }
        });
        this.AddPropertyChangeListener("TextExpression", changes => {
            try
            {
                this.UpdateModel();
            }
            catch(Exception ex)
            {
                this.Error($"Ошибка при обновлении свойства TextExpression = {TextExpression} => {ex}");
            }
        });        
        this.OnSelected = (option) => { };
    }


    /// <summary>
    /// Обнволение модели данных
    /// </summary>
    public void UpdateModel()
    {
        if(this.EntityName.ToType().GetProperties().Select(p=>p.Name).Contains(TextExpression))
        {
            this.Options.Clear();
            foreach (object item in this.EntityName.ToType().GetEntityFasadeForType().GetAll())
            {
                this.Options.Add(new($"{item.GetValue(TextExpression)}"));
            }
            this.ModelStatus = "success";
            this.Debug($"Обновление данных успешно завершено");
        } 
        else
        {
            this.ModelStatus = "error";
            this.Debug($"Обновление данных завершено с ошибкой");
        }
    }
}
