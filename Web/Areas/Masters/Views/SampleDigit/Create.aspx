<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.Common.SampleDigit>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
   SIS :: Master Maintenance :: Passenger Related :: Add Sample Digit Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
       Sample Digit Setup
    </h1><h2>
        Create Sample Digit</h2>
    <% using (Html.BeginForm("Create", "SampleDigit", FormMethod.Post, new { @id = "SampleDigitMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div class="editor-label">
            <label>
                <span class="required">* </span>Billing Month:</label>
            <%: Html.TextBoxFor(model => model.ProvisionalBillingMonth, new { @class = "numeric",@maxLength=6})%>
            <%: Html.ValidationMessageFor(model => model.ProvisionalBillingMonth) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Digit Announcement Date:</label>
            <%:Html.TextBox("DigitAnnouncementDateTime", null, new { @class = "datePicker", @id = "DigitAnnouncementDateTime" })%>
            <%: Html.ValidationMessageFor(model => model.DigitAnnouncementDateTime) %>
        </div>
        <div class="editor-label">
            <label>
                Active:</label>
            <%: Html.CheckBoxFor(model => model.IsActive, new { @Checked="checked"})%>
            <%: Html.ValidationMessageFor(model => model.IsActive) %>
        </div>
        <div class="buttonContainer">
            <input type="submit" value="Save" class="primaryButton" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","SampleDigit") %>'" />
        </div>
    </fieldset>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script src="<%:Url.Content("~/Scripts/Masters/SampleDigitValidate.js")%>" type="text/javascript"></script>

</asp:Content>
