<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<div class="gridContainer">
  <%= Html.Trirand().JQGrid(ViewData.Model, "UploadGridDataRequested")%>
</div>
