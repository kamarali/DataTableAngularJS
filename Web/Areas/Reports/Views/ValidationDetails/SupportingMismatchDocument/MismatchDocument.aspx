<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MemberProfile.AuditTrail>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Receivables - Supporting Documents Mismatch
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

  <% Html.BeginForm("", "", FormMethod.Post, new { id = "MismatchDoc" }); %>
    <h1>Receivables - Supporting Documents Mismatch</h1>
     
    <div>
        <% Html.RenderPartial("SearchControlForMismatchDoc", ViewData); %>
    </div>
    <div>
    </div>
    <div class="buttonContainer">
        <input type="submit" id="generateButton" class="primaryButton" value="Generate Report" onclick="GenerateReport();" />
    </div>
     <%Html.EndForm(); %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Reports/MismatchDocument.js")%>"></script>
      <script type="text/javascript">
          $(document).ready(function () {
              var cat = '<%=ViewData["CategoryName"].ToString() %>';
              $('#headers').text("CGO Supporting Attachments Mismatch Report");
              registerAutocomplete('MemberCode', 'MemberId', '<%:Url.Action("GetMemberList", "Data",  new  {  area = "" })%>', 0, true, null);

          });

          function GenerateReport() {
              var cat = '<%=ViewData["Category"].ToString() %>';
                 ValidateMismatchDocument("MismatchDoc", cat);

          }
</script>
</asp:Content>
