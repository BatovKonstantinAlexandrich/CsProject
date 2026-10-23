namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataApi
{

    /// <summary>
    /// Дополнительная валидация ValidateOptional() исп. в базовом методе валидации
    /// Validate в том случае если этот метод определён в модели.
    /// </summary>
    public interface IValidateOptional
    {
        public Dictionary<string, List<string>> ValidateOptional();
    }
}
