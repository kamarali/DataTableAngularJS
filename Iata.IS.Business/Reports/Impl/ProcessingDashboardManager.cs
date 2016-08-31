using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using Castle.Core.Smtp;
using Iata.IS.AdminSystem;
using Iata.IS.Business.Common;
using Iata.IS.Business.Common.Impl;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.Pax;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Core;
using Iata.IS.Core.Configuration;
using Iata.IS.Model.Base;
using Iata.IS.Data;
using System.Linq;
using Iata.IS.Model.Common;
using Iata.IS.Model.LateSubmission.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Reports;
using Iata.IS.Core.DI;
using Iata.IS.Data.Reports;
using iPayables.UserManagement;
using log4net;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Calendar;
using Iata.IS.Data.Pax;
using Iata.IS.Model.Pax.Enums;
using NVelocity;
using System.Configuration;
using Iata.IS.Model.Pax.Common;
using UnitOfWork = Iata.IS.Data.Impl.UnitOfWork;
using UserCategory = Iata.IS.Model.MemberProfile.Enums.UserCategory;
using Iata.IS.Business.Common;
using Iata.IS.Core.Exceptions;
using Ionic.Zip;

namespace Iata.IS.Business.Reports.Impl
{
  public class ProcessingDashboardManager : InvoiceManagerBase, IProcessingDashboardManager
  {
    public IRepository<InvoiceBase> InvoiceReository { get; set; }
    public IRepository<IsInputFile> FileRepository { get; set; }
    public ISamplingFormCRepository FormCRepository { get; set; }
    private readonly IMemberManager _memberManager;
    private readonly ICalendarManager _calenderManager;
    private readonly IReferenceManager _referenceManager;
    public IProcessingDashboardInvoiceStatusRepository ProcessingDashboardInvoiceStatusRepository { get; set; }
    public IRepository<IsInputFile> IsInputFileReository { get; set; }
    public IRepository<PassengerConfiguration> PassengerRepository { get; set; }
    public IRepository<CargoConfiguration> CargoRepository { get; set; }
    private readonly IUserManagement _iUserManagement;
    public IRepository<MiscellaneousConfiguration> MiscellaneousRepository { get; set; }
    public IRepository<UatpConfiguration> UatpRepository { get; set; }
    public IRepository<Member> MemberRepository { get; set; }
    public IRepository<UatpConfiguration> UatpReository { get; set; }
    public IInvoiceManager InvoiveManager { get; set; }
    public IInvoiceManager InvoiceManagerBase { get; set; }
    
    public IInvoiceRepository InvoiceRepository { get; set; }
    public IRepository<MemberLocationInformation> MemberLocationRepo { get; set; }
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public ProcessingDashboardManager(IMemberManager memberManager, ICalendarManager calenderManager, IReferenceManager referenceManager, IUserManagement iUserManagement)
    {
      _memberManager = memberManager;
      _calenderManager = calenderManager;
      _referenceManager = referenceManager;
      _iUserManagement = iUserManagement;
    }

    public List<int> GetAllInvoiceYear()
    {
      return GetBillingYears();
    }

    public List<int> GetAllIsFileLogYear()
    {
      return GetBillingYears();
    }

    public List<ProcessingDashboardInvoiceStatusResultSet> GetInvoiceStatusResultForProcDashBrd(ProcessingDashboardSearchEntity searchCriteria)
    {
      //get an instance of Processing Dashboard repository
      var processingDashboardRepository = Ioc.Resolve<IProcessingDashboardInvoiceStatusRepository>(typeof(IProcessingDashboardInvoiceStatusRepository));

      List<ProcessingDashboardInvoiceStatusResultSet> listOfInvoiceStatusResultSet = processingDashboardRepository.GetInvoiceStatusResultForProcDashBrd(searchCriteria);

      return listOfInvoiceStatusResultSet;
    }// end GetInvoiceStatusResultForProcDashBrd()

    /// <summary>
    /// Following method returns FileStatus result status depending on criteria specified
    /// </summary>
    /// <param name="searchCriteria">Search Criteria</param>
    /// <returns>List of FileStatus result</returns>
    public List<ProcessingDashboardFileStatusResultSet> GetFileStatusResultForProcDashBrd(ProcessingDashboardSearchEntity searchCriteria)
    {
      //get an instance of Processing Dashboard repository
      var processingDashboardRepository = Ioc.Resolve<IProcessingDashboardFileStatusRepository>(typeof(IProcessingDashboardFileStatusRepository));
      // Call GetFileStatusResultForProcDashBrd from repository which will return FileStatus result set
      List<ProcessingDashboardFileStatusResultSet> listOfInvoiceStatusResultSet = processingDashboardRepository.GetFileStatusResultForProcDashBrd(searchCriteria);
      // return retrieved file Status Result set
      return listOfInvoiceStatusResultSet;
    }// end GetInvoiceStatusResultForProcDashBrd()

    /// <summary>
    /// Following method is used to retrieve Invoice details for specific Invoice
    /// </summary>
    /// <param name="invoiceId">InvoiceId whose details are to be retrieved</param>
    /// <returns>Invoice details</returns>
    public ProcessingDashboardInvoiceDetail GetInvoiceDetailsForProcDashBrd(Guid invoiceId)
    {
      //get an instance of Processing Dashboard repository
      var processingDashboardRepository = Ioc.Resolve<IProcessingDashboardInvoiceStatusRepository>(typeof(IProcessingDashboardInvoiceStatusRepository));
      // Get invoice details by executing stored procedure
      var invoiceDetail = processingDashboardRepository.GetInvoiceDetailsForProcDashBrd(invoiceId);
      // return Invoice details
      return invoiceDetail;
    }// end GetInvoiceDetailsForProcDashBrd()

    public List<Member> GetAggregatedSponsoredMemberList(int memberId)
    {
      var memberList = _memberManager.GetAggregatedSponsoredMemberList(memberId, false);
      Member member = _memberManager.GetMember(memberId);
      memberList.Add(member);

      return memberList;
    }

    public ProcessingDashboardSearchEntity SetProcessingDashboardSearchEntity(int categoryId, int memberId)
    {
      // to fill logged in user's member is in billing member text box 
      ProcessingDashboardSearchEntity processingDashboardSearchEntity = new ProcessingDashboardSearchEntity();

      if (categoryId == (int)UserCategory.Member)
      {
        Member member = _memberManager.GetMember(memberId);
        processingDashboardSearchEntity.BillingMember = member.MemberCodeAlpha + "-" + member.MemberCodeNumeric + "-" +
                                                        member.CommercialName;
        processingDashboardSearchEntity.BillingMemberId = member.Id;

        // check condition to disable billing member textbox if member dosent have any sponsor or aggregator and user is member user
        if (_memberManager.GetAggregatedSponsoredMemberList(memberId, false).Count <= 0)
          processingDashboardSearchEntity.DisableBillingTextBox = "true";
      }
      //CMP529 : Daily Output Generation for MISC Bilateral Invoices
      processingDashboardSearchEntity.DailyDeliverystatusId = -1;

      return processingDashboardSearchEntity;
    }

    public byte[] GenerateCsv(ProcessingDashboardSearchEntity searchCriteria, char separator, bool isInvoiceStatusCsv)
    {
      if (isInvoiceStatusCsv)
        return GenerateInvoiceStatusCsvFile(searchCriteria, separator);
      else
        return GenerateFileStatusCsvFile(searchCriteria, separator);
    }

    /// <summary>
    /// Following method is used to update LateSubmission flag to true for selected Invoices.
    /// </summary>
    /// <param name="invoiceIdArray">Array of InvoiceId's</param>
    /// <param name="memberId">Member id of logged in user</param>
    /// <returns>List of Invoices with ActionStatus i.e. Success or Failure</returns>
    public List<ProcessingDashboardInvoiceActionStatus> MarkInvoiceForLateSubmission(string[] invoiceIdArray, int memberId, int userId, Boolean isMarkfileForLateSubmission = false)
    {
      // Create List of type ProcessingDashboardInvoiceActionStatus
      List<ProcessingDashboardInvoiceActionStatus> invoiceList = new List<ProcessingDashboardInvoiceActionStatus>();
      bool isManulaControl = true;
      List<InvoiceBase> autoAcceptedInvoices=new List<InvoiceBase>();
      bool isAchManulaControl = Convert.ToBoolean(SystemParameters.Instance.ACHDetails.ManualControlOnACHLateSubmission); 
      bool isIchManulaControl = Convert.ToBoolean(SystemParameters.Instance.ICHDetails.ManualControlOnIchLateSubmission); 
      // Iterate through InvoiceIdArray and retrieve Invoices/FormC details
      foreach (string item in invoiceIdArray)
      {
        if (string.IsNullOrEmpty(item))
          continue;

        // Convert item string to Guid
        Guid invoiceId = new Guid(item);

        // Get invoice details from Repository for specified InvoiceId
        InvoiceBase invoiceDetail = InvoiceReository.Single(invoice => invoice.Id == invoiceId);

        // Create instance of type ProcessingDashboardInvoiceActionStatus
        ProcessingDashboardInvoiceActionStatus invoiceAction = new ProcessingDashboardInvoiceActionStatus();

        // If Invoice exists perform operations to update LateSubmission flag, else check whether FORMC exists with that InvoiceId
        if (invoiceDetail != null)
        {
          // Declare variable of type Billing period
          BillingPeriod billingPeriodDetails = new BillingPeriod();

          // Retrieve Billing member details 
          var memberDetails = _memberManager.GetMember(invoiceDetail.BillingMemberId);
          // Retrieve Billed member details 
          var billedMember = _memberManager.GetMember(invoiceDetail.BilledMemberId);

          // Get Final Parent Details for SMI, Currency, Clearing House abd Suspended Flag validations
          var billingFinalParent = _memberManager.GetMember(_memberManager.GetFinalParentDetails(invoiceDetail.BillingMemberId));
          var billedFinalParent = _memberManager.GetMember(_memberManager.GetFinalParentDetails(invoiceDetail.BilledMemberId));

          var billedMemberStatus = billedMember.IsMembershipStatus;

          // If Invoice SettlementMethod == Bilateral or AdjustmentDueToProtest, Ignore that Invoice, else perform LateSubmission operations
          /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
          if ((_referenceManager.IsSmiLikeBilateral(invoiceDetail.SettlementMethodId, false)) || invoiceDetail.SettlementMethodId == (int)SMI.AdjustmentDueToProtest)
          {
            // Set ActionStatus text to "Ignored"
            invoiceAction.ActionStatus = "Ignored:Not eligible for settlement through CH";
          }// end if()
          else
          {
            // Declare bool variable to check whether Late Submission window is open
            //bool IsLateSubmissionWindowOpen = false;
            // Declare bool variable to check whether Late Acceptance is true
            bool IsLateAcceptanceAllowed = false;

            var clearingHouse = _referenceManager.GetClearingHouseForInvoice(invoiceDetail, billingFinalParent, billedFinalParent);

            string statusMsg = string.Empty;
            var ch = new ClearingHouse();

            // If SettlementMethodId belongs to ICH, check whether LateSubmissionWindow is open and LateAcceptance is allowed
            if (invoiceDetail.SettlementMethodId == (int)SMI.Ich || invoiceDetail.SettlementMethodId == (int)SMI.IchSpecialAgreement)
            {
              ch = ClearingHouse.Ich;
              isManulaControl = isIchManulaControl;//Convert.ToBoolean(AdminSystem.SystemParameters.Instance.ICHDetails.ManualControlOnIchLateSubmission);
              billingPeriodDetails = _calenderManager.GetLastClosedBillingPeriod(ch);
              IsLateAcceptanceAllowed = _calenderManager.IsLateSubmissionWindowOpen(ch, billingPeriodDetails);
              statusMsg = "Ignored:ICH Late Submission Not Opened";
            }
            // If SettlementMethodId belongs to ACH, check whether LateSubmissionWindow is open and LateAcceptance is allowed
            else if (invoiceDetail.SettlementMethodId == (int)SMI.Ach || invoiceDetail.SettlementMethodId == (int)SMI.AchUsingIataRules)
            {
              ch = ClearingHouse.Ach;
              isManulaControl = isAchManulaControl;//Convert.ToBoolean(SystemParameters.Instance.ACHDetails.ManualControlOnACHLateSubmission);
              billingPeriodDetails = _calenderManager.GetLastClosedBillingPeriod(ch);
              IsLateAcceptanceAllowed = _calenderManager.IsLateSubmissionWindowOpen(ch, billingPeriodDetails);
              statusMsg = "Ignored:ACH Late Submission Not Opened";
            }

            // revalidate invoice if manulal controll on late submission is false, check the is status of billed and billing
            if (IsLateAcceptanceAllowed)
            {
              statusMsg = string.Empty;

              // Update Invoice if ValidationStatusId == 4,BillingPeriod of Invoice == CurrentBillingPeriod, BillingMonth == CurrentBillingMonth, 
              // ClearanceType is != Bialateral or AdjustmentDueToProtest and IsLateSubmitted == false
              if (invoiceDetail.ValidationStatusId == 4 && invoiceDetail.BillingPeriod == billingPeriodDetails.Period &&
                  invoiceDetail.BillingMonth == billingPeriodDetails.Month && !invoiceDetail.IsLateSubmitted)
              {
                // revalidate if auto late submission
                if (!isManulaControl)
                {
                    autoAcceptedInvoices.Add(invoiceDetail);
                  var isBillingMemberStatus = memberDetails.IsMembershipStatus;
                  bool isDigitalSignReqrd = memberDetails.DigitalSignApplication;
                  string dsStatus = string.Empty;

                  if (!string.IsNullOrEmpty(invoiceDetail.DsStatus))
                    dsStatus = invoiceDetail.DsStatus.ToUpper();
                  bool flag = false;

                  if ((dsStatus == "Y" || dsStatus == "V") && !isDigitalSignReqrd)
                    flag = true;
                  else
                  {
                    var billingCatId = invoiceDetail.BillingCategoryId;
                    if (billingCatId == (int)BillingCategoryType.Pax ||
                        billingCatId == (int)BillingCategoryType.Cgo
                        || billingCatId == (int)BillingCategoryType.Misc)
                    {
                      if ((dsStatus == "Y" || dsStatus == "V") && isDigitalSignReqrd)
                      {
                        flag = true;
                      }
                    }
                    else if (billingCatId == (int)BillingCategoryType.Uatp)
                    {
                      var isUatpIgnore =
                        UatpReository.Single(u => u.MemberId == invoiceDetail.BillingMemberId).
                          ISUatpInvIgnoreFromDsproc;
                      if ((dsStatus == "Y" || dsStatus == "V") && isDigitalSignReqrd && isUatpIgnore)
                      {
                        flag = true;
                      }
                    }

                  }

                  if (((billedMemberStatus == MemberStatus.Terminated || isBillingMemberStatus == MemberStatus.Basic ||
                     isBillingMemberStatus == MemberStatus.Restricted ||
                     isBillingMemberStatus == MemberStatus.Terminated) || flag))
                  {
                    invoiceDetail.ValidationStatusId = (int)InvoiceValidationStatus.Failed;
                    invoiceDetail.InvoiceStatusId = (int)InvoiceStatusType.ErrorNonCorrectable;
                    // Update LastUpdatedOn field
                    invoiceDetail.LastUpdatedOn = DateTime.UtcNow;
                    // Update LasUpdatedBy fields
                    invoiceDetail.LastUpdatedBy = userId;

                    // Set Clearing House
                    invoiceDetail.ClearingHouse = clearingHouse;
                    
                    // Update Invoice Repository
                    InvoiceReository.Update(invoiceDetail);
                    continue;
                  }

                }

                // Set "IsLateSubmitted" flag to true
                invoiceDetail.IsLateSubmitted = true;
                if (isManulaControl)
                  invoiceDetail.LateSubmissionRequestStatusId = (int)LateSubmissionRequestStatus.Pending;
                else
                {
                  invoiceDetail.LateSubmissionRequestStatusId = (int)LateSubmissionRequestStatus.Accepted;
                  invoiceDetail.InvoiceStatusId = (int)InvoiceStatusType.ReadyForBilling;

                  //Later Used to check for legal archiving flag
                   invoiceAction.InvoiceStatus = (int)InvoiceStatusType.ReadyForBilling;

                  // Update the expiry period of transactions for purging purpose.
                  ProcessingDashboardInvoiceStatusRepository.UpdatePurgingExpiryPeriod(invoiceDetail.Id);

                  invoiceDetail.ValidationStatus = InvoiceValidationStatus.Completed;

                  /* SCP#340881 : Applying fix to update validation status date when
                 ACH/ICH Late Submission Manual is updated as ‘False’ */
                  invoiceDetail.ValidationDate = DateTime.UtcNow;
                }
                // Update LastUpdatedOn field
                invoiceDetail.LastUpdatedOn = DateTime.UtcNow;
                // Update LasUpdatedBy fields
                invoiceDetail.LastUpdatedBy = userId;
                // Set Clearing House
                invoiceDetail.ClearingHouse = clearingHouse;

                // Update Duplicate Coupon DU Mark
                InvoiceRepository.UpdateDuplicateCouponByInvoiceId(invoiceDetail.Id, invoiceDetail.BillingMemberId);

                // Update Invoice Repository
                InvoiceReository.Update(invoiceDetail);
                // Set ActionStatus to "Success"
                invoiceAction.ActionStatus = "Success";
                invoiceAction.InvoiceId = invoiceId;
                invoiceAction.ClearingHouseId = (int)ch;
              } // end if()
              else
              {
                if (invoiceDetail.ValidationStatusId != 4)
                  invoiceAction.ActionStatus = "Ignored:Invoice Not In Period Error";
                else if (invoiceDetail.BillingMonth != billingPeriodDetails.Month)
                  invoiceAction.ActionStatus = "Ignored:Invoice does not belong to current billing month";
                else if (invoiceDetail.IsLateSubmitted)
                  invoiceAction.ActionStatus = "Ignored:Invoice already submitted for late submission";
              }
              //IsLateAcceptanceAllowed = _calenderManager.IsLateSubmissionWindowOpen(ch, billingPeriodDetails);
            }
            else
              invoiceAction.ActionStatus = statusMsg;
          }// end else

          // Set Properties of ProcessingDashboardInvoiceActionStatus object
          invoiceAction.InvoiceId = invoiceId;
          invoiceAction.BillingMemberId = invoiceDetail.BilledMemberId;
          invoiceAction.BillingMemberCode = memberDetails.MemberCodeAlpha;
          invoiceAction.BillingMemberName = memberDetails.CommercialName;
          invoiceAction.InvoiceNo = invoiceDetail.InvoiceNumber;


        }// end if()
        else
        {
          // Get FormC details from repository
          var formCDetails = FormCRepository.Single(formc => formc.Id == invoiceId);

          // Retrieve Billing member details 
          var memberDetails = _memberManager.GetMember(formCDetails.FromMemberId);

          // Set Properties of ProcessingDashboardInvoiceActionStatus object
          invoiceAction.InvoiceId = invoiceId;
          invoiceAction.BillingMemberId = formCDetails.FromMemberId;
          invoiceAction.BillingMemberCode = memberDetails.MemberCodeAlpha;
          invoiceAction.BillingMemberName = memberDetails.CommercialName;
          invoiceAction.InvoiceNo = "Form C";
          invoiceAction.ActionStatus = "Ignored";
        }// end else

        // Add Invoice object to List
        invoiceList.Add(invoiceAction);

      }// end foreach()

      // Commit Invoice Repository changes
      UnitOfWork.CommitDefault();

      //if ACH/ICH Late Submission Manual is False
      foreach (var invoiceDetail in autoAcceptedInvoices)
      {
          //if (string.IsNullOrEmpty(item))
          //    continue;
          //// If SettlementMethodId belongs to ICH, check whether LateSubmissionWindow is open and LateAcceptance is allowed
          
          //// Convert item string to Guid
          //Guid invoiceId = new Guid(item);

          //// Get invoice details from Repository for specified InvoiceId
          //InvoiceBase invoiceDetail = InvoiceReository.Single(invoice => invoice.Id == invoiceId);

          // If Invoice exists perform operations to update LateSubmission flag, else check whether FORMC exists with that InvoiceId
          if (invoiceDetail != null)
          {
              bool IsLateAcceptanceAllowed = false;
              // Declare variable of type Billing period
              BillingPeriod billingPeriodDetails = new BillingPeriod();
              var billingMember = _memberManager.GetMember(invoiceDetail.BillingMemberId);
              var billedMember = _memberManager.GetMember(invoiceDetail.BilledMemberId);
              var clearingHouse = _referenceManager.GetClearingHouseForInvoice(invoiceDetail, billingMember, billedMember);
              var ch = new ClearingHouse();
              if (invoiceDetail.SettlementMethodId == (int)SMI.Ich)
              {
                  ch = ClearingHouse.Ich;
                  isManulaControl = isIchManulaControl;//Convert.ToBoolean(AdminSystem.SystemParameters.Instance.ICHDetails.ManualControlOnIchLateSubmission);
                  billingPeriodDetails = _calenderManager.GetLastClosedBillingPeriod(ch);
                  IsLateAcceptanceAllowed = _calenderManager.IsLateSubmissionWindowOpen(ch, billingPeriodDetails);
              }
              // If SettlementMethodId belongs to ACH, check whether LateSubmissionWindow is open and LateAcceptance is allowed
              else if (invoiceDetail.SettlementMethodId == (int)SMI.Ach || invoiceDetail.SettlementMethodId == (int)SMI.AchUsingIataRules)
              {
                  ch = ClearingHouse.Ach;
                  isManulaControl = isAchManulaControl;// Convert.ToBoolean(SystemParameters.Instance.ACHDetails.ManualControlOnACHLateSubmission);
                  billingPeriodDetails = _calenderManager.GetLastClosedBillingPeriod(ch);
                  IsLateAcceptanceAllowed = _calenderManager.IsLateSubmissionWindowOpen(ch, billingPeriodDetails);
              }
              if (IsLateAcceptanceAllowed && !isManulaControl && invoiceDetail.InvoiceStatusId == (int)InvoiceStatusType.ReadyForBilling)
              {
                  if (invoiceDetail.SubmissionMethodId == (int) SubmissionMethod.IsWeb)
                  {
                      InvoiveManager.UpdateMemberLocationInformationForLateSub(invoiceDetail);
                      InvoiceRepository.UpdateSourceCodeTotalVat(invoiceDetail.Id);
                  }
                  else
                  {
                      invoiceDetail.MemberLocationInformation = MemberLocationRepo.Get(l => l.InvoiceId == invoiceDetail.Id).ToList();
                  }
                  
                  InvoiveManager.UpdateInvoiceDetailsForLateSubmission(invoiceDetail, billingMember, billedMember);
                  
                  // call procedure UpdateInvoiceOnReadyForBilling after acepting invoice.
                  InvoiceRepository.UpdateInvoiceOnReadyForBilling(invoiceDetail.Id,
                                                                   invoiceDetail.BillingCategoryId,
                                                                   invoiceDetail.BillingMemberId,
                                                                   invoiceDetail.BilledMemberId,
                                                                   invoiceDetail.BillingCode);

                  // SCP186155
                  // Close correspondnece if Invoice contains BM on Corr (i.e. reason code '6A'/'6B')
                  InvoiceRepository.CloseCorrespondenceOnInvoiceReadyForBilling(invoiceDetail.Id, invoiceDetail.BillingCategoryId);
              }

          }
      }// end foreach()
      // send mail for late submission
      //foreach (var id in invoiceList.Where(i => i.ActionStatus == "Success").Select(f => f.BillingMemberId).Distinct())
      //{
      //  var result = invoiceList.Where(f => f.BillingMemberId == id && f.ActionStatus == "Success");
      //  var invoiceCount = result.Count();
      //  string memberName = result.Take(1).First().BillingMemberName;
      //  ClearingHouse clearingHouse = _memberManager.GetMemberCategory(memberId);
      //  SendMailForLateSubmissionOnProcessingDashboard(invoiceCount, memberName, clearingHouse, EmailTemplateId.InvoiceLateSubmission);
      //}

        //Update Legal Archive Flags 
        //foreach (var actionStatuse in invoiceList) //Concerns with sujit.
        //{
        //    if ((actionStatuse.ActionStatus == "Success") && (actionStatuse.InvoiceStatus == (int)InvoiceStatusType.ReadyForBilling))
        //    {
        //        InvoiceRepository.UpdateInvoiceAndSetLaParameters(actionStatuse.InvoiceId);
        //    }
        //}

        if (!isMarkfileForLateSubmission)
        {
            var member = _memberManager.GetMember(memberId);
            
            
            string memberName = string.Format("{0}-{1}-{2}", member.MemberCodeNumeric, member.MemberCodeAlpha, member.CommercialName);
            
            if (isIchManulaControl)
            {
                // for ich clearing house invoices 
                var count = invoiceList.Where(i => i.ActionStatus == "Success" && i.ClearingHouseId == 2).Count();
                if (count > 0) SendMailForLateSubmissionOnProcessingDashboard(count, memberName, ClearingHouse.Ich, EmailTemplateId.InvoiceLateSubmission);

            }
            if (isAchManulaControl)
            {
                // for ach clearing house invoices 
                var count = invoiceList.Where(i => i.ActionStatus == "Success" && i.ClearingHouseId == 1).Count();
                if (count > 0) SendMailForLateSubmissionOnProcessingDashboard(count, memberName, ClearingHouse.Ach, EmailTemplateId.InvoiceLateSubmission);
            }
        }
        // Return Invoice list 
      return invoiceList.ToList();
    }// end MarkInvoiceForLateSubmission()


    /// <summary>
    /// Following method is used to update LateSubmission flag to true for Invoices in selected Files
    /// </summary>
    /// <param name="selectedFileIds">comma separated string of FileIds</param>
    /// <param name="memberId">member id of logged in user</param>
    /// <param name="userId">user id of logged in user</param>
    /// <returns>List of Files with ActionStatus i.e. Success or Failure</returns>
    public List<ProcessingDashboardFileActionStatus> MarkFileForLateSubmission(string selectedFileIds, int memberId, int userId)
    {

      var isICHLateAcceptanceAllowed = 0;
      var isACHLateAcceptanceAllowed = 0;
        bool isManulaControl = true;
      //SCP51007 : SRM 4.5 - Pending Late Submissions alert 
      //Get the system parameter values of ICH & ACH Manual Control
      bool isAchManulaControl = Convert.ToBoolean(SystemParameters.Instance.ACHDetails.ManualControlOnACHLateSubmission);
      bool isIchManulaControl = Convert.ToBoolean(SystemParameters.Instance.ICHDetails.ManualControlOnIchLateSubmission); 
      // Declare variable of type Billing period
      BillingPeriod currentBillingPeriodDetails;


      currentBillingPeriodDetails = _calenderManager.GetLastClosedBillingPeriod(ClearingHouse.Ich);

      if (_calenderManager.IsLateSubmissionWindowOpen(ClearingHouse.Ich, currentBillingPeriodDetails))
        isICHLateAcceptanceAllowed = 1;

      currentBillingPeriodDetails = _calenderManager.GetLastClosedBillingPeriod(ClearingHouse.Ach);

      if (_calenderManager.IsLateSubmissionWindowOpen(ClearingHouse.Ach, currentBillingPeriodDetails))
        isACHLateAcceptanceAllowed = 1;


      //get an instance of Processing Dashboard repository
      var processingDashboardRepository = Ioc.Resolve<IProcessingDashboardFileStatusRepository>(typeof(IProcessingDashboardFileStatusRepository));
      // Get file details by executing stored procedure
      var fileActionStatusList = new List<ProcessingDashboardFileActionStatus>();

      // Declare variable of type StringBuilder
      var oracleGuidBuilder = new StringBuilder();

      // Iterate through file Id's and split on ','
      foreach (var netGuid in selectedFileIds.Split(','))
      {
        if (string.IsNullOrEmpty(netGuid))
          continue;
        // Call "ConvertNetGuidToOracleGuid()" method which will return string 
        oracleGuidBuilder.Append(ConvertNetGuidToOracleGuid(netGuid));
        oracleGuidBuilder.Append(",");
      

      }// end foreach()

      // Declare string variable
      string oracleGuid;
      // Set string variable to Guid in Oracle format
      oracleGuid = oracleGuidBuilder.ToString();
      // Trim last ',' of comma separated GUID string
      oracleGuid = oracleGuid.TrimEnd(',');

      // SCP ID : 67444 - ACH late submissions need manual approval
     // Implemented the late submission approval as Auto/Manual
      fileActionStatusList = processingDashboardRepository.MarkInvoicesForLateSubmissionWithinFile(oracleGuid,
          currentBillingPeriodDetails.Year, currentBillingPeriodDetails.Month,
          currentBillingPeriodDetails.Period, userId, isICHLateAcceptanceAllowed, isACHLateAcceptanceAllowed, isAchManulaControl, isIchManulaControl);

      //SCP51007 : SRM 4.5 - Pending Late Submissions alert 
      // Get Member Details
      var member = _memberManager.GetMember(memberId);
        string memberName = string.Empty;
        if(member!=null)
        memberName = string.Format("{0}-{1}-{2}", member.MemberCodeNumeric, member.MemberCodeAlpha, member.CommercialName);

      // send mail according to invoice seettlement method.
      foreach (var file in fileActionStatusList)
      {
        if (file.NumberOfActions <= 0) continue;

        //SCP51007 : SRM 4.5 - Pending Late Submissions alert 
        // get all late submitted invoices within a file.
         var invoices =
          InvoiceReository.Get(i => i.IsInputFileId == file.FileId && i.IsLateSubmitted == true);

          foreach (var invoiceBase in invoices)
          {

               bool IsLateAcceptanceAllowed = false;
              // Declare variable of type Billing period
              BillingPeriod billingPeriodDetails = new BillingPeriod();
              var billingMember = _memberManager.GetMember(invoiceBase.BillingMemberId);
              var billedMember = _memberManager.GetMember(invoiceBase.BilledMemberId);
              var ch = new ClearingHouse();
              if (invoiceBase.SettlementMethodId == (int)SMI.Ich || invoiceBase.SettlementMethodId == (int)SMI.IchSpecialAgreement)
              {
                  ch = ClearingHouse.Ich;
                  isManulaControl = isIchManulaControl;//Convert.ToBoolean(AdminSystem.SystemParameters.Instance.ICHDetails.ManualControlOnIchLateSubmission);
                  billingPeriodDetails = _calenderManager.GetLastClosedBillingPeriod(ch);
                  IsLateAcceptanceAllowed = _calenderManager.IsLateSubmissionWindowOpen(ch, billingPeriodDetails);
              }
              // If SettlementMethodId belongs to ACH, check whether LateSubmissionWindow is open and LateAcceptance is allowed
              else if (invoiceBase.SettlementMethodId == (int)SMI.Ach || invoiceBase.SettlementMethodId == (int)SMI.AchUsingIataRules)
              {
                  ch = ClearingHouse.Ach;
                  isManulaControl = isAchManulaControl;// Convert.ToBoolean(SystemParameters.Instance.ACHDetails.ManualControlOnACHLateSubmission);
                  billingPeriodDetails = _calenderManager.GetLastClosedBillingPeriod(ch);
                  IsLateAcceptanceAllowed = _calenderManager.IsLateSubmissionWindowOpen(ch, billingPeriodDetails);
              }
              if (IsLateAcceptanceAllowed && !isManulaControl && invoiceBase.InvoiceStatusId == (int)InvoiceStatusType.ReadyForBilling)
              {
                  if (invoiceBase.SubmissionMethodId == (int) SubmissionMethod.IsWeb)
                  {
                      InvoiveManager.UpdateMemberLocationInformationForLateSub(invoiceBase);
                      InvoiceRepository.UpdateSourceCodeTotalVat(invoiceBase.Id);
                  }
                  else
                  {
                      invoiceBase.MemberLocationInformation = MemberLocationRepo.Get(l => l.InvoiceId == invoiceBase.Id).ToList();
                  }
                  
                  InvoiveManager.UpdateInvoiceDetailsForLateSubmission(invoiceBase, billingMember, billedMember);

                  // call procedure UpdateInvoiceOnReadyForBilling after acepting invoice.
                  InvoiceRepository.UpdateInvoiceOnReadyForBilling(invoiceBase.Id,
                                                                   invoiceBase.BillingCategoryId,
                                                                   invoiceBase.BillingMemberId,
                                                                   invoiceBase.BilledMemberId,
                                                                   invoiceBase.BillingCode);
              }

          }


 
          

        // get late submiited invoice to ach
        var achInvoices =
          invoices.Where(
            n =>
            n.SettlementMethodId == (int)SettlementMethodValues.Ach ||
            n.SettlementMethodId == (int)SettlementMethodValues.AchUsingIATARules).ToList();
        //SCP51007 : SRM 4.5 - Pending Late Submissions alert 
        //Send mail to ICH or ACH Ops only when ICH & ACH is Manual Control and invoice cout is greater than zero.
        if (achInvoices.Count() > 0 && isAchManulaControl)
          SendMailForLateSubmissionOnProcessingDashboard(achInvoices.Count(), memberName, ClearingHouse.Ach, EmailTemplateId.InvoiceLateSubmission);

        // get late submiited invoice to ich
        var ichInvoices = invoices.Where(n => n.SettlementMethodId == (int)SettlementMethodValues.Ich).ToList();
        if (ichInvoices.Count() > 0 && isIchManulaControl)
          SendMailForLateSubmissionOnProcessingDashboard(ichInvoices.Count(), memberName, ClearingHouse.Ich, EmailTemplateId.InvoiceLateSubmission);
      }

      // Return File List
      return fileActionStatusList;
    }// end MarkInvoiceForLateSubmission()


    public bool validateVoidPeriod(InvoiceBase invoice)
    {
        var clearingHouse = _referenceManager.GetClearingHouseToFetchCurrentBillingPeriod(invoice.SettlementMethodId);
        // var tt = CalendarManager.GetCurrentBillingPeriod(clearingHouse);
        BillingPeriod currentPeriod;

        try
        {

            // Try to get the current billing period.
            currentPeriod = _calenderManager.GetCurrentBillingPeriod(clearingHouse); //GetCurrentBillingPeriod(clearingHouse);
        }
        catch (ISCalendarDataNotFoundException)
        {
            //// Current billing period not found, try to get the next billing period.
            //var previousBillingPeriod = _calenderManager.GetLastClosedBillingPeriod(clearingHouse);
            //if (!_calenderManager.IsLateSubmissionWindowOpen(clearingHouse, previousBillingPeriod))
            //{
            //    return false;
            //}
            //else
            //{
            //    return true;
            //}

            return false;
        }
        return true;

    }

    /// <summary>
    /// Following method is used to Increment BillingPeriod for selected Invoices.
    /// </summary>
    /// <param name="invoiceIdArray">Array of InvoiceId's</param>
    /// <returns>List of Invoices with ActionStatus i.e. Success or Failure</returns>
    public List<ProcessingDashboardInvoiceActionStatus> IncrementInvoiceBillingPeriod(string[] invoiceIdArray, int userId)
    {
        var logRefId = Guid.NewGuid();
        var log = _referenceManager.GetDebugLog(DateTime.Now, "IncrementInvoiceBillingPeriod-Manager", this.ToString(),
                                   "ProcessingDashboard", "Stage 1: IncrementInvoiceBillingPeriod-Manager start", userId, logRefId.ToString());
        _referenceManager.LogDebugData(log);
      // Create List of type ProcessingDashboardInvoiceActionStatus
      List<ProcessingDashboardInvoiceActionStatus> invoiceList = new List<ProcessingDashboardInvoiceActionStatus>();
      List<InvoiceBase> successInvoiceList = new List<InvoiceBase>();

      //SCP:170853 IS-Web response time - ICH Ops 
      //Dictionarey declartion to store invoice member details.
      //Billed and Billing Member details are retrieved and stored into Dictionary in first iteration. 
      //In next iteration member id is checked in Dictionary. if exists then retrieve the member details from dictionary
      //otherwise retrieve from database and store into Dictionary.
        Dictionary<int,Member> memberList=new Dictionary<int, Member>();
        
      // Iterate through InvoiceIdArray and retrieve Invoices
        var logCounter = 0;
      foreach (string item in invoiceIdArray)
      {

        if (String.IsNullOrEmpty(item))
          continue;

        // Convert item to Guid
        Guid invoiceId = new Guid(item);

            log = _referenceManager.GetDebugLog(DateTime.Now, "IncrementInvoiceBillingPeriod-Manager", this.ToString(),
                                                "ProcessingDashboard",
                                                "Stage 2: InvoiceReository.Single start", userId,
                                                logRefId.ToString());
            _referenceManager.LogDebugData(log);
        
          // Get invoice details from Repository for specified InvoiceId
        InvoiceBase invoiceDetail = InvoiceReository.Single(invoice => invoice.Id == invoiceId);


        log = _referenceManager.GetDebugLog(DateTime.Now, "IncrementInvoiceBillingPeriod-Manager", this.ToString(),
                                             "ProcessingDashboard",
                                             "Stage 2: InvoiceReository.Single completed", userId,
                                             logRefId.ToString());
        _referenceManager.LogDebugData(log);

        // Create instance of type ProcessingDashboardInvoiceActionStatus
        ProcessingDashboardInvoiceActionStatus invoiceAction = new ProcessingDashboardInvoiceActionStatus();

        // If Invoice exists perform operations to update LateSubmission flag, else check whether FORMC exists for that InvoiceId
        if (invoiceDetail != null)
        {
          // Declare variable of type Billing period
          BillingPeriod billingPeriodDetails = new BillingPeriod();

            Member memberDetails;
            //SCP:170853 IS-Web response time - ICH Ops 
            //check where member details are already retrieved or not 
            if(memberList.ContainsKey(invoiceDetail.BillingMemberId))
            {
                memberDetails = memberList[invoiceDetail.BillingMemberId];
            }
            else
            {
                // Retrieve Billing member details and add to collection 
                
                 memberDetails = _memberManager.GetMember(invoiceDetail.BillingMemberId);
                 memberList.Add(invoiceDetail.BillingMemberId,memberDetails);
            }
          

          log = _referenceManager.GetDebugLog(DateTime.Now, "IncrementInvoiceBillingPeriod-Manager", this.ToString(),
                                          "ProcessingDashboard",
                                          "Stage 3: GetMember completed", userId,
                                          logRefId.ToString());
          _referenceManager.LogDebugData(log);

          // Ignore if invoice in "Ready For Billing" or "Claimed" or "Processing Completed" or "Presented" state.
          if (invoiceDetail.InvoiceStatus == InvoiceStatusType.ReadyForBilling || invoiceDetail.InvoiceStatus == InvoiceStatusType.Claimed || invoiceDetail.InvoiceStatus == InvoiceStatusType.ProcessingComplete || invoiceDetail.InvoiceStatus == InvoiceStatusType.Presented)
          {

            invoiceAction.ActionStatus = "Ignored";
          }
          else
          {
            //If Invoice SettlementMethod == Bilateral or AdjustmentDueToProtest, Ignore that Invoice, else perform LateSubmission operations
              /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
            if ((_referenceManager.IsSmiLikeBilateral(invoiceDetail.SettlementMethodId, false)) || invoiceDetail.SettlementMethodId == (int)SMI.AdjustmentDueToProtest)
            {
              invoiceAction.ActionStatus = "Ignored";
            }// end if()



            bool isDigitalSignReqrd = memberDetails.DigitalSignApplication;
            string dsStatus = string.Empty;
            if (!string.IsNullOrEmpty(invoiceDetail.DsStatus))
              dsStatus = invoiceDetail.DsStatus.ToUpper();
            bool flag = false;

           
                log = _referenceManager.GetDebugLog(DateTime.Now, "IncrementInvoiceBillingPeriod-Manager", this.ToString(),
                                                    "ProcessingDashboard",
                                                    "Stage 7:  Invoice revalidation start", userId,
                                                    logRefId.ToString());
                _referenceManager.LogDebugData(log);
          
            // revalidate invoice
            if ((dsStatus == "Y" || dsStatus == "V") && !isDigitalSignReqrd)
              flag = true;
            else
            {
              var billingCatId = invoiceDetail.BillingCategoryId;
              if (billingCatId == (int)BillingCategoryType.Pax || billingCatId == (int)BillingCategoryType.Cgo
                || billingCatId == (int)BillingCategoryType.Misc)
              {
                if ((dsStatus == "Y" || dsStatus == "V") && isDigitalSignReqrd)
                {
                  flag = true;
                }
              }
              else if (billingCatId == (int)BillingCategoryType.Uatp)
              {
                var isUatpIgnore =
                  UatpReository.Single(u => u.MemberId == invoiceDetail.BillingMemberId).
                    ISUatpInvIgnoreFromDsproc;
                if ((dsStatus == "Y" || dsStatus == "V") && isDigitalSignReqrd && isUatpIgnore)
                {
                  flag = true;
                }
              }

            }

           
                log = _referenceManager.GetDebugLog(DateTime.Now, "IncrementInvoiceBillingPeriod-Manager", this.ToString(),
                                                    "ProcessingDashboard",
                                                    "Stage 7:  Invoice revalidation completed", userId,
                                                    logRefId.ToString());
                _referenceManager.LogDebugData(log);
           

            var billingMemberStatus = memberDetails.IsMembershipStatusId;

            //SCP:170853 IS-Web response time - ICH Ops 
           //check where member details are already retrieved or not 
            if (!memberList.ContainsKey(invoiceDetail.BilledMemberId))
            {//add member to collection
                memberList.Add(invoiceDetail.BilledMemberId, _memberManager.GetMember(invoiceDetail.BilledMemberId));
            }
            
              //var billedMemberStatus = _memberManager.GetMember(invoiceDetail.BilledMemberId).IsMembershipStatusId;
            var billedMemberStatus = memberList[invoiceDetail.BilledMemberId].IsMembershipStatusId;


              //check where member is having parent id>0 then only populate parent member  details 
              var billingFinalParentId = invoiceDetail.BillingMemberId; //default set to billing parent id 
              if(memberList[invoiceDetail.BillingMemberId].ParentMemberId>0)
              {
                  //retriev the parent details 
                  billingFinalParentId = _memberManager.GetFinalParentDetails(invoiceDetail.BillingMemberId);
              }
              //now check where the billingFinalParentId details exists in member list 
              if(!memberList.ContainsKey(billingFinalParentId))
              {
                  //retrieve  the billingFinalParent from db and also add to collection
                 memberList.Add(billingFinalParentId,_memberManager.GetMember(billingFinalParentId));
              }

              //Populate billed parent details 

              var billedFinalParentId = invoiceDetail.BilledMemberId; //default set to billing parent id 
              if (memberList[invoiceDetail.BilledMemberId].ParentMemberId > 0)
              {
                  //retriev the parent details 
                  billedFinalParentId = _memberManager.GetFinalParentDetails(invoiceDetail.BilledMemberId);
              }
              //now check where the billingFinalParentId details exists in member list 
              if (!memberList.ContainsKey(billedFinalParentId))
              {
                  //retrieve  the billingFinalParent from db and also add to collection
                  memberList.Add(billedFinalParentId, _memberManager.GetMember(billedFinalParentId));
              }


             // Get Final Parent Details for SMI, Currency, Clearing House abd Suspended Flag validations
          //  var billingFinalParent = _memberManager.GetMember(_memberManager.GetFinalParentDetails(invoiceDetail.BillingMemberId));
           // var billedFinalParent = _memberManager.GetMember(_memberManager.GetFinalParentDetails(invoiceDetail.BilledMemberId));

              //Populate the final parent details from member list 
              var billingFinalParent = memberList[billingFinalParentId];
              var billedFinalParent = memberList[billedFinalParentId]; 
           
                log = _referenceManager.GetDebugLog(DateTime.Now, "IncrementInvoiceBillingPeriod-Manager", this.ToString(),
                                                    "ProcessingDashboard",
                                                    "Stage 8:  *2 GetFinalParentDetails completed", userId,
                                                    logRefId.ToString());
                _referenceManager.LogDebugData(log);
                //CMP #624: ICH Rewrite-New SMI X : Validate SMI
                // SCP177435 - EXCHANGE RATE 
                if ((billedMemberStatus == (int)MemberStatus.Terminated || billingMemberStatus == (int)MemberStatus.Basic ||
                  billingMemberStatus == (int)MemberStatus.Restricted || billingMemberStatus == (int)MemberStatus.Terminated) || flag ||
                  (invoiceDetail.SettlementMethodId == (int)SMI.IchSpecialAgreement ? !ValidateSettlementMethodX(invoiceDetail, billingFinalParent, billedFinalParent) : !ValidateSettlementMethod(invoiceDetail, billingFinalParent, billedFinalParent)) ||
                                     !ValidateBillingCurrency(invoiceDetail, billingFinalParent, billedFinalParent) ||
                                     !ValidateListingCurrency(invoiceDetail, billingFinalParent, billedFinalParent))
                {
                  invoiceDetail.ValidationStatusId = (int) InvoiceValidationStatus.Failed;
                  invoiceDetail.InvoiceStatusId = (int) InvoiceStatusType.ErrorNonCorrectable;
                  // Update LastUpdatedOn field
                  invoiceDetail.LastUpdatedOn = DateTime.UtcNow;
                  // Update LasUpdatedBy fields
                  invoiceDetail.LastUpdatedBy = userId;


                  log = _referenceManager.GetDebugLog(DateTime.Now,
                                                      "IncrementInvoiceBillingPeriod-Manager",
                                                      this.ToString(),
                                                      "ProcessingDashboard",
                                                      "Stage 9: InvoiceReository.Update start",
                                                      userId,
                                                      logRefId.ToString());
                  _referenceManager.LogDebugData(log);


                  // Update Invoice Repository
                  InvoiceReository.Update(invoiceDetail);

                  // Set ActionStatus to "Failed"
                  invoiceAction.InvoiceId = invoiceId;
                  invoiceAction.BillingMemberId = invoiceDetail.BilledMemberId;
                  invoiceAction.BillingMemberCode = memberDetails.MemberCodeAlpha;
                  invoiceAction.BillingMemberName = memberDetails.CommercialName;
                  invoiceAction.InvoiceNo = invoiceDetail.InvoiceNumber;
                  invoiceAction.ActionStatus = "Failed";

                  invoiceList.Add(invoiceAction);

                  log = _referenceManager.GetDebugLog(DateTime.Now, "IncrementInvoiceBillingPeriod-Manager", this.ToString(), "ProcessingDashboard", "Stage 10: Continue", userId, logRefId.ToString());
                  _referenceManager.LogDebugData(log);

                  continue;
                }

            // end revalidation 

            if (memberDetails.IchMemberStatus)
              // Call GetCurrentBillingPeriod() method which will return Current billing details
              billingPeriodDetails = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
            else if (memberDetails.AchMemberStatus)
              // Call GetCurrentBillingPeriod() method which will return Current billing details
                billingPeriodDetails = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ach);
            else if (!memberDetails.IchMemberStatus || !memberDetails.AchMemberStatus)
              // Call GetCurrentBillingPeriod() method which will return Current billing details
                billingPeriodDetails = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
            else
              // Call GetCurrentBillingPeriod() method which will return Current billing details
                billingPeriodDetails = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);


            if (!validateVoidPeriod(invoiceDetail))
            {
               //webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoiceGuid, CargoErrorCodes.VoidPeriodValidationMsg));
               invoiceAction.ActionStatus = "The new Billing Period is not yet open. Please check IS Calendar and try again after the Submission Open timeline.";
            }
            else
            {
                
            
            // Update Invoice which has ValidationStatusId == 4, BillingPeriod != 4, 
            // BillingMonth == CurrentBillingMonth and BillingYear == CurrentBillingYear
            if (invoiceDetail.ValidationStatusId == 4 && invoiceDetail.BillingPeriod != 4 &&
                invoiceDetail.BillingMonth == billingPeriodDetails.Month &&
                invoiceDetail.BillingYear == billingPeriodDetails.Year)
            {

                 log = _referenceManager.GetDebugLog(DateTime.Now, "IncrementInvoiceBillingPeriod-Manager", this.ToString(),
                                                        "ProcessingDashboard",
                                                        "Stage 11: UpdateDuplicateCouponByInvoiceId start", userId,
                                                        logRefId.ToString());
                    _referenceManager.LogDebugData(log);
              

              // Update Duplicate Coupon DU Mark
              InvoiceRepository.UpdateDuplicateCouponByInvoiceId(invoiceDetail.Id, invoiceDetail.BillingMemberId);

 log = _referenceManager.GetDebugLog(DateTime.Now, "IncrementInvoiceBillingPeriod-Manager", this.ToString(),
                                                      "ProcessingDashboard",
                                                      "Stage 11: UpdateDuplicateCouponByInvoiceId completed", userId,
                                                      logRefId.ToString());
                  _referenceManager.LogDebugData(log);
            

              // Set "BillingPeriod" to current billing period
              invoiceDetail.BillingPeriod = billingPeriodDetails.Period;

              //var billingMember = _memberManager.GetMemberDetails(invoiceDetail.BillingMemberId);
              //var billedMember = _memberManager.GetMemberDetails(invoiceDetail.BilledMemberId);

              //var clearingHouse = _referenceManager.GetClearingHouseForInvoice(invoiceDetail, billingMember, billedMember);

              //// Update clearing house of invoice
              //invoiceDetail.ClearingHouse = clearingHouse;

              //invoiceDetail.SettlementFileStatus = InvoiceProcessStatus.Pending;
              //invoiceDetail.SettlementFileStatusId = (int)InvoiceProcessStatus.Pending;

              // Set ValidationStatus to Completed
              invoiceDetail.ValidationStatusId = (int)InvoiceValidationStatus.Completed;
              // Set ValidationStatusDate to current date
              invoiceDetail.ValidationDate = DateTime.UtcNow;
              invoiceDetail.IsLateSubmitted = false;
              // Update LastUpdatedBy field
              invoiceDetail.LastUpdatedBy = userId;
              // Update LastUpdatedOn field
              invoiceDetail.LastUpdatedOn = DateTime.UtcNow;
              // Set InvoiceStatus Id = 6 (i.e. ready for billing)
              invoiceDetail.InvoiceStatusId = (int)InvoiceStatusType.ReadyForBilling;
              // Update Invoice Repository
              InvoiceReository.Update(invoiceDetail);

                log = _referenceManager.GetDebugLog(DateTime.Now, "IncrementInvoiceBillingPeriod-Manager", this.ToString(),
                                                      "ProcessingDashboard",
                                                      "Stage 12: InvoiceReository.Update comnpleted", userId,
                                                      logRefId.ToString());
                  _referenceManager.LogDebugData(log);
             

              // Set ActionStatus to "Success"
              invoiceAction.ActionStatus = "Success";
              successInvoiceList.Add(invoiceDetail);
            } // end if()
            else
              invoiceAction.ActionStatus = "Ignored";

            }//end void period else
          }
          

          // Set Properties of ProcessingDashboardInvoiceActionStatus object
          invoiceAction.InvoiceId = invoiceId;
          invoiceAction.BillingMemberId = invoiceDetail.BilledMemberId;
          invoiceAction.BillingMemberCode = memberDetails.MemberCodeAlpha;
          invoiceAction.BillingMemberName = memberDetails.CommercialName;
          invoiceAction.InvoiceNo = invoiceDetail.InvoiceNumber;
        }// end if()
        else
        {
              log = _referenceManager.GetDebugLog(DateTime.Now, "IncrementInvoiceBillingPeriod-Manager", this.ToString(),
                                                    "ProcessingDashboard",
                                                    "Stage 3: FormCRepository.Single start", userId,
                                                    logRefId.ToString());
                _referenceManager.LogDebugData(log);
           


          // Get FormC details from repository
          var formCDetails = FormCRepository.Single(formc => formc.Id == invoiceId);

           log = _referenceManager.GetDebugLog(DateTime.Now, "IncrementInvoiceBillingPeriod-Manager", this.ToString(),
                                                  "ProcessingDashboard",
                                                  "Stage 3: FormCRepository.Single completed", userId,
                                                  logRefId.ToString());
              _referenceManager.LogDebugData(log);
         

          // Retrieve Billing member details 
          var memberDetails = _memberManager.GetMember(formCDetails.FromMemberId);

              log = _referenceManager.GetDebugLog(DateTime.Now, "IncrementInvoiceBillingPeriod-Manager", this.ToString(),
                                                  "ProcessingDashboard",
                                                  "Stage 4:  _memberManager.GetMember completed", userId,
                                                  logRefId.ToString());
              _referenceManager.LogDebugData(log);
        

          // Set Properties of ProcessingDashboardInvoiceActionStatus object
          invoiceAction.InvoiceId = invoiceId;
          invoiceAction.BillingMemberId = formCDetails.FromMemberId;
          invoiceAction.BillingMemberCode = memberDetails.MemberCodeAlpha;
          invoiceAction.BillingMemberName = memberDetails.CommercialName;
          invoiceAction.InvoiceNo = "Form C";
          invoiceAction.ActionStatus = "Ignored";
        }// end else

        // Add Invoice object to List
        invoiceList.Add(invoiceAction);

          //increment log_counter
          logCounter++;
      }// end foreach()

      
          log = _referenceManager.GetDebugLog(DateTime.Now, "IncrementInvoiceBillingPeriod-Manager", this.ToString(),
                                              "ProcessingDashboard",
                                              "Stage 3:  foreach  completed . Total Count is " + logCounter, userId,
                                              logRefId.ToString());
          _referenceManager.LogDebugData(log);
      
      // Commit Invoice Repository changes
      UnitOfWork.CommitDefault();
        foreach (var invoicedet in successInvoiceList)
        {
            if (invoicedet.SubmissionMethodId == (int)SubmissionMethod.IsWeb)
            {
                log = _referenceManager.GetDebugLog(DateTime.Now, "IncrementInvoiceBillingPeriod-Manager", this.ToString(),
                                            "ProcessingDashboard",
                                            "Stage 13:  UpdateMemberLocationInformationForLateSub start", userId,
                                            logRefId.ToString());
                _referenceManager.LogDebugData(log);

                InvoiveManager.UpdateMemberLocationInformationForLateSub(invoicedet);

                log = _referenceManager.GetDebugLog(DateTime.Now, "IncrementInvoiceBillingPeriod-Manager", this.ToString(),
                                          "ProcessingDashboard",
                                          "Stage 13:  UpdateMemberLocationInformationForLateSub completed", userId,
                                          logRefId.ToString());
                _referenceManager.LogDebugData(log);

                InvoiceRepository.UpdateSourceCodeTotalVat(invoicedet.Id);

                log = _referenceManager.GetDebugLog(DateTime.Now, "IncrementInvoiceBillingPeriod-Manager", this.ToString(),
                                         "ProcessingDashboard",
                                         "Stage 14:  UpdateSourceCodeTotalVat completed", userId,
                                         logRefId.ToString());
                _referenceManager.LogDebugData(log);
            }
            else
            {
                invoicedet.MemberLocationInformation = MemberLocationRepo.Get(l => l.InvoiceId == invoicedet.Id).ToList();
            }
          
            //var billingMember = _memberManager.GetMemberDetails(invoicedet.BillingMemberId);
            //var billedMember = _memberManager.GetMemberDetails(invoicedet.BilledMemberId);

            var billingMember = memberList[invoicedet.BillingMemberId];
            var billedMember = memberList[invoicedet.BilledMemberId];

            log = _referenceManager.GetDebugLog(DateTime.Now, "IncrementInvoiceBillingPeriod-Manager", this.ToString(),
                                        "ProcessingDashboard",
                                        "Stage 15:  UpdateInvoiceDetailsForLateSubmission start", userId,
                                        logRefId.ToString());
            _referenceManager.LogDebugData(log);

            InvoiveManager.UpdateInvoiceDetailsForLateSubmission(invoicedet, billingMember, billedMember);

            log = _referenceManager.GetDebugLog(DateTime.Now, "IncrementInvoiceBillingPeriod-Manager", this.ToString(),
                                       "ProcessingDashboard",
                                       "Stage 15:  UpdateInvoiceDetailsForLateSubmission completed", userId,
                                       logRefId.ToString());
            _referenceManager.LogDebugData(log);


            // call procedure UpdateInvoiceOnReadyForBilling after acepting invoice.
            InvoiceRepository.UpdateInvoiceOnReadyForBilling(invoicedet.Id, invoicedet.BillingCategoryId, invoicedet.BillingMemberId,
                                                             invoicedet.BilledMemberId, invoicedet.BillingCode);


            log = _referenceManager.GetDebugLog(DateTime.Now, "IncrementInvoiceBillingPeriod-Manager", this.ToString(),
                                       "ProcessingDashboard",
                                       "Stage 16:  UpdateInvoiceOnReadyForBilling completed", userId,
                                       logRefId.ToString());
            _referenceManager.LogDebugData(log);

            //InvoiceRepository.UpdateInvoiceAndSetLaParameters(invoicedet.Id);

            //log = _referenceManager.GetDebugLog(DateTime.Now, "IncrementInvoiceBillingPeriod-Manager", this.ToString(),
            //                          "ProcessingDashboard",
            //                          "Stage 17:  UpdateInvoiceAndSetLaParameters completed", userId,
            //                          logRefId.ToString());
            _referenceManager.LogDebugData(log);


        }
        
            log = _referenceManager.GetDebugLog(DateTime.Now, "IncrementInvoiceBillingPeriod-Manager", this.ToString(),
                                                "ProcessingDashboard",
                                                "Stage 12:  Before return ", userId,
                                                logRefId.ToString());
            _referenceManager.LogDebugData(log);
        
      // Return Invoice list 
      return invoiceList.ToList();
    }// end IncrementInvoiceBillingPeriod()

    /// <summary>
    /// Following method is used to Delete selected Invoices.
    /// </summary>
    /// <param name="invoiceIdArray">Array of InvoiceId's</param>
    /// <returns>List of Invoices with ActionStatus i.e. Success or Failure</returns>
    public List<ProcessingDashboardInvoiceActionStatus> DeleteSelectedInvoices(string[] invoiceIdArray,int dummyMemberId, int userId)
    {

        // Create List of type ProcessingDashboardInvoiceActionStatus
        var invoiceList = new List<ProcessingDashboardInvoiceActionStatus>();

        // Declare variable of type StringBuilder
        var guidBuilder = new StringBuilder();

        // Iterate through comma separated list of fileIds 
        foreach (string fileId in invoiceIdArray)
        {
            if (String.IsNullOrEmpty(fileId))
                continue;

            // Call ConvertNetGuidToOracleGuid() method which will convert .Net Guid to Oracle Guid
            guidBuilder.Append(ConvertNetGuidToOracleGuid(fileId));
            // Separate fileId using ","
            guidBuilder.Append(",");
        }// end foreach()

        // Convert guidBuilder to string and set it to string variable
        string invoiceIdStringInOracleFormat = guidBuilder.ToString();
        // Remove ',' at the end of invoiceIdStringInOracleFormat(i.e. We get invoice id string from .net Guid format to Oracle Guid format)
        invoiceIdStringInOracleFormat = invoiceIdStringInOracleFormat.TrimEnd(',');

        //get an instance of Processing Dashboard repository
        var processingDashboardRepository = Ioc.Resolve<IProcessingDashboardFileStatusRepository>(typeof(IProcessingDashboardFileStatusRepository));

        // Call "DeleteInvoices()" method which will Execute stored procedure and delete selected invoices
        var actionInvoiceList = processingDashboardRepository.DeleteInvoices(invoiceIdStringInOracleFormat, dummyMemberId, userId);

        foreach (var processingDashInvoiceDeleteActionStatuse in actionInvoiceList)
        {
            var invoiceAction = new ProcessingDashboardInvoiceActionStatus
            {
                ActionStatus = processingDashInvoiceDeleteActionStatuse.ActionStatus,
                BillingMemberId = processingDashInvoiceDeleteActionStatuse.BillingMemberId,
                BillingMemberCode = processingDashInvoiceDeleteActionStatuse.BillingMemberCode,
                BillingMemberName = processingDashInvoiceDeleteActionStatuse.BillingMemberName,
                InvoiceNo = processingDashInvoiceDeleteActionStatuse.InvoiceNo,
                InvoiceId = processingDashInvoiceDeleteActionStatuse.InvoiceId
            };


            invoiceList.Add(invoiceAction);
        }



        // Return Invoice list 
        return invoiceList.ToList();

    }// end DeleteSelectedInvoices()

    /// <summary>
    /// Following method is used to Resubmit Selected Invoices.
    /// </summary>
    /// <param name="invoiceIdArray">Array of InvoiceId's</param>
    /// <returns>List of Invoices with ActionStatus i.e. Success or Failure</returns>
    public List<ProcessingDashboardInvoiceActionStatus> ResubmitSelectedInvoices(string[] invoiceIdArray)
    {
        // Create List of type ProcessingDashboardInvoiceActionStatus
        List<ProcessingDashboardInvoiceActionStatus> invoiceList = new List<ProcessingDashboardInvoiceActionStatus>();

        // Iterate through InvoiceIdArray and retrieve Invoices
        foreach (string item in invoiceIdArray)
        {

            if (String.IsNullOrEmpty(item))
                continue;

            // Convert item to Guid
            Guid invoiceId = new Guid(item);

            // Get invoice details from Repository for specified InvoiceId
            InvoiceBase invoiceDetail = InvoiceReository.Single(invoice => invoice.Id == invoiceId);

            // Create instance of type ProcessingDashboardInvoiceActionStatus
            ProcessingDashboardInvoiceActionStatus invoiceAction = new ProcessingDashboardInvoiceActionStatus();

            // If Invoice exists perform operations to update ICH Settelment flag
            if (invoiceDetail != null)
            {
                invoiceAction.ActionStatus = "Only 'Claim Failed' ICH Settlement Status invoices can be Updated.";
                var memberDetails = MemberRepository.Single(m => m.Id == invoiceDetail.BillingMemberId);

                // check if invoice status id is 'Ready for billing' & IchSettlementStatus is 'Claim Failed' then update as null 
                //CMP626 : Invoices/Credit Notes where Invoice CreditNote status is Validated - Future Submission AND Invoice/CreditNote has been tagged as Claim Failed. 
                //Such Invoices/Credit Notes should be marked appropriately for another attempt of Provisional Settlement
                if (invoiceDetail.InvoiceStatusId == (int)InvoiceStatusType.ReadyForBilling || invoiceDetail.InvoiceStatusId == (int)InvoiceStatusType.FutureSubmitted)
                {
                    if (invoiceDetail.IchSettlementStatus ==(int)InvoiceSettlementStatusType.ClaimFailed)
                    {
                        invoiceDetail.IchSettlementStatus = null;
                       
                        InvoiceReository.Update(invoiceDetail);

                        invoiceAction.ActionStatus = "Success";
                        invoiceAction.InvoiceId = invoiceId;
                        invoiceAction.BillingMemberId = invoiceDetail.BilledMemberId;
                        invoiceAction.BillingMemberCode = memberDetails.MemberCodeAlpha;
                        invoiceAction.BillingMemberName = memberDetails.CommercialName;
                        invoiceAction.InvoiceNo = invoiceDetail.InvoiceNumber;
                        
                    }
                    else
                    {
                        invoiceAction.ActionStatus = "Ignored";
                        invoiceAction.InvoiceId = invoiceId;
                        invoiceAction.BillingMemberId = invoiceDetail.BilledMemberId;
                        invoiceAction.BillingMemberCode = memberDetails.MemberCodeAlpha;
                        invoiceAction.BillingMemberName = memberDetails.CommercialName;
                        invoiceAction.InvoiceNo = invoiceDetail.InvoiceNumber;
                    }
                }
                else
                {

                        invoiceAction.ActionStatus = "Ignored";
                        invoiceAction.InvoiceId = invoiceId;
                        invoiceAction.BillingMemberId = invoiceDetail.BilledMemberId;
                        invoiceAction.BillingMemberCode = memberDetails.MemberCodeAlpha;
                        invoiceAction.BillingMemberName = memberDetails.CommercialName;
                        invoiceAction.InvoiceNo = invoiceDetail.InvoiceNumber;
                   
                }

            } 
            // Add Invoice object to List
            invoiceList.Add(invoiceAction);

        }// end foreach()

        // Commit Invoice Repository changes
        UnitOfWork.CommitDefault();

        // Return Invoice list 
        return invoiceList.ToList();
    }// end ResubmitSelectedInvoices()

    /// <summary>
    /// Following method is used to increment BillingPeriod of Invoices within a File.
    /// </summary>
    /// <param name="fileIdsString">Comma separated list of File Id's</param>
    /// <param name="memberId">memberId of logged in user</param>
    /// <param name="userId">userId of logged in user</param>
    /// <returns>List of File details</returns>
    public List<ProcessingDashboardFileActionStatus> IncrementBillingPeriodForInvoicesWithinFile(string fileIdsString, int memberId, int userId)
    {
      // Declare List variable of type ProcessingDashboardFileActionStatus
      List<ProcessingDashboardFileActionStatus> fileList;

      // Get MemberDetails of LoggedIn user
      var memberDetails = _memberManager.GetMember(memberId);

      // Declare variable of type Billing period
      BillingPeriod currentBillingPeriodDetails;

      // Get Current billing period details depending on users Clearing house
      if (memberDetails.IchMemberStatus)
        // Call GetCurrentBillingPeriod() method which will return Current billing details
        currentBillingPeriodDetails = _calenderManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);
      else if (memberDetails.AchMemberStatus)
        // Call GetCurrentBillingPeriod() method which will return Current billing details
        currentBillingPeriodDetails = _calenderManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ach);
      else if (!memberDetails.IchMemberStatus || !memberDetails.AchMemberStatus)
        // Call GetCurrentBillingPeriod() method which will return Current billing details
        currentBillingPeriodDetails = _calenderManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);
      else
        // Call GetCurrentBillingPeriod() method which will return Current billing details
        currentBillingPeriodDetails = _calenderManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);

      // Declare variable of type StringBuilder
      var oracleGuidBuilder = new StringBuilder();

      // Iterate through file Id's and split on ','
      foreach (var netGuid in fileIdsString.Split(','))
      {
        if (String.IsNullOrEmpty(netGuid))
          continue;
        // Call "ConvertNetGuidToOracleGuid()" method which will return string 
        oracleGuidBuilder.Append(ConvertNetGuidToOracleGuid(netGuid));
        oracleGuidBuilder.Append(",");
      }// end foreach()

      // Declare string variable
      string oracleGuid;
      // Set string variable to Guid in Oracle format
      oracleGuid = oracleGuidBuilder.ToString();
      // Trim last ',' of comma separated GUID string
      oracleGuid = oracleGuid.TrimEnd(',');

      //get an instance of Processing Dashboard repository
      var processingDashboardRepository = Ioc.Resolve<IProcessingDashboardFileStatusRepository>(typeof(IProcessingDashboardFileStatusRepository));
      // Get file details by executing stored procedure
      fileList = processingDashboardRepository.IncrementBillingPeriodForInvoicesWithinFile(oracleGuid, currentBillingPeriodDetails.Year, currentBillingPeriodDetails.Month, currentBillingPeriodDetails.Period, userId);

      // Return File List
      return fileList.ToList();
    }// end IncrementBillingPeriodForInvoicesWithinFile()

    /// <summary>
    /// Following method is used to delete File
    /// </summary>
    /// <param name="selectedFileIds">Id's of file to be deleted</param>
    /// <returns>List of Non deleted files</returns>
    public List<ProcessingDashboardFileDeleteActionStatus> DeleteFiles(string selectedFileIds, int memberId, int userid)
    {
      // List to hold files which could not be deleted due to Error.
      List<ProcessingDashboardFileDeleteActionStatus> nonDeletedFilesList = new List<ProcessingDashboardFileDeleteActionStatus>();

      // Declare variable of type StringBuilder
      var guidBuilder = new StringBuilder();

      // Iterate through comma separated list of fileIds 
      foreach (string fileId in selectedFileIds.Split(','))
      {
        if (String.IsNullOrEmpty(fileId))
          continue;

        // Call ConvertNetGuidToOracleGuid() method which will convert .Net Guid to Oracle Guid
        guidBuilder.Append(ConvertNetGuidToOracleGuid(fileId));
        // Separate fileId using ","
        guidBuilder.Append(",");
      }// end foreach()

      // Convert guidBuilder to string and set it to string variable
      string fileIdStringInOracleFormat = guidBuilder.ToString();
      // Remove ',' at the end of fileIdString(i.e. We get fileId string from .net Guid format to Oracle Guid format)
      fileIdStringInOracleFormat = fileIdStringInOracleFormat.TrimEnd(',');

      //get an instance of Processing Dashboard repository
      var processingDashboardRepository = Ioc.Resolve<IProcessingDashboardFileStatusRepository>(typeof(IProcessingDashboardFileStatusRepository));
      // Call "DeleteFiles()" method which will Execute stored procedure and delete selected Files
      var fileList = processingDashboardRepository.DeleteFiles(fileIdStringInOracleFormat,memberId,userid);

      // Iterate through file list and get files which could not be deleted due to error, add such files to 
      // NonDeletedFiles list.
      string status;
      foreach (var file in fileList)
      {
        status = "File Not Deleted";
        // If file delete Status is 0, delete that file from Physical Location
        if (file.DeleteStatus == "0")
        {
          try
          {
            status = "File Deleted";
            // Get physical path of file
            string fileLocation = @file.FilePath;
            // If file exists, delete the file
            if (File.Exists(fileLocation))
            {
              // Delete file
              File.Delete(fileLocation);
              //status = "File Deleted";
            } // end if()
          } // end try
          catch (Exception)
          {
            Logger.Error("File: " + @file.FilePath + " could not be deleted.");
          } // end catch()
        } // end if()

        file.DeleteStatus = status;
        nonDeletedFilesList.Add(file);
      } // end foreach()

      // Return Non deleted file List
      return nonDeletedFilesList.ToList();
    }// end DeleteFiles()

    public List<ProcessingDashboardFileWarningDetail> GetFileInvoicesErrorWarning(Guid fileId)
    {
      //get an instance of file Processing Dashboard repository
      var processingDashboardRepository = Ioc.Resolve<IProcessingDashboardFileStatusRepository>(typeof(IProcessingDashboardFileStatusRepository));
      // Get file details by executing stored procedure
      var invoicesErrorWarning = processingDashboardRepository.GetFileInvoicesErrorWarning(fileId);

      // Return File List
      return invoicesErrorWarning.ToList();
    }

    /// <summary>
    /// Checks whether late submission is allowed or not
    /// </summary>
    /// <param name="categoryId">looged in users category Id </param>
    /// <param name="memberId">looged in users member Id </param>
    /// <returns>Message</returns>
    public bool IsLateSubmissionWindowOpen()
    {
      bool isIchLateAcceptanceAllowed = false;
      bool isAchLateAcceptanceAllowed = false;

      isIchLateAcceptanceAllowed = _calenderManager.IsLateSubmissionWindowOpen(ClearingHouse.Ich,
                                                                               _calenderManager.
                                                                                 GetLastClosedBillingPeriod(
                                                                                   ClearingHouse.Ich));

      isAchLateAcceptanceAllowed = _calenderManager.IsLateSubmissionWindowOpen(ClearingHouse.Ach,
                                                                               _calenderManager.
                                                                                 GetLastClosedBillingPeriod(
                                                                                   ClearingHouse.Ach));

      return (isIchLateAcceptanceAllowed || isAchLateAcceptanceAllowed);
    }

    // method used to get years(yyyy) from specified year in system parameter
    // TODO : Move to common class
    public List<int> GetBillingYears()
    {
      var startingYear = SystemParameters.Instance.General.StartingBillingYear;
      var currentYear = DateTime.UtcNow.Year;
      var yearDiff = currentYear - startingYear;
      var year = new List<int> { startingYear };

      for (int i = 1; i <= yearDiff; i++)
      {
        year.Add(startingYear + i);
      }

      return year.OrderByDescending(l => l).ToList();
    }

      public List<int> GetCalendarYear()
      {
          return  _calenderManager.GetCalendarYear().OrderByDescending(y=>y).ToList();
          
      }

    #region "Private Methods"

    /// <summary>
    /// Converts string(i.e. FileId) to GUID and converts it to ByteArray.
    /// Iterates through ByteArray, convert it to Hexadecimal equivalent and appends each hex values to 
    /// create a string(i.e. FileId in Oracle GUID format)
    /// </summary>
    /// <param name="oracleGuid">fileId i.e. .net GUID Format</param>
    /// <returns>fileId string i.e. Oracle GUID format</returns>
    private string ConvertNetGuidToOracleGuid(string oracleGuid)
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
    }// end ConvertNetGuidToOracleGuid()

    private byte[] GenerateInvoiceStatusCsvFile(ProcessingDashboardSearchEntity searchCriteria, char seperator)
    {
      List<ProcessingDashboardInvoiceStatusResultSet> invoiceList = GetInvoiceStatusResultForProcDashBrd(searchCriteria);

      // SCP245275: SRM: Dashboard without column "Daily Delivery Status
      var categoryId = _iUserManagement.GetUserByUserID(searchCriteria.IsUserId).CategoryID;

      // Write the headers.
      StringBuilder csvString = new StringBuilder();
      try
      {

        csvString.Append("Billing Period" + seperator + "Settlement Method Indicator" + seperator + "Billing Member");
        csvString.Append(seperator + "Billing Member Name" + seperator + "Billed Member" + seperator +
                         "Billed Member Name");
        csvString.Append(seperator + "Invoice Status");
        csvString.Append(seperator + "Invoice No.");
        csvString.Append(seperator + "Invoice Date");

        csvString.Append(seperator + "Billing Category");
        csvString.Append(seperator + "Invoice Currency");
        csvString.Append(seperator + "Invoice Amount");

        csvString.Append(seperator + "Clearance Currency");
        csvString.Append(seperator + "Clearance Amount");

        csvString.Append(seperator + "Suspended");
        csvString.Append(seperator + "Late Submitted");

        // if (searchCriteria.IncludeProcessingDatesTimestamp)
        csvString.Append(seperator + "Received in IS");

        csvString.Append(seperator + "Validation");
        // if (searchCriteria.IncludeProcessingDatesTimestamp)
        csvString.Append(seperator + "Validation Completed On");

        csvString.Append(seperator + "Value Confirmation");
        //  if (searchCriteria.IncludeProcessingDatesTimestamp)
        csvString.Append(seperator + "Value Confirmation Completed On");

        csvString.Append(seperator + "Digital Signature");
        // if (searchCriteria.IncludeProcessingDatesTimestamp)
        csvString.Append(seperator + "Digital Signature Completed On");


        csvString.Append(seperator + "Settlement File Sent");
        // if (searchCriteria.IncludeProcessingDatesTimestamp)
        csvString.Append(seperator + "Settlement File Sent On");
        //CMP529 : Daily Output Generation for MISC Bilateral Invoices
        // SCP245275: SRM: Dashboard without column "Daily Delivery Status
        if (categoryId == 1 || categoryId == 4)
        {
          //CMP529 : Daily Output Generation for MISC Bilateral Invoices
          csvString.Append(seperator + "Daily Delivery Status");
          //CMP529 : Daily Output deliverey status for MISC Bilateral Invoices
          csvString.Append(seperator + "Delivered On");
        }
        
        csvString.Append(seperator + "Presented");
        // if (searchCriteria.IncludeProcessingDatesTimestamp)
        csvString.Append(seperator + "Presented On");
        //CMP559 : Add Submission Method Column to Processing Dashboard
        csvString.Append(seperator + "Submission Method");
        
        if (categoryId == 1 || categoryId == 2)
        {
            csvString.Append(seperator + "Unique Invoice Id");
        }

        csvString.Append(Environment.NewLine);

        // Now write all the rows.
        foreach (var item in invoiceList)
        {
            string isSuspended = string.Empty;
            string isLateSubmitted = string.Empty;
            // var validationStatus;
            //string valueConfirmationStatus;
            // string digitalSignatureStatus;
            string settlementFileStatus;
            string presentedStatus;

            if (item.IsSuspendedLateSubmitted != null)
            {
                isSuspended = (item.IsSuspendedLateSubmitted == "10" || item.IsSuspendedLateSubmitted == "11")
                                  ? "Y"
                                  : string.Empty;
                isLateSubmitted = (item.IsSuspendedLateSubmitted == "01" || item.IsSuspendedLateSubmitted == "11")
                                      ? "Y"
                                      : string.Empty;
            }

            var validationId = Convert.ToInt32(item.ValidationStatusId);
            var validationStatus = (InvoiceValidationStatus) validationId;

            var valueConfirmationStatusId = Convert.ToInt32(item.ValueConfirmationStatusId);

            // Change Pending Status for intermediate status ‘Required But Not Requested’ and ‘Requested’
            var bvcStatusId = 0;
            if (valueConfirmationStatusId == (int) ValueConfirmationStatus.Requested ||
                valueConfirmationStatusId == (int) ValueConfirmationStatus.RequiredButNotRequested)
            {
                bvcStatusId = (int) ValueConfirmationStatus.Pending;
            }
            else
            {
                bvcStatusId = valueConfirmationStatusId;
            }

            var valueConfirmationStatus = ((ValueConfirmationStatus) bvcStatusId).ToString();
            if (valueConfirmationStatus == "None")
                valueConfirmationStatus = string.Empty;

            var digitalSignatureStatussId = Convert.ToInt32(item.DigitalSignatureStatusId);
            var digitalSignatureStatus = (DigitalSignatureStatus) (digitalSignatureStatussId);

            var settlementFileStatusId = Convert.ToInt32(item.SettlementFileStatusId);
            settlementFileStatus = GetInvoiceStatus(settlementFileStatusId);

            var presentedStatusId = Convert.ToInt32(item.PresentedStatusId);
            presentedStatus = GetInvoiceStatus(presentedStatusId);

            var presentedStatusDate = (item.PresentedStatusDate == DateTime.MinValue || presentedStatusId == 1)
                                          ? string.Empty
                                          : item.FormatedPresentedStatusDate;

            var validationStatusDate = (item.ValidationStatusDate == DateTime.MinValue || validationId == 1)
                                           ? string.Empty
                                           : item.FormatedValidationStatusDate;

            var digitalSignatureStatusDate = (item.DigitalSignatureStatusDate == DateTime.MinValue ||
                                              digitalSignatureStatussId == 1 || digitalSignatureStatussId == 4
                                              || digitalSignatureStatussId == 0)
                                                 ? string.Empty
                                                 : item.FormatedDigitalSignatureStatusDate;

            var settlementFileStatusDate = (item.SettlementFileStatusDate == DateTime.MinValue ||
                                            settlementFileStatusId == 1)
                                               ? string.Empty
                                               : item.FormatedSettlementFileStatusDate;

            var valueConfirmationStatusDate = (valueConfirmationStatusId == 3)
                                                  ? item.FormatedValueConfirmationStatusDate
                                                  : string.Empty;

            var recievedInIs = (item.ReceivedInIS == DateTime.MinValue || !item.ReceivedInIS.HasValue)
                                   ? string.Empty
                                   : item.ReceivedInIS.Value.ToString("dd MMM yyyy HH:mm");

            var invoiceDate = (item.InvoiceDate == DateTime.MinValue || !item.InvoiceDate.HasValue)
                                  ? string.Empty
                                  : item.InvoiceDate.Value.ToString("dd MMM yyyy HH:mm");

            //CMP559 : Add Submission Method Column to Processing Dashboard
            string submissionMethod = string.Empty;
            switch(item.SubmissionMethodId)
            {
                case 1:
                    submissionMethod = string.Format("IS-IDEC {0}", item.FileName);
                    break;
                case 2:
                    submissionMethod = string.Format("IS-XML {0}", item.FileName);
                    break;
                case 3:
                    submissionMethod = "IS-WEB";
                    break;
                case 4:
                    submissionMethod = "Auto-Billing";
                    break;
            }

            //CMP529 : Daily Output Generation for MISC Bilateral Invoices
            string DailydeliveryStatus = "NotRequired";
            switch(item.DailyDeliveryStatusId)
            {
                case 1:
                    DailydeliveryStatus = "Pending";
                    break;
                case 2:
                    DailydeliveryStatus = "Completed";
                    break;
            }
            csvString.Append(("\"" + item.BillingPeriod + "\"" ?? String.Empty) + seperator +
                             ("\"" + item.SettleMethodIndicator + "\"" ?? String.Empty) + seperator +
                             ("\"" + item.BillingMemberCode + "\"" ?? String.Empty));
            csvString.Append(seperator + "\"" + item.BillingMemberName + "\"" + seperator +
                             ("\"" + item.BilledMemberCode + "\"" ?? String.Empty));
            csvString.Append(seperator + "\"" + item.BilledMemberName + "\"" + seperator +
                             ("\"" + item.InvoiceStatusDescription + "\"" ?? String.Empty) + seperator +
                             ("\"" + item.InvoiceNo + "\"" ?? String.Empty));
            csvString.Append(seperator + invoiceDate);

            csvString.Append(seperator + ("\"" + item.BillingCategory + "\"" ?? String.Empty));
            csvString.Append(seperator + ("\"" + item.InvoiceCurrency + "\"" ?? String.Empty));
            csvString.Append(seperator + ("\"" + item.InvoiceAmount + "\"" ?? String.Empty));

            //CMP#415- Clearance Currency and Amount field in Dashboard
            csvString.Append(seperator + ("\"" + item.CurrancyOfBilling + "\"" ?? String.Empty));
            csvString.Append(seperator + ("\"" + item.CurrencyAmount + "\"" ?? String.Empty));

            csvString.Append(seperator + ("\"" + isSuspended + "\"" ?? String.Empty));
            csvString.Append(seperator + ("\"" + isLateSubmitted + "\"" ?? String.Empty));

            // if (searchCriteria.IncludeProcessingDatesTimestamp)
            csvString.Append(seperator + ("\"" + recievedInIs + "\"" ?? String.Empty));

            csvString.Append(seperator + ("\"" + validationStatus + "\"" ?? String.Empty));
            //if (searchCriteria.IncludeProcessingDatesTimestamp)
            csvString.Append(seperator + validationStatusDate);


            csvString.Append(seperator + valueConfirmationStatus);
            // if (searchCriteria.IncludeProcessingDatesTimestamp)
            csvString.Append(seperator + valueConfirmationStatusDate);

            csvString.Append(seperator + ("\"" + digitalSignatureStatus + "\"" ?? String.Empty));
            // if (searchCriteria.IncludeProcessingDatesTimestamp)
            csvString.Append(seperator + digitalSignatureStatusDate);


            csvString.Append(seperator + ("\"" + settlementFileStatus + "\"" ?? String.Empty));
            // if (searchCriteria.IncludeProcessingDatesTimestamp)
            csvString.Append(seperator + settlementFileStatusDate);

            //CMP529 : Daily Output Generation for MISC Bilateral Invoices
            // SCP245275: SRM: Dashboard without column "Daily Delivery Status 
            if (categoryId == 1 || categoryId == 4)
            {
              //CMP529 : Daily Output Generation for MISC Bilateral Invoices
              csvString.Append(seperator + DailydeliveryStatus);

              var deliveredOn = item.DeliveredOn == DateTime.MinValue
                                  ? string.Empty
                                  : item.DeliveredOn != null
                                      ? item.DeliveredOn.Value.ToString("dd MMM yyyy HH:mm")
                                      : string.Empty;

              csvString.Append(seperator + deliveredOn);
            }


            csvString.Append(seperator + ("\"" + presentedStatus + "\"" ?? String.Empty));
            // if (searchCriteria.IncludeProcessingDatesTimestamp)
            csvString.Append(seperator + presentedStatusDate);

            //CMP559 : Add Submission Method Column to Processing Dashboard
            csvString.Append(seperator + submissionMethod);

            if (categoryId == 1 || categoryId == 2)
            {
                csvString.Append(seperator + ("\"" + ConvertUtil.ConvertGuidToString(item.InvoiceId) + "\"" ?? String.Empty));
            }

            csvString.Append(Environment.NewLine);
        }
          //flag = true;
      }
      catch (Exception exception)
      {
        Logger.Error("Error occurred while generating csv file for Processing dashboard invoice download.", exception);
        //flag = false;
      }

      return ConvertStringToBytes(csvString.ToString());
    }

    private byte[] GenerateFileStatusCsvFile(ProcessingDashboardSearchEntity searchCriteria, char seperator)
    {
      // call method to get file status result
      List<ProcessingDashboardFileStatusResultSet> fileList = GetFileStatusResultForProcDashBrd(searchCriteria);

      // Write the headers.
      StringBuilder csvString = new StringBuilder();
      try
      {
        csvString.Append("File Name" + seperator + "Billing Category" + seperator + "Billing Member");
        csvString.Append(seperator + "File Format" + seperator + "Number Of Invoices In File");
        csvString.Append(seperator + "Number Of Invoices Passed Validation" + seperator + "Number Of Invoices Failed Validation");
        csvString.Append(seperator + "Received by IS" + seperator + "File Status");

        csvString.Append(Environment.NewLine);

        // Now write all the rows.
        foreach (var item in fileList)
        {

          csvString.Append(("\"" + item.FileName + "\"" ?? String.Empty) + seperator +
                           ("\"" + item.BillingCategory + "\"" ?? String.Empty) + seperator + ("\"" + item.BillingMemberName + "\"" ?? String.Empty) + seperator +
                           ("\"" + item.FileFormat + "\"" ?? String.Empty));

          csvString.Append(seperator + ("\"" + item.NumberOfInvoicesInFile + "\"" ?? String.Empty));
          csvString.Append(seperator + ("\"" + item.NumberOfValidInvoicesInFile + "\"" ?? String.Empty));
          csvString.Append(seperator + ("\"" + item.NumberOfInvalidInvoicesInFile + "\"" ?? String.Empty));

          csvString.Append(seperator + ("\"" + item.ReceivedByIS + "\"" ?? String.Empty));
          csvString.Append(seperator + ("\"" + item.FileStatus + "\"" ?? String.Empty));

          csvString.Append(Environment.NewLine);
        }
        //flag = true;
      }
      catch (Exception exception)
      {
        // Logger.Error("Error occurred while generating csv file using MemberLocationCsvGenerator service.", exception);
        //flag = false;
      }

      return ConvertStringToBytes(csvString.ToString());
    }

    private byte[] ConvertStringToBytes(string input)
    {
      MemoryStream stream = new MemoryStream();
      using (StreamWriter writer = new StreamWriter(stream))
      {
        writer.Write(input);
        writer.Flush();
      }
      return stream.ToArray();
    }

    private static string GetInvoiceStatus(int id)
    {
      string status = null;
      switch (id)
      {
        case 1:
          status = "Pending";
          break;
        case 2:
          status = "Failed";
          break;
        case 3:
          status = "Completed";
          break;
      }

      return status;
    }

    private static string GetSuspendedLateSubmittedStatus(string id)
    {
      string status = null;
      switch (id)
      {
        case "00":
          status = string.Empty;
          break;
        case "01":
          status = "L";
          break;
        case "10":
          status = "S";
          break;
        case "11":
          status = "S/L";
          break;
      }

      return status;
    }
/// <summary>
/// This function is used to send mail to ACH/ICH ops for pending invoices for late submission.
/// </summary>
/// <param name="lateSubmittedInvoiceCount"></param>
/// <param name="memberName"></param>
/// <param name="clearingHouse"></param>
/// <param name="emailTemplateId"></param>
/// <returns></returns>
    private static bool SendMailForLateSubmissionOnProcessingDashboard(int lateSubmittedInvoiceCount, string memberName, ClearingHouse clearingHouse, EmailTemplateId emailTemplateId)
    {
      bool flag;
      //SCP51007 : SRM 4.5 - Pending Late Submissions alert 
      //Get the system parameter values of ICH & ACH Manual Control
      bool isAchManulaControl = Convert.ToBoolean(SystemParameters.Instance.ACHDetails.ManualControlOnACHLateSubmission);
      Logger.Info("AchManulaControl :" + isAchManulaControl);
      bool isIchManulaControl = Convert.ToBoolean(SystemParameters.Instance.ICHDetails.ManualControlOnIchLateSubmission);
      Logger.Info("IchManulaControl :" + isIchManulaControl);
      //Send mail to ICH or ACH Ops only when ICH & ACH is Manual Control. Ref UC G-280.
      if (clearingHouse == ClearingHouse.Ich && isIchManulaControl == false) return true;
      if (clearingHouse == ClearingHouse.Ach && isAchManulaControl == false) return true;

      //create nvelocity data dictionary
      var context = new VelocityContext();
      //get an instance of email settings  repository
      var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));
      //get an object of the EmailSender component
      var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));
      //get an object of the TemplatedTextGenerator that is used to generate body text of email from a nvelocity template
      var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));

      try
      {

        context.Put("InvoiceCount", lateSubmittedInvoiceCount);
        context.Put("MemberName", memberName);
        context.Put("ClearingHouse", Convert.ToString(clearingHouse).ToUpper());
        string sisOpsEmail = SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail;
        context.Put("SISOpsEmailid", sisOpsEmail);
        var emailToInvoice = templatedTextGenerator.GenerateTemplatedText(emailTemplateId, context);

        Logger.Error("Template Created for processing dashboard late submission");

        //Get the eMail settings for member profile future update mails for Invoice Reference Data Updates contact type for email with csv
        var emailSettingForLateInvoiceSubmission =
          emailSettingsRepository.Get(esfirdu => esfirdu.Id == (int)EmailTemplateId.InvoiceLateSubmission);

        //create a mail object to send mail 
        var msgInvoice = new MailMessage
                           {
                             From =
                               new MailAddress(
                               emailSettingForLateInvoiceSubmission.SingleOrDefault().FromEmailAddress)
                           };
        msgInvoice.IsBodyHtml = true;

        //loop through the contacts list and add them to To list of mail to be sent
        string emailTo;

        emailTo = clearingHouse == ClearingHouse.Ich
                    ? SystemParameters.Instance.ICHDetails.IchOpsEmail.Replace(';', ',')
                    : SystemParameters.Instance.ACHDetails.AchOpsEmail.Replace(';', ',');

        foreach (var contact in emailTo.Split(','))
        {
          msgInvoice.To.Add(new MailAddress(contact));
        }

        //set subject of mail (replace special field placeholders with values)
        msgInvoice.Subject = emailSettingForLateInvoiceSubmission.SingleOrDefault().Subject;

        //set body text of mail
        msgInvoice.Body = emailToInvoice;

        //send the mail
        emailSender.Send(msgInvoice);

        //clear nvelocity context data
        context = null;
        flag = true;
      }

      catch (Exception exception)
      {
        Logger.Error("Error occurred occured in Sending mail for late submission.", exception);
        flag = false;
      }

      return flag;
    }

    #endregion

    /* CMP #675: Progress Status Bar for Processing of Billing Data Files.
    * Desc: SP called to get file procssing progress detail. */
    public bool GetFileProgressDetails(Guid fileLogId, ref string processName, ref string processState, ref int queuePosition)
    {
        try
        {
            /* Get an instance of Processing Dashboard repository */
            var processingDashboardRepository =
                Ioc.Resolve<IProcessingDashboardFileStatusRepository>(typeof(IProcessingDashboardFileStatusRepository));

            /* Call "GetFileProgressDetails()" method which will Execute stored procedure PROC_GET_FILE_PROGRESS_DETAILS */
            processingDashboardRepository.GetFileProgressDetails(fileLogId, ref processName, ref processState,
                                                                 ref queuePosition);

            return true;
        }
        catch (Exception exception)
        {
            Logger.Error(exception);
            return false;
        }
    }

  }
}
