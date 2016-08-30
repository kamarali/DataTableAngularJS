<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.SandBox.CertificationParameterMaster>" %>
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
                            File Format:</label>
                        <%: Html.FileFormatDropdownListFor(model => model.FileFormatId)%>
                    </div>
                    <div>
                        <label>
                            Transaction Type:</label>
                        <%:Html.TransactionTypeDropdownListFor(model => model.TransactionTypeId)%>
                    </div>
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
    </div>
</div>