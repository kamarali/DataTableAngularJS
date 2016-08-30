<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchAddOnChargeNameGrid", Url.Action("Edit", "AddOnChargeName"), Url.Action("Delete", "AddOnChargeName"))%>
<div class="gridContainer">
  <%= Html.Trirand().JQGrid(ViewData.Model, "SearchAddOnChargeNameGrid")%>
</div>
<script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
