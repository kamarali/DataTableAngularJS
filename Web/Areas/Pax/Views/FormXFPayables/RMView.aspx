<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.RejectionMemo>" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>

<%@ Import Namespace="Iata.IS.Model.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> :: Non-Sampling Invoice :: View Rejection Memo
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <%:ScriptHelper.GenerateGridViewScriptForBreakdown(Url, ControlIdConstants.RMCouponGridId, Url.Action("RMCouponView", new { transaction = "RMEdit" }))%>
  <script type="text/javascript">
    $(document).ready(function () {
      initializeParentForm('rejectionMemoForm');     
      $isOnView = true;
      SetPageToViewMode($isOnView);
      InitializeVatGrid(<%= new JavaScriptSerializer().Serialize(Model.RejectionMemoVat) %>);
      InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.Invoice.BilledMemberId, BillingCategoryType.Pax) %>', '<%: Url.Action("RMFormFXFAttachmentDownload","FormXFPayables", new { invoiceId = Model.InvoiceId } ) %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");
    });
  </script>
  <script type="text/javascript">
    // Set billing type from Viewdata
    billingType = '<%: ViewData[ViewDataConstants.BillingType] %>';
  </script>
  <script src="<%:Url.Content("~/Scripts/Pax/VatBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    View Rejection Memo</h1>
  <div>
    <%
      Html.RenderPartial(Url.Content("~/Areas/Pax/Views/Shared/SamplingInvoiceHeaderInfoControl.ascx"), Model.Invoice); %>
  </div>
  <div>
    <%Html.RenderPartial(Url.Content("~/Areas/Pax/Views/BaseFXF/RMDetailsControl.ascx"), Model); %>
    <div class="buttonContainer">    
     <%if (!string.IsNullOrEmpty(SessionUtil.PaxCorrSearchCriteria) || !string.IsNullOrEmpty(SessionUtil.PaxInvoiceSearchCriteria))
        {
      %>
        <%: Html.LinkButton("Back To Billing History", Url.Action("Index", "BillingHistory", new { back = true }))%>
      <%
        }else{%> 
    <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href='<%: Url.Action("View", "FormXFPayables", new { invoiceId = Model.Invoice.Id.Value() } ) %>'" />
    <%} %>
    </div>
  </div>
  <h2>
    Rejection Memo Coupon List
  </h2>
  <div>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.RMCouponListGrid]); %>
  </div>
  <div id="divVatBreakdown" class="hidden">
    <%
      Html.RenderPartial("~/Areas/Pax/Views/BaseFXF/RMVatControl.ascx", Model.RejectionMemoVat);%>
  </div>
  <div class="hidden" id="divAttachment">
    <%
      Html.RenderPartial("RMFormXFAttachmentControl", Model);%>
  </div>
</asp:Content>
