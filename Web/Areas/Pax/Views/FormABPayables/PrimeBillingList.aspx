<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.PaxInvoice>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger ::
  <%: ViewData[ViewDataConstants.BillingType].ToString() %>
  :: Non-Sampling Invoice :: Prime Billing
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
      if ((string)ViewData[ViewDataConstants.PageMode] == PageMode.View)
      {%>
        <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("View","FormABPayables",new{invoiceId = Model.Id.Value()})%>';" /> 
     <% } %>
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
  <script src="<%:Url.Content("~/Scripts/Pax/Billinghistory.js")%>" type="text/javascript"></script>
  <%
    if ((string)ViewData[ViewDataConstants.PageMode] == PageMode.View)
    {
  %>
  <%:ScriptHelper.GenerateGridViewScript(Url, ViewDataConstants.PrimeBillingGrid, Url.Action("PrimeBillingView"))%>
  <%
    }   
  %>
  <script type="text/javascript">
//    function SetRejectAccess() {
//      var couponsGridName = '<%: ViewDataConstants.PrimeBillingGrid %>';
//      var selectedCoupons = jQuery('#' + couponsGridName).getGridParam('selarrrow');
//      var $RejectButton = $('#RejectButton', '#content');
//      if (selectedCoupons != null && selectedCoupons.length > 0) {
//        $RejectButton.removeAttr('disabled');
//      }
//      else
//        $RejectButton.attr('disabled', 'disabled');
//    }
  </script>
</asp:Content>
