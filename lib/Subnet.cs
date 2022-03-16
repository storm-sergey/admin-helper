using System;
using System.Net;
using System.Net.NetworkInformation;
using static AdminHelper.Globals;

namespace AdminHelper.lib
{
    /// The class has only IPv4 support
    public static class Subnet
    {
        /// <summary>
        /// To check DNS availability
        /// </summary>
        /// <returns> Local host name </returns>
        public static string GetLocalhostName()
        {
            return Dns.GetHostName();
        }

        /// <summary>
        /// Using Dns.GetHostName() or Subnet.GetLocalhostName() as a net checking way donesn't work...
        /// Indicates whether any network connection is available.
        /// </summary>
        /// <returns> Newtwork availability flag </returns>
        public static bool IsNetworkAvailable()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                return false;

            foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if ((netInterface.OperationalStatus != OperationalStatus.Up)
                ||  (netInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                ||  (netInterface.NetworkInterfaceType == NetworkInterfaceType.Tunnel)
                ||  (netInterface.Description.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0)
                ||  (netInterface.Name.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0)
                ||  (netInterface.Description.Equals("Microsoft Loopback Adapter", StringComparison.OrdinalIgnoreCase)))
                    continue;

                return true;
            }
            return false;
        }

        /// <summary>
        /// A host can have several IP addresses because there can be several network interfaces.
        /// This method returns only IPv4 address by connection-specific DNS suffix "int.rolfcorp.ru"
        /// </summary>
        /// <returns> Local host IP </returns>
        public static IPAddress GetLocalhostIP()
        {
            NetworkInterface[] netInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface netInterface in netInterfaces)
            {
                IPInterfaceProperties ipProperties = netInterface.GetIPProperties();
                if (ipProperties.DnsSuffix.ToLower().Equals(ROLF_DNS_SUFFIX))
                {
                    foreach (UnicastIPAddressInformation ip in ipProperties.UnicastAddresses)
                    {
                        // IPv4 checking
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            return ip.Address;
                        }
                    }
                }
            }
            throw new Exception("Getting local host IP is failed");
        }

        /// <summary>
        /// Method overloading
        /// This parses subnet and passing to the overloaded method 
        /// </summary>
        /// <param name="localIP"> Only IPAddress</param>
        /// <param name="subnet"> Geting subnet like string "SUBNET_IP/CIDR" </param>
        /// <returns> A check flag </returns>
        public static bool IsRankedBySubnet(IPAddress localIP, string subnet)
        {
            try
            {
                string[] subnetCidr = subnet.Split('/');
                return IsRankedBySubnet(
                    localIP,
                    IPAddress.Parse(subnetCidr[0]),
                    Int32.Parse(subnetCidr[1]));
            }
            catch
            {
                throw new Exception("Subnet parsing is faled");
            }
        }

        public static bool IsRankedBySubnet(IPAddress localIP, IPAddress subnet, int cidr)
        {
            UInt32 firstIP = IPToUint32(subnet) + 1;
            UInt32 usableIPs = (UInt32.MaxValue >> cidr) - 1;
            UInt32 lastIP = firstIP + usableIPs - 1;
            UInt32 comparedIP = IPToUint32(localIP);
            return (comparedIP >= firstIP)
                && (comparedIP <= lastIP);
        }

        public static UInt32 IPToUint32(IPAddress ip)
        {
            byte[] bytes = ip.GetAddressBytes();
            Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }
    }
}