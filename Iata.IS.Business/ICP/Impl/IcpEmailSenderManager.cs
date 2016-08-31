using System;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using Castle.Core.Smtp;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using log4net;
using NVelocity;

namespace Iata.IS.Business.ICP.Impl
{
    /// <summary>
    /// This class is used to send email to sis ops and sis support for failure ICP web service.
    /// CMP #665: User Related Enhancements-FRS-v1.2
    /// </summary>
    public class IcpEmailSenderManager : IIcpEmailSenderManager
    {
        //Logger 
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public ITemplatedTextGenerator TemplatedTextGenerator { get; set; }

        /// <summary>
        /// This function is used to send email to sis ops and sis support for failure ICP web service.
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="emailId"></param>
        /// <param name="userCategory"></param>
        /// <param name="memberId"></param>
        /// <param name="memberCode"></param>
        /// <param name="template"></param>
        /// <param name="fedId"></param>
        /// <param name="requestType"></param>
        /// <param name="reasonFailure"></param>
        /// <param name="errorCode"></param>
        /// <param name="errorDesc"></param>
        /// <param name="isSuccess"></param>
        public void SendIcpFailedEmailNotification(string firstName, string lastName, string emailId, string userCategory, int memberId, string memberCode, EmailTemplateId template, string fedId = null, string requestType = null, string reasonFailure = null, string errorCode = null, string errorDesc = null, bool isSuccess  = true)
        {
            try
            {
                var context = new VelocityContext();

                // Get an object of the EmailSender component
                var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));

                // Get an instance of email settings repository
                var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));

                // Verify and log TemplatedTextGenerator for null value.
                _logger.Info(String.Format("TemplatedTextGenerator instance is: [{0}].", TemplatedTextGenerator == null ? "NULL" : "NOT NULL"));

                //Create Email Parameter Content
                try
                {
                    // Generate email body text for ICP service failure notification email
                    context.Put("RequestType", requestType);
                    context.Put("FirstName", firstName);
                    context.Put("LastName", lastName);
                    context.Put("EmailId", emailId);
                    context.Put("FederationId", fedId);
                    context.Put("UserCategory", userCategory);

                    //If member id greater than 0 then include member detail otherwise not include in the email template.
                    if (memberId > 0)
                    {
                        context.Put("MemberIdText", "Member ID: ");
                        context.Put("MemberId", memberId + "<BR/>");
                        context.Put("MemberCodeText", "Member Code: ");
                        context.Put("MemberCode", memberCode + "<BR/>");
                    }
                    else
                    {
                        context.Put("MemberIdText", String.Empty);
                        context.Put("MemberId", String.Empty);
                        context.Put("MemberCodeText", String.Empty);
                        context.Put("MemberCode", String.Empty);
                    }
                    context.Put("ReasonFailure", reasonFailure);

                    //If isSuccess is false then include error detail otherwise not include error detail in template.
                    if (!isSuccess)
                    {
                        context.Put("ErrorCodeText", "Error Code from ICP: ");
                        context.Put("ErrorCode", errorCode + "<BR/>");
                        context.Put("ErrorDescText", "Error Description  from ICP: ");
                        context.Put("ErrorDesc", errorDesc + "<BR/>");
                    }
                    else
                    {
                        context.Put("ErrorCodeText", String.Empty);
                        context.Put("ErrorCode", String.Empty);
                        context.Put("ErrorDescText", String.Empty);
                        context.Put("ErrorDesc", String.Empty);
                    }
                    
                    //Added request detail into Logger.
                    _logger.InfoFormat(
                        "Exception Notification has been sent, for RequestType: {0}, FirstName: {1}, LastName:{2}, EmailId:{3}, FederationId:{4}, UserCategory:{5}, MemberId:{6}, MemberCode:{7}, ReasonFailure:{8}, ErrorCode:{9}, ErrorDesc:{10}, ",
                        requestType, firstName, lastName, emailId, fedId, userCategory, memberId, memberCode,
                        reasonFailure, errorCode, errorDesc);
                }
                catch (Exception ex)
                {
                    _logger.Info("Error in getting Template Parameters : ", ex);
                }

                // Verify and log TemplatedTextGenerator for null value.
                _logger.Info(String.Format("TemplatedTextGenerator instance is: [{0}].", TemplatedTextGenerator == null ? "NULL" : "NOT NULL"));

                var emailSettingForIsAdminAlert = emailSettingsRepository.Get(esfopu => esfopu.Id == (int)template);

                // Generate email body text for own profile updates contact type mail.
                if (TemplatedTextGenerator != null)
                {
                    var bodyText = TemplatedTextGenerator.GenerateTemplatedText(template, context);

                    // Create a mail object to send mail.
                    var message = new MailMessage
                    {
                        From = new MailAddress(emailSettingForIsAdminAlert.SingleOrDefault().FromEmailAddress),
                        IsBodyHtml = true
                    };

                    var subject = emailSettingForIsAdminAlert.SingleOrDefault().Subject;
                    message.Subject = subject;
                    //Get support email id from sis config.xml file.
                    message.To.Add(
                        Core.Configuration.ConnectionString.GetconfigAppSetting("ExceptionEmailNotification"));

                    // loop through the contacts list and add them to To list of mail to be sent);)
                    if (!string.IsNullOrEmpty(AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail))
                    {
                        var mailAdressList = ConvertUtil.ConvertToMailAddresses(AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);

                        foreach (var mailaddr in mailAdressList)
                        {
                            message.To.Add(mailaddr);
                        }
                    }

                    //set body text of mail.
                    message.Body = bodyText;

                    //send the mail.
                    emailSender.Send(message);
                }
            }// End try
            catch (Exception ex)
            {
                _logger.Error("Error occurred while sending email to user.", ex);

                throw;
            }// End catch
        }
    }
}
