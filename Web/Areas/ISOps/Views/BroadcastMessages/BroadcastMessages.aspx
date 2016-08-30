<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="Iata.IS.Business.Common.Impl" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Broadcast Messages</h1>
  <h2>
    Create Broadcast Message</h2>
  <div class="solidBox dataEntry">
    <div class="fieldContainer horizontalFlow">
      <div>
        <input type="radio" id="rdAnnouncements" name="rdBroadCastType" value="Broadcast Announcements" checked="checked" />Broadcast Announcements
        <input type="radio" id="rdMessages" name="rdBroadCastType" value="Send Messages" />Send Messages
      </div>
    </div>
    <div class="fieldContainer verticalFlow" id="AnnouncementsContainer">
       <%if(ViewData["Announcements"] != null)
            {%> 
            <%Html.RenderPartial("Announcements",ViewData["Announcements"]); %>
            <%
            } else {%>
            <%Html.RenderPartial("Announcements"); %>
            <% }%>
      
      
      <input type="hidden" id="SendMesgOption" value="<%=ViewData["SendMesg"]%>" />
      
     <input type="hidden" id="DefaultExpiryDate" value="<%=ViewData["DefaultExpiryDate"]%>" />

    </div>
    <div class="fieldContainer verticalFlow" id="SendMessagesContainer">
      <%if (ViewData["Messages"] != null)
            {%> 
            <%Html.RenderPartial("SendMessages", ViewData["Messages"]); %>
            <%
            } else {%>
            <%Html.RenderPartial("SendMessages"); %>
            <% }%>
      
    </div>
   
  </div>
</asp:Content>

<asp:Content ID="scriptContent" ContentPlaceHolderID="Script" runat="server">
  <script src="<%: Url.Content("~/Scripts/BroadcastMessages.js")%>" type="text/javascript"></script>
  <script type="text/javascript">
    
      <% 
      if(ViewData["ErrorMessage"] == null || string.IsNullOrEmpty(ViewData["ErrorMessage"].ToString())) {%>
         var error = 'false';
         var errorIn = '';
      <%} else
         {%>
              var error = 'true';
              <% 
            if(ViewData["Announcements"] != null)
            {%> 
            var errorIn = 'Announcements';
            <%
            } else if (ViewData["Messages"] != null)
            {%>
            var errorIn = 'Messages';
            <%
            } else {%>
            var errorIn = '';
            <%} %>
      <%
         }%>

    $(document).ready(function () {
    $('#Message').focus();
       $('#Message').val('<%=ViewData["BrodMessage"]%>');
        $('#DefaultExpiryDate').val('<%=ViewData["DefaultExpiryDate"]%>');

        $('#StartDateTimeValue').val('<%=ViewData["StartDateTimeValue"]%>');
        $('#TimeHourMinutes').val('<%=ViewData["TimeHourMinutes"]%>');

        if(error == "false"){        
          initializeAnnouncement();          
          initializeMessage();         
        }
        else{
          WhenError();
        }
      
      $('#messageExpiryDate').val('<%=ViewData["DefaultExpiryDate"]%>');
      //If send button clicked then initialize the send message container.
      if( $('#SendMesgOption').val() == 1){
        initMessage();  
      }
      
    });

    function WhenError() {
    
    if (errorIn == 'Announcements') {
    
      // Show Add Announcement
      $('#SendMessagesContainer').hide();
      $('#AnnouncementsContainer').show();
      $('#rdMessages').removeAttr("checked");

      // Initialize Messages
      $('#rdAllSuperUsers').val(true);
      $('#IsMessagesAlerts_Message').val('');
      $('#MemberCategory').val('');

      }
    else if(errorIn == 'Messages'){
    
      // Show Add Messages
      $('#AnnouncementsContainer').hide();
      $('#SendMessagesContainer').show();
      $('#rdMessages').attr('checked','checked');

      if($('#rdAllSuperUsers').val() == 'True')
      {
        $('#rdAllSuperUsers').prop('checked',true);
        $('#rdAllUsers').prop('checked',false);
      }

      if($('#rdAllUsers').val() == 'True')
      {
        $('#rdAllSuperUsers').prop('checked',false);
        $('#rdAllUsers').prop('checked',true);
      }

      // Initialize Announcements
      $('#StartDateTimeValue').watermark();
      $('#messageExpiryDate').val($('#DefaultExpiryDate').val());
    }
  }

  function ClearAnnouncement()
  {
    
    initializeAnnouncement();
    $('#messageExpiryDate').val('<%=ViewData["DefaultExpiryDate"]%>');
  }

  </script>
</asp:Content>
