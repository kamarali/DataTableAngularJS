<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.BMCoupon>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> :: Non-Sampling Invoice :: Billing Memo Coupon Breakdown
  Prorate Slip
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Create Billing Memo Coupon Breakdown Prorate Slip</h1>
  <div>
    <% Html.RenderPartial(Url.Content("ReadOnlyInvoiceHeaderControl")); %>
  </div>
  <div>
    <label for="prorateSlip">
      Prorate Slip:
    </label>
    <%: Html.TextAreaFor(billingCouponBreakdown => billingCouponBreakdown.ProrateSlipDetails, 20, 160, ScrollBars.Both)%>
  </div>
</asp:Content>
