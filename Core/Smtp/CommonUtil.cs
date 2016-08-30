using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using log4net;

namespace Iata.IS.Core.Smtp
{
    public class CommonUtil
    {
        // Logger instance.
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static string IP_Address
        {
            get
            {

                var host = Dns.GetHostName();
                var ip = Dns.GetHostEntry(host);
                return ip.AddressList.Length > 1 ? ip.AddressList[1].ToString() : ip.AddressList[0].ToString();
            }
        }

        public static void SendEmail(Exception exception, string customMessage = null)
        {
            try
            {
                //read exception email address from main config file not from 
                //var emailAddresses = ConfigurationManager.AppSettings.Get("ExceptionEmailNotification");
                Logger.Error("Entered into send email method in common util");
                var emailAddresses = Iata.IS.Core.Configuration.ConnectionString.GetconfigAppSetting("ExceptionEmailNotification");
                Logger.Error("Exception email address " + emailAddresses);
                var smptSettings = Iata.IS.Core.Configuration.ConnectionString.GetSmptSetting();
                Logger.Error("smpt setting are" + smptSettings.HostIp + " & " + smptSettings.PortNumber + " & " + smptSettings.FromAddress);
                Logger.Error(emailAddresses);
                if (!string.IsNullOrEmpty(emailAddresses))
                {
                    var assembly = Assembly.GetEntryAssembly();
                    var serviceName = assembly == null ? "service" : assembly.GetName().Name;
                    var mail = new MailMessage
                    {
                        Subject = string.Format("An exception thrown in {0} on {1} server.", serviceName, IP_Address),
                        Body =
                           string.Format("An exception thrown in '{0}' on server:{4}. {3}Message: {1}  {3}StackTrace:{2}",
                                        serviceName,
                                        (string.IsNullOrEmpty(customMessage) ? exception.Message : customMessage),
                                        exception.StackTrace,
                                        Environment.NewLine, IP_Address),

                        From = new MailAddress(smptSettings.FromAddress)
                    };

                    foreach (var emailAddress in emailAddresses.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        mail.To.Add(new MailAddress(emailAddress));
                    }

                    var smtpClient = new SmtpClient(smptSettings.HostIp, smptSettings.PortNumber);
                    smtpClient.Send(mail);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occurred in SendEmail()", ex);
            }
        }
    }
}
