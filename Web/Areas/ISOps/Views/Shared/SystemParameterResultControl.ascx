<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<div>
  <%= Html.Trirand().JQGrid(ViewData.Model, "ISSystemParameterResultsGrid")%>
</div>
