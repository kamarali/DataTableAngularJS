<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.LegalArchive.LegalArchiveSearchCriteria>" %>
<script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
<script type="text/javascript">
    $(document).ready(function () {

        $("#MemberText").change(function () {
            if ($("#MemberText").val() == '') {
                $("#MemberId").val("");
            }
        });
        registerAutocomplete('MemberText', 'MemberId', '<%:Url.Action("GetMemberList", "Data", new { area = "" })%>', 0, true, null);

        $("#btnSearch").bind('click', function () {
            var selectedLocationIds = '';
            $("#AssociatedLocation option:selected").each(function () {
                selectedLocationIds = selectedLocationIds + ',' + $(this).text();
            });

            $("#ArchivalLocationId").val(selectedLocationIds);
            return true;
        });
    });

   
</script>
<div class="searchCriteria">
    <!-- CMP #596: Length of Member Accounting Code to be Increased to 12 
  Desc: Increasing field size from 85% to 120% to keep layout intact -->
    <div class="solidBox" style="width: 120%">
        <div class="fieldContainer horizontalFlow">
            <div style="width: 85%; float: left;">
                <div>
                    <label>
                        Invoice Number:</label>
                    <%:Html.TextBoxFor(m => m.InvoiceNumber, new { maxLength = 10 })%>
                </div>
                <div>
                    <label>
                        <span>*</span>Type:</label>
                    <%:Html.BillingTypeDropdownListForLegalArchieve("Type", Model.Type)%>
                </div>
                <div>
                    <label>
                        <span>*</span>Billing Year:</label>
                    <%--SCP221779: old invoices in SIS [Billing Year Dropdown value does't holds during Page post]--%>
                    <%: Html.BillingYearDropdownListForLegalArchieve("BillingYear", Model.BillingYear)%>
                </div>
                <div>
                    <label>
                        Billing Month:</label>
                    <%: Html.MonthsDropdownListFor(m => m.BillingMonth)%>
                </div>
                <div>
                    <label>
                        Billing Period:</label>
                    <%:Html.StaticBillingPeriodDropdownListFor(m => m.BillingPeriod, true)%>
                </div>
            </div>
            <div style="width: 85%; float: left;">
                <div>
                    <label>
                        Member:</label>
                    <%:Html.HiddenFor(m => m.MemberId)%>
                    <!-- CMP #596: Length of Member Accounting Code to be Increased to 12 
                    Desc: Increasing field size by specifying in-line width
                    Ref: 3.5 Table 19 Row 22 -->
                    <%:Html.TextBoxFor(m => m.MemberText, new { @class = "autocComplete textboxWidth" })%>
                </div>
                <div>
                    <label>
                        Billing Category:</label>
                    <%: Html.BillingCategoryDropdownListFor(m => m.BillingCategoryId)%>
                </div>
                <div>
                    <label>
                        Billing Location Country:</label>
                    <%:Html.CountryCodeDropdownListForLegalArchieve(m => m.BillingCountryCode)%>
                </div>
                <div>
                    <label>
                        Billed Location Country:</label>
                    <%:Html.CountryCodeDropdownListForLegalArchieve(m => m.BilledCountryCode)%>
                </div>
                <div>
                    <label>
                        Settlement Method:</label>
                    <%:Html.SettlementMethodStatusDropdownListForLegalArchieve("SettlementMethodId", Model.SettlementMethodId)%>
                </div>
            </div>
        <div style="width: 15%; margin-top:-50px;">
            <label>
                Locations (MISC Only):</label>
            <%:Html.ListBox("AssociatedLocation", (MultiSelectList)ViewData["AssociatedLocation"], new { @style = "width: 150px;height:90px;" })%>
            <%: Html.HiddenFor(m=>m.ArchivalLocationId)%>
        </div>
        <div style="width: 15%;"> </div>

    </div>
    <div class="clear">
    </div>
</div>
</div> <div class="buttonContainer"> <input class="primaryButton" type="submit" value="Search" id="btnSearch" /> </div> <div class="clear"> </div> 