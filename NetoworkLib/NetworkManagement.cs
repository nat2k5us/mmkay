namespace NetoworkLib
{
    using System;
    using System.Linq;
    using System.Management;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Reflection;

    public class NetworkManagement
    {
        #region Public Methods and Operators

        public static bool IsNetworkedStatusDown()
        {
            var niAdpaters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var networkInterface in niAdpaters)
            {
                return networkInterface.OperationalStatus == OperationalStatus.Down;
            }

            return false;
        }


        public static bool IsNetworkedWithStaticIp(string nameOrDescription)
        {
            var niAdpaters = NetworkInterface.GetAllNetworkInterfaces();
            try
            {
                foreach (var networkInterface in niAdpaters)
                {
                    var pv4InterfaceProperties = networkInterface.GetIPProperties().GetIPv4Properties();
                    var name = networkInterface.Name.Equals(nameOrDescription);
                    var desc = networkInterface.Description.Equals(nameOrDescription);
                    if (pv4InterfaceProperties != null && (name || desc))
                    {
                        return !pv4InterfaceProperties.IsDhcpEnabled;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(MethodBase.GetCurrentMethod().Name + e.Message + e?.InnerException?.Message);
                return false;
            }
            return false;
        }

        public static void PrintNetworkStatistics()
        {
            var adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var n in adapters)
            {
                Console.WriteLine(
                    "Name = {0}, Desc =  {1}, Speed = {2}, Supports MultiCast = {3}, Status = {4}, Id = {5}",
                    n.Name,
                    n.Description,
                    n.Speed,
                    n.SupportsMulticast,
                    n.OperationalStatus,
                    n.Id);
            }
        }

        public static void SetDhcp(string nicName)
        {
            try
            {
                var mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                var moc = mc.GetInstances();

                foreach (var o in moc)
                {
                    var managementObject = (ManagementObject)o;
                    // Make sure this is a IP enabled device. Not something like memory card or VM Ware
                    if ((bool)managementObject["IPEnabled"])
                    {
                        //// workaround of windows bug (windows refused to apply static ip in network properties dialog)
                        //var settingId = o.GetPropertyValue("SettingID"); // adapter = the management object
                        //using (var regKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\services\Tcpip\Parameters\Interfaces\" + settingId, true))
                        //{
                        //    if (regKey != null)
                        //    {
                        //        regKey.SetValue("EnableDHCP", 1);
                        //        regKey.Close();
                        //    }
                        //}
                        if (managementObject["Description"].Equals(nicName))
                        {
                            Console.WriteLine("Nic Name matched => Setting Network Interface to DHCP");
                            var newDns = managementObject.GetMethodParameters("SetDNSServerSearchOrder");
                            newDns["DNSServerSearchOrder"] = null;
                            managementObject.InvokeMethod("EnableDHCP", null, null);
                            managementObject.InvokeMethod("SetDNSServerSearchOrder", newDns, null);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            finally
            {
                Console.WriteLine("End of SetDhcp => " + nicName);
            }
        }

        /// <summary>Set's a new Gateway address of the local machine</summary>
        /// <param name="gateway">The Gateway IP Address</param>
        /// <param name="nicNameOrDesc"></param>
        /// <remarks>Requires a reference to the System.Management namespace</remarks>
        /// <returns>The <see cref="uint"/>.</returns>
        public static uint SetSystemGateway(string gateway, string nicNameOrDesc)
        {
            var managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
            var managementObjectCollection = managementClass.GetInstances();

            foreach (var o in managementObjectCollection)
            {
                var managementObject = (ManagementObject)o;
                if (managementObject["Description"].Equals(nicNameOrDesc))
                {
                    if ((bool)managementObject["IPEnabled"])
                    {
                        try
                        {
                            var newGateway = managementObject.GetMethodParameters("SetGateways");

                            newGateway["DefaultIPGateway"] = new[] { gateway };
                            newGateway["GatewayCostMetric"] = new[] { 1 };

                            var managementBaseObject = managementObject.InvokeMethod("SetGateways", newGateway, null);

                            if (managementBaseObject != null)
                            {
                                var u = (uint)managementBaseObject["returnValue"];
                                Console.WriteLine("SetGateway Result - " + u);
                                return u;
                            }
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message);
                            throw;
                        }
                    }
                }
            }
            return 0;
        }

        /// <summary>
        ///     Set's a new Gateway address of the local machine
        /// </summary>
        /// <param name="gateway">The Gateway IP Address</param>
        /// <remarks>Requires a reference to the System.Management namespace</remarks>
        public static void SetSystemGateway(string gateway)
        {
            var managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
            var managementObjectCollection = managementClass.GetInstances();

            foreach (var o in managementObjectCollection)
            {
                var managementObject = (ManagementObject)o;
                if ((bool)managementObject["IPEnabled"])
                {
                    try
                    {
                        var newGateway = managementObject.GetMethodParameters("SetGateways");

                        newGateway["DefaultIPGateway"] = new[] { gateway };
                        newGateway["GatewayCostMetric"] = new[] { 1 };

                        var managementBaseObject = managementObject.InvokeMethod("SetGateways", newGateway, null);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.Message);
                        throw;
                    }
                }
            }
        }

        public static IPAddress GetGatewayForAdaptor(string adaptorDescriptionOrName)
        {
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            try
            {
                foreach (var networkInterface in networkInterfaces)
                {
                    var nameMatched = networkInterface.Name.Equals(adaptorDescriptionOrName);
                    var descMatched = networkInterface.Description.Equals(adaptorDescriptionOrName);
                    if (nameMatched || descMatched)
                    {
                        var gatewayIpAddressInformation = networkInterface.GetIPProperties().GatewayAddresses.FirstOrDefault();
                        if (gatewayIpAddressInformation != null)
                        {
                            return gatewayIpAddressInformation.Address;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + (e.InnerException?.Message ?? string.Empty));
            }
            return IPAddress.None;
        }

        public static void SetSystemIp(string ipAddress, string subnetMask, string adapterDescription)
        {
            // var NetworkConnectionNames = NetworkInterface.GetAllNetworkInterfaces().Select(ni => ni.Name);
            // IEnumerable<NetworkInterface> NetworkConnections = NetworkInterface.GetAllNetworkInterfaces();
            var managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
            var managementObjectCollection = managementClass.GetInstances();

            foreach (var o in managementObjectCollection)
            {
                var managementObject = (ManagementObject)o;

                #region Windows Potential Issue - Might have been resolved

                //try
                //{
                //    // workaround of windows bug (windows refused to apply static ip in network properties dialog)
                //    var settingId = o.GetPropertyValue("SettingID"); // adapter = the management object
                //    using (var regKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\services\Tcpip\Parameters\Interfaces\" + settingId, true))
                //    {
                //        if (regKey != null)
                //        {
                //            regKey.SetValue("EnableDHCP", 0);
                //            regKey.Close();
                //        }
                //    }
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine(e.Message);
                //}

                #endregion

                if ((bool)managementObject["IPEnabled"])
                {
                    if (!string.IsNullOrEmpty(adapterDescription))
                    {
                        // Console.WriteLine(managementObject["Description"] + " =X? " + adapterDescription);
                        if (managementObject["Description"].Equals(adapterDescription))
                        {
                            try
                            {
                                var newIp = managementObject.GetMethodParameters("EnableStatic");

                                newIp["IPAddress"] = new[] { ipAddress };
                                newIp["SubnetMask"] = new[] { subnetMask };

                                var setIp = managementObject.InvokeMethod("EnableStatic", newIp, null);
                                if (setIp != null)
                                {
                                    Console.WriteLine("SetSystemIp Result - " + (uint)setIp["returnValue"]);
                                }

                            }
                            catch (Exception exception)
                            {
                                Console.WriteLine(exception.Message);
                                throw;
                            }
                            finally
                            {
                                Console.WriteLine("SetSystemIp called with => " + ipAddress + " : " + subnetMask);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("ERROR: Adaptor description was empty");
                    }
                }
            }
        }

        /// <summary>
        ///     Set's the DNS Server of the local machine
        /// </summary>
        /// <param name="nic">NIC address</param>
        /// <param name="dns">DNS server address</param>
        /// <remarks>Requires a reference to the System.Management namespace</remarks>
        public void SetSystemDns(string nic, string dns)
        {
            var managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
            var managementObjectCollection = managementClass.GetInstances();

            foreach (var o in managementObjectCollection)
            {
                var managementObject = (ManagementObject)o;
                if ((bool)managementObject["IPEnabled"])
                {
                    // if you are using the System.Net.NetworkInformation.NetworkInterface you'll need to change this line to if (objMO["Caption"].ToString().Contains(NIC)) and pass in the Description property instead of the name 
                    if (managementObject["Caption"].Equals(nic))
                    {
                        try
                        {
                            var newDns = managementObject.GetMethodParameters("SetDNSServerSearchOrder");
                            newDns["DNSServerSearchOrder"] = dns.Split(',');
                            var setDns = managementObject.InvokeMethod("SetDNSServerSearchOrder", newDns, null);
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message);
                            throw;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Set's WINS of the local machine
        /// </summary>
        /// <param name="nic">NIC Address</param>
        /// <param name="priWins">Primary WINS server address</param>
        /// <param name="secWins">Secondary WINS server address</param>
        /// <remarks>Requires a reference to the System.Management namespace</remarks>
        public void SetSystemWins(string nic, string priWins, string secWins)
        {
            var managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
            var managementObjectCollection = managementClass.GetInstances();

            foreach (var o in managementObjectCollection)
            {
                var managementObject = (ManagementObject)o;
                if ((bool)managementObject["IPEnabled"])
                {
                    if (managementObject["Caption"].Equals(nic))
                    {
                        try
                        {
                            var wins = managementObject.GetMethodParameters("SetWINSServer");
                            wins.SetPropertyValue("WINSPrimaryServer", priWins);
                            wins.SetPropertyValue("WINSSecondaryServer", secWins);

                            var managementBaseObject = managementObject.InvokeMethod("SetWINSServer", wins, null);
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message);
                            throw;
                        }
                    }
                }
            }
        }

        #endregion
    }
}
