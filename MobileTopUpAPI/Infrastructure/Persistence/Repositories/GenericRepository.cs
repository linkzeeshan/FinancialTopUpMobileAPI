using Application.Common.Interfaces.IRepositories;
using Domain.Common;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Generic Repository
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TEntityId"></typeparam>
    public class GenericRepository<TEntity, TEntityId> : IGenericRepository<TEntity, TEntityId> where TEntity : BaseAuditableEntity<TEntityId>
    {
        #region Fields
        protected readonly ApplicationDbContext _dbContext;
        private DbSet<TEntity> _table = null;
        #endregion

        #region Ctor
        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext)); ;
            _table = _dbContext.Set<TEntity>();
        }
        #endregion

        #region Function Implementation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IQueryable<TEntity> SQLQuery(FormattableString query)
        {
            return _dbContext.Database.SqlQuery<TEntity>(query);
        }
        public IQueryable<TEntity> ExecuteSqlQuery<TEntityTable>(FormattableString sql)
        {
            return _dbContext.Set<TEntity>().FromSqlInterpolated(sql).AsNoTracking();
        }


        /// <summary>
        /// Generic Table we can Implement any query on it
        /// </summary>
        /// <returns></returns>
        public IQueryable<TEntity> Queryable()
        {
            return _table.AsNoTracking().AsQueryable();
        }
        /// <summary>
        /// Get entity by entity id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TEntity> GetByIdAsync(TEntityId id)
        {
            return await _table.FindAsync(id);
        }
        /// <summary>
        /// Insert entity 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            await _table.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
        public async Task<IEnumerable<TEntity>> InsertRangeAsync(IEnumerable<TEntity> entities)
        {
            await _table.AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();
            return entities;
        }

        /// <summary>
        /// update entity 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            // Detach the existing tracked entity
            _table.Entry(entity).State = EntityState.Detached;

            // Attach the updated entity and mark it as modified
            _table.Attach(entity);
            _table.Entry(entity).State = EntityState.Modified;
            await SaveChangesAsync();
            // Reload the entity from the database to get the latest state
            await _table.Entry(entity).ReloadAsync();

            // Return the updated entity
            return entity;
        }
        /// <summary>
        /// list of record will update
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            // Attach all entities and mark them as modified
            foreach (var entity in entities)
            {
                _table.Attach(entity);
                _dbContext.Entry(entity).State = EntityState.Modified;
            }

            await SaveChangesAsync();

            // Reload each entity from the database to get the latest state
            foreach (var entity in entities)
            {
                await _table.Entry(entity).ReloadAsync();
            }

            // Return the updated entities
            return entities;
        }

        /// <summary>
        /// Delete entity by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(TEntityId id)
        {
            var entityToDelete = await _table.FindAsync(id);
            if (entityToDelete != null)
            {
                _table.Remove(entityToDelete);
                await SaveChangesAsync();
            }
        }
        /// <summary>
        /// Delete entity by id
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task DeleteAsync(TEntity entity)
        {
            if (entity != null)
            {
                _table.Remove(entity);
                await SaveChangesAsync();
            }
        }
        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public async Task DeleteAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            _table.RemoveRange(entities.ToList());
            await SaveChangesAsync();
        }
        /// <summary>
        /// entity save changes in table
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SaveChangesAsync()
        {
            return (await _dbContext.SaveChangesAsync() >= 0);
        }
        /// <summary>
        /// entity update
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task Update(TEntity entity)
        {
            if (_table == null)
            {
                throw new ArgumentNullException("entity");
            }
            _table.Update(entity);
            await SaveChangesAsync();
        }
        #endregion
    }
}
