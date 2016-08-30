<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="Trirand.Web.Mvc" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<%@ Import Namespace="Iata.IS.Web.UIModel.Grid.Common" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  Home Page
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <div>
    <h2>
      Upcoming Milestones</h2>
    <div style="width: 470px; float: left; margin-top: 10px;">
      <form method="get" id="form1" action="">
      <div id="UpcomingMilestonetabs" class="solidBox">
        <input type="hidden" id="timeId" name="timeId" value="rrr" />
        <ul>
          <li>
            <%=Html.ActionLink("IS", "GetIsEvents", "Home", "", new { id = "ISLink" })%></li>
          <li>
            <%= Html.ActionLink("ICH", "GetIchEvents", "Home", "", new { id = "ICHLink"})%></li>
          <li>
            <%= Html.ActionLink("ACH", "GetAchEvents", "Home", "", new { id = "ACHLink" })%></li>
        </ul>
      </div>
      </form>
    </div>
    <div style="float: right; width: 490px; margin-top: 10px;">
      <div id="caltabs" class="solidBox">
        <ul>
          <li>
            <input type="hidden" id="isAlertTabClicked" value="true" />
            <%: Html.ActionLink("Alerts", "GetAlerts", "Home", "", new { id = "AlertsLink" })%></li>
          <li>
            <%: Html.ActionLink("Messages", "GetMessages", "Home", "", new { id = "MessagesLink" })%></li>
        </ul>
        <div style="azimuth: center-right; vertical-align: middle;">
          <a style="text-decoration: underline; color: Blue; cursor: pointer;" onclick="ViewInSeperateWindow()">
            View in a separate window</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a style="text-decoration: underline;
              color: Blue; cursor: pointer;" onclick="RefreshClicked()">Refresh</a></div>
      </div>
    </div>
  </div>
  <div id="divAnnouncements" class="clear" style="min-width: 950px;">
    <% Html.RenderPartial("Announcements", ViewData["Announcement"]); %>

  </div>
  <div id="divAlertsGridPopup" class="hidden">
    <%
      Html.RenderPartial("Alerts", ViewData["AlertsGridPopUp"]); %>
      <a style="text-decoration: underline;
              color: Blue; cursor: pointer;" onclick="RefreshAlertPopupClicked()">Refresh</a>
  </div>
  <div id="divMessagesGridPopup" class="hidden">
    <%
      Html.RenderPartial("Messages", ViewData["MessagesGridPopUp"]);%>
      <a style="text-decoration: underline;
              color: Blue; cursor: pointer;" onclick="RefreshMessagePopupClicked()">Refresh</a>
  </div>
  <div>
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">

    $(document).ready(function () {
      CheckUserActiveSession();
      GetAlertMessageCount();
      
    });

   
  function RefreshAlertPopupClicked() {
      $('#AlertsGridPopup').trigger('reloadGrid');
  }

  function RefreshMessagePopupClicked() {
      $('#MessagesGridPopup').trigger('reloadGrid');
  }

    $(function () {
      $("#caltabs").tabs({
        load: function (event, ui) {
        }
    }).on('tabsactivate', function (event, ui) {

        if (ui.newTab.text().match("Messages"))
          $('#isAlertTabClicked').val(false);
        else
          $('#isAlertTabClicked').val(true);

        GetAlertMessageCount();

      });
    })

    $(function () {
      $("#UpcomingMilestonetabs").tabs({
        load: function (event, ui) {
        }
    }).on('tabsactivate', function (event, ui) {

      });
    })

    function ViewInSeperateWindow() {

      if ($('#isAlertTabClicked').val() == "true") {
        $('#AlertsGridPopup').trigger('reloadGrid');
        $("#divAlertsGridPopup").dialog({
          autoOpen: true,
          title: 'Alerts',
          height: 270,
          width: 620,
          modal: true,
          resizable: false
      });

         $("#AlertsGridPopup_pager_center").width(297); 
      }
      else {

        $("#MessagesGridPopup_pager_center").width(297); 

        $('#MessagesGridPopup').trigger('reloadGrid');
        $("#divMessagesGridPopup").dialog({
          autoOpen: true,
          title: 'Messages',
          height: 270,
          width: 620,
          modal: true,
          resizable: false
        });
      }

    }

    function RefreshClicked() {
      if ($('#isAlertTabClicked').val() == "true") {
        $('#AlertsGrid').trigger('reloadGrid');
      }
      else {
        $('#MessagesGrid').trigger('reloadGrid');
      }
    }

    function formatRAGIndicatorColumn(cellValue, options, rowObject) {
      var imageHtml;
      if (cellValue == "1") {
        imageHtml = '<img src="<%:Url.Content("~/Content/Images/status_failed.png") %>" />';
      }
      else if (cellValue == "2") {
        imageHtml = '<img src="<%:Url.Content("~/Content/Images/status_pending.png") %>" />';
      }
      else if (cellValue == "3") {
        imageHtml = '<img src="<%:Url.Content("~/Content/Images/status_succesfully_completed.png") %>" />';
      }
      else {
        imageHtml = '';
      }
      return imageHtml;
    }

    function formatRecipientsColumn(cellValue, options, rowObject) {

      var imageHtml;
      var title = 'title = "' + cellValue + '" alt = "' + cellValue + '" ';
      if (cellValue.length > 0) {
        imageHtml = '<img src="<%:Url.Content("~/Content/Images/multiple_recipients.png") %>"' + title + '/>';
      }
      else {
        imageHtml = '';
      }
      return imageHtml;
    }
    function formatClearColumn(cellValue, options, rowObject) {

        var imageHtml = '<img src="<%:Url.Content("~/Content/Images/clear_alert.png") %>" onclick="ClearClicked(\'' + rowObject.MessageId + '\')" />';

      return imageHtml;
    }

    function unformatRAGIndicatorColumn(cellValue, options, rowObject) {
      return $(cellObject.html()).attr("originalValue");
    }
    function unformatRecipientsColumn(cellValue, options, rowObject) {
      return '<a onclick="showDialog()" class = "link">' + cellValue + '</a>';
    }
    function unformatClearColumn(cellValue, options, rowObject) {
      return $(cellObject.html()).attr("originalValue");
    }

    function ClearClicked(data) {

      $.post('<%: Url.Action("ClearMessage","Home") %>', { id: data }, function () {
        ReloadGrid();
        GetAlertMessageCount();
      });
    }

    //SCP419603: Alerts refresh on the SIS page(5 to 10 minutes)
    setInterval(function () { ReloadGrid(); }, 2400000);

    function ReloadGrid() {

      GetAlertMessageCount();
      RefreshAnnouncements();
      if ($('#isAlertTabClicked').val() == "true") {
        $('#AlertsGrid').trigger('reloadGrid');
        $('#AlertsGridPopup').trigger('reloadGrid');
      }
      else {
        $('#MessagesGrid').trigger('reloadGrid');
        $('#MessagesGridPopup').trigger('reloadGrid');
      }
    }

    function GetAlertMessageCount() {
      $.post('<%: Url.Action("GetAlertMessagesCount","Home") %>', function (data) {
        $('#AlertsLink').text('Alerts (' + data.Alert + ')');
        $('#MessagesLink').text('Messages (' + data.Message + ')');
      });
    }

    function RefreshAnnouncements() {

      $.post('<%:Url.Action("GetAnnouncementsData","Home") %>', function (data) {
        if (data) {

          $('#divAnnouncements').text("");
          $('#divAnnouncements').html(data);
        }
      });

    }

    function CheckUserActiveSession() {
      $.ajax({
        type: "POST",
        url: '<%:Url.Action("CheckUserActiveSession", "Home", new { area = "" })%>',
        dataType: "json",
        success: function (result) {
          if (result.redirect) {
            window.location.href = result.redirect;
            return;
          }
        }
      });
    }   

  </script>
</asp:Content>