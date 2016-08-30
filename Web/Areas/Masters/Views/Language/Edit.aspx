<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.Language>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: General :: Language Master
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h1>Language Setup</h1>
 
  
    <h2>Edit Language</h2>

    <% using (Html.BeginForm("Edit", "Language", FormMethod.Post, new { @id = "LanguageMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div class="editor-label">
            <label>
                <span class="required">* </span>Language Code:</label>
            <%: Html.TextBoxFor(model => model.Language_Code, new { @class = "alphaNumeric lowerCase", @readonly = "readonly" })%>
            <%: Html.ValidationMessageFor(model => model.Language_Code) %>
        </div>
        
        <div class="editor-label">
            <label>
                Language Description:</label>
            <!--SCP304020: UAT 1.6: Misc Codes Setup-->
            <%: Html.TextAreaFor(model => model.Language_Desc, 3, 60, new { @maxLength = 200, @class = "validateCharactersForTextArea textAreaTrimText" })%>
            <%: Html.ValidationMessageFor(model => model.Language_Desc) %>
        </div>
       
        <div class="editor-label">
            <label>
                Is Required for Help:</label>
            <%: Html.CheckBoxFor(model => model.IsReqForHelp, new { @Checked="checked"})%>
            <%: Html.ValidationMessageFor(model => model.IsReqForHelp) %>
        </div>
        <div class="editor-label">
            <label>
                Is Required for PDF:</label>
            <%: Html.CheckBoxFor(model => model.IsReqForPdf, new { @Checked="checked"})%>
            <%: Html.ValidationMessageFor(model => model.IsReqForPdf) %>
        </div>

        <div class="buttonContainer">
            <input type="submit" value="Save" class="primaryButton" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","Language") %>'" />
        </div>
    </fieldset>

    <% } %>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script src="<%:Url.Content("~/Scripts/Masters/LanguageValidate.js")%>" type="text/javascript"></script>
<script type="text/javascript" language="javascript">    $(document).ready(function () { $('#Language_Desc').focus(); });   </script>
</asp:Content>