namespace Core.Common.Data
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;

    using global::Core.Common.Contracts;

    public abstract class DataRepositoryBase<T, U> : IDataRepository<T>
        where T : class, IIdentifiableEntity, new()
        where U : DbContext, new()
    {
        public T Add(T entity)
        {
            using (var entityContext = new U())
            {
                var addedEntity = this.AddEntity(entityContext, entity);
                entityContext.SaveChanges();
                return addedEntity;
            }
        }

        public IEnumerable<T> Get()
        {
            using (var entityContext = new U()) return this.GetEntities(entityContext).ToArray().ToList();
        }

        public T Get(int id)
        {
            using (var entityContext = new U()) return this.GetEntity(entityContext, id);
        }

        public void Remove(T entity)
        {
            using (var entityContext = new U())
            {
                entityContext.Entry(entity).State = EntityState.Deleted;
                entityContext.SaveChanges();
            }
        }

        public void Remove(int id)
        {
            using (var entityContext = new U())
            {
                var entity = this.GetEntity(entityContext, id);
                entityContext.Entry(entity).State = EntityState.Deleted;
                entityContext.SaveChanges();
            }
        }

        public T Update(T entity)
        {
            using (var entityContext = new U())
            {
                var existingEntity = this.UpdateEntity(entityContext, entity);

                // SimpleMapper.PropertyMap(entity, existingEntity);
                entityContext.SaveChanges();
                return existingEntity;
            }
        }

        protected abstract T AddEntity(U entityContext, T entity);

        protected abstract IEnumerable<T> GetEntities(U entityContext);

        protected abstract T GetEntity(U entityContext, int id);

        protected abstract T UpdateEntity(U entityContext, T entity);
    }
}