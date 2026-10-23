using Microsoft.EntityFrameworkCore;

public abstract class EntityAttributeBase: Attribute
{ 
    public abstract void OnModelCreating(ModelBuilder modelBuilder, Type entityType, DbContext messageDbContext, string property=null);
}