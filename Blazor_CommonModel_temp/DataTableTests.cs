using Blazor_CommonModel.Components.Shared.Components;

using pickpoint_delivery_service;

namespace Console_CommonModel.CommonLib.LibViews.SharedViewCollections.DataTable
{
    public class DataTableTests : TestingElement
    {

        private static void ShowProducts()
        {
            using (var db = new DeliveryDbContext())
            {
                var table = new global::TableView();
                table.SetDataRows(db.Products.ToList());
                var g = new DataTableControl();
                g.BindTableView(table);
                g.RunAtForm();
            }
        }

        public override void OnTest()
        {
            ShowProducts();
        }
    }
}
