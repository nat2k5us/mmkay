namespace Transit.Data.Repository
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Transit.Business.Entities;

    using Transit.Data.Repositoryinterfaces;

    [Export(typeof(ISignRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)] // Not Singleton
    public class SignRepository : DataRepositoryBase<Sign>, ISignRepository
    {
        protected override Sign AddEntity(TransitDbContext entityContext, Sign entity)
        {
            return entityContext.SignSet.Add(entity);
        }

        protected override IEnumerable<Sign> GetEntities(TransitDbContext entityContext)
        {
            try
            {
                return entityContext.SignSet.ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}");
                return new List<Sign>();
            }
           
        }

        protected override Sign GetEntity(TransitDbContext entityContext, int id)
        {
            var query = from e in entityContext.SignSet where e.SignId == id select e;
            var results = query.FirstOrDefault();
            return results;
        }

        protected override Sign UpdateEntity(TransitDbContext entityContext, Sign entity)
        {
            return (from e in entityContext.SignSet
                    where e.SignId == entity.SignId
                    select e).FirstOrDefault();
        }
    }
}