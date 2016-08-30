<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.SisMemberSubStatus>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	 SIS :: Master Maintenance :: General :: Add Member Sub Status Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
     <h1>
        Member Sub Status Setup
    </h1><h2>Add Member Sub Status</h2>
    <% using (Html.BeginForm("Create", "SisMemberSubStatus", FormMethod.Post, new { @id = "SisMemberSubStatusMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
        <%: Html.ValidationSummary(true) %>

       <fieldset class="solidBox dataEntry">
            <div class="editor-label">
                <label><span class="required">* </span>Description:</label>
                <%: Html.TextBoxFor(model => model.Description, new { @maxLength = 254 })%>
                <%: Html.ValidationMessageFor(model => model.Description) %>
            </div>
              <!-- CMP #665: Added four rows in the class SisMemberSubStatus. -->
            <div class="editor-label">
                 <label>Suppress OTP Email:</label>
                <%: Html.CheckBoxFor(model => model.SuppressOtpEmail)%>
                <%: Html.ValidationMessageFor(model => model.SuppressOtpEmail)%>
            </div>
            <div class="editor-label">
                 <label>Redirect Upon Login:</label>
                <%: Html.CheckBoxFor(model => model.RedirectUponLogin)%>
                <%: Html.ValidationMessageFor(model => model.RedirectUponLogin)%>
            </div>
            <div class="editor-label">
                 <label>Limited Member Profile Access:</label>
                <%: Html.CheckBoxFor(model => model.LimitedMemProfileAccess)%>
                <%: Html.ValidationMessageFor(model => model.LimitedMemProfileAccess)%>
            </div>
            <div class="editor-label">
                 <label>Disable User Profile Updates:</label>
                <%: Html.CheckBoxFor(model => model.DisableUserProfileUpdates)%>
                <%: Html.ValidationMessageFor(model => model.DisableUserProfileUpdates)%>
            </div>
            <div class="editor-label">
                 <label>Active:</label>
                <%: Html.CheckBoxFor(model => model.IsActive, new { @Checked = "checked"})%>
                <%: Html.ValidationMessageFor(model => model.IsActive) %>
            </div>
            <div class="buttonContainer">
            <input type="submit" value="Save" class="primaryButton" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","SisMemberSubStatus") %>'" />
        </div>
        </fieldset>

    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script src="<%:Url.Content("~/Scripts/Masters/SisMemberSubStatusValidate.js")%>" type="text/javascript"></script>

</asp:Content>

