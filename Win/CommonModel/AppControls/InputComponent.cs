using WinFormsApp.AppViews.BaseComponent;

namespace WinFormsApp.AppComponents.AppControls
{


    /*public class InputComponent : ComponentBase<FormViewModel, FormControlView>
    {
        [NotNullNotEmpty]
        public readonly FormViewModel formViewModel;

        [NotNullNotEmpty]
        public readonly FormControlView formControlView;

        public InputComponent(FormViewModel formViewModel, FormControlView formControlView = null) : base(formViewModel, formControlView)
        {
            this.formViewModel = formViewModel;
            if (formControlView is null)
            {
                formControlView = new FormControlView(formViewModel.Item);
                this.formControlView = formControlView;
            }
            this.EnsureIsValide();
        }

        public override void Bind()
        {
            throw new NotImplementedException();
        }
    }*/
}
