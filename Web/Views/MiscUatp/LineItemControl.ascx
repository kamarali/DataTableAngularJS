<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MiscUatp.LineItem>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<h2>
  Line Item
</h2>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div>
      <div>
        <label for="LineItemNumber">
          <strong>Line Item #: </strong>
        </label>
        <%:Html.TextBox(ControlIdConstants.LineItemNumber, Model.LineItemNumber, new { @readOnly = true, @class = "populated" })%>
      </div>
    </div>
    <div>
      <div>
        <label for="ChargeCodeId">
          <span>*</span> Charge Code:</label>
        <%:Html.ChargeCodeDropdownList(ControlIdConstants.ChargeCode, Model.ChargeCodeId, Model.Invoice.ChargeCategoryId)%>
      </div>
      <%
        if (Model.Invoice.BillingCategory == BillingCategoryType.Misc)
        {%>
      <div>
       <%-- CMP515: Add check location code is exist in invoice header for not.      --%>            
        <%
          if (!string.IsNullOrEmpty(Model.Invoice.LocationCode))
          {%> <label for="CityAirportText">
               Location (Airport/City Code):</label>
        <%:Html.TextBox(ControlIdConstants.LocationCode, Model.LocationCode, new { @readOnly = true, @class = "populated upperCase" })%>
         <%
          }
          else
          { %> <label for="CityAirportText">
               <span>*</span> Location (Airport/City Code):</label>
        <%:Html.TextBoxFor(lineItem => lineItem.LocationCode, new { maxLength = 5, @class = "alphaNumeric upperCase" })%>
        <%
          }%>
      </div>
      <%
        }%>
        <% if (Model.Invoice.BillingCategory == BillingCategoryType.Misc)
           {%>
      <div>
        <label>
          P.O. Line Number:</label>
        <%:Html.TextBoxFor(lineItem => lineItem.POLineItemNumber,
                                               new {@class = "numeric", maxLength = 6, min = 1, @value = ""})%>
        <%:Html.Hidden(ControlIdConstants.PONumber,
                                           Model.Invoice != null ? Model.Invoice.PONumber : string.Empty)%>
      </div>
      <div>
        <label>
          Product ID:</label>
        <%:Html.TextBoxFor(lineItem => lineItem.ProductId, new {maxLength = 25})%>
      </div>
      <%
           }%>
      <!-- If charge code type is not null then show charge code type. -->
      <div id="chargeCodeTypeDiv"
       <%
        if (ViewData[ViewDataConstants.IsChargeCodeTypeExists] == null)
        {%> class="hidden" <%
        }%>>    
        <label>
          Charge Code Type:</label>
        <%:Html.ChargeCodeTypeDropdownList(ControlIdConstants.ChargeCodeTypeId, Model.ChargeCodeTypeId, Model.ChargeCodeId, ViewData[ViewDataConstants.PageMode] == PageMode.View ? false: true)%>
      </div>
    </div>
    <%
        if (Model.Invoice.InvoiceType == InvoiceType.RejectionInvoice && Model.Invoice.BillingCategory == BillingCategoryType.Misc)
        {%>
    <div>
      <div>
        <label>
          Original Line Item Number:</label>
        <%:Html.TextBoxFor(lineItem => lineItem.OriginalLineItemNumber, new { maxLength = 6, min = 1, @class = "numeric" })%>

      </div>
      <%-- CMP#502 : [3.5] Rejection Reason for MISC Invoices--%>
      <div>
        <label>
         <span>*</span> Rejection Reason Code:</label>
        <%:Html.TextBoxFor(lineItem => lineItem.RejectionReasonCodeText, new { @class = "autocComplete upperCase" })%>
         <%:Html.HiddenFor(invoice => invoice.RejectionReasonCode)%>
      </div>
    </div>

    <%
        }%>
    <div>
      <div>
        <label>
          <span>*</span> Description:</label>
        <%:Html.TextAreaFor(lineItem => lineItem.Description, 4, 80, new { })%>
      </div>
    </div>
    <div>
      <div>
        <label>
          Service Start Date:</label>
        <%:Html.TextBox(ControlIdConstants.ServiceStartDate,
                                     Model.StartDate != null ? Model.StartDate.Value.ToString(FormatConstants.DateFormat) : null,
                                     new { @class = "datePicker populated" })%>
      </div>
      <div>
        <label>
          <span>*</span> Service End Date:</label>
        <%:Html.TextBox(ControlIdConstants.ServiceEndDate, Model.EndDate.ToString(FormatConstants.DateFormat), new { @class = "datePicker populated" })%>
      </div>
      <div>
        <label>
          <span>*</span> Quantity:</label>
        <%:Html.TextBoxFor(lineItem => lineItem.Quantity, new { @class = "populated pos_num_18_4", watermark = ControlIdConstants.FourDecimalPlaces, roundTo = 4 })%>
      </div>
      <div>
        <label>
          <span>*</span> UOM Code:</label>
        <%:Html.UomCodeDropdown(ControlIdConstants.UomCodeId, Model.UomCodeId)%>
      </div>
      <div>
        <label>
          <span>*</span> Unit Price:</label>
        <%:Html.TextBox(ControlIdConstants.UnitPrice, Model.UnitPrice, new { @class = "num_18_4", watermark = ControlIdConstants.FourDecimalPlaces, roundTo = 4 })%>
      </div>
    </div>
    <div>
      <div>
        <label>
          Scaling Factor:</label>
          <%if (Model.Invoice.BillingCategory == BillingCategoryType.Misc)
            {%>
        <%:Html.TextBoxFor(lineItem => lineItem.ScalingFactor, new { @class = "populated numeric", maxLength = 5, min = 1 })%>
        <%
            }
            else
            {%>
            <%:Html.TextBoxFor(lineItem => lineItem.ScalingFactor, new {@class = "populated numeric", maxLength = 5, min = 1, @readonly = true })%>
            <%
            }%>
      </div>
      <div>
        <label for="ChargeAmount">
          Line Total:</label>
        <%
        if (Model.MinimumQuantityFlag)
        {%>
        <%:Html.TextBoxFor(lineItem => Model.ChargeAmount, new { @class = "num_18_3 amount", watermark = ControlIdConstants.PositiveAmount })%>
        <%
        }%>
        <%
        else
        {%>
        <%:Html.TextBoxFor(lineItem => Model.ChargeAmount, new { @readonly = true, @class = "num_18_3 amount", watermark = ControlIdConstants.PositiveAmount })%>
        <%
        }%>
      </div>
      <div>
        <%:ScriptHelper.GenerateDialogueHtml("Tax Amount:", "Tax capture", "TaxBreakdown", 500, 900)%><br/>
        <%:Html.TextBox("TotalTaxAmount", Model.TotalTaxAmount, new { @class = "amountTextfield num_18_3 amount", @readOnly = true, @id = "TaxAmount" })%>
      </div>
      <div>
        <%:ScriptHelper.GenerateDialogueHtml(" VAT Amount:", "VAT Capture", "VATBreakdown", 500, 900)%><br/>
        <%:Html.TextBox("TotalVatAmount", Model.TotalVatAmount, new { @class = "amountTextfield num_18_3 amount", @readOnly = true, @id = "VatAmount" })%>
      </div>
      <div>
        <%:ScriptHelper.GenerateDialogueHtml("Add/Deduct Charge:", "Add/Deduct Charge Capture", "AddChargeBreakdown", 500, 900)%><br/>
        <%:Html.TextBox("TotalAddOnChargeAmount", Model.TotalAddOnChargeAmount, new { @class = "amountTextfield num_18_3 amount", @readOnly = true, id = "TotalAddChargeAmount" })%>
      </div>
    </div>
    <div>
      <div>
        <label for="TotalNetAmount">
          Line Net Total:</label>
        <%:Html.TextBox(ControlIdConstants.LineNetTotal, Model.TotalNetAmount, new { @readOnly = true, @class = "num_18_3 amount" })%>
      </div>   
      <div id="MinimumQuantityIndicator">
        <label for="MinimumQuantityFlag">
          Minimum Quantity Flag:</label>
        <%:Html.CheckBoxFor(lineItem => lineItem.MinimumQuantityFlag)%>
      </div>     
    </div>
    <div id="MainAddDetail" class="topLine">
      <div>
        <label>
          Additional Details:</label>
        <%:Html.TextBox(ControlIdConstants.AdditionalDetailDropdown, string.Empty, new { @class = "autocComplete additionalDetailRequired" })%>
      </div>
      <div class="dynamicTextArea">
        <label>
          Additional Details Description:</label>
        <%:Html.TextArea(ControlIdConstants.AdditionalDetailDescription, new { rows = 3, cols = 80, @class = "addDetailDescRequired" })%>
        <span>
          <img class="linkImage" id="AddDetail" title="Add fields" alt="Add fields" src='<%:Url.Content("~/Content/Images/plus.png")%>' /></span>
      </div>
    </div>
    <div id="AddDetailTemplate" class="hidden">
      <div id="AddTl">
        <div>
          <label>
            Additional Details:</label>
          <%:Html.TextBox(ControlIdConstants.AdditionalDetailDropdownTemplate, string.Empty, new { @class = "autocComplete additionalDetailRequired" })%>
        </div>
        <div class="dynamicTextArea">
          <label>
            Additional Details Description:</label>
          <%:Html.TextArea(ControlIdConstants.AdditionalDetailDescriptionTemplate, new { rows = 3, cols = 80, @class = "addDetailDescRequired" })%>
          <span>
            <img class="linkImage" title="Remove fields" alt="Remove fields" src='<%:Url.Content("~/Content/Images/minus.png")%>' /></span>
        </div>
      </div>
    </div>
  </div>
  <div class="clear"> 
  </div>
</div>
<div id="childTaxList" class="hidden">
</div>
<div id="childVatList" class="hidden">
</div>
<div id="childAddChargeList" class="hidden">
</div>
<div id="childAttachmentList" class="hidden">
</div>
 <script type="text/javascript">
     $(document).ready(function () {
         $("#LocationCode").val('<%:Model.LocationCode%>');
     });
     </script>