[Label("Баланс лицевого счёта")]
[Icon("attach_money")]
public class InputCurrencyAttribute : InputPositiveIntAttribute
{
    public InputCurrencyAttribute() : base(InputTypes.Currency) { }

    public InputCurrencyAttribute(string message) : base(message) { }
}