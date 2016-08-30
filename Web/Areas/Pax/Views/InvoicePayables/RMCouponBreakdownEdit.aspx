<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.RMCoupon>" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger ::
  <%:ViewData[ViewDataConstants.BillingType].ToString()%>
  :: Non-Sampling Invoice :: Edit Rejection Memo Coupon Breakdown
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    <%=ViewData[ViewDataConstants.PageMode]%>
    Rejection Memo Coupon
  </h1>
  <div>
    <%
      Html.RenderPartial(Url.Content("ReadOnlyRMCouponHeaderControl"), Model.RejectionMemoRecord);%>
  </div>
  <%
    using (Html.BeginForm(null, null, FormMethod.Post, new { id = "rejectionMemoCouponBreakdown" }))
    {%>
    <%: Html.AntiForgeryToken() %>
  <div>
    <%
      Html.RenderPartial("RMCouponDetailsControl", Model);%>
  </div>
  <div class="buttonContainer">
  <% if (Model.RejectionMemoRecord.ReasonCode != "1A")
{%>
    <input type="submit" value="Save and Add New" class="primaryButton ignoredirty" id="SaveAndAddNew" onclick="return changeAction('<%:Url.Action("RMCouponEdit",
                               "Invoice",
                               new { couponId = Model.Id.Value(), transactionId = Model.RejectionMemoId.Value(), invoiceId = Model.RejectionMemoRecord.InvoiceId.Value(), transaction = "RMEdit" })%>')" />
    <input type="submit" value="Save and Duplicate" class="primaryButton ignoredirty" id="SaveAndDuplicate" onclick="return changeAction('<%:Url.Action("RMCouponClone",
                               "Invoice",
                               new { couponId = Model.Id.Value(), transactionId = Model.RejectionMemoId.Value(), invoiceId = Model.RejectionMemoRecord.InvoiceId.Value(), transaction = "RMEdit" })%>')" />
    <%
}%>
    <input type="submit" value="Save and Back to Overview" class="primaryButton ignoredirty" id="SaveAndBackToOverview" onclick="return changeAction('<%:Url.Action("RMCouponEditAndReturn", "Invoice", new { couponId = Model.Id.Value(), transactionId = Model.RejectionMemoId.Value(), invoiceId = Model.RejectionMemoRecord.InvoiceId.Value(), transaction = "RMEdit" })%>')" />
    <%
      if (ViewData[ViewDataConstants.PageMode] != null && ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
      {%>
    <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.RouteUrl("transactions", new { action = "RMView", invoiceId = Model.RejectionMemoRecord.InvoiceId.Value(), transactionId = Model.RejectionMemoRecord.Id.Value() })%>';" />
    <%
        }
        else
        {%>
    <%:Html.LinkButton("Back", Url.RouteUrl("transactions", new { action = "RMEdit", invoiceId = Model.RejectionMemoRecord.InvoiceId.Value(), transactionId = Model.RejectionMemoRecord.Id.Value() }))%>
    <%
        }%>
  </div>
  <%
    }%>
  <div id="divTaxBreakdown" class="hidden">
    <%
      Html.RenderPartial("RMCouponTaxControl");%>
  </div>
  <div id="divVatBreakdown" class="hidden">
    <%
      Html.RenderPartial("RMCouponVatControl");%>
  </div>
  <div class="hidden" id="divAttachment">
    <%
      Html.RenderPartial("RMCouponAttachmentControl", Model);%>
  </div>
  <div id="divProrateSlip" class="hidden">
    <% Html.RenderPartial("ProrateSlipControl", Model.ProrateSlipDetails);%>
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">
    $(document).ready(function () {
      initializeParentForm('rejectionMemoCouponBreakdown');
      // Set variable to true if PageMode is "View"
      $isOnView = <%:(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View ? true : false).ToString().ToLower()%>;
      SetPageToViewMode(<%:(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View).ToString().ToLower()%>);
      /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: Issuing Airlines auto-complete should exclude pure numeric values having size 5 or greater
        Ref: FRS Section 3.4 Table 17 Row 3 */
      registerAutocomplete('TicketOrFimIssuingAirline', 'TicketOrFimIssuingAirline', '<%:Url.Action("GetTicketIssuingAirlineListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);
      registerAutocomplete('TaxCode', 'TaxCode', '<%:Url.Action("GetTaxCodes", "Data", new { area = "" })%>', 0, true, null);
      SetStage(<%:Model.RejectionMemoRecord.RejectionStage%>);            
      InitializeRMTaxGrid(<%=new JavaScriptSerializer().Serialize(Model.TaxBreakdown)%>);
      InitializeRMVatGrid(<%=new JavaScriptSerializer().Serialize(Model.VatBreakdown)%>);
      InitializeAttachmentGrid(<%=new JavaScriptSerializer().Serialize(Model.Attachments)%>, '<%:new FileAttachmentHelper().GetValidFileExtention(Model.RejectionMemoRecord.Invoice.BilledMemberId, BillingCategoryType.Pax)%>', '<%:Url.Action("RMCouponAttachmentDownload",
                                   "InvoicePayables",
                                   new { transaction = "RMEdit", transactionId = Model.RejectionMemoRecord.Id, invoiceId = Model.RejectionMemoRecord.InvoiceId })%>', "<%:Url.Content("~/Content/Images/busy.gif")%>");      
      InitializeLinkingFieldsInEditMode('<%:Model.RejectionMemoRecord.IsLinkingSuccessful%>', '<%:Model.RejectionMemoRecord.SourceCodeId%>');
      InitializeProrateSlip(80, 50);
      
      if ('<%: Convert.ToBoolean(ViewData["IsPostback"])%>' != 'True') {
        if('<%: ViewData["RMCouponRecord"] != null%>' == 'True'){
          $("#TicketOrFimIssuingAirline").val('<%: ViewData["RMCouponRecord"]%>'.split('-')[1]);

          $("#TicketOrFimCouponNumber").val('');
          $("#TicketDocOrFimNumber").val('');
          $("#CheckDigit").val('');
          $('#TicketOrFimCouponNumber').focus();
        }
        else if('<%: Convert.ToBoolean(ViewData["FromClone"])%>' == 'True'){
          $("#TicketOrFimCouponNumber").val('');
          $("#TicketDocOrFimNumber").val('');
          $("#CheckDigit").val('');
          $('#TicketOrFimCouponNumber').focus();
        }
        else{
          $('#TicketOrFimIssuingAirline').focus();
        } 
      }

      });
  </script>
  <script type="text/javascript">
    // Set billing type from Viewdata
    billingType = '<%: ViewData[ViewDataConstants.BillingType] %>';
  </script>
  <script src="<%:Url.Content("~/Scripts/Pax/RejectionMemoCouponBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/RMCouponTaxBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/VatBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/multifile_compressed.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/jquery.blockUI.js")%>" type="text/javascript"></script>
</asp:Content>
