using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Iata.IS.Business;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Core;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Web.Util;
using Iata.IS.Model.Base;
using Iata.IS.Web.Util.Filters;
using log4net;
using BillingType = Iata.IS.Web.Util.BillingType;

namespace Iata.IS.Web.Controllers.Base
{
  public abstract class InvoiceControllerBase<T> : ISController where T : InvoiceBase
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    public IRepository<IsHttpDownloadLink> IsHttpDownloadLinkRepository { get; set; }

    /// <summary>
    /// Gets the type of the invoice for which controller is created.
    /// 
    /// </summary>
    /// <value>The type of the invoice.</value>
    protected virtual InvoiceType InvoiceType { 
      get
      {
        return InvoiceType.Invoice;
      }
    }
    
    /// <summary>
    /// Used to fetch the invoice using the OnActionExecuting method. This instance is thereafter used in the action methods.
    /// </summary>
    protected T InvoiceHeader;

    protected IMemberManager MemberManager;

    /// <summary>
    /// Gets the invoice header. This method is used in the OnActionExecuting override, in order to check the invoice status and
    /// set the appropriate page mode and check for authorization (whether the logged in user is authorized to view/edit this invoice or not). If
    /// unauthorized access is attempted, then the user is navigated to an "Unauthorized access" page.
    /// </summary>
    /// <param name="invoiceNumber">The invoice number being accessed.</param>
    /// <returns>The invoice instance.</returns>
    protected abstract T GetInvoiceHeader(string invoiceNumber);

    /// <summary>
    /// Following action will check Invoice status and depending on it, will set PageMode i.e. View mode or Edit mode.
    /// Similar method present in FormCController.cs
    /// </summary>
    /// <param name="invoiceStatus">Invoice Status</param>
    public void SetPageMode(InvoiceStatusType invoiceStatus)
    {
      // If InvoiceStatus is equal to any of below set page mode to View, else Edit
      if (invoiceStatus == InvoiceStatusType.ReadyForBilling || invoiceStatus == InvoiceStatusType.ProcessingComplete || invoiceStatus == InvoiceStatusType.Presented ||
          invoiceStatus == InvoiceStatusType.Claimed || invoiceStatus == InvoiceStatusType.ErrorCorrectable || invoiceStatus == InvoiceStatusType.ErrorNonCorrectable ||
          invoiceStatus == InvoiceStatusType.OnHold || invoiceStatus == InvoiceStatusType.FutureSubmitted)
      {
        SetViewDataPageMode(PageMode.View);
      }
      else
      {
        SetViewDataPageMode(PageMode.Edit);
      }

    }

    /// <summary>
    /// Following action is used to check whether the user is either a billing or billed member. 
    /// </summary>
    /// <param name="invoice">Invoice object</param>
    private bool IsMemberAuthorized(InvoiceBase invoice)
    {
      bool isAuthorized = true;
      var loggedInMemberId = SessionUtil.MemberId;

      // Billed member is only allowed to view presented invoices.
      if ((loggedInMemberId == invoice.BilledMemberId) && (invoice.InvoiceStatus == InvoiceStatusType.Presented))
      {
        SetViewDataBillingType(BillingType.Payables);
        isAuthorized = true;
      }
      else if (loggedInMemberId == invoice.BillingMemberId)
      {
        SetViewDataBillingType(BillingType.Receivables);
        isAuthorized = true;
      }

      return isAuthorized;
    }

    /// <summary>
    /// Determines whether the billing code specified in URL matches that of the invoice retrieved.
    /// </summary>
    /// <param name="invoiceBase">The invoice base.</param>
    /// <returns>
    /// 	<c>true</c> if [is valid billing code] [the specified billing code id]; otherwise, <c>false</c>.
    /// </returns>
    protected virtual bool IsValidBillingCode(InvoiceBase invoiceBase)
    {
      return true;
    }

    //protected void SetViewDataBillingType(string billingType)
    //{
    //  ViewData[ViewDataConstants.BillingType] = billingType;
    //}

    protected ActionResult Unauthorized(string invoiceNumber)
    {
      TempData[ViewDataConstants.InvoiceNumber] = invoiceNumber;

      // Return Unauthorized view.
      return View("~/Views/Shared/UnAuthorized.aspx");
    }

    protected class MemberLocationInformationSorter : Comparer<MemberLocationInformation>
    {
      public override int Compare(MemberLocationInformation x, MemberLocationInformation y)
      {
        return x.IsBillingMember ? 1 : 0;
      }
    }

    protected void InitMemberLocationInfo(InvoiceBase invoice)
    {
      if (invoice.MemberLocationInformation.Count() == 0)
      {
        invoice.MemberLocationInformation = new List<MemberLocationInformation>
                                              {
                                                new MemberLocationInformation { IsBillingMember = true, Invoice = invoice },
                                                new MemberLocationInformation { IsBillingMember = false, Invoice = invoice }
                                              };
      }
      else if (invoice.MemberLocationInformation.Count() == 1)
      {
        if (invoice.MemberLocationInformation[0].IsBillingMember)
        {
          invoice.MemberLocationInformation.Add(new MemberLocationInformation { IsBillingMember = false, Invoice = invoice });
        }
        else
        {
          var billedMemberLocation = invoice.MemberLocationInformation[0];

          // billed member information should be at 1st position.
          invoice.MemberLocationInformation.Add(billedMemberLocation);

          // initialize billing member information.
          invoice.MemberLocationInformation[0] = new MemberLocationInformation { IsBillingMember = true, Invoice = invoice };
        }
      }
      else if (invoice.MemberLocationInformation.Count() == 2)
      {
        if (invoice.MemberLocationInformation[0].IsBillingMember == false)
        {
          // swap the 0th and 1st positions.
          invoice.MemberLocationInformation.Sort(new MemberLocationInformationSorter());
        }
      }
    }

    /// <summary>
    /// Following method will be executed before execution of any Action within this controller.
    /// </summary>
    /// <param name="filterContext">Context of requested action</param>
    protected override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        // Call base class's OnActionExecuting() method
        base.OnActionExecuting(filterContext);
        SetViewDataBillingType(BillingType.Receivables);

        const string invoiceIdParamKey = "invoiceId";

        // Check whether requested action contains parameter named "InvoiceId"
        var invoiceIdParam = filterContext.ActionParameters.ContainsKey(invoiceIdParamKey)
                                 ? filterContext.ActionParameters[invoiceIdParamKey]
                                 : null;

        // Check whether requested action is "POST".
        var isPost = filterContext.RequestContext.HttpContext.Request.HttpMethod == "POST";

       
        // If requested action is "GET" action and contains parameter named "InvoiceId" retrieve Invoice header details
        if ((!isPost) && (invoiceIdParam != null))
        {
            try
            {
                // Retrieve Invoice header details
                InvoiceHeader = GetInvoiceHeader(invoiceIdParam.ToString());

                if (InvoiceHeader.BillingMemberId != SessionUtil.MemberId && InvoiceHeader.BilledMemberId != SessionUtil.MemberId)
                {
                    throw new Exception();
                }

            }
            catch (Exception)
            {
                //SCP ID: 15780
                ShowErrorMessage(ErrorCodes.InvoiceNotFound);
                IDictionary<string, object> paramList = new Dictionary<string, object>();
                var area = filterContext.Controller.ControllerContext.RouteData.DataTokens["area"].ToString();
                string controller = string.Empty;

                if (area.ToUpper() == "UATP" || area.ToUpper() == "UATPPAYABLES")
                {
                    controller = "ManageUatpInvoice";
                }
                else if (area.ToUpper() == "MISC" || area.ToUpper() == "MISCPAYABLES")
                {
                    controller = "ManageMiscInvoice";
                }
                else if (area.ToUpper() == "CARGO")
                {
                    controller = "CargoManageInvoice";
                }
                else 
                {
                    controller = "ManageInvoice";
                }
                paramList.Add("controller", controller);
                paramList.Add("action", "Index");
                paramList.Add("area", filterContext.Controller.ControllerContext.RouteData.DataTokens["area"]);

                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(paramList));

                return;
            }
          
            // Check whether member is authorized to access this invoice.
            if (!IsMemberAuthorized(InvoiceHeader))
            {
                filterContext.Result = Unauthorized(InvoiceHeader.InvoiceNumber);
            }
            else if (!IsValidBillingCode(InvoiceHeader))
            {
                throw new Exception(Messages.InvalidBillingCode);
            }
            else
            {
                // Set the page mode to View or Edit depending on InvoiceStatus.
                SetPageMode(InvoiceHeader.InvoiceStatus);

                // Added code to set Page mode as view when Submission method is Auto Billing
                if (InvoiceHeader.SubmissionMethodId == (int) SubmissionMethod.AutoBilling)
                {
                    ViewData[ViewDataConstants.PageMode] = PageMode.View;
                }

                //SCP ID: 15780
                // If pageMode == View and selected action method has "RestrictUnauthorizedUpdateAttribute" attribute redirect user to Unauthorized View   
                //if (ViewData[ViewDataConstants.PageMode].Equals(PageMode.View) && filterContext.ActionDescriptor.GetCustomAttributes(typeof(RestrictUnauthorizedUpdateAttribute), true).Count() > 0)
                //{
                //  filterContext.Result = Unauthorized(InvoiceHeader.InvoiceNumber);
                //}
            }
        }

    }


    protected int GetDigitalSignatureRequired(int memberId)
    {
        var digitalSignatureRequired = -1;
        var ebillingConfig = MemberManager.GetEbillingConfig(SessionUtil.MemberId);

        if (ebillingConfig != null)
        {
            digitalSignatureRequired = ebillingConfig.IsDigitalSignatureRequired
                                           ? (int) DigitalSignatureRequired.Yes
                                           : (int) DigitalSignatureRequired.No;
        }
        return digitalSignatureRequired;
    }

      [HttpGet]
    public FileStreamResult DownloadFile(string id)
    {
      string errorMessage;

      try
      {
        var linkId = id.ToGuid();
        Logger.Info("Download File Id: " + linkId);
        var httpDownloadLink = IsHttpDownloadLinkRepository.First(link => link.Id.Equals(linkId));
        if (httpDownloadLink != null)
        {
          var fileStream = new FileStream(httpDownloadLink.FilePath, FileMode.Open, FileAccess.Read);
          Logger.Info("Download File Name: " + httpDownloadLink.FilePath);

          return File(fileStream, "application/octet", Path.GetFileName(httpDownloadLink.FilePath));
        }
        errorMessage = string.Format("No file to download for given download id {0}", linkId);
      }
      catch (Exception exception)
      {
        Logger.Error(string.Format("Exception:{0} StackTrace:{1}", exception.Message, exception.StackTrace), exception);
        errorMessage = string.Format("Exception:{0} StackTrace:{1}", exception.Message, exception.StackTrace);
      }

      try
      {
        var memoryStream = GetMemoryStreamForMessage("Error occurred while downloading file.");

        return File(memoryStream, "application/octet", "Download_File_Error.txt");
      }
      catch (Exception exception)
      {
        Logger.Error("Exception", exception);
      }

      return null;
    }

    private MemoryStream GetMemoryStreamForMessage(string message)
    {
      var errorContent = System.Text.Encoding.ASCII.GetBytes(message);
      var memoryStream = new MemoryStream();
      memoryStream.Write(errorContent, 0, errorContent.Length);
      memoryStream.Position = 0;
      return memoryStream;
    }
  }
}