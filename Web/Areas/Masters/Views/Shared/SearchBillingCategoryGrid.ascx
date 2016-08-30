<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchBillingCategoryGrid", Url.Action("Edit", "BillingCategory"), Url.Action("Delete", "BillingCategory"))%>
<div class="gridContainer">
  <%= Html.Trirand().JQGrid(ViewData.Model, "SearchBillingCategoryGrid")%>
</div>
<script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
