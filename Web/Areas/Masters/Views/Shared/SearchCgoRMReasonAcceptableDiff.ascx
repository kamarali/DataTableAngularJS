<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.CgoRMReasonAcceptableDiff>" %>
<script type="text/javascript" language="javascript">
    $(document).ready(function () {
        $('#EffectiveFrom').datepicker({
            dateFormat: "yymmdd"
        });
        $('#EffectiveFrom').watermark("YYYYMMPP");
        $('#EffectiveTo').datepicker({
            dateFormat: "yymmdd"
        });
        $('#EffectiveTo').watermark("YYYYMMPP");
    });
      
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
                            Transaction Type:</label>
                        <%: Html.TransactionTypeDropdownListFor(model => model.TransactionTypeId,2)%>
                    </div>
                    <div>
                        <label>
                            Reason Code:</label>
                        <%: Html.ReasonCodeDropdownListFor(model => model.ReasonCodeId, Model!=null ?Model.TransactionTypeId:0)%>
                    </div>
                    <div>
                        <label>
                            Effective From:</label>
                        <%:Html.TextBox("EffectiveFrom", null, new {  @id = "EffectiveFrom" })%>
                    </div>
                    <div>
                        <label>
                            Effective To:</label>
                        <%:Html.TextBox("EffectiveTo", null, new { @id = "EffectiveTo" })%>
                    </div>
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
    </div>
</div>

