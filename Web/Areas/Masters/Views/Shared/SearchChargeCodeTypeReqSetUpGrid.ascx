<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<%if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.ChargeCodeTypeReqSetupEditOrDelete))
  {%>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchChargeCodeTypeReqSetUpGrid", Url.Action("Edit", "ChargeCodeTypeRequirementSetUp"), Url.Action("Delete", "ChargeCodeTypeRequirementSetUp"), true)%>
<%} %>
<%else
  { %>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchChargeCodeTypeReqSetUpGrid", Url.Action("Edit", "ChargeCodeTypeRequirementSetUp"), Url.Action("Delete", "ChargeCodeTypeRequirementSetUp"), false)%>
<%} %>

<div class="gridContainer">
  <%= Html.Trirand().JQGrid(ViewData.Model, "SearchChargeCodeTypeReqSetUpGrid")%>
</div>
<script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
