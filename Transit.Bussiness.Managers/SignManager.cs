namespace Transit.Business.Managers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ServiceModel;

    using Core.Common.Contracts;
    using Core.Common.Core;
    using Core.Common.Exceptions;

    using Transit.Business.Entities;
    using Transit.Contracts.Service_Contracts;
    using Transit.Data.Repositoryinterfaces;

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall,
        ConcurrencyMode = ConcurrencyMode.Multiple,
        ReleaseServiceInstanceOnTransactionComplete = false)]
    public class SignManager : ISignService
    {
        [Import]
        private IDataRepositoryFactory dataRepositoryFactory;

        public SignManager()
        {
            ObjectBase.Container.SatisfyImportsOnce(this);
        }

        public SignManager(IDataRepositoryFactory dataRepositoryFactory)
        {
            this.dataRepositoryFactory = dataRepositoryFactory;
        }

        public Sign GetSign(int signId)
        {
            try
            {
                var signRepository = this.dataRepositoryFactory.GetDataRepository<ISignRepository>();
                var sign = signRepository.Get(signId);
                if (sign == null)
                {
                    var ex = new NotFoundException($"Sign with {signId} was not found");
                    throw new FaultException<NotFoundException>(ex, ex.Message);
                }

                return sign;
            }
            catch (FaultException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }

        public IEnumerable<Sign> GetSigns()
        {
            try
            {
                var signRepository = this.dataRepositoryFactory.GetDataRepository<ISignRepository>();

                var signs = signRepository.Get();
                if (signs == null)
                {
                    var ex = new NotFoundException($"No Signs were found");
                    throw new FaultException<NotFoundException>(ex, ex.Message);
                }

                return signs;
            }
            catch (FaultException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }
    }
}