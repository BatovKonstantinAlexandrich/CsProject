using ApplicationUnit.Encoder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class HaffmanAlgExtensions
{

    public static string Seriallize(this object target)
    {
        var enc = new CharacterEncoder();
        return enc.Encode(target.ToJson());
    }
    public static TTable Deseriallize<TTable>(this string target)
    {
        var enc = new CharacterEncoder();
        return enc.Decode(target).FromJson<TTable>();
    }
    public static TTable FromJson<TTable>(this string target)
    {
        return JsonConvert.DeserializeObject<TTable>(target);
        
    }
}