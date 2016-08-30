<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<%if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.SampleDigitEditOrDelete))
  {%>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchSampleDigitGrid", Url.Action("Edit", "SampleDigit"), Url.Action("Delete", "SampleDigit"), true)%>
<%} %>
<%else
  { %>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchSampleDigitGrid", Url.Action("Edit", "SampleDigit"), Url.Action("Delete", "SampleDigit"), false)%>
<%} %>
<div class="gridContainer">
  <%= Html.Trirand().JQGrid(ViewData.Model, "SearchSampleDigitGrid")%>
</div>
<script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
