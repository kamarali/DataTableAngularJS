<%@ Page Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MiscUatp.MiscUatpInvoice>" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Iata.IS.Model.MiscUatp.Enums" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<asp:Content ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Miscellaneous ::
  <%:ViewData[ViewDataConstants.BillingType] %>
  ::
  <%:ViewData[ViewDataConstants.PageMode]%>
  Invoice
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">
    // Set BillingType from Viewdata
    billingType = '<%: ViewData[ViewDataConstants.BillingType] %>';
  </script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/deleterecord.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/MemberReference.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Misc/InvoiceHeader.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Misc/TaxBreakdown.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Misc/VATBreakdown.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Misc/AddCharge.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Misc/FieldCloner.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Misc/AdditionalContact.js")%>"></script>
  <script src="<%:Url.Content("~/Scripts/multifile_compressed.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/jquery.blockUI.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Misc/AttachmentBreakdown.js")%>" type="text/javascript"></script>
  <script type="text/javascript">
  $(document).ready(function () {
    <%if(ViewData["IsLegalTextSet"] != null && Convert.ToBoolean(ViewData["IsLegalTextSet"]) == true)
    {%>
    isBillingLegalTextSet = true;
  <%
    }%>
    isOnEditInvoice = true;
    initializeParentForm('InvoiceForm');
    InitializeBilateralSMIs(<%: Convert.ToInt32(SMI.Bilateral) %>  ,'<%: ViewData[ViewDataConstants.BilateralSMIs] %>');
    // Set variable to true if PageMode is "View"
    $isOnView = <%:(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View ? true : false ).ToString().ToLower() %>;
    
    $varView = '<%:ViewData[ViewDataConstants.PageMode]%>';
    var isViewMode = <%:ViewData[ViewDataConstants.PageMode] !=null ?(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View).ToString().ToLower() : "false"%>;
    SetPageToViewModeEx(<%:((string) ViewData[ViewDataConstants.PageMode] == PageMode.View).ToString().ToLower()%>,'#InvoiceForm,#BillingMemberReference,#BilledMemberReference');
    
    SetInvoiceType(<%:(Model.InvoiceType == InvoiceType.CreditNote ? 1 : 0)%>);
    <%
      if (ViewData[ViewDataConstants.TransactionExists] != null)
      {%>
        InitializeEditMode(<%:ViewData[ViewDataConstants.TransactionExists].ToString().ToLower()%>, <%: Model.Attachments.Count %>);
    <%
      }%>

    if(isViewMode == false)
    {
      registerAutocomplete('BilledMemberText', 'BilledMemberId', '<%:Url.Action("GetMemberList", "Data", new { area = "" })%>', 0, true, function (selectedId) { BilledMemberText_AutoCompleteValueChange(selectedId); });
      InitReferenceData('<%:Url.Action("GetMemberLocationDetails", "Data", new { area = string.Empty })%>', '<%:Model.Id%>', '<%:Url.Action("GetSubdivisionNameList", "Data", new { area = string.Empty })%>');
    }
    
    $("#UploadAttachment").bind("click", openAttachment);
    BindEventOnCreateInvoice();
    InitiliazeCorrespondenceFields('<%:Model.Id%>', true);
    InitialiseInvoiceHeader('<%:Url.Action("GetMiscUatpBilledMemberLocationList", "Data", new { area = "" })%>', 
      '<%:Url.Action("GetExchangeRate", "Data", new { area = "" })%>', 
      '<%:Url.Action("GetRejectionInvoiceDetails", "MiscInvoice", new { area = "Misc" })%>', 
      '<%:Url.Action("GetCorrespondenceInvoiceDetails", "MiscInvoice", new { area = "Misc" })%>',
      '', 
      '<%:Url.Action("GetDefaultSettlementMethod", "Data", new { area = "" })%>', 
      '<%:Url.Action("GetLocationDetails", "Data", new { area = "" })%>',
      '<%:SessionUtil.MemberId%>','<%:Model.BillingCategoryId%>',
      '<%:Url.Action("GetDefaultCurrency", "Data", new { area = "" })%>', '<%:Url.Action("GetRejectionStageForSmi", "Data", new { area = "" })%>', '<%:Url.Action("IsRejectionOutsideTimeLimit", "MiscInvoice", new { area = "Misc" })%>');
    InitializeCurrentYearMonth('<%:DateTime.UtcNow.Month%>', '<%:DateTime.UtcNow.Year%>');
    InitializeAddChargeGrid('<%=new JavaScriptSerializer().Serialize(Model.AddOnCharges)%>', false);
    InitializeTaxGrid(<%=new JavaScriptSerializer().Serialize(Model.TaxBreakdown.Where(i => i.Type == TaxType.Tax))%>);
    InitializeVatGrid(<%=new JavaScriptSerializer().Serialize(Model.TaxBreakdown.Where(i => i.Type == TaxType.VAT))%>);
    InitializeAdditionalContactGrid(<%=new JavaScriptSerializer().Serialize(Model.MemberContacts.Where(i => i.MemberType == MemberType.Billing).ToList())%>, '#contactGrid',true);
    InitializeAdditionalContactGrid(<%=new JavaScriptSerializer().Serialize(Model.MemberContacts.Where(i => i.MemberType == MemberType.Billed).ToList())%>, '#billedContactGrid',false);
    
    InitializeParameters('#AddDetailTemplate', 'AddTl', 'AdditionalDetailDropdown', 'AdditionalDetailDescription', 'RemoveDescription', '#MainAddDetail', "#AddDetail", 20, 80, <%=new JavaScriptSerializer().Serialize(Model.AdditionalDetails.Where(i => i.AdditionalDetailType == AdditionalDetailType.AdditionalDetail).OrderBy(add => add.RecordNumber))%>, '<%:Url.Action("GetAdditionalDetails", "Data", new { area = "" })%>', <%:(int) (AdditionalDetailType.AdditionalDetail)%>, '<%:Convert.ToInt32(AdditionalDetailLevel.Invoice)%>');
    InitializeAttachmentGrid(<%=new JavaScriptSerializer().Serialize(Model.Attachments)%>, '<%:new FileAttachmentHelper().GetValidFileExtention(Model.BilledMemberId, Model.BillingCategory)%>','<%:Url.Action("DeleteAttachment", "MiscInvoice")%>','<%:Url.Action("InvoiceAttachmentDownload", "MiscPayInvoice")%>', "<%:Url.Content("~/Content/Images/busy.gif")%>");
    BindEventsForFieldClone();
    
    if(<%:((string) ViewData[ViewDataConstants.PageMode] == PageMode.View).ToString().ToLower()%>)
       $(".linkImage").hide();
    TogglePaymentDetailsDisplay();
    
    <%
      if (ViewData[ViewDataConstants.RejectedInvoiceNumberExist] != null && bool.Parse(ViewData[ViewDataConstants.RejectedInvoiceNumberExist].ToString()))
      {%>
        InitRejectionInvoiceDetail();
      <%
      }%>
//    if($isOnView == false)
//      PopulateRejectionInvDetails();
    InitializeNoteFields(<%=new JavaScriptSerializer().Serialize(Model.AdditionalDetails.Where(i => i.AdditionalDetailType == AdditionalDetailType.Note).OrderBy(add => add.RecordNumber).ToList())%>, '<%:Url.Action("GetAdditionalDetails", "Data", new { area = "" })%>', 2, '<%:Convert.ToInt32(AdditionalDetailLevel.Invoice)%>');
    InitializeExhangeRateField('<%:Model.InvoiceTypeId %>');
    InitializeLocationCode(<%:ViewData[ViewDataConstants.IsLocationCodePresent]%>);
    bindAttachmentDialogCloseEvent();
    PopulateSmiXfields();
  });

  
function RejectInvoice() {
  var selectedLineItemIds = jQuery('#LineItemGrid').getGridParam('selarrrow');
  //SCP0000:Impact on MISC/UATP rejection linking due to purging
  var IsPurged = $("#IsPurged").val();
  if(IsPurged=="True")
  {
    alert('This invoice cannot be rejected as it has been purged.');
    return false;
  }
  if(selectedLineItemIds.length == 0)
  {
    alert('Please select at least one line item.');
    return false;
  }
  var selectedItemNos = '';
  
  var part_num = 0;
  while (part_num < selectedLineItemIds.length)
 {
 
  var gridRow = $("#LineItemGrid").getRowData(selectedLineItemIds[part_num]);
  if(selectedItemNos != '' && selectedItemNos)
    selectedItemNos = gridRow.LineItemNumber + ',' + selectedItemNos; 
    else
    selectedItemNos  = gridRow.LineItemNumber;
  part_num++;
  }
          <%
      
      if (TempData["Reject"] != null && Convert.ToBoolean(TempData["Reject"]) && Request.Form["searchType"] != null && Request.Form["searchType"] == "p") // from payables
      {
      
%>
CallRejectInvoice('<%:Model.Id%>',selectedItemNos, "p", '<%:Url.Action("RejectInvoice", "MiscPayInvoice" )%>');
     <%
      }
      else if (TempData["Reject"] != null && Convert.ToBoolean(TempData["Reject"])) // from billing history
      {%>
     
  CallRejectInvoice('<%:Model.Id%>',selectedItemNos, "bh", '<%:Url.Action("RejectInvoice", "MiscPayInvoice" )%>');
  <%
      }%>

}

  </script>
  <%
    if ((string)ViewData[ViewDataConstants.PageMode] != PageMode.View)
    {%>
  <%: ScriptHelper.GenerateGridEditDeleteScript(Url, ControlIdConstants.LineItemGridId, Url.Action("LineItemEdit", "MiscInvoice"), "LineItemDelete")%>
  <%
    }
    else if (Request.Form["searchType"] != null && Request.Form["searchType"] == "p")
    {
      TempData["Reject"] = TempData["Reject"];%>
      <%: ScriptHelper.GenerateGridViewQuerystringScript(Url, ControlIdConstants.LineItemGridId, Url.Action("LineItemView"), "searchType=p")%>
    <%}
    else
    {
      TempData["Reject"] = TempData["Reject"];%>
  <%: ScriptHelper.GenerateGridViewScript(Url, ControlIdConstants.LineItemGridId, Url.Action("LineItemView"))%>
  <%
    }%>
</asp:Content>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    <%:ViewData[ViewDataConstants.PageMode]%>
    Miscellaneous Invoice</h1>
  <%
      using (Html.BeginForm("Edit", "MiscInvoice", FormMethod.Post, new { id = "InvoiceForm", @class = "validCharacters" }))
    {%>
    <%: Html.AntiForgeryToken() %>
  <div>
    <%
      Html.RenderPartial("~/Views/MiscUatp/InvoiceHeaderControl.ascx");%>
  </div>
  <div class="buttonContainer">
    <input class="primaryButton ignoredirty" type="submit" value="Save Invoice Header" id="SaveInvoiceHeader" />
   
   <% 
      if ((ViewData[ViewDataConstants.PageMode] != null ? (ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View).ToString().ToLower() : "false") == "false")
{%>
    <%:Html.LinkButton("Add Line Item", Url.Action("LineItemCreate"))%>

<%
}%>
    <input class="secondaryButton" type="button" value="Attachments" id="UploadAttachment" />
  </div>
  <%
    }%>
  <div id="TaxBreakdown" class="hidden">
    <%
      Html.RenderPartial("~/Views/MiscUatp/TaxControl.ascx", Model.TaxBreakdown);%>
  </div>
  <div id="VATBreakdown" class="hidden">
    <%
      Html.RenderPartial("~/Views/MiscUatp/VATControl.ascx", Model.TaxBreakdown);%>
  </div>
  <div id="BillingMemberReference" class="hidden">
    <%
      Html.RenderPartial("~/Views/Invoice/BillingMemberInfoControl.ascx", Model);%>
  </div>
  <div id="BilledMemberReference" class="hidden">
    <%
      Html.RenderPartial("~/Views/Invoice/BilledMemberInfoControl.ascx", Model);%>
  </div>
  <div id="BillingAdditionalDetails" class="hidden">
    <%
      Html.RenderPartial("~/Views/MiscUatp/AdditionalContactControl.ascx", Model.MemberContacts.Where(i => i.MemberType == MemberType.Billing).ToList());%>
  </div>
  <div id="BilledAdditionalDetails" class="hidden">
    <%
      Html.RenderPartial("~/Views/MiscUatp/BilledContactControl.ascx", Model.MemberContacts.Where(i => i.MemberType == MemberType.Billed).ToList());%>
  </div>
  <div id="AddChargeBreakdown" class="hidden">
    <%
      Html.RenderPartial("~/Views/MiscUatp/AddChargeControl.ascx", Model.AddOnCharges);%>
  </div>
  <div class="hidden" id="divAttachment">
    <%
      Html.RenderPartial("~/Views/MiscUatp/InvoiceAttachmentControl.ascx", Model);%>
  </div>
  <h2>
    Line Items</h2>
  <div>
    <%
      Html.RenderPartial("GridControl", ViewData[ViewDataConstants.LineItemGrid]);%>
  </div>
  <div class="buttonContainer">
  
    <%
        if ((ViewData["Reject"] != null && Convert.ToBoolean(ViewData["Reject"])) || (TempData["Reject"] != null && Convert.ToBoolean(TempData["Reject"])))
        {
            // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
            if (TempData["canCreate"] != null && Convert.ToBoolean(TempData["canCreate"]) && TempData["canView"] != null && Convert.ToBoolean(TempData["canView"]))
            {%>
                <input type="button" value="Reject" class="primaryButton" onclick="RejectInvoice()" />
          <%}
        }%>
    <%
      if (Model.InvoiceStatus == InvoiceStatusType.Open && Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Misc.Receivables.Invoice.Validate)) // || Model.InvoiceStatus == InvoiceStatusType.ReadyForValidation)
      {
        using (Html.BeginForm("ValidateInvoice", "MiscInvoice", FormMethod.Post))
        {%>
        <%: Html.AntiForgeryToken() %>
    <input class="primaryButton" type="submit" value="Validate Invoice" />
    <%
        }
      }
      if (Model.InvoiceStatus == InvoiceStatusType.ReadyForSubmission && Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Misc.Receivables.Invoice.Submit))
      {
        using (Html.BeginForm("Submit", "MiscInvoice", FormMethod.Post))
        {
    %>
    <%: Html.AntiForgeryToken() %>
    <input class="primaryButton" type="submit" value="Submit Invoice" />
    <%
        }
      }
      if (!string.IsNullOrEmpty(SessionUtil.InvoiceSearchCriteria) && SessionUtil.SearchType == "ManageInvoice")
      {%>
    <input class="secondaryButton" type="button" value="Back to Manage Invoice" onclick="javascript:location.href = '<%:SessionUtil.InvoiceSearchCriteria%>';" />
    <%
      }
    %>
    <%
      if (SessionUtil.SearchType == "MiscBillingHistory")
      {
    %>    
    <%: Html.LinkButton("Back to Billing History", Url.Action("Index", "BillingHistory", new { back = true }))%>
    <%
      }
      else if (!string.IsNullOrEmpty(SessionUtil.InvoiceSearchCriteria) && (SessionUtil.SearchType == "ManagePayablesInvoice" || SessionUtil.SearchType == "ManageDailyPayablesInvoice"))
      {%>
      <input class="secondaryButton" type="button" value="Back to Manage Invoice" onclick="javascript:location.href = '<%:SessionUtil.InvoiceSearchCriteria%>';" />
    <%
      }
    %>
  </div>
  <div class="clear">
  </div>
  <%
    if (Model.InvoiceStatus == InvoiceStatusType.ValidationError)
    {%>
  <h2>
    Validation Errors</h2>
  <div>
    <%
      Html.RenderPartial("GridControl", ViewData[ViewDataConstants.SubmittedErrorsGrid]);%>
  </div>
  <%
    }%>
  <div class="clear">
  </div>
</asp:Content>
