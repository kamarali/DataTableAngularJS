﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Cargo.CargoInvoice>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>

<asp:Content ID="titleBlock" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Cargo ::
  <%: ViewData[ViewDataConstants.BillingType].ToString() %>
  ::
  <%: ViewData[ViewDataConstants.PageMode] %>
  Credit Note
</asp:Content>
<asp:Content ID="scriptBlock" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Cargo/InvoiceHeader.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/MemberReference.js")%>"></script>
  <script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Cargo/Billinghistory.js")%>" type="text/javascript"></script>
  <script type="text/javascript">
      $(document).ready(function () { 
      $("#AwbSerialNumberCheckDigit").numeric(); 
      <%if(ViewData["IsLegalTextSet"] != null && Convert.ToBoolean(ViewData["IsLegalTextSet"]) == true)
    {%>
    isBillingLegalTextSet = true;
  <%
    }%>
      initializeParentForm('InvoiceForm');     
     SetInvoiceType(<%:(Model.InvoiceType == InvoiceType.CreditNote ? 1 : 0)%>);
     var isViewMode = <%:ViewData[ViewDataConstants.PageMode] !=null ?(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View).ToString().ToLower() : "false"%>;
     InitializeBilateralSMIs(<%: Convert.ToInt32(SMI.Bilateral) %>  ,'<%: ViewData[ViewDataConstants.BilateralSMIs] %>');
      InitialiseInvoiceHeader('<%:Url.Action("GetBilledMemberLocationList", "Data", new { area = "" })%>', '<%:Url.Action("GetExchangeRate", "Data", new { area = "" })%>', <%:(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View ? true : false ).ToString().ToLower() %>, '<%:Url.Action("GetDefaultSettlementMethod", "Data", new { area = "" })%>', '<%:Url.Action("GetDefaultCurrency", "Data", new { area = "" })%>', '<%:SessionUtil.MemberId%>', '#InvoiceForm,#BillingMemberReference,#BilledMemberReference', '<%:Url.Action("IsBillingAndBilledAchOrDualMember", "Data", new { area = "" })%>', '<%: Model.BillingCurrencyId %>');
      if(isViewMode == false)
        InitReferenceData('<%:Url.Action("GetMemberLocationDetails","Data",new{area = string.Empty})%>', '<%:Model.Id%>','<%:Url.Action("GetSubdivisionNameList","Data",new{area = string.Empty})%>');
      setInvoiceHeaderFocus();
        <%
          if (ViewData[ViewDataConstants.TransactionExists] != null && Convert.ToBoolean(ViewData[ViewDataConstants.TransactionExists]))
          {
%>
        SetControlAccess();
        $("#BilledMemberText").autocomplete({ disabled: true });
        <%
          }
%>
        
      });
      
//    // Following function is used to format SourceCodeVatTotal column within Jqgrid to display link
//    function formatSourceCodeVatTotalColumn(cellValue, options, rowObject) {
//      // Retrieve SourceCodeVatBreakdownId
//      var sourceCodeId = rowObject[10];
//      // Create link and specify onclick action passing it SourceCodeVatBreakdownId
//      var linkHtml = '<a href="#" onclick=showSourceCodeVatDialog("<%: Url.Action("GetSourceCodeVatTotal", "CreditNote", new { area = "Cargo" })%>","' + sourceCodeId + '")>' + "Source Code VAT" + '</a>';
//      // return link
//      return linkHtml;
//    }
   
   // Following function is used to format BillingCodeVatTotal column within Jqgrid to display link
   function formatBillingCodeVatTotalColumn(cellValue, options, rowObject) {
      // Retrieve BillingCodeVatTotalVatBreakdownId
      var sourceCodeId = rowObject.Id;
      // Create link and specify onclick action passing it BillingCodeVatTotalBreakdownId
      var linkHtml = '<a href="#" onclick=showBillingCodeCodeVatDialog("<%: Url.Action("GetBillingCodeVatTotal", "CreditNote", new { area = "Cargo" })%>","' + sourceCodeId + '")>' + "Billing Code VAT" + '</a>';
      // return link
      return linkHtml;
    }  
  
  </script>
  <%
    if (ViewData[ViewDataConstants.BillingType] != null && ViewData[ViewDataConstants.BillingType].ToString() == Iata.IS.Web.Util.BillingType.Payables)
    {%>
  <%:ScriptHelper.GeneratePaxPayablesGridViewRejectScript(Url, ControlIdConstants.CreditMemoGrid, Url.Action("CMView"), "CM", Model.Id.Value(), Url.Action("InitiateRejection", "CreditNote", new { area = "Cargo" }), Url.Action("InitiateDuplicateRejection", "CreditNote", new { area = "Cargo" }), Model.BillingCode, Model.BillingYear, Model.BillingMonth, Model.BillingPeriod, Model.SettlementMethodId)%>
  <%}
    else

      if (ViewData[ViewDataConstants.PageMode] != null && ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
      {
  %>
  <%:ScriptHelper.GenerateGridViewScriptForBreakdown(Url, ControlIdConstants.CreditMemoGrid, Url.Action("CMView", new { invoiceId = Model.Id }))%>
  <%
      }
      else
      {
  %>
  <%:ScriptHelper.GenerateGridEditDeleteScript(Url, ControlIdConstants.CreditMemoGrid, Url.RouteUrl("CGOtransactions", new { action = "CMEdit", controller = "CreditNote" }), Url.Action("CreditMemoDelete"))%>
  <%
          }
  %>

</asp:Content>
<asp:Content ID="contentBlock" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    <%: ViewData[ViewDataConstants.PageMode] %>
    Cargo Credit Note</h1>
  <%
    using (Html.BeginForm("Edit", "CreditNote", FormMethod.Post, new { id = "InvoiceForm", @class = "validCharacters" }))
    {%>
    <%: Html.AntiForgeryToken() %>
  <div>
    <%
      Html.RenderPartial("InvoiceHeaderControl", Model);%>
  </div>
  <div class="buttonContainer">
    <input class="primaryButton ignoredirty" type="submit" value="Save Credit Note Header" id="SaveInvoiceHeader" />
  </div>
  <%
    }%>
  <div>
    <%
      Html.RenderPartial("InvoiceTotalControl", Model.CGOInvoiceTotal);%>
  </div>
  <h2>
    Sub-Total List</h2>
  <div id="sourceCodeDetailsDiv">
    <%
      Html.RenderPartial("GridControl", ViewData[ViewDataConstants.SubTotalGrid]);%>
  </div>
  <h2>
    Credit Memo List
  </h2>
  <div>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.CreditMemoGrid]);%>
  </div>
  <div class="buttonContainer">
  <%--SCP# 410956 - SUBMISSION A CREDIT NOTE.--%>
    <%
      //Old Code - if (Model.InvoiceStatus == InvoiceStatusType.Open && Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Pax.Receivables.NonSampleCreditNote.Validate)) // || Model.InvoiceStatus == InvoiceStatusType.ReadyForValidation)
      if (Model.InvoiceStatus == InvoiceStatusType.Open && Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.Validate)) // || Model.InvoiceStatus == InvoiceStatusType.ReadyForValidation)
      {
        using (Html.BeginForm("ValidateInvoice", "CreditNote", FormMethod.Post))
        {%>
        <%: Html.AntiForgeryToken() %>
    <input class="primaryButton" type="submit" value="Validate Credit Note" />
    <%
        }
      }
      if (Model.InvoiceStatus == InvoiceStatusType.ReadyForSubmission && Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.Submit))
      {
        using (Html.BeginForm("Submit", "CreditNote", FormMethod.Post))
        {
    %>
    <%: Html.AntiForgeryToken() %>
    <input class="primaryButton" type="submit" value="Submit Credit Note" />
    <%
            }
          }
    %>
    <%
      if (ViewData[ViewDataConstants.PageMode].ToString() != PageMode.View)
      {
        using (Html.BeginForm("CMCreate", "CreditNote", FormMethod.Get))
        {%>
    <input type="submit" value="Create Credit Memo" class="primaryButton" id="btnAdd" />
    <%
        }
      }
    %>
    <div id="divTransactions">
      <%
        using (Html.BeginForm(((ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View) ? "VatView" : "Vat"), "CreditNote", FormMethod.Get))
        {%>
      <input class="secondaryButton" type="submit" value="Credit Note VAT" />
      <%
        }%>
      <%
        if (!string.IsNullOrEmpty(SessionUtil.InvoiceSearchCriteria))
        {%>
      <input class="secondaryButton" type="button" value="Back to Manage Invoice" onclick="javascript:location.href = '<%:SessionUtil.InvoiceSearchCriteria%>';" />
      <%
        }%>
    </div>
  </div>
  <div class="clear">
  </div>
  <div id="BillingMemberReference" class="hidden">
    <%
      Html.RenderPartial("~/Views/Invoice/BillingMemberInfoControl.ascx", Model);%>
  </div>
  <div id="BilledMemberReference" class="hidden">
    <%
      Html.RenderPartial("~/Views/Invoice/BilledMemberInfoControl.ascx", Model);%>
  </div>
  <%
    if (Model.InvoiceStatus == InvoiceStatusType.ValidationError)
    {%>
  <h2>
    Validation Errors</h2>
  <div class="horizontalFlow">
    <%
      Html.RenderPartial("GridControl", ViewData[ViewDataConstants.SubmittedErrorsGrid]);%>
  </div>
  <%
    }%>
  <div class="clear">
  </div>
  <!-- Following div is used for SourceCode Vat popup so add it only if BillingType is Payables -->
  <%if (ViewData[ViewDataConstants.BillingType].ToString() == Iata.IS.Web.Util.BillingType.Payables)
    {%>
  <div id="divAvailableVatGridResult" class="hidden">
    <%--<% Html.RenderPartial("~/Areas/Pax/Views/Shared/SourceCodeTotal.ascx", ViewData["VatGrid"]);%>--%>
    <% Html.RenderPartial("~/Areas/Cargo/Views/Shared/BillingCodeTotal.ascx", ViewData["VatGrid"]);%>
  </div>
  <%}%>
  <div id="divBillingHistoryInvoice" class="hidden">
    <%
      Html.RenderPartial("BillingHistoryInvoice");%>
  </div>
  <div id="divDuplicateRejections" class="hidden">
    <%
      Html.RenderPartial("DuplicateRejectionControl");%>
  </div>
</asp:Content>
