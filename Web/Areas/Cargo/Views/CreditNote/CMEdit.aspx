<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Cargo.CargoCreditMemo>" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>

<%@ Import Namespace="Iata.IS.Model.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Cargo :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> ::
  <%: ViewData[ViewDataConstants.PageMode] %>
  Credit Memo
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">
    // Declare variable to capture reasonCode before editing
    var previousReasonCode = '';
    // Following variable is used to check edit level. i.e. CMEdit or CmCouponEdit. Edit level is checked before calling "EnableDisableAmountFieldsOnreasonCode()"
    // This function should be executed only if we are editing CreditMemo. 
    var editLevel = 'CMEdit';
    $(document).ready(function () {
      $("#AwbSerialNumberCheckDigit").numeric();
      initializeParentForm('creditMemoForm');     
      // Get previous reasonCode value 
      previousReasonCode = $('#ReasonCode').val();
      // Get whether coupon breakdown exists
      couponBreakdownExists = '<%: ViewData[ViewDataConstants.BreakdownExists] %>';
      // Get pageMode
      pageMode = '<%: ViewData[ViewDataConstants.PageMode] %>';
            
      SetPageToViewModeEx(<%:(ViewData[ViewDataConstants.PageMode] == PageMode.View).ToString().ToLower()%>, '#creditMemoForm,#divVatBreakdown,#divAttachment'); 
      registerAutocomplete('ReasonCode', 'ReasonCode', '<%:Url.Action("GetReasonCodeListForAutoComplete", "Data", new { area="" })%>', 0, false, onReasonCodeChange, '', '<%:Convert.ToInt32(TransactionType.CargoCreditMemo)%>', null, onBlankReasonCode);
      
    <%
      
 if(ViewData[ViewDataConstants.PageMode] != null &&  ViewData[ViewDataConstants.PageMode].ToString() =="View")
 {
%>
        $('input[type=text]').attr('disabled', 'disabled');
        $('select').attr('disabled', 'disabled');
        $("#SaveFormD").hide();
        $('textarea').attr('disabled','disabled');
        $('input[type=checkbox]').attr('disabled','disabled');
         $isOnView = true;
  <%
 }
%>
    $("#SourceCodeId").autocomplete({ disabled: true });
      InitializeVatGrid(<%= new JavaScriptSerializer().Serialize(Model.VatBreakdown) %>);      
      InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.Invoice.BilledMemberId, BillingCategoryType.Cgo) %>', '<%: Url.Action("CreditMemoAttachmentDownload","CreditNote", new {invoiceId = Model.InvoiceId}) %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");      
      setControlAccess('<%: ViewData[ViewDataConstants.BreakdownExists] %>');   
      $("#VatIdentifierId option[value='3']").remove();
    });
  </script>
  <script type="text/javascript">
    // Set billing type from Viewdata
    billingType = '<%: ViewData[ViewDataConstants.BillingType] %>';
    // Get whether coupon breakdown exists
    couponBreakdownExists = '<%: ViewData[ViewDataConstants.BreakdownExists] %>';
  </script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Cargo/CargoCreditMemo.js")%>"></script>
  <script src="<%:Url.Content("~/Scripts/Cargo/VatBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Cargo/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    <%: ViewData[ViewDataConstants.PageMode] %>
    Credit Memo</h1>
  <div>
    <% Html.RenderPartial("ReadOnlyCreditNoteHeaderControl", Model.Invoice); %>
  </div>
  <div>
    <% using (Html.BeginForm("CMEdit", "CreditNote", FormMethod.Post, new { id = "cargoCreditMemoForm", @class = "validCharacters" }))
       {%>
       <%: Html.AntiForgeryToken() %>
       <%
         Html.RenderPartial("CMDetailsControl", Model); %>
  </div>
  <div class="buttonContainer">
    <% if (ViewData[ViewDataConstants.PageMode] != null && ViewData[ViewDataConstants.PageMode].ToString() == "View")
{
  if (!string.IsNullOrEmpty(SessionUtil.CGOCorrSearchCriteria) || !string.IsNullOrEmpty(SessionUtil.CGOInvoiceSearchCriteria))
  {
%>
<input class="secondaryButton" type="button" value="Back To Billing History" onclick="javascript:location.href = '<%:Url.Action("Index", "BillingHistory", new { back = true })%>'" />
<%
  }
  else
  {
%>
    <input class="secondaryButton" type="button" value="Back to View Credit Note" onclick="javascript:location.href='<%:Url.Action("View",
                                 "CreditNote",
                                 new
                                   {
                                     invoiceId = Model.Invoice.Id.Value()
                                   })%>'" />
    <%
  }
}
else
       { %>
    <input type="submit" value="Save" class="primaryButton ignoredirty" id="Save" />
    <input type="submit" value="AWB Prepaid Breakdown" class="secondaryButton" id="btnAdd" onclick="javascript:location.href='<%: Url.Action("CMAwbPrepaidCreate", "CreditNote", new { invoiceId = Model.Invoice.Id.Value(), transactionId = Model.Id.Value(), transaction = "CMEdit" })%>'; return false;" />
    <input type="submit" value="AWB Charge Collect Breakdown" class="secondaryButton" id="btnAddChargeCollectAwb" onclick="javascript:location.href='<%:Url.Action("CMAwbChargeCollectCreate", "CreditNote", new {invoiceId = Model.InvoiceId.Value(), transaction = "CMEdit", transactionId = Model.Id.Value() })%>'; return false;" />
    <% 
         if (!string.IsNullOrEmpty(SessionUtil.CGOCorrSearchCriteria) || !string.IsNullOrEmpty(SessionUtil.CGOInvoiceSearchCriteria))
  {
%>
<%: Html.LinkButton("Back To Billing History", Url.Action("Index", "BillingHistory", new { back = true }))%>
<%
  }
  else
  {
%>  
    <%: Html.LinkButton("Back to View Credit Note", Url.RouteUrl("CGOCreditNoteEdit", new { action = "Edit", invoiceId = Model.Invoice.Id.Value() }))%>
    <%
  }
} %>
    <% } %>
  </div>
  <h2>
    Credit Memo Awb List</h2>
  <div>
    <%Html.RenderPartial("GridControl", ViewData[ViewDataConstants.CreditMemoAwbGrid]); %>
  </div>
  <div class="hidden" id="divVatBreakdown">
    <% Html.RenderPartial("CMVatControl", Model.VatBreakdown); %>
  </div>
  <div class="hidden" id="divAttachment">
    <%
      Html.RenderPartial("CMAttachmentControl", Model);%>
  </div>
  <div class="clear">
  </div>
  <%
    if (ViewData[ViewDataConstants.PageMode] != null && ViewData[ViewDataConstants.PageMode].ToString() == "View")
    {
  %>
  <%:ScriptHelper.GenerateGridViewScriptForBreakdown(Url, ControlIdConstants.CreditMemoAwbGrid, Url.Action("CMAwbView", new { transaction = "CMEdit" }))%>
  <%
    }
    else
    {
  %>
  <%: ScriptHelper.GenerateGridEditDeleteScript(Url, ControlIdConstants.CreditMemoAwbGrid, Url.Action("CMAwbEdit", new { transaction = "CMEdit" }), Url.RouteUrl("CGOtransactions", new { action = "CMAwbDelete", controller = "CreditNote" }))%>
  <%
    }
  %>
  <script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
</asp:Content>
