<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.SandBox.CertificationTransactionDetailsReport>" %>
<script type="text/javascript">
    $(document).ready(function () {

        registerAutocomplete('MemberCode', 'MemberId', '<%:Url.Action("GetMemberList", "Data",  new  {  area = "" })%>', 0, true, null);
//        $('#MmbrId').val('<%= ViewData["MembrId"] %>');
        $('#FileSubmittedFromdate').val('<%=ViewData["CurrentDate"] %>');
        $('#FileSubmittedToDate').val('<%=ViewData["CurrentDate"] %>');

    });
</script>
<h2>
    Search Criteria</h2>
<div class="solidBox">
    <div class="fieldContainer horizontalFlow">
        <div>
            <div style="width: 180px;">
                <label>
                    From Date:<span style="color: Red">*</span></label>
                <%: Html.TextBoxFor(model => model.FileSubmittedFromdate, new {  @class = "datePicker", @readOnly = true })%>
            </div>
            <div style="width: 180px;">
                <label>
                    To Date:<span style="color: Red">*</span></label>
                <%: Html.TextBoxFor(model => model.FileSubmittedToDate, new {  @class = "datePicker", @readOnly = true })%>
            </div>
            <div style="width: 180px;">
                <label>
                    Member Code:</label>
                <!--    CMP#596: Length of Member Accounting Code to be Increased to 12 
                        Desc: Non layout related IS-WEB screen changes.
                        Ref: FRS Section 3.1 Table 2, Section 3.2 Table 8,Section 3.3 Table 12, Section 3.5 Table20 */ -->
                <%:Html.TextBoxFor(model => model.MemberCode, new { @class = "autocComplete textboxWidth" })%>
                <%:Html.TextBoxFor(model => model.MemberId, new { @class = "hidden" })%>
            </div>
        </div>
        <div>
            <div style="width: 180px;">
                <label>
                    File Type:</label>
                <%:Html.FileFormatDropdownListFor(searchCriteria => searchCriteria.FileFormatId)%>
            </div>
            <div style="width: 180px;">
                <label>
                    Billing Category:</label>
                <%:Html.BillingCategoryDropdownListFor(model => model.BillingCategoryId)%>
            </div>
            <div style="width: 180px;">
                <label>
                    Request Type:</label>
                <%:Html.SandboxRequestTyepDropdownListFor(model => model.RequestType)%>
            </div>
            <div style="width: 180px;">
                <label>
                    Group Status :</label>
                <%:Html.SandboxGroupStatusDropdownListFor(model => model.TransactionGroupId)%>
            </div>
        </div>
    </div>
    <div class="clear">
    </div>
</div>
