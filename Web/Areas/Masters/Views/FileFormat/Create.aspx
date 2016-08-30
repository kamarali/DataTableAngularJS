﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.FileFormat>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: General :: Add File Format Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        File Format Setup
    </h1>
    <h2>
        Add File Format</h2>
    <% using (Html.BeginForm("Create", "FileFormat", FormMethod.Post, new { @id = "FileFormatMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div class="editor-label">
            <label>
                <span class="required">* </span>File Version:</label>
            <%: Html.TextBoxFor(model => model.FileVersion, new { @maxLength = 20 })%>
            <%: Html.ValidationMessageFor(model => model.FileVersion) %>
        </div>
        <div>
            <label>
                <span class="required">* </span>Description:</label>
            <!--SCP304020: UAT 1.6: Misc Codes Setup-->
            <%: Html.TextAreaFor(model => model.Description, 3, 60, new { @maxLength = 255, @class = "validateCharactersForTextArea textAreaTrimText" })%>
            <%: Html.ValidationMessageFor(model => model.Description) %>
        </div>
        <div class="editor-label">
            <label>
                Downloadable:</label>
            <%: Html.CheckBoxFor(model => model.FileDownloadable)%>
            <%: Html.ValidationMessageFor(model => model.FileDownloadable)%>
        </div>
        <div class="editor-label">
            <label>
                Active:</label>
            <%: Html.CheckBoxFor(model => model.IsActive, new { @Checked="checked"})%>
            <%: Html.ValidationMessageFor(model => model.IsActive) %>
        </div>
        <div class="buttonContainer">
            <input type="submit" value="Save" class="primaryButton" onclick="return checkFileVersion();" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","FileFormat") %>'" />
        </div>
    </fieldset>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script src="<%:Url.Content("~/Scripts/Masters/FileFormatValidate.js")%>" type="text/javascript"></script>
    <script type="text/javascript">
        function checkFileVersion() {
            var re = new RegExp("^(\\d+)(.\\d+)?(.\\d+){0,9}(.\\d+)?$");
            var sValue = $("#FileVersion").val();
            if(sValue != ""){
                if (!sValue.match(re)) {
                    alert("Invalid file version.");
                    return false;
                }
            }
        }
    </script>
</asp:Content>
