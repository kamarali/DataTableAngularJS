<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Calendar.ISCalendar>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    IS Calendar
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <h2>
        Create IS Calendar</h2>
    <% using (Html.BeginForm("Create", "ISCalendar", FormMethod.Post, new { @id = "ISCalendarMaster" }))
       {%>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div class="editor-label">
            <label>
                <span class="required">* </span>Event Name:
            </label>
            <%: Html.TextBoxFor(model => model.Name, new { @maxLength = 100 })%>
            <%: Html.ValidationMessageFor(model => model.Name) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Month:
            </label>
            <%: Html.TextBoxFor(model => model.Month, new { @Class = "integer", @maxLength = 2 })%>
            <%: Html.ValidationMessageFor(model => model.Month) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Year:
            </label>
            <%: Html.TextBoxFor(model => model.Year, new { @Class = "integer", @maxLength = 4 })%>
            <%: Html.ValidationMessageFor(model => model.Year) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Period:
            </label>
            <%: Html.TextBoxFor(model => model.Period, new { @Class = "integer", @maxLength = 2 })%>
            <%: Html.ValidationMessageFor(model => model.Period) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Event Type:
            </label>
            <%: Html.TextBoxFor(model => model.EventCategory, new { @maxLength = 10 })%>
            <%: Html.ValidationMessageFor(model => model.EventCategory) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Event Description:
            </label>
            <%: Html.TextAreaFor(model => model.EventDescription, 3, 60, new { @maxLength = 255 })%>
            <%: Html.ValidationMessageFor(model => model.EventDescription) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Event Date Time:
            </label>
            <%:Html.TextBox("EventDateTime", null, new { @class = "dateTimePicker", @id = "EventDateTime" })%>
            <%: Html.ValidationMessageFor(model => model.EventDateTime) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Display On Home Page:
            </label>
            <%: Html.CheckBoxFor(model => model.DisplayOnHomePage) %>
            <%: Html.ValidationMessageFor(model => model.DisplayOnHomePage) %>
        </div>
        <div class="editor-label">
            <label>
                Active:
            </label>
            <%: Html.CheckBoxFor(model => model.IsActive)%>
            <%: Html.ValidationMessageFor(model => model.IsActive) %>
        </div>
        <div class="buttonContainer">
            <input type="submit" value="Save" class="primaryButton" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","ISCalendar") %>'" />
        </div>
    </fieldset>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script src="<%:Url.Content("~/Scripts/Masters/ISCalendarValidate.js")%>" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            $('#EventDateTime').datetimepicker({
                showSecond: true,
                dateFormat: "yy-m-d",
                timeFormat: "hh:mm:ss",
                separator: ' '
            });
        });
    </script>
</asp:Content>
