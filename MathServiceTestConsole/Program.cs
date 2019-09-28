using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathServiceTestConsole
{
    using System.ServiceModel;

    using MathServiceTestConsole.MathServiceReference;

    class Program
    {
        static void Main(string[] args)
        {
            MathService1Client client = new MathService1Client();
            Console.WriteLine(client.Add(5, 8));
            Console.WriteLine(client.Add(5000, 81234));
            Console.WriteLine(client.Divide(1, 2));

            try
            {
                Console.WriteLine(client.Divide(1, 0));
            }
            catch (FaultException<FaultInfo> exception)
            {
                Console.WriteLine(exception.Message);
            }
            catch (FaultException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (CommunicationException exception)
            {
                Console.WriteLine(exception.Message);
            }
            Console.ReadKey();
        }
    }
}
