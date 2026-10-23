namespace Console_CommonModel.CommonLib.LibModels.Console_InputModule.ViewModelsNew
{

    [Label("Таблица сущности")]
    public class EntityTable : TableView
    {

        [Label("Набор данных")]
        [InputEntityType]
        [NotNullNotEmpty]
        public string EntityName { get => Get<string>("EntityName"); set => Set("EntityName", value); }


        public EntityTable()
        {
            VisibleColumns = new() { "Id" };
            AddPropertyChangeListener("EntityName", changes =>
            {
                try
                {
                    if (changes.After is not null)
                    {
                        var ptype = changes.After.ToString().ToType();
                        VisibleColumns.Clear();
                        VisibleColumns.AddRange(ptype.GetPropertyNames());
                    }
                }
                catch (Exception ex)
                {
                    this.Error($"Ошибка при обновлении свойства EntityName = {EntityName} => {ex}");
                }
            });
            AddPropertyChangeListener("VisibleColumns", changes =>
            {
                try
                {
                    UpdateModel();
                }
                catch (Exception ex)
                {
                    this.Error($"Ошибка при обновлении свойства VisibleColumns = {VisibleColumns.ToJson()} => {ex}");
                }
            });

            EntityName = nameof(UserPerson);
        }

        private void UpdateModel()
        {
            DataRows.Clear();
            foreach (object item in EntityName.ToType().GetEntityFasadeForType().GetAll())
            {
                DataRows.Add(item);
            }
        }
    }
}
