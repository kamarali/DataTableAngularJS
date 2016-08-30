<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchSourceCodeGrid", Url.Action("Edit", "SourceCode"), Url.Action("Delete", "SourceCode"))%>
<div class="gridContainer">
  <%= Html.Trirand().JQGrid(ViewData.Model, "SearchSourceCodeGrid")%>
</div>
<script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
