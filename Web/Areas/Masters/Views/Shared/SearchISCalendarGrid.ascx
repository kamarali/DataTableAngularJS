<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchISCalendarGrid", Url.Action("Edit", "ISCalendar"), Url.Action("Delete", "ISCalendar"))%>
<div class="gridContainer">
  <%= Html.Trirand().JQGrid(ViewData.Model, "SearchISCalendarGrid")%>
</div>
<script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
