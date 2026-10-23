using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibDesigner.DesignerServices
{
    public class InputFormModelDesignerService : DesignerBase, IControlService<InputFormModel>
    {
        public InputFormModelDesignerService()
        {
        }

        public InputFormModel Control { get; set; }

        public void SetControl(InputFormModel item)
        {
            Control = item;
        }

        public void CreatePropertiesCheckList()
        {
            //имя-label
            var itemType = Control.ItemType.ToType();
            Dictionary<string, string> propertyLabels = new();
            foreach (var propertyName in itemType.GetPropertyNames())
            {
                propertyLabels[propertyName] = itemType.GetPropertyLabel(propertyName);
            }

            var allProperties = itemType.GetPropertyNames();
            var selectedProperties = new List<string>();
            var checkedList = Control.CreateCheckedListView(allProperties, (pindex, check) =>
            {
                if (check)
                {
                    selectedProperties.Add(allProperties[pindex]);
                }
                else
                {
                    selectedProperties.Remove(allProperties[pindex]);
                }
            });
            checkedList.ToSubmit((success, p) =>
            {

                if (success)
                {
                    Control.Properties.Clear();
                    Control.Properties.AddRange(selectedProperties);
                    Control.Update();
                }
                checkedList.ToControl().GetAppForm().Close();
            }).ShowDialog("Свойства");
        }

        protected override void Activating(ViewNode node)
        {
            if (node is AppViewBase view)
            {
                view.ContextMenu.AddContextServiceMethods("Настройки", new Dictionary<string, Action>()
                {
                    { "Свойства", ()=>{
                        CreatePropertiesCheckList();
                    } }
                });
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}