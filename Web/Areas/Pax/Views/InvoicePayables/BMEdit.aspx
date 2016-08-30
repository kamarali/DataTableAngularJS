<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.BillingMemo>" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>

<%@ Import Namespace="Iata.IS.Model.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger ::
  <%: ViewData[ViewDataConstants.BillingType].ToString() %>
  :: Non-Sampling Invoice ::
  <%:ViewData[ViewDataConstants.PageMode]%>
  Billing Memo
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">
    // Get pageMode
    pageMode = '<%: ViewData[ViewDataConstants.PageMode] %>';
    // Get whether coupon breakdown exists
    couponBreakdownExists = '<%: ViewData[ViewDataConstants.BreakdownExists] %>';
    // Set variable to true if PageMode is "View"
    $isOnView = <%:(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View ? true : false ).ToString().ToLower() %>;
    // Set billing type from Viewdata
    billingType = '<%: ViewData[ViewDataConstants.BillingType] %>';
  </script>
  <script src="<%:Url.Content("~/Scripts/Pax/BillingMemo.js")%>" type="text/javascript"></script>
  <script type="text/javascript">
  // Declare variable to capture reasonCode before editing
  var previousReasonCode = '';
  $(document).ready(function () {
    // Get previous reasonCode value 
    previousReasonCode = $('#ReasonCode').val();
    //registerAutocomplete('ReasonCode', 'ReasonCode', '<%:Url.Action("GetReasonCodeListForAutoComplete", "Data", new { area="" })%>', 0, false, onReasonCodeChange, '', '<%:Convert.ToInt32(TransactionType.BillingMemo)%>', null, onBlankReasonCode);
    registerAutocomplete('ReasonCode', 'ReasonCode', '<%:Url.Action("GetReasonCodeListForBillingMemo", "Data", new { area="" })%>', 0, false, onReasonCodeChange, '', null, null, onBlankReasonCode);
    registerAutocomplete('TaxCode', 'TaxCode', '<%:Url.Action("GetTaxCodes", "Data", new { area = "" })%>', 0, true, null);
    InitializeVatGrid(<%= new JavaScriptSerializer().Serialize(Model.VatBreakdown) %>);
    InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.Invoice.BilledMemberId, BillingCategoryType.Pax) %>', '<%: Url.Action("BillingMemoAttachmentDownload","InvoicePayables") %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");    
    setControlAccess('<%:(ViewData[ViewDataConstants.IsExceptionOccurred] != null && (bool)ViewData[ViewDataConstants.IsExceptionOccurred])%>');
    $("#SourceCodeId").autocomplete({ disabled: true });
    SetPageToViewModeEx(<%:(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View).ToString().ToLower()%>, '#billingMemoForm');
  });

  </script>
  <script src="<%:Url.Content("~/Scripts/Pax/VatBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    <%:ViewData[ViewDataConstants.PageMode] %>
    Billing Memo</h1>
  <div>
    <% Html.RenderPartial(Url.Content("ReadOnlyInvoiceHeaderControl"), Model.Invoice); %>
  </div>
  <% using (Html.BeginForm("BMEdit", "Invoice", FormMethod.Post, new { id = "billingMemoForm", @class = "validCharacters" }))
     {  %>
     <%: Html.AntiForgeryToken() %>
  <div>
    <% Html.RenderPartial("BMDetailsControl", Model); %>
  </div>
  <div class="buttonContainer">
    <input type="submit" value="Save" class="primaryButton ignoredirty" />
    <input type="submit" value="Save and Add New" class="primaryButton ignoredirty" id="btnSaveAndAddNew" onclick="javascript:return changeAction('<%: Url.Action("BMEditAndAddNew","Invoice", new
                                 {
                                   invoiceId = Model.InvoiceId.Value(),
                                   transaction = "BMEdit",
                                   transactionId = Model.Id.Value()
                                 }) %>')" />
    <input type="submit" value="Add Billing Memo Coupon" class="secondaryButton" id="btnAdd" onclick="javascript:location.href='<%:Url.Action("BMCouponCreate", "Invoice", new {invoiceId = Model.InvoiceId.Value(), transaction = "BMEdit", transactionId = Model.Id.Value() })%>'; return false;" />
    <%
      if (ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
      {
        if (!string.IsNullOrEmpty(SessionUtil.PaxCorrSearchCriteria) || !string.IsNullOrEmpty(SessionUtil.PaxInvoiceSearchCriteria))
        {
    %>
    <%: Html.LinkButton("Back To Billing History", Url.Action("Index", "BillingHistory", new { back = true }))%>
    <%
}
        else
        {
    %>
    <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.RouteUrl("transactions",
                                         new
                                           {
                                             action = "BMListView",
                                             controller = "InvoicePayables",
                                             invoiceId = Model.InvoiceId
                                           })%>';" />
    <%
      }
      }
      else
      {
    %>
    <%: Html.LinkButton("Back", Url.RouteUrl("transactions", new { action = "BMList", controller = "InvoicePayables", invoiceId = Model.InvoiceId.Value() }))%>
    <%
      }
    %>
  </div>
  <%} %>
  <h2>
    Billing Memo Coupon List</h2>
  <div>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.BillingMemoCouponGrid]); %>
    <%
      if (ViewData[ViewDataConstants.PageMode].ToString() != PageMode.View)
      {
    %>
    <%: ScriptHelper.GenerateGridEditDeleteScript(Url, ViewDataConstants.BillingMemoCouponGrid, Url.RouteUrl("breakdown", new { action = "BMCouponEdit", transaction = "BMEdit" }), Url.RouteUrl("breakdown", new { action = "BMCouponDelete", transaction = "BMEdit" }))%>
    <%
      }
      else
      {
    %>
    <%:ScriptHelper.GenerateGridViewScriptForBreakdown(Url, ViewDataConstants.BillingMemoCouponGrid, Url.Action("BMCouponView", new { transaction = "BMEdit" }))%>
    <%
      }
    %>
  </div>
  <div id="divVatBreakdown" class="hidden">
    <%
      Html.RenderPartial("BMVatControl", Model.VatBreakdown);%>
  </div>
  <div class="hidden" id="divAttachment">
    <%
      Html.RenderPartial("BillingMemoAttachmentControl", Model);%>
  </div>
</asp:Content>
