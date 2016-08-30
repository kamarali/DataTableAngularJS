<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Cargo.CargoRejectionMemo>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Cargo ::
  <%: ViewData[ViewDataConstants.BillingType].ToString() %>
  :: Invoice :: <% =ViewData[ViewDataConstants.PageMode]%> Rejection Memo
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <%
    if (ViewData[ViewDataConstants.PageMode].ToString() != PageMode.View)
    {
  %>
  <%: ScriptHelper.GenerateGridEditDeleteScript(Url, ViewDataConstants.RejectionMemoAwbGrid, Url.Action("RMAwbEdit", "Invoice", new { invoiceId = Model.InvoiceId.Value(), transactionId = Model.Id.Value(), transaction = "RMEdit" }), Url.RouteUrl("CGOtransactions", new { action = "RMAwbDelete", controller = "Invoice" }))%>
  <%
    }
    else
    {
  %>
  <%:ScriptHelper.GenerateGridViewScriptForBreakdown(Url, ViewDataConstants.RejectionMemoAwbGrid, Url.Action("RMAwbView", new { invoiceId = Model.InvoiceId.Value(), transactionId = Model.Id.Value(), transaction = "RMEdit" }))%>
  <%
    }
  %>

 <script type="text/javascript">
    // Declare variable to capture reasonCode before editing
    var previousReasonCode = '';
    
    $(document).ready(function () { 
    $("#AwbSerialNumber").numeric();  
    prepaidBillingCodeId = '<%:Convert.ToInt32(Iata.IS.Model.Cargo.Enums.BillingCode.AWBPrepaid) %>';
    chargeCollectBillingCodeId = '<%:Convert.ToInt32(Iata.IS.Model.Cargo.Enums.BillingCode.AWBChargeCollect) %>';

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
    if(cpnExist == 'True' || ($('#TotalVatAmountDifference').val() == '0.000')) {
      $('#vatBreakdown').hide();
    } 
    else {
      $('#vatBreakdown').show();
    }

    // Enable/Disable memo amout fields depending on Reason code mandating couponBreakdown and whether CouponBreakdown exists 
    EnableDisableMemoAmountFieldsInEditMode(cpnExist);

    SetControlAccessibility(cpnExist);

    if(cpnExist == 'True')
    $("#ReasonCode").autocomplete({ disabled: true });

    InitializeRMVatGrid(<%= new JavaScriptSerializer().Serialize(Model.RejectionMemoVat) %>);
    InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.Invoice.BilledMemberId,BillingCategoryType.Cgo) %>', '<%: Url.Action("RejectionMemoAttachmentDownload","InvoicePayables") %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");      
    InitializeLinkingFieldsInEditMode('<%: Model.IsLinkingSuccessful %>');
    InitializeLinkingSettings(false, '<%:Url.Action("GetRMLinkingDetails", "Invoice", new { area="Cargo" })%>','<%:Url.Action("GetLinkedMemoDetailsForRM", "Invoice", new { area="Cargo" })%>', '<%:SessionUtil.MemberId %>', '<%: Model.Invoice.BilledMemberId %>','<%: Model.InvoiceId %>');
    InitReferenceData('<%:Url.Action("GetReasonCodeListForCargo", "Data", new { area="" })%>');
    });
    
  </script>
  <script src="<%:Url.Content("~/Scripts/Cargo/RejectionMemo.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Cargo/VatBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Cargo/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.numeric.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/deleterecord.js") %>" type="text/javascript"></script>
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
    <input type="submit" value="Save and Add New" class="primaryButton ignoredirty" id="btnSaveAndAddNew" onclick="javascript:return changeAction('<%: Url.RouteUrl("CGOtransactions", new  { action = "RMEditAndAddNew", controller = "Invoice", area = "Cargo", invoiceId = Model.InvoiceId.Value(), transactionId = Model.Id.Value() } )%>')" />
    <input type="submit" value="Add Prepaid AWB" class="secondaryButton" id="btnAddChargeCollectAwb" onclick="javascript:return showAmountOverrideWarning('<%:Url.Action("RMPrepaidAwbCreate","Invoice",new{invoiceId = Model.InvoiceId.Value(),transaction = "RMEdit",transactionId = Model.Id.Value()})%>');" />
    <input type="submit" value="Add Charge Collect AWB" class="secondaryButton" id="btnAddPrepaidAwb" onclick="javascript:return showAmountOverrideWarning('<%:Url.Action("RMChargeCollectAwbCreate","Invoice",new{invoiceId = Model.InvoiceId.Value(),transaction = "RMEdit",transactionId = Model.Id.Value()})%>');" />

    <%

       if (!string.IsNullOrEmpty(SessionUtil.PaxCorrSearchCriteria) || !string.IsNullOrEmpty(SessionUtil.PaxInvoiceSearchCriteria))
      {
%>
<%: Html.LinkButton("Back To Billing History", Url.Action("Index", "BillingHistory", new { back = true }))%>
<%
      }
       else{
%>
<%if (ViewData[ViewDataConstants.PageMode] != null && ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
  {%>
  
    <%:Html.LinkButton("Back",
                                      Url.Action("RMList", "InvoicePayables",
                                                 new
                                                     {
                                                         invoiceId = Model.InvoiceId,
                                                         billingType = ViewData[ViewDataConstants.BillingType].ToString()
                                                     }))%>
    <%
  }%>
<%else
{%>
<%:Html.LinkButton("Back",
                                      Url.Action("RMList", "Invoice",
                                                 new
                                                     {
                                                         invoiceId = Model.InvoiceId,
                                                         billingType = ViewData[ViewDataConstants.BillingType].ToString()
                                                     }))%>
<%
}%>
    <%
      }
     }
        %>
  </div>
  <h2>
    Rejection Memo AWB List</h2>
  <div>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.RejectionMemoAwbGrid]); %>
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
