using ActionModels;

using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.ColumnsModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.TextModule;

namespace Console_CommonModel.CommonLib.LibViews.Others
{
    [Label("Карточка товара")]
    public class ProductCardView : AppViewBase
    {
        private readonly ColumnsView columnsPane;
        private readonly TextView titleLabel;
        private Product p;

        public ProductCardView() : this(new Product()) { }

        public ProductCardView(Product p)
        {
            this.p = p;
            //BorderWidth = 1;
            Margin = 20;
            MinWidth = 280;
            MinHeight = 280;
            LayoutFill(columnsPane = new ColumnsView());
            columnsPane.Width = 180;
            //columnsPane.Add(titleLabel = CreateTextCenter(p.ProductName));

            var imageView = new ImageView();
            imageView.Width = imageView.Height = 180;
            if (p.ProductImages.Count() > 0)
                imageView.Data = p.ProductImages.First().ImageData;
            columnsPane.Add(imageView);
        }
    }
}