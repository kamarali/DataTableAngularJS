using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Xml;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Business.ManageSystemParameter;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.Pax;
using Iata.IS.Business.Reports;
using Iata.IS.Core.DI;
using Iata.IS.Model;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
//using Iata.IS.Model.Enums.ErrorType;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.MiscUatp.Enums;
using Iata.IS.Model.Pax.Enums;
using iPayables.UserManagement;
using Iata.IS.Model.SupportingDocuments.Enums;
using Iata.IS.Business.Common.Impl;
using log4net.Repository.Hierarchy;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Master;

namespace Iata.IS.Web.Util.ExtensionHelpers
{
  public static class DropdownHelper
  {
    private const string PleaseSelectText = "Please Select";
    private const string HandlingFeeTypeOther = "OTHERS";
    private const string AllText = "All";
    private const string DefaultLocationCode = "Main";

    private const string AllMembers = "All Members";
    private const string AllICHMembers = "All ICH Members";
    private const string AllACHMembers = "All ACH Members";
    private const string IchAndAchText = "ICH & ACH";
    private const int MiscBillingCategory = 3;

    private static readonly SelectListItem PleaseSelectListItem = new SelectListItem { Text = PleaseSelectText, Value = "" };
    private static readonly SelectListItem PleaseSelectIntListItem = new SelectListItem { Text = PleaseSelectText, Value = "0" };
    private static readonly SelectListItem AllListItem = new SelectListItem { Text = AllText, Value = "-1" };
    private static readonly SelectListItem IchAndAchItem = new SelectListItem { Text = IchAndAchText, Value = "99" };
    private static readonly SelectListItem AllListItemWithZeroValue = new SelectListItem { Text = AllText, Value = "0" };

    public static bool IsViewMode(this HtmlHelper html)
    {

      if (html.ViewData.ContainsKey(ViewDataConstants.PageMode))
      {
        return string.Compare(PageMode.View, html.ViewData[ViewDataConstants.PageMode].ToString(), true) == 0;
      }

      return false;
    }
    public static bool IsEditMode(this HtmlHelper html)
    {
      if (html.ViewData.ContainsKey(ViewDataConstants.PageMode))
      {
        return string.Compare(PageMode.Edit, html.ViewData[ViewDataConstants.PageMode].ToString(), true) == 0;
      }

      return false;
    }
    public static bool IsPayablesBillingType(this HtmlHelper html)
    {
      if (html.ViewData.ContainsKey(ViewDataConstants.BillingType))
      {
        return string.Compare(BillingType.Payables, html.ViewData[ViewDataConstants.BillingType].ToString(), true) == 0;
      }

      return false;
    }


    #region Currency related Methods

    /// <summary>
    /// Creates currency dropdown and sets its selected value.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    public static MvcHtmlString CurrencyDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes = null)
    {
      // Get Currency Data
      var currencydSelectList = GetCurrencySelectList();

      return html.DropDownListFor(expression, currencydSelectList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Creates currency dropdown and sets its selected value.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="controlId">The control id.</param>
    /// <param name="selectedValue">The selected value.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString CurrencyDropdownList(this HtmlHelper html, string controlId, string selectedValue, IDictionary<string, object> htmlAttributes = null)
    {
      // Get Currency Data
      var currencySelectList = GetCurrencySelectList();

      var selectedItem = currencySelectList.SingleOrDefault(item => item.Value == selectedValue);

      if (IsViewMode(html))
      {
        return html.TextBox(controlId, selectedItem != null ? selectedItem.Text : string.Empty, htmlAttributes);
      }

      if (selectedItem != null)
      {
        selectedItem.Selected = true;
      }

      return html.DropDownList(controlId, currencySelectList, PleaseSelectText, htmlAttributes);
    }


    /// <summary>
    /// Transactions the type dropdown list for.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString TransactionTypeDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html,
                                                                        Expression<Func<TModel, TValue>> expression,
                                                                        object htmlAttributes = null)
    {
      // Get TransactionType Data
      var transactiontypeList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetTransactionTypeList();
      var pleaseSelectText = new SelectListItem() { Text = PleaseSelectText, Value = "0", Selected = true };
      var transactiontypeSelectList = new List<SelectListItem>();
      transactiontypeSelectList.Add(pleaseSelectText);

      transactiontypeSelectList.AddRange(transactiontypeList.Select(transactiontype => new SelectListItem
                                                                                           {
                                                                                             Text =
                                                                                                 transactiontype.
                                                                                                 Name,
                                                                                             Value =
                                                                                                 transactiontype.Id.
                                                                                                 ToString()
                                                                                           }).ToList());


      return html.DropDownListFor(expression, transactiontypeSelectList, htmlAttributes);
    }

    public static MvcHtmlString TransactionTypeDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html,
                                                                    Expression<Func<TModel, TValue>> expression, int billingCategoryId,
                                                                    object htmlAttributes = null)
    {
      // Get TransactionType Data
      var transactiontypeList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetTransactionTypeList().Where(tran => tran.BillingCategoryCode == billingCategoryId);

      var transactiontypeSelectList = transactiontypeList.Select(transactiontype => new SelectListItem
      {
        Text = transactiontype.Name,
        Value = transactiontype.Id.ToString()
      }).ToList();

      return html.DropDownListFor(expression, transactiontypeSelectList, PleaseSelectText, htmlAttributes);
    }
    /// <summary>
    /// Transaction Type the dropdown list.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="controlId">The control id.</param>
    /// <param name="selectedValue">The selected value.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString TransactionTypeDropdownList(this HtmlHelper html,
                                                                           string controlId,
                                                                           string selectedValue,
                                                                           object htmlAttributes = null)
    {
      // Get TransactionType Data
      var transactiontypeList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetTransactionTypeList();

      var transactiontypeSelectList = transactiontypeList.Select(transactiontype => new SelectListItem
      {
        Text = transactiontype.Name,
        Value = transactiontype.Id.ToString(),
        Selected = (transactiontype.Id.ToString() == selectedValue)
      }).ToList();

      return html.DropDownList(controlId, transactiontypeSelectList, PleaseSelectText, htmlAttributes);
    }

    public static MvcHtmlString FileFormatTypeDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html,
                                                                     Expression<Func<TModel, TValue>> expression, bool isDownLoadable, bool isReport,
                                                                     object htmlAttributes = null, bool? dailyOutputSearch = null)
    {
      IEnumerable<FileFormat> fileformatList;
      if (isReport)
        fileformatList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetFileFormatTypeList().Where(fileformat => fileformat.IsActive && (fileformat.Id == (int)FileFormatType.IsIdec || fileformat.Id == (int)FileFormatType.IsXml || fileformat.Id == (int)FileFormatType.FormCXml || fileformat.Id == (int)FileFormatType.Usage));
      else
        // Get TransactionType Data
        fileformatList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetFileFormatTypeList().Where(fileformat => fileformat.IsActive && fileformat.FileDownloadable == isDownLoadable);
      //fileformatList = fileformatList.Where(fileformat => fileformat.FileDownloadable == isDownLoadable);

      //CMP529 : Daily Output Generation for MISC Bilateral Invoices
      if (dailyOutputSearch.HasValue)
      {
        //CMP#622: MISC Outputs Split as per Location ID
        if (dailyOutputSearch.Value)
        {
          //only add two daily formats
          fileformatList =
            fileformatList.Where(f => f.Id == (int)FileFormatType.DailyMiscBilateralIsXml || f.Id == (int)FileFormatType.DailyMiscBilateralOfflineArchive || f.Id == (int)FileFormatType.DailyMiscBilateralIsXmlLocSpec || f.Id == (int)FileFormatType.DailyMiscBilateralOARLocSpec);
        }
        else
        {
          //exclude 2 daily formats
          fileformatList = fileformatList.Where(f => f.Id != (int)FileFormatType.DailyMiscBilateralIsXml && f.Id != (int)FileFormatType.DailyMiscBilateralOfflineArchive && f.Id != (int)FileFormatType.DailyMiscBilateralIsXmlLocSpec && f.Id != (int)FileFormatType.DailyMiscBilateralOARLocSpec);
        }
      }

      var fileformatSelectList = fileformatList.Select(fileformat => new SelectListItem
      {
        Text = fileformat.Description,
        Value = fileformat.Id.ToString()
      }).ToList().OrderBy(f => f.Text);
      return html.DropDownListFor(expression, fileformatSelectList, AllText, htmlAttributes);
    }



    public static MvcHtmlString sysMonitorFileFormatTypeDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html,
                                                                     Expression<Func<TModel, TValue>> expression, bool isDownLoadable, object htmlAttributes = null)
    {
      IEnumerable<FileFormat> fileformatList;
      // Get TransactionType Data
      fileformatList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).SysMonitorGetFileFormatTypeList().Where(fileformat => (fileformat.IsActive && fileformat.FileDownloadable == isDownLoadable) || fileformat.Id == 17 || fileformat.Id == 14);
      //fileformatList = fileformatList.Where(fileformat => fileformat.FileDownloadable == isDownLoadable);

      var fileformatSelectList = fileformatList.Select(fileformat => new SelectListItem
      {
        Text = fileformat.Description,
        Value = fileformat.Id.ToString()
      }).ToList().OrderBy(f => f.Text);

      return html.DropDownListFor(expression, fileformatSelectList, AllText, htmlAttributes);
    }


    /// <summary>
    /// Rfics the dropdown list for.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString RficDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html,
                                                                        Expression<Func<TModel, TValue>> expression,
                                                                        object htmlAttributes = null)
    {
      // Get TransactionType Data
      var rficList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetRficList().OrderBy(rfic => rfic.Id);

      var rficSelectList = rficList.Select(rfic => new SelectListItem
      {
        Text = rfic.Id,
        Value = rfic.Id.ToString()
      }).ToList();
      return html.DropDownListFor(expression, rficSelectList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Rfics the dropdown list.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="controlId">The control id.</param>
    /// <param name="selectedValue">The selected value.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString RficDropdownList(this HtmlHelper html,
                                                                           string controlId,
                                                                           string selectedValue,
                                                                           object htmlAttributes = null)
    {
      // Get TransactionType Data
      var rficList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetRficList();

      var rficSelectList = rficList.Select(rfic => new SelectListItem
      {
        Text = rfic.Description,
        Value = rfic.Id.ToString(),
        Selected = (rfic.Id.ToString() == selectedValue)
      }).ToList();

      return html.DropDownList(controlId, rficSelectList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Miscs the group dropdown list for.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString MiscCodeGroupDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html,
                                                                           Expression<Func<TModel, TValue>> expression,
                                                                           object htmlAttributes = null)
    {
      // Get TransactionType Data
      var miscCodeGroupList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetMiscCodeGroupList().OrderBy(group => group.MiscGroup);

      var miscCodeGroupSelectList = miscCodeGroupList.Select(Group => new SelectListItem
      {
        Text = Group.MiscGroup,
        Value = Group.Id.ToString()
      }).ToList();
      return html.DropDownListFor(expression, miscCodeGroupSelectList, PleaseSelectText, htmlAttributes);
    }


    /// <summary>
    /// Members the status dropdown list.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="controlId">The control id.</param>
    /// <param name="selectedValue">The selected value.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString MemberStatusDropdownList(this HtmlHelper html,
                                                                           string controlId,
                                                                           string selectedValue,
                                                                           object htmlAttributes = null)
    {
      var memberStatusList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetMemberStatusList();

      var memberStatusSelectList = memberStatusList.Select(membestatus => new SelectListItem
      {
        Text = membestatus.Name,
        Value = membestatus.Id.ToString()
      }).ToList();

      return html.DropDownList(controlId, memberStatusSelectList, PleaseSelectText, htmlAttributes);
    }

    public static MvcHtmlString UserQuestionDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html,
                                                                      Expression<Func<TModel, TValue>> expression, int categoryId,
                                                                      object htmlAttributes = null)
    {
      // Get Reason Code Data
      var UserQuestionList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetUserQuestionList(categoryId);

      var UserQuestionSelectList = UserQuestionList.Select(userQue => new SelectListItem
      {
        Text = userQue.Question,
        Value = userQue.Id.ToString()
      }).ToList();
      return html.DropDownListFor(expression, UserQuestionSelectList, PleaseSelectText, htmlAttributes);
    }
    public static MvcHtmlString UserQuestionDropdownList(this HtmlHelper html, string controlName, string userQuestion, int categoryId,
                                                                      object htmlAttributes = null)
    {
      var UserQuestionList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetUserQuestionList(categoryId);
      var UserQuestionSelectList = UserQuestionList.Select(userQue => new SelectListItem
      {
        Text = userQue.Question,
        Value = userQue.Question,
        Selected = (userQue.Question == userQuestion)
      });

      return html.DropDownList(controlName, UserQuestionSelectList, PleaseSelectText, htmlAttributes);
    }
    /// <summary>
    /// Reasons the code dropdown list for.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString ReasonCodeDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html,
                                                                        Expression<Func<TModel, TValue>> expression, int transactionTypeId,
                                                                        object htmlAttributes = null)
    {
      // Get Reason Code Data
      var ReasonCodeList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetReasonCodeList(transactionTypeId);

      var ReasonCodeSelectList = ReasonCodeList.Select(reasoncode => new SelectListItem
      {
        Text = reasoncode.Code,
        Value = reasoncode.Id.ToString()
      }).ToList();

      return html.DropDownListFor(expression, ReasonCodeSelectList, PleaseSelectText, htmlAttributes);
    }

    public static MvcHtmlString ReasonCodeDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html,
                                                                           Expression<Func<TModel, TValue>> expression,
                                                                           object htmlAttributes = null)
    {
      // Get Reason Code Data
      var ReasonCodeList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetReasonCodeList();

      var ReasonCodeSelectList = ReasonCodeList.Select(reasoncode => new SelectListItem
      {
        Text = reasoncode.Code,
        Value = reasoncode.Id.ToString()
      }).ToList();

      return html.DropDownListFor(expression, ReasonCodeSelectList, PleaseSelectText, htmlAttributes);
    }
    /// <summary>
    /// Reasons the code dropdown list.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="controlId">The control id.</param>
    /// <param name="selectedValue">The selected value.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString ReasonCodeDropdownList(this HtmlHelper html,
                                                                           string controlId,
                                                                           string selectedValue,
                                                                           object htmlAttributes = null)
    {
      // Get Reason Code Data
      var ReasonCodeList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetReasonCodeList();

      var ReasonCodeSelectList = ReasonCodeList.Select(reasoncode => new SelectListItem
      {
        Text = reasoncode.Code,
        Value = reasoncode.Id.ToString(),
        Selected = (reasoncode.Id.ToString() == selectedValue)
      }).ToList();

      return html.DropDownList(controlId, ReasonCodeSelectList, PleaseSelectText, htmlAttributes);
    }

    public static MvcHtmlString MiscBillingCurrencyDropdownList(this HtmlHelper html, string controlId, int selectedValue, int memberId, object htmlAttributes = null)
    {

      // Get Currency Data
      var currencySelectList = GetCurrencySelectList(true).ToList();

      var location = Ioc.Resolve<IMemberManager>(typeof(IMemberManager)).GetMemberDefaultLocation(memberId, DefaultLocationCode);

      if (location != null)
      {
        // Default selection should be currency code of the main location country of the Billing member
        var currencyId = location.CurrencyId.HasValue ? location.CurrencyId.Value : 0;

        if (currencyId > 0)
        {
          currencySelectList.SingleOrDefault(currencySelectItem => currencySelectItem.Value == currencyId.ToString()).Selected = true;
        }
      }

      var selectedItem = currencySelectList.SingleOrDefault(item => item.Value == selectedValue.ToString());

      if (IsViewMode(html))
      {
        return html.TextBox(controlId, selectedItem != null ? selectedItem.Text : string.Empty, htmlAttributes);
      }

      if (selectedItem != null)
      {
        selectedItem.Selected = true;
      }

      return html.DropDownList(controlId, currencySelectList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Get Billing Currency Dropdown List 
    /// </summary>
    /// <param name="html"></param>
    /// <param name="controlId"></param>
    /// <param name="selectedValue"></param>
    /// <param name="htmlAttributes"></param>
    /// <param name="settlementMethodId"></param>
    /// <returns></returns>
    public static MvcHtmlString BillingCurrencyDropdownList(this HtmlHelper html, string controlId, string selectedValue, object htmlAttributes = null, int settlementMethodId = 0)
    {
        // Updated for SCP:55911 
        //Check here for SMI 
        /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like Bilateral */
      var isLikeBilateral = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).IsSmiLikeBilateral(settlementMethodId, htmlAttributes == null ? true : false);

      if(settlementMethodId == (int)SMI.AdjustmentDueToProtest)
      {
        isLikeBilateral = true;
      }

      // Get Currency Data
      //If SMI is Bilateral or Like Bilateral,then return all currency list else return those currency which are defined in enum.
      var billingCurrencyList = isLikeBilateral ? GetCurrencySelectList() : EnumMapper.GetBillingCurrencyList();


      var selectedItem = billingCurrencyList.SingleOrDefault(item => item.Value == selectedValue);

      if (IsViewMode(html))
      {
        if (selectedItem == null)
        {
          // Get all currencies(For Bilateral and similar SMIs.)
          var currencySelectList = GetCurrencySelectList();

          selectedItem = currencySelectList.SingleOrDefault(item => item.Value == selectedValue);
        }

        return html.TextBox(controlId, selectedItem != null ? selectedItem.Text : string.Empty, htmlAttributes);
      }

      if (selectedItem != null)
      {
        selectedItem.Selected = true;
      }

      return html.DropDownList(controlId, billingCurrencyList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Get Ach Billing Currency for Dropdown List 
    /// CMP #553: ACH Requirement for Multiple Currency Handling
    /// </summary>
    /// <param name="html"></param>
    /// <param name="controlId"></param>
    /// <param name="selectedValue"></param>
    /// <param name="htmlAttributes"></param>
    /// <param name="settlementMethodId"></param>
    /// <returns></returns>
    public static MvcHtmlString BillingAchCurrencyDropdownList(this HtmlHelper html, string controlId, string selectedValue, object htmlAttributes = null, int settlementMethodId = 0)
    {
        var achCurrencyManager = Ioc.Resolve<IAchCurrencySetUpManager>(typeof (IAchCurrencySetUpManager));

        //Get ach currency list from DB.
        var achCurrencyList = achCurrencyManager.GetAchCurrencySetUpList(String.Empty, 1);

        //Add default currency as USD.
        var achCurrencies = new List<SelectListItem> { new SelectListItem { Text = "USD", Value = "840" } };

        achCurrencies.AddRange(achCurrencyList.Select(currency => new SelectListItem
                                                                      {
                                                                          Text = currency.CurrencyCode,
                                                                          Value = currency.Id.ToString()
                                                                      }).ToList());

        return html.DropDownList(controlId, achCurrencies, PleaseSelectText, htmlAttributes);
    }

    #endregion

    #region Billing Period related methods

    /// <summary>
    /// Returns Billing Period dropdown list.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="isForSearch"></param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    public static MvcHtmlString StaticBillingPeriodDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html,
                                                                                   Expression<Func<TModel, TValue>> expression,
                                                                                   bool isForSearch = false,
                                                                                   object htmlAttributes = null)
    {
      var bilingPeriodSelectList = EnumMapper.GetInvoicePeriodList();

      if (isForSearch)
      {
        bilingPeriodSelectList.Insert(0, AllListItem);

        return html.DropDownListFor(expression, bilingPeriodSelectList);
      }

      return html.DropDownListFor(expression, bilingPeriodSelectList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Returns Billing Period Dropdown List with first option as All.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="isForSearch"></param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    public static MvcHtmlString StaticBillingPeriodDropdownListAllFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, bool isForSearch = false, object htmlAttributes = null)
    {
      var bilingPeriodSelectList = EnumMapper.GetInvoicePeriodList();
      bilingPeriodSelectList.Insert(0, AllListItem);
      return html.DropDownListFor(expression, bilingPeriodSelectList);
    }

    public static MvcHtmlString StaticBillingPeriodDropdownList(this HtmlHelper html, string controlName, int selectedValue, bool isForSearch = false, object htmlAttributes = null)
    {

      if (IsViewMode(html))
      {
        return html.TextBox(controlName, selectedValue);
      }

      var bilingPeriodSelectList = EnumMapper.GetInvoicePeriodList();

      var bilingPeriodSelect = bilingPeriodSelectList.SingleOrDefault(item => item.Value == selectedValue.ToString());
      if (bilingPeriodSelect != null)
        bilingPeriodSelect.Selected = true;

      if (isForSearch)
      {
        bilingPeriodSelectList.Insert(0, AllListItem);

        return html.DropDownList(controlName, bilingPeriodSelectList, htmlAttributes);
      }

      return html.DropDownList(controlName, bilingPeriodSelectList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Dropdown list for Supporting document search criteria field Type
    /// </summary>
    /// <param name="html"></param>
    /// <param name="controlName"></param>
    /// <param name="selectedValue"></param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    public static MvcHtmlString SupportingDocTypeDropdownList(this HtmlHelper html, string controlName, int selectedValue, object htmlAttributes = null)
    {
      var supDocTypeSelectList = EnumMapper.GetSupportingDocTypeList();

      var supDocTypeSelect = supDocTypeSelectList.SingleOrDefault(item => item.Value == selectedValue.ToString());
      if (supDocTypeSelect != null)
        supDocTypeSelect.Selected = true;

      return html.DropDownList(controlName, supDocTypeSelectList);

    }

    /// <summary>
    /// Drodpdown list for Supporting document search criteria field Attachment Indicator
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    public static MvcHtmlString SupportingDocAttachmentIndicatorDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html,
                                                                                   Expression<Func<TModel, TValue>> expression,
                                                                                   object htmlAttributes = null)
    {
      var attchIndSelectList = EnumMapper.GetSupportingDocAttachIndicatorList();
      return html.DropDownListFor(expression, attchIndSelectList);

    }

    /// <summary>
    /// Drodpdown list for Supporting document search criteria field Attachment Indicator
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="billingCategoryType"></param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    public static MvcHtmlString SupportingDocAttachmentIndicatorDropdownList<TModel, TValue>(this HtmlHelper<TModel> html,
                                                                                   Expression<Func<TModel, TValue>> expression,
                                                                                   string billingCategoryType, object htmlAttributes = null)
    {
      var attchIndSelectList = EnumMapper.GetSupportingDocAttachIndicatorList();
      return html.DropDownListFor(expression, attchIndSelectList);

    }
    /// <summary>
    /// Returns Validated PMI dropdown list.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="isForSearch"></param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    public static MvcHtmlString StaticValidatedPMIDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html,
                                                                                   Expression<Func<TModel, TValue>> expression,
                                                                                   bool isForSearch = false,
                                                                                   object htmlAttributes = null)
    {
      var bilingPeriodSelectList = EnumMapper.GetPaxCouponRecordList();

      if (isForSearch)
      {
        bilingPeriodSelectList.Insert(0, AllListItem);

        return html.DropDownListFor(expression, bilingPeriodSelectList);
      }

      return html.DropDownListFor(expression, bilingPeriodSelectList, PleaseSelectText, htmlAttributes);
    }


    /// <summary>
    /// Returns Billing Period dropdown list with field value parameter as selected value.
    /// </summary>
    /// <param name="html"></param>
    /// <param name="fieldName"></param>
    /// <param name="fieldValue"></param>
    /// <param name="mode">Display mode.</param>
    /// <param name="htmlAttributes"></param>
    /// <param name="setDefault">if true then set default value like "Please select" otherwise set current period</param>
    /// <returns></returns>
    public static MvcHtmlString StaticBillingPeriodDropdownList(this HtmlHelper html,
                                                                string fieldName,
                                                                string fieldValue,
                                                                TransactionMode mode = TransactionMode.Transactions,
                                                                object htmlAttributes = null, bool setDefault = false)
    {
      var calendarManager = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager));
      BillingPeriod period;
      string selectedValue;
      var bilingPeriodSelectList = EnumMapper.GetInvoicePeriodList();

      switch (mode)
      {
        case TransactionMode.BillingHistory:
        case TransactionMode.InvoiceSearch:
          period = calendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);  //GetCurrentBillingPeriod();
          bilingPeriodSelectList.Insert(0, AllListItem);
          selectedValue = (fieldValue == "0") ? period.Period.ToString() : fieldValue;
          if (setDefault && (fieldValue == "0"))
          {
            selectedValue = "-1";
          }
          break;
        case TransactionMode.Payables:
          period = calendarManager.GetLastClosedBillingPeriod();
          bilingPeriodSelectList.Insert(0, AllListItem);
          selectedValue = (fieldValue == "0") ? period.Period.ToString() : fieldValue;
          break;
        case TransactionMode.CalendarSearch:
          bilingPeriodSelectList.Insert(0, AllListItem);
          selectedValue = fieldValue;
          break;
        case TransactionMode.ProcessingDashboard:
          bilingPeriodSelectList.Insert(0, AllListItemWithZeroValue);
          selectedValue = fieldValue;
          break;
        default:
          bilingPeriodSelectList.Insert(0, PleaseSelectIntListItem);
          selectedValue = fieldValue;
          break;
      }

      bilingPeriodSelectList.SingleOrDefault(item => item.Value == selectedValue).Selected = true;

      return MvcHtmlString.Create(DropdownList(html, fieldName, bilingPeriodSelectList));
    }


    /// <summary>
    /// Invoices the period dropdown list for processing dash board.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString InvoicePeriodDropdownListForProcessingDashBoard<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
    {
      var invoicePeriodList = new List<SelectListItem> {
                                                         new SelectListItem {
                                                                              Value = "-1",
                                                                              Text = AllText
                                                                            },
                                                         new SelectListItem {
                                                                              Value = "1",
                                                                              Text = "1"
                                                                            },
                                                         new SelectListItem {
                                                                              Value = "2",
                                                                              Text = "2"
                                                                            },
                                                         new SelectListItem {
                                                                              Value = "3",
                                                                              Text = "3"
                                                                            },
                                                         new SelectListItem {
                                                                              Value = "4",
                                                                              Text = "4"
                                                                            }
                                                       }; //EnumMapper.GetInvoicePeriodList();

      return html.DropDownListFor(expression, invoicePeriodList, htmlAttributes);
      // return html.DropDownList(controlName, invoicePeriodList, htmlAttributes);
    }


    //public static MvcHtmlString InvoicePeriodDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
    //{
    //  List<SelectListItem> invoicePeriodList = EnumMapper.GetInvoicePeriodList();

    //  invoicePeriodList.Insert(0,
    //                           new SelectListItem
    //                             {
    //                               Value = "-1",
    //                               Text = AllText
    //                             });

    //  return html.DropDownListFor(expression, invoicePeriodList, htmlAttributes);
    //}

    #endregion

    /// <summary>
    /// Author: Sachin Pharande
    /// Date: 21-03-2012
    /// Purpose: File Download typesdropdown list for SIS Usage Report.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    public static MvcHtmlString DownoadFileTypesDropdownListForSisUsageReport<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
    {
      var downoadFileTypesList = new List<SelectListItem> {
                                                         new SelectListItem {
                                                                              Value = "1",
                                                                              Text = "Excel"
                                                                            },
                                                         new SelectListItem {
                                                                              Value = "2",
                                                                              Text = "Pdf"
                                                                            }
                                                       };
      return html.DropDownListFor(expression, downoadFileTypesList, htmlAttributes);
    }

    /// <summary>
    /// Creates billing period dropdown and sets its selected value.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="name"></param>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="period"></param>
    /// <returns></returns>
    public static MvcHtmlString BillingYearMonthPeriodDropdown(this HtmlHelper html, string name, int year, int month, int period)
    {

      if (IsViewMode(html))
      {
        return html.TextBox(name, string.Format("{0}-{1}-{2}", year,
                                 CultureInfo.CurrentCulture.DateTimeFormat.
                          GetAbbreviatedMonthName(month), period));
      }

      var calendarManager = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager));

      var billingPeriods = calendarManager.GetRelevantBillingPeriods(string.Empty, ClearingHouse.Ich);
      var currentBillingPeriod = calendarManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);  //GetCurrentBillingPeriod();
      if (IsEditMode(html))
      {
        if (year != 0 && month != 0 && period != 0)
        {
          var isPeriodExist = billingPeriods.Where(p => p.Year == year && p.Month == month && p.Period == period).Count();
          if (isPeriodExist == 0)
          {
            billingPeriods.Add(new BillingPeriod(year, month, period));
          }
        }

      }
      var selectedPeriod = year == 0
                                ? string.Format("{0}-{1}-{2}", currentBillingPeriod.Year, currentBillingPeriod.Month, currentBillingPeriod.Period)
                                : string.Format("{0}-{1}-{2}", year, month, period);

      var billingPeriodSelectList = billingPeriods.Select(billingPeriod => new SelectListItem
      {
        Text =
          string.Format("{0}-{1}-{2}",
                        billingPeriod.Year,
                        CultureInfo.CurrentCulture.DateTimeFormat.
                          GetAbbreviatedMonthName(billingPeriod.Month),
                        billingPeriod.Period),
        Value =
          string.Format("{0}-{1}-{2}",
                        billingPeriod.Year,
                        billingPeriod.Month,
                        billingPeriod.Period),
        Selected =
          string.Format("{0}-{1}-{2}",
                        billingPeriod.Year,
                        billingPeriod.Month,
                        billingPeriod.Period) == selectedPeriod
      });

      return html.DropDownList(name, billingPeriodSelectList);
    }

    /// <summary>
    /// Returns Billing Month dropdown
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    [Obsolete]
    public static MvcHtmlString BillingMonthDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
    {
      // Get Billing Period Data
      var bilingMonthSelectList = new List<SelectListItem>();
      for (var i = 0; i < 12; i++)
      {
        var month = DateTime.UtcNow.AddMonths(-i).ToString(FormatConstants.MonthNameFormat);
        var monthValue = DateTime.UtcNow.AddMonths(-i).Month.ToString();
        bilingMonthSelectList.Add(new SelectListItem
                                    {
                                      Text = month,
                                      Value = monthValue
                                    });
      }

      return html.DropDownListFor(expression, bilingMonthSelectList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Returns Html string for Billing Month Dropdown.
    /// If page is in view mode, then textbox is returned.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="controlName">Name of the control.</param>
    /// <param name="selectedMonth">The selected month.</param>
    /// <returns></returns>
    public static MvcHtmlString BillingMonthDropdownList(this HtmlHelper html, string controlName, int selectedMonth, object htmlAttributes = null)
    {

      if (IsViewMode(html))
      {
        return html.TextBox(controlName, CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(selectedMonth));
      }

      // Get Billing Period Data
      var bilingMonthSelectList = new List<SelectListItem>();
      for (var i = 0; i < 12; i++)
      {
        var month = DateTime.UtcNow.AddMonths(-i).ToString(FormatConstants.MonthNameFormat);

        var monthValue = DateTime.UtcNow.AddMonths(-i).Month;
        var monthValueString = monthValue.ToString();
        bilingMonthSelectList.Add(new SelectListItem
        {
          Text = month,
          Value = monthValueString,
          Selected = (monthValue == selectedMonth)
        });
      }
      return html.DropDownList(controlName, bilingMonthSelectList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Generates a dropdown having items of year-Month format
    /// </summary>
    /// <param name="html">The Html</param>
    /// <param name="controlName">Name to be given to dropdown</param>
    /// <param name="provisionalBillingMonth">Month value from model</param>
    /// <param name="provisionalBillingYear">Year value from model</param>
    /// <param name="numberOfMonths">Number of months to be displayed in dropdown</param>
    /// <returns></returns>
    public static MvcHtmlString ProvisionalBillingMonthDropdownList(this HtmlHelper html, string controlName, int provisionalBillingMonth, int provisionalBillingYear, int numberOfMonths = 10)
    {

      if (IsViewMode(html))
      {
        return html.TextBox(controlName, string.Format("{0}-{1}", provisionalBillingYear,
                                                                 CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(provisionalBillingMonth)));
      }

      var currentBillingPeriod = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager)).GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);
      var currentBillingInDateTimeFormat = new DateTime(currentBillingPeriod.Year, currentBillingPeriod.Month, currentBillingPeriod.Period);

      DateTime time;
      var billingMonthYearSelectList = new List<SelectListItem>();
      var currentDateTime = DateTime.UtcNow;

      var selectedMonthYear = provisionalBillingMonth == 0
                                   ? string.Format("{0}-{1}", currentDateTime.Year, currentDateTime.Month)
                                   : string.Format("{0}-{1}", provisionalBillingYear, provisionalBillingMonth);

      for (var i = 0; i < numberOfMonths; i++)
      {

        time = currentBillingInDateTimeFormat.AddMonths(-i);

        var month = time.ToString(FormatConstants.MonthNameFormat);
        var itemValue = string.Format("{0}-{1}", time.Year, time.Month);

        billingMonthYearSelectList.Add(new SelectListItem
                                         {
                                           Selected = (itemValue == selectedMonthYear),
                                           Text = string.Format("{0}-{1}", time.Year, month),
                                           Value = itemValue
                                         });
      }

      return html.DropDownList(controlName, billingMonthYearSelectList);
    }

    /// <summary>
    /// Returns Billing Year dropdown
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    public static MvcHtmlString BillingYearDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
    {
      // Get Billing Period Data
      var bilingYearSelectList = new List<SelectListItem>();

      for (var i = 0; i < 6; i++)
      {
        var year = DateTime.UtcNow.AddYears(-i).ToString(FormatConstants.FullYearFormat);
        bilingYearSelectList.Add(new SelectListItem
                                   {
                                     Text = year,
                                     Value = year
                                   });
      }

      return html.DropDownListFor(expression, bilingYearSelectList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Author: Sachin R Pharande
    /// Date: 02-12-2011
    /// Purpose: To Display Last Two Billing Years from Current Billing Year
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    public static MvcHtmlString BillingYearTwoDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
    {
      // Get Billing Period Data
      var bilingYearSelectList = new List<SelectListItem>();


      var lastclosedbillingPeriod = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager)).GetLastClosedBillingPeriod();
      DateTime d = new DateTime(lastclosedbillingPeriod.Year, 1, 1);

      var year = lastclosedbillingPeriod.Year.ToString();
      bilingYearSelectList.Add(new SelectListItem
      {
        Text = year,
        Value = year
      });


      var nextYear = d.AddYears(-1).ToString(FormatConstants.FullYearFormat);

      bilingYearSelectList.Add(new SelectListItem
      {
        Text = nextYear,
        Value = nextYear
      });


      return html.DropDownListFor(expression, bilingYearSelectList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Author: Sanket Shrivastava
    /// Date: 23-07-2013
    /// Purpose: To Display Last Two Billing Years from Current Billing Year in 
    /// Misc Supporting Attachments report
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    public static MvcHtmlString BillingTwoYearDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
    {
      // Get Billing Period Data
      var bilingYearSelectList = new List<SelectListItem>();


      var billingPeriod = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager)).GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
      DateTime d = new DateTime(billingPeriod.Year, 1, 1);

      var year = billingPeriod.Year.ToString();
      bilingYearSelectList.Add(new SelectListItem
      {
        Text = year,
        Value = year
      });


      var nextYear = d.AddYears(-1).ToString(FormatConstants.FullYearFormat);

      bilingYearSelectList.Add(new SelectListItem
      {
        Text = nextYear,
        Value = nextYear
      });


      return html.DropDownListFor(expression, bilingYearSelectList, PleaseSelectText, htmlAttributes);
    }


    /// <summary>
    /// Author: Sachin R Pharande
    /// Date: 11-01-2012
    /// Purpose: To Display Billing Year Dropdown Which Shows Year of Current Billing Period + Last 3 Years (but not earlier than 2011). Default to Year of Current open Period.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    public static MvcHtmlString BillingYearLastThreeDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
    {
      // Get Billing Period Data
      var bilingYearSelectList = new List<SelectListItem>();

      for (var i = 0; i < 3; i++)
      {
        var year = DateTime.UtcNow.AddYears(-i).ToString(FormatConstants.FullYearFormat);
        if (Convert.ToInt32(year) >= 2011)
        {
          bilingYearSelectList.Add(new SelectListItem
          {
            Text = year,
            Value = year
          });
        }
      }

      return html.DropDownListFor(expression, bilingYearSelectList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Returns Html String for Billing Year Dropdown.
    /// If the page is in view mode, then text box is returned.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="controlName">Name of the control.</param>
    /// <param name="selectedYear">The selected year.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString BillingYearDropdownList(this HtmlHelper html, string controlName, int selectedYear, object htmlAttributes = null)
    {

      if (IsViewMode(html))
      {
        return html.TextBox(controlName, selectedYear);
      }

      // Get Billing Period Data
      var bilingYearSelectList = new List<SelectListItem>();

      for (var i = 0; i < 6; i++)
      {
        var year = DateTime.UtcNow.AddYears(-i);
        var yearString = year.ToString(FormatConstants.FullYearFormat);
        bilingYearSelectList.Add(new SelectListItem
        {
          Text = yearString,
          Value = yearString,
          Selected = (year.Year == selectedYear)
        });
      }

      return html.DropDownList(controlName, bilingYearSelectList, PleaseSelectText, htmlAttributes);
    }

    public static MvcHtmlString AutoBillingFileTypeDropdownList(this HtmlHelper html, string controlName, object htmlAttributes = null)
    {
      var autoBillFileTypeList = new List<SelectListItem>();

      autoBillFileTypeList.Add(new SelectListItem { Value = "RevenueRecognitionFile", Text = "Revenue Recognition File" });
      autoBillFileTypeList.Add(new SelectListItem { Value = "InvoicePostingFile", Text = "Invoice Posting File" });
      autoBillFileTypeList.Add(new SelectListItem { Value = "ValueRequestIrregularityReport", Text = "Value-Request Irregularity Report" });


      return html.DropDownList(controlName, autoBillFileTypeList, PleaseSelectText, htmlAttributes);

    }



    public static MvcHtmlString BillingPeriodDropdownList(this HtmlHelper html, string controlName, int selectedPeriod, object htmlAttributes = null)
    {
      var periodList = new List<SelectListItem> {
                                                         new SelectListItem {
                                                                              Value = "",
                                                                              Text = "Please Select"
                                                                            },
                                                         new SelectListItem {
                                                                              Value = "1",
                                                                              Text = "1",
                                                                              Selected = selectedPeriod==1?true:false
                                                                            },
                                                         new SelectListItem {
                                                                              Value = "2",
                                                                              Text = "2",
                                                                              Selected = selectedPeriod==2?true:false
                                                                            },
                                                         new SelectListItem {
                                                                              Value = "3",
                                                                              Text = "3",
                                                                              Selected = selectedPeriod==3?true:false
                                                                            },
                                                         new SelectListItem {
                                                                              Value = "4",
                                                                              Text = "4",
                                                                              Selected = selectedPeriod==4?true:false
                                                                            }
                                                       };


      return html.DropDownList(controlName, periodList, htmlAttributes);

    }


    public static MvcHtmlString OutputDropDownList(this HtmlHelper html, string controlName, object htmlAttributes = null)
    {
      var periodList = new List<SelectListItem> {
                                                         new SelectListItem {
                                                                              Value = "",
                                                                              Text = "Please Select"
                                                                            },
                                                         new SelectListItem {
                                                                              Value = "1",
                                                                              Text = "Only Sub Totals"
                                                                            },
                                                         new SelectListItem {
                                                                              Value = "2",
                                                                              Text = "Only Details"
                                                                            },
                                                         new SelectListItem {
                                                                              Value = "3",
                                                                              Text = "Subtotals and Details"
                                                                            }
                                                       };


      return html.DropDownList(controlName, periodList, htmlAttributes);

    }


    public static MvcHtmlString OutputDropDownSubmissionOverviewList(this HtmlHelper html, string controlName, object htmlAttributes = null)
    {
      var periodList = new List<SelectListItem> {
                                                         new SelectListItem {
                                                                              Value = "",
                                                                              Text = "Please Select"
                                                                            },
                                                         new SelectListItem {
                                                                              Value = "1",
                                                                              Text = "Only Sub Totals"
                                                                            },
                                                         new SelectListItem {
                                                                              Value = "2",
                                                                              Text = "Only Invoice Totals"
                                                                            },
                                                         new SelectListItem {
                                                                              Value = "3",
                                                                              Text = "Subtotals and Invoice Totals"
                                                                            }
                                                       };


      return html.DropDownList(controlName, periodList, htmlAttributes);

    }


    /// <summary>
    /// Returns Billing Year Month dropdown
    /// Mainly Use for Search.
    /// </summary>
    /// <param name="html"></param>
    /// <param name="name"></param>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="mode"></param>
    /// <param name="setDefault">if true then set default value like "Please select" otherwise set current period</param>
    /// <returns></returns>
    public static MvcHtmlString BillingYearMonthDropdown(this HtmlHelper html, string name, int year, int month, TransactionMode mode = TransactionMode.InvoiceSearch, bool setDefault = false)
    {
      var bilingMonthSelectList = new List<SelectListItem>();

      var currentBillingPeriod = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager)).GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich); // GetCurrentBillingPeriod();

      // Convert DateTime format of CurrentBillingPeriod
      var currentBillingInDateTimeFormat = new DateTime(currentBillingPeriod.Year, currentBillingPeriod.Month, currentBillingPeriod.Period);

      var selectedPeriod = ((year != 0 && month != 0) || setDefault) ? string.Format("{0}-{1}", year, month) : string.Format("{0}-{1}", currentBillingPeriod.Year, currentBillingPeriod.Month);

      if (IsViewMode(html))
      {
        return html.TextBox(name, selectedPeriod);
      }

      var counter = 0;
      if (mode == TransactionMode.InvoiceSearch) --counter;

      for (var i = counter; i < 12; i++)
      {
        // Previously Month was being added to UtcNow Date, instead modified code to add month in DateTime format created from CurrentBilling period.
        var strmonth = currentBillingInDateTimeFormat.AddMonths(-i).ToString(FormatConstants.MonthNameFormat);
        var stryear = currentBillingInDateTimeFormat.AddMonths(-i).Year.ToString();
        var monthValue = currentBillingInDateTimeFormat.AddMonths(-i).Month.ToString();

        bilingMonthSelectList.Add(new SelectListItem
                                    {
                                      Text = string.Format("{0}-{1}", stryear, strmonth),
                                      Value = string.Format("{0}-{1}", stryear, monthValue),
                                      Selected = string.Format("{0}-{1}", stryear, monthValue) == selectedPeriod
                                    });
      }

      return html.DropDownList(name, bilingMonthSelectList, PleaseSelectText);
    }

    public static MvcHtmlString CGOBillingYearMonthDropdown(this HtmlHelper html, string name, int year, int month, TransactionMode mode = TransactionMode.InvoiceSearch)
    {
      var bilingMonthSelectList = new List<SelectListItem>();

      var currentBillingPeriod = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager)).GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich); // GetCurrentBillingPeriod();

      // Convert DateTime format of CurrentBillingPeriod
      var currentBillingInDateTimeFormat = new DateTime(currentBillingPeriod.Year, currentBillingPeriod.Month, currentBillingPeriod.Period);

      var selectedPeriod = year != 0 && month != 0 ? string.Format("{0}-{1}", year, month) : string.Format("{0}-{1}", currentBillingPeriod.Year, currentBillingPeriod.Month);

      if (IsViewMode(html))
      {
        return html.TextBox(name, selectedPeriod);
      }

      var counter = 0;
      //if (mode == TransactionMode.InvoiceSearch) --counter;

      for (var i = counter; i < 12; i++)
      {
        // Previously Month was being added to UtcNow Date, instead modified code to add month in DateTime format created from CurrentBilling period.
        var strmonth = currentBillingInDateTimeFormat.AddMonths(-i).ToString(FormatConstants.MonthNameFormat);
        var stryear = currentBillingInDateTimeFormat.AddMonths(-i).Year.ToString();
        var monthValue = currentBillingInDateTimeFormat.AddMonths(-i).Month.ToString();

        bilingMonthSelectList.Add(new SelectListItem
        {
          Text = string.Format("{0}-{1}", stryear, strmonth),
          Value = string.Format("{0}-{1}", stryear, monthValue),
          Selected = string.Format("{0}-{1}", stryear, monthValue) == selectedPeriod
        });
      }

      return html.DropDownList(name, bilingMonthSelectList, PleaseSelectText);
    }

    /// <summary>
    ///  this method for year-month-period dropdownlist on validation Error correction Screen 
    /// </summary>
    /// <param name="html"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static MvcHtmlString BillingYearMonthDropdownValidationErrorCorrection(this HtmlHelper html, string name, int year, int month)
    {
      var calendarManager = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager));

      var billingPeriods = calendarManager.GetRelevantBillingPeriods(string.Empty, ClearingHouse.Ich, 0);
      var currentBillingPeriod = calendarManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);

      var selectedPeriod = year != 0 && month != 0 ? string.Format("{0}-{1}", year, month) : string.Format("{0}-{1}", currentBillingPeriod.Year, currentBillingPeriod.Month);


      // more lines added by me
      if (billingPeriods.Count == 2)
      {
        if (billingPeriods[0].Month == billingPeriods[1].Month)
        {
          billingPeriods.RemoveAt(1);
        }
      }

      var bilingMonthSelectList = billingPeriods.Select(billingPeriod => new SelectListItem
                    {
                      Text = string.Format("{0}-{1}",
                                      billingPeriod.Year,
                                     CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(billingPeriod.Month)),

                      Value =
                        string.Format("{0}-{1}",
                                      billingPeriod.Year,
                                      billingPeriod.Month),
                      Selected =
                        string.Format("{0}-{1}",
                                      billingPeriod.Year,
                                      billingPeriod.Month) == selectedPeriod
                    });


      return html.DropDownList(name, bilingMonthSelectList);

    }


    /// <summary>
    ///  this method for year-month-period dropdownlist on validation Error correction Screen 
    /// </summary>
    /// <param name="html"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static MvcHtmlString BillingYearMonthPeriodDropdownValidationErrorCorrection(this HtmlHelper html, string name, int year, int month, int period, TransactionMode mode = TransactionMode.InvoiceSearch, object htmlAttributes = null)
    {
      var calendarManager = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager));
      var bilingMonthSelectList = new List<SelectListItem>();
      var billingPeriods = calendarManager.GetRelevantBillingPeriods(string.Empty, ClearingHouse.Ich, 0);
      var currentBillingPeriod = calendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich); // GetCurrentBillingPeriod();

      // Convert DateTime format of CurrentBillingPeriod
      var currentBillingInDateTimeFormat = new DateTime(currentBillingPeriod.Year, currentBillingPeriod.Month, currentBillingPeriod.Period);

      var selectedPeriod = year != 0 && month != 0 && period != 0 ? string.Format("{0}-{1}-{2}", year, month, period) : string.Format("{0}-{1}-{2}", currentBillingPeriod.Year, currentBillingPeriod.Month, currentBillingPeriod.Period);

      if (billingPeriods.Count > 1) //if late submission is OPEN
      {
        var billingPeriodsTimeFormat = new DateTime(billingPeriods[0].Year, billingPeriods[0].Month, billingPeriods[0].Period);
        var stryear = billingPeriods[0].Year.ToString();
        var strmonth = billingPeriodsTimeFormat.ToString(FormatConstants.MonthNameFormat);
        var strPeriod = billingPeriods[0].Period;
        var monthValue = billingPeriods[0].Month.ToString();

        bilingMonthSelectList.Add(new SelectListItem
        {

          Text = string.Format("{0}-{1}-{2}", stryear, strmonth, strPeriod),
          Value = string.Format("{0}-{1}-{2}", stryear, monthValue, strPeriod),
          Selected =
              string.Format("{0}-{1}-{2}", stryear, monthValue, strPeriod) ==
              selectedPeriod
        });
      }
      if (IsViewMode(html))
      {
        return html.TextBox(name, selectedPeriod);
      }

      var counter = 0;
      if (mode == TransactionMode.InvoiceSearch) --counter;

      for (var i = 0; i < 2; i++)
      {
        // Previously Month was being added to UtcNow Date, instead modified code to add month in DateTime format created from CurrentBilling period.
        var strmonth = currentBillingInDateTimeFormat.AddMonths(i).ToString(FormatConstants.MonthNameFormat);
        var stryear = currentBillingInDateTimeFormat.AddMonths(i).Year.ToString();
        var monthValue = currentBillingInDateTimeFormat.AddMonths(i).Month.ToString();
        int strPeriod = period;

        if (i == 0)
        {
          //CMP#626 : TFS#9301
          strPeriod = currentBillingPeriod.Period;
        }
        else
        {
          strPeriod = 1;
        }
        for (var j = 0; j < 4; j++)
        {
          if (!bilingMonthSelectList.Exists(item => item.Value == string.Format("{0}-{1}-{2}", stryear, monthValue, strPeriod)))
          {
            bilingMonthSelectList.Add(new SelectListItem
            {
              Text = string.Format("{0}-{1}-{2}", stryear, strmonth, strPeriod),
              Value =
                  string.Format("{0}-{1}-{2}", stryear, monthValue, strPeriod),
              Selected =
                  string.Format("{0}-{1}-{2}", stryear, monthValue, strPeriod) ==
                  selectedPeriod
            });
          }
          if (i == 0 && strPeriod == 4)
          {
            j = 4;
          }
          strPeriod++;
        }
      }

      return html.DropDownList(name, bilingMonthSelectList, PleaseSelectText, htmlAttributes);

    }

    /// <summary>
    /// Returns Billing Year Month dropdown
    /// Mainly Use for Search.
    /// </summary>
    /// <param name="html"></param>
    /// <param name="name"></param>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static MvcHtmlString PayableBillingYearMonthDropdown(this HtmlHelper html, string name, int year, int month, TransactionMode mode = TransactionMode.InvoiceSearch)
    {
      var bilingMonthSelectList = new List<SelectListItem>();

      var currentBillingPeriod = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager)).GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich); //GetCurrentBillingPeriod();

      // Convert DateTime format of CurrentBillingPeriod
      var currentBillingInDateTimeFormat = new DateTime(currentBillingPeriod.Year, currentBillingPeriod.Month, currentBillingPeriod.Period);

      var selectedPeriod = year != 0 && month != 0 ? string.Format("{0}-{1}", year, month) : string.Format("{0}-{1}", currentBillingPeriod.Year, currentBillingPeriod.Month);

      if (IsViewMode(html))
      {
        return html.TextBox(name, selectedPeriod);
      }

      var counter = 0;
      if (mode == TransactionMode.InvoiceSearch) --counter;

      for (var i = counter; i < 12; i++)
      {
        var previousMonth = currentBillingInDateTimeFormat.AddMonths(-i);
        var strmonth = previousMonth.ToString(FormatConstants.MonthNameFormat);
        var stryear = previousMonth.Year.ToString();
        var monthValue = previousMonth.Month.ToString();

        bilingMonthSelectList.Add(new SelectListItem
        {
          Text = string.Format("{0}-{1}", stryear, strmonth),
          Value = string.Format("{0}-{1}", stryear, monthValue),
          Selected = string.Format("{0}-{1}", stryear, monthValue) == selectedPeriod
        });
      }

      return html.DropDownList(name, bilingMonthSelectList, PleaseSelectText);
    }

    /// <summary>
    /// Returns Billing Year Month dropdown
    /// Mainly Use for Search.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="name">The name.</param>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    /// <param name="suppDocType">Type of the supp doc.</param>
    /// <returns></returns>
    public static MvcHtmlString SupportingDocBillingYearMonthDropdown(this HtmlHelper html, string name, int year, int month, int suppDocType)
    {
      var bilingMonthSelectList = new List<SelectListItem>();

      var currentBillingPeriod = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager)).GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich); //GetCurrentBillingPeriod();
      var currentBillingInDateTimeFormat = new DateTime(currentBillingPeriod.Year, currentBillingPeriod.Month, currentBillingPeriod.Period);
      var selectedPeriod = year != 0 && month != 0 ? string.Format("{0}-{1}", year, month) : string.Format("{0}-{1}", currentBillingPeriod.Year, currentBillingPeriod.Month);

      if (IsViewMode(html))
      {
        return html.TextBox(name, selectedPeriod);
      }


      if (suppDocType == (int)SupportingDocType.InvoiceCreditNote || suppDocType == (int)SupportingDocType.MiscInvoice || suppDocType == (int)SupportingDocType.UatpInvoice)
      {
        for (var i = 0; i < 2; i++)
        {
          var strmonth = currentBillingInDateTimeFormat.AddMonths(-i).ToString(FormatConstants.MonthNameFormat);
          var stryear = currentBillingInDateTimeFormat.AddMonths(-i).Year.ToString();
          var monthValue = currentBillingInDateTimeFormat.AddMonths(-i).Month.ToString();

          bilingMonthSelectList.Add(new SelectListItem
          {
            Text = string.Format("{0}-{1}", stryear, strmonth),
            Value = string.Format("{0}-{1}", stryear, monthValue),
            Selected = string.Format("{0}-{1}", stryear, monthValue) == selectedPeriod
          });
        }
      }
      else if (suppDocType == (int)SupportingDocType.FormC)
      {
        for (var i = 0; i < 5; i++)
        {
          var strmonth = currentBillingInDateTimeFormat.AddMonths(-i).ToString(FormatConstants.MonthNameFormat);
          var stryear = currentBillingInDateTimeFormat.AddMonths(-i).Year.ToString();
          var monthValue = currentBillingInDateTimeFormat.AddMonths(-i).Month.ToString();

          bilingMonthSelectList.Add(new SelectListItem
          {
            Text = string.Format("{0}-{1}", stryear, strmonth),
            Value = string.Format("{0}-{1}", stryear, monthValue),
            Selected = string.Format("{0}-{1}", stryear, monthValue) == selectedPeriod
          });
        }
      }

      return html.DropDownList(name, bilingMonthSelectList, PleaseSelectText);
    }

    /// <summary>
    /// Returns Billing Year Month dropdown
    /// Mainly Use for Search.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="name">The name.</param>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    /// <param name="suppDocType">Type of the supp doc.</param>
    /// <returns></returns>
    public static MvcHtmlString PayableSupportingDocBillingYearMonthDropdown(this HtmlHelper html, string name, int year, int month, int suppDocType)
    {
      var bilingMonthSelectList = new List<SelectListItem>();

      var currentBillingPeriod = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager)).GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich); //GetCurrentBillingPeriod();

      var currentBillingInDateTimeFormat = new DateTime(currentBillingPeriod.Year, currentBillingPeriod.Month, currentBillingPeriod.Period);

      var selectedPeriod = year != 0 && month != 0 ? string.Format("{0}-{1}", year, month) : string.Format("{0}-{1}", currentBillingPeriod.Year, currentBillingPeriod.Month);

      if (IsViewMode(html))
      {
        return html.TextBox(name, selectedPeriod);
      }


      if (suppDocType == (int)SupportingDocType.InvoiceCreditNote)
      {
        for (var i = 0; i < 12; i++)
        {
          var previousMonth = currentBillingInDateTimeFormat.AddMonths(-i);
          var strmonth = previousMonth.ToString(FormatConstants.MonthNameFormat);
          var stryear = previousMonth.Year.ToString();
          var monthValue = previousMonth.Month.ToString();

          bilingMonthSelectList.Add(new SelectListItem
          {
            Text = string.Format("{0}-{1}", stryear, strmonth),
            Value = string.Format("{0}-{1}", stryear, monthValue),
            Selected = string.Format("{0}-{1}", stryear, monthValue) == selectedPeriod
          });
        }
      }
      else if (suppDocType == (int)SupportingDocType.FormC)
      {
        for (var i = 0; i < 12; i++)
        {
          var previousMonth = currentBillingInDateTimeFormat.AddMonths(-i);
          var strmonth = previousMonth.ToString(FormatConstants.MonthNameFormat);
          var stryear = previousMonth.Year.ToString();
          var monthValue = previousMonth.Month.ToString();

          bilingMonthSelectList.Add(new SelectListItem
          {
            Text = string.Format("{0}-{1}", stryear, strmonth),
            Value = string.Format("{0}-{1}", stryear, monthValue),
            Selected = string.Format("{0}-{1}", stryear, monthValue) == selectedPeriod
          });
        }
      }

      return html.DropDownList(name, bilingMonthSelectList, PleaseSelectText);
    }

    /// <summary>
    /// Creates Provisional Billing Month dropdown, display current month - 4 months and 2 months before that
    /// </summary>
    /// <param name="html"></param>
    /// <param name="controlName">control name</param>
    /// <param name="provisionalBillingMonth">provisional billing month for selected value</param>
    /// <param name="provisionalBillingYear">provisional billing year for selected value</param>
    /// <param name="validProvMonth"></param>
    /// <param name="numberOfMonths"></param>
    /// <returns></returns>
    public static MvcHtmlString ProvisionalBillingMonthFormDEDropdownList(this HtmlHelper html, string controlName, int provisionalBillingMonth, int provisionalBillingYear, int validProvMonth = 1, int numberOfMonths = 10)
    {
      if (IsViewMode(html))
      {
        return html.TextBox(controlName, string.Format("{0}-{1}", provisionalBillingYear,
                                                                 CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(provisionalBillingMonth)));
      }

      var billingMonthYearSelectList = new List<SelectListItem>();
      string selectedMonthYear = string.Empty;

      if (provisionalBillingMonth != 0 && provisionalBillingYear != 0)
      {
        selectedMonthYear = string.Format("{0}-{1}", provisionalBillingYear, provisionalBillingMonth);
      }

      // Calculate the drop-down values only if it is not view mode.
      if (!html.IsViewMode())
      {
        // Get current billing year-month - 4 year-months and 2 months before that
        var currentBillingPeriod = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager)).GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich); //GetCurrentBillingPeriod();

        var validProvBillingMonth = new DateTime(currentBillingPeriod.Year, currentBillingPeriod.Month, 1).AddMonths(-validProvMonth);

        for (var i = 0; i < numberOfMonths; i++)
        {
          var dateTime = validProvBillingMonth.AddMonths(-i);
          var month = dateTime.ToString(FormatConstants.MonthNameFormat);
          var itemValue = string.Format("{0}-{1}", dateTime.Year, dateTime.Month);

          billingMonthYearSelectList.Add(new SelectListItem
          {
            Text = string.Format("{0}-{1}", dateTime.Year, month),
            Value = itemValue,
            Selected = itemValue == selectedMonthYear
          });
        }

        return html.DropDownList(controlName, billingMonthYearSelectList, PleaseSelectText);
      }

      // Show a readonly textbox in case of view mode.
      return html.TextBox(controlName, selectedMonthYear, new { @readonly = "readonly" });
    }

    /// <summary>
    /// Creates a provisional billing month dropdown which contains current year-month + previous 12 year-month combinations by default.
    /// Use for Search.
    /// </summary>
    /// <param name="html">The calling html object</param>
    /// <param name="controlName">Dropdown name</param>
    /// <param name="provisionalBillingYear">Provisional Billing Year from Model</param>
    /// <param name="provisionalBillingMonth">Provisional Billing Month from Model</param>
    /// <param name="numberOfMonths">Number of months to be shown e.g. current month and previous 12 months</param>
    /// <returns></returns>
    public static MvcHtmlString ProvisionalBillingMonthFormCDropdownList(this HtmlHelper html, string controlName, int provisionalBillingYear, int provisionalBillingMonth, int numberOfMonths = 13)
    {
      var selectedMonthYear = string.Empty;
      var bilingYearMonthSelectList = new List<SelectListItem>();

      if (provisionalBillingYear != 0 && provisionalBillingMonth != 0)
      {
        selectedMonthYear = string.Format("{0}-{1}", provisionalBillingYear, provisionalBillingMonth);
      }

      // Calculate the drop-down values only if it is not view mode.
      if (!html.IsViewMode())
      {
        var currentBillingPeriod = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager)).GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich); //GetCurrentBillingPeriod();
        var provBillingMonth = new DateTime(currentBillingPeriod.Year, currentBillingPeriod.Month, 1);

        for (var i = 0; i < numberOfMonths; i++)
        {
          var previousMonth = provBillingMonth.AddMonths(-i);
          var month = previousMonth.ToString(FormatConstants.MonthNameFormat);
          var year = previousMonth.Year;
          var monthValue = previousMonth.Month;

          bilingYearMonthSelectList.Add(new SelectListItem
          {
            Text = string.Format("{0}-{1}", year, month),
            Value = string.Format("{0}-{1}", year, monthValue),
            Selected = string.Format("{0}-{1}", year, monthValue) == selectedMonthYear
          });
        }

        return html.DropDownList(controlName, bilingYearMonthSelectList, PleaseSelectText);
      }

      // Show a readonly textbox in case of view mode.
      return html.TextBox(controlName, selectedMonthYear, new { @readonly = "readonly" });
    }

    /// <summary>
    /// Shows the current year-month + specified number of previous year-months.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="name">Dropdown name</param>
    /// <param name="year">Year to select</param>
    /// <param name="month">Month to select</param>
    /// <param name="previousYearMonths">The previous year months.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns>HTML for the dropdown</returns>
    public static MvcHtmlString BilledInDropdown(this HtmlHelper html, string name, int year, int month, int previousYearMonths = 12, object htmlAttributes = null)
    {
      var bilingMonthSelectList = new List<SelectListItem>();

      var calendarManager = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager));
      var currentBillingPeriod = calendarManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);  //GetCurrentBillingPeriod();

      var selectedYearMonth = string.Format("{0}-{1}", year, month);
      var currentBillingMonth = new DateTime(currentBillingPeriod.Year, currentBillingPeriod.Month, 1);

      for (var i = 0; i < previousYearMonths; i++)
      {
        var strmonth = currentBillingMonth.AddMonths(-i).ToString("MMM");
        var stryear = currentBillingMonth.AddMonths(-i).Year.ToString();
        var monthValue = currentBillingMonth.AddMonths(-i).Month.ToString();

        bilingMonthSelectList.Add(new SelectListItem
                                    {
                                      Text = string.Format("{0}-{1}", stryear, strmonth),
                                      Value = string.Format("{0}-{1}", stryear, monthValue),
                                      Selected = string.Format("{0}-{1}", stryear, monthValue) == selectedYearMonth
                                    });
      }

      return html.DropDownList(name, bilingMonthSelectList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Invoices the year dropdown list for.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString InvoiceYearDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
    {
      //var invoiceYearList = Ioc.Resolve<IProcessingDashboardManager>(typeof(IProcessingDashboardManager)).GetAllInvoiceYear();

      var invoiceYearList = Ioc.Resolve<IProcessingDashboardManager>(typeof(IProcessingDashboardManager)).GetCalendarYear();

      var invoiceYearSelectList = invoiceYearList.Select(invoiceYear => new SelectListItem
                                                                                           {
                                                                                             Text = invoiceYear.ToString(),
                                                                                             Value = invoiceYear.ToString()
                                                                                           }).ToList();

      return html.DropDownListFor(expression, invoiceYearSelectList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Used in Payable search.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="name">Name of the dropdown control.</param>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    /// <param name="numberOfMonths">The number of months.</param>
    /// <returns></returns>
    public static MvcHtmlString BillingYearMonthDropdownForPayables(this HtmlHelper html, string name, int year, int month, int numberOfMonths = 12)
    {
      var bilingMonthSelectList = new List<SelectListItem>();

      var lastClosedBillingPeriod = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager)).GetLastClosedBillingPeriod();

      var selectedPeriod = year != 0 && month != 0 ? string.Format("{0}-{1}", year, month) : string.Format("{0}-{1}", lastClosedBillingPeriod.Year, lastClosedBillingPeriod.Month);

      var lastClosedPeriodDate = new DateTime(lastClosedBillingPeriod.Year, lastClosedBillingPeriod.Month, 1);






      for (var i = 0; i < numberOfMonths; i++)
      {
        var strmonth = lastClosedPeriodDate.AddMonths(-i).ToString(FormatConstants.MonthNameFormat);
        var stryear = lastClosedPeriodDate.AddMonths(-i).Year.ToString();
        var monthValue = lastClosedPeriodDate.AddMonths(-i).Month.ToString();

        bilingMonthSelectList.Add(new SelectListItem
        {
          Text = string.Format("{0}-{1}", stryear, strmonth),
          Value = string.Format("{0}-{1}", stryear, monthValue),
          Selected = string.Format("{0}-{1}", stryear, monthValue) == selectedPeriod
        });
      }

      return html.DropDownList(name, bilingMonthSelectList, PleaseSelectText);
    }
    /// <summary>
    ///  this method for year-month-period dropdownlist on validation Error correction Screen 
    /// </summary>
    /// <param name="html"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static MvcHtmlString BillingYearMonthPeriodDropdownListValidationErrorCorrection(this HtmlHelper html, string name, string month, string period)
    {
      var calendarManager = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager));

      var billingPeriods = calendarManager.GetRelevantBillingPeriods(string.Empty, ClearingHouse.Ich, 0);
      var currentBillingPeriod = calendarManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);

      var selectedPeriod = string.IsNullOrEmpty(month)
                                ? string.Format("{0}-{1}-{2}", currentBillingPeriod.Year, currentBillingPeriod.Month, currentBillingPeriod.Period)
                                : string.Format("{0}-{1}", month, period);

      // more lines added by me
      if (billingPeriods.Count == 2)
      {
        if (billingPeriods[0].Month == billingPeriods[1].Month)
        {
          billingPeriods.RemoveAt(1);
        }
      }
      // up to here 
      var billingPeriodSelectList = billingPeriods.Select(billingPeriod => new SelectListItem
      {
        Text =
          string.Format("{0}-{1}-{2}",
                        billingPeriod.Year,
                        CultureInfo.CurrentCulture.DateTimeFormat.
                          GetAbbreviatedMonthName(billingPeriod.Month),
                        billingPeriod.Period),
        Value =
          string.Format("{0}-{1}-{2}",
                        billingPeriod.Year,
                        billingPeriod.Month,
                        billingPeriod.Period),
        Selected =
          string.Format("{0}-{1}-{2}",
                        billingPeriod.Year,
                        billingPeriod.Month,
                        billingPeriod.Period) == selectedPeriod
      });

      return html.DropDownList(name, billingPeriodSelectList);

    }

    // get Billing Month Data
    /// <summary>
    /// Billings the month dropdown list.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="name">The name.</param>
    /// <param name="selectedMonth">The selected month.</param>
    /// <returns></returns>
    public static MvcHtmlString BillingMonthDropdownList(this HtmlHelper html, string name, int selectedMonth)
    {

      if (IsViewMode(html))
      {
        string monthName = string.Empty;
        if (selectedMonth > 0 && selectedMonth < 13) monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(selectedMonth);

        return html.TextBox(name, monthName);
      }

      var bilingMonthSelectList = new List<SelectListItem>();

      var currentBillingPeriod = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager)).GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich); // GetCurrentBillingPeriod();

      // Convert DateTime format of CurrentBillingPeriod
      var currentBillingInDateTimeFormat = new DateTime(currentBillingPeriod.Year, currentBillingPeriod.Month, currentBillingPeriod.Period);
      for (var i = 0; i < 12; i++)
      {
        var month = currentBillingInDateTimeFormat.AddMonths(-i).ToString(FormatConstants.MonthNameFormat);
        var monthValue = currentBillingInDateTimeFormat.AddMonths(-i).Month.ToString();
        bilingMonthSelectList.Add(new SelectListItem
                                    {
                                      Text = month,
                                      Value = monthValue,
                                      Selected = monthValue == selectedMonth.ToString()
                                    });
      }

      return html.DropDownList(name, bilingMonthSelectList, PleaseSelectText);
    }
    /// <summary>
    /// This will return Comparison Period (Period Or Month) 
    /// </summary>
    /// <param name="html"></param>
    /// <param name="controlName"></param>
    /// <param name="selectedMonth"></param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    public static MvcHtmlString ComparisonPeriodDropdownList(this HtmlHelper html, string controlName, int selectedMonth, object htmlAttributes = null)
    {

      // Get Billing Period Data
      var comparisonPeriodSelectList = new List<SelectListItem>();
      comparisonPeriodSelectList.Add(new SelectListItem
                              {
                                Text = "Period",
                                Value = "0",
                                Selected = (1 == selectedMonth)
                              });
      comparisonPeriodSelectList.Add(new SelectListItem
                                {
                                  Text = "Month",
                                  Value = "1",
                                  Selected = (1 == selectedMonth)
                                });

      return html.DropDownList(controlName, comparisonPeriodSelectList, PleaseSelectText, htmlAttributes);
    }

    public static string DropdownList(this HtmlHelper helper, string controlId, List<SelectListItem> select, string controlwidth = null)
    {
      var sb = new StringBuilder();

      if (string.IsNullOrEmpty(controlwidth))
        sb.AppendFormat("<select id={0} name={0}>", controlId);
      else
        sb.AppendFormat("<select id={0} name={0} style='Width:{1}px;'>", controlId, controlwidth);

      foreach (var item in select)
      {
        sb.AppendFormat("<option value='{0}' {2}>{1}</option>", item.Value, item.Text, (item.Selected ? "selected" : ""));
      }
      sb.Append("</select>");
      return sb.ToString();
    }

    ///// <summary>
    ///// Returns Rejection stage dropdown.
    ///// </summary>
    ///// <typeparam name="TModel"></typeparam>
    ///// <typeparam name="TValue"></typeparam>
    ///// <param name="html"></param>
    ///// <param name="expression"></param>
    ///// <param name="htmlAttributes">HTML attributes for dropdown.</param>
    ///// <returns></returns>
    //public static MvcHtmlString StaticRejectionStageDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
    //{
    //  var bilingPeriodSelectList = new List<SelectListItem>
    //                                 {
    //                                   new SelectListItem
    //                                     {
    //                                       Text = 1.ToString(),
    //                                       Value = 1.ToString()
    //                                     },
    //                                   new SelectListItem
    //                                     {
    //                                       Text = 2.ToString(),
    //                                       Value = 2.ToString()
    //                                     }
    //                                 };

    //  return html.DropDownListFor(expression, bilingPeriodSelectList, PleaseSelectText, htmlAttributes);
    //}

    ///// <summary>
    ///// Function to return  Rejection Memo Stage 
    ///// </summary>
    ///// <typeparam name="TModel"></typeparam>
    ///// <typeparam name="TValue"></typeparam>
    ///// <param name="html"></param>
    ///// <param name="expression"></param>
    ///// <returns></returns>
    //public static MvcHtmlString RejectionMemoStageDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
    //{
    //  var rejectionMemoSelectList = EnumMapper.GetRejectionMemoStageList();
    //  rejectionMemoSelectList.Insert(0, PleaseSelectIntListItem);

    //  return html.DropDownListFor(expression, rejectionMemoSelectList);
    //}

    ///// <summary>
    ///// Function to return  Rejection Memo Stage 
    ///// </summary>
    ///// <typeparam name="TModel"></typeparam>
    ///// <typeparam name="TValue"></typeparam>
    ///// <param name="html"></param>
    ///// <param name="expression"></param>
    ///// <returns></returns>
    //public static MvcHtmlString RejectionMemoStageDropdownListForBillingHistory<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
    //{
    //  List<SelectListItem> rejectionMemoSelectList = EnumMapper.GetRejectionMemoStageList();
    //  rejectionMemoSelectList.Insert(0, PleaseSelectListItem);

    //  return html.DropDownListFor(expression, rejectionMemoSelectList);
    //}

    ///// <summary>
    ///// Function to return  Rejection Memo Stage 
    ///// </summary>
    ///// <typeparam name="TModel"></typeparam>
    ///// <typeparam name="TValue"></typeparam>
    ///// <param name="html"></param>
    ///// <param name="expression"></param>
    ///// <returns></returns>
    //public static MvcHtmlString MiscRejectionMemoStageDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
    //{
    //  List<SelectListItem> rejectionMemoSelectList = EnumMapper.GetMiscRejectionMemoStageList();
    //  rejectionMemoSelectList.Insert(0, PleaseSelectIntListItem);

    //  return html.DropDownListFor(expression, rejectionMemoSelectList);
    //}

    /// <summary>
    /// Function to return  Rejection Memo Stage 
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="mode" />
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    public static MvcHtmlString RejectionStageDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, TransactionMode mode = TransactionMode.Transactions, object htmlAttributes = null)
    {
      List<SelectListItem> rejectionMemoSelectList = null;

      switch (mode)
      {
        case TransactionMode.BillingHistory:
        case TransactionMode.InvoiceSearch:
          rejectionMemoSelectList = EnumMapper.GetRejectionMemoStageList();
          rejectionMemoSelectList.Insert(0, PleaseSelectListItem);
          break;
        case TransactionMode.MiscUatpInvoiceSearch:
          rejectionMemoSelectList = EnumMapper.GetMiscRejectionMemoStageList();
          rejectionMemoSelectList.Insert(0, PleaseSelectIntListItem);
          break;
        case TransactionMode.MiscUatpInvoice:
          rejectionMemoSelectList = EnumMapper.GetMiscRejectionMemoStageList();
          rejectionMemoSelectList.Insert(0, PleaseSelectListItem);
          break;
        case TransactionMode.Transactions:
          rejectionMemoSelectList = EnumMapper.GetRejectionMemoStageList();
          rejectionMemoSelectList.Insert(0, PleaseSelectIntListItem);
          break;
      }

      return html.DropDownListFor(expression, rejectionMemoSelectList, htmlAttributes);
    }

    public static MvcHtmlString RejectionStageDropdownList(this HtmlHelper html, string controlId, int controlvalue, TransactionMode mode = TransactionMode.Transactions, object htmlAttributes = null)
    {
      var rejectionMemoSelectList = new List<SelectListItem>();

      switch (mode)
      {
        case TransactionMode.BillingHistory:
        case TransactionMode.InvoiceSearch:
          rejectionMemoSelectList = EnumMapper.GetRejectionMemoStageList();
          break;
        case TransactionMode.MiscUatpInvoiceSearch:
          rejectionMemoSelectList = EnumMapper.GetMiscRejectionMemoStageList();
          break;
        case TransactionMode.MiscUatpInvoice:
          rejectionMemoSelectList = EnumMapper.GetMiscRejectionMemoStageList();
          break;
        case TransactionMode.Transactions:
          rejectionMemoSelectList = EnumMapper.GetRejectionMemoStageList();
          break;
      }

      if (IsViewMode(html))
      {
        var rejectionMemoSelected = rejectionMemoSelectList.FirstOrDefault(rm => (rm.Value == controlvalue.ToString()));
        var displayText = rejectionMemoSelected == null ? string.Empty : rejectionMemoSelected.Text;
        return html.TextBox(controlId, displayText, htmlAttributes);
      }

      rejectionMemoSelectList.Insert(0, PleaseSelectListItem);

      return html.DropDownList(controlId, rejectionMemoSelectList, htmlAttributes);
    }

    /// <summary>
    /// Creates Member Location dropdown and sets its selected value.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="controlId">The control id.</param>
    /// <param name="controlvalue">The control value.</param>
    /// <param name="memberId">The member id</param>
    /// <param name="billingCategory">The billing category of invoice</param>
    /// <param name="memberType">member type either is billed / billing</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <param name="defaultLocation">The default location.</param>
    /// <param name="isForEdit"></param>
    /// <param name="submittedMemLocInfo"></param>
    /// <returns></returns>
    public static MvcHtmlString MemberLocationIdDropdownList(this HtmlHelper html,
                                                             string controlId,
                                                             string controlvalue,
                                                             int memberId,
                                                             BillingCategoryType billingCategory,
                                                             MemberType memberType,
                                                             object htmlAttributes = null,
                                                             string defaultLocation = DefaultLocationCode,
                                                             bool isForEdit = false,
                                                             MemberLocationInformation submittedMemLocInfo = null)
    {
      if (IsViewMode(html))
      {
        //In view mode, member location information details in inv_Mem_loc_info table should be displayed.
        var displayText = submittedMemLocInfo == null
                            ? string.Empty
                            : string.IsNullOrEmpty(submittedMemLocInfo.MemberLocationCode) ? string.Empty : string.Format("{0}-{1}-{2}", submittedMemLocInfo.MemberLocationCode, submittedMemLocInfo.CityName, submittedMemLocInfo.CountryCode);
        return html.TextBox(controlId, displayText, htmlAttributes);
      }
      else
      {
        const string uatpString = "UATP";
        var memberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));
        var memberContact = Ioc.Resolve<IManageContactsManager>(typeof(IManageContactsManager));

        //CMP #655: IS-WEB Display per Location ID
        var locationList = new List<Location>();
        if (billingCategory.Equals(BillingCategoryType.Misc) && memberId == SessionUtil.MemberId)
        {
            var memberAssociationLoc = memberContact.GetMemberAssociationLocForDropdown(SessionUtil.UserId, memberId);
            locationList.AddRange(memberAssociationLoc.Select(item => new Location
                                                                          {
                                                                              Currency = item.CurrencyCodeNum == null ? null : new Currency
                                                                                                                                   {
                                                                                                                                       Code = item.CurrencyCodeAlfa
                                                                                                                                   }, LocationCode = item.LocationCode, CityName = item.CityName, CountryId = item.CountryCode
                                                                          }));
        }
        else
        {
           var memberLocationList = memberManager.GetMemberLocationDetailsForDropdown(memberId, true);
           foreach (var item in memberLocationList)
           {
               var location = new Location();
               if (item.Currency != null)
               {
                   location.Currency = item.Currency;
                   location.Currency.Code = item.Currency.Code;
               }               
               location.LocationCode = item.LocationCode;
               location.CityName = item.CityName;
               location.CountryId = item.CountryId;
               locationList.Add(location);
           }
          // locationList = memberLocationList.Select(loc => new { loc.Currency, loc.LocationCode, loc.CityName, loc.CountryId, loc.Currency.Code }).ToList();
        }
        
        // Get only required properties of Member location.
       

        if (isForEdit)
        {
          defaultLocation = string.Empty;
        }
        else if (defaultLocation.Equals(uatpString))
        {
          var memberDefaultLocationExists = memberManager.MemberDefaultLocationExists(memberId, uatpString);
          defaultLocation = memberDefaultLocationExists ? defaultLocation : DefaultLocationCode;
        }

        //SCP # 48139 - Member Location ID incorrect order 
        if (locationList.Count() > 0)
        {
          var intLocationList = locationList.Where(l => Regex.IsMatch(l.LocationCode, @"^[0-9]+$")).OrderBy(lst => int.Parse(lst.LocationCode)).ToList();
          var stringLocationList = locationList.Where(l => !Regex.IsMatch(l.LocationCode, @"^[0-9]+$")).OrderBy(lst => lst.LocationCode).ToList();

          stringLocationList.AddRange(intLocationList);

          if (billingCategory.Equals(BillingCategoryType.Misc) && memberId == SessionUtil.MemberId)
          {
              if (stringLocationList.Count > 0)
              {
                  defaultLocation = stringLocationList[0].LocationCode;
              }
          }


          var memberLocationSelectListSorted = stringLocationList.Select(memberLocation => new SelectListItem
          {
            Text = memberLocation.Currency != null ?
              string.Format("{0}-{1}-{2}-{3}",
                            memberLocation.LocationCode,
                            memberLocation.CityName,
                            memberLocation.CountryId, memberLocation.Currency.Code) : string.Format("{0}-{1}-{2}",
                            memberLocation.LocationCode,
                            memberLocation.CityName,
                            memberLocation.CountryId),
            Value = memberLocation.LocationCode.ToString(),
            Selected = (memberLocation.LocationCode == (controlvalue ?? defaultLocation))
          }).ToList();

          //CMP496: before return dropdown check for billed member and remove first empty value.
          //This changes will not impact billing member as well as UATP billing and Billed member. 
          if (memberType.Equals(MemberType.Billed) && !billingCategory.Equals(BillingCategoryType.Uatp))
          {
              return html.DropDownList(controlId, memberLocationSelectListSorted, htmlAttributes);
          }
          
            //CMP #655: IS-WEB Display per Location ID
            return memberType.Equals(MemberType.Billing) && billingCategory.Equals(BillingCategoryType.Misc)
                       ? html.DropDownList(controlId, memberLocationSelectListSorted, htmlAttributes)
                       : html.DropDownList(controlId, memberLocationSelectListSorted, string.Empty, htmlAttributes);
        }


        var memberLocationSelectList = locationList.Select(memberLocation => new SelectListItem
        {
          Text = memberLocation.Currency != null ?
            string.Format("{0}-{1}-{2}-{3}",
                          memberLocation.LocationCode,
                          memberLocation.CityName,
                          memberLocation.CountryId, memberLocation.Currency.Code) : string.Format("{0}-{1}-{2}",
                          memberLocation.LocationCode,
                          memberLocation.CityName,
                          memberLocation.CountryId),
          Value = memberLocation.LocationCode.ToString(),
          Selected = (memberLocation.LocationCode == (controlvalue ?? defaultLocation))
        }).ToList();
          
        //CMP496: before return dropdown check for billed member and remove first empty value.
        //This changes will not impact billing member as well as UATP billing and Billed member. 
        if (memberType.Equals(MemberType.Billed) && !billingCategory.Equals(BillingCategoryType.Uatp))
        {
            return html.DropDownList(controlId, memberLocationSelectList, htmlAttributes);
        }

        //CMP #655: IS-WEB Display per Location ID
        return memberType.Equals(MemberType.Billing) && billingCategory.Equals(BillingCategoryType.Misc)
                   ? html.DropDownList(controlId, memberLocationSelectList, htmlAttributes)
                   : html.DropDownList(controlId, memberLocationSelectList, string.Empty, htmlAttributes);

      }
    }

    /// <summary>
    /// Creates Member Location dropdown and sets its selected value.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="memberId">The member id</param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    public static MvcHtmlString LocationIdDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, int memberId, object htmlAttributes = null)
    {
      var memberLocationList = Ioc.Resolve<IMemberManager>(typeof(IMemberManager)).GetMemberLocationList(memberId, false);
      var memberLocationSelectList = memberLocationList.Select(memberLocation => new SelectListItem
                                                                                     {
                                                                                       Text = memberLocation.LocationCode.ToString(),
                                                                                       Value = memberLocation.Id.ToString()
                                                                                     }).ToList();
      // Date: 18-06-2012
      // Modified By: Sachin Pharande
      // Action: Sorting code is moved to following location.
      // File: Iata.IS.Business.MemberProfile.Impl.MemberManager
      // Method: GetMemberLocationList()
      // Reason: Not able to display the Location codes in sorted order after partial page (tab) load on Location tab of member profile screen.
      // Issue: SCP ID : 23155 - Changing locations becomes adding location.

      return html.DropDownListFor(expression, memberLocationSelectList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Creates Member Location dropdown and sets its selected value.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="memberId">The member id</param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    public static MvcHtmlString LocationIdDropdownList(this HtmlHelper html, string controlId, string selectedValue, int memberId, IDictionary<string, object> htmlAttributes = null)
    {
      var memberLocationList = Ioc.Resolve<IMemberManager>(typeof(IMemberManager)).GetMemberLocationList(memberId);

      var memberLocationSelectList = memberLocationList.Select(memberLocation => new SelectListItem
                                                                                     {
                                                                                       Text = memberLocation.LocationCode.ToString(),
                                                                                       Value = memberLocation.Id.ToString(),
                                                                                       Selected = (memberLocation.Id.ToString() == selectedValue)
                                                                                     }).ToList();
      // Date: 18-06-2012
      // Modified By: Sachin Pharande
      // Action: Sorting code is moved to following location.
      // File: Iata.IS.Business.MemberProfile.Impl.MemberManager
      // Method: GetMemberLocationList()
      // Reason: Not able to display the Location codes in sorted order after partial page (tab) load on Contacts tab of member profile screen.
      // Issue: SCP ID : 23155 - Changing locations becomes adding location.

      return html.DropDownList(controlId, memberLocationSelectList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Creates member contact dropdown
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="memberId">The member id</param>
    /// <param name="controlId"></param>
    /// <returns></returns>
    public static MvcHtmlString MemberContactsDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, int memberId, string controlId)
    {
      var memberContactList = Ioc.Resolve<IMemberManager>(typeof(IMemberManager)).GetMemberContactList(memberId);

      var memberContactSelectList = memberContactList.Select(memberContacts => new SelectListItem
                                                                                   {
                                                                                     Text =
                                                                                         string.Format(
                                                                                             "{0} {1}",
                                                                                             memberContacts.
                                                                                                 FirstName,
                                                                                             memberContacts.
                                                                                                 LastName
                                                                                             ),
                                                                                     Value =
                                                                                         memberContacts.Id.
                                                                                         ToString()
                                                                                   }).ToList().OrderBy(
                                                                                       f => f.Text);

      return html.DropDownListFor(expression,
                                  memberContactSelectList,
                                  string.Empty,
                                  new
                                    {
                                      @Id = controlId
                                    });
    }


    public static MvcHtmlString MemberEmailsDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, int memberId, string controlId)
    {
      var memberContactList = Ioc.Resolve<IMemberManager>(typeof(IMemberManager)).GetMemberContactList(memberId);

      var memberContactSelectList = memberContactList.Select(memberContacts => new SelectListItem
      {
        Text =
            string.Format(
                "{0}",
                 memberContacts.EmailAddress),
        Value =
            memberContacts.Id.
            ToString()
      }).ToList().OrderBy(
                                                                                       f => f.Text);

      return html.DropDownListFor(expression,
                                  memberContactSelectList,
                                  string.Empty,
                                  new
                                  {
                                    @Id = controlId
                                  });
    }
    /// <summary>
    /// Creates all member contact names dropdown
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    public static MvcHtmlString MemberAllContactNamesDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
    {
      var memberContactList = Ioc.Resolve<IMemberManager>(typeof(IMemberManager)).GetAllMemberContactList();

      var memberContactSelectList = memberContactList.Select(memberContacts => new SelectListItem
                                                                                                  {
                                                                                                    Text = string.Format("{0}", memberContacts.Name),
                                                                                                    Value = memberContacts.Id.ToString()
                                                                                                  }).ToList();

      return html.DropDownListFor(expression, memberContactSelectList, PleaseSelectText);
    }

    /// <summary>
    /// Creates all member contact email addresses dropdown
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    public static MvcHtmlString MemberAllContactEmailsDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
    {
      var memberContactList = Ioc.Resolve<IMemberManager>(typeof(IMemberManager)).GetAllMemberContactList();

      var memberContactSelectList = memberContactList.Select(memberContacts => new SelectListItem
                                                                                                  {
                                                                                                    Text = string.Format("{0}", memberContacts.EmailAddress),
                                                                                                    Value = memberContacts.Id.ToString()
                                                                                                  }).ToList();

      return html.DropDownListFor(expression, memberContactSelectList, PleaseSelectText);
    }

    public static MvcHtmlString HandlingFeeTypeDropdownList(this HtmlHelper html, string controlId, string controlValue)
    {

      var handlingFeeTypeList = EnumMapper.GetHandlingFeeTypeList();

      var selectedItem = handlingFeeTypeList.SingleOrDefault(item => item.Value == controlValue);

      if (IsViewMode(html))
      {
        return html.TextBox(controlId, selectedItem == null ? string.Empty : selectedItem.Text);
      }

      if (selectedItem != null) selectedItem.Selected = true;

      return html.DropDownList(controlId, handlingFeeTypeList, HandlingFeeTypeOther);
    }

    /// <summary>
    /// Creates Settlement Method dropdown and sets its selected value.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="invoiceType"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static MvcHtmlString SettlementMethodDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, InvoiceType invoiceType, TransactionMode mode = TransactionMode.InvoiceSearch)
    {
      List<SelectListItem> settlementMethodList;
      if (mode == TransactionMode.InvoiceSearch || mode == TransactionMode.MiscUatpInvoiceSearch)
      {
        settlementMethodList = EnumMapper.GetSettlementMethodList();

        ////to remove the form C related entry from SMI if MISC search page is.
        if (settlementMethodList.Count >= 6)
          settlementMethodList.RemoveAt(5);

        settlementMethodList.Insert(0, AllListItem);

        return html.DropDownListFor(expression, settlementMethodList);
      }
      //CMP529 : Daily Output Generation for MISC Bilateral Invoices
      if (mode == TransactionMode.DailyMiscInvSearch)
      {
        settlementMethodList = EnumMapper.GetBilateralSettlementMethodList();
        settlementMethodList.Insert(0, AllListItem);
        return html.DropDownListFor(expression, settlementMethodList);
      }

      settlementMethodList = EnumMapper.GetSettlementMethodList(invoiceType);
      return html.DropDownListFor(expression, settlementMethodList, PleaseSelectText);
    }

    /* CMP #624: ICH Rewrite-New SMI X 
     * Description: Added parameter to bypass SMI X in case of Pax sampling invoice screens */
    public static MvcHtmlString SettlementMethodDropdownList(this HtmlHelper html, string controlId, string displayText, InvoiceType invoiceType, TransactionMode mode = TransactionMode.InvoiceSearch, bool isCalledFromSampling = false, object htmlAttributes = null)
    {
      if (IsViewMode(html))
      {
        return html.TextBox(controlId, displayText, htmlAttributes);
      }

      List<SelectListItem> settlementMethodList;
      SelectListItem selectedItem;
      if (mode == TransactionMode.InvoiceSearch || mode == TransactionMode.MiscUatpInvoiceSearch)
      {
        settlementMethodList = EnumMapper.GetSettlementMethodList();
        settlementMethodList.Insert(0, AllListItem);
        selectedItem = settlementMethodList.SingleOrDefault(item => item.Text == displayText);
        if (selectedItem != null) selectedItem.Selected = true;
        if (isCalledFromSampling)
        {
          /* CMP #624: ICH Rewrite-New SMI X 
          * Description: Smi X is removed in case of Pax Sampling */
          settlementMethodList.Remove(settlementMethodList.Where(smi => smi.Value == ((int)SettlementMethodValues.IchSpecialAgreement).ToString()).Single());
        }
        return html.DropDownList(controlId, settlementMethodList, htmlAttributes);
      }

      settlementMethodList = EnumMapper.GetSettlementMethodList(invoiceType);
      selectedItem = settlementMethodList.SingleOrDefault(item => item.Text == displayText);
      if (selectedItem != null) selectedItem.Selected = true;
      if (isCalledFromSampling)
      {
        /* CMP #624: ICH Rewrite-New SMI X 
        * Description: Smi X is removed in case of Pax Sampling */
        //SCP#311019 - FW: Form D/E creation
        var smiX = settlementMethodList.Where(smi => smi.Value == ((int)SettlementMethodValues.IchSpecialAgreement).ToString()).FirstOrDefault();
        if (smiX != null)
          settlementMethodList.Remove(settlementMethodList.Where(smi => smi.Value == ((int)SettlementMethodValues.IchSpecialAgreement).ToString()).FirstOrDefault());
      }
      return html.DropDownList(controlId, settlementMethodList, PleaseSelectText, htmlAttributes);
    }

    public static MvcHtmlString SettlementMethodDropdownList(this HtmlHelper html, string controlId, string controlValue, params int[] settlementMethodIds)
    {
      var settlementMethodList = EnumMapper.GetSettlementMethodList();

      if (controlValue != "0")
        settlementMethodList.SingleOrDefault(item => item.Value == controlValue).Selected = true;
      var settlementMethodSelectList = new List<SelectListItem>();
      foreach (var id in settlementMethodIds)
      {
        var id1 = id;
        var listItem = settlementMethodList.Single(item => item.Value == id1.ToString());
        settlementMethodSelectList.Add(listItem);
      }
      settlementMethodSelectList.Insert(0, AllListItem);

      return html.DropDownList(controlId, settlementMethodSelectList, AllListItem);
    }

    /// <summary>
    /// Coupon numbers dropdown list.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="controlId">The control id.</param>
    /// <param name="controlValue">The control value.</param>
    /// <returns></returns>
    public static MvcHtmlString CouponNumberDropdownList(this HtmlHelper html, string controlId, int controlValue)
    {
      if (IsViewMode(html))
      {
        return html.TextBox(controlId, controlValue);
      }

      var couponNumberList = new List<SelectListItem>
                               {
                                 new SelectListItem
                                   {
                                     Text = "1",
                                     Value = "1"
                                   },
                                 new SelectListItem
                                   {
                                     Text = "2",
                                     Value = "2"
                                   },
                                 new SelectListItem
                                   {
                                     Text = "3",
                                     Value = "3"
                                   },
                                 new SelectListItem
                                   {
                                     Text = "4",
                                     Value = "4"
                                   },
                                 new SelectListItem
                                   {
                                     Text = "9",
                                     Value = "9"
                                   },
                               };

      return html.DropDownList(controlId, couponNumberList, PleaseSelectText);
    }


    public static MvcHtmlString NonFimCouponNumberDropdownList(this HtmlHelper html, string controlId, int controlValue)
    {
      if (IsViewMode(html))
      {
        return html.TextBox(controlId, controlValue);
      }

      var couponNumberList = new List<SelectListItem>
                               {
                                 new SelectListItem
                                   {
                                     Text = "1",
                                     Value = "1"
                                   },
                                 new SelectListItem
                                   {
                                     Text = "2",
                                     Value = "2"
                                   },
                                 new SelectListItem
                                   {
                                     Text = "3",
                                     Value = "3"
                                   },
                                 new SelectListItem
                                   {
                                     Text = "4",
                                     Value = "4"
                                   },
                               };

      return html.DropDownList(controlId, couponNumberList, PleaseSelectText);
    }

    /// <summary>
    /// Creates Check Digit dropdown and sets its selected value.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    public static MvcHtmlString CheckDigitDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
    {
      var checkDigitList = new List<SelectListItem>
                             {
                               new SelectListItem
                                 {
                                   Text = "0",
                                   Value = "0"
                                 },
                               new SelectListItem
                                 {
                                   Text = "1",
                                   Value = "1"
                                 },
                               new SelectListItem
                                 {
                                   Text = "2",
                                   Value = "2"
                                 },
                               new SelectListItem
                                 {
                                   Text = "3",
                                   Value = "3"
                                 },
                               new SelectListItem
                                 {
                                   Text = "4",
                                   Value = "4"
                                 },
                               new SelectListItem
                                 {
                                   Text = "5",
                                   Value = "5"
                                 },
                               new SelectListItem
                                 {
                                   Text = "6",
                                   Value = "6"
                                 },
                               new SelectListItem
                                 {
                                   Text = "9",
                                   Value = "9"
                                 },
                             };

      return html.DropDownListFor(expression, checkDigitList, PleaseSelectText);
    }

    public static MvcHtmlString DigitalSignatureDropdownList(this HtmlHelper html, string controlId, string selectedValue, object htmlAttributes = null)
    {
      var digitalSignatureList = EnumMapper.GetDigitalSignatureList();
      var digitalSignature = digitalSignatureList.SingleOrDefault(item => item.Value == selectedValue);
      if (digitalSignature != null)
      {
        digitalSignature.Selected = true;
      }

      if (IsViewMode(html))
      {
        return html.TextBox(controlId, digitalSignature == null ? string.Empty : digitalSignature.Text, htmlAttributes);
      }

      //SCP471024: Failed Legal XML / PDF
      return html.DropDownList(controlId, digitalSignatureList);
    }

    /// <summary>
    /// Creates a dropdown for Nil Form C Indicator
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    public static MvcHtmlString NilFormCIndicatorDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
    {
      var nilFormCIndicatorList = new List<SelectListItem>
                                    {
                                      new SelectListItem
                                        {
                                          Text = "Yes",
                                          Value = "Y"
                                        },
                                      new SelectListItem
                                        {
                                          Text = "No",
                                          Value = "N",
                                          Selected = true
                                        },
                                    };

      return html.DropDownListFor(expression, nilFormCIndicatorList);
    }

    /// <summary>
    /// Creates country code dropdown and sets its selected value.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="htmlAttributes">HTML attributes for dropdown.</param>
    /// <returns></returns>
    public static MvcHtmlString CountryCodeDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes = null)
    {
      IList<Country> countryCodeList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetCountryList();

      List<SelectListItem> countrySelectList = countryCodeList.Select(countryCode => new SelectListItem
      {
        Text = countryCode.Name,
        Value = countryCode.Id.ToString()
      }).ToList();

      return html.DropDownListFor(expression, countrySelectList, PleaseSelectText, htmlAttributes);
    }

    public static MvcHtmlString CountryCodeDropdownListForICAO<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes = null)
    {
      //SCPID : 107323 - ICAO location code - Incorrect reference to country master
      IList<CountryIcao> countryIcaoList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetCountryIcaoList();

      var countrySelectList = countryIcaoList.Select(countryCode => new SelectListItem
      {
        Text = string.Format("{0} ({1})", countryCode.Name, countryCode.CountryCodeIcao),
        Value = countryCode.CountryCodeIcao
      }).ToList();

      return html.DropDownListFor(expression, countrySelectList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Countries the dropdown list.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="controlId">The control id.</param>
    /// <param name="selectedCountry">The selected country.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString CountryDropdownList(this HtmlHelper html, string controlId, string selectedCountry, object htmlAttributes = null)
    {
      var countryCodeList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetCountryList();

      var countrySelectList = countryCodeList.Select(countryCode => new SelectListItem
                                                                                       {
                                                                                         Text = countryCode.Name,
                                                                                         Value = countryCode.Id.ToString(),
                                                                                         Selected = (countryCode.Id.ToString() == selectedCountry)
                                                                                       }).ToList();

      return html.DropDownList(controlId, countrySelectList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Language the dropdown list.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="controlId">The control id.</param>
    /// <param name="selectedLanguage">The selected language.</param>
    /// <param name="IsReadOnly">if true then render control as label</param>
    /// <param name="IsReqForPdf">if true then show only pdf languages otherwise help langauges</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString LanguageDropdownList(this HtmlHelper html, string controlId, string selectedLanguage, bool IsReadOnly = false, bool IsReqForPdf = true, object htmlAttributes = null)
    {
      var languages = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetLanguageList();

      var languagesLst = IsReqForPdf
                             ? languages.Where(lang => lang.IsReqForPdf == true)
                             : languages.Where(lang => lang.IsReqForHelp == true);

      var languageSelectList = languagesLst.Select(lang => new SelectListItem
      {
        Text = lang.Language_Desc,
        Value = lang.Language_Code,
        Selected = (lang.Language_Code == selectedLanguage)
      }).ToList();

      if (IsReadOnly)
        return MvcHtmlString.Create(string.IsNullOrEmpty(selectedLanguage) ? string.Empty : languageSelectList.First(item => item.Value == selectedLanguage).Text);
      else
        return html.DropDownList(controlId, languageSelectList, string.Empty, htmlAttributes);
    }

    /// <summary>
    /// Countries the code dropdown list.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="controlId">The control id.</param>
    /// <param name="selectedCountry">The selected country.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString CountryCodeDropdownList(this HtmlHelper html, string controlId, string selectedCountry, IDictionary<string, object> htmlAttributes = null)
    {
      var countryCodeList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetCountryList();

      if (IsViewMode(html))
      {
        var countryCode = countryCodeList.FirstOrDefault(country => (country.Id == selectedCountry));
        return html.TextBox(controlId, countryCode == null ? string.Empty : countryCode.Name, htmlAttributes);
      }

      var countrySelectList = countryCodeList.Select(countryCode => new SelectListItem
                                                                                       {
                                                                                         Text = countryCode.Name,
                                                                                         Value = countryCode.Id,
                                                                                         Selected = (countryCode.Id == selectedCountry)
                                                                                       }).ToList();

      return html.DropDownList(controlId, countrySelectList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Creates sub division code dropdown and sets its selected value.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    public static MvcHtmlString SubDivisionCodeDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
    {
      var subDivisionCodeList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetSubDivisionList();

      var countrySelectList = subDivisionCodeList.Select(subDivisionCode => new SelectListItem
                                                                                               {
                                                                                                 Text = subDivisionCode.Name,
                                                                                                 Value = subDivisionCode.Code.ToString()
                                                                                               }).ToList();

      return html.DropDownListFor(expression, countrySelectList, PleaseSelectText);
    }

    /// <summary>
    /// Function to return  Billing Code 
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="isForSearch"></param>
    /// <returns></returns>
    public static MvcHtmlString BillingCodeDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, bool isForSearch = false)
    {
      var bilingCodeSelectList = EnumMapper.GetBillingCodeList();

      if (isForSearch)
      {
        bilingCodeSelectList.Insert(0, AllListItem);

        bilingCodeSelectList.RemoveAll(billingCodeItem => billingCodeItem.Value == Convert.ToInt32(BillingCode.SamplingFormC).ToString());
      }

      return html.DropDownListFor(expression, bilingCodeSelectList);
    }

    /// <summary>
    /// Function to Return Cargo Billing Code Dropdown list.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="isForSearch"></param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    public static MvcHtmlString StaticCgoBillingCodeDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html,
                                                                                   Expression<Func<TModel, TValue>> expression,
                                                                                   bool isForSearch = false,
                                                                                   object htmlAttributes = null)
    {
      var billingCodeSelectList = EnumMapper.GetCgoBillingCodeList();

      if (isForSearch)
      {
        billingCodeSelectList.Insert(0, AllListItem);

        return html.DropDownListFor(expression, billingCodeSelectList);
      }

      return html.DropDownListFor(expression, billingCodeSelectList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Correspondences the sub status dropdown list.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="controlId">The control id.</param>
    /// <param name="controlValue">The control value.</param>
    /// <param name="controlWidth">The control Width.</param>
    /// <returns></returns>
    public static MvcHtmlString CorrespondenceSubStatusDropdownList(this HtmlHelper html, string controlId, int controlValue, string controlWidth = null)
    {
      var correspondenceSubStatusSelectList = EnumMapper.GetPaxCorrespondenceSubStatusList();
      correspondenceSubStatusSelectList.Insert(0, AllListItem);

      var selectedValue = controlValue == 0 ? (int)CorrespondenceInitiatingMember.Self : controlValue;
      correspondenceSubStatusSelectList.SingleOrDefault(item => item.Value == selectedValue.ToString()).Selected = true;

      return MvcHtmlString.Create(DropdownList(html, controlId, correspondenceSubStatusSelectList, controlWidth));
    }
    /// <summary>
    /// This drop down list is used in correspondence report download search criteria
    /// </summary>
    /// <param name="html"></param>
    /// <param name="controlId"></param>
    /// <param name="controlValue"></param>
    /// <param name="controlWidth"></param>
    /// <returns></returns>
    public static MvcHtmlString CorrespondenceReportSubStatusDropdownList(this HtmlHelper html, string controlId, int controlValue, string controlWidth = null)
    {
      var correspondenceSubStatusSelectList = EnumMapper.GetPaxCorrespondenceSubStatusList();
      correspondenceSubStatusSelectList.Insert(0, AllListItem);
      var selectedValue = controlValue == 0 ? -1 : controlValue;
      correspondenceSubStatusSelectList.SingleOrDefault(item => item.Value == selectedValue.ToString()).Selected = true;

      return MvcHtmlString.Create(DropdownList(html, controlId, correspondenceSubStatusSelectList, controlWidth));
    }

    /// <summary>
    /// Correspondences the initiating member dropdown list.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="controlId">The control id.</param>
    /// <param name="controlValue">The control value.</param>
    /// <returns></returns>
    public static MvcHtmlString CorrespondenceInitiatingMemberDropdownList(this HtmlHelper html, string controlId, int? controlValue)
    {
      var correspondenceInitiatingMemberSelectList = EnumMapper.GetCorrespondenceInitiatingMemberList();

      var selectedValue = controlValue == 0 ? (int)CorrespondenceInitiatingMember.Self : controlValue;


      correspondenceInitiatingMemberSelectList.SingleOrDefault(item => item.Value == selectedValue.ToString()).Selected = true;

      return MvcHtmlString.Create(DropdownList(html, controlId, correspondenceInitiatingMemberSelectList));
    }

    /// <summary>
    /// This drop down list is used in correspondence report download search criteria
    /// </summary>
    /// <param name="html"></param>
    /// <param name="controlId"></param>
    /// <param name="controlValue"></param>
    /// <returns></returns>
    public static MvcHtmlString CorrespondenceReportInitiatingMemberDropdownList(this HtmlHelper html, string controlId, int? controlValue)
    {
      var correspondenceInitiatingMemberSelectList = EnumMapper.GetCorrespondenceInitiatingMemberList();

      var selectedValue = controlValue == 0 ? (int)CorrespondenceInitiatingMember.Either : controlValue;


      correspondenceInitiatingMemberSelectList.SingleOrDefault(item => item.Value == selectedValue.ToString()).Selected = true;

      return MvcHtmlString.Create(DropdownList(html, controlId, correspondenceInitiatingMemberSelectList));
    }

    /// <summary>
    /// Function which returns Invoice Status
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="isForSearch"></param>
    /// <returns></returns>
    public static MvcHtmlString InvoiceStatusDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, bool isForSearch)
    {
      var invoiceStatusList = EnumMapper.GetInvoiceStatusList();

      if (isForSearch)
      {
        invoiceStatusList.Insert(0, AllListItem);

        return html.DropDownListFor(expression, invoiceStatusList);
      }

      return html.DropDownListFor(expression, invoiceStatusList, PleaseSelectText);
    }

    /// <summary>
    /// Function which returns Invoice Status
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="isForSearch"></param>
    /// <returns></returns>
    public static MvcHtmlString CgoInvoiceStatusDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, bool isForSearch)
    {
      var invoiceStatusList = EnumMapper.GetCgoInvoiceStatusList();

      if (isForSearch)
      {
        invoiceStatusList.Insert(0, AllListItem);

        return html.DropDownListFor(expression, invoiceStatusList);
      }

      return html.DropDownListFor(expression, invoiceStatusList, PleaseSelectText);
    }

    /// <summary>
    /// Function which returns Invoice Status for Form C
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static MvcHtmlString InvoiceStatusFormCDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
    {
      var invoiceStatusList = EnumMapper.GetFormCInvoiceStatusList();

      invoiceStatusList.Insert(0, AllListItem);

      return html.DropDownListFor(expression, invoiceStatusList);
    }

      /// <summary>
      /// Function which returns submission Methods
      /// </summary>
      /// <typeparam name="TModel">The type of the model.</typeparam>
      /// <typeparam name="TValue">The type of the value.</typeparam>
      /// <param name="html">The HTML.</param>
      /// <param name="expression">The expression.</param>
      /// <param name="isForSearch">if set to <c>true</c> [is for search].</param>
      /// <param name="isForMiscUatp">if set to <c>true</c> [is for misc uatp].</param>
      /// <param name="isForCargo">if set to <c>true</c> [is for cargo].</param>
      /// <returns></returns>
      public static MvcHtmlString SubmissionMethodDropDownListFor<TModel, TValue>(this HtmlHelper<TModel> html,
                                                                                    Expression<Func<TModel, TValue>> expression,
                                                                                    bool isForSearch = false,
                                                                                    bool isForMiscUatp = false,
                                                                                    bool isForCargo = false)
    {
        var submissionMethodList = EnumMapper.GetSubmissionMethodList();

        if (isForMiscUatp)
        {
            var idecItem = submissionMethodList.Single(item => item.Text == EnumList.SubmissionMethodValues[SubmissionMethod.IsIdec]);
            submissionMethodList.Remove(idecItem);
            /* SCP#425722: Removing Auto-Billing Submission Method For Misc and Uatp */
            var autoBillingItem = submissionMethodList.SingleOrDefault(item => item.Value == "4");
            if(autoBillingItem != null)
                submissionMethodList.Remove(autoBillingItem);
        }
        if (isForCargo)
        {
            /* SCP#425722: Removing Auto-Billing Submission Method For Misc and Uatp */
            var autoBillingItem = submissionMethodList.SingleOrDefault(item => item.Value == "4");
            if (autoBillingItem != null)
                submissionMethodList.Remove(autoBillingItem);
        }

        submissionMethodList.Insert(0, AllListItem);

        return html.DropDownListFor(expression, submissionMethodList);
    }

    //CMP559 : Add Submission Method Column to Processing Dashboard
    public static MvcHtmlString InvoiceSubmissionMethodDropdownList(this HtmlHelper html, string fieldName, string fieldValue)
    {
      List<SelectListItem> lstSubmissionMethod = EnumMapper.GetInvoiceSubmissionMethodList();
      lstSubmissionMethod.Insert(0, AllListItem);
      fieldValue = fieldValue == "0" ? "-1" : fieldValue;
      lstSubmissionMethod.SingleOrDefault(i => i.Value == fieldValue).Selected = true;
      return MvcHtmlString.Create(DropdownList(html, fieldName, lstSubmissionMethod));
    }

    //CMP529 : Daily Output Generation for MISC Bilateral Invoices
    public static MvcHtmlString DailyDeliveryStatusDropdownList(this HtmlHelper html, string fieldName, string fieldValue)
    {
      List<SelectListItem> lstDailyDeliveryStatus = EnumMapper.GetDailyDeliveryStatusList();
      lstDailyDeliveryStatus.Insert(0, AllListItem);
      lstDailyDeliveryStatus.SingleOrDefault(i => i.Value == fieldValue).Selected = true;
      return MvcHtmlString.Create(DropdownList(html, fieldName, lstDailyDeliveryStatus));
    }

    public static MvcHtmlString InvoiceTypeDropDownListFor<TModel, TValue>(this HtmlHelper<TModel> html,
                                                                            Expression<Func<TModel, TValue>> expression,
                                                                            bool isForSearch = true,
                                                                            bool isForMiscUatp = true)
    {
      var invoiceTypeList = EnumMapper.GetInvoiceTypeList();

      if (!isForMiscUatp)
      {
        var rejectionInvoice = invoiceTypeList.Single(item => item.Text == EnumList.InvoiceTypeDictionary[InvoiceType.RejectionInvoice]);
        invoiceTypeList.Remove(rejectionInvoice);
      }

      invoiceTypeList.Insert(0, AllListItem);

      return html.DropDownListFor(expression, invoiceTypeList);
    }

    /// <summary>
    /// Returns Vat Identifier list.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="billingCategoryType">Type of the billing category.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString VatIdentifierListDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html,
                                                                                 Expression<Func<TModel, TValue>> expression,
                                                                                 BillingCategoryType billingCategoryType,
                                                                                 object htmlAttributes = null)
    {
      var vatIdentifierList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetVatIdentifierList(billingCategoryType);

      var vatIdentifierSelectList = vatIdentifierList.Select(vatIdentifier => new SelectListItem
                                                                                                 {
                                                                                                   Text = vatIdentifier.Description,
                                                                                                   Value = vatIdentifier.Id.ToString()
                                                                                                 }).ToList();

      return html.DropDownListFor(expression, vatIdentifierSelectList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Cgoes the vat identifier list dropdown list for.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="isOcApplicable">if set to <c>true</c> [is oc applicable].</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString CgoVatIdentifierListDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html,
                                                                                 Expression<Func<TModel, TValue>> expression,
                                                                                 bool? isOcApplicable = null,
                                                                                 object htmlAttributes = null)
    {
      var vatIdentifierList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetCgoVatIdentifierList(isOcApplicable);

      var vatIdentifierSelectList = vatIdentifierList.Select(vatIdentifier => new SelectListItem
      {
        Text = vatIdentifier.Description,
        Value = vatIdentifier.Id.ToString()
      }).ToList();

      return html.DropDownListFor(expression, vatIdentifierSelectList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Creates Rejection on Validation Failure dropdown and sets its selected value.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    public static MvcHtmlString RejectionOnValidatonFailureDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes = null)
    {
      var rejectionOnVAlidationFailureList = EnumMapper.GetRejectionOnValidationFailureList();

      rejectionOnVAlidationFailureList.Single(status => status.Value == ((int)RejectionOnValidationFailure.RejectInvoiceInError).ToString()).Selected = true;
      return html.DropDownListFor(expression, rejectionOnVAlidationFailureList, htmlAttributes);
    }

    /// <summary>
    /// Creates Rejection on Validation Failure dropdown and sets its selected value.
    /// </summary>
    /// <param name="selectedValue"></param>
    /// <param name="htmlAttributes">The HTML Attributes.</param>
    /// <param name="html"></param>
    /// <param name="controlId"></param>
    /// <returns></returns>
    public static MvcHtmlString RejectionOnValidatonFailureDropdownList(this HtmlHelper html, string controlId, string selectedValue, object htmlAttributes = null)
    {
      var rejectionOnVAlidationFailureList = EnumMapper.GetRejectionOnValidationFailureList();

      if (string.IsNullOrEmpty(selectedValue) || selectedValue == "0")
      {
        rejectionOnVAlidationFailureList.Single(status => status.Value == ((int)RejectionOnValidationFailure.RejectInvoiceInError).ToString()).Selected = true;
      }
      else
      {
        rejectionOnVAlidationFailureList.Single(status => status.Value == selectedValue).Selected = true;
      }
      return html.DropDownList(controlId, rejectionOnVAlidationFailureList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Creates Sampling Career Type dropdown and sets its selected value.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    public static MvcHtmlString SamplingCareerTypeDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
    {
      var samplingCareerTypeList = EnumMapper.GetSamplingCareerTypeList();

      samplingCareerTypeList.Single(carrierType => carrierType.Value == ((int)SamplingCarrierType.NotASamplingCarrier).ToString()).Selected = true;

      return html.DropDownListFor(expression, samplingCareerTypeList, htmlAttributes);
    }

    /// <summary>
    /// Creates Migration Status dropdown and sets its selected value.
    /// </summary>
    /// <typeparam name="controlId">dropdown control Id.</typeparam>
    /// <typeparam name="selectedValue">The type of the value.</typeparam>
    /// <param name="htmlAttributes">The HTML Attributes.</param>
    /// <param name="html"></param>
    /// <returns></returns>
    public static MvcHtmlString SamplingCareerTypeDropdownList(this HtmlHelper html, string controlId, string selectedValue, object htmlAttributes = null)
    {
      var samplingCareerTypeList = EnumMapper.GetSamplingCareerTypeList();

      if (string.IsNullOrEmpty(selectedValue) || (selectedValue == "0"))
      {
        samplingCareerTypeList.Single(carrierType => carrierType.Value == ((int)SamplingCarrierType.NotASamplingCarrier).ToString()).Selected = true;
      }
      else
      {
        samplingCareerTypeList.Single(carrierType => carrierType.Value == selectedValue).Selected = true;
      }

      return html.DropDownList(controlId, samplingCareerTypeList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Creates Migration Status dropdown and sets its selected value.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    public static MvcHtmlString MigrationStatusDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes = null)
    {
      var migrationStatusList = EnumMapper.GetMigrationStatusList();

      migrationStatusList.Single(status => status.Value == ((int)MigrationStatus.NotTested).ToString()).Selected = true;

      return html.DropDownListFor(expression, migrationStatusList, htmlAttributes);
    }

    /// <summary>
    /// Creates Migration Status dropdown and sets its selected value.
    /// </summary>
    /// <typeparam name="controlId">dropdown control Id.</typeparam>
    /// <typeparam name="selectedValue">The type of the value.</typeparam>
    /// <param name="htmlAttributes">The HTML Attributes.</param>
    /// <param name="html"></param>
    /// <returns></returns>
    public static MvcHtmlString MigrationStatusDropdownList(this HtmlHelper html, string controlId, string selectedValue, object htmlAttributes = null)
    {
      var migrationStatusList = EnumMapper.GetMigrationStatusList();

      if (string.IsNullOrEmpty(selectedValue) || selectedValue == "0")
      {
        migrationStatusList.Single(status => status.Value == ((int)MigrationStatus.NotTested).ToString()).Selected = true;
      }
      else
      {
        migrationStatusList.Single(status => status.Value == selectedValue).Selected = true;
      }
      return html.DropDownList(controlId, migrationStatusList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Creates ICH Membership Status dropdown and sets its selected value.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    public static MvcHtmlString IchMembershipStatusDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes = null)
    {
      var ichMemberStatusList = EnumMapper.GetIchMembershipStatusList();

      ichMemberStatusList.Single(status => status.Value == ((int)IchMemberShipStatus.NotAMember).ToString()).Selected = true;

      var dropdownContent = html.DropDownListFor(expression, ichMemberStatusList, htmlAttributes);

      return dropdownContent;
    }

    /// <summary>
    /// Iches the membership status dropdown list.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="controlId">The control id.</param>
    /// <param name="selectedValue">The selected value.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString IchMembershipStatusDropdownList(this HtmlHelper html, string controlId, string selectedValue, object htmlAttributes = null)
    {
      var statusList = EnumMapper.GetIchMembershipStatusList();

      if (string.IsNullOrEmpty(selectedValue) || selectedValue == "0")
      {
        statusList.Single(status => status.Value == ((int)IchMemberShipStatus.NotAMember).ToString()).Selected = true;
      }
      else
      {
        statusList.Single(status => status.Value == selectedValue).Selected = true;
      }
      return html.DropDownList(controlId, statusList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Creates ICH Zone dropdown and sets its selected value.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    public static MvcHtmlString IchZoneDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes = null)
    {
      var ichZoneList = EnumMapper.GetIchZoneList();

      if (htmlAttributes != null && htmlAttributes.ContainsKey("id"))
      {
        if (htmlAttributes["id"] == "ZoneTypeId")
        {
          ichZoneList.Insert(0, AllListItem);
          return html.DropDownListFor(expression, ichZoneList, htmlAttributes);
        }
        else if (htmlAttributes["id"] == "AchZoneTypeId")
        {
          htmlAttributes["id"] = "ZoneTypeId";
          var achZoneList = EnumMapper.GetIchZoneList().Where(item => item.Value == "3").ToList();

          return html.DropDownListFor(expression, achZoneList, htmlAttributes);
        }
        else if (htmlAttributes["id"] == "IchZoneId")
        {
          var ichZones = EnumList.IchZoneList.Select(zoneStatus => new SelectListItem
          {
            Value = Convert.ToString(zoneStatus.Key),
            Text = zoneStatus.Value
          }).ToList();

          return html.DropDownListFor(expression, ichZones, PleaseSelectText, htmlAttributes);
        }

      }

      return html.DropDownListFor(expression, ichZoneList, PleaseSelectText, htmlAttributes);

    }


    /// <summary>
    /// ICH zone dropdown list.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="controlId">Dropdown control id.</param>
    /// <param name="selectedValue">he type of the value.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString IchZoneDropdownList(this HtmlHelper html, string controlId, string selectedValue, object htmlAttributes = null)
    {
      var ichZoneList = EnumMapper.GetIchZoneList();

      if (!string.IsNullOrEmpty(selectedValue) && (selectedValue != "0"))
      {
        ichZoneList.Single(status => status.Value == selectedValue).Selected = true;
      }
      var x = html.DropDownList(controlId, ichZoneList, PleaseSelectText, htmlAttributes);

      return x;
    }

    /// <summary>
    /// Creates ICH Category dropdown and sets its selected value.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    public static MvcHtmlString IchCategoryDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes = null)
    {
      var ichCategoryList = EnumMapper.GetIchCategoryList();

      return html.DropDownListFor(expression, ichCategoryList, PleaseSelectText, htmlAttributes);
    }

    // <summary>
    /// Creates ICH Category dropdown and sets its selected value.
    /// </summary>
    /// <typeparam name="controlId">dropdown control Id.</typeparam>
    /// <typeparam name="selectedValue">The type of the value.</typeparam>
    /// <param name="htmlAttributes">The HTML Attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString IchCategoryDropdownList(this HtmlHelper html, string controlId, string selectedValue, object htmlAttributes = null)
    {
      var ichCategoryList = EnumMapper.GetIchCategoryList();

      if (!string.IsNullOrEmpty(selectedValue) && (selectedValue != "0"))
      {
        ichCategoryList.Single(status => status.Value == selectedValue).Selected = true;
      }
      return html.DropDownList(controlId, ichCategoryList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Creates ICH Category dropdown and sets its selected value.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    public static MvcHtmlString AggregatedTypeDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes = null)
    {
      var aggregatedTypeList = EnumMapper.GetaggregatedTypeList();

      return html.DropDownListFor(expression, aggregatedTypeList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Creates ICH Category dropdown and sets its selected value.
    /// </summary>
    /// <param name="selectedValue"></param>
    /// <param name="htmlAttributes">The HTML Attributes.</param>
    /// <param name="html"></param>
    /// <param name="controlId"></param>
    /// <returns></returns>
    public static MvcHtmlString AggregatedTypeDropdownList(this HtmlHelper html, string controlId, string selectedValue, object htmlAttributes = null)
    {
      var aggregatedTypeList = EnumMapper.GetaggregatedTypeList();

      if (!string.IsNullOrEmpty(selectedValue) && (selectedValue != "0"))
      {
        aggregatedTypeList.Single(type => type.Value == selectedValue).Selected = true;
      }
      return html.DropDownList(controlId, aggregatedTypeList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Creates ICH Web Report Options dropdown and sets its selected value.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    public static MvcHtmlString IchWebReportOptionsDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes = null)
    {
      var ichWebReportOptionsList = EnumMapper.GetIchWebReportList();

      return html.DropDownListFor(expression, ichWebReportOptionsList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Get Tax Code dropdown list
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    public static MvcHtmlString TaxCodeDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
    {
      // Get tax code Data
      var taxCodeList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetTaxCodeList();

      var taxCodeSelectList = taxCodeList.Select(taxCode => new SelectListItem
                                                                               {
                                                                                 Text = taxCode.Description,
                                                                                 Value = taxCode.Id.ToString()
                                                                               }).ToList();

      return html.DropDownListFor(expression, taxCodeSelectList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Populate Membership status values
    /// </summary>
    /// <param name="html"></param>
    /// <param name="controlId"></param>
    /// <param name="selectedValue"></param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    //public static MvcHtmlString MembershipStatusDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
    //{
    //  var memberStatusList = EnumMapper.GetMembershipStatusList();
    //  return html.DropDownListFor(expression, memberStatusList);
    //}
    public static MvcHtmlString MembershipStatusDropdownList(this HtmlHelper html, string controlId, string selectedValue, IDictionary<string, object> htmlAttributes = null)
    {
      // TODO: Get membership status values from database

      var str = string.Empty;

      var statusList = EnumMapper.GetMembershipStatusList();

      if (string.IsNullOrEmpty(selectedValue) || selectedValue == "0")
      {
        statusList.Single(status => status.Value == ((int)MemberStatus.Pending).ToString()).Selected = true;
      }
      else
      {
        statusList.Single(status => status.Value == selectedValue).Selected = true;
      }
      return html.DropDownList(controlId, statusList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Creates ACH Membership Status dropdown and sets its selected value.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    public static MvcHtmlString AchMembershipStatusDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
    {
      var achMemberStatusList = EnumMapper.GetAchMembershipStatusList();

      return html.DropDownListFor(expression, achMemberStatusList);
    }

    /// <summary>
    /// Creates ACH Category dropdown and sets its selected value.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static MvcHtmlString AchCategoryDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes = null)
    {
      List<SelectListItem> achCategoryList = EnumMapper.GetachCategoryList();

      return html.DropDownListFor(expression, achCategoryList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Creates Output File Delivery dropdown and sets its selected value.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static MvcHtmlString OutputFileDeliveryDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes = null)
    {
      var outputFileDeliveryList = EnumMapper.GetOutputFileDeliveryList();

      return html.DropDownListFor(expression, outputFileDeliveryList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Creates Delivery Preference dropdown and sets its selected value.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static MvcHtmlString DeliveryPreferenceDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
    {
      var deliveryPreferenceList = EnumMapper.GetDeliveryPreferencesList();

      deliveryPreferenceList.Single(deliveryPreference => deliveryPreference.Value == ((int)DeliveryPreference.Email).ToString()).Selected = true;

      return html.DropDownListFor(expression, deliveryPreferenceList);
    }

    /// <summary>
    /// Creates Delivery Preference dropdown and sets its selected value.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static MvcHtmlString SalutationDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
    {
      var salutationList = EnumMapper.GetSalutationList();

      //return html.DropDownListFor(expression, salutationList, PleaseSelectText);
      return html.DropDownListFor(expression, salutationList);
    }

    /// <summary>
    /// Creates Contact Status dropdown and sets its selected value.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static MvcHtmlString ContactStatusDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
    {
      var contactStatusList = EnumMapper.GetContactStatusList();

      return html.DropDownListFor(expression, contactStatusList, PleaseSelectText);
    }

    /// <summary>
    /// Get static PMI list
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    public static MvcHtmlString StaticPMIDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
    {
      // Get PMI List
      //TODO: Remove these hardcoded values 
      var pmiList = new List<string>
                      {
                        "M",
                        "Q",
                        "R",
                        "S",
                        "T",
                        "U",
                        "V",
                        "W",
                        "X",
                        "Z"
                      };

      var pmiSelectList = pmiList.Select(pmi => new SelectListItem
                                                                          {
                                                                            Text = pmi,
                                                                            Value = pmi
                                                                          });

      return html.DropDownListFor(expression, pmiSelectList, PleaseSelectText, htmlAttributes);
    }


    /// <summary>
    /// Get static Original PMI list
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    public static MvcHtmlString StaticOriPMIDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
    {
      // Get PMI List
      //TODO: Remove these hardcoded values 
      List<char> pmiList = "ABCDEFGHIJKLMN0PQRSTUVWXYZ".ToList();

      var pmiSelectList = pmiList.Select(pmi => new SelectListItem
      {
        Text = pmi.ToString(),
        Value = pmi.ToString()
      });

      return html.DropDownListFor(expression, pmiSelectList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Creates suspension period dropdown and sets its selected value.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="selectedValue"></param>
    /// <param name="htmlAttributes"></param>
    /// <param name="controlId"></param>
    /// <returns></returns> 
    public static MvcHtmlString SuspensionDropdown(this HtmlHelper html, string controlId, DateTime? selectedValue, object htmlAttributes = null)
    {
      var billingPeriods = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager)).GetSuspensionBillingPeriods(ClearingHouse.Ich, true);

      var currentbillingPeriod = billingPeriods[0]; // current or previous open period.

      var selectedBillingPeriod = selectedValue.HasValue ? selectedValue.Value.ToString(FormatConstants.BillingPeriodFormat) : string.Format("{0}-{1}-{2}",
                                                                                                                 currentbillingPeriod.Year,
                                                                                                                 currentbillingPeriod.Month,
                                                                                                                 currentbillingPeriod.Period);
      var billingPeriodSelectList = billingPeriods.Select(billingPeriod => new SelectListItem
                                                                                              {
                                                                                                Text =
                                                                                                  string.Format("{0}-{1}-{2}",
                                                                                                                billingPeriod.Year,
                                                                                                                CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(
                                                                                                                  billingPeriod.Month),
                                                                                                                billingPeriod.Period),
                                                                                                Value =
                                                                                                  string.Format("{0}-{1}-{2}",
                                                                                                                billingPeriod.Year,
                                                                                                                billingPeriod.Month,
                                                                                                                billingPeriod.Period),
                                                                                                Selected =
                                                                                                  (string.Format("{0}-{1}-{2}",
                                                                                                                 billingPeriod.Year,
                                                                                                                 billingPeriod.Month,
                                                                                                                 billingPeriod.Period) == selectedBillingPeriod)
                                                                                              }).ToList();

      if (selectedValue != null)
      {
        string strCurrentValue = selectedValue.Value.Year.ToString() + "-" + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(selectedValue.Value.Month) + "-" +
                                 selectedValue.Value.Day;
        SelectListItem currentValue = new SelectListItem { Text = strCurrentValue, Value = selectedBillingPeriod };
        billingPeriodSelectList.Insert(0, currentValue);
      }
      else
        billingPeriodSelectList.Insert(0, PleaseSelectListItem);

      return MvcHtmlString.Create(DropdownList(html, controlId, billingPeriodSelectList));
    }

    /// <summary>
    /// Creates Default suspension period dropdown and sets its selected value.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="selectedValue"></param>
    /// <param name="htmlAttributes"></param>
    /// <param name="controlId"></param>
    /// <returns></returns> 
    public static MvcHtmlString DefaultSuspensionDropdown(this HtmlHelper html, string controlId, DateTime? selectedValue, object htmlAttributes = null)
    {
      var billingPeriods = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager)).GetDefaultSuspensionPeriods(ClearingHouse.Ich, true);

      var currentbillingPeriod = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager)).GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);

      // In order to get billing period which is Suspension period -2 , the method which subtracts periods twice
      // because currently , implementation of subtracting periods is available only till subtracting 1 period
      var defaultSuspensionPeriod = currentbillingPeriod - 1;
      defaultSuspensionPeriod = defaultSuspensionPeriod - 1;

      var selectedDefaultSuspensionPeriod = selectedValue.HasValue ? selectedValue.Value.ToString(FormatConstants.BillingPeriodFormat) : string.Format("{0}-{1}-{2}",
                                                                                                                 defaultSuspensionPeriod.Year,
                                                                                                                 defaultSuspensionPeriod.Month,
                                                                                                                 defaultSuspensionPeriod.Period);

      var billingPeriodSelectList = billingPeriods.Select(billingPeriod => new SelectListItem
                                                                                              {
                                                                                                Text =
                                                                                                  string.Format("{0}-{1}-{2}",
                                                                                                                billingPeriod.Year,
                                                                                                                CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(
                                                                                                                  billingPeriod.Month),
                                                                                                                billingPeriod.Period),
                                                                                                Value =
                                                                                                  string.Format("{0}-{1}-{2}",
                                                                                                                billingPeriod.Year,
                                                                                                                billingPeriod.Month,
                                                                                                                billingPeriod.Period),
                                                                                                Selected =
                                                                                                  (string.Format("{0}-{1}-{2}",
                                                                                                                 billingPeriod.Year,
                                                                                                                 billingPeriod.Month,
                                                                                                                 billingPeriod.Period) == selectedDefaultSuspensionPeriod)
                                                                                              }).ToList();

      if (selectedValue != null)
      {
        string strCurrentValue = string.Format("{0}-{1}-{2}", selectedValue.Value.Year.ToString(), CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(selectedValue.Value.Month),
                                 selectedValue.Value.Day);
        SelectListItem currentValue = new SelectListItem { Text = strCurrentValue, Value = selectedDefaultSuspensionPeriod };
        billingPeriodSelectList.Insert(0, currentValue);
      }
      else
        billingPeriodSelectList.Insert(0, PleaseSelectListItem);


      return MvcHtmlString.Create(DropdownList(html, controlId, billingPeriodSelectList));
    }

    /// <summary>
    /// Populate Member List
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="controlId"></param>
    /// <param name="isdisabled"></param>
    /// <returns></returns>
    public static MvcHtmlString MemberDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string controlId, bool isdisabled)
    {
      var memberList = Ioc.Resolve<IMemberManager>(typeof(IMemberManager)).GetMemberListFromDB();

      var memberSelectList = memberList.Select(member => new SelectListItem
                                                                                   {
                                                                                     Text = member.CommercialName,
                                                                                     Value = member.Id.ToString()
                                                                                   });
      if ((isdisabled))
      {
        return html.DropDownListFor(expression,
                                    memberSelectList,
                                    PleaseSelectText,
                                    new
                                      {
                                        @id = controlId,
                                        @disabled = isdisabled
                                      });
      }

      return html.DropDownListFor(expression,
                                  memberSelectList,
                                  PleaseSelectText,
                                  new
                                    {
                                      @id = controlId
                                    });
    }

    /// <summary>
    /// Populate Ich Member List
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="controlId"></param>
    /// <param name="isdisabled"></param>
    /// <returns></returns>
    public static MvcHtmlString IchMemberDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string controlId, bool isdisabled)
    {
      var memberList = Ioc.Resolve<IMemberManager>(typeof(IMemberManager)).GetIchMemberList();

      var memberSelectList = memberList.Select(member => new SelectListItem
                                                                                   {
                                                                                     Text = member.CommercialName,
                                                                                     Value = member.Id.ToString()
                                                                                   });
      if ((isdisabled))
      {
        return html.DropDownListFor(expression,
                                    memberSelectList,
                                    PleaseSelectText,
                                    new
                                      {
                                        @id = controlId,
                                        @disabled = isdisabled
                                      });
      }

      return html.DropDownListFor(expression,
                                  memberSelectList,
                                  PleaseSelectText,
                                  new
                                    {
                                      @id = controlId
                                    });
    }

    /// <summary>
    /// Returns Dropdown List for Blocking Rules.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="controlId"></param>
    /// <param name="clearingHouse"></param>
    /// <returns></returns>
    public static MvcHtmlString BlockingRulesDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string controlId, string clearingHouse)
    {
      var blockingRulesList = Ioc.Resolve<IBlockingRulesManager>(typeof(IBlockingRulesManager)).GetBlockingRuleList("", "", "", clearingHouse);

      var blockingRulesSelectList = blockingRulesList.Select(blockingRule => new SelectListItem
                                                                                                       {
                                                                                                         Text = blockingRule.RuleName,
                                                                                                         Value = blockingRule.Id.ToString()
                                                                                                       });
      return html.DropDownListFor(expression,
                                  blockingRulesSelectList,
                                  PleaseSelectText,
                                  new
                                    {
                                      @id = controlId
                                    });
    }

    /// <summary>
    /// Populate Membership sub status values
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static MvcHtmlString MembershipSubStatusDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
    {
      var memberStatusList = EnumMapper.GetMembershipSubStatusList();

      return html.DropDownListFor(expression, memberStatusList, PleaseSelectText);
    }

    /// <summary>
    /// Populate Membership sub status values
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static MvcHtmlString MembershipSubStatusDropdownList(this HtmlHelper html, string controlId, string selectedValue, IDictionary<string, object> htmlAttributes = null)
    {
      var memberSubStatusList = EnumMapper.GetMembershipSubStatusList();

      var memberSubStatusSelectList = memberSubStatusList.Select(membesubstatus => new SelectListItem
      {
        Text = membesubstatus.Text,
        Value = membesubstatus.Value.ToString(),
        Selected = (Convert.ToInt16(membesubstatus.Value) == 1)  // CMP603:1 = MemberSubStatus.None
      }).ToList();

      return html.DropDownList(controlId, memberSubStatusSelectList, htmlAttributes);
    }


    /// <summary>
    /// Charges the category dropdown list.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="controlName">Name of the control.</param>
    /// <param name="chargeCategoryId">The charge category id.</param>
    /// <param name="billingCategory">The billing category.</param>
    /// <param name="isForSearch">if set to <c>true</c> [is for search].</param>
    /// <param name="isIncludeInactive">if it true then method will return all active and in-active Charge categories for billing category misc only</param>
    /// <returns></returns>
    public static MvcHtmlString ChargeCategoryDropdownList(this HtmlHelper html, string controlName, int chargeCategoryId, BillingCategoryType billingCategory, bool isForSearch = false, bool isIncludeInactive = false)
    {
      // CMP609: MISC Changes Required as per ISW2
      // added new parameter 'isIncludeInactive'. If it is true then method will return the all charge category for misc category including in-active.
      var chargeCategoryList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetChargeCategoryList(billingCategory, isIncludeInactive);
      var chargeCategorySelectList = chargeCategoryList.Select(chargeCategory => new SelectListItem
                                                                                                    {
                                                                                                      Text = chargeCategory.Name,
                                                                                                      Value = chargeCategory.Id.ToString(),
                                                                                                      Selected = chargeCategory.Id == chargeCategoryId
                                                                                                    }).ToList();

      if (IsViewMode(html))
      {
        var chargeCodeCategory = chargeCategoryList.FirstOrDefault(chargeCategory => (chargeCategory.Id == chargeCategoryId));
        return html.TextBox(controlName, chargeCodeCategory == null ? string.Empty : chargeCodeCategory.Name);
      }

      if (isForSearch)
      {
        chargeCategorySelectList.Insert(0,
                                        new SelectListItem
                                          {
                                            Value = "-1",
                                            Text = AllText,
                                            Selected = true
                                          });

        return html.DropDownList(controlName, chargeCategorySelectList);
      }

      // Do not show 'Please Select' item in dropdown if there is only item in the dropdown list.
      if (chargeCategorySelectList.Count == 1)
      {
        return html.DropDownList(controlName, chargeCategorySelectList);
      }

      return html.DropDownList(controlName, chargeCategorySelectList, PleaseSelectText);
    }

    /// <summary>
    /// Charges the category dropdown list.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="controlName">Name of the control.</param>
    /// <param name="chargeCategoryName">Name of the charge category.</param>
    /// <param name="billingCategory">The billing category.</param>
    /// <param name="isForSearch">if set to <c>true</c> [is for search].</param>
    /// <param name="isIncludeInactive">if it true then method will return all active and in-active Charge categories for billing category misc only</param>
    /// <returns></returns>
    public static MvcHtmlString ChargeCategoryDropdownList(this HtmlHelper html, string controlName, string chargeCategoryName, BillingCategoryType billingCategory, bool isForSearch = false, bool isIncludeInactive = false)
    {
      // CMP609: MISC Changes Required as per ISW2
      // added new parameter 'isIncludeInactive'. If it is true then method will return the all charge category for misc category including in-active.
      var chargeCategoryList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetChargeCategoryList(billingCategory, isIncludeInactive);
      var chargeCategorySelectList = chargeCategoryList.Select(chargeCategory => new SelectListItem
      {
        Text = chargeCategory.Name,
        Value = chargeCategory.Id.ToString(),
        Selected = chargeCategory.Name == chargeCategoryName
      }).ToList();

      if (IsViewMode(html))
      {
        var chargeCodeCategory = chargeCategoryList.FirstOrDefault(chargeCategory => (chargeCategory.Name == chargeCategoryName));
        return html.TextBox(controlName, chargeCodeCategory == null ? string.Empty : chargeCodeCategory.Name);
      }

      if (isForSearch)
      {
        chargeCategorySelectList.Insert(0,
                                        new SelectListItem
                                        {
                                          Value = "-1",
                                          Text = AllText,
                                          Selected = true
                                        });

        return html.DropDownList(controlName, chargeCategorySelectList);
      }

      return html.DropDownList(controlName, chargeCategorySelectList, PleaseSelectText);
    }

    /// <summary>
    /// Additionals the detail dropdown.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="controlName">Name of the control.</param>
    /// <param name="additionalDetailType">Type of the additional detail.</param>
    /// <param name="additionalDetailLevel">The additional detail level.</param>
    /// <param name="selectedValue">The selected value.</param>
    /// <returns></returns>
    public static MvcHtmlString AdditionalDetailDropdown(this HtmlHelper html,
                                                         string controlName,
                                                         AdditionalDetailType additionalDetailType,
                                                         AdditionalDetailLevel additionalDetailLevel,
                                                         string selectedValue = null)
    {
      var additionalDetailList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetAdditionalDetailList(additionalDetailType, additionalDetailLevel);
      var additionalDetailSelectList = additionalDetailList.Select(additionalDetail => new SelectListItem
                                                                                                                 {
                                                                                                                   Text = additionalDetail.Name,
                                                                                                                   Value = additionalDetail.Name,
                                                                                                                   Selected = (additionalDetail.Name == selectedValue)
                                                                                                                 });

      return html.DropDownList(controlName, additionalDetailSelectList, PleaseSelectText);
    }

    public static MvcHtmlString BillingTypeDropdownList(this HtmlHelper html, string controlId, int controlValue, bool isForAllType = false)
    {
      var billingTypeList = EnumMapper.GetBillingTypeList();
      var selectedValue = controlValue == 0 ? 1 : controlValue;
      if (isForAllType)
      {
        billingTypeList.Insert(0, AllListItemWithZeroValue);
        selectedValue = 0;
      }
      else
      {
        billingTypeList.Insert(0, PleaseSelectListItem);
      }

      billingTypeList.SingleOrDefault(item => item.Value == selectedValue.ToString()).Selected = true;

      return MvcHtmlString.Create(DropdownList(html, controlId, billingTypeList));
    }
    /// <summary>
    /// DropDown for BillingTypeDropdownList for RM-BM-CM Details Report 
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static MvcHtmlString BillingTypeDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
    {
      var billingTypeList = EnumMapper.GetBillingTypeList();
      billingTypeList.Insert(0, PleaseSelectListItem);

      return html.DropDownListFor(expression, billingTypeList);
    }

    /// <summary>
    /// FIMs/BM/CM indicator dropdown list for.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    public static MvcHtmlString FimBmCmIndicatorDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
    {
      var fimBmCmIndicatorList = EnumMapper.GetFimBmCmIndicatorList();
      fimBmCmIndicatorList.Insert(0, PleaseSelectListItem);

      return html.DropDownListFor(expression, fimBmCmIndicatorList);
    }

    public static MvcHtmlString FimBmCmIndicatorDropdownList(this HtmlHelper html, string controlId, string controlValue)
    {
      var fimBmCmIndicatorList = EnumMapper.GetFimBmCmIndicatorList();

      var fimBmCmIndicator = fimBmCmIndicatorList.FirstOrDefault(indicator => (indicator.Value == controlValue));

      if (IsViewMode(html))
      {
        return html.TextBox(controlId, fimBmCmIndicator == null ? string.Empty : fimBmCmIndicator.Text);
      }

      if (fimBmCmIndicator != null)
        fimBmCmIndicator.Selected = true;

      fimBmCmIndicatorList.Insert(0, PleaseSelectListItem);
      return html.DropDownList(controlId, fimBmCmIndicatorList);
    }

    public static MvcHtmlString BmCmIndicatorDropdownList(this HtmlHelper html, string controlId, string controlValue)
    {
      var bmCmIndicatorList = EnumMapper.GetBmCmIndicatorList();

      var bmCmIndicator = bmCmIndicatorList.FirstOrDefault(indicator => (indicator.Value == controlValue));

      if (IsViewMode(html))
      {
        return html.TextBox(controlId, bmCmIndicator == null ? string.Empty : bmCmIndicator.Text);
      }

      if (bmCmIndicator != null)
        bmCmIndicator.Selected = true;

      bmCmIndicatorList.Insert(0, PleaseSelectListItem);
      return html.DropDownList(controlId, bmCmIndicatorList);
    }

    /// <summary>
    /// Transactions the status dropdown list for.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    public static MvcHtmlString TransactionStatusDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
    {
      var transactionList = EnumMapper.GetTransactionStatusList();
      transactionList.Insert(0, PleaseSelectIntListItem);

      return html.DropDownListFor(expression, transactionList);
    }

    /// <summary>
    /// Charges the category dropdown list for.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="isIncludeInactive">if it true then method will return all active and in-active Charge categories for billing category misc only</param>
    /// <returns></returns>
    public static MvcHtmlString ChargeCategoryDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, bool isIncludeInactive = false)
    {
      // CMP609: MISC Changes Required as per ISW2
      // added new parameter 'isIncludeInactive'. If it is true then method will return the all charge category for misc category including in-active.
      var chargeCategoryList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetChargeCategoryList(BillingCategoryType.Misc, isIncludeInactive);

      var chargeCategorySelectList = chargeCategoryList.Select(chargeCategory => new SelectListItem
                                                                                                    {
                                                                                                      Text = chargeCategory.Name,
                                                                                                      Value = chargeCategory.Id.ToString()
                                                                                                    }).ToList();

      chargeCategorySelectList.Insert(0, PleaseSelectIntListItem);

      return html.DropDownListFor(expression, chargeCategorySelectList);
    }

    /// <summary>
    /// Charges the category dropdown list for.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="isIncludeInactive">if it true then method will return all active and in-active Charge categories for billing category misc only</param>
    /// <returns></returns>
    public static MvcHtmlString ChargeCategoryDropdownListWithAllFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, bool isIncludeInactive = false)
    {
      // CMP609: MISC Changes Required as per ISW2
      // added new parameter 'isIncludeInactive'. If it is true then method will return the all charge category for misc category including in-active.
      var chargeCategoryList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetChargeCategoryList(BillingCategoryType.Misc, isIncludeInactive);

      var chargeCategorySelectList = chargeCategoryList.Select(chargeCategory => new SelectListItem
      {
        Text = chargeCategory.Name,
        Value = chargeCategory.Id.ToString()
      }).ToList();

      chargeCategorySelectList.Insert(0,
                                      new SelectListItem
                                      {
                                        Value = "0",
                                        Text = AllText,
                                        Selected = true
                                      });

      return html.DropDownListFor(expression, chargeCategorySelectList);
    }



    /// <summary>
    /// Charges the category dropdown list for.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    public static MvcHtmlString ChargeCategoryDropdownListForUatp<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
    {
      var chargeCategoryList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetChargeCategoryList(BillingCategoryType.Uatp);

      var chargeCategorySelectList = chargeCategoryList.Select(chargeCategory => new SelectListItem
                                                                                     {
                                                                                       Text = chargeCategory.Name,
                                                                                       Value =
                                                                                           chargeCategory.Id.
                                                                                           ToString()
                                                                                     }).ToList();

      return html.DropDownListFor(expression, chargeCategorySelectList);
    }

    /// <summary>
    /// Correspondences the statusropdown list.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="controlId">The control id.</param>
    /// <param name="controlValue">The control value.</param>
    /// <returns></returns>
    /// 
    public static MvcHtmlString CorrespondenceStatusropdownList(this HtmlHelper html, string controlId, int controlValue)
    {
      var correspondenceSelectList = EnumMapper.GetPaxCorrespondenceStatusList();
      var selectedValue = controlValue == 0 ? (int)CorrespondenceStatus.Open : controlValue;
      correspondenceSelectList.Insert(0, AllListItem);
      correspondenceSelectList.SingleOrDefault(item => item.Value == selectedValue.ToString()).Selected = true;
      return MvcHtmlString.Create(DropdownList(html, controlId, correspondenceSelectList));
    }

    /// <summary>
    /// This drop down list is used in correspondence report download search criteria
    /// </summary>
    /// <param name="html"></param>
    /// <param name="controlId"></param>
    /// <param name="controlValue"></param>
    /// <returns></returns>
    public static MvcHtmlString CorrespondenceReportStatusDropdownList(this HtmlHelper html, string controlId, int controlValue)
    {
      var correspondenceSelectList = EnumMapper.GetPaxCorrespondenceStatusList();
      var selectedValue = controlValue == 0 ? -1 : controlValue;
      correspondenceSelectList.Insert(0, AllListItem);
      correspondenceSelectList.SingleOrDefault(item => item.Value == selectedValue.ToString()).Selected = true;
      return MvcHtmlString.Create(DropdownList(html, controlId, correspondenceSelectList));
    }

    /// <summary>
    /// Correspondences the owner dropdown list.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="controlId">The control id.</param>
    /// <param name="controlValue">The control value.</param>
    /// <param name="memberId">The member id.</param>
    /// <returns></returns>
    public static MvcHtmlString CorrespondenceOwnerDropdownList(this HtmlHelper html, string controlId, int? controlValue, int memberId)
    {
      //var ownerList = Ioc.Resolve<IMemberManager>(typeof(IMemberManager)).GetUserList(memberId);
      var ownerList = Ioc.Resolve<IUserManager>(typeof(IUserManager)).GetUsersByMemberId(memberId);
      var selectedValue = controlValue == null || controlValue == 0 ? -1 : controlValue;

      var ownserSelectList = ownerList.Select(owner => new SelectListItem
                                                                          {
                                                                            Text = string.Format("{0} {1}", owner.FirstName, owner.LastName),
                                                                            Value = owner.Id.ToString(),
                                                                            Selected = (owner.Id == selectedValue)
                                                                          }).ToList();

      ownserSelectList.Insert(0, AllListItem);

      return MvcHtmlString.Create(DropdownList(html, controlId, ownserSelectList));
    }

    /// <summary>
    /// Create Charge Category Dropdown List.
    /// </summary>
    /// <param name="html"></param>
    /// <param name="controlName"></param>
    /// <param name="chargeCategoryId"></param>
    /// <param name="billingCategoryId"></param>
    /// <param name="isIncludeInactive">if it true then method will return all active and in-active Charge categories for billing category misc only</param>
    /// <returns></returns>
    public static MvcHtmlString ChargeCategoryDropdownList(this HtmlHelper html, string controlName, int chargeCategoryId, int billingCategoryId, bool isIncludeInactive = false)
    {
      // CMP609: MISC Changes Required as per ISW2
      // added new parameter 'isIncludeInactive'. If it is true then method will return the all charge category for misc category including in-active.
      var chargeCategoryList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetChargeCategoryList((BillingCategoryType)billingCategoryId, isIncludeInactive);
      var chargeCategorySelectList = chargeCategoryList.Select(chargeCategory => new SelectListItem
      {
        Text = chargeCategory.Name,
        Value = chargeCategory.Id.ToString(),
        Selected = chargeCategory.Id == chargeCategoryId
      });
      return html.DropDownList(controlName, chargeCategorySelectList, PleaseSelectText);
    }

    /// <summary>
    /// Creates the Charge code dropdown list.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="controlName">Name of the control.</param>
    /// <param name="chargeCodeId">The charge code id.</param>
    /// <param name="chargeCategoryId">The charge category id.</param>
    /// <returns></returns>
    public static MvcHtmlString ChargeCodeDropdownList(this HtmlHelper html, string controlName, int chargeCodeId, int chargeCategoryId)
    {
      var chargeCodeList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetChargeCodeList(chargeCategoryId);
      var chargeCodeSelectList = chargeCodeList.Select(chargeCode => new SelectListItem
                                                                                               {
                                                                                                 Text = chargeCode.Name,
                                                                                                 Value = chargeCode.Id.ToString(),
                                                                                                 Selected = chargeCode.Id == chargeCodeId
                                                                                               });
      return html.DropDownList(controlName, chargeCodeSelectList, PleaseSelectText);
    }

    /// <summary>
    /// Creates the Charge code dropdown list.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="controlName">Name of the control.</param>
    /// <param name="chargeCodeId">The charge code id.</param>
    /// <param name="chargeCategoryId">The charge category id.</param>
    /// <returns></returns>
    //
    public static MvcHtmlString ChargeCodeDropdownList<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, int chargeCategoryId, bool isAllText = false)
    {
      var chargeCodeList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetChargeCodeList(chargeCategoryId);

      var chargeCodeSelectedList = chargeCodeList.Select(chargeCode => new SelectListItem
      {
        Text = chargeCode.Name,
        Value = chargeCode.Id.ToString()
      }).ToList();

      if (isAllText)
        chargeCodeSelectedList.Insert(0, new SelectListItem
                                                        {
                                                          Value = "0",
                                                          Text = AllText,
                                                          Selected = true
                                                        });
      else
        chargeCodeSelectedList.Insert(0, new SelectListItem
                                            {
                                              Value = "0",
                                              Text = PleaseSelectText,
                                              Selected = true
                                            });

      return html.DropDownListFor(expression, chargeCodeSelectedList);
    }

    /// <summary>
    /// Creates the charge code type dropdown list.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="controlName">Name of the control.</param>
    /// <param name="chargeCodeTypeId">The charge code type id.</param>
    /// <param name="chargeCodeId">The charge code id.</param>
    /// <returns></returns>
    public static MvcHtmlString ChargeCodeTypeDropdownList(this HtmlHelper html, string controlName, int? chargeCodeTypeId, int chargeCodeId, bool isActiveChargeCodeType = false)
    {
      var chargeCodeTypes = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetChargeCodeType(chargeCodeId, isActiveChargeCodeType);
      var chargeCodeTypeSelectList = chargeCodeTypes.Select(chargeCodeType => new SelectListItem
                                                                                                        {
                                                                                                          Text = chargeCodeType.Name,
                                                                                                          Value = chargeCodeType.Id.ToString(),
                                                                                                          Selected = chargeCodeType.Id == chargeCodeTypeId
                                                                                                        });
      return html.DropDownList(controlName, chargeCodeTypeSelectList, PleaseSelectText);
    }

    /// <summary>
    /// Create dropdown for Service Date in LineItemDetail
    /// </summary>
    /// <param name="html">html</param>
    /// <param name="controlName">Name of the control.</param>
    /// <param name="selectedMonth">Selected month value</param>
    /// <param name="selectedYear">Selected year value</param>
    /// <param name="htmlAttributes">htmlAttributes</param>
    /// <returns></returns>
    public static MvcHtmlString ServiceDateMonthYearDropdownList(this HtmlHelper html, string controlName, int selectedMonth, int selectedYear, object htmlAttributes = null)
    {
      // Get Billing Period Data
      var serviceDateSelectList = new List<SelectListItem>();
      var selectedValue = string.Format("{0}-{1}", selectedMonth, selectedYear);

      // Last 12 months, current open month and next 12 months.
      for (var i = -12; i < 13; i++)
      {
        var month = DateTime.UtcNow.AddMonths(-i).ToString(FormatConstants.MonthNameFormat);
        var monthValue = DateTime.UtcNow.AddMonths(-i).Month.ToString();
        var year = DateTime.UtcNow.AddMonths(-i).Year.ToString();
        serviceDateSelectList.Add(new SelectListItem
                                    {
                                      Text = string.Format("{0}-{1}", month, year),
                                      Value = string.Format("{0}-{1}", monthValue, year),
                                      Selected = string.Format("{0}-{1}", monthValue, year) == selectedValue
                                    });
      }

      #region SCP189889: Misc Invoices dated over 1 year not being accepted
      // if Service End Month-Year at Line Item Level is not within the range of Last 12 months, current open month and next 12 months
      // then keep Service End Month-Year at Line Item Details Level
      var currenctSelected = false;

      foreach (var selectListItem in serviceDateSelectList)
      {
        currenctSelected = selectListItem.Selected.Equals(true) ? true : false;
        if (currenctSelected) break;
      }

      if (!currenctSelected)
      {
        var month = "0";
        if (selectedMonth > 0 && selectedMonth < 13)
        {
          month = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(selectedMonth);
        }
        var monthValue = selectedMonth.ToString();
        var year = selectedYear.ToString();
        serviceDateSelectList.Add(new SelectListItem
        {
          Text = string.Format("{0}-{1}", month, year),
          Value = string.Format("{0}-{1}", monthValue, year),
          Selected = true
        });
      }
      #endregion

      return html.DropDownList(controlName, serviceDateSelectList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Dropdown for UOM Code
    /// </summary>
    /// <param name="html"></param>
    /// <param name="controlName"></param>
    /// <param name="uomCodeId"></param>
    /// <returns></returns>
    public static MvcHtmlString UomCodeDropdown(this HtmlHelper html, string controlName, string uomCodeId)
    {
      var uomCodeList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetUomCodeList().OrderBy(uom => uom.Id);
      var uomCodeSelectList = uomCodeList.Select(uomCode => new SelectListItem
                                                                                      {
                                                                                        Text = string.Format("{0}-{1}", uomCode.Id, uomCode.Description),
                                                                                        Value = uomCode.Id.ToString(),
                                                                                        Selected = (!string.IsNullOrEmpty(uomCodeId) && uomCode.Id == uomCodeId)
                                                                                      });

      return html.DropDownList(controlName, uomCodeSelectList, PleaseSelectText);
    }

    /// <summary>
    /// Creates Tax Category dropdown and sets its selected value.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="controlName">Name of the control</param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    public static MvcHtmlString TaxCategoryDropdownList(this HtmlHelper html, string controlName, object htmlAttributes)
    {
      var taxCategoryDropdownList = EnumMapper.GetTaxCategoryList();

      return html.DropDownList(controlName, taxCategoryDropdownList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Vats the sub type dropdown list.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="controlName">Name of the control.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString VatSubTypeDropdownList(this HtmlHelper html, string controlName, object htmlAttributes)
    {
      var vatSubTypeList = EnumMapper.GetVatSubTypes();

      return html.DropDownList(controlName, vatSubTypeList, htmlAttributes);
    }

    // CMP #534: Tax Issues in MISC and UATP Invoices. [Start]
    /// <summary>
    /// Tax Sub Type dropdown list when TaxType is Tax
    /// </summary>
    /// <param name="html"></param>
    /// <param name="controlName"></param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    public static MvcHtmlString TaxSubTypeDropdownList(this HtmlHelper html, string controlName, object htmlAttributes)
    {
      var taxSubTypeList = EnumMapper.GetTaxSubTypes();

      return html.DropDownList(controlName, taxSubTypeList, PleaseSelectText, htmlAttributes);
    }
    // CMP #534: Tax Issues in MISC and UATP Invoices. [End]

    /// <summary>
    /// Types the of contact type dropdown list for.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    public static MvcHtmlString TypeOfContactTypeDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
    {
      var achCategoryList = EnumMapper.GetTypeOfContactTypeList();

      return html.DropDownListFor(expression, achCategoryList, PleaseSelectText);
    }

    /// <summary>
    /// Returns the currency codes with default as the currency code for the main location of the billing member.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="reportTypeCategory"></param>
    /// <param name="htmlAttributes">Html attributes for the dropdown if any.</param>
    /// <returns>HTML for the dropdown</returns>
    public static MvcHtmlString UserDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, int reportTypeCategory, object htmlAttributes = null)
    {
      var iuser = Ioc.Resolve<IUserManagement>(typeof(IUserManagement));
      var user = iuser.GetUserByUserID(SessionUtil.UserId);

      var futureUpdatesManager = Ioc.Resolve<IFutureUpdatesManager>(typeof(IFutureUpdatesManager));
      var userList = futureUpdatesManager.GetAuditTrailUserList(reportTypeCategory, user.Member.MemberID);

      var selectedUserList = userList.Select(u => new SelectListItem
                                                                     {
                                                                       Text = string.Format(@"{0} {1}", u.FirstName, u.LastName),
                                                                       Value = u.UserId.ToString(),
                                                                     }).ToList();

      return html.DropDownListFor(expression, selectedUserList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Contacts type dropdown list.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="controlName">Name of the control.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString ContactTypeDropdownList(this HtmlHelper html, string controlName, object htmlAttributes)
    {
      var contactTypeDropdownList = EnumMapper.GetContactTypeList();

      return html.DropDownList(controlName, contactTypeDropdownList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Used in ICH and ACH blocking rule to populate Blocked By and Against values 
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="isForSearch"></param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    public static MvcHtmlString BlockedAgainstDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html,
                                                                              Expression<Func<TModel, TValue>> expression,
                                                                              bool isForSearch = false,
                                                                              object htmlAttributes = null)
    {
      var bilingPeriodSelectList = new List<SelectListItem>
                                     {
                                       new SelectListItem
                                         {
                                           Text = "By",
                                           Value = 0.ToString()
                                         },
                                       new SelectListItem
                                         {
                                           Text = "Against",
                                           Value = 1.ToString()
                                         },
                                     };

      return html.DropDownListFor(expression, bilingPeriodSelectList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static MvcHtmlString ContactTypeGroupDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
    {
      var contactTypeGroupList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetContactTypeGroupList();
      var contactTypeGroupSelectList = contactTypeGroupList.Select(contactTypeGroup => new SelectListItem
                                                                                                                 {
                                                                                                                   Text = contactTypeGroup.Name,
                                                                                                                   Value = contactTypeGroup.Id.ToString(),
                                                                                                                 });

      return html.DropDownListFor(expression, contactTypeGroupSelectList, PleaseSelectText);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static MvcHtmlString ContactTypeSubGroupDropdownList<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
    {
      var contactTypeSubGroupList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetAllContactTypeSubGroupList();
      var contactTypeSubGroupSelectList = contactTypeSubGroupList.Select(contactTypeGroup => new SelectListItem
                                                                                                                       {
                                                                                                                         Text = contactTypeGroup.Name,
                                                                                                                         Value = contactTypeGroup.Id.ToString(),
                                                                                                                       });
      return html.DropDownListFor(expression, contactTypeSubGroupSelectList, PleaseSelectText);
    }

    /// <summary>
    /// Returns Html string for text box.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="name">The name.</param>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString BilledInTextBox(this HtmlHelper html, string name, int year, int month, object htmlAttributes = null)
    {
      if (year == 0 || month == 0)
      {
        return html.TextBox(name, string.Empty, htmlAttributes);
      }

      var dt = new DateTime(year, month, 1);

      return html.TextBox(name, string.Format("{0}-{1}", dt.Year, dt.ToString(FormatConstants.MonthNameFormat)), htmlAttributes);
    }

    /// <summary>
    /// Creates MVC Html string for Billing year month period text box.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="name">The name.</param>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    /// <param name="period">The period.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString BillingYearMonthPeriodTextBox(this HtmlHelper html, string name, int year, int month, int period, object htmlAttributes = null)
    {
      var dt = new DateTime(year, month, 1);
      return html.TextBox(name, string.Format("{0}-{1}-{2}", dt.Year, dt.ToString("MMM"), period), htmlAttributes);
    }

    public static MvcHtmlString BillingCategoryDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string categoryType = "", object htmlAttributes = null, bool isAll = true)
    {
      var billingCategoryList = EnumMapper.GetBillingCategorysList();

      if (isAll)
      {
        billingCategoryList.Insert(0,
                                  new SelectListItem
                                  {
                                    Value = "-1",
                                    Text = AllText
                                  });
      }

      if (categoryType.ToLower() == "billingcategory")
      {
        billingCategoryList.Insert(0,
                               new SelectListItem
                               {
                                 Value = "0",
                                 Text = PleaseSelectText
                               });
        billingCategoryList.RemoveAt(1);
      }

      return html.DropDownListFor(expression, billingCategoryList, htmlAttributes);
      //return html.DropDownList(controlName, billingCategoryList, htmlAttributes);
    }


    public static MvcHtmlString SystemMonBillingCategoryDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string categoryType = "", object htmlAttributes = null)
    {
      var billingCategoryList = EnumMapper.GetBillingCategorysList();

      billingCategoryList.Insert(0,
                                 new SelectListItem
                                 {
                                   Value = "-1",
                                   Text = "Please Select"

                                 });
      if (categoryType.ToLower() == "billingcategory")
      {
        billingCategoryList.Insert(0,
                               new SelectListItem
                               {
                                 Value = "0",
                                 Text = PleaseSelectText
                               });
        billingCategoryList.RemoveAt(1);
      }



      return html.DropDownListFor(expression, billingCategoryList, htmlAttributes);
      //return html.DropDownList(controlName, billingCategoryList, htmlAttributes);
    }

    [Obsolete]
    // Use ClearanceTypesDropdownListFor instead.
    public static MvcHtmlString ClearanceTypeDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string UserType = "", object htmlAttributes = null)
    {
      var clearanceTypeList = EnumMapper.GetClearanceTypesList();

      clearanceTypeList.Insert(0,
                               new SelectListItem
                                 {
                                   Value = "-1",
                                   Text = AllText
                                 });

      if (UserType.ToLower() == "ich")
      {
        clearanceTypeList.RemoveRange(2, 4);
      }
      else if (UserType.ToLower() == "ach")
      {
        clearanceTypeList.RemoveAt(1);
        clearanceTypeList.RemoveRange(3, 2);
      }

      return html.DropDownListFor(expression, clearanceTypeList, htmlAttributes);
      //return html.DropDownList(controlName, clearanceTypeList, htmlAttributes);
    }

    public static MvcHtmlString ClearanceTypesDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string UserType = "", object htmlAttributes = null)
    {
      const string achUsingIataRulesSmiText = "ach using iata rules";
      const string ichSmiText = "ich";
      const string achSmiText = "ach";
      const string allItemText = "all";
      const string interClearanceFromAchToIch = "Inter Clearance From ACH To ICH"; //CMP602
      const string interClearanceFromIchToAch = "Inter Clearance From ICH To ACH"; //CMP602
      // Passing Credit-note as invoice type to exclude the 'No Settlement(Form C)' item from the SMI list.
      var clearanceTypeList = EnumMapper.GetSettlementMethodList(InvoiceType.CreditNote);

      clearanceTypeList.Insert(0,
                               new SelectListItem
                               {
                                 Value = "-1",
                                 Text = AllText
                               });

      if (UserType.ToLower() == ichSmiText)
      {
        clearanceTypeList.RemoveAll(item => item.Text.ToLower() != ichSmiText && item.Text.ToLower() != allItemText);
        //SCP314690: SF to be logged related to CMP #602
        clearanceTypeList.Insert(2,
                              new SelectListItem
                              {
                                Value = "-2",
                                Text = interClearanceFromIchToAch
                              });
        //CMP602
        clearanceTypeList.Insert(3,
                              new SelectListItem
                              {
                                Value = "-3",
                                Text = interClearanceFromAchToIch
                              });
      }
      else if (UserType.ToLower() == achSmiText)
      {
        clearanceTypeList.RemoveAll(item => item.Text.ToLower() != achSmiText && item.Text.ToLower() != allItemText && !item.Text.ToLower().Contains(achUsingIataRulesSmiText));
        //CMP602
        clearanceTypeList.Insert(3,
                              new SelectListItem
                              {
                                Value = "-2",
                                Text = interClearanceFromIchToAch
                              });
      }

      return html.DropDownListFor(expression, clearanceTypeList, htmlAttributes);
    }


    public static MvcHtmlString MonthsDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
    {
      var monthList = EnumMapper.GetMonthsList();
      monthList.Insert(0,
                       new SelectListItem
                         {
                           Value = "-1",
                           Text = AllText
                         });

      return html.DropDownListFor(expression, monthList, htmlAttributes);
      //return html.DropDownList(controlName, clearanceTypeList, htmlAttributes);
    }

    /// <summary>
    /// Returns Html string for Month Dropdown to implement mandatory month selection option.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString ClearanceMonthDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
    {
      var monthList = EnumMapper.GetMonthsList();
      return html.DropDownListFor(expression, monthList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Returns Html string for Month Dropdown.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="controlName">Name of the control.</param>
    /// <param name="selectedMonthIndex">The selected month.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString MonthsDropdownList(this HtmlHelper html, string controlName, int selectedMonthIndex, object htmlAttributes = null)
    {
      var monthList = EnumMapper.GetMonthsList();
      var selectedMonth = monthList.FirstOrDefault(month => month.Value.Equals(selectedMonthIndex));
      if (selectedMonth != null) selectedMonth.Selected = true;

      return html.DropDownList(controlName, monthList, AllText, htmlAttributes);
    }

    /// <summary>
    /// Determines whether [is file log year dropdown list for] [the specified HTML].
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString IsFileLogYearDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
    {
      var invoiceYearList = Ioc.Resolve<IProcessingDashboardManager>(typeof(IProcessingDashboardManager)).GetAllIsFileLogYear();

      var invoiceYearSelectList = invoiceYearList.Select(invoiceYear => new SelectListItem
                                                                                           {
                                                                                             Text = invoiceYear.ToString(),
                                                                                             Value = invoiceYear.ToString()
                                                                                           }).ToList();

      return html.DropDownListFor(expression, invoiceYearSelectList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Files the status dropdown list for.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString FileStatusDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
    {
      var fileStatusList = EnumMapper.GetFileStatusListForProcessingDashboard();

      return html.DropDownListFor(expression, fileStatusList, AllText, htmlAttributes);
    }

    /// <summary>
    /// To Show OutputFile Status Dropdown list
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="htmlAttributes"></param>
    /// <param name="isResendScreen"></param>
    /// <returns></returns>
    public static MvcHtmlString OutputFileStatusDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null, bool isResendScreen= false)
    {
      //var fileStatusList = EnumMapper.GetFileStatusList();
      var fileStatusIds = new int[] { 2, 9, 21, 22, 23 };

      //SCP#391788 - “validation Completed” in file status.
      //Desc: A file status "Validation Completed" has no use in dropdown list on resend tab of system monitor. 
      //For this SCP the file status has been removed from Resend Screen.
      if(isResendScreen)
      fileStatusIds = fileStatusIds.Except(new int[] {23}).ToArray();
      
      var fileStatusList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetFileStatusList().Where(f => fileStatusIds.Any(id => id == Convert.ToInt32(f.Key))).ToList();
      var outputFileStatusList = fileStatusList.Select(status => new SelectListItem
      {
        Text = status.Value,
        Value = status.Key.ToString()
      }).ToList();

      return html.DropDownListFor(expression, outputFileStatusList, AllText, htmlAttributes);
    }

    /// <summary>
    /// Files the format dropdown list for.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString FileFormatDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
    {
      var fileFormatList = EnumMapper.GetFileFormatListForProcessingDashboard();

      return html.DropDownListFor(expression, fileFormatList, AllText, htmlAttributes);
    }

    /// <summary>
    /// Invoices the owner drop down list for.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="isForSearch">if set to <c>true</c> [is for search].</param>
    /// <returns></returns>
    public static MvcHtmlString InvoiceOwnerDropDownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, bool isForSearch = false)
    {
      var users = Ioc.Resolve<IUserManager>(typeof(IUserManager)).GetUsersByMemberId(SessionUtil.MemberId);

      var dropdownOptions = users.Select(user => new SelectListItem
                                                                    {
                                                                      Text = string.Format("{0} {1}", user.FirstName, user.LastName),
                                                                      Value = user.Id.ToString()
                                                                    }).ToList();

      if (isForSearch)
      {
        dropdownOptions.Insert(0, AllListItem);
      }

      return html.DropDownListFor(expression, dropdownOptions);
    }

    /// <summary>
    /// Memoes the type dropdownlist for.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString MemoTypeDropdownlistFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
    {
      var invoicePeriodList = EnumMapper.GetMemoList();

      invoicePeriodList.Insert(0, AllListItem);

      return html.DropDownListFor(expression, invoicePeriodList, htmlAttributes);
    }

    /// <summary>
    /// Cargo Memo type dropdownlist for Cargo.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString CargoMemoTypeDropdownlistFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
    {
      var invoicePeriodList = EnumMapper.GetCargoMemoList();

      invoicePeriodList.Insert(0, AllListItem);

      return html.DropDownListFor(expression, invoicePeriodList, htmlAttributes);
    }


    /// <summary>
    /// Creates Resubmission Status dropdown and sets its selected value.
    /// </summary>
    /// <param name="html">The html.</param>
    /// <param name="controlId">The ControlId.</param>
    /// <param name="controlValue"></param>
    /// <param name="resubmissinoStatusIds"></param>
    /// <returns></returns>
    public static MvcHtmlString ResubmissionStatusDropdownListFor(this HtmlHelper html, string controlId, string controlValue, params int[] resubmissinoStatusIds)
    {
      var resubmissionStatusList = EnumMapper.GetResubmissionStatusList();

      if (controlValue != "0")
        resubmissionStatusList.SingleOrDefault(item => item.Value == controlValue).Selected = true;
      var resubmissionStatusSelectList = new List<SelectListItem>();
      foreach (var id in resubmissinoStatusIds)
      {
        var id1 = id;
        var listItem = resubmissionStatusList.Single(item => item.Value == id1.ToString());
        resubmissionStatusSelectList.Add(listItem);
      }
      resubmissionStatusSelectList.Insert(0, AllListItem);

      return html.DropDownList(controlId, resubmissionStatusSelectList, AllListItem);
    }
    /// <summary>
    /// used in IS Calender Search
    /// </summary>
    /// <param name="html"></param>
    /// <param name="name">Name of the dropdown control.</param>
    /// <param name="selectedYear"></param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    // Get Billing year Data
    public static MvcHtmlString IsCalendarBillingYearDropdownList(this HtmlHelper html, string name, int selectedYear, object htmlAttributes = null)
    {
      var bilingYearSelectList = new List<SelectListItem>();

      for (var i = 0; i < 6; i++)
      {
        var year = DateTime.UtcNow.AddYears(-i).ToString(FormatConstants.FullYearFormat);
        bilingYearSelectList.Add(new SelectListItem
                                   {
                                     Text = year,
                                     Value = year,
                                     Selected = year == selectedYear.ToString()
                                   });
      }

      return html.DropDownList(name, bilingYearSelectList, htmlAttributes);
    }







    // <summary>
    /// Creates Usre Category dropdown and sets its selected value.
    /// </summary>
    /// <typeparam name="controlId">dropdown control Id.</typeparam>
    /// <typeparam name="selectedValue">The type of the value.</typeparam>
    /// <param name="htmlAttributes">The HTML Attributes.</param>
    /// <returns></returns>


    public static MvcHtmlString UserCategoryDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html,
                                                                        Expression<Func<TModel, TValue>> expression,
                                                                         string selectedValue,
                                                                         object htmlAttributes = null)
    {
      // Get User Category
      var userCaltegoryList = EnumMapper.GetUserCategoryList();

      var currencydSelectList = userCaltegoryList.Select(category => new SelectListItem
      {
        Text = category.Text,
        Value = category.Value,
        Selected = category.Value == selectedValue
      }).ToList();

      return html.DropDownListFor(expression, currencydSelectList, PleaseSelectText, htmlAttributes);

    }

    public static MvcHtmlString TemplateNameDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html,
                                                                    Expression<Func<TModel, TValue>> expression,
                                                                     string selectedValue,
                                                                     object htmlAttributes = null)
    {
      // Get User Category

      var templateNameList = Ioc.Resolve<IPermissionManager>(typeof(IPermissionManager)).GetTemplateNameList();

      var templateSelectList = templateNameList.Select(template => new SelectListItem
      {
        Text = template.TemplateName,
        Value = template.Id.ToString(),
        Selected = template.Id == Convert.ToInt32(selectedValue),
      });
      return html.DropDownListFor(expression, templateSelectList, PleaseSelectText, htmlAttributes);

    }

    #region Helper Methods

    private static IEnumerable<SelectListItem> GetCurrencySelectList(bool isFromMiscBillingCurrency = false)
    {
      var currencyList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetCurrencyList().OrderBy(curr => curr.Code);

      if (isFromMiscBillingCurrency)
        return currencyList.Select(currency => new SelectListItem
                                            {
                                              Text = string.Format("{0}-{1}", currency.Code, currency.Name),
                                              Value = currency.Id.ToString()
                                            }).ToList();
      else
        return currencyList.Select(currency => new SelectListItem
                                               {
                                                 Text = currency.Code,
                                                 Value = currency.Id.ToString()
                                               }).ToList();
    }


    #endregion

    /// <summary>
    /// Populate Member List
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="controlId"></param>
    /// <returns></returns>

    public static MvcHtmlString MessageSendingMemberDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
    {
      var memberList = Ioc.Resolve<IMemberManager>(typeof(IMemberManager)).GetMemberListFromDB();

      var memberSelectList = memberList.Select(member => new SelectListItem
      {
        Text = member.CommercialName,
        Value = member.Id.ToString()
      }).OrderBy(l => l.Text).ToList();


      memberSelectList.Insert(0, new SelectListItem
                                       {
                                         Value = "L",
                                         Text = AllMembers
                                       });
      memberSelectList.Insert(1, new SelectListItem
      {
        Value = "I",
        Text = AllICHMembers
      });

      memberSelectList.Insert(2, new SelectListItem
      {
        Value = "A",
        Text = AllACHMembers
      });

      //  return MvcHtmlString.Create(DropdownList(html, controlId, memberSelectList));

      //var newMemberSelectList = memberSelectList.OrderBy(l => l.Text);

      return html.DropDownListFor(expression, memberSelectList, PleaseSelectText, htmlAttributes);
    }




    /// <summary>
    /// To show up Salutation Dropdown List 
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="selectedValue"></param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    public static MvcHtmlString SalutionDropdownList(this HtmlHelper html, string controlId,
                                                                       string selectedValue,
                                                                       IDictionary<string, object> htmlAttributes = null)
    {
      var salutionList = EnumMapper.GetSalutationMiscCode();

      var salutationSelectList = salutionList.Select(salutation => new SelectListItem
      {
        Text = salutation.Text,
        Value = salutation.Value,
        Selected = salutation.Value == selectedValue
      }).ToList();

      return html.DropDownList(controlId, salutationSelectList, PleaseSelectText, htmlAttributes);
    }


    public static MvcHtmlString UserStatusDropdownList<TModel, TValue>(this HtmlHelper<TModel> html,
                                                                       Expression<Func<TModel, TValue>> expression,
                                                                        string selectedValue,
                                                                        object htmlAttributes = null)
    {

      var userStatusList = EnumMapper.GetUserStatusMiscCode();

      var statusSelectList = userStatusList.Select(status => new SelectListItem
      {
        Text = status.Text,
        Value = status.Value,
        Selected = status.Value == selectedValue
      }).ToList();

      return html.DropDownListFor(expression, statusSelectList, htmlAttributes);

    }


    public static MvcHtmlString UnlinkedSupportingDocumentFormCYNDropdownList(this HtmlHelper html, string name, object htmlAttributes = null)
    {
      var formCYNList = new List<SelectListItem>
                                     {
                                         
                                       new SelectListItem
                                         {
                                           Text = "No",
                                           Value = "N"
                                         },
                                       new SelectListItem
                                         {
                                           Text = "Yes",
                                           Value = "Y"
                                         }
                                     };

      return html.DropDownList(name, formCYNList, PleaseSelectText, htmlAttributes);
    }


    /// <summary>
    /// Returns Billing Year Month dropdown
    /// </summary>
    /// <param name="html"></param>
    /// <param name="name"></param>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static MvcHtmlString UnlinkedSupportingDocumentBillingYearMonthDropdown(this HtmlHelper html, string name, int year, int month, TransactionMode mode = TransactionMode.InvoiceSearch)
    {
      var bilingMonthSelectList = new List<SelectListItem>();

      var currentBillingPeriod = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager)).GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich); // GetCurrentBillingPeriod();
      var selectedPeriod = year != 0 && month != 0 ? string.Format("{0}-{1}", year, month) : PleaseSelectText;

      // Convert DateTime format of CurrentBillingPeriod
      var currentBillingInDateTimeFormat = new DateTime(currentBillingPeriod.Year, currentBillingPeriod.Month, currentBillingPeriod.Period);

      var counter = 0;
      if (mode == TransactionMode.InvoiceSearch) --counter;

      for (var i = counter; i < 12; i++)
      {
        var previousMonth = currentBillingInDateTimeFormat.AddMonths(-i);

        var strmonth = previousMonth.ToString(FormatConstants.MonthNameFormat);
        var stryear = previousMonth.Year.ToString();
        var monthValue = previousMonth.Month.ToString();

        bilingMonthSelectList.Add(new SelectListItem
        {
          Text = string.Format("{0}-{1}", stryear, strmonth),
          Value = string.Format("{0}-{1}", stryear, monthValue),
          Selected = string.Format("{0}-{1}", stryear, monthValue) == selectedPeriod
        });
      }

      return html.DropDownList(name, bilingMonthSelectList, PleaseSelectText);
    }

    public static MvcHtmlString GetSystemParameterGroups<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
    {
      var systemParamGroup = Ioc.Resolve<ImanageSystemParameter>(typeof(ImanageSystemParameter)).GetSystemParameterGroup();

      var statusSelectList = systemParamGroup.Select(status => new SelectListItem
     {
       Text = status.Value,
       Value = status.Key

     }).ToList();

      return html.DropDownListFor(expression, statusSelectList, PleaseSelectText, htmlAttributes);
    }


    /// <summary>
    /// Settlement method statusdropdown list.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="controlId">The control id.</param>
    /// <param name="controlValue">The control value.</param>
    /// <returns></returns>
    public static MvcHtmlString SettlementMethodStatusropdownList(this HtmlHelper html, string controlId, int controlValue)
    {
      var SettlementMethodSelectList = EnumMapper.GetSettlementMethodStatusList();
      var selectedValue = controlValue == 0 ? (int)SettlementMethodValues.Ach : controlValue;
      SettlementMethodSelectList.Insert(0, AllListItem);
      SettlementMethodSelectList.SingleOrDefault(item => item.Value == selectedValue.ToString()).Selected = true;
      return MvcHtmlString.Create(DropdownList(html, controlId, SettlementMethodSelectList));
    }

    /// <summary>
    /// Settlement method statusdropdown list for Legal Archieve.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="controlId">The control id.</param>
    /// <param name="controlValue">The control value.</param>
    /// <returns></returns>
    public static MvcHtmlString SettlementMethodStatusDropdownListForLegalArchieve(this HtmlHelper html, string controlId, int controlValue)
    {
      var settlementMethodList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetSettlementMethodList();
      var settlementMethodSelectList = settlementMethodList.Select(item => new SelectListItem() { Text = item.Value, Value = item.Key.ToString() }).ToList();
      var selectedValue = controlValue == 0 ? (int)SettlementMethodValues.Ach : controlValue;
      settlementMethodSelectList.Insert(0, AllListItem);
      // settlementMethodSelectList.RemoveRange(6, 2);
      settlementMethodSelectList.SingleOrDefault(item => item.Value == selectedValue.ToString()).Selected = true;
      return MvcHtmlString.Create(DropdownList(html, controlId, settlementMethodSelectList));
    }

    /// <summary>
    /// Settlement method statusdropdown list.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="controlId">The control id.</param>
    /// <param name="controlValue">The control value.</param>
    /// <returns></returns>
    public static MvcHtmlString SettlementMethodStatusDropdownList(this HtmlHelper html, string controlId, int controlValue)
    {
      var settlementMethodList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetSettlementMethodList();
      var settlementMethodSelectList = settlementMethodList.Select(item => new SelectListItem() { Text = item.Value, Value = item.Key.ToString() }).ToList();

      var selectedValue = controlValue == 0 ? (int)SettlementMethodValues.Ach : controlValue;
      settlementMethodSelectList.Insert(0, AllListItem);
      settlementMethodSelectList.Insert(3, IchAndAchItem);
      settlementMethodSelectList.SingleOrDefault(item => item.Value == selectedValue.ToString()).Selected = true;

      return MvcHtmlString.Create(DropdownList(html, controlId, settlementMethodSelectList));
    }

    /// <summary>
    /// Settlement method dropdown list.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString SettlementMethodDropdownListForReport<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
    {
      var settlementMethodList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetSettlementMethodList();
      var settlementMethodSelectList = settlementMethodList.Select(item => new SelectListItem() { Text = item.Value, Value = item.Key.ToString() }).ToList();

      settlementMethodSelectList.Insert(0, AllListItem);
      settlementMethodSelectList.Insert(3, IchAndAchItem);

      return html.DropDownListFor(expression, settlementMethodSelectList, htmlAttributes);
    }

    /// <summary>
    /// Error Type dropdown list.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="controlId">The control id.</param>
    /// <param name="controlValue">The control value.</param>
    /// <returns></returns>
    public static MvcHtmlString ErrorTypeDropdownList(this HtmlHelper html, string controlId, int controlValue)
    {
      var ErrorTypeSelectList = EnumMapper.GetErrorTypeList();
      var selectedValue = controlValue == 0 ? (int)ErrorType.Correctable : controlValue;
      ErrorTypeSelectList.Insert(0, AllListItem);
      ErrorTypeSelectList.SingleOrDefault(item => item.Value == selectedValue.ToString()).Selected = true;
      return MvcHtmlString.Create(DropdownList(html, controlId, ErrorTypeSelectList));
    }

    /// <summary>
    /// Memoes the type dropdownlist for.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString MemoTypeReportDropdownlistForReport<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
    {
      var invoicePeriodList = EnumMapper.GetMemoListForReport();

      invoicePeriodList.Insert(0, AllListItem);

      return html.DropDownListFor(expression, invoicePeriodList, htmlAttributes);
    }

    /// <summary>
    /// Creates Resubmission Status dropdown and sets its selected value.
    /// </summary>
    /// <param name="html">The html.</param>
    /// <param name="controlId">The ControlId.</param>
    /// <param name="controlValue"></param>
    /// <param name="resubmissinoStatusIds"></param>
    /// <returns></returns>

    public static MvcHtmlString SandboxRequestTyepDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string categoryType = "", object htmlAttributes = null)
    {
      var requestTypeList = new List<SelectListItem>
                                     {
                                         
                                       new SelectListItem
                                         {
                                           Text = "All",
                                           Value = "0"
                                         },
                                       new SelectListItem
                                         {
                                           Text = "ST",
                                           Value = "1"
                                         },
                                       new SelectListItem
                                         {
                                           Text = "CT",
                                           Value = "2"
                                         }
                                     };

      return html.DropDownListFor(expression, requestTypeList, htmlAttributes);
    }

    /// <summary>
    /// Creates Resubmission Status dropdown and sets its selected value.
    /// </summary>
    /// <param name="html">The html.</param>
    /// <param name="controlId">The ControlId.</param>
    /// <param name="controlValue"></param>
    /// <param name="resubmissinoStatusIds"></param>
    /// <returns></returns>

    public static MvcHtmlString SandboxGroupStatusDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string categoryType = "", object htmlAttributes = null)
    {
      var groupStatusList = new List<SelectListItem>
                                     {
                                       new SelectListItem
                                         {
                                           Text = "All",
                                           Value = "0"
                                         },
                                         
                                       new SelectListItem
                                         {
                                           Text = "Passed",
                                           Value = "1"
                                         },
                                       new SelectListItem
                                         {
                                           Text = "Failed",
                                           Value = "2"
                                         }                                       
                                     };

      return html.DropDownListFor(expression, groupStatusList, htmlAttributes);
    }

    /// <summary>
    /// Get the UMO Code type list
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString UOMCodeTypeDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html,
                                                                           Expression<Func<TModel, TValue>> expression,
                                                                           object htmlAttributes = null)
    {
      // Get TransactionType Data
      var UOMCodeTypeList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetUomCodeTypeList();

      var UOMCodeTypeSelectList = UOMCodeTypeList.Select(Group => new SelectListItem
      {
        Text = Group.Description,
        Value = Group.Name.ToString()
      }).ToList();
      return html.DropDownListFor(expression, UOMCodeTypeSelectList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// To show up Other charge code list
    /// </summary>
    /// <param name="html"></param>
    /// <param name="controlId"></param>
    /// <param name="selectedValue"></param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    public static MvcHtmlString OtherChargeCodeDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html,
                                                                           Expression<Func<TModel, TValue>> expression,
                                                                           object htmlAttributes = null)
    {
      var otherChargeCodeList = EnumMapper.GetOtherChargeCodeMiscCode();

      var otherChargeCodeSelectList = otherChargeCodeList.Select(salutation => new SelectListItem
      {
        Text = string.Format("{0}-{1}", salutation.Value, salutation.Text),
        Value = salutation.Value,
        // Selected = salutation.Value == selectedValue
      }).ToList();
      return html.DropDownListFor(expression, otherChargeCodeSelectList, PleaseSelectText, htmlAttributes);
      // return html.DropDownList(controlId, otherChargeCodeSelectList, PleaseSelectText, htmlAttributes);
      // return null;
    }

    public static MvcHtmlString KgLbIndDropdownList<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
    {
      var invoicePeriodList = new List<SelectListItem> {
                                                         new SelectListItem {
                                                                              Value = "",
                                                                              Text = ""
                                                                            },
                                                         new SelectListItem {
                                                                              Value = "K",
                                                                              Text = "K - Kgs"
                                                                            },
                                                         new SelectListItem {
                                                                              Value = "L",
                                                                              Text = "L - Pounds"
                                                                            }
                                                        
                                                       }; //EnumMapper.GetInvoicePeriodList();

      return html.DropDownListFor(expression, invoicePeriodList, htmlAttributes);
      // return html.DropDownList(controlName, invoicePeriodList, htmlAttributes);
    }

    public static MvcHtmlString ProvisoreqspaDropdownList<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
    {
      var proReqSpaOptionList = new List<SelectListItem> {
                                                         new SelectListItem {
                                                                              Value = "",
                                                                              Text = ""
                                                                            },
                                                         new SelectListItem {
                                                                              Value = "P",
                                                                              Text = "Proviso"
                                                                            },
                                                         new SelectListItem {
                                                                              Value = "R",
                                                                              Text = "Requirement"
                                                                            },
                                                                            new SelectListItem {
                                                                              Value = "S",
                                                                              Text = "SPA"
                                                                            }
                                                        
                                                       };

      return html.DropDownListFor(expression, proReqSpaOptionList, htmlAttributes);
    }
    /// <summary>
    /// To get Rejection Reason Code List
    /// </summary>
    /// <typeparam name="TModel">The type of the model</typeparam>
    /// <typeparam name="TValue">The type of value</typeparam>
    /// <param name="html">The HTML</param>
    /// <param name="expression">The expression</param>
    /// <param name="htmlAttributes">The HTML attributes</param>
    /// <param name="transactionTypeId">The transaction Type Id</param>  
    /// <param name="code">The Reson Code</param>
    /// <returns></returns>
    public static MvcHtmlString RejectionResonCodeDrodownListFor<TModel, TValue>(this HtmlHelper<TModel> html,
                                                                                Expression<Func<TModel, TValue>> expression, int transactionTypeId = 0,
                                                                                object htmlAttributes = null)
    {
      //var rejectionReasonCodeList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetReasonCodeList(transactionTypeId);

      //var rejectionReasonCodeSelectList = rejectionReasonCodeList.Select(reasoncode => new SelectListItem
      //{
      //    Text = reasoncode.Code,
      //    Value = reasoncode.Id.ToString()
      //}).ToList();

      //return html.DropDownListFor(expression, rejectionReasonCodeSelectList, PleaseSelectText, htmlAttributes);

      var rejectionReasonCodeList = Ioc.Resolve<IReasonCodeManager>(typeof(IReasonCodeManager)).GetRejectionReasonCodeList(transactionTypeId);

      var rejectionReasonCodeSelectList = rejectionReasonCodeList.Select(reasonCode => new SelectListItem
      {
        Text =
             reasonCode.Id.ToString() + "-" + reasonCode.Name,
        Value =
            reasonCode.Id.
            ToString()

      }).ToList();


      return html.DropDownListFor(expression, rejectionReasonCodeSelectList, PleaseSelectText, htmlAttributes);

    }

    public static MvcHtmlString ApplicableMinFieldDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html,
                                                                                 Expression<Func<TModel, TValue>> expression,
                                                                                 bool isForSearch = false,
                                                                                 object htmlAttributes = null)
    {
      var applicableMinFieldSelectList = EnumMapper.GetApplicableMinimumField();

      if (isForSearch)
      {
        applicableMinFieldSelectList.Insert(0, AllListItem);

        return html.DropDownListFor(expression, applicableMinFieldSelectList);
      }

      return html.DropDownListFor(expression, applicableMinFieldSelectList, PleaseSelectText, htmlAttributes);
    }


    /// <summary>
    /// Returns Billing Year Month dropdown
    /// Mainly Use for Search.
    /// </summary>
    /// <param name="html"></param>
    /// <param name="name"></param>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static MvcHtmlString PaxBillingYearMonthDropdown(this HtmlHelper html, string name, int year, int month, int period, TransactionMode mode = TransactionMode.InvoiceSearch, object htmlAttributes = null)
    {
      var calendarManager = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager));
      var bilingMonthSelectList = new List<SelectListItem>();
      var billingPeriods = calendarManager.GetRelevantBillingPeriods(string.Empty, ClearingHouse.Ich, 0);
      var currentBillingPeriod = calendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich); // GetCurrentBillingPeriod();

      // Convert DateTime format of CurrentBillingPeriod
      var currentBillingInDateTimeFormat = new DateTime(currentBillingPeriod.Year, currentBillingPeriod.Month, currentBillingPeriod.Period);

      var selectedPeriod = year != 0 && month != 0 && period != 0 ? string.Format("{0}-{1}-{2}", year, month, period) : string.Format("{0}-{1}-{2}", currentBillingPeriod.Year, currentBillingPeriod.Month, currentBillingPeriod.Period);

      if (billingPeriods.Count > 1) //if late submission is OPEN
      {
        var billingPeriodsTimeFormat = new DateTime(billingPeriods[0].Year, billingPeriods[0].Month, billingPeriods[0].Period);
        var stryear = billingPeriods[0].Year.ToString();
        var strmonth = billingPeriodsTimeFormat.ToString(FormatConstants.MonthNameFormat);
        var strPeriod = billingPeriods[0].Period;
        var monthValue = billingPeriods[0].Month.ToString();

        bilingMonthSelectList.Add(new SelectListItem
        {

          Text = string.Format("{0}-{1}-{2}", stryear, strmonth, strPeriod),
          Value = string.Format("{0}-{1}-{2}", stryear, monthValue, strPeriod),
          Selected =
              string.Format("{0}-{1}-{2}", stryear, monthValue, strPeriod) ==
              selectedPeriod
        });
      }
      if (IsViewMode(html))
      {
        return html.TextBox(name, selectedPeriod);
      }

      var counter = 0;
      if (mode == TransactionMode.InvoiceSearch) --counter;

      for (var i = 0; i < 2; i++)
      {
        // Previously Month was being added to UtcNow Date, instead modified code to add month in DateTime format created from CurrentBilling period.
        var strmonth = currentBillingInDateTimeFormat.AddMonths(i).ToString(FormatConstants.MonthNameFormat);
        var stryear = currentBillingInDateTimeFormat.AddMonths(i).Year.ToString();
        var monthValue = currentBillingInDateTimeFormat.AddMonths(i).Month.ToString();
        int strPeriod = period;

        if (i == 0)
        {
          strPeriod = period;
        }
        else
        {
          strPeriod = 1;
        }
        //var strmonth = currentBillingInDateTimeFormat.AddMonths(i);//ToString(FormatConstants.MonthNameFormat);
        //var monthValue = strmonth.Month.ToString(FormatConstants.MonthNameFormat);
        //var stryear = strmonth.Year;
        //var yearValue = strmonth.ToString(FormatConstants.FullYearFormat);
        for (var j = 0; j < 4; j++)
        {
          if (!bilingMonthSelectList.Exists(item => item.Value == string.Format("{0}-{1}-{2}", stryear, monthValue, strPeriod)))
          {
            bilingMonthSelectList.Add(new SelectListItem
                                          {
                                            Text = string.Format("{0}-{1}-{2}", stryear, strmonth, strPeriod),
                                            Value =
                                                string.Format("{0}-{1}-{2}", stryear, monthValue, strPeriod),
                                            Selected =
                                                string.Format("{0}-{1}-{2}", stryear, monthValue, strPeriod) ==
                                                selectedPeriod
                                          });
          }
          if (i == 0 && strPeriod == 4)
          {
            j = 4;
          }
          strPeriod++;
        }
      }

      return html.DropDownList(name, bilingMonthSelectList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Returns Billing Year Month dropdown
    /// Mainly Use for Search.
    /// </summary>
    /// <param name="html"></param>
    /// <param name="name"></param>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static MvcHtmlString PaxSamplingBillingYearMonthDropdown(this HtmlHelper html, string name, int year, int month, TransactionMode mode = TransactionMode.InvoiceSearch)
    {
      var bilingMonthSelectList = new List<SelectListItem>();

      var currentBillingPeriod = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager)).GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich); // GetCurrentBillingPeriod();

      // Convert DateTime format of CurrentBillingPeriod
      var currentBillingInDateTimeFormat = new DateTime(currentBillingPeriod.Year, currentBillingPeriod.Month, currentBillingPeriod.Period);

      var selectedPeriod = year != 0 && month != 0 ? string.Format("{0}-{1}", year, month) : string.Format("{0}-{1}", currentBillingPeriod.Year, currentBillingPeriod.Month);

      if (IsViewMode(html))
      {
        return html.TextBox(name, selectedPeriod);
      }

      var counter = 0;
      if (mode == TransactionMode.InvoiceSearch) --counter;

      for (var i = 0; i < 4; i++)
      // for (var i = 0; i < 15; i++)
      {
        var strmonth = currentBillingInDateTimeFormat.AddMonths(counter).ToString(FormatConstants.MonthNameFormat);
        var stryear = currentBillingInDateTimeFormat.AddMonths(counter).Year.ToString();
        var monthValue = currentBillingInDateTimeFormat.AddMonths(counter).Month.ToString();
        if (i == 0)
        {
          selectedPeriod = string.Format("{0}-{1}", stryear, monthValue);
        }
        bilingMonthSelectList.Add(new SelectListItem
        {
          Text = string.Format("{0}-{1}", stryear, strmonth),
          Value = string.Format("{0}-{1}", stryear, monthValue),
          Selected = string.Format("{0}-{1}", stryear, monthValue) == selectedPeriod
        });
        counter--;
      }

      return html.DropDownList(name, bilingMonthSelectList, PleaseSelectText);
    }

    /// <summary>
    /// Returns Billing Type dropdown
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    public static MvcHtmlString BillingTypesDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
    {
      // Get Billing Type Data
      var bilingYearSelectList = new List<SelectListItem>();

      for (var i = 1; i < 2; i++)
      {
        if (i.Equals(1))
        {
          var types = "Receivable";
          bilingYearSelectList.Add(new SelectListItem
                                       {
                                         Text = types,
                                         Value = i.ToString()
                                       });
        }
        if (i.Equals(2))
        {
          var types = "Payable";
          bilingYearSelectList.Add(new SelectListItem
          {
            Text = types,
            Value = i.ToString()
          });
        }

      }

      return html.DropDownListFor(expression, bilingYearSelectList, PleaseSelectText, htmlAttributes);
    }

    /// <summary>
    /// Returns billing year for Legal archieve
    /// </summary>
    /// <param name="html"></param>
    /// <param name="controlId"></param>
    /// <param name="selectedValue"></param>
    /// <returns></returns>
    public static MvcHtmlString BillingYearDropdownListForLegalArchieve(this HtmlHelper html, string controlId, int selectedValue)
    {
      // SCP221779: old invoices in SIS [Billing Year Dropdown value does't holds during Page post]
      //var currentBillingPeriod = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager)).GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
      var selectedYear = selectedValue;// currentBillingPeriod.Year;

      // Get Billing Period Data
      var bilingYearSelectList = new List<SelectListItem>();

      for (var i = 0; i < 10; i++)
      {
        var year = DateTime.UtcNow.AddYears(-i).ToString(FormatConstants.FullYearFormat);
        bilingYearSelectList.Add(new SelectListItem
        {
          Text = year,
          Value = year
        });

        // Years before "2011" should not be displayed in dropdown list 
        if (year == "2011")
          break;
      }

      bilingYearSelectList.SingleOrDefault(item => item.Value == selectedYear.ToString()).Selected = true;

      return MvcHtmlString.Create(DropdownList(html, controlId, bilingYearSelectList));
    }

    /// <summary>
    /// Creates country code dropdown and sets its selected value.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="htmlAttributes">HTML attributes for dropdown.</param>
    /// <returns></returns>
    public static MvcHtmlString CountryCodeDropdownListForLegalArchieve<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes = null)
    {
      IList<Country> countryCodeList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetCountryList();

      List<SelectListItem> countrySelectList = countryCodeList.Select(countryCode => new SelectListItem
      {
        Text = countryCode.Name,
        Value = countryCode.Id.ToString()
      }).ToList();

      return html.DropDownListFor(expression, countrySelectList, AllText, htmlAttributes);
    }

    /// <summary>
    /// Returns Billing Type list
    /// </summary>
    /// <param name="html"></param>
    /// <param name="controlId"></param>
    /// <param name="selectedValue"></param>
    /// <returns></returns>

    public static MvcHtmlString BillingTypeDropdownListForLegalArchieve(this HtmlHelper html, string controlId, int selectedValue)
    {
      var billingTypeList = EnumMapper.GetBillingTypeList();
      var Value = selectedValue == 0 ? "" : selectedValue.ToString();
      billingTypeList.Insert(0, PleaseSelectListItem);

      billingTypeList.SingleOrDefault(item => item.Value == Value).Selected = true;

      return MvcHtmlString.Create(DropdownList(html, controlId, billingTypeList));
    }

    /// <summary>
    /// Creates Validaion params dropdown and sets its selected value.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    public static MvcHtmlString ValidationParamDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
    {
      var notificationTypes = new List<SelectListItem>
                                     {
                                         
                                       new SelectListItem
                                         {
                                           Text = ValidationParams.Warning.ToString(),
                                           Value = ((int)ValidationParams.Warning).ToString()
                                         },
                                       new SelectListItem
                                         {
                                           Text = ValidationParams.Error.ToString(),
                                           Value = ((int)ValidationParams.Error).ToString()
                                         }
                                     };

      return html.DropDownListFor(expression, notificationTypes, htmlAttributes);
    }

    // CMP # 533: RAM A13 New Validations and New Charge Code [Start]
    /// <summary>
    /// Creates Product Id dropdown.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="htmlAttributes"></param>
    /// <returns> Dropdown for Product Id</returns>
    public static MvcHtmlString ProductIdDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
    {
      var productIdSelectedList = EnumMapper.GetProductIdList();

      return html.DropDownListFor(expression, productIdSelectedList, "", htmlAttributes);
    }
    // CMP # 533: RAM A13 New Validations and New Charge Code [End]

    /// <summary>
    /// 
    /// </summary>
    /// <param name="html"></param>
    /// <param name="controlId"></param>
    /// <returns></returns>
    public static MvcHtmlString OfflineReportDropdown(this HtmlHelper html, string controlId)
    {
      //var OfflineReportList = 
      var offlineReportList = EnumMapper.GetOfflineReportList();

      //var selectedValue = controlValue == 0 ? 1 : controlValue;
      offlineReportList.Insert(0, AllListItem);

      offlineReportList.FirstOrDefault(item => item.Value == "-1").Selected = true;

      return MvcHtmlString.Create(DropdownList(html, controlId, offlineReportList));
    }

    /// <summary>
    /// This function is used to get dropdown value for charge code type requirement.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    //CMP #636: Standard Update Mobilization
    public static MvcHtmlString ChargeCodeTypeReqDropdownList<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
    {
      var chargeCodeTypeReqList = new List<SelectListItem> {
                                                         new SelectListItem {
                                                                              Value = "",
                                                                              Text = PleaseSelectText,
                                                                              Selected = true
                                                                            },
                                                         new SelectListItem {
                                                                              Value = "True",
                                                                              Text = "Mandatory"
                                                                            },
                                                         new SelectListItem {
                                                                              Value = "False",
                                                                              Text = "Optional"
                                                                            }
                                                        
                                                       };

      return html.DropDownListFor(expression, chargeCodeTypeReqList);
    }

    /// <summary>
    /// This function is used to load dropdown list based on master charge code type requirement.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    //CMP #636: Standard Update Mobilization
    public static MvcHtmlString ChargeCategoryDropdownListForMstChargeCodeType<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, bool isActiveChargeCodeTypeReq, object htmlAttributes = null)
    {
      var chargeCategoryList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetChargeCategoriesForMstChargeCodeType(isActiveChargeCodeTypeReq);

      var chargeCategorySelectList = chargeCategoryList.Select(chargeCategory => new SelectListItem
      {
        Text = chargeCategory.Name,
        Value = chargeCategory.Id.ToString()
      }).ToList();

      if (isActiveChargeCodeTypeReq)
        chargeCategorySelectList.Insert(0, new SelectListItem
                                                            {
                                                              Value = "0",
                                                              Text = PleaseSelectText,
                                                              Selected = true
                                                            });
      else
        chargeCategorySelectList.Insert(0, new SelectListItem
                                                            {
                                                              Value = "0",
                                                              Text = AllText,
                                                              Selected = true
                                                            });

      return html.DropDownListFor(expression, chargeCategorySelectList, htmlAttributes);
    }

    /// <summary>
    /// Creates the Charge code dropdown list.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="controlName">Name of the control.</param>
    /// <param name="chargeCodeId">The charge code id.</param>
    /// <param name="chargeCategoryId">The charge category id.</param>
    /// <returns></returns>
    //CMP #636: Standard Update Mobilization
    public static MvcHtmlString ChargeCodeDropdownListForMstChargeCodeType<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, int chargeCategoryId, bool isActiveChargeCodeTypeReq = false, object htmlAttributes = null)
    {
      var chargeCodeList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetChargeCodeListForMstChargeCodeType(chargeCategoryId, isActiveChargeCodeTypeReq);

      var chargeCodeSelectedList = chargeCodeList.Select(chargeCode => new SelectListItem
      {
        Text = chargeCode.Name,
        Value = chargeCode.Id.ToString()
      }).ToList();

      if (isActiveChargeCodeTypeReq)
        chargeCodeSelectedList.Insert(0, new SelectListItem
        {
          Value = "0",
          Text = PleaseSelectText,
          Selected = true
        });
      else
        chargeCodeSelectedList.Insert(0, new SelectListItem
        {
          Value = "0",
          Text = AllText,
          Selected = true
        });

      return html.DropDownListFor(expression, chargeCodeSelectedList, htmlAttributes);
    }

    /// <summary>
    /// CMP #642: Show Appropriate Currency Decimals in MISC PDFs
    ///  Currency Precision the select dropdown list.
    /// </summary>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">expression.</param>
    /// <param name="htmlAttributes">The HTML attributes.</param>
    /// <returns></returns>
    public static MvcHtmlString CurrencyPrecisionDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
    {
      var currencyPrecisionList = new List<SelectListItem>();
      //currencyPrecisionList.Add(new SelectListItem { Text = PleaseSelectText, Value = "-1" });
      currencyPrecisionList.Add(new SelectListItem { Text = "0", Value = "0" });
      currencyPrecisionList.Add(new SelectListItem { Text = "1", Value = "1" });
      currencyPrecisionList.Add(new SelectListItem { Text = "2", Value = "2" });
      currencyPrecisionList.Add(new SelectListItem { Text = "3", Value = "3" });

      return html.DropDownListFor(expression, currencyPrecisionList, PleaseSelectText, htmlAttributes);
    }

    //InvoiceTypeDictionary

    /// <summary>
    /// Creates the dropdown list for Transaction Type.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    /// CMP #663: MISC Invoice Summary Reports - Add 'Transaction Type'
    /// 
    public static MvcHtmlString Transaction_TypeDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html,
                                                                                  Expression<Func<TModel, TValue>> expression,
                                                                                  bool isForSearch = false,
                                                                                  object htmlAttributes = null)
    {

        var invoiceTypeList = EnumMapper.GetInvoiceTypeList();

        if (isForSearch)
        {
            invoiceTypeList.Insert(0, AllListItem);
            return html.DropDownListFor(expression, invoiceTypeList);
        }
        return html.DropDownListFor(expression, invoiceTypeList, AllText, htmlAttributes);
    }

    /// <summary>
    /// Creates the dropdown list for ACH currency.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    /// CMP #553: ACH Requirement for Multiple Currency Handling
    public static MvcHtmlString AchCurrencyDropdownList<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
    {
        var currencyList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetCurrencyListForAchCurrencySetUp();

        var currencySelectList = currencyList.Select(currency => new SelectListItem
        {
            Text = currency.Code,
            Value = currency.Id.ToString() + "|" + currency.Name
            
        }).ToList();

        currencySelectList.Insert(0, new SelectListItem
         {
             Value = "",
             Text = "",
             Selected = true
         });

        return html.DropDownListFor(expression, currencySelectList, htmlAttributes);
    }

    /// <summary>
    /// Create the Invoice Payment Status Applicable For Dropdown List
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="categoryType"></param>
    /// <param name="htmlAttributes"></param>
    /// <param name="isAll"></param>
    /// <returns></returns>
    public static MvcHtmlString InvPaymentStatusApplicableForDropdownListFor<TModel, TValue>(
                                      this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression,
                                      string categoryType = "",
                                      object htmlAttributes = null,
                                      bool isAll = true)
    {
        var invPaymentStatusApplicalbeForSelectList = EnumMapper.GetInvPaymentStatusApplicableForList();

        if (isAll)
        {
            invPaymentStatusApplicalbeForSelectList.Insert(0,
                                      new SelectListItem
                                      {
                                          Value = "-1",
                                          Text = AllText
                                      });
        }
        if (categoryType.ToLower() == "applicablefor")
        {
            invPaymentStatusApplicalbeForSelectList.Insert(0,
                                   new SelectListItem
                                   {
                                       Value = "0",
                                       Text = PleaseSelectText
                                   });
            invPaymentStatusApplicalbeForSelectList.RemoveAt(1);
        }

        return html.DropDownListFor(expression, invPaymentStatusApplicalbeForSelectList, htmlAttributes);

    }
  }
}

/*

    /// <summary>
    /// Creates Coupon Number dropdown and sets its selected value.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    public static MvcHtmlString CouponNumberDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
    {
      var couponNumberList = new List<SelectListItem>
                               {
                                 new SelectListItem
                                   {
                                     Text = "1",
                                     Value = "1"
                                   },
                                 new SelectListItem
                                   {
                                     Text = "2",
                                     Value = "2"
                                   },
                                 new SelectListItem
                                   {
                                     Text = "3",
                                     Value = "3"
                                   },
                                 new SelectListItem
                                   {
                                     Text = "4",
                                     Value = "4"
                                   },
                                 new SelectListItem
                                   {
                                     Text = "9",
                                     Value = "9"
                                   },
                               };

      return html.DropDownListFor(expression, couponNumberList, PleaseSelectText);
    }
 
    /// <summary>
    /// Creates Handling Fee dropdown and sets its selected value.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    public static MvcHtmlString HandlingFeeTypeDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
    {
      var handlingFeeTypeList = EnumMapper.GetHandlingFeeTypeList();

      return html.DropDownListFor(expression, handlingFeeTypeList, PleaseSelectText);
    }

    /// <summary>
    /// Returns the currency codes with default as the currency code for the main location of the billing member.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html"></param>
    /// <param name="expression"></param>
    /// <param name="memberId">Billing member ID</param>
    /// <param name="htmlAttributes">Html attributes for the dropdown if any.</param>
    /// <returns>HTML for the dropdown</returns>
    public static MvcHtmlString MiscBillingCurrencyDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, int memberId, object htmlAttributes = null)
    {
      var currencySelectList = new List<SelectListItem>();
      var location = Ioc.Resolve<IMemberManager>(typeof(IMemberManager)).GetMemberDefaultLocation(memberId, DefaultLocationCode);

      if (location != null)
      {
        // Default selection should be currency code of the main location country of the Billing member
        var currencyId = location.CurrencyId.HasValue ? location.CurrencyId.Value : 0;

        // Get Currency Data
        currencySelectList = GetCurrencySelectList().ToList();

        if (currencyId > 0)
        {
          currencySelectList.SingleOrDefault(currencySelectItem => currencySelectItem.Value == currencyId.ToString()).Selected = true;
        }
      }

      return html.DropDownListFor(expression, currencySelectList, PleaseSelectText, htmlAttributes);
    }


    /// <summary>
    /// Creates digital signature dropdown and sets its selected value.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    public static MvcHtmlString DigitalSignatureDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
    {
      var digitalSignatureList = EnumMapper.GetDigitalSignatureList();

      digitalSignatureList.Insert(0,
                                  new SelectListItem
                                    {
                                      Value = "-1",
                                      Text = PleaseSelectText
                                    });

      return html.DropDownListFor(expression, digitalSignatureList);
    }

    /// <summary>
    /// Creates billing currency dropdown and sets its selected value.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="html">The HTML.</param>
    /// <param name="expression">The expression.</param>
    /// <param name="htmlAttributes">Html attributes.</param>
    /// <returns />
    public static MvcHtmlString BillingCurrencyDropdownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
    {
      var billingCurrencyList = EnumMapper.GetBillingCurrencyList();

      return html.DropDownListFor(expression, billingCurrencyList, PleaseSelectText, htmlAttributes);
    }

*/

