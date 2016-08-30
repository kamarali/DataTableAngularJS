<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="Iata.IS.Model.Common" %>
<%@ Import Namespace="Iata.IS.Model.MiscUatp.BillingHistory" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Miscellaneous :: Billing History
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Misc/BillingHistory.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/ValidateDate.js")%>"></script>
  <script type="text/javascript">  
    
    var selectedInvoiceId = null;
    var isinvoiceSearch =  <%=(ViewData[ViewDataConstants.CorrespondenceSearch] != null && ViewData[ViewDataConstants.CorrespondenceSearch].ToString() == "Invoice").ToString().ToLower()%>;
    var loggedInMemberId = <%:SessionUtil.MemberId%>;

    $(document).ready(function () {
  BindEventForDate();
    InitialiseBillingHistory('<%:Url.Action("Correspondence", "Correspondence")%>','<%:Url.Action("ShowDetails", "MiscInvoice")%>','<%:Url.Action("MiscBHAuditTrail", "BillingHistory")%>','<%:Url.Action("CreateBillingMemo", "MiscInvoice")%>', '<%:Url.Action("IsCorrespondenceInvoiceOutSideTimeLimit", "BillingHistory")%>', '<%:Url.Action("IsCorrespondenceOutSideTimeLimit", "Correspondence")%>', '<%:Url.Action("IsCorrespondenceInvoiceExistsForCorrespondence", "Correspondence")%>');
    
    registerAutocomplete('BilledMemberCode', 'BilledMemberId', '<%:Url.Action("GetMemberList","Data",new{area = ""})%>', 0, true, null);
    
    registerAutocomplete('CorrBilledMemberText', 'CorrBilledMemberId', '<%:Url.Action("GetMemberList","Data",new{area = ""})%>', 0, true, null);
  });
  </script>
  <%
    if (ViewData[ViewDataConstants.CorrespondenceSearch] != null && ViewData[ViewDataConstants.CorrespondenceSearch].ToString() == "Invoice")
    {%>
  <%:ScriptHelper.GenerateMiscInvoiceBillingHistoryGridScript(Url, ControlIdConstants.BHSearchResultsGrid, Url.Action("Correspondence", "Correspondence"), Url.Action("MiscBHAuditTrail", "BillingHistory"), Url.Action("ShowDetails", "MiscInvoice"), Url.Action("ShowDetails", "MiscCreditNote"))%>
  <%
    }
    else
    {%>
  <%:ScriptHelper.GenerateMiscCorrespondenceBillingHistoryGridScript(Url, ControlIdConstants.BHSearchResultsGrid, Url.Action("CreateBillingMemo", "MiscInvoice"), Url.Action("OpenCorrespondenceForEdit", "Correspondence"), Url.Action("MiscBHAuditTrail", "BillingHistory"), SessionUtil.MemberId)%>
  <%
    }%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Billing History</h1>
  <%
    using (Html.BeginForm("Index",
                          "BillingHistory",
                          new
                            {
                              searchType = "Invoice",
                              billingCategoryId = "Misc"
                            },
                          FormMethod.Post,
                          new
                            {
                              id = "invoiceSearchCriteria"
                            }))
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
    using (Html.BeginForm("Index",
                          "BillingHistory",
                          new
                            {
                              searchType = "Correspondence",
                              billingCategoryId = "Misc"
                            },
                          FormMethod.Post,
                          new
                            {
                              id = "corrSearchCriteria"
                            }))
    {%>
  <div>
    <%
      Html.RenderPartial("CorrSearchCriteria", ViewData[ViewDataConstants.correspondenceSearchCriteria] as CorrespondenceSearchCriteria);%>
  </div>
  <div class="buttonContainer">
    <input type="submit" value="Search" class="primaryButton" id="SearchCorrespondence" />
    <input class="secondaryButton" type="button" onclick="resetForm('#corrSearchCriteria');" value="Clear" />
  </div>
  <%
    }
  %>
  <div>
    <h2>
      Search Result</h2>
    <%
      Html.RenderPartial("GridControl", ViewData[ViewDataConstants.BHSearchResultsGrid]);%>
  </div>
</asp:Content>
