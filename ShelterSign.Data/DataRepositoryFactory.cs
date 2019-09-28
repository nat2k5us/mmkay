namespace Transit.Data
{
    using System.ComponentModel.Composition;

    using Core.Common.Contracts;
    using Core.Common.Core;
    [Export(typeof(IDataRepositoryFactory))]
    [PartCreationPolicy(CreationPolicy.NonShared)] // Not Singleton
    public class DataRepositoryFactory : IDataRepositoryFactory
    {
        public T GetDataRepository<T>() where T : IDataRepository
        {
            return ObjectBase.Container.GetExportedValue<T>();
        }
    }
}