<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.PaxInvoice>" MasterPageFile="~/Views/Shared/Site.Master" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>

<asp:Content runat="server" ID="Content" ContentPlaceHolderID="TitleContent">
  SIS :: Passenger :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> :: Create Non-Sampling Invoice 
</asp:Content>

<asp:Content runat="server" ID="Content1" ContentPlaceHolderID="Script">
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Pax/InvoiceHeader.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/MemberReference.js")%>"></script>
  <script type="text/javascript">
    $(document).ready(function () {
    <%if(ViewData["IsLegalTextSet"] != null && Convert.ToBoolean(ViewData["IsLegalTextSet"]) == true)
    {%>
    isBillingLegalTextSet = true;
  <%
    }%>
    InitializeBilateralSMIs(<%: Convert.ToInt32(SMI.Bilateral) %>  ,'<%: ViewData[ViewDataConstants.BilateralSMIs] %>');
      InitialiseInvoiceHeader('<%: Url.Action("GetBilledMemberLocationList", "Data", new { area = ""})%>', '<%: Url.Action("GetExchangeRate", "Data", new { area = ""})%>', '', '<%: Url.Action("GetDefaultSettlementMethod", "Data", new { area = ""})%>', '<%:Url.Action("GetDefaultCurrency", "Data", new { area = "" })%>', '<%:SessionUtil.MemberId%>', '#InvoiceForm', '<%:Url.Action("IsBillingAndBilledAchOrDualMember", "Data", new { area = "" })%>', '<%: Model.BillingCurrencyId %>');
     // setInvoiceHeaderFocus();
      /*CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
        Ref: FRS Section 3.4 Table 15 Row 2 */
      registerAutocomplete('BilledMemberText', 'BilledMemberId', '<%:Url.Action("GetMemberListForPaxCgo", "Data", new { area = "" })%>', 0, true, function (selectedId) { BilledMemberText_AutoCompleteValueChange(selectedId); });
      InitReferenceData('<%:Url.Action("GetMemberLocationDetails","Data",new{area = string.Empty})%>', '<%:Model.Id%>', '<%:Url.Action("GetSubdivisionNameList","Data",new{area = string.Empty})%>');
    
      // CMP #624: ICH Rewrite-New SMI X 
      // Description: Refer FRS Section 2.14 Change #9
      if(<%: ViewData[ViewDataConstants.FromBillingHistory] != null ? ViewData[ViewDataConstants.FromBillingHistory].ToString().ToLower() : "false" %>)
      {
        //SCP#450827 - Rejections Process
        //Set focus on Invoice Number if Invoice created from Billing History Screen.
        setInvoiceHeaderFocus("True");
        //alert('<%: Model.SettlementMethodId %>')
        <% if( Model.SettlementMethodId != 8){%>
        //alert(' from if ')
        BilledMemberText_AutoCompleteValueChange('<%: Model.BilledMemberId %>', 0);
         <% } else {%> //alert(' from else ')
        BilledMemberText_AutoCompleteValueChange('<%: Model.BilledMemberId %>', 8);<%}%>
        
      }
      else{
        //SCP#450827 - Rejections Process
        //Set focus on Billed Member if Invoice created from Recievable Screen.
        setInvoiceHeaderFocus("False");
      }

    });
  </script>
</asp:Content>

<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="MainContent">
  <h1>
    Create Non-Sampling Invoice</h1>
  <%
    using (Html.BeginForm("Create", "Invoice", FormMethod.Post, new { id = "InvoiceForm", @class = "validCharacters" }))
    {%>
    <%: Html.AntiForgeryToken() %>
  <div>
    <%
      Html.RenderPartial("InvoiceHeaderControl");%>
  </div>
  <div class="buttonContainer">
    <input class="primaryButton ignoredirty" type="submit" value="Save Invoice Header"/>
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
