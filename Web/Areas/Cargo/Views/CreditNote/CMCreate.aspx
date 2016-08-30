<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Cargo.CargoCreditMemo>" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<%@ Import Namespace="Iata.IS.Model.Pax.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
   SIS :: Cargo :: Receivables :: Create Credit Memo
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
    registerAutocomplete('ReasonCode', 'ReasonCode', '<%:Url.Action("GetReasonCodeListForAutoComplete", "Data", new { area="" })%>', 0, false, onReasonCodeChange, '', '<%:Convert.ToInt32(TransactionType.CargoCreditMemo)%>', null, onBlankReasonCode);
    InitializeVatGrid(<%= new JavaScriptSerializer().Serialize(Model.VatBreakdown) %>);
    InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.Invoice.BilledMemberId, BillingCategoryType.Cgo) %>', '<%: Url.Action("CreditMemoAttachmentDownload","CreditNote") %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");    
    SetPageModeToCreateMode(<%:((string)ViewData[ViewDataConstants.PageMode] == PageMode.Create).ToString().ToLower()%>);

    // Following code is used to pre populate BatchSequenceNumber and RecordSequenceNumber  
    $("#BatchSequenceNumber").val(<%= new JavaScriptSerializer().Serialize(Model.BatchSequenceNumber) %>);
    $("#RecordSequenceWithinBatch").val(<%= new JavaScriptSerializer().Serialize(Model.RecordSequenceWithinBatch) %>);
      
    $("#VatIdentifierId option[value='3']").remove();
  });

  </script>
  <script src="<%:Url.Content("~/Scripts/Cargo/CargoCreditMemo.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Cargo/VatBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Cargo/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
  
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Create Credit Memo</h1>
  <div>
    <%
      Html.RenderPartial("ReadOnlyCreditNoteHeaderControl", Model.Invoice);%>
  </div>
  <% using (Html.BeginForm("CMCreate", "CreditNote", FormMethod.Post, new { area = "Cargo", id = "cargoCreditMemoForm", @class = "validCharacters" }))
     {  %>
     <%: Html.AntiForgeryToken() %>
  <div>
    <%
      Html.RenderPartial("CMDetailsControl", Model);%>
  </div>
  <div class="buttonContainer">
   <input type="submit" value="Save" class="primaryButton ignoredirty"  onclick="javascript:return changeAction('<%: Url.Action("CMCreate","CreditNote") %>')"  />
        <%: Html.LinkButton("Back", Url.Action("Edit", "CreditNote", new { invoiceId = Model.InvoiceId }))%>
     <%
      }
        %> 
  
  </div>

  <div id="divVatBreakdown" class="hidden">
    <% Html.RenderPartial("CMVatControl", Model.VatBreakdown);%>
  </div>
  <div class="hidden" id="divAttachment">
    <%
      Html.RenderPartial("CMAttachmentControl", Model);%>
  </div>
 
</asp:Content>
