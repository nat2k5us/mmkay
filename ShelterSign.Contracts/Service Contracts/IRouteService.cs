namespace Transit.Contracts.Service_Contracts
{
    using System.Collections.Generic;
    using System.ServiceModel;

    using Core.Common.Exceptions;

    using Transit.Business.Entities;
    using Transit.Common;


    [ServiceContract]
    public interface IRouteService
    {
        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        [FaultContract(typeof(AuthorizationValidationException))]
        Route GetRoute(int routeId);

        [OperationContract]
        [FaultContract(typeof(AuthorizationValidationException))]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        IEnumerable<Route> GetRoutes();

    }

}
