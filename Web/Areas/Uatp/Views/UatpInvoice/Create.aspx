﻿<%@ Page Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MiscUatp.MiscUatpInvoice>" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Iata.IS.Model.MiscUatp.Enums" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: UATP :: Receivables :: Create Invoice
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <%if (Model.InvoiceType == InvoiceType.CorrespondenceInvoice)
    {%>
  <h1>
    Create UATP Correspondence Invoice</h1>
  <%
  }
    else if (!Model.IsCreatedFromBillingHistory)
    {%>
  <h1>
    Create UATP Invoice</h1>
  <%
  }
    else
    {%>
  <h1>
    Create UATP Rejection Invoice</h1>
  <%
  }%>
  <%
    using (Html.BeginForm("Create", "UatpInvoice", FormMethod.Post, new { id = "InvoiceForm", @class = "validCharacters" }))
    {%>
    <%: Html.AntiForgeryToken() %>
  <div>
    <%
     Html.RenderPartial("~/Views/MiscUatp/InvoiceHeaderControl.ascx");%>
  </div>
  <div class="buttonContainer">
    <input class="primaryButton ignoredirty" type="submit" value="Save Invoice Header" id="SaveInvoiceHeader" />
    <%
     if (TempData["searchType"] != null && TempData["searchtype"].ToString() == "bh")
     {
       TempData["searchType"] = Request.Form["searchType"]; 
    %><%: Html.LinkButton("Back to Billing History", Url.Action("Index", "BillingHistory", new { back = true }))%>
    <%
     }
     else if (TempData["searchType"] != null && TempData["searchtype"].ToString() == "p")
     {
       TempData["searchType"] = Request.Form["searchType"]; 
    %><%: Html.LinkButton("Back to Manage Invoice", Url.Action("Index", "ManageUatpPayablesInvoice", new { area = "UatpPayables" }))%>
    <%
     }
    %>
  </div>
  <%
   }%>
  <div class="clear">
  </div>
  <div id="BillingMemberReference" class="hidden">
    <%
      Html.RenderPartial("~/Views/Invoice/BillingMemberInfoControl.ascx", Model);%>
  </div>
  <div id="BilledMemberReference" class="hidden">
    <%
      Html.RenderPartial("~/Views/Invoice/BilledMemberInfoControl.ascx", Model);%>
  </div>
  <div id="TaxBreakdown" class="hidden">
    <%
      Html.RenderPartial("~/Views/MiscUatp/TaxControl.ascx", Model.TaxBreakdown.Where(i => i.Type == TaxType.Tax).ToList());%>
  </div>
  <div id="VATBreakdown" class="hidden">
    <%
      Html.RenderPartial("~/Views/MiscUatp/VATControl.ascx", Model.TaxBreakdown.Where(i => i.Type == TaxType.VAT).ToList());%>
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
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/MemberReference.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Misc/InvoiceHeader.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Uatp/TaxBreakdown.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Uatp/VATBreakdown.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Misc/AddCharge.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Misc/FieldCloner.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Misc/AdditionalContact.js")%>"></script>
  <script type="text/javascript">
	$(document).ready(function () {
    <%if(ViewData["IsLegalTextSet"] != null && Convert.ToBoolean(ViewData["IsLegalTextSet"]) == true)
    {%>
    isBillingLegalTextSet = true;
  <%
    }%>
    initializeParentForm('InvoiceForm');
	//	$('#BilledMemberText').focus();
    
    SetInvoiceType(<%:(Model.InvoiceType == InvoiceType.CreditNote ? 1 : 0)%>);
		BindEventOnCreateInvoice();
        
    InitializeBilateralSMIs(<%: Convert.ToInt32(SMI.Bilateral) %>  ,'<%: ViewData[ViewDataConstants.BilateralSMIs] %>');
	  InitialiseInvoiceHeader('<%:Url.Action("GetMiscUatpBilledMemberLocationList", "Data", new { area = "" })%>', 
      '<%:Url.Action("GetExchangeRate", "Data", new { area = "" })%>', 
      '<%:Url.Action("GetRejectionInvoiceDetails", "UatpInvoice", new { area = "Uatp" })%>', 
      '<%:Url.Action("GetCorrespondenceInvoiceDetails", "UatpInvoice", new { area = "Uatp" })%>',
      '', 
      '<%:Url.Action("GetDefaultSettlementMethod", "Data", new { area = "" })%>', 
      '<%:Url.Action("GetLocationDetails", "Data", new { area = "" })%>',
      '<%:SessionUtil.MemberId%>','<%:Model.BillingCategoryId%>',
      '<%:Url.Action("GetDefaultCurrency", "Data", new { area = "" })%>','<%:Url.Action("GetRejectionStageForSmi", "Data", new { area = "" })%>', '<%:Url.Action("IsRejectionOutsideTimeLimit", "UatpInvoice", new { area = "Uatp" })%>');
    InitializeCurrentYearMonth(<%:DateTime.UtcNow.Month%>, <%:DateTime.UtcNow.Year%>);
		InitializeAddChargeGrid(<%=new JavaScriptSerializer().Serialize(Model.AddOnCharges)%>, false);
		InitializeAdditionalContactGrid(<%=new JavaScriptSerializer().Serialize(Model.MemberContacts.Where(i => i.MemberType == MemberType.Billing).ToList())%>, '#contactGrid',true);
		InitializeAdditionalContactGrid(<%=new JavaScriptSerializer().Serialize(Model.MemberContacts.Where(i => i.MemberType == MemberType.Billed).ToList())%>, '#billedContactGrid',false);  
		InitializeTaxGrid(<%=new JavaScriptSerializer().Serialize(Model.TaxBreakdown.Where(i => i.Type == TaxType.Tax))%>);
		InitializeVatGrid(<%=new JavaScriptSerializer().Serialize(Model.TaxBreakdown.Where(i => i.Type == TaxType.VAT))%>);
     <%
      if (Model.BilledMemberId != 0)
      {
%>
        BilledMemberText_AutoCompleteValueChange('<%:Model.BilledMemberId%>');
        //SCP#450827 - Rejections Process
        //Set on Billed Member if Invoice created from Recievable Screen.
        setInvoiceHeaderFocus("True");
     <% }
      else
      {%>
        //SCP#450827 - Rejections Process
        //Set focus on Billed Member if Invoice created from Recievable Screen.
        setInvoiceHeaderFocus("False");
    <%}%>

		InitReferenceData('<%:Url.Action("GetMemberLocationDetails", "Data", new { area = string.Empty })%>', '<%:Model.Id%>', '<%:Url.Action("GetSubdivisionNameList", "Data", new { area = string.Empty })%>');
		
    registerAutocomplete('BilledMemberText', 'BilledMemberId', '<%:Url.Action("GetMemberList", "Data", new { area = "" })%>', 0, true, function (selectedId) { BilledMemberText_AutoCompleteValueChange(selectedId); });    
    InitializeParameters('#AddDetailTemplate', 'AddTl', 'AdditionalDetailDropdown', 'AdditionalDetailDescription', 'RemoveDescription', '#MainAddDetail', "#AddDetail", 20, 80, <%=new JavaScriptSerializer().Serialize(Model.AdditionalDetails.Where(i => i.AdditionalDetailType == AdditionalDetailType.AdditionalDetail).OrderBy(add => add.RecordNumber))%>, '<%:Url.Action("GetAdditionalDetails", "Data", new { area = "" })%>', <%:(int) (AdditionalDetailType.AdditionalDetail)%>, '<%:Convert.ToInt32(AdditionalDetailLevel.Invoice)%>');
		BindEventsForFieldClone();

    InitializeNoteFields(<%=new JavaScriptSerializer().Serialize(Model.AdditionalDetails.Where(i => i.AdditionalDetailType == AdditionalDetailType.Note).OrderBy(add => add.RecordNumber).ToList())%>, '<%:Url.Action("GetAdditionalDetails", "Data", new { area = "" })%>', <%:Convert.ToInt32(AdditionalDetailType.Note)%>,'<%:Convert.ToInt32(AdditionalDetailLevel.Invoice)%>');
    TogglePaymentDetailsDisplay();
    InitiliazeCorrespondenceFields('', false);
    InitializeExhangeRateField('<%:Model.InvoiceTypeId %>');
	});
  
  </script>
</asp:Content>
