<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<%
  if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.MinMaxAcceptableAmtEditOrDelete))
  {%>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchMinMaxAcceptableAmountGrid", Url.Action("Edit", "MinMaxAcceptableAmount"), Url.Action("Delete", "MinMaxAcceptableAmount"), true)%>
<%} %>
<%else
    { %>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchMinMaxAcceptableAmountGrid", Url.Action("Edit", "MinMaxAcceptableAmount"), Url.Action("Delete", "MinMaxAcceptableAmount"), false)%>
<%} %>
<div class="gridContainer">
  <%= Html.Trirand().JQGrid(ViewData.Model, "SearchMinMaxAcceptableAmountGrid")%>
</div>
<script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
