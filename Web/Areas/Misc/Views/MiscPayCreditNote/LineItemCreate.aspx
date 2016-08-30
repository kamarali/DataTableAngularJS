<%@ Page Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MiscUatp.LineItem>" Language="C#"
  MasterPageFile="~/Views/Shared/Site.Master" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>

<%@ Import Namespace="Iata.IS.Model.MiscUatp.Enums" %>
<asp:Content ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Miscellaneous :: Receivables :: Create Line Item
</asp:Content>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Create Line Item</h1>
  <% using (Html.BeginForm("LineItemCreate", "MiscCreditNote", FormMethod.Post, new { id = "LineItemForm", @class = "validCharacters" }))  
     {  %>
     <%: Html.AntiForgeryToken() %>
  <div>
    <% Html.RenderPartial("~/Views/MiscUatp/InvoiceHeaderInfoControl.ascx", Model.Invoice); %>
  </div>
  <div>
    <% Html.RenderPartial("~/Views/MiscUatp/LineItemControl.ascx", Model); %>
  </div>
  <div class="buttonContainer">
    <input class="primaryButton ignoredirty" type="submit" value="Save" id="SaveLineItem" onclick="calculateAmounts();"/>
    <input class="secondaryButton" type="button" value="Attachments" id="UploadAttachment" />
    <% if (ViewData[ViewDataConstants.PageMode] != null && ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
    { %>
      <%: Html.LinkButton("Back", Url.RouteUrl("MiscInvoiceView")) %>
    <%} else {%>
      <%: Html.LinkButton("Back", Url.RouteUrl("MiscInvoiceEdit")) %>
    <%} %>
  </div>
  <%} %>
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
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Misc/FieldCloner.js") %>"></script>
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
    $("#UploadAttachment").bind("click", openAttachment);
    InitializeAddChargeGrid(<%= new JavaScriptSerializer().Serialize(Model.AddOnCharges) %>, true);
    InitializeTaxGrid(<%= new JavaScriptSerializer().Serialize(Model.TaxBreakdown.Where(i=>i.Type==TaxType.Tax)) %>);
    InitializeVatGrid(<%= new JavaScriptSerializer().Serialize(Model.TaxBreakdown.Where(i=>i.Type == TaxType.VAT)) %>);
    InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Invoice.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.Invoice.BilledMemberId, Model.Invoice.BillingCategory) %>', '<%: Url.Action("DeleteAttachment", "MiscCreditNote") %>','<%: Url.Action("InvoiceAttachmentDownload","MiscPayCreditNote") %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");
    InitializeParameters('#AddDetailTemplate', 'AddTl', 'AdditionalDetailDropdown', 'AdditionalDetailDescription', 'RemoveDescription', '#MainAddDetail', "#AddDetail", 10, 80, <%= new JavaScriptSerializer().Serialize(Model.LineItemAdditionalDetails.Where(i => i.AdditionalDetailType == AdditionalDetailType.AdditionalDetail).OrderBy(add => add.RecordNumber))%>, '<%:Url.Action("GetAdditionalDetails","Data",new{area = ""})%>', <%: (int)(AdditionalDetailType.AdditionalDetail) %>, '<%: Convert.ToInt32(AdditionalDetailLevel.LineItem)%>');
    SetPageModeToCreateMode(<%:((string)ViewData[ViewDataConstants.PageMode] == PageMode.Create).ToString().ToLower()%>);
    BindEventsForFieldClone();
    // for credit note only
    $('#UnitPrice').attr('watermark','negativeFourDecimalPlaces');
    $('#ChargeAmount').attr('watermark','negativeAmount');
    InitialiseLineItem('<%: Url.Action("GetChargeCodeTypeList", "Data", new { area = ""})%>', '<%: Url.Action("IsLineItemDetailsExpected", "Data", new { area = ""})%>', '<%: Url.Action("GetChargeCodeDetail", "Data", new { area = ""})%>');
    invoiceType = <%: (int) Model.Invoice.InvoiceType %>;  
  });
  </script>
</asp:Content>
