using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using OrderApi.Data.Database;

namespace OrderApi.Data.Repository
{
    // Class for basic CRUD operations for Entity
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, new()
    {
        protected readonly OrderContext orderContext;

        public Repository(OrderContext orderContext)
        {
            this.orderContext = orderContext;
        }

        public IEnumerable<TEntity> GetAll()
        {
            try
            {
                var entities = orderContext.Set<TEntity>();
                return entities;
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't retrieve entities: {ex.Message}");
            }
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentException($"{nameof(entity)} must not be null");
            }

            try
            {
                await orderContext.AddAsync(entity);
                await orderContext.SaveChangesAsync();

                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(entity)} could not be saved: {ex.Message}");
            }
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentException($"{nameof(entity)} must not be null");
            }

            try
            {
                orderContext.Update(entity);
                await orderContext.SaveChangesAsync();

                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(entity)} could not be updated: {ex.Message}");
            }
        }

        public async Task UpdateRangeAsync(List<TEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentException($"{nameof(entities)} must not be null");
            }

            try
            {
                orderContext.UpdateRange(entities);
                await orderContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(entities)} could not be updated: {ex.Message}");
            }
        }
    }
}
