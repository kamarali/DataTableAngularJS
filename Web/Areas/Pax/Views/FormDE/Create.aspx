<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.PaxInvoice>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> :: Create Form D/E
</asp:Content>

<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="Script">
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Pax/InvoiceHeader.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Pax/FormDEHeader.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/MemberReference.js")%>"></script>
  <script type="text/javascript">
    $(document).ready(function () {
    <%if(ViewData["IsLegalTextSet"] != null && Convert.ToBoolean(ViewData["IsLegalTextSet"]) == true)
    {%>
    isBillingLegalTextSet = true;
  <%
    }%>
      InitializeBilateralSMIs(<%: Convert.ToInt32(SMI.Bilateral) %>  ,'<%: ViewData[ViewDataConstants.BilateralSMIs] %>');
      InitialiseInvoiceHeader('<%: Url.Action("GetBilledMemberLocationList", "Data", new { area = ""})%>', '<%: Url.Action("GetExchangeRate", "Data", new { area = ""})%>', '', '<%: Url.Action("GetDefaultSettlementMethod", "Data", new { area = ""})%>', '<%:Url.Action("GetDefaultCurrency", "Data", new { area = "" })%>', '<%:SessionUtil.MemberId%>');
      setFocusForFormDEHeader();
      InitReferenceData('<%:Url.Action("GetMemberLocationDetails","Data",new{area = string.Empty})%>', '<%:Model.Id%>', '<%:Url.Action("GetSubdivisionNameList","Data",new{area = string.Empty})%>');
      /*CMP #596: Length of Member Accounting Code to be Increased to 12 
          Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
          Ref: FRS Section 3.4 Table 15 Row 6 */
      registerAutocomplete('BilledMemberText', 'BilledMemberId', '<%:Url.Action("GetMemberListForPaxCgo", "Data", new { area = "" })%>', 0, true, function (selectedId) { BilledMemberText_AutoCompleteValueChange(selectedId); });
    });
  </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Create Form D/E</h1>
  <% using (Html.BeginForm("Create", "FormDE", FormMethod.Post, new { id = "InvoiceForm", @class = "validCharacters" }))
     {%>
     <%: Html.AntiForgeryToken() %>
  <div>
    <% Html.RenderPartial("SamplingInvoiceHeaderControl", Model); %>
  </div>
  <div class="buttonContainer">
    <input class="primaryButton ignoredirty" type="submit" value="Save Form D/E Header" />
    <%
      }%>
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
</asp:Content>
