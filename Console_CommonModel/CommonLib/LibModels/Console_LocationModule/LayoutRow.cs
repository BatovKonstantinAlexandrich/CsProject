using Console_CommonModel.CommonLib.LibData.AbstractionModel;

using Console_CommonModel.CommonLib.LibModels.Console_LocationModule;

namespace Console_CommonModel.CommonLib.LibModels.Console_LocationModule
{
    public class LayoutRow : BaseEntity
    {
        public List<LocationDbContext.PageComponent> children { get; set; } = new();

        public LayoutRow() { }
        public LayoutRow(LocationDbContext.PageComponent[] children)
        {
            this.children.AddRange(children);
        }
    }
}