<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.AutoBillingSearchCriteria>" %>
<%@ Import Namespace="System.Security.Policy" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Passenger :: Receivables :: Correct AutoBilling Invoices
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>AutoBilling Prime Coupon Search</h1>
  <h2>
    Search Criteria</h2>
  <div>
    <%
      using (Html.BeginForm("Index", "AutoBilling", new { postBack = "true" }, FormMethod.Post, new { id = "AutoBillingInvoiceSearchForm", postBack = "true" }))
      {
        Html.RenderPartial("AutoBillingInvoiceSearchControl", Model);
      } 
    %>
  </div>
  <h2>Search Results</h2>
    <div>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.SearchGrid]); %>
  </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">
      /*CMP #596: Length of Member Accounting Code to be Increased to 12 
      Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
      Ref: FRS Section 3.4 Table 15 Row 13 */
      registerAutocomplete('BilledMemberText', 'BilledMemberId', '<%:Url.Action("GetMemberListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);

    $(document).ready(function () {
      if ('<%: Convert.ToBoolean(ViewData[ViewDataConstants.IsPostback])%>' == 'False') {
        if ($("#SourceCode").val() == 0)
          $("#SourceCode").val('')
        if ($("#TicketDocNumber").val() == 0)
          $("#TicketDocNumber").val('')
        if ($("#CouponNumber").val() == 0)
          $("#CouponNumber").val('');
      }

      // Code to validate Billed member
      $("#AutoBillingInvoiceSearchForm").validate({
        rules: {
          BilledMemberText: "required"
        },
        messages: {
          BilledMemberText: "Billed Member Required"
        }
      });

    });

    function resetForm() {
      $(':input', '#AutoBillingInvoiceSearchForm')
        .not(':button, :submit, :reset, :hidden')
        .val('')
        .removeAttr('selected');
      $("#BilledMemberText").val("");
      $("#DailyRevenueRecognitionFileDate").val("");
      $("#CouponNumber").val("");
      $("#TicketDocNumber").val("");
      $("#TicketIssuingAirline").val("");
      $("#ProrateMethodology").val("");
      $("#SourceCode").val("");
      $("#InvoiceNumber").val("");
    }

  </script>
  <script src="<%:Url.Content("~/Scripts/Pax/AutoBillingDeleteRecord.js")%>" type="text/javascript"></script>

  <%:ScriptHelper.GenerateGridEditDeleteScript(Url, ViewDataConstants.SearchGrid, Url.Action("PrimeBillingEdit"), Url.Action("PrimeBillingDelete"))%>
</asp:Content>
