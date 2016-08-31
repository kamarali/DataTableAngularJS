using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using Castle.Core.Smtp;
using Iata.IS.Business.Common;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;
using log4net;
using NVelocity;

namespace Iata.IS.Business.MemberProfile.Impl
{
  class MemberLocationUpdateHandler : IMemberLocationUpdateHandler
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public bool LocationUpdateSenderForFutureUpdates(int memberId, List<FutureUpdateTemp> futureUpdateTemps)
    {
      bool flag = false;
      //get an object of the member manager
      var memberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));

      //declare an object of the nVelocity data dictionary
      VelocityContext context;

      //get an object of the TemplatedTextGenerator that is used to generate body text of email from a nvelocity template
      var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));

      //get an instance of email settings  repository
      var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));

      //get an object of the EmailSender component
      var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));

      //Get an instance of IFileManager for FTP Pull functionality
      var fileMgr = Ioc.Resolve<IFileManager>(typeof(IFileManager));

      try
      {
        // get the location related future updates for this particular member
        var memLocationFutureUpdates = from mlfu in futureUpdateTemps
                                       where mlfu.MemberId == memberId && mlfu.TableName == "MEM_LOCATION"
                                       select mlfu;

        //If this members location data was updated then we need to generate csv of all the locations of this member
        if (memLocationFutureUpdates.Count() > 0)
        {
          //get list of contacts of contact type Other Members Invoice Reference Data Updates for this member
          var invoiceRefContactList = memberManager.GetContactsForContactType(memberId, ProcessingContactType.OtherMembersInvoiceReferenceDataUpdates);

          //if contacts of type Other Members Invoice Reference Data Updates are found
          if (invoiceRefContactList != null && invoiceRefContactList.Count > 0)
          {
            var futureUpdate = memLocationFutureUpdates.First();

            //create nvelocity data dictionary
            context = new VelocityContext();
            context.Put("MemberPrefix", futureUpdate.MemberCodeNumeric);
            context.Put("MemberDesignator", futureUpdate.MemberCodeAlpha);
            context.Put("MemberCommercialName", futureUpdate.MemberCommercialName);
            context.Put("SISOpsEmail", AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);

            //generate email body text for Other Members Invoice Reference Data Updates contact type mail
            var emailToInvoiceRefContactsText = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.MemberLocationUpdatedWithCsv, context);

            //Get the eMail settings for member profile future update mails for Invoice Reference Data Updates contact type for email with csv
            var emailSettingForInvoiceReferenceDataUpdatesCsvEmail =
              emailSettingsRepository.Get(
                esfirdu => esfirdu.Id == (int)EmailTemplateId.MemberLocationUpdatedWithCsv);

            //create a mail object to send mail with csv attachment
            var msgInvoiceReferenceDataUpdatesForeMailWithCsv = new MailMessage { From = new MailAddress(emailSettingForInvoiceReferenceDataUpdatesCsvEmail.SingleOrDefault().FromEmailAddress) };
            msgInvoiceReferenceDataUpdatesForeMailWithCsv.IsBodyHtml = true;

            //loop through the contacts list and add them to To list of mail to be sent
            foreach (var contact in invoiceRefContactList)
            {
                // SCP182339 - Invoice Reference Data - Updates - SIS Production
              msgInvoiceReferenceDataUpdatesForeMailWithCsv.Bcc.Add(new MailAddress(contact.EmailAddress));
            }

            //set subject of mail (replace special field placeholders with values)
            msgInvoiceReferenceDataUpdatesForeMailWithCsv.Subject = emailSettingForInvoiceReferenceDataUpdatesCsvEmail.SingleOrDefault().Subject.Replace("$MemberName$", memLocationFutureUpdates.First().MemberCommercialName);

            //set body text of mail
            msgInvoiceReferenceDataUpdatesForeMailWithCsv.Body = emailToInvoiceRefContactsText;

            //send the mail
            emailSender.Send(msgInvoiceReferenceDataUpdatesForeMailWithCsv);

            //clear nvelocity context data
            context = null;
            flag = true;
          }
        }
      }
      catch (Exception exception)
      {
        Logger.Error("Error occurred occured in MemberLocationUpdateHandler component.", exception);
        flag = false;
      }

      return flag;
    }

    public bool LocationUpdateSenderForImmediateUpdates(int memberId, List<FutureUpdates> futureUpdates)
    {
      bool flag = false;
      //get an object of the member manager
      var memberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));

      //declare an object of the nVelocity data dictionary
      VelocityContext context;
      string emailToInvoiceRefContactsText = null;
      //get an object of the TemplatedTextGenerator that is used to generate body text of email from a nvelocity template
      var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));

      //get an instance of email settings  repository
      var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));

      //get an object of the EmailSender component
      var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));

      //Get an instance of IFileManager for FTP Pull functionality
      var fileMgr = Ioc.Resolve<IFileManager>(typeof(IFileManager));

      try
      {
        // get the location related future updates for this particular member
        var memLocationFutureUpdates = from mlfu in futureUpdates
                                       where mlfu.MemberId == memberId && mlfu.TableName == "MEM_LOCATION"
                                       select mlfu;

        //If this members location data was updated then we need to generate csv of all the locations of this member
        if (memLocationFutureUpdates.Count() > 0)
        {
          //get list of contacts of contact type Other Members Invoice Reference Data Updates for this member
          var invoiceRefContactList = memberManager.GetContactsForContactType(ProcessingContactType.OtherMembersInvoiceReferenceDataUpdates);

          //if contacts of type Other Members Invoice Reference Data Updates are found
          if (invoiceRefContactList != null && invoiceRefContactList.Count > 0)
          {
            //create nvelocity data dictionary
            context = new VelocityContext();

            //fill nvelocity data dictionary with data specific to template used for Other Members Invoice Reference Data Updates contact type mail
            //var pathParts = newCsvFileName.Split(Path.DirectorySeparatorChar);
            var futureUpdate = memLocationFutureUpdates.FirstOrDefault();

            if (futureUpdate != null)
            {
              context.Put("MemberPrefix", futureUpdate.Member.MemberCodeNumeric);
              context.Put("MemberDesignator", futureUpdate.Member.MemberCodeAlpha);
              context.Put("MemberCommercialName", futureUpdate.Member.CommercialName);
              context.Put("SISOpsEmail", AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);

              //generate email body text for Other Members Invoice Reference Data Updates contact type mail
              emailToInvoiceRefContactsText =
                templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.MemberLocationUpdatedWithCsv, context);
            }

            //Get the eMail settings for member profile future update mails for Invoice Reference Data Updates contact type for email with csv
            var emailSettingForInvoiceReferenceDataUpdatesCsvEmail =
              emailSettingsRepository.Get(
                esfirdu => esfirdu.Id == (int)EmailTemplateId.MemberLocationUpdatedWithCsv);

            //create a mail object to send mail with csv attachment
            var msgInvoiceReferenceDataUpdatesForeMailWithCsv = new MailMessage { From = new MailAddress(emailSettingForInvoiceReferenceDataUpdatesCsvEmail.SingleOrDefault().FromEmailAddress) };
            msgInvoiceReferenceDataUpdatesForeMailWithCsv.IsBodyHtml = true;
            
            var distinctContactList = invoiceRefContactList.Select(m => m.EmailAddress).Distinct(); ;

            //loop through the contacts list and add them to To list of mail to be sent)
            foreach (var contact in distinctContactList)
            {
                // SCP182339 - Invoice Reference Data - Updates - SIS Production
              msgInvoiceReferenceDataUpdatesForeMailWithCsv.Bcc.Add(new MailAddress(contact));
            }

            //set subject of mail (replace special field placeholders with values)
            msgInvoiceReferenceDataUpdatesForeMailWithCsv.Subject = emailSettingForInvoiceReferenceDataUpdatesCsvEmail.SingleOrDefault().Subject.Replace("$MemberName$", memLocationFutureUpdates.FirstOrDefault().Member.CommercialName);

            //set body text of mail
            if (emailToInvoiceRefContactsText != null)
            {
              msgInvoiceReferenceDataUpdatesForeMailWithCsv.Body = emailToInvoiceRefContactsText;
            }

            //send the mail
            emailSender.Send(msgInvoiceReferenceDataUpdatesForeMailWithCsv);

            flag = true;
          }
        }
      }
      catch (Exception exception)
      {
        Logger.Error("Error occurred occured in MemberLocationUpdateHandler component.", exception);
        flag = false;
      }

      return flag;
    }
  }
}
