<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.PaxInvoice>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger ::
  <%: ViewData[ViewDataConstants.BillingType].ToString() %>
  :: <%:ViewData[ViewDataConstants.PageMode] %> Non-Sampling Invoice :: Prime Billing List
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Prime Billing Coupons</h1>
  <div>
    <% Html.RenderPartial("ReadOnlyInvoiceHeaderControl", Model); %>
  </div>
  <h2>
    Prime Billing Coupon List</h2>
  <div>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.PrimeBillingGrid]); %>
  </div>
  <div class="buttonContainer">
    <%
      if ((string)ViewData[ViewDataConstants.PageMode] != PageMode.View)
      {
        using (Html.BeginForm("PrimeBillingCreate", "Invoice", FormMethod.Get))
        {%>
    <input type="submit" value="Add" class="primaryButton" id="btnAdd" />
    <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Edit", "Invoice", new { invoiceId = Model.Id.Value() })%>';" />
    <%
      }
      }
      else
      {
        if (ViewData[ViewDataConstants.BillingType] != null && ViewData[ViewDataConstants.BillingType].ToString() == BillingType.Payables)
        {%>
    <input class="primaryButton" type="button" value="Reject" onclick="javascript:InitiateRejForSpecificTrans('<%: ViewDataConstants.PrimeBillingGrid %>', 'PC', '<%: Model.Id %>', '<%:Url.Action("InitiateRejection", "Invoice", new { area = "Pax" })%>','<%:Url.Action("InitiateDuplicateRejection", "Invoice", new { area = "Pax" })%>', <%:Model.BillingCode %>, <%:Model.BillingYear %>,<%:Model.BillingMonth %>,<%:Model.BillingPeriod %>,<%:Model.SettlementMethodId %>);" id ="RejectButton"/>
    <%}%>
    <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("View","Invoice",new{invoiceId = Model.Id.Value()})%>';" />
    <%
      }%>
  </div>
  <div id="divBillingHistoryInvoice" class="hidden">
    <%
      Html.RenderPartial("BillingHistoryInvoice");%>
  </div>
  <div id="divDuplicateRejections" class="hidden">
    <%
      Html.RenderPartial("DuplicateRejectionControl");%>
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/Billinghistory.js")%>" type="text/javascript"></script>
  <%
    if ((string)ViewData[ViewDataConstants.PageMode] == PageMode.View)
    {
  %>
  -
  <%:ScriptHelper.GenerateGridViewScript(Url, ViewDataConstants.PrimeBillingGrid, Url.Action("PrimeBillingView"))%>
  <%
    }
    else
    {
  %>
  <%:ScriptHelper.GenerateGridEditDeleteScript(Url, ViewDataConstants.PrimeBillingGrid, Url.Action("PrimeBillingEdit"), Url.Action("PrimeBillingDelete"))%>
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
