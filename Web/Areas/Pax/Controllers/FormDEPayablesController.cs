using System;
using System.Linq;
using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Core;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.Pax.Sampling;
using Iata.IS.Web.Areas.Pax.Controllers.Base;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.UIModel.Grid.Misc;
using Iata.IS.Web.UIModel.Grid.Pax;
using Iata.IS.Business.Pax;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;
using TransactionType = Iata.IS.Model.Enums.TransactionType;
using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;
using Iata.IS.Web.UIModel.Grid.Common;
using System.Globalization;

namespace Iata.IS.Web.Areas.Pax.Controllers
{
  public class FormDEPayablesController : PaxInvoiceControllerBase
  {
    private readonly ISamplingFormDEManager _samplingformDEManager;
    private readonly IReferenceManager _referenceManager;
    private const string FormDSourceCodeGridAction = "FormDSourceCodeGridData";
    private const string FormEVatGridAction = "FormEVatGridData";
    private const string FormEAvailableVatGridAction = "FormEAvailableVatGridData";
    private const string FormEUnappliedAmountVatGridAction = "FormEUnappliedAmountVatGridData";
    private const string FormDGridAction = "FormDGridData";
    private const string ProvisionalInvoiceGridAction = "ProvisionalInvoiceGridData";

    public FormDEPayablesController(ISamplingFormDEManager samplingformDEManager, IReferenceManager referenceManager, IMemberManager memberManager)
      : base(samplingformDEManager)
    {
      _samplingformDEManager = samplingformDEManager;
      _referenceManager = referenceManager;
      MemberManager = memberManager;
    }

    protected override int BillingCodeId
    {
      get { return Convert.ToInt32(BillingCode.SamplingFormDE); }
    }

    /// <summary>
    /// Allows to edit an form DE header.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Pax.Payables.SampleFormDE.View)]
    [HttpGet]
    public new ActionResult View(string invoiceId)
    {
      var smIsToBeTreatedBilateral = _referenceManager.GetSMIsToBeTreatedBilateral();
      ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateral);
      // check whether transactions exist for this form DE header
      ViewData[ViewDataConstants.TransactionExists] = _samplingformDEManager.IsTransactionExists(invoiceId);

      MakeInvoiceRenderReady(InvoiceHeader.Id, InvoiceHeader);

      // Create grid instance for Form D Details grid
      var formDSourceCodeGrid = new SamplingFormDDetailsSourceCodeGrid(ControlIdConstants.FormDSourceCodeGridId, Url.Action(FormDSourceCodeGridAction, new { invoiceId }));
      ViewData[ControlIdConstants.FormDSourceCodeGridId] = formDSourceCodeGrid.Instance;

      // Create grid instance for Form D List
      bool isRejectionAllowed = false;
      if (ViewData[ViewDataConstants.BillingType] != null && ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables)
      {
        isRejectionAllowed = true;
      }

      var samplingFormDGrid = new SamplingFormDGrid(ControlIdConstants.FormDGridId, Url.Action(FormDGridAction, new { invoiceId }), isRejectionAllowed);

      ViewData[ControlIdConstants.FormDGridId] = samplingFormDGrid.Instance;

      return View("Edit", InvoiceHeader);
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Payables.SampleFormDE.View)]
    [HttpGet]
    public ActionResult FormDView(string invoiceId, string transactionId)
    {
      var samplingFormDRecord = _samplingformDEManager.GetSamplingFormD(transactionId);
      samplingFormDRecord.Invoice = InvoiceHeader;

      return View("FormDEdit", samplingFormDRecord);
    }

    /// <summary>
    /// Download form D attachment
    /// </summary>
    /// <param name="invoiceId">Invoice id</param>
    /// <param name="transactionId">Transaction id</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Pax.Payables.SampleFormDE.Download)]
    [HttpGet]
    public FileStreamResult FormDAttachmentDownload(string invoiceId, string transactionId)
    {
      var fileDownloadHelper = new FileAttachmentHelper
                                                  {
                                                    Attachment = _samplingformDEManager.GetSamplingFormDAttachment(transactionId)
                                                  };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Payables.SampleFormDE.View)]
    [HttpGet]
    public ActionResult FormEView(string invoiceId)
    {
      var samplingFormEDetail = _samplingformDEManager.GetSamplingFormE(invoiceId);
      samplingFormEDetail.Invoice = _samplingformDEManager.GetInvoiceHeaderDetails(invoiceId);

      return View("FormEEdit", samplingFormEDetail);
    }

    /// <summary>
    /// GET: Edit Form E
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Pax.Payables.SampleFormDE.View)]
    public ActionResult Details(string invoiceId)
    {
      var samplingFormEDetail = _samplingformDEManager.GetSamplingFormE(invoiceId);
      samplingFormEDetail.Invoice = InvoiceHeader;
      ViewData["FormAbTotal"] = samplingFormEDetail.TotalAmountFormB;
   

      return View("FormEEdit", samplingFormEDetail);
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Payables.SampleFormDE.View)]
    [RestrictUnauthorizedUpdate]
    [HttpGet]
    public ActionResult ProvisionalInvoice(string invoiceId)
    {
      var record = new ProvisionalInvoiceRecordDetail();

      /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like Bilateral */
      if (!ReferenceManager.IsSmiLikeBilateral(InvoiceHeader.SettlementMethodId, true))
      {
        ViewData[ViewDataConstants.NotBilateralSettlementMethod] = true;
      }
      
      record.InvoiceId = invoiceId.ToGuid();
      record.Invoice = InvoiceHeader;

      var provisionalInvoiceGrid = new ProvisionalInvoiceGrid(ControlIdConstants.ProvisionalInvoiceGridId, Url.Action(ProvisionalInvoiceGridAction, new
                                                                                                                                                    {
                                                                                                                                                      invoiceId
                                                                                                                                                    }), InvoiceHeader.IsFormABViaIS);

      ViewData[ControlIdConstants.ProvisionalInvoiceGridId] = provisionalInvoiceGrid.Instance;
      var smIsToBeTreatedBilateral = _referenceManager.GetSMIsToBeTreatedBilateral();
      ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateral);

      return View(record);
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Payables.SampleFormDE.View)]
    [HttpGet]
    public ActionResult ProvisionalInvoiceView(string invoiceId)
    {
      var record = new ProvisionalInvoiceRecordDetail();

      /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like Bilateral */
      if (!ReferenceManager.IsSmiLikeBilateral(InvoiceHeader.SettlementMethodId, true))
      {
        ViewData[ViewDataConstants.NotBilateralSettlementMethod] = true;
      }
    
      record.InvoiceId = invoiceId.ToGuid();
      record.Invoice = InvoiceHeader;

      var provisionalInvoiceGrid = new ProvisionalInvoiceGrid(ControlIdConstants.ProvisionalInvoiceGridId, Url.Action(ProvisionalInvoiceGridAction, new
      {
        invoiceId
      }), InvoiceHeader.IsFormABViaIS);

      ViewData[ControlIdConstants.ProvisionalInvoiceGridId] = provisionalInvoiceGrid.Instance;
      var smIsToBeTreatedBilateral = _referenceManager.GetSMIsToBeTreatedBilateral();
      ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateral);

      return View("ProvisionalInvoice", record);
    }

    /// <summary>
    /// Fetch data form Provisional Invoices
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    public JsonResult ProvisionalInvoiceGridData(string invoiceId)
    {
      var provisionalInvoiceGrid = new ProvisionalInvoiceGrid(ControlIdConstants.ProvisionalInvoiceGridId, Url.Action(ProvisionalInvoiceGridAction));
      var provisionalInvoiceRecords = _samplingformDEManager.GetProvisionalInvoiceList(invoiceId);

      return provisionalInvoiceGrid.DataBind(provisionalInvoiceRecords);
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Payables.SampleFormDE.View)]
    public override ActionResult VatView(string invoiceId)
    {
      return View("Vat", VatBase(invoiceId));
    }

    private PaxInvoice VatBase(string invoiceId)
    {
      // Flag to set whether grid is to be displayed in View mode
      bool isGridViewOnly = false;

      // If Page mode is view set "isGridViewOnly" variable to true, depending on this variable action column is displayed.  
      if (ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
      {
        isGridViewOnly = true;
      }

      // Create grid instance for invoice vat list
      var invoiceVatGrid = new VatGrid(ControlIdConstants.InvoiceVatGridId, Url.Action(FormEVatGridAction, new { invoiceId }), isGridViewOnly);
      ViewData[ViewDataConstants.InvoiceVatGrid] = invoiceVatGrid.Instance;

      if (ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Receivables)
      {
        // Create grid instance for available vat 
        var availableVatGrid = new AvailableVatGrid(ControlIdConstants.AvailableVatGridId, Url.Action(FormEAvailableVatGridAction, new { invoiceId }));
        ViewData[ViewDataConstants.AvailableVatGrid] = availableVatGrid.Instance;

        //Create grid instance for vat not applied amount
        var unappliedAmountVatGrid = new UnappliedVat(ControlIdConstants.UnappliedAmountVatGridId, Url.Action(FormEUnappliedAmountVatGridAction, new { invoiceId }));
        ViewData[ViewDataConstants.UnappliedAmountVatGrid] = unappliedAmountVatGrid.Instance;
      }

      return InvoiceHeader;
    }

    /// <summary>
    /// Save Invoice level VAT 
    /// </summary>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Pax.Payables.SampleFormDE.View)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = true, TableName = TransactionTypeTable.INVOICE)]
    public override JsonResult Vat(FormCollection form, string invoiceId)
    {
      try
      {
        var vat = new JavaScriptSerializer().Deserialize(form[0], typeof(SamplingFormEDetailVat));
        var record = vat as SamplingFormEDetailVat;
        _samplingformDEManager.AddSamplingFormEVat(record);

        var details = new UIExceptionDetail
        {
          IsFailed = false,
          Message = Messages.RecordSaveSuccessful
        };
        return Json(details);
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode);
        var details = new UIExceptionDetail
        {
          IsFailed = false,
          Message = string.Format(Messages.RecordSaveException, GetDisplayMessage(businessException.ErrorCode))
        };
        return Json(details);
      }
    }

    /// <summary>
    /// Vat Data to populate in the Grid
    /// </summary>
    public JsonResult FormEVatGridData(string invoiceId)
    {
      //Create grid instance and retrieve data from database
      var invoiceVatGrid = new VatGrid(ControlIdConstants.InvoiceVatGridId, Url.Action(FormEVatGridAction, new { invoiceId }));

      var vatData = _samplingformDEManager.GetSamplingFormEVatList(invoiceId);
      return invoiceVatGrid.DataBind(vatData);
    }

    /// <summary>
    /// Available Vat Data to populate in the Grid
    /// </summary>
    public JsonResult FormEAvailableVatGridData(string invoiceId)
    {
      // Create grid instance and retrieve data from database
      var vatGrid = new AvailableVatGrid(ControlIdConstants.AvailableVatGridId, Url.Action(FormEAvailableVatGridAction, new { invoiceId }));

      var vatData = _samplingformDEManager.GetFormDInvoiceLevelDerivedVatList(invoiceId).AsQueryable();
      int count = 1;
      foreach (var derivedVatDetails in vatData)
      {
        derivedVatDetails.RowNumber = count++;
      }
      return vatGrid.DataBind(vatData);
    }

    /// <summary>
    /// Unapplied Vat amount Data to populate in the Grid
    /// </summary>
    public JsonResult FormEUnappliedAmountVatGridData(string invoiceId)
    {
      //Create grid instance and retrieve data from database
      var unappliedVatGrid = new UnappliedVat(ControlIdConstants.UnappliedAmountVatGridId, Url.Action(FormEUnappliedAmountVatGridAction, new { invoiceId }));
      var notAppliedVatList = _samplingformDEManager.GetFormDNonAppliedVatList(invoiceId).AsQueryable();
      int count = 1;
      foreach (var nonAppliedVatDetails in notAppliedVatList)
      {
        nonAppliedVatDetails.RowNumber = count++;
      }
      return unappliedVatGrid.DataBind(notAppliedVatList);
    }

    /// <summary>
    /// Fetch data for Form D and display it in grid
    /// </summary>
    /// <returns></returns>
    public JsonResult FormDGridData(string invoiceId)
    {
      // Create grid instance and retrieve data from database
      var formDGrid = new SamplingFormDGrid(ControlIdConstants.FormDGridId, Url.Action(FormDGridAction, new { invoiceId }));

      var formDList = _samplingformDEManager.GetSamplingFormDList(invoiceId);

      return formDGrid.DataBind(formDList);
    }

    /// <summary>
    /// Fetch data for Form D Source Code and display it in grid
    /// </summary>
    /// <returns></returns>
    public JsonResult FormDSourceCodeGridData(string invoiceId)
    {
      // Create grid instance and retrieve data from database
      var formDSourceCodeGrid = new SamplingFormDDetailsSourceCodeGrid(ControlIdConstants.FormDSourceCodeGridId, Url.Action(FormDSourceCodeGridAction, new { invoiceId }));

      var sourceCodeList = _samplingformDEManager.GetSourceCodeList(invoiceId).AsQueryable();
      return formDSourceCodeGrid.DataBind(sourceCodeList);
    }



    [ISAuthorize(Business.Security.Permissions.Pax.Payables.SampleFormDE.View)]
    [HttpPost]
    public JsonResult GetFormDLinkedCouponDetails(Guid invoiceId, int ticketCouponNumber, long ticketDocNumber, string issuingAirline)
    {
      var linkedCoupons = _samplingformDEManager.GetFormDLinkedCouponDetails(invoiceId, ticketCouponNumber, ticketDocNumber, issuingAirline);

      string errorMessage = string.Empty;
      if (linkedCoupons.Count == 0) errorMessage = Messages.CouponDoesNotExist;
      else if (!string.IsNullOrEmpty(linkedCoupons[0].ErrorCode))
        errorMessage = Messages.ResourceManager.GetString(linkedCoupons[0].ErrorCode);

      var linkedCouponDetails = new LinkedCouponDetails
      {
        LinkedCoupons = linkedCoupons,
        ErrorMessage = errorMessage
      };
      return Json(linkedCouponDetails);
    }
  }
}
