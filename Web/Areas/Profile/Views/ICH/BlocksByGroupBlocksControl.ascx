<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<%: ScriptHelper.GenerateBlockByGroupsGridDeleteScript(Url, "BlocksByGroupBlocksGrid", "")%>
<%= Html.Trirand().JQGrid(ViewData.Model, "BlocksByGroupBlocksGrid")%>
<script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
