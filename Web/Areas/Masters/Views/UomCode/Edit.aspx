<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MiscUatp.Common.UomCode>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Master Maintenance :: Miscellaneous Related :: Edit Unit Of Measure Code Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Unit Of Measure Code Setup
  </h1>
  <h2>
    Edit Unit Of Measure Code</h2>
  <% using (Html.BeginForm("Edit", "UomCode", FormMethod.Post, new { @id = "UomCodeMaster" }))
     {%>
     <%: Html.AntiForgeryToken() %>
  <fieldset class="solidBox dataEntry">
    <div class="editor-label">
      <label>
        <span class="required">* </span>UOM Code:</label>
      <%: Html.TextBox("Code", Model.Id, new { @class = "alphaNumeric upperCase", @maxLength = 3, @readonly = "readonly" })%>
    </div>
    <div class="editor-label">
      <label>
        <span class="required">* </span>UOM Code Type:</label>
      <%: Html.UOMCodeTypeDropdownListFor(model => model.Type, new { disabled = true })%>
    </div>
    <div class="editor-label">
      <label>
        Description:</label>
      <!--SCP304020: UAT 1.6: Misc Codes Setup-->
      <%: Html.TextAreaFor(model => model.Description, 3, 60, new { @maxLength = 255, @class = "validateCharactersForTextArea textAreaTrimText" })%>
    </div>
    <div class="editor-label">
      <label>
        Active:</label>
      <%: Html.CheckBoxFor(model => model.IsActive)%>
    </div>
    <div class="buttonContainer">
      <input type="submit" value="Save" class="primaryButton" />
      <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","UomCode") %>'" />
    </div>
  </fieldset>
  <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script src="<%:Url.Content("~/Scripts/Masters/UomCodeValidate.js")%>" type="text/javascript"></script>
  <script type="text/javascript">
    $(document).ready(function () {
      $("#Type").attr("disabled");
    });
  </script>
</asp:Content>
