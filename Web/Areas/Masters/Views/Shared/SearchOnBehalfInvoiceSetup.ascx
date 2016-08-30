<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.OnBehalfInvoiceSetup>" %>
<div>
    <h2>
        Search Criteria</h2>
    <div class="searchCriteriaMedium">
        <div class="solidBox">
            <div class="fieldContainer horizontalFlow">
                <div>
                <div>
                        <label>
                            Billing Category:</label>
                        <%: Html.BillingCategoryDropdownListFor(model => model.BillingCategoryId)%>
                    </div>
                    <div>
                        <label>
                            Transmitter Code:</label>
                        <%: Html.TextBoxFor(model => model.TransmitterCode, new { @class = "alphaNumeric upperCase", @maxLength = 50 })%>
                    </div>
                     <div>
                        <label>
                            Charge Category Code:</label>
                        <%: Html.ChargeCategoryDropdownList("ChargeCategoryId",Model!=null ? Model.ChargeCategoryId:0,Model!=null ?Model.BillingCategoryId:0)%>
                    </div>
                     <div>
                        <label>
                            Charge Code:</label>                                                          
                             <%: Html.ChargeCodeDropdownList("ChargeCodeId",Model!=null ?Model.ChargeCodeId:0,Model!=null ?Model.ChargeCategoryId:0)%>
                    </div>
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
    </div>
</div>
