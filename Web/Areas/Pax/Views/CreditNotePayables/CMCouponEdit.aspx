<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.CMCoupon>" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<%@ Import Namespace="System.Security.Policy" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger ::
  <%: ViewData[ViewDataConstants.BillingType].ToString() %>
  :: Credit Memo
  <%: ViewData[ViewDataConstants.PageMode] %>
  :: Coupon
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">
    // Retrieve pagemode
    pageMode = '<%: ViewData[ViewDataConstants.PageMode] %>';
    // Set Edit level
    editLevel = 'CmCouponEdit';
  </script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Pax/CMCoupon.js")%>"></script>
  <script type="text/javascript">
    $(document).ready(function () {
      initializeParentForm('creditMemoCouponForm');
      isOnCouponPage = true;
      // Set variable for the tax breakdown on CM breakdown
      $isCreditMemo = true;

      SetPageToViewMode(<%:(ViewData[ViewDataConstants.PageMode] == PageMode.View).ToString().ToLower()%>); 
      registerAutocomplete('TaxCode', 'TaxCode', '<%:Url.Action("GetTaxCodes", "Data", new { area = "" })%>', 0, true, null);  
      registerAutocomplete('TicketOrFimIssuingAirline', 'TicketOrFimIssuingAirline', '<%:Url.Action("GetTicketIssuingAirlineList", "Data", new { area = "" })%>', 0, true, null);
    <%
    if(ViewData[ViewDataConstants.PageMode] != null &&  ViewData[ViewDataConstants.PageMode].ToString() =="View")
    {%>
      $('input[type=text]').attr('disabled', 'disabled');
      $('select').attr('disabled', 'disabled');
      $("#SaveFormD").hide();
      $('textarea').attr('disabled','disabled');
      $('input[type=checkbox]').attr('disabled','disabled');
      $isOnView = true;
  <%}%> 
      InitializeTaxGrid(<%= new JavaScriptSerializer().Serialize(Model.TaxBreakdown) %>);
      InitializeVatGrid(<%= new JavaScriptSerializer().Serialize(Model.VatBreakdown) %>);
      InitializeProrateSlip(80, 50);
      InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.CreditMemoRecord.Invoice.BilledMemberId, BillingCategoryType.Pax) %>', '<%: Url.Action("CreditMemoCouponAttachmentDownload","CreditNotePayables") %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");
    });
  </script>
  <script type="text/javascript">
    // Set billing type from Viewdata
    billingType = '<%: ViewData[ViewDataConstants.BillingType] %>';
  </script>
  <script src="<%:Url.Content("~/Scripts/Pax/TaxBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/VatBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    <%: ViewData[ViewDataConstants.PageMode] %>
    Credit Memo Coupon</h1>
  <div>
    <%
      Html.RenderPartial("CreditMemoHeaderControl", Model.CreditMemoRecord);%>
  </div>
  <div class="clear">
  </div>
  <h2>
    Credit Memo Coupon Details</h2>
  <%
    using (Html.BeginForm(null, null, FormMethod.Post, new { @id = "creditMemoCouponForm", @class = "validCharacters" }))
    {%>
    <%: Html.AntiForgeryToken() %>
  <div>
    <% Html.RenderPartial("CMCouponDetailsControl", Model);%>
  </div>
  <div class="buttonContainer">
    <input type="submit" value="Save and Add New" class="primaryButton ignoredirty" id="btnSaveAndAddNew" onclick="javascript:changeAction('<%: Url.Action("CreditMemoCouponEdit","CreditNote", new {couponId = Model.Id.Value(), transactionId = Model.CreditMemoId.Value(), transaction = "CMEdit"}) %>')" />
    <input type="submit" value="Save and Duplicate" class="primaryButton ignoredirty" id="btnSaveAndDuplicate" onclick="javascript:changeAction('<%: Url.Action("CreditMemoCouponClone","CreditNote", new {couponId = Model.Id.Value(), transactionId = Model.CreditMemoId.Value(), invoiceId = Model.CreditMemoRecord.InvoiceId.Value(), transaction = "CMEdit"}) %>')" />
    <input type="submit" value="Save and Back to Overview" class="primaryButton ignoredirty" id="SaveAndBackToOverview" onclick="javascript:changeAction('<%: Url.Action("CreditMemoEditAndReturn","CreditNote", new {couponId = Model.Id.Value(), transactionId = Model.CreditMemoId.Value(), invoiceId = Model.CreditMemoRecord.InvoiceId.Value(), transaction = "CMEdit"}) %>')" />
    <%if (ViewData[ViewDataConstants.PageMode] != null && ViewData[ViewDataConstants.PageMode].ToString() == "View")
      {%>
    <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%: Url.Action("CreditMemoView" ,"CreditNotePayables", new {transactionId = Model.CreditMemoRecord.Id.Value(), invoiceId = Model.CreditMemoRecord.InvoiceId.Value()}) %>'" />
    <%}
      else
      { %>
    <%: Html.LinkButton("Back", Url.Action("CreditMemoEdit", "CreditNote", new { transactionId = Model.CreditMemoRecord.Id.Value(), invoiceId = Model.CreditMemoRecord.InvoiceId.Value() }))%>
    <%} %>
  </div>
  <%
    }
  %>
  <div class="hidden" id="divTaxBreakdown">
    <% Html.RenderPartial("CouponTaxControl", Model.TaxBreakdown);%>
  </div>
  <div class="hidden" id="divVatBreakdown">
    <% Html.RenderPartial("CouponVatControl", Model.VatBreakdown);%>
  </div>
  <div class="hidden" id="divAttachment">
    <%
      Html.RenderPartial("CMCouponAttachmentControl", Model);%>
  </div>
  <div id="divProrateSlip" class="hidden">
    <% Html.RenderPartial("ProrateSlipControl", Model.ProrateSlipDetails);%>
  </div>
</asp:Content>
