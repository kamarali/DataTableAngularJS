<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<div>
  <br />
</div>
<div class="gridContainer" style="min-width: 420px;">
  <%= Html.Trirand().JQGrid(ViewData.Model, "FSFileActionResultsGrid")%>
</div>
