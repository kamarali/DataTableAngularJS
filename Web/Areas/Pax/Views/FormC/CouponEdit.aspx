<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.Sampling.SamplingFormCRecord>" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger ::
  <%: ViewData[ViewDataConstants.BillingType].ToString() %>
  :: Edit Sampling Form C Coupon
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">
    $(document).ready(function () {
    initializeParentForm('formCCoupon');
    SetPageToViewMode(<%: ((string)ViewData[ViewDataConstants.PageMode] == PageMode.View).ToString().ToLower()%>);
    
    InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.SamplingFormC.ProvisionalBillingMemberId, BillingCategoryType.Pax) %>', '<%: Url.Action("CouponAttachmentDownload","FormC", new { invoiceId = Model.SamplingFormCId }) %>', "<%: Url.Content("~/Content/Images/busy.gif") %>");
    <%if(!Model.SamplingFormC.IsLinkingSuccessful && (string)ViewData[ViewDataConstants.PageMode] == PageMode.Edit){ %>
      /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: Issuing Airlines auto-complete should exclude pure numeric values having size 5 or greater
        Ref: FRS Section 3.4 Table 17 Row 6 */
      registerAutocomplete('TicketIssuingAirline', 'TicketIssuingAirline', '<%:Url.Action("GetTicketIssuingAirlineListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);
    <%}%>
    <%if((string)ViewData[ViewDataConstants.PageMode] != PageMode.View){ %>
      registerAutocomplete('ReasonCode', 'ReasonCode', '<%:Url.Action("GetReasonCodeList", "Data", new { area="" })%>', 0, true, null, '', '<%:Convert.ToInt32(TransactionType.SamplingFormC) %>');
    <%}%>
    InitializeLinkingFieldsInEditMode('<%:Model.SamplingFormC.IsLinkingSuccessful%>');
    });
  </script>
  <script type="text/javascript">
    // Set billing type from Viewdata
    billingType = '<%: ViewData[ViewDataConstants.BillingType] %>';
  </script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Pax/FormCCoupon.js")%>"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    <%=ViewData[ViewDataConstants.PageMode] %>
    Sampling Form C Coupon</h1>
  <div>
    <% Html.RenderPartial("ReadOnlyHeaderControl", Model.SamplingFormC); %>
  </div>
  <div class="clear">
  </div>
  <h2>
    Form C Coupon Details</h2>
  <% using (Html.BeginForm("CouponEdit", "FormC", new { invoiceId = Model.SamplingFormC.Id.Value(), transactionId = Model.Id.Value() }, FormMethod.Post, new { id = "formCCoupon", @class = "validCharacters" }))
     { %>
     <%: Html.AntiForgeryToken() %>
  <div>
    <% Html.RenderPartial("CouponDetailsControl", Model); %>
  </div>
  <div class="buttonContainer">
    <input class="primaryButton ignoredirty" type="submit" value="Save" />
    <%if (ViewData[ViewDataConstants.PageMode].ToString() != PageMode.View)
      {%>
    <%: Html.LinkButton("Back", Url.Action("Edit", "FormC", new { invoiceId = Model.SamplingFormCId }))%>
    <%
          }
      else
      {

        if (!string.IsNullOrEmpty(SessionUtil.PaxCorrSearchCriteria) || !string.IsNullOrEmpty(SessionUtil.PaxInvoiceSearchCriteria))
        {
    %>
    <input class="secondaryButton" type="button" value="Back To Billing History" onclick="javascript:location.href = '<%:Url.Action("Index","BillingHistory")%>'" />
    <%
       }
       else
       {
    %>
    <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href='<%:Url.Action("ViewDetails",
                               "FormC",
                               new
                                 {
                                   provisionalBillingMonth = Model.SamplingFormC.ProvisionalBillingMonth,
                                   provisionalBillingYear = Model.SamplingFormC.ProvisionalBillingYear,
                                   provisionalBillingMemberId = Model.SamplingFormC.ProvisionalBillingMemberId,
                                   fromMemberId = Model.SamplingFormC.FromMemberId, 
                                   listingCurrencyId = Model.SamplingFormC.ListingCurrencyId, 
                                   invoiceStatusId = Model.SamplingFormC.InvoiceStatusId
                                 })%>';" />
    <%}
     } %>
  </div>
  <%} %>
  <div class="hidden" id="divAttachment">
    <%
      Html.RenderPartial("FormCRecordAttachmentControl", Model);%>
  </div>
</asp:Content>
