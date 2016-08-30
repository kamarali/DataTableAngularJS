<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.PaxInvoice>" MasterPageFile="~/Views/Shared/Site.Master" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>

<asp:Content runat="server" ID="Content" ContentPlaceHolderID="TitleContent">
  SIS :: Passenger ::
  <%: ViewData[ViewDataConstants.BillingType].ToString() %>
  :: Sampling Invoice ::
  <%:ViewData[ViewDataConstants.PageMode] %>
  Invoice
</asp:Content>
<asp:Content runat="server" ID="Content1" ContentPlaceHolderID="Script">
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Pax/InvoiceHeader.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/MemberReference.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Pax/ValidateInvoice.js")%>"></script>
  <script type="text/javascript">
      $(document).ready(function () { 
      var isViewMode = <%:ViewData[ViewDataConstants.PageMode] !=null ?(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View).ToString().ToLower() : "false"%>;
      InitializeBilateralSMIs(<%: Convert.ToInt32(SMI.Bilateral) %>  ,'<%: ViewData[ViewDataConstants.BilateralSMIs] %>');
      InitialiseInvoiceHeader('<%: Url.Action("GetBilledMemberLocationList", "Data", new { area = ""})%>', '<%: Url.Action("GetExchangeRate", "Data", new { area = ""})%>',<%:ViewData[ViewDataConstants.PageMode] !=null ?(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View).ToString().ToLower() : "false"%>, '<%: Url.Action("GetDefaultSettlementMethod", "Data", new { area = ""})%>', '<%:Url.Action("GetDefaultCurrency", "Data", new { area = "" })%>','<%:SessionUtil.MemberId%>', '#InvoiceForm,#BillingMemberReference,#BilledMemberReference');
      
      <%
      if(ViewData[ViewDataConstants.TransactionExists] != null && Convert.ToBoolean(ViewData[ViewDataConstants.TransactionExists]))
      {%>
        SetControlAccess();
       <% }%>
      });
      
    // Following function is used to format SourceCodeVatTotal column within Jqgrid to display link
    function formatSourceCodeVatTotalColumn(cellValue, options, rowObject) {
      // Retrieve SourceCodeVatBreakdownId
      var sourceCodeId = rowObject.Id;
      // Create link and specify onclick action passing it SourceCodeVatBreakdownId
      var linkHtml = '<a href="#" onclick=showSourceCodeVatDialog("<%: Url.Action("GetSourceCodeVatTotal", "FormAB", new { area = "Pax" })%>","' + sourceCodeId + '")>' + "Source Code VAT" + '</a>';
      // return link
      return linkHtml;
    }            
  </script>
</asp:Content>
<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="MainContent">
  <h1>
    <%:ViewData[ViewDataConstants.PageMode] %>
    Sampling Invoice</h1>
  <%
    using (Html.BeginForm("Edit", "Invoice", FormMethod.Post, new { id = "InvoiceForm", @class = "validCharacters" }))
    {%>
    <%: Html.AntiForgeryToken() %>
  <div>
    <%
      Html.RenderPartial("InvoiceHeaderControl");%>
  </div>
  <div>
    <% Html.RenderPartial("InvoiceTotalControl", Model.InvoiceTotalRecord);%>
  </div>
  <div class="buttonContainer">
    <input class="primaryButton ignoredirty" type="submit" value="Save Invoice" id="SaveInvoiceHeader" />
  </div>
  <%
    }%>
  <div>
    <% Html.RenderPartial("ProvisionalAdjustmentDetails", Model.InvoiceTotalRecord); %>
  </div>
  <h2>
    <div class="clear">
    </div>
    <h2>
      Source Code Summary</h2>
    <div id="sourceCodeDetailsDiv">
      <div>
        <%
          Html.RenderPartial("GridControl", ViewData[ViewDataConstants.SourceCodeGrid]);%>
      </div>
    </div>
    <div class="clear">
    </div>
    <div class="buttonContainer">
      <div class="viewModeDisplayButtons">
        <%
          using (Html.BeginForm("VatView", "FormAB", FormMethod.Get))
          {%>
        <input class="secondaryButton" type="submit" value="Invoice VAT" />
        <%
          }%>

        
        <%
          using (Html.BeginForm("PrimeBillingListView", "FormAB", FormMethod.Get))
          {%>
        <input class="secondaryButton" type="submit" value="Prime Listing" id="PrimeBillingButton" />
        <%
          }   
        if (!string.IsNullOrEmpty(SessionUtil.InvoiceSearchCriteria))
          { %>
        <%: Html.LinkButton("Back to Manage Invoice", Url.Action("Index", "ManageInvoice"))%>
        <%
          }
        %>
      </div>
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
    <div id="BillingMemberReference" class="hidden">
      <%
        Html.RenderPartial("~/Views/Invoice/BillingMemberInfoControl.ascx", Model);%>
    </div>
    <div id="BilledMemberReference" class="hidden">
      <%
        Html.RenderPartial("~/Views/Invoice/BilledMemberInfoControl.ascx", Model);%>
    </div>
    <!-- Following div is used for SourceCode Vat popup so add it only if BillingType is Payables -->
    <%if (ViewData[ViewDataConstants.BillingType].ToString() == Iata.IS.Web.Util.BillingType.Payables) {%>
      <div id="divAvailableVatGridResult" class="hidden">
        <% Html.RenderPartial("~/Areas/Pax/Views/Shared/SourceCodeTotal.ascx", ViewData["VatGrid"]);%>
      </div>
    <%}%>
</asp:Content>
