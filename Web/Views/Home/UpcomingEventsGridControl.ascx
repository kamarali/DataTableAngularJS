<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JQGrid>" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<div>
    Current Period: <b><%=ViewData["currentPeriod"] %></b>
</div>
<div  style="height:5px"></div>
<div>
   
  <%= Html.Trirand().JQGrid(ViewData.Model, "UpcomingEventsGrid")%>
    
</div>

<script type="text/javascript">

  $(document).ready(function () {
    // "View 1-5 of 5" message on jqgrid was not displayed properly in IE7, so hide the div which displays this message
    $('#pg_UpcomingEventsGrid_pager').hide();
  });
  </script>

