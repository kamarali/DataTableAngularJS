<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Reports.IsClearingHouseCalendar>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS:: Reports :: IS and CH Calendar Report
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        IS and CH Calendar Report</h1>
    <% using (Html.BeginForm("IsCalendar", "IsCalendar", FormMethod.Post, new { @id = "IsCalendarReport" }))
       {%>
    <div>
        <% Html.RenderPartial("IsCalendarSearch"); %>
    </div>
    <div class="buttonContainer">
        <input type="submit" id="generateButton" class="primaryButton" value="Generate Report"
            onclick="CheckValidation();" />
    </div>
    <%} %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script type="text/javascript" src="<%:Url.Content("~/Scripts/Reports/ManageIsCalendarReport.js")%>"></script>
    <script type="text/javascript">
        function CheckValidation() {
            GenerateIsCalendarReport("IsCalendarReport");
        }
        $(document).ready(function () { $('#ClearanceYear').focus(); }); 
    </script>
</asp:Content>
