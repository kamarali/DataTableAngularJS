<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Cargo.RMAwb>" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Iata.IS.Model.Cargo.Enums" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Cargo :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> ::  <%=ViewData[ViewDataConstants.PageMode]%> Prepaid AWB
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
      SetProrateLadderHeaderFields('<%: Model.ProrateCalCurrencyId%>', '<%: Model.TotalProrateAmount %>');
      isChargeCollect = false;
      CalculateIscAmountsForPrepaid();
      initializeParentForm('rejectionMemoPrepaidAwb');
      SetPageToViewMode(<%:(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View).ToString().ToLower()%>);
      // Set variable to true if PageMode is "View"
      $isOnView = <%:((ViewData[ViewDataConstants.PageMode]!= null && ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View ? true : false)).ToString().ToLower() %>;
      registerAutocomplete('OtherChargeCode', 'OtherChargeCode', '<%:Url.Action("GetOtherChargeCodeList", "Data", new { area = "" })%>', 0, true, null);
      /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: Issuing Airlines auto-complete should exclude pure numeric values having size 5 or greater
        Ref: FRS Section 3.4 Table 17 Row 13 */      
      registerAutocomplete('AwbIssueingAirline', 'AwbIssueingAirline', '<%:Url.Action("GetTicketIssuingAirlineListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);     
      registerAutocomplete('CarrierPrefix', 'CarrierPrefix', '<%:Url.Action("GetTicketIssuingAirlineList", "Data", new { area = "" })%>', 0, true, null);
      InitializeRMVatGrid(<%= new JavaScriptSerializer().Serialize(Model.AwbVat) %>);
      InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.RejectionMemoRecord.Invoice.BilledMemberId, BillingCategoryType.Cgo) %>', '<%: Url.Action("RMAwbAttachmentDownload","InvoicePayables", new { transaction = "RMEdit", transactionId = Model.RejectionMemoRecord.Id, invoiceId = Model.RejectionMemoRecord.InvoiceId } ) %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");
      InitializeProrateLadderGrid(<%= new JavaScriptSerializer().Serialize(Model.ProrateLadder) %>, '<%:Url.Action("ValidateFromToSectors", "Invoice") %>','<%: Model.ProrateCalCurrencyId %>');
      InitializeOtherChargeGrid(<%= new JavaScriptSerializer().Serialize(Model.OtherCharge) %>);
      SetStage('<%:Model.RejectionMemoRecord.RejectionStage %>');
      InitializeLinkingFieldsInEditMode('<%:Model.RejectionMemoRecord.IsLinkingSuccessful%>');      
       $("#VatIdentifierId option[value='3']").remove();
    });
  </script>
  <%--InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.Invoice.BilledMemberId,BillingCategoryType.Cgo) %>', '<%: Url.Action("RejectionMemoAttachmentDownload","Invoice") %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");      --%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
     <%=ViewData[ViewDataConstants.PageMode]%> Prepaid AWB</h1>
  <div>
    <%
      Html.RenderPartial("RMAwbHeaderControl", Model.RejectionMemoRecord);%>
  </div>
  <% using (Html.BeginForm(null, null, FormMethod.Post, new { id = "rejectionMemoPrepaidAwb", @class = "validCharacters" }))
     {%>
     <%: Html.AntiForgeryToken() %>
  <div>
    <%
       Html.RenderPartial("RMPrepaidAwbDetailsControl", Model);%>
  </div>
   <div class="buttonContainer">
     <input type="submit" value="Save and Add New" class="primaryButton ignoredirty" id="btnSaveAndAddNew"
      onclick="javascript:return SaveRMAwbRecord('<%: Url.Action("RMPrepaidAwbEdit","Invoice", new { invoiceId = Model.RejectionMemoRecord.InvoiceId, transactionId = Model.RejectionMemoRecord.Id, transaction = "RMEdit", couponId = Model.Id }) %>')" />
    <input type="submit" value="Save and Duplicate" class="primaryButton ignoredirty"
      id="btnSaveAndDuplicate" onclick="javascript:return SaveRMAwbRecord('<%: Url.Action("RMPrepaidAwbClone","Invoice", new { invoiceId = Model.RejectionMemoRecord.InvoiceId, transactionId = Model.RejectionMemoRecord.Id, transaction = "RMEdit", couponId = Model.Id }) %>')" />
    <input type="submit" value="Save and Back to Overview" class="primaryButton ignoredirty"
      id="SaveAndBackToOverview" onclick="javascript:return SaveRMAwbRecord('<%: Url.Action("RMPrepaidAwbEditAndReturn","Invoice", new { invoiceId = Model.RejectionMemoRecord.InvoiceId, transactionId = Model.RejectionMemoRecord.Id, transaction = "RMEdit", couponId = Model.Id }) %>')" />
     
      <%
     }%>
    <%--<%using (Html.BeginForm("RMAwbProrateLadder", "Invoice", FormMethod.Get))
      {%>
    <input type="submit" value="Prorate Ladder" class="secondaryButton" id="btnRMAwbPrepaidbrkdwn" />
    <%
      }
    %>--%>
      <%
      if (ViewData[ViewDataConstants.PageMode] != null && ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
      {%>      
    <input class="secondaryButton" type="button" value="Back to View Invoice" onclick="javascript:location.href = '<%:Url.RouteUrl("CGOtransactions", new { action = "RMView", invoiceId = Model.RejectionMemoRecord.InvoiceId.Value(), transactionId = Model.RejectionMemoRecord.Id.Value() })%>';" />
    <%
        }
        else
        {%>        
    <%:Html.LinkButton("Back", Url.RouteUrl("CGOtransactions", new { action = "RMEdit", invoiceId = Model.RejectionMemoRecord.InvoiceId.Value(), transactionId = Model.RejectionMemoRecord.Id.Value() }))%>
    <%
        }%>
    <%-- <input class="secondaryButton" type="button" value="Back to Manage Invoice" onclick="javascript:return changeAction('<%: Url.Action("RMCreate","Invoice") %>')" />--%>
  </div>
  <div class="hidden" id="divAttachment">
    <% Html.RenderPartial("RMAwbAttachmentControl", Model);%>
  </div>
  <div id="childAttachmentList" class="">
  </div>
  <div id="childVatList" class="hidden">
  </div>
  <div id="childOtherChargeList" class="hidden">
  </div>
  <div class="hidden" id="divProrateLadder">
    <% Html.RenderPartial("RMAwbProrateLadderControl", Model.ProrateLadder);%>
  </div>
  <div class="hidden" id="divVatBreakdown">
    <% Html.RenderPartial("RMAwbVatControl", Model.AwbVat);%>
  </div>
  <div class="hidden" id="divOtherCharge">
    <% Html.RenderPartial("RMAwbOtherChargeControl", Model.OtherCharge);%>
  </div>
</asp:Content>
