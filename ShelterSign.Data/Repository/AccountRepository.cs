namespace Transit.Data.Repository
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Transit.Business.Entities;
    using Transit.Data.Repositoryinterfaces;

    [Export(typeof(IAccountRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AccountRepository : DataRepositoryBase<Account>, IAccountRepository
    {
        public Account GetByLogin(string login)
        {
            using (var entityContext = new TransitDbContext())
            {
                return (from a in entityContext.AccountSet
                        where a.LoginEmail == login
                        select a).FirstOrDefault();
            }
        }

        protected override Account AddEntity(TransitDbContext entityContext, Account entity)
        {
            return entityContext.AccountSet.Add(entity);
        }

        protected override IEnumerable<Account> GetEntities(TransitDbContext entityContext)
        {
            return from e in entityContext.AccountSet
                   select e;
        }

        protected override Account GetEntity(TransitDbContext entityContext, int id)
        {
            var query = from e in entityContext.AccountSet
                        where e.AccountId == id
                        select e;

            var results = query.FirstOrDefault();

            return results;
        }

        protected override Account UpdateEntity(TransitDbContext entityContext, Account entity)
        {
            return (from e in entityContext.AccountSet
                    where e.AccountId == entity.AccountId
                    select e).FirstOrDefault();
        }
    }
}