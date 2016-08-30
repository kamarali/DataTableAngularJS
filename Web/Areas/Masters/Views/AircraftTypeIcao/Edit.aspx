<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.AircraftTypeIcao>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
 SIS :: Master Maintenance :: Miscellaneous Related :: Edit Aircraft Type ICAO Setup
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
       Aircraft Type ICAO Setup
    </h1> <h2>Edit Aircraft Type ICAO</h2>

    <% using (Html.BeginForm("Edit", "AircraftTypeIcao", FormMethod.Post, new { @id = "AircraftTypeIcaoMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
        <%: Html.ValidationSummary(true) %>

        <fieldset class="solidBox dataEntry">
         <div class="editor-label">
            <label>
                <span class="required">* </span>Aircraft Type ICAO Code:
            </label>
            <%: Html.TextBoxFor(model => model.Id, new { @class = "alphaNumeric upperCase", @maxLength = 4, @readonly = "readonly" })%>
            <%: Html.ValidationMessageFor(model => model.Id) %>
        </div>
        <div class="editor-label">
            <label>
                Description:
            </label>
            <!--SCP304020: UAT 1.6: Misc Codes Setup-->
            <%: Html.TextAreaFor(model => model.Description, 3, 60, new { @maxLength = 255, @class = "validateCharactersForTextArea textAreaTrimText" })%>
            <%: Html.ValidationMessageFor(model => model.Description) %>
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
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","AircraftTypeIcao") %>'" />
        </div>
        </fieldset>

    <% } %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script src="<%:Url.Content("~/Scripts/Masters/AircraftTypeIcaoValidate.js")%>" type="text/javascript"></script>

</asp:Content>