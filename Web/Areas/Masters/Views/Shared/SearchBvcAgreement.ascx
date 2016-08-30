<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.BvcAgreement>" %>
<script type="text/javascript" language="javascript">
    $(document).ready(function () {
        registerAutocomplete('BillingMemberText', 'BillingMemberId', '<%:Url.Action("GetBVCMemberList", "Data", new { area = "", isBilling = true  })%>', 0, true, null);
        registerAutocomplete('BilledMemberText', 'BilledMemberId', '<%:Url.Action("GetBVCMemberList", "Data", new { area = "" , isBilling = false })%>', 0, true, null);
    $('#BillingMemberText').focus(); }
    );

    
</script>
<div>
    <h2>
        Search Criteria</h2>
    <div class="searchCriteriaMedium">
        <div class="solidBox">
            <div class="fieldContainer horizontalFlow">
                <div>
                    <div>
                        <label>
                            Billing Member:</label>
                        <%: Html.TextBoxFor(model => model.BillingMemberText, new { @class = "autocComplete"})%>
                        <%:Html.HiddenFor(invoice => invoice.BillingMemberId)%>
                    </div>
                    <div>
                        <label>
                            Billed Member:</label>
                        <%: Html.TextBoxFor(model => model.BilledMemberText, new { @class = "autocComplete"})%>
                        <%:Html.HiddenFor(invoice => invoice.BilledMemberId)%>
                    </div>
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
    </div>
</div>
