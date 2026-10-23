using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibViews.Others
{
    [Label("Контейнер представления")]
    public class ContainerView : AppViewBase
    {

        [InputConstructor<AppViewBase>()]
        [NotNullNotEmpty]
        [Label("Service")]
        public object Service { get; set; }

        public ContainerView()
        {
            BackgroundColor = "#f1f2f3";
            BorderWidth = 1;
            MinHeight = MinWidth = 320;
        }

        List<Action> rollback = new();



        public void AddContextMenuForActions(string name = "Действия")
        {
            foreach (var todo in rollback) todo();
            rollback.Clear();
            if (Service is null)
                throw new ArgumentNullException(nameof(Service));
            rollback.Add(AddContextServiceMethods(name, Service, met =>
                GetUserInputParametersSide(met)));
        }

        public void AddContextMenuForActions(List<string> methods)
        {
            if (Service is null)
                throw new ArgumentNullException(nameof(Service));
            rollback.Add(AddContextServiceMethods("Действия", Service, met =>
                GetUserInputParameters(met)));
        }


        public void Create()
        {
            if (Service is null)
                throw new ArgumentNullException(nameof(Service));
            rollback.Add(AddContextServiceMethods("Действия", Service, met =>
                GetUserInputParameters(met)));
        }
    }
}
