<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <%: ScriptHelper.GenerateUnlinkedSupportingDocumentGridEditDeleteScript(Url, ControlIdConstants.SupportingDocumentSearchGrid, Url.Action("GetSelectedUnlinkedSupportingDocumentDetails", "UnlinkedSupportingDocument"), Url.Action("DeleteUnLinkDocuments", "UnlinkedSupportingDocument"))%>
  <script type="text/javascript" src="<%: Url.Content("~/Scripts/UnlinkedSupportingDocument.js") %>"></script>
  <script type="text/javascript" src="<%: Url.Content("~/Scripts/DeleteRecord.js") %>"></script>
  <script type="text/javascript">
    function resetForm() {
      $(':input', '#ISCalendarSearchForm')
        .not(':button, :submit, :reset, :hidden')
        .val('')
        .removeAttr('selected');
      $("#SupportingDocumentBillingYearMonth").val("");
      $("#PeriodNumber").val("-1");
      $("#BilledMemberText").val("");
      $("#InvoiceNumber").val("");
      $("#OriginalFileName").val("");
      $("#SubmissionDate").val("");
  }
  $('#ChargeCat').hide();
    InitialiseGetSelectedUnlinkedSupportingDocumentDetailsMethod('<%:Url.Action("GetSelectedUnlinkedSupportingDocumentDetails", "UnlinkedSupportingDocument", "Uatp")%>', '<%:Url.Action("SearchResultMismatchGridData", "UnlinkedSupportingDocument", "Uatp")%>', '<%:Url.Action("LinkDocuments", "UnlinkedSupportingDocument", "Uatp")%>', '<%:SessionUtil.MemberId%>', '<%:(int)Iata.IS.Model.Enums.BillingCategoryType.Uatp%>');
    registerAutocomplete('BilledMemberText', 'BilledMemberId', '<%:Url.Action("GetMemberList", "Data", new { area = "" })%>', 0, true, null);
    registerAutocomplete('SupportingDocumentBilledMemberDetailView', 'SupportingDocumentBilledMemeberIdDetailView', '<%:Url.Action("GetMemberList", "Data", new { area = "" })%>', 0, true, null);
    registerAutocomplete('MismatchTransactionBilledMember', 'HiddenBilledMemberId', '<%:Url.Action("GetMemberList", "Data", new { area = "" })%>', 0, true, null);
  </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Uatp :: Correct Supporting Document Linking Errors
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Correct Supporting Document Linking Errors</h1>
  <h2>
    Query Criteria</h2>
  <div>
    <%using (Html.BeginForm("Index", "UnlinkedSupportingDocument", FormMethod.Post, new { id = "UnlinkedSupportingDocumentSearchForm" }))
      {%>
      <%: Html.AntiForgeryToken() %>
      <%
        Html.RenderPartial("CorrectDocumentsSearchControl");
      } 
    %>
  </div>
  <h2>
    Search Results</h2>
  <div>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.supportingDocumentSearchGrid]); %>
  </div>
  <div class="hidden" id="divUnlinkedSupportingDocumentDetails">
    <% Html.RenderPartial("UnlinkedSupportingDocumentDetailsControl");%>
  </div>
  <div class="hidden" id="divMismatchTransaction">
    <%Html.RenderPartial("MismatchTransactionControl"); %>
  </div>
</asp:Content>
