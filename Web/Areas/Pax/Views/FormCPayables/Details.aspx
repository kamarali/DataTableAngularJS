<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.Sampling.SamplingFormC>" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="TitleContent">
  SIS :: Passenger ::
  <%: ViewData[ViewDataConstants.BillingType].ToString() %>
  :: Sampling Form C Summary/Coupons List
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script src="<%: Url.Content("~/Scripts/Pax/FormC.js") %>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
  <% if (ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
     {%>
  <%: ScriptHelper.GenerateGridViewScriptForBreakdown(Url, ControlIdConstants.FormCCouponGridId, Url.Action("CouponView", "FormCPayables", new
    {
      provisionalBillingMonth = Model.ProvisionalBillingMonth,
      provisionalBillingYear = Model.ProvisionalBillingYear,
      provisionalBillingMemberId = Model.ProvisionalBillingMemberId,
      fromMemberId = Model.FromMemberId,
      listingCurrencyId = Model.ListingCurrencyId,
      invoiceStatusId = Model.InvoiceStatusId
    }))%>
  <%}
     else
     {  %>
  <%: ScriptHelper.GenerateGridEditDeleteScript(Url, ControlIdConstants.FormCCouponGridId, Url.Action("CouponEdit", "FormC", new { invoiceId = Model.Id }), Url.RouteUrl("Pax_default", new { action = "CouponDelete", controller = "FormC", invoiceId = Model.Id }))%>
  <%} %>
</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="MainContent">
  <h1>
    Form C Summary/Coupons</h1>
  <div>
    <% Html.RenderPartial("ReadOnlyHeaderControl", Model); %>
  </div>
  <h2>
    Form C Summary List</h2>
  <div>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.FormCSummaryListGrid]); %>
  </div>
  <div class="clear">
  </div>
  <h2>
    Form C Coupon List</h2>
  <div>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.FormCCouponListGrid]); %>
  </div>
  <div class="buttonContainer">
    <%
      using (Html.BeginForm("", "FormC", new { transactionId = Model.Id }, FormMethod.Post, "FormCDetailsId"))
      { %>
    <!-- If Form C Status is open or Error, then user is allowed to Validate  -->
    <%
        if (!string.IsNullOrEmpty(SessionUtil.FormCSearchCriteria))
        { 
    %>
    <input class="secondaryButton" type="button" value="Back to Form C Search" onclick="javascript:location.href = '<%:SessionUtil.FormCSearchCriteria%>';" />
    <%
        }
    %>
    <% }%>
  </div>
</asp:Content>
