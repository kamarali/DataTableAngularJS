<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Iata.IS.Model.Cargo.CMAwbProrateLadderDetail>>" %>
<%@ Import Namespace="Iata.IS.Model.Cargo" %>
<h2>
  Prorate Ladder Capture</h2>
<div class="solidBox dataEntry">
<form id ="formProrateHeader" action="">
<div class="fieldContainer horizontalFlow">
  <div>
    <div>
      <label for="ProrateCalCurrencyId">
        <span>*</span> Prorate Calculate Currency:</label>
      <%: Html.CurrencyDropdownList("ProrateCalCurrencyIdInPopup", "")%>
    </div>
    <div>
      <label for="TotalAmount">
        <span>*</span> Total Amount:</label>
      <%: Html.TextBox("TotalProrateAmountInPopup", "", new { @class = "amount pos_amt_11_3" })%>
    </div>
  </div>
</div>
</form>
 <div class="clear">
  </div>
</div>
<div>&nbsp;
</div>
<form id="formProrateLadderDetails" method="get" action=""> 
<div>
  <% Html.RenderPartial("CMAwbProrateLadderDetailsControl", new CMAwbProrateLadderDetail());%>
</div>
<div class="buttonContainer">
  <input class="primaryButton ignoredirty" type="submit" value="Add" />
</div>
</form>
<h2>Prorate Ladder List</h2>
<div>
  <table id="prorateLadderGrid">
  </table>
</div>
<div class="clear">
</div>
<div class="buttonContainer">
  <input class="secondaryButton" type="button" value="Close" onclick="closeProrateLadderDetail();" />
</div>
