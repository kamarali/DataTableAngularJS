<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.PaxInvoice>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger ::
  <%: ViewData[ViewDataConstants.BillingType].ToString() %>
  ::
  <%:(ViewData[ViewDataConstants.PageMode].ToString())%>
  Sampling Form XF
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    <%:(ViewData[ViewDataConstants.PageMode].ToString())%>
    Sampling Form XF
  </h1>
  <% using (Html.BeginForm("Edit", "FormXF", FormMethod.Post, new { id = "SamplingRMForm", @class = "validCharacters" }))
     {%>
     <%: Html.AntiForgeryToken() %>
  <div>
    <%
       Html.RenderPartial("~/Areas/Pax/Views/BaseFXF/InvoiceHeaderControl.ascx");%>
  </div>
  <div class="buttonContainer">
    <div>
      <input class="primaryButton ignoredirty" type="submit" value="Save Form XF Header" id="SaveHeader" />
    </div>
    <%
     }%>
  </div>
  <div id="divInvoiceTotalControl">
    <% Html.RenderPartial("~/Areas/Pax/Views/BaseFXF/InvoiceTotalControl.ascx", Model.InvoiceTotalRecord); %>
  </div>
  <h2>
    Summary List</h2>
  <div id="sourceCodeDetailsDiv">
    <% Html.RenderPartial("~/Areas/Pax/Views/BaseFXF/SummaryGridControl.ascx", ViewData[ViewDataConstants.FormFSummaryListGrid]); %>
  </div>
  <h2>
    Form XF Rejection Memo List
  </h2>
  <div>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.RejectionMemoListGrid]); %>
    <% if (ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
       {%>
    <%: ScriptHelper.GenerateGridViewScript(Url, ControlIdConstants.RejectionMemoGridId, Url.Action("RMView"))%>
    <%}
       else
       {  %>
    <%: ScriptHelper.GenerateGridEditDeleteScript(Url, ControlIdConstants.RejectionMemoGridId, Url.Action("RMEdit", "FormXF"), Url.Action("RMDelete", "FormXF"))%>
    <%} %>
  </div>
  <div class="buttonContainer">
    <%
      if ((Model.InvoiceStatus == InvoiceStatusType.Open || Model.InvoiceStatus == InvoiceStatusType.ValidationError) && Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Pax.Receivables.SampleFormXF.Validate))
      {
        using (Html.BeginForm("ValidateInvoice", "FormXF", new { invoiceId = Model.Id.Value() }, FormMethod.Post))
        {
    %>
    <%: Html.AntiForgeryToken() %>
    <input class="primaryButton" type="submit" value="Validate Invoice" />
    <%
        }
      }
      else if (Model.InvoiceStatus == InvoiceStatusType.ReadyForSubmission && Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Pax.Receivables.SampleFormXF.Submit))
      {
        using (Html.BeginForm("Submit", "FormXF", FormMethod.Post))
        {
    %>
    <%: Html.AntiForgeryToken() %>
    <input class="primaryButton" type="submit" value="Submit Invoice" />
    <%
          }
        } %>
    <% if ((string)ViewData[ViewDataConstants.PageMode] != PageMode.View)
       {
         using (Html.BeginForm("RMCreate", "FormXF", FormMethod.Get))
         {%>
    <input type="submit" value="Add Rejection Memo" class="primaryButton" />
    <%
         }%>
    <%
       }%>
    <div id="divTransactions">
      <% using (Html.BeginForm(((ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View) ? "VatView" : "Vat"), "FormXFPayables", FormMethod.Get))
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
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
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
      InitialiseInvoiceHeader('<%: Url.Action("GetBilledMemberLocationList", "Data", new { area = ""})%>', '<%: Url.Action("GetExchangeRate", "Data", new { area = ""})%>', <%:(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View).ToString().ToLower()%>, '<%: Url.Action("GetDefaultSettlementMethod", "Data", new { area = ""})%>', '<%:Url.Action("GetDefaultCurrency", "Data", new { area = "" })%>', '<%:SessionUtil.MemberId%>', '#SamplingRMForm,#divInvoiceTotalControl,#BillingMemberReference,#BilledMemberReference');
      if(isViewMode == false)
        InitReferenceData('<%:Url.Action("GetMemberLocationDetails","Data",new{area = string.Empty})%>', '<%:Model.Id%>','<%:Url.Action("GetSubdivisionNameList","Data",new{area = string.Empty})%>');
      InitializeRMEditHeader('<%: ViewData[ViewDataConstants.IsSubmittedStatus]%>', '<%: ViewData[ViewDataConstants.TransactionExists]%>');
      registerAutocomplete('BilledMemberText', 'BilledMemberId', '<%:Url.Action("GetMemberList", "Data", new { area = "" })%>', 0, true, function (selectedId) { BilledMemberText_AutoCompleteValueChange(selectedId); });
    });  
  </script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Pax/InvoiceHeader.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Pax/SamplingRMHeader.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/MemberReference.js")%>"></script>
  <script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
</asp:Content>
