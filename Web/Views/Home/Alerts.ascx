<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>

<div>
    <% if(ViewData["Popup"] == null ? true : false)
      { %>
      <%= Html.Trirand().JQGrid(ViewData.Model, "AlertsGrid")%>
    <%}
      else
      {%>
      <%= Html.Trirand().JQGrid(ViewData.Model, "AlertsGridPopup")%>
    <%
      }%>
</div>

