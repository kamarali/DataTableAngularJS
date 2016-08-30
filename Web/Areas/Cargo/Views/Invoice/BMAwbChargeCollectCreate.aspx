<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Cargo.CargoBillingMemoAwb>" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<%@ Import Namespace="Iata.IS.Model.Pax.Enums" %>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script type="text/javascript">
    pageMode =  <%:((ViewData[ViewDataConstants.PageMode]!=null && ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View) ? true : false).ToString().ToLower() %>;
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
   $("#serialNoDiv").addClass('hidden');
    registerAutocomplete('OtherChargeCode', 'OtherChargeCode', '<%:Url.Action("GetOtherChargeCodeList", "Data", new { area = "" })%>', 0, true, null);  
    //SCP#449352:Issue with Reason Code field
    //registerAutocomplete('ReasonCode', 'ReasonCode', '<%:Url.Action("GetReasonCodeListForCargoBillingMemo", "Data", new { area="" })%>', 0, false, onReasonCodeChange, '', <%: Convert.ToInt32(TransactionType.CargoBillingMemo) %>, null, '');
    /* CMP #596: Length of Member Accounting Code to be Increased to 12 
    Desc: Issuing Airlines auto-complete should exclude pure numeric values having size 5 or greater
    Ref: FRS Section 3.4 Table 17 Row 16 */    
    registerAutocomplete('AwbIssueingAirline', 'AwbIssueingAirline', '<%:Url.Action("GetTicketIssuingAirlineListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);
    registerAutocomplete('CarrierPrefix', 'CarrierPrefix', '<%:Url.Action("GetTicketIssuingAirlineList", "Data", new { area = "" })%>', 0, true, null);
    InitializeVatGrid(<%= new JavaScriptSerializer().Serialize(Model.AwbVat) %>);
    InitializeOtherChargeGrid(<%= new JavaScriptSerializer().Serialize(Model.OtherCharges) %>);  
    InitializeProrateLadderGrid(<%= new JavaScriptSerializer().Serialize(Model.ProrateLadder) %>, '<%:Url.Action("ValidateFromToSectors", "Invoice") %>','<%: Model.ProrateCalCurrencyId %>');
    InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.BillingMemoRecord.Invoice.BilledMemberId, BillingCategoryType.Cgo) %>', '<%: Url.Action("BMAwbAttachmentDownload","Invoice") %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");    
    if ('<%: Convert.ToBoolean(ViewData["IsPostback"])%>' != 'True') // If exception occurs, do not do anything..
      {
        if('<%: Convert.ToBoolean(ViewData["FromClone"])%>' == 'True'){ // If 'Save And Duplicate' is clicked, do not duplicate AWB serial number and attachments.
          SetProrateLadderHeaderFields('<%: Model.ProrateCalCurrencyId%>', '<%: Model.TotalProrateAmount %>');
          $("#AwbSerialNumberCheckDigit").val('');
          $("#AwbSerialNumber").val('');
          $("#AwbCheckDigit").val('');
          $("#AwbIssueingAirline").val('');
          $("#AttachmentIndicatorOriginal").val('No');
        }
        else{
          SetPageModeToCreateMode(<%:((string)ViewData[ViewDataConstants.PageMode] == PageMode.Create).ToString().ToLower()%>);
          $("#AwbSerialNumberCheckDigit").val('');
          $.watermark.showAll();
        }
      }
      $("#VatIdentifierId option[value='3']").remove();
      //$('#KgLbIndicator').attr('disabled', 'disabled');
  });

  </script>
  <script src="<%:Url.Content("~/Scripts/Cargo/BMAwbChargeCollect.js")%>" type="text/javascript"></script>
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
  ::BM AWB Prepaid Invoice :: Create BM AWB Charge Collect Billing
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Create AWB Charge Collect Billing</h1>
  <div>
    <%
      Html.RenderPartial(Url.Content("ReadOnlyBillingMemoHeaderControl"), Model.BillingMemoRecord);%>
  </div>
  <% using (Html.BeginForm(null, null, FormMethod.Post, new { id = "formBMAwbPrepaid", @class = "validCharacters" }))
     {  %>
     <%: Html.AntiForgeryToken() %>
  <div>
    <%
       Html.RenderPartial("BMAWBChargeCollectBreakdownDetails", Model);%>
  </div>
  <div class="buttonContainer">
    <input type="submit" value="Save and Add New" class="primaryButton ignoredirty" id="btnSaveAndAddNew"
      onclick="javascript:return SaveBMAwbRecord('<%: Url.Action("BMAwbChargeCollectCreate","Invoice",new { transactionId = Model.BillingMemoRecord.Id.Value(),  invoiceId= Model.BillingMemoRecord.Invoice.Id.Value(), transaction="BMEdit" }) %>')" />
    <input type="submit" value="Save and Duplicate" class="primaryButton ignoredirty"
      id="btnSaveAndDuplicate" onclick="javascript:return SaveBMAwbRecord('<%: Url.Action("BMAwbChargeCollectDuplicate","Invoice",new { transactionId = Model.BillingMemoRecord.Id.Value(),  invoiceId= Model.BillingMemoRecord.Invoice.Id.Value(), transaction="BMEdit" }) %>')" />
    <input type="submit" value="Save and Back to Overview" class="primaryButton ignoredirty"
      id="SaveAndBackToOverview" onclick="javascript:return SaveBMAwbRecord('<%: Url.Action("BMAwbChargeCollectCreateAndReturn","Invoice",new { transactionId = Model.BillingMemoRecord.Id.Value(),  invoiceId= Model.BillingMemoRecord.Invoice.Id.Value(), transaction="BMEdit" }) %>')" />
    <%:Html.LinkButton("Back", Url.RouteUrl("CGOtransactions", new { action = "BMEdit", invoiceId = Model.BillingMemoRecord.InvoiceId.Value(), transactionId = Model.BillingMemoRecord.Id.Value() }))%>

    
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


