using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using Castle.Core.Smtp;
using Iata.IS.AdminSystem;
using Iata.IS.Business.FileCore;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.SalesForce;
using log4net;
using log4net.Repository.Hierarchy;
using NVelocity;

namespace Iata.IS.Business.SalesForce.Impl
{
    public class SalesForce : ISalesForce
    {
        public IRepository<SalesForceMemberDetails> MemberDetailsRepository { get; set; }
        public IRepository<SalesForceContactDetails> ContactDetailsRepository { get; set; }
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public ITemplatedTextGenerator TemplatedTextGenerator { get; set; }
        public IEmailSender EmailSender { get; set; }
    

        public void CreateSalesForce()
        {
            bool isContactCsvgenerated = false;
            bool isCsvgenerated = false;
            VelocityContext context;

            try
            {
                var filteredList = MemberDetailsRepository.GetAll().ToList();

                if (filteredList.Count > 0)
                {
                    //SCP422476: Failed to upload CSVs. - SIS Production
                    _logger.Info("count of list" + filteredList.Count);
                    string memberFilePath = GetIataDownloadfolderPath(1);
                     _logger.Info("member csv path" + memberFilePath);
                    if (memberFilePath != null)
                    {
                         isCsvgenerated = GenerateCSV(filteredList, memberFilePath);

                        if(isCsvgenerated)
                           _logger.Info(" For SIS Member data - SIS_MEMBER.csv generated successfully");
                        else
                            _logger.Info(" For SIS Member data - SIS_MEMBER.csv not generated");
                    }
                }

                var filteredContactList = ContactDetailsRepository.GetAll().ToList();

                if (filteredContactList.Count > 0)
                {
                     _logger.Info("count of list" + filteredContactList.Count);
                    string memberFilePath = GetIataDownloadfolderPath(0);
                     _logger.Info("contactr csv path" + memberFilePath);
                    if (memberFilePath != null)
                    {
                         isContactCsvgenerated = GenerateCSV(filteredContactList, memberFilePath);

                        if (isContactCsvgenerated)
                            _logger.Info(" For SIS Contact data - SIS_CONTACT.csv generated successfully");
                        else
                            _logger.Info(" For SIS Contact data - SIS_CONTACT.csv not generated");
                    }
                }

                if(isCsvgenerated  && isContactCsvgenerated)
                {
                    if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["NotificationEmail"]))
                    {
                        _logger.Info("Email Address is not specified in config file.");
                        return;
                    }
                    context = new VelocityContext();
                    //context.Put("FileName", rechargeDataFileName);

                    context.Put("SISOpsEmailId", SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);
                    SendNotificationToISAdmin(context, EmailTemplateId.SalesForceTemplate,
                                             ConfigurationManager.AppSettings["NotificationEmail"].ToString());
                }
                else
                {
                    if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["NotificationEmail"]))
                    {
                        _logger.Info("Email Address is not specified in config file.");
                        return;
                    }
                    context = new VelocityContext();
                    if (isContactCsvgenerated == false)
                    {
                        context.Put("filename1", "For SIS Contact data - SIS_CONTACT.csv");
                        context.Put("message1", "Failed to generate.");
                    }
                    else
                    {
                        context.Put("filename1", "For SIS Contact data - SIS_CONTACT.csv");
                        context.Put("message1", "Successfully generated.");
                    }
                    if (isCsvgenerated == false)
                    {
                        context.Put("filename2", "For SIS Member data - SIS_MEMBER.csv");
                        context.Put("message2", "Failed to generate.");
                    }
                    else
                    {
                        context.Put("filename2", "For SIS Member data - SIS_MEMBER.csv");
                        context.Put("message2", "Successfully generated.");
                    }
                    context.Put("SISOpsEmailId", SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);
                    SendNotificationToISAdmin(context, EmailTemplateId.SalesForceFailureTemplate,
                                             ConfigurationManager.AppSettings["NotificationEmail"].ToString());
                } 
                
            }

            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["NotificationEmail"]))
                {
                    _logger.Info("Email Address is not specified in config file.");
                    return;
                }
                context = new VelocityContext();
                if (isContactCsvgenerated == false)
                {
                    context.Put("filename1", "For SIS Contact data - SIS_CONTACT.csv");
                    context.Put("message1", "Failed to generate.");
                }
                else
                {
                    context.Put("filename1", "For SIS Contact data - SIS_CONTACT.csv");
                    context.Put("message1", "Successfully generated.");
                }
                if (isCsvgenerated == false)
                {
                    context.Put("filename2", "For SIS Member data - SIS_MEMBER.csv");
                    context.Put("message2", "Failed to generate.");
                }
                else
                {
                    context.Put("filename2", "For SIS Member data - SIS_MEMBER.csv");
                    context.Put("message2", "Successfully generated.");
                }

                //SCP422476 - SRM: Failed to upload CSVs. - SIS Production [Remove hard code email address]
                context.Put("SISOpsEmailId", SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);
                SendNotificationToISAdmin(context, EmailTemplateId.SalesForceFailureTemplate,
                                         ConfigurationManager.AppSettings["NotificationEmail"].ToString());

                _logger.Error("error message inside catch" + ex.Message + "stack" + ex.StackTrace);
            }
        }


        public string GetIataDownloadfolderPath(int identifier)
        {
            // Get IATA FTP Download folder path. 
            //string ftpPathIata = FileIo.GetFtpDownloadFolderPath("0IA");
            string ftpPathIata = ConfigurationManager.AppSettings["TargetFolderPath"];

            //\\10.1.2.145\san\SIT\FTPRoot\0IA\Download\

            string fileName = identifier == 1 ? "SIS_MEMBER.csv" : "SIS_CONTACT.csv";

            // Check if download folder path is not null or empty.
            if (!string.IsNullOrEmpty(ftpPathIata))
            {
                var MemberDetailsFilePath = Path.Combine(ftpPathIata, fileName);

                // Delete File if already exists.
                if (File.Exists(MemberDetailsFilePath))
                {
                    File.Delete(MemberDetailsFilePath);
                }// End if

                return MemberDetailsFilePath;
            }

            return null;
        }

        public bool GenerateCSV<T>(List<T> list, string outputPath)
        {
           
            if (list.Count <= 0)
            {
                return false;
            }

            var sbCsvData = new StringBuilder();

            //Get all properties for Type T
            var propInfos = typeof(T).GetProperties();
            var displayPropInfos = new List<PropertyInfo>();

            //Write headers in CSV
            for (var propertyCount = 0; propertyCount <= propInfos.Length - 1; propertyCount++)
            {
                if (propInfos[propertyCount].GetCustomAttributes(typeof(DisplayNameAttribute), true).Cast<DisplayNameAttribute>().Count() <= 0)
                {
                    continue;
                }

                displayPropInfos.Add(propInfos[propertyCount]);

                var attribute = propInfos[propertyCount].GetCustomAttributes(typeof(DisplayNameAttribute), true).Cast<DisplayNameAttribute>().Single();
                var displayName = attribute.DisplayName;

                sbCsvData.Append(displayName.Trim());

                if (propertyCount < propInfos.Length - 1)
                {
                    sbCsvData.Append(",");
                }
            }
            sbCsvData.AppendLine();

            // Write headers data in CSV
            for (var propCollectionCount = 0; propCollectionCount <= list.Count - 1; propCollectionCount++)
            {
                T item = list[propCollectionCount];

                var propInfoCount = 0;
                foreach (var propInfo in displayPropInfos)
                {
                    var objectValue = item.GetType().GetProperty(propInfo.Name).GetValue(item, null);
                    if (objectValue != null)
                    {
                        var value = objectValue.ToString();


                        if (objectValue.GetType().Equals(typeof(decimal)))
                        {
                            value = ((decimal)objectValue).ToString("00.000");
                        }

                        //Check if the value contains a comma and place it in quotes if so
                        if (value.Contains(","))
                        {
                            value = string.Concat("\"", value, "\"");
                        }

                        //Replace any \r or \n special characters from a new line with a space
                        if (value.Contains("\r"))
                        {
                            value = value.Replace("\r", " ");
                        }
                        if (value.Contains("\n"))
                        {
                            value = value.Replace("\n", " ");
                        }
                        sbCsvData.Append(value);
                    }

                    if (propInfoCount < propInfos.Length - 1)
                    {
                        sbCsvData.Append(",");
                    }

                    propInfoCount++;
                }

                sbCsvData.AppendLine();
            }

            if (string.IsNullOrEmpty(sbCsvData.ToString()))
            {
                return false;
            }

            System.IO.File.WriteAllText(outputPath, sbCsvData.ToString());

            return System.IO.File.Exists(outputPath);

        }


        //private void SendInvoiceFailureNotificationToISAdmin(string clearancePeriod)
        private void SendNotificationToISAdmin(VelocityContext context, EmailTemplateId templateId, string receiverEmail)
        {

            try
            {
                //get an object of the EmailSender component
                //var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));

                //get an object of the TemplatedTextGenerator that is used to generate body text of email from a nvelocity template
                //var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));

                //Get an instance of email settings  repository
                var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));

                var emailSettingForISAdminAlert = emailSettingsRepository.Get(esfopu => esfopu.Id == (int)templateId);

                // Verify and log TemplatedTextGenerator for null value.
                _logger.Info(String.Format("TemplatedTextGenerator instance is: [{0}].", TemplatedTextGenerator == null ? "NULL" : "NOT NULL"));

                // Generate email body text for own profile updates contact type mail
                // ReSharper disable PossibleNullReferenceException
                var emailToISAdminText = TemplatedTextGenerator.GenerateTemplatedText(templateId, context);
                // ReSharper restore PossibleNullReferenceException

                // Create a mail object to send mail
                var msgForISAdminAlert = new MailMessage
                {
                    From = new MailAddress(emailSettingForISAdminAlert.SingleOrDefault().FromEmailAddress),
                    IsBodyHtml = true
                };

               
                //loop through the contacts list and add them to To list of mail to be sent
                msgForISAdminAlert.To.Add(receiverEmail); // new MailAddress(ConfigurationManager.AppSettings["ISAdminEmail"])

                //set subject of mail (replace special field placeholders with values)
                var subject = emailSettingForISAdminAlert.SingleOrDefault().Subject;
                msgForISAdminAlert.Subject = subject;

                //set body text of mail
                msgForISAdminAlert.Body = emailToISAdminText;

                // Verify and log EmailSender for null value.
                _logger.Info(String.Format("EmailSender instance is: [{0}]", EmailSender == null ? "NULL" : "NOT NULL"));

                //send the mail
                // ReSharper disable PossibleNullReferenceException
                EmailSender.Send(msgForISAdminAlert);
                // ReSharper restore PossibleNullReferenceException

            }// End try
            catch (Exception ex)
            {

                _logger.Error("Error occurred while sending Membr and Contact csv Successfully generated notification to IS Administrator", ex);
                _logger.Error("message" + ex.Message + "stacktrace" + ex.StackTrace);
                throw new ISBusinessException(ErrorCodes.ErrorSendingRechargeDataNotification, "Error ocurred while sending  Notification to IS Administrator");

            }// End catch
        }
    }
}
