<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.RMCoupon>" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger ::
  <%: ViewData[ViewDataConstants.BillingType].ToString() %>
  :: Non-Sampling Invoice :: Create Rejection Memo Coupon
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">
    $(document).ready(function () {
      initializeParentForm('rejectionMemoCouponBreakdown');
      // Set variable to true if PageMode is "View"
      $isOnView = <%:((ViewData[ViewDataConstants.PageMode]!= null && ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View ? true : false)).ToString().ToLower() %>;
      /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: Issuing Airlines auto-complete should exclude pure numeric values having size 5 or greater
        Ref: FRS Section 3.4 Table 17 Row 3 */
      registerAutocomplete('TicketOrFimIssuingAirline', 'TicketOrFimIssuingAirline', '<%:Url.Action("GetTicketIssuingAirlineListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);
      registerAutocomplete('TaxCode', 'TaxCode', '<%:Url.Action("GetTaxCodes", "Data", new { area = "" })%>', 0, true, null);

      InitializeRMTaxGrid(<%= new JavaScriptSerializer().Serialize(Model.TaxBreakdown) %>);
      InitializeRMVatGrid(<%= new JavaScriptSerializer().Serialize(Model.VatBreakdown) %>);
      InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.RejectionMemoRecord.Invoice.BilledMemberId, BillingCategoryType.Pax) %>', '<%: Url.Action("RMCouponAttachmentDownload","Invoice", new { transaction = "RMEdit", transactionId = Model.RejectionMemoRecord.Id, invoiceId = Model.RejectionMemoRecord.InvoiceId } ) %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");      
      InitializeProrateSlip(80, 50);
      SetStage('<%:Model.RejectionMemoRecord.RejectionStage %>');
      //in case of rejection Memo SessionUtil.MemberId consider as BilledmemberId  and  Model.RejectionMemoRecord.Invoice.BilledMemberId consider as BillingMemberId
      if ('<%:Model.RejectionMemoRecord.IsLinkingSuccessful%>' == 'True'  &&   '<%:Convert.ToBoolean(ViewData["IsAwbLinkingRequired"])%>' == 'True')
      {
        InitialiseLinking('<%: Url.Action("GetRMCouponBreakdownDetails", "Invoice", "Pax")%>', '<%: Url.Action("GetRMCouponBreakdownSingleRecordDetails", "Invoice", "Pax")%>', '<%: Model.RejectionMemoRecord.Id %>', '<%: Model.RejectionMemoRecord.Invoice.BilledMemberId %>', '<%: SessionUtil.MemberId %>', 'True', '<%: Model.RejectionMemoRecord.SourceCodeId%>', '<%: ViewData[ViewDataConstants.IsExceptionOccurred]%>');
      }
      else
      {
        InitialiseLinking('<%: Url.Action("GetRMCouponBreakdownDetails", "Invoice", "Pax")%>', '<%: Url.Action("GetRMCouponBreakdownSingleRecordDetails", "Invoice", "Pax")%>', '<%: Model.RejectionMemoRecord.Id %>', '<%: Model.RejectionMemoRecord.Invoice.BilledMemberId %>', '<%: SessionUtil.MemberId %>', 'False', '<%: Model.RejectionMemoRecord.SourceCodeId%>', '<%: ViewData[ViewDataConstants.IsExceptionOccurred]%>');
      }

      if ('<%: Convert.ToBoolean(ViewData["IsPostback"])%>' != 'True') {
        if('<%: ViewData["RMCouponRecord"] != null%>' == 'True'){
          SetPageModeToCreateMode(<%:((string)ViewData[ViewDataConstants.PageMode] == PageMode.Create).ToString().ToLower()%>);
          $("#TicketOrFimIssuingAirline").val('<%: ViewData["RMCouponRecord"]%>'.split('-')[1]);

          $("#TicketOrFimCouponNumber").val('');
          $("#TicketDocOrFimNumber").val('');
          $("#CheckDigit").val('');
          $('#TicketOrFimCouponNumber').focus();
        }
        else if('<%: Convert.ToBoolean(ViewData["FromClone"])%>' == 'True'){
          $("#TicketOrFimCouponNumber").val('');
          $("#TicketDocOrFimNumber").val('');
          $("#CheckDigit").val('');
          $('#TicketOrFimCouponNumber').focus();
          $("#AttachmentIndicatorOriginal").val('No');
        }
        else{
          SetPageModeToCreateMode(<%:((string)ViewData[ViewDataConstants.PageMode] == PageMode.Create).ToString().ToLower()%>);
          $("#TicketOrFimIssuingAirline").val('');
          $('#TicketOrFimIssuingAirline').focus();
        } 
      }
           
//      // In 'Clone' mode, clear values in TicketOrFimCouponNumber, TicketDocOrFimNumber, CheckDigit fields.
//      if(<%:((string)ViewData[ViewDataConstants.PageMode] == PageMode.Clone).ToString().ToLower()%>){  
//        $("#TicketOrFimCouponNumber").val('');
//        $("#TicketDocOrFimNumber").val('');
//        $("#CheckDigit").val('');
//      }

      // Display Serial Number textbox only in Edit mode and hide in Create mode
      if(<%:((string)ViewData[ViewDataConstants.PageMode] == PageMode.Create || (string)ViewData[ViewDataConstants.PageMode] == PageMode.Clone).ToString().ToLower()%>){
        $("#serialNoDiv").addClass('hidden');  
        $("#SettlementAuthorizationCode").val('');

      }

    });
  </script>
  <script type="text/javascript">
    // Set billing type from Viewdata
    billingType = '<%: ViewData[ViewDataConstants.BillingType] %>';
  </script>
  <script src="<%:Url.Content("~/Scripts/Pax/RejectionMemoCouponBreakdown.js")%>" type="text/JavaScript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/RMCouponTaxBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/VatBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Create Rejection Memo Coupon
  </h1>
  <div>
    <% Html.RenderPartial(Url.Content("ReadOnlyRMCouponHeaderControl"), Model.RejectionMemoRecord); %>
  </div>
  <% using (Html.BeginForm(null, null, FormMethod.Post, new { id = "rejectionMemoCouponBreakdown", @class = "validCharacters" }))
     {%>
     <%: Html.AntiForgeryToken() %>
  <div>
    <%
       Html.RenderPartial("RMCouponDetailsControl", Model);%>
  </div>
  <div class="buttonContainer">
  <% if (Model.RejectionMemoRecord.ReasonCode != "1A")
{%>
    <input type="submit" value="Save and Add New" class="primaryButton ignoredirty" id="SaveAndAddNew" onclick="return changeAction('<%:Url.Action("RMCouponCreate",
                               "Invoice",
                               new { invoiceId = Model.RejectionMemoRecord.InvoiceId.Value(), transaction = "RMEdit", transactionId = Model.RejectionMemoRecord.Id.Value() })%>')" />
    <input type="submit" value="Save and Duplicate" class="primaryButton ignoredirty" id="SaveAndDuplicate" onclick="return changeAction('<%:Url.Action("RMCouponDuplicate",
                               "Invoice",
                               new { invoiceId = Model.RejectionMemoRecord.InvoiceId.Value(), transaction = "RMEdit", transactionId = Model.RejectionMemoRecord.Id.Value() })%>')" />
    <%
}%>
    <input type="submit" value="Save and Back to Overview" class="primaryButton ignoredirty" id="SaveAndBackToOverview" onclick="return changeAction('<%:Url.Action("RMCouponCreateAndReturn", "Invoice", new {invoiceId = Model.RejectionMemoRecord.InvoiceId.Value(),transaction = "RMEdit", transactionId = Model.RejectionMemoRecord.Id.Value()})%>')" />
    <%: Html.LinkButton("Back", Url.RouteUrl("transactions", new { action = "RMEdit", invoiceId = Model.RejectionMemoRecord.InvoiceId.Value(), transactionId = Model.RejectionMemoRecord.Id.Value()}))%>
  </div>
  <%
     }%>
  <div id="divVatBreakdown" class="hidden">
    <%
      Html.RenderPartial("RMCouponVatControl");%>
  </div>
  <div id="divTaxBreakdown" class="hidden">
    <%
      Html.RenderPartial("RMCouponTaxControl");%>
  </div>
  <div class="hidden" id="divAttachment">
    <%
      Html.RenderPartial("RMCouponAttachmentControl", Model);%>
  </div>
  <div class="hidden">
    <% Html.RenderPartial("RMLinkingDuplicateRecordControl");%>
  </div>
  <div id="divProrateSlip" class="hidden">
    <% Html.RenderPartial("ProrateSlipControl", Model.ProrateSlipDetails);%>
  </div>
</asp:Content>
