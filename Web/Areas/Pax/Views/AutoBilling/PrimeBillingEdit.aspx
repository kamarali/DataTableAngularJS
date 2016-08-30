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
    registerAutocomplete('TicketOrFimIssuingAirline', 'TicketOrFimIssuingAirline', '<%:Url.Action("GetTicketIssuingAirlineList", "Data", new { area = "" })%>', 0, true, null);
    registerAutocomplete('TaxCode', 'TaxCode', '<%:Url.Action("GetTaxCodes", "Data", new { area = "" })%>', 0, true, null);
    SetPageToViewMode(<%: ((string)ViewData[ViewDataConstants.PageMode] == PageMode.View).ToString().ToLower()%>);
    SetControlAccess();
    InitializeTaxGrid(<%= new JavaScriptSerializer().Serialize(Model.TaxBreakdown)%>);
    
    InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments)%>, '<%:new FileAttachmentHelper().GetValidFileExtention(Model.Invoice.BilledMemberId,BillingCategoryType.Pax)%>', '<%:Url.Action("PrimeBillingAttachmentDownload", "AutoBilling")%>', "<%:Url.Content("~/Content/Images/busy.gif")%>");
  });
  </script>
  <script type="text/javascript">
    // Set billing type from Viewdata
    billingType = '<%: ViewData[ViewDataConstants.BillingType] %>';
  </script>
  <script src="<%:Url.Content("~/Scripts/Pax/CouponRecord.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/TaxBreakdown.js")%>" type="text/javascript"></script>
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
    <input type="submit" value="Save" class="primaryButton ignoredirty" id="SaveAndAddNew" onclick="javascript:return changeAction('<%:Url.Action("PrimeBillingEdit", "AutoBilling", new { invoiceId = Model.InvoiceId, transactionId = Model.Id })%>')" />
     <%
       if (!string.IsNullOrEmpty(SessionUtil.PaxCorrSearchCriteria) || !string.IsNullOrEmpty(SessionUtil.PaxInvoiceSearchCriteria))
       {%>
    <input class="secondaryButton" type="button" value="Back To Billing History" onclick="javascript:location.href = '<%:Url.Action("Index","BillingHistory", new { back = true })%>'" />
     <%}
       else
       {%>
            <%:Html.LinkButton("Back", Url.Action("Index", "AutoBilling", new { backButtonClicked = "true" }))%> 
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
