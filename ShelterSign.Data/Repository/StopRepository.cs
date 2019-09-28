namespace Transit.Data.Repository
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Transit.Business.Entities;
    using Transit.Data.Repositoryinterfaces;

    [Export(typeof(IStopRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)] // Not Singleton
    public class StopRepository : DataRepositoryBase<Stop>, IStopRepository
    {
        protected override Stop AddEntity(TransitDbContext entityContext, Stop entity)
        {
            return entityContext.StopSet.Add(entity);
        }

        protected override IEnumerable<Stop> GetEntities(TransitDbContext entityContext)
        {
            return from e in entityContext.StopSet select e;
        }

        protected override Stop GetEntity(TransitDbContext entityContext, int id)
        {
            var query = from e in entityContext.StopSet where e.StopId == id select e;
            var results = query.FirstOrDefault();
            return results;
        }

        protected override Stop UpdateEntity(TransitDbContext entityContext, Stop entity)
        {
            return (from e in entityContext.StopSet
                    where e.StopId == entity.StopId
                    select e).FirstOrDefault();
        }
    }
}