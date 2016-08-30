<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>

<% Html.BeginForm("", "", FormMethod.Post); %>
<div class="gridContainer">
  <%= Html.Trirand().JQGrid(ViewData.Model, "ProfileUpdateGrid")%>
</div>
<div class="buttonContainer">
  <input type="button" class="primaryButton" value="Print" />
  <input type="button" class="secondaryButton" value="Download To Excel" />
  <input type="button" class="secondaryButton" value="Back" onclick="history.back();" />
</div>
<%Html.EndForm(); %>
