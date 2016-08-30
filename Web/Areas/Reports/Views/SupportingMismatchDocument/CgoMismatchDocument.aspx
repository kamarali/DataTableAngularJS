<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MemberProfile.AuditTrail>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    CGO Supporting Attachments Mismatch Report
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <script type="text/javascript">
      $(document).ready(function () {

          $("#MemberCode").change(function () {
              if ($("#MemberCode").val() == '') {
                  $("#MemberId").val("");
              }
          });
          /*CMP #596: Length of Member Accounting Code to be Increased to 12 
          Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
          Ref: FRS Section 3.4 Table 15 Row 47 */
          registerAutocomplete('MemberCode', 'MemberId', '<%:Url.Action("GetMemberListForPaxCgo", "Data",  new  {  area = "" })%>', 0, true, null);
      });
  </script>
  <% Html.BeginForm("", "", FormMethod.Post, new { id = "MismatchDoc" }); %>
    <h1>Cargo Supporting Attachments Mismatch Report</h1>
     
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
//        $(document).ready(function () {

//            //ValidateMismatchDocument();
//        });

        function GenerateReport() {

            var cat = '<%=ViewData["Category"].ToString() %>';
            ValidateMismatchDocument("MismatchDoc", cat, '<%:Url.Action("ValidateBillingMonthYearPeriodSearch","SupportingMismatchDocument")%>');
        }
 
  </script>
</asp:Content>
