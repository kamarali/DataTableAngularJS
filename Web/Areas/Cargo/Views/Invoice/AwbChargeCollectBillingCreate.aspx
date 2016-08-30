<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Cargo.AwbRecord>" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<%@ Import Namespace="Iata.IS.Model.Pax.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Cargo ::
  <%: ViewData[ViewDataConstants.BillingType].ToString() %>
  :: AWB Prepaid Invoice :: Add Charge Collect AWB
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">
    $(document).ready(function () {
     
     $("#AwbSerialNumber").numeric();
     $("#VatIdentifierId option[value='3']").remove();
     $('#AwbIssueingAirline').focus();
//     $('#KgLbIndicator').attr('disabled', 'disabled');
      initializeParentForm('prepaidBillingDetails');  
      InitializeVatGrid(<%= new JavaScriptSerializer().Serialize(Model.VatBreakdown) %>);
    InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.Invoice.BilledMemberId, BillingCategoryType.Cgo) %>', '<%: Url.Action("AwbAttachmentDownload","Invoice") %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");    
//    InitializeOtherChargeGrid(<%= new JavaScriptSerializer().Serialize(Model.OtherCharges) %>);    
      // Set variable to true if PageMode is "View"
//      registerAutocomplete('ReasonCode', 'ReasonCode', '<%:Url.Action("GetReasonCodeListForCargo", "Data", new { area="" })%>', 0, true, null, '', <%:Convert.ToInt32(TransactionType.CargoPrimeChargeCollect) %>);
      $isOnView = <%:((ViewData[ViewDataConstants.PageMode]!=null && ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View) ? true : false).ToString().ToLower() %>;
      // Value of 'AirlineFlightDesignator' and 'CheckDigit' field gets cleared in SetPageModeToCreateMode() function, so fetch its value and set again
      //airlineFlightDesignator = $('#AirlineFlightDesignator').val();
      //checkDigit = $('#CheckDigit').val();
     
      registerAutocomplete('OtherChargeCode', 'OtherChargeCode', '<%:Url.Action("GetOtherChargeCodeList", "Data", new { area = "" })%>', 0, true, null);
      /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: Issuing Airlines auto-complete should exclude pure numeric values having size 5 or greater
        Ref: FRS Section 3.4 Table 17 Row 12 */ 
      registerAutocomplete('AwbIssueingAirline', 'AwbIssueingAirline', '<%:Url.Action("GetTicketIssuingAirlineListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);
      registerAutocomplete('TaxCode', 'TaxCode', '<%:Url.Action("GetTaxCodes", "Data", new { area = "" })%>', 0, true, null);
            if('<%: Convert.ToBoolean(ViewData["FromClone"])%>' == 'True'){ // If 'Save And Duplicate' is clicked, do not duplicate AWB serial number and attachments.
         
          $("#AwbSerialNumber").val('');
          $("#AwbIssueingAirline").val('');
          $("#AttachmentIndicatorOriginal").val('No');
        }
          
      if('<%: Convert.ToBoolean(ViewData[ViewDataConstants.IsPostback])%>' == 'False'){
        
        // In 'Clone' mode, clear values in TicketOrFimCouponNumber, TicketDocOrFimNumber and SettlementAuthorizationCode
        if('<%: (string)ViewData[ViewDataConstants.PageMode] == PageMode.Clone %>' == 'True'){
//          $("#TicketOrFimCouponNumber").val('');
//          $("#TicketDocOrFimNumber").val('');
//          $("#SettlementAuthorizationCode").val(''); 
//          $('#TicketOrFimCouponNumber').focus();
        }
        else{
            SetPageModeToCreateMode(<%:((string)ViewData[ViewDataConstants.PageMode] == PageMode.Create).ToString().ToLower()%>);
          //Set values of SourceCode, BatchSequenceNumber, RecordSequenceWithinBatch, these values are set when clicked on 'Save and Add new' option
          if('<%: ViewData[ViewDataConstants.AwbChargeCollectRecord] != null%>' == 'True'){        
            //SetPageModeToCreateMode(<%:((string)ViewData[ViewDataConstants.PageMode] == PageMode.Create).ToString().ToLower()%>);
            $("#AwbIssuingAirline").val('');
            $("#BatchSequenceNumber").val('<%: ViewData[ViewDataConstants.AwbChargeCollectRecord]%>'.split('-')[0]);
            $("#RecordSequenceWithinBatch").val('<%: ViewData[ViewDataConstants.AwbChargeCollectRecord]%>'.split('-')[1]);
             
        
            // $('#TicketOrFimCouponNumber').focus();
         }
        
          else{      
            
            $("#AwbIssuingAirline").val('');
            $("#BatchSequenceNumber").val('');
            $("#RecordSequenceWithinBatch").val('');
          
          }   
        }  
      }  
      InitializeAirWayBilling();
      SetPageModeToClone(<%:((string)ViewData[ViewDataConstants.PageMode] == PageMode.Clone).ToString().ToLower()%>);
      InitializeTaxGrid(<%= new JavaScriptSerializer().Serialize(Model.TaxBreakdown) %>);
      InitializeVatGrid(<%= new JavaScriptSerializer().Serialize(Model.VatBreakdown) %>);
      InitializeOtherChargeGrid(<%= new JavaScriptSerializer().Serialize(Model.OtherChargeBreakdown) %>);
      InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.Invoice.BilledMemberId,BillingCategoryType.Cgo) %>', '<%: Url.Action("AwbAttachmentDownload","Invoice") %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");  

      // Following code is used to pre populate BatchSequenceNumber and RecordSequenceNumber  
      $("#BatchSequenceNumber").val(<%= new JavaScriptSerializer().Serialize(Model.BatchSequenceNumber) %>);
      $("#RecordSequenceWithinBatch").val(<%= new JavaScriptSerializer().Serialize(Model.RecordSequenceWithinBatch) %>);
     
      var isPageModeClone = <%:((ViewData[ViewDataConstants.PageMode]!=null && ViewData[ViewDataConstants.PageMode].ToString() == PageMode.Clone) ? true : false).ToString().ToLower() %>;
      // If PageMode is Clone and AgreementIndicator has value any of the below disable Original PMI text field, else enable it.          
//    if ('<%: ViewData[ViewDataConstants.IsPostback] %>' == 'True' || isPageModeClone == true)  {
//      setOriginalPmiAccess();
//    }

    $('#chkPartShipment').change(function() {
   
    if($(this).prop('checked')==true) {
    $('#PartShipmentIndicator').val("P");
    }
    else {
        $('#PartShipmentIndicator').val("");
    }
  });
    
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
    // Set billing type from Viewdata
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
    Add Charge Collect AWB</h1>
  <div>
    <%
      Html.RenderPartial(Url.Content("ReadOnlyInvoiceHeaderControl"), Model.Invoice);%>
  </div>
  <% using (Html.BeginForm(null, null, FormMethod.Post, new { id = "prepaidBillingDetails", @class = "validCharacters" }))
     {  %>
     <%: Html.AntiForgeryToken() %>
  <div>
    <%
       Html.RenderPartial("AwbChargeCollectBillingDetailsControl", Model);%>
  </div>
  <div class="buttonContainer">
    <%--<input type="submit" value="Save and Add New" class="primaryButton ignoredirty" id="btnSaveAndAddNew"
      onclick="javascript:return changeAction('<%: Url.Action("AwbChargeCollectBillingCreate","Invoice") %>')" />--%>
      <input type="submit" value="Save and Add New" class="primaryButton ignoredirty" id="btnSaveAndAddNew"
      onclick="javascript:return SavePrepaidAwbRecord('<%:Url.Action("AwbChargeCollectBillingCreate", "Invoice") %>')" />
    
     <%-- <input type="submit" value="Save and Duplicate" class="primaryButton ignoredirty"
      id="btnSaveAndDuplicate" onclick="javascript:return changeAction('<%: Url.Action("AwbChargeCreditBillingDuplicate","Invoice") %>')" />--%>
       <input type="submit" value="Save and Duplicate" class="primaryButton ignoredirty"
      id="btnSaveAndDuplicate" onclick="javascript:return SavePrepaidAwbRecord('<%:Url.Action("AwbChargeCreditBillingDuplicate", "Invoice") %>')" />
    <%--<input type="submit" value="Save and Back to Overview" class="primaryButton ignoredirty"
      id="SaveAndBackToOverview" onclick="javascript:return changeAction('<%: Url.Action("AwbChargeCollectBillingCreateAndReturn","Invoice") %>')" />--%>
      <input type="submit" value="Save and Back to Overview" class="primaryButton ignoredirty"
      id="SaveAndBackToOverview" onclick="javascript:return SavePrepaidAwbRecord('<%:Url.Action("AwbChargeCollectBillingCreateAndReturn", "Invoice") %>')" />
    <%: Html.LinkButton("Back", Url.Action("AwbChargeCollectBillingList", "Invoice", new { invoiceId = Model.InvoiceId.Value() }))%>
  </div>
  <%
     }%>
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
</asp:Content>
