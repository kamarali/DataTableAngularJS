<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Cargo.RMAwb>" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Iata.IS.Model.Cargo.Enums" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
   SIS :: Cargo :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> :: Add Charge Collect AWB
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script src="<%:Url.Content("~/Scripts/Cargo/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Cargo/VatBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Cargo/CargoOtherChargeBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/Cargo/RMAwb.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/Cargo/ProrateLadder.js") %>" type="text/javascript"></script>
  <script type="text/javascript">
    $(document).ready(function () {
      $("#AwbSerialNumber").numeric();
      $("#serialNoDiv").addClass('hidden');
      initializeParentForm('rejectionMemoChargeCollectAwb');
      CalculateIscAmountsForCC();
      // Set variable to true if PageMode is "View"
      $isOnView = <%:((ViewData[ViewDataConstants.PageMode]!= null && ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View ? true : false)).ToString().ToLower() %>;
      /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: Issuing Airlines auto-complete should exclude pure numeric values having size 5 or greater
        Ref: FRS Section 3.4 Table 17 Row 14 */      
      registerAutocomplete('AwbIssueingAirline', 'AwbIssueingAirline', '<%:Url.Action("GetTicketIssuingAirlineListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);
      registerAutocomplete('OtherChargeCode', 'OtherChargeCode', '<%:Url.Action("GetOtherChargeCodeList", "Data", new { area = "" })%>', 0, true, null);
      registerAutocomplete('CarrierPrefix', 'CarrierPrefix', '<%:Url.Action("GetTicketIssuingAirlineList", "Data", new { area = "" })%>', 0, true, null);      
      InitializeRMVatGrid(<%= new JavaScriptSerializer().Serialize(Model.AwbVat) %>);
      InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.RejectionMemoRecord.Invoice.BilledMemberId, BillingCategoryType.Cgo) %>', '<%: Url.Action("RMAwbAttachmentDownload","Invoice", new { transaction = "RMEdit", transactionId = Model.RejectionMemoRecord.Id, invoiceId = Model.RejectionMemoRecord.InvoiceId } ) %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");
      InitializeOtherChargeGrid(<%= new JavaScriptSerializer().Serialize(Model.OtherCharge) %>);
      InitializeProrateLadderGrid(<%= new JavaScriptSerializer().Serialize(Model.ProrateLadder) %>, '<%:Url.Action("ValidateFromToSectors", "Invoice") %>','<%: Model.ProrateCalCurrencyId %>');
      SetStage('<%:Model.RejectionMemoRecord.RejectionStage %>');
      if ('<%:Model.RejectionMemoRecord.IsLinkingSuccessful%>' == 'True'  &&   '<%:Convert.ToBoolean(ViewData["IsAwbLinkingRequired"])%>' == 'True')
      {
        InitialiseLinking('<%: Url.Action("GetRMAwbBreakdownDetails", "Invoice", "Cargo")%>', '<%: Url.Action("GetRMAwbBreakdownSingleRecordDetails", "Invoice", "Cargo")%>', '<%: Model.RejectionMemoRecord.Id %>', '<%: Model.RejectionMemoRecord.Invoice.BilledMemberId %>', '<%: SessionUtil.MemberId %>', ('True'), '<%: Convert.ToInt32(Iata.IS.Model.Cargo.Enums.BillingCode.AWBChargeCollect) %>', '<%: ViewData[ViewDataConstants.IsExceptionOccurred]%>');
      }
      else
      {
        InitialiseLinking('<%: Url.Action("GetRMAwbBreakdownDetails", "Invoice", "Cargo")%>', '<%: Url.Action("GetRMAwbBreakdownSingleRecordDetails", "Invoice", "Cargo")%>', '<%: Model.RejectionMemoRecord.Id %>', '<%: Model.RejectionMemoRecord.Invoice.BilledMemberId %>', '<%: SessionUtil.MemberId %>', ('False'), '<%: Convert.ToInt32(Iata.IS.Model.Cargo.Enums.BillingCode.AWBChargeCollect) %>', '<%: ViewData[ViewDataConstants.IsExceptionOccurred]%>');
      }
      if ('<%: Convert.ToBoolean(ViewData["IsPostback"])%>' != 'True') // If exception occurs, do not do anything..
      {
        if('<%: Convert.ToBoolean(ViewData["FromClone"])%>' == 'True'){ // If 'Save And Duplicate' is clicked, do not duplicate AWB serial number and attachments.
          SetProrateLadderHeaderFields('<%: Model.ProrateCalCurrencyId%>', '<%: Model.TotalProrateAmount %>');
          $("#AwbSerialNumber").val('');
          $("#AwbIssueingAirline").val('');
          $("#AttachmentIndicatorOriginal").val('No');
        }
        else{
          SetPageModeToCreateMode(<%:((string)ViewData[ViewDataConstants.PageMode] == PageMode.Create).ToString().ToLower()%>);
          $.watermark.showAll();
        }
      }
       $("#VatIdentifierId option[value='3']").remove();
    });
  </script>  
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Add Charge Collect AWB</h1>
  <div>
    <%
      Html.RenderPartial("RMAwbHeaderControl", Model.RejectionMemoRecord);%>
  </div>
  <% using (Html.BeginForm(null, null, FormMethod.Post, new { id = "rejectionMemoChargeCollectAwb", @class = "validCharacters" }))
     {%>
     <%: Html.AntiForgeryToken() %>
  <div>
    <%
       Html.RenderPartial("RMChargeCollectAwbDetailsControl", Model);%>
  </div>
  <div class="buttonContainer">
      <input type="submit" value="Save and Add New" class="primaryButton ignoredirty" id="btnSaveAndAddNew"
      onclick="javascript:return SaveRMAwbRecord('<%: Url.Action("RMChargeCollectAwbCreate","Invoice", new { invoiceId = Model.RejectionMemoRecord.InvoiceId, transactionId = Model.RejectionMemoRecord.Id }) %>')" />
    <input type="submit" value="Save and Duplicate" class="primaryButton ignoredirty"
      id="btnSaveAndDuplicate" onclick="javascript:return SaveRMAwbRecord('<%: Url.Action("RMChargeCollectAwbDuplicate","Invoice", new { invoiceId = Model.RejectionMemoRecord.InvoiceId, transactionId = Model.RejectionMemoRecord.Id }) %>')" />
    <input type="submit" value="Save and Back to Overview" class="primaryButton ignoredirty"
      id="SaveAndBackToOverview" onclick="javascript:return SaveRMAwbRecord('<%: Url.Action("RMChargeCollectAwbCreateAndReturn","Invoice", new { invoiceId = Model.RejectionMemoRecord.InvoiceId, transactionId = Model.RejectionMemoRecord.Id }) %>')" />
    <%: Html.LinkButton("Back", Url.RouteUrl("CGOtransactions", new { action = "RMEdit", invoiceId = Model.RejectionMemoRecord.InvoiceId.Value(), transactionId = Model.RejectionMemoRecord.Id.Value()}))%>
      <%
     }%>   
    <%-- <input class="secondaryButton" type="button" value="Back to Manage Invoice" onclick="javascript:return changeAction('<%: Url.Action("RMCreate","Invoice") %>')" />--%>
  </div>
  <div class="hidden" id="divAttachment">
    <% Html.RenderPartial("RMAwbAttachmentControl", Model);%>
  </div>  
  <div class="hidden" id="divVatBreakdown">
    <% Html.RenderPartial("RMAwbVatControl", Model.AwbVat);%>
  </div>
  <div class="hidden" id="divOtherCharge">
    <% Html.RenderPartial("RMAwbOtherChargeControl", Model.OtherCharge);%>
  </div>
  <div class="hidden">
    <% Html.RenderPartial("~/Areas/Pax/Views/Shared/RMLinkingDuplicateRecordControl.ascx");%>
  </div>
  <div class="hidden" id="divProrateLadder">
    <% Html.RenderPartial("RMAwbProrateLadderControl", Model.ProrateLadder);%>
  </div>
</asp:Content>
