<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.RMCoupon>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Passenger :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> :: Non-Sampling Invoice :: Rejection Memo Coupon
    Breakdown Prorate Slip
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        Create Rejection Memo Coupon Breakdown Prorate Slip</h1>
    <div>
        <% Html.RenderPartial(Url.Content("ReadOnlyInvoiceHeaderControl"), Model.RejectionMemoRecord.Invoice); %>
    </div>
    <div>
        <% Html.RenderPartial("RMCouponBreakdownProrateSlipControl"); %>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
</asp:Content>
