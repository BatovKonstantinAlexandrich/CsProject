using Console_CommonModel.CommonLib.LibData.AbstractionModel;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Label("Группировка пользователей")]
[UniqueConstraint($"{nameof(UserId)},{nameof(GroupId)}")]
public class UserGroupBinding: BaseEntity 
{

    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual UserContext  User { get; set; }
    public int UserId { get; set; } = 2;


    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual UserGroup Group { get; set; }
    public int GroupId { get; set; } = 2;

    public UserGroupBinding()
    {
        
    }


    public void Init()
    {
        
        using (var db = new DbContextUser())
        {
            var pairs = new List<string>();
            db.UserContexts.Select(user => user.Id).ToList().ForEach(id => {
                db.UserGroups.Select(g => $"{id}-{g.Id}").ToList().ForEach(pairs.Add);
            });
            var current = db.UserGroupsUserGroup.Select(ug => $"{ug.UserId}-{ug.GroupId}").ToList();
            var free = pairs.Where(pair => !current.Contains(pair));
            var p = free.FirstOrDefault();
            if (p is not null)
            {
                this.UserId = int.Parse(p.Split("-")[0]);
                this.GroupId = int.Parse(p.Split("-")[1]);
            }
        }
    }
}
