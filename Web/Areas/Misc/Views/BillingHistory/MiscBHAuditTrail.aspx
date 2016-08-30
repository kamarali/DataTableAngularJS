<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Web.UIModel.BillingHistory.Misc.AuditTrail>" %>

<%@ Import Namespace="Iata.IS.Model.Enums" %>
<%@ Import Namespace="Iata.IS.Model.MiscUatp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Miscellaneous :: Billing History Audit Trail
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Billing History Audit Trail</h1>
  <div class="buttonContainer">
  </div>
  <%
			if (Model.CorrespondenceInvoice != null)
			{
  %>
  <h2>
    Stage
    <%
					 if (Model.RejectionInvoiceList.Count == 0)
					 {
    %>
    <%:Model.InvoiceStageCount - 1%>
    <%
					 }
					 else
					 {
    %>
    <%:Model.InvoiceStageCount + Model.RejectionInvoiceList[Model.RejectionInvoiceList.Count - 1].Correspondences.Count - 1%>
    <%
					 }
    %>
    Invoice Due To Correspondence</h2>
  <% Html.RenderPartial("BillingHistoryCorrInvoice", Model.CorrespondenceInvoice); %>
  <div class="stageSeparator">
  </div>
  <% }
  %>
  <%
    var rejectionInv = new MiscUatpInvoice();
		if (Model.RejectionInvoiceList.Count > 0)
		{
      if (Model.CorrespondenceInvoice != null)
      {
        rejectionInv = Model.RejectionInvoiceList.Where(inv => inv.InvoiceNumber == Model.CorrespondenceInvoice.RejectedInvoiceNumber).OrderByDescending(invoice => invoice.RejectionStage).ToList()[0];  
      }
      else
      {
        rejectionInv = Model.RejectionInvoiceList.OrderByDescending(invoice => invoice.RejectionStage).ToList()[0];
      }

			foreach (var correspondence in rejectionInv.Correspondences.OrderByDescending(corr => corr.CorrespondenceStage))
			{
        //if ((correspondence.CorrespondenceSubStatus != CorrespondenceSubStatus.Saved && correspondence.CorrespondenceSubStatus != CorrespondenceSubStatus.ReadyForSubmit && correspondence.CorrespondenceSubStatus != CorrespondenceSubStatus.DueToExpiry) || ((correspondence.CorrespondenceSubStatus == CorrespondenceSubStatus.Saved || correspondence.CorrespondenceSubStatus == CorrespondenceSubStatus.ReadyForSubmit || correspondence.CorrespondenceSubStatus == CorrespondenceSubStatus.DueToExpiry) && correspondence.FromMemberId == SessionUtil.MemberId))
        // SCP92456 - Two AZ prime MISC billings are being linked in the audit trail
        if ((correspondence.CorrespondenceSubStatus != CorrespondenceSubStatus.Saved && correspondence.CorrespondenceSubStatus != CorrespondenceSubStatus.ReadyForSubmit && correspondence.CorrespondenceSubStatus != CorrespondenceSubStatus.DueToExpiry) || ((correspondence.CorrespondenceSubStatus == CorrespondenceSubStatus.Saved || correspondence.CorrespondenceSubStatus == CorrespondenceSubStatus.ReadyForSubmit || correspondence.CorrespondenceSubStatus == CorrespondenceSubStatus.DueToExpiry)))
				{
%><h2>
Stage
<%:correspondence.CorrespondenceStage + Model.RejectionInvoiceList.Count%>
Correspondence
<%:correspondence.CorrespondenceStage%></h2>
<div>
<%
					Html.RenderPartial("BillingHistoryCorrespondence", correspondence);%>
</div>
<div class="stageSeparator">
</div>
<%
				}
			}
		}
  %>
  <%
			 if(Model.RejectionInvoiceList.Count>0)
			 foreach (var rejectionInvoice in Model.RejectionInvoiceList.OrderByDescending(invoice => invoice.RejectionStage).ToList())
{
  if (rejectionInvoice.RejectionStage == 1)
  {
    var itInvoice = rejectionInvoice;
    foreach (LineItem item in itInvoice.LineItems)
    {
      if (Model.OriginalInvoice != null && Model.OriginalInvoice.LineItems.Find(originalItem => originalItem.LineItemNumber == item.LineItemNumber) != null) item.ChargeAmount = Model.OriginalInvoice.LineItems.Find(originalItem => originalItem.LineItemNumber == item.LineItemNumber).TotalNetAmount;
      else item.ChargeAmount = 0.00M;
    }
  }
  else
  {
    var itInvoice = rejectionInvoice;
    foreach (LineItem item in itInvoice.LineItems)
    {
      bool isStage1InvoiceFound = false;
      foreach (var rejInvoice in Model.RejectionInvoiceList)
      {
        // Charge amount property, here, is used to display the Original Line Item Net amount.
        if (rejInvoice.RejectionStage == 1 && rejInvoice.LineItems.Find(originalItem => originalItem.LineItemNumber == item.OriginalLineItemNumber) != null){ item.ChargeAmount = rejInvoice.LineItems.Find(itemO => itemO.LineItemNumber == item.OriginalLineItemNumber).TotalNetAmount;
          isStage1InvoiceFound = true;
        }
      }

      if (isStage1InvoiceFound == false)
      {
        if (rejectionInvoice.LineItems.Find(originalItem => originalItem.LineItemNumber == item.LineItemNumber) != null)
        {
          item.ChargeAmount = 0.00M; // in case, the stage 1 rejection invoice does not exist.
        }
      }
    }
  }
  %>
  <h2>
    Stage
    <%: rejectionInvoice.RejectionStage%>
    Rejection Invoice
    <%:rejectionInvoice.RejectionStage%></h2>
  <div>
    <% Html.RenderPartial("BillingHistoryRejectionInvoice", rejectionInvoice); %>
  </div>
  <div class="stageSeparator">
  </div>
  <%
}
  %>
  <%
		if (Model.OriginalInvoice != null)
		{
  %>
  <h2>
    Stage 0, Original Invoice</h2>
  <div>
    <% Html.RenderPartial("BillingHistoryInvoice", Model.OriginalInvoice); %>
  </div>
  <% }%>
  <div class="buttonContainer">
    <input type="button" class="secondaryButton" value="Back" onclick="history.go(-1);" />
    <%--CMP508:Audit Trail Download with Supporting Documents--%>
    <%--<input type="button" class="secondaryButton" value="Generate PDF" onclick="javascript:location.href ='<%: Url.Action("GenerateBillingHistoryAuditTrailPdfForMisc", "BillingHistory", new {invoiceId = ViewData["invoiceId"].ToString(), areaName = "Miscellaneous"}) %>'" />--%>
    <input type="button" class="secondaryButton" value="Generate PDF" onclick="DownloadFile('<%: Url.Action("GenerateBillingHistoryAuditTrailPdfForMisc", "BillingHistory", new {invoiceId = ViewData["invoiceId"].ToString(), areaName = "Miscellaneous"}) %>','<%: Url.Action("EnqueBillingHistoryAuditTrailDownload", "BillingHistory", new {invoiceId = ViewData["invoiceId"].ToString(), areaName = "Miscellaneous", billingCategory = BillingCategoryType.Misc}) %>')" />
    <input type="checkbox" id = "IncludeSuppDocs" name="Include Supporting Document(s)" value="IncludeSuppDocs" /> Include Supporting Document(s)
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Misc/BillingHistory.js")%>"></script>
  <style type="text/css">
    .stageSeparator {
      border-top: 5px solid #888;
      margin: 5px 0px;
    }
    .numeric {
      text-align: right;
    }
    table.formattedTable {
      background-color: #edeeee;
      border: 1px solid #666;
      padding: 0px 0px 0px 0px;
      margin: 5px 0px 10px 0px;
      border-collapse: collapse;
    }
    table.formattedTable > thead > tr {
      background-color: #d7e9f8;
      color: #000;
      font-weight: bold;
    }
    table.formattedTable > thead > tr > td {
      vertical-align: top;
      text-align: center;
      border: 1px solid #666;
      padding: 8px 3px 8px 3px;
    }
    table.formattedTable > tbody > tr {
      background-color: #fff;
    }
    table.formattedTable > tbody > tr > td {
      padding: 8px 3px 8px 3px;
      font: normal 8pt Arial, Helvetica, sans-serif;
      color: #000000;
      border: 1px solid #666;
    }
  </style>
</asp:Content>
