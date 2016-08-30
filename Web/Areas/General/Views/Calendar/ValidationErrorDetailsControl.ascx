<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<div>
  <%if (ViewData["CalendarValidationErrors"] != null)
    {
      var errors = (List<Iata.IS.Business.Common.CalendarValidationError>)ViewData["CalendarValidationErrors"];
      if (errors.Count > 0)
      {
  %>
  <table>
    <tr>
      <th>
        Line Number
      </th>
      <th>
        Message
      </th>
      
    </tr>
    <%
      foreach (var calendarValidationError in errors)
      {
    %>
    <tr>
      <td>
        <%:calendarValidationError.RecordNo %>
      </td>
      <td>
        <%:calendarValidationError.Message %>
      </td>
      
    </tr>
    <%
      }%>
  </table>
  <%
    }
      } %>
</div>
