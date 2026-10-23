using System;
using System.Linq;


public class AuthorizationValidation
{
    public AuthorizationValidation() {}  
    public void Validate(ApplicationDbContext db)
    {
        AuthorizationOptions options = new AuthorizationOptions();
        BusinessResource basRole = db.BusinessResources.Where(r => r.Code == options.PublicRole).SingleOrDefault();
        if (basRole == null)
        {
            throw new Exception("Не определена базовая роль пользователя, либо отсутсвует запись о ней в базе данных");
        }
        db.BusinessResources.Add(new BusinessResource() { 
            Name = options.PublicRole,
            Code = options.PublicRole,
            Description = 
                    "Пользователь имеют возможность обмениваться сообщениями, " +
                    "создавать и вступать в гуппы. "
        });
        db.SaveChanges();         
    }
}