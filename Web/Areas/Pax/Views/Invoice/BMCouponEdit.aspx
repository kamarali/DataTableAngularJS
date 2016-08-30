<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.BMCoupon>" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger ::
  <%: ViewData[ViewDataConstants.BillingType].ToString() %>
  :: Non-Sampling Invoice ::
  <%:ViewData[ViewDataConstants.PageMode] %>
  Billing Memo Coupon
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">
    $(document).ready(function () {
      initializeParentForm('billingMemoCouponBreakdownDetails');
      // Set variable to true if PageMode is "View"
      $isOnView = <%:(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View ? true : false ).ToString().ToLower() %>;
      SetPageToViewMode(<%:(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View).ToString().ToLower()%>);
      /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: Issuing Airlines auto-complete should exclude pure numeric values having size 5 or greater
        Ref: FRS Section 3.4 Table 17 Row 4 */
      registerAutocomplete('TicketOrFimIssuingAirline', 'TicketOrFimIssuingAirline', '<%:Url.Action("GetTicketIssuingAirlineListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);
      registerAutocomplete('TaxCode', 'TaxCode', '<%:Url.Action("GetTaxCodes", "Data", new { area = "" })%>', 0, true, null);
      InitializeTaxGrid(<%= new JavaScriptSerializer().Serialize(Model.TaxBreakdown) %>);
      InitializeVatGrid(<%= new JavaScriptSerializer().Serialize(Model.VatBreakdown) %>);
      InitializeProrateSlip(80, 50);
      InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.BillingMemo.Invoice.BilledMemberId, BillingCategoryType.Pax) %>', '<%: Url.Action("BMCouponAttachmentDownload","Invoice") %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");
    });
  </script>
  <script type="text/javascript">
    // Set billing type from Viewdata
    billingType = '<%: ViewData[ViewDataConstants.BillingType] %>';
  </script>
  <script src="<%:Url.Content("~/Scripts/Pax/TaxBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/VatBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/BillingMemoCouponBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    <%=ViewData[ViewDataConstants.PageMode] %>
    Billing Memo Coupon</h1>
  <div>
    <% Html.RenderPartial(Url.Content("ReadOnlyBillingMemoHeaderControl"), Model.BillingMemo); %>
  </div>
  <% using (Html.BeginForm(null, null, FormMethod.Post, new { id = "billingMemoCouponBreakdownDetails" }))
       {  %>
       <%: Html.AntiForgeryToken() %>
  <div>
    <% Html.RenderPartial("BMCouponBreakdownDetailsControl", Model); %>
  </div>
  <div class="buttonContainer">
    <input type="submit" value="Save and Add New" class="primaryButton ignoredirty" id="btnSaveAndAddNew" onclick="javascript:changeAction('<%: Url.Action("BMCouponEdit","Invoice", new { transactionId = Model.BillingMemoId.Value(),  invoiceId= Model.BillingMemo.InvoiceId.Value(), transaction="BMEdit",couponId = Model.Id.Value() }) %>')" />
    <input type="submit" value="Save and Duplicate" class="primaryButton ignoredirty" id="btnSaveAndDuplicate" onclick="javascript:changeAction('<%: Url.Action("BMCouponClone","Invoice", new { transactionId = Model.BillingMemoId.Value(),  invoiceId= Model.BillingMemo.InvoiceId.Value(), transaction="BMEdit",couponId = Model.Id.Value() }) %>')" />
    <input type="submit" value="Save and Back to Overview" class="primaryButton ignoredirty" id="SaveAndBackToOverview" onclick="javascript:changeAction('<%: Url.Action("BMCouponEditAndReturn","Invoice", new { transactionId = Model.BillingMemoId.Value(),  invoiceId= Model.BillingMemo.InvoiceId.Value(), transaction="BMEdit",couponId = Model.Id.Value() }) %>')" />
    <input class="secondaryButton" type="submit" id="btnAddNew" value="Add New Coupon" onclick="javascript:location.href='<%:Url.Action("BMCouponCreate", "Invoice", new {invoiceId = Model.BillingMemo.InvoiceId.Value(), transaction = "BMEdit", transactionId = Model.BillingMemo.Id.Value() })%>'; return false;" />
    <%if (ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
{%>
    <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.RouteUrl("transactions",
                                 new
                                   {
                                     action = "BMView",
                                     invoiceId = Model.BillingMemo.InvoiceId.Value(),
                                     transactionId = Model.BillingMemo.Id.Value()
                                   })%>';" />
    <%
}else
{%>
    <%: Html.LinkButton("Back", Url.RouteUrl("transactions", new { action = "BMEdit", invoiceId = Model.BillingMemo.InvoiceId.Value(), transactionId = Model.BillingMemo.Id.Value()}))%>
    <%
}%>
  </div>
  <%
        }%>
  <div class="hidden" id="divTaxBreakdown">
    <% Html.RenderPartial("BMTaxControl", Model.TaxBreakdown);%>
  </div>
  <div class="hidden" id="divVatBreakdown">
    <% Html.RenderPartial("BMCouponVatControl", Model.VatBreakdown);%>
  </div>
  <div class="hidden" id="divAttachment">
    <%
            Html.RenderPartial("BMCouponAttachmentControl", Model);%>
  </div>
    <div id="divProrateSlip" class="hidden">
    <% Html.RenderPartial("ProrateSlipControl", Model.ProrateSlipDetails);%>
  </div>
</asp:Content>
