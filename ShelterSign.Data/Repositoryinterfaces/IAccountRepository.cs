namespace Transit.Data.Repositoryinterfaces
{
    using Core.Common.Contracts;

    using Transit.Business.Entities;

    public interface IAccountRepository : IDataRepository<Account>
    {
        Account GetByLogin(string login);
    }
}