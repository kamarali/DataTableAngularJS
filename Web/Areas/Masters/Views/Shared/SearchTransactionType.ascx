<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.TransactionType>" %>
<script type="text/javascript" language="javascript">    $(document).ready(function () { $('#Name').focus(); });   </script>
<div>
    <h2>
        Search Criteria</h2>
    <div >
        <div class="solidBox">
            <div class="fieldContainer horizontalFlow">
                <div>
                    <div>
                        <label>
                            Transaction Type:</label>
                        <%: Html.TextBoxFor(model => model.Name, new { @class = "upperCase", @maxLength = 50 })%>
                        <%: Html.ValidationMessageFor(model => model.Name) %>
                    </div>
                    <div>
                        <label>
                            Billing Category:</label>
                        <%= Html.BillingCategoryDropdownListFor(model => model.BillingCategoryCode)%>
                    </div>
                    <div style="width:400px;">
                        <label>
                            Description:</label>
                        <%: Html.TextBoxFor(model => model.Description, new { @id = "Description", @maxLength = 255 })%>                                               
                        <%: Html.ValidationMessageFor(model => model.Description)%>
                    </div>
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
    </div>
</div>
