<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<%
    if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.MISCPaymentStatusEditOrDelete))
  {%>
  <%: ScriptHelper.GenerateGridActiveDeactiveScriptOnly(Url, "SearchInvPaymentStatusGrid", Url.Action("Delete", "InvPaymentStatus"),  true)%>

<%} %>
<%else
    { %>
<%: ScriptHelper.GenerateGridActiveDeactiveScriptOnly(Url, "SearchInvPaymentStatusGrid", Url.Action("Delete", "InvPaymentStatus"),  false)%>
<%} %>
<div class="gridContainer">
  <%= Html.Trirand().JQGrid(ViewData.Model, "SearchInvPaymentStatusGrid")%>
</div>
<script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
