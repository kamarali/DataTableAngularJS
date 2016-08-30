<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<div>
  <%= Html.Trirand().JQGrid(ViewData.Model, "ContactsAssignemetsGrid")%>
</div>
