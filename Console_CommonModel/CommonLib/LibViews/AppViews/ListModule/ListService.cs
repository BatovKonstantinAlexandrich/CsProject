namespace Console_CommonModel.CommonLib.LibViews.AppViews.ListModule
{
    public class ListService
    {
        public int InjectDataModel<TEntity>(ListView view, IEnumerable<TEntity> dbset)
            where TEntity : DescriptableObject
        {
            view.Options.Clear();
            foreach (var name in dbset.Select(p => p.Label).ToList())
            {
                view.Options.Add(new Option()
                {
                    Value = name
                });
            }
            return 1;
        }
        public int InjectDataModel(ListView view, IEnumerable<DescriptableObject> dbset)
        {
            view.Options.Clear();
            foreach (var name in dbset.Select(p => p.Label).ToList())
            {
                view.Options.Add(new Option()
                {
                    Value = name
                });
            }
            return 1;
        }
        public int InjectDataModel(ListView view, IEnumerable<DescriptableObject> dbset, Action<DescriptableObject> onselect)
        {
            view.Options.Clear();
            foreach (var p in dbset.ToList())
            {
                view.Options.Add(new Option()
                {
                    Value = p.Label,
                    Item = p
                });
            }
            view.OnSelected += (item) =>
            {



                if (onselect is not null)
                {

                    onselect((DescriptableObject)item.Item);

                }
            };
            return 1;
        }

        public int InjectDataModel<TEntity>(ListView view, IEnumerable<TEntity> dbset, Action<TEntity> onselect)
            where TEntity : DescriptableObject
        {
            view.Options.Clear();
            foreach (var p in dbset.ToList())
            {
                view.Options.Add(new Option()
                {
                    Value = p.Label,
                    Item = p
                });
            }
            view.OnSelected += (p) =>
            {


                var item = p is Option ? p.Item : p;
                if (onselect is not null)
                {
                    if (item is not NamedObject)
                    {
                        if (item is string)
                        {
                            dbset.Select(p => p.Name).ToList().ToJsonOnScreen().WriteToConsole();
                            this.Info(item);
                            onselect(dbset.First(p => p.Label == item));
                        }
                        else
                        {
                            throw new Exception($"Неожиданный тип объекта {item.GetType().GetTypeName()}");
                        }

                    }
                    else
                    {
                        onselect((TEntity)item);
                    }
                }
            };
            return 1;
        }
    }
}
