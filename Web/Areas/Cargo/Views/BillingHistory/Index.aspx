<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Iata.IS.Business.Security.Permissions.Cargo.BillingHistoryAndCorrespondence" %>
<%@ Import Namespace="Iata.IS.Model.Common" %>
<%@ Import Namespace="Iata.IS.Model.Cargo.BillingHistory" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Cargo :: Billing History
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Cargo/BillingHistory.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/ValidateDate.js")%>"></script>
  <script type="text/javascript"> 
    var selectedInvoiceId = null;
    var isinvoiceSearch =  <%=(ViewData[ViewDataConstants.CorrespondenceSearch] != null && ViewData[ViewDataConstants.CorrespondenceSearch].ToString() == "Invoice").ToString().ToLower()%>;
    var loggedInMemberId = <%:SessionUtil.MemberId%>;
    
    $(document).ready(function () {
    BindEventForDate();
    InitialiseBillingHistory('<%:Url.Action("CargoCreateCorrespondence", "Correspondence")%>','<%:Url.Action("InitiateRejection", "BillingHistory")%>','<%:Url.Action("InitiateCorrespondence", "Correspondence")%>','<%:Url.Action("CargoBillingHistoryAuditTrail", "BillingHistory")%>','<%:Url.Action("ClearSearch", "BillingHistory")%>', '<%:Url.Action("InitiateDuplicateRejection", "BillingHistory")%>', '<%:Url.Action("IsBillingMemoExistsForCorrespondence", "BillingHistory")%>',  '<%:Url.Action("IsCorrespondenceOutSideTimeLimit", "Correspondence")%>',  '<%:Url.Action("IsBillingMemoInvoiceOutSideTimeLimit", "BillingHistory")%>');
    // Integer class does not work in Firefox, so instead added below line which only accepts integers and works in both IE and Firefox.
    $("#AwbSerialNumber").numeric();
    registerReasonCodeAutocomplete('ReasonCodeId', 'ReasonCodeId', '<%:Url.Action("GetReasonCodeListForCargoBillingHistory", "Data", new { area="" })%>', 0, false, '', '', '#RejectionStageId', '#MemoTypeId', '');        
    /*CMP #596: Length of Member Accounting Code to be Increased to 12 
      Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
      Ref: FRS Section 3.4 Table 15 Row 26 */
    registerAutocomplete('BilledMemberCode', 'BilledMemberId', '<%:Url.Action("GetMemberListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);
    /* CMP #596: Length of Member Accounting Code to be Increased to 12 
    Desc: Issuing Airlines auto-complete should exclude pure numeric values having size 5 or greater
    Ref: FRS Section 3.4 Table 17 Row 10 */
    registerAutocomplete('IssuingAirline', 'IssuingAirlineId', '<%:Url.Action("GetTicketIssuingAirlineListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);
    /*CMP #596: Length of Member Accounting Code to be Increased to 12 
      Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
      Ref: FRS Section 3.4 Table 15 Row 27 */
    registerAutocomplete('CorrBilledMemberText', 'CorrBilledMemberId', '<%:Url.Action("GetMemberListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);        
     SetTransaction();

     // If View data value is not null and is set to true, clear AwbSerialNumber field value 
     clearField = '<%: ViewData["ClearIntegerFields"] != null ? ViewData["ClearIntegerFields"].ToString() : null %>';
     if(clearField != null && clearField == "True")
     {
         $("#AwbSerialNumber").val('');
     }
     else
     {
         $("#AwbSerialNumber").val('<%:ViewData["AwbSerialNumber"].ToString()%>');
             if($("#AwbSerialNumber").val()==0)
             {
                $("#AwbSerialNumber").val('');
             }
     }
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
  <%--SCP312528 - IS-Web Performance (Controller: BillingHistory - Log Action: CargoBillingHistoryAuditTrail)--%>
   <%: ScriptHelper.GenerateGridCargoAuditTrailForLinkedCorrRM(Url, ControlIdConstants.LinkedRejectionMemoGridId,
                          Url.RouteUrl("CargoBillingHistoryAuditTrail", new { controller = "BillingHistory", action = "CargoBillingHistoryAuditTrail", area = "Cargo", transactionId = "0", transactionType = "1" }))%>
  <%
    if (ViewData[ViewDataConstants.CorrespondenceSearch] != null && ViewData[ViewDataConstants.CorrespondenceSearch].ToString() == "Invoice")
    {
%>
  <%--SCP312528 - IS-Web Performance (Controller: BillingHistory - Log Action: CargoBillingHistoryAuditTrail)--%>
  <%:ScriptHelper.GenerateCargoInvoiceBillingHistoryGridScript(Url, ControlIdConstants.BHSearchResultsGrid, 
                                                             Url.Action("View", "Invoice"),
                                                                         Url.RouteUrl("CargoBillingHistoryAuditTrail", new { controller = "BillingHistory", action = "CargoBillingHistoryAuditTrail", area = "Cargo", transactionId = "0", transactionType = "1" }))%>

  <%
    }
    else
    {
%>
  <%:ScriptHelper.GenerateCargoCorrespondenceBillingHistoryGridScript(Url, ControlIdConstants.BHSearchResultsGrid,
                                                                          Url.Action("View", "Invoice"), Url.RouteUrl("CargoBillingHistoryAuditTrail", new { controller = "BillingHistory", action = "CargoBillingHistoryAuditTrail", area = "Cargo", transactionId = "0", transactionType = "1" }))%>
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
      <% Html.RenderPartial("CorrSearchCriteria", ViewData[ViewDataConstants.correspondenceSearchCriteria] as CorrespondenceSearchCriteria);%>
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
    <% Html.RenderPartial("BillingHistoryInvoice");%>
  </div>

  <div id = "divDuplicateRejections" class ="hidden">
    <% Html.RenderPartial("DuplicateRejectionControl");%>
  </div>
  <div id = "divDuplicateBillingMemos" class ="hidden">
    <% Html.RenderPartial("DuplicateBillingMemoControl");%>
  </div>

  <%
    using (Html.BeginForm("CargoCreateCorrespondenceFor", "Correspondence", FormMethod.Post, new { id = "frmInitiateCorrespondence", area = "Cargo" }))
    {%>
    <%: Html.AntiForgeryToken() %>
    <input type="hidden" id="invoiceId" name="invoiceId" value="" />
    <input type="hidden" id="rejectionMemoIds" name="rejectionMemoIds" value="" />
    <%
    }%>

</asp:Content>
