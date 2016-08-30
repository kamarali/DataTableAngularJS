<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Cargo.CargoBillingMemoAwb>" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<%@ Import Namespace="Iata.IS.Model.Pax.Enums" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script type="text/javascript">
    pageMode =  <%:((ViewData[ViewDataConstants.PageMode]!=null && ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View) ? true : false).ToString().ToLower() %>;
    $isOnView = <%:(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View ? true : false).ToString().ToLower()%>;
    // Set couponBreakdownExists to "False" in create mode
    couponBreakdownExists = 'False';
    // Set billing type from Viewdata
    billingType = '<%: ViewData[ViewDataConstants.BillingType] %>'; 
  $('#chkPartShipment').change(function() {
      if($(this).prop('checked')==true) {
    $('#PartShipmentIndicator').val("P");
     }
     else{
     $('#PartShipmentIndicator').val('');
     }
    });
   
  $(document).ready(function () {
    $("#AwbSerialNumberCheckDigit").numeric();
    SetProrateLadderHeaderFields('<%: Model.ProrateCalCurrencyId%>', '<%: Model.TotalProrateAmount %>');
    //SCP#449352:Issue with Reason Code field
    //registerAutocomplete('ReasonCode', 'ReasonCode', '<%:Url.Action("GetReasonCodeListForCargoBillingMemo", "Data", new { area="" })%>', 0, false, onReasonCodeChange, '', <%: Convert.ToInt32(TransactionType.CargoBillingMemo) %>, null, '');
    registerAutocomplete('OtherChargeCode', 'OtherChargeCode', '<%:Url.Action("GetOtherChargeCodeList", "Data", new { area = "" })%>', 0, true, null);     
    /* CMP #596: Length of Member Accounting Code to be Increased to 12 
    Desc: Issuing Airlines auto-complete should exclude pure numeric values having size 5 or greater
    Ref: FRS Section 3.4 Table 17 Row 15 */              
    registerAutocomplete('AwbIssueingAirline', 'AwbIssueingAirline', '<%:Url.Action("GetTicketIssuingAirlineListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);
    registerAutocomplete('CarrierPrefix', 'CarrierPrefix', '<%:Url.Action("GetTicketIssuingAirlineList", "Data", new { area = "" })%>', 0, true, null);
    InitializeVatGrid(<%= new JavaScriptSerializer().Serialize(Model.AwbVat) %>);
    InitializeOtherChargeGrid(<%= new JavaScriptSerializer().Serialize(Model.OtherCharges) %>);  
    InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.BillingMemoRecord.Invoice.BilledMemberId, BillingCategoryType.Cgo) %>', '<%: Url.Action("BMAwbAttachmentDownload","Invoice") %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");    
    InitializeProrateLadderGrid(<%= new JavaScriptSerializer().Serialize(Model.ProrateLadder) %>, '<%:Url.Action("ValidateFromToSectors", "Invoice") %>','<%: Model.ProrateCalCurrencyId %>');
   // SetPageModeToCreateMode(<%:((string)ViewData[ViewDataConstants.PageMode] == PageMode.Create).ToString().ToLower()%>);
    SetPageToViewMode(<%: ((string)ViewData[ViewDataConstants.PageMode] == PageMode.View).ToString().ToLower()%>);
    $("#VatIdentifierId option[value='3']").remove();
//     var billed_Weight = $("#BilledWeight").val();
//  if (billed_Weight != 0) {
//    $('#KgLbIndicator').attr('disabled', '');
//  }
//  else {
//    $("#KgLbIndicator").val("");
//    $('#KgLbIndicator').attr('disabled', 'disabled');
//  }
   var prorate_Per = $("#PrpratePercentage").val();
  if (prorate_Per == 0) {
     $("#PrpratePercentage").val("");
  }
  if ($("#ProvisionalReqSpa").val() != "") {
    $("#PrpratePercentage").val("");
    $("#PrpratePercentage").attr("readOnly", true);
  }
  else {
    $("#PrpratePercentage").removeAttr("readOnly");
  }
  });

  </script>
  <script src="<%:Url.Content("~/Scripts/Cargo/BMAwbPrepaid.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Cargo/VatBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Cargo/CargoOtherChargeBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Cargo/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/Cargo/ProrateLadder.js") %>" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Cargo ::
  <%: ViewData[ViewDataConstants.BillingType].ToString() %>
  ::BM AWB Prepaid Invoice :: Create BM AWB Prepaid Billing
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    <%:ViewData[ViewDataConstants.PageMode] %> AWB Prepaid Billing</h1>
  <div>
    <%
      Html.RenderPartial(Url.Content("ReadOnlyInvoiceHeaderControl"), Model.BillingMemoRecord.Invoice);%>
  </div>
  <% using (Html.BeginForm(null, null, FormMethod.Post, new { id = "formBMAwbPrepaid", @class = "validCharacters" }))
     {  %>
     <%: Html.AntiForgeryToken() %>
  <div>
    <%
       Html.RenderPartial("BMAWBPrepaidBreakdownDetails", Model);%>
  </div>
  <div class="buttonContainer">
    <input type="submit" value="Save and Add New" class="primaryButton ignoredirty" id="btnSaveAndAddNew"
      onclick="javascript: return SaveBMAwbRecord('<%: Url.Action("BMAwbPrepaidEdit","Invoice",new { transactionId = Model.BillingMemoRecord.Id.Value(),  invoiceId= Model.BillingMemoRecord.Invoice.Id.Value(), transaction="BMEdit", couponId = Model.Id }) %>')" />
    <input type="submit" value="Save and Duplicate" class="primaryButton ignoredirty"
      id="btnSaveAndDuplicate" onclick="javascript:return SaveBMAwbRecord('<%: Url.Action("BMPrepaidAwbClone","Invoice",new { transactionId = Model.BillingMemoRecord.Id.Value(),  invoiceId= Model.BillingMemoRecord.Invoice.Id.Value(), transaction="BMEdit", couponId = Model.Id }) %>')" />
    <input type="submit" value="Save and Back to Overview" class="primaryButton ignoredirty"
      id="SaveAndBackToOverview" onclick="javascript:return SaveBMAwbRecord('<%: Url.Action("BMPrepaidAwbEditAndReturn","Invoice",new { transactionId = Model.BillingMemoRecord.Id.Value(),  invoiceId= Model.BillingMemoRecord.Invoice.Id.Value(), transaction="BMEdit" , couponId = Model.Id}) %>')" />
    
    <%if (ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
{%>
    <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.RouteUrl("CGOtransactions",
                                 new
                                   {
                                     action = "BMView",
                                     invoiceId = Model.BillingMemoRecord.InvoiceId.Value(),
                                     transactionId = Model.BillingMemoRecord.Id.Value()
                                   })%>';" />
    <%
}else
{%>
    <%:Html.LinkButton("Back", Url.RouteUrl("CGOtransactions", new { action = "BMEdit", invoiceId = Model.BillingMemoRecord.InvoiceId.Value(), transactionId = Model.BillingMemoRecord.Id.Value() }))%>
    <%
}%>
  </div>
  <%
     }%>

    <div id="divVatBreakdown" class="hidden">
    <% Html.RenderPartial("BMAwbVatControl", Model.AwbVat);%>
  </div>
  <div class="hidden" id="divAttachment">
    <%
      Html.RenderPartial("BMAwbAttachmentControl", Model);%>
  </div>
  <div class="hidden" id="divOtherCharge">
    <% Html.RenderPartial("BMAwbOtherChargeControl", Model.OtherCharges);%>
  </div>
  <div class="hidden" id="divProrateLadder">
    <% Html.RenderPartial("BMAwbProrateLadderControl", Model.ProrateLadder);%>
  </div>
</asp:Content>


