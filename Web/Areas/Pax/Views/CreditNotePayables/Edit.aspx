<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.PaxInvoice>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<%@ Import Namespace="System.Security.Policy" %>

<asp:Content ID="titleBlock" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger ::
  <%: ViewData[ViewDataConstants.BillingType].ToString() %>
  ::
  <%: ViewData[ViewDataConstants.PageMode] %>
  Credit Note
</asp:Content>
<asp:Content ID="scriptBlock" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Pax/InvoiceHeader.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/MemberReference.js")%>"></script>
  <script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/Billinghistory.js")%>" type="text/javascript"></script>
  <script type="text/javascript">
      $(document).ready(function () {  
      <%if(ViewData["IsLegalTextSet"] != null && Convert.ToBoolean(ViewData["IsLegalTextSet"]) == true)
    {%>
    isBillingLegalTextSet = true;
  <%
    }%>
      initializeParentForm('InvoiceForm');
     SetInvoiceType(<%:(Model.InvoiceType == InvoiceType.CreditNote ? 1 : 0)%>);
     var isViewMode = <%:ViewData[ViewDataConstants.PageMode] !=null ?(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View).ToString().ToLower() : "false"%>;
     InitializeBilateralSMIs(<%: Convert.ToInt32(SMI.Bilateral) %>  ,'<%: ViewData[ViewDataConstants.BilateralSMIs] %>');
      InitialiseInvoiceHeader('<%:Url.Action("GetBilledMemberLocationList", "Data", new { area = "" })%>', '<%:Url.Action("GetExchangeRate", "Data", new { area = "" })%>', <%:(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View ? true : false ).ToString().ToLower() %>, '<%:Url.Action("GetDefaultSettlementMethod", "Data", new { area = "" })%>', '<%:Url.Action("GetDefaultCurrency", "Data", new { area = "" })%>', '<%:SessionUtil.MemberId%>', '#InvoiceForm,#BillingMemberReference,#BilledMemberReference');
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
      
    // Following function is used to format SourceCodeVatTotal column within Jqgrid to display link
    function formatSourceCodeVatTotalColumn(cellValue, options, rowObject) {
      // Retrieve SourceCodeVatBreakdownId
      var sourceCodeId = rowObject.Id;
      // Create link and specify onclick action passing it SourceCodeVatBreakdownId
      var linkHtml = '<a href="#" onclick=showSourceCodeVatDialog("<%: Url.Action("GetSourceCodeVatTotal", "CreditNotePayables", new { area = "Pax" })%>","' + sourceCodeId + '")>' + "Source Code VAT" + '</a>';
      // return link
      return linkHtml;
    }
  
  </script>
  <%
    if (ViewData[ViewDataConstants.BillingType] != null && ViewData[ViewDataConstants.BillingType].ToString() == Iata.IS.Web.Util.BillingType.Payables)
    {%>
  <%:ScriptHelper.GeneratePaxPayablesGridViewRejectScript(Url, ControlIdConstants.CreditMemoGrid, Url.Action("CreditMemoView"), "CM", Model.Id.Value(), Url.Action("InitiateRejection", "CreditNotePayables", new { area = "Pax" }), Url.Action("InitiateDuplicateRejection", "CreditNotePayables", new { area = "Pax" }), Model.BillingCode, Model.BillingYear, Model.BillingMonth, Model.BillingPeriod, Model.SettlementMethodId)%>
  <%}
    else

      if (ViewData[ViewDataConstants.PageMode] != null && ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
      {
  %>
  <%:ScriptHelper.GenerateGridViewScriptForBreakdown(Url, ControlIdConstants.CreditMemoGrid, Url.Action("CreditMemoView", new { invoiceId = Model.Id }))%>
  <%
      }
      else
      {
  %>
  <%:ScriptHelper.GenerateGridEditDeleteScript(Url, ControlIdConstants.CreditMemoGrid, Url.RouteUrl("transactions", new { action = "CreditMemoEdit", controller = "CreditNote" }), Url.Action("CreditMemoDelete"))%>
  <%
          }
  %>

</asp:Content>
<asp:Content ID="contentBlock" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    <%: ViewData[ViewDataConstants.PageMode] %>
    Non-Sampling Credit Note</h1>
  <%
    using (Html.BeginForm("Edit", "CreditNotePayables", FormMethod.Post, new { id = "InvoiceForm", @class = "validCharacters" }))
    {%>
  <div>
    <%
      Html.RenderPartial("InvoiceHeaderControl", Model);%>
  </div>
  <div class="buttonContainer">
    <input class="primaryButton ignoredirty" type="submit" value="Save Credit Note Header" />
  </div>
  <%
    }%>
  <div>
    <%
      Html.RenderPartial("InvoiceTotalControl", Model.InvoiceTotalRecord);%>
  </div>
  <h2>
    Summary List</h2>
  <div id="sourceCodeDetailsDiv">
    <%
      Html.RenderPartial("GridControl", ViewData[ViewDataConstants.SourceCodeGrid]);%>
  </div>
  <h2>
    Credit Memo List
  </h2>
  <div>
    <%
      Html.RenderPartial("GridControl", ViewData[ViewDataConstants.CreditMemoGrid]);%>
  </div>
  <div class="buttonContainer">
    <%
      if (Model.InvoiceStatus == InvoiceStatusType.Open && Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Pax.Receivables.NonSampleCreditNote.Validate)) // || Model.InvoiceStatus == InvoiceStatusType.ReadyForValidation)
      {
        using (Html.BeginForm("ValidateInvoice", "CreditNote", FormMethod.Post))
        {%>
        <%: Html.AntiForgeryToken() %>
    <input class="primaryButton" type="submit" value="Validate Credit Note" />
    <%
        }
      }
      if (Model.InvoiceStatus == InvoiceStatusType.ReadyForSubmission && Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Pax.Receivables.NonSampleCreditNote.Submit))
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
        using (Html.BeginForm("CreditMemoCreate", "CreditNote", FormMethod.Get))
        {%>
    <input type="submit" value="Create Credit Memo" class="primaryButton" id="btnAdd" />
    <%
        }
      }
    %>
    <div id="divTransactions">
      <%
        using (Html.BeginForm(((ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View) ? "VatView" : "Vat"), "CreditNotePayables", FormMethod.Get))
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
    <% Html.RenderPartial("~/Areas/Pax/Views/Shared/SourceCodeTotal.ascx", ViewData["VatGrid"]);%>
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
