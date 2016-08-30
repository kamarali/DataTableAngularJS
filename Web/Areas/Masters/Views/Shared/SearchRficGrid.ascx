<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<%if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.RFICEditOrDelete))
  {%>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchRficGrid", Url.Action("Edit", "Rfic"), Url.Action("Delete", "Rfic"), true)%>
<%} %>
<%else
  { %>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchRficGrid", Url.Action("Edit", "Rfic"), Url.Action("Delete", "Rfic"), false)%>
<%} %>
<div class="gridContainer">
  <%= Html.Trirand().JQGrid(ViewData.Model, "SearchRficGrid")%>
</div>
<script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
