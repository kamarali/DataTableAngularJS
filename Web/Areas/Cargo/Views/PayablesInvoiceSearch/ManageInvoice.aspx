<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Cargo.PayableSearch>" %>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script src="<%:Url.Content("~/Scripts/InvoiceSearch.js")%>" type="text/javascript"></script>
    <script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
    <script src="<%:Url.Content("~/Scripts/validateRecord.js")%>" type="text/javascript"></script>
    <script src="<%:Url.Content("~/Scripts/downloadZip.js")%>" type="text/javascript"></script>
    <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
    <script type="text/javascript">

    $(document).ready(function () {
        $("#frmInvoiceSearch").validate({
              rules: {
                  BillingYearMonth: "required",
                  BillingPeriod: "required"
              },
              messages: {
                  BillingYearMonth: "Billing Year / Month required",
                  BillingPeriod: "Billing Period required"
              }
          });
      });
        registerAutocomplete('BillingMemberText', 'BillingMemberId', '<%:Url.Action("GetMemberList", "Data", new { area = "" })%>', 0, true, null);
        
        //Reset function
        function resetForm() {
           // debugger;
            $(':input', '#frmInvoiceSearch')
        .not(':button, :submit, :reset, :hidden')
        .val('')
        .removeAttr('selected');
            $("#InvoiceStatus").val("-1");
            $("#SMI").val("-1");
            $("#BillingPeriod").val("-1");
            $("#OwnerId").val("-1");
            $("#InvoiceNoteNumber").val("-1");
            $("#BillingCode").val("-1");
        }
    </script>
    <%:ScriptHelper.GenerateMiscGridEditViewValidateDeleteScript(Url, ControlIdConstants.PayableInvoiceSearchGrid,
                 Url.Action("EditInvoice", "PayablesInvoiceSearch"),
                        Url.RouteUrl("CGOInvoiceSearch", new { controller = "PayablesInvoiceSearch", action = "ValidateInvoice" }),
                        Url.RouteUrl("CGOInvoiceSearch", new { controller = "PayablesInvoiceSearch", action = "DeleteInvoice" }),
                        Url.RouteUrl("CGOInvoiceSearch", new { controller = "PayablesInvoiceSearch", action = "ViewInvoice" }),
               Url.Action("DownloadPdf", "PayablesInvoiceSearch", new { area = "Cargo" }),
               Url.Action("DownloadZip", "PayablesInvoiceSearch", new { area = "Cargo" }),
        (int)ViewData[ViewDataConstants.RejectionOnValidationFlag])%>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Cargo :: Payables :: Invoice Search
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        Invoice Search</h1>
    <h2>
        Search Criteria</h2>
    <div>
        <% using (Html.BeginForm("ManageInvoice", "PayablesInvoiceSearch", FormMethod.Get, new { id = "frmInvoiceSearch" }))
           { %>
        <% Html.RenderPartial("PayableManageInvoiceSearch", Model); %>
        <% } %>
    </div>
    <h2>
        Search Results</h2>
    <div>
        <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.PayableInvoiceSearchGrid]); %>
    </div>
    <div class="buttonContainer">
        <input class="primaryButton" type="button" value="Submit Selected Invoices" onclick="submitInvoices('#<%:ControlIdConstants.SearchGrid %>','<%:Url.Action("SubmitInvoices","SearchResultGridData") %>');" />
        <input class="tempButton hidden" type="button" value="Present Selected Invoices (testing only)"
            onclick="submitInvoices('#<%:ControlIdConstants.SearchGrid %>','<%:Url.Action("PresentInvoices","SearchResultGridData") %>');" />
        <input class="tempButton hidden" type="button" value="Mark Invoices To Processing Complete (testing only)"
            onclick="submitInvoices('#<%:ControlIdConstants.SearchGrid %>','<%:Url.Action("MarkInvoicesToProcessingComplete","SearchResultGridData") %>');" />
    </div>
</asp:Content>
