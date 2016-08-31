using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using Castle.Core.Smtp;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.MemberProfile.Impl;
using Iata.IS.Business.Pax;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Data.Pax;
using Iata.IS.Data.Pax.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.Pax.Enums;
using log4net;
using NVelocity;

namespace Iata.IS.Business.Reports.Impl
{
  public class MissingSubmissionAchManager : IMissingSubmissionAchManager
  {
    public IRepository<AchConfiguration> AchRepository { get; set; }

    public IRepository<InvoiceBase> InvoiceRepository { get; set; }

    public IRepository<Member> MemberRepository { get; set; }

    private readonly ICalendarManager _calenderManager;

    private readonly IInvoiceManager _invoiceManager;

    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public MissingSubmissionAchManager(ICalendarManager calenderManager,IInvoiceManager invoiceManager)
    {
      _calenderManager = calenderManager;
      _invoiceManager = invoiceManager;

    }
    private List<AchConfiguration> GetAchMemberRecordList()
    {
      var achMemberRecords = AchRepository.Get(achConfig => achConfig.AchMembershipStatusId == (int)IchMemberShipStatus.Live && (achConfig.AchClearanceInvoiceSubmissionPatternCgoId > 0 || achConfig.AchClearanceInvoiceSubmissionPatternMiscId > 0 || achConfig.AchClearanceInvoiceSubmissionPatternPaxId > 0 || achConfig.AchClearanceInvoiceSubmissionPatternUatpId > 0));
      return achMemberRecords.ToList();

    }
    /// <summary>
    /// Gets the list of members with missing submission.
    /// </summary>
    /// <param name="isSummary">Is Summary</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <returns></returns>
    private List<string> GetMissingSubmissionMembers(bool isSummary, BillingPeriod billingPeriod)
    {
      var memberwithMissingSubmissionList = new List<string>();
      Logger.Info("GetAchMemberRecordList()");
      var members = GetAchMemberRecordList();
      if (members != null) Logger.InfoFormat("Member Count:{0}", members.Count);
      foreach (var achConfiguration in members)
      {
        //SCP147427 - Missing submission summary for billing period 20130603
        Logger.InfoFormat("Check for missing Submission formemberId : {0}", achConfiguration.MemberId.ToString());
        var missingBillingCategory = achConfiguration.MemberId.ToString();
        missingBillingCategory = missingBillingCategory + '|' + (CheckMissingSubmissionForBillingCategory((int)BillingCategoryType.Pax, achConfiguration.AchClearanceInvoiceSubmissionPatternPaxId, achConfiguration.MemberId, (int)SMI.Ach, isSummary, "A", billingPeriod) && CheckMissingSubmissionForBillingCategory((int)BillingCategoryType.Pax, achConfiguration.AchClearanceInvoiceSubmissionPatternPaxId, achConfiguration.MemberId, (int)SMI.AchUsingIataRules, isSummary, "A", billingPeriod));
        missingBillingCategory = missingBillingCategory + ',' + (CheckMissingSubmissionForBillingCategory((int)BillingCategoryType.Cgo, achConfiguration.AchClearanceInvoiceSubmissionPatternCgoId, achConfiguration.MemberId, (int)SMI.Ach, isSummary, "A", billingPeriod) && CheckMissingSubmissionForBillingCategory((int)BillingCategoryType.Cgo, achConfiguration.AchClearanceInvoiceSubmissionPatternCgoId, achConfiguration.MemberId, (int)SMI.AchUsingIataRules, isSummary, "A", billingPeriod));
        missingBillingCategory = missingBillingCategory + ',' + (CheckMissingSubmissionForBillingCategory((int)BillingCategoryType.Misc, achConfiguration.AchClearanceInvoiceSubmissionPatternMiscId, achConfiguration.MemberId, (int)SMI.Ach, isSummary, "A", billingPeriod) && CheckMissingSubmissionForBillingCategory((int)BillingCategoryType.Misc, achConfiguration.AchClearanceInvoiceSubmissionPatternMiscId, achConfiguration.MemberId, (int)SMI.AchUsingIataRules, isSummary, "A", billingPeriod));
        missingBillingCategory = missingBillingCategory + ',' + (CheckMissingSubmissionForBillingCategory((int)BillingCategoryType.Uatp, achConfiguration.AchClearanceInvoiceSubmissionPatternUatpId, achConfiguration.MemberId, (int)SMI.Ach, isSummary, "A", billingPeriod) && CheckMissingSubmissionForBillingCategory((int)BillingCategoryType.Uatp, achConfiguration.AchClearanceInvoiceSubmissionPatternUatpId, achConfiguration.MemberId, (int)SMI.AchUsingIataRules, isSummary, "A", billingPeriod));
        missingBillingCategory = missingBillingCategory + ',' + CheckMissingSubmissionForBillingCategory((int)BillingCategoryType.Pax, achConfiguration.InterClearanceInvoiceSubmissionPatternPaxId, achConfiguration.MemberId, (int)SMI.AchUsingIataRules, isSummary, "B", billingPeriod);
        missingBillingCategory = missingBillingCategory + ',' + CheckMissingSubmissionForBillingCategory((int)BillingCategoryType.Cgo, achConfiguration.InterClearanceInvoiceSubmissionPatternCgoId, achConfiguration.MemberId, (int)SMI.AchUsingIataRules, isSummary, "B", billingPeriod);
        missingBillingCategory = missingBillingCategory + ',' + CheckMissingSubmissionForBillingCategory((int)BillingCategoryType.Misc, achConfiguration.InterClearanceInvoiceSubmissionPatternMiscId, achConfiguration.MemberId, (int)SMI.AchUsingIataRules, isSummary, "B", billingPeriod);
        missingBillingCategory = missingBillingCategory + ',' + CheckMissingSubmissionForBillingCategory((int)BillingCategoryType.Uatp, achConfiguration.InterClearanceInvoiceSubmissionPatternUatpId, achConfiguration.MemberId, (int)SMI.AchUsingIataRules, isSummary, "B", billingPeriod);
        memberwithMissingSubmissionList.Add(missingBillingCategory);
      }
      return memberwithMissingSubmissionList;
    }
    /// <summary>
    /// Method to check if there is any missing submission based on different billing categories
    /// taking in account SMI as well as Clearance House
    /// Logic:-  1. For ACH Clearance, it gets invoice count based on (SMI = 2 OR (SMI = 5 and Clearing_House = “A”))
    /// 2. For Inter Clearance, it gets invoice count based on (SMI = 5 and Clearing_House = “B”)
    /// </summary>
    /// <param name="categoryType">Billing Category</param>
    /// <param name="submissionPattern">Invoice Submission Pattern</param>
    /// <param name="memberId">Member Id</param>
    /// <param name="smi">Settlement Method ID</param>
    /// <param name="isSummary">Is Summary</param>
    /// <param name="clearanceHouse">Clearance House</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <returns></returns>

    public bool CheckMissingSubmissionForBillingCategory(int categoryType, int submissionPattern, int memberId, int smi, bool isSummary, string clearanceHouse, BillingPeriod billingPeriod)
    {
      //SCP147427 - Missing submission summary for billing period 20130603
      var period = GetClearenceInvoiceSubmissionPattern(submissionPattern);
      if (period[billingPeriod.Period - 1] == 1)
      {
        //Change done to check for at least one invoice having invoice status Ready for Billing/Claimed/Presented/Processing Complete.
        Logger.InfoFormat("Get Invoice count for memberId:{0} and BillingCategoryId:{1}", memberId, categoryType);
          //var count = InvoiceRepository.GetCount(invoice => invoice.BillingMemberId == memberId && 
          //                                                  invoice.BillingCategoryId == categoryType && 
          //                                                  invoice.BillingYear == billingPeriod.Year && 
          //                                                  invoice.BillingMonth == billingPeriod.Month && 
          //                                                  invoice.BillingPeriod == billingPeriod.Period && 
          //                                                  invoice.SettlementMethodId == smi && 
          //                                                  (invoice.InvoiceStatusId == (int)InvoiceStatusType.Claimed || invoice.InvoiceStatusId == (int)InvoiceStatusType.ProcessingComplete || invoice.InvoiceStatusId == (int)InvoiceStatusType.Presented || invoice.InvoiceStatusId == (int)InvoiceStatusType.ReadyForBilling) &&
          //                                                  ((!string.IsNullOrEmpty(clearanceHouse) && invoice.ClearingHouse == clearanceHouse)));
        var count = _invoiceManager.GetAchInvoiceCount(memberId, categoryType, billingPeriod.Year, billingPeriod.Month, billingPeriod.Period, smi, clearanceHouse);
        Logger.InfoFormat("Invoice count:{0}", count);
        if (count == 0)
        {
          return true;
        }
        return false;
      }
      return false;
    }

    private static int[] GetClearenceInvoiceSubmissionPattern(int submissionPattern)
    {
      var period = new int[4];

      if (submissionPattern == 1)
      {
        period[0] = 0;
        period[1] = 0;
        period[2] = 0;
        period[3] = 1;
      }
      else switch (submissionPattern.ToString().Length)
        {
          case 3:
            period[0] = 0;
            period[1] = int.Parse((submissionPattern.ToString()[0]).ToString());
            period[2] = int.Parse((submissionPattern.ToString()[1]).ToString());
            period[3] = int.Parse((submissionPattern.ToString()[2]).ToString());
            break;
          case 2:
            period[0] = 0;
            period[1] = 0;
            period[2] = int.Parse((submissionPattern.ToString()[0]).ToString());
            period[3] = int.Parse((submissionPattern.ToString()[1]).ToString());
            break;
          case 4:
            period[0] = int.Parse((submissionPattern.ToString()[0]).ToString());
            period[1] = int.Parse((submissionPattern.ToString()[1]).ToString());
            period[2] = int.Parse((submissionPattern.ToString()[2]).ToString());
            period[3] = int.Parse((submissionPattern.ToString()[3]).ToString());
            break;
        }
      return period;
    }

    public bool SendMissingSubmissionNotification(int memberId, string memberRecord, string billingPeriod, bool isSummary)
    {
      Logger.Info("SendMissingSubmissionNotification");
      try
      {
        if (!isSummary)
        {
          //declare an object of the nVelocity data dictionary
          VelocityContext context;
          var achBillingCategorypax = "";
          var achBillingCategorycgo = "";
          var achBillingCategorymisc = "";
          var achBillingCategoryuatp = "";
          var interBillingCategorypax = "";
          var interBillingCategorycgo = "";
          var interBillingCategorymisc = "";
          var interBillingCategoryuatp = "";
          var interBillingCategory = new string[4];
          //get an object of the EmailSender component
          var emailSender = Ioc.Resolve<IEmailSender>(typeof (IEmailSender));
          var str = memberRecord.Split('|');
          var missingBillingCategory = str[1].Split(',');

          if (bool.Parse(missingBillingCategory[0]))
            achBillingCategorypax = "Passenger";
          if (bool.Parse(missingBillingCategory[1]))
            achBillingCategorycgo = "Cargo";
          if (bool.Parse(missingBillingCategory[2]))
            achBillingCategorymisc = "Miscellaneous";
          if (bool.Parse(missingBillingCategory[3]))
            achBillingCategoryuatp = "UATP";

          if (bool.Parse(missingBillingCategory[4]))
            interBillingCategorypax = "Passenger";
          if (bool.Parse(missingBillingCategory[5]))
            interBillingCategorycgo = "Cargo";
          if (bool.Parse(missingBillingCategory[6]))
            interBillingCategorymisc = "Miscellaneous";
          if (bool.Parse(missingBillingCategory[7]))
            interBillingCategoryuatp = "UATP";

          //get an object of the TemplatedTextGenerator that is used to generate body text of email from a nvelocity template
          var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof (ITemplatedTextGenerator));
          //get an instance of email settings  repository
          var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof (IRepository<EmailTemplate>));
          //object of the nVelocity data dictionary
          context = new VelocityContext();
          context.Put("achBillingCategorypax", achBillingCategorypax);
          context.Put("achBillingCategorycgo", achBillingCategorycgo);
          context.Put("achBillingCategorymisc", achBillingCategorymisc);
          context.Put("achBillingCategoryuatp", achBillingCategoryuatp);
          context.Put("interBillingCategorypax", interBillingCategorypax);
          context.Put("interBillingCategorycgo", interBillingCategorycgo);
          context.Put("interBillingCategorymisc", interBillingCategorymisc);
          context.Put("interBillingCategoryuatp", interBillingCategoryuatp);
          context.Put("BillingPeriod", billingPeriod);
          var emailSettingForMissingSubmission =
            emailSettingsRepository.Get(esfopu => esfopu.Id == (int) EmailTemplateId.MissingSubmissionNotification);

          //generate email body text for own profile updates contact type mail
          var emailToMemberPrimaryContactsText =
            templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.MissingSubmissionNotification, context);
          //create a mail object to send mail
          var msgForMissingSubmission = new MailMessage
                                          {
                                            From =
                                              new MailAddress(
                                              emailSettingForMissingSubmission.SingleOrDefault().FromEmailAddress),
                                            IsBodyHtml = true
                                          };
          //loop through the contacts list and add them to To list of mail to be sent

          // TODO:Populate email Address list for members whom missing submission notifiation is to be sent 

          var memberManager = Ioc.Resolve<Iata.IS.Business.MemberProfile.Impl.MemberManager>(typeof (IMemberManager));
          var achMemberContactList = memberManager.GetContactsForContactType(memberId,
                                                                             ProcessingContactType.ACHPrimaryContact);
          if (achMemberContactList != null)
            foreach (var contact in achMemberContactList)
            {
              msgForMissingSubmission.To.Add(new MailAddress(contact.EmailAddress));
              Logger.Info("Notification email sent to :" + contact.EmailAddress);
            }

          //set subject of mail (replace special field placeholders with values)

          msgForMissingSubmission.Subject =
            emailSettingForMissingSubmission.SingleOrDefault().Subject.Replace("$BillingPeriod$", billingPeriod);

          //set body text of mail
          msgForMissingSubmission.Body = emailToMemberPrimaryContactsText;

          //send the mail
          if (msgForMissingSubmission.To.Count() > 0)
            emailSender.Send(msgForMissingSubmission);
          Logger.Info("Notification email sent to " + memberId);
          //Logger.Info("Notification email Body " + emailToMemberPrimaryContactsText);
          //clear nvelocity context data
          context = null;
          return true;
        }
      }
      catch (Exception exception)
      {
        Logger.Error("Error occurred in Missing Submission Notification Email Handler (Send Mails for a multiple Members method).", exception);
        return false;
      }

      return true;
    }

    public bool SendMissingSuspenssionSummary(List<string[]> memberRecord, string billingPeriod,bool isSummary)
    {
      Logger.Info("SendMissingSuspenssionSummary Start.");
      try
      {
        if (isSummary)
        {
          //declare an object of the nVelocity data dictionary
          VelocityContext context;
          //get an object of the EmailSender component
          var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));

          //get an object of the TemplatedTextGenerator that is used to generate body text of email from a nvelocity template
          var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));
          //get an instance of email settings  repository
          var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));
          //object of the nVelocity data dictionary
          context = new VelocityContext();
          context.Put("memberRecord", memberRecord);
          // context.Put("BillingPeriod", billingPeriod);
          var emailSettingForMissingSubmission = emailSettingsRepository.Get(esfopu => esfopu.Id == (int) EmailTemplateId.MissingSubmissionSummary);

          //generate email body text for own profile updates contact type mail
          var emailToMemberPrimaryContactsText = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.MissingSubmissionSummary, context);
          //create a mail object to send mail
          var msgForMissingSubmission = new MailMessage { From = new MailAddress(emailSettingForMissingSubmission.SingleOrDefault().FromEmailAddress), IsBodyHtml = true };
          //loop through the contacts list and add them to To list of mail to be sent

          // TODO:Populate email Address list for members whom missing submission notifiation is to be sent 
          var memberManager = Ioc.Resolve<Iata.IS.Business.MemberProfile.Impl.MemberManager>(typeof(IMemberManager));

          Logger.Info("Getting ACH email IDs for sending missing submission email");


          if (!string.IsNullOrEmpty(AdminSystem.SystemParameters.Instance.ACHDetails.ACHMissingSubmissionContact))
          {
            var emailAddressList = AdminSystem.SystemParameters.Instance.ACHDetails.ACHMissingSubmissionContact;
            var formatedEmailList = emailAddressList.Replace(',', ';');
            var mailAdressList = ConvertUtil.ConvertToMailAddresses(formatedEmailList);

            // loop through the contacts list and add them to To list of mail to be sent
            foreach (var mailaddr in mailAdressList)
            {
              msgForMissingSubmission.To.Add(mailaddr);
            }
          }

          // Set subject of mail (replace special field placeholders with values)
          msgForMissingSubmission.Subject = emailSettingForMissingSubmission.SingleOrDefault().Subject.Replace("$BillingPeriod$", billingPeriod);

          // Set body text of mail
          msgForMissingSubmission.Body = emailToMemberPrimaryContactsText;

          // Send the mail
          if (msgForMissingSubmission.To.Count() > 0)
          {
            emailSender.Send(msgForMissingSubmission);
          }
          var achEmailId = AdminSystem.SystemParameters.Instance.ACHDetails.ACHMissingSubmissionContact;
          Logger.InfoFormat("Summary email sent to {0}", achEmailId);
          Logger.Info("SendMissingSuspenssionSummary End.");
          return true;
        }
      }
      catch (Exception exception)
      {
        Logger.Error("Error occurred in Missing Submission Notification Email Handler (Send Mails for a multiple Members method).", exception);
        return false;
      }

      return true;
    }

    public void CheckMissingSubmission(bool isSummary)
    {
      //SCP147427 - Missing submission summary for billing period 20130603
      Logger.Info("Called CheckMissingSubmission()");
      BillingPeriod billingPeriod;
      //var currentBillingPeriod = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ach);
      if (isSummary)
      {
        billingPeriod = _calenderManager.GetLastClosedBillingPeriod(ClearingHouse.Ach);

      }

      else
      {
        billingPeriod = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ach);
      }

      var memberRecordList = GetMissingSubmissionMembers(isSummary, billingPeriod);
      Logger.InfoFormat("Member Count :{0}",memberRecordList.Count);
      if (memberRecordList.Count > 0)
      {
        var summaryList = new List<string>();
        var currentMonth = "";
        

        if (billingPeriod.Month.ToString().Length == 1)
          currentMonth = "0" + billingPeriod.Month;
        else
        {
          currentMonth = billingPeriod.Month.ToString();
        }
        foreach (var memberRecord in memberRecordList)
        {
          var record = memberRecord;
          var isMailSend = false;
          var str = memberRecord.Split('|');
          var missingSubmissions = str[1].Split(',');
          foreach (var missingSubmission in missingSubmissions)
          {
            if (bool.Parse(missingSubmission))
            {
              isMailSend = true;
              break;
            }
          }
          if (isMailSend)
          {
            summaryList.Add(record);
            SendMissingSubmissionNotification(int.Parse(str[0]), memberRecord,
                                              billingPeriod.Year.ToString() +
                                              currentMonth + "0" +
                                              billingPeriod.Period.ToString(), isSummary);

          }

        }

        if ((summaryList.Count > 0))
        {
          var memberRecords = GetFormattedRecord(summaryList);
          SendMissingSuspenssionSummary(memberRecords, billingPeriod.Year.ToString() + currentMonth + "0" +
                                        billingPeriod.Period.ToString(), isSummary);
        }
      }
    }

    private List<string[]> GetFormattedRecord(IEnumerable<string> memberRecords)
    {
      Logger.Info("GetFormattedRecord Start.");
      var formattedRecords = new List<string[]>();
      foreach (var memberRecord in memberRecords)
      {
        var record = new string[9];
        var str = memberRecord.Split('|');
        var memberId = int.Parse(str[0]);
        var memRecord = MemberRepository.Single(member => member.Id == memberId);
        if (memRecord != null)
        {
          record[0] = memRecord.MemberCodeAlpha + memRecord.MemberCodeNumeric + " " + memRecord.CommercialName;

        }
        var billingCategory = str[1].Split(',');
        for (int i = 0; i < billingCategory.Count(); i++)
        {
          if (bool.Parse(billingCategory[i]))
            record[i + 1] = "X";
          else
          {
            record[i + 1] = "-";
          }
        }
        formattedRecords.Add(record);
      }
      Logger.Info("GetFormattedRecord End.");
      return formattedRecords;
    }

 //   public Model.Calendar.BillingPeriod currentBillingPeriod { get; set; }
  }
}
