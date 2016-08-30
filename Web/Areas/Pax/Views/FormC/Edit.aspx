<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.Sampling.SamplingFormC>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger ::
  <%: ViewData[ViewDataConstants.BillingType].ToString() %>
  :: Edit Sampling Form C
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Script" runat="server">
  <% if (ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
     {%>
  <%: ScriptHelper.GenerateGridViewScriptForBreakdown(Url, ControlIdConstants.FormCCouponGridId, Url.Action("CouponView", new
    {
      provisionalBillingMonth = Model.ProvisionalBillingMonth,
      provisionalBillingYear = Model.ProvisionalBillingYear,
      provisionalBillingMemberId = Model.ProvisionalBillingMemberId,
      fromMemberId = Model.FromMemberId,
      listingCurrencyId = Model.ListingCurrencyId,
      invoiceStatusId = Model.InvoiceStatusId
    }))%>
  <%}
     else
     {  %>
  <%: ScriptHelper.GenerateGridEditDeleteScript(Url, ControlIdConstants.FormCCouponGridId, Url.Action("CouponEdit", "FormC", new { invoiceId = Model.Id }), Url.Action("CouponDelete", "FormC", new { invoiceId = Model.Id }))%>
  <%} %>
  <script type="text/javascript">
    $(document).ready(function () {

     var isViewMode = <%:ViewData[ViewDataConstants.PageMode] !=null ?(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View).ToString().ToLower() : "false"%>;
		  $isOnView = isViewMode;
          /*CMP #596: Length of Member Accounting Code to be Increased to 12 
          Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
          Ref: FRS Section 3.4 Table 15 Row 5 */
      registerAutocomplete('ProvisionalBillingMemberText', 'ProvisionalBillingMemberId', '<%:Url.Action("GetMemberListForPaxCgo", "Data", new { area = "" })%>', 0, true, GetFormABListingCurrency);
      <%if(Model.NumberOfRecords > 0 || ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
        {%>
          // make header readonly
          SetPageToViewModeEx(true, "#samplingformC");
      <%}%>
      InitializeFormC('<%:Url.Action("GetFormABListingCurrency", "FormC") %>', '<%:SessionUtil.MemberId %>');
    });
  </script>
  <script type="text/javascript" src="<%: Url.Content("~/Scripts/Pax/FormC.js") %>">
  </script>
  <script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Edit Sampling Form C</h1>
  <% using (Html.BeginForm("Edit", "FormC", FormMethod.Post, new { id = "samplingformC", @class = "validCharacters" }))
     {%>
     <%: Html.AntiForgeryToken() %>
  <div>
    <% Html.RenderPartial("HeaderControl"); %>
  </div>
  <div class="buttonContainer">
    <input class="primaryButton ignoredirty" type="submit" value="Save Form C Header" id="btnSaveHeader" />
  </div>
  <%} %>
  <div class="clear">
  </div>
  <h2>
    Summary List</h2>
  <div>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.FormCSummaryListGrid]); %>
  </div>
  <div class="clear">
  </div>
  <h2>
    Form C Coupon List</h2>
  <div>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.FormCCouponListGrid]); %>
  </div>
  <div class="buttonContainer">
    <%
      using (Html.BeginForm("", "FormC", new { transactionId = Model.Id }, FormMethod.Post))
      { %>
      <%: Html.AntiForgeryToken() %>
    <!-- If Form C Status is open or Error, then user is allowed to Validate  -->
    <% if ((Model.InvoiceStatusId == Convert.ToInt32(InvoiceStatusType.Open) || Model.InvoiceStatusId == Convert.ToInt32(InvoiceStatusType.ValidationError)) && Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Pax.Receivables.SampleFormC.Validate))
       { %>
    <input class="primaryButton" type="submit" value="Validate Form C" onclick="return changeAction('<%: Url.Action("Validate","FormC", new { transactionId = Model.Id }) %>')" />
    <%}
       // If Form C Status is Ready for Submission, then user is allowed to Submit 
       else if (Model.InvoiceStatusId == Convert.ToInt32(InvoiceStatusType.ReadyForSubmission) && Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Pax.Receivables.SampleFormC.Submit))
       { %>
    <input class="primaryButton" type="submit" value="Submit Form C" onclick="return changeAction('<%: Url.Action("Submit","FormC", new { transactionId = Model.Id }) %>')" />
    <%}%>
    <% if (Model.NilFormCIndicator == "N" && ViewData[ViewDataConstants.PageMode].ToString() != PageMode.View)
       { %>
    <input class="secondaryButton" type="button" value="Add Form C Item" onclick="javascript:location.href='<%: Url.Action("CouponCreate", "FormC", new {
                                                      invoiceId = Model.Id.Value()
                                                      })%>';" />
    <%} %>
    <%
        if (!string.IsNullOrEmpty(SessionUtil.FormCSearchCriteria))
        { 
    %>
    <%: Html.LinkButton("Back to Form C Search", Url.Action("Index", "FormC"))%>
    <%
      }
    %>
    <% }%>
  </div>
  <%if (Model.InvoiceStatus == InvoiceStatusType.ValidationError)
    { 
  %>
  <h2>
    Validation Errors</h2>
  <div class="horizontalFlow">
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.SubmittedErrorsGrid]); %>
  </div>
  <%}%>
  <div class="clear">
  </div>
</asp:Content>
