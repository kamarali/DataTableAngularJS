<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Reports :: Passenger :: Payables - RM BM CM Summary
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            /*CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
            Ref: FRS Section 3.4 Table 15 Row 40 */
            registerAutocomplete('BillingEntityCode', 'BillingEntityCodeId', '<%:Url.Action("GetMemberListForPaxCgo", "Data",  new  {  area = "" })%>', 0, true, null);
        });
    </script>
    <%Html.BeginForm("", "", FormMethod.Post, new { id = "PayablesReport" }); %>
    <h1>
        Passenger: Payables - RM BM CM Summary
    </h1>
    <div>
        <% Html.RenderPartial("PayablesReportSearch", ViewData); %>
    </div>
    <div />
    <div class="buttonContainer">
        <input type="submit" id="generateReport" class="primaryButton" value="Generate Offline CSV Report" onclick="PayableReport(this);" />
    </div>
    <%Html.EndForm(); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script type="text/javascript" src="<%:Url.Content("~/Scripts/Reports/PayablesReport.js")%>"></script>
    <script type="text/javascript">

        function PayableReport(e) {
          ValidatePayablesReport("PayablesReport", '<%: Url.Action("PayablesReport", "PayablesReport") %>', '<%:Url.Action("ValidateBillingMonthYearPeriodSearch","SupportingMismatchDocument")%>');
        }

        //CMP-523: Source Code in Passenger RM BM CM Summary Reports
        var methodUrl = '<%: Url.Action("GetSourceCodesList", "Data",  new  {  area = "" }) %>';
        function MemoType_OnChange(e) {
            $.ajax({
                url: methodUrl,
                data: { transId: $("#MemoType option:selected").val() },
                dataType: "html",
                success: function (response) {
                    //alert(response);
                    $('#SourceCode').html(response);
                    //alert($('#SourceCode').html());
                }
            })
        };

        $(document).ready(function () {
            /*CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
            Ref: FRS Section 3.4 Table 15 Row 40 */
            registerAutocomplete('MemberCode', 'MemberId', '<%:Url.Action("GetMemberListForPaxCgo", "Data",  new  {  area = "" })%>', 0, true, null);
            $("#BillingEntityCode").change(function () {
                if ($("#BillingEntityCode").val() == '') {
                    $("#BillingEntityCodeId").val("");
                }
            });
        });
    </script>
</asp:Content>