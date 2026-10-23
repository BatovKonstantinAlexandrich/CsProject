 
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
public static class ModelBuilderExtensions
{
    public static void ApplyAnnotations( this ModelBuilder builder, DbContext context )
    {
   
        foreach (var type in context.GetEntitiesTypes())
        {
            Dictionary<string, string> dictionary = GetEntityContrainsts(type);
            foreach (var p in dictionary)
            {
                switch (p.Key)
                {
                    case nameof(EntityLabelAttribute):
                        builder.Entity(type).HasComment(p.Value);
                        break;

                    case nameof(UniqueConstraintAttribute):
                        builder.Entity(type).HasIndex(p.Value.Split(",")).IsUnique();
                        break;
                     
                }

            }
        }
       
    }
    public static Dictionary<string, string> GetEntityContrainsts(Type type)
    {
        Dictionary<string, string> attrs = new Dictionary<string, string>();
        foreach (var data in type.GetCustomAttributesData())
        {
            if (data.AttributeType.IsExtendsFrom(typeof(ModelCreatingAttribute).Name))
            {
                foreach (var arg in data.ConstructorArguments)
                {
                    string value = arg.Value.ToString();
                    attrs[data.AttributeType.Name] = value;
                }
            }

        }
        return attrs;
    }
}
 