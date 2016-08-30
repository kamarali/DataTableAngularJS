<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MiscUatp.MiscSearchCriteria>" %>
<%@ Import Namespace="System.Security.Policy" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
<script src="<%:Url.Content("~/Scripts/validateRecord.js")%>" type="text/javascript"></script>
<script src="<%:Url.Content("~/Scripts/Uatp/UatpInvoiceSearch.js")%>" type="text/javascript"></script>
<script src="<%:Url.Content("~/Scripts/downloadZip.js")%>" type="text/javascript"></script>

 
<script type="text/javascript" >
    registerAutocomplete('BilledMemberText', 'BilledMemberId', '<%:Url.Action("GetMemberList", "Data", new { area = "" })%>', 0, true, null);
    clearSearchUrl = '<%: Url.Action("ClearSearch", "ManageUatpInvoice") %>';
  $(document).ready(function () {

    $("#UatpInvoiceSearchForm").validate({
      rules: {
        BillingYearMonth: "required"
      },
      messages: {
        BillingYearMonth: "Billing Year / Month required"
      }
    });

  });

</script> 
<%--SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions --%>
<%: ScriptHelper.GenerateScriptForUatpRecManage(Url, ControlIdConstants.SearchGrid,
                                                Url.Action("ViewInvoice", "ManageUatpInvoice"),
                                                Url.RouteUrl("UatpInvoiceSearch", new { controller = "ManageUatpInvoice", action = "DownloadPdf" }),
                                                Url.RouteUrl("UatpInvoiceSearch", new { controller = "ManageUatpInvoice", action = "DownloadZip" }))%>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	SIS :: UATP :: Search Invoice
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>Invoice Search</h1>
  <h2>Search Criteria</h2>
  <div>
    <%
      using (Html.BeginForm("Index", "ManageUatpInvoice", FormMethod.Get, new { id = "UatpInvoiceSearchForm" }))
      {
        Html.RenderPartial("~/Views/MiscUatp/UatpInvoiceSearchControl.ascx", Model); %>
        <div class="buttonContainer">
    <input class="primaryButton" type="submit" value="Search" />
    <input class="secondaryButton" type="button" onclick="javascript:ResetSearch('#UatpInvoiceSearchForm');" value="Clear" />  
      <%} 
    %>
    
</div>
  </div>
  <h2>
    Search Results</h2>
  <div>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.SearchGrid]); %>
  </div>
  <div class="buttonContainer">
    <input class="primaryButton" type="button" value="Submit Selected Invoices" onclick="submitInvoices('#<%:ControlIdConstants.SearchGrid %>','<%:Url.Action("SubmitInvoices","ManageUatpInvoice") %>');" />
  </div>
  <div id="InvoiceDownloadOptions" class="hidden">
    <%--<% Html.RenderPartial("~/Views/Shared/InvoiceDownloadOptionsControl.ascx", Model);%>--%>
    <% Html.RenderPartial("~/Views/Shared/InvoiceDownloadOptionsControl.ascx", Iata.IS.Model.Enums.InvoiceDownloadOptions.UatpReceivables);%> 
  </div>
</asp:Content>