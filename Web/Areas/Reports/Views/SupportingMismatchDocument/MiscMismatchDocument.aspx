<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MemberProfile.AuditTrail>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Reports :: Miscellaneous :: Supporting Attachments Mismatch Report
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <script type="text/javascript">
      $(document).ready(function () {

          $("#MemberCode").change(function () {
              if ($("#MemberCode").val() == '') {
                  $("#MemberId").val("");
              }
          });
          registerAutocomplete('MemberCode', 'MemberId', '<%:Url.Action("GetMemberList", "Data",  new  {  area = "" })%>', 0, true, null);
      });
  </script>
  <% Html.BeginForm("", "", FormMethod.Post, new { id = "MismatchDoc" }); %>
    <h1>Miscellaneous-Supporting Attachments Mismatch Report</h1>
     
    <div>
        <% Html.RenderPartial("SearchControlForMismatchDoc"); %>
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
      
        });

        function GenerateReport() {

            var cat = '<%=ViewData["Category"].ToString() %>';
            ValidateMismatchDocument("MismatchDoc", cat, '<%:Url.Action("ValidateBillingMonthYearPeriodSearch","SupportingMismatchDocument")%>');
        }
 
  </script>
</asp:Content>
