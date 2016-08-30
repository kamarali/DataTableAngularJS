<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Anonymous.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="forgotPasswordTitle" ContentPlaceHolderID="TitleContent" runat="server">
	ForgotPassword
</asp:Content>

<asp:Content ID="forgotPasswordContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Forgot Password</h2>
    <div id="passwordControls">
        <label for="passPhraseQuestionTitle">Pass-Phrase Question:</label>
        <span id="passPhraseQuestion"><%:ViewData["Question"]%></span>
        <br /><br /><label for="passPhraseAnswer">Pass-Phrase Answer:</label>
        <input id="passPhraseAnswer" name="passPhraseAnswer" type="password" />
        <div class="buttonContainer">
            <input class="primaryButton" value="Submit" id="submitButton" type="submit"/>
            <input type="button" value="Back" class="secondaryButton" name="btnBack" id="backButton" />
        </div>
    </div>
</asp:Content>

<asp:Content ID="script" ContentPlaceHolderID="Script" runat="server">
    
</asp:Content>
