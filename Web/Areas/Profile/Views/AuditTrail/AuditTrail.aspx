<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MemberProfile.AuditTrail>" %>
 

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Profile and User Management :: View Profile Changes 
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <% Html.BeginForm("", "", FormMethod.Post, new { id = "auditTrail" }); %>
    <h1>Audit Trail</h1>
     
    <div>
    <h2>Search Criteria</h2>
        <% Html.RenderPartial("SearchControl"); %>
    </div>
    <div>
    </div>
    <div class="buttonContainer">
        <input type="submit" id="generateButton" class="primaryButton" value="Generate Report" />
        <input type="button" class="secondaryButton" id="selectAll" onclick="selectAllCheckBox();"
            value="Select All" />
    </div>
     <%Html.EndForm(); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script type="text/javascript" src="<%:Url.Content("~/Scripts/Member/AuditTrail.js")%>"></script>
    <script type="text/javascript">
      $(document).ready(function () {
        validateAuditTrail("member", "auditTrail");
      });
  </script>
</asp:Content>
