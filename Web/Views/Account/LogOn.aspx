<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Anonymous.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="loginTitle" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Log On
</asp:Content>
<asp:Content ID="script" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">
    $(document).ready(function () {
      $("#username").watermark("Username").focus();
      $("#password").watermark("Password");
      //ClearUserSessions();
      getimeZone();
            
    });

      function getTimezoneName() {
          
      tmSummer = new Date(Date.UTC(2005, 6, 30, 0, 0, 0, 0));
      so = -1 * tmSummer.getTimezoneOffset();
      
      tmWinter = new Date(Date.UTC(2005, 12, 30, 0, 0, 0, 0));
      wo = -1 * tmWinter.getTimezoneOffset();
      
      if (-660 == so && -660 == wo) return 'Alaskan Standard Time';
      if (-600 == so && -600 == wo) return 'Hawaiian Standard Time';
      if (-570 == so && -570 == wo) return 'Alaskan Standard Time';
      if (-540 == so && -600 == wo) return 'Hawaiian Standard Time';
      if (-540 == so && -540 == wo) return 'Alaskan Standard Time';
      if (-480 == so && -540 == wo) return 'Alaskan Standard Time';
      if (-480 == so && -480 == wo) return 'Pacific Standard Time';
      if (-420 == so && -480 == wo) return 'Pacific Standard Time';
      if (-420 == so && -420 == wo) return 'Central America Standard Time';
      if (-360 == so && -420 == wo) return 'Mountain Standard Time';
      if (-360 == so && -360 == wo) return 'Central America Standard Time';
      if (-360 == so && -300 == wo) return 'SA Pacific Standard Time';
      if (-300 == so && -360 == wo) return 'Central Standard Time';
      if (-300 == so && -300 == wo) return 'SA Pacific Standard Time';
      if (-240 == so && -300 == wo) return 'Eastern Standard Time';
      if (-240 == so && -240 == wo) return 'Paraguay Standard Time';
      if (-240 == so && -180 == wo) return 'Pacific SA Standard Time';
      if (-180 == so && -240 == wo) return 'Atlantic Standard Time';
      if (-180 == so && -180 == wo) return 'Montevideo Standard Time';
      if (-180 == so && -120 == wo) return 'Pacific SA Standard Time';
      if (-150 == so && -210 == wo) return 'Pacific SA Standard Time';
      if (-120 == so && -180 == wo) return 'UTC-02';
      if (-120 == so && -120 == wo) return 'UTC-02';
      if (-60 == so && -60 == wo) return 'Mid-Atlantic Standard Time';
      if (0 == so && -60 == wo) return 'Mid-Atlantic Standard Time';
      if (0 == so && 0 == wo) return 'Greenwich Standard Time';
      if (60 == so && 0 == wo) return 'W. Europe Standard Time';
      if (60 == so && 60 == wo) return 'W. Central Africa Standard Time';
      if (60 == so && 120 == wo) return 'South Africa Standard Time';
      if (120 == so && 60 == wo) return 'W. Europe Standard Time';
      if (120 == so && 120 == wo) return 'South Africa Standard Time';
      if (180 == so && 120 == wo) return 'E. Europe Standard Time';
      if (180 == so && 180 == wo) return 'E. Africa Standard Time';
      if (240 == so && 180 == wo) return 'Russian Standard Time';
      if (240 == so && 240 == wo) return 'Arabian Standard Time';
      if (270 == so && 210 == wo) return 'Afghanistan Standard Time';
      if (270 == so && 270 == wo) return 'Afghanistan Standard Time';
      if (300 == so && 240 == wo) return 'Azerbaijan Standard Time';
      if (300 == so && 300 == wo) return 'Pakistan Standard Time';
      if (330 == so && 330 == wo) return 'India Standard Time';
      if (345 == so && 345 == wo) return 'Nepal Standard Time';
      if (360 == so && 300 == wo) return 'Central Asia Standard Time';
      if (360 == so && 360 == wo) return 'Central Asia Standard Time';
      if (390 == so && 390 == wo) return 'Myanmar Standard Time';
      if (420 == so && 360 == wo) return 'North Asia Standard Time';
      if (420 == so && 420 == wo) return 'SE Asia Standard Time';
      if (480 == so && 420 == wo) return 'North Asia Standard Time';
      if (480 == so && 480 == wo) return 'W. Australia Standard Time';
      if (540 == so && 480 == wo) return 'North Asia East Standard Time';
      if (540 == so && 540 == wo) return 'Tokyo Standard Time';
      if (570 == so && 570 == wo) return 'AUS Central Standard Time';
      if (570 == so && 630 == wo) return 'Cen. Australia Standard Time';
      if (600 == so && 540 == wo) return 'Yakutsk Standard Time';
      if (600 == so && 600 == wo) return 'E. Australia Standard Time';
      if (600 == so && 660 == wo) return 'AUS Eastern Standard Time';
      if (630 == so && 660 == wo) return 'AUS Eastern Standard Time';
      if (660 == so && 600 == wo) return 'Vladivostok Standard Time';
      if (660 == so && 660 == wo) return 'Central Pacific Standard Time';
      if (690 == so && 690 == wo) return 'Central Pacific Standard Time';
      if (720 == so && 660 == wo) return 'Central Pacific Standard Time';
      if (720 == so && 720 == wo) return 'Fiji Standard Time';
      if (720 == so && 780 == wo) return 'New Zealand Standard Time';
      if (765 == so && 825 == wo) return 'Tonga Standard Time';
      if (780 == so && 780 == wo) return 'Tonga Standard Time'
      if (840 == so && 840 == wo) return 'Tonga Standard Time';
      return 'Pacific Standard Time';
  }




    function getimeZone() {

      var loaclTimeZoneId = getTimezoneName();

     document.getElementById('localTimeZoneId').value = loaclTimeZoneId
 
    }


    function ClearUserSessions() {

      $.ajax({
        type: "POST",
        url: '<%:Url.Action("ClearUserSessions", "Account", new { area = "" })%>',
        dataType: "json",
        success: function (result) {
        }
              });

    }

  </script>
</asp:Content>
<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
  <img alt="Simplified Interline Settlement" src="<%: Url.Content("~/Content/Images/SIS_login_banner_new.png") %>"
    id="banner" />
  <div id="text">
    Welcome to IATA's Simplified Invoicing and Settlement service, the service that
    enables electronic billing and settlement for the air transport industry. To access
    the service, please log-in now. If you do not have a log-in, please contact the
    SIS Super User of your organization.
  </div>
  <% using (Html.BeginForm("LogOn", "Account", FormMethod.Post, new { id = "frmLogOn" }))
     { %>
      <%--SCP#483886 : Error message
      Desc: Html.AntiForgeryToken() removed --%> 
  <div id="login">
    <div style="color: Red; font-weight: bold;">
      <%: Html.ValidationSummary("Login was unsuccessful. Please correct the errors and try again.") %>
    </div>
    <table class="basictable" width="150px">
      <tr>
        <td align="right">
          Username:
        </td>
        <td>
          <%: Html.TextBox("username", ViewData["UserName"])%>
        </td>
      </tr>
      <tr>
        <td align="right">
          Password:
        </td>
        <td>
          <%: Html.Password("password", ViewData["Password"], new { autocomplete = "off" })%>
        </td>
      </tr>     
    </table>
    <%: Html.Hidden("localTimeZoneId") %>
    <table width="210px" style="font-size: 8pt;">
      <tr>
        <td>
          <ul>
            <li>
              <%: Html.ActionLink("Forgot Password?","ForgotPassword","Account") %></li>
            <%--SCP270790: IS login page - hyperlink--%>
            <li><a href="mailto:SIShelp@iata.org?subject=User Access Problem&amp;">Contact Helpdesk</a></li>
          </ul>
        </td>
        <td align="right">
          <input class="primaryButton" type="submit" value="Log On" />
        </td>
      </tr>
    </table>
  </div>
  <% } %>
</asp:Content>
