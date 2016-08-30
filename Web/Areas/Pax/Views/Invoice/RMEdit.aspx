<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.RejectionMemo>" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>

<%@ Import Namespace="Iata.IS.Model.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> :: Non-Sampling Invoice ::
  <%:ViewData[ViewDataConstants.PageMode]%>
  Rejection Memo
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <%
    if (ViewData[ViewDataConstants.PageMode].ToString() != PageMode.View)
    {
  %>
  <%: ScriptHelper.GenerateGridEditDeleteScript(Url, ViewDataConstants.RMCouponListGrid, Url.RouteUrl("breakdown", new { action = "RMCouponEdit", transaction = "RMEdit" }), Url.Action("RMCouponDelete", new { transaction = "RMEdit" }))%>
  <%
    }
    else
    {
  %>
  <%:ScriptHelper.GenerateGridViewScriptForBreakdown(Url, ViewDataConstants.RMCouponListGrid, Url.Action("RMCouponView", new { transaction = "RMEdit" }))%>
  <%
    }
  %>
  <script type="text/javascript">
    // Declare variable to capture reasonCode before editing
    var previousReasonCode = '';
    
    $(document).ready(function () {   
    initializeParentForm('rejectionMemoForm');      
    // Get previous reasonCode value 
    previousReasonCode = $('#ReasonCode').val();
    SetPageToViewModeEx(<%:(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View).ToString().ToLower()%>, '#rejectionMemoForm'); 
    // TODO: Remove this.
    cpnExist = '<%:ViewData[ViewDataConstants.BreakdownExists]%>';

    //Set value to false in case of EDIT
    isFromBillingHistory= false;
    // Get pageMode
    pageMode = '<%: ViewData[ViewDataConstants.PageMode] %>';

    // If CouponBreakdown exists OR Total vat amount difference is equal to 0, hide VatBreakdown link, else show 
    if(cpnExist == 'True' || ($('#TotalVatAmountDifference').val() == '0.00')) {
      $('#vatBreakdown').hide();
    } 
    else {
      $('#vatBreakdown').show();
    }

    // Enable/Disable memo amout fields depending on Reason code mandating couponBreakdown and whether CouponBreakdown exists 
    EnableDisableMemoAmountFieldsInEditMode(cpnExist);

    SetControlAccessibility(cpnExist);

    $("#SourceCodeId").autocomplete({ disabled: true });

    if(cpnExist == 'True')
    $("#ReasonCode").autocomplete({ disabled: true });

    InitializeRMVatGrid(<%= new JavaScriptSerializer().Serialize(Model.RejectionMemoVat) %>);
    InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.Invoice.BilledMemberId,BillingCategoryType.Pax) %>', '<%: Url.Action("RejectionMemoAttachmentDownload","Invoice") %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");      
    InitializeLinkingFieldsInEditMode('<%: Model.IsLinkingSuccessful %>');
    InitializeLinkingSettings(false, '<%:Url.Action("GetRMLinkingDetails", "Invoice", new { area="Pax" })%>','<%:Url.Action("GetLinkedMemoDetailsForRM", "Invoice", new { area="Pax" })%>', '<%:SessionUtil.MemberId %>', '<%: Model.Invoice.BilledMemberId %>','<%: Model.InvoiceId %>');
    InitReferenceData('<%:Url.Action("GetReasonCodeListForAutoComplete", "Data", new { area="" })%>');
    });
    
  </script>

  <script type="text/javascript">
    // Set billing type from Viewdata
    billingType = '<%: ViewData[ViewDataConstants.BillingType] %>';
  </script>
  <script src="<%:Url.Content("~/Scripts/Pax/RejectionMemo.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/VatBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
  <![if IE 7]>
  <script type="text/javascript" src="<%: Url.Content("~/Scripts/json2.js")%>"></script>
  <![endif]>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    <% =ViewData[ViewDataConstants.PageMode]%>
    Rejection Memo
  </h1>
  <div>
    <%
      Html.RenderPartial(Url.Content("ReadOnlyInvoiceHeaderControl"), Model.Invoice);%>
  </div>
  <% using (Html.BeginForm("RMEdit", "Invoice", FormMethod.Post, new { id = "rejectionMemoForm", @class = "validCharacters" }))
     {  %>
     <%: Html.AntiForgeryToken() %>
  <div>
    <% Html.RenderPartial("RMDetailsControl", Model);%>
  </div>
  <div class="buttonContainer">
    <input type="submit" value="Save" class="primaryButton ignoredirty" id="btnSave" />
    <input type="submit" value="Save and Add New" class="primaryButton ignoredirty" id="btnSaveAndAddNew" onclick="javascript:return changeAction('<%: Url.RouteUrl("transactions", new  { action = "RMEditAndAddNew", controller = "Invoice", invoiceId = Model.InvoiceId.Value(), transactionId = Model.Id.Value() } )%>')" />
       
    <!-- If CouponBreakdown is not mandatory and RejectionMemo CouponBreakdown does not exists, popup an Confirm box, else redirect user to Add coupon breakdown -->
    <input type="submit" value="Add Rejection Memo Coupon" class="secondaryButton" id="btnAdd" onclick="javascript:if(checkReasonCode1A('<%:ViewData[ViewDataConstants.BreakdownExists] %>')==true){location.href='<%:Url.Action("RMCouponCreate",
                               "Invoice",
                               new
                                 {
                                   invoiceId = Model.InvoiceId.Value(),
                                   transaction = "RMEdit",
                                   transactionId = Model.Id.Value()
                                 })%>'; return false;}return false;" />

    <%

       if (!string.IsNullOrEmpty(SessionUtil.PaxCorrSearchCriteria) || !string.IsNullOrEmpty(SessionUtil.PaxInvoiceSearchCriteria))
      {
%>
<input class="secondaryButton" type="button" value="Back To Billing History" onclick="javascript:location.href = '<%:Url.Action("Index","BillingHistory",new{back = true})%>';" />
<%--<%: Html.LinkButton("Back To Billing History", Url.Action("Index", "BillingHistory", new { back = true }))%>--%>
<%
      }
       else{
%>
<input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("RMList","Invoice",new{invoiceId = Model.InvoiceId})%>';" />
   <%-- <%: Html.LinkButton("Back", Url.Action("RMList", "Invoice", new { invoiceId = Model.InvoiceId }))%>--%>
    <%
      }
     }
        %>
  </div>
  <h2>
    Rejection Memo Coupon List</h2>
  <div>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.RMCouponListGrid]); %>
  </div>
  <div id="divVatBreakdown" class="hidden">
    <%
      Html.RenderPartial("RMVatControl");%>
  </div>
  <div class="hidden" id="divAttachment">
    <%
      Html.RenderPartial("RejectionMemoAttachmentControl", Model);%>
  </div>
</asp:Content>
