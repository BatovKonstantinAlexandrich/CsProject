using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/// <summary>
/// Определение языковых признаков
/// </summary>
public static class TextLangExtensions
{
    public static bool IsRus(this string word)
    {
        string alf = "АаБбВвГгДдЕеЁёЖжЗзИиЙйКкЛлМмНнОоПпРрСсТтУуФфХхЦцЧчШшЩщЪъЫыЬьЭэЮюЯя";
        string text = word;
        for (int i = 0; i < text.Length; i++)
        {
            if (!alf.Contains(text[i]))
            {
                return false;
            }
        }
        return true;
    }

    public static bool IsEng(this string word)
    {
        string alf = "qwertyuiopasdfghjklzxcvbnm" + "qwertyuiopasdfghjklzxcvbnm".ToUpper();
        string text = word;
        for (int i = 0; i < text.Length; i++)
        {
            if (!alf.Contains(text[i]))
            {
                return false;
            }
        }
        return true;
    }
}