using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Iata.IS.Core.Network
{
    public class NetworkPropertyList
    {
        #region "Getter properties"
        public static IList<NetworkProperty> NetworkProperties { get { return GetNetworkProperties(); } }
        #endregion

        #region "Load Remote Config File"
        private static IList<NetworkProperty> GetNetworkProperties()
        {
            string returnValue = string.Empty;
            var AppConfigPath = string.Empty;
            StreamReader readSettings = null;
            var configSettings = string.Empty;
            IList<NetworkProperty> networkInfoList =  new List<NetworkProperty>();

            var objXmlDocumt = new XmlDocument();
            AppConfigPath = ConfigurationManager.AppSettings["AppSettingPath"].ToString();

            if (!string.IsNullOrEmpty(AppConfigPath) && System.IO.File.Exists(AppConfigPath))
            {
                readSettings = System.IO.File.OpenText(AppConfigPath);
            }

            if (readSettings != null)
            {
                configSettings = Convert.ToString(readSettings.ReadToEnd());
                readSettings.Close();
            }

            if (!string.IsNullOrEmpty(configSettings))
            {
                objXmlDocumt.LoadXml(configSettings);
                var networkInfoNodeList =
                    objXmlDocumt.SelectNodes("/appConfig/networkInfoProperties/networkInfoProperty");
                if (networkInfoNodeList != null)
                {
                    foreach (XmlElement node in networkInfoNodeList)
                    {
                        if (node != null)
                        {
                            NetworkProperty networkProperty = new NetworkProperty();
                            networkProperty.NetworkName = node.SelectSingleNode("networkName") == null
                                                             ? ""
                                                             : node.SelectSingleNode("networkName").InnerText;
                            networkProperty.NetworkfolderPath = node.SelectSingleNode("networkfolderPath") == null
                                                                    ? ""
                                                                    : node.SelectSingleNode("networkfolderPath").InnerText;
                            networkProperty.UserName = node.SelectSingleNode("userName") == null
                                                           ? ""
                                                           : node.SelectSingleNode("userName").InnerText;
                            networkProperty.Password = node.SelectSingleNode("password") == null
                                                           ? ""
                                                           : node.SelectSingleNode("password").InnerText;


                            networkInfoList.Add(networkProperty);
                        }

                    }
                }
            }
            return networkInfoList;
        }
        #endregion
    }
}
