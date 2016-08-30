<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<%if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.AchCurrencySetUpEditOrDelete))
  {%>
<%: ScriptHelper.GenerateGridActiveDeactiveScript(Url, "SearchAchCurrencySetUpGrid", Url.Action("Delete", "AchCurrencySetUp"), Url.Action("IsValidCurrency", "AchCurrencySetUp"), true)%>
<%} %>
<%else
  { %>
<%: ScriptHelper.GenerateGridActiveDeactiveScript(Url, "SearchAchCurrencySetUpGrid", Url.Action("Delete", "AchCurrencySetUp"), Url.Action("IsValidCurrency", "AchCurrencySetUp"), false)%>
<%} %>
<div class="gridContainer">
  <%= Html.Trirand().JQGrid(ViewData.Model, "SearchAchCurrencySetUpGrid")%>
</div>
<script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
