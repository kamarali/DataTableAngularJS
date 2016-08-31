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
using Iata.IS.Data.MemberProfile;
using iPayables.UserManagement;
using log4net;
using NVelocity;
using UnitOfWork = Iata.IS.Data.Impl.UnitOfWork;

namespace Iata.IS.Business.MemberProfile.Impl
{
  public class FutureUpdatesManager : IFutureUpdatesManager
  {
    private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    private IFutureUpdatesRepository FutureUpdateRepository { get; set; }

    public IFutureUpdatesServiceRepository FutureUpdatesServiceRepository { get; set; }

    public FutureUpdatesManager(IFutureUpdatesRepository futureUpdateRepository)
    {
      FutureUpdateRepository = futureUpdateRepository;
    }

    /// <summary>
    /// Adds future update audit trail record for a member
    /// </summary>
    /// <param name="futureUpdates">Future Updates class object</param>
    /// <returns>Future Updates class object</returns>
    public FutureUpdates AddFuturepdates(FutureUpdates futureUpdates)
    {
      FutureUpdateRepository.Add(futureUpdates);
      UnitOfWork.CommitDefault();
      return futureUpdates;
    }

    /// <summary>
    /// Returns list of future updates audit trail records for passed search criteria
    /// </summary>
    /// <returns>List of Future Updates class object</returns>
    public List<FutureUpdates> GetFutureUpdatesList(int memberId, int elementGroupTypeId, string tableName, int? locationId, int? relationId)
    {
      IQueryable<FutureUpdates> futureUpdates = null;

      /*TODO: Tablename parameter will not be required once MST_ELEMENT_GROUP is mapped in edmx*/
      if (relationId == null)
      {
        futureUpdates =
          FutureUpdateRepository.Get(
            futureupdate =>
            futureupdate.MemberId == memberId && futureupdate.TableName == tableName &&
            futureupdate.IsChangeApplied == false);
      }
      else if (relationId > 0)
      {
        futureUpdates =
          FutureUpdateRepository.Get(
            futureupdate =>
            futureupdate.MemberId == memberId && futureupdate.TableName == tableName && futureupdate.RelationId == relationId &&
            futureupdate.IsChangeApplied == false);
      }
 
      if (futureUpdates != null)
      {
        return futureUpdates.ToList();
      }

      return null;
    }

    /// <summary>
    /// Gets future update audit trail records corresponding to an element group and member ID
    /// </summary>
    /// <param name="egType">Element group</param>
    /// <param name="memberId">ID of member</param>
    /// <param name="elementName">Element for which audit trail records should be fetched.If null then records for all elements should be fetched</param>
    /// <param name="relationId"></param>
    /// <returns>Future Updates class object</returns>
    public List<FutureUpdates> GetPendingFutureUpdates(ElementGroupType egType, int memberId, string elementName, int? relationId)
    {
      // get an instance of element group repository
      var elementGroupRepository = Ioc.Resolve<IRepository<ElementGroup>>(typeof(IRepository<ElementGroup>));

      var elementGroup = elementGroupRepository.Get(eg => eg.Id == (int)egType);
      var tableName = elementGroup.FirstOrDefault().TableName;

      List<FutureUpdates> futureUpdateData;

      _logger.Debug("Inside GetPendingFutureUpdates");

      if (relationId == null)
      {
        _logger.Debug("Inside relationid == null");
        futureUpdateData =
          FutureUpdateRepository.Get(
            futureUpdate => futureUpdate.MemberId == memberId && futureUpdate.ElementName == elementName && futureUpdate.TableName == tableName && futureUpdate.IsChangeApplied == false).ToList();
      }
      else
      {
        _logger.Debug("Inside relationid != null");
        futureUpdateData =
          FutureUpdateRepository.Get(
          futureUpdate => futureUpdate.MemberId == memberId && futureUpdate.ElementName == elementName && futureUpdate.TableName == tableName && futureUpdate.IsChangeApplied == false && futureUpdate.RelationId == relationId).ToList();
      }

      if (futureUpdateData.Count > 0)
      {
        _logger.Debug("Return GetPendingFutureUpdates");
        return futureUpdateData;
      }
      _logger.Debug("Return null");
      return null;
    }

    public List<FutureUpdates> GetPendingFutureUpdates(int memberId, ElementGroupType elementGroupType, int? relationId = null)
    {
      // Get an instance of element group repository.
      var elementGroupRepository = Ioc.Resolve<IRepository<ElementGroup>>(typeof(IRepository<ElementGroup>));

      // Retrieve the table name for the group.
      var elementGroup = elementGroupRepository.Get(eg => eg.Id == (int)elementGroupType);
      var tableName = elementGroup.FirstOrDefault().TableName;

      List<FutureUpdates> pendingFutureUpdates;
      // Get the pending updates for the specified member for the given table name.
      //SCP325349 - Unable to change member legal name(Get pending future update based on relation id).
      if (relationId != null)
        pendingFutureUpdates = FutureUpdateRepository.Get(futureUpdate => futureUpdate.MemberId == memberId && futureUpdate.RelationId == relationId && futureUpdate.TableName == tableName && futureUpdate.IsChangeApplied == false).ToList();
      else
        pendingFutureUpdates = FutureUpdateRepository.Get(futureUpdate => futureUpdate.MemberId == memberId && futureUpdate.TableName == tableName && futureUpdate.IsChangeApplied == false).ToList();

      // Returned the pending future updates.
      return pendingFutureUpdates.Count > 0 ? pendingFutureUpdates : null;
    }

    /// <summary>
    /// Gets future update member list based on newvalue and element name specified
    /// </summary>
    /// <param name="egType">Element group</param>
    /// <param name="memberId"></param>
    /// <param name="elementName"></param>
    /// <param name="newValue"></param>
    /// <param name="relationId"></param>
    /// <param name="oldValue">this variable to get deleted member list</param>
    /// <returns>Future Updates class object</returns>
    public List<FutureUpdates> GetPendingFutureUpdates(ElementGroupType egType, int memberId, string elementName, string newValue, int? relationId, string oldValue= null)
    {
      List<FutureUpdates> futureUpdateData = null;

      //get an instance of element group repository
      var elementGroupRepository = Ioc.Resolve<IRepository<ElementGroup>>(typeof(IRepository<ElementGroup>));

      var elementGroup = elementGroupRepository.Get(eg => eg.Id == (int)egType);
      var tableName = elementGroup.FirstOrDefault().TableName;

      try
      {
         // get deleted member list.
          if (!string.IsNullOrEmpty(oldValue) && (relationId == null) && (memberId == 0))
          {
              futureUpdateData =
              FutureUpdateRepository.Get(
                futureUpdate => futureUpdate.NewVAlue == null
                  && futureUpdate.OldVAlue == oldValue
                  && futureUpdate.ElementName == elementName
                  && futureUpdate.TableName == tableName
                  && futureUpdate.IsChangeApplied == false).ToList();
          }
          else if ((relationId == null) && (memberId == 0))
          {
          if (newValue == null)
            futureUpdateData =
              FutureUpdateRepository.Get(
                futureUpdate => futureUpdate.NewVAlue == null
                  && futureUpdate.ElementName == elementName
                  && futureUpdate.TableName == tableName
                  && futureUpdate.IsChangeApplied == false).ToList();
          else
            futureUpdateData =
              FutureUpdateRepository.Get(
                futureUpdate => futureUpdate.NewVAlue == newValue
                  && futureUpdate.ElementName == elementName
                  && futureUpdate.TableName == tableName
                  && futureUpdate.IsChangeApplied == false).ToList();
         
        
        }
        else if ((relationId == null) && (memberId > 0))
        {
          if (newValue == null)
            futureUpdateData =
              FutureUpdateRepository.Get(
                futureUpdate => futureUpdate.NewVAlue == null
                  && futureUpdate.ElementName == elementName
                  && futureUpdate.TableName == tableName
                  && futureUpdate.IsChangeApplied == false
                  && futureUpdate.MemberId == memberId).ToList();
          else
            futureUpdateData =
            FutureUpdateRepository.Get(
              futureUpdate => futureUpdate.NewVAlue == newValue
                && futureUpdate.ElementName == elementName
                && futureUpdate.TableName == tableName
                && futureUpdate.IsChangeApplied == false
                && futureUpdate.MemberId == memberId).ToList();
        }
        else if ((relationId != null) && (memberId == 0))
        {
          if (newValue == null)
            futureUpdateData = FutureUpdateRepository.Get(futureUpdate =>
              futureUpdate.NewVAlue == null && futureUpdate.ElementName == elementName
              && futureUpdate.TableName == tableName && futureUpdate.IsChangeApplied == false
              && futureUpdate.RelationId == relationId).ToList();
          else
            futureUpdateData = FutureUpdateRepository.Get(futureUpdate =>
              futureUpdate.NewVAlue == newValue && futureUpdate.ElementName == elementName
              && futureUpdate.TableName == tableName && futureUpdate.IsChangeApplied == false
              && futureUpdate.RelationId == relationId).ToList();
        }
        else if ((relationId != null) && (memberId > 0))
        {
          if (newValue == null)
            futureUpdateData = FutureUpdateRepository.Get(futureUpdate =>
              futureUpdate.NewVAlue == null && futureUpdate.ElementName == elementName
              && futureUpdate.TableName == tableName && futureUpdate.IsChangeApplied == false
              && futureUpdate.RelationId == relationId
              && futureUpdate.MemberId == memberId).ToList();
          else
            futureUpdateData = FutureUpdateRepository.Get(futureUpdate =>
              futureUpdate.NewVAlue == newValue && futureUpdate.ElementName == elementName
              && futureUpdate.TableName == tableName && futureUpdate.IsChangeApplied == false
              && futureUpdate.RelationId == relationId
              && futureUpdate.MemberId == memberId).ToList();
        }
      }
      catch (Exception)
      {
        return null;
      }
      return futureUpdateData;
    }
      
    public List<FutureUpdateDetails> GetFutureUpdatesList(string elementList, DateTime? fromDate, DateTime? todate, int userId, int userType, int isDateOrPeriodSearch, int memberId, string reportType)
    {
      return FutureUpdateRepository.GetAuditTrialList(fromDate, todate, userId, elementList, userType,
                                                      isDateOrPeriodSearch, memberId, reportType);
    }

    public List<FutureUpdates> GetFutureUpdatesList(string elementList, string fromDate, string todate)
    {
      var futureUpdatesList = new List<FutureUpdates>();
      string[] elements = elementList.Split('!');
      foreach (var element in elements)
      {
        var frmDate = DateTime.Parse(fromDate);
        var tDate = DateTime.Parse(todate);
        tDate = tDate.AddDays(1);
        if (!elements[0].Equals(element))
        {
          string[] elementsplit = element.Split('|');
          if (elementsplit[1].Equals("true"))
          {
            var elementGroupId = int.Parse(elementsplit[0]);
            var futureUpdatesRecords = FutureUpdateRepository.Get(futureupdate => futureupdate.ElementGroupTypeId == elementGroupId && futureupdate.LastUpdatedOn >= frmDate && futureupdate.LastUpdatedOn < tDate);
            futureUpdatesList.AddRange(futureUpdatesRecords);
          }

        }
      }
      return futureUpdatesList;
    }

    public List<FutureUpdateTemp> GetFutureUpdatesDoneByService(DateTime ichBillingPeriod, DateTime achBillingPeriod)
    {
      List<FutureUpdateTemp> listOfFutureUpdatesDoneByService = FutureUpdatesServiceRepository.GetFutureUpdatesDoneByService(ichBillingPeriod, achBillingPeriod);
      return listOfFutureUpdatesDoneByService;
    }

    public String GetRelationIdDisplayName(int futureUpdateId)
    {
      return FutureUpdatesServiceRepository.GetRelationIdDisplayName(futureUpdateId);
    }

    /// <summary>
    /// Get user list for given user category and member id.
    /// </summary>
    /// <param name="userCategoryId">user category id</param>
    /// <param name="memberId"> member id</param>
    /// <returns>list of audit trail users corresponding to given user category id and member id.</returns>
    public List<AuditTrailUserDetails> GetAuditTrailUserList(int userCategoryId, int memberId)
    {
      return FutureUpdateRepository.GetAuditTrailUserList(userCategoryId, memberId);
    }

    /// <summary>
    /// This method will be used for applying the future updates applicable for current billing period or current date.
    /// </summary>
    public void ApplyFutureUpdates()
    {
      ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
      try
      {
        Ioc.Initialize();
        logger.Info("Future Update service trigerred.");

        var calendarManager = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager));
        logger.Info(string.Format("calendarMgr instance is: [{0}]", calendarManager != null ? "NOT NULL" : "NULL"));

        //SCP427407: KALE- Location activation future update
        // Retrieve current open billing periods or Last Closed Billing Period for both clearing houses
        var currentIchBillingPeriod = calendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
        var currentAchBillingPeriod = calendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ach);

        var futureUpdateManager = Ioc.Resolve<IFutureUpdatesManager>(typeof(IFutureUpdatesManager));
        logger.Info(string.Format("futureUpdateManager instance is: [{0}]", futureUpdateManager != null ? "NOT NULL" : "NULL"));
        
        // Call the sp to perform member profile future updates and get the list of future updates done by the service in this run/session 
        logger.Info(string.Format("ICH Year: [{0}]", currentIchBillingPeriod.Year > 0 ? currentIchBillingPeriod.Year : 0));
        logger.Info(string.Format("ICH Month: [{0}]", currentIchBillingPeriod.Month > 0 ? currentIchBillingPeriod.Month : 0));
        logger.Info(string.Format("ICH Period: [{0}]", currentIchBillingPeriod.Period > 0 ? currentIchBillingPeriod.Period : 0));

        logger.Info(string.Format("ACH Year: [{0}]", currentAchBillingPeriod.Year > 0 ? currentAchBillingPeriod.Year : 0));
        logger.Info(string.Format("ACH Month: [{0}]", currentAchBillingPeriod.Month > 0 ? currentAchBillingPeriod.Month : 0));
        logger.Info(string.Format("ACH Period: [{0}]", currentAchBillingPeriod.Period > 0 ? currentAchBillingPeriod.Period : 0));

        var futureUpdatesDoneList = futureUpdateManager.GetFutureUpdatesDoneByService(new DateTime(currentIchBillingPeriod.Year, currentIchBillingPeriod.Month, currentIchBillingPeriod.Period), new DateTime(currentAchBillingPeriod.Year, currentAchBillingPeriod.Month, currentAchBillingPeriod.Period));
        logger.InfoFormat("Future Update service SP executed. [{0}] updates applied.", futureUpdatesDoneList.Count);

        // Check if no future updates were performed by the service in current session
        if (futureUpdatesDoneList.Count == 0) return;

        // Declare an object of the nVelocity data dictionary
        VelocityContext contextChanger;
        VelocityContext htmlContentContextChanger;
        VelocityContext contextContacts;
        VelocityContext htmlContentContextContacts;

        var memberRepository = Ioc.Resolve<IMemberRepository>();

        // Get an object of the EmailSender component
        var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender)); ;
        logger.Info(string.Format("emailSender instance is: [{0}]", emailSender != null ? "NOT NULL" : "NULL"));
        
        // Get an object of the TemplatedTextGenerator that is used to generate body text of email from a nvelocity template
        var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));
        logger.Info(string.Format("templatedTextGenerator instance is: [{0}]", templatedTextGenerator != null ? "NOT NULL" : "NULL"));
        
        // Get an object of the member manager
        var memberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));

        // Get an instance of email settings  repository
        var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));
        logger.Info(string.Format("templatedTextGenerator instance is: [{0}]", templatedTextGenerator != null ? "NOT NULL" : "NULL"));
        
        // Get the user Object (this will provide us email address of member profile updating users)
        var iSUser = Ioc.Resolve<I_ISUser>(typeof(I_ISUser));
        logger.Info(string.Format("iSUser instance is: [{0}]", iSUser != null ? "NOT NULL" : "NULL"));
        
        var memberLocationUpdateHandler = Ioc.Resolve<IMemberLocationUpdateHandler>(typeof(IMemberLocationUpdateHandler));
        logger.Info(string.Format("memberLocationUpdateHandler instance is: [{0}]", memberLocationUpdateHandler != null ? "NOT NULL" : "NULL"));
        
        // Get the eMail settings for member profile future update mails for own member update contact type
      
        // Get the distinct list of members for which updates were done by the service
        var distinctMemberIds = (from fu in futureUpdatesDoneList
                                 select fu.MemberId).Distinct();



        // SCP186215: Member Code Mismatch between Member and Location Details
        // Email alerts to all Members for change of Reference Data should not be sent when the Member’s IS Membership Status is Pending
        var nonPendeingDistinctMemberIds = memberRepository.GetAll().Where(mem => distinctMemberIds.Contains(mem.Id)
                                                                               && mem.IsMembershipStatusId != (int) MemberStatus.Pending).Select(mem => mem.Id).ToList();

        //SCP262240: IS Membership Status not updating
        foreach (var memberId in distinctMemberIds)
        {
          // Remove the member from the cache.
          memberRepository.Invalidate(memberId);
          logger.InfoFormat("Remove member from cache for member id:[{0}]", memberId);
        }

        // Now loop through the distinct member ids)
        foreach (var memId in nonPendeingDistinctMemberIds)
        {
          // Now get the future updates for this particular member
          var memFutureUpdates = from mfu in futureUpdatesDoneList
                                 where mfu.MemberId == memId
                                 select mfu;

          // Send mails to own profile Update contact type contacts

          // Get email settings corresponding to mail which should be sent to own profile updates contact
          var emailSettingForOwnProfileUpdate = emailSettingsRepository.Get(esfopu => esfopu.Id == (int)EmailTemplateId.FutureUpdatesEffectToContacts);
          var emailSettingForOwnProfileUpdateHtml = emailSettingsRepository.Get(esfopu => esfopu.Id == (int)EmailTemplateId.FutureUpdatesEffectToContactsHtmlContents);

          // Get list of contacts of contact type own profile updates for this member
          var ownProfileUpdateContactList = memberManager.GetContactsForContactType(memId, ProcessingContactType.OwnProfileUpdates);

          // Contacts of type Own Profile Update are found
          if ((ownProfileUpdateContactList != null && ownProfileUpdateContactList.Count > 0) && (memFutureUpdates.Count() > 0))
          {
            // Object of the nVelocity data dictionary
            contextContacts = new VelocityContext();
            htmlContentContextContacts = new VelocityContext();

            // Get member details for member represented by UpdateList
            var memberCommercialName = string.Empty;
            var memberCodePrefix = string.Empty;
            var memberCodeDesignator = string.Empty;
            var commonFutureUpdateProperties = memFutureUpdates.First();

            if (commonFutureUpdateProperties.MemberId > 0)
            {
              memberCommercialName = memberManager.GetMemberCommercialName(commonFutureUpdateProperties.MemberId);
              memberCodePrefix = memberManager.GetMemberCode(commonFutureUpdateProperties.MemberId);
              memberCodeDesignator = memberManager.GetMemberCodeAlpha(commonFutureUpdateProperties.MemberId);
            }

            // Fill nVelocity data dictionary with data specific to template used for own profile updates contact type mail
            htmlContentContextContacts.Put("FutureUpdates", memFutureUpdates);
            contextContacts.Put("MemberCommercialName", memberCommercialName);
            contextContacts.Put("MemberPrefix", memberCodePrefix);
            contextContacts.Put("MemberDesignator", memberCodeDesignator);
            contextContacts.Put("EffectiveOn", DateTime.UtcNow.ToString("dd-MMM-yy"));
            contextContacts.Put("SISOpsEmail", ConfigurationManager.AppSettings["SisOpsEmail"]);
            contextContacts.Put("n", "\n");

            // Generate email body text for own profile updates contact type mail
            var emailToOwnProfileUpdateContactsText = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.FutureUpdatesEffectToContacts, contextContacts);
            var emailContentforAttachment = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.FutureUpdatesEffectToContactsHtmlContents, htmlContentContextContacts);

            // Create a mail object to send mail
            var msgForOwnprofileUpdate = new MailMessage { From = new MailAddress(emailSettingForOwnProfileUpdate.SingleOrDefault().FromEmailAddress) };
            msgForOwnprofileUpdate.IsBodyHtml = false;
            
            // Loop through the contacts list and add them to To list of mail to be sent
            foreach (var contact in ownProfileUpdateContactList)
            {
              msgForOwnprofileUpdate.To.Add(new MailAddress(contact.EmailAddress));
            }

            // Set subject of mail (replace special field placeholders with values)
            msgForOwnprofileUpdate.Subject = emailSettingForOwnProfileUpdate.SingleOrDefault().Subject;

            // Set body text of mail
            msgForOwnprofileUpdate.Body = emailToOwnProfileUpdateContactsText;

            var attachmentPath = Path.Combine(Path.GetTempPath(), memberCodePrefix + " - List of Post Dated Member profile changes made effective.htm");
            var contactAttachmentFileStream = new StreamWriter(attachmentPath, false);

            contactAttachmentFileStream.WriteLine(emailContentforAttachment);
            contactAttachmentFileStream.Close();
            msgForOwnprofileUpdate.Attachments.Add(new Attachment(attachmentPath));
            
            // Send the mail
            emailSender.Send(msgForOwnprofileUpdate);
          }

          memberLocationUpdateHandler.LocationUpdateSenderForFutureUpdates(memId, futureUpdatesDoneList);

          logger.InfoFormat("Future Update service processing completed for member id [{0}]", memId);
        }


        // Get the distinct list of users who have updated member profiles to send mails to them that their changes are effective now
        var distinctUserIds = (from dufu in futureUpdatesDoneList
                               select dufu.LastUpdatedBy).Distinct();

        // Get the eMail settings for member profile future update mail, to be sent to user updating the data
        var emailSettingsForChanger = emailSettingsRepository.Get(esfc => esfc.Id == (int)EmailTemplateId.FutureUpdatesEffectToChanger);
        var emailContentforAttachmentChanger = emailSettingsRepository.Get(esfc => esfc.Id == (int)EmailTemplateId.FutureUpdatesEffectToChangerHtmlContents);

        // Now loop through the distinct user ids
        foreach (var userId in distinctUserIds)
        {
          // Now get the future updates for this particular user
          var userFutureUpdates = from ufu in futureUpdatesDoneList
                                  where ufu.LastUpdatedBy == userId
                                  select ufu;

          // Create nVelocity data dictionary
          contextChanger = new VelocityContext();
          htmlContentContextChanger = new VelocityContext();

          // Fill nVelocity data dictionary with data specific to template, used for sending mail to user who had updated member profile data
          htmlContentContextChanger.Put("FutureUpdates", userFutureUpdates);
          contextChanger.Put("EffectiveOn", DateTime.UtcNow.ToString("dd-MMM-yy"));
          contextChanger.Put("SISOpsEmail", AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);
          contextChanger.Put("n", "\n");
          
          // Generate email body text for mail for user who had updated member profile data
          var emailToUserText = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.FutureUpdatesEffectToChanger, contextChanger);
          var emailToUserAttachment = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.FutureUpdatesEffectToChangerHtmlContents, htmlContentContextChanger);

          // Initialize IS_User object. This object will give us the email id of the user who had updated member profile data
          iSUser.UserID = userId;

          // Create a mail object to send mail to user who had updated member profile data
          var msgForChanger = new MailMessage
                                {
                                  From = new MailAddress(emailSettingsForChanger.SingleOrDefault().FromEmailAddress),
                                  IsBodyHtml = false,
                                  Subject = emailSettingsForChanger.SingleOrDefault().Subject,
                                  Body = emailToUserText
                                };

          // Add email of user to 'To' list of mail to be sent
          msgForChanger.To.Add(new MailAddress(iSUser.Email));

          var attachmentPath = Path.Combine(Path.GetTempPath(), "List of Post Dated Member profile changes made effective.htm");
          
          // Set body text of mail
          var contactAttachmentFileStream = new StreamWriter(attachmentPath, false);

          contactAttachmentFileStream.WriteLine(emailToUserAttachment);
          contactAttachmentFileStream.Close();
          msgForChanger.Attachments.Add(new Attachment(attachmentPath));

          // Send the mail to the user who had updated member profile data
          emailSender.Send(msgForChanger);
        }
      }
      catch (Exception exception)
      {
        logger.Error("Error occurred in Future Update Service.", exception);
      }
    }

    /// <summary>
    /// Inserts the member future updates message in oracle queue.
    /// </summary>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingPeriod">The billing period.</param>
    public void InsertMemberFutureUpdatesMessageInOracleQueue(int billingYear, int billingMonth, int billingPeriod)
    {
      // en-queue message
      IDictionary<string, string> messages = new Dictionary<string, string> {
                                                                              { "BILLING_YEAR", billingYear.ToString() },
                                                                              { "BILLING_MONTH", billingMonth.ToString() },
                                                                              { "BILLING_PERIOD", billingPeriod.ToString() }
                                                                            };
      var queueHelper = new QueueHelper(ConfigurationManager.AppSettings["MemberFutureUpdatesJobQueueName"].Trim());
      queueHelper.Enqueue(messages);
    }
  }
}
