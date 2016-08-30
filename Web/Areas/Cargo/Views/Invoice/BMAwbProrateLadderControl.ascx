<%--<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Cargo.BMAwbProrateLadder>" %>
<h2>
  Prorate Ladder Header</h2>
<div class="solidBox">
  <div class="fieldContainer horizontalFlowFor4FieldsPerLine" id="divOtherChargDetails">
    <div>
      <div>
        <label for="AwbSerialNumber">
          AWB Serial Number:</label>
        <%: Html.TextBoxFor(pl => pl.AwbSerialNumber, new { @readOnly = true })%>
      </div>
    </div>
    <div id="">
      <div>
        <label for="FromSector">
          <span>*</span> Prorate Calculate Currency:</label>
        <%: Html.CurrencyDropdownListFor(pl => pl.ProrateCalCurrencyId)%>
      </div>
      <div>
        <label for="ToSector">
          <span>*</span> Total Amount:</label>
        <%: Html.TextBoxFor(pl => pl.TotalAmount, new { min = 0, max = 99999999999.999, @class = "amount" })%>
      </div>
    </div>
    <%if (Model.Id != Guid.Empty)
      {%>
    <div class="buttonContainer">
      <input class="primaryButton ignoredirty" type="submit" value="Edit" onclick="javascript:return changeAction('<%: Url.Action("BMAwbProrateLadderUpdate","Invoice",new { couponId =Model.ParentId.Value(),  prorateLadderId= Model.Id.Value(), transaction="BMEdit" }) %>')" />
      <input class="primaryButton ignoredirty" type="submit" value="Delete" onclick="javascript:return changeAction('<%: Url.Action("BMAwbProrateLadderDelete","Invoice",new {  couponId =Model.ParentId.Value(), prorateLadderId= Model.Id.Value(), transaction="BMEdit" }) %>')" />
    </div>
    <%
      }%>
  </div>
  <div class="clear">
  </div>
</div>
--%>

<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Iata.IS.Model.Cargo.BMAwbProrateLadderDetail>>" %>
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
  <% Html.RenderPartial("BMAwbProrateLadderDetailsControl", new BMAwbProrateLadderDetail());%>
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
