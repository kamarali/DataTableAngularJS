<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Cargo.AwbRecord>" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<%@ Import Namespace="Iata.IS.Model.Pax.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
   SIS :: Cargo ::
  <%: ViewData[ViewDataConstants.BillingType].ToString() %>
  :: AWB Charge Collect Invoice :: Edit AWB Charge Collect Billing Records
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">
  $(document).ready(function () {
  $('#chkPartShipment').change(function() {
      if($(this).prop('checked')==true) {
    $('#PartShipmentIndicator').val("P");
     }
     else{
     $('#PartShipmentIndicator').val('');
     }
    });
   $("#AwbSerialNumber").numeric();
   $("#VatIdentifierId option[value='3']").remove();
   $('#AwbIssueingAirline').focus();
   var awbserNumber=SetAWBSerialNumberCheckDigit($("#AwbSerialNumber").val());
   $("#AwbSerialNumber").val(awbserNumber);
//   var billed_Weight = $("#BilledWeight").val();
//  if (billed_Weight != 0) {
//    $('#KgLbIndicator').attr('disabled', '');
//  }
//  else {
//    $("#KgLbIndicator").val("");
//    $('#KgLbIndicator').attr('disabled', 'disabled');
//  }
   var prorate_Per = $("#ProratePer").val();
  if (prorate_Per == 0) {
     $("#ProratePer").val("");
  }
  if ($("#ProvisoReqSpa").val() != "") {
    $("#ProratePer").val("");
    $("#ProratePer").attr("readOnly", true);
  }
  else {
    $("#ProratePer").removeAttr("readOnly");
  }
    initializeParentForm('prepaidBillingDetails');
      registerAutocomplete('OtherChargeCode', 'OtherChargeCode', '<%:Url.Action("GetOtherChargeCodeList", "Data", new { area = "" })%>', 0, true, null);     
//    registerAutocomplete('ReasonCode', 'ReasonCode', '<%:Url.Action("GetEntireReasonCodeList", "Data", new { area="" })%>', 0, true, null);
    $isOnView = <%:(ViewData[ViewDataConstants.PageMode] != null ? ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View  : false ).ToString().ToLower() %>;
   // InitializeAirWayBilling();
  
    registerAutocomplete('TaxCode', 'TaxCode', '<%:Url.Action("GetTaxCodes", "Data", new { area = "" })%>', 0, true, null);
    SetPageToViewMode(<%: ((string)ViewData[ViewDataConstants.PageMode] == PageMode.View).ToString().ToLower()%>);
    SetControlAccess();
    //registerAutocomplete('ReasonCode', 'ReasonCode', '<%:Url.Action("GetReasonCodeListForCargo", "Data", new { area="" })%>', 0, true, null, '', <%:Convert.ToInt32(TransactionType.CargoPrimeChargeCollect) %>);
      /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: Issuing Airlines auto-complete should exclude pure numeric values having size 5 or greater
        Ref: FRS Section 3.4 Table 17 Row 12 */     
    registerAutocomplete('AwbIssueingAirline', 'AwbIssueingAirline', '<%:Url.Action("GetTicketIssuingAirlineListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);
      
       InitializeAirWayBilling();
      SetPageModeToClone(<%:((string)ViewData[ViewDataConstants.PageMode] == PageMode.Clone).ToString().ToLower()%>);
     
      InitializeVatGrid(<%= new JavaScriptSerializer().Serialize(Model.VatBreakdown) %>);
     
      InitializeOtherChargeGrid(<%= new JavaScriptSerializer().Serialize(Model.OtherChargeBreakdown) %>);
      InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.Invoice.BilledMemberId, BillingCategoryType.Cgo) %>', '<%: Url.Action("AwbAttachmentDownload","Invoice") %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");    

  });
  $("#IscPer").blur(function () {
      setCCPercentage("#IscPer", "#AmountSubjectToIsc", "#IscAmount");
      calculateCCAmount();
    });  

     $("#CurrencyAdjustmentIndicator").keyup(function () {
      var currAdjInd = $("#CurrencyAdjustmentIndicator").val();
      $("#CurrencyAdjustmentIndicator").val(currAdjInd.toUpperCase());
    });
     $("#AmountSubjectToIsc").blur(function () {
      setCCPercentage("#IscPer", "#AmountSubjectToIsc", "#IscAmount");
      calculateCCAmount();
    });
//    $("#TaxAmount").blur(function () {
//      calculateNetCCAmount();
//    });
    $("#OtherChargeAmount").blur(function () {
       calculateCCAmount();
    });

     $("#VatAmount").blur(function () {
      calculateCCAmount();
    });
    $("#WeightCharges").blur(function () {
      calculateCCAmount();
    });
    $("#ValuationCharges").blur(function () {
      calculateCCAmount();
    });
  </script>
  <script type="text/javascript">
    billingType = '<%: ViewData[ViewDataConstants.BillingType] %>';

   
  </script>
  <script src="<%:Url.Content("~/Scripts/Cargo/CargoOtherChargeBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Cargo/AwbRecord.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Cargo/TaxBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Cargo/VatBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Cargo/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
  
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    <%:ViewData[ViewDataConstants.PageMode] %>
     AWB Charge Collect Billing Records</h1>
  <div>
    <%
      Html.RenderPartial(Url.Content("ReadOnlyInvoiceHeaderControl"), Model.Invoice);%>
  </div>
  <%
    using (Html.BeginForm(null, null, FormMethod.Post, new { id = "prepaidBillingDetails", @class = "validCharacters" }))
    {%>
    <%: Html.AntiForgeryToken() %>
  <div>
    <%
      Html.RenderPartial("AwbChargeCollectBillingDetailsControl", Model);%>
  </div>
  <div class="buttonContainer">
    <%--<input type="submit" value="Save and Add New" class="primaryButton ignoredirty" id="SaveAndAddNew"
      onclick="javascript:return changeAction('<%:Url.Action("AwbChargeCollectRecordEdit", "Invoice", new { invoiceId = Model.InvoiceId, transactionId = Model.Id })%>')" />--%>
      <input type="submit" value="Save and Add New" class="primaryButton ignoredirty" id="SaveAndAddNew"
      onclick="javascript:return SavePrepaidAwbRecord('<%:Url.Action("AwbChargeCollectRecordEdit", "Invoice", new { invoiceId = Model.InvoiceId, transactionId = Model.Id }) %>')"  />
       <input type="submit" value="Save and Duplicate" class="primaryButton ignoredirty"
      id="btnSaveAndDuplicate" onclick="javascript:return SavePrepaidAwbRecord('<%:Url.Action("AwbChargeCreditBillingClone", "Invoice", new { invoiceId = Model.InvoiceId, transactionId = Model.Id })%>')" />
 
       <input type="submit" value="Save and Back to Overview" class="primaryButton ignoredirty"
      id="SaveAndBackToOverview" onclick="javascript:return SavePrepaidAwbRecord('<%:Url.Action("AwbChargeCollectBillingEditAndReturn", "Invoice",new { invoiceId = Model.InvoiceId, transactionId = Model.Id }) %>')" />

       <%
      if (!string.IsNullOrEmpty(SessionUtil.PaxInvoiceSearchCriteria) || !string.IsNullOrEmpty(SessionUtil.PaxInvoiceSearchCriteria))
      {%>
    <input class="secondaryButton" type="button" value="Back To Billing History" onclick="javascript:location.href = '<%:Url.Action("Index","BillingHistory", new { back = true })%>'" />
    <%}
       else
       {%>
    <% if (ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
       {%>
    <%:Html.LinkButton("Back", Url.Action("AwbChargeCollectBillingListView", "Invoice", new { invoiceId = Model.InvoiceId }))%>
    <%}
       else
       {%>
    <%:Html.LinkButton("Back", Url.Action("AwbChargeCollectBillingList", "Invoice", new { invoiceId = Model.InvoiceId }))%>
    <%}%>
    <%
       }%>    
  </div>
  <%}%>
  <div class="hidden" id="divTaxBreakdown">
    <% Html.RenderPartial("AwbBillingTaxControl", Model.TaxBreakdown);%>
  </div>
  <div class="hidden" id="divVatBreakdown">
    <% Html.RenderPartial("AwbBillingVatControl", Model.VatBreakdown);%>
  </div>
  <div class="hidden" id="divAttachment">
    <% Html.RenderPartial("AwbBillingAttachmentControl", Model);%>
  </div>
  <div class="hidden" id="divOtherCharge">
    <% Html.RenderPartial("AwbOtherChargeControl", Model.OtherChargeBreakdown);%>
  </div>
  <%--<div id="childVatList" class="hidden">
</div>
<div id="childAttachmentList" class="hidden">
</div>
<div id="childOtherChargeList" class="hidden">
</div>--%>
</asp:Content>
