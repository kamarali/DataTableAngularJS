<%@ Page Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MiscUatp.LineItem>" Language="C#"
  MasterPageFile="~/Views/Shared/Site.Master" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>

<%@ Import Namespace="Iata.IS.Model.Enums" %>

<%@ Import Namespace="Iata.IS.Model.MiscUatp.Enums" %>

<%@ Import Namespace="Iata.IS.Web.Util" %>

<%@ Import Namespace="System.Security.Policy" %>
<asp:Content ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Miscellaneous :: <%:ViewData[ViewDataConstants.BillingType] %> :: <%:ViewData[ViewDataConstants.PageMode] %> Line Item
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    <%:ViewData[ViewDataConstants.PageMode] %> Line Item</h1>
  <% using (Html.BeginForm("LineItemEdit", "MiscInvoice", FormMethod.Post, new { id = "LineItemForm", @class = "validCharacters" }))
     {  %>
     <%: Html.AntiForgeryToken() %>
  <div>
    <% Html.RenderPartial("~/Views/MiscUatp/InvoiceHeaderInfoControl.ascx", Model.Invoice); %>
  </div>
  <div>
    <% Html.RenderPartial("~/Views/MiscUatp/LineItemControl.ascx", Model); %>
  </div>
  <div class="buttonContainer">
    <input class="primaryButton ignoredirty" type="submit" value="Save" id="SaveLineItem" onclick="calculateAmounts();" />
   
     <% if (ViewData[ViewDataConstants.PageMode] != null && ViewData[ViewDataConstants.PageMode].ToString() != PageMode.View)
       {%>
    
         <%: Html.LinkButton("Add New Line Item", Url.RouteUrl("LineItem", new { action = "LineItemCreate", controller = "MiscInvoice" }))%>
         <%: Html.LinkButton("Add Line Item Detail", Url.Action("LineItemDetailCreate"))%>
    
    <% }%>
        
    <input class="secondaryButton" type="button" value="Attachments" id="UploadAttachment" />
    
    <% if (ViewData[ViewDataConstants.PageMode] != null && ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
       {
         if (Request.Form["searchType"] != null && Request.Form["searchType"] == "p")
         {
           TempData["Reject"] = TempData["Reject"];
    %>

      <%:Html.LinkButton("Back", Url.RouteUrl("MiscInvoiceView", new { searchType = "p" }))%>
    <%
      }
      else
         {
           TempData["Reject"] = TempData["Reject"];%>
         <%:Html.LinkButton("Back", Url.RouteUrl("MiscInvoiceView"))%>
         <%}
       } else {%>
      <%: Html.LinkButton("Back", Url.RouteUrl("MiscInvoiceEdit")) %>
    <%} %>
  
  </div>
  <%} %>
  <h2>
    Line Item Details</h2>
  <div>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.LineItemDetailGrid]); %>
  </div>
  <div class="clear">
  </div>
  <div id="TaxBreakdown" class="hidden">
    <% Html.RenderPartial("~/Views/MiscUatp/LineItemTaxControl.ascx", Model.TaxBreakdown.Where(tax => tax.Type == TaxType.Tax).ToList());%>
  </div>
  <div id="VATBreakdown" class="hidden">
    <% Html.RenderPartial("~/Views/MiscUatp/LineItemVATControl.ascx", Model.TaxBreakdown.Where(tax => tax.Type == TaxType.VAT).ToList());%>
  </div>
  <div id="AddChargeBreakdown" class="hidden">
    <% Html.RenderPartial("~/Views/MiscUatp/LineItemAddChargeControl.ascx", Model.AddOnCharges);%>
  </div>
  <div class="hidden" id="divAttachment">
    <% Html.RenderPartial("~/Views/MiscUatp/InvoiceAttachmentControl.ascx", Model.Invoice);%>
  </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">
    // Set BillingType from Viewdata
    billingType = '<%: ViewData[ViewDataConstants.BillingType] %>';
  </script>
  <script type="text/javascript" src="<%: Url.Content("~/Scripts/deleterecord.js")%>"></script>
  <script type="text/javascript" src="<%: Url.Content("~/Scripts/Misc/FieldCloner.js") %>"></script>
  <script type="text/javascript" src="<%: Url.Content("~/Scripts/Misc/LineItem.js") %>"></script>
  <script type="text/javascript" src="<%: Url.Content("~/Scripts/Misc/TaxBreakdown.js") %>"></script>
  <script type="text/javascript" src="<%: Url.Content("~/Scripts/Misc/VATBreakdown.js") %>"></script>
  <script type="text/javascript" src="<%: Url.Content("~/Scripts/Misc/AddCharge.js") %>"></script>
  <script type="text/javascript" src="<%: Url.Content("~/Scripts/validator.js") %>"></script>
  <script src="<%:Url.Content("~/Scripts/Misc/AttachmentBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
  <script type="text/javascript">
  
    $(document).ready(function () {
      initializeParentForm('LineItemForm');
      $('#LocationCode').val('<%:Model.LocationCode%>');
      // Set variable to true if PageMode is "View"
      $isOnView = <%:(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View ? true : false ).ToString().ToLower() %>;

      $("#UploadAttachment").bind("click", openAttachment);
      SetPageToViewModeEx(<%: ((string)ViewData[ViewDataConstants.PageMode] == PageMode.View).ToString().ToLower()%>, '#LineItemForm');
      InitializeAddChargeGrid(<%= new JavaScriptSerializer().Serialize(Model.AddOnCharges) %>, true);
      InitializeTaxGrid(<%= new JavaScriptSerializer().Serialize(Model.TaxBreakdown.Where(i=>i.Type==TaxType.Tax)) %>);
      InitializeVatGrid(<%= new JavaScriptSerializer().Serialize(Model.TaxBreakdown.Where(i=>i.Type == TaxType.VAT)) %>);
      InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Invoice.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.Invoice.BilledMemberId, Model.Invoice.BillingCategory) %>', '<%: Url.Action("DeleteAttachment", "MiscInvoice") %>','<%: Url.Action("LineItemAttachmentDownload","MiscInvoice") %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");
      InitializeParameters('#AddDetailTemplate', 'AddTl', 'AdditionalDetailDropdown', 'AdditionalDetailDescription', 'RemoveDescription', '#MainAddDetail', "#AddDetail", 10, 80, <%= new JavaScriptSerializer().Serialize(Model.LineItemAdditionalDetails.Where(i => i.AdditionalDetailType == AdditionalDetailType.AdditionalDetail).OrderBy(add => add.RecordNumber))%>, '<%:Url.Action("GetAdditionalDetails","Data",new{area = ""})%>', <%: (int)(AdditionalDetailType.AdditionalDetail) %>,  '<%: Convert.ToInt32(AdditionalDetailLevel.LineItem)%>');

      BindEventsForFieldClone();

      InitialiseLineItem('<%: Url.Action("GetChargeCodeTypeList", "Data", new { area = ""})%>', '<%: Url.Action("IsLineItemDetailsExpected", "Data", new { area = ""})%>', '<%: Url.Action("GetChargeCodeDetail", "Data", new { area = ""})%>');
      SetControllAccess(<%:ViewData[ViewDataConstants.TransactionExists]%>);    
      
      InitializeControlProperties(<%:ViewData[ViewDataConstants.FieldMetaDataExists]%>);
                     

      if(<%: ((string)ViewData[ViewDataConstants.PageMode] == PageMode.View).ToString().ToLower()%>)
       { $(".linkImage").hide();
      }
     //CMP#502 : [3.5] Rejection Reason for MISC Invoices
        <%
	if (Model.Invoice.InvoiceType == InvoiceType.RejectionInvoice && Model.Invoice.BillingCategory == BillingCategoryType.Misc && Model.Invoice.RejectionStage == 1)
	{%>
    registerMiscReasonCodeAutocomplete('RejectionReasonCodeText', 'RejectionReasonCode', '<%:Url.Action("GetMiscRejectionReasonCodeListForAutoComplete", "Data", new { area="" })%>', 0, true, null, '', '<%:Convert.ToInt32(TransactionType.MiscRejection1)%>');
    <%
	}%>
  <%
	if (Model.Invoice.InvoiceType == InvoiceType.RejectionInvoice && Model.Invoice.BillingCategory == BillingCategoryType.Misc && Model.Invoice.RejectionStage == 2)
	{%>
    registerMiscReasonCodeAutocomplete('RejectionReasonCodeText', 'RejectionReasonCode', '<%:Url.Action("GetMiscRejectionReasonCodeListForAutoComplete", "Data", new { area="" })%>', 0, true, null, '', '<%:Convert.ToInt32(TransactionType.MiscRejection2)%>');
    <%
	}%>
       invoiceType = <%: (int) Model.Invoice.InvoiceType %>;
    });
  </script>
  
   <% if ((string)ViewData[ViewDataConstants.PageMode] != PageMode.View) 
     { %> 
   <%: ScriptHelper.GenerateGridEditDeleteScript(Url, ControlIdConstants.LineItemDetailGridId, Url.Action("LineItemDetailEdit","MiscInvoice"), Url.Action("LineItemDetailDelete", "MiscInvoice"))%>
   <% }
      else if (Request.Form["searchType"] != null && Request.Form["searchType"] == "p")
    {%>
      <%: ScriptHelper.GenerateGridViewQuerystringScript(Url, ControlIdConstants.LineItemDetailGridId, Url.Action("LineItemDetailView"), "searchType=p")%>
    <%}
     else 
     {%>
   <%: ScriptHelper.GenerateGridViewScript(Url, ControlIdConstants.LineItemDetailGridId, Url.Action("LineItemDetailView"))%>
   <% } %>

</asp:Content>
