<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.Sampling.SamplingFormDRecord>" %>
<div>
  <label for="ProrateSlip">
    Prorate Slip:</label>
  <%: Html.TextAreaFor(formD => formD.ProrateSlipDetails, 20, 160, ScrollBars.Both)%>
</div>
<div class="buttonContainer">
  <input class="primaryButton ignoredirty" type="submit" value="Save" />
  <input class="secondaryButton" type="button" value="Back" onclick="$('#divProrateSlip').dialog('close');" />
</div>

