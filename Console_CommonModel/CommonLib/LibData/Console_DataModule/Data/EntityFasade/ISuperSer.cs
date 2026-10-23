using Console_CommonModel.CommonLib.LibData.AbstractionModel;

using System.Collections.Generic;

public interface ISuperSer<TEntity> : ISet<TEntity>, System.Linq.IQueryable<TEntity> where TEntity : BaseEntity
{
    void Update<TEntity>(TEntity data) where TEntity : BaseEntity;
}