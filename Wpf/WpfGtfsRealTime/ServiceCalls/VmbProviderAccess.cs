namespace WpfGtfsRealTime.ServiceCalls
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Configuration;

   

    using NLog;

    public static class VmbProviderAccess
    {
        #region Static Fields

        private static readonly Logger NLogger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Public Methods and Operators

        public static ICollection<string> GetActiveRoutes(ICollection<string> destinations, string jurisdictionName)
        {
            var endPointName = "wsHttp";
            try
            {
                var httpBinding = new BasicHttpBinding();
                EndpointAddress endpointAddress = null;
                var serviceModelClientConfigSection = ConfigurationManager.GetSection("system.serviceModel/client") as ClientSection;
                if (serviceModelClientConfigSection != null)
                    foreach (ChannelEndpointElement endpoint in serviceModelClientConfigSection.Endpoints)
                        if (endpoint.Name == endPointName)
                        {
                            endpointAddress = new EndpointAddress(endpoint.Address.ToString().Replace(endPointName, ""));
                            break;
                        }

                var myChannelFactory = new ChannelFactory<IVmbProviderService>(httpBinding, endpointAddress);
                var vmbProviderService = myChannelFactory.CreateChannel();
                var activeRoutes = new List<string>();
                foreach (var destination in destinations)
                {
                    var dest = destination.Trim();
                    var routeItem = vmbProviderService.GetRouteByGtfsRouteId(dest, jurisdictionName);
                    if (routeItem != null && routeItem.Enabled) activeRoutes.Add(dest);
                }
                return activeRoutes.ToList();
            }
            catch (Exception ex)
            {
                NLogger.Error(ex.Message);
                //throw;
            }
            return new List<string>();
        }

        public static bool IsRouteValid(string route, string jurisdictionName)
        {
            try
            {
                var httpBinding = new BasicHttpBinding();
                var endpointAddress = new EndpointAddress("http://localhost:9089/VmbProvider");
                var myChannelFactory = new ChannelFactory<IVmbProviderService>(httpBinding, endpointAddress);
                var vmbProviderService = myChannelFactory.CreateChannel();
                var dest = route.Trim();
                var routeItem = vmbProviderService.GetRouteByGtfsRouteId(dest, jurisdictionName);
                ((ICommunicationObject)vmbProviderService).Close();
                if (routeItem != null && routeItem.Enabled) return true;
            }
            catch (Exception ex)
            {
                NLogger.Error(ex.Message);
            }
            return false;
        }

        public static bool IsVmbAvailable()
        {
            try
            {
                var httpBinding = new BasicHttpBinding();
                var endpointAddress = new EndpointAddress("http://localhost:9089/VmbProvider");
                var myChannelFactory = new ChannelFactory<IVmbProviderService>(httpBinding, endpointAddress);
                var vmbProviderService = myChannelFactory.CreateChannel();
                ((ICommunicationObject)vmbProviderService).Close();
                return true;

            }
            catch (Exception ex)
            {
                NLogger.Error(ex.Message);
                return false;
            }
           
        }

        #endregion
    }
}