<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.Common.RMReasonAcceptableDiff>" %>
<script type="text/javascript" language="javascript">
    $(document).ready(function () {
//        $('#EffectiveFrom').datepicker({
//            dateFormat: "yymmdd"
//        });
        $('#EffectiveFrom').watermark("YYYYMMPP");
//        $('#EffectiveTo').datepicker({
//            dateFormat: "yymmdd"
//        });
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
                            <span class="required">* </span>Transaction Type:</label>
                        <%: Html.TransactionTypeDropdownListFor(model => model.TransactionTypeId,1)%>
                    </div>
                    <div>
                        <label>
                            <span class="required">* </span>Reason Code:</label>
                        <%: Html.ReasonCodeDropdownListFor(model => model.ReasonCodeId, Model!=null ?Model.TransactionTypeId:0)%>
                    </div>
                    <div>
                        <label>
                            Effective From:</label>
                        <%:Html.TextBox("EffectiveFrom", null, new { @id = "EffectiveFrom", @maxLength = 8 })%>
                    </div>
                    <div>
                        <label>
                            Effective To:</label>
                        <%:Html.TextBox("EffectiveTo", null, new { @id = "EffectiveTo", @maxLength = 8 })%>
                    </div>
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
    </div>
</div>
