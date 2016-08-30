﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MiscUatp.MiscSearchCriteria>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<div class="searchCriteria" style="width: 100%">
    <!-- CMP #596: Length of Member Accounting Code to be Increased to 12 
  Desc: Increasing field size from 85% to 120% to keep layout intact -->
    <div class="solidBox">
        <div class="fieldContainer horizontalFlow">
            <div style="width: 60%;">
                <div style="width: 100%;">
                    <div style="width: 20%; float: left;">
                        <label>
                            <span>*</span>Delivery Date From:</label>
                        <%:Html.TextBox(ControlIdConstants.DeliveryDateFrom, Model.DeliveryDateFrom != null ? Model.DeliveryDateFrom.Value.ToString(FormatConstants.DateFormat) : null, new { @class = "datePicker" })%>
                    </div>
                    <div style="width: 20%; float: left;">
                        <label>
                            <span>*</span> Delivery Date To:</label>
                        <%:Html.TextBox(ControlIdConstants.DeliveryDateTo, Model.DeliveryDateTo != null ? Model.DeliveryDateTo.Value.ToString(FormatConstants.DateFormat) : null, new { @class = "datePicker" })%>
                    </div>
                    <div style="width: 20%; float: left;">
                        <label>
                            Transaction Type :</label>
                        <%:Html.InvoiceTypeDropDownListFor(m => m.InvoiceTypeId)%>
                    </div>
                    <div style="width: 20%; float: left;">
                        <label>
                            Billing Member:</label>
                        <%:Html.HiddenFor(invoice => invoice.BillingMemberId)%>
                        <!-- CMP #596: Length of Member Accounting Code to be Increased to 12 
                            Desc: Increasing field size by specifying in-line width
                            Ref: 3.5 Table 19 Row 9 -->
                        <%:Html.TextBoxFor(invoice => invoice.BilledMemberText, new { @class = "autocComplete textboxWidth" })%>
                    </div>
                </div>
                <div style="width: 100%;">
                    <div style="width: 20%; float: left;">
                        <label>
                            Invoice/Credit Note Number:</label>
                        <%:Html.TextBoxFor(m => m.InvoiceNumber, new { maxLength = 10 })%>
                    </div>
                    <div style="width: 20%; float: left;">
                        <label>
                            SMI:</label>
                        <%:Html.SettlementMethodDropdownListFor(m => m.SettlementMethodId, InvoiceType.Invoice, TransactionMode.DailyMiscInvSearch)%>
                    </div>
                    <div style="width: 20%; float: left;">
                        <label>
                            Charge Category:</label>
                        <%--CMP609: MISC Changes Required as per ISW2. Added new parameter 'isIncludeInactive'. If it is true then method will return the all charge category for misc category including in-active.--%>
                        <%: Html.ChargeCategoryDropdownList(ControlIdConstants.ChargeCategory, Model.ChargeCategoryId, Model.BillingCategory, true, isIncludeInactive: true)%>
                    </div>
                    <% // Show Location only when BillingCategory is Miscellaneous
                        if (Model.BillingCategory == BillingCategoryType.Misc)
                        {%>
                    <div style="width: 20%; float: left;">
                        <label>
                            Location:</label>
                        <%:Html.TextBoxFor(invoice => invoice.LocationCode)%>
                    </div>
                    <%}%>
                </div>
            </div>
            <div style="width: 15%; margin-left: -100px">
                <% //CMP #655: IS-WEB Display per Location ID
                    //2.11	MISC IS-WEB PAYABLES - VIEW DAILY BILATERAL INVOICES SCREEN
                    if (Model.BillingCategory == BillingCategoryType.Misc)
                    {%>
                <label>
                    <span>*</span> Billed to Location ID:</label>
                <%:Html.ListBox("AssociatedLocation", (MultiSelectList)ViewData["AssociatedLocation"], new { @style = "width: 150px;height:100px;" })%>
                <%: Html.HiddenFor(m=>m.BillingMemberLoc)%>
                <%}%>
            </div>
        </div>
        <div class="clear">
        </div>
    </div>
</div>
<div class="clear">
</div>