<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<%
  if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.VatIdentifierEditOrDelete))
  {%>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchVatIdentifierGrid", Url.Action("Edit", "VatIdentifier"), Url.Action("Delete", "VatIdentifier"), true)%>
<%} %>
<%else
    { %>
<%: ScriptHelper.GenerateGridEditActiveDeactiveScript(Url, "SearchVatIdentifierGrid", Url.Action("Edit", "VatIdentifier"), Url.Action("Delete", "VatIdentifier"), false)%>
<%} %>
<div class="gridContainer">
  <%= Html.Trirand().JQGrid(ViewData.Model, "SearchVatIdentifierGrid")%>
</div>
<script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
