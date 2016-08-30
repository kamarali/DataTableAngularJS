<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<%if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.PaxRMReasonEditOrDelete))
  {%>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchRMReasonAcceptableDiffGrid", Url.Action("Edit", "RMReasonAcceptableDiff"), Url.Action("Delete", "RMReasonAcceptableDiff"), true)%>
<%} %>
<%else
  { %>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchRMReasonAcceptableDiffGrid", Url.Action("Edit", "RMReasonAcceptableDiff"), Url.Action("Delete", "RMReasonAcceptableDiff"), false)%>
<%} %>
<div class="gridContainer">
  <%= Html.Trirand().JQGrid(ViewData.Model, "SearchRMReasonAcceptableDiffGrid")%>
</div>
<script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
