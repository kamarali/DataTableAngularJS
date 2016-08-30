<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Cargo.CargoBillingMemo>" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>

<%@ Import Namespace="Iata.IS.Model.Enums" %>

<%@ Import Namespace="Iata.IS.Model.Pax.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
   SIS :: Cargo :: Receivables :: Create Billing Memo
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script type="text/javascript">
    pageMode =  <%:((ViewData[ViewDataConstants.PageMode]!=null && ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View) ? true : false).ToString().ToLower() %>;
    // Set couponBreakdownExists to "False" in create mode
    couponBreakdownExists = 'False';
    // Set billing type from Viewdata
    billingType = '<%: ViewData[ViewDataConstants.BillingType] %>'; 
  
  $(document).ready(function () {
    $("#AwbSerialNumberCheckDigit").numeric();
    //Keep data autopopulated from billing history transaction.
    isFromBillingHistory = <%=ViewData.ContainsKey(ViewDataConstants.FromBillingHistory).ToString().ToLower()%>;

    if(isFromBillingHistory)
    {
      $('#CorrespondenceReferenceNumber').addClass('populated');   
      $('#SourceCodeId').addClass('populated');  
      $('#ReasonCode').addClass('populated');
      $('#SourceCodeId').attr('readonly','readonly');
      $('#CorrespondenceReferenceNumber').attr('readonly','readonly');
      $('#ReasonCode').attr('readonly','readonly');
      $('#NetBilledAmount').addClass('populated');
      $('#BilledTotalWeightCharge').addClass('populated');
    }
    else
    {
      registerAutocomplete('ReasonCode', 'ReasonCode', '<%:Url.Action("GetReasonCodeListForCargoBillingMemo", "Data", new { area="" })%>', 0, false, onReasonCodeChange, '', <%: Convert.ToInt32(TransactionType.CargoBillingMemo) %>, null, onBlankReasonCode);
    }

    // registerAutocomplete('ReasonCode', 'ReasonCode', '<%:Url.Action("GetReasonCodeListForBillingMemo", "Data", new { area="" })%>', 0, false, onReasonCodeChange, '', null, null, onBlankReasonCode);
   registerAutocomplete('OtherChargeCode', 'OtherChargeCode', '<%:Url.Action("GetOtherChargeCodeList", "Data", new { area = "" })%>', 0, true, null);     
    InitializeVatGrid(<%= new JavaScriptSerializer().Serialize(Model.BillingMemoVat) %>);
    InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.Invoice.BilledMemberId, BillingCategoryType.Cgo) %>', '<%: Url.Action("BillingMemoAttachmentDownload","Invoice") %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");    
    SetPageModeToCreateMode(<%:((string)ViewData[ViewDataConstants.PageMode] == PageMode.Create).ToString().ToLower()%>);

    // Following code is used to pre populate BatchSequenceNumber and RecordSequenceNumber  
    $("#BatchSequenceNumber").val(<%= new JavaScriptSerializer().Serialize(Model.BatchSequenceNumber) %>);
    $("#RecordSequenceWithinBatch").val(<%= new JavaScriptSerializer().Serialize(Model.RecordSequenceWithinBatch) %>);
  });

  </script>
  <script src="<%:Url.Content("~/Scripts/Cargo/CargoBillingMemo.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Cargo/VatBreakdown.js")%>" type="text/javascript"></script>
  <%--<script src="<%:Url.Content("~/Scripts/Cargo/CargoOtherChargeBreakdown.js")%>" type="text/javascript"></script>--%>
  <script src="<%:Url.Content("~/Scripts/Cargo/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.numeric.js") %>" type="text/javascript"></script>
  
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Create Billing Memo</h1>
  <div>
    <%
      Html.RenderPartial("ReadOnlyInvoiceHeaderControl", Model.Invoice);%>
  </div>
  <% using (Html.BeginForm("BMCreate", "Invoice", FormMethod.Post, new { area = "Cargo", id = "cargoBillingMemoForm", @class = "validCharacters" }))
     {  %>
     <%: Html.AntiForgeryToken() %>
  <div>
    <%
      Html.RenderPartial("BMDetailsControl", Model);%>
  </div>
  <div class="buttonContainer">
    <%--SCP#391029: FW: CORRESPONDENCE_REF_NO equals "0" in PAX and Cargo--%>
   <input type="submit" value="Save" class="primaryButton ignoredirty" id="btnSave" onclick="javascript:return changeAction('<%: Url.Action("BMCreate","Invoice") %>')"  />
        <%: Html.LinkButton("Back", Url.Action("BMList", "Invoice", new { invoiceId = Model.InvoiceId }))%>
     <%
      }
        %> 
  
  </div>

  <div id="divVatBreakdown" class="hidden">
    <% Html.RenderPartial("BMVatControl", Model.BillingMemoVat);%>
  </div>
  <div class="hidden" id="divAttachment">
    <%
      Html.RenderPartial("BMAttachmentControl", Model);%>
  </div>
 
</asp:Content>
