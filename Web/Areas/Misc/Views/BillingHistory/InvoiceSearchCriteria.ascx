<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MiscUatp.BillingHistory.InvoiceSearchCriteria>" %>
<h2>
    Invoice Search Criteria</h2>

   <%-- <td class="fieldContainer horizontalFlow">--%>

    <table class="solidBox dataEntry" style="width:100%;padding: 1em;">
          <tr>
            <td style="width:20%;">
                <label for="BillingType">
                    <span>*</span> Billing Type:</label>
                <%:Html.BillingTypeDropdownList("BillingTypeId", Model.BillingTypeId)%>
            </td>
            <td style="width:20%;">
                <label for="BillingYearMonth">
                    <span class="required">*</span> Billing Year / Month.:</label>
                <%: Html.BillingYearMonthDropdown(ControlIdConstants.BillingYearMonthDropDown, Model.BillingYear, Model.BillingMonth, TransactionMode.BillingHistory)%>
            </td>
            <td style="width:20%;">
                <label for="BillingPeriod">
                    <span class="required">*</span> Billing Period:</label>
                <%: Html.StaticBillingPeriodDropdownList(ControlIdConstants.BillingPeriod, Model.BillingPeriod.ToString(), TransactionMode.InvoiceSearch)%>
            </td>
            <td style="width:20%;">
                <label for="MemberCode">
                    <span class="required">*</span> Member Code:</label>
                <!-- CMP #596: Length of Member Accounting Code to be Increased to 12 
                         Desc: Increasing field size by specifying in-line width
                         Ref: 3.5 Table 19 Row 5 -->
                <%:Html.TextBoxFor(model => model.BilledMemberCode, new { @class = "autocComplete textboxWidth" })%>
                <%:Html.TextBoxFor(model => model.BilledMemberId, new { @class= "hidden" }) %>
            </td>

            <td rowspan="2" style="padding-bottom:0px;width:20%;">
                <label>
                    <span>*</span> Billed from/to Location ID:</label>
                <%:Html.ListBox("AssociatedLocation", (MultiSelectList)ViewData["AssociatedLocation"], new { @style = "width: 120px;height:80px;" })%>
                <%: Html.HiddenFor(m => m.MemberLocation)%>
            </td>
        </tr>
        <tr>
            <td>
                <label for="InvoiceNumber">
                    Invoice Number:</label>
                <%:Html.TextBoxFor(model => model.InvoiceNumber, new {maxLength = 10}) %>
            </td>
            <td>
                <label for="ChargeCategory">
                    Charge Category:</label>
                <%--CMP609: MISC Changes Required as per ISW2. Added new parameter 'isIncludeInactive'. If it is true then method will return the all charge category for misc category including in-active.--%>
                <%:Html.ChargeCategoryDropdownListFor(model => model.ChargeCategoryId, isIncludeInactive: true)%>
            </td>
            <td>
                <label for="OurReference">
                    Rejection Stage:</label>
                <%:Html.RejectionStageDropdownListFor(model => model.RejectionStageId, TransactionMode.MiscUatpInvoiceSearch)%>
            </td>
            <td style="display: none;">
                <label for="TransactionStatus">
                    Transaction Status:</label>
                <%: Html.TransactionStatusDropdownListFor(model => model.TransactionStatusId) %>
            </td>
        </tr>
    </table>
    <div class="clear">
    </div>
 
