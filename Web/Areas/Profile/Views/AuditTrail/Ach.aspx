<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MemberProfile.SearchEntity>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  View Ach Profile Updates
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Member/AuditTrail.js")%>"></script>
  <script type="text/javascript">
    $(document).ready(function () {
      validateAuditTrail("ach", "achAuditTrail");
      if ($("#userCategory").val() == 1)
        registerAutocomplete('CommercialName', 'Id', '<%:Url.Action("GetMemberListForIchOrAch", "Data", new { area = "",menuType="ach" })%>', 0, true, function (selectedId) { getMemberUser(selectedId); });
      else
        registerAutocomplete('CommercialName', 'Id', '<%:Url.Action("GetMemberListForIchOrAch", "Data", new { area = "",menuType="ach" })%>', 0, true, null);
    });
  </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<% Html.BeginForm("", "", FormMethod.Post, new { id = "achAuditTrail" }); %>
  <h1>Profile Changes - ACH</h1>
   <h2>Search Criteria</h2>
  <div>
    <% Html.RenderPartial("~/Areas/Profile/Views/Shared/ProfileUpdateControl.ascx"); %>
    <% Html.RenderPartial("SearchControl"); %>
  </div>
  <div class="buttonContainer">
    <input type="submit" class="primaryButton" value="Generate Report" />
  </div>
   <%Html.EndForm(); %>
</asp:Content>
