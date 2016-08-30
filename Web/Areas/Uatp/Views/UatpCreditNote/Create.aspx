﻿<%@ Page Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MiscUatp.MiscUatpInvoice>" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Iata.IS.Model.MiscUatp.Enums" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<asp:Content ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: UATP :: Receivables :: Create UATP Credit Note
</asp:Content>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Create UATP Credit Note</h1>
  <%
    using (Html.BeginForm("Create", "UatpCreditNote", FormMethod.Post, new { id = "InvoiceForm", @class = "validCharacters" }))
   {%>
   <%: Html.AntiForgeryToken() %>
  <div>
    <%
     Html.RenderPartial("~/Views/MiscUatp/InvoiceHeaderControl.ascx");%>
  </div>
  <div class="buttonContainer">
    <input class="primaryButton ignoredirty" type="submit" value="Save Credit Note Header" id="SaveInvoiceHeader" />
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
<asp:Content ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Misc/InvoiceHeader.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Uatp/TaxBreakdown.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Uatp/VATBreakdown.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Misc/AddCharge.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/MemberReference.js")%>"></script>
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
		$('#BilledMemberText').focus();
		BindEventOnCreateInvoice();
    SetInvoiceType(<%:(Model.InvoiceType == InvoiceType.CreditNote ? 1 : 0)%>);
    InitializeBilateralSMIs(<%: Convert.ToInt32(SMI.Bilateral) %>  ,'<%: ViewData[ViewDataConstants.BilateralSMIs] %>');
		InitialiseInvoiceHeader('<%:Url.Action("GetMiscUatpBilledMemberLocationList", "Data", new { area = "" })%>', '<%:Url.Action("GetExchangeRate", "Data", new { area = "" })%>', '<%:Url.Action("GetRejectionInvoiceDetails", "UatpCreditNote", new { area = "Uatp" })%>', '<%:Url.Action("GetCorrespondenceInvoiceDetails", "UatpCreditNote", new { area = "Uatp" })%>','', '<%:Url.Action("GetDefaultSettlementMethod", "Data", new { area = "" })%>',                                 '<%:Url.Action("GetLocationDetails", "Data", new { area = "" })%>','<%:SessionUtil.MemberId%>', '<%:Model.BillingCategoryId%>',
      '<%:Url.Action("GetDefaultCurrency", "Data", new { area = "" })%>', '<%:Url.Action("GetRejectionStageForSmi", "Data", new { area = "" })%>');

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
     <%
      }
%>

		InitReferenceData('<%:Url.Action("GetMemberLocationDetails", "Data", new { area = string.Empty })%>', '<%:Model.Id%>', '<%:Url.Action("GetSubdivisionNameList", "Data", new { area = string.Empty })%>');
		registerAutocomplete('BilledMemberText', 'BilledMemberId', '<%:Url.Action("GetMemberList", "Data", new { area = "" })%>', 0, true, function (selectedId) { BilledMemberText_AutoCompleteValueChange(selectedId); });    
    InitializeParameters('#AddDetailTemplate', 'AddTl', 'AdditionalDetailDropdown', 'AdditionalDetailDescription', 'RemoveDescription', '#MainAddDetail', "#AddDetail", 20, 80, <%=new JavaScriptSerializer().Serialize(Model.AdditionalDetails.Where(i => i.AdditionalDetailType == AdditionalDetailType.AdditionalDetail).OrderBy(add => add.RecordNumber))%>, '<%:Url.Action("GetAdditionalDetails", "Data", new { area = "" })%>', <%:(int) (AdditionalDetailType.AdditionalDetail)%>, '<%:Convert.ToInt32(AdditionalDetailLevel.Invoice)%>');
		BindEventsForFieldClone();
    InitializeNoteFields(<%=new JavaScriptSerializer().Serialize(Model.AdditionalDetails.Where(i => i.AdditionalDetailType == AdditionalDetailType.Note).OrderBy(add => add.RecordNumber).ToList())%>, '<%:Url.Action("GetAdditionalDetails", "Data", new { area = "" })%>', <%:Convert.ToInt32(AdditionalDetailType.Note)%>, '<%:Convert.ToInt32(AdditionalDetailLevel.Invoice)%>');
    TogglePaymentDetailsDisplay();
    InitializeExhangeRateField('<%:Model.InvoiceTypeId%>');
	});
  </script>
</asp:Content>