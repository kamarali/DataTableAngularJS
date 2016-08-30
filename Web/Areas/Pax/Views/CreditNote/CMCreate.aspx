<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.CreditMemo>" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>

<%@ Import Namespace="Iata.IS.Model.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Passenger :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> :: Create Credit Memo
</asp:Content>
<asp:Content ContentPlaceHolderID="Script" runat="server">
    <script type="text/javascript">
    $(document).ready(function () {
      initializeParentForm('creditMemoForm');     
      // Retrieve page mode
      pageMode = '<%: ViewData[ViewDataConstants.PageMode] %>';
      // Set couponBreakdownExists to 'False' in Create mode
      couponBreakdownExists = 'False';
      // Set variable to true if PageMode is "View"
      $isOnView = <%:((ViewData[ViewDataConstants.PageMode]!=null && ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View) ? true : false ).ToString().ToLower() %>;
      registerAutocomplete('ReasonCode', 'ReasonCode', '<%:Url.Action("GetReasonCodeListForAutoComplete", "Data", new { area="" })%>', 0, false, onReasonCodeChange, '', '<%:Convert.ToInt32(TransactionType.CreditMemo)%>', null, onBlankReasonCode);
      // Set page mode to Create only on Pageload, if exception occurs while Submit do not set page to Create mode.  
      if('<%: Convert.ToBoolean(ViewData[ViewDataConstants.IsPostback])%>' == 'False'){
        SetPageModeToCreateMode(<%:((string)ViewData[ViewDataConstants.PageMode] == PageMode.Create).ToString().ToLower()%>);
      }
      InitializeVatGrid(<%= new JavaScriptSerializer().Serialize(Model.VatBreakdown) %>);
      InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.Invoice.BilledMemberId, BillingCategoryType.Pax) %>', '<%: Url.Action("CreditMemoAttachmentDownload","CreditNote") %>', "<%:Url.Content("~/Content/Images/busy.gif")%>");
    });

    </script>
    <script type="text/javascript">
      // Set billing type from Viewdata
      billingType = '<%: ViewData[ViewDataConstants.BillingType] %>';
    </script>  
    <script type="text/javascript" src="<%:Url.Content("~/Scripts/Pax/creditNote.js")%>">
    </script>
    <script src="<%:Url.Content("~/Scripts/Pax/VatBreakdown.js")%>" type="text/javascript"></script>
    <script src="<%:Url.Content("~/Scripts/Pax/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
    <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
    <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        Create Credit Memo</h1>
    <div>
        <% Html.RenderPartial("ReadOnlyCreditNoteHeaderControl", Model.Invoice); %>
    </div>
    <div>
        <% using (Html.BeginForm("CreditMemoCreate", "CreditNote", FormMethod.Post, new { id = "creditMemoForm", @class = "validCharacters" }))
           {%>
           <%: Html.AntiForgeryToken() %>
           <%
               Html.RenderPartial("CMDetailsControl", Model);%>
    </div>
    <div class="buttonContainer">
        <input type="submit" value="Save" class="primaryButton ignoredirty" id="Save" />
        <%: Html.LinkButton("Back", Url.Action("Edit", "CreditNote", new { invoiceId = Model.Invoice.Id.Value() }))%>
    </div>
    <%
        }%>
    <div class="hidden" id="divVatBreakdown">
        <% Html.RenderPartial("VATControl", Model.VatBreakdown); %>
    </div>
    <div class="hidden" id="divAttachment">
        <%
            Html.RenderPartial("CreditMemoAttachmentControl", Model);%>
    </div>
    <div class="clear">
    </div>
</asp:Content>
