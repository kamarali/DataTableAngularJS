<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<%
  if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.SubDivisionEditOrDelete))
  {%>
<%: ScriptHelper.GenerateSubDivisionGridScript(Url, "SearchSubDivisionGrid", Url.Action("Edit", "SubDivision"), Url.Action("Delete", "SubDivision"), true)%>
<%} %>
<%else
    { %>
<%: ScriptHelper.GenerateSubDivisionGridScript(Url, "SearchSubDivisionGrid", Url.Action("Edit", "SubDivision"), Url.Action("Delete", "SubDivision"), false)%>
<%} %>
<div class="gridContainer">
  <%= Html.Trirand().JQGrid(ViewData.Model, "SearchSubDivisionGrid")%>
</div>
<script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
