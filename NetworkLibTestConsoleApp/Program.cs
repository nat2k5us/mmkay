using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Xml;

using NetoworkLib;

namespace NetworkLibTestConsoleApp
{
    using System.Runtime.InteropServices;

    class Program
    {
        #region Static Fields

        private static string adaptorDescription = string.Empty;

        private static bool choiceMade;

        private static bool isDhcp;

     

        #endregion

        #region Methods

        private static void DisplayNics( bool onlyPhysicalNics = false)
        {
          NetworkUtils.ShowNetworkInterfaces(onlyPhysicalNics);
            if (!choiceMade)
            {
                UserMessages.WelcomeMessage();
            }
            else
            {
                UserMessages.HelpKeysMessage();
            }
          
        }

        private static void ShowNicNames()
        {
            int i = 1;
            foreach (var x in NetworkInterface.GetAllNetworkInterfaces().ToList())
            {
                Console.WriteLine(i++ + ". " + x.Description + " : " + x.Name + " : " + x.OperationalStatus);
                Console.WriteLine(
                    "   IP Address: .....................................: " + NetworkUtils.GetIpFromAdaptorDesc(x.Description));
                Console.WriteLine(
                    "   DHCP Enabled.....................................: "
                    + !NetworkManagement.IsNetworkedWithStaticIp(x.Description));
                Console.WriteLine();

              
            }
         
        }

        private static void Main(string[] args)
        {
            ShowNicNames();
            UserMessages.WelcomeMessage();

            var isProcessing = false;
            while (!Console.KeyAvailable)
            {
                var keey = Console.ReadKey(true).Key;
                if (keey == ConsoleKey.Escape)
                {
                    break;
                }
                if (choiceMade)
                {
                    if (!string.IsNullOrEmpty(adaptorDescription))
                    {
                        UserMessages.HelpKeysMessage();
                        if (keey == ConsoleKey.S && !isProcessing)
                        {
                            isProcessing = true;
                            try
                            {
                                Console.WriteLine("S - Key pressed - Setting static IP then resetting to original");
                                Console.Write("Enter the Static IP:");
                                var staticIp = Console.ReadLine();
                                Console.Write("Subnet Mask (presss Enter to use 255.255.255.0):");
                                var subnetMask = Console.ReadLine();
                                if (string.IsNullOrEmpty(subnetMask))
                                {
                                    subnetMask = "255.255.255.0";
                                }
                                var gatewayForAdaptor = NetworkManagement.GetGatewayForAdaptor(adaptorDescription);
                                Console.Write("Gateway (presss Enter to use current {0}: ", gatewayForAdaptor);
                                var gateway = Console.ReadLine();
                                if (string.IsNullOrEmpty(gateway))
                                {
                                    gateway = gatewayForAdaptor.ToString();
                                }
                                isDhcp = !NetworkManagement.IsNetworkedWithStaticIp(adaptorDescription);

                                if (isDhcp)
                                {
                                    NetworkManagement.SetSystemIp(staticIp, subnetMask, adaptorDescription);
                                    NetworkManagement.SetSystemGateway(gateway, adaptorDescription);
                                    //  Thread.Sleep(10000);
                                    //   NetworkManagement.SetDhcp(adaptorDescription);
                                    //   Thread.Sleep(10000);
                                    DisplayNics(true);
                                }
                                else
                                {
                                    var currentIpAddress = NetworkUtils.GetIpFromAdaptorDesc(adaptorDescription);
                                    var currentGateway = gatewayForAdaptor;
                                    var currentsubnetMask = NetworkUtils.GetIp4MaskFromAdaptorDesc(adaptorDescription);
                                    NetworkManagement.SetSystemIp(staticIp, subnetMask, adaptorDescription);
                                    NetworkManagement.SetSystemGateway(gateway, adaptorDescription);
                                    //Thread.Sleep(10000);

                                    //  NetworkManagement.SetSystemIp(currentIpAddress.ToString(), currentsubnetMask.ToString(), adaptorDescription);
                                    //  NetworkManagement.SetSystemGateway(currentGateway.ToString(), adaptorDescription);
                                    DisplayNics(true);
                                }
                                isProcessing = false;

                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        }


                        if (keey == ConsoleKey.D)
                        {
                            Console.WriteLine("D - Key pressed - setting to DHCP then resetting to original");
                            isProcessing = true;
                            isDhcp = !NetworkManagement.IsNetworkedWithStaticIp(adaptorDescription);
                            if (isDhcp)
                            {
                                Console.WriteLine("Already in DHCP - Skipping");
                            }
                            else
                            {
                                var currentIpAddress = NetworkUtils.GetIpFromAdaptorDesc(adaptorDescription);
                                var currentsubnetMask = NetworkUtils.GetIp4MaskFromAdaptorDesc(adaptorDescription);
                                NetworkManagement.SetDhcp(adaptorDescription);
                                Thread.Sleep(10000);
                                NetworkManagement.SetSystemIp(currentIpAddress.ToString(), currentsubnetMask.ToString(), adaptorDescription);
                                Thread.Sleep(10000);
                            }
                            isProcessing = false;
                            DisplayNics();
                        }

                        if (keey == ConsoleKey.E)
                        {
                            DisplayNics( true);
                        }

                        if (keey == ConsoleKey.I)
                        {
                            Console.WriteLine(NetworkUtils.ActiveNetworkInterface());
                        }
                        if (keey == ConsoleKey.V)
                        {
                            NetworkUtils.ShowNetworkInterfaces(false, true);
                        }
                        if (keey == ConsoleKey.N)
                        {
                           ShowNicNames();
                        }

                        if (keey == ConsoleKey.T)
                        {
                            Console.WriteLine("Key T Pressed - Is Static IP");
                            Console.WriteLine(NetworkUtils.GetAdapterIp4Address(adaptorDescription));
                            Console.WriteLine(NetworkManagement.IsNetworkedWithStaticIp(adaptorDescription));
                        }

                        if (keey == ConsoleKey.G)
                        {
                            try
                            {
                                Console.Write("Key G Pressed - Enter the gateway IP:");
                                var gateway = Console.ReadLine();
                                Console.WriteLine(" Set the Gateway to {0} ", gateway);
                                Console.WriteLine(NetworkManagement.SetSystemGateway(gateway, adaptorDescription));
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        }
                        if (keey == ConsoleKey.F)
                        {
                            var Ip = NetworkUtils.GetRandomIp();

                            Console.WriteLine("Key F Pressed - Set the IP to : " + Ip);
                            NetworkManagement.SetSystemIp(Ip, "255.255.255.0", adaptorDescription);
                            Thread.Sleep(5000);
                            NetworkUtils.ShowNetworkInterfaces(true, false);
                        }

                        if (keey == ConsoleKey.M)
                        {
                            Console.WriteLine("Returning to Nic Adaptor Selection");
                            choiceMade = false;
                            DisplayNics();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Adapter is not set");
                        choiceMade = false;
                    }
                }
                else
                {
                    switch (keey)
                    {
                        case ConsoleKey.NumPad1:
                        case ConsoleKey.D1:
                            SetActiveNetworkAdaptor(1);
                            break;
                        case ConsoleKey.NumPad2:
                        case ConsoleKey.D2:
                            SetActiveNetworkAdaptor(2);
                            break;
                        case ConsoleKey.NumPad3:
                        case ConsoleKey.D3:
                            SetActiveNetworkAdaptor(3);
                            break;
                        case ConsoleKey.NumPad4:
                        case ConsoleKey.D4:
                            SetActiveNetworkAdaptor(4);
                            break;
                        case ConsoleKey.NumPad5:
                        case ConsoleKey.D5:
                            SetActiveNetworkAdaptor(5);
                            break;
                        case ConsoleKey.NumPad6:
                        case ConsoleKey.D6:
                            SetActiveNetworkAdaptor(6);
                            break;
                        case ConsoleKey.NumPad7:
                        case ConsoleKey.D7:
                            SetActiveNetworkAdaptor(7);
                            break;
                        default:
                            Console.WriteLine("Invalid Selection");
                            break;
                    }
                }
            }
        }

        private static void SetActiveNetworkAdaptor(int i)
        {
            var idx = 1;
            foreach (var x in NetworkInterface.GetAllNetworkInterfaces().ToList())//NetworkUtils.GetAllActiveLocalPhysicalNics())
            {
                if (i == idx++)
                {
                    adaptorDescription = x.Description;
                    Console.WriteLine(" Selected Network Adaptor : " + adaptorDescription);

                    choiceMade = true;
                    DisplayNics();
                    return;
                }
            }
            choiceMade = false;
            Console.WriteLine("Invalid Network Adaptor selected, try again...");
        }

        #endregion
    }
}
