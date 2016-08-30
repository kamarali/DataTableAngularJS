<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchContactsGrid", Url.Action("Edit", "ManageContacts"), Url.Action("Delete", "ManageContacts"),true)%>
<div class="gridContainer">
  <%= Html.Trirand().JQGrid(ViewData.Model, "SearchContactsGrid")%>
</div>
<script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
