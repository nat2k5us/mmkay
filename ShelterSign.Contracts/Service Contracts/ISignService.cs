namespace Transit.Contracts.Service_Contracts
{
    using System.Collections.Generic;
    using System.ServiceModel;

    using Core.Common.Exceptions;

    using Transit.Business.Entities;
    using Transit.Common;

    [ServiceContract]
    public interface ISignService
    {
        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        [FaultContract(typeof(AuthorizationValidationException))]
        Sign GetSign(int signId);

        [OperationContract]
        [FaultContract(typeof(AuthorizationValidationException))]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        IEnumerable<Sign> GetSigns();
    }
}
