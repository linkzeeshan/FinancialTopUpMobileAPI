using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.IRepositories
{
    /// <summary>
    /// Generic Repo 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TEntityId"></typeparam>
    public interface IGenericRepository<TEntity, TEntityId> where TEntity : BaseAuditableEntity<TEntityId>
    {
        IQueryable<TEntity> Queryable();
        IQueryable<TEntity> SQLQuery(FormattableString query);
        IQueryable<TEntity> ExecuteSqlQuery<TEntityTable>(FormattableString sql);
        Task<TEntity> GetByIdAsync(TEntityId id);

        Task<TEntity> InsertAsync(TEntity entity);
        Task<IEnumerable<TEntity>> InsertRangeAsync(IEnumerable<TEntity> entities);

        Task<TEntity> UpdateAsync(TEntity entity);
        Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities);
        Task DeleteAsync(TEntityId id);
        Task DeleteAsync(TEntity entity);
        Task DeleteAsync(IEnumerable<TEntity> entities);
        Task<bool> SaveChangesAsync();
    }
}
