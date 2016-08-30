<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Iata.IS.Business.Security.Permissions.Pax" %>
<%@ Import Namespace="Iata.IS.Model.Common" %>
<%@ Import Namespace="Iata.IS.Model.Pax.BillingHistory" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger :: Billing History
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Pax/BillingHistory.js")%>"></script>
  <script type="text/javascript"> 
    var selectedInvoiceId = null;
    var isinvoiceSearch =  <%=(ViewData[ViewDataConstants.CorrespondenceSearch] != null && ViewData[ViewDataConstants.CorrespondenceSearch].ToString() == "Invoice").ToString().ToLower()%>;
    var loggedInMemberId = <%:SessionUtil.MemberId%>;
    
    $(document).ready(function () {
    InitialiseBillingHistory('<%:Url.Action("PaxCreateCorrespondence", "Correspondence")%>','<%:Url.Action("InitiateRejection", "BillingHistory")%>','<%:Url.Action("InitiateCorrespondence", "Correspondence")%>','<%:Url.Action("PaxBillingHistoryAuditTrail", "BillingHistory")%>','<%:Url.Action("ClearSearch", "BillingHistory")%>', '<%:Url.Action("InitiateDuplicateRejection", "BillingHistory")%>', '<%:Url.Action("IsBillingMemoExistsForCorrespondence", "BillingHistory")%>',  '<%:Url.Action("IsCorrespondenceOutSideTimeLimit", "Correspondence")%>',  '<%:Url.Action("IsBillingMemoInvoiceOutSideTimeLimit", "BillingHistory")%>');
    //This is of invoice search
    registerAutocomplete('SourceCodeId', 'SourceCodeId', '<%:Url.Action("GetEntireSourceCodeList", "Data", new { area = "" })%>', 0, true,null);      
    registerReasonCodeAutocomplete('ReasonCodeId', 'ReasonCodeId', '<%:Url.Action("GetPaxReasonCodeListForBillingHistory", "Data", new { area = "" })%>', 0, false, '', '#RejectionStageId','#BillingCode', '#MemoTypeId', '');        
    /*CMP #596: Length of Member Accounting Code to be Increased to 12 
      Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
      Ref: FRS Section 3.4 Table 15 Row 14 */
    registerAutocomplete('BilledMemberCode', 'BilledMemberId', '<%:Url.Action("GetMemberListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);
    /* CMP #596: Length of Member Accounting Code to be Increased to 12 
    Desc: Issuing Airlines auto-complete should exclude pure numeric values having size 5 or greater
    Ref: FRS Section 3.4 Table 17 Row 1 */
    registerAutocomplete('IssuingAirline', 'IssuingAirlineId', '<%:Url.Action("GetTicketIssuingAirlineListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);
    /*CMP #596: Length of Member Accounting Code to be Increased to 12 
      Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
      Ref: FRS Section 3.4 Table 15 Row 15 */
    registerAutocomplete('CorrBilledMemberText', 'CorrBilledMemberId', '<%:Url.Action("GetMemberListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);        
    //CMP526 - Passenger Correspondence Identifiable by Source Code - Correspondence Search
    registerAutocomplete('SourceCode', 'SourceCode', '<%:Url.Action("GetSourceCodeListForCorrespondence", "Data", new { area = "" })%>', 0, true,null,'<%:Convert.ToInt32(Iata.IS.Model.Enums.BillingCategoryType.Pax)%>');
    
    SetTransaction();

    });           

    function ResetCorrespondence()
    {
      resetForm('#corrSearchCriteria');
      $("#CorrespondenceStatusId").val('1');
      $("#CorrespondenceSubStatusId").val('1');
      //TFS#10003:Firefox: v45: "Clear" button removing value from "Correspondence Initiating Member" for PAX. 
      $("#CorrespondenceOwnerId").val($("#CorrespondenceOwnerId option:first").val());
      $("#InitiatingMember").val($("#InitiatingMember option:first").val());  
      $("#ToDate").datepicker('setDate', new Date());
      $("#FromDate").datepicker('setDate', new Date());

      SetSubStatus();
    }

  </script>
    <%--CMP #612: Changes to PAX CGO Correspondence Audit Trail Download--%>
    <%: ScriptHelper.GenerateGridPaxAuditTrailForLinkedCorrRM(Url, ControlIdConstants.LinkedRejectionMemoGridId,
                 Url.RouteUrl("PaxBillingHistoryAuditTrail", new { controller = "BillingHistory", action = "PaxBillingHistoryAuditTrail", area = "Pax", transactionId = "0", transactionType = "1" }))%>

  <%
    if (ViewData[ViewDataConstants.CorrespondenceSearch] != null && ViewData[ViewDataConstants.CorrespondenceSearch].ToString() == "Invoice")
    {
%>
  <%:ScriptHelper.GeneratePaxInvoiceBillingHistoryGridScript(Url, ControlIdConstants.BHSearchResultsGrid, 
                                                             Url.Action("View", "Invoice"),
                                                                   Url.RouteUrl("PaxBillingHistoryAuditTrail", new { controller = "BillingHistory", action = "PaxBillingHistoryAuditTrail", area = "Pax", transactionId = "0", transactionType = "1" }),
                                                             Url.RouteUrl("ViewFormCCoupon", new { controller = "BillingHistory", action = "ViewFormCCoupon", area = "Pax" }))%>

  <%
    }
    else
    {
%>
  <%:ScriptHelper.GeneratePaxCorrespondenceBillingHistoryGridScript(Url, ControlIdConstants.BHSearchResultsGrid,
                                                                          Url.Action("View", "Invoice"), Url.RouteUrl("PaxBillingHistoryAuditTrail", new { controller = "BillingHistory", action = "PaxBillingHistoryAuditTrail", area = "Pax", transactionId = "0", transactionType = "1" }))%>
  <%
    }
%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Billing History</h1>
  <%
    using (Html.BeginForm("Index", "BillingHistory", new { searchType = "Invoice" }, FormMethod.Post, new { id = "invoiceSearchCriteria" }))
    {%>
  <div>
    <%
      Html.RenderPartial("InvoiceSearchCriteria", ViewData[ViewDataConstants.invoiceSearchCriteria] as InvoiceSearchCriteria);%>
  </div>
  <div class="buttonContainer">
    <input type="submit" value="Search" class="primaryButton" id="Search" />
    <input class="secondaryButton" type="button" onclick="resetForm('#invoiceSearchCriteria');" value="Clear" />
  </div>
  <%
    }%>
  <%
    using (Html.BeginForm("Index", "BillingHistory", new { searchType = "Correspondence" }, FormMethod.Post, new { id = "corrSearchCriteria" }))
    {%>
  <div>
    <%
      Html.RenderPartial("CorrSearchCriteria", ViewData[ViewDataConstants.correspondenceSearchCriteria] as CorrespondenceSearchCriteria);%>
  </div>
  <div class="buttonContainer">
    <input type="submit" value="Search" class="primaryButton" id="SearchCorrespondence" />
    <input class="secondaryButton" type="button" onclick="ResetCorrespondence();" value="Clear" />
  </div>
  <%
    }
%>
  <div id="searchGrid">
    <h2>
      Search Results</h2>
      <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.BHSearchResultsGrid]);%>
  </div>
  <div class="buttonContainer" id="gridButtons">
    <input type="button" value="Initiate Rejection" class="primaryButton" id="InitiateRejection" onclick="InitiateRMRejection();" />
    <%
    if (Html.IsAuthorized(BillingHistoryAndCorrespondence.DraftCorrespondence))
    {
%>
    <input type="button" value="Initiate Correspondence" disabled="disabled" class="primaryButton" id="InitiateCorrespondence" onclick="InitiateCorrespondence();" />
    <%
    }
%>
    <input type="button" value="Initiate Billing Memo" class="primaryButton" id="InitiateBilling" onclick="InitiateBillingMemo();" />
  </div>
  
  <%--CMP612: Changes to PAX CGO Correspondence Audit Trail Download--%>
  <div id="showRejection">
  <%if (ViewData[ViewDataConstants.CorrespondenceSearch] != null && ViewData[ViewDataConstants.CorrespondenceSearch].ToString() == "Correspondence")
    {
   Html.RenderPartial("LinkedCorrRejectionMemo");
    }
  %>
  </div>
  <%------------------------%>


  <div id="divBillingHistoryInvoice" class="hidden">
    <%
    Html.RenderPartial("BillingHistoryInvoice");%>
  </div>

  <div id = "divDuplicateRejections" class ="hidden">
    <%
    Html.RenderPartial("DuplicateRejectionControl");%>
  </div>
  <div id = "divDuplicateBillingMemos" class ="hidden">
    <%
    Html.RenderPartial("DuplicateBillingMemoControl");%>
  </div>
  <%
    using (Html.BeginForm("PaxCreateCorrespondenceFor", "Correspondence", FormMethod.Post, new { id = "frmInitiateCorrespondence", area = "Pax" }))
    {%>
    <%: Html.AntiForgeryToken() %>
    <input type="hidden" id="invoiceId" name="invoiceId" value="" />
    <input type="hidden" id="rejectionMemoIds" name="rejectionMemoIds" value="" />
    <%
    }%>

</asp:Content>
