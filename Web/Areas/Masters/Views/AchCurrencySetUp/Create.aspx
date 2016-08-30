<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MiscUatp.Common.AchCurrencySetUp>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
   SIS :: ACH Ops ::  ACH Currencies of Clearance
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
      Allowed ACH Currencies of Clearance Setup
    </h1><h2>
        Add New Currency</h2>
    <% using (Html.BeginForm("Create", "AchCurrencySetUp", FormMethod.Post, new { @id = "AchCurrencySetUpMaster" }))
       {%>
    <%: Html.ValidationSummary(true) %>
    <%= Html.AntiForgeryToken() %>
     <fieldset class="solidBox dataEntry">
        <div class="editor-label">
            <label>
                <span class="required">* </span>Currency Of Clearance:
            </label>
            <%: Html.AchCurrencyDropdownList(model => model.Id, new { style = "width:140px" })%>
            <%: Html.Hidden("achCurrencyCode") %>
        </div>
        <div class="editor-label">
            <label>
                Currency Name:
            </label>
           <%: Html.TextBoxFor(model=>model.Name, new { @readOnly = true, style = "width:140px" })%>
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
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","AchCurrencySetUp") %>'" />
        </div>

    </fieldset>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script src="<%:Url.Content("~/Scripts/Masters/AchCurrencySetUp.js")%>" type="text/javascript"></script>
  <script type="text/javascript">
      $(document).ready(function () {
          $("#achCurrencyCode").val($("#Id").val().split('|')[0]);
          InitialiseAchCurrency('<%: Url.Action("GetCurrencyCodeName", "AchCurrencySetUp", new { area = "Masters"})%>');
      });
  </script>
</asp:Content>