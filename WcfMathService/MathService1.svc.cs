using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WcfMathService
{
    using Sample.Models;

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "MathService1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select MathService1.svc or MathService1.svc.cs at the Solution Explorer and start debugging.
    public class MathService1 : IMathService1
    {
        public Int32 Add(Int32 x, Int32 y)
        {
            return x + y;
        }

        public int Divide(int x, int y)
        {
            var result = 0;
            try
            {
                result = x / y;
            }
            catch (Exception exception)
            {
                FaultInfo fi = new FaultInfo { Reason = "Division by Zero." };
                  throw new FaultException<FaultInfo>(fi, new FaultReason(fi.Reason));
                //throw new FaultException(exception.Message);
                Console.WriteLine(exception);
            }
           
            return result;
        }
    }
}
