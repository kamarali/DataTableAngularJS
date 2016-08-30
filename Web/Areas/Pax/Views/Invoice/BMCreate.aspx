<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.BillingMemo>" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>

<%@ Import Namespace="Iata.IS.Model.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> :: Non-Sampling Invoice :: Create Billing Memo
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">
    // Retrieve pagemode
    pageMode =  <%:((ViewData[ViewDataConstants.PageMode]!=null && ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View) ? true : false).ToString().ToLower() %>;
    // Set couponBreakdownExists to "False" in create mode
    couponBreakdownExists = 'False';
    // Set billing type from Viewdata
    billingType = '<%: ViewData[ViewDataConstants.BillingType] %>';
  </script>
  
  <script type="text/javascript">
  $(document).ready(function () {
    // Set variable to true if PageMode is "View"
    $isOnView = <%:(ViewData[ViewDataConstants.PageMode]!= null && ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View ? true : false ).ToString().ToLower() %>;
  
    registerAutocomplete('TaxCode', 'TaxCode', '<%:Url.Action("GetTaxCodes", "Data", new { area = "" })%>', 0, true, null);
    
    //Keep data autopopulated from billing history transaction.
    isFromBillingHistory = <%=ViewData.ContainsKey(ViewDataConstants.FromBillingHistory).ToString().ToLower()%>;

    if(isFromBillingHistory)
    {
      $('#CorrespondenceRefNumber').addClass('populated');   
      $('#SourceCodeId').addClass('populated');  
      $('#ReasonCode').addClass('populated');
      //$('#SourceCodeId').attr('readonly','readonly');
      registerAutocomplete('SourceCodeId', 'SourceCodeId', '<%:Url.Action("GetSourceCodeList", "Data", new { area = "" })%>', 0, true,null,'<%:Convert.ToInt32(TransactionType.BillingMemo) %>');
      $('#CorrespondenceRefNumber').attr('readonly','readonly');
      $('#ReasonCode').attr('readonly','readonly');
      $('#TotalGrossAmountBilled').attr('readonly','readonly');
      $('#TotalGrossAmountBilled').addClass('populated');
      $('#NetAmountBilled').addClass('populated');
    }
    else
    {
      registerAutocomplete('SourceCodeId', 'SourceCodeId', '<%:Url.Action("GetSourceCodeList", "Data", new { area = "" })%>', 0, true,null,'<%:Convert.ToInt32(TransactionType.BillingMemo) %>');      
    //registerAutocomplete('ReasonCode', 'ReasonCode', '<%:Url.Action("GetReasonCodeListForAutoComplete", "Data", new { area="" })%>', 0, false, onReasonCodeChange, '', '<%:Convert.ToInt32(TransactionType.BillingMemo)%>', null, onBlankReasonCode);
    registerAutocomplete('ReasonCode', 'ReasonCode', '<%:Url.Action("GetReasonCodeListForBillingMemo", "Data", new { area="" })%>', 0, false, onReasonCodeChange, '', null, null, onBlankReasonCode);
    }
    
    SetPageModeToCreateMode(<%:((string)ViewData[ViewDataConstants.PageMode] == PageMode.Create).ToString().ToLower()%>);
    InitializeVatGrid(<%= new JavaScriptSerializer().Serialize(Model.VatBreakdown) %>);
    InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.Invoice.BilledMemberId, BillingCategoryType.Pax) %>', '<%: Url.Action("BillingMemoAttachmentDownload","Invoice") %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");    
    });

  </script>
  <script src="<%:Url.Content("~/Scripts/Pax/BillingMemo.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/VatBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
  
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Create Billing Memo</h1>
  <div>
    <% Html.RenderPartial(Url.Content("ReadOnlyInvoiceHeaderControl"), Model.Invoice); %>
  </div>
  <% using (Html.BeginForm("BMCreate", "Invoice", FormMethod.Post, new { id = "billingMemoForm", @class = "validCharacters" }))
     {  %>
     <%: Html.AntiForgeryToken() %>
  <div>
    <% Html.RenderPartial("BMDetailsControl", Model); %>
  </div>
  <div class="buttonContainer">
    <%--SCP#391029: FW: CORRESPONDENCE_REF_NO equals "0" in PAX and Cargo--%>
    <input type="submit" value="Save" class="primaryButton ignoredirty" id="btnSave" />
    <%

       if (!string.IsNullOrEmpty(SessionUtil.PaxCorrSearchCriteria) || !string.IsNullOrEmpty(SessionUtil.PaxInvoiceSearchCriteria))
      {
%>
<input class="secondaryButton" type="button" value="Back To Billing History" onclick="javascript:location.href = '<%:Url.Action("Index","BillingHistory")%>'" />
<%
      }
      else
      {
%>
    <%: Html.LinkButton("Back", Url.Action("BMList", "Invoice", new { invoiceId = Model.InvoiceId }))%>
    <%
      }
        %>   
  </div>
  <%} %>
  <div id="divVatBreakdown" class="hidden">
    <% Html.RenderPartial("BMVatControl", Model.VatBreakdown);%>
  </div>
  <div class="hidden" id="divAttachment">
    <%
      Html.RenderPartial("BillingMemoAttachmentControl", Model);%>
  </div>
</asp:Content>
