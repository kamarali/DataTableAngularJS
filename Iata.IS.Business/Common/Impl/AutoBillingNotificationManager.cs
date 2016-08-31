using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Castle.Core.Smtp;
using Iata.IS.AdminSystem;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile.Enums;
using log4net;
using System.Net.Mail;
using NVelocity;

namespace Iata.IS.Business.Common.Impl
{
  public class AutoBillingNotificationManager : IAutoBillingNotificationManager
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// To send the AutoBilling Email notification for UnavaliableInvoiceFound and
    /// For the Threshould value of the Invoice Reached.
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="processingContact"></param>
    /// <param name="sisOpsMail"></param>
    /// <param name="emailTemplateId"></param>
    /// <param name="invThresholdValue"></param>
    /// <param name="avaliableInvoices"></param>
    /// <returns></returns>
    public bool SendUnavaliableOrThresholdReachedInvoiceNotification(int memberId, ProcessingContactType processingContact, string sisOpsMail, int emailTemplateId, long invThresholdValue = 0, long avaliableInvoices = 0)
    {
      var message = new MailMessage();
      try
      {
        var memberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));
        var contactTypeList = memberManager.GetContactsForContactType(memberId, processingContact);

        var toMailIds = new StringBuilder();
        var index = 0;

        if (contactTypeList != null)
        {
          foreach (var contact in contactTypeList)
          {
            index += 1;
            toMailIds.Append(index != contactTypeList.Count ? string.Format("{0}{1}", contact.EmailAddress, ",") : contact.EmailAddress);
          }
        }

        if (!string.IsNullOrEmpty(toMailIds.ToString()))
        {
          message.To.Add(toMailIds.ToString());
        }
        else
        {
          Logger.Info("Email address is not available for the user, so not able to send the message");
          return false;
        }

        var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));
        var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));

        var emailABillingUnavaliableInvoiceNotification = emailSettingsRepository.Get(abillEmail => abillEmail.Id == emailTemplateId);
        message.From = new MailAddress(emailABillingUnavaliableInvoiceNotification.SingleOrDefault().FromEmailAddress, "SIS No Reply");
        message.Subject = emailABillingUnavaliableInvoiceNotification.SingleOrDefault().Subject;

        var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));
        var context = new VelocityContext();
        if (contactTypeList != null)
        {
          if (emailTemplateId.Equals((int)EmailTemplateId.AutoBillingInvoiceUnavalableNotification))
          {
            context.Put("Message", new AutoBillingExceptionNotification { RecipientName = string.IsNullOrEmpty(contactTypeList[0].FirstName) ? string.Empty : contactTypeList[0].FirstName });
          }
          else if (emailTemplateId.Equals((int)EmailTemplateId.AutoBillingInvoiceThreshouldValueReachedNotification))
          {
            context.Put("Message", new AutoBillingExceptionNotification
                                     {
                                       RecipientName = string.IsNullOrEmpty(contactTypeList[0].FirstName) ? string.Empty : contactTypeList[0].FirstName,
                                       ThreshouldLimitValue = invThresholdValue,
                                       AvalableInvoiceCount = avaliableInvoices
                                     });
          }
          context.Put("SISOpsEmailId", sisOpsMail ?? SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);
        }

        message.Body = templatedTextGenerator.GenerateTemplatedText((EmailTemplateId)emailTemplateId, context);
        
        // replace '(From environment)' with '' from Subject line
        message.Subject.Replace("(From environment)", "(From environment)");

        emailSender.Send(message);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed sending the mail", ex);
        message.Dispose();
        return false;
      }

      return true;
    }
    
  }
}
