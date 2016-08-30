<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SIS.Web.UIModels.Account.SearchBarModel>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<%= Html.Trirand().JQGrid(Model.OrdersGrid, "SearchGrid") %>
