<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<%if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.CgoRMReasonEditOrDelete))
  {%>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchCgoRMReasonAcceptableDiffGrid", Url.Action("Edit", "CgoRMReasonAcceptableDiff"), Url.Action("Delete", "CgoRMReasonAcceptableDiff"), true)%>
<%} %>
<%else
  { %>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchCgoRMReasonAcceptableDiffGrid", Url.Action("Edit", "CgoRMReasonAcceptableDiff"), Url.Action("Delete", "CgoRMReasonAcceptableDiff"), false)%>
<%} %>
<div class="gridContainer">
  <%= Html.Trirand().JQGrid(ViewData.Model, "SearchCgoRMReasonAcceptableDiffGrid")%>
</div>
<script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
