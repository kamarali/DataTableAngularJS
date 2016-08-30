<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<%
  if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.ReasonCodeEditOrDelete))
  {%>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchReasonCodeGrid", Url.Action("Edit", "ReasonCode"), Url.Action("Delete", "ReasonCode"), true)%>
<%} %>
<%else
    { %>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchReasonCodeGrid", Url.Action("Edit", "ReasonCode"), Url.Action("Delete", "ReasonCode"), false)%>
<%} %>
<div class="gridContainer">
  <%= Html.Trirand().JQGrid(ViewData.Model, "SearchReasonCodeGrid")%>
</div>
<script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
