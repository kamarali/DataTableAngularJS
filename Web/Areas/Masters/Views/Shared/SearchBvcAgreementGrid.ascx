<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<%
  if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.BvcAgreementSetupEditOrDelete))
  {%>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchBvcAgreementGrid", Url.Action("Edit", "OneWayBVCAgreement"), Url.Action("Active", "OneWayBVCAgreement"), true)%>
<%} %>
<%else
    { %>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchBvcAgreementGrid", Url.Action("Edit", "OneWayBVCAgreement"), Url.Action("Active", "OneWayBVCAgreement"), false)%>
<%} %>
<div class="gridContainer">
  <%= Html.Trirand().JQGrid(ViewData.Model, "SearchBvcAgreementGrid")%>
</div>  
<script src="<%:Url.Content("~/Scripts/Masters/BvcAgreementValidate.js")%>" type="text/javascript"></script>
