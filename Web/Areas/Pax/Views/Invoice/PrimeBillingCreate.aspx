<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.PrimeCoupon>" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>

<%@ Import Namespace="Iata.IS.Model.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> :: Non-Sampling Invoice :: Create Prime Billing
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">
    $(document).ready(function () {
      initializeParentForm('primeBillingDetails');      
      // Set variable to true if PageMode is "View"
      $isOnView = <%:((ViewData[ViewDataConstants.PageMode]!=null && ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View) ? true : false).ToString().ToLower() %>;
      // Value of 'AirlineFlightDesignator' and 'CheckDigit' field gets cleared in SetPageModeToCreateMode() function, so fetch its value and set again
      airlineFlightDesignator = $('#AirlineFlightDesignator').val();
      checkDigit = $('#CheckDigit').val();
      registerAutocomplete('SourceCodeId', 'SourceCodeId', '<%:Url.Action("GetSourceCodeList", "Data", new { area = "" })%>', 0, true, onSourceCodeChange,'<%:Convert.ToInt32(TransactionType.Coupon) %>');      
      /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: Issuing Airlines auto-complete should exclude pure numeric values having size 5 or greater
        Ref: FRS Section 3.4 Table 17 Row 2 */
      registerAutocomplete('TicketOrFimIssuingAirline', 'TicketOrFimIssuingAirline', '<%:Url.Action("GetTicketIssuingAirlineListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);
      registerAutocomplete('TaxCode', 'TaxCode', '<%:Url.Action("GetTaxCodes", "Data", new { area = "" })%>', 0, true, null);
            
      if('<%: Convert.ToBoolean(ViewData[ViewDataConstants.IsPostback])%>' == 'False'){
        
        // In 'Clone' mode, clear values in TicketOrFimCouponNumber, TicketDocOrFimNumber and SettlementAuthorizationCode
        if('<%: (string)ViewData[ViewDataConstants.PageMode] == PageMode.Clone %>' == 'True'){
          //Validate Source Code from Prime Coupon, If FIM source code and Tax Breakdown exist then show message.  
          //CMP #672: Validation on Taxes in PAX FIM Billings
          var sourceCode = $("#SourceCodeId").val();
          if (sourceCode == 14) {
            $("#TaxAmount").val('');
          }
          $("#TicketOrFimCouponNumber").val('');
          $("#TicketDocOrFimNumber").val('');
          $("#SettlementAuthorizationCode").val(''); 
          $('#TicketOrFimCouponNumber').focus();
        }
        else{
          //Set values of SourceCode, BatchSequenceNumber, RecordSequenceWithinBatch, these values are set when clicked on 'Save and Add new' option
          if('<%: ViewData[ViewDataConstants.PrimeCouponRecord] != null%>' == 'True'){        
            SetPageModeToCreateMode(<%:((string)ViewData[ViewDataConstants.PageMode] == PageMode.Create).ToString().ToLower()%>);
            $("#SourceCodeId").val('<%: ViewData[ViewDataConstants.PrimeCouponRecord]%>'.split('-')[0]);
            $("#BatchSequenceNumber").val('<%: ViewData[ViewDataConstants.PrimeCouponRecord]%>'.split('-')[1]);
            $("#RecordSequenceWithinBatch").val('<%: ViewData[ViewDataConstants.PrimeCouponRecord]%>'.split('-')[2]);
            $("#TicketOrFimCouponNumber").val('');
            $("#TicketDocOrFimNumber").val('');        
        
            $('#TicketOrFimCouponNumber').focus();
          }
        
          else{      
            SetPageModeToCreateMode(<%:((string)ViewData[ViewDataConstants.PageMode] == PageMode.Create).ToString().ToLower()%>);
            $("#SourceCodeId").val('');
            $("#BatchSequenceNumber").val('');
            $("#RecordSequenceWithinBatch").val('');
          
          }   
        }  
      }  
      InitializeCoupon("PrimeCpn");
      SetPageModeToClone(<%:((string)ViewData[ViewDataConstants.PageMode] == PageMode.Clone).ToString().ToLower()%>);
      InitializeTaxGrid(<%= new JavaScriptSerializer().Serialize(Model.TaxBreakdown) %>);
      InitializeVatGrid(<%= new JavaScriptSerializer().Serialize(Model.VatBreakdown) %>);
      InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.Invoice.BilledMemberId,BillingCategoryType.Pax) %>', '<%: Url.Action("PrimeBillingAttachmentDownload","Invoice") %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");  

      // Set value for AirlineFlightDesignator field
      $('#AirlineFlightDesignator').val(airlineFlightDesignator);
      $('#CheckDigit').val(checkDigit);
      
    var isPageModeClone = <%:((ViewData[ViewDataConstants.PageMode]!=null && ViewData[ViewDataConstants.PageMode].ToString() == PageMode.Clone) ? true : false).ToString().ToLower() %>;
      // If PageMode is Clone and AgreementIndicator has value any of the below disable Original PMI text field, else enable it.          
    if ('<%: ViewData[ViewDataConstants.IsPostback] %>' == 'True' || isPageModeClone == true)  {
      setOriginalPmiAccess();
    }

    function setOriginalPmiAccess()
    {
      var agreementIndSupplied = $("#AgreementIndicatorSupplied").val();
      if (agreementIndSupplied == "I" || agreementIndSupplied == 'J' || agreementIndSupplied == 'K' || agreementIndSupplied == 'W' || agreementIndSupplied == 'V' || agreementIndSupplied == 'T') {
        $("#OriginalPmi").attr("readonly", true);
      }
      else {
        $("#OriginalPmi").attr("readonly", false); 
      }
    }
    });    

     //CMP #672: Validation on Taxes in PAX FIM Billings
     function changePCpnActionUrl(actionName) {
         
         $("form").attr("action", actionName);
         
   }

  </script>
  <script type="text/javascript">
    // Set billing type from Viewdata
    billingType = '<%: ViewData[ViewDataConstants.BillingType] %>';
  </script>
  <script src="<%:Url.Content("~/Scripts/Pax/CouponRecord.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/TaxBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/VatBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Create Prime Billing</h1>
  <div>
    <%
      Html.RenderPartial(Url.Content("ReadOnlyInvoiceHeaderControl"), Model.Invoice);%>
  </div>
  <% using (Html.BeginForm(null, null, FormMethod.Post, new { id = "primeBillingDetails", @class = "validCharacters" }))
     {  %>
     <%: Html.AntiForgeryToken() %>
  <div>
    <%
      Html.RenderPartial("PrimeBillingDetailsControl", Model);%>
  </div>
  <div class="buttonContainer">
    <input type="submit" value="Save and Add New" class="primaryButton ignoredirty" id="btnSaveAndAddNew" onclick="javascript:return changePCpnActionUrl('<%: Url.Action("PrimeBillingCreate","Invoice") %>')" />
    <input type="submit" value="Save and Duplicate" class="primaryButton ignoredirty" id="btnSaveAndDuplicate" onclick="javascript:return changePCpnActionUrl('<%: Url.Action("PrimeBillingDuplicate","Invoice") %>')" />
    <input type="submit" value="Save and Back to Overview" class="primaryButton ignoredirty" id="SaveAndBackToOverview" onclick="javascript:return changePCpnActionUrl('<%: Url.Action("PrimeBillingCreateAndReturn","Invoice") %>')" />
    <%: Html.LinkButton("Back", Url.Action("PrimeBillingList", "Invoice", new { invoiceId = Model.InvoiceId.Value() })) %>
  </div>
  <%
    }%>
  <div class="hidden" id="divTaxBreakdown">
    <% Html.RenderPartial("PrimeBillingTaxControl", Model.TaxBreakdown);%>
  </div>
  <div class="hidden" id="divVatBreakdown">
    <% Html.RenderPartial("PrimeBillingVatControl", Model.VatBreakdown);%>
  </div>
  <div class="hidden" id="divAttachment">
    <% Html.RenderPartial("PrimeBillingAttachmentControl", Model);%>
  </div>
</asp:Content>
