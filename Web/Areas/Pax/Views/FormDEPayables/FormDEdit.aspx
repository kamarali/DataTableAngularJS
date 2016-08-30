<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.Sampling.SamplingFormDRecord>" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger ::
  <%: ViewData[ViewDataConstants.BillingType].ToString() %>
  :: <%:(ViewData[ViewDataConstants.PageMode].ToString())%> Sampling Form D Details
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    <%:(ViewData[ViewDataConstants.PageMode].ToString())%> Sampling Form D</h1>
  <% using (Html.BeginForm("FormDEdit", "FormDE", FormMethod.Post, new { id = "formDCoupon", @class = "validCharacters" }))
     {%>
     <%: Html.AntiForgeryToken() %>
  <div>
    <%
       Html.RenderPartial("SamplingInvoiceHeaderInfoControl", Model.Invoice);%>
  </div>
  <div>
    <%
       Html.RenderPartial("FormDDetailsControl", Model);%>
  </div>
  <div class="buttonContainer">
   <%
       if (ViewData[ViewDataConstants.PageMode] != null && ViewData[ViewDataConstants.PageMode].ToString() != PageMode.View)
       {
%>
    <input type="submit" value="Save and Add New" class="primaryButton ignoredirty" id="Submit1" onclick="return changeAction('<%:Url.Action("FormDEdit", "FormDE", new { invoiceId = Model.InvoiceId, transactionId = Model.Id })%>')" />
    <input type="submit" value="Save and Duplicate" class="primaryButton ignoredirty" id="Submit2" onclick="return changeAction('<%:Url.Action("FormDClone", "FormDE", new { invoiceId = Model.InvoiceId, transactionId = Model.Id })%>')" />
    <input type="submit" value="Save and Back to Overview" class="primaryButton ignoredirty" id="Submit3" onclick="return changeAction('<%:Url.Action("FormDEditAndReturn", "FormDE", new { invoiceId = Model.InvoiceId, transactionId = Model.Id })%>')" />
    <%
       }
       if (ViewData[ViewDataConstants.PageMode] != null && ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
       {
         if (!string.IsNullOrEmpty(SessionUtil.PaxCorrSearchCriteria) || !string.IsNullOrEmpty(SessionUtil.PaxInvoiceSearchCriteria))
         {
    %>
    <input class="secondaryButton" type="button" value="Back To Billing History" onclick="javascript:location.href = '<%:Url.Action("Index", "BillingHistory", new { back = true })%>'" />
    <%
         }
         else
         {

    %>
    <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("View",
																			 "FormDEPayables",
																			 new
																				 {
																					 invoiceId = Model.Invoice.Id.Value()
																				 })%>';" />
    <%
         }
       }
       else
       {
         if (!string.IsNullOrEmpty(SessionUtil.PaxCorrSearchCriteria) || !string.IsNullOrEmpty(SessionUtil.PaxInvoiceSearchCriteria))
         {
    %>
    <%: Html.LinkButton("Back To Billing History", Url.Action("Index", "BillingHistory", new { back = true }))%>
    <%
         }
         else
         {
    %>
    <%: Html.LinkButton("Back", Url.Action("Edit", "FormDE", new { invoiceId = Model.Invoice.Id.Value() }))%>
    <%
        }
       }
     }%>
  </div>
  <div id="divTaxBreakdown" class="hidden">
    <% Html.RenderPartial("FormDTaxControl"); %>
  </div>
  <div id="divVatBreakdown" class="hidden">
    <% Html.RenderPartial("FormDVatControl"); %>
  </div>
  <div id="divAttachment" class="hidden">
    <% Html.RenderPartial("SamplingFormDAttachmentControl", Model); %>
  </div>
  <div id="divProrateSlip" class="hidden">
    <% Html.RenderPartial("ProrateSlipControl", Model.ProrateSlipDetails);%>
  </div>
</asp:Content>
<asp:Content ID="CreateFormDScriptContent" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">
  $(document).ready(function () {
    initializeParentForm('formDCoupon');
	  /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: Issuing Airlines auto-complete should exclude pure numeric values having size 5 or greater
        Ref: FRS Section 3.4 Table 17 Row 7 */
      registerAutocomplete('TicketIssuingAirline', 'TicketIssuingAirline', '<%:Url.Action("GetTicketIssuingAirlineListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);
	  registerAutocomplete('TaxCode', 'TaxCode', '<%:Url.Action("GetTaxCodes", "Data", new { area="" })%>', 0, true, null, '', '','');
    var isViewMode = <%:ViewData[ViewDataConstants.PageMode] !=null ?(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View).ToString().ToLower() : "false"%>;
		$isOnView = isViewMode;
		InitializeTaxGrid(<%= new JavaScriptSerializer().Serialize(Model.TaxBreakdown) %>);
		InitializeVatGrid(<%= new JavaScriptSerializer().Serialize(Model.VatBreakdown) %>);
		InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.Invoice.BilledMemberId, BillingCategoryType.Pax) %>', '<%: Url.Action("FormDAttachmentDownload","FormDEPayables") %>', '<%:Url.Content("~/Content/Images/busy.gif")%>');
    InitializeProrateSlip(80, 50);
      
		InitializeFormDControls('<%:Model.Invoice.IsFormABViaIS %>', isViewMode);
        if(isViewMode)
        {
         var $ProrateSlipDetails = $('#ProrateSlipDetailsText');
             $ProrateSlipDetails.removeAttr("disabled");
             $ProrateSlipDetails.attr('readonly', 'readonly');
        }
    
      if ('<%: Convert.ToBoolean(ViewData[ViewDataConstants.IsPostback])%>' != 'True') {
        if ('<%: (string)ViewData[ViewDataConstants.PageMode] == PageMode.Clone %>' == 'True') {
          $('#CouponNumber').focus();
          $("#CouponNumber").val('');
          $("#TicketDocNumber").val('');
          $("#BatchNumberOfProvisionalInvoice").val('');
          $("#RecordSeqNumberOfProvisionalInvoice").val('');
        }
        else {
	        $('#VatBaseAmount').val('');
	        $('#VatPercentage').val('');

          if ('<%: ViewData[ViewDataConstants.SamplingFormDRecord] != null%>' == 'True') {
            SetPageModeToCreateMode('<%:((string)ViewData[ViewDataConstants.PageMode] == PageMode.Create).ToString().ToLower()%>');
            $("#SourceCodeId").val('<%: ViewData[ViewDataConstants.SamplingFormDRecord]%>'.split('-')[0]);
            $("#TicketIssuingAirline").val('<%: ViewData[ViewDataConstants.SamplingFormDRecord]%>'.split('-')[1]);
            $("#CouponNumber").val('');
            $("#TicketDocNumber").val('');
            $('#CouponNumber').focus();
          }
        }
      }
      else {
	      $('#VatBaseAmount').val('');
	      $('#VatPercentage').val('');
      }
	});
  </script>
  <script type="text/javascript">
    // Set billing type from Viewdata
    billingType = '<%: ViewData[ViewDataConstants.BillingType] %>';
  </script>
  <script src="<%:Url.Content("~/Scripts/Pax/SamplingFormD.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/TaxBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/VatBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
</asp:Content>
