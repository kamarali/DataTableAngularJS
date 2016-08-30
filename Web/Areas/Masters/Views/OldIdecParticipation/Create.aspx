<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.OldIdecParticipation>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  Old IDEC Participation
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  
  <h2>
    Add Old IDEC Participation</h2>
  <% using (Html.BeginForm("Create", "OldIdecParticipation", FormMethod.Post, new { @id = "OldIdecParticipationMaster" }))
     {%>
  <%: Html.ValidationSummary(true) %>
  <fieldset class="solidBox dataEntry">
    <div class="editor-label">
      <label>
        <span class="required">* </span>Member:</label>
      <%: Html.TextBoxFor(model => model.MemberText, new { @id = "MemberText", @Class = "autocComplete" })%>
      <%: Html.HiddenFor(model => model.MemberId, new { @id = "MemberId" })%>
      <%: Html.ValidationMessageFor(model => model.MemberId) %>
    </div>
    <div class="editor-label">
      <label>
        PAX Incoming Allowed:</label>
      <%: Html.CheckBoxFor(model => model.PaxIncomingAllowed) %>
      <%: Html.ValidationMessageFor(model => model.PaxIncomingAllowed) %>
    </div>
    <div class="editor-label">
      <label>
        PAX Outgoing Allowed:</label>
      <%: Html.CheckBoxFor(model => model.PaxOutGoingAllowed)%>
      <%: Html.ValidationMessageFor(model => model.PaxOutGoingAllowed) %>
    </div>
    <div class="editor-label">
      <label>
        CGO Incoming Allowed:</label>
      <%: Html.CheckBoxFor(model => model.CgoIncomingAllowed)%>
      <%: Html.ValidationMessageFor(model => model.CgoIncomingAllowed) %>
    </div>
    <div class="editor-label">
      <label>
        CGO Outgoing Allowed:</label>
      <%: Html.CheckBoxFor(model => model.CgoOutGoingAllowed)%>
      <%: Html.ValidationMessageFor(model => model.CgoOutGoingAllowed) %>
    </div>
    <div class="editor-label">
      <label>
        Active:</label>
      <%: Html.CheckBoxFor(model => model.IsActive, new { @checked = "checked" })%>
      <%: Html.ValidationMessageFor(model => model.IsActive) %>
    </div>
    <div class="buttonContainer">
      <input type="submit" value="Save" class="primaryButton" />
      <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","OldIdecParticipation") %>'" />
    </div>
  </fieldset>
  <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
 <script src="<%:Url.Content("~/Scripts/Masters/OldIdecParticipationValidate.js")%>" type="text/javascript"></script>
  <script type="text/javascript">

      $(document).ready(function () {
          registerAutocomplete('MemberText', 'MemberId', '<%:Url.Action("GetAllMemberList", "Data", new { area = "" })%>', 0, true, null);
      });
  </script>
</asp:Content>
