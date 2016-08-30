<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.CMCoupon>" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger ::
  <%: ViewData[ViewDataConstants.BillingType].ToString() %>
  :: Credit Memo Capture :: Coupon
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">
    $(document).ready(function () {
      initializeParentForm('creditMemoCouponForm');
      isOnCouponPage = true;
      // Value of 'AirlineFlightDesignator' field gets cleared in SetPageModeToCreateMode() function, so fetch its value and set again
      airlineFlightDesignator = $('#AirlineFlightDesignator').val();
      // Retrieve pagemode
      pageMode = '<%: ViewData[ViewDataConstants.PageMode] %>';
      // Set variable to true if PageMode is "View"
      $isOnView = <%:(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View ? true : false ).ToString().ToLower() %>;
      //Set variable for the tax breakdown on CM breakdown
      $isCreditMemo = true;

      registerAutocomplete('TaxCode', 'TaxCode', '<%:Url.Action("GetTaxCodes", "Data", new { area = "" })%>', 0, true, null);
      /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: Issuing Airlines auto-complete should exclude pure numeric values having size 5 or greater
        Ref: FRS Section 3.4 Table 17 Row 5 */
      registerAutocomplete('TicketOrFimIssuingAirline', 'TicketOrFimIssuingAirline', '<%:Url.Action("GetTicketIssuingAirlineListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);
      SetPageModeToCreateMode(<%:((string)ViewData[ViewDataConstants.PageMode] == PageMode.Create).ToString().ToLower()%>);
      SetPageModeToClone(<%:((string)ViewData[ViewDataConstants.PageMode] == PageMode.Clone).ToString().ToLower()%>);
      InitializeTaxGrid(<%= new JavaScriptSerializer().Serialize(Model.TaxBreakdown) %>);
      InitializeVatGrid(<%= new JavaScriptSerializer().Serialize(Model.VatBreakdown) %>);
      InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.CreditMemoRecord.Invoice.BilledMemberId, BillingCategoryType.Pax) %>', '<%: Url.Action("CreditMemoCouponAttachmentDownload","CreditNote", new { transaction = "CMEdit", transactionId = Model.CreditMemoRecord.Id.Value(), invoiceId = Model.CreditMemoRecord.InvoiceId.Value() }) %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");      
      InitializeProrateSlip(80, 50);
      
      // Set value for AirlineFlightDesignator field
      $('#AirlineFlightDesignator').val(airlineFlightDesignator);

      // Display Serial Number textbox only in Edit mode and hide in Create mode
      if(<%:((string)ViewData[ViewDataConstants.PageMode] == PageMode.Create).ToString().ToLower()%>){
        $("#serialNoDiv").addClass('hidden');  
      } 
      else if(<%:((string)ViewData[ViewDataConstants.PageMode] == PageMode.Clone).ToString().ToLower()%>){      
         //SCP65997 
        //$("#serialNoDiv").addClass('hidden'); 
        //$("#TicketOrFimCouponNumber").val('');
        //$("#TicketDocOrFimNumber").val('');
        //$("#CheckDigit").val('9');
        //$("#SettlementAuthorizationCode").val('');
      }        
    });
  </script>
  <script type="text/javascript">
    // Set billing type from Viewdata
    billingType = '<%: ViewData[ViewDataConstants.BillingType] %>';
  </script>
  <script src="<%:Url.Content("~/Scripts/Pax/CMCoupon.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/TaxBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/VatBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Create Credit Memo Coupon</h1>
  <div>
    <%
            Html.RenderPartial("CreditMemoHeaderControl", Model.CreditMemoRecord);%>
  </div>
  <div class="clear">
  </div>
  <h2>
    Credit Memo Coupon Details</h2>
  <%
        using (Html.BeginForm(null, null, FormMethod.Post, new { @id = "creditMemoCouponForm" }))
        {%>
        <%: Html.AntiForgeryToken() %>
  <div>
    <% Html.RenderPartial("CMCouponDetailsControl", Model);%>
  </div>
  <div class="buttonContainer">
    <input type="submit" value="Save and Add New" class="primaryButton ignoredirty" id="btnSaveAndAddNew" onclick="javascript:changeAction('<%: Url.Action("CreditMemoCouponCreate","CreditNote") %>');" />
    <input type="submit" value="Save and Duplicate" class="primaryButton ignoredirty" id="btnSaveAndDuplicate" onclick="changeAction('<%: Url.Action("CreditMemoCouponDuplicate","CreditNote") %>');" />
    <input type="submit" value="Save and Back to Overview" class="primaryButton ignoredirty" id="SaveAndBackToOverview" onclick="javascript:changeAction('<%: Url.Action("CreditMemoCouponCreateAndReturn","CreditNote") %>')" />
    <%: Html.LinkButton("Back", Url.Action("CreditMemoEdit", "CreditNote", new { transactionId = Model.CreditMemoRecord.Id.Value(), invoiceId = Model.CreditMemoRecord.InvoiceId.Value() }))%>
  </div>
  <%
        
        }
  %>
  <div class="hidden" id="divTaxBreakdown">
    <% Html.RenderPartial("CouponTaxControl", Model.TaxBreakdown);%>
  </div>
  <div class="hidden" id="divVatBreakdown">
    <% Html.RenderPartial("CouponVatControl", Model.VatBreakdown);%>
  </div>
  <div class="hidden" id="divAttachment">
    <%
            Html.RenderPartial("CMCouponAttachmentControl", Model);%>
  </div>
  <div id="divProrateSlip" class="hidden">
    <% Html.RenderPartial("ProrateSlipControl", Model.ProrateSlipDetails);%>
  </div>
</asp:Content>
