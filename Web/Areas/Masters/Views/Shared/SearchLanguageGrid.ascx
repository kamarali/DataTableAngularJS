<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<%
  if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.LanguageSetupEditOrDelete))
  {%>
<%: ScriptHelper.GenerateGridEditScript(Url, "SearchLanguageGrid", Url.Action("Edit", "Language"), true)%>
<%} %>
<%else
    { %>
<%: ScriptHelper.GenerateGridEditScript(Url, "SearchLanguageGrid", Url.Action("Edit", "Language"), false)%>
<%} %>
<div class="gridContainer">
  <%= Html.Trirand().JQGrid(ViewData.Model, "SearchLanguageGrid")%>
</div>  
<script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
