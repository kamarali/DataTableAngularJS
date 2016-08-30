<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.PaxInvoice>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger ::
  <%: ViewData[ViewDataConstants.BillingType].ToString() %>
  ::
  <%:(ViewData[ViewDataConstants.PageMode].ToString())%>
  Form D/E
</asp:Content>
<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="Script">
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Pax/InvoiceHeader.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Pax/FormDEHeader.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/MemberReference.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Pax/BillingHistory.js")%>"></script>
  <script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
  <script type="text/javascript">
      $(document).ready(function () {
      <%if(ViewData["IsLegalTextSet"] != null && Convert.ToBoolean(ViewData["IsLegalTextSet"]) == true)
    {%>
    isBillingLegalTextSet = true;
  <%
    }%>
      var isViewMode = <%:ViewData[ViewDataConstants.PageMode] !=null ?(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View).ToString().ToLower() : "false"%>;
      InitializeBilateralSMIs(<%: Convert.ToInt32(SMI.Bilateral) %>  ,'<%: ViewData[ViewDataConstants.BilateralSMIs] %>');
      InitialiseInvoiceHeader('<%: Url.Action("GetBilledMemberLocationList", "Data", new { area = ""})%>', '<%: Url.Action("GetExchangeRate", "Data", new { area = ""})%>', <%:(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View).ToString().ToLower()%>, '<%: Url.Action("GetDefaultSettlementMethod", "Data", new { area = ""})%>', '<%:Url.Action("GetDefaultCurrency", "Data", new { area = "" })%>','<%:SessionUtil.MemberId%>', '#InvoiceForm,#BillingMemberReference,#BilledMemberReference');
      if(isViewMode == false)
        InitReferenceData('<%:Url.Action("GetMemberLocationDetails","Data",new{area = string.Empty})%>', '<%:Model.Id%>','<%:Url.Action("GetSubdivisionNameList","Data",new{area = string.Empty})%>');
      <% if(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
         {
      %>
        $("#SaveHeader").hide();
      <%
         }
      %>
        
        <%
       
        if((ViewData[ViewDataConstants.TransactionExists] != null && Convert.ToBoolean(ViewData[ViewDataConstants.TransactionExists])))
        {
%>
        InitializeEditFormDE();
        $("#BilledMemberText").autocomplete({ disabled: true });
        <%
        }
         else
      {%>
      
      if ($('#IsFormABViaIS').val() == true || $('#IsFormCViaIS').val() == 'True')
      {
          InitializeEditFormDE();
          $("#BilledMemberText").autocomplete({ disabled: true });
      }
      else
      {
      /*CMP #596: Length of Member Accounting Code to be Increased to 12 
          Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
          Ref: FRS Section 3.4 Table 15 Row 6 */
      registerAutocomplete('BilledMemberText', 'BilledMemberId', '<%:Url.Action("GetMemberListForPaxCgo", "Data", new { area = "" })%>', 0, true, function (selectedId) { BilledMemberText_AutoCompleteValueChange(selectedId); });        
      }
      <% } %>
      });

      function SetRejectAccess(gridName) {
        var couponsGridName = '<%: ControlIdConstants.FormDGridId %>';
        var selectedCoupons = jQuery('#' + couponsGridName).getGridParam('selarrrow');
        var $RejectButton = $('#RejectButton', '#content');
        if (selectedCoupons != null && selectedCoupons.length > 0)
          $RejectButton.removeAttr('disabled');      
        else
          $RejectButton.attr('disabled', 'disabled');
      }
  </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    <%:(ViewData[ViewDataConstants.PageMode].ToString())%>
    Form D/E</h1>
  <% using (Html.BeginForm("Edit", "FormDE", FormMethod.Post, new { id = "InvoiceForm", @class = "validCharacters" }))
     {%>
     <%: Html.AntiForgeryToken() %>
  <div>
    <%
       Html.RenderPartial("SamplingInvoiceHeaderControl", Model);%>
  </div>
  <div class="buttonContainer">
    <input class="primaryButton ignoredirty" type="submit" value="Save Form D/E Header" id="SaveHeader" />
  </div>
  <%
     }%>
  <div class="clear">
  </div>
  <div id="sourceCodeDetailsDiv">
    <h2>
      Summary List</h2>
    <% Html.RenderPartial("GridControl", ViewData[ControlIdConstants.FormDSourceCodeGridId]); %>
  </div>
  <div>
    <h2>
      Form D List</h2>
    <% Html.RenderPartial("GridControl", ViewData[ControlIdConstants.FormDGridId]); %>
    <%
      if ((string)ViewData[ViewDataConstants.PageMode] == PageMode.View)
      {
    %>
    <%:ScriptHelper.GenerateGridViewScript(Url, ControlIdConstants.FormDGridId, Url.Action("FormDView"))%>
    <%
      }
      else
      {
    %>
    <%:ScriptHelper.GenerateGridEditDeleteScript(Url, ControlIdConstants.FormDGridId, Url.Action("FormDEdit"), Url.Action("FormDDelete"))%>
    <%
      }
    %>
  </div>
  <div class="clear">
  </div>
  <div class="buttonContainer">
    <%
  
      if ((string)ViewData[ViewDataConstants.PageMode] != PageMode.View)
      {
        using (Html.BeginForm("FormDCreate", "FormDE", FormMethod.Get))
        {%>
    <input class="primaryButton" type="submit" value="Add Form D Item" />
    <%
        }
      }
    %>
    <div id="divTransactions">
        <%if (ViewData[ViewDataConstants.BillingType] != null && ViewData[ViewDataConstants.BillingType].ToString() == Iata.IS.Web.Util.BillingType.Payables)
        {%>
    <input class="secondaryButton" type="button" value="Reject" onclick="javascript:InitiateRejForSpecificTrans('<%: ControlIdConstants.FormDGridId %>', 'FD', '<%: Model.Id %>', '<%:Url.Action("InitiateRejection", "FormDE", new { area = "Pax" })%>','<%:Url.Action("InitiateDuplicateRejection", "FormDE", new { area = "Pax" })%>', <%:Model.BillingCode %>, <%:Model.BillingYear %>,<%:Model.BillingMonth %>,<%:Model.BillingPeriod %>,<%:Model.SettlementMethodId %>);" id ="RejectButton"/>
    <%}%>
      <% if (ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
         {
           using (Html.BeginForm("FormEView", "FormDE", FormMethod.Get))
           {%>
      <input class="secondaryButton" type="submit" value="Form E Details" />
      <% }
         }
         else
         {
           using (Html.BeginForm("FormEEdit", "FormDE", FormMethod.Get))
           {%>
      <input class="secondaryButton" type="submit" value="Form E Details" />
      <% }
         }%>
      <% if (ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
         {
           using (Html.BeginForm("ProvisionalInvoiceView", "FormDE", FormMethod.Get))
           {%>
      <input class="secondaryButton" type="submit" value="Provisional Invoice(s)" />
      <%
           }
         }
         else
         {
           using (Html.BeginForm("ProvisionalInvoice", "FormDE", FormMethod.Get))
           {%>
      <input class="secondaryButton" type="submit" value="Provisional Invoice(s)" />
      <%
           }
         }%>
      <% using (Html.BeginForm(((ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View) ? "VatView" : "Vat"), "FormDE", FormMethod.Get))
         { %>
      <input class="secondaryButton" type="submit" value="Invoice VAT" /><% } %>
      <div class="buttonContainer">
        <%
          // TODO: Need to figure out which invoice status should be check here
          if ((Model.InvoiceStatus == InvoiceStatusType.Open || Model.InvoiceStatus == InvoiceStatusType.ValidationError) && Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Pax.Receivables.SampleFormDE.Validate))// || Model.Invoice.InvoiceStatus == InvoiceStatus.ReadyForValidation || Model.Invoice.InvoiceStatus == InvoiceStatus.PendingForCorrections)
          {
            using (Html.BeginForm("ValidateDEHeaderInvoice", "FormDE", FormMethod.Post))
            {
        %>
        <%: Html.AntiForgeryToken() %>
        <input class="primaryButton" type="submit" value="Validate Invoice" />
        <%
            }
          }
          if (Model.InvoiceStatus == InvoiceStatusType.ReadyForSubmission && Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Pax.Receivables.SampleFormDE.Submit))
          {
            using (Html.BeginForm("SubmitDEHeader", "FormDE", FormMethod.Post))
            {%>
            <%: Html.AntiForgeryToken() %>
        <input class="primaryButton" type="submit" value="Submit Invoice" />
        <%
            }
          }%>
      </div>
      <%
        if (!string.IsNullOrEmpty(SessionUtil.InvoiceSearchCriteria))
        { 
      %>
      <input class="secondaryButton" type="button" value="Back to Manage Invoice" onclick="javascript:location.href = '<%:SessionUtil.InvoiceSearchCriteria%>';" />
      <%
        }
      %>
    </div>
  </div>
  <div class="clear">
  </div>
  <div id="BillingMemberReference" class="hidden">
    <%
      Html.RenderPartial("~/Views/Invoice/BillingMemberInfoControl.ascx", Model);%>
  </div>
  <div id="BilledMemberReference" class="hidden">
    <%
      Html.RenderPartial("~/Views/Invoice/BilledMemberInfoControl.ascx", Model);%>
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
  <div class="clear">
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
