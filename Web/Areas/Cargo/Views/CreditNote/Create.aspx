<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Cargo.CargoInvoice>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>

<asp:Content ID="titleBlock" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Cargo :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> :: <%:ViewData[ViewDataConstants.PageMode] %> Credit Note
</asp:Content>

<asp:Content ID="scriptBlock" ContentPlaceHolderID="Script" runat="server">
<script type="text/javascript" src="<%:Url.Content("~/Scripts/Cargo/InvoiceHeader.js")%>"></script>
<script type="text/javascript" src="<%:Url.Content("~/Scripts/MemberReference.js")%>"></script>
<script type="text/javascript">
  $(document).ready(function () {
   $("#AwbSerialNumberCheckDigit").numeric();
  <%if(ViewData["IsLegalTextSet"] != null && Convert.ToBoolean(ViewData["IsLegalTextSet"]) == true)
    {%>
    isBillingLegalTextSet = true;
  <%
    }%>
    SetInvoiceType(<%:(Model.InvoiceType == InvoiceType.CreditNote ? 1 : 0)%>);
    InitializeBilateralSMIs(<%: Convert.ToInt32(SMI.Bilateral) %>  ,'<%: ViewData[ViewDataConstants.BilateralSMIs] %>');
    InitialiseInvoiceHeader('<%: Url.Action("GetBilledMemberLocationList", "Data", new { area = ""})%>', '<%: Url.Action("GetExchangeRate", "Data", new { area = ""})%>', '', '<%: Url.Action("GetDefaultSettlementMethod", "Data", new { area = ""})%>', '<%:Url.Action("GetDefaultCurrency", "Data", new { area = "" })%>', '<%:SessionUtil.MemberId%>', '#InvoiceForm', '<%:Url.Action("IsBillingAndBilledAchOrDualMember", "Data", new { area = "" })%>','<%: Model.BillingCurrencyId %>');
    setInvoiceHeaderFocus();
    /*CMP #596: Length of Member Accounting Code to be Increased to 12 
      Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
      Ref: FRS Section 3.4 Table 15 Row 22 */
    registerAutocomplete('BilledMemberText', 'BilledMemberId', '<%:Url.Action("GetMemberListForPaxCgo", "Data", new { area = "" })%>', 0, true, function (selectedId) { BilledMemberText_AutoCompleteValueChange(selectedId); });
    InitReferenceData('<%:Url.Action("GetMemberLocationDetails","Data",new{area = string.Empty})%>', '<%:Model.Id%>','<%:Url.Action("GetSubdivisionNameList","Data",new{area = string.Empty})%>');
  });
</script>
</asp:Content>

<asp:Content ID="contentBlock" ContentPlaceHolderID="MainContent" runat="server">
  <h1>Create Cargo Credit Note</h1>
  <% using (Html.BeginForm("Create", "CreditNote", FormMethod.Post, new { id = "InvoiceForm", @class = "validCharacters" }))
     {  %>
     <%: Html.AntiForgeryToken() %>
  <div>
    <% Html.RenderPartial("InvoiceHeaderControl", Model); %>
  </div>
  <div class="buttonContainer">
    <input class="primaryButton ignoredirty" type="submit" value="Save Credit Note Header"  />
  </div>
  <%}%>
  <div id="BillingMemberReference" class="hidden">
	<%
    Html.RenderPartial("~/Views/Invoice/BillingMemberInfoControl.ascx", Model);%>
</div>

<div id="BilledMemberReference" class="hidden">
	<%
    Html.RenderPartial("~/Views/Invoice/BilledMemberInfoControl.ascx", Model);%>
</div>
</asp:Content>
