<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Trirand.Web.Mvc.JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<div class="gridContainer">
  <%= Html.Trirand().JQGrid(ViewData.Model, "ManageSuspendedInvoicesSearchResultGrid")%>
</div>
