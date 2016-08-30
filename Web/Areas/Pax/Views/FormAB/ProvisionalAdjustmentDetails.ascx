<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.InvoiceTotal>" %>
<h2>Provisional Adjustment Details</h2>

<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div>
      <div>
        
      </div>
      <div style="width:100px;">
        <b>Percentage</b>
      </div>
      <div style="width:100px;">
        <b>Amount</b>
      </div>
    </div>
    <div>
      <div>
        Fare Absorption
      </div>
      <div style="width:100px; text-align:center;">
        <% if (Model.FareAbsorptionPercent >= 0)
           {%> <%: Model.FareAbsorptionPercent%>

        <%
           } else
           { %>
             <%: "(" + Math.Abs(Model.FareAbsorptionPercent) + ")" %>
           <%}
        %>
      </div>
      <div style="width:70px; text-align:center;">
         <% if (Model.FareAbsorptionAmount >= 0)
           {%> <%: Model.FareAbsorptionAmount%>

        <%
           } else
           { %>
             <%: "(" + Math.Abs(Model.FareAbsorptionAmount) + ")"%>
           <%}
        %>
      </div>
    </div>
    <div>
      <div>
        ISC Absorption
      </div>
      <div style="width:100px; text-align:center;">
        <% if (Model.IscAbsorptionPercent >= 0)
           {%> <%: Model.IscAbsorptionPercent%>
        <%
           } else
           { %>
             <%: "(" + Math.Abs(Model.IscAbsorptionPercent) + ")"%>
           <%}
        %>
      </div>
      <div style="width:70px; text-align:center;">
        <% if (Model.IscAbsorptionAmount >= 0)
           {%> <%: Model.IscAbsorptionAmount%>
        <%
           } else
           { %>
             <%: "(" + Math.Abs(Model.IscAbsorptionAmount) + ")"%>
           <%}
        %>
      </div>
    </div>
    <div>
      <div>
        Other Commission Absorption
      </div>
      <div style="width:100px; text-align:center;">
        <% if (Model.OtherCommissionAbsorptionPercent >= 0)
           {%> <%: Model.OtherCommissionAbsorptionPercent%>
        <%
           } else
           { %>
             <%: "(" + Math.Abs(Model.OtherCommissionAbsorptionPercent) + ")"%>
           <%}
        %>
      </div>
      <div style="width:70px; text-align:center;">
        <% if (Model.OtherCommissionAbsorptionAmount >= 0)
           {%> <%: Model.OtherCommissionAbsorptionAmount%>
        <%
           } else
           { %>
             <%: "(" + Math.Abs(Model.OtherCommissionAbsorptionAmount) + ")"%>
           <%}
        %>
      </div>
    </div>
    <div>
      <div>
        UATP Absorption
      </div>
      <div style="width:100px; text-align:center;">
        <% if (Model.UatpAbsorptionPercent >= 0)
           {%> <%: Model.UatpAbsorptionPercent%>
        <%
           } else
           { %>
             <%: "(" + Math.Abs(Model.UatpAbsorptionPercent) + ")"%>
           <%}
        %>
      </div>
      <div style="width:70px; text-align:center;">
        <% if (Model.UatpAbsorptionAmount >= 0)
           {%> <%: Model.UatpAbsorptionAmount%>
        <%
           } else
           { %>
             <%: "(" + Math.Abs(Model.UatpAbsorptionAmount) + ")"%>
           <%}
        %>
      </div>
      </div>
    <div>
    <div>
      Handling Fee Absorption
    </div>
    <div style="width:100px; text-align:center;">
        <% if (Model.HandlingFeeAbsorptionPercent >= 0)
           {%> <%: Model.HandlingFeeAbsorptionPercent%>
        <%
           } else
           { %>
             <%: "(" + Math.Abs(Model.HandlingFeeAbsorptionPercent) + ")"%>
           <%}
        %>
    </div>
    <div style="width:70px; text-align:center;">
        <% if (Model.HandlingFeeAbsorptionAmount >= 0)
           {%> <%: Model.HandlingFeeAbsorptionAmount%>
        <%
           } else
           { %>
             <%: "(" + Math.Abs(Model.HandlingFeeAbsorptionAmount) + ")"%>
           <%}
        %>
    </div>
  </div>
      <div>
    <div>
      Tax Absorption
    </div>
    <div style="width:100px; text-align:center;">
        <% if (Model.TaxAbsorptionPercent >= 0)
           {%> <%: Model.TaxAbsorptionPercent%>
        <%
           } else
           { %>
             <%: "(" + Math.Abs(Model.TaxAbsorptionPercent) + ")"%>
           <%}
        %>
    </div>
    <div style="width:70px; text-align:center;">
        <% if (Model.TaxAbsorptionAmount >= 0)
           {%> <%: Model.TaxAbsorptionAmount%>
        <%
           } else
           { %>
             <%: "(" + Math.Abs(Model.TaxAbsorptionAmount) + ")"%>
           <%}
        %>
    </div>
  </div>
      <div>
    <div>
      VAT Absorption
    </div>
    <div style="width:100px; text-align:center;">
        <% if (Model.VatAbsorptionPercent >= 0)
           {%> <%: Model.VatAbsorptionPercent%>
        <%
           } else
           { %>
             <%: "(" + Math.Abs(Model.VatAbsorptionPercent) + ")"%>
           <%}
        %>
    </div>
    <div style="width:70px; text-align:center;">
        <% if (Model.VatAbsorptionAmount >= 0)
           {%> <%: Model.VatAbsorptionAmount%>
        <%
           } else
           { %>
             <%: "(" + Math.Abs(Model.VatAbsorptionAmount) + ")"%>
           <%}
        %>
    </div>
  </div>
      <div>
    <div>
      Total Provisional Absorption
    </div>
    <div style="width:100px; text-align:center;">
        <% if (Model.ProvAdjustmentRate >= 0)
           {%> <%: Model.ProvAdjustmentRate%>
        <%
           } else
           { %>
             <%: "(" + Math.Abs(Model.ProvAdjustmentRate) + ")"%>
           <%}
        %>
    </div>
    <div style="width:70px; text-align:center;">
        <% if (Model.TotalProvisionalAdjustmentAmount >= 0)
           {%> <%: Model.TotalProvisionalAdjustmentAmount%>
        <%
           } else
           { %>
             <%: "(" + Math.Abs(Model.TotalProvisionalAdjustmentAmount) + ")"%>
           <%}
        %>
    </div>
  </div>
  </div>
    <div class="clear">
  </div>
</div>