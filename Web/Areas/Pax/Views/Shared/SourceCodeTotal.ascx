<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Trirand.Web.Mvc.JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<div>
  <%= Html.Trirand().JQGrid(ViewData.Model, "AvailableVatGrid")%>
</div>
