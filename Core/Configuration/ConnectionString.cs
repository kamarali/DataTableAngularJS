using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Configuration;

namespace Iata.IS.Core.Configuration
{
    public class ConnectionString
    {

        #region  "Class properties"

        private static ConnectionString _sInstance;

        /// <summary>
        /// constructor
        /// </summary>
        private ConnectionString()
        {
        }

        /// <summary>
        /// Create an class instance
        /// </summary>
        public static ConnectionString Instance
        {
            get { return _sInstance ?? (_sInstance = new ConnectionString()); }
        }

        #endregion

        #region "Getter properties"

        public string ISDataContextContainer
        {
            get { return GetconfigConnection("ISDataContextContainer"); }
        }

        public string UserManagementContainer
        {
            get { return GetconfigConnection("UserManagementContainer"); }
        }

        public string DirectConnectionString
        {
            get { return GetconfigConnection("DirectConnectionString"); }
        }

        public string ServiceConnectionString
        {
            get { return GetconfigConnection("ServiceConnectionString"); }
        }

        #endregion

        #region "Load Remote Config File"

        //This method will return san folder path
        public static string GetAppSetting(string elementId)
        {
            return ConfigurationManager.AppSettings["AppSettingPath"].ToString();
        }

        //This will concate san file path and sisconfig.xml file path
        private static string GetSisConfigXmlFilePath()
        {
            const string filePath = "App_Data\\SisConfig.xml";

            string _appSettingPath = ConfigurationManager.AppSettings["AppSettingPath"].ToString();

            if (_appSettingPath.LastIndexOf("\\") != _appSettingPath.Length - 1)
            {
                _appSettingPath = _appSettingPath + "\\";
            }

            return Path.Combine(_appSettingPath, filePath);
        }

        //This method will read connectionStrings section 
        public static string GetconfigConnection(string elementId)
        {
            string returnValue = string.Empty;
            var appSettingPath = string.Empty;
            StreamReader readSettings = null;
            var configSettings = string.Empty;
            var objXmlDocumt = new XmlDocument();
            appSettingPath = GetSisConfigXmlFilePath();

            if (!string.IsNullOrEmpty(appSettingPath) && System.IO.File.Exists(appSettingPath))
            {
                readSettings = System.IO.File.OpenText(appSettingPath);
            }

            if (readSettings != null)
            {
                configSettings = Convert.ToString(readSettings.ReadToEnd());
                readSettings.Close();
                //configSettings = Crypto.DecryptString(configSettings);
            }

            if (!string.IsNullOrEmpty(configSettings))
            {
                objXmlDocumt.LoadXml(configSettings);
                var element = objXmlDocumt.GetElementsByTagName("connectionStrings");

                // ReSharper disable PossibleNullReferenceException
                foreach (XmlElement node in element.Item(0))
                // ReSharper restore PossibleNullReferenceException
                {
                    var nameAttribute = GetAttributeValue(node, "name");
                    if (string.Equals(nameAttribute, elementId))
                    {
                        returnValue = GetAttributeValue(node, "connectionString");
                        break;
                    }

                }
            }

            return Crypto.DecryptString(returnValue);

        }

        //This method will read appSetting section
        public static string GetconfigAppSetting(string elementId)
        {
            string returnValue = string.Empty;
            var appSettingPath = string.Empty;
            StreamReader readSettings = null;
            var configSettings = string.Empty;
            var objXmlDocumt = new XmlDocument();
            appSettingPath = GetSisConfigXmlFilePath();

            if (!string.IsNullOrEmpty(appSettingPath) && System.IO.File.Exists(appSettingPath))
            {
                readSettings = System.IO.File.OpenText(appSettingPath);
            }

            if (readSettings != null)
            {
                configSettings = Convert.ToString(readSettings.ReadToEnd());
                readSettings.Close();
                //configSettings = Crypto.DecryptString(configSettings);
            }

            if (!string.IsNullOrEmpty(configSettings))
            {
                objXmlDocumt.LoadXml(configSettings);
                var element = objXmlDocumt.GetElementsByTagName("appSettings");

                // ReSharper disable PossibleNullReferenceException
                foreach (XmlElement node in element.Item(0))
                // ReSharper restore PossibleNullReferenceException
                {
                    var nameAttribute = GetAttributeValue(node, "key");
                    if (string.Equals(nameAttribute, elementId))
                    {
                        returnValue = GetAttributeValue(node, "value");
                        break;
                    }

                }
            }

            return returnValue;

        }

        //This method will read appSetting section
        public static SmtpSettings GetSmptSetting()
        {
            const string from = "from";
            const string host = "host";
            const string port = "port";
            
            var appSettingPath = string.Empty;
            StreamReader readSettings = null;
            var configSettings = string.Empty;
            var objXmlDocumt = new XmlDocument();
            appSettingPath = GetSisConfigXmlFilePath();

            if (!string.IsNullOrEmpty(appSettingPath) && System.IO.File.Exists(appSettingPath))
            {
                readSettings = System.IO.File.OpenText(appSettingPath);
            }

            if (readSettings != null)
            {
                configSettings = Convert.ToString(readSettings.ReadToEnd());
                readSettings.Close();
                //configSettings = Crypto.DecryptString(configSettings);
            }

            var settings = new SmtpSettings();

            if (!string.IsNullOrEmpty(configSettings))
            {
                objXmlDocumt.LoadXml(configSettings);
                var element = objXmlDocumt.GetElementsByTagName("smtpSettings");

                // ReSharper disable PossibleNullReferenceException
                foreach (XmlElement node in element.Item(0))
                // ReSharper restore PossibleNullReferenceException
                {
                    var nameAttribute = GetAttributeValue(node, "key");
                    if (string.Equals(nameAttribute, from))
                        settings.FromAddress = GetAttributeValue(node, "value");
                    if (string.Equals(nameAttribute, host))
                        settings.HostIp = GetAttributeValue(node, "value");
                    if (string.Equals(nameAttribute, port))
                    {
                        string portNumber = GetAttributeValue(node, "value");
                        settings.PortNumber = string.IsNullOrEmpty(portNumber)
                                                  ? 0
                                                  : Convert.ToInt16(portNumber);
                    }
                }
            }

            return settings;

        }

       /// <summary>
       /// Author: Sachin Pharande
       /// Date: 19-03-2013
       /// Purpose: This method will read appSetting section for 'smtpSecondarySettings'
       /// </summary>
       /// <returns> SmtpSettings object with From address, Host and Port</returns>
       public static SmtpSettings GetSmtpSecondarySetting()
       {
          // Constants and variables declaration used in this method.
          const string from = "from";
          const string host = "host";
          const string port = "port";
          var appSettingPath = string.Empty;
          StreamReader readSettings = null;
          var configSettings = string.Empty;
          var objXmlDocumt = new XmlDocument();
          var smtpSecondarySettings = new SmtpSettings();
          
          // Read the config file path.
          appSettingPath = GetSisConfigXmlFilePath();

          // Read appSettings from config file.
          if (!string.IsNullOrEmpty(appSettingPath) && System.IO.File.Exists(appSettingPath))
          {
            readSettings = System.IO.File.OpenText(appSettingPath);
          } // End if

          if (readSettings != null)
          {
            configSettings = Convert.ToString(readSettings.ReadToEnd());
            readSettings.Close();
          } // End if

          if (!string.IsNullOrEmpty(configSettings))
          {
            objXmlDocumt.LoadXml(configSettings);
            var element = objXmlDocumt.GetElementsByTagName("smtpSecondarySettings");

            // ReSharper disable PossibleNullReferenceException
            foreach (XmlElement node in element.Item(0))
            // ReSharper restore PossibleNullReferenceException
            {
              var nameAttribute = GetAttributeValue(node, "key");
              if (string.Equals(nameAttribute, from))
                smtpSecondarySettings.FromAddress = GetAttributeValue(node, "value");
              if (string.Equals(nameAttribute, host))
                smtpSecondarySettings.HostIp = GetAttributeValue(node, "value");
              if (string.Equals(nameAttribute, port))
              {
                string portNumber = GetAttributeValue(node, "value");
                smtpSecondarySettings.PortNumber = string.IsNullOrEmpty(portNumber) ? 0 : Convert.ToInt16(portNumber);
              } // End if
            } // End if
          } // End if
          return smtpSecondarySettings;
        }// End of GetSmtpSecondarySetting()

        private static string GetAttributeValue(XmlElement element, string attributename)
        {
            return element != null && element.HasAttribute(attributename)
                       ? element.Attributes[attributename].Value
                       : string.Empty;
        }

        public static NameValueCollection GetQuartzProperties()
        {
            NameValueCollection properties = new NameValueCollection();
            var appSettingPath = string.Empty;
            StreamReader readSettings = null;
            var configSettings = string.Empty;

            var objXmlDocumt = new XmlDocument();
            appSettingPath = GetSisConfigXmlFilePath();

            if (!string.IsNullOrEmpty(appSettingPath) && System.IO.File.Exists(appSettingPath))
            {
                readSettings = System.IO.File.OpenText(appSettingPath);
            }

            if (readSettings != null)
            {
                configSettings = Convert.ToString(readSettings.ReadToEnd());
                readSettings.Close();
                //configSettings = Crypto.DecryptString(configSettings);
            }

            if (!string.IsNullOrEmpty(configSettings))
            {
                objXmlDocumt.LoadXml(configSettings);
                var element = objXmlDocumt.GetElementsByTagName("quartz").Item(0);
                // ReSharper disable PossibleNullReferenceException)
                foreach (XmlElement node in element.ChildNodes) // ReSharper restore PossibleNullReferenceException
                {
                    var nameAttribute = GetAttributeValue(node, "key");
                    var valueAttribute = GetAttributeValue(node, "value");
                    if (nameAttribute.Contains("quartz"))
                    {
                        properties.Add(nameAttribute, valueAttribute);
                    }
                }
                //SCP0000: Connection Time out parameter added in connection string and the same is applied at code level.
                if (properties != null)
                    properties.Add("quartz.dataSource.default.connectionString", GetconfigConnection("QuartzConnectionString"));
            }

            return properties;
        }

        public class SmtpSettings
        {
            private int portNumber;
            private const int defaultPortNumber = 25;

            public string FromAddress { get; set; }
            public string HostIp { get; set; }
           
            public int PortNumber
            {
                get { return portNumber == 0 ? defaultPortNumber : portNumber; }
                set { portNumber = value; }
            }
        }

        #endregion
    }
}
