<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Cargo.CargoInvoice>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Cargo ::
  <%: ViewData[ViewDataConstants.BillingType].ToString() %>
  ::
  <%--<%:ViewData[ViewDataConstants.PageMode] %> Non-Sampling--%>
  Invoice :: Prepaid AWB Listing
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
   Prepaid AWB Billing Records</h1>
  <div>
    <% Html.RenderPartial("ReadOnlyInvoiceHeaderControl", Model); %>
  </div>
  <h2>
    AWB Prepaid Billing Record List</h2>
   <div>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.PrimeBillingGrid]); %>
  </div>
  <div class="buttonContainer">
    <%
      if ((string)ViewData[ViewDataConstants.PageMode] != PageMode.View)
      {
        using (Html.BeginForm("AwbPrepaidBillingCreate", "Invoice", FormMethod.Get))
        {%>
    <input type="submit" value="Add" class="primaryButton" id="btnAdd" />
    <input class="secondaryButton" type="button" value="Back to View Invoice" onclick="javascript:location.href = '<%:Url.Action("Edit", "Invoice", new { invoiceId = Model.Id.Value() })%>';" />
    <%
      }
      }
      else
      {
        if (ViewData[ViewDataConstants.BillingType] != null && ViewData[ViewDataConstants.BillingType].ToString() == BillingType.Payables)
        {
            // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
            if (TempData["canCreate"] != null && Convert.ToBoolean(TempData["canCreate"]) && TempData["canView"] != null && Convert.ToBoolean(TempData["canView"]))
            {%>
                <input class="primaryButton" type="button" value="Reject" onclick="javascript:InitiateRejForSpecificTrans('<%:ViewDataConstants.PrimeBillingGrid%>', 'AWB', '<%:Model.Id%>', '<%:Url.Action("InitiateRejection", "InvoicePayables", new {area = "Cargo"})%>','<%:Url.Action("InitiateDuplicateRejection", "InvoicePayables", new {area = "Cargo"})%>', <%:Model.BillingCode%>, <%:Model.BillingYear%>,<%:Model.BillingMonth%>,<%:Model.BillingPeriod%>,<%:Model.SettlementMethodId%>);" id ="RejectButton" />
          <%}
      }%>
    <input class="secondaryButton" type="button" value="Back to View Invoice" onclick="javascript:location.href = '<%:Url.Action("View","InvoicePayables",new{invoiceId = Model.Id.Value()})%>';" />
    <%
      }%>
  </div>
  <div id="divBillingHistoryInvoice" class="hidden">
    <%
      Html.RenderPartial("BillingHistoryInvoice");%>
  </div>
  <div id="divDuplicateRejections" class="hidden">
    <%
      Html.RenderPartial("~/Areas/Cargo/Views/Shared/DuplicateRejectionControl.ascx");%>
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script src="<%:Url.Content("~/Scripts/Cargo/CargoOtherChargeBreakdown.js")%>" type="text/javascript"></script>
<script src="<%:Url.Content("~/Scripts/Cargo/AwbRecord.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Cargo/Billinghistory.js")%>" type="text/javascript"></script>
  <%
    if ((string)ViewData[ViewDataConstants.PageMode] == PageMode.View)
    {
  %>
  -
  <%:ScriptHelper.GenerateGridViewScript(Url, ViewDataConstants.PrimeBillingGrid, Url.Action("AwbPrepaidBillingView", "InvoicePayables", new { billingType = ViewData[ViewDataConstants.BillingType].ToString(), invoiceId = Model.Id }))%>
  <%
    }
    else
    {
  %>
  <%:ScriptHelper.GenerateGridEditDeleteScript(Url, ViewDataConstants.PrimeBillingGrid, Url.Action("AwbPrepaidRecordEdit"), Url.Action("AwbPrepaidRecordDelete"))%>
  <%
    }
  %>
  <script type="text/javascript">
    function SetRejectAccess() {
          var couponsGridName = '<%: ViewDataConstants.PrimeBillingGrid %>';
          var selectedCoupons = jQuery('#' + couponsGridName).getGridParam('selarrrow');
          var $RejectButton = $('#RejectButton', '#content');
          if (selectedCoupons != null && selectedCoupons.length > 0) {
              $RejectButton.removeAttr('disabled');
          }
          else
              $RejectButton.attr('disabled', 'disabled');
      }
  </script>
</asp:Content>
