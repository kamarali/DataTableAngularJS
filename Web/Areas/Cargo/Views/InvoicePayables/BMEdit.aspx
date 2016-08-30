<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Cargo.CargoBillingMemo>" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>

<%@ Import Namespace="Iata.IS.Model.Enums" %>

<%@ Import Namespace="Iata.IS.Model.Pax.Enums" %>

<%@ Import Namespace="Iata.IS.Web.Util" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Cargo ::
  <%: ViewData[ViewDataConstants.BillingType].ToString() %>
  :: Invoice ::
  <%:ViewData[ViewDataConstants.PageMode]%>
  Billing Memo
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<%
    if (ViewData[ViewDataConstants.PageMode].ToString() != PageMode.View)
    {
  %>
  <%: ScriptHelper.GenerateGridEditDeleteScript(Url, ViewDataConstants.BillingMemoAwbGrid, Url.Action("BMAwbEdit", "Invoice", new { invoiceId = Model.InvoiceId.Value(), transactionId = Model.Id.Value(), transaction = "BMEdit" }), Url.RouteUrl("CGOtransactions", new { action = "BMAwbDelete", controller = "Invoice" }))%>
  <%
    }
    else
    {
  %>
  <%:ScriptHelper.GenerateGridViewScriptForBreakdown(Url, ViewDataConstants.BillingMemoAwbGrid, Url.Action("BMAwbView", new { transaction = "BMEdit" }))%>
  <%
    }
  %>
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
    $("#AwbSerialNumberCheckDigit").numeric();
    prepaidBillingCodeId = '<%:Convert.ToInt32(Iata.IS.Model.Cargo.Enums.BillingCode.AWBPrepaid) %>';
    chargeCollectBillingCodeId = '<%:Convert.ToInt32(Iata.IS.Model.Cargo.Enums.BillingCode.AWBChargeCollect) %>';

    // Get previous reasonCode value 
    previousReasonCode = $('#ReasonCode').val();
    registerAutocomplete('OtherChargeCode', 'OtherChargeCode', '<%:Url.Action("GetOtherChargeCodeList", "Data", new { area = "" })%>', 0, true, null);     
    registerAutocomplete('ReasonCode', 'ReasonCode', '<%:Url.Action("GetReasonCodeListForCargoBillingMemo", "Data", new { area="" })%>', 0, false, onReasonCodeChange, '',  <%: Convert.ToInt32(TransactionType.CargoBillingMemo) %>, null, onBlankReasonCode);
    InitializeVatGrid(<%= new JavaScriptSerializer().Serialize(Model.BillingMemoVat) %>);
    InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.Invoice.BilledMemberId, BillingCategoryType.Cgo) %>', '<%: Url.Action("BillingMemoAttachmentDownload","InvoicePayables") %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");    
    setControlAccess('<%:(ViewData[ViewDataConstants.IsExceptionOccurred] != null && (bool)ViewData[ViewDataConstants.IsExceptionOccurred])%>');
   // SetPageToViewModeEx(<%:(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View).ToString().ToLower()%>, '#billingMemoForm');
   
   // If reason code is 6A or 6B i.e. Correspondence, disable reason code field 
   if($('#ReasonCode').val() == "6A" || $('#ReasonCode').val() == "6B"){
    $('#ReasonCode').attr('readonly','readonly');
   }
   else{
    $('#ReasonCode').attr('readonly', false);
   }

 SetPageToViewMode(<%: ((string)ViewData[ViewDataConstants.PageMode] == PageMode.View).ToString().ToLower()%>);
  });

  </script>
   <script src="<%:Url.Content("~/Scripts/Cargo/CargoBillingMemo.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Cargo/VatBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Cargo/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.numeric.js") %>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    <%:ViewData[ViewDataConstants.PageMode] %>
    Billing Memo</h1>
  <div>
    <% Html.RenderPartial(Url.Content("ReadOnlyInvoiceHeaderControl"), Model.Invoice); %>
  </div>
  <% using (Html.BeginForm("BMEdit", "Invoice", FormMethod.Post, new { id = "cargoBillingMemoForm", @class = "validCharacters" }))
     {  %>
     <%: Html.AntiForgeryToken() %>
  <div>
    <% Html.RenderPartial("BMDetailsControl", Model); %>
  </div>
  
  <div class="buttonContainer">
   <input type="submit" value="Save" class="primaryButton ignoredirty"  onclick="javascript:return changeAction('<%: Url.Action("BMEdit","Invoice") %>')"  />
   <input type="submit" value="Save and Add New" class="primaryButton ignoredirty" id="btnSaveAndAddNew" onclick="javascript:return changeAction('<%: Url.Action("BMEditAndAddNew","Invoice", new
                                 {
                                   invoiceId = Model.InvoiceId.Value(),
                                   transaction = "BMEdit",
                                   transactionId = Model.Id.Value()
                                 }) %>')" />
  <input type="submit" value="Add Prepaid AWB" class="secondaryButton" id="btnAddPrepaidAwb" onclick="javascript:location.href='<%:Url.Action("BMAwbPrepaidCreate", "Invoice", new {invoiceId = Model.InvoiceId.Value(), transaction = "BMEdit", transactionId = Model.Id.Value() })%>'; return false;" />
  <input type="submit" value="Add Collect AWB" class="secondaryButton" id="btnAddChargeCollectAwb" onclick="javascript:location.href='<%:Url.Action("BMAwbChargeCollectCreate", "Invoice", new {invoiceId = Model.InvoiceId.Value(), transaction = "BMEdit", transactionId = Model.Id.Value() })%>'; return false;" />
   <%-- <%: Html.LinkButton("Back", Url.Action("BMList", "Invoice", new { invoiceId = Model.InvoiceId }))%>--%>
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
    <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.RouteUrl("CGOtransactions",
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
    <%: Html.LinkButton("Back", Url.Action("BMList", "Invoice", new { invoiceId = Model.InvoiceId }))%>
    <%
      }
    %>
 
  </div>
 <%
      }
        %>
        <h2>
    Billing Memo AWB List</h2>
  <div>
    <%  Html.RenderPartial("GridControl", ViewData[ViewDataConstants.BillingMemoAwbGrid]);%>
  </div>
  <div id="divVatBreakdown" class="hidden">
    <% Html.RenderPartial("BMVatControl", Model.BillingMemoVat);%>
  </div>
  <div class="hidden" id="divAttachment">
    <%
      Html.RenderPartial("BMAttachmentControl", Model);%>
  </div>
 
</asp:Content>