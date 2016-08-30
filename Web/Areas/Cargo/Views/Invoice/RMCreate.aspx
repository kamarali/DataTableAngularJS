<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Cargo.CargoRejectionMemo>" %>

<%@ Import Namespace="Iata.IS.Model.Enums" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Iata.IS.Model.Cargo.Enums" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Cargo ::
  <%: ViewData[ViewDataConstants.BillingType].ToString() %>
  :: Create Invoice :: Create Rejection Memo
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">
    $(document).ready(function () {
      $("#AwbSerialNumber").numeric();
      initializeParentForm('rejectionMemoForm');
      
      // Get pageMode
      pageMode =  <%:((ViewData[ViewDataConstants.PageMode]!=null && ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View) ? true : false).ToString().ToLower() %>;;
      // Set billing type from Viewdata
      billingType = '<%: ViewData[ViewDataConstants.BillingType] %>';
      isFromBillingHistory = <%=ViewData.ContainsKey(ViewDataConstants.FromBillingHistory).ToString().ToLower()%>;
      
       //Keep data autopopulated from billing history transaction.
      if(isFromBillingHistory)
      {
        SetBillingHistoryControlData();
        <% if(Model.BMCMIndicatorId == (int)BMCMIndicator.BMNumber || Model.BMCMIndicatorId == (int)BMCMIndicator.CMNumber){%>
            $('#TotalGrossAmountBilled').addClass('populated');
            $('#TotalGrossAcceptedAmount').addClass('populated');
        <%}%>
      }
      
      InitializeLinkingSettings(false, '<%:Url.Action("GetRMLinkingDetails", "Invoice", new { area="Cargo" })%>','<%:Url.Action("GetLinkedMemoDetailsForRM", "Invoice", new { area="Cargo" })%>', '<%:SessionUtil.MemberId %>', '<%: Model.Invoice.BilledMemberId %>','<%: Model.InvoiceId %>');

      // Hide vatBreakdown link
      $('#vatBreakdown').hide();

      //SCP200973 - ERROR IN FILE AUG-13 -  Irrespective of any reason code, the Amount fields should be disable at create RM level. 
      // 'NA' paramamter is used to execute the else part of below function. It means Read Only will assign to all amount fields
      EnableDisableMemoAmountFieldsInEditMode('NA');

     SetPageModeToCreateMode( <%:((string)ViewData[ViewDataConstants.PageMode] == PageMode.Create).ToString().ToLower()%>);
     InitializeLinkingSettings(false, '<%:Url.Action("GetRMLinkingDetails", "Invoice", new { area="Cargo" })%>','<%:Url.Action("GetLinkedMemoDetailsForRM", "Invoice", new { area="Cargo" })%>', '<%:SessionUtil.MemberId %>', '<%: Model.Invoice.BilledMemberId %>','<%: Model.InvoiceId %>');
     InitializeRMVatGrid(<%= new JavaScriptSerializer().Serialize(Model.RejectionMemoVat) %>);      
     InitReferenceData('<%:Url.Action("GetReasonCodeListForCargo", "Data", new { area="" })%>');
     InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.Invoice.BilledMemberId,BillingCategoryType.Cgo) %>', '<%: Url.Action("RejectionMemoAttachmentDownload","Invoice") %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");

     // Following code is used to pre populate BatchSequenceNumber and RecordSequenceNumber  
    $("#BatchSequenceNumber").val(<%= new JavaScriptSerializer().Serialize(Model.BatchSequenceNumber) %>);
    $("#RecordSequenceWithinBatch").val(<%= new JavaScriptSerializer().Serialize(Model.RecordSequenceWithinBatch) %>);
    // Autopopulate reason code, if any value is present inside reason code
    $("#ReasonCode").val(<%= new JavaScriptSerializer().Serialize(Model.ReasonCode) %>);
    });

  </script>
  <script src="<%:Url.Content("~/Scripts/Cargo/RejectionMemo.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Cargo/VatBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Cargo/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.numeric.js") %>" type="text/javascript"></script>
  <![if IE 7]>
  <script type="text/javascript" src="<%: Url.Content("~/Scripts/json2.js")%>"></script>
  <![endif]>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Create Rejection Memo</h1>
  <div>
    <%Html.RenderPartial("ReadOnlyInvoiceHeaderControl", Model.Invoice);%>
  </div>
  <% using (Html.BeginForm("RMCreate", "Invoice", FormMethod.Post, new { id = "rejectionMemoForm", @class = "validCharacters" }))
     {%> 
     <%: Html.AntiForgeryToken() %>
  <div>
    <%
       Html.RenderPartial("RMDetailsControl", Model);%>
  </div>
  <div class="buttonContainer">
    <input type="submit" value="Save" class="primaryButton ignoredirty" id="btnSave"
      onclick="javascript:return changeAction('<%: Url.Action("RMCreate","Invoice") %>')" />
    <input type="submit" value="Save and Add New" class="primaryButton ignoredirty" id="btnSaveAndAddNew"
      onclick="javascript:return changeAction('<%: Url.Action("RMCreateAndAddNew","Invoice") %>')" />
           <%

       if (!string.IsNullOrEmpty(SessionUtil.CGOCorrSearchCriteria) || !string.IsNullOrEmpty(SessionUtil.CGOInvoiceSearchCriteria))
      {
%>
<%: Html.LinkButton("Back To Billing History", Url.Action("Index", "BillingHistory", new { back = true }))%>
<%
  }
       else if (!string.IsNullOrEmpty(SessionUtil.InvoiceSearchCriteria))
{	%>
  <%: Html.LinkButton("Back To Invoice Search", Url.RouteUrl("InvoiceSearchCGO", new { action = "Index", billingType = "Receivables" }))%>
<%}
      else
      {
%>
    <%: Html.LinkButton("Back", Url.Action("RMList", "Invoice", new { invoiceId = Model.InvoiceId }))%>
    <%
      }
        %>
  </div>
  <% }%>
  <div id="divVatBreakdown" class="hidden">
    <%Html.RenderPartial("RMVatControl");%>
  </div>
  <div class="hidden" id="divAttachment">
    <% Html.RenderPartial("RejectionMemoAttachmentControl", Model);%>
  </div>
  <div class="hidden">
    <% Html.RenderPartial("~/Areas/Pax/Views/Shared/RMLinkingDuplicateRecordControl.ascx");%>
  </div>
</asp:Content>
