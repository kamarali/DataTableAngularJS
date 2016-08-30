<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Iata.IS.Model.Common.AlertsMessagesAnnouncementsResultSet>>" %>
<%@ Import Namespace="Iata.IS.Business.Common.Impl" %>

<div>
  <h2>
    Announcements</h2>
  <div style="border: 1px solid #ddd; height: 80px; overflow:auto">
    <ul style="padding-left: 15px;">
      <%foreach (var o in Model)
        {
            o.FromDate = CalendarManager.ConvertUtcTimeToYmq(o.FromDate);
%>
          <li><b><%:String.Format("{0:d-MMM-yyyy HH:mm}", o.FromDate)%> :</b>&nbsp;<%:o.Detail%></li><%
        }
%>
    </ul>
  </div>
</div>

