<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewPage<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<!--TODO: ScriptHelper delete function to delete selected grid rows -->
<div class="gridContainer">
  <%= Html.Trirand().JQGrid(ViewData.Model, Model.ID)%>
</div>