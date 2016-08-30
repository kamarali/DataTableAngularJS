<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.CreditMemo>" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<%@ Import Namespace="System.Security.Policy" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Passenger ::
    <%: ViewData[ViewDataConstants.BillingType].ToString() %>
    ::
    <%: ViewData[ViewDataConstants.PageMode] %>
    Credit Memo
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script type="text/javascript">
    // Declare variable to capture reasonCode before editing
    var previousReasonCode = '';
    // Following variable is used to check edit level. i.e. CMEdit or CmCouponEdit. Edit level is checked before calling "EnableDisableAmountFieldsOnreasonCode()"
    // This function should be executed only if we are editing CreditMemo. 
    var editLevel = 'CreditMemoEdit';
    $(document).ready(function () {
      initializeParentForm('creditMemoForm');     
      // Get previous reasonCode value 
      previousReasonCode = $('#ReasonCode').val();
      // Get whether coupon breakdown exists
      couponBreakdownExists = '<%: ViewData[ViewDataConstants.BreakdownExists] %>';
      // Get pageMode
      pageMode = '<%: ViewData[ViewDataConstants.PageMode] %>';
            
      SetPageToViewModeEx(<%:(ViewData[ViewDataConstants.PageMode] == PageMode.View).ToString().ToLower()%>, '#creditMemoForm,#divVatBreakdown,#divAttachment'); 
      registerAutocomplete('ReasonCode', 'ReasonCode', '<%:Url.Action("GetReasonCodeListForAutoComplete", "Data", new { area="" })%>', 0, false, onReasonCodeChange, '', '<%:Convert.ToInt32(TransactionType.CreditMemo)%>', null, onBlankReasonCode);
      
    <%
      
 if(ViewData[ViewDataConstants.PageMode] != null &&  ViewData[ViewDataConstants.PageMode].ToString() =="View")
 {
%>
        $('input[type=text]').attr('disabled', 'disabled');
        $('select').attr('disabled', 'disabled');
        $("#SaveFormD").hide();
        $('textarea').attr('disabled','disabled');
        $('input[type=checkbox]').attr('disabled','disabled');
         $isOnView = true;
  <%
 }
%>
    $("#SourceCodeId").autocomplete({ disabled: true });
      InitializeVatGrid(<%= new JavaScriptSerializer().Serialize(Model.VatBreakdown) %>);      
      InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.Invoice.BilledMemberId, BillingCategoryType.Pax) %>', '<%: Url.Action("CreditMemoAttachmentDownload","CreditNotePayables", new {invoiceId = Model.InvoiceId}) %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");      
      setControlAccess('<%: ViewData[ViewDataConstants.BreakdownExists] %>');      
    });
    </script>
    <script type="text/javascript">
        // Set billing type from Viewdata
        billingType = '<%: ViewData[ViewDataConstants.BillingType] %>';
    </script>
    <script type="text/javascript" src="<%:Url.Content("~/Scripts/Pax/creditNote.js")%>"></script>
    <script src="<%:Url.Content("~/Scripts/Pax/VatBreakdown.js")%>" type="text/javascript"></script>
    <script src="<%:Url.Content("~/Scripts/Pax/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
    <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
    <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        <%: ViewData[ViewDataConstants.PageMode] %>
        Credit Memo</h1>
    <div>
        <% Html.RenderPartial("ReadOnlyCreditNoteHeaderControl", Model.Invoice); %>
    </div>
    <div>
        <% using (Html.BeginForm("CreditMemoEdit", "CreditNotePayables", FormMethod.Post, new { id = "creditMemoForm", @class = "validCharacters" }))
           {
               Html.RenderPartial("CMDetailsControl", Model); %>
    </div>
    <div class="buttonContainer">
        <% if (ViewData[ViewDataConstants.PageMode] != null && ViewData[ViewDataConstants.PageMode].ToString() == "View")
           {
               if (!string.IsNullOrEmpty(SessionUtil.PaxCorrSearchCriteria) || !string.IsNullOrEmpty(SessionUtil.PaxInvoiceSearchCriteria))
               {
        %>
        <input class="secondaryButton" type="button" value="Back To Billing History" onclick="javascript:location.href = '<%:Url.Action("Index", "BillingHistory", new { back = true })%>'" />
        <%
               }
               else
               {
        %>
        <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href='<%:Url.Action("View",
                                 "CreditNotePayables",
                                 new
                                   {
                                     invoiceId = Model.Invoice.Id.Value()
                                   })%>'" />
        <%
  }
           }
           else
           { %>
        <input type="submit" value="Save" class="primaryButton ignoredirty" id="Save" />
        <input type="submit" value="Add Credit Memo Coupon" class="secondaryButton" id="btnAdd"
            onclick="javascript:location.href='<%: Url.Action("CreditMemoCouponCreate", "CreditNote", new { invoiceId = Model.Invoice.Id.Value(), transactionId = Model.Id.Value(), transaction = "CMEdit" })%>'; return false;" />
        <% 
               if (!string.IsNullOrEmpty(SessionUtil.PaxCorrSearchCriteria) || !string.IsNullOrEmpty(SessionUtil.PaxInvoiceSearchCriteria))
               {
        %>
        <%: Html.LinkButton("Back To Billing History", Url.Action("Index", "BillingHistory", new { back = true }))%>
        <%
           }
           else
           {
        %>
        <%: Html.LinkButton("Back", Url.RouteUrl("CreditNoteEdit", new { action = "Edit", invoiceId = Model.Invoice.Id.Value() }))%>
        <%
  }
           } %>
        <% } %>
    </div>
    <h2>
        Credit Memo Coupon List</h2>
    <div>
        <%Html.RenderPartial("GridControl", ViewData[ViewDataConstants.CMCouponGrid]); %>
    </div>
    <div class="hidden" id="divVatBreakdown">
        <% Html.RenderPartial("VatControl", Model.VatBreakdown); %>
    </div>
    <div class="hidden" id="divAttachment">
        <%
            Html.RenderPartial("CreditMemoAttachmentControl", Model);%>
    </div>
    <div class="clear">
    </div>
    <%
        if (ViewData[ViewDataConstants.PageMode] != null && ViewData[ViewDataConstants.PageMode].ToString() == "View")
        {
    %>
    <%:ScriptHelper.GenerateGridViewScriptForBreakdown(Url, ControlIdConstants.CreditMemoCouponGrid, Url.Action("CreditMemoCouponView", new { transaction = "CMView" }))%>
    <%
        }
        else
        {
    %>
    <%: ScriptHelper.GenerateGridEditDeleteScript(Url, ControlIdConstants.CreditMemoCouponGrid, Url.Action("CreditMemoCouponEdit", new { transaction = "CMEdit" }), Url.Action("CreditMemoCouponDelete", new { transaction = "CMEdit" }))%>
    <%
    }
    %>
    <script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
</asp:Content>
