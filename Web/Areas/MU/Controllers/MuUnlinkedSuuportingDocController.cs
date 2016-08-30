using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Business.Common;
using Iata.IS.Business.Common.Impl;
using Iata.IS.Business.Pax;
using Iata.IS.Business.SupportingDocuments;
using Iata.IS.Core;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.SupportingDocuments;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.UIModel.Grid.UnlinkedSupportingDocument;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;
using BillingType = Iata.IS.Web.Util.BillingType;

namespace Iata.IS.Web.Areas.MU.Controllers
{
    public class MuUnlinkedSuuportingDocController : ISController
    {
        private const string SearchResultGridAction = "SearchResultGridData";
        private readonly ISupportingDocumentManager _iSupportingDocumentManager;
        public ICalendarManager _calendarManager;
        private readonly bool _isUatp;
        private readonly IReferenceManager _referenceManager;
        public MuUnlinkedSuuportingDocController(ISupportingDocumentManager iSupportingDocumentManager, ICalendarManager CalendarManager, IReferenceManager referenceManager, bool isUatp)
        {
            _iSupportingDocumentManager = iSupportingDocumentManager;
            _calendarManager = CalendarManager;
            _isUatp = isUatp;
            _referenceManager = referenceManager;
        }

        [HttpGet]
        [ISAuthorize(Business.Security.Permissions.UATP.Receivables.CorrectSupportingDocumentsLinkingErrors.Correct)]
        public ActionResult Index()
        {
            var objUnlinkedSupportingDocumentEx = new UnlinkedSupportingDocumentEx();
            try
            {
               
                    var unlinkedSupportingDocumentSearchGrid =
                        new UnlinkedSupportingDocumentGrid(ControlIdConstants.SupportingDocumentSearchGrid,
                                                           Url.Action(SearchResultGridAction,
                                                                      new
                                                                          {
                                                                              billingYear = 0,
                                                                              billingMonth = 0,
                                                                              billingPeriod = -1,
                                                                              billedmember = 0,
                                                                              billingMember = SessionUtil.MemberId,
                                                                              invoiceNumber = string.Empty,
                                                                              fileName = string.Empty
                                                                          }), _isUatp ? BillingCategoryType.Uatp : BillingCategoryType.Misc);

                    ViewData[ViewDataConstants.supportingDocumentSearchGrid] =
                        unlinkedSupportingDocumentSearchGrid.Instance;
                
                ViewData[ViewDataConstants.MismatchTransactionModel] = new SupportingDocumentRecord();
                ViewData[ViewDataConstants.BillingType] = BillingType.Receivables;

                // Set first day of previous period as submission date, if exists.
                try
                {
                    //var previousPeriod = _calendarManager.GetLastClosedBillingPeriod();
                    //objUnlinkedSupportingDocumentEx.SubmissionDate = previousPeriod.StartDate;
                }
                catch (ISCalendarDataNotFoundException) { }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
            }
            return View(objUnlinkedSupportingDocumentEx);
        }

        /// <summary>
        /// Indexes the specified search criteria.
        /// </summary>
        /// <param name="searchCriteria">The search criteria.</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Index(UnlinkedSupportingDocumentEx searchCriteria)
        {
            try
            {
                if (searchCriteria != null)
                {
                    var unlinkedSupportingDocumentSearchGrid = new UnlinkedSupportingDocumentGrid(ControlIdConstants.SupportingDocumentSearchGrid,
                                                                                                  Url.Action(SearchResultGridAction,
                                                                                                             new
                                                                                                             {
                                                                                                                 billingYear = searchCriteria.BillingYear,
                                                                                                                 billingMonth = searchCriteria.BillingMonth,
                                                                                                                 billingPeriod = searchCriteria.PeriodNumber,
                                                                                                                 billedmember = searchCriteria.BilledMemberId,
                                                                                                                 billingMember = SessionUtil.MemberId,
                                                                                                                 invoiceNumber = searchCriteria.InvoiceNumber,
                                                                                                                 fileName = searchCriteria.OriginalFileName,
                                                                                                                 submissionDate = searchCriteria.SubmissionDate
                                                                                                             }), _isUatp ? BillingCategoryType.Uatp : BillingCategoryType.Misc);

                    ViewData[ViewDataConstants.supportingDocumentSearchGrid] = unlinkedSupportingDocumentSearchGrid.Instance;
                    ViewData[ViewDataConstants.MismatchTransactionModel] = new SupportingDocumentRecord();
                    ViewData[ViewDataConstants.BillingType] = BillingType.Receivables;
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
            }

            return View(searchCriteria);
        }

        public JsonResult SearchResultGridData(int billingYear, int billingMonth, int billingPeriod, int billedmember, int billingMember, string invoiceNumber, string fileName, DateTime? submissionDate)
        {
            //Create grid instance and retrieve data from database
            var unlinkedSupportingDocumentSearchGrid = new UnlinkedSupportingDocumentGrid(ControlIdConstants.SupportingDocumentSearchGrid,
                                                                                          Url.Action(SearchResultGridAction,
                                                                                                     new
                                                                                                     {
                                                                                                         billingYear,
                                                                                                         billingMonth,
                                                                                                         billingPeriod,
                                                                                                         billedmember,
                                                                                                         billingMember,
                                                                                                         invoiceNumber,
                                                                                                         fileName,
                                                                                                         submissionDate
                                                                                                     }), _isUatp ? BillingCategoryType.Uatp : BillingCategoryType.Misc);

            //Get data from the database and bind to the grid
            var lisOfUnlinkedSupportingDocument =
              _iSupportingDocumentManager.GetUnlinkedSupportingDocuments(billingYear,
                                                                         billingMonth,
                                                                         billingPeriod,
                                                                         billedmember,
                                                                         billingMember,
                                                                         invoiceNumber,
                                                                         fileName,
                                                                         submissionDate,
                                                                         null,
                                                                         null,
                                                                         null,
                                                                          _isUatp ? (int)BillingCategoryType.Uatp : (int)BillingCategoryType.Misc).ToArray().AsQueryable();
            //set databind property of DataGrid 
            return unlinkedSupportingDocumentSearchGrid.DataBind(lisOfUnlinkedSupportingDocument);
        }

        public JsonResult SearchResultMismatchGridData(int billedMemberId,
                                                       int billingMemberId,
                                                       int clearanceMonth,
                                                       int clearancePeriod,
                                                       int billingCategory,
                                                       string invoiceNumber,
                                                       int batchNumber,
                                                       int sequenceNumber,
                                                       int breakdownSerialNumber, int chargeCategoryId, int Mismatch)
        {
            var recordSearchCriteria = new RecordSearchCriteria
            {
                BilledMemberId = billedMemberId,
                BillingMemberId = billingMemberId,
                ClearanceMonth = clearanceMonth,
                ClearancePeriod = clearancePeriod,
                BillingCategory = billingCategory,
                InvoiceNumber = invoiceNumber,
                BatchNumber = batchNumber,
                SequenceNumber = sequenceNumber,
                BreakdownSerialNumber = breakdownSerialNumber,
                ChargeCategoryId = chargeCategoryId > 0 ? (int?)chargeCategoryId : null,
                Mismatch = Mismatch
            };

            if( _isUatp)
            {
                IList<ChargeCategory> chargelist = _referenceManager.GetChargeCategoryList(BillingCategoryType.Uatp);
                foreach (var chargeCategory in chargelist)
                {
                    recordSearchCriteria.ChargeCategoryId = chargeCategory.Id;
                    break;
                }
            }
            var lisOfMismatchedTransactionDocuments = _iSupportingDocumentManager.GetRecordListWithAttachments(recordSearchCriteria).ToArray().AsQueryable();

            //TODO
            if (lisOfMismatchedTransactionDocuments.Count() > 0)
            {
                var lisOfMismatchedTransactionDocument = lisOfMismatchedTransactionDocuments.ElementAt(0);
                return Json(lisOfMismatchedTransactionDocument);
            }
            return Json(null);
        }

        [HttpPost]
        public JsonResult GetSelectedUnlinkedSupportingDocumentDetails(string unlinkedDocumentId)
        {
            var lisOfUnlinkedSupportingDocumentDetails = _iSupportingDocumentManager.GetSelectedUnlinkedSupportingDocumentDetails(unlinkedDocumentId.ToGuid());
            return Json(lisOfUnlinkedSupportingDocumentDetails);
        }

        [HttpPost]
        public JsonResult LinkDocuments(string invoiceNumber,
                                        int billingMemberId,
                                        int billedMemberId,
                                        int billingYear,
                                        int billingMonth,
                                        int periodNumber,
                                        int batchNumber,
                                        int sequenceNumber,
                                        int breakdownSerialNumber,
                                        string filePath,
                                        string originalFileName,
                                        string id)
        {
            var unlinkedSupportingDocument = new UnlinkedSupportingDocument
            {
                InvoiceNumber = invoiceNumber,
                BillingMemberId = billingMemberId,
                BilledMemberId = billedMemberId,
                BillingYear = billingYear,
                BillingMonth = billingMonth,
                PeriodNumber = periodNumber,
                BatchNumber = batchNumber,
                SequenceNumber = sequenceNumber,
                FilePath = filePath,
                OriginalFileName = originalFileName,
                BillingCategoryId = _isUatp ? (int)BillingCategoryType.Uatp : (int)BillingCategoryType.Misc,
                CouponBreakdownSerialNumber = breakdownSerialNumber,
                Id = id.ToGuid()
            };

            var returnFlag = _iSupportingDocumentManager.LinkDocument(unlinkedSupportingDocument, null);
            return Json(returnFlag);
        }

        /// <summary>
        /// Deletes the unlink documents.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteUnLinkDocuments(string id)
        {
            var unlinkedSupportingDocument = new UnlinkedSupportingDocument
            {
                Id = id.ToGuid()
            };

            var returnFlag = _iSupportingDocumentManager.DeleteUnlinkedDocuments(unlinkedSupportingDocument);
            var details = returnFlag
                                        ? new UIMessageDetail
                                        {
                                            IsFailed = false,
                                            Message = "Unlinked document deleted sucessfully."
                                        }
                                        : new UIMessageDetail
                                        {
                                            IsFailed = true,
                                            Message = "Error while deleting Unlinked document."
                                        };
            return Json(details);
        }

    }
}
