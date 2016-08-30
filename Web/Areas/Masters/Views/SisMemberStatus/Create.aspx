﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.SisMemberStatus>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	 SIS :: Master Maintenance :: General :: Add Member Status Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        Member Status Setup
    </h1> <h2>Add Member Status</h2>
    <% using (Html.BeginForm("Create", "SisMemberStatus", FormMethod.Post, new { @id = "SisMemberStatusMaster" }))
       {%>
        <%: Html.ValidationSummary(true) %>

       <fieldset class="solidBox dataEntry">
            <div class="editor-label">
                <label><span class="required">* </span>Member Status Code:</label>
                <%: Html.TextBoxFor(model => model.MemberStatus, new { @class="alphabet upperCase",@maxLength=1})%>
                <%: Html.ValidationMessageFor(model => model.MemberStatus)%>
            </div>
            <div class="editor-label">
                <label><span class="required">* </span>Description:</label>
                <%: Html.TextAreaFor(model => model.Description, 3, 60, new { @maxLength = 255, @class = "validateCharactersForTextArea" })%>
                <%: Html.ValidationMessageFor(model => model.Description) %>
            </div>
            <div class="editor-label">
                 <label>Active:</label>
                <%: Html.CheckBoxFor(model => model.IsActive, new { @Checked = "checked"})%>
                <%: Html.ValidationMessageFor(model => model.IsActive) %>
            </div>
            <div class="buttonContainer">
            <input type="submit" value="Save" class="primaryButton" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","SisMemberStatus") %>'" />
        </div>
        </fieldset>

    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script src="<%:Url.Content("~/Scripts/Masters/SisMemberStatusValidate.js")%>" type="text/javascript"></script>

</asp:Content>
