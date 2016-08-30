<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<%: ScriptHelper.GenerateGridEditDeleteScript(Url, "BlockingRuleGrid", "BlockingRuleEdit", "DeleteBlockingRule")%>
<div>
  <%= Html.Trirand().JQGrid(ViewData.Model, "BlockingRuleGrid")%>
</div>
<script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
