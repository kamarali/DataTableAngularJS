<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.PrimeCouponVat>" %>
<script type="text/javascript">
  $(document).ready(function () {
    $("#VATPercentage").blur(function () {
      //trim the whitespace
      var couponGrossValue = $('#VATBaseAmount').val().replace(/^\s\s*/, '').replace(/\s\s*$/, '');
      setVATCalculatedAmount(couponGrossValue);
    }); //end blur

    $("#VATBaseAmount").blur(function () {
      //trim the whitespace
      var couponGrossValue = $('#VATBaseAmount').val().replace(/^\s\s*/, '').replace(/\s\s*$/, '');
      setVATCalculatedAmount(couponGrossValue);
    }); //end blur

    function setVATCalculatedAmount(couponGrossValue) {
      //trim white space
      var vatValue = $("#VATPercentage").val().replace(/^\s\s*/, '').replace(/\s\s*$/, '');
      var vatPercentage = vatValue / 100 * couponGrossValue;
      if (!isNaN(vatPercentage))
        $("#VATCalculatedAmount").val(vatPercentage.toFixed(2));
    }
  });
</script>
<h2>
  VAT Breakdown Capture</h2>
<% Html.BeginForm(); %>
<div class="solidBox">
  <div class="fieldContainer horizontalFlow">
    <div>
      <div>
        <label>
          VAT Identifier</label>      
        <%:Html.TextBoxFor(vat => vat.VatIdentifier)%>
      </div>
      <div>
        <label>
          VAT Label</label>
        <%:Html.TextBoxFor(vat=>vat.VatLabel)%>
      </div>
      <div>
        <label>
          VAT Text</label>
        <%: Html.TextBoxFor(vat => vat.VatText, new { @class = "largeTextField alphaNumericWithSpace" })%>
      </div>
    </div>
    <div>
      <div>
        <label>
          VAT Base Amount</label>
        <%:Html.TextBoxFor(vat=>vat.VatBaseAmount, new { @class = "amount" })%>
      </div>
      <div>
        <label>
          VAT Percentage</label>
        <%:Html.TextBoxFor(vat => vat.VatPercentage, new { @class = "percent amt_5_3" })%>
      </div>
      <div>
        <label>
          VAT Calculated Amount</label>
        <%: Html.TextBoxFor(vat=>vat.VatCalculatedAmount, new {@readonly = true} )%>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
<div class="buttonContainer">
  <input class="primaryButton" type="button" value="Add" />
</div>
<div class="clear">
</div>
<% Html.EndForm(); %>
