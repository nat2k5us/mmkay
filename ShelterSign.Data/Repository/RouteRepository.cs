namespace Transit.Data.Repository
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Transit.Business.Entities;
  
    using Transit.Data.Repositoryinterfaces;

    [Export(typeof(RouteRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)] // Not Singleton
    public class RouteRepository : DataRepositoryBase<Route>, IRouteRepository
    {
        protected override Route AddEntity(TransitDbContext entityContext, Route entity)
        {
            return entityContext.RouteSet.Add(entity);
        }

        protected override IEnumerable<Route> GetEntities(TransitDbContext entityContext)
        {
            return from e in entityContext.RouteSet select e;
        }

        protected override Route GetEntity(TransitDbContext entityContext, int id)
        {
            var query = from e in entityContext.RouteSet where e.RouteId == id select e;
            var results = query.FirstOrDefault();
            return results;
        }

        protected override Route UpdateEntity(TransitDbContext entityContext, Route entity)
        {
            return (from e in entityContext.RouteSet
                    where e.RouteId == entity.RouteId
                    select e).FirstOrDefault();
        }
    }
}