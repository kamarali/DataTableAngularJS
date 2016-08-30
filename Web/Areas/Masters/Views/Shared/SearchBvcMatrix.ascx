<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.Common.BvcMatrix>" %>
<script type="text/javascript" language="javascript">
    $(document).ready(function () {
        $('#EffectiveFrom').watermark("YYYYMMPP");
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
                            Validated PMI:</label>
                        <%: Html.TextBoxFor(model => model.ValidatedPmi, new { @class = "alphaNumeric upperCase", @maxLength = 1 })%>
                    </div>
                    <div>
                        <label>
                            Effective From:</label>
                        <%= Html.TextBoxFor(model => model.EffectiveFrom, new { @class = "numeric", @maxLength = 8 })%>
                    </div>
                    <div>
                        <label>
                            Effective To:</label>
                        <%= Html.TextBoxFor(model => model.EffectiveTo, new { @class = "numeric", @maxLength = 8 })%>
                    </div>
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
    </div>
</div>
