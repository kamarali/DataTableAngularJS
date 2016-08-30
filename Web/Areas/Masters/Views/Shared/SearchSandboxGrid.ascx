<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<%
  if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Sandbox.SandboxCertParameterAccess))
  {%>
<%:ScriptHelper.GenerateGridEditScript(Url, "SearchSandboxGrid", Url.Action("Edit", "SandBox"))%>
<%} %>

<div>
  <%= Html.Trirand().JQGrid(ViewData.Model, "SearchSandboxGrid")%>
</div>
<script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
