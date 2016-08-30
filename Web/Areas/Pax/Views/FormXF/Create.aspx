<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.PaxInvoice>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> :: Create Sampling Form XF"
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Create Sampling Form XF
  </h1>
  <% using (Html.BeginForm("Create", "FormXF", FormMethod.Post, new { id = "SamplingRMForm", @class = "validCharacters" }))
     {%>
     <%: Html.AntiForgeryToken() %>
  <div>
    <%
      Html.RenderPartial("~/Areas/Pax/Views/BaseFXF/InvoiceHeaderControl.ascx");%>
  </div>
  <div class="buttonContainer">
    <input class="primaryButton ignoredirty" type="submit" value="Save Form XF Header" />
  </div>
  <%
    }%>
  <div id="BillingMemberReference" class="hidden">
    <%
      Html.RenderPartial("~/Views/Invoice/BillingMemberInfoControl.ascx", Model);%>
  </div>
  <div id="BilledMemberReference" class="hidden">
    <%
      Html.RenderPartial("~/Views/Invoice/BilledMemberInfoControl.ascx", Model);%>
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Pax/InvoiceHeader.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Pax/SamplingRMHeader.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/MemberReference.js")%>"></script>
  
  <script type="text/javascript">
     $(document).ready(function () {
     <%if(ViewData["IsLegalTextSet"] != null && Convert.ToBoolean(ViewData["IsLegalTextSet"]) == true)
    {%>
    isBillingLegalTextSet = true;
  <%
    }%>
      InitializeBilateralSMIs(<%: Convert.ToInt32(SMI.Bilateral) %>  ,'<%: ViewData[ViewDataConstants.BilateralSMIs] %>');
      InitialiseInvoiceHeader('<%: Url.Action("GetBilledMemberLocationList", "Data", new { area = ""})%>', '<%: Url.Action("GetExchangeRate", "Data", new { area = ""})%>', '', '<%: Url.Action("GetDefaultSettlementMethod", "Data", new { area = ""})%>', '<%:Url.Action("GetDefaultCurrency", "Data", new { area = "" })%>','<%:SessionUtil.MemberId%>');
      InitReferenceData('<%:Url.Action("GetMemberLocationDetails","Data",new{area = string.Empty})%>', '<%:Model.Id%>', '<%:Url.Action("GetSubdivisionNameList","Data",new{area = string.Empty})%>');
      InitializeCreateRMHeader(<%: (ViewData[ViewDataConstants.PageMode] == PageMode.Create).ToString().ToLower()%>);
      /*CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
        Ref: FRS Section 3.4 Table 15 Row 8 */
      registerAutocomplete('BilledMemberText', 'BilledMemberId', '<%:Url.Action("GetMemberListForPaxCgo", "Data", new { area = "" })%>', 0, true, function (selectedId) { BilledMemberText_AutoCompleteValueChange(selectedId); });

      InitializeLinking('<%:Url.Action("GetFormFSamplingConstant", "Data",  new { area = ""}) %>', '<%:SessionUtil.MemberId %>');

      if(<%: ViewData[ViewDataConstants.FromBillingHistory] != null ? ViewData[ViewDataConstants.FromBillingHistory].ToString().ToLower() : "false" %>)
      {
        BilledMemberText_AutoCompleteValueChange('<%: Model.BilledMemberId %>');
      }
    });
  </script>
</asp:Content>
