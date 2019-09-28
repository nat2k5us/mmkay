namespace WcfMathService
{
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using Sample.Models;

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IMathService1" in both code and config file together.
    [ServiceContract]
    public interface IMathService1
    {
        [OperationContract]
        [FaultContract(typeof(FaultInfo))]
        // [WebInvoke(Method = "Add", RequestFormat = WebMessageFormat.Xml)]
        int Add(int x, int y);

        [OperationContract]
        [FaultContract(typeof(FaultInfo))]
        // [WebInvoke(Method = "Add", RequestFormat = WebMessageFormat.Xml)]
        int Divide(int x, int y);
    }

}