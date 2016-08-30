<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.RejectionMemo>" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>


<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger ::
  <%: ViewData[ViewDataConstants.BillingType].ToString() %>
  :: Non-Sampling Invoice :: Create Rejection Memo
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">    
    $(document).ready(function () {   
      initializeParentForm('rejectionMemoForm');        
      // TODO: Transaction type needs to pass based on Rejection stage.
      registerAutocomplete('TaxCode', 'TaxCode', '<%:Url.Action("GetTaxCodes", "Data", new { area = "" })%>', 0, true, null);
      // Get pageMode
      pageMode =  <%:((ViewData[ViewDataConstants.PageMode]!=null && ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View) ? true : false).ToString().ToLower() %>;;


      // Hide vatBreakdown link
      $('#vatBreakdown').hide();

      isFromBillingHistory = <%=ViewData.ContainsKey(ViewDataConstants.FromBillingHistory).ToString().ToLower()%>;
      //Keep data autopopulated from billing history transaction.
      if(isFromBillingHistory)
      {
        SetSourceCode('<%: ViewData[ViewDataConstants.IsPostback] != null && ViewData[ViewDataConstants.IsPostback].ToString() == "True" %>');
        var $FimBMCMNumber = $('#FimBMCMNumber');
        var $FimCouponNumber = $('#FimCouponNumber');
        SetBillingHistoryControlData();

        <% if(Model.FIMBMCMIndicatorId == (int)FIMBMCMIndicator.FIMNumber)
           {%>
            $FimBMCMNumber.attr('readonly', 'readonly');
            $FimCouponNumber.attr('readonly', 'readonly');
            <%
           }else if(Model.FIMBMCMIndicatorId == (int)FIMBMCMIndicator.BMNumber || Model.FIMBMCMIndicatorId == (int)FIMBMCMIndicator.CMNumber)
           {%>
            $FimBMCMNumber.attr('readonly', 'readonly');
            $('#TotalGrossAmountBilled').addClass('populated');
            $('#TotalGrossAcceptedAmount').addClass('populated');
        <%
           }%>
      }
      else
      {
         registerAutocomplete('SourceCodeId', 'SourceCodeId', '<%:Url.Action("GetSourceCodeList", "Data", new { area = "" })%>', 0, true,null,'<%:Convert.ToInt32(TransactionType.RejectionMemo1)%>');
      }

      //SCP200973 - ERROR IN FILE AUG-13 -  Irrespective of any reason code, the Amount fields should be disable at create RM level. 
      // 'NA' paramamter is used to execute the else part of below function. It means Read Only will assign to all amount fields
      EnableDisableMemoAmountFieldsInEditMode('NA');

      SetPageModeToCreateMode( <%:((string)ViewData[ViewDataConstants.PageMode] == PageMode.Create).ToString().ToLower()%>);
      InitializeRMVatGrid(<%= new JavaScriptSerializer().Serialize(Model.RejectionMemoVat) %>);      
      InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.Invoice.BilledMemberId,BillingCategoryType.Pax) %>', '<%: Url.Action("RejectionMemoAttachmentDownload","Invoice") %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");      
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
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
  <![if IE 7]>
  <script type="text/javascript" src="<%: Url.Content("~/Scripts/json2.js")%>"></script>
  <![endif]>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        Create Rejection Memo</h1>
    <div>
        <%
            Html.RenderPartial(Url.Content("ReadOnlyInvoiceHeaderControl"), Model.Invoice);%>
    </div>
    <% using (Html.BeginForm("RMCreate", "Invoice", FormMethod.Post, new { id = "rejectionMemoForm", @class = "validCharacters" }))
       {  %>
       <%: Html.AntiForgeryToken() %>
    <div>
        <%
            Html.RenderPartial("RMDetailsControl", Model);%>
    </div>
    <div class="buttonContainer">
        <input type="submit" value="Save" class="primaryButton ignoredirty" id="btnSave" onclick="javascript:return changeAction('<%: Url.Action("RMCreate","Invoice") %>')" />
        <input type="submit" value="Save and Add New" class="primaryButton ignoredirty" id="btnSaveAndAddNew" onclick="javascript:return changeAction('<%: Url.Action("RMCreateAndAddNew","Invoice") %>')" />
        <%

       if (!string.IsNullOrEmpty(SessionUtil.PaxCorrSearchCriteria) || !string.IsNullOrEmpty(SessionUtil.PaxInvoiceSearchCriteria))
      {
%>
<%: Html.LinkButton("Back To Billing History", Url.Action("Index", "BillingHistory", new { back = true }))%>
<%
  }
       else if (!string.IsNullOrEmpty(SessionUtil.InvoiceSearchCriteria))
{	%>
  <%: Html.LinkButton("Back To Invoice Search", Url.RouteUrl("InvoiceSearch", new { action = "Index", billingType = "Receivables" }))%>
<%}
      else
      {
%>
    <%: Html.LinkButton("Back", Url.Action("RMList", "Invoice", new { invoiceId = Model.InvoiceId }))%>
    <%
      }
     }
        %>
        </div>
    <div id="divVatBreakdown" class="hidden">
        <%
            Html.RenderPartial("RMVatControl");%>
    </div>
    <div class="hidden" id="divAttachment">
        <%
            Html.RenderPartial("RejectionMemoAttachmentControl", Model);%>
    </div>
    <div class="hidden">
    <% Html.RenderPartial("RMLinkingDuplicateRecordControl");%>
    </div>
</asp:Content>

