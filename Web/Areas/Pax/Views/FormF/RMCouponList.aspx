<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.RejectionMemo>" %>

<%@ Import Namespace="Iata.IS.Model.Pax.Enums" %>

<%@ Import Namespace="Iata.IS.Model.Enums" %>
<asp:Content ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> :: Rejection Memo Coupon List
</asp:Content>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Rejection Memo Coupon List</h1>
  <div>
    <% Html.RenderPartial(Url.Content("~/Areas/Pax/Views/BaseFXF/RMHeaderInfoControl.ascx"), Model); %>
  </div>
  <h2>
    Rejection Memo Coupon List
  </h2>
  <div>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.RMCouponListGrid]); %>
  </div>
  <div class="buttonContainer">
    <% if (Model.Invoice.InvoiceStatusId >= Convert.ToInt32(InvoiceStatusType.ReadyForBilling) && Model.Invoice.InvoiceStatusId < Convert.ToInt32(InvoiceStatusType.ReadyForSubmission))
       {%>
    <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%: Url.Action("RMView", "FormF", new { invoiceId = Model.InvoiceId.Value(), transactionId = Model.Id.Value() } ) %>'" />
    <%}
       else
       {%>
    <% using (Html.BeginForm("RMCouponCreate", "FormF", FormMethod.Get))
       { %>
    <input type="submit" value="Add" class="primaryButton" />
    <%} %>
    <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.RouteUrl("transactions", new {action = "RMEdit", controller = "FormF", invoiceId = Model.InvoiceId.Value(), transactionId = Model.Id.Value() } ) %>'" />
    <%}%>
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <% if (Model.Invoice.InvoiceStatusId >= Convert.ToInt32(InvoiceStatusType.ReadyForBilling) && Model.Invoice.InvoiceStatusId < Convert.ToInt32(InvoiceStatusType.ReadyForSubmission))
     {%>
  <%: ScriptHelper.GenerateGridViewScript(Url, ControlIdConstants.RMCouponGridId, Url.Action("RMCouponView"))%>
  <%}
     else
     {  %>
  <%: ScriptHelper.GenerateGridEditViewDeleteScript(Url, ControlIdConstants.RMCouponGridId, "RMCouponEdit", "RMCouponView", "RMCouponDelete")%>
  <%} %>
  <script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
</asp:Content>
