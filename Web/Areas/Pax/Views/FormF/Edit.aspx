<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.PaxInvoice>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>

<asp:Content ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger ::
  <%: ViewData[ViewDataConstants.BillingType].ToString() %>
  ::
  <%:(ViewData[ViewDataConstants.PageMode].ToString())%>
  Sampling Form F
</asp:Content>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    <%:(ViewData[ViewDataConstants.PageMode].ToString())%>
    Sampling Form F
  </h1>
  <% using (Html.BeginForm("Edit", "FormF", FormMethod.Post, new { id = "SamplingRMForm", @class = "validCharacters" }))
     {%>
     <%: Html.AntiForgeryToken() %>
  <div>
    <%
       Html.RenderPartial("~/Areas/Pax/Views/BaseFXF/InvoiceHeaderControl.ascx");%>
  </div>
  <div class="buttonContainer">
    <div>
      <input class="primaryButton ignoredirty" type="submit" value="Save Form F Header" id="SaveHeader" />
    </div>
    <%
     }%>
  </div>
  <div>
    <% Html.RenderPartial("~/Areas/Pax/Views/BaseFXF/InvoiceTotalControl.ascx", Model.InvoiceTotalRecord); %>
  </div>
  <h2>
    Summary List</h2>
  <div id="sourceCodeDetailsDiv">
    <% Html.RenderPartial("~/Areas/Pax/Views/BaseFXF/SummaryGridControl.ascx", ViewData[ViewDataConstants.FormFSummaryListGrid]); %>
  </div>
  <h2>
    Form F Rejection Memo List</h2>
  <div>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.RejectionMemoListGrid]); %>
   <%
    if (ViewData[ViewDataConstants.BillingType] != null && ViewData[ViewDataConstants.BillingType].ToString() == Iata.IS.Web.Util.BillingType.Payables)
    {%>
  <%:ScriptHelper.GeneratePaxPayablesGridViewRejectScript(Url, ControlIdConstants.RejectionMemoGridId, Url.Action("RMView"), "RM", Model.Id.Value(), Url.Action("InitiateRejection", "FormF", new { area = "Pax" }), Url.Action("InitiateDuplicateRejection", "FormF", new { area = "Pax" }), Model.BillingCode, Model.BillingYear, Model.BillingMonth, Model.BillingPeriod, Model.SettlementMethodId)%>
  <%} %>
  <%else if (ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
    {%>
  <%:ScriptHelper.GenerateGridViewScript(Url, ControlIdConstants.RejectionMemoGridId, Url.Action("RMView"))%>
  <%}
    else
    {%>    
  <%:ScriptHelper.GenerateGridEditDeleteScript(Url, ControlIdConstants.RejectionMemoGridId, Url.Action("RMEdit", "FormF"), Url.Action("RMDelete", "FormF"))%>
  <%}%>
  </div>
  <div class="buttonContainer">
    <%
      if ((Model.InvoiceStatus == InvoiceStatusType.Open || Model.InvoiceStatus == InvoiceStatusType.ValidationError) && Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Pax.Receivables.SampleFormF.Validate))
      {
        using (Html.BeginForm("ValidateInvoice", "FormF", new { invoiceId = Model.Id.Value() }, FormMethod.Post))
        {
    %>
    <%: Html.AntiForgeryToken() %>
    <input class="primaryButton" type="submit" value="Validate Invoice" />
    <%
        }
      }
      if (Model.InvoiceStatus == InvoiceStatusType.ReadyForSubmission && Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Pax.Receivables.SampleFormF.Submit))
      {
        using (Html.BeginForm("Submit", "FormF", FormMethod.Post))
        {%>
        <%: Html.AntiForgeryToken() %>
    <input class="primaryButton" type="submit" value="Submit Invoice" />
    <%
        }
      }%>
    <% if ((string)ViewData[ViewDataConstants.PageMode] != PageMode.View)
       {
         using (Html.BeginForm("RMCreate", "FormF", FormMethod.Get))
         {%>
         
    <input type="submit" value="Add Rejection Memo" class="primaryButton" />
    <%
         }%>
    <%
       }%>
    <div id="divTransactions">
      <% using (Html.BeginForm(((ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View) ? "VatView" : "Vat"), "FormF", new { invoiceId = Model.Id.Value() }, FormMethod.Get))
         { %>
      <input class="secondaryButton" type="submit" value="Invoice VAT" />
      <% } %>
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
<asp:Content ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">
    $(document).ready(function () {
    <%if(ViewData["IsLegalTextSet"] != null && Convert.ToBoolean(ViewData["IsLegalTextSet"]) == true)
    {%>
    isBillingLegalTextSet = true;
  <%
    }%>
    initializeParentForm('SamplingRMForm');     
      var isViewMode = <%:ViewData[ViewDataConstants.PageMode] !=null ?(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View).ToString().ToLower() : "false"%>;
      InitializeBilateralSMIs(<%: Convert.ToInt32(SMI.Bilateral) %>  ,'<%: ViewData[ViewDataConstants.BilateralSMIs] %>');
      InitialiseInvoiceHeader('<%: Url.Action("GetBilledMemberLocationList", "Data", new { area = ""})%>', '<%: Url.Action("GetExchangeRate", "Data", new { area = ""})%>', <%:(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View).ToString().ToLower()%>, '<%: Url.Action("GetDefaultSettlementMethod", "Data", new { area = ""})%>', '<%:Url.Action("GetDefaultCurrency", "Data", new { area = "" })%>', '<%:SessionUtil.MemberId%>', '#SamplingRMForm,#BillingMemberReference,#BilledMemberReference');
      if(isViewMode == false)
        InitReferenceData('<%:Url.Action("GetMemberLocationDetails","Data",new{area = string.Empty})%>', '<%:Model.Id%>','<%:Url.Action("GetSubdivisionNameList","Data",new{area = string.Empty})%>');
      InitializeRMEditHeader('<%: ViewData[ViewDataConstants.IsSubmittedStatus]%>', '<%: ViewData[ViewDataConstants.TransactionExists]%>');
      /*CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
        Ref: FRS Section 3.4 Table 15 Row 7 */
      registerAutocomplete('BilledMemberText', 'BilledMemberId', '<%:Url.Action("GetMemberListForPaxCgo", "Data", new { area = "" })%>', 0, true, function (selectedId) { BilledMemberText_AutoCompleteValueChange(selectedId); });
      InitializeLinking('<%:Url.Action("GetFormDESamplingConstant", "Data", new { area = ""}) %>', '<%:Model.BillingMemberId %>');
    });  
  </script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Pax/InvoiceHeader.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Pax/SamplingRMHeader.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/MemberReference.js")%>"></script>
  <script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Pax/Billinghistory.js")%>"></script>
</asp:Content>
