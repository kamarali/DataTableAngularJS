<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<%if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.ChargeCodeTypeNameSetupEditOrDelete))
  {%>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchChargeCodeTypeNameSetUpGrid", Url.Action("Edit", "ChargeCodeTypeNameSetUp"), Url.Action("Delete", "ChargeCodeTypeNameSetUp"), true)%>
<%} %>
<%else
  { %>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchChargeCodeTypeNameSetUpGrid", Url.Action("Edit", "ChargeCodeTypeNameSetUp"), Url.Action("Delete", "ChargeCodeTypeNameSetUp"), false)%>
<%} %>
<div class="gridContainer">
  <%= Html.Trirand().JQGrid(ViewData.Model, "SearchChargeCodeTypeNameSetUpGrid")%>
</div>
<script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
