<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<%
  if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.OnBehalfInvoiceSetupEditOrDelete))
  {%>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchOnBehalfInvoiceSetupGrid", Url.Action("Edit", "OnBehalfInvoiceSetup"), Url.Action("Delete", "OnBehalfInvoiceSetup"), true)%>
<%} %>
<%else
    { %>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchOnBehalfInvoiceSetupGrid", Url.Action("Edit", "OnBehalfInvoiceSetup"), Url.Action("Delete", "OnBehalfInvoiceSetup"), false)%>
<%} %>
<div class="gridContainer">
  <%= Html.Trirand().JQGrid(ViewData.Model, "SearchOnBehalfInvoiceSetupGrid")%>
</div>
<script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
