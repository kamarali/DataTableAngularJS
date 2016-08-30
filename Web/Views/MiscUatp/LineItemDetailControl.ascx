<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Web.Areas.Misc.Models.LineItemDetailViewModel>" %>
<h2>Line Item Detail</h2>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow" id="staticFields">
    <% Html.RenderPartial("~/Views/MiscUatp/LineItemDetailStaticFieldsControl.ascx", Model.LineItemDetail); %>
  </div>
  <div class="fieldContainer horizontalFlowFor4FieldsPerLine">
    <div class="topLine">
    <% Html.RenderPartial("~/Views/MiscUatp/RequiredDynamicFieldsControl.ascx", Model.RequiredFields.Where(field => field.RequiredType != Iata.IS.Model.MiscUatp.Enums.RequiredType.Optional).ToList()); %>
    </div>
  </div>
  <div  class="hidden fieldContainer horizontalFlowFor4FieldsPerLine" id="RecommendedFootNote">
    The fields marked with '<span style="color: Red;">+</span>' are recommended.
  </div>
  <div>
    <% Html.RenderPartial("~/Views/MiscUatp/OptionalDynamicFieldsControl.ascx", Model.RequiredFields.Where(field => field.RequiredType == Iata.IS.Model.MiscUatp.Enums.RequiredType.Optional).ToList()); %>
  </div>
  <div class="fieldContainer horizontalFlowFor4FieldsPerLine">
    <div>
    <h2>Select Optional Group</h2>
      <div>
      <%--SCP207711:UATP details should not be allowed to be captured for MISC invoices at LID level --%>
      <%: Html.DynamicOptionalDropdownList(ControlIdConstants.OptionalGroupDropdownList, "", Model.LineItemDetail.LineItem.ChargeCodeId, Model.LineItemDetail.LineItem.ChargeCodeTypeId, Model.LineItemDetail.LineItem.Invoice.BillingCategory) %>
      <select id="HiddenOptionalGroupDropdownList" name="HiddenOptionalGroupDropdownList" class="hidden">
      </select>
      <input class="primaryButton" type="button" value="Add" id="AddOptionaGroup" onclick="AddOptionalGroup();" />
      </div>
    </div>
  </div>
  <%-- TODO: Replace buttons with images--%>
  <div class="clear">
    </div>
</div>

<div id="childTaxList" class="hidden">
</div>
<div id="childVatList" class="hidden">
</div>
<div id="childAddChargeList" class="hidden">
</div>
