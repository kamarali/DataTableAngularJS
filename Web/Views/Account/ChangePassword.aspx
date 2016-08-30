<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="Iata.IS.Web.Util" %>
<asp:Content ID="changePasswordTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Change Password
</asp:Content>
<asp:Content ID="ChangeSecurityQuestionsContent" ContentPlaceHolderID="Script" runat="server">
    <script src="<%:Url.Content("~/Scripts/jquery.validate.min.js")%>" type="text/javascript"></script>
    <script src="<%:Url.Content("~/Scripts/jquery.dirtyforms.js")%>" type="text/javascript"></script>
    <script src="<%:Url.Content("~/Scripts/validator.js")%>" type="text/javascript"></script>
    <script src="<%:Url.Content("~/Scripts/jquery.form.js")%>" type="text/javascript"></script>
    <script type="text/javascript">

        $(function () {
            $("#frmChangePassword").validate({
                rules: {

                    currentPassword: { required: true },
                    newPassword: { required: true },
                    confirmPassword: { required: true }

                },
                messages: {

                    currentPassword: "Current Password Required",
                    newPassword: "New Password Required",
                    confirmPassword: "Confirm New Password Required"
                }
            });
        });

    </script>
</asp:Content>
<asp:Content ID="changePasswordContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Change Password</h2>
    <p>
        <%:Html.Encode(ViewData["ExpiredPasswordMessage"])%>
        Please use the form below to change your password.
    </p>
    <p>
        New password is required to be minimum of seven characters in length and contains
        at least one alpha and one numeric value.
    </p>
    <div style="color: Red">
        <%: Html.ValidationSummary("Password change was unsuccessful. Please correct the errors and try again.")%>
    </div>
    <% using (Html.BeginForm("ChangePassword", "Account", FormMethod.Post, new { id = "frmChangePassword" }))
       { %>
    <%: Html.AntiForgeryToken() %>
    <fieldset class="solidBox dataEntry">
        <div class="fieldContainer horizontalFlow">
            <legend style="color: #0075BD; font: bold 10pt Arial,Helvetica,sans-serif; margin: 0;
                padding-top: 5px;">Account Information</legend>
            <div class="bottomLine">
            </div>
            <div>
                <label for="currentPassword">
                    Current Password:</label>
                <%: Html.Password("currentPassword",null, new { autocomplete = "off" }) %>
            </div>
            <div class="bottomLine">
            </div>
            <div>
                <label for="newPassword">
                    New Password:</label>
                <%: Html.Password("newPassword",null, new { autocomplete = "off" }) %>
            </div>
            <div class="bottomLine">
            </div>
            <div>
                <label for="confirmPassword">
                    Confirm New Password:</label>
                <%: Html.Password("confirmPassword",null, new { autocomplete = "off" }) %>
            </div>
            <div class="bottomLine">
            </div>
            <div class="bottomLine">
                <input type="submit" value="<%= Iata.IS.Web.AppSettings.ChangePasswordText %>" class="primaryButton" />
                <%if (SessionUtil.IsLoggedIn)
                  { %>
                <input class="secondaryButton" type="button" value="<%=Iata.IS.Web.AppSettings.CancelButtonText %>"
                    onclick="javascript:location.href ='<%=Url.Action("UserModify","Account") %>'" />
                <%}%>
            </div>
        </div>
    </fieldset>
    <% } %>
</asp:Content>
