using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Xml;
using Castle.Core.Smtp;
using Iata.IS.AdminSystem;
using Iata.IS.Business.BroadcastMessages;
using Iata.IS.Business.Common;
using Iata.IS.Business.Common.Impl;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.Pax;
using Iata.IS.Business.Pax.Impl;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Core;
using Iata.IS.Core.Configuration;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Calendar;
using Iata.IS.Data.Impl;
using Iata.IS.Data.LateSubmission;
using Iata.IS.Data.MemberProfile;
using Iata.IS.Data.Pax;
using Iata.IS.Model.Base;
using Iata.IS.Model.BroadcastMessages;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.LateSubmission;
using Iata.IS.Model.LateSubmission.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.Reports;
using log4net;
using NVelocity;
using System.IO;

namespace Iata.IS.Business.LateSubmission.Impl
{
  public class ProcessLateSubmissionManager : IProcessLateSubmissionManager
  {
    // Logger instance.
    private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public IProcessLateSubmissionRepository ProcessLateSubmissionRepository { get; set; }
    public ICalendarManager _calenderManager { get; set; }
    public IMemberManager _membererManager { get; set; }
    public ICalendarRepository CalendarRepository { get; set; }
    public ICalendarManager CalendarManager { get; set; }
    public IInvoiceManager invoiveManager { get; set; }
    public IRepository<InvoiceBase> invoice { get; set; }
    public IBroadcastMessagesManager BroadcastMessages { get; set; }
    public IInvoiceRepository InvoiceRepository { get; set; }
    public InvoiceManagerBase InvoiceManagerBase { get; set; }
    public IRepository<MemberLocationInformation> MemberLocationRepo { get; set; }
    public ITemplatedTextGenerator TemplatedTextGenerator { get; set; }
    private readonly IReferenceManager _referenceManager;

    public ProcessLateSubmissionManager(IReferenceManager referenceManager)
    { _referenceManager = referenceManager;
    }
    public List<LateSubmissionMemberSummary> GetLateSubmittedInvoiceMemberSummary(string clearanceType)
    {
      //SCP 280475 - SRM: System Performance - ICH late submisson tab
      //CMP#624 : TFS#9311
      var billingPeriod = _calenderManager.GetLastClosedBillingPeriod(ClearingHouse.Ich);

      List<LateSubmissionMemberSummary> list = ProcessLateSubmissionRepository.GetLateSubmittedInvoiceMemberSummary(clearanceType,billingPeriod);

      IEnumerable<int> memberIds = list.Select(l => l.MemberId).Distinct();
      var newlist = new List<LateSubmissionMemberSummary>();

      // prepare new list in below format : 

      // MEmber        Count of   Passanger Billing  Cargo Billing    .....
      //               invoices
      // TT 987 - air   3         USD 43,234          USD 458         .....
      //                          CAD 34,567          CAD 789

      foreach (var memberId in memberIds)
      {
        List<LateSubmissionMemberSummary> memberReocords = list.Where(l => l.MemberId == memberId).ToList();
        var memberHeader = new LateSubmissionMemberSummary();

        memberHeader.MemberName = memberReocords.Take(1).SingleOrDefault().MemberName;
        memberHeader.MemberId = memberId;

        foreach (var lateSubmittedMemberHeader in memberReocords)
        {
          memberHeader.NoOfInvoices += lateSubmittedMemberHeader.NoOfInvoices;
          memberHeader.FormattedPassengerBilling += lateSubmittedMemberHeader.Currency + " " + lateSubmittedMemberHeader.PassengerBilling.ToString("N3") + "<br/>";
          memberHeader.FormattedCargoBilling += lateSubmittedMemberHeader.Currency + " " + lateSubmittedMemberHeader.CargoBilling.ToString("N3") + "<br/>";
          memberHeader.FormattedMiscBilling += lateSubmittedMemberHeader.Currency + " " + lateSubmittedMemberHeader.MiscBilling.ToString("N3") + "<br/>";
          memberHeader.FormattedUatpBilling += lateSubmittedMemberHeader.Currency + " " + lateSubmittedMemberHeader.UatpBilling.ToString("N3") + "<br/>";
        }

        newlist.Add(memberHeader);
      }

      return newlist;
    }

    public List<LateSubmittedInvoices> GetLateSubmittedInvoicesByMemberId(string clearanceType, int memberId)
    {
      //Additional Issue SCP 280475 - SRM: System Performance - ICH late submisson tab
      //CMP#624 : TFS#9311
      var billingPeriod = _calenderManager.GetLastClosedBillingPeriod(ClearingHouse.Ich);
      return ProcessLateSubmissionRepository.GetLateSubmittedInvoicesByMemberId(clearanceType, memberId, billingPeriod);
    }


    public List<InvoiceBase> GetLateSubmissionInvoicesByInvoiceIds(string invoiceIds)
    {
      var lateInvoiceList = new List<InvoiceBase>();
      foreach (var invoiceId in invoiceIds.Split(','))
      {
        var guid = ConvertUtil.ConvertStringtoGuid(ConvertNetGuidToOracleGuid(invoiceId));
        var inv = invoice.Single(p => p.Id == guid);
        if (inv != null)
        {
          lateInvoiceList.Add(inv);
        }
      }
      return lateInvoiceList;
    }

    public List<InvoiceBase> AcceptLateSubmittedInvoice(string invoiceIds)
    {
        var logRefId = Guid.NewGuid();
        var log = _referenceManager.GetDebugLog(DateTime.Now, "AcceptLateSubmittedInvoice", this.ToString(),
                                    "Ich", "Stage 1: ProcessLateSubmissionRepository.AcceptLateSubmittedInvoice start", 0, logRefId.ToString());
        _referenceManager.LogDebugData(log);

      var acceptedinvoiceIds =
          ProcessLateSubmissionRepository.AcceptLateSubmittedInvoice(ConvertNetGuidToOracleGuid(invoiceIds));
      var acceptedInvoiceList = new List<InvoiceBase>();

      log = _referenceManager.GetDebugLog(DateTime.Now, "AcceptLateSubmittedInvoice", this.ToString(),
                                  "Ich", "Stage 1: ProcessLateSubmissionRepository.AcceptLateSubmittedInvoice completed", 0, logRefId.ToString());
      _referenceManager.LogDebugData(log);


      //Get Current Billing Period
      var curBillingPeriod = CalendarManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);

      log = _referenceManager.GetDebugLog(DateTime.Now, "AcceptLateSubmittedInvoice", this.ToString(),
                             "Ich", "Stage 2: GetCurrentPeriodIfOpenOrNextAsCurrent completed", 0, logRefId.ToString());
      _referenceManager.LogDebugData(log);
      var logCounter = 0;
      if (acceptedinvoiceIds != "")
      {
          //SCP:170853 IS-Web response time - ICH Ops 
          //Dictionarey declartion to store invoice member details.
          //Billed and Billing Member details are retrieved and stored into Dictionary in first iteration. 
          //In next iteration member id is checked in Dictionary. if exists then retrieve the member details from dictionary
          //otherwise retrieve from database and store into Dictionary.

         Dictionary<int,Member> memberList=new Dictionary<int, Member>();
        foreach (var invoiceId in acceptedinvoiceIds.Split(','))
        {
            logCounter++;
                log = _referenceManager.GetDebugLog(DateTime.Now, "AcceptLateSubmittedInvoice", this.ToString(),
                             "Ich", "Stage 3: ConvertStringtoGuid start", 0, logRefId.ToString());
                _referenceManager.LogDebugData(log); 

          var guid = ConvertUtil.ConvertStringtoGuid(invoiceId);
          var inv = invoice.Single(p => p.Id == guid);

         log = _referenceManager.GetDebugLog(DateTime.Now, "AcceptLateSubmittedInvoice", this.ToString(),
                           "Ich", "Stage 4: invoice.Single completed", 0, logRefId.ToString());
              _referenceManager.LogDebugData(log);
            

          if (inv != null)
          {
            acceptedInvoiceList.Add(inv);
            if (inv.SubmissionMethodId == (int)SubmissionMethod.IsWeb)
            {
               
                    log = _referenceManager.GetDebugLog(DateTime.Now, "AcceptLateSubmittedInvoice", this.ToString(),
                                 "Ich", "Stage 5: UpdateMemberLocationInformationForLateSub start", 0, logRefId.ToString());
                    _referenceManager.LogDebugData(log);

               
              invoiveManager.UpdateMemberLocationInformationForLateSub(inv);

             
                  log = _referenceManager.GetDebugLog(DateTime.Now, "AcceptLateSubmittedInvoice", this.ToString(),
                               "Ich", "Stage 5: UpdateMemberLocationInformationForLateSub completd", 0, logRefId.ToString());
                  _referenceManager.LogDebugData(log);


              InvoiceRepository.UpdateSourceCodeTotalVat(inv.Id);

             
                  log = _referenceManager.GetDebugLog(DateTime.Now, "AcceptLateSubmittedInvoice", this.ToString(),
                               "Ich", "Stage 6: UpdateSourceCodeTotalVat completd", 0, logRefId.ToString());
                  _referenceManager.LogDebugData(log);

             
            }
            else
            {
               
                    log = _referenceManager.GetDebugLog(DateTime.Now, "AcceptLateSubmittedInvoice", this.ToString(),
                                 "Ich", "Stage 5: MemberLocationRepo.Get start", 0, logRefId.ToString());
                    _referenceManager.LogDebugData(log);

              inv.MemberLocationInformation = MemberLocationRepo.Get(l => l.InvoiceId == inv.Id).ToList();

             
                  log = _referenceManager.GetDebugLog(DateTime.Now, "AcceptLateSubmittedInvoice", this.ToString(),
                               "Ich", "Stage 5: MemberLocationRepo.Get completed", 0, logRefId.ToString());
                  _referenceManager.LogDebugData(log);

            }

              //SCP:170853 IS-Web response time - ICH Ops 
              //check  member details already exists or not 
              if(!memberList.ContainsKey(inv.BillingMemberId))
              {
                  //get the member details and store into dictionary
                  memberList.Add(inv.BillingMemberId,_membererManager.GetMember(inv.BillingMemberId));    
              }

              //var billingMember = _membererManager.GetMember(inv.BillingMemberId);
              //get the member details from dictionary
              var billingMember = memberList[inv.BillingMemberId]; 

           log = _referenceManager.GetDebugLog(DateTime.Now, "AcceptLateSubmittedInvoice", this.ToString(),
                             "Ich", "Stage 7: GetMember completed", 0, logRefId.ToString());
                _referenceManager.LogDebugData(log);

                //SCP:170853 IS-Web response time - ICH Ops 
                //check  member detail already exists or not 
                if (!memberList.ContainsKey(inv.BilledMemberId))
                { 
                    //get the member details and add into dictionary
                    memberList.Add(inv.BilledMemberId, _membererManager.GetMember(inv.BilledMemberId));
                }

            //var billedMember = _membererManager.GetMember(inv.BilledMemberId);

                //get the member details from dictionary
                var billedMember = memberList[inv.BilledMemberId];

            // Update Duplicate Coupon DU Mark
            InvoiceRepository.UpdateDuplicateCouponByInvoiceId(inv.Id, inv.BillingMemberId);

                log = _referenceManager.GetDebugLog(DateTime.Now, "AcceptLateSubmittedInvoice", this.ToString(),
                             "Ich", "Stage 8: UpdateDuplicateCouponByInvoiceId completed", 0, logRefId.ToString());
                _referenceManager.LogDebugData(log);

            invoiveManager.UpdateInvoiceDetailsForLateSubmission(inv, billingMember, billedMember);

           
                log = _referenceManager.GetDebugLog(DateTime.Now, "AcceptLateSubmittedInvoice", this.ToString(),
                             "Ich", "Stage 9: UpdateInvoiceDetailsForLateSubmission completed", 0, logRefId.ToString());
                _referenceManager.LogDebugData(log);

            // call procedure UpdateInvoiceOnReadyForBilling after acepting invoice.
            InvoiceRepository.UpdateInvoiceOnReadyForBilling(inv.Id, inv.BillingCategoryId, inv.BillingMemberId,
                                                             inv.BilledMemberId, inv.BillingCode);

            // SCP186155
            // Close correspondnece if Invoice contains BM on Corr (i.e. reason code '6A'/'6B')
            InvoiceRepository.CloseCorrespondenceOnInvoiceReadyForBilling(inv.Id, inv.BillingCategoryId);


            
                log = _referenceManager.GetDebugLog(DateTime.Now, "AcceptLateSubmittedInvoice", this.ToString(),
                             "Ich", "Stage 10: UpdateInvoiceOnReadyForBilling completed", 0, logRefId.ToString());
                _referenceManager.LogDebugData(log);

           
              //Set Here Invoice Legal Archive Parameter For Late Submission
              //ProcessLateSubmissionRepository.SetInvoiceArchiveParameterForLateSubmission(inv.Id, curBillingPeriod.Period);


              //    log = _referenceManager.GetDebugLog(DateTime.Now, "AcceptLateSubmittedInvoice", this.ToString(),
              //                 "Ich", "Stage 11: SetInvoiceArchiveParameterForLateSubmission completed", 0, logRefId.ToString());
                  _referenceManager.LogDebugData(log);
          }
        }
      }

      log = _referenceManager.GetDebugLog(DateTime.Now, "AcceptLateSubmittedInvoice", this.ToString(),
                        "Ich", "Stage 13: AcceptLateSubmittedInvoice completed for " + logCounter +" invoices.", 0, logRefId.ToString());
      _referenceManager.LogDebugData(log);

      return acceptedInvoiceList;

    }

    public bool RejectLateSubmittedInvoice(string invoiceIds, int categoryID)
    {
      List<LateSubmissionRejectInvoices> lateSubmittedInvoiceses = ProcessLateSubmissionRepository.RejectLateSubmittedInvoice(ConvertNetGuidToOracleGuid(invoiceIds));
      string clearenceHouse = categoryID == (int)Iata.IS.Model.MemberProfile.Enums.UserCategory.IchOps ? "ICH Operations" : "ACH Operations";

      lateSubmittedInvoiceses = GetContactsAndSendMail(lateSubmittedInvoiceses, clearenceHouse);

      return lateSubmittedInvoiceses.Count > 0;

    }

    public string CloseLateSubmissionWindow(string clearenceHouse, BillingPeriod billingPeriod, int userId, DateTime actualDateTime)
    {

        var logRefId = Guid.NewGuid();
        var log = _referenceManager.GetDebugLog(DateTime.Now, "CloseLateSubmissionWindow-Manager", this.ToString(),
                                     "Ich", "Stage 1: CloseLateSubmissionWindow start", userId, logRefId.ToString());
        _referenceManager.LogDebugData(log);

      string eventDesc = clearenceHouse.ToLower().Equals("ich")
                   ? CalendarConstants.ClosureOfLateSubmissionsIchColumn
                   : CalendarConstants.ClosureOfLateSubmissionsAchColumn;

      var closeLateSubmissionwindow =
CalendarRepository.Single(c => c.Period == billingPeriod.Period && c.Month == billingPeriod.Month && c.Year == billingPeriod.Year && c.EventDescription == eventDesc);

      log = _referenceManager.GetDebugLog(DateTime.Now, "CloseLateSubmissionWindow-Manager", this.ToString(),
                                   "Ich", "Stage 2: closeLateSubmissionwindow  selected", userId, logRefId.ToString());
      _referenceManager.LogDebugData(log);

      if (closeLateSubmissionwindow.EventDateTime < actualDateTime)
      {
        logger.InfoFormat(@"The {0} late submission window was already closed on {1} and actual scheduled time is {2}.", clearenceHouse.ToUpper(), closeLateSubmissionwindow.EventDateTime, actualDateTime);
        return string.Format("The {0} late submission window was already closed.", clearenceHouse.ToUpper());
      }

      closeLateSubmissionwindow.EventDateTime = actualDateTime;
      closeLateSubmissionwindow.LastUpdatedOn = actualDateTime;
      closeLateSubmissionwindow.LastUpdatedBy = userId;

      CalendarRepository.Update(closeLateSubmissionwindow);

      log = _referenceManager.GetDebugLog(DateTime.Now, "CloseLateSubmissionWindow-Manager", this.ToString(),
                                 "Ich", "Stage 3: closeLateSubmissionwindow  update completed ", userId, logRefId.ToString());
      _referenceManager.LogDebugData(log);

      UnitOfWork.CommitDefault();

      log = _referenceManager.GetDebugLog(DateTime.Now, "CloseLateSubmissionWindow-Manager", this.ToString(),
                               "Ich", "Stage 4: closeLateSubmissionwindow  commit completed ", userId, logRefId.ToString());
      _referenceManager.LogDebugData(log);


      CloseLateSubmissionWindow(clearenceHouse, billingPeriod);

      log = _referenceManager.GetDebugLog(DateTime.Now, "CloseLateSubmissionWindow-Manager", this.ToString(),
                             "Ich", "Stage 1: CloseLateSubmissionWindow completed ", userId, logRefId.ToString());
      _referenceManager.LogDebugData(log);


      return string.Empty;
    }

    public bool CloseLateSubmissionWindow(string clearenceHouse, BillingPeriod prevBillingDetails)
    {
        var logRefId = Guid.NewGuid();
        var log = _referenceManager.GetDebugLog(DateTime.Now, "CloseLateSubmissionWindow-clearenceHouse", this.ToString(),
                                     "Ich", "Stage 1: CloseLateSubmissionWindow start", 0, logRefId.ToString());
        _referenceManager.LogDebugData(log);

      var clearingHouse = new ClearingHouse();
      if (Enum.IsDefined(typeof(ClearingHouse), clearenceHouse))
        clearingHouse = (ClearingHouse)Enum.Parse(typeof(ClearingHouse), clearenceHouse);

      //SCP:170853 IS-Web response time - ICH Ops 
      //Commented below code. Used the passed  BillingPeriod details.
      // get closing billing period
      //BillingPeriod prevBillingDetails = _calenderManager.GetLastClosedBillingPeriod(clearingHouse);

       log = _referenceManager.GetDebugLog(DateTime.Now, "CloseLateSubmissionWindow-clearenceHouse", this.ToString(),
                                   "Ich", "Stage 2: GetLastClosedBillingPeriod completed", 0, logRefId.ToString());
      _referenceManager.LogDebugData(log);


      string billingPeriod = Convert.ToString(prevBillingDetails.Year) + Convert.ToString(prevBillingDetails.Month).PadLeft(2, '0') +
                             Convert.ToString(prevBillingDetails.Period).PadLeft(2, '0');

         
          // set pending invoice flag as rejecetd and get the deatils of rejected invoices for sending to respective member
         List<LateSubmissionRejectInvoices> lateSubmittedInvoiceses=
              ProcessLateSubmissionRepository.RejectLateSubmittedInvoiceOnLateSubmissionWindowClosing(clearenceHouse,
                                                                                                      prevBillingDetails);

          log = _referenceManager.GetDebugLog(DateTime.Now, "CloseLateSubmissionWindow-clearenceHouse", this.ToString(),
                                              "Ich",
                                              "Stage 3: RejectLateSubmittedInvoiceOnLateSubmissionWindowClosing completed",
                                              0, logRefId.ToString());
          _referenceManager.LogDebugData(log);

          //send mail to respective contacts (according to category)
          clearenceHouse = clearingHouse == ClearingHouse.Ich ? "ICH Operations" : "ACH Operations";
          lateSubmittedInvoiceses = GetContactsAndSendMail(lateSubmittedInvoiceses, clearenceHouse);

          log = _referenceManager.GetDebugLog(DateTime.Now, "CloseLateSubmissionWindow-clearenceHouse", this.ToString(),
                                              "Ich", "Stage 4: GetContactsAndSendMail completed", 0, logRefId.ToString());
          _referenceManager.LogDebugData(log);
      

        // communicate to Ich Closing Of LateSubmission
      if (clearingHouse == ClearingHouse.Ich)
      {
        CommunicateIchForLateSubmissionWindowClosing(billingPeriod, logRefId.ToString());

          log = _referenceManager.GetDebugLog(DateTime.Now, "CloseLateSubmissionWindow-clearenceHouse", this.ToString(),
                                              "Ich", "Stage 5: CommunicateIchForLateSubmissionWindowClosing completed", 0, logRefId.ToString());
          _referenceManager.LogDebugData(log);
      }

      log = _referenceManager.GetDebugLog(DateTime.Now, "CloseLateSubmissionWindow-clearenceHouse", this.ToString(),
                                           "Ich", "Stage 1: CloseLateSubmissionWindow completed", 0, logRefId.ToString());
      _referenceManager.LogDebugData(log);
     return lateSubmittedInvoiceses.Count > 0;
     
    }

    public void AutomaticOpeningOfICHLateSubmissionWindow()
    {
      // set system level parameter
      // SystemParameters.Instance.ICHLateAcceptanceAllowed = true;
    }

    public void AutomaticOpeningOfACHLateSubmissionWindow()
    {
      // set system level parameter
      //SystemParameters.Instance.ACHLateAcceptanceAllowed = true;
    }

    public void AutomaticClosingOfICHLateSubmissionWindow(string clearenceHouse, BillingPeriod billingPeriod, DateTime scheduledFireTimeUtc)
    {
      // set system level parameter
      //SystemParameters.Instance.ICHLateAcceptanceAllowed = false;
      CloseLateSubmissionWindow(clearenceHouse, billingPeriod, 0, scheduledFireTimeUtc);
    }

    public void AutomaticClosingOfACHLateSubmissionWindow()
    {
      // set system level parameter
      //SystemParameters.Instance.ACHLateAcceptanceAllowed = false;
        //SCP:170853 IS-Web response time - ICH Ops 
        //Get the previous billing period details
        var prevBillingDetails = _calenderManager.GetLastClosedBillingPeriod(ClearingHouse.Ach);
      CloseLateSubmissionWindow("Ach",prevBillingDetails);
    }

    #region "Private Methods"

    //This code related to CMP 353 but it was commented out since Iata didn't want it to be included in CMP353
    //private void SendMailForClaimFailedInvoices(List<ProcessingDashboardInvoiceActionStatus> claimfailedInvoices)
    //{
    //    try
    //    {
    //        //get an object of the EmailSender component
    //        var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));

    //        //get an instance of email settings  repository
    //        var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));

    //        //get an object of the TemplatedTextGenerator that is used to generate body text of email from a nvelocity template
    //        var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));

    //        //object of the Velocity data dictionary
    //        var context = new VelocityContext();
    //        var htmlContentContext = new VelocityContext();
    //        //fill nvelocity data dictionary 

    //        context.Put("n", "\n");
    //        htmlContentContext.Put("ClaimFailedInvoices", claimfailedInvoices);


    //        //Get the eMail settings for reacp sheet overview 
    //        var emailSetting = emailSettingsRepository.Get(es => es.Id == (int)EmailTemplateId.ICHSettlementClaimFailedInvoices);

    //        //generate email body text f
    //        var body = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.ICHSettlementClaimFailedInvoices, context);
    //        var emailContentforAttachment = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.ICHSettlementClaimFailedInvoicesContent, htmlContentContext);

    //        //create a mail object to send mail
    //        var overview = new MailMessage { From = new MailAddress(emailSetting.SingleOrDefault().FromEmailAddress) };
    //        overview.IsBodyHtml = true;

    //        // loop through the contacts list and add them to To list of mail to be sent
    //        if (!string.IsNullOrEmpty(AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail))
    //        {
    //            var mailAdressList = ConvertUtil.ConvertToMailAddresses(AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);

    //            foreach (var mailaddr in mailAdressList)
    //            {
    //                overview.To.Add(mailaddr);
    //            }
    //        }

    //        if (!string.IsNullOrEmpty(AdminSystem.SystemParameters.Instance.ICHDetails.IchOpsEmail))
    //        {
    //            var mailAdressList = ConvertUtil.ConvertToMailAddresses(AdminSystem.SystemParameters.Instance.ICHDetails.IchOpsEmail);

    //            foreach (var mailaddr in mailAdressList)
    //            {
    //                overview.To.Add(mailaddr);
    //            }
    //        }
    //        //set body text of mail
    //        overview.Body = body;
    //        //Get Temp File Path
    //        string tempFoldePath = Path.GetTempPath();
    //        BillingPeriod lastBillingMonthPeriod = _calenderManager.GetLastClosedBillingPeriod(ClearingHouse.Ich);
    //        string billingPeriod = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(lastBillingMonthPeriod.Month) + Convert.ToString(lastBillingMonthPeriod.Year) + Convert.ToString(lastBillingMonthPeriod.Period).PadLeft(2, 'P');

    //        string fileName = string.Format("{0} - List of Claim Failed ICH Settelment Status invoices-  {0}.htm", billingPeriod);
    //        string filePath = Path.Combine(tempFoldePath, fileName);
    //        var attachmentFileStream = new StreamWriter(filePath, false);
    //        attachmentFileStream.WriteLine(emailContentforAttachment);
    //        attachmentFileStream.Close();
    //        overview.Attachments.Add(new System.Net.Mail.Attachment(filePath));
    //        logger.Info("Sending mail List of Claim Failed ICH Settelment Status invoices: " + body);
    //        //send the mail
    //        emailSender.Send(overview);

    //        //clear nvelocity context data
    //        context = null;
    //    }
    //    catch (Exception ex)
    //    {
    //        logger.Error("Exception occured while sending mail to members of late submitted rejected invoices.", ex);
    //    }
    //}

    private void SendMailForRejecetedInvoices(int memberId, IEnumerable<string> toEmailList, List<LateSubmissionRejectInvoices> lateSubmittedInvoiceses, string clearingHouse)
    {
      try
      {
        //get an object of the EmailSender component
        var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));

        //get an instance of email settings  repository
        var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));

        //get an object of the TemplatedTextGenerator that is used to generate body text of email from a nvelocity template
        var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));
        //Get billing Member
        var billingMember = _membererManager.GetMemberDetails(memberId);
        //object of the Velocity data dictionary
        var context = new VelocityContext();
        var htmlContentContext = new VelocityContext();
        //fill nvelocity data dictionary 
        context.Put("CH", clearingHouse);
        context.Put("SISOpsEmail", SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);
        //context.Put("n", "\n");
        //context.Put("RejectInvoices", lateSubmittedInvoiceses);
        htmlContentContext.Put("RejectInvoices", lateSubmittedInvoiceses);
        //Get the eMail settings for reacp sheet overview 
        var emailSetting = emailSettingsRepository.Get(es => es.Id == (int)EmailTemplateId.LateSubmissionInvoiceRejectNotification);

        //generate email body text f
        var body = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.LateSubmissionInvoiceRejectNotification, context);
        var emailContentforAttachment = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.LateSubmissionInvoiceRejectHtmlContents, htmlContentContext);

        //create a mail object to send mail
        var overview = new MailMessage { From = new MailAddress(emailSetting.SingleOrDefault().FromEmailAddress) };
        overview.IsBodyHtml = true;

        //loop through the contacts list and add them to To list of mail to be sent
        //string[] toList = toEmailList.Split(',');

        foreach (var contact in toEmailList)
        {
          overview.To.Add(new MailAddress(contact));
        }

        // prepare CC list
        string[] ccList = clearingHouse.Contains("ACH") ? AdminSystem.SystemParameters.Instance.ACHDetails.AchOpsEmail.Replace(';', ',').Split(',') : AdminSystem.SystemParameters.Instance.ICHDetails.IchOpsEmail.Replace(';', ',').Split(',');

        foreach (var cc in ccList)
        {
          overview.CC.Add(new MailAddress(cc));
        }

        //set subject of mail 
        overview.Subject = emailSetting.SingleOrDefault().Subject.Replace("$MemberCode$", string.Format("{0}-{1}", billingMember.MemberCodeNumeric.PadLeft(3, '0'), billingMember.MemberCodeAlpha));

        //set body text of mail
        overview.Body = body;
        //Get Temp File Path
        string tempFoldePath = Path.GetTempPath();
        
        // SCP63770: Rejected Late Processing Request - by 016-UA - SIS
        // Clearing house 'ACH' or 'ICH' is not able to match (character case is upper) with enums defined in enum 'ClearingHouse',
        // and hence it is changed to 'Ach' and 'Ich', so that it can match with the enum and returns correct Clearing house.
        if (clearingHouse.ToUpper().Equals("ACH"))
        {
          clearingHouse = "Ach";
        }
        else if (clearingHouse.ToUpper().Equals("ICH"))
        {
          clearingHouse = "Ich";
        }

        var clearenceHouse = new ClearingHouse();
        if (Enum.IsDefined(typeof(ClearingHouse), clearingHouse))
          clearenceHouse = (ClearingHouse)Enum.Parse(typeof(ClearingHouse), clearingHouse);
        BillingPeriod lastBillingMonthPeriod = _calenderManager.GetLastClosedBillingPeriod(clearenceHouse);
        string billingPeriod = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(lastBillingMonthPeriod.Month) + Convert.ToString(lastBillingMonthPeriod.Year) + Convert.ToString(lastBillingMonthPeriod.Period).PadLeft(2, 'P');
        string fileName = string.Format("{0}- List of invoices rejected for late submission-{1}.htm", billingMember.MemberCodeNumeric.PadLeft(3, '0'), billingPeriod);
        string filePath = Path.Combine(tempFoldePath, fileName);
        var attachmentFileStream = new StreamWriter(filePath, false);
        attachmentFileStream.WriteLine(emailContentforAttachment);
        attachmentFileStream.Close();
        overview.Attachments.Add(new System.Net.Mail.Attachment(filePath));
        logger.Info("Sending mail to members of rejected invoices: " + body);
        //send the mail
        emailSender.Send(overview);

        //clear nvelocity context data
        context = null;
      }
      catch (Exception ex)
      {
        logger.Error("Exception occured while sending mail to members of late submitted rejected invoices.", ex);
      }

    }

    private void SendMailForWebServiceErrorOrUnavailability(List<string> error, string BillingPeriod)
    {
      try
      {
        //get an object of the EmailSender component
        var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));

        //get an instance of email settings  repository
        var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));

        //get an object of the TemplatedTextGenerator that is used to generate body text of email from a nvelocity template
        var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));

        //object of the nVelocity data dictionary
        var context = new VelocityContext();

        //fill nvelocity data dictionary 
        context.Put("WsError", error);
        context.Put("BilingPeriod", BillingPeriod);
        context.Put("SISOpsemailid", AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);
        //Get the eMail settings for recap sheet overview 
        var emailSetting = emailSettingsRepository.Get(es => es.Id == (int)EmailTemplateId.LateSubmissionWSUnavailabilityNotification);

        //generate email body text f
        var body = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.LateSubmissionWSUnavailabilityNotification, context);

        //create a mail object to send mail
        var mailOverview = new MailMessage { From = new MailAddress(emailSetting.SingleOrDefault().FromEmailAddress) };
        mailOverview.IsBodyHtml = true;

        //loop through the contacts list and add them to To list of mail to be sent
        //string[] toList = ClearingHouse.Ach == clearingHouse? System.Configuration.ConfigurationManager.AppSettings["AchOpsEmail"].Split(','): System.Configuration.ConfigurationManager.AppSettings["IchOpsEmail"].Split(',');
        string[] toList = AdminSystem.SystemParameters.Instance.ICHDetails.IchOpsEmail.Split(',');

        foreach (var contact in toList)
        {
          mailOverview.To.Add(new MailAddress(contact));
        }

        //set subject of mail 
        mailOverview.Subject = emailSetting.SingleOrDefault().Subject;

        //set body text of mail
        mailOverview.Body = body;

        logger.Info("Sending mail to members of rejected invoices: " + body);
        //send the mail
        emailSender.Send(mailOverview);

        //clear nvelocity context data
        context = null;
      }
      catch (Exception ex)
      {
        logger.Error("Exception occured while sending mail to members of late submitted rejected invoices.", ex);
      }

    }

    private void SendMailForWebServiceErrorResponce(string errorCode, string BillingPeriod)
    {
      try
      {
        //get an object of the EmailSender component
        var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));

        //get an instance of email settings  repository
        var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));

        //get an object of the TemplatedTextGenerator that is used to generate body text of email from a nvelocity template
        var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));

        //object of the nVelocity data dictionary
        var context = new VelocityContext();

        //fill nvelocity data dictionary 
        context.Put("ErrorCode", errorCode);
        context.Put("BilingPeriod", BillingPeriod);
        context.Put("SISOpsemailid", AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);
        //Get the eMail settings for reacp sheet overview 
        var emailSetting = emailSettingsRepository.Get(es => es.Id == (int)EmailTemplateId.AutoClosingICHLateSubFailureNotification);

        //generate email body text f
        var body = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.AutoClosingICHLateSubFailureNotification, context);

        //create a mail object to send mail
        var mailOverview = new MailMessage { From = new MailAddress(emailSetting.SingleOrDefault().FromEmailAddress) };
        mailOverview.IsBodyHtml = true;

        //loop through the contacts list and add them to To list of mail to be sent
        //string[] toList = ClearingHouse.Ach == clearingHouse? System.Configuration.ConfigurationManager.AppSettings["AchOpsEmail"].Split(','): System.Configuration.ConfigurationManager.AppSettings["IchOpsEmail"].Split(',');
        string[] toList = AdminSystem.SystemParameters.Instance.ICHDetails.IchOpsEmail.Split(',');

        foreach (var contact in toList)
        {
          mailOverview.To.Add(new MailAddress(contact));
        }

        //set subject of mail 
        mailOverview.Subject = emailSetting.SingleOrDefault().Subject;

        //set body text of mail
        mailOverview.Body = body;

        logger.Info("Sending mail to members of late submition Window closure: " + body);
        //send the mail
        emailSender.Send(mailOverview);

        //clear nvelocity context data
        context = null;
      }
      catch (Exception ex)
      {
        logger.Error("Exception occured while sending mail to members of late submition Window closure", ex);
      }

    }

    private List<LateSubmissionRejectInvoices> GetContactsAndSendMail(List<LateSubmissionRejectInvoices> lateSubmittedInvoiceses, string clearenceHouse)
    {
      IEnumerable<string> emailAddress = null;
      var invoiceses = new List<LateSubmissionRejectInvoices>();

      // remove invoice which are  already rejected
      lateSubmittedInvoiceses = lateSubmittedInvoiceses.Where(l => l.LateSubRequestStatus != (int)LateSubmissionRequestStatus.Rejected).ToList();

      IEnumerable<int> memberIds = lateSubmittedInvoiceses.Select(l => l.BillingMemberId).Distinct();

      foreach (var memberId in memberIds)
      {

        var processingContactType = clearenceHouse.Contains("ACH")
                                      ? ProcessingContactType.ACHPrimaryContact
                                      : ProcessingContactType.ICHPrimaryContact;

        var contacts = _membererManager.GetContactsForContactType(memberId, processingContactType);
        invoiceses = lateSubmittedInvoiceses.Where(l => l.BillingMemberId == memberId).ToList();

        // get the contacts email address
        if (contacts != null)
        {
          if (contacts.Count > 0)
            emailAddress = contacts.Select(c => c.EmailAddress);
        }

        // send mail to contact about rejection of invoices
        SendMailForRejecetedInvoices(memberId, emailAddress, invoiceses, clearenceHouse);

        // add alerts
        AddAlert(memberId, processingContactType);
      }

      return lateSubmittedInvoiceses;

    }

    private string PrepareRequestXml(string billingPeriod)
    {
      // starts preparing request xml
      XmlDocument doc = new XmlDocument();

      // XML declaration
      XmlNode declaration = doc.CreateNode(XmlNodeType.XmlDeclaration, null, null);
      doc.AppendChild(declaration);

      // Root element: LateSubmissionClosure
      XmlElement root = doc.CreateElement("LateSubmissionClosure");
      doc.AppendChild(root);

      // Sub-element: BillingPeriod
      XmlElement author = doc.CreateElement("BillingPeriod");
      author.InnerText = billingPeriod;
      root.AppendChild(author);

      //  Sub-element: Closed
      XmlElement isadmin = doc.CreateElement("Closed");
      isadmin.InnerText = "Y";
      root.AppendChild(isadmin);

      //TODO : Validate xml
      return doc.InnerXml;

    }

    private void CommunicateIchForLateSubmissionWindowClosing(string billingPeriod, string logRefId)
    {
      // starts: communicate to Ich Closing Of LateSubmission
      string xml = PrepareRequestXml(billingPeriod);

      string response;
      XmlDocument responseXml = new XmlDocument();
      List<string> wsError = new List<string>();

      try
      {
        // web service call starts.
        string _errorcode = string.Empty;
        var obj = new SISGatewayService.SISGatewayServiceClient();
        System.Net.ServicePointManager.ServerCertificateValidationCallback =
          ((sender1, certificate, chain, sslPolicyErrors) => true);

        //obj.ClientCredentials.UserName.UserName = "user1";
        //obj.ClientCredentials.UserName.Password = "pass1";
        obj.ClientCredentials.UserName.UserName = SystemParameters.Instance.ICHDetails.SisGatewayServiceUserName; // ConfigurationManager.AppSettings.Get("SISGatewayServiceUserName");
        obj.ClientCredentials.UserName.Password = SystemParameters.Instance.ICHDetails.SisGatewayServiceUsersPassword; // ConfigurationManager.AppSettings.Get("SISGatewayServicePassword");
        logger.Info("Calling Of Ich Webservice for late submission window closing Started");

        var log = _referenceManager.GetDebugLog(DateTime.Now, "CloseLateSubmissionWindow-clearenceHouse >> CommunicateIchForLateSubmissionWindowClosing", this.ToString(),
                                              "Ich", "Stage 4.1: Call Ich Webservice for late submission window closing Started", 0, logRefId);
        _referenceManager.LogDebugData(log);

        response = obj.LateSubmissionClosureNotification(xml);

        log = _referenceManager.GetDebugLog(DateTime.Now, "CloseLateSubmissionWindow-clearenceHouse >> CommunicateIchForLateSubmissionWindowClosing", this.ToString(),
                                              "Ich", "Stage 4.1: Call Ich Webservice for late submission window closing Completed.", 0, logRefId);
        _referenceManager.LogDebugData(log);

        log = _referenceManager.GetDebugLog(DateTime.Now, "CloseLateSubmissionWindow-clearenceHouse >> CommunicateIchForLateSubmissionWindowClosing", this.ToString(),
                                              "Ich", string.Format("Stage 4.1: Response Recieved from ICH: [{0}]", response ?? string.Empty), 0, logRefId);
        _referenceManager.LogDebugData(log);

        logger.Info("Ich Webservice Responce : " + response);
        logger.Info("Calling Of Ich Webservice for late submission window closing Completed");
        if (string.IsNullOrEmpty(response))
        {
          _errorcode = "Null response got from Web service ";
          SendMailForWebServiceErrorResponce(_errorcode, billingPeriod);
          return;
        }
        responseXml.InnerXml = response;
        // check respone if does not contain success elemennt then send error email
        if (responseXml.DocumentElement.SelectSingleNode("//Success") == null)
        {
          // prepaer error list to send in email to ich ops
          XmlNodeList nodes = responseXml.DocumentElement.SelectNodes("//Error");
          for (int i = 0; i < nodes.Count; i++)
          {
            //wsError.Add(nodes[i].Attributes["ErrorLongDescription"].InnerText);
            //innerNode.Attribute("ErrorCode").Value
            _errorcode = nodes[i].Attributes["ErrorCode"].InnerText;
          }
          // send mail on webservice error
          SendMailForWebServiceErrorResponce(_errorcode, billingPeriod);
        }

      }

      catch (Exception exception)
      {
        logger.Error("Error occured Calling Of Ich Webservice for late submission window closing : ", exception);
        wsError.Add(exception.Message);
        // send mail on webservice exception
        SendMailForWebServiceErrorOrUnavailability(wsError, billingPeriod);
      }

    }

    /// <summary>
    /// Converts string(i.e. FileId) to GUID and converts it to ByteArray.
    /// Iterates through ByteArray, convert it to Hexadecimal equivalent and appends each hex values to 
    /// create a string(i.e. FileId in Oracle GUID format)
    /// </summary>
    private string ConvertNetGuidToOracleGuid(string invoiceIds)
    {
      // Declare variable of type StringBuilder
      var oracleGuidBuilder = new StringBuilder();

      // Iterate through file Id's and split on ','
      foreach (var netGuid in invoiceIds.Split(','))
      {
        // Call "ConvertNetGuidToOracleGuid()" method which will return string 
        oracleGuidBuilder.Append(ToOracleGuid(netGuid));
        oracleGuidBuilder.Append(",");
      }// end foreach()

      // Declare string variable
      string oracleGuid;
      // Set string variable to Guid in Oracle format
      oracleGuid = oracleGuidBuilder.ToString();
      // Trim last ',' of comma separated GUID string
      oracleGuid = oracleGuid.TrimEnd(',');

      return oracleGuid;

    }// end ConvertNetGuidToOracleGuid()

    private string ToOracleGuid(string oracleGuid)
    {
      // Convert string to Guid
      Guid netGuid = new Guid(oracleGuid);
      // Convert Guid to ByteArray
      byte[] guidBytes = netGuid.ToByteArray();
      // Create StringBuilder
      var oracleGuidBuilder = new StringBuilder();
      // Iterate through ByteArray, get Hex equivalent of each byte and append it to string
      foreach (var singleByte in guidBytes)
      {
        // Get Hex equivalent of each byte
        var hexEqui = singleByte.ToString("X");
        // Append each Hex equivalent to construct Guid.(Pad '0' to single byte)
        oracleGuidBuilder.Append(hexEqui.ToString().PadLeft(2, '0'));
      }// end foreach()

      // Return Guid string in Oracle format
      return oracleGuidBuilder.ToString();
    }

    private void AddAlert(int memberId, ProcessingContactType contactType)
    {
      var messageRecipients = new ISMessageRecipients
      {
        MemberId = memberId,
        ContactTypeId = Convert.ToString((int)contactType),

        IsMessagesAlerts = new ISMessagesAlerts
        {
          Message = "Reject Invoice Alert",
          StartDateTime = DateTime.UtcNow,
          LastUpdatedOn = DateTime.UtcNow,
          IsActive = true,
          TypeId = (int)MessageType.Alert,
          RAGIndicator = (int)RAGIndicator.Red
        }
      };

      BroadcastMessages.AddAlerts(messageRecipients);

    }
    #endregion


  }
}

