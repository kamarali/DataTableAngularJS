<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<%
  if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.SisMembershipSubStatusEditOrDelete))
  {%>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchSisMemberSubStatusGrid", Url.Action("Edit", "SisMemberSubStatus"), Url.Action("ActiveDeactiveMemberSubStatus", "SisMemberSubStatus"), true)%>
<%} %>
<%else
    { %>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchSisMemberSubStatusGrid", Url.Action("Edit", "SisMemberSubStatus"), Url.Action("ActiveDeactiveMemberSubStatus", "SisMemberSubStatus"), false)%>
<%} %>
<div class="gridContainer">
  <%= Html.Trirand().JQGrid(ViewData.Model, "SearchSisMemberSubStatusGrid")%>
</div>
<script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
