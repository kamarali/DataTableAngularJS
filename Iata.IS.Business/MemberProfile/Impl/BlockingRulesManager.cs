using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Castle.Core.Smtp;
using Iata.IS.Business.Common;
using Iata.IS.Business.Pax;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Impl;
using Iata.IS.Data.MemberProfile;
using Iata.IS.Data.MemberProfile.Impl;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using log4net;
using System.Reflection;
using Devart.Data.Oracle;
using System.Configuration;
using NVelocity;

namespace Iata.IS.Business.MemberProfile.Impl
{
  public class BlockingRulesManager : IBlockingRulesManager
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// 
    /// </summary>
    private readonly IReferenceManager _referenceManager;

    public IMemberRepository MemberRepository { get; set; }
   
    /// <summary>
    /// Gets or sets the blockingRules repository.
    /// </summary>
    /// <value>The blockingRules  repository.</value>
    //Replaced generic implementation of repository with actual implementation
    public IBlockingRulesRepository BlockingRulesRepository { get; set; }
    /// <summary>
    /// Gets or sets the block member repository.
    /// </summary>
    /// <value>The block member repository.</value>
    public IBlockMemberRepository BlockMemberRepository { get; set; }

    /// <summary>
    /// Gets or sets the blocking rule repository.
    /// </summary>
    /// <value>The blocking rule repository.</value>
    public IRepository<BlockGroup> BlockGroupRepository { get; set; }

    /// <summary>
    /// Gets or sets the block by group exception repository.
    /// </summary>
    /// <value>The block by group exception repository.</value>
    public IBlockGroupExceptionRepository BlockGroupExceptionRepository { get; set; }

    public IRepository<EmailTemplate> EmailSettingsRepository { get; set; }

    public ITemplatedTextGenerator TemplatedTextGenerator { get; set; }

    /// <summary>
    /// Creates blocking rule.
    /// </summary>
    /// <param name="blockingRule">Blocking rule object.</param>
    /// <returns>Blocking rule object </returns>
    public BlockingRule AddBlockingRule(BlockingRule blockingRule)
    {
      //CMP 560: Creation date time not set when new rule is added
      blockingRule.CreatedOn = DateTime.UtcNow;
      BlockingRulesRepository.Add(blockingRule);
      UnitOfWork.CommitDefault();

     // Send mail to IChOps about the blocking rule addition
      SendMailToIchForBlockingRuleUpdate(blockingRule.RuleName, blockingRule.MemberId);
      return blockingRule;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BlockingRulesManager"/> class.
    /// </summary>
    public BlockingRulesManager()
    {
      _referenceManager = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager));
    }


    public BlockingRule UpdateBlockingRule(BlockingRule blockingRule)
    {
      //Check whether blockingRules details exist already
      var blockingRuleRecordInDb = BlockingRulesRepository.Single(blockingRules => blockingRules.Id == blockingRule.Id);
      BlockingRule blockingRuleRecord = null;

      if (blockingRuleRecordInDb == null)
      {
          if (BlockingRulesRepository.Get(blockingRules => blockingRules.RuleName.ToUpper().Trim() == blockingRule.RuleName.ToUpper().Trim() && blockingRules.ClearingHouse.ToUpper() == blockingRule.ClearingHouse.ToUpper().Trim() && blockingRules.MemberId == blockingRule.MemberId && blockingRules.IsDeleted == false).Count() != 0)
          throw new ISBusinessException(ErrorCodes.DuplicateBlockingRuleFound);
        blockingRuleRecord = AddBlockingRule(blockingRule);

      }
      else
      {
        //CMP 560: Creation date time not set when new rule is added
        blockingRule.CreatedOn = blockingRuleRecordInDb.CreatedOn;
        // Following condition is used to check whether user has changed RuleName.
        if (blockingRuleRecordInDb.RuleName != blockingRule.RuleName)
        {
          if (BlockingRulesRepository.Get(blockingRules => blockingRules.RuleName.ToUpper().Trim() == blockingRule.RuleName.ToUpper().Trim() && blockingRules.ClearingHouse.ToUpper() == blockingRule.ClearingHouse.ToUpper().Trim() && blockingRules.MemberId == blockingRule.MemberId && blockingRule.IsDeleted == false).Count() != 0)
            throw new ISBusinessException(ErrorCodes.DuplicateBlockingRuleFound);
        }// end if()
        else
        {
          // Check whether record exists for given details, else throw Exception
          if (BlockingRulesRepository.Get(blockingRules => blockingRules.RuleName.ToUpper().Trim() == blockingRule.RuleName.ToUpper().Trim() && blockingRules.ClearingHouse.ToUpper() == blockingRule.ClearingHouse.ToUpper().Trim() && blockingRules.MemberId == blockingRule.MemberId && blockingRule.IsDeleted == false ).Count() <= 0)
            throw new ISBusinessException(ErrorCodes.DuplicateBlockingRuleFound);
        }// end else

        // Update record in Database
        blockingRuleRecord = BlockingRulesRepository.Update(blockingRule);
        UnitOfWork.CommitDefault();

        // Sendmail to ICHOPS for the updation of blocking rule
        SendMailToIchForBlockingRuleUpdate(blockingRule.RuleName, blockingRule.MemberId);
      }
      
      return blockingRuleRecord;
    }

    /// <summary>
    /// Following method is used to generate BlockingRule update XML when blocking rule is updated
    /// </summary>
    /// <param name="blockingRuleId">Blocking rule Id</param>
    /// <returns>Blocking rule update Xml string</returns>
    public void GenerateBlockingRuleUpdateXml(int blockingRuleId)
    {
        // Call InsertMessageInOracleQueue() method which will insert record in Oracle message Queue.
        InsertMessageInOracleQueue("BlockingRuleUpdate", blockingRuleId);

       //BlockingRuleUpdateHandler blockingRuleHandler = new BlockingRuleUpdateHandler();
       // blockingRuleHandler.GenerateXMLForBlockingRuleUpdates(blockingRuleId); 
    }// end GenerateBlockingRuleUpdateXml()

    private void InsertMessageInOracleQueue(string messageType, int blockingRuleId)
    {
        // Create new Oracle database Connection
        //var oracleConnection = new OracleConnection(ConfigurationManager.AppSettings["connectionString"].Trim());
      var memberManager = Ioc.Resolve<MemberManager>(typeof(IMemberManager));
        try
        {
            IDictionary<string, string> messages = new Dictionary<string, string> {
                                                                              { "MSG_TYPE", messageType },
                                                                              { "MSG_KEY", blockingRuleId.ToString() }
                                                                            };
            var queueHelper = new QueueHelper(ConfigurationManager.AppSettings["ProfileUpdateQueueName"].Trim());
            queueHelper.Enqueue(messages);
            // Open Oracle database connection
            //oracleConnection.Open();

            //// Instantiate OracleQueue passing it queue name
            //var batchFileMessageQueue = new OracleQueue(ConfigurationManager.AppSettings["ProfileUpdateQueueName"].Trim(), oracleConnection);

            //// Instantiate OracleQueueMessage
            //var oracleQueueMessage = new OracleQueueMessage();

            //// Instantiate OracleObject passing it MessageType
            //var obj = new OracleObject(ConfigurationManager.AppSettings["ProfileUpdateMessageType"], oracleConnection);

            //// Set Message type and Message Key
            //obj["MSG_TYPE"] = messageType;
            //obj["MSG_KEY"] = blockingRuleId;

            //// Following code sets payload of object type for object type queues
            //oracleQueueMessage.ObjectPayload = obj;

            //// Set Delivery mode for Oracle queue messages
            //oracleQueueMessage.MessageProperties.DeliveryMode = OracleQueueDeliveryMode.PersistentOrBuffered;

            //// Log message
            //Logger.InfoFormat("New message added in Batch File Upload Service Queue Queue for [{0}].", obj["MSG_TYPE"].ToString());

            //// Add message to specified Queue
            //batchFileMessageQueue.Enqueue(oracleQueueMessage);
        }// end try
        catch (Exception exception)
        {
            var blockMemberList = GetBlockingRuleDetails(blockingRuleId);
            Logger.Error("Error occurred while adding message to queue.", exception);

            memberManager.SendUnexpectedErrorNotificationToISAdmin("Blocking Rule Update/Delete", exception.Message, blockMemberList.MemberId);
        }// end catch

    }// end InsertMessageInOracleQueue()

    public BlockMember AddBlockMember(BlockMember blockMember)
    {
      BlockMemberRepository.Add(blockMember);
      UnitOfWork.CommitDefault();
      return blockMember;

    }

    public List<BlockMember> GetBlockMemberList(int blockingRuleId, bool isDebtor)
    {
      var blockMemberList = new List<BlockMember>();
      if (blockingRuleId != 0)
      {
        blockMemberList = BlockMemberRepository.Get(ml => ml.BlockingRuleId == blockingRuleId && ml.IsDebtors == isDebtor).ToList();
      }

      // Commented following code to return empty BlockMemberList while loading BlockRuleDetails page. i.e. when blockingRuleId = 0    
      /* if (blockingRuleId == 0)
      {
          blockMemberList = BlockMemberRepository.Get(ml => ml.IsDebtors == isDebtor).ToList();
      } */

      return blockMemberList;
    }


    /// <summary>
    /// Gets the count debtor or creditor members for given block rule id.
    /// </summary>
    /// <param name="blockingRuleId">blocking rule id</param>
    /// <param name="isDebtor">Boolean flag to indicate that is debtor or creditor</param>
    /// <returns>Returns the count of members matching the given criteria.</returns>
    public long GetBlockMemberCount(int blockingRuleId, bool isDebtor)
    {
      return BlockMemberRepository.GetCount(ml => ml.BlockingRuleId == blockingRuleId && ml.IsDebtors == isDebtor);
    }

    public List<BlockingRule> GetBlockingRuleList(string memberId, string blockingRule, string description, string clearingHouse)
    {
      var blockingRuleList = new List<BlockingRule>();

      if (!string.IsNullOrEmpty(blockingRule)) blockingRule = blockingRule.ToLower();

      if (!string.IsNullOrEmpty(description)) description = description.ToLower();

      if (string.IsNullOrEmpty(memberId) && string.IsNullOrEmpty(blockingRule) && string.IsNullOrEmpty(description))
      {
        blockingRuleList = BlockingRulesRepository.Get(ml => ml.ClearingHouse == clearingHouse && ml.IsDeleted==false).ToList();
      }

      if ((!string.IsNullOrEmpty(memberId)) && (!string.IsNullOrEmpty(blockingRule)) && (!string.IsNullOrEmpty(description)))
      {
        var memberIds = int.Parse(memberId);
        blockingRuleList = BlockingRulesRepository.Get(ml => (ml.RuleName.ToLower().Contains(blockingRule)) && ml.MemberId == memberIds && (ml.Description.ToLower().Contains(description)) && ml.ClearingHouse == clearingHouse && ml.IsDeleted == false).ToList();
      }

      if ((!string.IsNullOrEmpty(memberId)) && (!string.IsNullOrEmpty(blockingRule)) && string.IsNullOrEmpty(description))
      {
        var memberIds = int.Parse(memberId);
        blockingRuleList = BlockingRulesRepository.Get(ml => (ml.RuleName.ToLower().Contains(blockingRule)) && ml.MemberId == memberIds && ml.ClearingHouse == clearingHouse && ml.IsDeleted == false).ToList();
      }

      if ((!string.IsNullOrEmpty(memberId)) && string.IsNullOrEmpty(blockingRule) && string.IsNullOrEmpty(description))
      {
        var memberIds = int.Parse(memberId);
        blockingRuleList = BlockingRulesRepository.Get(ml => ml.MemberId == memberIds && ml.ClearingHouse == clearingHouse && ml.IsDeleted == false).ToList();
      }

      if (string.IsNullOrEmpty(memberId) && (!string.IsNullOrEmpty(blockingRule)) && (!string.IsNullOrEmpty(description)))
      {
        blockingRuleList = BlockingRulesRepository.Get(ml => (ml.RuleName.ToLower().Contains(blockingRule)) && (ml.Description.ToLower().Contains(description)) && ml.ClearingHouse == clearingHouse && ml.IsDeleted == false).ToList();
      }

      if (string.IsNullOrEmpty(memberId) && (!string.IsNullOrEmpty(blockingRule)) && string.IsNullOrEmpty(description))
      {
        blockingRuleList = BlockingRulesRepository.Get(ml => (ml.RuleName.ToLower().Contains(blockingRule)) && (ml.ClearingHouse == clearingHouse && ml.IsDeleted == false)).ToList();
      }

      if ((!string.IsNullOrEmpty(memberId)) && string.IsNullOrEmpty(blockingRule) && (!string.IsNullOrEmpty(description)))
      {
        var memberIds = int.Parse(memberId);
        blockingRuleList = BlockingRulesRepository.Get(ml => ml.MemberId == memberIds && (ml.Description.ToLower().Contains(description)) && ml.ClearingHouse == clearingHouse && ml.IsDeleted == false).ToList();
      }

      if (string.IsNullOrEmpty(memberId) && string.IsNullOrEmpty(blockingRule) && (!string.IsNullOrEmpty(description)))
      {
          blockingRuleList = BlockingRulesRepository.Get(ml => (ml.Description.ToLower().Contains(@"" +description)) && ml.ClearingHouse == clearingHouse && ml.IsDeleted == false).ToList();
      }
      return blockingRuleList;
    }

    public BlockGroup AddBlockGroup(BlockGroup blockGroup)
    {
      BlockGroupRepository.Add(blockGroup);
      UnitOfWork.CommitDefault();
      return blockGroup;
    }

    public bool DeleteBlockingRule(int blockingRuleId)
    {
      var blockingRuleRecord = BlockingRulesRepository.Single(ml => ml.Id == blockingRuleId);
      if (blockingRuleRecord != null)
      {
          // Instantiate BlockingRuleUpdatehandler
          BlockingRuleUpdateHandler blockingRuleHandler = new BlockingRuleUpdateHandler();
          // Call GenerateXmlForBlockingRuleDelete() method which will generate blocking rule delete Xml 
          // and will delete the record from database
          // blockingRuleHandler.GenerateXmlForBlockingRuleDelete(blockingRuleId);

        //Set Is Deleted flag to true for deleted blocking rule if blocking rule belongs to ICh.The blocking rule will be deleted physically when the updates are sent to ICH
        //In case of ACH blocking rules the record will get deleted physically as soon as delete button is clicked
          if (blockingRuleRecord.ClearingHouse == "ICH")
          {
            blockingRuleRecord.IsDeleted = true;
            BlockingRulesRepository.Update(blockingRuleRecord);
            UnitOfWork.CommitDefault();
            // Call InsertMessageInOracleQueue() method which will insert blocking rule delete record in Oracle message queue
            InsertMessageInOracleQueue("BlockingRuleDelete", blockingRuleId);
           // blockingRuleHandler.GenerateXmlForBlockingRuleDelete(blockingRuleId);

            // send mail to ICHOPS aboout the delete of record.
            SendMailToIchForBlockingRuleDelete(blockingRuleRecord.RuleName, blockingRuleRecord.MemberId);

          }
          else if (blockingRuleRecord.ClearingHouse == "ACH")
          {
            BlockingRulesRepository.Delete(blockingRuleRecord);
            UnitOfWork.CommitDefault();
            
          }

          return true;
      }// end if()
      return false;
    }// end DeleteBlockingRule()

    public BlockingRule GetBlockingRuleDetails(int blockingRuleId)
    {
      var blockingRuleRecord = BlockingRulesRepository.Single(ml => ml.Id == blockingRuleId);
    
      return blockingRuleRecord;
    }

    public List<BlockGroup> GetBlockGroupList(int blockingRuleId)
    {
      var blockedGroupList = new List<BlockGroup>();

      // Commented below code which returns BlockGroup list when blockingRuleId = 0 while loading BlockRuleDetails page initially.  
      /* if (blockingRuleId == 0)
        blockedGroupList = BlockGroupRepository.GetAll().ToList();
      else */
      blockedGroupList = BlockGroupRepository.Get(ml => ml.BlockingRuleId == blockingRuleId).ToList();
      
      // DiplayText Retrieval from miscCodes and setting it to required property.
      foreach (var blockGroup in blockedGroupList)
      {
        blockGroup.DisplayZoneType = _referenceManager.GetIchZoneDisplayValue(blockGroup.ZoneTypeId);
      }
      
      return blockedGroupList;
    }
    
    /// <summary>
    /// Gets the count of blocked groups for given block rule id.
    /// </summary>
    /// <param name="blockingRuleId">blocking rule id</param>
    /// <returns>Returns the count of blocked groups matching the given criteria.</returns>
    public long GetBlockGroupCount(int blockingRuleId)
    {
      return BlockGroupRepository.GetCount(ml => ml.BlockingRuleId == blockingRuleId);
    }

    public bool DeleteBlockedGroup(int groupId)
    {
      var blockGroupRecord = BlockGroupRepository.Single(ml => ml.Id == groupId);
      if (blockGroupRecord != null)
      {
        BlockGroupRepository.Delete(blockGroupRecord);
        UnitOfWork.CommitDefault();
        return true;
      }
      return false;
    }

    public bool DeleteBlockedMember(int memberId)
    {
      var blockedMemberRecord = BlockMemberRepository.Single(ml => ml.Id == memberId);
      if (blockedMemberRecord != null)
      {
        BlockMemberRepository.Delete(blockedMemberRecord);
        UnitOfWork.CommitDefault();
        return true;
      }
      return false;
    }

    public List<BlockGroupException> GetBlockGroupExceptionsList(int groupId)
    {
      var blockGroupExceptionsList = BlockGroupExceptionRepository.Get(ml => ml.BlockGroupId == groupId);
      return blockGroupExceptionsList.ToList();
    }

    public bool DeleteBlockedGroupException(int blockGroupId, int exceptionMemberId)
    {
      var blockGroupExceptionRecord = BlockGroupExceptionRepository.Single(ml => ml.ExceptionMemberId == exceptionMemberId && ml.BlockGroupId == blockGroupId);
      if (blockGroupExceptionRecord != null)
      {
        BlockGroupExceptionRepository.Delete(blockGroupExceptionRecord);
        UnitOfWork.CommitDefault();
        return true;
      }
      return false;
    }

    public BlockGroupException AddBlockGroupException(BlockGroupException blockGroupException)
    {
      BlockGroupExceptionRepository.Add(blockGroupException);
      UnitOfWork.CommitDefault();
      return blockGroupException;
    }

    /// <summary>
    /// Following method is used to update Block member in database
    /// </summary>
    /// <param name="blockMember">Block member details</param>
    public void UpdateBlockMember(BlockMember blockMember)
    {
      // Retrieve BlockMember to updated from database
      var blockedMemberRecord = BlockMemberRepository.Single(ml => ml.Id == blockMember.MemberId);

      // If Block member exists set Pax, Cargo, Misc and Uatp values 
      if (blockedMemberRecord != null)
      {
        blockedMemberRecord.Pax = blockMember.Pax;
        blockedMemberRecord.Cargo = blockMember.Cargo;
        blockedMemberRecord.Misc = blockMember.Misc;
        blockedMemberRecord.Uatp = blockMember.Uatp;

        // Update repository
        BlockMemberRepository.Update(blockedMemberRecord);
        // Commit
        UnitOfWork.CommitDefault();
      }// end if()
    }// end UpdateBlockMember()

    /// <summary>
    /// Following method is used to update Block Group in database
    /// </summary>
    /// <param name="blockMember">Block Group details</param>
    public void UpdateBlockGroup(BlockGroup blockGroup)
    {
      // Retrieve BlockGroup to updated from database
      var blockGroupRecord = BlockGroupRepository.Single(ml => ml.Id == blockGroup.Id);

      // If Block Group exists set Pax, Cargo, Misc and Uatp values 
      if (blockGroupRecord != null)
      {
        blockGroupRecord.Pax = blockGroup.Pax;
        blockGroupRecord.Cargo = blockGroup.Cargo;
        blockGroupRecord.Misc = blockGroup.Misc;
        blockGroupRecord.Uatp = blockGroup.Uatp;

        // Update BlockGroup repository
        BlockGroupRepository.Update(blockGroupRecord);
        // Commit
        UnitOfWork.CommitDefault();
      }// end if()
    }// end UpdateBlockMember()

    /// <summary>
    /// To get the memberlist of blocked creditor/Debitor.
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="isDebtor"></param>
    /// <param name="isPax"></param>
    /// <param name="isMisc"></param>
    /// <param name="isUatp"></param>
    /// <param name="isCargo"></param>
    /// <returns></returns>
    public List<BlockMember> GetBlockedMemberList(int memberId, bool isDebtor,bool isPax ,bool isMisc ,bool isUatp ,bool isCargo )
    {
        var blockMemberList = new List<BlockMember>();
        if (memberId != 0)
        {  
            if(isPax)
             blockMemberList = BlockMemberRepository.Get(ml => ml.MemberId == memberId && ml.IsDebtors == isDebtor && ml.Pax == isPax).ToList();
            if(isMisc)
              blockMemberList = BlockMemberRepository.Get(ml => ml.MemberId == memberId && ml.IsDebtors == isDebtor && ml.Misc == isMisc).ToList();
            if(isUatp)
              blockMemberList = BlockMemberRepository.Get(ml => ml.MemberId == memberId && ml.IsDebtors == isDebtor && ml.Uatp == isUatp).ToList();
        }
        return blockMemberList;
    }

    /// <summary>
    /// To check wheather member is blocked or not.
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="blockedMemberList"></param>
    /// <returns></returns>
    public bool IsMemeberBlocked(int memberId, List<BlockMember> blockedMemberList)
    {
        var isBlocked = false;
        foreach (var blockMember in
            blockedMemberList.Where(blockMember => BlockingRulesRepository.Get(rules => rules.Id == blockMember.BlockingRuleId && rules.MemberId == memberId && rules.IsDeleted == false).ToList().Count() > 0))
        {
          isBlocked = true;
        }       
        return isBlocked;
    }

    /// <summary>
    /// To get blocking rules by meberId
    /// </summary>
    /// <param name="memberId"></param>
    /// <returns></returns>
    public List<BlockingRule> GetBlockingRuleDetailsByMemberId(int memberId)
    {
      var blockingRuleList = new List<BlockingRule>();
      blockingRuleList = BlockingRulesRepository.GetBlockingRuleWithGroup(ml => ml.MemberId == memberId && ml.IsDeleted == false).ToList();
      return blockingRuleList;
    }

    private bool SendMailToIchForBlockingRuleUpdate(string blockingruleName, int memberId)
    {
        if (ConfigurationManager.AppSettings["EmailNotification"] == "true")
        {
            try
            {
                // Get an object of the EmailSender component
                var memberPrefix = string.Empty;
                var memberDesignator = string.Empty;
                var emailSender = Ioc.Resolve<IEmailSender>(typeof (IEmailSender));
                var memberData = MemberRepository.Single(mem => mem.Id == memberId);

                Logger.Info("Getting Member data for sending email");

                if (memberData != null)
                {
                    memberPrefix = memberData.MemberCodeNumeric;
                    memberDesignator = memberData.MemberCodeAlpha;
                }
                if (memberId == 0)
                {
                    memberPrefix = "ICH";
                    memberDesignator = "Settlement";
                }

                Logger.Info(string.Format("EmailSender instance is: [{0}]", emailSender != null ? "NOT NULL" : "NULL"));

                // Object of the nVelocity data dictionary
                var context = new VelocityContext();
                context.Put("BlockingRuleName", blockingruleName);
                context.Put("MemberName", memberPrefix + "-" + memberDesignator);
                context.Put("MemberId", memberId);


                Logger.Info(string.Format("EmailSettingsRepository instance is: [{0}]",
                                          EmailSettingsRepository != null ? "NOT NULL" : "NULL"));


                var emailSettingForISAdminAlert =
                    EmailSettingsRepository.Get(
                        esfopu => esfopu.Id == (int) EmailTemplateId.ICHBlockingRuleUpdateNotification);

                // Generate email body text for own profile updates contact type mail
                Logger.Info(string.Format("TemplatedTextGenerator instance is: [{0}]",
                                          TemplatedTextGenerator != null ? "NOT NULL" : "NULL"));
                var emailToISAdminText =
                    TemplatedTextGenerator.GenerateTemplatedText(EmailTemplateId.ICHBlockingRuleUpdateNotification,
                                                                 context);

                // Create a mail object to send mail
                var msgForISAdminAlert = new MailMessage
                                             {
                                                 From =
                                                     new MailAddress(
                                                     emailSettingForISAdminAlert.SingleOrDefault().FromEmailAddress),
                                                 IsBodyHtml = true
                                             };

                var subject = emailSettingForISAdminAlert.SingleOrDefault().Subject;
                msgForISAdminAlert.Subject = subject;
                msgForISAdminAlert.Subject = subject.Replace("$MemberName$", memberPrefix + "-" + memberDesignator);

                // Loop through the contacts list and add them to To list of mail to be sent
                if (!string.IsNullOrEmpty(AdminSystem.SystemParameters.Instance.ICHDetails.IchOpsEmail))
                {
                    var emailAddressList = AdminSystem.SystemParameters.Instance.ICHDetails.IchOpsEmail;
                    // If (emailAddressList.Contains(','))
                    var formatedEmailList = emailAddressList.Replace(',', ';');
                    var mailAdressList = ConvertUtil.ConvertToMailAddresses(formatedEmailList);

                    foreach (var mailaddr in mailAdressList)
                    {
                        msgForISAdminAlert.To.Add(mailaddr);
                    }
                }

                // Set body text of mail
                msgForISAdminAlert.Body = emailToISAdminText;
                // Send the mail
                emailSender.Send(msgForISAdminAlert);

                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(
                    "Error occurred while sending alert to ICH Admin for BlockinRuleUpdate/Add/Delete Notification",
                    exception);
                return false;
            }
        }
        return false;
    }


    private bool SendMailToIchForBlockingRuleDelete(string blockingruleName, int memberId)
    {
        if (ConfigurationManager.AppSettings["EmailNotification"] == "true")
        {
            try
            {
                // Get an object of the EmailSender component
                var memberPrefix = string.Empty;
                var memberDesignator = string.Empty;
                var emailSender = Ioc.Resolve<IEmailSender>(typeof (IEmailSender));
                var memberData = MemberRepository.Single(mem => mem.Id == memberId);

                Logger.Info("Getting Member data for sending email");

                if (memberData != null)
                {
                    memberPrefix = memberData.MemberCodeNumeric;
                    memberDesignator = memberData.MemberCodeAlpha;
                }
                if (memberId == 0)
                {
                    memberPrefix = "ICH";
                    memberDesignator = "Settlement";
                }

                Logger.Info(string.Format("EmailSender instance is: [{0}]", emailSender != null ? "NOT NULL" : "NULL"));

                // Object of the nVelocity data dictionary
                var context = new VelocityContext();
                context.Put("BlockingRuleName", blockingruleName);
                context.Put("MemberName", memberPrefix + "-" + memberDesignator);
                context.Put("MemberId", memberId);


                Logger.Info(string.Format("EmailSettingsRepository instance is: [{0}]",
                                          EmailSettingsRepository != null ? "NOT NULL" : "NULL"));


                var emailSettingForISAdminAlert =
                    EmailSettingsRepository.Get(
                        esfopu => esfopu.Id == (int) EmailTemplateId.ICHBlockingRuleDeleteNotification);

                // Generate email body text for own profile updates contact type mail
                Logger.Info(string.Format("TemplatedTextGenerator instance is: [{0}]",
                                          TemplatedTextGenerator != null ? "NOT NULL" : "NULL"));
                var emailToISAdminText =
                    TemplatedTextGenerator.GenerateTemplatedText(EmailTemplateId.ICHBlockingRuleDeleteNotification,
                                                                 context);

                // Create a mail object to send mail
                var msgForISAdminAlert = new MailMessage
                                             {
                                                 From =
                                                     new MailAddress(
                                                     emailSettingForISAdminAlert.SingleOrDefault().FromEmailAddress),
                                                 IsBodyHtml = true
                                             };

                var subject = emailSettingForISAdminAlert.SingleOrDefault().Subject;
                msgForISAdminAlert.Subject = subject;
                msgForISAdminAlert.Subject = subject.Replace("$MemberName$", memberPrefix + "-" + memberDesignator);

                // Loop through the contacts list and add them to To list of mail to be sent
                if (!string.IsNullOrEmpty(AdminSystem.SystemParameters.Instance.ICHDetails.IchOpsEmail))
                {
                    var emailAddressList = AdminSystem.SystemParameters.Instance.ICHDetails.IchOpsEmail;
                    // If (emailAddressList.Contains(','))
                    var formatedEmailList = emailAddressList.Replace(',', ';');
                    var mailAdressList = ConvertUtil.ConvertToMailAddresses(formatedEmailList);

                    foreach (var mailaddr in mailAdressList)
                    {
                        msgForISAdminAlert.To.Add(mailaddr);
                    }
                }

                // Set body text of mail
                msgForISAdminAlert.Body = emailToISAdminText;
                // Send the mail
                emailSender.Send(msgForISAdminAlert);

                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(
                    "Error occurred while sending alert to ICH Admin for BlockinRuleUpdate/Add/Delete Notification",
                    exception);
                return false;
            }
        }
        return false;
    }

    /// <summary>
    /// Get list of all active blocking rules data for given clearing house.
    /// </summary>
    /// <param name="clearingHouse">clearing house string. e.g. ICH/ACH.</param>
    /// <returns>List of all active blocking rules data for given clearing house.</returns>
    public List<DownloadBlockingRules> GetBlokingRulesForClearingHouse(string clearingHouse)
    {
      return BlockingRulesRepository.GetBlokingRulesForClearingHouse(clearingHouse).ToList();
    }// End GetBlokingRulesForClearingHouse()

  }
}
