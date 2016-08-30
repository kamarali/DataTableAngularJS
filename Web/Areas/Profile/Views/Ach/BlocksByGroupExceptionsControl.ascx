<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<%: ScriptHelper.GenerateExceptionsGridDeleteScript(Url, "BlocksByGroupExceptionsGrid", "")%>
<%= Html.Trirand().JQGrid(ViewData.Model, "BlocksByGroupExceptionsGrid")%>
<script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
