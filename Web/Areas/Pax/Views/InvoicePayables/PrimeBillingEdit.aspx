<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.PrimeCoupon>" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>

<%@ Import Namespace="Iata.IS.Model.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> :: Non-Sampling Invoice :: Edit Prime Billing
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Script" runat="server">
    
  <script type="text/javascript">
  $(document).ready(function () {
    initializeParentForm('primeBillingDetails');

    // Set variable to true if PageMode is "View"
    $isOnView = <%:(ViewData[ViewDataConstants.PageMode] != null ? ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View  : false ).ToString().ToLower() %>;
    InitializeCoupon();
    /* CMP #596: Length of Member Accounting Code to be Increased to 12 
    Desc: Issuing Airlines auto-complete should exclude pure numeric values having size 5 or greater
    Ref: FRS Section 3.4 Table 17 Row 2 */
    registerAutocomplete('TicketOrFimIssuingAirline', 'TicketOrFimIssuingAirline', '<%:Url.Action("GetTicketIssuingAirlineListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);
    registerAutocomplete('TaxCode', 'TaxCode', '<%:Url.Action("GetTaxCodes", "Data", new { area = "" })%>', 0, true, null);
    SetPageToViewMode(<%: ((string)ViewData[ViewDataConstants.PageMode] == PageMode.View).ToString().ToLower()%>);
    SetControlAccess();
    InitializeTaxGrid(<%= new JavaScriptSerializer().Serialize(Model.TaxBreakdown)%>);
    InitializeVatGrid(<%= new JavaScriptSerializer().Serialize(Model.VatBreakdown)%>);
    InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments)%>, '<%:new FileAttachmentHelper().GetValidFileExtention(Model.Invoice.BilledMemberId,BillingCategoryType.Pax)%>', '<%:Url.Action("PrimeBillingAttachmentDownload", "InvoicePayables")%>', "<%:Url.Content("~/Content/Images/busy.gif")%>");
  });
  </script>
  <script type="text/javascript">
    // Set billing type from Viewdata
    billingType = '<%: ViewData[ViewDataConstants.BillingType] %>';

    // If PageMode is Edit and AgreementIndicator has value any of the below disable Original PMI text field, else enable it.
    if ('<%: ViewData[ViewDataConstants.PageMode].ToString() %>' == 'Edit') {
      var agreementIndSupplied = $("#AgreementIndicatorSupplied").val();
      if (agreementIndSupplied == "I" || agreementIndSupplied == 'J' || agreementIndSupplied == 'K' || agreementIndSupplied == 'W' || agreementIndSupplied == 'V' || agreementIndSupplied == 'T') {
        $("#OriginalPmi").attr("readonly", true);
      }
      else {
        $("#OriginalPmi").attr("readonly", false); 
      }
    }
  </script>
  <script src="<%:Url.Content("~/Scripts/Pax/CouponRecord.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/TaxBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/VatBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/multifile_compressed.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/jquery.blockUI.js")%>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    <%:ViewData[ViewDataConstants.PageMode] %>
    Prime Billing</h1>
  <div>
    <%
      Html.RenderPartial(Url.Content("ReadOnlyInvoiceHeaderControl"), Model.Invoice);%>
  </div>
  <%
    using (Html.BeginForm(null, null, FormMethod.Post, new { id = "primeBillingDetails", @class = "validCharacters" }))
    {%>
    <%: Html.AntiForgeryToken() %>
  <div>
    <%
      Html.RenderPartial("PrimeBillingDetailsControl", Model);%>
  </div>
  <div class="buttonContainer">
    <input type="submit" value="Save and Add New" class="primaryButton ignoredirty" id="SaveAndAddNew" onclick="javascript:return changeAction('<%:Url.Action("PrimeBillingEdit", "Invoice", new { invoiceId = Model.InvoiceId, transactionId = Model.Id })%>')" />
    <input type="submit" value="Save and Duplicate" class="primaryButton ignoredirty" id="SaveAndDuplicate" onclick="javascript:return changeAction('<%:Url.Action("PrimeBillingClone", "Invoice", new { invoiceId = Model.InvoiceId, transactionId = Model.Id })%>')" />
    <input type="submit" value="Save and Back to Overview" class="primaryButton ignoredirty" id="SaveAndBackToOverview" onclick="javascript:return changeAction('<%:Url.Action("PrimeBillingEditAndReturn", "Invoice", new { invoiceId = Model.InvoiceId, transactionId = Model.Id })%>')" />
     <%
       if (!string.IsNullOrEmpty(SessionUtil.PaxCorrSearchCriteria) || !string.IsNullOrEmpty(SessionUtil.PaxInvoiceSearchCriteria))
       {%>
    <input class="secondaryButton" type="button" value="Back To Billing History" onclick="javascript:location.href = '<%:Url.Action("Index","BillingHistory", new { back = true })%>'" />
     <%}
       else
       {%>
       <% if (ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View){%>
        <%:Html.LinkButton("Back", Url.Action("PrimeBillingListView", "InvoicePayables", new { invoiceId = Model.InvoiceId }))%>
       <%} else {%>
            <%:Html.LinkButton("Back", Url.Action("PrimeBillingList", "InvoicePayables", new { invoiceId = Model.InvoiceId }))%>
          <%}%>                                          
     <%
       }%>
  </div>
  
  <%}%>

  <div class="hidden" id="divTaxBreakdown">
    <%
      Html.RenderPartial("PrimeBillingTaxControl", Model.TaxBreakdown);%>
  </div>
  <div class="hidden" id="divVatBreakdown">
    <%
      Html.RenderPartial("PrimeBillingVatControl", Model.VatBreakdown);%>
  </div>
  <div class="hidden" id="divAttachment">
    <%
      Html.RenderPartial("PrimeBillingAttachmentControl", Model);%>
  </div>
</asp:Content>
