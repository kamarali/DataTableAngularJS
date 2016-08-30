<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.BMCoupon>" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>

<%@ Import Namespace="Iata.IS.Model.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> :: Non-Sampling Invoice :: Create Billing Memo Coupon Breakdown
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">
    $(document).ready(function () {
      initializeParentForm('billingMemoCouponBreakdownDetails');
      // Set variable to true if PageMode is "View"
      $isOnView =  <%:((ViewData[ViewDataConstants.PageMode]!=null && ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View) ? true : false).ToString().ToLower() %>;;

      // Value of 'AirlineFlightDesignator' field gets cleared in SetPageModeToCreateMode() function, so fetch its value and set again
      airlineFlightDesignator = $('#AirlineFlightDesignator').val();
      /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: Issuing Airlines auto-complete should exclude pure numeric values having size 5 or greater
        Ref: FRS Section 3.4 Table 17 Row 4 */
      registerAutocomplete('TicketOrFimIssuingAirline', 'TicketOrFimIssuingAirline', '<%:Url.Action("GetTicketIssuingAirlineListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);
      registerAutocomplete('TaxCode', 'TaxCode', '<%:Url.Action("GetTaxCodes", "Data", new { area = "" })%>', 0, true, null);
      InitializeTaxGrid(<%= new JavaScriptSerializer().Serialize(Model.TaxBreakdown) %>);
      InitializeVatGrid(<%= new JavaScriptSerializer().Serialize(Model.VatBreakdown) %>);
      InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.BillingMemo.Invoice.BilledMemberId, BillingCategoryType.Pax) %>', '<%: Url.Action("BMCouponAttachmentDownload","Invoice", new{ transaction = "BMEdit", transactionId = Model.BillingMemo.Id, invoiceId = Model.BillingMemo.InvoiceId }) %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");
      InitializeProrateSlip(80, 50);
      SetPageModeToCreateMode(<%:((string)ViewData[ViewDataConstants.PageMode] == PageMode.Create).ToString().ToLower()%>);

      // Set value for AirlineFlightDesignator field
      $('#AirlineFlightDesignator').val(airlineFlightDesignator);
      
      if('<%: ViewData["IsAddNewBMCoupon"] != null && Convert.ToBoolean(ViewData["IsAddNewBMCoupon"])%>' == 'True')
        $('#TicketOrFimCouponNumber').focus();
      else
        $('#TicketOrFimIssuingAirline').focus();

      // Display Serial Number textbox only in Edit mode and hide in Create mode
      if(<%:((string)ViewData[ViewDataConstants.PageMode] == PageMode.Create).ToString().ToLower()%>){
        $("#serialNoDiv").addClass('hidden');  
      }
      else{
        if(<%:((string)ViewData[ViewDataConstants.PageMode] == PageMode.Clone).ToString().ToLower()%>){      
          $("#serialNoDiv").addClass('hidden'); 
          $("#TicketOrFimCouponNumber").val('');
          $("#TicketDocOrFimNumber").val('');
          $("#CheckDigit").val('');
          $("#SettlementAuthorizationCode").val('');
        }
      }      
    });
  </script>
  <script type="text/javascript">
    // Set BillingType from Viewdata
    billingType = '<%: ViewData[ViewDataConstants.BillingType] %>';
  </script>
  <script src="<%:Url.Content("~/Scripts/Pax/BillingMemoCouponBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/TaxBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/VatBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Create Billing Memo Coupon</h1>
  <div>
    <% Html.RenderPartial(Url.Content("ReadOnlyBillingMemoHeaderControl"), Model.BillingMemo); %>
  </div>
  <% using (Html.BeginForm(null, null, FormMethod.Post, new { id = "billingMemoCouponBreakdownDetails", @class = "validCharacters" }))
     {  %>
     <%: Html.AntiForgeryToken() %>
  <div>
    <% Html.RenderPartial("BMCouponBreakdownDetailsControl", Model); %>
  </div>
  <div class="buttonContainer">
    <input type="submit" value="Save and Add New" class="primaryButton ignoredirty" id="btnSaveAndAddNew" onclick="javascript:changeAction('<%: Url.Action("BMCouponCreate","Invoice", new { transactionId = Model.BillingMemo.Id.Value(),  invoiceId= Model.BillingMemo.Invoice.Id.Value(), transaction="BMEdit" }) %>')" />
    <input type="submit" value="Save and Duplicate" class="primaryButton ignoredirty" id="btnSaveAndDuplicate" onclick="javascript:changeAction('<%: Url.Action("BMCouponDuplicate","Invoice", new { transactionId = Model.BillingMemo.Id.Value(),  invoiceId= Model.BillingMemo.Invoice.Id.Value(), transaction="BMEdit" }) %>')" />
    <input type="submit" value="Save and Back to Overview" class="primaryButton ignoredirty" id="SaveAndBackToOverview" onclick="javascript:changeAction('<%: Url.Action("BMCouponCreateAndReturn","Invoice", new { transactionId = Model.BillingMemo.Id.Value(),  invoiceId= Model.BillingMemo.Invoice.Id.Value(), transaction="BMEdit" }) %>')" />
    <%: Html.LinkButton("Back", Url.RouteUrl("transactions", new { action = "BMEdit", invoiceId = Model.BillingMemo.InvoiceId.Value(), transactionId = Model.BillingMemo.Id.Value()})) %>
  </div>
  <%
    }%>
  <div class="hidden" id="divTaxBreakdown">
    <% Html.RenderPartial("BMTaxControl", Model.TaxBreakdown);%>
  </div>
  <div class="hidden" id="divVatBreakdown">
    <% Html.RenderPartial("BMCouponVatControl", Model.VatBreakdown);%>
  </div>
  <div class="hidden" id="divAttachment">
    <%
      Html.RenderPartial("BMCouponAttachmentControl", Model);%>
  </div>
  <div id="divProrateSlip" class="hidden">
    <% Html.RenderPartial("ProrateSlipControl", Model.ProrateSlipDetails);%>
  </div>
</asp:Content>
