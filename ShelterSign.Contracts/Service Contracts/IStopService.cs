namespace Transit.Contracts.Service_Contracts
{
    using System.Collections.Generic;
    using System.ServiceModel;

    using Core.Common.Exceptions;

    using Transit.Business.Entities;
    using Transit.Common;


    [ServiceContract]
    public interface IStopService
    {
        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        [FaultContract(typeof(AuthorizationValidationException))]
        Stop GetStop(int stopId);

        [OperationContract]
        [FaultContract(typeof(AuthorizationValidationException))]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        IEnumerable<Stop> GetStops();

    }

}
