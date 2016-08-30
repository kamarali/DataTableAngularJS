<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<%
  if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.SisMembershipStatusEditOrDelete))
  {%>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchSisMemberStatusGrid", Url.Action("Edit", "SisMemberStatus"), Url.Action("Delete", "SisMemberStatus"), true)%>
<%} %>
<%else
    { %>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchSisMemberStatusGrid", Url.Action("Edit", "SisMemberStatus"), Url.Action("Delete", "SisMemberStatus"), false)%>
<%} %>
<div class="gridContainer">
  <%= Html.Trirand().JQGrid(ViewData.Model, "SearchSisMemberStatusGrid")%>
</div>
<script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
