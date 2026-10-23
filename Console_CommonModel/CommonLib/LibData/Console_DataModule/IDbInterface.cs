using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule
{
    public interface IDbInterface
    {
        public EditView CreateExecuterForm(string schema, string name);
    }
}