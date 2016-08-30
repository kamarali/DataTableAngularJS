<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<%
  if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.AirportICAOEditOrDelete))
  {%>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchAirportGrid", Url.Action("Edit", "Airport"), Url.Action("Delete", "Airport"), true)%>
<%} %>
<%else
    { %>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchAirportGrid", Url.Action("Edit", "Airport"), Url.Action("Delete", "Airport"), false)%>
<%} %>
<div class="gridContainer">
  <%= Html.Trirand().JQGrid(ViewData.Model, "SearchAirportGrid")%>
</div>
<script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
