<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<%: ScriptHelper.GenerateCreditorsDebitorsGridDeleteScript(Url, Model != null ? Model.ID : "0", "")%>
<div>
  <%= Html.Trirand().JQGrid(ViewData.Model, Model != null ? Model.ID : "0")%>
</div>
<script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
