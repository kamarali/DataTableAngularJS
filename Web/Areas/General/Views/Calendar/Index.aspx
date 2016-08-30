<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">
    function resetForm() {
      $(':input', '#ISCalendarSearchForm')
        .not(':button, :submit, :reset, :hidden')
        .val('')
        .removeAttr('selected');
      $("#calendarSearchYear").val((new Date).getFullYear());
      $("#calendarSearchMonth").val("-1");
      $("#calendarSearchPeriod").val("-1");

    }
  </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: General :: IS Calendar Search
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    IS and Clearing House Calendar</h1>
  <h2>
    Query Criteria</h2>
  <div>
    <%using (Html.BeginForm("Index", "Calendar", FormMethod.Post, new { id = "ISCalendarSearchForm" }))
      {
          Html.RenderPartial("CalendarSearchControl"); 
      } 
    %>
  </div>
  <h2>
    Search Results</h2>
  <div>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.ISCalendarSearchGrid]); %>
  </div>
</asp:Content>
