<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Cargo.CMAirWayBill>" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<%@ Import Namespace="Iata.IS.Model.Pax.Enums" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<%@ Import Namespace="System.Security.Policy" %>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script type="text/javascript">
    pageMode =  <%:((ViewData[ViewDataConstants.PageMode]!=null && ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View) ? true : false).ToString().ToLower() %>;
    // Set couponBreakdownExists to "False" in create mode
    couponBreakdownExists = 'False';
    // Set billing type from Viewdata
    billingType = '<%: ViewData[ViewDataConstants.BillingType] %>'; 
  
  $(document).ready(function () {
    $("#AwbSerialNumberCheckDigit").numeric();
    // Get AwbSerialNumberCheckDigit field text length
    var textLength = $("#AwbSerialNumberCheckDigit").val().length;
    
    // If field length is less than 8, pad zeros
    if(textLength < 8)
    {
      var value = PadZerosToAwbSerialNumberAndCheckDigitField($("#AwbSerialNumberCheckDigit").val(), 8);
      $("#AwbSerialNumberCheckDigit").val(value);
    }

    // Set variable to true if PageMode is "View"
    $isOnView = <%:((ViewData[ViewDataConstants.PageMode]!= null && ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View ? true : false)).ToString().ToLower() %>;
    SetProrateLadderHeaderFields('<%: Model.ProrateCalCurrencyId%>', '<%: Model.TotalProrateAmount %>');
    registerAutocomplete('OtherChargeCode', 'OtherChargeCode', '<%:Url.Action("GetOtherChargeCodeList", "Data", new { area = "" })%>', 0, true, null);
    registerAutocomplete('ReasonCode', 'ReasonCode', '<%:Url.Action("GetReasonCodeListForCargoBillingMemo", "Data", new { area="" })%>', 0, false, onReasonCodeChange, '', <%: Convert.ToInt32(TransactionType.CargoCreditMemo) %>, null, '');
    /* CMP #596: Length of Member Accounting Code to be Increased to 12 
    Desc: Issuing Airlines auto-complete should exclude pure numeric values having size 5 or greater
    Ref: FRS Section 3.4 Table 17 Row 17 */    
    registerAutocomplete('AwbIssueingAirline', 'AwbIssueingAirline', '<%:Url.Action("GetTicketIssuingAirlineListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);
    registerAutocomplete('CarrierPrefix', 'CarrierPrefix', '<%:Url.Action("GetTicketIssuingAirlineList", "Data", new { area = "" })%>', 0, true, null);
    InitializeVatGrid(<%= new JavaScriptSerializer().Serialize(Model.CMAwbVatBreakdown) %>);
    InitializeOtherChargeGrid(<%= new JavaScriptSerializer().Serialize(Model.CMAwbOtherCharges) %>);  
    InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.CreditMemoRecord.Invoice.BilledMemberId, BillingCategoryType.Cgo) %>', '<%: Url.Action("CreditMemoAwbAttachmentDownload","CreditNotePayables") %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");    
    InitializeProrateLadderGrid(<%= new JavaScriptSerializer().Serialize(Model.CMAwbProrateLadder) %>, '<%:Url.Action("ValidateFromToSectors", "CreditNote") %>','<%: Model.ProrateCalCurrencyId %>');
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
   var prorate_Per = $("#ProratePercentage").val();
  if (prorate_Per == 0) {
     $("#ProratePercentage").val("");
  }
  if ($("#ProvisionalReqSpa").val() != "") {
    $("#ProratePercentage").val("");
    $("#ProratePercentage").attr("readOnly", true);
  }
  else {
    $("#ProratePercentage").removeAttr("readOnly");
  }

  });

  </script>
  <script src="<%:Url.Content("~/Scripts/Cargo/CMAwbPrepaid.js")%>" type="text/javascript"></script>
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
  ::CM AWB Prepaid Invoice :: Create CM AWB Prepaid Billing
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1><%: ViewData[ViewDataConstants.PageMode] %>
    AWB Prepaid Billing</h1>
  <div>
    <%
      Html.RenderPartial(Url.Content("ReadOnlyInvoiceHeaderControl"), Model.CreditMemoRecord.Invoice);%>
  </div>
  <% using (Html.BeginForm(null, null, FormMethod.Post, new { id = "formCMAwbPrepaid", @class = "validCharacters" }))
     {  %>
     <%: Html.AntiForgeryToken() %>
  <div>
    <%
       Html.RenderPartial("CMAWBPrepaidBreakdownDetails", Model);%>
  </div>
  <div class="buttonContainer">
    <input type="submit" value="Save and Add New" class="primaryButton ignoredirty" id="btnSaveAndAddNew"
      onclick="javascript: return SaveCMAwbRecord('<%: Url.Action("CMAwbPrepaidEdit","CreditNote",new { transactionId = Model.CreditMemoRecord.Id.Value(),  invoiceId= Model.CreditMemoRecord.Invoice.Id.Value(), transaction="CMEdit", couponId = Model.Id }) %>')" />
    <input type="submit" value="Save and Duplicate" class="primaryButton ignoredirty"
      id="btnSaveAndDuplicate" onclick="javascript:return SaveCMAwbRecord('<%: Url.Action("CMPrepaidAwbClone","CreditNote",new { transactionId = Model.CreditMemoRecord.Id.Value(),  invoiceId= Model.CreditMemoRecord.Invoice.Id.Value(), transaction="CMEdit", couponId = Model.Id }) %>')" />
    <input type="submit" value="Save and Back to Overview" class="primaryButton ignoredirty"
      id="SaveAndBackToOverview" onclick="javascript:return SaveCMAwbRecord('<%: Url.Action("CMPrepaidAwbEditAndReturn","CreditNote",new { transactionId = Model.CreditMemoRecord.Id.Value(),  invoiceId= Model.CreditMemoRecord.Invoice.Id.Value(), transaction="CMEdit" , couponId = Model.Id}) %>')" />
    <%if (ViewData[ViewDataConstants.PageMode] != null && ViewData[ViewDataConstants.PageMode].ToString() == "View")
      {%>
        <%:Html.LinkButton("Back to View Credit Memo", Url.RouteUrl("CGOtransactions", new { action = "CMView", invoiceId = Model.CreditMemoRecord.InvoiceId.Value(), transactionId = Model.CreditMemoRecord.Id.Value() }))%>
    <%}
    else
    {%>
        <%:Html.LinkButton("Back to View Invoice", Url.RouteUrl("CGOtransactions", new { action = "CMEdit", invoiceId = Model.CreditMemoRecord.InvoiceId.Value(), transactionId = Model.CreditMemoRecord.Id.Value() }))%>
    <%}%>
  </div>
  <%
     }%>

    <div id="divVatBreakdown" class="hidden">
    <% Html.RenderPartial("CMAwbVatControl", Model.CMAwbVatBreakdown);%>
  </div>
  <div class="hidden" id="divAttachment">
    <%
      Html.RenderPartial("CMAwbAttachmentControl", Model);%>
  </div>
  <div class="hidden" id="divOtherCharge">
    <% Html.RenderPartial("CMAwbOtherChargeControl", Model.CMAwbOtherCharges);%>
  </div>
  <div class="hidden" id="divProrateLadder">
    <% Html.RenderPartial("CMAwbProrateLadderControl", Model.CMAwbProrateLadder);%>
  </div>
</asp:Content>


