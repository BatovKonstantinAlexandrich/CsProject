using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class TreeService
{

    public static Tree ToTree(string TypeName)
    {
        using (var db = new ApplicationDbContext())
        {

            Func<object, bool> GetParentIdProperty = (item) => {
                return ReflectionService.GetValueFor(item, "ParentID") == null;
            };
            return db.GetDbSet(TypeName).Where(GetParentIdProperty).FirstOrDefault().ToTree();
        }
    }
}