using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.InputModule
{
    [Label("Представление формы редактирования")]
    public class InputView2 : AppViewBase
    {

        [Label("target")]
        public object target { get => Get<object>(nameof(target)); set => Set(nameof(target), value); }

        [Label("properties")]
        public ChangingList<string> properties { get => Get<ChangingList<string>>(nameof(properties)); set => Set(nameof(properties), value); }

        [Label("errors")]
        public ChangingDictionary<string, ChangingList<string>> errors
        {
            get => Get<ChangingDictionary<string, ChangingList<string>>>(nameof(errors));
            set => Set(nameof(errors), value);
        }

        public ChangingDictionary<string, Action> buttons
        {
            get => Get<ChangingDictionary<string, Action>>(nameof(buttons));
            set => Set(nameof(buttons), value);
        }
        [InputBool]
        [Label("Is Validated")]
        public bool IsValidated { get => Get<bool>(nameof(IsValidated)); set => Set(nameof(IsValidated), value); }

        public InputView2()
        {
            target = new UserAccount();
            errors = new();
            buttons = new();
            IsValidated = false;
            properties = new ChangingList<string>(typeof(UserAccount).GetOwnInputPropertyNames());
        }

        public void ValidateTarget()
        {
            var results = target.Validate();
            errors.Values.ToList().ForEach(r => r.Clear());
            foreach (var item in results)
            {
                errors[item.Key].AddRange(item.Value);
            }
            IsValidated = errors.Values.Sum(v => v.Count()) == 0;
        }

        public void Set(object target, List<string> properties)
        {
            this.target = target;
            this.properties.Clear();
            this.properties.AddRange(properties);
        }

        //public override Control ToControl()
        //{
        //    var ctrl = new InputControl();
        //    ctrl.Set(this.target, this.properties.Select(p => p.ToString()).ToList());
        //    properties.OnAdded((i,p) => ctrl.input._properties.Add($"{p}"));
        //    return ctrl;
        //}
    }
}
