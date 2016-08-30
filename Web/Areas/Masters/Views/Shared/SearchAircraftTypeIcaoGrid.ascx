<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<%if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.AircraftTypeICAOEditOrDelete))
  {%>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchAircraftTypeIcaoGrid", Url.Action("Edit", "AircraftTypeIcao"), Url.Action("Delete", "AircraftTypeIcao"), true)%>
<%} %>
<%else
  { %>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchAircraftTypeIcaoGrid", Url.Action("Edit", "AircraftTypeIcao"), Url.Action("Delete", "AircraftTypeIcao"), false)%>
<%} %>
<div class="gridContainer">
  <%= Html.Trirand().JQGrid(ViewData.Model, "SearchAircraftTypeIcaoGrid")%>
</div>
<script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
