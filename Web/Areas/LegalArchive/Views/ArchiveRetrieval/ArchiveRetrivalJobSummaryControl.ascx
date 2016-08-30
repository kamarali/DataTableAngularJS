<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<% Html.BeginForm("", "", FormMethod.Post); %>
<div>
  <%= Html.Trirand().JQGrid(ViewData.Model, "ArchiveRetrivalJobSummaryGridControl")%>
</div>
<div>
</div>
<%Html.EndForm(); %>


