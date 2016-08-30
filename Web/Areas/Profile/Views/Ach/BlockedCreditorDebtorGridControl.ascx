<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<%: ScriptHelper.GenerateAchCreditorsDebitorsGridDeleteScript(Url, Model.ID, "")%>
<div>
  <%= Html.Trirand().JQGrid(ViewData.Model,Model.ID)%>
</div>
<script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
