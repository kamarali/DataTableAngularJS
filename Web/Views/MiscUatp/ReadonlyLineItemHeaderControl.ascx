<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MiscUatp.LineItem>" %>
<%@ Import Namespace="Iata.IS.AdminSystem" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<h2>
  Line Item</h2>
<div class="solidBox">
  <div class="fieldContainer horizontalFlow">
    <div>
      <div>
        Line Item Number: <strong>
          <%: Model.LineItemNumber %>
        </strong>
      </div>
      <div>
        Charge Code: <strong>
          <%: (Model.ChargeCode != null ? Model.ChargeCode.Name : string.Empty)%>
          <%: Html.Hidden("LineItemChargeCodeId", Model.ChargeCodeId) %>
        </strong>
      </div>
      <%if (Model.ChargeCodeTypeId != null && Model.ChargeCodeTypeId != 0)
        {%>
      <div>
        Charge Code Type: <strong>
          <%: Model.ChargeCodeType != null ? Model.ChargeCodeType.Name : string.Empty %>
          <%: Html.Hidden("LineItemChargeCodeTypeId", Model.ChargeCodeTypeId)%>
        </strong>
      </div>
      <%
        }%>
      <div>
        Service Start Date: <strong>
          <%: Model.StartDate != null ? Model.StartDate.Value.ToString(FormatConstants.DateFormat) : string.Empty %></strong>
      </div>
      <div>
        Service End Date: <strong>
          <%: Model.EndDate.ToString(FormatConstants.DateFormat) %></strong>
      </div>
      <%if (Model.Invoice.BillingCategory == BillingCategoryType.Misc)
        {%>
      <div>
        Location Code: <strong>
          <%:Model.LocationCode%></strong>
      </div>
      <%
        }%>
      <div>
        Quantity: <strong>
          <%: Model.Quantity %></strong>
      </div>
      <div>
        UOM Code: <strong>
          <%: Model.UomCodeId != null ? Model.UomCodeId : string.Empty%></strong>
      </div>
      <div>
        Unit Price: <strong>
          <%: Model.UnitPrice.ToString(FormatConstants.FourDecimalsFormat) %></strong>
      </div>
      <div>
        Scaling Factor: <strong>
          <%: Model.ScalingFactor %></strong>
      </div>
      <div>
        Line Total: <strong>
          <%: Model.ChargeAmount.ToString(FormatConstants.ThreeDecimalsFormat) %></strong>
      </div>
      <div>
        Tax: <strong>
          <%: Model.TotalTaxAmount.HasValue ? Model.TotalTaxAmount.Value.ToString(FormatConstants.ThreeDecimalsFormat) : "0.000"%></strong>
      </div>
      <div>
        VAT:<strong>
          <%: Model.TotalVatAmount.HasValue ? Model.TotalVatAmount.Value.ToString(FormatConstants.ThreeDecimalsFormat) : "0.000"%></strong>
      </div>
      <div>
        Add Charge/ Deduction:<strong>
          <%: Model.TotalAddOnChargeAmount.HasValue ? Model.TotalAddOnChargeAmount.Value.ToString(FormatConstants.ThreeDecimalsFormat) : "0.000"%></strong>
      </div>
      <div>
        Line Net Total: <strong>
          <%: Model.TotalNetAmount.ToString(FormatConstants.ThreeDecimalsFormat)%></strong>
      </div>
    </div>
    <div style="overflow: hidden;">
      Description: <strong>
        <%: Model.Description %></strong>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
