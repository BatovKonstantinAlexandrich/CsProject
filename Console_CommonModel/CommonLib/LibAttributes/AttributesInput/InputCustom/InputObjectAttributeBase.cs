namespace Console_CommonModel.CommonLib.LibAttributes.AttributesInput.InputCustom
{
    [Label("Динамический объект")]
    public abstract class InputObjectAttributeBase<TControl> : ControlAttribute
    {
        protected readonly string _typeName;

        public InputObjectAttributeBase() : base(InputTypes.Object)
        {
            _InputType = InputTypes.Object;
        }

        public InputObjectAttributeBase(string typeName) : base(InputTypes.Object)
        {
            _InputType = InputTypes.Object;
            _typeName = typeName;
        }


    }

}
