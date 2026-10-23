using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Console_CommonModel.CommonLib.LibViews.AppViews;

namespace Console_CommonModel.CommonLib.LibViews.LayoutVIews
{
    internal class DatasourceLayoutViewTest : TestingElement
    {
        public override void OnTest()
        {
            var dsv = new DatasourceLayoutView();
            dsv.ShowDialog();
        }
    }
}
