<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.RMCoupon>" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger ::
  <%: ViewData[ViewDataConstants.BillingType].ToString() %>
  :: Edit Rejection Memo Coupon
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Edit Rejection Memo Coupon</h1>
  <div>
    <% Html.RenderPartial(Url.Content("~/Areas/Pax/Views/BaseFXF/RMHeaderInfoControl.ascx"), Model.RejectionMemoRecord); %>
  </div>
  <%using (Html.BeginForm(null, null, FormMethod.Post, new { id = "rejectionMemoCouponBreakdown" }))
      { %>
      <%: Html.AntiForgeryToken() %>
  <div>
    <% Html.RenderPartial("RMCouponDetailsControl"); %>
  </div>
  <div class="buttonContainer">
    <div>
      <input type="submit" value="Save" class="primaryButton ignoredirty" id="SaveAndAddNew" onclick="javascript:return changeAction('<%:Url.Action("RMCouponEdit", "FormXF", new { couponId = Model.Id })%>');" />
      <%: Html.LinkButton("Back", Url.Action("RMEdit", "FormXF", new { invoiceId = Model.RejectionMemoRecord.InvoiceId.Value(), transactionId = Model.RejectionMemoId.Value()}))%>
    </div>
    <%} %>
  </div>
  <div class="hidden" id="divTaxBreakdown">
    <% Html.RenderPartial(Url.Content("~/Areas/Pax/Views/BaseFXF/RMCouponTaxControl.ascx"), Model.TaxBreakdown);%>
  </div>
  <div class="hidden" id="divVatBreakdown">
    <% Html.RenderPartial(Url.Content("~/Areas/Pax/Views/BaseFXF/RMCouponVatControl.ascx"), Model.VatBreakdown);%>
  </div>
  <div class="hidden" id="divAttachment">
    <%Html.RenderPartial("RMCouponFormXFAttachmentControl", Model);%>
  </div>
  <div id="divProrateSlip" class="hidden">
    <% Html.RenderPartial("ProrateSlipControl", Model.ProrateSlipDetails);%>
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">
    // Set billing type from Viewdata
    billingType = '<%: ViewData[ViewDataConstants.BillingType] %>';
  </script>
  <script src="<%: Url.Content("~/Scripts/Pax/RejectionMemoCouponBreakdown.js") %>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/RMCouponTaxBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/VatBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
  <script type="text/javascript"> 
  $(document).ready(function () {
    initializeParentForm('rejectionMemoCouponBreakdown');
    registerAutocomplete('TaxCode', 'TaxCode', '<%:Url.Action("GetTaxCodes", "Data", new { area = "" })%>', 0, true, null);
    /* CMP #596: Length of Member Accounting Code to be Increased to 12 
    Desc: Issuing Airlines auto-complete should exclude pure numeric values having size 5 or greater
    Ref: FRS Section 3.4 Table 17 Row 9 */
    registerAutocomplete('TicketOrFimIssuingAirline', 'TicketOrFimIssuingAirline', '<%:Url.Action("GetTicketIssuingAirlineListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);
    
    SetStage('<%:Model.RejectionMemoRecord.RejectionStage %>');
    InitializeRMTaxGrid(<%= new JavaScriptSerializer().Serialize(Model.TaxBreakdown) %>);
    InitializeVatGrid(<%= new JavaScriptSerializer().Serialize(Model.VatBreakdown) %>);
    InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.RejectionMemoRecord.Invoice.BilledMemberId, BillingCategoryType.Pax) %>','<%: Url.Action("RMCouponFormFXFAttachmentDownload","FormXF") %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");
    InitializeProrateSlip(80, 50);
    InitializeLinkingFieldsInEditMode('<%:Model.RejectionMemoRecord.IsLinkingSuccessful %>', '<%:Model.RejectionMemoRecord.SourceCodeId %>');
  });
  </script>
</asp:Content>
