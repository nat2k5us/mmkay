namespace NetoworkLib
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    using NLog;

    public static class NetworkUtils
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly Random randStart = new Random(DateTime.Now.Ticks.GetHashCode());

        #region Public Methods and Operators

        public static string ActiveNetworkInterface()
        {
            var GoogleIp = "216.58.219.238";
            var u = new UdpClient(GoogleIp, 1);
            var localAddr = ((IPEndPoint)u.Client.LocalEndPoint).Address;

            foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                var ipProps = nic.GetIPProperties();
                foreach (var addrinfo in ipProps.UnicastAddresses)
                {
                    if (localAddr.Equals(addrinfo.Address))
                    {
                        return nic.Description;
                    }
                }
            }
            return "Adapter not found.";
        }

        public static IPAddress DecrementIpAddress(IPAddress address)
        {
            var bytes = address.GetAddressBytes();

            for (var k = bytes.Length - 1; k >= 0; k--)
            {
                if (bytes[k] == byte.MaxValue)
                {
                    bytes[k] = 0;
                    continue;
                }

                bytes[k]--;

                var result = new IPAddress(bytes);
                return result;
            }

            // cannot decrement, return the original address.
            return address;
        }

        public static string GetAdapterDescFromIp4Address(IPAddress ipAddress)
        {
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    //Console.WriteLine(ni.Description);

                    foreach (var ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            if (ipAddress.Equals(ip.Address))
                            {
                                // Console.WriteLine(ip.Address + " : " + ni.Description);
                                return ni.Description;
                            }
                        }
                    }
                }
            }
            return string.Empty;
        }

        public static IPAddress GetAdapterIp4Address(string description)
        {
            var local = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault(i => i.Description == description);
            if (local != null)
            {
                var stringAddress = local.GetIPProperties().UnicastAddresses[0].Address.ToString();
                var tempIp = IPAddress.Parse(stringAddress);
                if (tempIp.AddressFamily == AddressFamily.InterNetwork)
                {
                    return tempIp;
                }
            }
            return IPAddress.None;
        }

        public static IPAddress GetIp4MaskFromAdaptorDesc(string desc)
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var adapter in interfaces)
            {
                var ipProps = adapter.GetIPProperties();

                foreach (var ip in ipProps.UnicastAddresses)
                {
                    if ((adapter.OperationalStatus == OperationalStatus.Up)
                        && (ip.Address.AddressFamily == AddressFamily.InterNetwork) &&
                        adapter.Description.Equals(desc))
                    {
                        //Console.Out.WriteLine(ip.Address + "|" + adapter.Description);
                        return ip.IPv4Mask;
                    }
                }
            }
            return IPAddress.None;
        }

        public static IPAddress GetIpFromAdaptorDesc(string desc)
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var adapter in interfaces)
            {
                var ipProps = adapter.GetIPProperties();

                foreach (var ip in ipProps.UnicastAddresses)
                {
                    if ((adapter.OperationalStatus == OperationalStatus.Up)
                        && (ip.Address.AddressFamily == AddressFamily.InterNetwork) &&
                        adapter.Description.Equals(desc))
                    {
                        //Console.Out.WriteLine(ip.Address + "|" + adapter.Description);
                        return ip.Address;
                    }
                }
            }
            return IPAddress.None;
        }

        public static string GetAdapterNameFromIp4Address(IPAddress ipAddress)
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var adapter in interfaces)
            {
                var ipProps = adapter.GetIPProperties();

                if (
                    host.AddressList.Any(
                        ip =>
                        (adapter.OperationalStatus.ToString() == "Up") && (ip.AddressFamily == AddressFamily.InterNetwork) && adapter.SupportsMulticast && ipAddress.Equals(ip)))
                {
                    return adapter.Name;
                }
            }
            return string.Empty;
        }

        public static List<IPAddress> GetAllActiveLocalIpAddress()
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddresses = new List<IPAddress>();
            foreach (var adapter in interfaces.Where(adapter => adapter.OperationalStatus.ToString() == "Up"))
            {
                ipAddresses.AddRange(host.AddressList.Where(ipAddress => ipAddress.AddressFamily == AddressFamily.InterNetwork));
            }
            return ipAddresses.Distinct().ToList();
        }

        public static List<NetworkInterface> GetAllActiveLocalNics()
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            return interfaces.Where(adapter => adapter.OperationalStatus.ToString() == "Up").ToList();
        }

        public static List<NetworkInterface> GetAllActiveLocalPhysicalNics()
        {
            return
                NetworkInterface.GetAllNetworkInterfaces()
                    .Where(
                        networkInterface =>
                        networkInterface.OperationalStatus.ToString() == "Up" && networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet
                        && !networkInterface.Description.Contains("VirtualBox"))
                    .ToList();
        }

        public static void PrintAllNetworkInterfaceInfo()
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var networkInterface in interfaces)
            {
                Console.WriteLine(networkInterface.Name + " : " + networkInterface.Description + " : " + networkInterface.NetworkInterfaceType);
            }
        }

        public static void RestorePrevNetworkInfo(AdaptorNetworkInfo prevAdaptorNetworkInfo)
        {
            if (prevAdaptorNetworkInfo == null)
            {
                return;
            }

            Logger.Info("Restoring Previos Network Information" + prevAdaptorNetworkInfo);
            try
            {
                if (prevAdaptorNetworkInfo.IsDhcpEnabled)
                {
                    NetworkManagement.SetDhcp(prevAdaptorNetworkInfo.Name);
                }
                else
                {
                    NetworkManagement.SetSystemIp(prevAdaptorNetworkInfo.IpAddress.ToString(), "255.255.255.0", prevAdaptorNetworkInfo.Description);
                }
            }
            catch (Exception e)
            {
                Logger.Info(e.Message + (e.InnerException?.Message ?? ""));
            }
        }

        public static AdaptorNetworkInfo GetAdaptorNetworkInfo(string nameOrDesc)
        {
            Logger.Info("GetAdaptorNetworkInfo for " + nameOrDesc);
            var physicalNics = GetAllActiveLocalPhysicalNics();
            foreach (var networkInterface in physicalNics)
            {
                if (networkInterface.Description.Equals(nameOrDesc) || networkInterface.Name.Equals(nameOrDesc))
                {
                    var adaptorDescriptionOrName = networkInterface.Description;
                    var adaptorNetworkInfo = new AdaptorNetworkInfo
                                                 {
                                                     Description = adaptorDescriptionOrName,
                                                     IpAddress = GetAdapterIp4Address(adaptorDescriptionOrName),
                                                     Gateway = NetworkManagement.GetGatewayForAdaptor(adaptorDescriptionOrName),
                                                     IsDhcpEnabled = !NetworkManagement.IsNetworkedWithStaticIp(adaptorDescriptionOrName),
                                                     Name = networkInterface.Name
                                                 };
                    Logger.Info("GetAdaptorNetworkInfo Found Match " + adaptorNetworkInfo);
                    return adaptorNetworkInfo;
                }
            }

            return new AdaptorNetworkInfo();
        }

        public static List<IPAddress> GetAllLocalIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            return host.AddressList.Where(ipAddress => ipAddress.AddressFamily == AddressFamily.InterNetwork).ToList();
        }

        public static int GetBestNetworkAdaptorForMulticast()
        {
            var nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var adapter in nics)
            {
                if (!adapter.GetIPProperties().MulticastAddresses.Any())
                {
                    continue; // most of VPN adapters will be skipped
                }
                if (!adapter.SupportsMulticast)
                {
                    continue; // multicast is meaningless for this type of connection
                }
                if (OperationalStatus.Up != adapter.OperationalStatus)
                {
                    continue; // this adapter is off or not connected
                }
                var p = adapter.GetIPProperties().GetIPv4Properties();
                if (null == p)
                {
                    continue; // IPv4 is not configured on this adapter
                }

                // This is a suitable adaptor for multicast
                //my_sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface, (int)IPAddress.HostToNetworkOrder(p.Index));
                return IPAddress.HostToNetworkOrder(p.Index);
            }
            return -1; // no network adaptors valid for MUltiCast
        }

        public static IPAddress GetBestNetworkAdaptorIpForMulticast()
        {
            var nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var adapter in nics)
            {
                if (!adapter.GetIPProperties().MulticastAddresses.Any())
                {
                    continue; // most of VPN adapters will be skipped
                }
                if (!adapter.SupportsMulticast)
                {
                    continue; // multicast is meaningless for this type of connection
                }
                if (OperationalStatus.Up != adapter.OperationalStatus)
                {
                    continue; // this adapter is off or not connected
                }
                var p = adapter.GetIPProperties().GetIPv4Properties();
                if (null == p)
                {
                    continue; // IPv4 is not configured on this adapter
                }
                foreach (var ip in adapter.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.Address;
                    }
                }
            }
            return IPAddress.None; // no network adaptors valid for MUltiCast
        }

        public static IPAddress GetIpAddress(this EndPoint endPoint)
        {
            return endPoint != null ? ((IPEndPoint)endPoint).Address : IPAddress.None;
        }

        public static IPAddress GetLocalIpAddress()
        {
            var localIpAddr = IPAddress.None; // IPAddress.Parse(Console.ReadLine());

            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ipAddress in host.AddressList)
            {
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIpAddr = ipAddress;
                    break;
                }
            }
            return localIpAddr;
        }

        public static List<IPAddress> GetPhysicalIpAdress()
        {
            var allIps = new List<IPAddress>();
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                var addr = ni.GetIPProperties().GatewayAddresses.FirstOrDefault();
                if (addr != null && !addr.Address.ToString().Equals("0.0.0.0"))
                {
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        foreach (var ip in ni.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                allIps.Add(ip.Address);
                            }
                        }
                    }
                }
            }
            return allIps;
        }

        public static IPAddress IncrementIpAddress(IPAddress address)
        {
            var bytes = address.GetAddressBytes();

            for (var k = bytes.Length - 1; k >= 0; k--)
            {
                if (bytes[k] == byte.MaxValue)
                {
                    bytes[k] = 0;
                    continue;
                }

                bytes[k]++;

                var result = new IPAddress(bytes);
                return result;
            }

            // Un-incrementable, return the original address.
            return address;
        }

        public static bool IsNetworkAvailable()
        {
            // only recognizes changes related to Internet adapters
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                // however, this will include all adapters
                var interfaces = NetworkInterface.GetAllNetworkInterfaces();

                foreach (var face in interfaces)
                {
                    // filter so we see only Internet adapters
                    if (face.OperationalStatus == OperationalStatus.Up)
                    {
                        if ((face.NetworkInterfaceType != NetworkInterfaceType.Tunnel) && (face.NetworkInterfaceType != NetworkInterfaceType.Loopback))
                        {
                            var statistics = face.GetIPv4Statistics();

                            // all testing seems to prove that once an interface
                            // comes online it has already accrued statistics for
                            // both received and sent...

                            if ((statistics.BytesReceived > 0) && (statistics.BytesSent > 0))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public static bool IsReachableUri(string uriInput)
        {
            // Variable to Return
            bool testStatus;
            // Create a request for the URL.
            var request = WebRequest.Create(uriInput);
            request.Timeout = 15000; // 15 Sec

            WebResponse response;
            try
            {
                response = request.GetResponse();
                testStatus = true; // Uri does exist                 
                response.Close();
            }
            catch (Exception exception)
            {
                testStatus = false; // Uri does not exist
                Console.WriteLine(exception.Message);
            }
            // Result
            return testStatus;
        }

        public static void ResetGateway(string adaptorName)
        {
            //netsh interface ipv4 set address name="Local Area Connection" gateway=none
            var process = new Process();
            var startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = " netsh interface ipv4 set address name=" + adaptorName + " source=dhcp gateway=none";
            Console.WriteLine("NetshReset => " + startInfo.FileName + startInfo.Arguments);
            process.StartInfo = startInfo;
            process.Start();
        }

        public static void NetshReset()
        {
            var process = new Process();
            var startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = " /C netsh int ip reset c:\networkdata.log";
            Console.WriteLine("NetshReset => " + startInfo.FileName + startInfo.Arguments);
            process.StartInfo = startInfo;
            process.Start();
        }

        public static void NetworkCommand(string parameter)
        {
            var process = new Process();
            var startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = " /C ipconfig " + " /" + parameter;
            Console.WriteLine("NetworkCommand => " + startInfo.FileName + startInfo.Arguments);
            process.StartInfo = startInfo;
            process.Start();
        }

        public static void NetworkReset()
        {
            NetworkCommand("release");
            NetworkCommand("renew");
            NetworkCommand("flushdns");
            NetshReset();
        }

        public static void ShowNetworkInterfaces(bool onlyPhysicalAdaptors = true, bool verbose = false)
        {
            var idx = 1;
            var computerProperties = IPGlobalProperties.GetIPGlobalProperties();
            var nics = onlyPhysicalAdaptors ? GetAllActiveLocalPhysicalNics() : NetworkInterface.GetAllNetworkInterfaces().ToList();
            Console.WriteLine("Interface information for {0}.{1}     ", computerProperties.HostName, computerProperties.DomainName);
            if (nics == null || nics.Count < 1)
            {
                Console.WriteLine("  No network interfaces found.");
                return;
            }

            Console.WriteLine("  Number of interfaces .................... : {0}", nics.Count);
            foreach (var adapter in nics)
            {
                var properties = adapter.GetIPProperties();
                Console.WriteLine();
                Console.WriteLine(idx++ + ". " + adapter.Description);
                Console.WriteLine(string.Empty.PadLeft(adapter.Description.Length, '='));
                Console.WriteLine("  Interface type .......................... : {0}", adapter.NetworkInterfaceType);
                Console.WriteLine("  Physical Address ........................ : {0}", adapter.GetPhysicalAddress());
                Console.WriteLine("  Operational status ...................... : {0}", adapter.OperationalStatus);
                Console.WriteLine("  IP4 Address ............................. : {0}", GetIpFromAdaptorDesc(adapter.Description));
                Console.WriteLine("  IP4 Subnet Mask ..........................: {0}", GetIp4MaskFromAdaptorDesc(adapter.Description));
                Console.WriteLine("  Default Gateway ..........................: {0}", NetworkManagement.GetGatewayForAdaptor(adapter.Description));
                var versions = "";

                // Create a display string for the supported IP versions.
                if (adapter.Supports(NetworkInterfaceComponent.IPv4))
                {
                    versions = "IPv4";
                }
                if (adapter.Supports(NetworkInterfaceComponent.IPv6))
                {
                    if (versions.Length > 0)
                    {
                        versions += " ";
                    }
                    versions += "IPv6";
                }
                Console.WriteLine("  IP version .............................. : {0}", versions);
                if (verbose) ShowIPAddresses(properties);

                // The following information is not useful for loopback adapters.
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                {
                    continue;
                }
                Console.WriteLine("  DNS suffix .............................. : {0}", properties.DnsSuffix);

                string label;
                if (adapter.Supports(NetworkInterfaceComponent.IPv4))
                {
                    var ipv4 = properties.GetIPv4Properties();
                    Console.WriteLine("  MTU...................................... : {0}", ipv4.Mtu);
                    if (ipv4.UsesWins)
                    {
                        var winsServers = properties.WinsServersAddresses;
                        if (winsServers.Count > 0)
                        {
                            label = "  WINS Servers ............................ :";
                            ShowIPAddresses(label, winsServers);
                        }
                    }
                }

                Console.WriteLine("  DNS enabled ............................. : {0}", properties.IsDnsEnabled);
                Console.WriteLine("  Dynamically configured DNS .............. : {0}", properties.IsDynamicDnsEnabled);
                Console.WriteLine("  Receive Only ............................ : {0}", adapter.IsReceiveOnly);
                Console.WriteLine("  Multicast ............................... : {0}", adapter.SupportsMulticast);
                // ShowInterfaceStatistics(adapter);

                Console.WriteLine();
            }
        }

        private static void ShowIPAddresses(IPInterfaceProperties properties)
        {
            Console.WriteLine("Dhcp Server：");
            ShowIPAddressCollection(properties.DhcpServerAddresses);
            Console.WriteLine("DNS Servers：");
            ShowIPAddressCollection(properties.DnsAddresses);
            Console.WriteLine("Gateways：");
            ShowGatewayIPAddressInformationCollection(properties.GatewayAddresses);
            Console.WriteLine(" (DNS) Enabled ：" + properties.IsDnsEnabled);
            Console.WriteLine(" Dynamic DNS Enabled ：" + properties.IsDynamicDnsEnabled);
            Console.WriteLine("Multicast Addresses：");
            ShowMulticastIPAddressInformationCollection(properties.MulticastAddresses);
            Console.WriteLine("Windows Internet  (WINS) ：");
            ShowIPAddressCollection(properties.WinsServersAddresses);
        }

        private static void ShowIPAddresses(string label, IPAddressCollection winsServers)
        {
            Console.WriteLine("-----" + label + "-----");
            ShowIPAddressCollection(winsServers);
        }

        private static void ShowIPAddressInformationCollection(IPAddressInformationCollection collection)
        {
            for (var i = 0; i < collection.Count; i++)
            {
                Console.WriteLine(GetIPAddressInfo(collection[i].Address));
            }
        }

        private static void ShowIPAddressCollection(IPAddressCollection collection)
        {
            for (var i = 0; i < collection.Count; i++)
            {
                Console.WriteLine("                                   :" + GetIPAddressInfo(collection[i]));
            }
        }

        private static void ShowMulticastIPAddressInformationCollection(MulticastIPAddressInformationCollection collection)
        {
            for (var i = 0; i < collection.Count; i++)
            {
                Console.WriteLine("                                   :" + GetIPAddressInfo(collection[i].Address));
            }
        }

        private static string GetIPAddressInfo(IPAddress address)
        {
            var bytes = address.GetAddressBytes();
            string ipString = null;
            for (var i = 0; i < bytes.Length - 1; i++)
            {
                ipString += bytes[i] + ".";
            }
            return ipString + bytes[bytes.Length - 1];
        }

        private static void ShowGatewayIPAddressInformationCollection(GatewayIPAddressInformationCollection collection)
        {
            for (var i = 0; i < collection.Count; i++)
            {
                Console.WriteLine("                                   :" + GetIPAddressInfo(collection[i].Address));
            }
        }

        private static readonly Random _random = new Random();

        public static string GetRandomIp()
        {
            return string.Format("{0}.{1}.{2}.{3}", _random.Next(0, 255), _random.Next(0, 255), _random.Next(0, 255), _random.Next(0, 255));
        }

        //public static async void PingLoop(List<string> hostList )
        //{
        //    var longRunningTask = PingAsync(hostList);

        //    int result = await longRunningTask;
           

        //    //eventually want to loop here but for now just want to understand how this works
        //}
        //public static async Task<List<PingReply>> PingAsync(List<string> hostList)
        //{

        //    var tasks = hostList.Select<List<PingReply>>(ip => new Ping().SendAsync(ip, 2000));
        //    var results = await Task.WhenAll(tasks);

        //    return results.ToList();
        //}
        public static bool PingHost(string nameOrAddress, out long time)
        {
            var pingable = false;
          
            var pinger = new Ping();
            try
            {
                var reply = pinger.Send(nameOrAddress);
                if (reply != null)
                {
                    pingable = reply.Status == IPStatus.Success;
                    time = reply.RoundtripTime;
                }
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
                Logger.Info($"Ping at {nameOrAddress} result was not response - returning false.");
              
            }
            time = 0;
            return pingable;
        }

        public static List<string> GetArp()
        {
            var result = new List<string>();
            try
            {
                var arpProcess = new Process
                                     {
                                         StartInfo =
                                             {
                                                 FileName = "arp.exe",
                                                 CreateNoWindow = true,
                                                 Arguments = "-a",
                                                 RedirectStandardOutput = true,
                                                 UseShellExecute = false,
                                                 RedirectStandardError = true
                                             }
                                     };
                arpProcess.Start();
                var streamReader = new StreamReader(arpProcess.StandardOutput.BaseStream, arpProcess.StandardOutput.CurrentEncoding);
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    if (line.StartsWith("  "))
                    {
                        var items = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (items.Length == 3)
                        {
                            try
                            {
                                var ip = IPAddress.Parse(items[0]); // Parse to make sure we have a valid Ip - if not move on to next ip
                                result.Add(items[0]);
                            }
                            catch (Exception e)
                            {
                                Logger.Info(e.Message + e.InnerException?.Message);
                            }
                        }
                    }
                }

                streamReader.Close();
            }
            catch (Exception e)
            {
                Logger.Info(e.Message + e.InnerException?.Message);
            }

            return result;
        }

        public static IPAddress GetNextFreeIpAddress(IPAddress ipAddress)
        {
            var result = GetArp();
            result.ForEach(x => Logger.Info($" Arp Table: - {x}"));
            var freeIpAddress = IncrementIpAddress(ipAddress, randStart.Next(2, 10));
            if (!result.Contains(freeIpAddress.ToString()))
            {
                long time = 0;
                if (!PingHost(freeIpAddress.ToString(), out time))
                {
                    Logger.Info($" {freeIpAddress} : Ip address ping response was false. So using it.");
                }
            }
            var increment = true;
            for (var i = 0; i < 255; i++)
            {
                if (GetIpAddressLastOctet(freeIpAddress) >= 254) increment = false;
                if (GetIpAddressLastOctet(freeIpAddress) <= 1) increment = true;
                Logger.Info($" Increment : {increment}");
                freeIpAddress = increment ? IncrementIpAddress(freeIpAddress) : DecrementIpAddress(freeIpAddress);
                Logger.Info($" Free IP Address : {freeIpAddress}");
                if (!result.Contains(freeIpAddress.ToString()))
                {
                    Logger.Warn($"Got a free IP adresss {freeIpAddress} pinging it... after ");
                    long time = 0;
                    if (!PingHost(freeIpAddress.ToString(), out time))
                    {
                        Logger.Info($" {freeIpAddress} : Ip address ping response was false. So using it.");
                        return freeIpAddress;
                    }
                }
            }
            return IPAddress.Any;
        }

        public static uint GetIpAddressLastOctet(this IPAddress ipAddress)
        {
            return ipAddress.GetAddressBytes()[3];
        }

        public static uint GetIpAddressLastOctet(this string ipAddress)
        {
            return IPAddress.Parse(ipAddress).GetAddressBytes()[3];
        }

        public static string GetNextIpAddress(string ipAddress, int increment)
        {
            var addressBytes = IPAddress.Parse(ipAddress).GetAddressBytes().Reverse().ToArray();
            var ipAsUint = BitConverter.ToUInt32(addressBytes, 0);
            var nextAddress = BitConverter.GetBytes(ipAsUint + increment);
            return string.Join(".", nextAddress.Reverse());
        }

        public static IPAddress IncrementIpAddress(IPAddress ipAddress, int increment)
        {
            var ip = ipAddress;
            for (var i = 0; i < increment; i++)
            {
                ip = IncrementIpAddress(ip);
            }
            return ip;
        }

        public static IPAddress SetLastOctetTo(this IPAddress ip, int lastOctet)
        {
            var bytes = IPAddress.Parse(ip.ToString()).GetAddressBytes();
            bytes[3] =  (byte)lastOctet;
            IPAddress ipAddress = new IPAddress(bytes);
            return ipAddress;
        }

        #endregion
    }
}