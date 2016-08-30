using System;
using System.Reflection;
using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Core.DI;
using Iata.IS.Model.Enums;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;
using log4net;

namespace Iata.IS.Web
{
  [LogActions]
  [ElmahHandleError]
  [Authorize]
  [ValidateInput(true)]
  public abstract class ISController : Controller
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    protected override void OnException(ExceptionContext filterContext)
    {
      // Log the exception.
      Logger.Error("Unhandled exception.", filterContext.Exception);

      base.OnException(filterContext);
      filterContext.ExceptionHandled = true;
      object routeValue = null;

      if (filterContext.Exception is ISSessionExpiredException || SessionUtil.UserId <= 0)
      {
        Session.Clear();
        filterContext.HttpContext.ClearError();
        filterContext.HttpContext.Response.Cookies.Remove("ASP.NET_SessionId");
        routeValue = new { title = "Session Expired", area = "" };
      }
        // Show custom error for injection attacks.
      else if (filterContext.Exception is System.Web.HttpRequestValidationException)
      {
        routeValue = new { title = "Wrong Input", area = "" };
      }
      else
      {
        routeValue = new { area = "" };
      }
      filterContext.Result = RedirectToAction("Error", "Home", routeValue);
    }


    /// <summary>
    /// retrun success message by fetching the message from the resource file.
    /// </summary>
    /// <param name="messageCode"></param>
    public string ShowMessageText(string messageCode)
    {
        var message = string.IsNullOrEmpty(messageCode) ? "Unknown error message (please define an error code)" : string.IsNullOrEmpty(Messages.ResourceManager.GetString(messageCode)) ? messageCode : string.Format("{0} - {1}", messageCode, Messages.ResourceManager.GetString(messageCode));
        return message;
    }

    /// <summary>
    /// Shows a success message by fetching the message from the resource file.
    /// </summary>
    /// <param name="messageCode"></param>
    /// <param name="crossRequest"></param>
    public void ShowSuccessMessage(string messageCode, bool crossRequest = true)
    {
      var message = string.IsNullOrEmpty(messageCode) ? "Unknown error message (please define an error code)" : messageCode;

      if (crossRequest)
      {
        TempData[ViewDataConstants.SuccessMessage] = message;
      }
      else
      {
        ViewData[ViewDataConstants.SuccessMessage] = message;
      }
    }

    public void ShowErrorMessage(string messageCode, bool crossRequest = false)
    {
      var message = string.IsNullOrEmpty(messageCode) ? "Unknown error message (please define an error code)" : string.IsNullOrEmpty(Messages.ResourceManager.GetString(messageCode)) ? messageCode : string.Format("{0} - {1}", messageCode, Messages.ResourceManager.GetString(messageCode));

      if (crossRequest)
      {
        TempData[ViewDataConstants.ErrorMessage] = message;
      }
      else
      {
        ViewData[ViewDataConstants.ErrorMessage] = message;
      }
    }

    /// <summary>
    /// Show error message with parameters.
    /// SCP199693 - create BM and close correspondence at same time - related to Spira [IN:008756] [TC:082202]
    /// </summary>
    /// <param name="messageCode"></param>
    /// <param name="values"></param>
    public void ShowErrorMessage(string messageCode, params  String[] values)
    {
        var errorDescription = Messages.ResourceManager.GetString(messageCode);

        // Replace place holders in error message with appropriate values.
        errorDescription = string.IsNullOrEmpty(errorDescription) ? "Unknown error message (please define an error code)" : String.Format(errorDescription, values);

        TempData[ViewDataConstants.ErrorMessage] = String.Format("{0} - {1}", messageCode, errorDescription);
    }

    /// <summary>
    /// This function is added to display error message for LineItemDetail
    /// In case of LineItemDetail, dynamic field server validaiton error, error code with messgae is return in exception code.
    /// This function is used to remove additional - at the end of error message in Server side validation error
    /// </summary>
    /// <param name="messageCode"></param>
    public void ShowErrorMessageForServerValidation(string messageCode)
    {
      var message = string.IsNullOrEmpty(messageCode) ? "Unknown error message (please define an error code)" : (string.IsNullOrEmpty(Messages.ResourceManager.GetString(messageCode)) ? messageCode : string.Format("{0} - {1}", messageCode, Messages.ResourceManager.GetString(messageCode)));

      ViewData[ViewDataConstants.ErrorMessage] = message;
    }

    public static string GetDisplayMessageWithErrorCode(string messageCode)
    {
      var message = string.IsNullOrEmpty(messageCode) ? "Unknown message (please define an message code)" : string.Format("{0} - {1}", messageCode, Messages.ResourceManager.GetString(messageCode));

      return message;
    }

    public static string GetDisplayMessage(string messageCode)
    {
      var message = string.IsNullOrEmpty(messageCode) ? "Unknown message (please define an message code)" : Messages.ResourceManager.GetString(messageCode);

      return message;
    }

    /// <summary>
    /// Sets the value of the PageMode key of ViewData to the specified page mode.
    /// </summary>
    /// <param name="pageMode">The page mode to set.</param>
    public void SetViewDataPageMode(string pageMode)
    {
      ViewData[ViewDataConstants.PageMode] = pageMode;
    }

    public void SetViewDataBillingType(string billingType)
    {
      ViewData[ViewDataConstants.BillingType] = billingType;
    }

    public void ShowCustomErrorMessage(string messageCode, string errorMsg, bool crossRequest = false)
    {
        var message = string.IsNullOrEmpty(messageCode)
                          ? "Unknown error message (please define an error code)"
                          : string.Format("{0} - {1} {2}", messageCode, errorMsg,
                                          Messages.ResourceManager.GetString(messageCode));
        //CMP#624 : Removed 'Error in the application.' text from error message. 
        if (errorMsg == "Error in the application.")
        {
            message = message.Replace("- Error in the application.", "");
        }

        // CMP#650 error messages contain dynamic values and so these messages are formatted
        // inside CargoInvoiceManager rather than on controller
        if (!string.IsNullOrEmpty(messageCode) && (messageCode.Equals(CargoErrorCodes.InvalidRMReasonCodeForBM) ||
                                                   messageCode.Equals(CargoErrorCodes.InvalidRMReasonCodeForStage1) ||
                                                   messageCode.Equals(CargoErrorCodes.InvalidRMReasonCodeForRejectedRM) ||
                                                   messageCode.Equals(CargoErrorCodes.InvalidRMReasonCodeForStage2And3)))
        {
            message = string.Format("{0} - {1}", messageCode, errorMsg);
        }

        if (crossRequest)
        {
            TempData[ViewDataConstants.ErrorMessage] = message;
        }
        else
        {
            ViewData[ViewDataConstants.ErrorMessage] = message;
        }
    }

    public string ValidateBillingMonthYearPeriodSearch(int month, int year, int period = 0, int thresholdmonth = -6)
    {
      var errorMsg = "Valid Parameters";
      var calendarManager = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager));

      if (calendarManager != null)
      {
        var currBillingPeriod = calendarManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);

        var thresholdDate =
          new DateTime(currBillingPeriod.Year, currBillingPeriod.Month, currBillingPeriod.Period).AddMonths(thresholdmonth);

        var inputDate = new DateTime(year, month, period <= 0 ? 1 : period);

        if (inputDate < thresholdDate)
        {//CMP#570: Enhancements to PAX Non-Sampling Rejection Analysis Report
          errorMsg = thresholdmonth == -6 ? "Data older than 6 Billing Months cannot be retrieved. Please modify your search criteria." : string.Format("Data older than {0} Billing Months cannot be retrieved. Please modify your search criteria.", thresholdmonth);
        }
      }

      return errorMsg;
    }

    /// <summary>
    /// Validates the billing month year search.
    /// CMP #570: Enhancements to PAX Non-Sampling Rejection Analysis Report
    /// CMP #691: PAX Non-Sampling and CGO -Modifications to Rejection Analysis Reports
    /// </summary>
    /// <param name="fromMonth">From month.</param>
    /// <param name="fromYear">From year.</param>
    /// <param name="toMonth">To month.</param>
    /// <param name="toYear">To year.</param>
    /// <returns></returns>
    public string ValidateBillingMonthYearSearch(int fromMonth, int fromYear, int toMonth, int toYear)
    {    

      try
      {
        var fromInputDate = new DateTime(fromYear, fromMonth, 1);
        var toInputDate = new DateTime(toYear, toMonth, 1);

        //CMP#570 Change: Reports cannot be generated for Original Billing Months earlier than 30 months. 
        //CMP #691: PAX Non-Sampling and CGO -Modifications to Rejection Analysis Reports
        //Validation logic updated in CMP#691

        string errMsg = "Valid Parameters";

        var errorMsg = ValidateBillingMonthYearPeriodSearch(fromMonth, fromYear, 1, -30);

        if (errorMsg.ToUpper() != errMsg.ToUpper())
        {
          return errorMsg;
        }
        else if (fromInputDate > toInputDate)
        {
          return "The combination of ‘To’ Billing Year/Month /should not be earlier than the combination of ‘From’ Billing Year/Month";
        }
        else
        {
          int diffInMonths = ((toInputDate.Year - fromInputDate.Year) * 12) + toInputDate.Month - fromInputDate.Month;
          if ( Math.Abs(diffInMonths) > 12)
          {
            return "Report cannot be generated for more than 12 Original Billing Months";
          }
        }
        return errorMsg;
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat("ValidateBillingMonthYearSearch failed: [{0}].", ex.Message);
        return "Validation failed!";
      }
    }

   protected override void HandleUnknownAction(string actionName)
        {
            // Log the exception.
            Logger.ErrorFormat("Unknown action method [{0}].", actionName);

            try
            {
                base.HandleUnknownAction(actionName);
            }
            catch
            {
                Session.Abandon();
                HttpContext.Response.Redirect(AdminSystem.SystemParameters.Instance.General.LogOnURL);
            }
        }
    
    
    /// <summary>
    // If member id is 0 by any chance, user should not be able to view records of other members, hence redirect him to login page.
    /// </summary>
    /// <param name="memberId">Member id from session.</param>
    protected void IsMemberNullOrEmpty(int memberId)
    {
      if(SessionUtil.MemberId <= 0)
      {
        HttpContext.Response.Redirect(AdminSystem.SystemParameters.Instance.General.LogOnURL);
      }
    }

      protected void ClearSession()
      {
          Session.RemoveAll();
          Session.Abandon();
          Session.Clear();
      }

      #region 

      public void ShowSmiXWebServiceErrorMessage(string message, bool crossRequest = false)
      {
          if (crossRequest)
          {
              TempData[ViewDataConstants.ErrorMessage] = message;
          }
          else
          {
              ViewData[ViewDataConstants.ErrorMessage] = message;
          }
      }
      
      #endregion
  }
}
