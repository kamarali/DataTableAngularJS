<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<%if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.UomCodeEditOrDelete))
  {%>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchUomCodeGrid", Url.Action("Edit", "UomCode"), Url.Action("Delete", "UomCode"), true)%>
<%} %>
<%else
  { %>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchUomCodeGrid", Url.Action("Edit", "UomCode"), Url.Action("Delete", "UomCode"), false)%>
<%} %>
<div class="gridContainer">
  <%= Html.Trirand().JQGrid(ViewData.Model, "SearchUomCodeGrid")%>
</div>
<script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
