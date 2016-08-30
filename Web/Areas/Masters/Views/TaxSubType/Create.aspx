<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MiscUatp.Common.TaxSubType>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
   SIS :: Master Maintenance :: Miscellaneous Related :: Add Tax Sub Type Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
     <h1>
       Tax Sub Type Setup
    </h1><h2>
        Add Tax Sub Type</h2>
    <% using (Html.BeginForm("Create", "TaxSubType", FormMethod.Post, new { @id = "TaxSubTypeMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div class="editor-label">
            <label>
                <span class="required">* </span>Tax Sub Type:
            </label>
            <%: Html.TextBoxFor(model => model.SubType, new { @class = "alphabetsOnly upperCase", @maxLength = 20 })%>
            <%: Html.ValidationMessageFor(model => model.SubType) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Tax Type:
            </label>
            <%: Html.TextBoxFor(model => model.Type, new { @class = "alphabet upperCase", @maxLength = 1 })%>
            <%: Html.ValidationMessageFor(model => model.Type) %>
        </div>
        <div class="editor-label">
            <label>
                Active:
            </label>
            <%: Html.CheckBoxFor(model => model.IsActive, new { @Checked="checked"})%>
            <%: Html.ValidationMessageFor(model => model.IsActive) %>
        </div>
        <div class="buttonContainer">
            <input type="submit" value="Save" class="primaryButton" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","TaxSubType") %>'" />
        </div>
    </fieldset>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script src="<%:Url.Content("~/Scripts/Masters/TaxSubTypeValidate.js")%>" type="text/javascript"></script>

</asp:Content>
