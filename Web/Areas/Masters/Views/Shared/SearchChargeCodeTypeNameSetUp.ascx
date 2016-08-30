<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MiscUatp.Common.ChargeCodeType>" %>
<div>
  <h2>
    Search Criteria</h2>
  <div class="searchCriteriaMedium">
    <div class="solidBox">
      <div class="fieldContainer horizontalFlow">
        <div>
          <div>
            <label>
              Charge Category:</label>
            <%: Html.ChargeCategoryDropdownListForMstChargeCodeType(model => model.ChargeCategoryId, false, new { style = "width:130px" })%>
          </div>
          <div>
            <label>
              Charge Code:</label>
            <%: Html.ChargeCodeDropdownListForMstChargeCodeType(model => model.ChargeCodeId, Model != null ? Convert.ToInt32(Model.ChargeCategoryId) : 0, false, new { style = "width:130px" })%>
          </div>
          <div>
            <label>
              Charge Code Type Name:</label>
            <%: Html.TextBoxFor(model => model.Name, new { @maxlength = 50, style = "width:130px" })%>
          </div>
        </div>
      </div>
      <div class="clear">
      </div>
    </div>
  </div>
</div>
