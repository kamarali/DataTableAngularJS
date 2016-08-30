using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Business.MiscUatp;
using Iata.IS.Core.DI;
using Iata.IS.Model.Enums;
using Iata.IS.Web.Util;
using Iata.IS.Model.MiscUatp.BillingHistory;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Web.Util.Filters;
using System.Web.Script.Serialization;
using Iata.IS.Model.Common;
using Iata.IS.Web.UIModel.Grid.MU;
using Enums = Iata.IS.Model.Enums;
using Iata.IS.Web.UIModel.BillingHistory.Misc;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Core;
using Iata.IS.Model.MemberProfile;
using AuditTrail = Iata.IS.Web.UIModel.BillingHistory.Misc.AuditTrail;

namespace Iata.IS.Web.Areas.MU.Controllers
{
    public class BillingHistoryControllerBase : ISController
    {
        public const string BillingHistorySearchGridAction = "BillingHistorySearchGridData";
        public readonly IMiscInvoiceManager _miscInvoiceManager;
        public readonly IMiscUatpInvoiceManager _miscUatpInvoiceManager;
        public readonly IUatpInvoiceManager _uatpInvoiceManager;
        public readonly IQueryAndDownloadDetailsManager _queryAndDownloadDetailsManager;
        public readonly IReferenceManager _referenceManager;
        
        public BillingHistoryControllerBase(IMiscInvoiceManager miscInvoiceManager, IUatpInvoiceManager uatpInvoiceManager, IMiscUatpInvoiceManager miscUatpInvoiceManager, IQueryAndDownloadDetailsManager queryAndDownloadDetailsManager, IReferenceManager referenceManager)
        {
            _miscInvoiceManager = miscInvoiceManager;
            _uatpInvoiceManager = uatpInvoiceManager;
            _miscUatpInvoiceManager = miscUatpInvoiceManager;
            _queryAndDownloadDetailsManager = queryAndDownloadDetailsManager;
            _referenceManager = referenceManager;

        }

        [ISAuthorize(Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.ViewAuditTrail)]
        public virtual ActionResult Index(InvoiceSearchCriteria searchCriteria, CorrespondenceSearchCriteria correspondenceSearchCriteria, string searchType, int? billingCategoryId)
        {
            string criteria = null;
            

            switch (searchType)
            {
                case "Invoice":
                    criteria = searchCriteria != null ? new JavaScriptSerializer().Serialize(searchCriteria) : string.Empty;
                    SessionUtil.MiscCorrSearchCriteria = null;
                    SessionUtil.MiscInvoiceSearchCriteria = criteria;
                    SessionUtil.BillingCategoryId = billingCategoryId;
                    ViewData[ViewDataConstants.CorrespondenceSearch] = "Invoice";
                    break;
                case "Correspondence":
                    criteria = correspondenceSearchCriteria != null ? new JavaScriptSerializer().Serialize(correspondenceSearchCriteria) : string.Empty;
                    SessionUtil.MiscCorrSearchCriteria = criteria;
                    SessionUtil.MiscInvoiceSearchCriteria = null;
                    ViewData[ViewDataConstants.CorrespondenceSearch] = "Correspondence";
                    break;
                default:

                    // Criteria values should not be picked from the Session when user comes from the menu.
                    if (Request.QueryString["back"] == null)
                    {
                        // clear the session.
                        SessionUtil.MiscInvoiceSearchCriteria = null;
                        SessionUtil.MiscCorrSearchCriteria = null;
                        SessionUtil.BillingCategoryId = null;
                    } // Values should come from Session only when user has clicked the Back to Billing history button.
                    else
                    {
                        if (SessionUtil.MiscCorrSearchCriteria != null)
                        {
                            criteria = SessionUtil.MiscCorrSearchCriteria;
                            correspondenceSearchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(CorrespondenceSearchCriteria)) as CorrespondenceSearchCriteria;
                            if (correspondenceSearchCriteria != null)
                            {
                                correspondenceSearchCriteria.FromDate = correspondenceSearchCriteria.FromDate.Value.ToLocalTime();
                                correspondenceSearchCriteria.ToDate = correspondenceSearchCriteria.ToDate.Value.ToLocalTime();
                            }

                            ViewData[ViewDataConstants.CorrespondenceSearch] = "Correspondence";
                        }
                        else if (SessionUtil.MiscInvoiceSearchCriteria != null)
                        {
                            criteria = SessionUtil.MiscInvoiceSearchCriteria;
                            billingCategoryId = SessionUtil.BillingCategoryId;
                            searchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(InvoiceSearchCriteria)) as InvoiceSearchCriteria;
                            ViewData[ViewDataConstants.CorrespondenceSearch] = "Invoice";
                        }
                    }

                    break;
            }

            if (SessionUtil.MiscCorrSearchCriteria == null) correspondenceSearchCriteria.FromDate = correspondenceSearchCriteria.ToDate = DateTime.UtcNow;


            //CMP #655: IS-WEB Display per Location ID
            var memberLocation = Ioc.Resolve<IManageContactsManager>(typeof(IManageContactsManager));
            var associatedLocations = memberLocation.GetMemberAssociationLocForSearch(SessionUtil.UserId,
                                                                                       SessionUtil.MemberId);
            if (searchCriteria != null && searchCriteria.MemberLocation == null)
            {
                
                if (searchCriteria.MemberLocation == null)
                {
                    foreach (var item in associatedLocations)
                    {
                        searchCriteria.MemberLocation += "," + item.LocationCode;
                    }
                    if (associatedLocations.Count == 0) searchCriteria.MemberLocation = "0";
                }
            }
            else
            {
                // server Side Validation for Associatin Location
                if (searchCriteria != null)
                {
                    var selectedBillingMemberLocationList = searchCriteria.MemberLocation.Split(Convert.ToChar(","));
                    searchCriteria.MemberLocation = "";
                    foreach (var location in from location in selectedBillingMemberLocationList
                                             where location != null
                                             let contains = associatedLocations.SingleOrDefault(l => l.LocationCode == location)
                                             where contains != null
                                             select location)
                    {
                        searchCriteria.MemberLocation += "," + location;
                    }
                    if (searchCriteria.MemberLocation.Length == 0) searchCriteria.MemberLocation = "0";
                }
                    
            }
            ViewData["AssociatedLocation"] = new MultiSelectList(associatedLocations.ToArray(), "locationId", "locationCode");
            //End Code CMP#655


            ViewData[ViewDataConstants.invoiceSearchCriteria] = searchCriteria;
            ViewData[ViewDataConstants.correspondenceSearchCriteria] = correspondenceSearchCriteria;

            var billingHistorySearchResultGrid = new BillingHistorySearchGrid(ControlIdConstants.BHSearchResultsGrid,
                                                                              Url.Action(BillingHistorySearchGridAction,
                                                                                         new
                                                                                         {
                                                                                             criteria,billingCategoryId
                                                                                         }), billingCategoryId);
            ViewData[ViewDataConstants.BHSearchResultsGrid] = billingHistorySearchResultGrid.Instance;

            return View();
        }

        public virtual JsonResult BillingHistorySearchGridData(string criteria,int billingCategoryId)
        {
            // Retrieve MemberId from Session variable and use it across the method
            var memberId = SessionUtil.MemberId;
            InvoiceSearchCriteria searchCriteria = null;
            CorrespondenceSearchCriteria correspondenceSearchCriteria = null;

            var billingHistorySearchResultGrid = new BillingHistorySearchGrid(ControlIdConstants.BHSearchResultsGrid,
                                                                              Url.Action(BillingHistorySearchGridAction,
                                                                                         new
                                                                                         {
                                                                                             area = "Misc"
                                                                                         }), billingCategoryId);

            if (SessionUtil.MiscInvoiceSearchCriteria != null && criteria != null)
            {
                searchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(InvoiceSearchCriteria)) as InvoiceSearchCriteria;

                if (searchCriteria != null)
                {
                    if (searchCriteria.BillingTypeId == (int)Enums.BillingType.Payables)
                    {
                        searchCriteria.BillingMemberId = searchCriteria.BilledMemberId;
                        searchCriteria.BilledMemberId = memberId;
                    }
                    else
                    {
                        searchCriteria.BillingMemberId = memberId;
                    }
                }
            }
            else if (SessionUtil.MiscCorrSearchCriteria != null && criteria != null)
            {
                correspondenceSearchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(CorrespondenceSearchCriteria)) as CorrespondenceSearchCriteria;

                if (correspondenceSearchCriteria != null)
                {
                    correspondenceSearchCriteria.CorrBillingMemberId = memberId;
                    if (correspondenceSearchCriteria.FromDate != null)
                    {
                        correspondenceSearchCriteria.FromDate = correspondenceSearchCriteria.FromDate.Value.ToLocalTime();
                    }
                    if (correspondenceSearchCriteria.ToDate != null)
                    {
                        correspondenceSearchCriteria.ToDate = correspondenceSearchCriteria.ToDate.Value.ToLocalTime();
                    }
                }
            }

            IQueryable<MiscBillingHistorySearchResult> invoiceSearchedData;
            if (searchCriteria != null)
            {
                invoiceSearchedData = _miscUatpInvoiceManager.GetBillingHistorySearchResult(searchCriteria,billingCategoryId);
            }
            else if (correspondenceSearchCriteria != null)
            {
                invoiceSearchedData = _miscUatpInvoiceManager.GetBillingHistoryCorrSearchResult(correspondenceSearchCriteria,billingCategoryId);
            }
            else
            {
                invoiceSearchedData = null;
            }

            invoiceSearchedData = invoiceSearchedData != null ? invoiceSearchedData.OrderBy(result => result.TransactionDate) : invoiceSearchedData;

            return billingHistorySearchResultGrid.DataBind(invoiceSearchedData);
        }

        [ISAuthorize(Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.ViewAuditTrail)]
        [HttpGet]
        public virtual ViewResult BHAuditTrail(string invoiceId)
        {
            /* SCP 250695: Correspondence Invoice raised is in Ready for Billing status and is visible to both the airline on Audit-trail. 
             * Description: Sending memberId to identify if its a payable view/receivable view
             */
            var invoiceList = _miscUatpInvoiceManager.GetBillingHistoryAuditTrail(invoiceId, SessionUtil.MemberId);
            var auditTrail = new AuditTrail();
            ViewData["invoiceId"] = invoiceId;

            foreach (var miscUatpInvoice in invoiceList)
            {
                switch (miscUatpInvoice.InvoiceTypeId)
                {
                    case (int)InvoiceType.Invoice:
                        auditTrail.OriginalInvoice = miscUatpInvoice;
                        break;
                    case (int)InvoiceType.CreditNote:
                        auditTrail.OriginalInvoice = miscUatpInvoice;
                        break;
                    case (int)InvoiceType.RejectionInvoice:
                        auditTrail.RejectionInvoiceList.Add(miscUatpInvoice);
                        break;
                    case (int)InvoiceType.CorrespondenceInvoice:
                        auditTrail.CorrespondenceInvoice = miscUatpInvoice;
                        break;
                }
            }

            return View(auditTrail);
        }

        public virtual ActionResult GenerateBillingHistoryAuditTrailPdf(string invoiceId, string areaName)
        {
            // Retrieve auditTrail details for selected transaction
            /* SCP 250695: Correspondence Invoice raised is in Ready for Billing status and is visible to both the airline on Audit-trail. 
             * Description: Sending memberId to identify if its a payable view/receivable view
             */

            var invoiceList = _miscUatpInvoiceManager.GetBillingHistoryAuditTrail(invoiceId, SessionUtil.MemberId);
            var auditTrail = new AuditTrailPdf();

            foreach (var miscUatpInvoice in invoiceList)
            {
                switch (miscUatpInvoice.InvoiceTypeId)
                {
                    case (int)InvoiceType.Invoice:
                        auditTrail.OriginalInvoice = miscUatpInvoice;
                        break;
                    case (int)InvoiceType.CreditNote:
                        auditTrail.OriginalInvoice = miscUatpInvoice;
                        break;
                    case (int)InvoiceType.RejectionInvoice:
                        auditTrail.RejectionInvoiceList.Add(miscUatpInvoice);
                        break;
                    case (int)InvoiceType.CorrespondenceInvoice:
                        auditTrail.CorrespondenceInvoice = miscUatpInvoice;
                        break;
                }
            }

            // Generate Audit trail html string through NVelocity 
            string htmlString = _miscUatpInvoiceManager.GenerateMiscBillingHistoryAuditTrailPdf(auditTrail, SessionUtil.MemberId, areaName);

            var filePath = Server.MapPath(@"\AuditTrailPdf");
            if (!System.IO.Directory.Exists(filePath))
            {
                System.IO.Directory.CreateDirectory(filePath);
            }

            string fileLocation = filePath + "\\" + areaName.Trim() + "AuditTrail.pdf";

            // Following call will generate Audit trail pdf file from html string
            GenerateAuditTrailPdfFromHtmlString(htmlString, fileLocation);

            const string contentType = "application/pdf";
            // If pdf file is not null return file
            return File(fileLocation, contentType, System.IO.Path.GetFileName(fileLocation));
        }

        [HttpPost]
        public virtual void GenerateAuditTrailPdfFromHtmlString(string auditTrailHtmlString, string fileLocation)
        {
            var guid = Guid.NewGuid().ToString();

            // wkhtmltopdf.exe file path which converts html string to pdf
            var htmlToPdfExePath = Request.PhysicalApplicationPath + @"bin\wkhtmltopdf.exe";
            // Following call converts html file to pdf 
            var file = _queryAndDownloadDetailsManager.ConvertHtmlToPdf(auditTrailHtmlString, string.Format(@"AuditTrail_{0}", guid), htmlToPdfExePath);
            // Write all file content
            System.IO.File.WriteAllBytes(fileLocation, file);
        }

        [ISAuthorize(Business.Security.Permissions.Misc.Receivables.Invoice.Download)]
        [HttpGet]
        public virtual FileStreamResult BillingHistoryAttachmentDownload(string invoiceId)
        {
            var fileDownloadHelper = new FileAttachmentHelper
            {
                Attachment = _miscUatpInvoiceManager.GetInvoiceAttachmentDetail(invoiceId)
            };

            return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
        }

        //[HttpPost]
        public virtual JsonResult IsCorrespondenceInvoiceOutSideTimeLimit(string invoiceId, int correspondenceStatusId, bool authorityToBill, DateTime correspondenceDate, string correspondenceRefNumber, string invoice)
        {
            if (!_miscUatpInvoiceManager.IsCorrespondenceOutSideTimeLimit(invoiceId, correspondenceStatusId, authorityToBill, correspondenceDate))
            {
                return
                  Json(new UIMessageDetail() { IsFailed = false, RedirectUrl = Url.Action("CreateBillingMemo", invoice , new { rejectedInvoiceId = invoiceId, correspondenceReferenceNumber = correspondenceRefNumber }) });
            }

            return
                //SCP#477333- SRM: Error Message- Misc-Correspondence
                Json(new UIMessageDetail() { IsFailed = true, Message = Messages.BMISC_10238 });
        }

        //CMP508:Audit Trail Download with Supporting Documents
        public JsonResult EnqueBillingHistoryAuditTrailDownload(string invoiceId, string areaName, BillingCategoryType billingCategory)
        {
            JsonResult result = new JsonResult();
            if (SessionUtil.UserId > 0)
            {
              DateTime utcTime = DateTime.UtcNow;
              string fileName = string.Format("{0}-Audit Trail-{1}-{2}-{3}",
                                              Enum.GetName(typeof(BillingCategoryType), billingCategory).ToUpper(),
                                              SessionUtil.UserId, utcTime.ToString("yyyyMMdd"), utcTime.ToString("HHMMss"));

              AuditTrailPackageRequest data = new AuditTrailPackageRequest()
              {
                FileName = fileName,
                TransactionId = invoiceId,
                TransactionType = areaName
              };

              ReportDownloadRequestMessage enqueMessage = new ReportDownloadRequestMessage();
              enqueMessage.RecordId = Guid.NewGuid();
              enqueMessage.BillingCategoryType = billingCategory;
              enqueMessage.UserId = SessionUtil.UserId;
              enqueMessage.RequestingMemberId = SessionUtil.MemberId;
              enqueMessage.InputData = ConvertUtil.SerializeXml(data, data.GetType());
              enqueMessage.DownloadUrl = GetUrl(billingCategory);
              enqueMessage.OfflineReportType = OfflineReportType.AuditTrailPackageDownload;
              // Message will display on screen depending on Success or Failure of Enqueing message to queue.
              var isEnqueSuccess = false;
              isEnqueSuccess = _referenceManager.EnqueTransactionTrailReport(enqueMessage);

              if (isEnqueSuccess)
              {
                //Display success message to user
                result.Data = new UIMessageDetail
                {
                  Message =
                      string.Format(
                          @"Generation of the audit trail package is in progress. You will be notified via 
                        email once it is ready for download. [File: {0}.zip]",
                          fileName),
                  IsFailed = false
                };
              }
              else
              {
                //Display failure message to user
                result.Data = new UIMessageDetail
                {
                  Message = "Failed to download the audit trail package, please try again!",
                  IsFailed = true
                };
              }
            }
            else
            {
              //Display failure message to user
              result.Data = new UIMessageDetail
              {
                Message = "Failed to download the audit trail package, please try again!",
                IsFailed = true
              };
            }
            return result;
        }

        public string GetUrl(BillingCategoryType billingCategory)
        {
            if (billingCategory == BillingCategoryType.Uatp)
                return UrlHelperEx.ToAbsoluteUrl(Url.Action("DownloadFile",
                                                             "UatpInvoice",
                                                             new
                                                             {
                                                                 area = "Uatp",
                                                                 billingType = "Receivables"
                                                             }));

            return UrlHelperEx.ToAbsoluteUrl(Url.Action("DownloadFile",
                                                        "MiscInvoice",
                                                        new
                                                            {
                                                                area = "Misc",
                                                                billingType = "Receivables"
                                                            }));
        }
    }
}
