<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.RejectionMemo>" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>

<%@ Import Namespace="Iata.IS.Model.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> :: Create Form XF Rejection Memo
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">
    // Set billing type from Viewdata
    billingType = '<%: ViewData[ViewDataConstants.BillingType] %>';
  </script>
  <script src="<%:Url.Content("~/Scripts/Pax/SamplingRM.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/VatBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
  <script type="text/javascript">
   isFromBillingHistory = false;
    $(document).ready(function () 
    {
    initializeParentForm('rejectionMemoForm');     
    isFromBillingHistory = <%=ViewData.ContainsKey(ViewDataConstants.FromBillingHistory).ToString().ToLower()%>;
      //Keep data autopopulated from billing history transaction.
      if(isFromBillingHistory)
      {
        $('#YourInvoiceNumber').addClass('populated');
        $('#YourRejectionNumber').addClass('populated');
        $('#SourceCodeId').addClass('populated'); 
        $('#FimBMCMNumber').addClass('populated');
        $('#YourInvoiceNumber').attr('readonly','true');
        $('#YourInvoiceBillingYear').attr('disabled','disabled');
        $('#YourInvoiceBillingMonth').attr('disabled','disabled');
        $('#YourInvoiceBillingPeriod').attr('disabled','disabled');
        $('#FIMBMCMIndicatorId').attr('disabled','disabled');
        $('#SaveButton').removeAttr('disabled');
        $('#YourRejectionNumber').attr('readonly', true);
      }

      // Set variable to true if PageMode is "View"
      $isOnView = <%:(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View ? true : false ).ToString().ToLower() %>;
      _rmStage = 3;
      registerAutocomplete('ReasonCode', 'ReasonCode', '<%:Url.Action("GetReasonCodeListForAutoComplete", "Data", new { area="" })%>', 0, false, onReasonCodeChangeForFormXF, '', '<%:Convert.ToInt32(TransactionType.SamplingFormXF)%>', null, onBlankReasonCodeForFormXF);
      InitializeVatGrid(<%= new JavaScriptSerializer().Serialize(Model.RejectionMemoVat) %>);    
       <%
      if(ViewData[ViewDataConstants.PageMode] == null || ViewData[ViewDataConstants.PageMode].ToString() != "Clone")
      {
      %>
        InitializeRMCreate();
      <%
      }%>
       $("#AttachmentIndicatorOriginal").val("No");  
      InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.Invoice.BilledMemberId, BillingCategoryType.Pax) %>', '<%: Url.Action("RMFormFXFAttachmentDownload","FormXF") %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");
      InitializeFormXFLinking('<%:Model.Invoice.IsFormFViaIS %>', '<%:Url.Action("GetLinkedFormFDetails", "FormXF", new { area="Pax" })%>','<%:Url.Action("GetLinkedMemoAmountDetails", "FormXF", new { area="Pax" })%>', '<%:SessionUtil.MemberId %>', '<%: Model.Invoice.BilledMemberId %>','<%: Model.Invoice.Id %>', '<%:Model.Invoice.ProvisionalBillingMonth %>', '<%:Model.Invoice.ProvisionalBillingYear %>');
      InitializeFormXFLinkingInCreateMode('<%:Model.Invoice.IsFormFViaIS %>');

      // Hide vatBreakdown link
      $('#vatBreakdown').hide();
    });

  </script>
</asp:Content>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Create Form XF Rejection Memo
  </h1>
  <div>
    <% Html.RenderPartial(Url.Content("~/Areas/Pax/Views/Shared/SamplingInvoiceHeaderInfoControl.ascx"), Model.Invoice); %>
  </div>
  <div>
    <% using (Html.BeginForm("RMCreate", "FormXF", FormMethod.Post, new { id = "rejectionMemoForm", @class = "validCharacters" }))
       { %>
           <%: Html.AntiForgeryToken() %>
       <%  Html.RenderPartial(Url.Content("~/Areas/Pax/Views/BaseFXF/RMDetailsControl.ascx"), Model); %>
    <div class="buttonContainer">
      <input type="submit" value="Save Rejection Memo" class="primaryButton ignoredirty" id="btnSaveAndAddNew" />
      <%: Html.LinkButton("Back", Url.Action("Edit", "FormXF", new { invoiceId = Model.Invoice.Id.Value() }))%>
    </div>
    <% } %>
  </div>
  <div id="divVatBreakdown" class="hidden">
    <%
      Html.RenderPartial("~/Areas/Pax/Views/BaseFXF/RMVatControl.ascx", Model.RejectionMemoVat);%>
  </div>
  <div class="hidden" id="divAttachment">
    <%
      Html.RenderPartial("RMFormXFAttachmentControl", Model);%>
  </div>
  <div class="hidden">
    <% Html.RenderPartial("RMLinkingDuplicateRecordControl");%>
  </div>
</asp:Content>
