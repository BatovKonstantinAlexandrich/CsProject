using Console_CommonModel.CommonLib.LibViews.AppViewModules.TextBoxModule;
using Console_CommonModel.CommonLib.LibViews.AppViews;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.ImageModule
{
    [Label($"Тестирование {nameof(ImageView)}")]
    public class ImageTests : ViewTesting<ImageView>
    {
        public ImageTests() : base()
        {
        }

        public ImageTests(IServiceProvider provider) : base(provider)
        {
        }
    }
}
