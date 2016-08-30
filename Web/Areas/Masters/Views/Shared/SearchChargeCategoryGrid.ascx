<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchChargeCategoryGrid", Url.Action("Edit", "ChargeCategory"), Url.Action("Delete", "ChargeCategory"))%>
<div class="gridContainer">
  <%= Html.Trirand().JQGrid(ViewData.Model, "SearchChargeCategoryGrid")%>
</div>
<script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
