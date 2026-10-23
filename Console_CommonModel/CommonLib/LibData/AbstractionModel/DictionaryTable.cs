using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Справочник ( дополнительная информация, не обрабатывается приложением )
/// </summary>
public class DictionaryTable: DescriptableObject
{

    public async Task<IEnumerable<object>> SearchTag<T>(params string[] tags) where T: DictionaryTable
    {        
        var dbContextType = this.GetDbContextWithEntity();
        using (var pdb = dbContextType.New<DbContext>())
        {
            var pdbset = pdb.GetDbSet(this.GetTypeName());
            if (pdbset is null)
                throw new Exception("Не найден DbSet для " + this.GetTypeName());
            return ((IQueryable<dynamic>)pdbset).Where( record => tags.Any(tag => ((object)record).ToJson().IndexOf(tag.ToLower())!=-1) ).Select(p => (object)p);
        } 
    }

    public override string? ToString()
    {
        return this.Name;
    }
}
