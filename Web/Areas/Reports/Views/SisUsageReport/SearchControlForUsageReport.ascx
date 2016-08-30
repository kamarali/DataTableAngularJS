<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Reports.ProcessingDashBoard.SisUsageReportModel>" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<script type="text/javascript">
    $(document).ready(function () {
        $('#FromBillingYear').focus();
        $('#FromBillingYear').val('<%=ViewData["currentYear"]%>');
        $('#ToBillingYear').val('<%=ViewData["currentYear"]%>');
        $('#FromBillingMonth').val('<%=ViewData["currentMonth"]%>');
        $('#ToBillingMonth').val('<%=ViewData["currentMonth"]%>');
        $('#FromPeriod').val('<%=ViewData["Period"] %>');
        $("#FromPeriod option[value='']").remove();
        $('#ToPeriod').val('<%=ViewData["Period"] %>');
        $("#ToPeriod option[value='']").remove();

        if ('<%=ViewData["UserCategory"]%>' == 4) {
            $('#participantTypeLabel').hide();
            $('#participantDropdown').hide();
        }
    });
  
</script>
<h2>
    Search Criteria</h2>
<div class="solidBox">
    <div class="fieldContainer horizontalFlow">
        <div>
            <div>
                <label>
                    <span class="required">* </span>From Billing Year:
                </label>
                <%:
                Html.BillingYearDropdownListFor(model => model.FromBillingYear)  %>
                <label>
                    <span class="required">* </span>To Billing Year:
                </label>
                <%: Html.BillingYearDropdownListFor(model => model.ToBillingYear)  %>
                <label>
                    Download File Format:</label>
                <%: Html.DownoadFileTypesDropdownListForSisUsageReport(model => model.DownloadFileFormats)%>
            </div>
            <div>
                <label>
                    <span class="required">* </span>From Billing Month:
                </label>
                <%: Html.BillingMonthDropdownListFor(model => model.FromBillingMonth)  %>
                <label>
                    <span class="required">* </span>To Billing Month:
                </label>
                <%: Html.BillingMonthDropdownListFor(model => model.ToBillingMonth)  %>
                <%if (SessionUtil.MemberId == 0)
                  {%>
                <label>
                    Member Prefix:</label>
                 <!--   CMP#596: Length of Member Accounting Code to be Increased to 12 
                        Desc: Non layout related IS-WEB screen changes.
                        Ref: FRS Section 3.1 Table 2, Section 3.2 Table 8,Section 3.3 Table 12, Section 3.5 Table20 */ -->
                <%:Html.TextBoxFor(model => model.MemberCode, new { @class = "autocComplete textboxWidth" })%>
                <%:Html.TextBoxFor(model => model.MemberId, new {@class = "hidden"})%>
                <% }%>
            </div>
            <div>
                <label>
                    <span class="required">* </span>From Period:
                </label>
                <%: Html.StaticBillingPeriodDropdownListFor(model => model.FromPeriod)%>
                <label>
                    <span class="required">* </span>To Period:
                </label>
                <%: Html.StaticBillingPeriodDropdownListFor(model => model.ToPeriod)%>
                <label id="participantTypeLabel">
                    Participant Type:</label>
                <%: Html.IchCategoryDropdownList("ParticipantType", "", new { @class = "currentFieldValue", @id = "participantDropdown" })%>
            </div>
        </div>
    </div>
    <div class="clear">
    </div>
</div>
