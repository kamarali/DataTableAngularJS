<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="Iata.IS.Model.MemberProfile.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: System Monitor :: Manage
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <%--<script type="text/javascript" src="http://www.google.com/jsapi"></script>--%>
    <script src="<%=Url.Content("~/Scripts/SystemMonitor.js")%>" type="text/javascript"></script>
    <script type="text/javascript">

        $(function () {
            $("#tabs").tabs({
                load: function (event, ui) {
                }
            //TFS#9977 :IE:Version 11 - Message is not removed when switch to other tab.
            }).on('tabsactivate', function (event, ui) {

                $("#clientErrorMessageContainer").css("display", "none");
                $("#clientSuccessMessageContainer").css("display", "none");
                $("#errorNote").html("");

            });


        });

        function formatClearColumn(cellValue, options, rowObject) {

            var imageHtml = '<img src="<%:Url.Content("~/Content/Images/clear_alert.png") %>" onclick="ClearClicked(\'' + rowObject.MessageId + '\')" />';

            return imageHtml;
        }

        function unformatClearColumn(cellValue, options, rowObject) {
            return $(cellObject.html()).attr("originalValue");
        }
        function ClearClicked(data) {

            $.post('<%: Url.Action("ClearMessage","Home", new { area = ""}) %>', { id: data }, function () {
                $('#SystemAlertGrid').trigger('reloadGrid');
            });
        }


        function CreateProcessedFiles() {
        var url = " <%:Url.Action("CreateProcessedFilesChart", "ManageSystemMonitor" )%>";
            $("#chart").fadeTo(1500, 0.15).attr("src", url + "?t=" + new Date().getTime()).fadeTo(1500, 1);
        }

        function ProcessedSupprotingFiles() {
        var url = " <%:Url.Action("CreateProcessedSupprotingChart", "ManageSystemMonitor" )%>";
            $("#Processedchart").fadeTo(1000, 0.15).attr("src", url + "?t=" + new Date().getTime()).fadeTo(1000, 1);
        }    


    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%if (ViewData.ContainsKey("DownloadFileError")) %>
    <%{ %>
    <div id="errorNote">
        <font color="red">
            <%= Html.Encode(ViewData["DownloadFileError"])%></font>
    </div>
    <%} %>
    <h1>
        System Monitor</h1>
    <div id="tabs">
        <ul class="ui-tabs-nav">
            <%: Html.Tab("Current Stats", "CurrentStats", "ManageSystemMonitor", new { area = "ISOps" }, new { id = "ManageSystemMonitorCurrentStats-tab" }, UserCategory.SisOps)%>
            <%: Html.Tab("Reprocessing", "Reprocessing", "ManageSystemMonitor", new { area = "ISOps" }, new { id = "ManageSystemMonitorReprocessing-tab" }, UserCategory.SisOps)%>
            <%: Html.Tab("Graphs", "Graphs", "ManageSystemMonitor", new { area = "ISOps" }, new { id = "ManageSystemMonitorGraphs-tab" }, UserCategory.SisOps)%>
            <%: Html.Tab("Resend Output Files", "ResendFile", "ManageSystemMonitor", new { area = "ISOps" }, new { id = "ManageSystemMonitorResending-tab" }, UserCategory.SisOps)%>
        </ul>
    </div>
</asp:Content>
