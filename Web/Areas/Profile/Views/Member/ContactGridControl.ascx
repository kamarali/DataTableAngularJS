<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<%: ScriptHelper.GenerateGridDeleteScript(Url, "ContactsGrid", "DeleteContact", selectedMemberId: (int) Model[0])%>
<div>
  <%= Html.Trirand().JQGrid((JQGrid)Model[1], "ContactsGrid")%>
</div>
<script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
