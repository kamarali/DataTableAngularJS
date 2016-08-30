<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.ReasonCode>" %>
<script type="text/javascript" language="javascript">    $(document).ready(function () { $('#Code').focus(); });   </script>
<div>
  <h2>
    Search Criteria</h2>
  <div class="searchCriteriaMedium">
    <div class="solidBox">
      <div class="fieldContainer horizontalFlow">
        <div>
        <div>
            <label>
             Reason Code:</label>
            <%: Html.TextBoxFor(model => model.Code, new { @class = "alphaNumeric upperCase", @maxLength = 5 })%>
          </div>
           <div >
               <label>Transaction Type:</label>
                 <%: Html.TransactionTypeDropdownListFor(model => model.TransactionTypeId) %>
            </div>
        </div>
      </div>
      <div class="clear">
      </div>
    </div>
  </div>
</div>

