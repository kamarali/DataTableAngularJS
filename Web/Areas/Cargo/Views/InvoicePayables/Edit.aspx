<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Cargo.CargoInvoice>"
  MasterPageFile="~/Views/Shared/Site.Master" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<asp:Content runat="server" ID="Content" ContentPlaceHolderID="TitleContent">
  SIS :: Cargo ::
  <%--<%: ViewData[ViewDataConstants.BillingType].ToString() %>--%>
  ::
  <%-- <%:ViewData[ViewDataConstants.PageMode] %>--%>
  Invoice
</asp:Content>
<asp:Content runat="server" ID="Content1" ContentPlaceHolderID="Script">
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Cargo/InvoiceHeader.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/MemberReference.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Pax/ValidateInvoice.js")%>"></script>
  <script type="text/javascript">
  
      $(document).ready(function () {
      <%if(ViewData["IsLegalTextSet"] != null && Convert.ToBoolean(ViewData["IsLegalTextSet"]) == true)
    {%>
    isBillingLegalTextSet = true;
  <%
    }%>
      var isViewMode = <%:ViewData[ViewDataConstants.PageMode] !=null ?(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View).ToString().ToLower() : "false"%>;
      InitializeBilateralSMIs(<%: Convert.ToInt32(SMI.Bilateral) %>  ,'<%: ViewData[ViewDataConstants.BilateralSMIs] %>');
      InitialiseInvoiceHeader('<%: Url.Action("GetBilledMemberLocationList", "Data", new { area = ""})%>', '<%: Url.Action("GetExchangeRate", "Data", new { area = ""})%>',<%:ViewData[ViewDataConstants.PageMode] !=null ?(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View).ToString().ToLower() : "false"%>, '<%: Url.Action("GetDefaultSettlementMethod", "Data", new { area = ""})%>', '<%:Url.Action("GetDefaultCurrency", "Data", new { area = "" })%>','<%:SessionUtil.MemberId%>', '#InvoiceForm,#BillingMemberReference,#BilledMemberReference');
      if(isViewMode == false)
        InitReferenceData('<%:Url.Action("GetMemberLocationDetails","Data",new{area = string.Empty})%>', '<%:Model.Id%>','<%:Url.Action("GetSubdivisionNameList","Data",new{area = string.Empty})%>');
      setInvoiceHeaderFocus();
        <%
        if(ViewData[ViewDataConstants.TransactionExists] != null && Convert.ToBoolean(ViewData[ViewDataConstants.TransactionExists]))
        {%>
        SetControlAccess();
        $("#BilledMemberText").autocomplete({ disabled: true });
        <%
        }
        else
        {%>
        registerAutocomplete('BilledMemberText', 'BilledMemberId', '<%:Url.Action("GetMemberList","Data",new{area = ""})%>', 0, true, function (selectedId) { BilledMemberText_AutoCompleteValueChange(selectedId); });
      <%}%>
        $('#PrimeBillingButton').show();
        $('#RejectionMemoButton').show();
        $('#BillingMemoButton').show();

      });
      
    // Following function is used to format SourceCodeVatTotal column within Jqgrid to display link
    function formatBillingCodeVatTotalColumn(cellValue, options, rowObject) {
      // Retrieve SourceCodeVatBreakdownId
      var sourceCodeId = rowObject.Id;
      // Create link and specify onclick action passing it SourceCodeVatBreakdownId
      var linkHtml = '<a href="#" onclick=showSourceCodeVatDialog("<%: Url.Action("GetBillingCodeVatTotal", "InvoicePayables", new { area = "Cargo" })%>","' + sourceCodeId + '")>' + "Billing Code VAT" + '</a>';
      // return link
      return linkHtml;
    }      
  </script>
</asp:Content>
<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="MainContent">
  <h1>
    <%:ViewData[ViewDataConstants.PageMode] %>
    Invoice</h1>
  <% 
    using (Html.BeginForm("Edit", "Invoice", FormMethod.Post, new { id = "InvoiceForm", @class = "validCharacters" }))
    {%>
    <%: Html.AntiForgeryToken() %>
  <div>
    <%
      Html.RenderPartial("InvoiceHeaderControl");%>
  </div>
  <div class="buttonContainer">
    <input class="primaryButton ignoredirty" type="submit" value="Save Invoice Header"
      id="SaveInvoiceHeader" />
  </div>
  <%
    }%>
  <div>
    <%
      Html.RenderPartial("InvoiceTotalControl", Model.CGOInvoiceTotal);%>
  </div>
  
    <div class="clear">
    </div>
    <h2>
      Sub Total List</h2>
    <div id="sourceCodeDetailsDiv">
      <div>
        <%
          Html.RenderPartial("GridControl", ViewData[ViewDataConstants.SourceCodeGrid]);%>
      </div>
    </div>
    <div class="clear">
    </div>
    <div class="buttonContainer">

      <div>
        <%if (ViewData[ViewDataConstants.PageMode].ToString() != PageMode.View)
          {
            using (Html.BeginForm("AwbPrepaidBillingCreate", "Invoice", new { invoiceId = Model.Id }, FormMethod.Get))
            {%>
        <input type="submit" value="Prepaid AWB Capture" class="primaryButton" id="btnPrepaidCapture" />
        <%
            }
          }
          using (Html.BeginForm(((ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View) ? "AwbPrepaidBillingListView" : "AwbPrepaidBillingList"), "InvoicePayables", FormMethod.Get))
          {%>
        <input class="secondaryButton" type="submit" value="Prepaid AWB Listing" id="PrepaidBillingButton" />
        <%
          }
        %>
        <%if (ViewData[ViewDataConstants.PageMode].ToString() != PageMode.View)
          {
            using (Html.BeginForm("AwbChargeCollectBillingCreate", "Invoice", FormMethod.Get))
            {%>
        <input type="submit" value="Collect AWB Capture" class="primaryButton" id="btnChargeCollectCapture" />
        <%
            }
          }

          using (Html.BeginForm(((ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View) ? "AwbChargeCollectBillingListView" : "AwbChargeCollectBillingList"), "InvoicePayables", FormMethod.Get, new { invoiceId = Model.Id, billingType = ViewData[ViewDataConstants.BillingType].ToString()}))
          {%>
        <input class="secondaryButton" type="submit" value="Collect AWB Listing" id="ChargeCollectBillingButton" />
        <%
          }
           if (ViewData[ViewDataConstants.PageMode].ToString() != PageMode.View)
          {
          using (Html.BeginForm("RMCreate", "Invoice", FormMethod.Get))
          {%>
        <input type="submit" value="RM Capture" class="primaryButton" id="btnRMCapture" />
        <% }
             }
          using (Html.BeginForm(((ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View) ? "RMListView" : "RMList"), "InvoicePayables", FormMethod.Get, new { invoiceId = Model.Id, billingType = ViewData[ViewDataConstants.BillingType].ToString()}))
            {%>
        <input class="secondaryButton" type="submit" value="RM Listing" id="RejectionMemoButton" />
        <%}
          if (ViewData[ViewDataConstants.PageMode].ToString() != PageMode.View)
          {
          using (Html.BeginForm("BMCreate", "Invoice", FormMethod.Get))
          {%>
        <input type="submit" value="BM Capture" class="primaryButton" id="btnBMCapture" />
        <%
          }
          }

          using (Html.BeginForm(((ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View) ? "BMListView" : "BMList"), "InvoicePayables", FormMethod.Get, new { invoiceId = Model.Id, billingType = ViewData[ViewDataConstants.BillingType].ToString() }))
          {%>
        <input class="secondaryButton" type="submit" value="BM Listing" id="BillingMemoButton" />
        <%
        }%>
        <div class="viewModeDisplayButtons">
        <%
          using (Html.BeginForm(((ViewData[ViewDataConstants.PageMode].ToString() != PageMode.View) ? "VatView" : "Vat"), "InvoicePayables", FormMethod.Get))
          //using (Html.BeginForm(((ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View) ? "Vat" : "Vat"), "Invoice", FormMethod.Get))
          {%>
        <input class="secondaryButton" type="submit" value="Invoice VAT" />
        <%
          }%>
    </div>
    <br/>

      <div>
      <%
        // TODO: Need to figure out which invoice status should be checked here
        if ((Model.InvoiceStatus == Iata.IS.Model.Enums.InvoiceStatusType.Open))
        {
          using (Html.BeginForm("ValidateInvoice", "Invoice", FormMethod.Post))
          {%>
          <%: Html.AntiForgeryToken() %>
      <input class="primaryButton" type="submit" value="Validate Invoice" />
      <%
          }
        }
        if (Model.InvoiceStatus == Iata.IS.Model.Enums.InvoiceStatusType.ReadyForSubmission)
        {
          using (Html.BeginForm("Submit", "Invoice", FormMethod.Post))
          {
      %>
      <%: Html.AntiForgeryToken() %>
      <input class="primaryButton" type="submit" value="Submit Invoice" />
      <%
          }
        }
      %>
      <%--<div class="viewModeDisplayButtons">
        <%
          using (Html.BeginForm(((ViewData[ViewDataConstants.PageMode].ToString() != PageMode.View) ? "VatView" : "Vat"), "Invoice", FormMethod.Get))
          //using (Html.BeginForm(((ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View) ? "Vat" : "Vat"), "Invoice", FormMethod.Get))
          {%>
        <input class="secondaryButton" type="submit" value="Invoice VAT" />
        <%
          }%>--%>
   

        <%
          if (!string.IsNullOrEmpty(SessionUtil.CGOCorrSearchCriteria) || !string.IsNullOrEmpty(SessionUtil.CGOInvoiceSearchCriteria))
          {
        %>
        <%: Html.LinkButton("Back To Billing History", Url.Action("Index", "BillingHistory", new { back = true }))%>
        <%}

          else if (!string.IsNullOrEmpty(SessionUtil.InvoiceSearchCriteria))
          { %>
        <input class="secondaryButton" type="button" value="Back to Manage Invoice" onclick="javascript:location.href = '<%:SessionUtil.InvoiceSearchCriteria%>';" />
        <%
          }
        %>
      </div>
    </div>
    </div>
     <%if (Model.InvoiceStatus == InvoiceStatusType.ValidationError)
      { 
    %>
    <h2>
      Validation Errors</h2>
    <div class="horizontalFlow">
      <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.SubmittedErrorsGrid]); %>
    </div>
    <%}%>
    <div id="BillingMemberReference" class="hidden">
      <%
        Html.RenderPartial("~/Views/Invoice/BillingMemberInfoControl.ascx", Model);%>
    </div>
    <div id="BilledMemberReference" class="hidden">
      <%
        Html.RenderPartial("~/Views/Invoice/BilledMemberInfoControl.ascx", Model);%>
    </div>
    <!-- Following div is used for SourceCode Vat popup so add it only if BillingType is Payables -->
     <%if (ViewData[ViewDataConstants.BillingType].ToString() == Iata.IS.Web.Util.BillingType.Payables)
       {%>
      <div id="divAvailableVatGridResult" class="hidden">
        <% Html.RenderPartial("~/Areas/Cargo/Views/Shared/BillingCodeTotal.ascx", ViewData["VatGrid"]);%>
      </div>
    <%}%>
</asp:Content>
