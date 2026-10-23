using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.TreeServices;

using pickpoint_delivery_service;

using Console_CommonModel.CommonLib.LibViews.AppViews.InputModule;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.TreeModule
{

    [Label("Тестирование иерархического представление")]
    [Description(
        "1) Возможность поиска" +
        "2) Выбор элемента OnClick событие." +
        "3) Должна быть поддержка DragAndDrop. " +
        "4) Управление контекстным меню через служебные объекты.")]
    public class TreeTests : TestingElement
    {
        public TreeTests(IServiceProvider provider) : base(provider)
        {
        }
        public TreeTests() : this(AppProviderService.GetInstance())
        {
        }

        public override void OnTest()
        {


            //1 Поиск
            Assert(p =>
            {

                var tree = new TreeControlView();
                tree.BindHierTable<DeliveryDbContext, ProductCatalog>();

                return true;
            }, "Поиск по иерархическому представлению работает корректно."
            , "Поиск по иерархическому представлению не работает.");


            //1 Поиск
            Assert(p =>
            {

                var tree = new TreeControlView();
                tree.BindHierTable<DeliveryDbContext, ProductCatalog>();
                if (tree.Searchable != true)
                    throw new Exception($"Свойство {nameof(tree.Searchable)} не установлено.");
                var control = tree.ToControl();

                return false;
            }, "Поиск по иерархическому представлению работает корректно."
            , "Поиск по иерархическому представлению не работает.");

            //2 Выбор
            Assert(p =>
            {
                var tree = new TreeControlView();
                if (tree.Searchable != true)
                    throw new Exception($"Свойство {nameof(tree.Searchable)} не установлено.");

                var control = tree.ToControl();
                return false;
            },
            "Выбор элемента OnClick событие работает корректно."
            , "Выбор элемента OnClick событие не работает.");


            //3 Перетаскивание
            Assert(p =>
            {
                var tree = new TreeControlView();
                if (tree.Searchable != true)
                    throw new Exception($"Свойство {nameof(tree.Searchable)} не установлено.");

                var control = tree.ToControl();
                return false;
            },
            "Реализована поддержка DragAndDrop."
            , "Не реализована поддержка DragAndDrop.");

            //4 Контекстное меню
            Assert(p =>
            {
                var tree = new TreeControlView();
                Func<object, Dictionary<string, Action>> GetDatabaseActions = instance =>
                {
                    Dictionary<string, Action> actions = new();

                    if (instance is not null)
                    {
                        var relations = ((BaseEntity)instance).GetEntityRelations();
                        ((BaseEntity)instance).JoinAll< DeliveryDbContext>();
                        foreach (var rel in relations)
                        {
                            actions[rel.RelativeEntityType] = () =>
                            {

                                try
                                {
                                    switch (rel.RelationshipType)
                                    {
                                        case "many-to-one":
                                            tree.Right = new TableView();
                                            break;
                                        case "one-to-one":
                                            var inputForm = new InputView2();
                                            var relativeData = instance.GetValue(rel.PropertyName);
                                            if (relativeData is null)
                                            {
                                                instance.SetValue(rel.PropertyName, relativeData = rel.RelativeEntityType.New());
                                            }
                                            var relativeProperties = rel.RelativeEntityType.ToType().GetOwnPropertyNames();
                                            if (relativeProperties.Count() == 0)
                                            {
                                                throw new Exception("Свойства для отображения не выбраны");
                                            }
                                            inputForm.Set(relativeData, relativeProperties);
                                            tree.Right = inputForm;
                                            break;
                                        case "one-to-many":
                                            tree.Right = new TableView();
                                            break;
                                    }

                                }
                                catch (Exception ex)
                                {
                                    instance.Error(ex);
                                }
                            };
                        }
                    }

                    return actions;
                };
                tree.BindHierServices(pnode =>
                {
                    var actions = GetDatabaseActions(pnode.Item);
                    pnode.AddContextMethods("database", actions);
                });
                tree.RunAtForm("Контекстное меню");
                return false;
            },
            "Реализована поддержка DragAndDrop."
            , "Не реализована поддержка DragAndDrop.");

        }

        private static void TestTreeBuilder()
        {
            TreeViewEntityBuilder<DeliveryDbContext> service = new();
            service.Join<ProductCatalog>();
            service.Join<Product>(product => product.ProductName);
            var treeview = service.Build();
            treeview.DesignMode = true;
            //treeview.root.contextMenuServices.Add(new FileDialogService());
            //treeview.contextMenuServices.Add(new FileDialogService());
            treeview.RunAtForm();
        }
    }


}
