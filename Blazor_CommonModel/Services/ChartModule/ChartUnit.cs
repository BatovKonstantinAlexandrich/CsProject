using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


[Label("Проверка представлений графиков (Charts)")]
public class ChartsUnit : ViewTesting<ChartView>
{
    public ChartsUnit() : base()
    {
    }

    public ChartsUnit(IServiceProvider provider) : base(provider)
    {
    }
}
