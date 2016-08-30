<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.UnlocCode>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: Area Related :: Edit UN Location Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        UN Location Setup</h1>
    <h2>
        Edit UN Location</h2>
    <% using (Html.BeginForm("Edit", "UnlocCode", FormMethod.Post, new { @id = "UnlocCodeMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div>
            <label>
                <span class="required">* </span>UN Location Code:
            </label>
            <%: Html.TextBoxFor(model => model.Id, new { @Class = "alphabetsOnly upperCase", @maxLength = 5, @readonly = "readonly" })%>
            <%: Html.ValidationMessageFor(model => model.Id) %>
        </div>
        <div>
            <label>
                <span class="required">* </span>UN Location Name:
            </label>
            <%: Html.TextBoxFor(model => model.Name, new {@maxLength = 50 })%>
            <%: Html.ValidationMessageFor(model => model.Name) %>
        </div>
        <div>
            <label>
                Active:
            </label>
            <%: Html.CheckBoxFor(model => model.IsActive)%>
            <%: Html.ValidationMessageFor(model => model.IsActive) %>
        </div>
        <div class="buttonContainer">
            <input type="submit" value="Save" class="primaryButton" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","UnlocCode") %>'" />
        </div>
    </fieldset>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script src="<%:Url.Content("~/Scripts/Masters/UnlocCodeValidate.js")%>" type="text/javascript"></script>
</asp:Content>
