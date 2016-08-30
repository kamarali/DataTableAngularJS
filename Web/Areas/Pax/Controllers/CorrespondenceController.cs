using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Business.Common.Impl;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.Pax;
using Iata.IS.Business.Security;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.Pax;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.UIModel.Grid.Pax;
using Iata.IS.Web.Util;
using System.Web;
using Iata.IS.Web.Util.Filters;
using log4net;
using System.Reflection;
using Iata.IS.Business.Web;

namespace Iata.IS.Web.Areas.Pax.Controllers
{
    public class CorrespondenceController : ISController
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IPaxCorrespondenceManager _paxCorrespondenceManager;
        private readonly ICalendarManager _calendarManager;
        private readonly IAuthorizationManager _authorizationManager;

        private readonly ICorrespondenceManager _correspondenceManager;
        
        private const string CorrespondenceRejectionGridAction = "CorrespondenceRejectionGridData";
        private const string CorrespondenceHistoryGridAction = "CorrespondenceHistoryGridData";
        public INonSamplingInvoiceManager NonSamplingInvoiceManager { get; set; }
        private const int paxBillingCategory = 1;

        // SCP#481926 :PAXCREATECORRESPONDENCEFOR is slow (April 2016)
        public IReferenceManager ReferenceManager { get; set; }
        public Guid logRefId = Guid.NewGuid();

        public CorrespondenceController(IPaxCorrespondenceManager paxCorresondenceManager, ICalendarManager calendarManager, IAuthorizationManager authorizationManager, ICorrespondenceManager correspondenceManager)
        {
            _paxCorrespondenceManager = paxCorresondenceManager;
            _calendarManager = calendarManager;
            _authorizationManager = authorizationManager;
            _correspondenceManager = correspondenceManager;
        }

        private void CheckCorrespondenceStatus(Correspondence correspondenceInDB)
        {
            //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
            //Commented below code.used correspondence parameter passed
            //var correspondenceInDB = _paxCorrespondenceManager.GetCorrespondenceDetails(transactionId);

            if (correspondenceInDB != null && correspondenceInDB.CorrespondenceSubStatus == CorrespondenceSubStatus.Responded)
            {
                throw new ISBusinessException(ErrorCodes.ErrorCorrespondenceAlreadySent);
            }
        }

        /// <summary>
        /// Retrieves Correspondence details for given invoice id.
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        [OutputCache(CacheProfile = "donotCache")]
        public ActionResult PaxCreateCorrespondenceFor(string invoiceId, string rejectionMemoIds)
        {
            //SCP210204: IS-WEB Outage (done Changes to improve performance)
            
            SetViewDataPageMode(PageMode.Create);

            TempData[TempDataConstants.RejectedRecordIds] = rejectionMemoIds;
            var correspondence = new Correspondence
            {
              CorrespondenceDate = DateTime.UtcNow,
              CorrespondenceStage = 1,
              CorrespondenceStatus = CorrespondenceStatus.Open,
              CorrespondenceSubStatus = CorrespondenceSubStatus.ReadyForSubmit,
              CorrespondenceOwnerId = SessionUtil.UserId,
              CorrespondenceOwnerName = SessionUtil.Username,
              RejectionMemoIds = rejectionMemoIds,
            };

            Guid inv_Id;
            Guid.TryParse(invoiceId, out inv_Id);
            // SCP#481926 :PAXCREATECORRESPONDENCEFOR is slow (April 2016)
            

            var log = ReferenceManager.GetDebugLog(DateTime.Now, "PaxCreateCorrespondenceFor", this.ToString(), "Passenger",
                                         "Step 1:GetInvAndMemberDetails Start", SessionUtil.UserId, logRefId.ToString());

            ReferenceManager.LogDebugData(log);
            correspondence = new PaxCorrespondenceManager().GetInvAndMemberDetails(ConvertUtil.ConvertGuidToString(inv_Id), rejectionMemoIds, correspondence);

            // SCP#481926 :PAXCREATECORRESPONDENCEFOR is slow (April 2016)
            log = ReferenceManager.GetDebugLog(DateTime.Now, "PaxCreateCorrespondenceFor", this.ToString(), "Passenger",
                                         "Step 2:GetInvAndMemberDetails End", SessionUtil.UserId, logRefId.ToString());
            ReferenceManager.LogDebugData(log);
            ViewData[ViewDataConstants.InvoiceId] = invoiceId;


            // SCP#481926 :PAXCREATECORRESPONDENCEFOR is slow (April 2016)
            log = ReferenceManager.GetDebugLog(DateTime.Now, "CheckCorrespondenceInitiator", this.ToString(), "Passenger",
                                         "Step 3:CheckCorrespondenceInitiator Start", SessionUtil.UserId, logRefId.ToString());
            ReferenceManager.LogDebugData(log);
            CheckCorrespondenceInitiator(null);

            // SCP#481926 :PAXCREATECORRESPONDENCEFOR is slow (April 2016)
            log = ReferenceManager.GetDebugLog(DateTime.Now, "CheckCorrespondenceInitiator", this.ToString(), "Passenger",
                                         "Step 4:CheckCorrespondenceInitiator End", SessionUtil.UserId, logRefId.ToString());
            ReferenceManager.LogDebugData(log);

            GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId,
                                         Url.Action("InitiateCorrespondenceRejectionGridData",
                                                    new
                                                    {
                                                      invoiceId = invoiceId
                                                    }));

            GetCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId,
                                         Url.Action(CorrespondenceHistoryGridAction,
                                                    new
                                                    {
                                                        invoiceId = correspondence.Id
                                                    }));
            return View("PaxCreateCorrespondence", correspondence);
        }

        [HttpGet]
        [OutputCache(CacheProfile = "donotCache")]
        public ActionResult PaxCreateCorrespondence(string invoiceId)
        {
            SetViewDataPageMode(PageMode.Create);

            var rejectedMemos = SessionUtil.RejectionMemoRecordIds;
            var sRejectedMemos = SessionUtil.RejectionMemoRecordIds.Split(',');

            TempData[TempDataConstants.RejectedRecordIds] = sRejectedMemos;

            var correspondence = new Correspondence
            {
              CorrespondenceDate = DateTime.UtcNow,
              CorrespondenceStage = 1,
              CorrespondenceStatus = CorrespondenceStatus.Open,
              CorrespondenceSubStatus = CorrespondenceSubStatus.ReadyForSubmit,
              CorrespondenceOwnerId = SessionUtil.UserId,
              CorrespondenceOwnerName = SessionUtil.Username
            };

            Guid inv_Id;
            Guid.TryParse(invoiceId, out inv_Id);

            correspondence = new PaxCorrespondenceManager().GetInvAndMemberDetails(ConvertUtil.ConvertGuidToString(inv_Id), rejectedMemos, correspondence);

            ViewData[ViewDataConstants.InvoiceId] = invoiceId;

            CheckCorrespondenceInitiator(correspondence.CorrespondenceNumber);

            
            GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId,
                                         Url.Action("InitiateCorrespondenceRejectionGridData",
                                                    new
                                                    {
                                                      invoiceId = invoiceId
                                                    }));
            GetCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId,
                                         Url.Action(CorrespondenceHistoryGridAction,
                                                    new
                                                    {
                                                        invoiceId = correspondence.Id
                                                    }));
            return View(correspondence);
        }

        /// <summary>
        /// Called on Save.
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="correspondence"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        //Check for Correspondence Updates.
        [RestrictInvoiceUpdate(InvParamName = "invoiceId",
                               CorrespondenceParamName = "correspondence",
                               InvList = false, TableName = TransactionTypeTable.PAX_CORRESPONDENCE,
                               ActionParamName = "ViewCorrespondence")]
        public ActionResult PaxCreateCorrespondence(string invoiceId, Correspondence correspondence)
        {
            //CMP526 - Passenger Correspondence Identifiable by Source Code
            int? sourceCode = null;
            correspondence.CorrespondenceStatus = CorrespondenceStatus.Open;
            correspondence.CorrespondenceSubStatus = CorrespondenceSubStatus.Saved;
            correspondence.CorrespondenceOwnerId = SessionUtil.UserId;
            var correspondenceAttachmentIds = correspondence.Attachments.Select(attachment => attachment.Id).ToList();
            /* SCP106534: ISWEB No-02350000768 
                      Desc: Added support for operation status parameter
                      Date: 20/06/2013*/
            int operationStatusIndicator = -1;

            try
            {
                if (invoiceId == null)
                {
                    //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
                    //var originalCorr = _paxCorrespondenceManager.GetOriginalCorrespondenceDetails(correspondence.CorrespondenceNumber);
                    var originalCorr = new PaxCorrespondenceManager().GetPaxCorrespondenceFieldDetails(Convert.ToInt64(correspondence.CorrespondenceNumber),1);
                    correspondence.Attachments.Clear();
                    //CMP526 - Passenger Correspondence Identifiable by Source Code
                    if (originalCorr != null)
                        sourceCode = originalCorr.SourceCode;
                    correspondence.InvoiceId = originalCorr.InvoiceId;
                    //CMP526 - Passenger Correspondence Identifiable by Source Code
                    if (!sourceCode.HasValue)
                        throw new ISBusinessException("The correspondence trail could not be found.");
                    correspondence.SourceCode = sourceCode;

                    //correspondence = _paxCorrespondenceManager.AddCorrespondence(correspondence);
                    ////Update parent Id for attachment
                    //_paxCorrespondenceManager.UpdatePaxCorrespondenceAttachment(correspondenceAttachmentIds, correspondence.Id);
                    correspondence.Attachments.Clear();
                    correspondence = _paxCorrespondenceManager.AddCorrespondenceAndUpdateRejection(correspondence,
                                                                                                     correspondenceAttachmentIds,
                                                                                                                        SessionUtil.RejectionMemoRecordIds, ref operationStatusIndicator);

                }
                else
                {
                    correspondence.Attachments.Clear();

                    correspondence.InvoiceId = invoiceId.ToGuid();
                    
                    correspondence = _paxCorrespondenceManager.AddCorrespondenceAndUpdateRejection(correspondence,
                                                                                                   correspondenceAttachmentIds,
                                                                                                                      SessionUtil.RejectionMemoRecordIds, ref operationStatusIndicator);

                }

                ShowSuccessMessage(Messages.PaxCorrespondenceCreateSuccessful);

                return RedirectToAction("EditPaxCorrespondence", new
                {
                    transactionId = correspondence.Id.Value()
                });
            }
            catch (ISBusinessException exception)
            {

                /* SCP106534: ISWEB No-02350000768 
                 Desc: Added support for operation status parameter.
                  * it is 1 or 2 or 3 then this is business error while adding corr in DB because of paralel operation so it is success
                 Date: 20/06/2013*/
                //SCP210204: IS-WEB Outage (QA Issue Fix)
                if (operationStatusIndicator != -1 && operationStatusIndicator != 4 && correspondence.Id != Guid.Empty)
                {
                    ShowErrorMessage(exception.ErrorCode, true);
                    return RedirectToAction("EditPaxCorrespondence", new
                    {
                        transactionId = correspondence.Id.Value()
                    });
                }

                ShowErrorMessage(exception.ErrorCode);
                correspondence.Attachments = _paxCorrespondenceManager.GetCorrespondenceAttachments(correspondenceAttachmentIds);
                var billingMember = _paxCorrespondenceManager.GetMember(correspondence.FromMemberId);
                var billedMember = _paxCorrespondenceManager.GetMember(correspondence.ToMemberId);
                correspondence.FromMember = billingMember;
                correspondence.ToMember = billedMember;

                ViewData[ViewDataConstants.InvoiceId] = correspondence.InvoiceId;


                //GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action(CorrespondenceRejectionGridAction, new { invoiceId = correspondence.Id }));
                //GetCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new { invoiceId = correspondence.Id }));

            }

            if (correspondence.Id != Guid.Empty)
            {
              GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action(CorrespondenceRejectionGridAction, new { invoiceId = correspondence.Id }));
              GetCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new { invoiceId = correspondence.Id }));
              CheckCorrespondenceInitiator(correspondence.CorrespondenceNumber);
            }
            else
            {
              var previousCorrespondenceId = Guid.Empty;
              if (TempData != null )
              {
                if (!string.IsNullOrWhiteSpace(Convert.ToString(TempData["PreviousCorrespondenceId"])))
                {
                  var previousCorrId = Convert.ToString(TempData["PreviousCorrespondenceId"]);
                  previousCorrespondenceId = previousCorrId.ToGuid();
                }
              }
              if (correspondence.CorrespondenceStage > 1 && previousCorrespondenceId != Guid.Empty)
              {
                GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action(CorrespondenceRejectionGridAction, new { invoiceId = previousCorrespondenceId }));
                GetCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new { invoiceId = previousCorrespondenceId }));

                CheckCorrespondenceInitiator(correspondence.CorrespondenceNumber);
                TempData["PreviousCorrespondenceId"] = previousCorrespondenceId;
              }
              else
              {
                char[] seperator = { ',' };
                var rejectedMemos = SessionUtil.RejectionMemoRecordIds.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
                var rejectedMemoIds = rejectedMemos;
                if (!string.IsNullOrEmpty(SessionUtil.RejectionMemoRecordIds))
                {
                  if (rejectedMemoIds != null && rejectedMemoIds.Count() > 0)
                  {
                    string sRejectedMemoIds = string.Join(",", rejectedMemoIds);

                    GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action("InitiateCorrespondenceRejectionGridData",
                                                                                                              new
                                                                                                              {
                                                                                                                invoiceId = correspondence.InvoiceId,
                                                                                                                rejectedMemoIds = sRejectedMemoIds
                                                                                                              }));
                    TempData[TempDataConstants.RejectedRecordIds] = sRejectedMemoIds;

                  }
                  GetCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new { invoiceId = correspondence.InvoiceId }));

                  CheckCorrespondenceInitiator(correspondence.CorrespondenceNumber);
                }
              }

            }
            
            return View(correspondence);
        }

        private void CheckCorrespondenceInitiator(long? correspondenceNumber)
        {
            //SCP85039: IS Web Performance Feedback / Billing History & Correspondence / Other issues
            //Get original corr by correspondence number
            if (correspondenceNumber == null)
            {
                ViewData[ViewDataConstants.CorrespondenceInitiator] = true;
                return;
            }

            ViewData[ViewDataConstants.CorrespondenceInitiator] = new PaxCorrespondenceManager().CheckCorrespondenceInitiator(correspondenceNumber,SessionUtil.MemberId); ;
        }

        /// <summary>
        /// Update Pax Correspondence
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        [HttpGet]
        [OutputCache(CacheProfile = "donotCache")]
        public ActionResult EditPaxCorrespondence(string transactionId)
        {
            SetViewDataPageMode(PageMode.Edit);

            Logger.InfoFormat("Transaction ID {0}", transactionId);
            var corrId = ConvertUtil.ConvertGuidToString(transactionId.ToGuid());
            var correspondence = new PaxCorrespondenceManager().GetPaxCorrespondenceDetails(corrId);

            // SCP251331: FW: Rare Occuring Issue found for Closed Pax Correspondence Functionalit
            if (correspondence == null)
            {
              return RedirectToAction("Index", "BillingHistory");
            }

            correspondence.Attachments = _paxCorrespondenceManager.GetPaxCorrespondenceAttachments(transactionId.ToGuid());

            //SCP210204: IS-WEB Outage (Retrive invoice Id of 1st Corr. only if current corr. dose not have invoice Id)
            // Retrieve Coreespondence related Invoice
            ViewData[ViewDataConstants.InvoiceId] = (correspondence != null && correspondence.InvoiceId == new Guid())
                                                      ? _paxCorrespondenceManager.GetCorrespondenceRelatedInvoice(transactionId, correspondence).Id
                                                      : correspondence.InvoiceId;

            CheckCorrespondenceInitiator(correspondence.CorrespondenceNumber);
            
            GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action(CorrespondenceRejectionGridAction, new { invoiceId = transactionId }));
            GetCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new { invoiceId = transactionId }));

            //CMP527: Start
            if (_authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.Pax.BillingHistoryAndCorrespondence.AcceptAndCloseCorrespondence))
            {
                var corrCanClose = _paxCorrespondenceManager.CanCorrespondenceClose(paxBillingCategory, transactionId);
                var closeCorrespondence =
                  Convert.ToBoolean(corrCanClose[(int)CorrespondenceCloseStatus.CorrespondenceCanClose]);

                if (closeCorrespondence)
                {
                    //var correspondenceInDb = _paxCorrespondenceManager.GetOriginalCorrespondenceDetails(correspondence.CorrespondenceNumber);
                    ViewData[ViewDataConstants.CorrespondenceCanClose] = ViewData[ViewDataConstants.CorrespondenceInitiator];
                    ViewData[ViewDataConstants.CorrespondeneClosedScenario] = corrCanClose[(int)CorrespondenceCloseStatus.CorrespondenceCloseScenario];
                }
            }
            //CMP527: End
            // SCP109163
            // Check if correspondence expiry date is crossed.
            if (correspondence.ExpiryDate < DateTime.UtcNow.Date)
            {
                ShowErrorMessage(ErrorCodes.ExpiredCorrespondence);
            }
            
            return View(correspondence);
        }

        /// <summary>
        /// Update Pax Correspondence
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="correspondence"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        //Check for Correspondence Updates.
        [RestrictInvoiceUpdate(TransactionParamName = "transactionId",
                               CorrespondenceParamName = "correspondence",
                               InvList = false, TableName = TransactionTypeTable.PAX_CORRESPONDENCE,
                               ActionParamName = "ViewCorrespondence")]
        public ActionResult EditPaxCorrespondence(string transactionId, Correspondence correspondence)
        {
            var correspondenceAttachmentIds = correspondence.Attachments.Select(attachment => attachment.Id).ToList();
            correspondence.CorrespondenceStatus = CorrespondenceStatus.Open;
            correspondence.CorrespondenceSubStatus = CorrespondenceSubStatus.Saved;
            try
            {
                //CMP526 - Passenger Correspondence Identifiable by Source Code
                int? sourceCode = null;
                correspondence.Id = transactionId.ToGuid();
                var corr = new PaxCorrespondenceManager().GetPaxCorrespondenceFieldDetails(correspondence.Id);
                //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
                CheckCorrespondenceStatus(corr);
                correspondence.InvoiceId = corr.InvoiceId;
                //CMP526 - Passenger Correspondence Identifiable by Source Code
                sourceCode = corr.SourceCode;
                if (!sourceCode.HasValue)
                    throw new ISBusinessException("The correspondence trail could not be found.");
                correspondence.SourceCode = sourceCode;

                // SCP109163
                // Check if correspondence expiry date is crossed.
                if (corr.ExpiryDate < DateTime.UtcNow.Date)
                {
                    throw new ISBusinessException(ErrorCodes.ExpiredCorrespondence);
                }
                //SCP210204: IS-WEB Outage (QA Issue Fix)
                correspondence.ExpiryDate = corr.ExpiryDate;
                correspondence.CorrespondenceOwnerId = corr.CorrespondenceOwnerId;

                if (_paxCorrespondenceManager.ValidateCorrespondence(correspondence))
                {
                  //validate attachment for duplicate file name
                  var dbAttachments = _paxCorrespondenceManager.GetPaxCorrespondenceAttachments(transactionId.ToGuid());
                  var corrAttachments = _paxCorrespondenceManager.GetCorrespondenceAttachments(correspondenceAttachmentIds);
                  var isvalidAttachments = new PaxCorrespondenceManager().ValidateCorrespondenceAttachmentFileName(dbAttachments, corrAttachments);
                  if (!isvalidAttachments)
                  {
                    throw new ISBusinessException(ErrorCodes.DuplicateFileName);
                  }
                  
                  var result = new PaxCorrespondenceManager().UpdatePaxCorrespondence(correspondence);
                  if(!result)
                  {
                    throw new ISBusinessException(ErrorCodes.InternalDBErrorInCorrespondenceCreation);
                  }
                 
                }
              //SCP210204: IS-WEB Outage (Retrive invoice Id of 1st Corr. only if current corr. dose not have invoice Id)
                ViewData[ViewDataConstants.InvoiceId] = correspondence.InvoiceId == new Guid() ? _paxCorrespondenceManager.GetCorrespondenceRelatedInvoice(transactionId, correspondence).Id : correspondence.InvoiceId;

                ShowSuccessMessage(Messages.MiscCorrespondenceUpdateSuccessful);

                return RedirectToAction("EditPaxCorrespondence", new
                {
                    transactionId = correspondence.Id.Value()
                });
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);

                correspondence.Attachments = _paxCorrespondenceManager.GetCorrespondenceAttachments(correspondenceAttachmentIds);
                var billingMember = _paxCorrespondenceManager.GetMember(correspondence.FromMemberId);
                var billedMember = _paxCorrespondenceManager.GetMember(correspondence.ToMemberId);
                correspondence.FromMember = billingMember;
                correspondence.ToMember = billedMember;
            }
            CheckCorrespondenceInitiator(correspondence.CorrespondenceNumber);
            ViewData[ViewDataConstants.InvoiceId] = correspondence.InvoiceId;
            GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action(CorrespondenceRejectionGridAction, new { invoiceId = correspondence.Id }));
            GetCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new { invoiceId = correspondence.Id }));
           
            return View(correspondence);
        }


        /// <summary>
        /// Fetch data for correspondence related rejections and display in grid.
        /// </summary>
        /// <param name="invoiceId">Correspondence ID</param>
        /// <returns></returns>
        public JsonResult CorrespondenceRejectionGridData(string invoiceId)
        {
            var correspondenceRejectionGrid =
                new CorrespondenceRejectionsGrid(ControlIdConstants.CorrespondenceRejectionsGridId,
                                                 Url.Action(CorrespondenceRejectionGridAction, new { invoiceId }));

            var corrId = ConvertUtil.ConvertGuidToString(invoiceId.ToGuid());
            var list = new PaxCorrespondenceManager().GetCorrespondenceRejectionMemoList(corrId);
            var gridData = correspondenceRejectionGrid.DataBind(list.AsQueryable());
            return gridData;
        }

        public JsonResult InitiateCorrespondenceRejectionGridData(string invoiceId)
        {
            var correspondenceRejectionGrid = new CorrespondenceRejectionsGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action("InitiateCorrespondenceRejectionGridData", new { invoiceId }));
            var rejectedMemos = TempData[TempDataConstants.RejectedRecordIds] as string;

            // SCP#481926 :PAXCREATECORRESPONDENCEFOR is slow (April 2016)
            var log = ReferenceManager.GetDebugLog(DateTime.Now, "InitiateCorrespondenceRejectionGridData", this.ToString(), "Passenger",
                                        "Step 5:InitiateCorrespondenceRejectionGridData Start", SessionUtil.UserId, logRefId.ToString());
            ReferenceManager.LogDebugData(log);

            var list = new PaxCorrespondenceManager().GetLinkedRejectionMemoList(rejectedMemos);

            log = ReferenceManager.GetDebugLog(DateTime.Now, "InitiateCorrespondenceRejectionGridData", this.ToString(), "Passenger",
                                         "Step 6:InitiateCorrespondenceRejectionGridData End", SessionUtil.UserId, logRefId.ToString());
            ReferenceManager.LogDebugData(log);

            if (list != null && list.Count > 0)
            {
              return correspondenceRejectionGrid.DataBind(list.AsQueryable());
            }

            return null;
        }

        private static IQueryable<LinkedRejection> CopyRejectionMemoToLinkedRejections(IQueryable<RejectionMemo> correspondenceRejectionTotal, PaxInvoice invoice)
        {
            var billingMember = string.Format("{0}-{1}-{2}", invoice.BillingMember.MemberCodeAlpha, invoice.BillingMember.MemberCodeNumeric, invoice.BillingMember.CommercialName);
            var invoiceNumber = invoice.InvoiceNumber;
            var period = string.Format("{0} {1} P{2}", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(invoice.BillingMonth), invoice.BillingYear, invoice.BillingPeriod);

            return correspondenceRejectionTotal.Select(rejectionMemo => new LinkedRejection
            {
                Id = rejectionMemo.Id.ToString(),
                InvoiceId = rejectionMemo.InvoiceId.ToString(),
                BillingMemberText = billingMember,
                DisplayBillingPeriod = period,
                InvoiceNumber = invoiceNumber,
                RejectionMemoNumber = rejectionMemo.RejectionMemoNumber,
                BillingCode = invoice.BillingCode
            }).ToList().AsQueryable();
        }

        public ActionResult ViewCorrespondence(string correspondenceId, string invoiceId)
        {
            /* SCP106534: ISWEB No-02350000768 
            Desc: Create corr is pushed to DB for better concurrency control. This prevents creation of orphan stage 1 corr in pax and cgo.
             * Added try-catch block to prevent screen crashing.
            Date: 20/06/2013*/
            try
            {
                SetViewDataPageMode(PageMode.View);
                if (string.IsNullOrEmpty(correspondenceId)) correspondenceId = invoiceId;
                //SCP85039: IS Web Performance Feedback / Billing History & Correspondence / Other issues
                //var correspondence = _paxCorrespondenceManager.GetPaxCorrespondenceDetails(correspondenceId);
                var corrId = ConvertUtil.ConvertGuidToString(correspondenceId.ToGuid());
                var correspondence = new PaxCorrespondenceManager().GetPaxCorrespondenceDetails(corrId);
                correspondence.Attachments = _paxCorrespondenceManager.GetPaxCorrespondenceAttachments(correspondenceId.ToGuid());
                //SCP210204: IS-WEB Outage (Retrive invoice Id of 1st Corr. only if current corr. dose not have invoice Id)
                ViewData[ViewDataConstants.InvoiceId] = correspondence.InvoiceId == new Guid() ? _paxCorrespondenceManager.GetCorrespondenceRelatedInvoice(correspondenceId, correspondence).Id : correspondence.InvoiceId;
                //CMP 573: User to be Able to Reply to a Correspondence by Clicking the URL in a Correspondence Alert
                ViewData[ViewDataConstants.IsCorrespondenceEligibleForReply] =
                  _paxCorrespondenceManager.IsCorrespondenceEligibleForReply((int)BillingCategoryType.Pax, correspondence.Id,
                                                                             SessionUtil.UserId, SessionUtil.MemberId,
                                                                             Business.Security.Permissions.Pax.
                                                                               BillingHistoryAndCorrespondence.
                                                                               DraftCorrespondence);

                GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId,
                                            Url.Action(CorrespondenceRejectionGridAction,
                                                       new
                                                       {
                                                           invoiceId = correspondenceId
                                                       }));
                GetCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId,
                                             Url.Action(CorrespondenceHistoryGridAction,
                                                        new
                                                        {
                                                            invoiceId = correspondenceId
                                                        }));

                // If logged in member is the To member Id of the correspondence, 
                // then correspondence sub-status should be seen as 'Received'.
                if (correspondence.ToMemberId == SessionUtil.MemberId && correspondence.CorrespondenceSubStatus != CorrespondenceSubStatus.AcceptedByCorrespondenceInitiator) correspondence.CorrespondenceSubStatus = CorrespondenceSubStatus.Received;
                
                return View("PaxCorrespondence", correspondence);
            }
            catch (Exception ex)
            {
                ShowErrorMessage("This Correspondence has already been updated by another user, please try again.", true);
                return RedirectToAction("Index", "BillingHistory", new { area = "Pax", back = true });
            }
        }
      
        [OutputCache(CacheProfile = "donotCache")]
        public ActionResult ReplyCorrespondence(string transactionId)
        {

            //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
            //var correspondenceInDb = _paxCorrespondenceManager.GetCorrespondenceDetails(transactionId);
            //var correspondenceInDb = _paxCorrespondenceManager.GetCorrespondenceDetailsForSaveAndSend(transactionId);
            var corrId = ConvertUtil.ConvertGuidToString(transactionId.ToGuid());
            var correspondenceInDb = new PaxCorrespondenceManager().GetPaxCorrespondenceDetailForReplyCorrespondence(corrId);
            //var invoice = _paxCorrespondenceManager.GetCorrespondenceRelatedInvoice(transactionId, correspondenceInDb);
            //SCP210204: IS-WEB Outage (Retrive invoice Id of 1st Corr. only if current corr. dose not have invoice Id)
            ViewData[ViewDataConstants.InvoiceId] = correspondenceInDb.InvoiceId;
            //ViewData[ViewDataConstants.SettlementMethodId] = invoice.SettlementMethodId;

            CheckCorrespondenceInitiator(correspondenceInDb.CorrespondenceNumber);

            //Mail ids of correspondence contact of to member.
            var toEmailIds = correspondenceInDb.ToEmailId;

            var correspondence = new Correspondence
            {
                CorrespondenceDate = DateTime.UtcNow,
                CorrespondenceStage = correspondenceInDb.CorrespondenceStage + 1,
                ToMember = correspondenceInDb.FromMember,
                ToMemberId = correspondenceInDb.FromMemberId,
                FromMember = correspondenceInDb.ToMember,
                FromMemberId = correspondenceInDb.ToMemberId,
                CurrencyId = correspondenceInDb.CurrencyId,
                AmountToBeSettled = correspondenceInDb.AmountToBeSettled,
                CorrespondenceOwnerId = SessionUtil.UserId,
                CorrespondenceOwnerName = SessionUtil.Username,
                ToEmailId = toEmailIds,
                CorrespondenceNumber = correspondenceInDb.CorrespondenceNumber,
                YourReference = correspondenceInDb.OurReference,
                CorrespondenceStatus = CorrespondenceStatus.Open,
                CorrespondenceSubStatus = CorrespondenceSubStatus.ReadyForSubmit,
                AuthorityToBill = correspondenceInDb.AuthorityToBill,
                Subject = correspondenceInDb.Subject,
                //CMP526 - Passenger Correspondence Identifiable by Source Code
                SourceCode = correspondenceInDb.SourceCode,
                /* CMP#657: Retention of Additional Email Addresses in Correspondences
                   Adding code to get email ids from initiator and non-initiator*/
                AdditionalEmailInitiator = correspondenceInDb.AdditionalEmailInitiator,
                AdditionalEmailNonInitiator = correspondenceInDb.AdditionalEmailNonInitiator
            };

            GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId,
                                        Url.Action(CorrespondenceRejectionGridAction,
                                                   new
                                                   {
                                                       invoiceId = transactionId
                                                   }));
            GetCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId,
                                         Url.Action(CorrespondenceHistoryGridAction,
                                                    new
                                                    {
                                                        invoiceId = transactionId
                                                    }));

            TempData["PreviousCorrespondenceId"] = transactionId;

            return View("PaxCreateCorrespondence", correspondence);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        //Check for Correspondence Updates.
        [RestrictInvoiceUpdate(InvParamName = "invoiceId",
                               CorrespondenceParamName = "correspondence",
                               InvList = false, TableName = TransactionTypeTable.PAX_CORRESPONDENCE,
                               ActionParamName = "ViewCorrespondence")]
        public ActionResult CreateAndSendCorrespondence(Correspondence correspondence, string invoiceId)
        {
            correspondence.CorrespondenceStatus = CorrespondenceStatus.Open;
            correspondence.CorrespondenceSubStatus = CorrespondenceSubStatus.Responded;
            correspondence.CorrespondenceOwnerId = SessionUtil.UserId;
            var correspondenceAttachmentIds = correspondence.Attachments.Select(attachment => attachment.Id).ToList();

            /* SCP106534: ISWEB No-02350000768 
             Desc: Added support for operation status parameter
             Date: 20/06/2013*/
            int operationStatusIndicator = -1;

            try
            {

                //Get attachment Id list
                
                correspondence.Attachments.Clear();
                //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
                //Commented below line as it is required only in else condition
                //var originalCorrespondence = _paxCorrespondenceManager.GetOriginalCorrespondenceDetails(correspondence.CorrespondenceNumber);
                var invoiceIdToInsert = Guid.Empty;
                //CMP526 - Passenger Correspondence Identifiable by Source Code
                int? sourceCode = null;

                if (correspondence.CorrespondenceStage > 1)
                {

                    correspondence = new PaxCorrespondenceManager().GetPaxCorrespondenceFieldDetails(Convert.ToInt64(correspondence.CorrespondenceNumber), 1, correspondence);
                    invoiceIdToInsert = correspondence.InvoiceId;  
                    sourceCode = correspondence.SourceCode;
                    //CMP526 - Passenger Correspondence Identifiable by Source Code
                    if (!sourceCode.HasValue)
                        throw new ISBusinessException("The correspondence trail could not be found.");
                }

                if (invoiceIdToInsert == Guid.Empty)
                    if (invoiceId != null)
                    {
                        invoiceIdToInsert = invoiceId.ToGuid();
                    }
                correspondence.InvoiceId = invoiceIdToInsert;
                var currentPeriod = _calendarManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);
                correspondence.CorrespondenceSentOnDate = new DateTime(currentPeriod.Year, currentPeriod.Month, currentPeriod.Period);

                //TFS Bug 9737 - Rel 1.7.17.0 : “To E-Mail ID(s)” 'is not re-populating at the time of 'Sending' the correspondence for Passenger.
                //SCP#446268: Correspondence Problem (Check if TO_Email ID is blank re-fetch the contacts from Profile)
                if (String.IsNullOrEmpty(correspondence.ToEmailId))
                {
                    //Mail ids of correspondence contact of To Member.
                    var toEmail = _paxCorrespondenceManager.GetToEmailIds(correspondence.ToMemberId, ProcessingContactType.PaxCorrespondence);
                    if (!String.IsNullOrEmpty(toEmail))
                        correspondence.ToEmailId = toEmail;
                }

                correspondence = _paxCorrespondenceManager.AddCorrespondenceAndUpdateRejection(correspondence,
                                                                                                  correspondenceAttachmentIds,
                                                                                                  SessionUtil.
                                                                                                      RejectionMemoRecordIds, ref operationStatusIndicator);

                correspondence = _paxCorrespondenceManager.GetCorrespondenceDetailsForSaveAndSend(correspondence.Id.ToString());

                var correspondenceUrl = string.Format("{0}/{1}", UrlHelperEx.ToAbsoluteUrl(Url.Action("ViewLinkedCorrespondence", "Correspondence")), correspondence.Id);

                // Send the correspondence.

                /* CMP#657: Retention of Additional Email Addresses in Correspondences
                    Adding code to get email ids from initiator and non-initiator*/
                var toEmailIds = _correspondenceManager.GetEmailIdsList(correspondence.ToEmailId,
                                                                        correspondence.AdditionalEmailInitiator,
                                                                        correspondence.AdditionalEmailNonInitiator);
                
                Logger.InfoFormat("Before CreateAndSendCorrespondence email time: {0}", DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss:ffff"));
                // CMP#657: Retention of Additional Email Addresses in Correspondences.
                // Logic Moved to common location. i.e in corresponceManager.cs  
                // _paxCorrespondenceManager.SendCorrespondenceMail(correspondenceUrl, toEmailIds, string.Format("{0}-{1}-{2}", "SIS:Correspondence", correspondence.CorrespondenceNumber, correspondence.Subject));
                var frmMember = _correspondenceManager.GetCachedCopyOfMemberUsingId(correspondence.FromMemberId);
                var toMember = _correspondenceManager.GetCachedCopyOfMemberUsingId(correspondence.ToMemberId);
              _correspondenceManager.EmailAlertsOnSendingOfCorrespondences(BillingCategoryType.Pax, correspondenceUrl, toEmailIds,
                                                                           string.Format("{0}-{1}-{2}", "SIS:Correspondence", correspondence.CorrespondenceNumber, correspondence.Subject),
                                                                           string.Format("{0}-{1}", frmMember.MemberCodeAlpha, frmMember.MemberCodeNumeric),
                                                                           string.Format("{0}-{1}", toMember.MemberCodeAlpha, toMember.MemberCodeNumeric));
                Logger.InfoFormat("After CreateAndSendCorrespondence email time: {0}", DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss:ffff"));

                ShowSuccessMessage(Messages.MiscCorrespondenceSentSuccessful);

                return RedirectToAction("Correspondence", "Correspondence", new
                {
                    invoiceId = correspondence.Id.Value()
                });
            }
            catch (ISBusinessException exception)
            {
                /* SCP106534: ISWEB No-02350000768 
                 Desc: Added support for operation status parameter.
                  * it is 1 or 2 or 3 then this is business error while adding corr in DB because of paralel operation so it is success
                 Date: 20/06/2013*/
                //SCP210204: IS-WEB Outage (QA Issue Fix)
                if (operationStatusIndicator != -1 && operationStatusIndicator != 4 && correspondence.Id != Guid.Empty)
                {
                    ShowErrorMessage(exception.ErrorCode, true);
                    return RedirectToAction("Correspondence", "Correspondence", new
                    {
                        invoiceId = correspondence.Id.Value()
                    });
                }

                ViewData[ViewDataConstants.InvoiceId] = correspondence.InvoiceId;
                ShowErrorMessage(exception.ErrorCode);
                correspondence.Attachments = _paxCorrespondenceManager.GetCorrespondenceAttachments(correspondenceAttachmentIds);
                var billingMember = _paxCorrespondenceManager.GetMember(correspondence.FromMemberId);
                var billedMember = _paxCorrespondenceManager.GetMember(correspondence.ToMemberId);
                correspondence.FromMember = billingMember;
                correspondence.ToMember = billedMember;
            }

            if (correspondence.Id != Guid.Empty)
            {
              GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action(CorrespondenceRejectionGridAction, new { invoiceId = correspondence.Id }));
              GetCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new { invoiceId = correspondence.Id }));
              CheckCorrespondenceInitiator(correspondence.CorrespondenceNumber);
            }
            else
            {
              var previousCorrespondenceId = Guid.Empty;
              if (TempData != null)
              {
                if (!string.IsNullOrWhiteSpace(Convert.ToString(TempData["PreviousCorrespondenceId"])))
                {
                  var previousCorrId = Convert.ToString(TempData["PreviousCorrespondenceId"]);
                  previousCorrespondenceId = previousCorrId.ToGuid();
                }
              }
              if (correspondence.CorrespondenceStage > 1 && previousCorrespondenceId != Guid.Empty)
              {
                GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action(CorrespondenceRejectionGridAction, new { invoiceId = previousCorrespondenceId }));
                GetCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new { invoiceId = previousCorrespondenceId }));

                CheckCorrespondenceInitiator(correspondence.CorrespondenceNumber);
                TempData["PreviousCorrespondenceId"] = previousCorrespondenceId;
              }
              else
              {
                char[] seperator = { ',' };
                var rejectedMemos = SessionUtil.RejectionMemoRecordIds.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
                var rejectedMemoIds = rejectedMemos;
                if (!string.IsNullOrEmpty(SessionUtil.RejectionMemoRecordIds))
                {
                  if (rejectedMemoIds != null && rejectedMemoIds.Count() > 0)
                  {
                    string sRejectedMemoIds = string.Join(",", rejectedMemoIds);

                    GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action("InitiateCorrespondenceRejectionGridData",
                                                                                                              new
                                                                                                              {
                                                                                                                invoiceId = correspondence.InvoiceId,
                                                                                                                rejectedMemoIds = sRejectedMemoIds
                                                                                                              }));
                    TempData[TempDataConstants.RejectedRecordIds] = sRejectedMemoIds;

                  }
                  GetCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new { invoiceId = correspondence.InvoiceId }));

                  CheckCorrespondenceInitiator(correspondence.CorrespondenceNumber);
                }
              }

            }
            
            return View("PaxCreateCorrespondence", correspondence);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        //Check for Correspondence Updates.
        [RestrictInvoiceUpdate(TransactionParamName = "transactionId",
                               CorrespondenceParamName = "correspondence",
                               InvList = false, TableName = TransactionTypeTable.PAX_CORRESPONDENCE,
                               ActionParamName = "ViewCorrespondence")]
        public ActionResult SendCorrespondence(string transactionId, Correspondence correspondence)
        {
            correspondence.CorrespondenceStatus = CorrespondenceStatus.Open;
            correspondence.CorrespondenceSubStatus = CorrespondenceSubStatus.Responded;
            correspondence.CorrespondenceOwnerId = SessionUtil.UserId;
            //CMP526 - Passenger Correspondence Identifiable by Source Code
            int? sourceCode = null;
            var currentPeriod = _calendarManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);
            correspondence.CorrespondenceSentOnDate = new DateTime(currentPeriod.Year, currentPeriod.Month, currentPeriod.Period);
            correspondence.CorrespondenceDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month,
                                                             DateTime.UtcNow.Day);
            var correspondenceAttachmentIds = correspondence.Attachments.Select(attachment => attachment.Id).ToList();
            try
            {
                correspondence.Id = transactionId.ToGuid();

                // Get the absolute url for the correspondence.
                var correspondenceUrl = string.Format("{0}/{1}", UrlHelperEx.ToAbsoluteUrl(Url.Action("ViewCorrespondence", "Correspondence")), transactionId);

                /* CMP#657: Retention of Additional Email Addresses in Correspondences
                   Adding code to get email ids from initiator and non-initiator*/
                var toEmailIds = _correspondenceManager.GetEmailIdsList(correspondence.ToEmailId,
                                                                        correspondence.AdditionalEmailInitiator,
                                                                        correspondence.AdditionalEmailNonInitiator);
                //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
                //.GetCorrespondenceDetailsForSaveAndSend method is used instead of GetCorrespondenceDetails.

                //var savedCorr = _paxCorrespondenceManager.GetCorrespondenceDetailsForSaveAndSend(transactionId);
                correspondence = new PaxCorrespondenceManager().GetPaxCorrespondenceFieldDetails(correspondence.Id, correspondence);
                    

                // Check if correspondence is already in responded status. 
                CheckCorrespondenceStatus(correspondence);

                //CMP526 - Passenger Correspondence Identifiable by Source Code
                sourceCode = correspondence.SourceCode;
                if (!sourceCode.HasValue)
                    throw new ISBusinessException("The correspondence trail could not be found.");

                correspondence.CorrespondenceStatus = CorrespondenceStatus.Open;
                correspondence.CorrespondenceSubStatus = CorrespondenceSubStatus.Responded;
                correspondence.CorrespondenceOwnerId = SessionUtil.UserId;

                // SCP109163
                // Check if correspondence expiry date is crossed.
                if (correspondence.ExpiryDate < DateTime.UtcNow.Date)
                {
                    throw new ISBusinessException(ErrorCodes.ExpiredCorrespondence);
                }
                

                //correspondence = _paxCorrespondenceManager.UpdateCorrespondence(correspondence);
                if (_paxCorrespondenceManager.ValidateCorrespondence(correspondence))
                {
                  //validate attachment for duplicate file name
                  
                  var dbAttachments = _paxCorrespondenceManager.GetPaxCorrespondenceAttachments(transactionId.ToGuid());
                  var corrAttachments = _paxCorrespondenceManager.GetCorrespondenceAttachments(correspondenceAttachmentIds);
                  var isvalidAttachments = new PaxCorrespondenceManager().ValidateCorrespondenceAttachmentFileName(dbAttachments, corrAttachments);
                  if (!isvalidAttachments)
                  {
                    throw new ISBusinessException(ErrorCodes.DuplicateFileName);
                  }

                  var result = new PaxCorrespondenceManager().UpdatePaxCorrespondence(correspondence);
                  if (!result)
                  {
                    throw new ISBusinessException(ErrorCodes.InternalDBErrorInCorrespondenceCreation);
                  }
                  correspondence = _paxCorrespondenceManager.GetCorrespondenceDetailsForSaveAndSend(correspondence.Id.ToString());
                }

                //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
                //Commented the log
                // Logger.InfoFormat("Before SendCorrespondence email time: {0}", DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss:ffff"));
                // CMP#657: Retention of Additional Email Addresses in Correspondences.
                // Logic Moved to common location. i.e in corresponceManager.cs
                // _paxCorrespondenceManager.SendCorrespondenceMail(correspondenceUrl, toEmailIds, string.Format("{0}-{1}-{2}", "SIS:Correspondence", correspondence.CorrespondenceNumber, correspondence.Subject));
                var frmMember = _correspondenceManager.GetCachedCopyOfMemberUsingId(correspondence.FromMemberId);
                var toMember = _correspondenceManager.GetCachedCopyOfMemberUsingId(correspondence.ToMemberId);
                _correspondenceManager.EmailAlertsOnSendingOfCorrespondences(BillingCategoryType.Pax, correspondenceUrl, toEmailIds,
                                                                             string.Format("{0}-{1}-{2}", "SIS:Correspondence", correspondence.CorrespondenceNumber, correspondence.Subject),
                                                                             string.Format("{0}-{1}", frmMember.MemberCodeAlpha, frmMember.MemberCodeNumeric),
                                                                             string.Format("{0}-{1}", toMember.MemberCodeAlpha, toMember.MemberCodeNumeric));
                //Logger.InfoFormat("After SendCorrespondence email time: {0}", DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss:ffff"));
                //}

                ShowSuccessMessage(Messages.MiscCorrespondenceSentSuccessful);

                return RedirectToAction("Correspondence", "Correspondence", new
                {
                    invoiceId = transactionId
                });

            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                correspondence.Attachments = _paxCorrespondenceManager.GetCorrespondenceAttachments(correspondenceAttachmentIds);
                var billingMember = _paxCorrespondenceManager.GetMember(correspondence.FromMemberId);
                var billedMember = _paxCorrespondenceManager.GetMember(correspondence.ToMemberId);
                correspondence.FromMember = billingMember;
                correspondence.ToMember = billedMember;
            }
            //SCP210204: IS-WEB Outage (Retrive invoice Id of 1st Corr. only if current corr. dose not have invoice Id)
            ViewData[ViewDataConstants.InvoiceId] = correspondence.InvoiceId == new Guid() ? _paxCorrespondenceManager.GetCorrespondenceRelatedInvoice(transactionId, correspondence).Id : correspondence.InvoiceId;


            CheckCorrespondenceInitiator(correspondence.CorrespondenceNumber);

            var rejectedMemoIds = TempData[TempDataConstants.RejectedRecordIds] as string[];

            if (rejectedMemoIds != null)
            {
                string sRejectedMemoIds = string.Join(",", rejectedMemoIds);
                GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action("InitiateCorrespondenceRejectionGridData",
                                                                                                          new
                                                                                                          {
                                                                                                              invoiceId = correspondence.InvoiceId,
                                                                                                              rejectedMemoIds = sRejectedMemoIds
                                                                                                          }));
                TempData[TempDataConstants.RejectedRecordIds] = rejectedMemoIds;
            }
            //SCP210204: IS-WEB Outage (QA Issue Fix)
            else
            {
                GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId,
                                             Url.Action(CorrespondenceRejectionGridAction,
                                                           new { invoiceId = correspondence.Id }));
            }

            GetCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new { invoiceId = correspondence.Id }));


            return View("EditPaxCorrespondence", correspondence);
        }

        public FilePathResult DownloadCorrespondence(string transactionId)
        {
            FileStream file = null;
            try
            {
                string pdfPath = _paxCorrespondenceManager.CreateCorrespondenceFormatPdf(transactionId);

                file = System.IO.File.Open(pdfPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                return File(pdfPath, "application/pdf");
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
            }
        }

        /// <summary>
        /// Download Correspondence Attachment  
        /// </summary>
        /// <param name="invoiceId">Invoice Id</param>
        /// <returns></returns>
        [HttpGet]
        public FileStreamResult CorrespondenceAttachmentDownload(string invoiceId)
        {
            var fileDownloadHelper = new FileAttachmentHelper
            {
                Attachment = _paxCorrespondenceManager.GetCorrespondenceAttachmentDetail(invoiceId)
            };

            return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
        }

        [HttpGet]
        public ActionResult Correspondence(string invoiceId)
        {
            // SCPID133670 - Missing attachment in SIS for incoming and outgoing
            //SCP85039: IS Web Performance Feedback / Billing History & Correspondence / Other issues
            //var correspondence = _paxCorrespondenceManager.GetPaxCorrespondenceDetails(invoiceId);
            var corrId = ConvertUtil.ConvertGuidToString(invoiceId.ToGuid());
            var correspondence = new PaxCorrespondenceManager().GetPaxCorrespondenceDetails(corrId);
            correspondence.Attachments = _paxCorrespondenceManager.GetPaxCorrespondenceAttachments(invoiceId.ToGuid());
           
            //CMP 573: User to be Able to Reply to a Correspondence by Clicking the URL in a Correspondence Alert
            ViewData[ViewDataConstants.IsCorrespondenceEligibleForReply] =
              _paxCorrespondenceManager.IsCorrespondenceEligibleForReply((int)BillingCategoryType.Pax, correspondence.Id,
                                                                         SessionUtil.UserId, SessionUtil.MemberId,
                                                                         Business.Security.Permissions.Pax.
                                                                           BillingHistoryAndCorrespondence.
                                                                           DraftCorrespondence);
            /*
             Due to  ID : 133670 - Missing attachment in SIS for incoming and outgoing. Undo changes related to SCP 85039. 
            //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
            // var correspondence = _paxCorrespondenceManager.GetCorrespondenceDetailsForSaveAndSend(invoiceId);
            */
            try
            {
                //SCP210204: IS-WEB Outage (Retrive invoice Id of 1st Corr. only if current corr. dose not have invoice Id)
                ViewData[ViewDataConstants.InvoiceId] = correspondence.InvoiceId == new Guid() ? _paxCorrespondenceManager.GetCorrespondenceRelatedInvoice(invoiceId, correspondence).Id : correspondence.InvoiceId;

                //check whether transactions exist for this invoice.
                if (correspondence != null)
                {
                    //INC 8863, I get an unexpected error occurred.
                    // var linkedRejections = _paxCorrespondenceManager.GetCorrespondenceRejectionList(invoiceId).Select(rejection => rejection.Id);
                    // TempData[TempDataConstants.RejectedRecordIds] = string.Join(",", linkedRejections);

                    if (correspondence.CorrespondenceStatus != CorrespondenceStatus.Closed && (correspondence.CorrespondenceSubStatus == CorrespondenceSubStatus.Saved || correspondence.CorrespondenceSubStatus == CorrespondenceSubStatus.ReadyForSubmit) && correspondence.FromMemberId == SessionUtil.MemberId)
                    {

                        return RedirectToAction("EditPaxCorrespondence", "Correspondence", new { transactionId = correspondence.Id });
                    }
                    ViewData[ViewDataConstants.TransactionExists] = true;
                }
                else
                {
                    return RedirectToAction("PaxCreateCorrespondence", "Correspondence", new { invoiceId });
                }

                GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action(CorrespondenceRejectionGridAction, new { invoiceId = correspondence.Id }));
                GetCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new { invoiceId = correspondence.Id }));

                ViewData[ViewDataConstants.ReplyCorrespondence] = false;

                //CMP527: Start 
                if (_authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.Pax.BillingHistoryAndCorrespondence.AcceptAndCloseCorrespondence))
                {
                    var corrCanClose = _paxCorrespondenceManager.CanCorrespondenceClose(paxBillingCategory, invoiceId);
                    var closeCorrespondence = Convert.ToBoolean(corrCanClose[(int)CorrespondenceCloseStatus.CorrespondenceCanClose]);

                    if (closeCorrespondence)
                    {
                        var correspondenceInDb = _paxCorrespondenceManager.GetOriginalCorrespondenceDetails(correspondence.CorrespondenceNumber);
                        ViewData[ViewDataConstants.CorrespondenceCanClose] = correspondenceInDb.FromMemberId ==
                                                                             SessionUtil.MemberId;
                        ViewData[ViewDataConstants.CorrespondeneClosedScenario] =
                          corrCanClose[(int)CorrespondenceCloseStatus.CorrespondenceCloseScenario];
                    }
                }
                //CMP527: End
                if (correspondence.ToMemberId == SessionUtil.MemberId && correspondence.CorrespondenceSubStatus != CorrespondenceSubStatus.AcceptedByCorrespondenceInitiator)
                {
                    correspondence.CorrespondenceSubStatus = CorrespondenceSubStatus.Received;
                }


            }
            catch (Exception exception)
            {
                ShowErrorMessage("Error occured while retrieving the correspondence details. Please try again, or contact your administrator", true);
                
                return RedirectToAction("Index", "BillingHistory", new { area = "Pax", back = true });

            }
            
            return View("PaxCorrespondence", correspondence);
        }

        [ISAuthorize(Business.Security.Permissions.Pax.BillingHistoryAndCorrespondence.ViewCorrespondence)]
        public ActionResult ViewLinkedCorrespondence(string invoiceId)
        {
            //SCP85039: IS Web Performance Feedback / Billing History & Correspondence / Other issues
            //var correspondence = _paxCorrespondenceManager.GetPaxCorrespondenceDetails(invoiceId);
            var corrId = ConvertUtil.ConvertGuidToString(invoiceId.ToGuid());
            var correspondence = new PaxCorrespondenceManager().GetPaxCorrespondenceDetails(corrId);
            correspondence.Attachments = _paxCorrespondenceManager.GetPaxCorrespondenceAttachments(invoiceId.ToGuid());

          //CMP 573: User to be Able to Reply to a Correspondence by Clicking the URL in a Correspondence Alert
          ViewData[ViewDataConstants.IsCorrespondenceEligibleForReply] =
            _paxCorrespondenceManager.IsCorrespondenceEligibleForReply((int) BillingCategoryType.Pax, correspondence.Id,
                                                                       SessionUtil.UserId,SessionUtil.MemberId,
                                                                       Business.Security.Permissions.Pax.
                                                                         BillingHistoryAndCorrespondence.
                                                                         DraftCorrespondence);

            GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action(CorrespondenceRejectionGridAction, new { invoiceId = correspondence.Id }));
            GetCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new { invoiceId = correspondence.Id }));
            //SCP210204: IS-WEB Outage (Retrive invoice Id of 1st Corr. only if current corr. dose not have invoice Id)
            ViewData[ViewDataConstants.InvoiceId] = correspondence.InvoiceId == new Guid() ? _paxCorrespondenceManager.GetCorrespondenceRelatedInvoice(invoiceId, correspondence).Id : correspondence.InvoiceId;

            SetViewDataPageMode(PageMode.View);
            if (correspondence.ToMemberId == SessionUtil.MemberId && correspondence.CorrespondenceSubStatus != CorrespondenceSubStatus.AcceptedByCorrespondenceInitiator) correspondence.CorrespondenceSubStatus = CorrespondenceSubStatus.Received;
            
            return View("PaxCorrespondence", correspondence);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        //Check for Correspondence Updates.
        [RestrictInvoiceUpdate(InvParamName = "invoiceId",
                               TransactionParamName = "transactionId",
                               CorrespondenceParamName = "correspondence",
                               InvList = false, TableName = TransactionTypeTable.PAX_CORRESPONDENCE,
                               ActionParamName = "ViewCorrespondence")]
        public ActionResult ReadyToSubmitCorrespondence(string invoiceId, string transactionId, Correspondence correspondence)
        {
            Correspondence originalCorr = null;
            bool createdSuccessfully = true;

            //Get attachment Id list

            var correspondenceAttachmentIds = correspondence.Attachments.Select(attachment => attachment.Id).ToList();

            correspondence.CorrespondenceStatus = CorrespondenceStatus.Open;
            correspondence.CorrespondenceSubStatus = CorrespondenceSubStatus.ReadyForSubmit;

            /* SCP106534: ISWEB No-02350000768 
              Desc: Added support for operation status parameter
              Date: 20/06/2013*/
            int operationStatusIndicator = -1;

            try
            {
                //CMP526 - Passenger Correspondence Identifiable by Source Code
                int? sourceCode = null;
                if (transactionId.ToGuid() == Guid.Empty)
                {

                    correspondence.CorrespondenceOwnerId = SessionUtil.UserId;
                    
                    correspondence = _paxCorrespondenceManager.AddCorrespondenceAndUpdateRejection(correspondence,
                                                                                                  correspondenceAttachmentIds,
                                                                                            SessionUtil.
                                                                                                      RejectionMemoRecordIds, ref operationStatusIndicator);

                    ShowSuccessMessage(Messages.PaxCorrespondenceCreateSuccessful);
                    
                }
                else
                {

                    correspondence.Id = transactionId.ToGuid();

                    //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
                    //Get CorrepondaenceDetails for save and send. because only InvoiceId and CorrespondenceOwnerId is used
                    var corrId = ConvertUtil.ConvertGuidToString(transactionId.ToGuid());
                    var corrDetails = new PaxCorrespondenceManager().GetPaxCorrespondenceFieldDetails(transactionId.ToGuid());


                    //Check correspondence status
                    CheckCorrespondenceStatus(corrDetails);

                    if (corrDetails != null)
                    {
                        correspondence.InvoiceId = corrDetails.InvoiceId;
                        correspondence.CorrespondenceOwnerId = corrDetails.CorrespondenceOwnerId;
                        //CMP526 - Passenger Correspondence Identifiable by Source Code
                        sourceCode = corrDetails.SourceCode;
                        // SCP109163
                        // Check if correspondence expiry date is crossed.
                        if (corrDetails.ExpiryDate < DateTime.UtcNow.Date)
                        {
                            throw new ISBusinessException(ErrorCodes.ExpiredCorrespondence);
                        }
                        //SCP210204: IS-WEB Outage (QA Issue Fix)
                        correspondence.ExpiryDate = corrDetails.ExpiryDate;

                    }

                    //CMP526 - Passenger Correspondence Identifiable by Source Code
                    if (!sourceCode.HasValue)
                        throw new ISBusinessException("The correspondence trail could not be found.");

                    correspondence.SourceCode = sourceCode;

                    //correspondence = _paxCorrespondenceManager.UpdateCorrespondence(correspondence);
                    if (_paxCorrespondenceManager.ValidateCorrespondence(correspondence))
                    {
                      //validate attachment for duplicate file name
                      var dbAttachments = _paxCorrespondenceManager.GetPaxCorrespondenceAttachments(transactionId.ToGuid());
                      var corrAttachments = _paxCorrespondenceManager.GetCorrespondenceAttachments(correspondenceAttachmentIds);
                      var isvalidAttachments = new PaxCorrespondenceManager().ValidateCorrespondenceAttachmentFileName(dbAttachments, corrAttachments);
                      if (!isvalidAttachments)
                      {
                        throw new ISBusinessException(ErrorCodes.DuplicateFileName);
                      }
                      var result = new PaxCorrespondenceManager().UpdatePaxCorrespondence(correspondence);
                      if (!result)
                      {
                        throw new ISBusinessException(ErrorCodes.InternalDBErrorInCorrespondenceCreation);
                      }
                    }

                    ShowSuccessMessage(Messages.MiscCorrespondenceUpdateSuccessful);
                }

                return RedirectToAction("EditPaxCorrespondence", new
                {
                    transactionId = correspondence.Id.Value()
                });
            }
            catch (ISBusinessException exception)
            {
                /* SCP106534: ISWEB No-02350000768 
                 Desc: Added support for operation status parameter.
                  * it is 1 or 2 or 3 then this is business error while adding corr in DB because of paralel operation so it is success
                 Date: 20/06/2013*/
                //SCP210204: IS-WEB Outage (QA Issue Fix)
              if (operationStatusIndicator != -1 && operationStatusIndicator != 4 && correspondence.Id != Guid.Empty)
                {
                    ShowErrorMessage(exception.ErrorCode, true);
                    return RedirectToAction("EditPaxCorrespondence", new
                    {
                        transactionId = correspondence.Id.Value()
                    });
                }

                ShowErrorMessage(exception.ErrorCode);
                correspondence.Attachments = _paxCorrespondenceManager.GetCorrespondenceAttachments(correspondenceAttachmentIds);
                var billingMember = _paxCorrespondenceManager.GetMember(correspondence.FromMemberId);
                var billedMember = _paxCorrespondenceManager.GetMember(correspondence.ToMemberId);
                correspondence.FromMember = billingMember;
                correspondence.ToMember = billedMember;

                ViewData[ViewDataConstants.InvoiceId] = correspondence.InvoiceId;

            }
            if (correspondence.Id != Guid.Empty)
            {
              GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action(CorrespondenceRejectionGridAction, new { invoiceId = correspondence.Id }));
              GetCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new { invoiceId = correspondence.Id }));
              CheckCorrespondenceInitiator(correspondence.CorrespondenceNumber);
            }
            else
            {
              var previousCorrespondenceId = Guid.Empty;
              if(TempData != null)
              {
                if (!string.IsNullOrWhiteSpace(Convert.ToString(TempData["PreviousCorrespondenceId"])))
                {
                  var previousCorrId = Convert.ToString(TempData["PreviousCorrespondenceId"]);
                  previousCorrespondenceId = previousCorrId.ToGuid();
                }
              }
              if (correspondence.CorrespondenceStage > 1 && previousCorrespondenceId != Guid.Empty)
              {
                GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action(CorrespondenceRejectionGridAction, new { invoiceId = previousCorrespondenceId }));
                GetCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new { invoiceId = previousCorrespondenceId }));
                
                CheckCorrespondenceInitiator(correspondence.CorrespondenceNumber);
                TempData["PreviousCorrespondenceId"] = previousCorrespondenceId;
              }
              else
              {
                char[] seperator = { ',' };
                var rejectedMemos = SessionUtil.RejectionMemoRecordIds.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
                var rejectedMemoIds = rejectedMemos;
                if (!string.IsNullOrEmpty(SessionUtil.RejectionMemoRecordIds))
                {
                  if (rejectedMemoIds != null && rejectedMemoIds.Count() > 0)
                  {
                    string sRejectedMemoIds = string.Join(",", rejectedMemoIds);

                    GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action("InitiateCorrespondenceRejectionGridData",
                                                                                                              new
                                                                                                              {
                                                                                                                invoiceId = correspondence.InvoiceId,
                                                                                                                rejectedMemoIds = sRejectedMemoIds
                                                                                                              }));
                    TempData[TempDataConstants.RejectedRecordIds] = sRejectedMemoIds;

                  }
                  GetCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new { invoiceId = correspondence.InvoiceId }));

                  CheckCorrespondenceInitiator(correspondence.CorrespondenceNumber);
                }
              }
              
            }

            
            if (string.IsNullOrEmpty(transactionId) || transactionId.Equals(Guid.Empty.ToString()))
            {
                return View("PaxCreateCorrespondence", correspondence);
            }
            
            return View("EditPaxCorrespondence", correspondence);
        }


        public JsonResult InitiateCorrespondence(string rejectedRecordIds)
        {
            if (!new PaxCorrespondenceManager().CheckCorrepondenceAlreadyExistsForRm(rejectedRecordIds))
            {
                return Json(new UIMessageDetail
                {
                    isRedirect = false
                });
            }

            TempData[TempDataConstants.RejectedRecordIds] = rejectedRecordIds;

            SessionUtil.RejectionMemoRecordIds = rejectedRecordIds;

            return Json(new UIMessageDetail
            {
                isRedirect = true
            });
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult CorrespondenceAttachmentUpload(string invoiceId, string transactionId)
        {
            var files = string.Empty;
            var attachments = new List<CorrespondenceAttachment>();
          // SCP335749: SRM Admin Alert - Offline collection generation failure notification - SIS Production 05 FEB 2015 [Pax]  
          var isUploadSuccess = false;
            string message;
            HttpPostedFileBase fileToSave;
            FileAttachmentHelper fileUploadHelper = null;
            try
            {
                foreach (string file in Request.Files)
                {
                  isUploadSuccess = false;
                    fileToSave = Request.Files[file];
                    if (Equals(fileToSave, null) || String.IsNullOrEmpty(fileToSave.FileName) || !(fileToSave.ContentLength > 0)) continue;

                    if (invoiceId == null)
                    {
                        invoiceId = _paxCorrespondenceManager.GetCorrespondenceRelatedInvoice(transactionId).Id.ToString();
                    }
                    //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
                    //Get invoice header details
                    PaxInvoice invoice = NonSamplingInvoiceManager.GetInvoiceDetailForFileUpload(invoiceId);

                    fileUploadHelper = new FileAttachmentHelper
                    {
                        FileToSave = fileToSave,
                        FileRelativePath = String.Format("{0}_{1}_{2}", invoice.BilledMemberId, invoice.BillingYear,
                                                  invoice.BillingMonth)
                    };

                    //On Correpondence create/update
                    if (!Equals(transactionId.ToGuid(), Guid.Empty) && _paxCorrespondenceManager.IsDuplicatePaxCorrespondenceAttachmentFileName(fileUploadHelper.FileOriginalName,
                                                                                       transactionId.ToGuid()))
                    {
                        throw new ISBusinessException(Messages.FileDuplicateError);
                    }
                    if (fileUploadHelper.InvalidCharCheck(fileUploadHelper.FileOriginalName))
                    {
                        throw new ISBusinessException(Messages.InvalidFileName);
                    }
                    if (!fileUploadHelper.ValidateFileExtention(invoice.BilledMemberId, invoice.BillingCategory))
                    {
                        throw new ISBusinessException(Messages.InvalidFileExtension);
                    }
                    if (fileUploadHelper.SaveFile())
                    {
                        files = String.Format("{0}{1},", files, fileUploadHelper.FileOriginalName);
                        var attachment = new CorrespondenceAttachment
                        {
                            Id = fileUploadHelper.FileServerName,
                            OriginalFileName = fileUploadHelper.FileOriginalName,
                            // Convert file size to KB.
                            FileSize = fileUploadHelper.FileToSave.ContentLength,
                            LastUpdatedBy = SessionUtil.UserId,
                            ServerId = fileUploadHelper.FileServerInfo.ServerId,
                            FileStatus = FileStatusType.Received,
                            FilePath = fileUploadHelper.FileRelativePath
                        };
                        attachment = _paxCorrespondenceManager.AddCorrespondenceAttachment(attachment);

                        if (attachment.UploadedBy == null)
                        {
                            attachment.UploadedBy = new User();
                        }

                        //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
                        // assign user info from session and file server info.
                        attachment.UploadedBy.Id = SessionUtil.UserId;
                        attachment.UploadedBy.FirstName = SessionUtil.Username;
                        isUploadSuccess = true;
                        attachments.Add(attachment);
                    }
                }
                message = string.Format(Messages.FileUploadSuccessful, files.TrimEnd(','));
            }
            catch (ISBusinessException ex)
            {
                message = string.Format(Messages.FileUploadBusinessException, ex.ErrorCode);
                if (fileUploadHelper != null && isUploadSuccess == false) fileUploadHelper.DeleteFile();
            }
            catch (Exception)
            {
                message = Messages.FileUploadUnexpectedError;
                if (fileUploadHelper != null && isUploadSuccess == false) fileUploadHelper.DeleteFile();
            }

            return new FileUploadJsonResult { Data = new { IsFailed = !isUploadSuccess, Message = message, Attachment = attachments, Length = attachments.Count } };
        }

        private void GetCorrespondenceRejectGrid(string conrolId, string urlAction)
        {
            var gridModel = new CorrespondenceRejectionsGrid(conrolId, urlAction);
            ViewData[ViewDataConstants.CorrespondenceRejectionsGrid] = gridModel.Instance;
        }

        private void GetCorrespondenceHistoryGrid(string conrolId, string urlAction)
        {
            var historyGridModel = new CorrespondenceHistoryGrid(conrolId, urlAction);
            ViewData[ViewDataConstants.CorrespondenceHistoryGrid] = historyGridModel.Instance;
        }


        /// <summary>
        /// Fetch data for Correspondence history code data and display in grid.
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public JsonResult CorrespondenceHistoryGridData(string invoiceId)
        {
            var correspondenceHistoryGrid = new CorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new
            {
                invoiceId
            }));
            var corrId = ConvertUtil.ConvertGuidToString(invoiceId.ToGuid());
            // SCP#481926 :PAXCREATECORRESPONDENCEFOR is slow (April 2016)
            var log = ReferenceManager.GetDebugLog(DateTime.Now, "CorrespondenceHistoryGridData", this.ToString(), "Passenger",
                                        "Step 7:CorrespondenceHistoryGridAction Start", SessionUtil.UserId, logRefId.ToString());
            ReferenceManager.LogDebugData(log);
            var historyList = new PaxCorrespondenceManager().GetCorrespondenceHistoryList(corrId, SessionUtil.MemberId);

            // SCP#481926 :PAXCREATECORRESPONDENCEFOR is slow (April 2016)
            log = ReferenceManager.GetDebugLog(DateTime.Now, "CorrespondenceHistoryGridData", this.ToString(), "Passenger",
                                        "Step 8:CorrespondenceHistoryGridAction End", SessionUtil.UserId, logRefId.ToString());

            ReferenceManager.LogDebugData(log);

            IOrderedQueryable<Correspondence> correspondenceHistoryTotal = null;

            if (historyList != null && historyList.Count > 0)
                correspondenceHistoryTotal = historyList.AsQueryable().OrderBy(corr => corr.CorrespondenceStage);

            return correspondenceHistoryGrid.DataBind(correspondenceHistoryTotal);
        }

        [HttpPost]
        public JsonResult CheckIfBMExistsOnReply(long correspondenceRefNumber)
        {
            int toMemberId = SessionUtil.MemberId;

            var billingMemos = NonSamplingInvoiceManager.GetBillingMemosForCorrespondence(correspondenceRefNumber, toMemberId);
            if (billingMemos.Transactions != null && billingMemos.Transactions.Count > 0)
            {
                return Json(new UIMessageDetail
                                {
                                    //SCP199693 - create BM and close correspondence at same time - related to Spira [IN:008756] [TC:082202]
                                    //Message = string.Format(Messages.CannotReplyToCorrespondence, billingMemos.Transactions[0].BillingMemoNumber, billingMemos.Transactions[0].InvoiceNumber),
                                    Message =
                                        string.Format(Messages.BPAXNS_10940,
                                            billingMemos.Transactions[0].InvoiceNumber,
                                            billingMemos.Transactions[0].InvoicePeriod,
                                            billingMemos.Transactions[0].BatchNumber,
                                            billingMemos.Transactions[0].SequenceNumber,
                                            billingMemos.Transactions[0].BillingMemoNumber),
                                    IsFailed = true
                                });
            }
            
            return Json(new UIMessageDetail
            {
                IsFailed = false
            });
        }


        [HttpPost]
        public JsonResult IsCorrespondenceOutSideTimeLimit(string invoiceId)
        {
            bool isTimeLimitRecordFound = true;
            if (!_paxCorrespondenceManager.IsCorrespondenceOutsideTimeLimit(invoiceId, ref isTimeLimitRecordFound))
            {
                return
                  Json(new UIMessageDetail { IsFailed = false });
            }

            if (isTimeLimitRecordFound)
            {
                return Json(new UIMessageDetail { IsFailed = true, Message = Messages.CorrespondenceOutSideTimeLimit });
            }
            else
            {
                /* SCP#387982 - SRM: Initiate Correspondence timelimit incorrect for SMI I 
                Desc: system failed to retrieve Time Limit data from the master */
                return Json(new UIMessageDetail {IsFailed = true, Message = Messages.BGEN_10906});
            }
        }

        /// <summary>
        /// CMP 527:CloseCorrespondence
        /// </summary>
        /// <param name="correspondenceId">correspondence Id</param>
        /// <param name="correspondenceStage">correspondence stage</param>
        /// <param name="correspondenceStatus">correspondence status</param>
        /// <param name="correspondenceSubStatus">correspondence sub status</param>
        /// <param name="scenarioId">close scenario</param>
        /// <param name="userAcceptanceComment">acceptance comments</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CloseCorrespondence(string correspondenceId, string correspondenceStage, string correspondenceStatus, string correspondenceSubStatus, int scenarioId, string userAcceptanceComment)
        {

            var returnMsg = string.Empty;
            var saved = false;
            var correspondence = _paxCorrespondenceManager.GetFirstCorrespondenceDetails(correspondenceId, true);

            if (correspondence != null)
            {

                //SCP199693 - create BM and close correspondence at same time - related to Spira [IN:008756] [TC:082202]
                var nonSamplingInvoiceManager = Ioc.Resolve<INonSamplingInvoiceManager>(typeof(INonSamplingInvoiceManager));

                var billingMemos =
                  nonSamplingInvoiceManager.GetBillingMemosForCorrespondence(correspondence.CorrespondenceNumber.Value,
                                                                             correspondence.FromMemberId);

                if (billingMemos.Transactions != null && billingMemos.Transactions.Count > 0)
                {
                    ShowErrorMessage(string.Format(Messages.BPAXNS_10940,
                                                   billingMemos.Transactions[0].InvoiceNumber,
                                                   billingMemos.Transactions[0].InvoicePeriod,
                                                   billingMemos.Transactions[0].BatchNumber,
                                                   billingMemos.Transactions[0].SequenceNumber,
                                                   billingMemos.Transactions[0].BillingMemoNumber), true);

                    return RedirectToAction("Correspondence", "Correspondence", new
                                                                                  {
                                                                                      invoiceId = correspondenceId
                                                                                  });
                }

                var correspondenceNumber = correspondence.CorrespondenceNumber.HasValue
                                             ? Convert.ToString(correspondence.CorrespondenceNumber.Value)
                                             : string.Empty;
                var rejectionMemoDetails = _paxCorrespondenceManager.GetCorrespondenceRejectionList(correspondenceId);
                var rejectionInvoice =
                  _paxCorrespondenceManager.GetInvoiceDetail(rejectionMemoDetails.First().InvoiceId.ToString());

                //var closedCorrespondence = _paxCorrespondenceManager.GetOnlyCorrespondenceUsingLoadStrategy(correspondenceId);
                var closedCorrespondence = _paxCorrespondenceManager.GetLastRespondedCorrespondene(correspondence.CorrespondenceNumber.Value);

                saved = _paxCorrespondenceManager.CloseCorrespondence(correspondenceNumber, correspondenceStage,
                                                                      correspondenceStatus, correspondenceSubStatus,
                                                                      scenarioId, paxBillingCategory, userAcceptanceComment,
                                                                      SessionUtil.UserId, DateTime.UtcNow, ref returnMsg);
                if (saved)
                {
                    //SCP329272 - SIS:Correspondence - Closure of Passenger Correspondence No. 790000302
                    //Get Last Corr Detail for ToEmailId and ToAddtionalEmailId
                    var l_correspondence = _paxCorrespondenceManager.GetLastCorrespondenceDetails(correspondence.CorrespondenceNumber);

                    if (l_correspondence == null && correspondence.CorrespondenceStage == 1)
                    {
                        l_correspondence = correspondence;
                    }

                  /* CMP#657: Retention of Additional Email Addresses in Correspondences
                    FRS Section: 2.5 Email Alerts on Acceptance of Closure of Correspondences*/
                  //SCP426039: FW: SIS:Correspondence - Closure of Passenger Correspondence No. 1760005673 - SIS Production
                  _paxCorrespondenceManager.SendCorrespondenceAlertOnClose(rejectionInvoice, correspondenceNumber,
                                                                             correspondence.FromMemberId,
                                                                             correspondence.ToMemberId, scenarioId,
                                                                             SessionUtil.UserId, l_correspondence.ToEmailId,
                                                                             closedCorrespondence == null ? string.Empty : closedCorrespondence.AdditionalEmailInitiator,
                                                                             closedCorrespondence == null ? string.Empty : closedCorrespondence.AdditionalEmailNonInitiator,
                                                                             string.Join(",",
                                                                                         rejectionMemoDetails.Select(
                                                                                           rej => rej.RejectionMemoNumber).
                                                                                           ToList()));
                    ShowSuccessMessage(returnMsg);
                }
                else
                {
                    ShowErrorMessage(returnMsg, true);
                }

            }
            if (scenarioId <= 3 || scenarioId == 5 || scenarioId == 6)
            {
                return RedirectToAction("Index", "BillingHistory", new { area = "Pax" });
            }
            else
            {
                return RedirectToAction("Correspondence", "Correspondence", new
                                                                              {
                                                                                  invoiceId = correspondenceId
                                                                              });
            }
          
        }
    }
}

