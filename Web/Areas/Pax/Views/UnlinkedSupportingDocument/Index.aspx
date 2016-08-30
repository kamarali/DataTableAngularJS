﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

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
    InitialiseGetSelectedUnlinkedSupportingDocumentDetailsMethod('<%:Url.Action("GetSelectedUnlinkedSupportingDocumentDetails", "UnlinkedSupportingDocument", "Pax")%>', '<%:Url.Action("SearchResultMismatchGridData", "UnlinkedSupportingDocument", "Pax")%>', '<%:Url.Action("LinkDocuments", "UnlinkedSupportingDocument", "Pax")%>', '<%:SessionUtil.MemberId%>', '<%:(int)Iata.IS.Model.Enums.BillingCategoryType.Pax%>');
    /*CMP #596: Length of Member Accounting Code to be Increased to 12 
    Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
    Ref: FRS Section 3.4 Table 15 Row 10 */
    registerAutocomplete('BilledMemberText', 'BilledMemberId', '<%:Url.Action("GetMemberListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);
    registerAutocomplete('SupportingDocumentBilledMemberDetailView', 'SupportingDocumentBilledMemeberIdDetailView', '<%:Url.Action("GetMemberList", "Data", new { area = "" })%>', 0, true, null);
    registerAutocomplete('MismatchTransactionBilledMember', 'HiddenBilledMemberId', '<%:Url.Action("GetMemberList", "Data", new { area = "" })%>', 0, true, null);
    </script>

</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	SIS :: Pax :: Correct Supporting Document Linking Errors
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h1>Correct Supporting Document Linking Errors</h1>
    <h2>Query Criteria</h2>
    <div>
    <%using (Html.BeginForm("Index", "UnlinkedSupportingDocument", FormMethod.Post, new { id = "UnlinkedSupportingDocumentSearchForm" }))
      {%>
          <%: Html.AntiForgeryToken() %>
          <% Html.RenderPartial("CorrectDocumentsSearchControl"); %>
     <% } 
        %>
    </div>
    <h2>
    Search Results</h2>
  <div>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.supportingDocumentSearchGrid]); %>
  </div>
  
  
   <div class="hidden"  id="divUnlinkedSupportingDocumentDetails">
    <% Html.RenderPartial("UnlinkedSupportingDocumentDetailsControl");%>
  </div>

  <div class="hidden"  id="divMismatchTransaction">
 <%Html.RenderPartial("MismatchTransactionControl"); %>
  </div>
</asp:Content>


