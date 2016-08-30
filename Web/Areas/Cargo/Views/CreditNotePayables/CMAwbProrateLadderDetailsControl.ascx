<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Cargo.CMAwbProrateLadderDetail>" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>

<div class="solidBox">
  <div class="fieldContainer horizontalFlow" id="divProrateLadderDetails">
    <div>
      <div>
        <label for="FromSector">
          <span>*</span> From Sector:</label>
        <%: Html.TextBoxFor(pl => pl.FromSector, new { @class = "upperCase alphabetsOnly", maxlength = 4 })%>
        <%: Html.Hidden("ProrateLadderId", Model.Id)%>
      </div>
      <div>
        <label for="ToSector">
          <span>*</span> To Sector:</label>
        <%: Html.TextBoxFor(pl => pl.ToSector, new { @class = "upperCase alphabetsOnly", maxlength = 4 })%>
      </div>
      <div>
        <label for="CarrierPrefix">
          <span>*</span> Carrier Prefix:</label>
        <%: Html.TextBoxFor(pl => pl.CarrierPrefix, new { @class = "upperCase autocComplete populated", maxlength = 3 })%>
      </div>
      <div>
        <label for="ProvisoReqSpa">
           Proviso/Req/Spa:</label>
        <%:Html.ProvisoreqspaDropdownList(pl => pl.ProvisoReqSpa)%>
      </div>
    </div>
    <div>
      <div>
        <label for="SequenceNumber">
          <span>*</span> Serial No.:</label>
        <%: Html.TextBoxFor(pl => pl.SequenceNumber, new { @class = "digits", @maxlength = 5 })%>
      </div>
      <div>
        <label for="ProrateFactor">
           Prorate Factor:</label>
        <%: Html.TextBoxFor(pl => pl.ProrateFactor, new { @class = "digits", maxlength = 10 })%>
      </div>
      <div>
        <label for="PercentShare">
           Percent Share:</label>
        <%: Html.TextBoxFor(pl => pl.PercentShare, new { @class = "percent amt_6_3" })%>
      </div>
      <div>
        <label for="Amount">
           Amount:</label>
        <%: Html.TextBoxFor(pl => pl.Amount, new { @class = "amountTextfield amount amt_11_3" })%>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
