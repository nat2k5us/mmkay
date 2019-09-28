namespace Transit.Data.Repository
{
    using Core.Common.Contracts;
    using Core.Common.Data;

    public abstract class DataRepositoryBase<T> : DataRepositoryBase<T, TransitDbContext>
        where T : class, IIdentifiableEntity, new()
    {
         
    }
}