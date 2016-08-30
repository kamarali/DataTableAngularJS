using System;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax.BillingHistory;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Core.DI;
using Iata.IS.Business.Security;
using Iata.IS.Business.Common;

namespace Iata.IS.Web.Util
{
  public class ScriptHelper
  {
    /// <summary>
    /// This method will generate script that will provide edit, View and delete link for grid.
    /// </summary>
    /// <param name="uri">Uri object to get relative path of images</param>
    /// <param name="jqGridId">grid id</param>
    /// <param name="actionEdit">action method name for Edit</param>
    /// <param name="actionView">action method name for View</param>
    /// <param name="actionDelete">action method name for Delete</param>
    public static MvcHtmlString GenerateGridEditViewDeleteScript(UrlHelper uri, string jqGridId, string actionEdit = null, string actionView = null, string actionDelete = null)
    {
      var jqGridMethodName = string.Format("{0}_EditViewDeleteRecord(cellValue, options, rowObject)", jqGridId);
      string deleteMethod = null;

      if (!string.IsNullOrEmpty(actionDelete))
      {
        deleteMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", actionDelete, "#" + jqGridId);
      }

      var sb = new StringBuilder();
      sb.Append("<script type='text/javascript'>");
      sb.AppendFormat("function {0} {{", jqGridMethodName);
      sb.Append("return '");

      if (!string.IsNullOrEmpty(actionEdit))
      {
        sb.AppendFormat("<a style=cursor:hand target=_parent href={0}/'+cellValue+'> <img title=Edit alt=Edit style=border-style:none src={1} /></a>&nbsp; ",
                        uri.Action(actionEdit),
                        uri.Content("~/Content/Images/edit.png"));
      }

      if (!string.IsNullOrEmpty(actionView))
      {
        sb.AppendFormat("<a style=cursor:hand target=_parent href={0}/'+cellValue+'> <img title=View alt=View style=border-style:none src={1} /></a>&nbsp; ",
                        uri.Action(actionView),
                        uri.Content("~/Content/Images/view.png"));
      }

      if (!string.IsNullOrEmpty(actionDelete))
      {
        sb.AppendFormat("<a style=cursor:hand target=_parent href=javascript:deleteRecord({0});><img title=Delete style=border-style:none src={1} /></a> ",
                        deleteMethod,
                        uri.Content("~/Content/Images/delete.png"));
      }

      sb.AppendFormat("'");

      sb.Append("} </script>");

      return MvcHtmlString.Create(sb.ToString());
    }

    public static MvcHtmlString GenerateGridEditDeleteScript(UrlHelper uri, string jqGridId, string actionEdit, string actionDelete)
    {
      var jqGridEditMethodName = string.Format("{0}_EditViewDeleteRecord(cellValue, options, rowObject)", jqGridId); // DO NOT CHANGE THE SCRIPT METHOD NAME
      var deleteMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", actionDelete, "#" + jqGridId);
      var sb = new StringBuilder();
      sb.Append("<script type='text/javascript'>");
      sb.AppendFormat("function {0} {{", jqGridEditMethodName);
      sb.AppendFormat(
        "return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=Edit alt=Edit style=border-style:none src={1} /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:deleteRecord({2});><img title=Delete style=border-style:none src={3} /></a>';",
        actionEdit,
        uri.Content("~/Content/Images/edit.png"),
        deleteMethod,
        uri.Content("~/Content/Images/delete.png"));
      sb.Append("} </script>");
      return MvcHtmlString.Create(sb.ToString());
    }

    /// <summary>
    /// Generates the grid edit active/deactivate script.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="jqGridId">The jqGrid id.</param>
    /// <param name="actionEdit">The action edit.</param>
    /// <param name="actionDelete">The action delete.</param>
    /// <returns></returns>
    public static MvcHtmlString GenerateGridEditActiveDeactiveScript(UrlHelper uri, string jqGridId, string actionEdit, string actionDelete, bool flag)
    {
      var jqGridEditMethodName = string.Format("{0}_EditViewDeleteRecord(cellValue, options, rowObject)", jqGridId); // DO NOT CHANGE THE SCRIPT METHOD NAME
      var deleteMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", actionDelete, "#" + jqGridId);
      var sb = new StringBuilder();
      sb.Append("<script type='text/javascript'>");
      sb.AppendFormat("function {0} {{", jqGridEditMethodName);

      //sb.AppendFormat("return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=Edit alt=Edit style=border-style:none src={1} /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:deleteRecord({2});><img title=Delete style=border-style:none src={3} /></a>';", actionEdit, uri.Content("~/Content/Images/edit.png"), deleteMethod, uri.Content("~/Content/Images/delete.png"));
      //sb.Append("} </script>");
      if (flag == true)
      {
          sb.AppendFormat("if(rowObject.IsActive=='True'){{");
        sb.AppendFormat(
            "return '<a style=cursor:hand target=_parent  href={0}/'+cellValue+'><img title=Edit alt=Edit style=border-style:none src={1} /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:dactivateRecord({2});><img title=Deactivate style=border-style:none src={3} /></a>';",
            actionEdit,
            uri.Content("~/Content/Images/edit.png"),
            deleteMethod,
            uri.Content("~/Content/Images/delete.png"));
        sb.Append("} else {");
        sb.AppendFormat(
            "return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=Edit alt=Edit style=border-style:none src={1} /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:activateRecord({2});><img title=Activate style=border-style:none src={3} /></a>';",
            actionEdit,
            uri.Content("~/Content/Images/edit.png"),
            deleteMethod,
            uri.Content("~/Content/Images/Validate.png"));
      }
      else
      {
          sb.AppendFormat("if(rowObject.IsActive=='True'){{");
        sb.AppendFormat(
            "return '<a style=display:none target=_parent  href={0}/'+cellValue+'><img title=Edit alt=Edit style=border-style:none src={1} /></a>';",
            actionEdit,
            uri.Content("~/Content/Images/edit.png"));
        sb.Append("} else {");
        sb.AppendFormat(
            "return '<a style=display:none target=_parent href={0}/'+cellValue+'><img title=Edit alt=Edit style=border-style:none src={1} /></a>';",
            actionEdit,
            uri.Content("~/Content/Images/edit.png"));
      }
      sb.Append("}}");

      sb.Append("</script>");
      return MvcHtmlString.Create(sb.ToString());
    }

    public static MvcHtmlString GenerateSubDivisionGridScript(UrlHelper uri, string jqGridId, string actionEdit, string actionDelete, bool flag)
    {
      var jqGridEditMethodName = string.Format("{0}_EditViewDeleteRecord(cellValue, options, rowObject)", jqGridId); // DO NOT CHANGE THE SCRIPT METHOD NAME
      var deleteMethod = string.Format("\"{0}\",\"'+rowObject.IsActive+'\",\"{1}\"", actionDelete + "?Id='+cellValue+'&countryId='+rowObject.CountryId+'", "#" + jqGridId);

      var sb = new StringBuilder();
      sb.Append("<script type='text/javascript'>");
      sb.AppendFormat("function {0} {{", jqGridEditMethodName);

      //sb.AppendFormat("return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=Edit alt=Edit style=border-style:none src={1} /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:deleteRecord({2});><img title=Delete style=border-style:none src={3} /></a>';", actionEdit, uri.Content("~/Content/Images/edit.png"), deleteMethod, uri.Content("~/Content/Images/delete.png"));
      //sb.Append("} </script>");
      if (flag == true)
      {
          sb.AppendFormat("if(rowObject.IsActive=='True'){{");
        sb.AppendFormat(
            "return '<a style=cursor:hand target=_parent  href={0}?Id='+cellValue+'&countryId='+rowObject.CountryId+'><img title=Edit alt=Edit style=border-style:none src={1} /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:activatedeactivateRecord({2});><img title=Deactivate style=border-style:none src={3} /></a>';",
            actionEdit,
            uri.Content("~/Content/Images/edit.png"),
            deleteMethod,
            uri.Content("~/Content/Images/delete.png"));
        sb.Append("} else {");
        sb.AppendFormat(
            "return '<a style=cursor:hand target=_parent href={0}?Id='+cellValue+'&countryId='+rowObject.CountryId+'><img title=Edit alt=Edit style=border-style:none src={1} /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:activatedeactivateRecord({2});><img title=Activate style=border-style:none src={3} /></a>';",
            actionEdit,
            uri.Content("~/Content/Images/edit.png"),
            deleteMethod,
            uri.Content("~/Content/Images/Validate.png"));
      }
      else
      {
          sb.AppendFormat("if(rowObject.IsActive=='True'){{");
        sb.AppendFormat(
            "return '<a style=display:none target=_parent  href={0}?Id='+cellValue+'&countryId='+rowObject.CountryId+'><img title=Edit alt=Edit style=border-style:none src={1} /></a>';",
            actionEdit,
            uri.Content("~/Content/Images/edit.png"));
        sb.Append("} else {");
        sb.AppendFormat(
            "return '<a style=display:none target=_parent href={0}?Id='+cellValue+'&countryId='+rowObject.CountryId+'><img title=Edit alt=Edit style=border-style:none src={1} /></a>';",
            actionEdit,
            uri.Content("~/Content/Images/edit.png"));
      }
      sb.Append("}}");

      sb.Append("</script>");
      return MvcHtmlString.Create(sb.ToString());
    }

    public static MvcHtmlString GenerateGridViewActiveDeactiveScript(UrlHelper uri, string jqGridId, string actionView, string actionDelete, bool flag)
    {
      var jqGridEditMethodName = string.Format("{0}_EditViewDeleteRecord(cellValue, options, rowObject)", jqGridId); // DO NOT CHANGE THE SCRIPT METHOD NAME
      var deleteMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", actionDelete, "#" + jqGridId);
      var sb = new StringBuilder();
      sb.Append("<script type='text/javascript'>");
      sb.AppendFormat("function {0} {{", jqGridEditMethodName);
      //sb.AppendFormat("return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=Edit alt=Edit style=border-style:none src={1} /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:deleteRecord({2});><img title=Delete style=border-style:none src={3} /></a>';", actionEdit, uri.Content("~/Content/Images/edit.png"), deleteMethod, uri.Content("~/Content/Images/delete.png"));
      //sb.Append("} </script>");

      if (flag == true)
      {
        sb.AppendFormat("if(rowObject[1]=='True'){{");
        sb.AppendFormat(
            "return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:dactivateRecord({2});><img title=Deactivate style=border-style:none src={3} /></a>';",
            actionView,
            uri.Content("~/Content/Images/view.png"),
            deleteMethod,
            uri.Content("~/Content/Images/delete.png"));
        sb.Append("} else {");
        sb.AppendFormat(
            "return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:activateRecord({2});><img title=Activate style=border-style:none src={3} /></a>';",
            actionView,
            uri.Content("~/Content/Images/view.png"),
            deleteMethod,
            uri.Content("~/Content/Images/Validate.png"));
        sb.Append("}} </script>");
      }
      else
      {
        sb.AppendFormat("if(rowObject[1]=='True'){{");
        sb.AppendFormat(
           "return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a>';",
           actionView,
           uri.Content("~/Content/Images/view.png"));
        sb.Append("} else {");
        sb.AppendFormat(
            "return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a>';",
            actionView,
            uri.Content("~/Content/Images/view.png"));
        sb.Append("}} </script>");
      }


      return MvcHtmlString.Create(sb.ToString());
    }

    public static MvcHtmlString GenerateGridViewScript(UrlHelper uri, string jqGridId, string actionViewUrl)
    {
      var jqGridEditMethodName = string.Format("{0}_EditViewDeleteRecord(cellValue, options, rowObject)", jqGridId); // DO NOT CHANGE THE SCRIPT METHOD NAME

      var sb = new StringBuilder();

      sb.Append("<script type='text/javascript'>");
      sb.AppendFormat("function {0} {{", jqGridEditMethodName);
      sb.AppendFormat("return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a>';",
                      actionViewUrl,
                      uri.Content("~/Content/Images/view.png"));
      sb.Append("} </script>");

      return MvcHtmlString.Create(sb.ToString());
    }

    public static MvcHtmlString GenerateGridViewQuerystringScript(UrlHelper uri, string jqGridId, string actionViewUrl, string querystring)
    {
      var jqGridEditMethodName = string.Format("{0}_EditViewDeleteRecord(cellValue, options, rowObject)", jqGridId); // DO NOT CHANGE THE SCRIPT METHOD NAME

      var sb = new StringBuilder();

      sb.Append("<script type='text/javascript'>");
      sb.AppendFormat("function {0} {{", jqGridEditMethodName);
      sb.AppendFormat("return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'?{2}><img title=View alt=View style=border-style:none src={1} /></a>';",
                      actionViewUrl,
                      uri.Content("~/Content/Images/view.png"), querystring);
      sb.Append("} </script>");

      return MvcHtmlString.Create(sb.ToString());
    }

    public static MvcHtmlString GenerateGridEditScript(UrlHelper uri, string jqGridId, string actionViewUrl, bool flag = true)
    {
      var jqGridEditMethodName = string.Format("{0}_EditRecord(cellValue, options, rowObject)", jqGridId); // DO NOT CHANGE THE SCRIPT METHOD NAME

      var sb = new StringBuilder();

      sb.Append("<script type='text/javascript'>");
      sb.AppendFormat("function {0} {{", jqGridEditMethodName);
      if (flag)
      {
          sb.AppendFormat(
              "return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a>';",
              actionViewUrl,
              uri.Content("~/Content/Images/edit.png"));
      }
      else
      {
          sb.AppendFormat(
              "return '<a style=display:none target=_parent href={0}/'+cellValue+'></a>';",
              actionViewUrl,
              uri.Content("~/Content/Images/edit.png"));
      }
        sb.Append("} </script>");

      return MvcHtmlString.Create(sb.ToString());
    }

    public static MvcHtmlString GenerateGridViewScriptForBreakdown(UrlHelper uri, string jqGridId, string actionView)
    {
      var jqGridEditMethodName = string.Format("{0}_EditViewDeleteRecord(cellValue, options, rowObject)", jqGridId); // DO NOT CHANGE THE SCRIPT METHOD NAME

      var sb = new StringBuilder();

      sb.Append("<script type='text/javascript'>");
      sb.AppendFormat("function {0} {{", jqGridEditMethodName);
      sb.AppendFormat("return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a>';",
                      actionView,
                      uri.Content("~/Content/Images/view.png"));
      sb.Append("} </script>");

      return MvcHtmlString.Create(sb.ToString());
    }

    /// <summary>
    /// This will allow delete record.
    /// </summary>
    public static MvcHtmlString GenerateGridDeleteScript(UrlHelper uri, string jqGridId, string actionDelete,int selectedMemberId = 0)
    {
      var jqGridEditMethodName = string.Format("{0}_DeleteRecord(cellValue, options, rowObject)", jqGridId);
      var deleteMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\",\"{2}\"", actionDelete, "#" + jqGridId,selectedMemberId );

      var sb = new StringBuilder();

      sb.Append("<script type='text/javascript'>");
      sb.AppendFormat("function {0}{{", jqGridEditMethodName);
      sb.AppendFormat("return '<a style=cursor:hand target=_parent href=javascript:deleteRecordMethod({0});><img title=Delete style=border-style:none src={1} /></a>';",
                      deleteMethod,
                      uri.Content("~/Content/Images/delete.png"));
      sb.Append("} </script>");

      return MvcHtmlString.Create(sb.ToString());
    }
    public static MvcHtmlString GenerateGridVatDeleteScript(UrlHelper uri, string jqGridId, string actionDelete, int selectedMemberId = 0)
    {
      var jqGridEditMethodName = string.Format("{0}_DeleteRecord(cellValue, options, rowObject)", jqGridId);
      var deleteMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\",\"{2}\"", actionDelete, "#" + jqGridId, selectedMemberId);

      var sb = new StringBuilder();

      sb.Append("<script type='text/javascript'>");
      sb.AppendFormat("function {0}{{", jqGridEditMethodName);
      sb.AppendFormat("return '<a style=cursor:hand target=_parent href=javascript:deleteRecord({0});><img title=Delete style=border-style:none src={1} /></a>';",
                      deleteMethod,
                      uri.Content("~/Content/Images/delete.png"));
      sb.Append("} </script>");

      return MvcHtmlString.Create(sb.ToString());
    }
    public static MvcHtmlString GenerateCreditorsDebitorsGridDeleteScript(UrlHelper uri, string jqGridId, string actionDelete)
    {
      var jqGridEditMethodName = string.Format("{0}_DeleteRecord(cellValue, options, rowObject)", jqGridId);
      var deleteMethod = string.Format("{0}(\"'+cellValue+'\");", jqGridId.Equals("BlockedCreditorsGrid") ? "DeleteCreditorRowFromDatabase" : "DeleteDebtorRowFromDatabase");

      var sb = new StringBuilder();

      sb.Append("<script type='text/javascript'>");
      sb.AppendFormat("function {0}{{", jqGridEditMethodName);
      sb.AppendFormat("return '<a style=cursor:hand target=_parent href=javascript:{0}> <img title=Delete style=border-style:none src={1} /></a>';",
                      deleteMethod,
                      uri.Content("~/Content/Images/delete.png"));
      sb.Append("} </script>");

      return MvcHtmlString.Create(sb.ToString());
    }

    public static MvcHtmlString GenerateExceptionsGridDeleteScript(UrlHelper uri, string jqGridId, string actionDelete)
    {
      var jqGridEditMethodName = string.Format("{0}_DeleteRecord(cellValue, options, rowObject)", jqGridId);
      var deleteMethod = string.Format("\"'+cellValue+'\"");

      var sb = new StringBuilder();

      sb.Append("<script type='text/javascript'>");
      sb.AppendFormat("function {0}{{", jqGridEditMethodName);
      sb.AppendFormat("return '<a style=cursor:hand target=_parent href=javascript:DeleteExceptionRowFromDatabase({0});><img title=Delete style=border-style:none src={1} /></a>';",
                      deleteMethod,
                      uri.Content("~/Content/Images/delete.png"));
      sb.Append("} </script>");

      return MvcHtmlString.Create(sb.ToString());
    }

    public static MvcHtmlString GenerateBlockByGroupsGridDeleteScript(UrlHelper uri, string jqGridId, string actionDelete)
    {
      var jqGridEditMethodName = string.Format("{0}_DeleteRecord(cellValue, options, rowObject)", jqGridId);
      var deleteMethod = string.Format("\"'+cellValue+'\"");

      var sb = new StringBuilder();

      sb.Append("<script type='text/javascript'>");
      sb.AppendFormat("function {0}{{", jqGridEditMethodName);
      sb.AppendFormat("return '<a style=cursor:hand target=_parent href=javascript:DeleteGroupBlockRowFromDatabase({0});><img title=Delete style=border-style:none src={1} /></a>';",
                      deleteMethod,
                      uri.Content("~/Content/Images/delete.png"));
      sb.Append("} </script>");

      return MvcHtmlString.Create(sb.ToString());
    }

    /// <summary>
    /// Following method is used to generate Delete script for Creditors and Debtors record on ACH tab
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="jqGridId"></param>
    /// <param name="actionDelete"></param>
    /// <returns>Script to delete Creditor and Debtor record.</returns>
    public static MvcHtmlString GenerateAchCreditorsDebitorsGridDeleteScript(UrlHelper uri, string jqGridId, string actionDelete)
    {
      var jqGridEditMethodName = string.Format("{0}_DeleteRecord(cellValue, options, rowObject)", jqGridId);
      var deleteMethod = string.Format("\"'+cellValue+'\"");

      var sb = new StringBuilder();

      sb.Append("<script type='text/javascript'>");
      sb.AppendFormat("function {0}{{", jqGridEditMethodName);
      sb.AppendFormat("return '<a style=cursor:hand target=_parent href=javascript:DeleteAchCreditorRowFromDatabase({0});><img title=Delete style=border-style:none src={1} /></a>';",
                      deleteMethod,
                      uri.Content("~/Content/Images/delete.png"));
      sb.Append("} </script>");

      return MvcHtmlString.Create(sb.ToString());
    }

    public static MvcHtmlString GenerateSearchGridScript(UrlHelper uri, string jqGridId, string actionEdit, string actionView, string actionDelete, string actionValidate)
    {
      var jqGridEditMethodName = string.Format("{0}_EditViewDeleteRecord(cellValue, options, rowObject)", jqGridId);
      var deleteMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", actionDelete, "#" + jqGridId);
      var validateMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", actionValidate, "#" + jqGridId);

      var sb = new StringBuilder();

      sb.Append("<script type='text/javascript'>");
      sb.AppendFormat("function {0} {{", jqGridEditMethodName);
      sb.AppendFormat(
        "return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=Edit alt=Edit style=border-style:none src={1} /></a> &nbsp;<a style=cursor:hand target=_parent href={2}/'+cellValue+'><img title=View alt=View style=border-style:none src={3} /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:validateRecord({4});><img title=Validate alt=View style=border-style:none src={5} /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:deleteRecord({6});><img title=Delete style=border-style:none src={7} /></a>';",
        actionEdit,
        uri.Content("~/Content/Images/edit.png"),
        uri.Action(actionView),
        uri.Content("~/Content/Images/view.png"),
        validateMethod,
        uri.Content("~/Content/Images/validate.png"),
        deleteMethod,
        uri.Content("~/Content/Images/delete.png"));
      sb.Append("} </script>");

      return MvcHtmlString.Create(sb.ToString());
    }

    /// <summary>
    /// This function is uses to generate Script for Form C grid.
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="jqGridId"></param>
    /// <param name="actionEdit"></param>
    /// <param name="actionView"></param>
    /// <param name="actionValidate"></param>
    /// <param name="actionDelete"></param>
    /// <param name="actionDownloadZip"></param>
    /// <param name="rejectionOnValidationFlag"></param>
    /// <returns></returns>
    //SCP419601: PAX permissions issue
    public static MvcHtmlString GenerateFormCGridEditViewDeleteScript(UrlHelper uri, string jqGridId, string actionEdit, string actionView, string actionValidate, string actionDelete, string actionDownloadZip, int rejectionOnValidationFlag)
    {
      string jqGridEditMethodName = string.Format("{0}_EditViewDeleteRecord(cellValue, options, rowObject)", jqGridId);

      // Indices: 8-ProvisionalBillingYear, 9-ProvisionalBillingMonth,10-ProvisionalBillingMemberId,11-FromMemberId,12-ListingCurrencyId,13-InvoiceStatusId

      // Methods to be invoked when listing currency is not null.
      // SCP155930: FORM C APRIL and MAY 2013
      // for only delete method we carrying .net FormC Id in fromMembtId variable.
      string deleteMethodWithListingCurrency = string.Format("\"{0}\",'+rowObject.ProvisionalBillingYear +','+rowObject.ProvisionalBillingMonth+','+rowObject.ProvisionalBillingMemberId+',\"'+rowObject.FormCId+'\",'+rowObject.ListingCurrencyId+','+rowObject.InvoiceStatusId+',\"{1}\"", actionDelete, "#" + jqGridId);
      string validateMethodWithListingCurrency = string.Format("\"{0}\",'+rowObject.ProvisionalBillingYear +','+rowObject.ProvisionalBillingMonth+','+rowObject.ProvisionalBillingMemberId+','+rowObject.FromMemberId+','+rowObject.ListingCurrencyId+','+rowObject.InvoiceStatusId+',\"{1}\"", actionValidate, "#" + jqGridId);

      // Methods to be invoked when listing currency is null.
      // SCP155930: FORM C APRIL and MAY 2013
      // for only delete method we carrying .net FormC Id in fromMembtId variable.
      string deleteMethod = string.Format("\"{0}\",'+rowObject.ProvisionalBillingYear+','+rowObject.ProvisionalBillingMonth+','+rowObject.ProvisionalBillingMemberId+',\"'+rowObject.FormCId+'\",\"NO_VALUE\",'+rowObject.InvoiceStatusId+',\"{1}\"", actionDelete, "#" + jqGridId);
      string validateMethod = string.Format("\"{0}\",'+rowObject.ProvisionalBillingYear+','+rowObject.ProvisionalBillingMonth+','+rowObject.ProvisionalBillingMemberId+','+rowObject.FromMemberId+',\"NO_VALUE\",'+rowObject.InvoiceStatusId+',\"{1}\"", actionValidate, "#" + jqGridId);

      var downloadZipMethodParams = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", actionDownloadZip, "#" + jqGridId);

        //Resolve IUserManager
        var userManager = Ioc.Resolve<IUserManager>();

        // Get the list of user permissions.
        var permissionList = userManager.GetUserPermissions(SessionUtil.UserId);

        //Form C invoice permission
        var hasFormCEditPermission = permissionList.Contains(Business.Security.Permissions.Pax.Receivables.SampleFormC.CreateOrEdit);
        var hasFormCValidatePermission = permissionList.Contains(Business.Security.Permissions.Pax.Receivables.SampleFormC.Validate);
        var hasFormCViewPermission = permissionList.Contains(Business.Security.Permissions.Pax.Receivables.SampleFormC.View);
        var hasFormCDownloadPermission = permissionList.Contains(Business.Security.Permissions.Pax.Receivables.SampleFormC.Download);
      var sb = new StringBuilder();

      sb.Append("<script type='text/javascript'>");
      sb.Append("function " + jqGridEditMethodName + " {");


      //rowObject[12]=>ListingCurrencyId,  rowObject[13]=>InvoiceStatusId  
      //rowObject[14]=>SubmissionMethodId, rowObject[15]=>FileStatusId

      // sb.Append("alert(rowObject[14] + '-' + rowObject[15] + '-' + rowObject[12] + '-' + rowObject[13]);");

      // If Listing currency id is null, do not send it in the URL.
      sb.Append("if(rowObject.ListingCurrencyId == '') ");
      sb.Append("var parameters = rowObject.ProvisionalBillingYear+'/'+rowObject.ProvisionalBillingMonth+'/'+rowObject.ProvisionalBillingMemberId+'/'+rowObject.FromMemberId+'/'+rowObject.InvoiceStatusId;");
      sb.Append("else var parameters = rowObject.ProvisionalBillingYear+'/'+rowObject.ProvisionalBillingMonth+'/'+rowObject.ProvisionalBillingMemberId+'/'+rowObject.FromMemberId+'/'+rowObject.InvoiceStatusId+'/'+rowObject.ListingCurrencyId;");

      // If Form C status is Ready For Submission, show only edit and delete icons.
      sb.AppendFormat("if(rowObject.InvoiceStatusId=={0}){{", (int)InvoiceStatusType.ReadyForSubmission);

      sb.Append(" if(rowObject.ListingCurrencyId == '') { ");
        if (hasFormCEditPermission)
        {
            sb.Append("return '<a style=cursor:hand target=_parent href=" + actionEdit +
                      "/'+parameters+'><img title=Edit alt=Edit style=border-style:none src=" +
                      uri.Content("~/Content/Images/edit.png") +
                      " /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:deleteRecord(" + deleteMethod +
                      ");><img title=Delete style=border-style:none src=" + uri.Content("~/Content/Images/delete.png") +
                      " /></a>';");

        }
        else
        {
            sb.Append("return '';");
        }

       sb.Append(" } else { ");
        if (hasFormCEditPermission)
        {
            sb.Append("return '<a style=cursor:hand target=_parent href=" + actionEdit +
                      "/'+parameters+'><img title=Edit alt=Edit style=border-style:none src=" +
                      uri.Content("~/Content/Images/edit.png") +
                      " /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:deleteRecord(" +
                      deleteMethodWithListingCurrency + ");><img title=Delete style=border-style:none src=" +
                      uri.Content("~/Content/Images/delete.png") + " /></a>';");
        }
        else
        {
            sb.Append("return '';");
        }

       sb.Append(" } } ");
      // Invoice Status 3, 4, 5, 6 - Ready for Billing, Claimed, Processing complete, Presented.
      // If form C status is Submitted or Claimed, show only View icon
       sb.AppendFormat("if(rowObject.InvoiceStatusId=={0}||rowObject.InvoiceStatusId=={1}){{", (int)InvoiceStatusType.ReadyForBilling, (int)InvoiceStatusType.Claimed);
        if (hasFormCViewPermission)
        {
            sb.Append("return '<a style=cursor:hand target=_parent href=" + actionView +
                      "/'+parameters+'><img title=View alt=View style=border-style:none src=" +
                      uri.Content("~/Content/Images/view.png") +
                " /></a>';}");
        }
        else
        {
            sb.Append("return '';}");
        }
      // If invoice status is Presented or Processing complete, then show View and zip icons.
        sb.AppendFormat("else if(rowObject.InvoiceStatusId=={0}||rowObject.InvoiceStatusId=={1}){{", (int)InvoiceStatusType.Presented, (int)InvoiceStatusType.ProcessingComplete);
        sb.Append(GetFormCViewZipScript(uri, actionView, downloadZipMethodParams, hasFormCViewPermission, hasFormCDownloadPermission));
      sb.Append("}");

      //If SUbmission Method is IS-XML or IS-IDEC
      //*********************************************************************************************
      sb.AppendFormat("if(rowObject.SubmissionMethodId=={0} || rowObject.SubmissionMethodId=={1}){{",
        (int)SubmissionMethod.IsXml, (int)SubmissionMethod.IsIdec);

      sb.AppendFormat("if(((rowObject.InvoiceStatusId=={0}) || (rowObject.InvoiceStatusId=={1})) && (rowObject.FileStatusId=={2}) ){{",
          (int)InvoiceStatusType.ErrorCorrectable, (int)InvoiceStatusType.ErrorNonCorrectable, (int)FileStatusType.ValidationCompleted);

      // script for delete....
      sb.Append("if(rowObject.ListingCurrencyId == ''){");
        sb.Append(GetFormCViewDeleteScript(uri, actionView, deleteMethod, hasFormCViewPermission, hasFormCEditPermission));
      sb.Append("}else{");
        sb.Append(GetFormCViewDeleteScript(uri, actionView, deleteMethodWithListingCurrency, hasFormCViewPermission, hasFormCEditPermission));
        
      sb.Append("}}else{");
        sb.Append(GetFormCViewScript(uri, actionView, hasFormCViewPermission));
      sb.Append("}");
      //SCP#68397 - Deleted invoice (XB-T01) pushed to ICH 
      //If SUbmission Method is IS-Web
      //*********************************************************************************************
      sb.AppendFormat("}} else if(rowObject.SubmissionMethodId=={0}){{", (int)SubmissionMethod.IsWeb);

      sb.AppendFormat("if((rowObject.InvoiceStatusId=={0}) || (rowObject.InvoiceStatusId=={1}) || (rowObject.InvoiceStatusId=={2}) || (rowObject.InvoiceStatusId=={3}) ){{",
          (int)InvoiceStatusType.Open, (int)InvoiceStatusType.ReadyForSubmission, (int)InvoiceStatusType.ValidationError, (int)InvoiceStatusType.ErrorNonCorrectable);
      // script for delete....
      sb.Append("if(rowObject.ListingCurrencyId == ''){");
      //SCP239795: Issue with Form C Edit.
        sb.Append(GetFormCEditDeleteScript(uri, actionEdit, deleteMethod, hasFormCEditPermission));
      sb.Append("}else{");
        sb.Append(GetFormCEditDeleteScript(uri, actionEdit, deleteMethodWithListingCurrency, hasFormCEditPermission));
     
      sb.Append("}}else{");
        sb.Append(GetFormCViewScript(uri, actionView, hasFormCViewPermission));
      sb.Append("}");
      //*********************************************************************************************

      // For other invoice status
      // Listing currency is null..
      sb.Append("if(rowObject.ListingCurrencyId == ''){");
        //Create function for generate script for edit and validate with permission.
        sb.Append(GetEditValidateScript(uri, actionEdit, validateMethod, hasFormCEditPermission, hasFormCValidatePermission));
      sb.Append("}else{");
        sb.Append(GetEditValidateScript(uri, actionEdit, validateMethodWithListingCurrency, hasFormCEditPermission, hasFormCValidatePermission));
      sb.Append("}}} </script>");

      return MvcHtmlString.Create(sb.ToString());
    }

    /// <summary>
    /// Generates action buttons for correspondence search results in Billing History.
    /// </summary>
    public static MvcHtmlString GenerateMiscCorrespondenceBillingHistoryGridScript(UrlHelper uri,
                                                                               string jqGridId,
                                                                               string billingMemoMethod,
                                                                               string correspondenceMethod,
                                                                               string auditTrailMethod,
                                                                               int memberId)
    {
      string jqGridEditMethodName = string.Format("{0}_GenerateBillingHistoryActions(cellValue, options, rowObject)", jqGridId);

      // Indices: 9 - DisplayCorrespondenceStatus, 12- AuthorityToBill, 1- BillingMemberId, 4 - TransactionNumber, 14 - CorrInitiatingMember, 15 - Correspondence Id.
      var sb = new StringBuilder();
      //invoiceId, correspondenceStatusId, authorityToBill, correspondenceDate, correspondenceRefNumber
      sb.Append("<script type='text/javascript'>");
      sb.Append("function " + jqGridEditMethodName + " { ");
      const string billingMemoParams = "\"'+rowObject.InvoiceId +'\",\"'+rowObject.CorrespondenceStatusId+'\",\"'+rowObject.AuthorityToBill+'\",\"'+rowObject.TransactionDate+'\",\"'+rowObject.TransactionNumber.toString().replace(/^\\s+/, \"\")+'\"";

      sb.Append("/*}else */if(rowObject.DisplayCorrespondenceStatus != \"Closed\" && (rowObject.AuthorityToBill == \"True\" || rowObject.DisplayCorrespondenceStatus == \"Expired\")&&rowObject.CorrInitiatingMember ==" + memberId + "){");
      sb.AppendFormat("return '<a style=cursor:hand target=_parent href=javascript:IsTransactionOutsideTimeLimit({0});><img title=\"Correspondence Invoice\"  alt=Correspondence Invoice style=border-style:none src=" + uri.Content("~/Content/Images/lineitemdetails.png") +
                " /></a>  <a style=cursor:hand target=_parent href=" + auditTrailMethod + "/'+rowObject.InvoiceId+'><img title=\"Show Audit Trail\"  alt=View Audit Trail style=border-style:none src=" +
                uri.Content("~/Content/Images/show_audit_trail.png") + " /></a>  <a style=cursor:hand target=_parent href=" + correspondenceMethod +
                "/'+rowObject.InvoiceId+'/'+rowObject.CorrespondenceId+'><img title=\"Show/ Create Correspondence\"  alt=Correspondence style=border-style:none src=" + uri.Content("~/Content/Images/show_correspondence.png") +
                " /></a>';", billingMemoParams);
      sb.Append("}else{");
      sb.Append("return '<a style=cursor:hand target=_parent href=" + auditTrailMethod + "/'+rowObject.InvoiceId+'><img title=\"Show Audit Trail\"  alt=View Audit Trail style=border-style:none src=" +
                uri.Content("~/Content/Images/show_audit_trail.png") + " /></a>  <a style=cursor:hand target=_parent href=" + correspondenceMethod +
                "/'+rowObject.InvoiceId+'/'+rowObject.CorrespondenceId+'><img title=\"Show/ Create Correspondence\"  alt=Correspondence style=border-style:none src=" + uri.Content("~/Content/Images/show_correspondence.png") +
                " /></a>';");
      sb.Append("}} </script>");

      return MvcHtmlString.Create(sb.ToString());
    }

    /// <summary>
    /// Generates action buttons for correspondence search results in Billing History.
    /// </summary>
    public static MvcHtmlString GenerateCorrespondenceBillingHistoryGridScript(UrlHelper uri,
                                                                                 string jqGridId,
                                                                                 string billingMemoMethod,
                                                                                 string correspondenceMethod,
                                                                                 string auditTrailMethod,
                                                                                 int memberId)
    {
        string jqGridEditMethodName = string.Format("{0}_GenerateBillingHistoryActions(cellValue, options, rowObject)", jqGridId);

        // Indices: 8 - DisplayCorrespondenceStatus, 11- AuthorityToBill, 1- BillingMemberId, 4 - TransactionNumber, 13 - CorrInitiatingMember, 14 - Correspondence Id.
        var sb = new StringBuilder();
        //invoiceId, correspondenceStatusId, authorityToBill, correspondenceDate, correspondenceRefNumber
        sb.Append("<script type='text/javascript'>");
        sb.Append("function " + jqGridEditMethodName + " { ");
        //sb.Append("var billingMemoParams = rowObject[0]+','+rowObject[15]+','+ rowObject[11] +','+rowObject[3]+','+rowObject[4];"); //invoice id, transaction number
        // TFSBug#9707
        const string billingMemoParams = "\"'+rowObject.InvoiceId+'\",\"'+rowObject.CorrespondenceStatusId+'\",\"'+rowObject.AuthorityToBill+'\",\"'+rowObject.TransactionDate+'\",\"'+rowObject.TransactionNumber.toString().replace(/^\\s+/, \"\")+'\"";

        /*
         * CMP288 Changes to show "Create correspondence button" on Expired status
         * sb.Append("if(rowObject[8] == \"Expired\" && rowObject[13] ==" + memberId + "){");
         //sb.Append("return '<a style=cursor:hand target=_parent href=" + billingMemoMethod +
         //          "/'+billingMemoParams+'><img title=\"Correspondence Invoice\"  alt=Correspondence Invoice style=border-style:none src=" + uri.Content("~/Content/Images/lineitemdetails.png") +
         //          " /></a>  <a style=cursor:hand target=_parent href=" + auditTrailMethod + "/'+rowObject[0]+'><img title=\"Show Audit Trail\"  alt=View Audit Trail style=border-style:none src=" +
         //          uri.Content("~/Content/Images/show_audit_trail.png") + " /></a>  <a style=cursor:hand href = " + showDetails +
         //          "/'+rowObject[0]+'/'+rowObject[14]+'><img title=\"Show Details\"  alt=Show Details style=border-style:none src=" + uri.Content("~/Content/Images/view.png") + " /></a>';");

         sb.AppendFormat("return '<a style=cursor:hand target=_parent href=javascript:IsTransactionOutsideTimeLimit({0});><img title=\"Correspondence Invoice\"  alt=Correspondence Invoice style=border-style:none src=" + uri.Content("~/Content/Images/lineitemdetails.png") +
                   " /></a>  <a style=cursor:hand target=_parent href=" + auditTrailMethod + "/'+rowObject[0]+'><img title=\"Show Audit Trail\"  alt=View Audit Trail style=border-style:none src=" +
                   uri.Content("~/Content/Images/show_audit_trail.png") + " /></a>';", billingMemoParams);*/


        sb.Append("if(rowObject.DisplayCorrespondenceStatus != \"Closed\" && (rowObject.AuthorityToBill == \"True\" || rowObject.DisplayCorrespondenceStatus == \"Expired\")&&rowObject.CorrInitiatingMember ==" + memberId + "){");
        sb.AppendFormat("return '<a style=cursor:hand target=_parent href=javascript:IsTransactionOutsideTimeLimit({0});><img title=\"Correspondence Invoice\"  alt=Correspondence Invoice style=border-style:none src=" + uri.Content("~/Content/Images/lineitemdetails.png") +
                  " /></a>  <a style=cursor:hand target=_parent href=" + auditTrailMethod + "/'+rowObject.InvoiceId+'><img title=\"Show Audit Trail\"  alt=View Audit Trail style=border-style:none src=" +
                  uri.Content("~/Content/Images/show_audit_trail.png") + " /></a>  <a style=cursor:hand target=_parent href=" + correspondenceMethod +
                "/'+rowObject.InvoiceId+'/'+rowObject.CorrespondenceId+'><img title=\"Show/ Create Correspondence\"  alt=Correspondence style=border-style:none src=" + uri.Content("~/Content/Images/show_correspondence.png") +
                  " /></a>';", billingMemoParams);
        sb.Append("}else{");
        sb.Append("return '<a style=cursor:hand target=_parent href=" + auditTrailMethod + "/'+rowObject.InvoiceId+'><img title=\"Show Audit Trail\"  alt=View Audit Trail style=border-style:none src=" +
                  uri.Content("~/Content/Images/show_audit_trail.png") + " /></a>  <a style=cursor:hand target=_parent href=" + correspondenceMethod +
                "/'+rowObject.InvoiceId+'/'+rowObject.CorrespondenceId+'><img title=\"Show/ Create Correspondence\"  alt=Correspondence style=border-style:none src=" + uri.Content("~/Content/Images/show_correspondence.png") +
                  " /></a>';");
        sb.Append("}} </script>");

        return MvcHtmlString.Create(sb.ToString());
    }


    /// <summary>
    /// Generates action buttons for invoice search results in Misc Billing History.
    /// </summary>
    public static MvcHtmlString GenerateMiscInvoiceBillingHistoryGridScript(UrlHelper uri, string jqGridId, string correspondenceMethod, string auditTrailMethod, string showInvoiceDetailsMethod, string showCreditNoteDetailsMethod)
    {
        string jqGridEditMethodName = string.Format("{0}_GenerateBillingHistoryActions(cellValue, options, rowObject)", jqGridId);
        var invoiceIdParams = "\"'+rowObject.InvoiceId+'\"";
        // Indices: 9 - DisplayCorrespondenceStatus, 12- AuthorityToBill, 1- BillingMemberId, 2- ClearingHouse, 8-RejectionStage, 17-InvoiceTypeId
        var sb = new StringBuilder();
        
        Model.MiscUatp.BillingHistory.InvoiceSearchCriteria searchCriteria = null;
        if (SessionUtil.MiscInvoiceSearchCriteria != null && SessionUtil.BillingCategoryId != null)
        {
            searchCriteria = new JavaScriptSerializer().Deserialize(SessionUtil.MiscInvoiceSearchCriteria, typeof(Model.MiscUatp.BillingHistory.InvoiceSearchCriteria)) as Model.MiscUatp.BillingHistory.InvoiceSearchCriteria;

            if (searchCriteria != null)
            {
                if (searchCriteria.BillingTypeId == 1)
                {  
                        showInvoiceDetailsMethod = uri.Action("ShowDetails", "MiscPayInvoice", new { area = "Misc" });
                        showCreditNoteDetailsMethod = uri.Action("ShowDetails", "MiscPayCreditNote", new { area = "Misc" });
                }
            }
        }
        // End

        sb.Append("<script type='text/javascript'>");
        sb.Append("function " + jqGridEditMethodName + " {");

        sb.Append(" if($('#BillingTypeId').val() == 1 &&(((rowObject.ClearingHouse == 'I' || rowObject.ClearingHouse == '') && (rowObject.RejectionStage == 1||rowObject.RejectionStage == 2)) || (rowObject.ClearingHouse == 'A' && rowObject.RejectionStage  == 2))){");
        sb.AppendFormat("if(rowObject.InvoiceTypeId== {0}){{", (int)InvoiceType.CreditNote);
        sb.AppendFormat("return '<a style=cursor:hand target=_parent href=javascript:IsCorrespondenceOutsideTimeLimit({0});><img title=\"Show/ Create Correspondence\"  alt=Correspondence style=border-style:none src=" +
                  uri.Content("~/Content/Images/show_correspondence.png") + " /></a>  <a style=cursor:hand target=_parent href=" + auditTrailMethod +
                  "/'+rowObject.InvoiceId+'><img title=\"Show Audit Trail\"  alt=View Audit Trail style=border-style:none src=" + uri.Content("~/Content/Images/show_audit_trail.png") +
                  " /></a>  <a style=cursor:hand href = " + showCreditNoteDetailsMethod + "/' +rowObject.InvoiceId+'><img title=\"Show Details\"  alt=Show Details style=border-style:none src=" +
                  uri.Content("~/Content/Images/show_details.png") + " /></a>';", invoiceIdParams);

        sb.Append("}");
        sb.AppendFormat("return '<a style=cursor:hand target=_parent href=javascript:IsCorrespondenceOutsideTimeLimit({0});><img title=\"Show/ Create Correspondence\"  alt=Correspondence style=border-style:none src=" +
                  uri.Content("~/Content/Images/show_correspondence.png") + " /></a>  <a style=cursor:hand target=_parent href=" + auditTrailMethod +
                  "/'+rowObject.InvoiceId+'><img title=\"Show Audit Trail\"  alt=View Audit Trail style=border-style:none src=" + uri.Content("~/Content/Images/show_audit_trail.png") +
                  " /></a>  <a style=cursor:hand href = " + showInvoiceDetailsMethod + "/' +rowObject.InvoiceId+'><img title=\"Show Details\"  alt=Show Details style=border-style:none src=" +
                  uri.Content("~/Content/Images/show_details.png") + " /></a>';", invoiceIdParams);

        sb.Append("}else{");
        sb.AppendFormat("if(rowObject.InvoiceTypeId== {0}){{", (int)InvoiceType.CreditNote);
        sb.Append("return '<a style=cursor:hand target=_parent href=" + auditTrailMethod + "/'+rowObject.InvoiceId+'><img title=\"Show Audit Trail\"  alt=View Audit Trail style=border-style:none src=" +
                  uri.Content("~/Content/Images/show_audit_trail.png") + " /></a>  <a style=cursor:hand href = " + showCreditNoteDetailsMethod +
                  "/' +rowObject.InvoiceId+'><img title=\"Show Details\"  alt=Show Details style=border-style:none src=" + uri.Content("~/Content/Images/show_details.png") + " /></a>';");
        sb.Append("}");
        sb.Append("return '<a style=cursor:hand target=_parent href=" + auditTrailMethod + "/'+rowObject.InvoiceId+'><img title=\"Show Audit Trail\"  alt=View Audit Trail style=border-style:none src=" +
                  uri.Content("~/Content/Images/show_audit_trail.png") + " /></a>  <a style=cursor:hand href = " + showInvoiceDetailsMethod +
                  "/' +rowObject.InvoiceId+'><img title=\"Show Details\"  alt=Show Details style=border-style:none src=" + uri.Content("~/Content/Images/show_details.png") + " /></a>';");
        sb.Append("}");
        sb.Append("}</script>");

        return MvcHtmlString.Create(sb.ToString());
    }



    /// <summary>
    /// Generates action buttons for invoice search results in Billing History.
    /// </summary>
    public static MvcHtmlString GenerateInvoiceBillingHistoryGridScript(UrlHelper uri, string jqGridId, string correspondenceMethod, string auditTrailMethod, string showInvoiceDetailsMethod, string showCreditNoteDetailsMethod)
    {
      string jqGridEditMethodName = string.Format("{0}_GenerateBillingHistoryActions(cellValue, options, rowObject)", jqGridId);
      var invoiceIdParams = "\"'+rowObject.InvoiceId+'\"";
      // Indices: 9 - DisplayCorrespondenceStatus, 11- AuthorityToBill, 1- BillingMemberId, 2- ClearingHouse, 7-RejectionStage, 16-InvoiceTypeId
      var sb = new StringBuilder();

      // Start
      // Purpose: To separate Payables & Receivables
      // Author: Sachin Pharande
      // Date: 22-08-2012
      Model.MiscUatp.BillingHistory.InvoiceSearchCriteria searchCriteria = null;
      if (SessionUtil.MiscInvoiceSearchCriteria != null && SessionUtil.BillingCategoryId != null)
      {
        searchCriteria = new JavaScriptSerializer().Deserialize(SessionUtil.MiscInvoiceSearchCriteria, typeof(Model.MiscUatp.BillingHistory.InvoiceSearchCriteria)) as Model.MiscUatp.BillingHistory.InvoiceSearchCriteria;

        if (searchCriteria != null)
        {
          if (searchCriteria.BillingTypeId == 1)
          {
            if (SessionUtil.BillingCategoryId == 3)
            {
              showInvoiceDetailsMethod = uri.Action("ShowDetails", "MiscPayInvoice", new { area = "Misc" });
              showCreditNoteDetailsMethod = uri.Action("ShowDetails", "MiscPayCreditNote", new { area = "Misc" });
            }
            else if (SessionUtil.BillingCategoryId == 4)
            {
              showInvoiceDetailsMethod = uri.Action("ShowDetails", "UatpPayInvoice", new { area = "Uatp" });
              showCreditNoteDetailsMethod = uri.Action("ShowDetails", "UatpPayCreditNote", new { area = "Uatp" });
            }
          }
        }
      }
      // End
      // TFSBug#9707
      /*
       * rowObject[0] is InvoiceId
       * rowObject[2] is ClearingHouse
       * rowObject[8] is RejectionStage
       * rowObject[17] is InvoiceTypeId
       */

      sb.Append("<script type='text/javascript'>");
      sb.Append("function " + jqGridEditMethodName + " {");

      sb.Append(" if($('#BillingTypeId').val() == 1 &&(((rowObject.ClearingHouse == 'I' || rowObject.ClearingHouse == '') && (rowObject.RejectionStage == 1||rowObject.RejectionStage == 2)) || (rowObject.ClearingHouse == 'A' && rowObject.RejectionStage  == 2))){");
      sb.AppendFormat("if(rowObject.InvoiceTypeId== {0}){{", (int)InvoiceType.CreditNote);
      sb.AppendFormat("return '<a style=cursor:hand target=_parent href=javascript:IsCorrespondenceOutsideTimeLimit({0});><img title=\"Show/ Create Correspondence\"  alt=Correspondence style=border-style:none src=" +
                uri.Content("~/Content/Images/show_correspondence.png") + " /></a>  <a style=cursor:hand target=_parent href=" + auditTrailMethod +
                "/'+rowObject.InvoiceId+'><img title=\"Show Audit Trail\"  alt=View Audit Trail style=border-style:none src=" + uri.Content("~/Content/Images/show_audit_trail.png") +
                " /></a>  <a style=cursor:hand href = " + showCreditNoteDetailsMethod + "/' +rowObject.InvoiceId+'><img title=\"Show Details\"  alt=Show Details style=border-style:none src=" +
                uri.Content("~/Content/Images/show_details.png") + " /></a>';", invoiceIdParams);

      sb.Append("}");
      sb.AppendFormat("return '<a style=cursor:hand target=_parent href=javascript:IsCorrespondenceOutsideTimeLimit({0});><img title=\"Show/ Create Correspondence\"  alt=Correspondence style=border-style:none src=" +
                uri.Content("~/Content/Images/show_correspondence.png") + " /></a>  <a style=cursor:hand target=_parent href=" + auditTrailMethod +
                "/'+rowObject.InvoiceId+'><img title=\"Show Audit Trail\"  alt=View Audit Trail style=border-style:none src=" + uri.Content("~/Content/Images/show_audit_trail.png") +
                " /></a>  <a style=cursor:hand href = " + showInvoiceDetailsMethod + "/' +rowObject.InvoiceId+'><img title=\"Show Details\"  alt=Show Details style=border-style:none src=" +
                uri.Content("~/Content/Images/show_details.png") + " /></a>';", invoiceIdParams);

      sb.Append("}else{");
      sb.AppendFormat("if(rowObject.InvoiceTypeId== {0}){{", (int)InvoiceType.CreditNote);
      sb.Append("return '<a style=cursor:hand target=_parent href=" + auditTrailMethod + "/'+rowObject.InvoiceId+'><img title=\"Show Audit Trail\"  alt=View Audit Trail style=border-style:none src=" +
                uri.Content("~/Content/Images/show_audit_trail.png") + " /></a>  <a style=cursor:hand href = " + showCreditNoteDetailsMethod +
                "/' +rowObject.InvoiceId+'><img title=\"Show Details\"  alt=Show Details style=border-style:none src=" + uri.Content("~/Content/Images/show_details.png") + " /></a>';");
      sb.Append("}");
      sb.Append("return '<a style=cursor:hand target=_parent href=" + auditTrailMethod + "/'+rowObject.InvoiceId+'><img title=\"Show Audit Trail\"  alt=View Audit Trail style=border-style:none src=" +
                uri.Content("~/Content/Images/show_audit_trail.png") + " /></a>  <a style=cursor:hand href = " + showInvoiceDetailsMethod +
                "/' +rowObject.InvoiceId+'><img title=\"Show Details\"  alt=Show Details style=border-style:none src=" + uri.Content("~/Content/Images/show_details.png") + " /></a>';");
      sb.Append("}");
      sb.Append("}</script>");

      return MvcHtmlString.Create(sb.ToString());
    }

    public static MvcHtmlString GeneratePaxCorrespondenceBillingHistoryGridScript(UrlHelper url, string bhSearchResultsGrid, string action, string billingHistoryUrl)
    {
      string jqGridEditMethodName = string.Format("{0}_GenerateBillingHistoryActions(cellValue, options, rowObject)", bhSearchResultsGrid);

      // Indices: 9 - DisplayCorrespondenceStatus, 11- AuthorityToBill, 1- BillingMemberId, 2- ClearingHouse, 8-RejectionStage
      var sb = new StringBuilder();

      var showCorrespondence = url.Action("Correspondence", "Correspondence", new { area = "Pax" });
      // var billingHistoryUrl = url.Action("PaxBillingHistoryAuditTrail", "BillingHistory", new { area = "Pax" });
      //var billingHistoryUrl = url.RouteUrl("PaxBillingHistoryAuditTrail", new { controller = "BillingHistory", action = "PaxBillingHistoryAuditTrail", area = "Pax" });
      billingHistoryUrl = billingHistoryUrl.Replace("/0/1", "");

      //CMP612: Changes to PAX CGO Correspondence Audit Trail Download.
      //Add one more icon for show rejection.
      sb.Append("<script type='text/javascript'>");
      sb.Append("function " + jqGridEditMethodName + " {");
      sb.Append(" return '<a style=cursor:hand href = " + showCorrespondence + "/' +rowObject.TransactionId+'><img title=\"Show/ Create Correspondence\" alt=Show Details style=border-style:none src=" +
                url.Content("~/Content/Images/show_correspondence.png") + " /></a>&nbsp;<a style=cursor:hand href = " + billingHistoryUrl + "/' +rowObject.TransactionId+'/CO><img title=\"Show Audit Trail\" alt=Show Details style=border-style:none src=" +
                url.Content("~/Content/Images/show_audit_trail.png") + " /></a>&nbsp;<img style=cursor:pointer title=\"Show Rejections\" alt=\"Show Details\" onclick=ShowLinkedCorrRejectionMemo(\"'+rowObject.TransactionId+'\") style=border-style:none src=" + url.Content("~/Content/Images/show_rejections.png") + "  />';");
      sb.Append("}</script>");

      return MvcHtmlString.Create(sb.ToString());
    }

    public static MvcHtmlString GenerateCargoCorrespondenceBillingHistoryGridScript(UrlHelper url, string bhSearchResultsGrid, string action, string billingHistoryUrl)
    {
      string jqGridEditMethodName = string.Format("{0}_GenerateBillingHistoryActions(cellValue, options, rowObject)", bhSearchResultsGrid);

      // Indices: 9 - DisplayCorrespondenceStatus, 11- AuthorityToBill, 1- BillingMemberId, 2- ClearingHouse, 8-RejectionStage
      var sb = new StringBuilder();

      var showCorrespondence = url.Action("Correspondence", "Correspondence", new { area = "Cargo" });
      billingHistoryUrl = billingHistoryUrl.Replace("/0/1", "");

      sb.Append("<script type='text/javascript'>");
      sb.Append("function " + jqGridEditMethodName + " {");
      sb.Append(" return '<a style=cursor:hand href = " + showCorrespondence + "/' +rowObject.TransactionId+'><img title=\"Show/ Create Correspondence\" alt=Show Details style=border-style:none src=" +
                url.Content("~/Content/Images/show_correspondence.png") + " /></a>&nbsp;<a style=cursor:hand href = " + billingHistoryUrl + "/' +rowObject.TransactionId+'/CO><img title=\"Show Audit Trail\" alt=Show Details style=border-style:none src=" +
                url.Content("~/Content/Images/show_audit_trail.png") + " /></a>&nbsp;<img style=cursor:pointer title=\"Show Rejections\" alt=\"Show Details\" onclick=ShowLinkedCorrRejectionMemo(\"'+rowObject.TransactionId+'\") style=border-style:none src=" + url.Content("~/Content/Images/show_rejections.png") + "  />';");
      sb.Append("}</script>");

      return MvcHtmlString.Create(sb.ToString());
    }

    public static MvcHtmlString GeneratePaxInvoiceBillingHistoryGridScript(UrlHelper uri, string jqGridId, string showDetailsMethod, string billingHistoryUrl, string showFormC)
    {
      string jqGridEditMethodName = string.Format("{0}_GenerateBillingHistoryActions(cellValue, options, rowObject)", jqGridId);

      // Indices: 9 - DisplayCorrespondenceStatus, 11- AuthorityToBill, 1- BillingMemberId, 2- ClearingHouse, 8-RejectionStage
      var sb = new StringBuilder();

      var showCreditNote = uri.Action("View", "CreditNote", new { area = "Pax" });

      var showInvoice = uri.Action("View", "Invoice", new { area = "Pax" });

      var showFormF = uri.Action("View", "FormF", new { area = "Pax" });

      var showFormDE = uri.Action("View", "FormDE", new { area = "Pax" });

      var showFormXF = uri.Action("View", "FormXF", new { area = "Pax" });

      InvoiceSearchCriteria searchCriteria = null;
      if (SessionUtil.PaxInvoiceSearchCriteria != null)
      {
        searchCriteria = new JavaScriptSerializer().Deserialize(SessionUtil.PaxInvoiceSearchCriteria, typeof(InvoiceSearchCriteria)) as InvoiceSearchCriteria;

        if (searchCriteria != null)
          if (searchCriteria.BillingTypeId == 1)
          {
            showCreditNote = uri.Action("View", "CreditNotePayables", new { area = "Pax" });

            showInvoice = uri.Action("View", "InvoicePayables", new { area = "Pax" });

            showFormF = uri.Action("View", "FormFPayables", new { area = "Pax" });

            showFormDE = uri.Action("View", "FormDEPayables", new { area = "Pax" });

            showFormXF = uri.Action("View", "FormXFPayables", new { area = "Pax" });
          }
      }

      billingHistoryUrl = billingHistoryUrl.Replace("/0/1", "");

      sb.Append("<script type='text/javascript'>");
      sb.Append("function " + jqGridEditMethodName + " {");

      sb.Append("if(rowObject.BillingCodeId == '" + (int)BillingCode.SamplingFormC + "'){ return '<a style=cursor:hand href = " + showFormC +
          "/' + rowObject.TransactionId + '><img title=\"Show Details\" alt=Show Details style=border-style:none src=" + uri.Content("~/Content/Images/show_details.png") +
          " /></a>&nbsp;<a style=cursor:hand href = " + billingHistoryUrl + "/' +rowObject.TransactionId+'/FC><img title=\"Show Audit Trail\" alt=Show Details style=border-style:none src=" +
          uri.Content("~/Content/Images/show_audit_trail.png") + " /></a>';} ");


      sb.Append("if(rowObject.BillingCodeId == '" + (int)BillingCode.SamplingFormAB + "'){ return '<a style=cursor:hand href = " + showInvoice.Replace("Invoice", "FormAB").Replace("/View", "") +
                "/' +rowObject.InvoiceId+'/PrimeBillingView/' + rowObject.TransactionId + '><img title=\"Show Details\" alt=Show Details style=border-style:none src=" + uri.Content("~/Content/Images/show_details.png") +
                " /></a>&nbsp;<a style=cursor:hand href = " + billingHistoryUrl + "/' +rowObject.TransactionId+'/PM><img title=\"Show Audit Trail\" alt=Show Details style=border-style:none src=" +
                uri.Content("~/Content/Images/show_audit_trail.png") + " /></a>';");
      sb.Append("} else { if(rowObject.TransactionType == 'CM'){");
      sb.Append(" return '<a style=cursor:hand href = " + showCreditNote.Replace("/View", "") +
                "/' +rowObject.InvoiceId+'/CreditMemoView/' + rowObject.TransactionId + '><img title=\"Show Details\" alt=\"Show Details\" style=border-style:none src=" + uri.Content("~/Content/Images/show_details.png") +
                " /></a>&nbsp;<a style=cursor:hand href = " + billingHistoryUrl + "/' +rowObject.TransactionId+'/CM><img title=\"Show Audit Trail\" alt=Show Details style=border-style:none src=" +
                uri.Content("~/Content/Images/show_audit_trail.png") + " /></a>';}");
      sb.Append("else if(rowObject.TransactionType == 'RM'){");
      sb.Append("if(rowObject.BillingCodeId == '6'){");
      sb.Append(" return '<a style=cursor:hand href = " + showFormF.Replace("/View", "") +
                "/' +rowObject.InvoiceId+'/RMView/' + rowObject.TransactionId + '><img title=\"Show Details\" alt=\"Show Details\" style=border-style:none src=" + uri.Content("~/Content/Images/show_details.png") +
                " /></a>&nbsp;<a style=cursor:hand href = " + billingHistoryUrl + "/' +rowObject.TransactionId+'/RM><img title=\"Show Audit Trail\" alt=Show Details style=border-style:none src=" +
                uri.Content("~/Content/Images/show_audit_trail.png") + " /></a>';}");
      sb.Append(" else if(rowObject.BillingCodeId == '7'){");
      sb.Append(" return '<a style=cursor:hand href = " + showFormXF.Replace("/View", "") +
                "/' +rowObject.InvoiceId+'/RMView/' + rowObject.TransactionId + '><img title=\"Show Details\" alt=\"Show Details\" style=border-style:none src=" + uri.Content("~/Content/Images/show_details.png") +
                " /></a>&nbsp;<a style=cursor:hand href = " + billingHistoryUrl + "/' +rowObject.TransactionId+'/RM><img title=\"Show Audit Trail\" alt=Show Details style=border-style:none src=" +
                uri.Content("~/Content/Images/show_audit_trail.png") + " /></a>';}");
      sb.Append(" else {");
      sb.Append(" return '<a style=cursor:hand href = " + showInvoice.Replace("/View", "") +
                "/' +rowObject.InvoiceId+'/RMView/' + rowObject.TransactionId + '><img title=\"Show Details\" alt=\"Show Details\" style=border-style:none src=" + uri.Content("~/Content/Images/show_details.png") +
                " /></a>&nbsp;<a style=cursor:hand href =  " + billingHistoryUrl + "/' +rowObject.TransactionId+'/RM><img title=\"Show Audit Trail\" alt=Show Details style=border-style:none src=" +
                uri.Content("~/Content/Images/show_audit_trail.png") + " /></a>';}}");
      sb.Append("else if(rowObject.TransactionType == 'BM'){");
      sb.Append(" return '<a style=cursor:hand href = " + showInvoice.Replace("/View", "") +
                "/' +rowObject.InvoiceId+'/BMView/' + rowObject.TransactionId + '><img title=\"Show Details\" alt=\"Show Details\" style=border-style:none src=" + uri.Content("~/Content/Images/show_details.png") +
                " /></a>&nbsp;<a style=cursor:hand href = " + billingHistoryUrl + "/' +rowObject.TransactionId+'/BM><img title=\"Show Audit Trail\" alt=Show Details style=border-style:none src=" +
                uri.Content("~/Content/Images/show_audit_trail.png") + " /></a>';}");
      sb.Append("else if(rowObject.TransactionType == 'PC'){");
      sb.Append(" return '<a style=cursor:hand href = " + showInvoice.Replace("/View", "") +
                "/' +rowObject.InvoiceId+'/PrimeBillingView/' + rowObject.TransactionId + '><img title=\"Show Details\" alt=Show Details style=border-style:none src=" + uri.Content("~/Content/Images/show_details.png") +
                " /></a>&nbsp;<a style=cursor:hand href = " + billingHistoryUrl + "/' +rowObject.TransactionId+'/PM><img title=\"Show Audit Trail\" alt=Show Details style=border-style:none src=" +
                uri.Content("~/Content/Images/show_audit_trail.png") + " /></a>';}");
      sb.Append("else if(rowObject.TransactionType == 'FD'){");
      sb.Append(" return '<a style=cursor:hand href = " + showFormDE.Replace("/View", "") +
                "/' +rowObject.InvoiceId+'/FormDView/' + rowObject.TransactionId + '><img title=\"Show Details\" alt=Show Details style=border-style:none src=" + uri.Content("~/Content/Images/show_details.png") +
                " /></a>&nbsp;<a style=cursor:hand href =  " + billingHistoryUrl + "/' +rowObject.TransactionId+'/FD><img title=\"Show Audit Trail\" alt=Show Details style=border-style:none src=" +
                uri.Content("~/Content/Images/show_audit_trail.png") + " /></a>';}");
      sb.Append("}}</script>");

      return MvcHtmlString.Create(sb.ToString());
    }

    public static MvcHtmlString GenerateCargoInvoiceBillingHistoryGridScript(UrlHelper uri, string jqGridId, string showDetailsMethod, string billingHistoryUrl)
    {

      string jqGridEditMethodName = string.Format("{0}_GenerateBillingHistoryActions(cellValue, options, rowObject)", jqGridId);

      var sb = new StringBuilder();

      var showCreditNote = uri.Action("View", "CreditNote", new { area = "Cargo" });
      var showInvoice = uri.Action("View", "Invoice", new { area = "Cargo" });

      Model.Cargo.BillingHistory.InvoiceSearchCriteria searchCriteria = null;

      if (SessionUtil.CGOInvoiceSearchCriteria != null)
      {
        searchCriteria = new JavaScriptSerializer().Deserialize(SessionUtil.CGOInvoiceSearchCriteria, typeof(Model.Cargo.BillingHistory.InvoiceSearchCriteria)) as Model.Cargo.BillingHistory.InvoiceSearchCriteria;

        if (searchCriteria != null)
          if (searchCriteria.BillingTypeId == 1)
          {
            showCreditNote = uri.Action("View", "CreditNotePayables", new { area = "Cargo" });
            showInvoice = uri.Action("View", "InvoicePayables", new { area = "Cargo" });
          }
      }

      billingHistoryUrl = billingHistoryUrl.Replace("/0/1", "");

      sb.Append("<script type='text/javascript'>");
      sb.Append("function " + jqGridEditMethodName + " {");

      sb.Append("if(rowObject.TransactionType == 'AWB Prepaid'){ return '<a style=cursor:hand href = " + showInvoice.Replace("/View", "") +
                "/' +rowObject.InvoiceId+'/AwbPrepaidBillingView/' + rowObject.TransactionId + '><img title=\"Show Details\" alt=\"Show Details\" style=border-style:none src=" + uri.Content("~/Content/Images/show_details.png") +
                " /></a>&nbsp;<a style=cursor:hand href = " + billingHistoryUrl + "/' +rowObject.TransactionId+'/AWB><img title=\"Show Audit Trail\" alt=Show Details style=border-style:none src=" +
                uri.Content("~/Content/Images/show_audit_trail.png") + " /></a>';}");
      sb.Append("if(rowObject.TransactionType == 'AWB Charge Collect'){ return '<a style=cursor:hand href = " + showInvoice.Replace("/View", "") +
                "/' +rowObject.InvoiceId+'/AwbChargeCollectBillingView/' + rowObject.TransactionId + '><img title=\"Show Details\" alt=\"Show Details\" style=border-style:none src=" + uri.Content("~/Content/Images/show_details.png") +
                " /></a>&nbsp;<a style=cursor:hand href = " + billingHistoryUrl + "/' +rowObject.TransactionId+'/AWB><img title=\"Show Audit Trail\" alt=Show Details style=border-style:none src=" +
                uri.Content("~/Content/Images/show_audit_trail.png") + " /></a>';}");
      sb.Append("else if(rowObject.TransactionType == 'Rejection Memo'){");
      sb.Append(" return '<a style=cursor:hand href = " + showInvoice.Replace("/View", "") +
                "/' +rowObject.InvoiceId+'/RMView/' + rowObject.TransactionId + '><img title=\"Show Details\" alt=\"Show Details\" style=border-style:none src=" + uri.Content("~/Content/Images/show_details.png") +
                " /></a>&nbsp;<a style=cursor:hand href = " + billingHistoryUrl + "/' +rowObject.TransactionId+'/RM><img title=\"Show Audit Trail\" alt=Show Details style=border-style:none src=" +
                uri.Content("~/Content/Images/show_audit_trail.png") + " /></a>';}");
      sb.Append("else if(rowObject.TransactionType == 'Billing Memo'){");
      sb.Append(" return '<a style=cursor:hand href = " + showInvoice.Replace("/View", "") +
                "/' +rowObject.InvoiceId+'/BMView/' + rowObject.TransactionId + '><img title=\"Show Details\" alt=\"Show Details\" style=border-style:none src=" + uri.Content("~/Content/Images/show_details.png") +
                " /></a>&nbsp;<a style=cursor:hand href = " + billingHistoryUrl + "/' +rowObject.TransactionId+'/BM><img title=\"Show Audit Trail\" alt=Show Details style=border-style:none src=" +
                uri.Content("~/Content/Images/show_audit_trail.png") + " /></a>';}");
      sb.Append("else if(rowObject.TransactionType == 'Credit Memo'){");
      sb.Append(" return '<a style=cursor:hand href = " + showCreditNote.Replace("/View", "") +
                "/' +rowObjectInvoiceId+'/CMView/' + rowObject.TransactionId + '><img title=\"Show Details\" alt=\"Show Details\" style=border-style:none src=" + uri.Content("~/Content/Images/show_details.png") +
                " /></a>&nbsp;<a style=cursor:hand href = " + billingHistoryUrl + "/' +rowObject.TransactionId+'/CM><img title=\"Show Audit Trail\" alt=Show Details style=border-style:none src=" +
                uri.Content("~/Content/Images/show_audit_trail.png") + " /></a>';}}");
      sb.Append("</script>");

      return MvcHtmlString.Create(sb.ToString());
    }

    /// <summary>
    /// Generates html for a hyperlink which should open up a dialog on click.
    /// </summary>
    /// <param name="displayName">Text of the hyperlink.</param>
    /// <param name="title">Title of the popup window.</param>
    /// <param name="divName">Id of div that should pop up.</param>
    /// <param name="height">Height of modal dialog.</param>
    /// <param name="width">Width of model dialog.</param>
    /// <returns>Html of the link which will open up the specified div in a popup.</returns>
    public static MvcHtmlString GenerateDialogueHtml(string displayName, string title, string divName, int height, int width)
    {
         var sb = new StringBuilder();
        sb.AppendFormat(
          "<a href='#' class='ignoredirty' onclick=\"$('#{0}').dialog({{ closeOnEscape: false, title: '{1}',height: {2}, width: {3}, modal: true, resizable: false}}); return false;\">{4}</a>",
          divName,
          title,
          height,
          width,
          displayName);
        return MvcHtmlString.Create(sb.ToString());
    }

    /// <summary>
    /// This method generates script to display control as modal dialog. Generate url with given Id
    /// </summary>
    public static MvcHtmlString GenerateDialogueHtml(string displayName, string title, string divName, int height = 500, int width = 800, string id = null)
    {
        //SCP47630: Unable to submit "Miscellaneous" rejection
        //logic: remove '!!!' mark when user click on location id link
        var sb = new StringBuilder();
        sb.Append("<a href='#' class='ignoredirty' ");
        if (!string.IsNullOrEmpty(id))
        {
            sb.AppendFormat("id='{0}' ", id);
        }

        sb.AppendFormat(" onclick=\"$('#{0}').dialog({{open:function(ui,event) {{$('#MemberLocationInformation_1__OrganizationName').val($('#MemberLocationInformation_1__OrganizationName').val().replace('!!!',''));}},closeOnEscape: false, title: '{1}',height: {2}, width: {3}, modal: true, resizable: false}}); return false;\">{4}</a>",
                        divName,
                        title,
                        height,
                        width,
                        displayName);

        return MvcHtmlString.Create(sb.ToString());
    }

    public static MvcHtmlString GenerateGridEditViewValidateDeleteScript(UrlHelper uri, string jqGridId, string actionEdit, string actionValidate, string actionDelete, string actionView)
    {
      var jqGridEditMethodName = string.Format("{0}_EditViewDeleteRecord(cellValue, options, rowObject)", jqGridId);
      var deleteMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", actionDelete, "#" + jqGridId);
      var validateMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", actionValidate, "#" + jqGridId);
      var sb = new StringBuilder();
      // Index 9 : InvoiceStatusId
      sb.Append("<script type='text/javascript'>");
      sb.AppendFormat("function {0} {{", jqGridEditMethodName);
      sb.AppendFormat("if(rowObject[14]=={0} || rowObject[14]=={1} || rowObject[14]=={2} || rowObject[14]=={3} || rowObject[14]=={4} || rowObject[14]=={5} || rowObject[14]=={6}){{",
                      (int)InvoiceStatusType.ReadyForBilling,
                      (int)InvoiceStatusType.Presented,
                      (int)InvoiceStatusType.ProcessingComplete,
                      (int)InvoiceStatusType.Claimed,
                      (int)InvoiceStatusType.ErrorNonCorrectable,
                      (int)InvoiceStatusType.ErrorNonCorrectable,
                      (int)InvoiceStatusType.OnHold);
      sb.AppendFormat("return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a>';",
                      actionView,
                      uri.Content("~/Content/Images/view.png"));
      sb.Append("} else {");
      sb.AppendFormat(
        "return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=Edit alt=Edit style=border-style:none src={1} /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:validateRecord({2});><img title=Validate alt=View style=border-style:none src={3} /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:deleteRecord({4});><img title=Delete style=border-style:none src={5} /></a>';",
        actionEdit,
        uri.Content("~/Content/Images/edit.png"),
        validateMethod,
        uri.Content("~/Content/Images/validate.png"),
        deleteMethod,
        uri.Content("~/Content/Images/delete.png"));
      sb.Append("}} </script>");

      return MvcHtmlString.Create(sb.ToString());
    }

    public static MvcHtmlString GenerateMiscGridEditViewValidateDeleteScript(UrlHelper uri,
                                                                             string jqGridId,
                                                                             string actionEdit,
                                                                             string actionValidate,
                                                                             string actionDelete,
                                                                             string actionView,
                                                                             string actionDownloadPdf,
                                                                             string actionDownloadZip,
                                                                             int rejectionOnValidationFlag)
    {
      var jqGridEditMethodName = string.Format("{0}_EditViewDeleteRecord(cellValue, options, rowObject)", jqGridId);
      var deleteMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", actionDelete, "#" + jqGridId);
      var validateMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", actionValidate, "#" + jqGridId);
      var downloadZipMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", actionDownloadZip, "#" + jqGridId);
      var sb = new StringBuilder();

      sb.Append("<script type='text/javascript'>");
      sb.AppendFormat("function {0} {{", jqGridEditMethodName);

      //rowObject[0]=>InvoiceId,  rowObject[1]=>InvoiceStatusId  
      //rowObject[2]=>SubmissionMethodId, rowObject[6]=>InputFileStatusId
      //sb.Append("alert(rowObject[0] + '-' + rowObject[1] + '-' + rowObject[2] + '-' + rowObject[6]);");
      //SCP#68397 - Deleted invoice (XB-T01) pushed to ICH 
      // generate script for invoice with Submission method as IS-XML or IS-IDEC.
      //**************************************************************************************************************************
      sb.AppendFormat("if(rowObject[2]=={0} || rowObject[2]=={1}){{", (int)SubmissionMethod.IsXml, (int)SubmissionMethod.IsIdec);
      // conditions for Invoice Status in IS-XML or IS-IDEC
      sb.AppendFormat("if(((rowObject[1]=={0}) ||(rowObject[1]=={1})) && (rowObject[6]=={2}) ){{",
          (int)InvoiceStatusType.ErrorCorrectable, (int)InvoiceStatusType.ErrorNonCorrectable, (int)FileStatusType.ValidationCompleted);
      
      // sb.Append(GetDeleteScript(uri, deleteMethod));
      sb.Append(GetViewDeleteScript(uri, actionView, deleteMethod));
     
      sb.AppendFormat("}} else if((rowObject[1]=={0}) ) {{",
                      (int)InvoiceStatusType.OnHold);
      // script for view....
      sb.Append(GetViewScript(uri, actionView));
      sb.AppendFormat("}} else if( (rowObject[1]=={0}) || (rowObject[1]=={1}) || (rowObject[1]=={2}) || (rowObject[1]=={3}) || (rowObject[1]=={4}) ){{",
                      (int)InvoiceStatusType.ProcessingComplete,
                      (int)InvoiceStatusType.Presented,
                      (int)InvoiceStatusType.Claimed,
                      (int)InvoiceStatusType.ReadyForBilling,
                      (int)InvoiceStatusType.FutureSubmitted);
      // script for View, PDF, Zip
      sb.AppendFormat("if(rowObject[4] == 1 && (rowObject[5]=={0} || rowObject[5]=={1})) {{ ", (int)DigitalSignatureStatus.Completed, (int)DigitalSignatureStatus.NotRequired); // check for if PDF has been generated.
      sb.Append(GetViewPdfZipScript(uri, actionView, actionDownloadPdf, downloadZipMethod));
      sb.Append("} else {"); // if PDF is not generated then do not include PDF download in actions.
      sb.Append(GetInvoiceViewZipScript(uri, actionView, downloadZipMethod));
      sb.Append("}"); // end of check for PDF is generated.
      //SCP ID : 130459 - BVD / AB testing & ID: 130459 - BVD / AB testing
      //Logic: Download PDF button will be display in case of Autobilling also. 
      sb.AppendFormat("}} else if(rowObject[2]=={0}){{", (int)SubmissionMethod.AutoBilling); // Empty action script for default condition when invoice status criteria is not match.
      sb.AppendFormat("if(rowObject[4] == 1 && (rowObject[5]=={0} || rowObject[5]=={1})) {{ ", (int)DigitalSignatureStatus.Completed, (int)DigitalSignatureStatus.NotRequired); // check for if PDF has been generated.
      sb.Append(GetViewPdfZipScript(uri, actionView, actionDownloadPdf, downloadZipMethod));
      sb.Append("} else {"); // if PDF is not generated then do not include PDF download in actions.
      sb.Append(GetInvoiceViewZipScript(uri, actionView, downloadZipMethod));
      sb.Append("}"); // end of check for PDF is generated.
      sb.Append(GetViewScript(uri, actionView));
      sb.Append("} else {"); // Empty action script for default condition when invoice status criteria is not match.
      sb.Append(GetViewScript(uri, actionView));
      sb.Append("}"); // end of conditions Invoice Status in IS-XML or IS-IDEC.
      
      // else Submission method is IS-Web.
      //**************************************************************************************************************************
      sb.AppendFormat("}} else if(rowObject[2]=={0}) {{ ", (int)SubmissionMethod.IsWeb); 
      sb.AppendFormat("if((rowObject[1]=={0}) || (rowObject[1]=={1}) ) {{", (int)InvoiceStatusType.Open, (int)InvoiceStatusType.ValidationError);
      // script for Edit, Delete, Validate
      sb.Append(GetEditDeleteValidateScript(uri, actionEdit, deleteMethod, validateMethod));
      sb.AppendFormat("}} else if(rowObject[1]=={0}){{", (int)InvoiceStatusType.ReadyForSubmission);
      // script for Edit, Delete, Submit
      sb.Append(GetEditDeleteScript(uri, actionEdit, deleteMethod));
      sb.AppendFormat("}} else if((rowObject[1]=={0}) || (rowObject[1]=={1}) || (rowObject[1]=={2}) || (rowObject[1]=={3}) || (rowObject[1]=={4}) ){{",
                      (int)InvoiceStatusType.Presented,
                      (int)InvoiceStatusType.ProcessingComplete,
                      (int)InvoiceStatusType.Claimed,
                      (int)InvoiceStatusType.ReadyForBilling, 
                      (int)InvoiceStatusType.FutureSubmitted);
      // script for View, PDF, Zip
      sb.AppendFormat("if(rowObject[4] == 1 && (rowObject[5]=={0} || rowObject[5]=={1})) {{ ", (int)DigitalSignatureStatus.Completed, (int)DigitalSignatureStatus.NotRequired); // check for if PDF has been generated.
      sb.Append(GetViewPdfZipScript(uri, actionView, actionDownloadPdf, downloadZipMethod));
      sb.Append(" } else { "); // if PDF is not generated then do not include PDF download in actions.
      sb.Append(GetInvoiceViewZipScript(uri, actionView, downloadZipMethod));
      sb.Append(" }"); // end of check for PDF is generated.
      sb.Append("} else {"); // Empty action script for default condition when invoice status criteria is not match.
      sb.Append(GetEmptyActionScript());
      sb.Append("}"); // end of conditions Invoice Status in IS-Web.
      //SCP ID : 130459 - BVD / AB testing & ID: 130459 - BVD / AB testing
      //Logic: Download PDF button will be display in case of Autobilling also. 
      sb.AppendFormat("}} else if (rowObject[2] == {0}) {{", (int)SubmissionMethod.AutoBilling); // end of Else If Submission method is IS-Web.
      sb.AppendFormat("if(rowObject[4] == 1 && (rowObject[5]=={0} || rowObject[5]=={1})) {{ ", (int)DigitalSignatureStatus.Completed, (int)DigitalSignatureStatus.NotRequired); // check for if PDF has been generated.
      sb.Append(GetViewPdfZipScript(uri, actionView, actionDownloadPdf, downloadZipMethod));
      sb.Append(" } else { "); // if PDF is not generated then do not include PDF download in actions.
      sb.Append(GetInvoiceViewZipScript(uri, actionView, downloadZipMethod));
      sb.Append("}"); // end of check for PDF is generated.
      sb.Append(GetViewScript(uri, actionView));
      sb.Append("} else {"); // end of default condition for Submission method block.
      sb.Append(GetEmptyActionScript());
      sb.Append("}");
      sb.Append("} </script>");

      return MvcHtmlString.Create(sb.ToString());
    }

      /// <summary>
      ///  Jqgrid script for MISC Receivable Manage Invoice Screen
      ///  SCP401669 - Misc Permissions issue
      /// </summary>
      /// <param name="uri"></param>
      /// <param name="jqGridId"></param>
      /// <param name="actionEdit"></param>
      /// <param name="actionValidate"></param>
      /// <param name="actionDelete"></param>
      /// <param name="actionView"></param>
      /// <param name="actionDownloadPdf"></param>
      /// <param name="actionDownloadZip"></param>
      /// <param name="rejectionOnValidationFlag"></param>
      /// <returns></returns>
    public static MvcHtmlString GenerateRecMiscGridEditViewValidateDeleteScript(UrlHelper uri,
                                                                         string jqGridId,
                                                                         string actionEdit,
                                                                         string actionValidate,
                                                                         string actionDelete,
                                                                         string actionView,
                                                                         string actionDownloadPdf,
                                                                         string actionDownloadZip,
                                                                         int rejectionOnValidationFlag)
    {
        var authorizationManager = Ioc.Resolve<IAuthorizationManager>();

        var jqGridEditMethodName = string.Format("{0}_EditViewDeleteRecord(cellValue, options, rowObject)", jqGridId);
        var deleteMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", actionDelete, "#" + jqGridId);
        var validateMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", actionValidate, "#" + jqGridId);
        var downloadZipMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", actionDownloadZip, "#" + jqGridId);

        var canInvCreateOrEdit = authorizationManager.IsAuthorized(SessionUtil.UserId, (int)Business.Security.Permissions.Misc.Receivables.CreditNote.CreateOrEdit);
        var canInvDownload = authorizationManager.IsAuthorized(SessionUtil.UserId, (int)Business.Security.Permissions.Misc.Receivables.CreditNote.Download);
        var canInvValidate = authorizationManager.IsAuthorized(SessionUtil.UserId, (int)Business.Security.Permissions.Misc.Receivables.CreditNote.Validate);
        var canInvView = authorizationManager.IsAuthorized(SessionUtil.UserId, (int)Business.Security.Permissions.Misc.Receivables.CreditNote.View);
      
        // rowObject[0] is InvoiceId.
        // rowObject[1] is InvoiceStatusId.
        // rowObject[2] is SubmissionMethodId.
        // rowObject[5] is DigitalSignatureStatus.
        // rowObject[6] is InputFileStatusId.
        // rowObject[22] is InvoiceTypeId.

        var sb = new StringBuilder();

        // Start of Script block
        sb.Append("<script type='text/javascript'>");
        sb.AppendFormat("function {0} {{", jqGridEditMethodName);

        /*  Invoice Type = Credit Note */

        // if Invoice type is Credit Note
        sb.AppendFormat("if(rowObject.InvoiceTypeId=={0} ){{", (int)InvoiceType.CreditNote);

        // if Invoice Submission method is IS-XML or IS-IDEC
        sb.AppendFormat("if(rowObject.SubmissionMethodId=={0}){{", (int)SubmissionMethod.IsXml);

        sb.AppendFormat("if(((rowObject.InvoiceStatusId=={0}) ||(rowObject.InvoiceStatusId=={1})) && (rowObject.InputFileStatusId=={2}) ){{",
            (int)InvoiceStatusType.ErrorCorrectable, (int)InvoiceStatusType.ErrorNonCorrectable, (int)FileStatusType.ValidationCompleted);

        sb.Append(GetViewDeleteScript(uri, actionView, deleteMethod, canInvView, canInvCreateOrEdit));

        sb.AppendFormat("}} else if((rowObject.InvoiceStatusId=={0}) ) {{",
                        (int)InvoiceStatusType.OnHold);
        // script for view....
        if (canInvView)
        {
            sb.Append(GetViewScript(uri, actionView));
        }
        else { sb.Append(GetEmptyActionScript()); }
        sb.AppendFormat("}} else if( (rowObject.InvoiceStatusId=={0}) || (rowObject.InvoiceStatusId=={1}) || (rowObject.InvoiceStatusId=={2}) || (rowObject.InvoiceStatusId=={3}) || (rowObject.InvoiceStatusId=={4}) ){{",
                        (int)InvoiceStatusType.ProcessingComplete,
                        (int)InvoiceStatusType.Presented,
                        (int)InvoiceStatusType.Claimed,
                        (int)InvoiceStatusType.ReadyForBilling,
                        (int)InvoiceStatusType.FutureSubmitted);
        // script for View, PDF, Zip
        sb.AppendFormat("if(rowObject.IsLegalPdfGenerated == 1 && (rowObject.DigitalSignatureStatusId=={0} || rowObject.DigitalSignatureStatusId=={1})) {{ ", (int)DigitalSignatureStatus.Completed, (int)DigitalSignatureStatus.NotRequired); // check for if PDF has been generated.
    
        sb.Append(GetViewPdfZipScript(uri, actionView, actionDownloadPdf, downloadZipMethod, canInvView, canInvDownload));
        
        sb.Append("} else {"); // if PDF is not generated then do not include PDF download in actions.
        sb.Append(GetInvoiceViewZipScript(uri, actionView, downloadZipMethod, canInvView, canInvDownload));
        sb.Append("}"); // end of check for PDF is generated.
        
        //Logic: Download PDF button will be display in case of Autobilling also. 
        sb.AppendFormat("}} else if(rowObject.SubmissionMethodId=={0}){{", (int)SubmissionMethod.AutoBilling); // Empty action script for default condition when invoice status criteria is not match.
        sb.AppendFormat("if(rowObject.IsLegalPdfGenerated == 1 && (rowObject.DigitalSignatureStatusId=={0} || rowObject.DigitalSignatureStatusId=={1})) {{ ", (int)DigitalSignatureStatus.Completed, (int)DigitalSignatureStatus.NotRequired); // check for if PDF has been generated.
        sb.Append(GetViewPdfZipScript(uri, actionView, actionDownloadPdf, downloadZipMethod, canInvView, canInvDownload));
        sb.Append("} else {"); // if PDF is not generated then do not include PDF download in actions.
        sb.Append(GetInvoiceViewZipScript(uri, actionView, downloadZipMethod, canInvView, canInvDownload));        
        sb.Append("}"); // end of check for PDF is generated.
        if (canInvView)
        {
            sb.Append(GetViewScript(uri, actionView));
        }
        else { sb.Append(GetEmptyActionScript()); }
        sb.Append("} else {"); // Empty action script for default condition when invoice status criteria is not match.
        if (canInvView)
        {
            sb.Append(GetViewScript(uri, actionView));
        }
        else { sb.Append(GetEmptyActionScript()); }
        sb.Append("}"); // end of conditions Invoice Status in IS-XML or IS-IDEC.

        // else Submission method is IS-Web.
        //**************************************************************************************************************************
        sb.AppendFormat("}} else if(rowObject.SubmissionMethodId=={0}) {{ ", (int)SubmissionMethod.IsWeb);
        sb.AppendFormat("if((rowObject.InvoiceStatusId=={0}) || (rowObject.InvoiceStatusId=={1}) ) {{", (int)InvoiceStatusType.Open, (int)InvoiceStatusType.ValidationError);
        // script for Edit, Delete, Validate
        sb.Append(GetEditDeleteValidateScript(uri, actionEdit, deleteMethod, validateMethod, canInvCreateOrEdit, canInvValidate));
        sb.AppendFormat("}} else if(rowObject.InvoiceStatusId=={0}){{", (int)InvoiceStatusType.ReadyForSubmission);
        // script for Edit, Delete, Submit
        if (canInvCreateOrEdit)
        {
            sb.Append(GetEditDeleteScript(uri, actionEdit, deleteMethod));
        }
        else { sb.Append(GetEmptyActionScript()); }
        sb.AppendFormat("}} else if((rowObject.InvoiceStatusId=={0}) || (rowObject.InvoiceStatusId=={1}) || (rowObject.InvoiceStatusId=={2}) || (rowObject.InvoiceStatusId=={3}) || (rowObject.InvoiceStatusId=={4}) ){{",
                        (int)InvoiceStatusType.Presented,
                        (int)InvoiceStatusType.ProcessingComplete,
                        (int)InvoiceStatusType.Claimed,
                        (int)InvoiceStatusType.ReadyForBilling,
                        (int)InvoiceStatusType.FutureSubmitted);
        // script for View, PDF, Zip
        sb.AppendFormat("if(rowObject.IsLegalPdfGenerated == 1 && (rowObject.DigitalSignatureStatusId=={0} || rowObject.DigitalSignatureStatusId=={1})) {{ ", (int)DigitalSignatureStatus.Completed, (int)DigitalSignatureStatus.NotRequired); // check for if PDF has been generated.
        sb.Append(GetViewPdfZipScript(uri, actionView, actionDownloadPdf, downloadZipMethod, canInvView, canInvDownload));
        sb.Append(" } else { "); // if PDF is not generated then do not include PDF download in actions.
        sb.Append(GetInvoiceViewZipScript(uri, actionView, downloadZipMethod, canInvView, canInvDownload));
        sb.Append(" }"); // end of check for PDF is generated.
        sb.Append("} else {"); // Empty action script for default condition when invoice status criteria is not match.
        sb.Append(GetEmptyActionScript());
        sb.Append("}"); // end of conditions Invoice Status in IS-Web.
        
        //Logic: Download PDF button will be display in case of Autobilling also. 
        sb.AppendFormat("}} else if (rowObject.SubmissionMethodId == {0}) {{", (int)SubmissionMethod.AutoBilling); // end of Else If Submission method is IS-Web.
        sb.AppendFormat("if(rowObject.IsLegalPdfGenerated == 1 && (rowObject.DigitalSignatureStatusId=={0} || rowObject.DigitalSignatureStatusId=={1})) {{ ", (int)DigitalSignatureStatus.Completed, (int)DigitalSignatureStatus.NotRequired); // check for if PDF has been generated.
        sb.Append(GetViewPdfZipScript(uri, actionView, actionDownloadPdf, downloadZipMethod, canInvView, canInvDownload));
        sb.Append(" } else { "); // if PDF is not generated then do not include PDF download in actions.
        sb.Append(GetInvoiceViewZipScript(uri, actionView, downloadZipMethod, canInvView, canInvDownload));
        sb.Append("}"); // end of check for PDF is generated.
        if (canInvView)
        {
            sb.Append(GetViewScript(uri, actionView));
        }
        else { sb.Append(GetEmptyActionScript()); }
        sb.Append("} else {"); // end of default condition for Submission method block.
        sb.Append(GetEmptyActionScript());
        sb.Append("}");

        // End of If Invoice type is Credit Note
        sb.Append("} else {");
        // else Invoice type is NOT Credit Note

        /*  Invoice Type = Invoice */

        canInvCreateOrEdit = authorizationManager.IsAuthorized(SessionUtil.UserId, (int)Business.Security.Permissions.Misc.Receivables.Invoice.CreateOrEdit);
        canInvDownload = authorizationManager.IsAuthorized(SessionUtil.UserId, (int)Business.Security.Permissions.Misc.Receivables.Invoice.Download);
        canInvValidate = authorizationManager.IsAuthorized(SessionUtil.UserId, (int)Business.Security.Permissions.Misc.Receivables.Invoice.Validate);
        canInvView = authorizationManager.IsAuthorized(SessionUtil.UserId, (int)Business.Security.Permissions.Misc.Receivables.Invoice.View);

        // if Invoice Submission method is IS-XML or IS-IDEC
        sb.AppendFormat("if(rowObject.SubmissionMethodId=={0}){{", (int)SubmissionMethod.IsXml);

        sb.AppendFormat("if(((rowObject.InvoiceStatusId=={0}) ||(rowObject.InvoiceStatusId=={1})) && (rowObject.InputFileStatusId=={2}) ){{",
            (int)InvoiceStatusType.ErrorCorrectable, (int)InvoiceStatusType.ErrorNonCorrectable, (int)FileStatusType.ValidationCompleted);

        sb.Append(GetViewDeleteScript(uri, actionView, deleteMethod, canInvView, canInvCreateOrEdit));

        sb.AppendFormat("}} else if((rowObject.InvoiceStatusId=={0}) ) {{",
                        (int)InvoiceStatusType.OnHold);
        // script for view....
        if (canInvView)
        {
            sb.Append(GetViewScript(uri, actionView));
        }
        else { sb.Append(GetEmptyActionScript()); }
        sb.AppendFormat("}} else if( (rowObject.InvoiceStatusId=={0}) || (rowObject.InvoiceStatusId=={1}) || (rowObject.InvoiceStatusId=={2}) || (rowObject.InvoiceStatusId=={3}) || (rowObject.InvoiceStatusId=={4}) ){{",
                        (int)InvoiceStatusType.ProcessingComplete,
                        (int)InvoiceStatusType.Presented,
                        (int)InvoiceStatusType.Claimed,
                        (int)InvoiceStatusType.ReadyForBilling,
                        (int)InvoiceStatusType.FutureSubmitted);
        // script for View, PDF, Zip
        sb.AppendFormat("if(rowObject.IsLegalPdfGenerated == 1 && (rowObject.DigitalSignatureStatusId=={0} || rowObject.DigitalSignatureStatusId=={1})) {{ ", (int)DigitalSignatureStatus.Completed, (int)DigitalSignatureStatus.NotRequired); // check for if PDF has been generated.

        sb.Append(GetViewPdfZipScript(uri, actionView, actionDownloadPdf, downloadZipMethod, canInvView, canInvDownload));

        sb.Append("} else {"); // if PDF is not generated then do not include PDF download in actions.
        sb.Append(GetInvoiceViewZipScript(uri, actionView, downloadZipMethod, canInvView, canInvDownload));
        sb.Append("}"); // end of check for PDF is generated.

        //Logic: Download PDF button will be display in case of Autobilling also. 
        sb.AppendFormat("}} else if(rowObject.SubmissionMethodId=={0}){{", (int)SubmissionMethod.AutoBilling); // Empty action script for default condition when invoice status criteria is not match.
        sb.AppendFormat("if(rowObject.IsLegalPdfGenerated == 1 && (rowObject.DigitalSignatureStatusId=={0} || rowObject.DigitalSignatureStatusId=={1})) {{ ", (int)DigitalSignatureStatus.Completed, (int)DigitalSignatureStatus.NotRequired); // check for if PDF has been generated.
        sb.Append(GetViewPdfZipScript(uri, actionView, actionDownloadPdf, downloadZipMethod, canInvView, canInvDownload));
        sb.Append("} else {"); // if PDF is not generated then do not include PDF download in actions.
        sb.Append(GetInvoiceViewZipScript(uri, actionView, downloadZipMethod, canInvView, canInvDownload));
        sb.Append("}"); // end of check for PDF is generated.
        if (canInvView)
        {
            sb.Append(GetViewScript(uri, actionView));
        }
        else { sb.Append(GetEmptyActionScript()); }
        sb.Append("} else {"); // Empty action script for default condition when invoice status criteria is not match.
        if (canInvView)
        {
            sb.Append(GetViewScript(uri, actionView));
        }
        else { sb.Append(GetEmptyActionScript()); }
        sb.Append("}"); // end of conditions Invoice Status in IS-XML or IS-IDEC.

        // else Submission method is IS-Web.
        //**************************************************************************************************************************
        sb.AppendFormat("}} else if(rowObject.SubmissionMethodId=={0}) {{ ", (int)SubmissionMethod.IsWeb);
        sb.AppendFormat("if((rowObject.InvoiceStatusId=={0}) || (rowObject.InvoiceStatusId=={1}) ) {{", (int)InvoiceStatusType.Open, (int)InvoiceStatusType.ValidationError);
        // script for Edit, Delete, Validate
        sb.Append(GetEditDeleteValidateScript(uri, actionEdit, deleteMethod, validateMethod, canInvCreateOrEdit, canInvValidate));
        sb.AppendFormat("}} else if(rowObject.InvoiceStatusId=={0}){{", (int)InvoiceStatusType.ReadyForSubmission);
        // script for Edit, Delete, Submit
        if (canInvCreateOrEdit)
        {
            sb.Append(GetEditDeleteScript(uri, actionEdit, deleteMethod));
        }
        else { sb.Append(GetEmptyActionScript()); }
        sb.AppendFormat("}} else if((rowObject.InvoiceStatusId=={0}) || (rowObject.InvoiceStatusId=={1}) || (rowObject.InvoiceStatusId=={2}) || (rowObject.InvoiceStatusId=={3}) || (rowObject.InvoiceStatusId=={4}) ){{",
                        (int)InvoiceStatusType.Presented,
                        (int)InvoiceStatusType.ProcessingComplete,
                        (int)InvoiceStatusType.Claimed,
                        (int)InvoiceStatusType.ReadyForBilling,
                        (int)InvoiceStatusType.FutureSubmitted);
        // script for View, PDF, Zip
        sb.AppendFormat("if(rowObject.IsLegalPdfGenerated == 1 && (rowObject.DigitalSignatureStatusId=={0} || rowObject.DigitalSignatureStatusId=={1})) {{ ", (int)DigitalSignatureStatus.Completed, (int)DigitalSignatureStatus.NotRequired); // check for if PDF has been generated.
        sb.Append(GetViewPdfZipScript(uri, actionView, actionDownloadPdf, downloadZipMethod, canInvView, canInvDownload));
        sb.Append(" } else { "); // if PDF is not generated then do not include PDF download in actions.
        sb.Append(GetInvoiceViewZipScript(uri, actionView, downloadZipMethod, canInvView, canInvDownload));
        sb.Append(" }"); // end of check for PDF is generated.
        sb.Append("} else {"); // Empty action script for default condition when invoice status criteria is not match.
        sb.Append(GetEmptyActionScript());
        sb.Append("}"); // end of conditions Invoice Status in IS-Web.
        
        //Logic: Download PDF button will be display in case of Autobilling also. 
        sb.AppendFormat("}} else if (rowObject.SubmissionMethodId == {0}) {{", (int)SubmissionMethod.AutoBilling); // end of Else If Submission method is IS-Web.
        sb.AppendFormat("if(rowObject.IsLegalPdfGenerated == 1 && (rowObject.DigitalSignatureStatusId=={0} || rowObject.DigitalSignatureStatusId=={1})) {{ ", (int)DigitalSignatureStatus.Completed, (int)DigitalSignatureStatus.NotRequired); // check for if PDF has been generated.
        sb.Append(GetViewPdfZipScript(uri, actionView, actionDownloadPdf, downloadZipMethod, canInvView, canInvDownload));
        sb.Append(" } else { "); // if PDF is not generated then do not include PDF download in actions.
        sb.Append(GetInvoiceViewZipScript(uri, actionView, downloadZipMethod, canInvView, canInvDownload));
        sb.Append("}"); // end of check for PDF is generated.
        if (canInvView)
        {
            sb.Append(GetViewScript(uri, actionView));
        }
        else { sb.Append(GetEmptyActionScript()); }
        sb.Append("} else {"); // end of default condition for Submission method block.
        sb.Append(GetEmptyActionScript());
        sb.Append("}");

        sb.Append("}");
        // End of else Invoice type is NOT Credit Note

        sb.Append("} </script>");
        // Start of Script block

        return MvcHtmlString.Create(sb.ToString());
    }


    private static string GetEditDeleteScript(UrlHelper uri, string actionEdit, string deleteMethod)
    {
      return
        string.Format(
          "return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=Edit alt=Edit style=border-style:none src={1} /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:deleteRecord({2});><img title=Delete style=border-style:none src={3} /></a>';",
          actionEdit,
          uri.Content("~/Content/Images/edit.png"),
          deleteMethod,
          uri.Content("~/Content/Images/delete.png"));
    }

    private static string GetEmptyActionScript()
    {
      return "return '';";
    }

    //CMP #665: User Related Enhancements-FRS-v1.2.doc [Sec 2.9: IS-WEB MISC Payables Invoice Search Screen]
    //Only rename the method name from GenerateMiscPayablesGridViewScript to GenerateUatpPayablesGridViewScript.
    //So that CMP #665 changes should not be impact on UATP category.
    public static MvcHtmlString GenerateUatpPayablesGridViewScript(UrlHelper uri, string jqGridId, string actionView, string actionReject, string actionDownloadPdf, string actionDownloadZip,
                                                       int correspondenceInvoiceTypeId, int rejectionInvoiceTypeId, int ichSettlementMethodId, int bilateralSettlementMethodId, int adjustmentDueToProtestSmi)
    {
        var jqGridEditMethodName = string.Format("{0}_ViewRejectRecord(cellValue, options, rowObject)", jqGridId);
        var rejectMethod = string.Format("\"{0}\",\"'+cellValue+'\"", actionReject);
        var downloadZipMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", actionDownloadZip, "#" + jqGridId);
        var sb = new StringBuilder();
        // Indices : 1- InvoiceTypeId, 2: RejectionStage, 3: SettlementMethodId.
        sb.Append("<script type='text/javascript'>");
        sb.Append("function " + jqGridEditMethodName + "{");
        //sb.Append(" if(rowObject[5] == 1 && (rowObject[6]!=2 && rowObject[6]!=4)) {alert(rowObject[5] + ' ' + rowObject[6]);}");
        // If invoice is correspondence invoice OR rejection invoice with rejection stage 1 and SMI =  I or B OR rejection invoice of stage 2, do not show reject icon.
        sb.AppendFormat("if(rowObject.InvoiceTypeId == {0} || (rowObject.InvoiceTypeId == {1} && rowObject.RejectionStage == 1 && (rowObject.SettlementMethodId == {2} || rowObject.SettlementMethodId == {3} || rowObject.SettlementMethodId == {4})) || (rowObject.InvoiceTypeId == {1} && rowObject.RejectionStage == 2)){{",
                        correspondenceInvoiceTypeId,
                        rejectionInvoiceTypeId,
                        bilateralSettlementMethodId,
                        ichSettlementMethodId,
                        adjustmentDueToProtestSmi);
        sb.AppendFormat("if(rowObject.IsLegalPdfGenerated == 1 &&(rowObject.DigitalSignatureStatusId=={0} || rowObject.DigitalSignatureStatusId=={1})) {{ ", (int)DigitalSignatureStatus.Completed, (int)DigitalSignatureStatus.NotRequired); // check for if PDF has been generated.
        sb.Append(GetViewPdfZipScript(uri, actionView, actionDownloadPdf, downloadZipMethod));
        sb.Append("} else {"); // if PDF is not generated then do not include PDF download in actions.
        sb.Append(GetInvoiceViewZipScript(uri, actionView, downloadZipMethod));
        sb.Append("}"); // end of check for PDF is generated.
        sb.Append("}else{");
        sb.AppendFormat("if(rowObject.IsLegalPdfGenerated == 1 && (rowObject.DigitalSignatureStatusId=={0} || rowObject.DigitalSignatureStatusId=={1})) {{ ", (int)DigitalSignatureStatus.Completed, (int)DigitalSignatureStatus.NotRequired); // check for if PDF has been generated.
        sb.AppendFormat("return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:callRejectLineItems({2});><img title=Reject style=border-style:none src={3} /></a> &nbsp;<a style=cursor:hand target=_parent href={4}/'+cellValue+'><img title=Pdf alt=Pdf style=border-style:none src={5} /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:downloadZip({6});><img title=Zip alt=Zip style=border-style:none src={7} /></a>';",
         actionView,
         uri.Content("~/Content/Images/view.png"),
         rejectMethod,
         uri.Content("~/Content/Images/reject_invoice.png"),
         actionDownloadPdf,
         uri.Content("~/Content/Images/pdf.png"),
         downloadZipMethod,
         uri.Content("~/Content/Images/zip.png"));
        sb.Append("} else {"); // if PDF is not generated then do not include PDF download in actions.
        sb.AppendFormat("return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:callRejectLineItems({2});><img title=Reject style=border-style:none src={3} /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:downloadZip({4});><img title=Zip alt=Zip style=border-style:none src={5} /></a>';",
         actionView,
         uri.Content("~/Content/Images/view.png"),
         rejectMethod,
         uri.Content("~/Content/Images/reject_invoice.png"),
         downloadZipMethod,
         uri.Content("~/Content/Images/zip.png"));
        sb.Append("}"); // end of check for PDF is generated.
        sb.Append("}}</script>");

        return MvcHtmlString.Create(sb.ToString());
    }


    //CMP #665: User Related Enhancements-FRS-v1.2.doc [Sec 2.9: IS-WEB MISC Payables Invoice Search Screen]
    //Added two new icon as 'Download Listing', 'Attachment' for is-web misc payable invoice search.
    public static MvcHtmlString GenerateMiscPayablesGridViewScript(UrlHelper uri, string jqGridId, string actionView, string actionReject, string actionDownloadPdf, string actionDownloadListing, string actionDownloadZip,
                                                            int correspondenceInvoiceTypeId, int rejectionInvoiceTypeId, int ichSettlementMethodId, int bilateralSettlementMethodId, int adjustmentDueToProtestSmi, int creditNoteTypeId)
    {
      //CMP #665: User Related Enhancements-FRS-v1.2.doc[Sec 2.9: IS-WEB MISC Payables Invoice Search Screen]
      //Resolve authorization manager.
      var authorizationManager = Ioc.Resolve<IAuthorizationManager>();

      var jqGridEditMethodName = string.Format("{0}_ViewRejectRecord(cellValue, options, rowObject)", jqGridId);
      var rejectMethod = string.Format("\"{0}\",\"'+cellValue+'\"", actionReject);
      var downloadZipMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", actionDownloadZip, "#" + jqGridId);
      // CMP #665: User Related Enhancements-FRS-v1.2.doc[Sec 2.9: IS-WEB MISC Payables Invoice Search Screen]
      var canCreditInvDownload = authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.Misc.Payables.CreditNoteBillings.Download);
      var canInvDownload = authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.Misc.Payables.Invoice.Download);
      
      var sb = new StringBuilder();
      // Indices : 1- InvoiceTypeId, 2: RejectionStage, 3: SettlementMethodId.
      sb.Append("<script type='text/javascript'>");
      sb.Append("function " + jqGridEditMethodName + "{");
      //CMP #665: User Related Enhancements-FRS-v1.2.doc[Sec 2.9: IS-WEB MISC Payables Invoice Search Screen]
      //If invoice type is credit memo, pass the permission respectively.
      sb.AppendFormat("if(rowObject[1] == {0}) {{", creditNoteTypeId);
      GenerateMiscPayablesGridScript(sb, correspondenceInvoiceTypeId, rejectionInvoiceTypeId, bilateralSettlementMethodId, ichSettlementMethodId, adjustmentDueToProtestSmi, uri, actionView, actionDownloadPdf, downloadZipMethod, actionDownloadListing, canCreditInvDownload, rejectMethod);
      sb.Append("} else {");
      GenerateMiscPayablesGridScript(sb, correspondenceInvoiceTypeId, rejectionInvoiceTypeId, bilateralSettlementMethodId, ichSettlementMethodId, adjustmentDueToProtestSmi, uri, actionView, actionDownloadPdf, downloadZipMethod, actionDownloadListing, canInvDownload, rejectMethod);
      sb.Append("}");
      sb.Append("}</script>");
      return MvcHtmlString.Create(sb.ToString());
    }


    /// <summary>
    /// This function is used to generate misc daily payable grid view.
    /// CMP #665: User Related Enhancements-FRS-v1.2.doc[Sec 2.10: IS-WEB MISC Daily Payables Invoice Search Screen
    /// CMP529 : Daily Output Generation for MISC Bilateral Invoices
    /// SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions [Misc Payable View Daily Bilateral Invoices]
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="jqGridId"></param>
    /// <param name="actionView"></param>
    /// <param name="actionDownloadPdf"></param>
    /// <param name="actionDownloadListing"></param>
    /// <param name="actionDownloadZip"></param>
    /// <returns></returns>
    public static MvcHtmlString GenerateMiscDailyPayablesGridViewScript(UrlHelper uri, string jqGridId, string actionView, string actionDownloadPdf,
                                                                        string actionDownloadListing, string actionDownloadZip)
    {
        var jqGridEditMethodName = string.Format("{0}_ActionItems(cellValue, options, rowObject)", jqGridId);
        var downloadZipMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", actionDownloadZip, "#" + jqGridId);

        var authorizationManager = Ioc.Resolve<IAuthorizationManager>();

        var canView = authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.Misc.Payables.DailyBilateralDelivery.View);
        var canDownload = authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.Misc.Payables.DailyBilateralDelivery.Download);

        var stringBuilder = new StringBuilder();
        stringBuilder.Append("<script type='text/javascript'>");
        stringBuilder.Append("function " + jqGridEditMethodName + "{");

        stringBuilder.AppendFormat("if(rowObject.IsLegalPdfGenerated == 1 &&(rowObject.DigitalSignatureStatusId=={0} || rowObject.DigitalSignatureStatusId=={1})) {{",
                                                                          (int)DigitalSignatureStatus.Completed, (int)DigitalSignatureStatus.NotRequired); // check for if PDF has been generated.
        stringBuilder.Append(GetMiscPayViewPdfListingZipScript(uri, actionView, actionDownloadPdf, actionDownloadListing, downloadZipMethod, canView, canDownload, true));
        stringBuilder.Append("} else {"); // if PDF is not generated then do not include PDF download in actions.
        stringBuilder.Append(GetMiscPayInvoiceViewZipScript(uri, actionView, actionDownloadListing, downloadZipMethod, canView, canDownload, true));
        stringBuilder.Append("}}"); // end of check for PDF is generated.
        stringBuilder.Append("</script>");

        return MvcHtmlString.Create(stringBuilder.ToString());
    }

    private static string GetMiscPayViewPdfListingZipScript(UrlHelper uri, string actionView, string actionDownloadPdf, string actionDownloadListing,
                                                          string downloadZipMethod, bool hasViewPermission, bool hasInvDownloadPermission, bool isDailyPayable = false)
    {
        if (hasViewPermission && hasInvDownloadPermission)
        {
            return GetMiscPayViewPdfListingZipScript(uri, actionView, actionDownloadPdf, actionDownloadListing, downloadZipMethod, isDailyPayable);
        }
        else if (hasViewPermission)
        {
            return GetViewScript(uri, actionView);
        }
        else if (hasInvDownloadPermission)
        {
            return GetMiscPayDownloadPdfListingZipScript(uri, actionDownloadPdf, actionDownloadListing, downloadZipMethod, isDailyPayable);
        }
        return "return '';";
    }

    private static string GetMiscPayInvoiceViewZipScript(UrlHelper uri, string actionView, string actionDownloadListing, string downloadZipMethod,
                                                         bool hasViewPermission, bool hasInvDownloadPermission, bool isDailyPayable = false)
    {
        if (hasViewPermission && hasInvDownloadPermission)
        {
            return GetMiscPayInvoiceViewZipScript(uri, actionView, actionDownloadListing, downloadZipMethod, isDailyPayable);
        }
        else if (hasViewPermission)
        {
            return GetViewScript(uri, actionView);
        }
        else if (hasInvDownloadPermission)
        {
            return GetMiscPayInvoiceZipScript(uri, actionDownloadListing, downloadZipMethod, isDailyPayable);
        }
        return "return '';";
    }

    private static string GetMiscPayViewPdfListingZipScript(UrlHelper uri, string actionView, string actionDownloadPdf, string actionDownloadListing,
                                                                             string downloadZipMethod, bool isDailyPayable = false)
    {
        //return script include 5 icon as Invoice View, PDF, Zip, Listing, Attachment.
        return string.Format(
        "return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a>&nbsp;<a style=cursor:hand target=_parent href={2}/'+cellValue+'><img title=Pdf alt=Pdf style=border-style:none src={3} /></a>&nbsp;<a style=cursor:hand target=_parent href={4}?id='+cellValue+'&IsDailyPayable={5}><img title=Listing alt=Listing style=border-style:none src={6} /></a>&nbsp;<a style=cursor:hand target=_parent href=javascript:loadAttachment(\"'+cellValue+'\");><img  title=Attachment alt=Attachment style=border-style:none src={7} /></a>&nbsp;<a style=cursor:hand target=_parent href=javascript:downloadZip({8});><img title=Zip alt=Zip style=border-style:none src={9} /></a>';",
        actionView,
        uri.Content("~/Content/Images/view.png"),
        actionDownloadPdf,
        uri.Content("~/Content/Images/pdf.png"),
        actionDownloadListing,
        Convert.ToInt32(isDailyPayable),
        uri.Content("~/Content/Images/listing.png"),
        uri.Content("~/Content/Images/attachment.png"),
        downloadZipMethod,
        uri.Content("~/Content/Images/zip.png"));
    }

    private static string GetMiscPayDownloadPdfListingZipScript(UrlHelper uri, string actionDownloadPdf, string actionDownloadListing, string downloadZipMethod, bool isDailyPayable = false)
    {
        //return script include 4 icon as Invoice PDF, Zip, Listing, Attachment.
        return string.Format(
        "return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=Pdf alt=Pdf style=border-style:none src={1} /></a>&nbsp;<a style=cursor:hand target=_parent href={2}?id='+cellValue+'&IsDailyPayable={3}><img title=Listing alt=Listing style=border-style:none src={4} /></a>&nbsp;<a style=cursor:hand target=_parent href=javascript:loadAttachment(\"'+cellValue+'\");><img  title=Attachment alt=Attachment style=border-style:none src={5} /></a>&nbsp;<a style=cursor:hand target=_parent href=javascript:downloadZip({6});><img title=Zip alt=Zip style=border-style:none src={7} /></a>';",
        actionDownloadPdf,
        uri.Content("~/Content/Images/pdf.png"),
        actionDownloadListing,
        Convert.ToInt32(isDailyPayable),
        uri.Content("~/Content/Images/listing.png"),
        uri.Content("~/Content/Images/attachment.png"),
        downloadZipMethod,
        uri.Content("~/Content/Images/zip.png"));
    }

    private static string GetMiscPayInvoiceViewZipScript(UrlHelper uri, string actionView, string actionDownloadListing, string downloadZipMethod, bool isDailyPayable = false)
    {
        //return script include 5 icon as Invoice View, PDF, Zip, Listing, Attachment.
        return string.Format(
        "return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a>&nbsp;<a style=cursor:hand target=_parent href={2}?id='+cellValue+'&IsDailyPayable={3}><img title=Listing alt=Listing style=border-style:none src={4} /></a>&nbsp;<a style=cursor:hand target=_parent href=javascript:loadAttachment(\"'+cellValue+'\");><img  title=Attachment alt=Attachment style=border-style:none src={5} /></a>&nbsp;<a style=cursor:hand target=_parent href=javascript:downloadZip({6});><img title=Zip alt=Zip style=border-style:none src={7} /></a>';",
        actionView,
        uri.Content("~/Content/Images/view.png"),
        actionDownloadListing,
        Convert.ToInt32(isDailyPayable),
        uri.Content("~/Content/Images/listing.png"),
        uri.Content("~/Content/Images/attachment.png"),
        downloadZipMethod,
        uri.Content("~/Content/Images/zip.png"));
    }

    private static string GetMiscPayInvoiceZipScript(UrlHelper uri, string actionDownloadListing, string downloadZipMethod, bool isDailyPayable = false)
    {
        //return script include 3 icon as Invoice Listing, Attachment, Zip.
        return string.Format(
        "return '<a style=cursor:hand target=_parent href={0}?id='+cellValue+'&IsDailyPayable={1}><img title=Listing alt=Listing style=border-style:none src={2} /></a>&nbsp;<a style=cursor:hand target=_parent href=javascript:loadAttachment(\"'+cellValue+'\");><img  title=Attachment alt=Attachment style=border-style:none src={3} /></a>&nbsp;<a style=cursor:hand target=_parent href=javascript:downloadZip({4});><img title=Zip alt=Zip style=border-style:none src={5} /></a>';",
        actionDownloadListing,
        Convert.ToInt32(isDailyPayable),
        uri.Content("~/Content/Images/listing.png"),
        uri.Content("~/Content/Images/attachment.png"),
        downloadZipMethod,
        uri.Content("~/Content/Images/zip.png"));
    }

    public static MvcHtmlString GeneratePaxPayablesGridViewRejectScript(UrlHelper uri, string jqGridId, string actionView, string transactionType, string invoiceId, string  initiateRejectionUrl, string duplicateRejectionUrl, int billingCode, int billingYear, int billingMonth, int billingPeriod, int smi)
    {
      var jqGridEditMethodName = string.Format("{0}_EditViewDeleteRecord(cellValue, options, rowObject)", jqGridId);

      var sb = new StringBuilder();
      var rejectionParams = string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"'+cellValue+'\"", jqGridId, transactionType, invoiceId, initiateRejectionUrl, duplicateRejectionUrl, billingCode, billingYear, billingMonth, billingPeriod, smi);
      sb.Append("<script type='text/javascript'>");
      sb.Append("function " + jqGridEditMethodName + "{");

      // For non-sampling RM, if Rejection stage is 3, show only view icon.
      if (transactionType == "RM" && billingCode == (int)BillingCode.NonSampling)
      {
        // Index 15: RejectionStage.
          sb.AppendFormat("if(rowObject.RejectionStage == 3)");
        sb.AppendFormat("return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a>';",
                        actionView,
                        uri.Content("~/Content/Images/view.png"));
        // For rejection stage 1
        sb.AppendFormat("if(rowObject.RejectionStage == 1){{");
        sb.AppendFormat("rejectionTransType = {0};", Convert.ToInt32(TransactionType.RejectionMemo2));
        // For rejection stage 2
        sb.Append("}else{");
        sb.AppendFormat("rejectionTransType = {0};", Convert.ToInt32(TransactionType.RejectionMemo3));
        sb.Append("}");
      }
      // For Form F
      else if (transactionType == "RM")
      {
        sb.AppendFormat("rejectionTransType = {0};", Convert.ToInt32(TransactionType.SamplingFormXF));
      }
      // For BM, if reason code is 6A or 6B, show only view icon.
      else if (transactionType == "BM")
      {
        sb.AppendFormat("rejectionTransType = {0};", Convert.ToInt32(TransactionType.RejectionMemo1)); // Transaction type : RM1
        // Index 13: Reason code.
        sb.AppendFormat("if((rowObject.ReasonCode == '6A') || (rowObject.ReasonCode == '6B'))");
        sb.AppendFormat("return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a>';",
                        actionView,
                        uri.Content("~/Content/Images/view.png"));
      }
      // For CM
      else if (transactionType == "CM")
      {
        sb.AppendFormat("rejectionTransType = {0};", Convert.ToInt32(TransactionType.RejectionMemo1)); // Transaction type : RM1
      }
      else if (transactionType == "FD")
      {
        sb.AppendFormat("rejectionTransType = {0};", Convert.ToInt32(TransactionType.SamplingFormF));
      }

      sb.AppendFormat("return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:InitiateRejForSpecificTrans({2});><img title=Reject style=border-style:none src={3} /></a>';",
      actionView,
      uri.Content("~/Content/Images/view.png"),
      rejectionParams,
      uri.Content("~/Content/Images/reject_invoice.png"));
      sb.Append("}</script>");

      return MvcHtmlString.Create(sb.ToString());
    }

    /// <summary>
    /// Method to generate Cargo Payable Credit Note grid action column icon script.
    /// SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="jqGridId"></param>
    /// <param name="actionView"></param>
    /// <param name="transactionType"></param>
    /// <param name="invoiceId"></param>
    /// <param name="initiateRejectionUrl"></param>
    /// <param name="duplicateRejectionUrl"></param>
    /// <param name="billingCode"></param>
    /// <param name="billingYear"></param>
    /// <param name="billingMonth"></param>
    /// <param name="billingPeriod"></param>
    /// <param name="smi"></param>
    /// <returns></returns>
    public static MvcHtmlString GenerateCgoPayablesCNoteGridViewRejectScript(UrlHelper uri, string jqGridId, string actionView, string transactionType, string invoiceId, string initiateRejectionUrl, string duplicateRejectionUrl, int billingCode, int billingYear, int billingMonth, int billingPeriod, int smi)
    {
        var jqGridEditMethodName = string.Format("{0}_EditViewDeleteRecord(cellValue, options, rowObject)", jqGridId);

        var sb = new StringBuilder();
        var rejectionParams = string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"'+cellValue+'\"", jqGridId, transactionType, invoiceId, initiateRejectionUrl, duplicateRejectionUrl, billingCode, billingYear, billingMonth, billingPeriod, smi);
        sb.Append("<script type='text/javascript'>");
        sb.Append("function " + jqGridEditMethodName + "{");

        // For non-sampling RM, if Rejection stage is 3, show only view icon.
        if (transactionType == "RM" && billingCode == (int)BillingCode.NonSampling)
        {
            // Index 15: RejectionStage.
            sb.AppendFormat("if(rowObject.RejectionStage == 3)");
            sb.AppendFormat("return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a>';",
                            actionView,
                            uri.Content("~/Content/Images/view.png"));
            // For rejection stage 1
            sb.AppendFormat("if(rowObject.RejectionStage == 1){{");
            sb.AppendFormat("rejectionTransType = {0};", Convert.ToInt32(TransactionType.RejectionMemo2));
            // For rejection stage 2
            sb.Append("}else{");
            sb.AppendFormat("rejectionTransType = {0};", Convert.ToInt32(TransactionType.RejectionMemo3));
            sb.Append("}");
        }
        // For Form F
        else if (transactionType == "RM")
        {
            sb.AppendFormat("rejectionTransType = {0};", Convert.ToInt32(TransactionType.SamplingFormXF));
        }
        // For BM, if reason code is 6A or 6B, show only view icon.
        else if (transactionType == "BM")
        {
            sb.AppendFormat("rejectionTransType = {0};", Convert.ToInt32(TransactionType.RejectionMemo1)); // Transaction type : RM1
            // Index 13: Reason code.
            sb.AppendFormat("if((rowObject.ReasonCode == '6A') || (rowObject.ReasonCode == '6B'))");
            sb.AppendFormat("return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a>';",
                            actionView,
                            uri.Content("~/Content/Images/view.png"));
        }
        // For CM
        else if (transactionType == "CM")
        {
            sb.AppendFormat("rejectionTransType = {0};", Convert.ToInt32(TransactionType.RejectionMemo1)); // Transaction type : RM1
        }
        else if (transactionType == "FD")
        {
            sb.AppendFormat("rejectionTransType = {0};", Convert.ToInt32(TransactionType.SamplingFormF));
        }

        // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
        var authorizationManager = Ioc.Resolve<IAuthorizationManager>();
        // Get the user permissions.
        var canCreate = authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit);
        var canView = authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.Cargo.Payables.CreditNote.View);

        if (canCreate && canView)
        {
            sb.AppendFormat("return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:InitiateRejForSpecificTrans({2});><img title=Reject style=border-style:none src={3} /></a>';",
                                                                              actionView, uri.Content("~/Content/Images/view.png"),
                                                                              rejectionParams, uri.Content("~/Content/Images/reject_invoice.png"));
        }
        else
        {
            sb.AppendFormat("return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a>';",
                                                                              actionView, uri.Content("~/Content/Images/view.png"));
        }

        sb.Append("}</script>");

        return MvcHtmlString.Create(sb.ToString());
    }

    public static MvcHtmlString GenerateCgoPayablesGridViewRejectScript(UrlHelper uri, string jqGridId, string actionView, string transactionType, string invoiceId, string initiateRejectionUrl, string duplicateRejectionUrl, int billingCode, int billingYear, int billingMonth, int billingPeriod, int smi)
    {
        var jqGridEditMethodName = string.Format("{0}_EditViewDeleteRecord(cellValue, options, rowObject)", jqGridId);

        var sb = new StringBuilder();
        var rejectionParams = string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"'+cellValue+'\"", jqGridId, transactionType, invoiceId, initiateRejectionUrl, duplicateRejectionUrl, billingCode, billingYear, billingMonth, billingPeriod, smi);
        sb.Append("<script type='text/javascript'>");
        sb.Append("function " + jqGridEditMethodName + "{");

        // For non-sampling RM, if Rejection stage is 3, show only view icon.
        if (transactionType == "RM" )
        {
            // Index 15: RejectionStage.
            sb.AppendFormat("if(rowObject.RejectionStage == 3)");
            sb.AppendFormat("return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a>';",
                            actionView,
                            uri.Content("~/Content/Images/view.png"));
            // For rejection stage 1
            sb.AppendFormat("if(rowObject.RejectionStage == 1){{");
            sb.AppendFormat("rejectionTransType = {0};", Convert.ToInt32(TransactionType.CargoRejectionMemoStage2));
            // For rejection stage 2
            sb.Append("}else{");
            sb.AppendFormat("rejectionTransType = {0};", Convert.ToInt32(TransactionType.CargoRejectionMemoStage3));
            sb.Append("}");
        }
        
        // For BM, if reason code is 6A or 6B, show only view icon.
        else if (transactionType == "BM")
        {
            sb.AppendFormat("rejectionTransType = {0};", Convert.ToInt32(TransactionType.RejectionMemo1)); // Transaction type : RM1
            // Index 10: Reason code.
            sb.AppendFormat("if((rowObject.ReasonCode == '6A') || (rowObject.ReasonCode == '6B'))");
            sb.AppendFormat("return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a>';",
                            actionView,
                            uri.Content("~/Content/Images/view.png"));
        }
        // For CM
        else if (transactionType == "CM")
        {
            sb.AppendFormat("rejectionTransType = {0};", Convert.ToInt32(TransactionType.RejectionMemo1)); // Transaction type : RM1
        }
        else if (transactionType == "FD")
        {
            sb.AppendFormat("rejectionTransType = {0};", Convert.ToInt32(TransactionType.SamplingFormF));
        }

        // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
        var authorizationManager = Ioc.Resolve<IAuthorizationManager>();
        var canCreate = authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit);
        var canView = authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.Cargo.Payables.Invoice.View);

        if (canCreate && canView)
        {
            sb.AppendFormat("return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:InitiateRejForSpecificTrans({2});><img title=Reject style=border-style:none src={3} /></a>';",
                                                                              actionView, uri.Content("~/Content/Images/view.png"),
                                                                              rejectionParams, uri.Content("~/Content/Images/reject_invoice.png"));
        }
        else
        {
            sb.AppendFormat("return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a>';",
                                                                              actionView, uri.Content("~/Content/Images/view.png"));
        }

        sb.Append("}</script>");

        return MvcHtmlString.Create(sb.ToString());
    }

    private static string GetEditDeleteValidateScript(UrlHelper uri, string actionEdit, string deleteMethod, string validateMethod)
    {
      return
        string.Format(
          "return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=Edit alt=Edit style=border-style:none src={1} /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:validateRecord({2});><img title=Validate alt=View style=border-style:none src={3} /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:deleteRecord({4});><img title=Delete style=border-style:none src={5} /></a>';",
          actionEdit,
          uri.Content("~/Content/Images/edit.png"),
          validateMethod,
          uri.Content("~/Content/Images/validate.png"),
          deleteMethod,
          uri.Content("~/Content/Images/delete.png"));
    }

    private static string GetViewScript(UrlHelper uri, string actionView)
    {
      return string.Format("return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a>';",
                           actionView,
                           uri.Content("~/Content/Images/view.png"));
    }
      
    private static string GetViewPdfZipScript(UrlHelper uri, string actionView, string actionDownloadPdf, string downloadZipMethod)
    {
      return
        string.Format(
          "return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a> &nbsp;<a style=cursor:hand target=_parent href={2}/'+cellValue+'><img title=Pdf alt=Pdf style=border-style:none src={3} /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:downloadZip({4});><img title=Zip alt=Zip style=border-style:none src={5} /></a>';",
          actionView,
          uri.Content("~/Content/Images/view.png"),
          actionDownloadPdf,
          uri.Content("~/Content/Images/pdf.png"),
          downloadZipMethod,
          uri.Content("~/Content/Images/zip.png"));
    }

   

    private static string GetInvoiceViewZipScript(UrlHelper uri, string actionView, string downloadZipMethod)
    {
      return
        string.Format(
          "return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:downloadZip({2});><img title=Zip alt=Zip style=border-style:none src={3} /></a>';",
          actionView,
          uri.Content("~/Content/Images/view.png"),
          downloadZipMethod,
          uri.Content("~/Content/Images/zip.png"));
    }

    public static MvcHtmlString GenerateFormCPayablesSearchScript(UrlHelper uri, string jqGridId, string actionView, string actionDownloadZip)
    {
      var jqGridEditMethodName = string.Format("{0}_ViewDownloadZipRecord(cellValue, options, rowObject)", jqGridId);

      //Resolve IUserManager
      var userManager = Ioc.Resolve<IUserManager>();

      // Get the list of user permissions.
      var permissionList = userManager.GetUserPermissions(SessionUtil.UserId);

      //Form C invoice permission
      var hasViewFormCPermission = permissionList.Contains(Business.Security.Permissions.Pax.Payables.SampleFormC.View);
      var hasDownloadFormCPermission = permissionList.Contains(Business.Security.Permissions.Pax.Payables.SampleFormC.Download);

      // Indices: 8-ProvisionalBillingYear, 9-ProvisionalBillingMonth,10-ProvisionalBillingMemberId,11-FromMemberId,12-ListingCurrencyId,13-InvoiceStatusId
      var sb = new StringBuilder();
      var downloadZipMethodParams = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", actionDownloadZip, "#" + jqGridId);
      sb.Append("<script type='text/javascript'>");
      sb.Append("function " + jqGridEditMethodName + " {");
      sb.Append("if(rowObject.ListingCurrencyId == '')");
      sb.Append("var parameters = rowObject.ProvisionalBillingYear+'/'+rowObject.ProvisionalBillingMonth+'/'+rowObject.ProvisionalBillingMemberId+'/'+rowObject.FromMemberId+'/'+rowObject.InvoiceStatusId;");
      sb.Append("else ");
      sb.Append("var parameters = rowObject.ProvisionalBillingYear+'/'+rowObject.ProvisionalBillingMonth+'/'+rowObject.ProvisionalBillingMemberId+'/'+rowObject.FromMemberId+'/'+rowObject.InvoiceStatusId+'/'+rowObject.ListingCurrencyId;");

      sb.Append("if(rowObject.NilFormCIndicator=='Y'){"); // If Nil Form C indicator is Yes, do not show any of the action icons.
      sb.Append("return '';");
      sb.Append("}else{");
        sb.Append(GetFormCViewZipScript(uri, actionView, downloadZipMethodParams, hasViewFormCPermission,
                                        hasDownloadFormCPermission));
      sb.Append("}} </script>");

      return MvcHtmlString.Create(sb.ToString());
    }


    private static string GetFormCViewZipScript(UrlHelper uri, string actionView, string downloadZipParams, bool hasViewPermission, bool hasDownloadPermission)
    {
        if(hasViewPermission && hasDownloadPermission)
        {
            return GetViewZipScript(uri, actionView, downloadZipParams);
        }
        if(hasViewPermission)
        {
            return GetFormCViewScript(uri, actionView);
        }
        if(hasDownloadPermission)
        {
            return GetInvoiceZipScript(uri, downloadZipParams);
        }
         return "return '';";
    }

    private static string GetViewZipScript(UrlHelper uri, string actionView, string downloadZipParams)
    {
        return
            string.Format(
                "return '<a style=cursor:hand target=_parent href=" + actionView +
                "/'+parameters+'><img title=View alt=View style=border-style:none src=" +
                uri.Content("~/Content/Images/view.png") +
                " /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:downloadZip({0});><img title=Zip style=border-style:none src=" +
                uri.Content("~/Content/Images/zip.png") + " /></a>';", downloadZipParams);
    }

    private static string GetViewDeleteScript(UrlHelper uri, string actionView, string deleteMethod)
    {
      return
        string.Format(
          "return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:deleteRecord({2});><img title=Delete style=border-style:none src={3} /></a>';",
          actionView,
          uri.Content("~/Content/Images/view.png"),
          deleteMethod,
          uri.Content("~/Content/Images/delete.png"));
    }

    /// <summary>
    /// This function is used to get script for View and delete.
    /// SCP419601: PAX permissions issue
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="actionView"></param>
    /// <param name="deleteMethod"></param>
    /// <param name="hasViewPermission"></param>
    /// <param name="hasDeletePermission"></param>
    /// <returns></returns>
    private static string GetFormCViewDeleteScript(UrlHelper uri, string actionView, string deleteMethod, bool hasViewPermission, bool hasDeletePermission)
    {
        if(hasViewPermission && hasDeletePermission)
        {
            return GetFormCViewDeleteScript(uri, actionView, deleteMethod);
        }

        if(hasViewPermission)
        {
            return GetFormCViewScript(uri, actionView);
        }

        if(hasDeletePermission)
        {
            return GetDeleteScript(uri, deleteMethod);
        }

         return "return '';";
    }

    /// <summary>
    /// This function is used to generate script for view and delete for form c
    /// SCP419601: PAX permissions issue
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="actionView"></param>
    /// <param name="deleteMethod"></param>
    /// <returns></returns>
    private static string GetFormCViewDeleteScript(UrlHelper uri, string actionView, string deleteMethod)
    {
      return
        string.Format(
          "return '<a style=cursor:hand target=_parent href=" + actionView + "/'+parameters+'><img title=View alt=View style=border-style:none src={1} /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:deleteRecord({2});><img title=Delete style=border-style:none src={3} /></a>';",
          actionView,
          uri.Content("~/Content/Images/view.png"),
          deleteMethod,
          uri.Content("~/Content/Images/delete.png"));
    }

    /// <summary>
    /// This function is used to generate script for edit and delete for form c
    /// SCP419601: PAX permissions issue
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="actionEdit"></param>
    /// <param name="deleteMethod"></param>
    /// <param name="hasEditDeletePermission"></param>
    /// <returns></returns>
    private static string GetFormCEditDeleteScript(UrlHelper uri, string actionEdit, string deleteMethod, bool hasEditDeletePermission)
      {
          if (hasEditDeletePermission)
          {
              return GetFormCEditDeleteScript(uri, actionEdit, deleteMethod);
          }

           return "return '';";
      }

      /// <summary>
    /// This function is used to return Edit and Delete action for Form C.
    /// </summary>
    /// <param name="uri">Url helper path.</param>
    /// <param name="actionEdit"> Action Edit Url.</param>
    /// <param name="deleteMethod">Action Delete Url.</param>
    /// <returns>retun script which contaion edit and delete action for Form C.</returns>
    //SCP239795: Issue with Form C Edit
    private static string GetFormCEditDeleteScript(UrlHelper uri, string actionEdit, string deleteMethod)
    {
        return
            string.Format(
                "return '<a style=cursor:hand target=_parent href=" + actionEdit +
                "/'+parameters+'><img title=Edit alt=Edit style=border-style:none src={1} /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:deleteRecord({2});><img title=Delete style=border-style:none src={3} /></a>';",
                actionEdit,
                uri.Content("~/Content/Images/edit.png"),
                deleteMethod,
                uri.Content("~/Content/Images/delete.png"));
    }

      /// <summary>
      /// This function is used to generate script for view for form c
      /// SCP419601: PAX permissions issue
      /// </summary>
      /// <param name="uri"></param>
      /// <param name="actionView"></param>
      /// <param name="hasViewPermission"></param>
      /// <returns></returns>
      private static string GetFormCViewScript(UrlHelper uri, string actionView, bool hasViewPermission)
      {
          if(hasViewPermission)
          {
             return GetFormCViewScript(uri, actionView);
          }

           return "return '';";
      }

    private static string GetFormCViewScript(UrlHelper uri, string actionView)
    {
      return
        string.Format(
          "return '<a style=cursor:hand target=_parent href=" + actionView + "/'+parameters+'><img title=View alt=View style=border-style:none src={0} /></a>';",
          uri.Content("~/Content/Images/view.png"));
    }

    private static string GetDeleteScript(UrlHelper uri, string deleteMethod)
    {
      return
        string.Format(
          "return '<a style=cursor:hand target=_parent href=javascript:deleteRecord({0});><img title=Delete style=border-style:none src={1} /></a>';",
          deleteMethod,
          uri.Content("~/Content/Images/delete.png"));
    }

    public static MvcHtmlString GeneratePaxPayableGridViewScript(UrlHelper uri, string jqGridId, string actionView, string actionDownloadPdf, string actionDownloadZip)
    {
      var jqGridEditMethodName = string.Format("{0}_ViewRejectRecord(cellValue, options, rowObject)", jqGridId);
      var downloadZipMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", actionDownloadZip, "#" + jqGridId);
      var sb = new StringBuilder();

      sb.Append("<script type='text/javascript'>");
      sb.Append("function " + jqGridEditMethodName + "{");
      //sb.Append(" if(rowObject[1] == 1 && (rowObject[2]!=2 && rowObject[2]!=4)) {alert(rowObject[1] + ' ' + rowObject[2]);}");
      sb.AppendFormat(" if(rowObject[1] == 1 && (rowObject[2]=={0} || rowObject[2]=={1})) {{ ", (int)DigitalSignatureStatus.Completed, (int)DigitalSignatureStatus.NotRequired); // check for if PDF has been generated.
      sb.AppendFormat(
        "return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a> &nbsp;<a style=cursor:hand target=_parent href={2}/'+cellValue+'><img title=Pdf alt=Pdf style=border-style:none src={3} /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:downloadZip({4});><img title=Zip alt=Zip style=border-style:none src={5} /></a>';",
        actionView,
        uri.Content("~/Content/Images/view.png"),
        actionDownloadPdf,
        uri.Content("~/Content/Images/pdf.png"),
        downloadZipMethod,
        uri.Content("~/Content/Images/zip.png"));
      sb.Append("} else {"); // if PDF is not generated then do not include PDF download in actions.
      sb.AppendFormat(
        "return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:downloadZip({2});><img title=Zip alt=Zip style=border-style:none src={3} /></a>';",
        actionView,
        uri.Content("~/Content/Images/view.png"),
        downloadZipMethod,
        uri.Content("~/Content/Images/zip.png"));
      sb.Append("}"); // end of check for PDF is generated.
      sb.Append("}</script>");
      return MvcHtmlString.Create(sb.ToString());
    }

    /// <summary>
    /// This function is used to generate pax payable grid view script
    /// SCP419601: PAX permissions issue
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="jqGridId"></param>
    /// <param name="actionView"></param>
    /// <param name="actionDownloadPdf"></param>
    /// <param name="actionDownloadZip"></param>
    /// <returns></returns>
    public static MvcHtmlString GeneratePaxPayableGridScript(UrlHelper uri, string jqGridId, string actionView, string actionDownloadPdf, string actionDownloadZip)
    {
        var jqGridEditMethodName = string.Format("{0}_ViewRejectRecord(cellValue, options, rowObject)", jqGridId);
        var downloadZipMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", actionDownloadZip, "#" + jqGridId);

        //Resolve IUserManager
        var userManager = Ioc.Resolve<IUserManager>();

        // Get the list of user permissions.
        var permissionList = userManager.GetUserPermissions(SessionUtil.UserId);

        //Pax Payable Non Sampling invoice permission
        var hasInvView = permissionList.Contains(Business.Security.Permissions.Pax.Payables.NonSampleInvoice.View);
        var hasInvDownload =
            permissionList.Contains(Business.Security.Permissions.Pax.Payables.NonSampleInvoice.Download);

        //Pax Payable Credit Note invoice permission
        var hasCNView = permissionList.Contains(Business.Security.Permissions.Pax.Payables.NonSampleCreditNote.View);
        var hasCNDownload =
            permissionList.Contains(Business.Security.Permissions.Pax.Payables.NonSampleCreditNote.Download);

        //Pax Payable Form DE invoice permission
        var hasFormDEView = permissionList.Contains(Business.Security.Permissions.Pax.Payables.SampleFormDE.View);
        var hasFormDEDownload = permissionList.Contains(Business.Security.Permissions.Pax.Payables.SampleFormDE.Download);

        //Pax Payable Form F invoice permission
        var hasFormFView = permissionList.Contains(Business.Security.Permissions.Pax.Payables.SampleFormF.View);
        var hasFormFDownload = permissionList.Contains(Business.Security.Permissions.Pax.Payables.SampleFormF.Download);

        //Pax Payable FORM XF invoice permission
        var hasFormXFView = permissionList.Contains(Business.Security.Permissions.Pax.Payables.SampleFormXF.View);
        var hasFormXFDownload = permissionList.Contains(Business.Security.Permissions.Pax.Payables.SampleFormXF.Download);

        var sb = new StringBuilder();
        sb.Append("<script type='text/javascript'>");
        sb.Append("function " + jqGridEditMethodName + "{");

        //If billing code is '0'(Non Sampling).
        sb.AppendFormat("if(rowObject.BillingCodeId=={0}){{", (int)BillingCode.NonSampling);

        //Invoice Type is invoice then execute below script
        sb.AppendFormat("if(rowObject.InvoiceTypeId=={0}){{", (int)InvoiceType.Invoice);
        GeneratePaxPayableGridScript(sb, uri, actionView, actionDownloadPdf, downloadZipMethod, hasInvView,
                                     hasInvDownload);
        sb.AppendFormat("}}");

        //Invoice Type is Credit Note then execute below script
        sb.AppendFormat("if(rowObject.InvoiceTypeId=={0}){{", (int)InvoiceType.CreditNote);
        GeneratePaxPayableGridScript(sb, uri, actionView, actionDownloadPdf, downloadZipMethod, hasCNView, hasCNDownload);
        sb.AppendFormat("}}");

        sb.AppendFormat("}}"); //End If

        //If billing code is '3', '5'(Form AB, Form DE).
        sb.AppendFormat("if(rowObject.BillingCodeId=={0} || rowObject.BillingCodeId=={1}){{", (int)BillingCode.SamplingFormAB,
                        (int) BillingCode.SamplingFormDE);
        GeneratePaxPayableGridScript(sb, uri, actionView, actionDownloadPdf, downloadZipMethod, hasFormDEView,
                                     hasFormDEDownload);
        sb.AppendFormat("}}");

        //If billing code is '6'(Form F).
        sb.AppendFormat("if(rowObject.BillingCodeId=={0}){{", (int)BillingCode.SamplingFormF);
        GeneratePaxPayableGridScript(sb, uri, actionView, actionDownloadPdf, downloadZipMethod, hasFormFView,
                                     hasFormFDownload);
        sb.AppendFormat("}}");

        //If billing code is '7'(Form XF).
        sb.AppendFormat("if(rowObject.BillingCodeId=={0}){{", (int)BillingCode.SamplingFormXF);
        GeneratePaxPayableGridScript(sb, uri, actionView, actionDownloadPdf, downloadZipMethod, hasFormXFView,
                                     hasFormXFDownload);
        sb.AppendFormat("}}");

        sb.Append("}</script>");
        return MvcHtmlString.Create(sb.ToString());
    }

      /// <summary>
    /// This function is used to generate pax payable grid script.
    /// SCP419601: PAX permissions issue
    /// </summary>
    /// <param name="sb"></param>
    /// <param name="uri"></param>
    /// <param name="actionView"></param>
    /// <param name="actionDownloadPdf"></param>
    /// <param name="downloadZipMethod"></param>
    /// <param name="hasViewPermission"></param>
    /// <param name="hasDownloadPermission"></param>
    private static void GeneratePaxPayableGridScript(StringBuilder sb, UrlHelper uri, string actionView, string actionDownloadPdf, string downloadZipMethod, bool hasViewPermission, bool hasDownloadPermission)
    {
        //sb.Append(" if(rowObject[1] == 1 && (rowObject[2]!=2 && rowObject[2]!=4)) {alert(rowObject[1] + ' ' + rowObject[2]);}");
        sb.AppendFormat(" if(rowObject.IsLegalPdfGenerated == 1 && (rowObject.DigitalSignatureStatusId=={0} || rowObject.DigitalSignatureStatusId=={1})) {{ ",
                        (int) DigitalSignatureStatus.Completed, (int) DigitalSignatureStatus.NotRequired);
            // check for if PDF has been generated.
        sb.AppendFormat(GetViewPdfZipScript(uri, actionView, actionDownloadPdf, downloadZipMethod, hasViewPermission,
                                            hasDownloadPermission));
        sb.Append("} else {"); // if PDF is not generated then do not include PDF download in actions.
        sb.AppendFormat(GetInvoiceViewZipScript(uri, actionView, downloadZipMethod, hasViewPermission,
                                                hasDownloadPermission));
        sb.Append("}"); // end of check for PDF is generated.
    }

    public static MvcHtmlString GenerateUnlinkedSupportingDocumentGridEditDeleteScript(UrlHelper uri, string jqGridId, string actionEdit, string actionDelete)
    {
      var jqGridEditMethodName = string.Format("{0}_EditViewDeleteRecord(cellValue, options, rowObject)", jqGridId); // DO NOT CHANGE THE SCRIPT METHOD NAME
      var editMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", "", "");
      var deleteMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", actionDelete, "#" + jqGridId);
      var sb = new StringBuilder();
      sb.Append("<script type='text/javascript'>");
      sb.AppendFormat("function {0} {{", jqGridEditMethodName);
      //sb.AppendFormat("return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=Edit alt=Edit style=border-style:none src={1} /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:deleteRecord({2});><img title=Delete style=border-style:none src={3} /></a>';", actionEdit, uri.Content("~/Content/Images/edit.png"), deleteMethod, uri.Content("~/Content/Images/delete.png"));
      sb.AppendFormat(
        "return '<a style=cursor:hand target=_parent href=javascript:EditUnlinkedSupportingDocument({0});><img title=Edit alt=Edit style=border-style:none src={1} /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:deleteRecord({2});><img title=Delete style=border-style:none src={3} /></a>';",
        editMethod,
        uri.Content("~/Content/Images/edit.png"),
        deleteMethod.Replace("/SearchResultGridData", ""),
        uri.Content("~/Content/Images/delete.png"));
      sb.Append("} </script>");
      return MvcHtmlString.Create(sb.ToString());
    }

    /// <summary>
    /// Create script for Add/Remove attachment button for manage supporting document
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="jqGridId"></param>
    /// <param name="actionUpload"></param>
    /// <returns></returns>
    public static MvcHtmlString GenerateSupportingDocumentGridScript(UrlHelper uri, string jqGridId, string actionUpload)
    {
      var jqGridEditMethodName = string.Format("{0}_UploadAttachment(cellValue, options, rowObject)", jqGridId);

      var sb = new StringBuilder();

      sb.Append("<script type='text/javascript'>");
      sb.AppendFormat("function {0} {{", jqGridEditMethodName);
      sb.AppendFormat("var imageTag = '<img title=Add/Remove Attachment alt=AddRemoveAttachment style=border-style:none src=\"{0}\" />';", uri.Content("~/Content/Images/add_remove_attachments.png"));
      sb.AppendFormat("if(rowObject.IsFormCAttachmentAllowed > 0) imageTag = '<img title=View Attachment alt=ViewAttachment style=border-style:none src=\"{0}\" />';", uri.Content("~/Content/Images/view.png"));
      sb.AppendFormat(
        "return '<a style=cursor:hand target=_parent href=javascript:loadAttachment(\"{0}\",\"'+cellValue+'\",\"'+rowObject.InvoiceId+'\",\"'+rowObject.RecordType+'\",'+rowObject.IsFormCAttachmentAllowed+');>'+imageTag+'</a>';",
        actionUpload);
      sb.Append("} </script>");

      return MvcHtmlString.Create(sb.ToString());
    }

    public static MvcHtmlString GeneratePayableSupportingDocumentGridScript(UrlHelper uri, string jqGridId, string actionUpload)
    {
      var jqGridEditMethodName = string.Format("{0}_UploadAttachment(cellValue, options, rowObject)", jqGridId);

      var sb = new StringBuilder();

      sb.Append("<script type='text/javascript'>");
      sb.AppendFormat("function {0} {{", jqGridEditMethodName);
      sb.AppendFormat(
        "if(rowObject.AttachmentNumber != '0') return '<a style=cursor:hand target=_parent href=javascript:loadAttachment(\"{0}\",\"'+cellValue+'\",\"'+rowObject.InvoiceId+'\",\"'+rowObject.RecordType+'\");><img title= View Attachment alt=AddRemoveAttachment style=border-style:none src={1} /></a>'; else return ' ';",
        actionUpload,
        uri.Content("~/Content/Images/view.png"));
      sb.Append("} </script>");

      return MvcHtmlString.Create(sb.ToString());
    }

    public static MvcHtmlString GenerateCargoPayableSupportingDocumentGridScript(UrlHelper uri, string jqGridId, string actionUpload)
    {
        var jqGridEditMethodName = string.Format("{0}_UploadAttachment(cellValue, options, rowObject)", jqGridId);

        var sb = new StringBuilder();

        sb.Append("<script type='text/javascript'>");
        sb.AppendFormat("function {0} {{", jqGridEditMethodName);
        sb.AppendFormat(
          "if(rowObject.AttachmentNumber != '0') return '<a style=cursor:hand target=_parent href=javascript:loadAttachment(\"{0}\",\"'+cellValue+'\",\"'+rowObject.InvoiceId+'\",\"'+rowObject.RecordType+'\");><img title= View Attachment alt=AddRemoveAttachment style=border-style:none src={1} /></a>'; else return ' ';",
          actionUpload,
          uri.Content("~/Content/Images/view.png"));
        sb.Append("} </script>");

        return MvcHtmlString.Create(sb.ToString());
    }
    public static MvcHtmlString GenerateSupportingDocDeleteScript(UrlHelper uri, string jqGridId, string actionDelete)
    {
      var jqGridEditMethodName = string.Format("{0}_DeleteAttachment(cellValue, options, rowObject)", jqGridId);
      var deleteMethod = string.Format("\"{0}\",\"'+cellValue+'\"", actionDelete);

      var sb = new StringBuilder();

      sb.Append("<script type='text/javascript'>");
      sb.AppendFormat("function {0}{{", jqGridEditMethodName);
      sb.AppendFormat("return '<a style=cursor:hand target=_parent href=javascript:deleteAttachment({0});><img title=Delete style=border-style:none src={1} /></a>';",
                      deleteMethod,
                      uri.Content("~/Content/Images/delete.png"));
      sb.Append("} </script>");

      return MvcHtmlString.Create(sb.ToString());
    }

    /// <summary>
    /// Create script for Add/Remove attachment button for manage supporting document
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="jqGridId"></param>
    /// <param name="actionUpload"></param>
    /// <returns></returns>
    public static MvcHtmlString GenerateMiscSupportingDocumentGridScript(UrlHelper uri, string jqGridId)
    {
      var jqGridUploadFunctionName = string.Format("{0}_UploadMiscAttachment(cellValue, options, rowObject)", jqGridId);

      var sb = new StringBuilder();

      sb.Append("<script type='text/javascript'>");
      sb.AppendFormat("function {0} {{", jqGridUploadFunctionName);
      sb.AppendFormat(
        " if(rowObject.AttachmentNumber != '0') return '<a style=cursor:hand target=_parent href=javascript:loadAttachment(\"'+cellValue+'\");><img title=View Attachment alt=ViewAttachment style=border-style:none src={0} /></a>'; else return ' ';",
        uri.Content("~/Content/Images/view.png"));
      sb.Append("} </script>");

      return MvcHtmlString.Create(sb.ToString());
    }
    /// <summary>
    /// Create script for Add/Remove attachment button for manage supporting document
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="jqGridId"></param>
    /// <returns></returns>
    public static MvcHtmlString GenerateMiscReceivablesSupportingDocGridScript(UrlHelper uri, string jqGridId)
    {
      var jqGridUploadFunctionName = string.Format("{0}_UploadMiscAttachment(cellValue, options, rowObject)", jqGridId);

      var sb = new StringBuilder();

      sb.Append("<script type='text/javascript'>");
      sb.AppendFormat("function {0} {{", jqGridUploadFunctionName);
      sb.AppendFormat(
        "return '<a style=cursor:hand target=_parent href=javascript:loadAttachment(\"'+cellValue+'\");><img title=Add/Remove Attachment alt=AddRemoveAttachment style=border-style:none src={0} /></a>';",
        uri.Content("~/Content/Images/add_remove_attachments.png"));
      sb.Append("} </script>");

      return MvcHtmlString.Create(sb.ToString());
    }

    /// <summary>
    /// This function is used to create download script for offline report.
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="jqGridId"></param>
    /// <param name="actionDownloadZip"></param>
    /// <returns></returns>
		public static MvcHtmlString GenerateGridDownloadScript(UrlHelper uri,string jqGridId,string actionDownloadZip)
		{
			var JqGridDownloadZipName = string.Format("{0}_downloadZip(cellValue, options, rowObject)", jqGridId);
            var downloadZipMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"'+rowObject.Id+'\",\"{1}\"", actionDownloadZip, "#" + jqGridId);
			var sb = new StringBuilder();

			sb.Append("<script type='text/javascript'>");
			sb.Append("function " + JqGridDownloadZipName + "{");
            sb.AppendFormat(" if(rowObject.DownloadLinkId != '')");
			sb.AppendFormat(
				"return '<a style=cursor:hand target=_parent href=javascript:downloadZip({0});><img title=Zip alt=Zip style=border-style:none src={1} /></a>';",
				downloadZipMethod,
				uri.Content("~/Content/Images/zip.png"));
			sb.AppendFormat(" else return '' ");
			sb.Append("}</script>");
			return MvcHtmlString.Create(sb.ToString());
		}

    /// <summary>
    /// This function is used to create script for rejection memo linked with correspondence.
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="jqGridId"></param>
    /// <param name="actionauditTrail"></param>
    /// <returns></returns>
    //CMP612: Changes to PAX CGO Correspondence Audit Trail Download
    public static MvcHtmlString GenerateGridPaxAuditTrailForLinkedCorrRM(UrlHelper uri, string jqGridId, string billingHistoryUrl)
    {
      var JqGridShowRejection = string.Format("{0}_showRejection(cellValue, options, rowObject)", jqGridId);
      var sb = new StringBuilder();
      billingHistoryUrl = billingHistoryUrl.Replace("/0/1", "");

      sb.Append("<script type='text/javascript'>");
      sb.Append("function " + JqGridShowRejection + "{");
      sb.AppendFormat(" if(rowObject.Id != '')");
      sb.AppendFormat(
        "return '<a style=cursor:hand target=_parent href=" + billingHistoryUrl + "/' +rowObject.Id+'/RM><img title=\"Show Audit Trail\" alt=\"Show Detail\" style=border-style:none src=" +
                uri.Content("~/Content/Images/show_audit_trail.png") + " /></a>';");
      sb.AppendFormat(" else return '' ");
      sb.Append("}</script>");
      return MvcHtmlString.Create(sb.ToString());
    }

    /// <summary>
    /// This function is used to create script for rejection memo linked with correspondence.
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="jqGridId"></param>
    /// <param name="actionauditTrail"></param>
    /// <returns></returns>
    //CMP612: Changes to PAX CGO Correspondence Audit Trail Download
    public static MvcHtmlString GenerateGridCargoAuditTrailForLinkedCorrRM(UrlHelper uri, string jqGridId, string billingHistoryUrl)
    {
      var JqGridShowRejection = string.Format("{0}_showRejection(cellValue, options, rowObject)", jqGridId);
      // var showRejectionMethod = string.Format("\"{0}\",\"'+rowObject[0]+'\",\"{1}\"", actionauditTrail, "#" + jqGridId);
      var sb = new StringBuilder();
      billingHistoryUrl = billingHistoryUrl.Replace("/0/1", "");

      sb.Append("<script type='text/javascript'>");
      sb.Append("function " + JqGridShowRejection + "{");
      sb.AppendFormat(" if(rowObject.Id != '')");
      sb.AppendFormat(
        "return '<a style=cursor:hand target=_parent href=" + billingHistoryUrl + "/' +rowObject.Id+'/RM><img title=\"Show Audit Trail\" alt=\"Show Detail\" style=border-style:none src=" +
                uri.Content("~/Content/Images/show_audit_trail.png") + " /></a>';");
      sb.AppendFormat(" else return '' ");
      sb.Append("}</script>");
      return MvcHtmlString.Create(sb.ToString());
    }

      #region "SCP401669 - Misc Permissions issue"

    private static string GetViewDeleteScript(UrlHelper uri, string actionView, string deleteMethod, bool hasViewPermission, bool hasDeletePermission)
    {
        if (hasViewPermission && hasDeletePermission)
        {
            return GetViewDeleteScript(uri, actionView, deleteMethod);
        }
        if (hasViewPermission)
        {
            return GetViewScript(uri, actionView);
        }
        if (hasDeletePermission)
        {
            return GetDeleteScript(uri, deleteMethod);
        }
        return "return '';";
    }

    private static string GetViewPdfZipScript(UrlHelper uri, string actionView, string actionDownloadPdf, string downloadZipMethod, bool hasViewPermission, bool hasInvDownloadPermission)
    {
        if (hasViewPermission && hasInvDownloadPermission)
        {
            return GetViewPdfZipScript(uri, actionView, actionDownloadPdf, downloadZipMethod);
        }
        if (hasViewPermission)
        {
            return GetViewScript(uri, actionView);
        }
        if (hasInvDownloadPermission)
        {
            return GetDownloadPdfZipScript(uri, actionDownloadPdf, downloadZipMethod);
        }
        return "return '';";
    }

    private static string GetInvoiceViewZipScript(UrlHelper uri, string actionView, string downloadZipMethod, bool hasViewPermission, bool hasInvDownloadPermission)
    {
        if (hasViewPermission && hasInvDownloadPermission)
        {
            return GetInvoiceViewZipScript(uri, actionView,downloadZipMethod);
        }
        if (hasViewPermission)
        {
            return GetViewScript(uri, actionView);
        }
        if (hasInvDownloadPermission)
        {
            return GetInvoiceZipScript(uri,downloadZipMethod);
        }
        return "return '';";
    }

    private static string GetEditDeleteValidateScript(UrlHelper uri, string actionEdit, string deleteMethod, string validateMethod, bool hasEditPermission, bool hasValidatePermission)
    {
        if (hasValidatePermission && hasEditPermission)
        {
            return GetEditDeleteValidateScript(uri, actionEdit, deleteMethod, validateMethod);
        }
        if (hasValidatePermission)
        {
            return GetValidateScript(uri, validateMethod);
        }
        if (hasEditPermission)
        {
            return GetEditDeleteScript(uri, actionEdit, deleteMethod);
        }
        return "return '';";
    }

    private static string GetValidateScript(UrlHelper uri, string validateMethod)
    {
        return
        string.Format(
          "return '<a style=cursor:hand target=_parent href=javascript:validateRecord({0});><img title=Validate alt=View style=border-style:none src={1} /></a>';",
          validateMethod,
          uri.Content("~/Content/Images/validate.png"));

    }
      
    private static string GetDownloadPdfZipScript(UrlHelper uri, string actionDownloadPdf, string downloadZipMethod)
    {
        return
          string.Format(
            "return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=Pdf alt=Pdf style=border-style:none src={1} /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:downloadZip({2});><img title=Zip alt=Zip style=border-style:none src={3} /></a>';",  
            actionDownloadPdf,
            uri.Content("~/Content/Images/pdf.png"),
            downloadZipMethod,
            uri.Content("~/Content/Images/zip.png"));
    }

    private static string GetInvoiceZipScript(UrlHelper uri, string downloadZipMethod)
    {
        return
          string.Format(
            "return '<a style=cursor:hand target=_parent href=javascript:downloadZip({0});><img title=Zip alt=Zip style=border-style:none src={1} /></a>';",            
            downloadZipMethod,
            uri.Content("~/Content/Images/zip.png"));
    }


      #endregion

    #region CMP #665: User Related Enhancements-FRS-v1.2.doc[Sec 2.9: IS-WEB MISC Payables Invoice Search Screen]

    /// <summary>
    /// This function is used to generate script for grid, [there are 4 icon, will show as Invoice View, Zip, Listing, Attachment]
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="actionView">The action view.</param>
    /// <param name="downloadZipMethod">The download zip method.</param>
    /// <param name="downloadListing">The download listing.</param>
    /// <returns>return String.</returns>
    private static string GetViewZipListingAttachmentScript(UrlHelper uri, string actionView, string downloadZipMethod, string downloadListing)
    {
        //return script include 4 icon as Invoice View, Zip, Listing, Attachment.
        return
            string.Format(
                "return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a>&nbsp;<a style=cursor:hand target=_parent href={2}/'+cellValue+'><img title=Listing alt=Listing style=border-style:none src={3} /></a>&nbsp;<a style=cursor:hand target=_parent href=javascript:loadAttachment(\"'+cellValue+'\");><img title=Attachment alt=attachment style=border-style:none src={4} /></a>&nbsp;<a style=cursor:hand target=_parent href=javascript:downloadZip({5});><img title=Zip alt=Zip style=border-style:none src={6} /></a> ';",
                actionView,
                uri.Content("~/Content/Images/view.png"),
                downloadListing,
                uri.Content("~/Content/Images/listing.png"),
                uri.Content("~/Content/Images/attachment.png"),
                downloadZipMethod,
                uri.Content("~/Content/Images/zip.png"));
    }
    
    /// <summary>
    /// This function is used to generate script for grid, [there are 4 icon will show as Invoice View, Zip, Listing, Attachment] based on permission.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="actionView">The action view.</param>
    /// <param name="downloadZipMethod">The download zip method.</param>
    /// <param name="downloadListing">The download listing.</param>
    /// <param name="hasInvDownloadPermission">if set to <c>true</c> [has inv download permission].</param>
    /// <returns>return String</returns>
    private static string GetViewZipListingAttachmentPermissionScript(UrlHelper uri, string actionView, string downloadZipMethod, string downloadListing, bool hasInvDownloadPermission)
    {
        //If user has not download permission, then will display only 2 icon as Invoice View and Download Zip otherwise will display 4 icon as Invoice View, Zip, Listing, Attachment.
        if (hasInvDownloadPermission)
        {
            return GetViewZipListingAttachmentScript(uri, actionView, downloadZipMethod, downloadListing);
        }

        return GetInvoiceViewZipScript(uri, actionView, downloadZipMethod);
    }

    /// <summary>
    /// This function is used to generate script for grid, [there are 5 icon will show as Invoice View, PDF, Zip, Listing, Attachment].
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="actionView">The action view.</param>
    /// <param name="actionDownloadPdf">The action download PDF.</param>
    /// <param name="downloadListing">The download listing.</param>
    /// <param name="downloadZipMethod">The download zip method.</param>
    /// <returns>return String.</returns>
    private static string GetViewPdfZipListingAttachScript(UrlHelper uri, string actionView, string actionDownloadPdf, string downloadListing, string downloadZipMethod)
    {
        //return script include 5 icon as Invoice View, PDF, Zip, Listing, Attachment.
        return
          string.Format(
            "return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a>&nbsp;<a style=cursor:hand target=_parent href={2}/'+cellValue+'><img title=Pdf alt=Pdf style=border-style:none src={3} /></a>&nbsp;<a style=cursor:hand target=_parent href={4}/'+cellValue+'><img title=Listing alt=Listing style=border-style:none src={5} /></a>&nbsp;<a style=cursor:hand target=_parent href=javascript:loadAttachment(\"'+cellValue+'\");><img  title=Attachment alt=Attachment style=border-style:none src={6} /></a>&nbsp;<a style=cursor:hand target=_parent href=javascript:downloadZip({7});><img title=Zip alt=Zip style=border-style:none src={8} /></a>';",
            actionView,
            uri.Content("~/Content/Images/view.png"),
            actionDownloadPdf,
            uri.Content("~/Content/Images/pdf.png"),
             downloadListing,
            uri.Content("~/Content/Images/listing.png"),
            uri.Content("~/Content/Images/attachment.png"),
            downloadZipMethod,
            uri.Content("~/Content/Images/zip.png"));
    }

    /// <summary>
    /// This function is used to generate script for grid, [there are 5 icon will show as Invoice View, PDF, Zip, Listing, Attachment] based on permission.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="actionView">The action view.</param>
    /// <param name="actionDownloadPdf">The action download PDF.</param>
    /// <param name="downloadZipMethod">The download zip method.</param>
    /// <param name="downloadListing">The download listing.</param>
    /// <param name="hasInvDownloadPermission">if set to <c>true</c> [has inv download permission].</param>
    /// <returns>return String.</returns>
    private static string GetViewPdfZipListingAttachmentPermissionScript(UrlHelper uri, string actionView, string actionDownloadPdf, string downloadZipMethod, string downloadListing, bool hasInvDownloadPermission)
    {
        //If user has not download permission, then will display only 2 icon as Invoice View, PDF and Download Zip otherwise will display 5 icon as Invoice View, 
        //PDF, Zip, Listing, Attachment.
        if (hasInvDownloadPermission)
        {
            return GetViewPdfZipListingAttachScript(uri, actionView, actionDownloadPdf, downloadListing,
                                                    downloadZipMethod);
        }

        return GetViewPdfZipScript(uri, actionView, actionDownloadPdf, downloadZipMethod);
    }

    /// <summary>
    /// This function is used to generate script for grid, [there are 6 icon will show as Invoice View, Reject Invoice, PDF, Zip, Listing, Attachment].
    /// If actionDownloadPdf is NULL, then will not include PDF icon in this script otherwise will inclue.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="actionView">The action view.</param>
    /// <param name="actionDownloadPdf">The action download PDF.</param>
    /// <param name="downloadZipMethod">The download zip method.</param>
    /// <param name="downloadListing">The download listing.</param>
    /// <param name="rejectMethod">The reject method.</param>
    /// <returns>System.String.</returns>
    private static string GetViewRejPdfZipListingAttachScript(UrlHelper uri, string actionView, string actionDownloadPdf, string downloadZipMethod, string downloadListing, string rejectMethod)
    {
        //if actionDownloadPdf is null or empty then will display only 5 icon as Invoice View, Reject Invoice, Zip, Listing, Attachment 
        //otherwise will display 6 icon as Invoice View, Reject Invoice, PDF, Zip, Listing, Attachment
        if (!String.IsNullOrEmpty(actionDownloadPdf))
        {
            return
                string.Format(
                    "return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a>&nbsp;<a style=cursor:hand target=_parent href=javascript:callRejectLineItems({2});><img title=Reject alt=Reject style=border-style:none src={3} /></a>&nbsp;<a style=cursor:hand target=_parent href={4}/'+cellValue+'><img title=Pdf alt=Pdf style=border-style:none src={5} /></a>&nbsp;<a style=cursor:hand target=_parent href={6}/'+cellValue+'><img title=Listing alt=Listing style=border-style:none src={7} /></a>&nbsp;<a style=cursor:hand target=_parent href=javascript:loadAttachment(\"'+cellValue+'\");><img  title=Attachment alt=Attachment style=border-style:none src={8} /></a>&nbsp;<a style=cursor:hand target=_parent href=javascript:downloadZip({9});><img title=Zip alt=Zip style=border-style:none src={10} /></a>';",
                    actionView,
                    uri.Content("~/Content/Images/view.png"),
                    rejectMethod,
                    uri.Content("~/Content/Images/reject_invoice.png"),
                    actionDownloadPdf,
                    uri.Content("~/Content/Images/pdf.png"),
                    downloadListing,
                    uri.Content("~/Content/Images/listing.png"),
                    uri.Content("~/Content/Images/attachment.png"),
                    downloadZipMethod,
                    uri.Content("~/Content/Images/zip.png"));
        }

        return
            string.Format(
                "return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a>&nbsp;<a style=cursor:hand target=_parent href=javascript:callRejectLineItems({2});><img title=Reject alt=Reject style=border-style:none src={3} /></a>&nbsp;<a style=cursor:hand target=_parent href={4}/'+cellValue+'><img title=Listing alt=Listing style=border-style:none src={5} /></a>&nbsp;<a style=cursor:hand target=_parent href=javascript:loadAttachment(\"'+cellValue+'\");><img  title=Attachment alt=Attachment style=border-style:none src={6} /></a>&nbsp;<a style=cursor:hand target=_parent href=javascript:downloadZip({7});><img title=Zip alt=Zip style=border-style:none src={8} /></a> ';",
                actionView,
                uri.Content("~/Content/Images/view.png"),
                rejectMethod,
                uri.Content("~/Content/Images/reject_invoice.png"),
                downloadListing,
                uri.Content("~/Content/Images/listing.png"),
                uri.Content("~/Content/Images/attachment.png"),
                downloadZipMethod,
                uri.Content("~/Content/Images/zip.png"));

    }

    /// <summary>
    /// This function is used to generate script for grid, [there are6 icon will show as Invoice View, Reject Invoice, PDF, Zip, Listing, Attachment].
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="actionView">The action view.</param>
    /// <param name="actionDownloadPdf">The action download PDF.</param>
    /// <param name="downloadZipMethod">The download zip method.</param>
    /// <param name="rejectMethod">The reject method.</param>
    /// <returns>Return Script.</returns>
    private static string GetViewRejPdfZipScript(UrlHelper uri, string actionView, string actionDownloadPdf, string downloadZipMethod, string rejectMethod)
    {
        //if actionDownloadPdf is null or empty then will display only 3 icon as Invoice View, Reject Invoice, Zip
        //otherwise will display 4 icon as Invoice View, Reject Invoice, Reject Invoice, PDF, Zip
        if (!String.IsNullOrEmpty(actionDownloadPdf))
        {
            return
                string.Format(
                    "return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a>&nbsp;<a style=cursor:hand target=_parent href=javascript:callRejectLineItems({2});><img title=Reject alt=Reject style=border-style:none src={3} /></a>&nbsp;<a style=cursor:hand target=_parent href={4}/'+cellValue+'><img title=Pdf alt=Pdf style=border-style:none src={5} /></a>&nbsp;<a style=cursor:hand target=_parent href=javascript:downloadZip({6});><img title=Zip alt=Zip style=border-style:none src={7} /></a>';",
                    actionView,
                    uri.Content("~/Content/Images/view.png"),
                    rejectMethod,
                    uri.Content("~/Content/Images/reject_invoice.png"),
                    actionDownloadPdf,
                    uri.Content("~/Content/Images/pdf.png"),
                    downloadZipMethod,
                    uri.Content("~/Content/Images/zip.png"));
        }

        return
            string.Format(
                "return '<a style=cursor:hand target=_parent href={0}/'+cellValue+'><img title=View alt=View style=border-style:none src={1} /></a>&nbsp;<a style=cursor:hand target=_parent href=javascript:callRejectLineItems({2});><img title=Reject alt=Reject style=border-style:none src={3} /></a>&nbsp;<a style=cursor:hand target=_parent href=javascript:downloadZip({4});><img title=Zip alt=Zip style=border-style:none src={5} /></a> ';",
                actionView,
                uri.Content("~/Content/Images/view.png"),
                rejectMethod,
                uri.Content("~/Content/Images/reject_invoice.png"),
                downloadZipMethod,
                uri.Content("~/Content/Images/zip.png"));

    }

    /// <summary>
    /// This function is used to generate script for grid,  [there are 6 icon, will display as Invoice View, Reject Invoice, PDF, Zip, Listing, Attachment] based on permission.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="actionView">The action view.</param>
    /// <param name="actionDownloadPdf">The action download PDF.</param>
    /// <param name="downloadZipMethod">The download zip method.</param>
    /// <param name="downloadListing">The download listing.</param>
    /// <param name="hasInvDownloadPermission">if set to <c>true</c> [has inv download permission].</param>
    /// <param name="rejectMethod">The reject method.</param>
    /// <returns>System.String.</returns>
    private static string GetViewRejPdfZipListingAttachPermissionScript(UrlHelper uri, string actionView, string actionDownloadPdf, string downloadZipMethod, string downloadListing, bool hasInvDownloadPermission, string rejectMethod)
    {
        //If user has not download permission, then will display only 4 icon as Invoice View, Reject Invoice, PDF and Download Zip 
        //otherwise will display 6 icon as Invoice View, Reject Invoice, PDF, Zip, Listing, Attachment.
        if (hasInvDownloadPermission)
        {
            return GetViewRejPdfZipListingAttachScript(uri, actionView, actionDownloadPdf, downloadZipMethod, downloadListing, rejectMethod);
        }

        return GetViewRejPdfZipScript(uri, actionView, actionDownloadPdf, downloadZipMethod, rejectMethod);
    }

    /// <summary>
    /// This function is used to Generate Misc Payables Grid Script.
    /// </summary>
    /// <param name="sb">The string builder Object.</param>
    /// <param name="correspondenceInvoiceTypeId">The correspondence invoice type identifier.</param>
    /// <param name="rejectionInvoiceTypeId">The rejection invoice type identifier.</param>
    /// <param name="bilateralSettlementMethodId">The bilateral settlement method identifier.</param>
    /// <param name="ichSettlementMethodId">The ich settlement method identifier.</param>
    /// <param name="adjustmentDueToProtestSmi">The adjustment due to protest smi.</param>
    /// <param name="uri">The URI.</param>
    /// <param name="actionView">The action view.</param>
    /// <param name="actionDownloadPdf">The action download PDF.</param>
    /// <param name="downloadZipMethod">The download zip method.</param>
    /// <param name="downloadListing">The download listing.</param>
    /// <param name="canInvDownload">if set to <c>true</c> [can inv download].</param>
    /// <param name="rejectMethod">The reject method.</param>
    private static void GenerateMiscPayablesGridScript(StringBuilder sb, int correspondenceInvoiceTypeId, int rejectionInvoiceTypeId, int bilateralSettlementMethodId, int ichSettlementMethodId, int adjustmentDueToProtestSmi, UrlHelper uri, string actionView, string actionDownloadPdf, string downloadZipMethod, string downloadListing, bool canInvDownload, string rejectMethod)
    {
        //CMP #665: User Related Enhancements-FRS-v1.2.doc[Sec 2.9: IS-WEB MISC Payables Invoice Search Screen]
        //Changed 3 function as GetViewPdfZipListingAttachmentPermissionScript, GetViewZipListingAttachmentPermissionScript,
        //GetViewRejPdfZipListingAttachPermissionScriptcalling from this function.
        //sb.Append(" if(rowObject[5] == 1 && (rowObject[6]!=2 && rowObject[6]!=4)) {alert(rowObject[5] + ' ' + rowObject[6]);}");
        // If invoice is correspondence invoice OR rejection invoice with rejection stage 1 and SMI =  I or B OR rejection invoice of stage 2, do not show reject icon.
        sb.AppendFormat("if(rowObject[1] == {0} || (rowObject[1] == {1} && rowObject[2] == 1 && (rowObject[3] == {2} || rowObject[3] == {3} || rowObject[3] == {4})) || (rowObject[1] == {1} && rowObject[2] == 2)){{",
                        correspondenceInvoiceTypeId,
                        rejectionInvoiceTypeId,
                        bilateralSettlementMethodId,
                        ichSettlementMethodId,
                        adjustmentDueToProtestSmi);
        sb.AppendFormat("if(rowObject[5] == 1 &&(rowObject[6]=={0} || rowObject[6]=={1})) {{ ", (int)DigitalSignatureStatus.Completed, (int)DigitalSignatureStatus.NotRequired); // check for if PDF has been generated.

        sb.Append(GetViewPdfZipListingAttachmentPermissionScript(uri, actionView, actionDownloadPdf, downloadZipMethod, downloadListing, canInvDownload));
        sb.Append("} else {"); // if PDF is not generated then do not include PDF download in actions.

        sb.Append(GetViewZipListingAttachmentPermissionScript(uri, actionView, downloadZipMethod, downloadListing, canInvDownload));
        sb.Append("}"); // end of check for PDF is generated.
        sb.Append("}else{");
        sb.AppendFormat("if(rowObject[5] == 1 && (rowObject[6]=={0} || rowObject[6]=={1})) {{ ", (int)DigitalSignatureStatus.Completed, (int)DigitalSignatureStatus.NotRequired); // check for if PDF has been generated.

        sb.Append(GetViewRejPdfZipListingAttachPermissionScript(uri, actionView, actionDownloadPdf, downloadZipMethod,
                                                                downloadListing, canInvDownload, rejectMethod));
        sb.Append("} else {"); // if PDF is not generated then do not include PDF download in actions.
        sb.Append(GetViewRejPdfZipListingAttachPermissionScript(uri, actionView, null, downloadZipMethod,
                                                                downloadListing, canInvDownload, rejectMethod));
        sb.Append("}"); // end of check for PDF is generated.
        sb.Append("}");
    }

    #endregion
 public static MvcHtmlString GeneratePaxGridEditViewValidateDeleteScript(UrlHelper uri, string jqGridId,string actionEdit,string actionValidate,string actionDelete,
                                                                            string actionView,string actionDownloadPdf,string actionDownloadZip, int rejectionOnValidationFlag)
    {
        var jqGridEditMethodName = string.Format("{0}_EditViewDeleteRecord(cellValue, options, rowObject)", jqGridId);
        var deleteMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", actionDelete, "#" + jqGridId);
        var validateMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", actionValidate, "#" + jqGridId);
        var downloadZipMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", actionDownloadZip, "#" + jqGridId);

        //SCP419601: PAX permissions issue
        var userManager = Ioc.Resolve<IUserManager>();

        // Get the list of user permissions.
        var permissionList = userManager.GetUserPermissions(SessionUtil.UserId); 

        //Non Sampling invoice permission
        var hasInvCreateOrEdit = permissionList.Contains(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit);
        var hasInvValidate = permissionList.Contains(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.Validate);
        var hasInvView = permissionList.Contains(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.View);
        var hasInvDownload = permissionList.Contains(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.Download);

        //Non Sampling invoice permission
        var hasCNCreateOrEdit = permissionList.Contains(Business.Security.Permissions.Pax.Receivables.NonSampleCreditNote.CreateOrEdit);
        var hasCNValidate = permissionList.Contains(Business.Security.Permissions.Pax.Receivables.NonSampleCreditNote.Validate);
        var hasCNView = permissionList.Contains(Business.Security.Permissions.Pax.Receivables.NonSampleCreditNote.View);
        var hasCNDownload = permissionList.Contains(Business.Security.Permissions.Pax.Receivables.NonSampleCreditNote.Download);

        //Sampling Form D/E invoice permission
        var hasFormDECreateOrEdit = permissionList.Contains(Business.Security.Permissions.Pax.Receivables.SampleFormDE.CreateOrEdit);
        var hasFormDEValidate = permissionList.Contains(Business.Security.Permissions.Pax.Receivables.SampleFormDE.Validate);
        var hasFormDEView = permissionList.Contains(Business.Security.Permissions.Pax.Receivables.SampleFormDE.View);
        var hasFormDEDownload = permissionList.Contains(Business.Security.Permissions.Pax.Receivables.SampleFormDE.Download);

        //Sampling Form F invoice permission
        var hasFormFCreateOrEdit = permissionList.Contains(Business.Security.Permissions.Pax.Receivables.SampleFormF.CreateOrEdit);
        var hasFormFValidate = permissionList.Contains(Business.Security.Permissions.Pax.Receivables.SampleFormF.Validate);
        var hasFormFView = permissionList.Contains(Business.Security.Permissions.Pax.Receivables.SampleFormF.View);
        var hasFormFDownload = permissionList.Contains(Business.Security.Permissions.Pax.Receivables.SampleFormF.Download);

        //Sampling Form XF invoice permission
        var hasFormXFCreateOrEdit = permissionList.Contains(Business.Security.Permissions.Pax.Receivables.SampleFormXF.CreateOrEdit);
        var hasFormXFValidate = permissionList.Contains(Business.Security.Permissions.Pax.Receivables.SampleFormXF.Validate);
        var hasFormXFView = permissionList.Contains(Business.Security.Permissions.Pax.Receivables.SampleFormXF.View);
        var hasFormXFDownload = permissionList.Contains(Business.Security.Permissions.Pax.Receivables.SampleFormXF.Download);
        
        var sb = new StringBuilder();

        sb.Append("<script type='text/javascript'>");
        sb.AppendFormat("function {0} {{", jqGridEditMethodName);
        
        //If billing code is '0'(Non Sampling).
        sb.AppendFormat("if(rowObject.BillingCodeId =={0}){{", (int)BillingCode.NonSampling);

        //Invoice Type is invoice then execute below script
        sb.AppendFormat("if(rowObject.InvoiceTypeId =={0}){{", (int)InvoiceType.Invoice);
        GeneratePaxGridScript(sb, uri, actionView, deleteMethod, actionDownloadPdf, downloadZipMethod, actionEdit,
                              validateMethod, hasInvCreateOrEdit, hasInvValidate, hasInvView, hasInvDownload);
        sb.AppendFormat("}}");

        //Invoice Type is Credit Note then execute below script
        sb.AppendFormat("if(rowObject.InvoiceTypeId =={0}){{", (int)InvoiceType.CreditNote);
        GeneratePaxGridScript(sb, uri, actionView, deleteMethod, actionDownloadPdf, downloadZipMethod, actionEdit,
                              validateMethod, hasCNCreateOrEdit, hasCNValidate, hasCNView, hasCNDownload);
        sb.AppendFormat("}}");

        sb.AppendFormat("}}");//End if

        //If billing code is '3', '5'(Form AB, Form DE).
        sb.AppendFormat("if(rowObject.BillingCodeId =={0} || rowObject.BillingCodeId =={1}){{", (int)BillingCode.SamplingFormAB, (int)BillingCode.SamplingFormDE);
         GeneratePaxGridScript(sb, uri, actionView, deleteMethod, actionDownloadPdf, downloadZipMethod, actionEdit,
                              validateMethod, hasFormDECreateOrEdit, hasFormDEValidate, hasFormDEView, hasFormDEDownload);
        sb.AppendFormat("}}");

        //If billing code is '6'(Form F).
        sb.AppendFormat("if(rowObject.BillingCodeId =={0}){{", (int)BillingCode.SamplingFormF);
        GeneratePaxGridScript(sb, uri, actionView, deleteMethod, actionDownloadPdf, downloadZipMethod, actionEdit,
                              validateMethod, hasFormFCreateOrEdit, hasFormFValidate, hasFormFView, hasFormFDownload);
        sb.AppendFormat("}}");

        //If billing code is '7'(Form XF).
        sb.AppendFormat("if(rowObject.BillingCodeId =={0}){{", (int)BillingCode.SamplingFormXF);
        GeneratePaxGridScript(sb, uri, actionView, deleteMethod, actionDownloadPdf, downloadZipMethod, actionEdit,
                              validateMethod, hasFormXFCreateOrEdit, hasFormXFValidate, hasFormXFView, hasFormXFDownload);
        sb.AppendFormat("}}");

        sb.Append("} </script>");

        return MvcHtmlString.Create(sb.ToString());
    }

      /// <summary>
      /// This function is used to generate script for manage screen grid.
      /// SCP419601: PAX permissions issue
      /// </summary>
      /// <param name="sb"></param>
      /// <param name="uri"></param>
      /// <param name="actionView"></param>
      /// <param name="deleteMethod"></param>
      /// <param name="actionDownloadPdf"></param>
      /// <param name="downloadZipMethod"></param>
      /// <param name="actionEdit"></param>
      /// <param name="validateMethod"></param>
      /// <param name="hasEditPermission"></param>
      /// <param name="hasValidatePermission"></param>
      /// <param name="hasViewPermission"></param>
      /// <param name="hasDownloadPermission"></param>
      private static void GeneratePaxGridScript(StringBuilder sb, UrlHelper uri, string actionView, string deleteMethod, string actionDownloadPdf, string downloadZipMethod, string actionEdit, string validateMethod,
                                                bool hasEditPermission, bool hasValidatePermission, bool hasViewPermission, bool hasDownloadPermission)
      {
          sb.AppendFormat("if(rowObject.SubmissionMethodId =={0} || rowObject.SubmissionMethodId =={1}){{", (int)SubmissionMethod.IsXml,
                          (int) SubmissionMethod.IsIdec);
          // conditions for Invoice Status in IS-XML or IS-IDEC
          sb.AppendFormat("if(((rowObject.InvoiceStatusId =={0}) ||(rowObject.InvoiceStatusId =={1})) && (rowObject.InputFileStatusId =={2}) ){{",
                          (int) InvoiceStatusType.ErrorCorrectable, (int) InvoiceStatusType.ErrorNonCorrectable,
                          (int) FileStatusType.ValidationCompleted);

          // sb.Append(GetDeleteScript(uri, deleteMethod));
          sb.Append(GetViewDeleteScript(uri, actionView, deleteMethod, hasViewPermission, hasEditPermission));

          sb.AppendFormat("}} else if((rowObject.InvoiceStatusId =={0}) ) {{",
                          (int) InvoiceStatusType.OnHold);
          // script for view....
          sb.Append(GetViewScript(uri, actionView, hasViewPermission));
          sb.AppendFormat(
              "}} else if( (rowObject.InvoiceStatusId =={0}) || (rowObject.InvoiceStatusId =={1}) || (rowObject.InvoiceStatusId =={2}) || (rowObject.InvoiceStatusId =={3}) || (rowObject.InvoiceStatusId =={4}) ){{",
              (int) InvoiceStatusType.ProcessingComplete,
              (int) InvoiceStatusType.Presented,
              (int) InvoiceStatusType.Claimed,
              (int) InvoiceStatusType.ReadyForBilling,
              (int) InvoiceStatusType.FutureSubmitted);
          // script for View, PDF, Zip
          sb.AppendFormat("if(rowObject.IsLegalPdfGenerated == 1 && (rowObject.DigitalSignatureStatusId=={0} || rowObject.DigitalSignatureStatusId=={1})) {{ ",
                          (int) DigitalSignatureStatus.Completed, (int) DigitalSignatureStatus.NotRequired);
              // check for if PDF has been generated.
          sb.Append(GetViewPdfZipScript(uri, actionView, actionDownloadPdf, downloadZipMethod, hasViewPermission, hasDownloadPermission));
          sb.Append("} else {"); // if PDF is not generated then do not include PDF download in actions.
          sb.Append(GetInvoiceViewZipScript(uri, actionView, downloadZipMethod, hasViewPermission, hasDownloadPermission));
          sb.Append("}"); // end of check for PDF is generated.
          //SCP ID : 130459 - BVD / AB testing & ID: 130459 - BVD / AB testing
          //Logic: Download PDF button will be display in case of Autobilling also. 
          sb.AppendFormat("}} else if(rowObject.SubmissionMethodId =={0}){{", (int)SubmissionMethod.AutoBilling);
              // Empty action script for default condition when invoice status criteria is not match.
          sb.AppendFormat("if(rowObject.IsLegalPdfGenerated == 1 && (rowObject.DigitalSignatureStatusId =={0} || rowObject.DigitalSignatureStatusId =={1})) {{ ",
                          (int) DigitalSignatureStatus.Completed, (int) DigitalSignatureStatus.NotRequired);
              // check for if PDF has been generated.
          sb.Append(GetViewPdfZipScript(uri, actionView, actionDownloadPdf, downloadZipMethod, hasViewPermission, hasDownloadPermission));
          sb.Append("} else {"); // if PDF is not generated then do not include PDF download in actions.
          sb.Append(GetInvoiceViewZipScript(uri, actionView, downloadZipMethod, hasViewPermission, hasDownloadPermission));
          sb.Append("}"); // end of check for PDF is generated.
          sb.Append(GetViewScript(uri, actionView, hasViewPermission));
          sb.Append("} else {"); // Empty action script for default condition when invoice status criteria is not match.
          sb.Append(GetViewScript(uri, actionView, hasViewPermission));
          sb.Append("}"); // end of conditions Invoice Status in IS-XML or IS-IDEC.

          // else Submission method is IS-Web.
          //**************************************************************************************************************************
          sb.AppendFormat("}} else if(rowObject.SubmissionMethodId =={0}) {{ ", (int)SubmissionMethod.IsWeb);
          sb.AppendFormat("if((rowObject.InvoiceStatusId =={0}) || (rowObject.InvoiceStatusId =={1}) ) {{", (int)InvoiceStatusType.Open,
                          (int) InvoiceStatusType.ValidationError);
          // script for Edit, Delete, Validate
          sb.Append(GetEditDeleteValidateScript(uri, actionEdit, deleteMethod, validateMethod, hasEditPermission, hasValidatePermission));
          sb.AppendFormat("}} else if(rowObject.InvoiceStatusId =={0}){{", (int)InvoiceStatusType.ReadyForSubmission);
          // script for Edit, Delete, Submit
          sb.Append(GetEditDeleteScript(uri, actionEdit, deleteMethod, hasEditPermission));
          sb.AppendFormat(
                            "}} else if((rowObject.InvoiceStatusId =={0}) || (rowObject.InvoiceStatusId =={1}) || (rowObject.InvoiceStatusId =={2}) || (rowObject.InvoiceStatusId =={3}) || (rowObject.InvoiceStatusId =={4}) ){{",
              (int) InvoiceStatusType.Presented,
              (int) InvoiceStatusType.ProcessingComplete,
              (int) InvoiceStatusType.Claimed,
              (int) InvoiceStatusType.ReadyForBilling,
              (int) InvoiceStatusType.FutureSubmitted);
          // script for View, PDF, Zip
          sb.AppendFormat("if(rowObject.IsLegalPdfGenerated == 1 && (rowObject.DigitalSignatureStatusId =={0} || rowObject.DigitalSignatureStatusId =={1})) {{ ",
                          (int) DigitalSignatureStatus.Completed, (int) DigitalSignatureStatus.NotRequired);
              // check for if PDF has been generated.
          sb.Append(GetViewPdfZipScript(uri, actionView, actionDownloadPdf, downloadZipMethod, hasViewPermission, hasDownloadPermission));
          sb.Append(" } else { "); // if PDF is not generated then do not include PDF download in actions.
          sb.Append(GetInvoiceViewZipScript(uri, actionView, downloadZipMethod, hasViewPermission, hasDownloadPermission));
          sb.Append(" }"); // end of check for PDF is generated.
          sb.Append("} else {"); // Empty action script for default condition when invoice status criteria is not match.
          sb.Append(GetEmptyActionScript());
          sb.Append("}"); // end of conditions Invoice Status in IS-Web.
          //SCP ID : 130459 - BVD / AB testing & ID: 130459 - BVD / AB testing
          //Logic: Download PDF button will be display in case of Autobilling also. 
          sb.AppendFormat("}} else if (rowObject.SubmissionMethodId == {0}) {{", (int)SubmissionMethod.AutoBilling);
              // end of Else If Submission method is IS-Web.
          sb.AppendFormat("if(rowObject.IsLegalPdfGenerated == 1 && (rowObject.DigitalSignatureStatusId =={0} || rowObject.DigitalSignatureStatusId =={1})) {{ ",
                          (int) DigitalSignatureStatus.Completed, (int) DigitalSignatureStatus.NotRequired);
              // check for if PDF has been generated.
          sb.Append(GetViewPdfZipScript(uri, actionView, actionDownloadPdf, downloadZipMethod, hasViewPermission, hasDownloadPermission));
          sb.Append(" } else { "); // if PDF is not generated then do not include PDF download in actions.
          sb.Append(GetInvoiceViewZipScript(uri, actionView, downloadZipMethod, hasViewPermission, hasDownloadPermission));
          sb.Append("}"); // end of check for PDF is generated.
          sb.Append(GetViewScript(uri, actionView, hasViewPermission));
          sb.Append("} else {"); // end of default condition for Submission method block.
          sb.Append(GetEmptyActionScript());
          sb.Append("}");
      }

      /// <summary>
      /// This function is used to get view script with view permission
      /// SCP419601: PAX permissions issue
      /// </summary>
      /// <param name="uri"></param>
      /// <param name="actionView"></param>
      /// <param name="hasViewPermission"></param>
      /// <returns></returns>
      private static string GetViewScript(UrlHelper uri, string actionView, bool hasViewPermission)
      {

          if (hasViewPermission)
              return GetViewScript(uri, actionView);
          return "return '';";
      }

      /// <summary>
      /// This function is used to get edit and delete script with CreateOrEdit permission
      /// SCP419601: PAX permissions issue
      /// </summary>
      /// <param name="uri"></param>
      /// <param name="actionEdit"></param>
      /// <param name="deleteMethod"></param>
      /// <param name="hasEditPermission"></param>
      /// <returns></returns>
      private static string GetEditDeleteScript(UrlHelper uri, string actionEdit, string deleteMethod, bool hasEditPermission)
      {
          if (hasEditPermission)
              return GetEditDeleteScript(uri, actionEdit, deleteMethod);

          return "return '';";
      }

      /// <summary>
      /// This function is used to get edit and validate script with CreateOrEdit and validate permission
      /// SCP419601: PAX permissions issue
      /// </summary>
      /// <param name="uri"></param>
      /// <param name="actionEdit"></param>
      /// <param name="validateMethod"></param>
      /// <param name="hasEditPermission"></param>
      /// <returns></returns>
      private static string GetEditValidateScript(UrlHelper uri, string actionEdit, string validateMethod, bool hasEditPermission, bool hasValidatePermission)
       {
           //Edit and validate permission then return edit and validate script.
           if (hasEditPermission && hasValidatePermission)
           {
               return "return '<a style=cursor:hand target=_parent href=" + actionEdit +
                      "/'+parameters+'><img title=Edit alt=Edit style=border-style:none src=" +
                      uri.Content("~/Content/Images/edit.png") +
                      " /></a> &nbsp;<a style=cursor:hand target=_parent href=javascript:validateRecord(" +
                      validateMethod + ");><img title=Validate alt=View style=border-style:none src=" +
                      uri.Content("~/Content/Images/validate.png") + " /></a>';";
           }

           //Edit permission then return edit script.
           if (hasEditPermission)
           {
               return "return '<a style=cursor:hand target=_parent href=" + actionEdit +
                      "/'+parameters+'><img title=Edit alt=Edit style=border-style:none src=" +
                      uri.Content("~/Content/Images/edit.png") +
                      " /></a>';";
           }

           //Validate permission then return validate script.
           if (hasValidatePermission)
           {
               return "return '<a style=cursor:hand target=_parent href=javascript:validateRecord(" +
                      validateMethod + ");><img title=Validate alt=View style=border-style:none src=" +
                      uri.Content("~/Content/Images/validate.png") + " /></a>';";
           }

           return "return '';";
       }

      /// <summary>
      /// Generates the grid active/deactivate script.
      /// CMP #553: ACH Requirement for Multiple Currency Handling 
      /// </summary>
      /// <param name="uri">The URI.</param>
      /// <param name="jqGridId">The jqGrid id.</param>
      /// <param name="actionDelete">The action delete.</param>
      /// <param name="achCurrencyUrl"></param>
      /// <returns></returns>
      public static MvcHtmlString GenerateGridActiveDeactiveScript(UrlHelper uri, string jqGridId, string actionDelete,string achCurrencyUrl,  bool flag)
      {
          var jqGridEditMethodName = string.Format("{0}_DeleteRecord(cellValue, options, rowObject)", jqGridId); 
          var deleteMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", actionDelete, "#" + jqGridId);
          var activeMethod = string.Format("\"{0}\",\"{1}\",\"'+cellValue+'\",\"{2}\"", actionDelete, achCurrencyUrl, "#" + jqGridId);
          var sb = new StringBuilder();
          sb.Append("<script type='text/javascript'>");
          sb.AppendFormat("function {0} {{", jqGridEditMethodName);

          if (flag)
          {
              sb.AppendFormat("if(rowObject.IsActive=='True'){{");
              sb.AppendFormat(
                      "return '<a style=margin-left:28px;cursor:hand target=_parent href=javascript:dactivateRecord({0});><img title=Deactivate style=border-style:none src={1} /></a>';",
                      deleteMethod,
                      uri.Content("~/Content/Images/delete.png"));
              sb.Append("} else {");
              sb.AppendFormat(
                  "return '<a style=margin-left:28px;cursor:hand target=_parent href=javascript:activateAchCurrency({0});><img title=Activate style=border-style:none src={1} /></a>';",
                  activeMethod,
                  uri.Content("~/Content/Images/Validate.png"));

              sb.Append("}");
          }
          else
          {
              sb.AppendFormat("return '';");
          }
          sb.Append("}");

          sb.Append("</script>");
          return MvcHtmlString.Create(sb.ToString());
      }

      /// <summary>
      /// Generates the grid active/deactivate script only.
      /// CMP #692: Misc Payment Status Master
      /// </summary>
      /// <param name="uri">The URI.</param>
      /// <param name="jqGridId">The jqGrid id.</param>
      /// <param name="actionDelete">The action delete.</param>
      /// <returns></returns>
      public static MvcHtmlString GenerateGridActiveDeactiveScriptOnly(UrlHelper uri, string jqGridId, string actionDelete, bool flag)
      {
          var jqGridEditMethodName = string.Format("{0}_DeleteRecord(cellValue, options, rowObject)", jqGridId);
          var deleteMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", actionDelete, "#" + jqGridId);
          var sb = new StringBuilder();
          sb.Append("<script type='text/javascript'>");
          sb.AppendFormat("function {0} {{", jqGridEditMethodName);

          if (flag)
          {
              sb.AppendFormat("if(rowObject.IsActive=='True'){{");
              sb.AppendFormat(
                      "return '<a style=margin-left:28px;cursor:hand target=_parent href=javascript:dactivateRecord({0});><img title=Deactivate style=border-style:none src={1} /></a>';",
                      deleteMethod,
                      uri.Content("~/Content/Images/delete.png"));
              sb.Append("} else {");
              sb.AppendFormat(
                  "return '<a style=margin-left:28px;cursor:hand target=_parent href=javascript:activateRecord({0});><img title=Activate style=border-style:none src={1} /></a>';",
                  deleteMethod,
                  uri.Content("~/Content/Images/Validate.png"));

              sb.Append("}");
              
          }
          else
          {
              sb.AppendFormat("return '';");
          }
          sb.Append("}");

          sb.Append("</script>");
          return MvcHtmlString.Create(sb.ToString());
      }

      #region Cargo

      /// <summary>
      /// Method to generate Cargo Receivable grid action column icon script.
      /// SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
      /// </summary>
      /// <param name="uri"></param>
      /// <param name="jqGridId"></param>
      /// <param name="actionEdit"></param>
      /// <param name="actionValidate"></param>
      /// <param name="actionDelete"></param>
      /// <param name="actionView"></param>
      /// <param name="actionDownloadPdf"></param>
      /// <param name="actionDownloadZip"></param>
      /// <param name="rejectionOnValidationFlag"></param>
      /// <returns></returns>
      public static MvcHtmlString GenerateScriptForCargoRecManage(UrlHelper uri, string jqGridId, string actionEdit, string actionValidate,
                                                                  string actionDelete, string actionView, string actionDownloadPdf,
                                                                  string actionDownloadZip, int rejectionOnValidationFlag)
      {
          var jqGridEditMethodName = string.Format("{0}_EditViewDeleteRecord(cellValue, options, rowObject)", jqGridId);
          var deleteMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", actionDelete, "#" + jqGridId);
          var validateMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", actionValidate, "#" + jqGridId);
          var downloadZipMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", actionDownloadZip, "#" + jqGridId);

          var sb = new StringBuilder();

          sb.Append("<script type='text/javascript'>");
          sb.AppendFormat("function {0} {{", jqGridEditMethodName);

          //sb.AppendFormat("alert('rowObject[0] = '+ rowObject[1]);"); // is InvoiceId
          //sb.AppendFormat("alert('rowObject[1] = '+ rowObject[1]);"); // is InvoiceStatusId.
          //sb.AppendFormat("alert('rowObject[2] = '+ rowObject[2]);"); // is SubmissionMethodId.
          //sb.AppendFormat("alert('rowObject[4] = '+ rowObject[4]);");
          //sb.AppendFormat("alert('rowObject[5] = '+ rowObject[5]);"); // is DigitalSignatureStatus.
          //sb.AppendFormat("alert('rowObject[6] = '+ rowObject[6]);"); // is InputFileStatusId.
          //sb.AppendFormat("alert('rowObject[20] = '+ rowObject[20]);"); // is InvoiceTypeId.

          sb.AppendFormat("if(rowObject.InvoiceTypeId =={0}){{", (int)InvoiceType.Invoice);
          sb.Append(GetCargoInvoiceCreditNoteScript(uri, actionView, deleteMethod, actionDownloadPdf, actionEdit, validateMethod, downloadZipMethod, InvoiceType.Invoice, true));
          sb.Append("} else {");
          sb.Append(GetCargoInvoiceCreditNoteScript(uri, actionView, deleteMethod, actionDownloadPdf, actionEdit, validateMethod, downloadZipMethod, InvoiceType.CreditNote, true));
          sb.Append("}} </script>");

          return MvcHtmlString.Create(sb.ToString());
      }

      /// <summary>
      /// Method to generate Cargo Payables grid action column icon script.
      /// SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
      /// </summary>
      /// <param name="uri"></param>
      /// <param name="jqGridId"></param>
      /// <param name="actionView"></param>
      /// <param name="actionDownloadPdf"></param>
      /// <param name="actionDownloadZip"></param>
      /// <returns></returns>
      public static MvcHtmlString GenerateScriptForCargoPayableManage(UrlHelper uri, string jqGridId, string actionView,
                                                                      string actionDownloadPdf, string actionDownloadZip)
      {
          var jqGridEditMethodName = string.Format("{0}_ViewRejectRecord(cellValue, options, rowObject)", jqGridId);
          var downloadZipMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", actionDownloadZip, "#" + jqGridId);

          var sb = new StringBuilder();

          sb.Append("<script type='text/javascript'>");
          sb.Append(" function " + jqGridEditMethodName + "{");

          sb.Append("if(rowObject.InvoiceTypeId == " + (int)InvoiceType.Invoice + "){");
          sb.Append(GetCargoInvoiceCreditNoteScript(uri, actionView, string.Empty, actionDownloadPdf, string.Empty, string.Empty, downloadZipMethod, InvoiceType.Invoice, false));
          sb.Append("} else {");
          sb.Append(GetCargoInvoiceCreditNoteScript(uri, actionView, string.Empty, actionDownloadPdf, string.Empty, string.Empty, downloadZipMethod, InvoiceType.CreditNote, false));
          sb.Append("}} </script>");

          return MvcHtmlString.Create(sb.ToString());
      }

      /// <summary>
      /// Method to generate Cargo grid action column icon script.
      /// SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
      /// </summary>
      /// <param name="uri"></param>
      /// <param name="actionView"></param>
      /// <param name="deleteMethod"></param>
      /// <param name="actionDownloadPdf"></param>
      /// <param name="actionEdit"></param>
      /// <param name="validateMethod"></param>
      /// <param name="downloadZipMethod"></param>
      /// <param name="invoiceType"></param>
      /// <param name="isReceivables"></param>
      /// <returns></returns>
      private static string GetCargoInvoiceCreditNoteScript(UrlHelper uri, string actionView, string deleteMethod, string actionDownloadPdf, string actionEdit,
                                                            string validateMethod, string downloadZipMethod, InvoiceType invoiceType, bool isReceivables)
      {
          var userManager = Ioc.Resolve<IUserManager>();

          // Get the list of user permissions.
          var permissionList = userManager.GetUserPermissions(SessionUtil.UserId);

          var sb = new StringBuilder();

          var canView = false;
          var canDownload = false;

          if (isReceivables)
          {
              var canCreateOrEdit = false;
              var canValidate = false;
              if (invoiceType == InvoiceType.CreditNote)
              {
                  canView = permissionList.Contains(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.View);
                  canCreateOrEdit = permissionList.Contains(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.CreateOrEdit);
                  canDownload = permissionList.Contains(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.Download);
                  canValidate = permissionList.Contains(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.Validate);
              }
              else
              {
                  canView = permissionList.Contains(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.View);
                  canCreateOrEdit = permissionList.Contains(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit);
                  canDownload = permissionList.Contains(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.Download);
                  canValidate = permissionList.Contains(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.Validate);
              }

              // rowObject[0] is InvoiceId.
              // rowObject[1] is InvoiceStatusId.
              // rowObject[2] is SubmissionMethodId.
              // rowObject[4] is IsLegalPdfGenerated 
              // rowObject[5] is DigitalSignatureStatus.
              // rowObject[6] is InputFileStatusId.
              // rowObject[20] is InvoiceTypeId.

              // If Submission method is IS-Web.
              sb.AppendFormat("if(rowObject.SubmissionMethodId =={0}) {{ ", (int)SubmissionMethod.IsWeb);
              sb.AppendFormat("if((rowObject.InvoiceStatusId =={0}) || (rowObject.InvoiceStatusId =={1}) ) {{", (int)InvoiceStatusType.Open, (int)InvoiceStatusType.ValidationError);
              // script for Edit, Delete, Validate
              sb.Append(GetEditDeleteValidateScript(uri, actionEdit, deleteMethod, validateMethod, canCreateOrEdit, canValidate));
              sb.AppendFormat("}} else if(rowObject.InvoiceStatusId =={0}){{", (int)InvoiceStatusType.ReadyForSubmission);
              // script for Edit, Delete, Submit
              sb.Append(canCreateOrEdit ? GetEditDeleteScript(uri, actionEdit, deleteMethod) : GetEmptyActionScript());
              sb.AppendFormat("}} else if((rowObject.InvoiceStatusId =={0}) || (rowObject.InvoiceStatusId =={1}) || (rowObject.InvoiceStatusId =={2}) || (rowObject.InvoiceStatusId =={3}) || (rowObject.InvoiceStatusId =={4}) ){{",
                                                         (int)InvoiceStatusType.Presented,
                                                         (int)InvoiceStatusType.ProcessingComplete,
                                                         (int)InvoiceStatusType.Claimed,
                                                         (int)InvoiceStatusType.ReadyForBilling,
                                                         (int)InvoiceStatusType.FutureSubmitted);
              // script for View, PDF, Zip
              sb.AppendFormat("if(rowObject.IsLegalPdfGenerated == 1 && (rowObject.DigitalSignatureStatusId =={0} || rowObject.DigitalSignatureStatusId =={1})) {{ ", (int)DigitalSignatureStatus.Completed, (int)DigitalSignatureStatus.NotRequired); // check for if PDF has been generated.
              sb.Append(GetViewPdfZipScript(uri, actionView, actionDownloadPdf, downloadZipMethod, canView, canDownload));
              sb.Append(" } else { "); // if PDF is not generated then do not include PDF download in actions.
              sb.Append(GetInvoiceViewZipScript(uri, actionView, downloadZipMethod, canView, canDownload));
              sb.Append(" }"); // end of check for PDF is generated.
              sb.Append("} else {"); // Empty action script for default condition when invoice status criteria is not match.
              sb.Append(GetEmptyActionScript());
              sb.Append("}}"); // end of conditions Invoice Status in IS-Web.

              // else If Invoice Submission method is IS-IDEC or IS-XML
              sb.AppendFormat("else if(rowObject.SubmissionMethodId =={0} || rowObject.SubmissionMethodId =={1}){{", (int)SubmissionMethod.IsIdec, (int)SubmissionMethod.IsXml);

              sb.AppendFormat("if(((rowObject.InvoiceStatusId =={0}) || (rowObject.InvoiceStatusId=={1})) && (rowObject.InputFileStatusId =={2} || rowObject.InputFileStatusId =={3}) ){{",
                                                  (int)InvoiceStatusType.ErrorCorrectable,
                                                  (int)InvoiceStatusType.ErrorNonCorrectable,
                                                  (int)FileStatusType.ErrorCorrectable, (int)FileStatusType.ValidationCompleted);

              sb.Append(GetViewDeleteScript(uri, actionView, deleteMethod, canView, canCreateOrEdit));

              sb.AppendFormat("}} else if((rowObject.InvoiceStatusId=={0})) {{",
                                                         (int)InvoiceStatusType.OnHold);

              // script for view....
              sb.Append(canView ? GetViewScript(uri, actionView) : GetEmptyActionScript());

              sb.AppendFormat("}} else if( (rowObject.InvoiceStatusId =={0}) || (rowObject.InvoiceStatusId =={1}) || (rowObject.InvoiceStatusId =={2}) || (rowObject.InvoiceStatusId =={3}) || (rowObject.InvoiceStatusId =={4}) ){{",
                                                          (int)InvoiceStatusType.ProcessingComplete,
                                                          (int)InvoiceStatusType.Presented,
                                                          (int)InvoiceStatusType.Claimed,
                                                          (int)InvoiceStatusType.ReadyForBilling,
                                                          (int)InvoiceStatusType.FutureSubmitted);

              // script for View, PDF, Zip
              sb.AppendFormat("if(rowObject.IsLegalPdfGenerated == 1 && (rowObject.DigitalSignatureStatusId =={0} || rowObject.DigitalSignatureStatusId =={1})) {{ ",
                                                                      (int)DigitalSignatureStatus.Completed,
                                                                      (int)DigitalSignatureStatus.NotRequired); // check for if PDF has been generated.

              sb.Append(GetViewPdfZipScript(uri, actionView, actionDownloadPdf, downloadZipMethod, canView, canDownload));

              sb.Append("} else {"); // if PDF is not generated then do not include PDF download in actions.
              sb.Append(GetInvoiceViewZipScript(uri, actionView, downloadZipMethod, canView, canDownload));
              sb.Append("}}}"); // end of check for PDF is generated.
          }
          else
          {
              if (invoiceType == InvoiceType.Invoice)
              {
                  canView = permissionList.Contains(Business.Security.Permissions.Cargo.Payables.Invoice.View);
                  canDownload = permissionList.Contains(Business.Security.Permissions.Cargo.Payables.Invoice.Download);
              }
              else
              {
                  canView = permissionList.Contains(Business.Security.Permissions.Cargo.Payables.CreditNote.View);
                  canDownload = permissionList.Contains(Business.Security.Permissions.Cargo.Payables.CreditNote.Download);
              }

              sb.Append(" if(rowObject.InvoiceStatusId == 1 && (rowObject.DigitalSignatureStatusId == " + (int)DigitalSignatureStatus.Completed + " || rowObject.DigitalSignatureStatusId == " + (int)DigitalSignatureStatus.NotRequired + ")) {"); // check for if PDF has been generated.
              sb.Append(GetViewPdfZipScript(uri, actionView, actionDownloadPdf, downloadZipMethod, canView, canDownload));
              sb.Append("} else {"); // if PDF is not generated then do not include PDF download in actions.
              sb.Append(GetInvoiceViewZipScript(uri, actionView, downloadZipMethod, canView, canDownload));
              sb.Append("}"); // end of check for PDF is generated.
              sb.Append(canView ? GetViewScript(uri, actionView) : GetEmptyActionScript());
          }

          return sb.ToString();
      }

      #endregion

      #region Misc

      #region Misc Payables

      /// <summary>
      /// Method to generate Misc Payable Invoice search grid action column script.
      /// SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
      /// </summary>
      /// <param name="uri"></param>
      /// <param name="jqGridId"></param>
      /// <param name="actionView"></param>
      /// <param name="actionReject"></param>
      /// <param name="actionDownloadPdf"></param>
      /// <param name="actionDownloadListing"></param>
      /// <param name="actionDownloadZip"></param>
      /// <returns></returns>
      public static MvcHtmlString GenerateScriptForMiscPayableManage(UrlHelper uri, string jqGridId, string actionView, string actionReject,
                                                                     string actionDownloadPdf, string actionDownloadListing, string actionDownloadZip)
      {
          var jqGridEditMethodName = string.Format("{0}_ViewRejectRecord(cellValue, options, rowObject)", jqGridId);
          var rejectMethod = string.Format("\"{0}\",\"'+cellValue+'\"", actionReject);
          var downloadZipMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", actionDownloadZip, "#" + jqGridId);

          // rowObject[1] is InvoiceTypeId
          // rowObject[2] is RejectionStage
          // rowObject[3] is SettlementMethodId
          // rowObject[6] is DigitalSignatureStatus

          var stringBuilder = new StringBuilder();

          stringBuilder.Append("<script type='text/javascript'>");
          stringBuilder.Append("function " + jqGridEditMethodName + "{");

          stringBuilder.Append("if(rowObject.InvoiceTypeId == " + (int)InvoiceType.CreditNote + "){");
          stringBuilder.Append(GetMiscInvoiceCreditNoteScript(uri, actionView, rejectMethod, actionDownloadPdf, actionDownloadListing, downloadZipMethod, InvoiceType.CreditNote));

          stringBuilder.Append("} else {");
          stringBuilder.Append(GetMiscInvoiceCreditNoteScript(uri, actionView, rejectMethod, actionDownloadPdf, actionDownloadListing, downloadZipMethod, InvoiceType.Invoice));
          stringBuilder.Append("}");
          stringBuilder.Append("}</script>");

          return MvcHtmlString.Create(stringBuilder.ToString());
      }

      /// <summary>
      /// Method to generate Misc Payable Invoice search grid action column script.
      /// SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
      /// </summary>
      /// <param name="uri"></param>
      /// <param name="actionView"></param>
      /// <param name="rejectMethod"></param>
      /// <param name="actionDownloadPdf"></param>
      /// <param name="actionDownloadListing"></param>
      /// <param name="downloadZipMethod"></param>
      /// <param name="invoiceType"></param>
      /// <returns></returns>
      private static string GetMiscInvoiceCreditNoteScript(UrlHelper uri, string actionView, string rejectMethod, string actionDownloadPdf,
                                                           string actionDownloadListing, string downloadZipMethod, InvoiceType invoiceType)
      {
          var userManager = Ioc.Resolve<IUserManager>();

          // Get the list of user permissions.
          var permissionList = userManager.GetUserPermissions(SessionUtil.UserId);

          var stringBuilder = new StringBuilder();

          var canCreate = false;
          var canView = false;
          var canDownload = false;

          if (invoiceType == InvoiceType.CreditNote)
          {
              canCreate = permissionList.Contains(Business.Security.Permissions.Misc.Receivables.CreditNote.CreateOrEdit);
              canView = permissionList.Contains(Business.Security.Permissions.Misc.Payables.CreditNoteBillings.View);
              canDownload = permissionList.Contains(Business.Security.Permissions.Misc.Payables.CreditNoteBillings.Download);
          }
          else
          {
              canCreate = permissionList.Contains(Business.Security.Permissions.Misc.Receivables.Invoice.CreateOrEdit);
              canView = permissionList.Contains(Business.Security.Permissions.Misc.Payables.Invoice.View);
              canDownload = permissionList.Contains(Business.Security.Permissions.Misc.Payables.Invoice.Download);
          }

          // If invoice is correspondence invoice OR rejection invoice with rejection stage 1 and SMI =  I or B OR rejection invoice of stage 2, do not show reject icon.
          stringBuilder.AppendFormat("if(rowObject.InvoiceTypeId == {0} || (rowObject.InvoiceTypeId == {1} && rowObject.RejectionStage == 1 && (rowObject.SettlementMethodId == {2} || rowObject.SettlementMethodId == {3} || rowObject.SettlementMethodId == {4})) || (rowObject.InvoiceTypeId == {1} && rowObject.RejectionStage == 2)){{",
                                                         (int)InvoiceType.CorrespondenceInvoice,
                                                         (int)InvoiceType.RejectionInvoice, (int)SMI.Bilateral, (int)SMI.Ich, (int)SMI.AdjustmentDueToProtest);
          stringBuilder.AppendFormat("if(rowObject.IsLegalPdfGenerated == 1 &&(rowObject.DigitalSignatureStatusId=={0} || rowObject.DigitalSignatureStatusId=={1})) {{ ",
                                                                 (int)DigitalSignatureStatus.Completed,
                                                                 (int)DigitalSignatureStatus.NotRequired); // check for if PDF has been generated.

          stringBuilder.Append(GetMiscPayViewPdfListingZipScript(uri, actionView, actionDownloadPdf, actionDownloadListing, downloadZipMethod, canView, canDownload, false));

          stringBuilder.Append("} else {"); // if PDF is not generated then do not include PDF download in actions.

          stringBuilder.Append(GetMiscPayInvoiceViewZipScript(uri, actionView, actionDownloadListing, downloadZipMethod, canView, canDownload, false));
          stringBuilder.Append("}"); // end of check for PDF is generated.
          stringBuilder.Append("}else{");
          stringBuilder.AppendFormat("if(rowObject.IsLegalPdfGenerated == 1 && (rowObject.DigitalSignatureStatusId=={0} || rowObject.DigitalSignatureStatusId=={1})) {{ ",
                                                            (int)DigitalSignatureStatus.Completed,
                                                            (int)DigitalSignatureStatus.NotRequired); // check for if PDF has been generated.

          stringBuilder.Append(GetMiscPayViewPdfListingZipScript(uri, actionView, actionDownloadPdf, actionDownloadListing, downloadZipMethod, canView, canDownload, false));

          // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
          canCreate = permissionList.Contains(Business.Security.Permissions.Misc.Receivables.Invoice.CreateOrEdit);

          if (canCreate && canView)
          {
              var tempStringBuilder = stringBuilder.ToString();
              if (!String.IsNullOrEmpty(tempStringBuilder))
              {
                  stringBuilder.Clear();
                  stringBuilder.Append(tempStringBuilder.TrimEnd(';').TrimEnd((char)39));
                  stringBuilder.Append(GetRejectScript(uri, rejectMethod));
              }
          }
          stringBuilder.Append("} else {"); // if PDF is not generated then do not include PDF download in actions.
          stringBuilder.Append(GetMiscPayInvoiceViewZipScript(uri, actionView, actionDownloadListing, downloadZipMethod, canView, canDownload, false));

          if (canCreate && canView)
          {
              var tempStringBuilder = stringBuilder.ToString();
              if (!String.IsNullOrEmpty(tempStringBuilder))
              {
                  stringBuilder.Clear();
                  stringBuilder.Append(tempStringBuilder.TrimEnd(';').TrimEnd((char)39));
                  stringBuilder.Append(GetRejectScript(uri, rejectMethod));
              }
          }

          stringBuilder.Append("}}"); // end of check for PDF is generated.

          return stringBuilder.ToString();
      }

      #endregion

      #endregion

      #region UATP

      /// <summary>
      /// Method to generate UATP Receivable Manage Invoice grid action column script.
      /// SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
      /// </summary>
      /// <param name="uri"></param>
      /// <param name="jqGridId"></param>
      /// <param name="actionView"></param>
      /// <param name="actionDownloadPdf"></param>
      /// <param name="actionDownloadZip"></param>
      /// <returns></returns>
      public static MvcHtmlString GenerateScriptForUatpRecManage(UrlHelper uri, string jqGridId, string actionView, string actionDownloadPdf, string actionDownloadZip)
      {
          var jqGridEditMethodName = string.Format("{0}_EditViewDeleteRecord(cellValue, options, rowObject)", jqGridId);
          var downloadZipMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", actionDownloadZip, "#" + jqGridId);

          var stringBuilder = new StringBuilder();
          // rowObject[22] = InvoiceTypeId
          stringBuilder.Append("<script type='text/javascript'>");
          stringBuilder.AppendFormat("function {0} {{", jqGridEditMethodName);
          stringBuilder.AppendFormat("if(rowObject.InvoiceTypeId=={0}){{", (int)InvoiceType.CreditNote);
          stringBuilder.Append(GetUatpInvoiceCreditNoteScript(uri, actionView, actionDownloadPdf, downloadZipMethod, InvoiceType.CreditNote, true));
          stringBuilder.Append("} else {");
          stringBuilder.Append(GetUatpInvoiceCreditNoteScript(uri, actionView, actionDownloadPdf, downloadZipMethod, InvoiceType.Invoice, true));
          stringBuilder.Append("}} </script>");

          return MvcHtmlString.Create(stringBuilder.ToString());
      }

      /// <summary>
      /// Method to generate UATP Payable Invoice search grid action column script.
      /// SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
      /// </summary>
      /// <param name="uri"></param>
      /// <param name="jqGridId"></param>
      /// <param name="actionView"></param>
      /// <param name="actionDownloadPdf"></param>
      /// <param name="actionDownloadZip"></param>
      /// <returns></returns>
      public static MvcHtmlString GenerateScriptForUatpPayableManage(UrlHelper uri, string jqGridId, string actionView, string actionDownloadPdf, string actionDownloadZip)
      {
          var jqGridEditMethodName = string.Format("{0}_ViewRejectRecord(cellValue, options, rowObject)", jqGridId);
          var downloadZipMethod = string.Format("\"{0}\",\"'+cellValue+'\",\"{1}\"", actionDownloadZip, "#" + jqGridId);
          var stringBuilder = new StringBuilder();
          // Indices : 1- InvoiceTypeId, 2: RejectionStage, 3: SettlementMethodId.
          stringBuilder.Append("<script type='text/javascript'>");
          stringBuilder.Append("function " + jqGridEditMethodName + "{");
          stringBuilder.AppendFormat("if(rowObject.InvoiceTypeId=={0}){{", (int)InvoiceType.CreditNote);
          stringBuilder.Append(GetUatpInvoiceCreditNoteScript(uri, actionView, actionDownloadPdf, downloadZipMethod, InvoiceType.CreditNote, false));
          stringBuilder.Append("}else{");
          stringBuilder.Append(GetUatpInvoiceCreditNoteScript(uri, actionView, actionDownloadPdf, downloadZipMethod, InvoiceType.Invoice, false));
          stringBuilder.Append("}}</script>");

          return MvcHtmlString.Create(stringBuilder.ToString());
      }

      /// <summary>
      /// Method to generate UATP Invoice search grid action column script.
      /// SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
      /// </summary>
      /// <param name="uri"></param>
      /// <param name="actionView"></param>
      /// <param name="actionDownloadPdf"></param>
      /// <param name="downloadZipMethod"></param>
      /// <param name="invoiceType"></param>
      /// <param name="isReceivables"></param>
      /// <returns></returns>
      private static string GetUatpInvoiceCreditNoteScript(UrlHelper uri, string actionView, string actionDownloadPdf,
                                                           string downloadZipMethod, InvoiceType invoiceType, bool isReceivables)
      {
          var authorizationManager = Ioc.Resolve<IAuthorizationManager>();

          var stringBuilder = new StringBuilder();

          var canView = false;
          var canDownload = false;

          if (isReceivables)
          {
              if (invoiceType == InvoiceType.CreditNote)
              {
                  canView = authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.UATP.Receivables.CreditNote.View);
                  canDownload = authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.UATP.Receivables.CreditNote.Download);
              }
              else
              {
                  canView = authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.UATP.Receivables.Invoice.View);
                  canDownload = authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.UATP.Receivables.Invoice.Download);
              }

              // rowObject[0] = InvoiceId
              // rowObject[1] = InvoiceStatusId
              // rowObject[2] = SubmissionMethodId
              // rowObject[6] = InputFileStatusId
              // rowObject[22] = InvoiceTypeId

              // generate script for invoice with Submission method as IS-XML or IS-IDEC.
              stringBuilder.AppendFormat("if(rowObject.SubmissionMethodId=={0} || rowObject.SubmissionMethodId=={1}){{", (int)SubmissionMethod.IsXml, (int)SubmissionMethod.IsIdec);
              // conditions for Invoice Status in IS-XML or IS-IDEC
              stringBuilder.AppendFormat("if(((rowObject.InvoiceStatusId=={0}) ||(rowObject.InvoiceStatusId=={1})) && (rowObject.InputFileStatusId=={2}) ){{",
                                                             (int)InvoiceStatusType.ErrorCorrectable,
                                                             (int)InvoiceStatusType.ErrorNonCorrectable,
                                                             (int)FileStatusType.ValidationCompleted);

              //SCP#474713 :SRM - "undefined" value in UATP Search screen 
              stringBuilder.Append(canView ? GetViewScript(uri, actionView) : GetEmptyActionScript());

              stringBuilder.AppendFormat("}} else if((rowObject.InvoiceStatusId=={0}) ) {{", (int)InvoiceStatusType.OnHold);

              stringBuilder.Append(canView ? GetViewScript(uri, actionView) : GetEmptyActionScript());

              stringBuilder.AppendFormat("}} else if( (rowObject.InvoiceStatusId=={0}) || (rowObject.InvoiceStatusId=={1}) || (rowObject.InvoiceStatusId=={2}) || (rowObject.InvoiceStatusId=={3}) || (rowObject.InvoiceStatusId=={4}) ){{",
                              (int)InvoiceStatusType.ProcessingComplete,
                              (int)InvoiceStatusType.Presented,
                              (int)InvoiceStatusType.Claimed,
                              (int)InvoiceStatusType.ReadyForBilling,
                              (int)InvoiceStatusType.FutureSubmitted);

              // script for View, PDF, Zip
              stringBuilder.AppendFormat("if(rowObject.IsLegalPdfGenerated == 1 && (rowObject.DigitalSignatureStatusId=={0} || rowObject.DigitalSignatureStatusId=={1})) {{ ",
                                                                      (int)DigitalSignatureStatus.Completed,
                                                                      (int)DigitalSignatureStatus.NotRequired); // check for if PDF has been generated.

              stringBuilder.Append(GetViewPdfZipScript(uri, actionView, actionDownloadPdf, downloadZipMethod, canView, canDownload));
              stringBuilder.Append("} else {"); // if PDF is not generated then do not include PDF download in actions.
              stringBuilder.Append(GetInvoiceViewZipScript(uri, actionView, downloadZipMethod, canView, canDownload));
              stringBuilder.Append("}"); // end of check for PDF is generated.
              stringBuilder.AppendFormat("}} else if(rowObject.SubmissionMethodId=={0}){{",
                                                                   (int)SubmissionMethod.AutoBilling); // Empty action script for default condition when invoice status criteria is not match.
              stringBuilder.AppendFormat("if(rowObject.IsLegalPdfGenerated == 1 && (rowObject.DigitalSignatureStatusId=={0} || rowObject.DigitalSignatureStatusId=={1})) {{ ",
                                                                                 (int)DigitalSignatureStatus.Completed,
                                                                                 (int)DigitalSignatureStatus.NotRequired); // check for if PDF has been generated.
              stringBuilder.Append(GetViewPdfZipScript(uri, actionView, actionDownloadPdf, downloadZipMethod, canView, canDownload));
              stringBuilder.Append("} else {"); // if PDF is not generated then do not include PDF download in actions.
              stringBuilder.Append(GetInvoiceViewZipScript(uri, actionView, downloadZipMethod, canView, canDownload));
              stringBuilder.Append("}"); // end of check for PDF is generated.
              stringBuilder.Append(canView ? GetViewScript(uri, actionView) : GetEmptyActionScript());
              stringBuilder.Append("} else {"); // Empty action script for default condition when invoice status criteria is not match.
              stringBuilder.Append(canView ? GetViewScript(uri, actionView) : GetEmptyActionScript());
              stringBuilder.Append("}"); // end of conditions Invoice Status in IS-XML or IS-IDEC.

              // else Submission method is IS-Web.
              stringBuilder.AppendFormat("}} else if(rowObject.SubmissionMethodId=={0}) {{ ",
                                                                   (int)SubmissionMethod.IsWeb);
              stringBuilder.AppendFormat("if((rowObject.InvoiceStatusId=={0}) || (rowObject.InvoiceStatusId=={1}) ) {{",
                                                            (int)InvoiceStatusType.Open,
                                                            (int)InvoiceStatusType.ValidationError);
              stringBuilder.AppendFormat("}} else if(rowObject.InvoiceStatusId=={0}){{",
                                                                   (int)InvoiceStatusType.ReadyForSubmission);
              stringBuilder.AppendFormat("}} else if((rowObject.InvoiceStatusId=={0}) || (rowObject.InvoiceStatusId=={1}) || (rowObject.InvoiceStatusId=={2}) || (rowObject.InvoiceStatusId=={3}) || (rowObject.InvoiceStatusId=={4}) ){{",
                                                                    (int)InvoiceStatusType.Presented,
                                                                    (int)InvoiceStatusType.ProcessingComplete,
                                                                    (int)InvoiceStatusType.Claimed,
                                                                    (int)InvoiceStatusType.ReadyForBilling,
                                                                    (int)InvoiceStatusType.FutureSubmitted);
              // script for View, PDF, Zip
              stringBuilder.AppendFormat("if(rowObject.IsLegalPdfGenerated == 1 && (rowObject.DigitalSignatureStatusId=={0} || rowObject.DigitalSignatureStatusId=={1})) {{ ",
                                                                                 (int)DigitalSignatureStatus.Completed,
                                                                                 (int)DigitalSignatureStatus.NotRequired); // check for if PDF has been generated.
              stringBuilder.Append(GetViewPdfZipScript(uri, actionView, actionDownloadPdf, downloadZipMethod, canView, canDownload));
              stringBuilder.Append(" } else { "); // if PDF is not generated then do not include PDF download in actions.
              stringBuilder.Append(GetInvoiceViewZipScript(uri, actionView, downloadZipMethod, canView, canDownload));
              stringBuilder.Append(" }"); // end of check for PDF is generated.
              stringBuilder.Append("} else {"); // Empty action script for default condition when invoice status criteria is not match.
              stringBuilder.Append(GetEmptyActionScript());
              stringBuilder.Append("}"); // end of conditions Invoice Status in IS-Web.
              //Logic: Download PDF button will be display in case of Autobilling also. 
              stringBuilder.AppendFormat("}} else if (rowObject.SubmissionMethodId == {0}) {{",
                                                                      (int)SubmissionMethod.AutoBilling); // end of Else If Submission method is IS-Web.
              stringBuilder.AppendFormat("if(rowObject.IsLegalPdfGenerated == 1 && (rowObject.DigitalSignatureStatusId=={0} || rowObject.DigitalSignatureStatusId=={1})) {{ ",
                                                                                 (int)DigitalSignatureStatus.Completed,
                                                                                 (int)DigitalSignatureStatus.NotRequired); // check for if PDF has been generated.
              stringBuilder.Append(GetViewPdfZipScript(uri, actionView, actionDownloadPdf, downloadZipMethod, canView, canDownload));
              stringBuilder.Append(" } else { "); // if PDF is not generated then do not include PDF download in actions.
              stringBuilder.Append(GetInvoiceViewZipScript(uri, actionView, downloadZipMethod, canView, canDownload));
              stringBuilder.Append("}"); // end of check for PDF is generated.
              stringBuilder.Append(canView ? GetViewScript(uri, actionView) : GetEmptyActionScript());
              stringBuilder.Append("} else {"); // end of default condition for Submission method block.
              stringBuilder.Append(GetEmptyActionScript());
              stringBuilder.Append("}");
          }
          else
          {
              if (invoiceType == InvoiceType.CreditNote)
              {
                  canView = authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.UATP.Payables.CreditNoteBillings.View);
                  canDownload = authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.UATP.Payables.CreditNoteBillings.Download);
              }
              else
              {
                  canView = authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.UATP.Payables.Invoice.View);
                  canDownload = authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.UATP.Payables.Invoice.Download);
              }

              stringBuilder.AppendFormat("if(rowObject.InvoiceTypeId == {0} || (rowObject.InvoiceTypeId == {1} && rowObject.RejectionStage == 1 && (rowObject.SettlementMethodId == {2} || rowObject.SettlementMethodId == {3} || rowObject.SettlementMethodId == {4})) || (rowObject.InvoiceTypeId == {1} && rowObject.RejectionStage == 2)){{",
                                             (int)InvoiceType.CorrespondenceInvoice, (int)InvoiceType.RejectionInvoice,
                                             (int)SMI.Bilateral, (int)SMI.Ich, (int)SMI.AdjustmentDueToProtest);
              stringBuilder.AppendFormat("if(rowObject.IsLegalPdfGenerated == 1 &&(rowObject.DigitalSignatureStatusId=={0} || rowObject.DigitalSignatureStatusId=={1})) {{ ",
                                                                     (int)DigitalSignatureStatus.Completed,
                                                                     (int)DigitalSignatureStatus.NotRequired); // check for if PDF has been generated.
              stringBuilder.Append(GetViewPdfZipScript(uri, actionView, actionDownloadPdf, downloadZipMethod, canView, canDownload));
              stringBuilder.Append("} else {"); // if PDF is not generated then do not include PDF download in actions.
              stringBuilder.Append(GetInvoiceViewZipScript(uri, actionView, downloadZipMethod, canView, canDownload));
              stringBuilder.Append("}"); // end of check for PDF is generated.
              stringBuilder.Append("}else{");
              stringBuilder.AppendFormat("if(rowObject.IsLegalPdfGenerated == 1 && (rowObject.DigitalSignatureStatusId=={0} || rowObject.DigitalSignatureStatusId=={1})) {{ ",
                                                                      (int)DigitalSignatureStatus.Completed,
                                                                      (int)DigitalSignatureStatus.NotRequired); // check for if PDF has been generated.
              stringBuilder.Append(GetViewPdfZipScript(uri, actionView, actionDownloadPdf, downloadZipMethod, canView, canDownload));
              stringBuilder.Append("} else {"); // if PDF is not generated then do not include PDF download in actions.
              stringBuilder.Append(GetInvoiceViewZipScript(uri, actionView, downloadZipMethod, canView, canDownload));
              stringBuilder.Append("}}"); // end of check for PDF is generated.
          }

          return stringBuilder.ToString();
      }

      #endregion

      /// <summary>
      /// Method to generate Reject icon script.
      /// SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
      /// </summary>
      /// <param name="uri"></param>
      /// <param name="actionReject"></param>
      /// <returns></returns>
      private static string GetRejectScript(UrlHelper uri, string actionReject)
      {
          return
              string.Format(
                  "&nbsp;<a style=cursor:hand target=_parent href=javascript:callRejectLineItems({0});><img title=Reject style=border-style:none src={1} /></a>';",
                  actionReject, uri.Content("~/Content/Images/reject_invoice.png"));
      }


      /// <summary>
      /// SCP442581 - legal archive in SIS
      /// </summary>
      /// <param name="uri"></param>
      /// <param name="jqGridId"></param>
      /// <param name="actionDownload"></param>
      /// <returns></returns>
      public static MvcHtmlString GenerateArchiveRetrivalJobDetailsScript(UrlHelper uri, string jqGridId, string actionDownload)
      {
          const string jqGridEditMethodName = "DownloadZipRecord(cellValue, options, rowObject)";
          var sb = new StringBuilder();
          sb.Append("<script type='text/javascript'>");
          sb.AppendFormat("function {0} {{", jqGridEditMethodName);
          sb.Append("if(rowObject.IsFileExist != 'False' ) { ");
          sb.AppendFormat("return   '<a style=cursor:hand target=_parent href=javascript:DownloadLegalZip(\"'+cellValue+'\");><img title=View alt=View style=border-style:none src={0} /></a>';",
                          uri.Content("~/Content/Images/zip.png"));
          sb.Append("} else { return ' ';}} </script>");
          return MvcHtmlString.Create(sb.ToString());
      }

      /* CMP #675: Progress Status Bar for Processing of Billing Data Files. 
       * Desc: Function to generate dynamic script for the action column in grid. */
      public static MvcHtmlString GenerateProgressBarGridScript(UrlHelper url, string progreeBarGrid)
      {
          string jqGridEditMethodName = string.Format("{0}_GenerateProgressBarAction(cellValue, options, rowObject)",
                                                  progreeBarGrid);

          var sb = new StringBuilder();

          sb.Append("<script type='text/javascript'>");
          sb.Append("function " + jqGridEditMethodName + " {");
              
          if(string.Equals(progreeBarGrid, "PDFileStatusGrid", StringComparison.InvariantCultureIgnoreCase))
          {
              sb.Append("if(rowObject.FileProgressStatus==1){ ");
              sb.Append(
                  " return '<img style=cursor:pointer title=\"File Progress Status\" alt=\"File_Progress_Status\" onclick=ShowFileProgressStatus(\"'+rowObject.IsFileLogId+'\") style=border-style:none src=" +
                  url.Content("~/Content/Images/File_Progress_Status.png") + "  />';");
          }
          else if(string.Equals(progreeBarGrid, "UploadFileStatusGrid", StringComparison.InvariantCultureIgnoreCase))
          {
              sb.Append("if(rowObject.FileProgressStatus==1){ ");
              sb.Append(
                  " return '<img style=cursor:pointer title=\"File Progress Status\" alt=\"File_Progress_Status\" onclick=ShowFileProgressStatus(\"'+rowObject.IsFileLogId+'\") style=border-style:none src=" +
                  url.Content("~/Content/Images/File_Progress_Status.png") + "  />';");
          }
          else if(string.Equals(progreeBarGrid, "SMCurrentStatsGrid", StringComparison.InvariantCultureIgnoreCase))
          {
              sb.Append("if(rowObject.FileProgressStatus==1){ ");
              sb.Append(
                  " return '<img style=cursor:pointer title=\"File Progress Status\" alt=\"File_Progress_Status\" onclick=ShowFileProgressStatus(\"'+rowObject.FileId+'\") style=border-style:none src=" +
                  url.Content("~/Content/Images/File_Progress_Status.png") + "  />';");
          }
          
          sb.Append(" } else { ");
          sb.Append(" return '';");
          sb.Append(" } ");
          sb.Append("}</script>");

          return MvcHtmlString.Create(sb.ToString());
      }
  }
}
