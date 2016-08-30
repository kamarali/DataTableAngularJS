<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.RMCoupon>" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>

<%@ Import Namespace="Iata.IS.Model.Enums" %>
<asp:Content ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Passenger :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> :: View Rejection Memo Coupon
</asp:Content>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        View Rejection Memo Coupon</h1>
    <div>
        <% Html.RenderPartial(Url.Content("~/Areas/Pax/Views/BaseFXF/RMHeaderInfoControl.ascx"), Model.RejectionMemoRecord); %>
    </div>
    <%using (Html.BeginForm(null, null, FormMethod.Post, new { id = "rejectionMemoCouponBreakdown" }))
      { %>
    <div>
        <% Html.RenderPartial("RMCouponDetailsControl"); %>
    </div>
    <div class="buttonContainer">
        <div>
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.RouteUrl("transactions", new { action = "RMView", invoiceId = Model.RejectionMemoRecord.InvoiceId.Value(), transactionId = Model.RejectionMemoRecord.Id.Value()}) %>'" />
        </div>
        <%} %>
    </div>
    <div class="hidden" id="divTaxBreakdown">
        <% Html.RenderPartial(Url.Content("~/Areas/Pax/Views/BaseFXF/RMCouponTaxControl.ascx"), Model.TaxBreakdown);%>
    </div>
    <div class="hidden" id="divVatBreakdown">
        <% Html.RenderPartial(Url.Content("~/Areas/Pax/Views/BaseFXF/RMCouponVatControl.ascx"), Model.VatBreakdown);%>
    </div>
    <div id="divProrateSlip" class="hidden">
      <% Html.RenderPartial("ProrateSlipControl", Model.ProrateSlipDetails);%>
    </div>
       <div class="hidden" id="divAttachment">
        <%
            Html.RenderPartial("RMCouponFormFAttachmentControl", Model);%>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">
    // Set billing type from Viewdata
    billingType = '<%: ViewData[ViewDataConstants.BillingType] %>';
  </script>
  <script src="<%:Url.Content("~/Scripts/Pax/RMCouponTaxBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/VatBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
    
    <script type="text/javascript"> 
  $(document).ready(function () {
    initializeParentForm('rejectionMemoCouponBreakdown');
    $isOnView = true;
    SetPageToViewMode($isOnView);
    InitializeRMTaxGrid(<%= new JavaScriptSerializer().Serialize(Model.TaxBreakdown) %>);
    InitializeVatGrid(<%= new JavaScriptSerializer().Serialize(Model.VatBreakdown) %>);
    InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.RejectionMemoRecord.Invoice.BilledMemberId, BillingCategoryType.Pax) %>', '<%: Url.Action("RMCouponFormFXFAttachmentDownload","FormFPayables") %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");
    InitializeProrateSlip(80, 50);
  });
    </script>
</asp:Content>
