<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.Sampling.SamplingFormCRecord>" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>

<%@ Import Namespace="Iata.IS.Model.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> :: Create Sampling Form C Coupon
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript"> 
    $(document).ready(function () {
      initializeParentForm('formCCoupon');
      SetPageModeToCreateMode(<%:((string)ViewData[ViewDataConstants.PageMode] == PageMode.Create).ToString().ToLower()%>);
      InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.SamplingFormC.ProvisionalBillingMemberId, BillingCategoryType.Pax) %>', '<%: Url.Action("CouponAttachmentDownload","FormC") %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");
      /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: Issuing Airlines auto-complete should exclude pure numeric values having size 5 or greater
        Ref: FRS Section 3.4 Table 17 Row 6 */
      registerAutocomplete('TicketIssuingAirline', 'TicketIssuingAirline', '<%:Url.Action("GetTicketIssuingAirlineListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);
      registerAutocomplete('ReasonCode', 'ReasonCode', '<%:Url.Action("GetReasonCodeList", "Data", new { area="" })%>', 0, true, null, '', '<%:Convert.ToInt32(TransactionType.SamplingFormC) %>');
      InializeLinking('<%: Model.SamplingFormC.IsLinkingSuccessful %>','<%:SessionUtil.MemberId %>','<%:Model.SamplingFormC.ProvisionalBillingMemberId %>','<%:Model.SamplingFormC.ProvisionalBillingYear %>','<%:Model.SamplingFormC.ProvisionalBillingMonth %>', '<%:Url.Action("GetLinkedCouponDetails", "FormC") %>');
    });
  </script>
  <script type="text/javascript">
    // Set billing type from Viewdata
    billingType = '<%: ViewData[ViewDataConstants.BillingType] %>';
  </script>
  <script src="<%:Url.Content("~/Scripts/Pax/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
  
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Pax/FormCCoupon.js")%>"></script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Create Sampling Form C Coupon</h1>
  <div>
    <% Html.RenderPartial("ReadOnlyHeaderControl", Model.SamplingFormC); %>
  </div>
  <div class="clear">
  </div>
  <h2>
    Form C Coupon Details</h2>
  <div>
    <% using (Html.BeginForm("CouponCreate", "FormC", FormMethod.Post, new { id = "formCCoupon", @class = "validCharacters" }))
       { %>
       <%: Html.AntiForgeryToken() %>
    <% Html.RenderPartial("CouponDetailsControl", Model); %>
  </div>
  <div class="buttonContainer">
    <input class="primaryButton ignoredirty" type="submit" value="Save and Add New" id = "SaveButton"/>
    <%if (ViewData[ViewDataConstants.PageMode] !=null && ViewData[ViewDataConstants.PageMode].ToString() != PageMode.View)
      {%>
        <%: Html.LinkButton("Back", Url.Action("Edit", "FormC", new { invoiceId = Model.SamplingFormC.Id }))%>
    <%
      }
      else
      {%>
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
    <%
      }%>
  </div>
  <%} %>
  <div class="hidden" id="divAttachment">
    <%
      Html.RenderPartial("FormCRecordAttachmentControl", Model);%>
  </div>
  <div class="hidden" id="linkedCoupons">
    <h2>
      Below coupons were found from provisional invoices. Please select one.</h2>
    <div>
      <table id="linkedCouponsGrid">
      </table>
    </div>
    <div class="buttonContainer">
      <input class="secondaryButton" type="button" value="OK" onclick="onLinkingDialogClose()" />
    </div>
  </div>
</asp:Content>
