<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.BillingMemo>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> :: Non-Sampling Invoice :: Billing Memo Coupon List
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Billing Memo Coupon List</h1>
  <div>
    <% Html.RenderPartial("ReadOnlyBillingMemoHeaderControl", Model); %>
  </div>
  <h2>
    Billing Memo Coupon List</h2>
  <div>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.BillingMemoCouponGrid]); %>
  </div>
  <div class="buttonContainer">
    <% using (Html.BeginForm("BMCouponCreate", "Invoice", new { invoiceId = Model.InvoiceId.Value(), transactionId = Model.Id.Value() }, FormMethod.Get))
       {  %>
    <input type="submit" value="Add" class="primaryButton" id="btnAdd" />
    <% } %>
    <%
      if (ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
      {
    %>
    <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.RouteUrl("transactions", new { action ="BMView", controller = "Invoice", invoiceId = Model.InvoiceId.Value(), transactionId = Model.Id.Value() })%>';" />
    <%
      }
      else
      {
    %>
    <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.RouteUrl("transactions", new { action ="BMEdit", controller = "Invoice", invoiceId = Model.InvoiceId.Value(), transactionId = Model.Id.Value() })%>';" />
    <%
      }
    %>
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
  <%
    if (ViewData[ViewDataConstants.PageMode].ToString() != PageMode.View)
    {
  %>
  <%: ScriptHelper.GenerateGridEditViewDeleteScript(Url, ViewDataConstants.BillingMemoCouponGrid, "BMCouponEdit", "BMCouponView", "BMCouponDelete")%>
  <%
    }
    else
    {
  %>
  <%:ScriptHelper.GenerateGridViewScript(Url, ViewDataConstants.BillingMemoCouponGrid, Url.Action("BMCouponView"))%>
  <%
    }
  %>
</asp:Content>
