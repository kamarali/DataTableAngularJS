<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MemberProfile.AuditTrail>" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Reports :: SIS Usage Report
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <% Html.BeginForm("SISUsageReport", "SisUsageReport", FormMethod.Post, new { id = "SISUsageReport" }); %>
    <h1>SIS Usage Report</h1>
     
    <div>
        <% Html.RenderPartial("SearchControlForUsageReport", ViewData); %>
    </div>
    <div>
    </div>
    <div class="buttonContainer">
        <input type="submit" id="generateButton" class="primaryButton" value="Download Report"  />
    </div>
     <%Html.EndForm(); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script type="text/javascript" src='<%:Url.Content("~/Scripts/SISUsageReport.js")%>'></script>
    <script type="text/javascript">
        $(document).ready(function () {
          <% if (SessionUtil.MemberId == 0)
              {%>
            registerAutocomplete('MemberCode', 'MemberId', '<%:Url.Action("GetMemberList", "Data", new {area = ""})%>', 0, true, null);
            <%
              }
              %>
            ValidateSISUsageReport("SISUsageReport");
        }); 
  </script>
</asp:Content>
