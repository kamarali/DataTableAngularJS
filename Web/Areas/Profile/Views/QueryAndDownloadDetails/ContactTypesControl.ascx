<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %> 

<%= Html.Trirand().JQGrid(ViewData.Model, "ContactTypesGrid")%>

