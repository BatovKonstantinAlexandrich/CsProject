
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

using Console_CommonModel.CommonLib.LibViews.AppViews.TextModule;

namespace Console_CommonModel.CommonLib.LibViews.SharedViewCollections
{

    [Label("Карточка сущности")]
    public class EntityFormCard : BaseFormBuilder
    {
        [Label("Title")]

        public string Title { get; set; }
        private ChangingList<AppViewBase> formControls = new();
        public EntityFormCard()
        {
            this.Add(new TextView() { Text = Title, FontSize = 20 });
            Direction = "TopDown";
            MinWidth = MinHeight = 250;
            formFields.Bind(formControls, createCardView);
            formControls.OnChanged += OnFormControlsChanged;
        }

        private void OnFormControlsChanged(object sender, ListChangedEvent evt)
        {
            switch (evt.Target.Action)
            {
                case "added": Add((AppViewBase)evt.Target.Item); break;
                case "removed": Remove((AppViewBase)evt.Target.Item); break;
            }
        }

        private AppViewBase createCardView(InputFormField field)
        {
            /*this.Add(new TextView() { Text = Title, FontSize = 20 });

            var card = new FlexView();
            card.Add(card.CreateLabel(field.Name));
            card.Height = 200;*/
            var b = new ButtonView() { Text = field.Name };
            Add(b);
            return b;
        }

        //public override Control ToControl()
        //{
        //    return base.ToControl<CardView>();
        //}
    }
}
