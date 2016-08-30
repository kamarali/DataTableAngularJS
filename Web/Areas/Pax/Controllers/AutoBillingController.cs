using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Business.Pax;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.AutoBilling;
using Iata.IS.Data.Pax;
using Iata.IS.Data.Pax.Impl;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Web.Areas.Pax.Controllers.Base;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.UIModel.Grid.Pax;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;
using log4net;

namespace Iata.IS.Web.Areas.Pax.Controllers
{
    public class AutoBillingController : PaxInvoiceControllerBase
    {
        private const string SearchResultGridAction = "SearchResultGridData";
        private readonly INonSamplingInvoiceManager _nonSamplingInvoiceManager;
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public IRepository<PassengerConfiguration> PassengerReository { get; set; }
        InvoiceRepository InvoiceRepository = new InvoiceRepository();
        
        public AutoBillingController(INonSamplingInvoiceManager nonSamplingInvoiceManager)
            : base(nonSamplingInvoiceManager)
        {
            _nonSamplingInvoiceManager = nonSamplingInvoiceManager;
        }

        protected override int BillingCodeId
        {
            get { return Convert.ToInt32(BillingCode.NonSampling); }
        }


        [ISAuthorize(Business.Security.Permissions.Pax.Receivables.CorrectAutoBillingInvoices.Correct)]
        public ActionResult Index(AutoBillingSearchCriteria searchCriteria, string postBack, string backButtonClicked)
        {
            string criteria = null;

            if(!string.IsNullOrEmpty(postBack))
            {
              criteria = searchCriteria != null ? new JavaScriptSerializer().Serialize(searchCriteria) : string.Empty;
              SessionUtil.InvoiceSearchCriteria = criteria;
            }

          // If Back button is clicked on Prime billing Edit, get search criteria from Session    
          if(!String.IsNullOrEmpty(backButtonClicked))
          {
            if (SessionUtil.InvoiceSearchCriteria != null)
            {
              criteria = SessionUtil.InvoiceSearchCriteria;
              searchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(AutoBillingSearchCriteria)) as AutoBillingSearchCriteria;
            }
          }

          if (string.IsNullOrEmpty(postBack))
          {
            ViewData[ViewDataConstants.IsPostback] = false;
          }
          else
          {
            ViewData[ViewDataConstants.IsPostback] = true;
          }
          
          var primeSearchGrid = new AutoBillingPrimeBillingGrid(ControlIdConstants.SearchGrid, Url.Action(SearchResultGridAction, new { area = "Pax", criteria }));
          ViewData[ViewDataConstants.SearchGrid] = primeSearchGrid.Instance;


            return View(searchCriteria);
        }

        /// <summary>
        /// Fetch invoice searched result and display it in grid
        /// </summary>
        /// <returns></returns>
        public JsonResult SearchResultGridData(string criteria)
        {
            AutoBillingSearchCriteria searchCriteria = null;

            if (Request.UrlReferrer != null)
            {
                // SessionUtil.InvoiceSearchCriteria = Request.UrlReferrer.ToString();
                SessionUtil.PaxCorrSearchCriteria = null;
                SessionUtil.PaxInvoiceSearchCriteria = null;
            }

            if (!string.IsNullOrEmpty(criteria))
            {
                searchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(AutoBillingSearchCriteria)) as AutoBillingSearchCriteria;
            }

            // Create grid instance and retrieve data from database
            var primeSearchGrid = new AutoBillingPrimeBillingGrid(ControlIdConstants.SearchGrid, Url.Action(SearchResultGridAction, new { area = "Pax", searchCriteria }));
            object invoiceSearchedData = null;

            // add billing member id to search criteria.
            if (searchCriteria != null)
            {

                searchCriteria.BillingMemberId = SessionUtil.MemberId;

                invoiceSearchedData = _nonSamplingInvoiceManager.GetAutoBillingPrimeCouponList(searchCriteria).AsQueryable();
            }

            return primeSearchGrid.DataBind(invoiceSearchedData);
        }

        /// <summary>
        /// Retrieves prime billing coupon details for given coupon id.
        /// </summary>
        [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
        [RestrictUnauthorizedUpdate]
        [HttpGet]
        public ActionResult PrimeBillingEdit(string id)
        {
            var couponRecord = GetCouponRecord(id);

            // Set Airline flight designator to Billing Member name
            couponRecord.AirlineFlightDesignator = couponRecord.Invoice.BillingMember.MemberCodeAlpha;
            ViewData[ViewDataConstants.PageMode] = PageMode.Edit;
            return View(couponRecord);
        }

        private PrimeCoupon GetCouponRecord(string transactionId)
        {
            var couponRecord = _nonSamplingInvoiceManager.GetCouponRecordDetails((transactionId));
            var invoice = GetInvoiceHeader(couponRecord.InvoiceId.ToString());
            couponRecord.Invoice = invoice;
            couponRecord.LastUpdatedBy = SessionUtil.UserId;
            return couponRecord;
        }

        /// <summary>
        /// Update prime billing coupon details for given coupon id and redirect to Get version of same action.
        /// </summary>
        [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
        [HttpPost]
        public ActionResult PrimeBillingEdit(string invoiceId, string transactionId, PrimeCoupon couponRecord)
        {
          try
          {
            // Check if Invoice belongs to current Auto Billing open period
            var invoice = InvoiceRepository.Single(id: new Guid(couponRecord.InvoiceId.ToString()));

            var offSetHrs = 0;
            if (PassengerReository != null)
            {
              offSetHrs = PassengerReository.Single(p => p.MemberId == invoice.BillingMemberId) != null
                            ? PassengerReository.Single(p => p.MemberId == invoice.BillingMemberId).CutOffTime
                            : 0;
            }

            var invoiceBillingPeriod = new BillingPeriod();
            invoiceBillingPeriod.Period = invoice.BillingPeriod;
            invoiceBillingPeriod.Month = invoice.BillingMonth;
            invoiceBillingPeriod.Year = invoice.BillingYear;

            var calendarManager = Ioc.Resolve<ICalendarManager>(typeof (ICalendarManager));
            var autBillingOpenPeriod = calendarManager.GetAutoBillingPeriod(DateTime.Now, offSetHrs, invoiceBillingPeriod);

            if (autBillingOpenPeriod.Period == invoiceBillingPeriod.Period && autBillingOpenPeriod.Month == invoiceBillingPeriod.Month && autBillingOpenPeriod.Year == invoiceBillingPeriod.Year)
            {

              ArchiveOriginalCoupon(transactionId, (int) AutoBillingStatusType.Edit);
              couponRecord.Id = transactionId.ToGuid();
              couponRecord.LastUpdatedBy = SessionUtil.UserId;
              couponRecord.AutoBillingCouponStatus = (int)AutoBillingStatusType.Edit;
              couponRecord.IncludeInDailyRevenueRecogn = false;
              foreach (var tax in couponRecord.TaxBreakdown)
              {
                tax.ParentId = couponRecord.Id;
              }

              foreach (var vat in couponRecord.VatBreakdown)
              {
                vat.ParentId = couponRecord.Id;
              }
              
              string duplicateErrorMessage;
              _nonSamplingInvoiceManager.UpdateCouponRecord(couponRecord, out duplicateErrorMessage,
                                                            (int) SubmissionMethod.AutoBilling);

              //ShowSuccessMessage(Messages.PrimeBillingUpdateSuccessful + duplicateErrorMessage);
              ShowSuccessMessage(Messages.PrimeBillingUpdateSuccessful);
              if (!String.IsNullOrEmpty(duplicateErrorMessage))
                ShowErrorMessage(duplicateErrorMessage, true);

              TempData[TempDataConstants.PrimeCouponRecord] = string.Format(@"{0}-{1}-{2}", couponRecord.SourceCodeId,
                                                                            couponRecord.BatchSequenceNumber,
                                                                            couponRecord.RecordSequenceWithinBatch + 1);
            }
            else
            {
              ShowErrorMessage(Messages.BPAXAUTOBILL_00011);
              couponRecord.Invoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
              return View("PrimeBillingEdit", couponRecord);
            }
          }
          catch (ISBusinessException exception)
          {
            // Set ViewData, "IsPostback" to true, if exception occurs
            ViewData[ViewDataConstants.IsPostback] = true;
            ShowErrorMessage(exception.ErrorCode);
            var couponAttachmentIds = couponRecord.Attachments.Select(attachment => attachment.Id).ToList();
            couponRecord.Attachments = _nonSamplingInvoiceManager.GetCouponRecordAttachments(couponAttachmentIds);
            couponRecord.Invoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
            return View("PrimeBillingEdit", couponRecord);
          }
            finally
            {
                SetViewDataPageMode(PageMode.Edit);
            }

            couponRecord.Invoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

            return RedirectToAction("PrimeBillingEdit", new { transactionId});
        }

        /// <summary>
        /// Download prime billing attachment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.Download)]
        [HttpGet]
        public FileStreamResult PrimeBillingAttachmentDownload(string id)
        {
            var fileDownloadHelper = new FileAttachmentHelper { Attachment = _nonSamplingInvoiceManager.GetCouponLevelAttachmentDetails(id) };

            return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
        }

        /// <summary>
        /// Upload prime billing Attachment 
        /// </summary>
        /// <param name="invoiceId">Invoice Id</param>
        /// <param name="transactionId">Transaction Id</param>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult PrimeBillingAttachmentUpload(string invoiceId, string transactionId)
        {
            var files = string.Empty;
            var attachments = new List<PrimeCouponAttachment>();
            // SCP335749: SRM Admin Alert - Offline collection generation failure notification - SIS Production 05 FEB 2015 [Pax]
            var isUploadSuccess = false;
            string message;
            HttpPostedFileBase fileToSave;
            FileAttachmentHelper fileUploadHelper = null;
            try
            {
                PaxInvoice invoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

                foreach (string file in Request.Files)
                {
                  isUploadSuccess = false;
                    fileToSave = Request.Files[file];
                    if (Equals(fileToSave, null) || String.IsNullOrEmpty(fileToSave.FileName) || !(fileToSave.ContentLength > 0))
                    {
                        continue;
                    }

                    fileUploadHelper = new FileAttachmentHelper { FileToSave = fileToSave, FileRelativePath = String.Format("{0}_{1}_{2}", invoice.BilledMemberId, invoice.BillingYear, invoice.BillingMonth) };

                    // On Prime Billing Edit
                    if (!Equals(transactionId.ToGuid(), Guid.Empty) && _nonSamplingInvoiceManager.IsDuplicateCouponAttachmentFileName(fileUploadHelper.FileOriginalName, transactionId.ToGuid()))
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
                        var attachment = new PrimeCouponAttachment
                        {
                            Id = fileUploadHelper.FileServerName,
                            OriginalFileName = fileUploadHelper.FileOriginalName,
                            FileSize = fileUploadHelper.FileToSave.ContentLength,
                            LastUpdatedBy = SessionUtil.UserId,
                            ServerId = fileUploadHelper.FileServerInfo.ServerId,
                            FileStatus = FileStatusType.Received,
                            FilePath = fileUploadHelper.FileRelativePath
                        };


                        attachment = _nonSamplingInvoiceManager.AddCouponLevelAttachment(attachment);
                        isUploadSuccess = true;
                        attachments.Add(attachment);
                    }
                }
                message = string.Format(Messages.FileUploadSuccessful, files.TrimEnd(','));
            }
            catch (ISBusinessException ex)
            {
                message = string.Format(Messages.FileUploadBusinessException, ex.ErrorCode);
                Logger.Error("Exception", ex);
                if (fileUploadHelper != null && isUploadSuccess == false)
                {
                    fileUploadHelper.DeleteFile();
                }
            }
            catch (Exception ex)
            {
                message = Messages.FileUploadUnexpectedError;
                Logger.Error("Exception", ex);
                if (fileUploadHelper != null && isUploadSuccess == false)
                {
                    fileUploadHelper.DeleteFile();
                }
            }

            return new FileUploadJsonResult { Data = new { IsFailed = !isUploadSuccess, Message = message, Attachment = attachments, Length = attachments.Count } };
        }

        public void ArchiveOriginalCoupon(string transactionId, int statusTypeId)
        {
            var oldCouponRecord = _nonSamplingInvoiceManager.GetCouponRecordDetails((transactionId));

            var autoBillingPrimeCoupon = new AutoBillingPrimeCoupon();

            // Convert PrimeCoupon object to AutoBilling Prime Coupon
            PropertyInfo[] autoBillingPrimeCouponProperties = autoBillingPrimeCoupon.GetType().GetProperties();
            foreach (PropertyInfo autoBillingPrimeCouponProperty in autoBillingPrimeCouponProperties)
            {
                    
                if (autoBillingPrimeCouponProperty.Name.Equals("AutoBillingTaxBreakdown"))
                {
                    foreach (PrimeCouponTax couponTax in oldCouponRecord.TaxBreakdown)
                    {
                        AutoBillingPrimeCouponTax autoBillingPrimeCouponTax = SetTaxBreakDown(couponTax, statusTypeId);
                        autoBillingPrimeCoupon.AutoBillingTaxBreakdown.Add(autoBillingPrimeCouponTax);
                    }
                }
                else if (autoBillingPrimeCouponProperty.Name.Equals("AutoBillingVatBreakdown"))
                {
                    foreach (PrimeCouponVat couponVat in oldCouponRecord.VatBreakdown)
                    {
                        AutoBillingPrimeCouponVat autoBillingPrimeCouponVat = SetVatBreakDown(couponVat, statusTypeId);
                        autoBillingPrimeCoupon.AutoBillingVatBreakdown.Add(autoBillingPrimeCouponVat);
                    }
                }
                else if (autoBillingPrimeCouponProperty.Name.Equals("AutoBillingAttachments"))
                {
                    foreach (PrimeCouponAttachment couponAttachment in oldCouponRecord.Attachments)
                    {
                        AutoBillingPrimeCouponAttachment autoBillingPrimeCouponAttachment = SetAttachments(couponAttachment, statusTypeId);
                        autoBillingPrimeCoupon.AutoBillingAttachments.Add(autoBillingPrimeCouponAttachment);
                    }
                }
                else
                {
                    PropertyInfo couponRecordProperty = oldCouponRecord.GetType().GetProperty(autoBillingPrimeCouponProperty.Name);
                    if (couponRecordProperty != null)
                    {
                        if (couponRecordProperty.Name.Equals("Id"))
                        {
                            autoBillingPrimeCouponProperty.SetValue(autoBillingPrimeCoupon, Guid.NewGuid(), null);
                            continue;
                        }
                        MethodInfo setInfo = couponRecordProperty.GetSetMethod();
                        if (setInfo != null)
                            autoBillingPrimeCouponProperty.SetValue(autoBillingPrimeCoupon, couponRecordProperty.GetValue(oldCouponRecord, null), null);
                    }
                }
            }

            autoBillingPrimeCoupon.Status = statusTypeId;
          autoBillingPrimeCoupon.IncludeInDailyRevenueRecogn = false;

            IAutoBillingCouponRecordRepository autoBillingCouponRecordRepository = new AutoBillingCouponRecordRepository();
            autoBillingCouponRecordRepository.Add(autoBillingPrimeCoupon);

            UnitOfWork.CommitDefault();
        }

        /// <summary>
        /// Set AutoBilling Tax from Prime Coupon tax
        /// </summary>
        /// <param name="couponTaxRecord"></param>
        /// <returns></returns>
        public AutoBillingPrimeCouponTax SetTaxBreakDown(PrimeCouponTax couponTaxRecord, int statusTypeId)
        {
            var autoBillingTax = new AutoBillingPrimeCouponTax();

             // Convert PrimeCouponTax object to AutoBilling Prime Coupon
                PropertyInfo[] autoBillingTaxProperties = autoBillingTax.GetType().GetProperties();
                foreach (PropertyInfo autoBillingTaxProperty in autoBillingTaxProperties)
                {
                    PropertyInfo couponTaxRecordProperty = couponTaxRecord.GetType().GetProperty(autoBillingTaxProperty.Name);
                    if (couponTaxRecordProperty != null)
                    {
                        if (couponTaxRecordProperty.Name.Equals("Id"))
                        {
                            autoBillingTaxProperty.SetValue(autoBillingTax, Guid.NewGuid(), null);
                            continue;
                        }
                        MethodInfo setInfo = couponTaxRecordProperty.GetSetMethod();
                        if (setInfo != null)
                            autoBillingTaxProperty.SetValue(autoBillingTax, couponTaxRecordProperty.GetValue(couponTaxRecord, null), null);
                    }
                }
                autoBillingTax.Status = statusTypeId;
            return autoBillingTax;
        }

        /// <summary>
        /// Set AutoBilling Vat from Prime Coupon vat
        /// </summary>
        /// <param name="couponVatRecord"></param>
        /// <returns></returns>
        public AutoBillingPrimeCouponVat SetVatBreakDown(PrimeCouponVat couponVatRecord, int statusTypeId)
        {
            var autoBillingVat = new AutoBillingPrimeCouponVat();

            // Convert PrimeCouponTax object to AutoBilling Prime Coupon
            PropertyInfo[] autoBillingVatProperties = autoBillingVat.GetType().GetProperties();
            foreach (PropertyInfo autoBillingVatProperty in autoBillingVatProperties)
            {
                PropertyInfo couponVatRecordProperty = couponVatRecord.GetType().GetProperty(autoBillingVatProperty.Name);
                if (couponVatRecordProperty != null)
                {
                    if (couponVatRecordProperty.Name.Equals("Id"))
                    {
                        autoBillingVatProperty.SetValue(autoBillingVat, Guid.NewGuid(), null);
                        continue;
                    }
                    MethodInfo setInfo = couponVatRecordProperty.GetSetMethod();
                    if (setInfo != null)
                        autoBillingVatProperty.SetValue(autoBillingVat, couponVatRecordProperty.GetValue(couponVatRecord, null), null);
                }
            }
            autoBillingVat.Status = statusTypeId;
            return autoBillingVat;
        }

        /// <summary>
        /// Set AutoBilling Attachments from Prime Coupon attachment
        /// </summary>
        /// <param name="couponAttachment"></param>
        /// <returns></returns>
        public AutoBillingPrimeCouponAttachment SetAttachments(PrimeCouponAttachment couponAttachment, int statusTypeId)
        {
            var autoBillingAttachment = new AutoBillingPrimeCouponAttachment();

            // Convert PrimeCouponTax object to AutoBilling Prime Coupon
            PropertyInfo[] autoBillingAttachmentProperties = autoBillingAttachment.GetType().GetProperties();
            foreach (PropertyInfo autoBillingAttachmentProperty in autoBillingAttachmentProperties)
            {
                PropertyInfo couponAttachmentRecordProperty = couponAttachment.GetType().GetProperty(autoBillingAttachmentProperty.Name);
                if (couponAttachmentRecordProperty != null)
                {
                    if(couponAttachmentRecordProperty.Name.Equals("Id"))
                    {
                        autoBillingAttachmentProperty.SetValue(autoBillingAttachment, Guid.NewGuid() , null);
                        continue;
                    }
                    MethodInfo setInfo = couponAttachmentRecordProperty.GetSetMethod();
                    if (setInfo != null)
                        autoBillingAttachmentProperty.SetValue(autoBillingAttachment, couponAttachmentRecordProperty.GetValue(couponAttachment, null), null);
                }
            }
            autoBillingAttachment.Status = statusTypeId;
            return autoBillingAttachment;

        }

        /// <summary>
        /// Delete prime billing record
        /// </summary>
        /// <param name="id">Id of auto billing prime billing record which is to be deleted</param>
        /// <param name="isLastCoupon"></param>
        [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
        [HttpPost]
        public JsonResult PrimeBillingDelete(string id, bool isLastCoupon)
        {
            UIMessageDetail details;
            try
            {
                ArchiveOriginalCoupon(id, (int)AutoBillingStatusType.Delete);

                if (isLastCoupon)
                {
                    // Delete Coupon record and Invoice
                    var isDeleted = _nonSamplingInvoiceManager.DeleteCouponRecord(id);
                    details = GetDeleteMessage(isDeleted);
                }
                else
                {
                    // Delete Copuon record only
                    var isDeleted = _nonSamplingInvoiceManager.DeleteCouponRecord(id);
                    details = GetDeleteMessage(isDeleted);
                }
            }
            catch (ISBusinessException ex)
            {
                details = HandleDeleteException(ex.ErrorCode);
            }

            return Json(details);
        }

    }
}
