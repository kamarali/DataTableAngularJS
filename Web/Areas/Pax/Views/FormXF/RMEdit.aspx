<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.RejectionMemo>" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>

<%@ Import Namespace="Iata.IS.Model.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Passenger :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> :: Non-Sampling Invoice :: Edit Rejection Memo
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
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
    $(document).ready(function () {
      initializeParentForm('rejectionMemoForm');     
      // Set variable to true if PageMode is "View"
      $isOnView = <%:(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View ? true : false ).ToString().ToLower() %>;
      _rmStage = 3;
      registerAutocomplete('ReasonCode', 'ReasonCode', '<%:Url.Action("GetReasonCodeListForAutoComplete", "Data", new { area="" })%>', 0, false, onReasonCodeChangeForFormXF, '', '<%:Convert.ToInt32(TransactionType.SamplingFormXF)%>', null, onBlankReasonCodeForFormXF);
      InitializeVatGrid(<%= new JavaScriptSerializer().Serialize(Model.RejectionMemoVat) %>);
      InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.Invoice.BilledMemberId, BillingCategoryType.Pax) %>', '<%: Url.Action("RMFormFXFAttachmentDownload","FormXF", new {invoiceId = Model.InvoiceId }) %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");
      InitializeFormXFLinking('<%:Model.Invoice.IsFormFViaIS %>', '<%:Url.Action("GetLinkedFormFDetails", "FormXF", new { area="Pax" })%>','<%:Url.Action("GetLinkedMemoAmountDetails", "FormXF", new { area="Pax" })%>', '<%:SessionUtil.MemberId %>', '<%: Model.Invoice.BilledMemberId %>','<%: Model.InvoiceId %>', '<%:Model.Invoice.ProvisionalBillingMonth %>', '<%:Model.Invoice.ProvisionalBillingYear %>');
      InitializeEditRM('<%:ViewData[ViewDataConstants.BreakdownExists]%>');

      // Check whether breakdown exists
      var cpnExist = '<%:ViewData[ViewDataConstants.BreakdownExists]%>';

      // If CouponBreakdown exists OR Total vat amount difference is equal to 0, hide VatBreakdown link, else show 
      if(cpnExist == 'True' || ($('#TotalVatAmountDifference').val() == '0.00')) {
       $('#vatBreakdown').hide();
      } 
      else {
        $('#vatBreakdown').show();
      }

      EnableDisableMemoAmountFieldsInEditMode('<%:ViewData[ViewDataConstants.BreakdownExists]%>');
      });
    </script>
    <%: ScriptHelper.GenerateGridEditDeleteScript(Url, ControlIdConstants.RMCouponGridId, Url.RouteUrl("breakdown", new { action = "RMCouponEdit", transaction = "RMEdit" }), Url.Action("RMCouponDelete", new { transaction = "RMEdit" }))%>  
  <script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        Edit Rejection Memo</h1>
    <div>
        <%
            Html.RenderPartial(Url.Content("~/Areas/Pax/Views/Shared/SamplingInvoiceHeaderInfoControl.ascx"), Model.Invoice); %>
    </div>
    <div>
    <% using (Html.BeginForm("RMEdit", "FormXF", FormMethod.Post, new { id = "rejectionMemoForm", @class = "validCharacters" }))
       {%>
           <%: Html.AntiForgeryToken() %>
       <%  Html.RenderPartial(Url.Content("~/Areas/Pax/Views/BaseFXF/RMDetailsControl.ascx"), Model); %>
    <div class="buttonContainer">
    <input type="submit" value="Save Rejection Memo" class="primaryButton ignoredirty" id="btnSaveAndAddNew"/>

    <input type="submit" value="Add Rejection Memo Coupon" class="secondaryButton" onclick="javascript:if(checkReasonCode1A('<%:ViewData[ViewDataConstants.BreakdownExists] %>')==true){location.href='<%:Url.Action("RMCouponCreate", "FormXF", new {invoiceId = Model.InvoiceId.Value(),transaction = "RMEdit",transactionId = Model.Id.Value()})%>'; return false;}return false;"/>
    <%
      
         if (!string.IsNullOrEmpty(SessionUtil.PaxCorrSearchCriteria) || !string.IsNullOrEmpty(SessionUtil.PaxInvoiceSearchCriteria))
      {
%>
<%: Html.LinkButton("Back To Billing History", Url.Action("Index", "BillingHistory", new { back = true }))%>
<%
      }
      else
      {
%>
    <%: Html.LinkButton("Back", Url.Action("Edit", "FormXF", new { invoiceId = Model.InvoiceId.Value() }))%>
    <%
      }
       } %>    
    </div>
    
  </div>
  <h2>
      Rejection Memo Coupon List
    </h2>
  <div>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.RMCouponListGrid]); %>
    </div>
    
    <div id="divVatBreakdown" class="hidden">
        <%
          Html.RenderPartial("~/Areas/Pax/Views/BaseFXF/RMVatControl.ascx", Model.RejectionMemoVat);%>
    </div>
    <div class="hidden" id="divAttachment">
        <%
            Html.RenderPartial("RMFormXFAttachmentControl", Model);%>
    </div>
</asp:Content>
