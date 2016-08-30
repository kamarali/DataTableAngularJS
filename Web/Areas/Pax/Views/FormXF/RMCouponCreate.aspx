<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.RMCoupon>" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>

<%@ Import Namespace="Iata.IS.Model.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> :: Create Rejection Memo Coupon
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Create Rejection Memo Coupon</h1>
  <div>
    <% Html.RenderPartial(Url.Content("~/Areas/Pax/Views/BaseFXF/RMHeaderInfoControl.ascx"), Model.RejectionMemoRecord); %>
  </div>
  <%using (Html.BeginForm(null, null, FormMethod.Post, new { id = "rejectionMemoCouponBreakdown", @class = "validCharacters" }))
    { %>
    <%: Html.AntiForgeryToken() %>
  <div>
    <% Html.RenderPartial("RMCouponDetailsControl", Model); %>
  </div>
  <div class="buttonContainer">
    <div>
      <input type="submit" value="Save" class="primaryButton ignoredirty" id="btnSave" onclick="javascript:return changeAction('<%: Url.Action("RMCouponCreate","FormXF") %>');" />
      <%: Html.LinkButton("Back", Url.Action("RMEdit", "FormXF", new { invoiceId = Model.RejectionMemoRecord.InvoiceId.Value(), transactionId = Model.RejectionMemoRecord.Id.Value() }))%>
    </div>
    <%} %>
  </div>
  <div class="hidden" id="divTaxBreakdown">
    <% Html.RenderPartial("~/Areas/Pax/Views/BaseFXF/RMCouponTaxControl.ascx", Model.TaxBreakdown);%>
  </div>
  <div class="hidden" id="divVatBreakdown">
    <% Html.RenderPartial(Url.Content("~/Areas/Pax/Views/BaseFXF/RMCouponVatControl.ascx"), Model.VatBreakdown);%>
  </div>
  <div class="hidden" id="divAttachment">
    <%
      Html.RenderPartial("RMCouponFormXFAttachmentControl", Model);%>
  </div>
    <div class="hidden">
     <% Html.RenderPartial("RMLinkingDuplicateRecordControl");%>
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
  <script src="<%:Url.Content("~/Scripts/Pax/RMCouponTaxBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/VatBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/Pax/RejectionMemoCouponBreakdown.js") %>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
  
  <script type="text/javascript">
  $(document).ready(function () {
    initializeParentForm('rejectionMemoCouponBreakdown');
    // Set variable to true if PageMode is "View"
    $isOnView = <%:(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View ? true : false ).ToString().ToLower() %>;
    registerAutocomplete('TaxCode', 'TaxCode', '<%:Url.Action("GetTaxCodes", "Data", new { area = "" })%>', 0, true, null);
    /* CMP #596: Length of Member Accounting Code to be Increased to 12 
    Desc: Issuing Airlines auto-complete should exclude pure numeric values having size 5 or greater
    Ref: FRS Section 3.4 Table 17 Row 9 */
    registerAutocomplete('TicketOrFimIssuingAirline', 'TicketOrFimIssuingAirline', '<%:Url.Action("GetTicketIssuingAirlineListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);
    SetStage('<%:Model.RejectionMemoRecord.RejectionStage %>');
   <%
     if(ViewData["PageMode"] == null || ViewData[ViewDataConstants.PageMode].ToString() != "Clone")
     {
%>
    InitializeCreateRMCoupon('create');
    <%
     }%>
     
      // Display Serial Number textbox only in Edit mode and hide in Create mode
      if(<%:((string)ViewData[ViewDataConstants.PageMode] == PageMode.Create).ToString().ToLower()%>){
        $("#serialNoDiv").addClass('hidden');  
      }

     InitializeRMTaxGrid(<%= new JavaScriptSerializer().Serialize(Model.TaxBreakdown) %>);
     InitializeVatGrid(<%= new JavaScriptSerializer().Serialize(Model.VatBreakdown) %>);
      // $("#AttachmentIndicatorOriginal").val("No");
     InitializeProrateSlip(80, 50);
      
     InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments)%>, '<%:new FileAttachmentHelper().GetValidFileExtention(Model.RejectionMemoRecord.Invoice.BilledMemberId, BillingCategoryType.Pax)%>', '<%:Url.Action("RMCouponFormFXFAttachmentDownload", "FormXF")%>', "<%:Url.Content("~/Content/Images/busy.gif")%>");
     InitialiseLinking('<%: Url.Action("GetRMCouponBreakdownDetails", "FormXF", "Pax")%>', '<%: Url.Action("GetRMCouponBreakdownSingleRecordDetails", "FormXF", "Pax")%>', '<%: Model.RejectionMemoRecord.Id %>', '<%: Model.RejectionMemoRecord.Invoice.BilledMemberId %>', '<%: SessionUtil.MemberId %>', '<%: Model.RejectionMemoRecord.IsLinkingSuccessful%>', '<%: Model.RejectionMemoRecord.SourceCodeId%>', '<%: ViewData[ViewDataConstants.IsExceptionOccurred] %>');
  });
  </script>
</asp:Content>
