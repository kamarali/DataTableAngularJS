<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.RejectionMemo>" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>

<%@ Import Namespace="Iata.IS.Model.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> :: Non-Sampling Invoice :: Edit Rejection Memo
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
 
 <script type="text/javascript">
  $(document).ready(function () {
    _rmStage = 2;
    initializeParentForm('rejectionMemoForm');
    // Set variable to true if PageMode is "View"
    $isOnView = <%:(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View ? true : false ).ToString().ToLower() %>;
    registerAutocomplete('ReasonCode', 'ReasonCode', '<%:Url.Action("GetReasonCodeListForAutoComplete", "Data", new { area="" })%>', 0, false, onReasonCodeChangeForFormF, '', '<%:Convert.ToInt32(TransactionType.SamplingFormF)%>', null, onBlankReasonCodeForFormF);
    // registerAutocomplete('ReasonCode', 'ReasonCode', '<%:Url.Action("GetReasonCodeList", "Data", new { area="" })%>', 0, true, null, '', '<%:Convert.ToInt32(TransactionType.SamplingFormF)%>');      
    InitializeVatGrid(<%= new JavaScriptSerializer().Serialize(Model.RejectionMemoVat) %>);
    InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.Invoice.BilledMemberId, BillingCategoryType.Pax) %>', '<%: Url.Action("RMFormFXFAttachmentDownload","FormF", new { invoiceId = Model.InvoiceId }) %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");
    InitializeFormFLinking('<%:Model.Invoice.IsFormDEViaIS %>', '<%:Url.Action("GetLinkedFormDEDetails", "FormF") %>', '<%:Model.Invoice.BillingMemberId %>', '<%:Model.Invoice.BilledMemberId %>', '<%:Model.Invoice.ProvisionalBillingMonth%>', '<%:Model.Invoice.ProvisionalBillingYear %>', '<%:Model.Invoice.Id %>');
    InitializeEditRM('<%:ViewData[ViewDataConstants.BreakdownExists]%>');

    // Check whether breakdown exists
    var cpnExist = '<%:ViewData[ViewDataConstants.BreakdownExists]%>';

    // If CouponBreakdown exists OR Total vat amount difference is equal to 0, hide VatBreakdown link, else show 
    if(cpnExist == 'True' || ($('#TotalVatAmountDifference').val() == '0.00')) {
      $('#vatBreakdown').hide();
    } 
    else {
      $('#vatBreakdown').show();
    }

    EnableDisableMemoAmountFieldsInEditMode('<%:ViewData[ViewDataConstants.BreakdownExists]%>');
  });
  </script>
  <script type="text/javascript">
    // Set billing type from Viewdata
    billingType = '<%: ViewData[ViewDataConstants.BillingType] %>';
  </script>
  <script src="<%:Url.Content("~/Scripts/Pax/SamplingRM.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/VatBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
  <%: ScriptHelper.GenerateGridEditDeleteScript(Url, ControlIdConstants.RMCouponGridId, Url.RouteUrl("breakdown", new { action = "RMCouponEdit", transaction = "RMEdit" }), Url.Action("RMCouponDelete", new { transaction = "RMEdit" }))%>
  <script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Edit Rejection Memo</h1>
  <div>
    <%
      Html.RenderPartial(Url.Content("~/Areas/Pax/Views/Shared/SamplingInvoiceHeaderInfoControl.ascx"), Model.Invoice); %>
  </div>
  <div>
    <% using (Html.BeginForm("RMEdit", "FormF", FormMethod.Post, new { id = "rejectionMemoForm", @class = "validCharacters" }))
       {%>
           <%: Html.AntiForgeryToken() %>
        <% Html.RenderPartial(Url.Content("~/Areas/Pax/Views/BaseFXF/RMDetailsControl.ascx"), Model); %>
    <div class="buttonContainer">
      <input type="submit" value="Save Rejection Memo" class="primaryButton ignoredirty" id="SaveButton" />
      <input type="submit" value="Add Rejection Memo Coupon" class="secondaryButton" onclick="javascript:if(checkReasonCode1A('<%:ViewData[ViewDataConstants.BreakdownExists] %>')==true){location.href='<%:Url.Action("RMCouponCreate", "FormF", new {invoiceId = Model.InvoiceId.Value(), transaction = "RMEdit", transactionId = Model.Id.Value() })%>'; return false;}return false;" />      
         <%
        if (!string.IsNullOrEmpty(SessionUtil.PaxCorrSearchCriteria) || !string.IsNullOrEmpty(SessionUtil.PaxInvoiceSearchCriteria))
        {
      %>
        <%: Html.LinkButton("Back To Billing History", Url.Action("Index", "BillingHistory", new { back = true }))%>
      <%
        }
      else
      {
      %>
        <%: Html.LinkButton("Back", Url.Action("Edit", "FormF", new { invoiceId = Model.InvoiceId.Value() }))%>
      <%
        }
       } %>
    </div>
  </div>
  <h2>
    Rejection Memo Coupon List
  </h2>
  <div>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.RMCouponListGrid]); %>
  </div>
  <div id="divVatBreakdown" class="hidden">
    <%
      Html.RenderPartial("~/Areas/Pax/Views/BaseFXF/RMVatControl.ascx", Model.RejectionMemoVat);%>
  </div>
  <div class="hidden" id="divAttachment">
    <%
      Html.RenderPartial("RMFormFAttachmentControl", Model);%>
  </div>
</asp:Content>
