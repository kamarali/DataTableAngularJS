<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Cargo.BillingHistory.CargoAuditTrail>" %>

<%@ Import Namespace="Iata.IS.Model.Cargo" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Cargo :: Billing History Audit Trail
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <%
    var memoList = new List<CargoRejectionMemo>();
    foreach (var invoice in Model.Invoices.Where(invoice => invoice.CGORejectionMemo.Count > 0))
    {
      memoList.AddRange(invoice.CGORejectionMemo);
    }
    
    memoList = memoList.OrderByDescending(memo => memo.RejectionStage).ToList();

    if (memoList.Count > 0)
    {
      var displayedRMIds = string.Empty;
      foreach (var rejectionMemo in memoList)
      {

        if (rejectionMemo.RejectionStage == 3)
        {
          if (rejectionMemo.Correspondence != null)
          {
            var corr = rejectionMemo.Correspondence;
            foreach (CargoInvoice cargoInvoice in Model.Invoices.Where(cargoInvoice => cargoInvoice.CGOBillingMemo.Count > 0))
            {
              foreach (CargoBillingMemo billingMemo in cargoInvoice.CGOBillingMemo)
              {

                if (billingMemo.CorrespondenceReferenceNumber == corr.CorrespondenceNumber)
                {
%>
  <h2>
    Stage
    <%:rejectionMemo.RejectionStage + rejectionMemo.Correspondences.Count + 1%>, Billing Memo</h2>
  <%
                  Html.RenderPartial("BillingMemoControl", billingMemo); %>
                  <div class="stageSeparator"></div>
               <% }

              }

            }

            rejectionMemo.Correspondences = rejectionMemo.Correspondences.OrderByDescending(c => c.CorrespondenceStage).ToList();
            foreach (CargoCorrespondence corres in rejectionMemo.Correspondences)
            {
  %>
  <h2>
    Stage
    <%:rejectionMemo.RejectionStage + corres.CorrespondenceStage%>, Correspondence</h2>
  <%
Html.RenderPartial("CorrespondenceControl", corres); %>
                  <div class="stageSeparator"></div>
            <%}
          }

  %>
  <br /> <h2> Stage <%:rejectionMemo.RejectionStage%>, Rejection Memo </h2>
  <%
Html.RenderPartial("RejectionMemoControl", rejectionMemo); %>
<div class="stageSeparator"></div>
<% var stage2RM =
  memoList.Find(
    memo =>
    memo.RejectionMemoNumber.ToUpper() == (rejectionMemo.YourRejectionNumber == null ? "" : rejectionMemo.YourRejectionNumber.ToUpper()) && memo.Invoice.InvoiceNumber.ToUpper() == rejectionMemo.YourInvoiceNumber.ToUpper() &&
    memo.Invoice.BillingPeriod == rejectionMemo.YourInvoiceBillingPeriod && memo.Invoice.BillingMonth == rejectionMemo.YourInvoiceBillingMonth && memo.Invoice.BillingYear == rejectionMemo.YourInvoiceBillingYear && memo.RejectionStage == 2);
if (stage2RM != null && stage2RM.RejectionStage == 2)
{
  displayedRMIds = displayedRMIds + ',' + stage2RM.Id.Value();
  %>
  <h2>
    Stage
    <%:stage2RM.RejectionStage%>, Rejection Memo
    </h2>
  <%
Html.RenderPartial("RejectionMemoControl", stage2RM); %>
<div class="stageSeparator"></div>
<% var stage1RM =
  memoList.Find(
    memo =>
    memo.RejectionMemoNumber.ToUpper() == (stage2RM.YourRejectionNumber == null ? "" : stage2RM.YourRejectionNumber.ToUpper()) && memo.Invoice.InvoiceNumber.ToUpper() == stage2RM.YourInvoiceNumber.ToUpper() &&
    memo.Invoice.BillingPeriod == stage2RM.YourInvoiceBillingPeriod && memo.Invoice.BillingMonth == stage2RM.YourInvoiceBillingMonth && memo.Invoice.BillingYear == stage2RM.YourInvoiceBillingYear && memo.RejectionStage == 1);

if (stage1RM != null)
{
  displayedRMIds = displayedRMIds + ',' + stage1RM.Id.Value();
  %>
  <h2>
    Stage
    <%:stage1RM.RejectionStage%>, Rejection Memo
    </h2>
  <%
Html.RenderPartial("RejectionMemoControl", stage1RM); %>

<div class="stageSeparator"></div>
  <%foreach (var cargoInvoice in Model.Invoices.Where(cargoInvoice => cargoInvoice.AwbDataRecord.Count > 0))
  {
    if (stage1RM.YourInvoiceNumber.ToUpper() == cargoInvoice.InvoiceNumber.ToUpper())
    {
%>
  <h2>
    Stage 0, AWB Record</h2>
  <%
      foreach (var coupon in cargoInvoice.AwbDataRecord)
      {
        Html.RenderPartial("AWBPrimeControl", coupon);
      }
%>
  <div class="stageSeparator">
  </div>
  <%
    }
  }
  
    //Dispay the values for credit memo
  foreach (CargoInvoice cargoInvoice in Model.Invoices.Where(cargoInvoice => cargoInvoice.CGOCreditMemo.Count > 0))
{
  List<CargoCreditMemo> cmRecord = cargoInvoice.CGOCreditMemo.Where(cm => cm.CreditMemoNumber.ToUpper() == stage1RM.YourBillingMemoNumber.ToUpper()).ToList();
  if (cmRecord.Count > 0)
  {
  %>
  <h2>
    Stage 0, Credit Memo</h2>
  <%
                Html.RenderPartial("CreditMemoControl", cmRecord[0]);
                
                %>
  <div class="stageSeparator">
  </div>
  <% 
              }
              
            }
  // Display the values of BM
   foreach (CargoInvoice cargoInvoice in Model.Invoices.Where(cargoInvoice => cargoInvoice.CGOBillingMemo.Count > 0))
            {
              if (cargoInvoice.InvoiceNumber.ToUpper() == stage1RM.YourInvoiceNumber.ToUpper())
              {
  %>
  <h2>
   Stage 0, Billing Memo</h2>
  <%
                Html.RenderPartial("BillingMemoControl", cargoInvoice.CGOBillingMemo[0]);

   %>
  <div class="stageSeparator">
  </div>
  <%
              }
            }
  
}

}
 }
 else if (rejectionMemo.RejectionStage == 2 && !displayedRMIds.Contains(rejectionMemo.Id.Value()))
        {
  %>
  <br />
  <h2>
    Stage
    <%:rejectionMemo.RejectionStage%>, Rejection Memo
    </h2>
  <%
Html.RenderPartial("RejectionMemoControl", rejectionMemo);

var stage1RM =
  memoList.Find(
    memo =>
    memo.RejectionMemoNumber.ToUpper() == (rejectionMemo.YourRejectionNumber == null ? "" : rejectionMemo.YourRejectionNumber.ToUpper()) && memo.Invoice.InvoiceNumber.ToUpper() == rejectionMemo.YourInvoiceNumber.ToUpper() &&
    memo.Invoice.BillingPeriod == rejectionMemo.YourInvoiceBillingPeriod && memo.Invoice.BillingMonth == rejectionMemo.YourInvoiceBillingMonth && memo.Invoice.BillingYear == rejectionMemo.YourInvoiceBillingYear && memo.RejectionStage == 1);

if (stage1RM != null)
{
  displayedRMIds = displayedRMIds + ',' + stage1RM.Id.Value();
  %>
  <h2>
    Stage
    <%:stage1RM.RejectionStage%>, Rejection Memo
    </h2>
  <%
Html.RenderPartial("RejectionMemoControl", stage1RM);

foreach (var cargoInvoice in Model.Invoices.Where(cargoInvoice => cargoInvoice.AwbDataRecord.Count > 0))
  {
    if (stage1RM.YourInvoiceNumber.ToUpper() == cargoInvoice.InvoiceNumber.ToUpper())
    {
%>
  <h2>
    Stage 0, AWB Record</h2>
  <%
      foreach (var coupon in cargoInvoice.AwbDataRecord)
      {
        Html.RenderPartial("AWBPrimeControl", coupon);

      }
%>
  <div class="stageSeparator">
  </div>
  <%
    }
  }

  //Dispay the values for credit memo
  foreach (CargoInvoice cargoInvoice in Model.Invoices.Where(cargoInvoice => cargoInvoice.CGOCreditMemo.Count > 0))
{
  List<CargoCreditMemo> cmRecord = cargoInvoice.CGOCreditMemo.Where(cm => cm.CreditMemoNumber.ToUpper() == stage1RM.YourBillingMemoNumber.ToUpper()).ToList();
  if (cmRecord.Count > 0)
  {
  %>
  <h2>
    Stage 0, Credit Memo</h2>
  <%
    Html.RenderPartial("CreditMemoControl", cmRecord[0]);
                
                 %>
  <div class="stageSeparator">
  </div>
  <%
    }
    }
    
   // Display the values of BM
  foreach (CargoInvoice cargoInvoice in Model.Invoices.Where(paxInvoice => paxInvoice.CGOBillingMemo.Count > 0))
            {
              if (cargoInvoice.InvoiceNumber.ToUpper() == stage1RM.YourInvoiceNumber.ToUpper())
              {
  %>
  <h2>
   Stage 0, Billing Memo</h2>
  <%
                Html.RenderPartial("BillingMemoControl", cargoInvoice.CGOBillingMemo[0]);

   %>
  <div class="stageSeparator">
  </div>
  <%
              }
            }
    
}
else
{
  foreach (var cargoInvoice in Model.Invoices.Where(cargoInvoice => cargoInvoice.AwbDataRecord.Count > 0))
  {
  %>
  <h2>
    Stage 0, AWB Record</h2>
  <%
    foreach (var coupon in cargoInvoice.AwbDataRecord)
    {
      Html.RenderPartial("AWBPrimeControl", coupon);

    }
  }
  %>
  <div class="stageSeparator">
  </div>
  <%
}
        }
        else if (rejectionMemo.RejectionStage == 1 && !displayedRMIds.Contains(rejectionMemo.Id.Value()))
        {
  %>
  <br />
  <h2>
    Stage
    <%:rejectionMemo.RejectionStage%>, Rejection Memo
    </h2>
  <%
Html.RenderPartial("RejectionMemoControl", rejectionMemo);


foreach (var cargoInvoice in Model.Invoices.Where(cargoInvoice => cargoInvoice.AwbDataRecord.Count > 0))
{
  if (rejectionMemo.YourInvoiceNumber.ToUpper() == cargoInvoice.InvoiceNumber.ToUpper())
  {
%>
  <h2>
    Stage 0, AWB Record</h2>
  <%
    foreach (var coupon in cargoInvoice.AwbDataRecord)
    {
      Html.RenderPartial("AWBPrimeControl", coupon);

    }
%>
  <div class="stageSeparator">
  </div>
  <%
  }
}


          foreach (CargoInvoice cargoInvoice in Model.Invoices.Where(cargoInvoice => cargoInvoice.CGOCreditMemo.Count > 0))
{
  List<CargoCreditMemo> cmRecord = cargoInvoice.CGOCreditMemo.Where(cm => cm.CreditMemoNumber.ToUpper() == rejectionMemo.YourBillingMemoNumber.ToUpper()).ToList();
  if (cmRecord.Count > 0)
  {            
  %>
  <h2>
    Stage 0, Credit Memo</h2>
  <%
    Html.RenderPartial("CreditMemoControl", cmRecord[0]);
                
                %>
  <div class="stageSeparator">
  </div>
  <% 
              }
            }


          foreach (CargoInvoice cargoInvoice in Model.Invoices.Where(cargoInvoice => cargoInvoice.CGOBillingMemo.Count > 0))
            {
              if (cargoInvoice.InvoiceNumber.ToUpper() == rejectionMemo.YourInvoiceNumber.ToUpper())
              {
  %>
  <h2>
   Stage 0, Billing Memo</h2>
  <%
                Html.RenderPartial("BillingMemoControl", cargoInvoice.CGOBillingMemo[0]);

   %>
  <div class="stageSeparator">
  </div>
  <%
              }
            }
        }
      }
    }

    else
    {
      foreach (var cargoInvoice in Model.Invoices.Where(cargoInvoice => cargoInvoice.AwbDataRecord.Count > 0))
      {
  %>
  <h2>
    Stage 0, AWB Record</h2>
  <%
        foreach (var coupon in cargoInvoice.AwbDataRecord)
        {
          Html.RenderPartial("AWBPrimeControl", coupon);
        }
        
 }

      foreach (CargoInvoice cargoInvoice in Model.Invoices.Where(cargoInvoice => cargoInvoice.CGOCreditMemo.Count > 0))
      {
             
  %>
  <h2>
    Stage 0, Credit Memo</h2>
  <%
        Html.RenderPartial("CreditMemoControl", cargoInvoice.CGOCreditMemo[0]);
            }


      foreach (CargoInvoice cargoInvoice in Model.Invoices.Where(cargoInvoice => cargoInvoice.CGOBillingMemo.Count > 0))
            {
  %>
  <h2>
     Stage 0, Billing Memo</h2>
  <%
              Html.RenderPartial("BillingMemoControl", cargoInvoice.CGOBillingMemo[0]);
              
            }
             %>
  <% if (Model.Invoices.Count != 0)
   {%>
  <div class="stageSeparator"></div>
  <%
   }
   else {%>
   <h2>No audit trail found for selected transaction.</h2>
<%}%>
  <%
    }
  %>
  <div class="buttonContainer">
    <input type="button" class="secondaryButton" value="Back" onclick="history.go(-1);" />
     <% if (Model.Invoices.Count != 0)
   {%>
    <%--CMP508:Audit Trail Download with Supporting Documents--%>
    <input type="button" class="secondaryButton" value="Generate PDF" onclick="DownloadFile('<%: Url.Action("GenerateBillingHistoryAuditTrailPdfForCgo", "BillingHistory", new {transactionId = ViewData["TransactionId"].ToString(), transactionType = ViewData["TransactionType"].ToString()}) %>','<%: Url.Action("EnqueBillingHistoryAuditTrailDownload", "BillingHistory", new {transactionId = ViewData["TransactionId"].ToString(), transactionType = ViewData["TransactionType"].ToString()}) %>')" />
    <input type="checkbox" id = "IncludeSuppDocs" name="Include Supporting Document(s)" value="IncludeSuppDocs" /> Include Supporting Document(s)
    <% }%>
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Cargo/BillingHistory.js")%>"></script>
  
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
