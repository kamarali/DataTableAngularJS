<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="aboutTitle" ContentPlaceHolderID="TitleContent" runat="server">
  Execution Context
</asp:Content>
<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
  <h1>Set Test Execution Context</h1>
  <h2>
    Select Billing Member</h2>
  <p>
    <b>
      <%: string.Format("Current Billing Member is: {0}", SessionUtil.MemberId) %></b>
  </p>
  <p>
    <%
      using (Html.BeginForm("SetUserSession", "Home", FormMethod.Post))
      {
    %>
    Billing Member:
    <input name="BillingMemberText" id="BillingMemberText" class="autocComplete" type="text" />
    <input name="BillingMemberId" id="BillingMemberId" type="hidden" />
    <input type="submit" class="tempButton" value="Set Billing Member (For testing)" />
    <%
      }
    %>
  </p>
  <hr />
  <h2>
    Unsaved Warning Messages</h2>
  <p>
  <p>
    <%
      using (Html.BeginForm("EnableWarningMessages", "Home", FormMethod.Post))
      {
    %>
    Show warning messages for unsaved data:
    <%: Html.CheckBox("unsavedWarningMessages", SessionUtil.UnsavedWarningMessagesEnabled)%>
    <input type="submit" class="tempButton" value="Save (For testing)" />
    <%
      }
    %>
  </p>

</asp:Content>
<asp:Content ID="scriptContent" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">
    $(document).ready(function () {
      registerAutocomplete('BillingMemberText', 'BillingMemberId', '<%:Url.Action("GetMemberList", "Data", new { area = "" })%>', 0, true, null);
    });
  </script>
</asp:Content>
