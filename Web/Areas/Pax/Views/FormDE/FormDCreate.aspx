<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.Sampling.SamplingFormDRecord>" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<asp:Content ID="CreateFormDContent" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger ::
  <%: ViewData[ViewDataConstants.BillingType].ToString() %>
  :: Create Sampling Form D Details
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Create Sampling Form D</h1>
  <% using (Html.BeginForm(null, null, FormMethod.Post, new { id = "formDCoupon", @class = "validCharacters" }))
     { %>
     <%: Html.AntiForgeryToken() %>
  <div>
    <% Html.RenderPartial("SamplingInvoiceHeaderInfoControl", Model.Invoice); %>
  </div>
  <div>
    <% Html.RenderPartial("FormDDetailsControl", Model); %>
  </div>
  <div class="buttonContainer">
    <input type="submit" value="Save and Add New" class="primaryButton ignoredirty" id="SaveAndAddNew" onclick="return changeAction('<%:Url.Action("FormDCreate", "FormDE")%>')" />
    <input type="submit" value="Save and Duplicate" class="primaryButton ignoredirty" id="SaveAndDuplicate" onclick="return changeAction('<%:Url.Action("FormDDuplicate", "FormDE")%>')" />
    <input type="submit" value="Save and Back to Overview" class="primaryButton ignoredirty" id="SaveAndBackToOverview" onclick="return changeAction('<%:Url.Action("FormDCreateAndReturn", "FormDE")%>')" />
    <%: Html.LinkButton("Back", Url.Action("Edit", "FormDE", new { invoiceId = Model.Invoice.Id.Value() }))%>
    <%} %>
  </div>
  <div id="divTaxBreakdown" class="hidden">
    <% Html.RenderPartial("FormDTaxControl"); %>
  </div>
  <div id="divVatBreakdown" class="hidden">
    <% Html.RenderPartial("FormDVatControl"); %>
  </div>
  <div id="divAttachment" class="hidden">
    <% Html.RenderPartial("SamplingFormDAttachmentControl", Model); %>
  </div>
  <div id="divProrateSlip" class="hidden">
    <% Html.RenderPartial("ProrateSlipControl", Model.ProrateSlipDetails);%>
  </div>
  <div class="hidden" id="linkedCoupons">
    <h2>
      Below coupons were found from provisional invoices. Please select one.</h2>
    <div>
      <table id="linkedCouponsGrid">
      </table>
    </div>
    <div class="buttonContainer">
      <input class="secondaryButton" type="button" value="OK" onclick="onLinkingDialogClose()" />
    </div>
  </div>
</asp:Content>
<asp:Content ID="CreateFormDScriptContent" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">

    $(document).ready(function () {
      initializeParentForm('formDCoupon');
      /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: Issuing Airlines auto-complete should exclude pure numeric values having size 5 or greater
        Ref: FRS Section 3.4 Table 17 Row 7 */
      registerAutocomplete('TicketIssuingAirline', 'TicketIssuingAirline', '<%:Url.Action("GetTicketIssuingAirlineListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);
      registerAutocomplete('TaxCode', 'TaxCode', '<%:Url.Action("GetTaxCodes", "Data", new { area="" })%>', 0, true, null);
      $("#AttachmentIndicatorOriginal").val("No");
      InitializeTaxGrid(<%= new JavaScriptSerializer().Serialize(Model.TaxBreakdown) %>);
      InitializeVatGrid(<%= new JavaScriptSerializer().Serialize(Model.VatBreakdown) %>);
      InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.Invoice.BilledMemberId, BillingCategoryType.Pax) %>', '<%: Url.Action("FormDAttachmentDownload","FormDE") %>', '<%:Url.Content("~/Content/Images/busy.gif")%>');
      InitializeLinking('<%: Model.Invoice.IsFormABViaIS %>', '<%: Model.InvoiceId %>', '<%:Url.Action("GetFormDLinkedCouponDetails", "FormDE") %>');
      InitializeProrateSlip(80, 50);
      
      if ('<%: Convert.ToBoolean(ViewData[ViewDataConstants.IsPostback])%>' != 'True') {
        if ('<%: (string)ViewData[ViewDataConstants.PageMode] == PageMode.Clone %>' == 'True') {
          $('#CouponNumber').focus();
          $("#CouponNumber").val('');
          $("#TicketDocNumber").val('');
          $("#BatchNumberOfProvisionalInvoice").val('');
          $("#RecordSeqNumberOfProvisionalInvoice").val('');
        }
        else {
           //SCP65997 
          //clearAmountDefaultZeroValue();
          if ('<%: ViewData[ViewDataConstants.SamplingFormDRecord] != null%>' == 'True') {
            SetPageModeToCreateMode('<%:((string)ViewData[ViewDataConstants.PageMode] == PageMode.Create).ToString().ToLower()%>');
            $("#SourceCodeId").val('<%: ViewData[ViewDataConstants.SamplingFormDRecord]%>'.split('-')[0]);
            $("#TicketIssuingAirline").val('<%: ViewData[ViewDataConstants.SamplingFormDRecord]%>'.split('-')[1]);
            $("#CouponNumber").val('');
            $("#TicketDocNumber").val('');
            $('#CouponNumber').focus();
          }
          else {
            SetPageModeToCreateMode('<%:((string)ViewData[ViewDataConstants.PageMode] == PageMode.Create).ToString().ToLower()%>');
          }
        }
      }
      else {
         //SCP65997 
        //clearAmountDefaultZeroValue();
      }

    }); 
  </script>
  <script type="text/javascript">
    // Set billing type from Viewdata
    billingType = '<%: ViewData[ViewDataConstants.BillingType] %>';
  </script>
  <script src="<%:Url.Content("~/Scripts/Pax/SamplingFormD.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/TaxBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/VatBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
</asp:Content>
