<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.SearchCriteria>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger :: Payables :: Manage Invoice Sampling Form C
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Script" runat="server">
  <script src="<%:Url.Content("~/Scripts/Pax/FormCSearch.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/downloadZip.js")%>" type="text/javascript"></script>
  <script type="text/javascript">
  $(document).ready(function () {
    registerAutocomplete('BilledMemberText', 'BilledMemberId', '<%:Url.Action("GetMemberList", "Data", new { area = "" })%>', 0, true, null);
  });
  </script>
  <%: ScriptHelper.GenerateFormCPayablesSearchScript(Url, ControlIdConstants.FormCSearchGrid, Url.RouteUrl("Pax_default", new { action = "ViewDetails", controller = "FormC" }), Url.Action("DownloadZip", "FormC", new{ area = "Pax", isReceivable = false }))%>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Sampling Form C Search</h1>
    <% using (Html.BeginForm("PayablesSearch", "FormC", FormMethod.Get, new { id = "FormCSearchForm" }))
       { %>
  <div>
    <% Html.RenderPartial("FormCPayablesSearchControl", Model); %>
  </div>
  <%} %>
  <div>
    <h2>
      Search Results</h2>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.FormCSearchResults]); %>
  </div>
   <div id="InvoiceDownloadOptions" class="hidden">
    <% Html.RenderPartial("~/Views/Shared/InvoiceDownloadOptionsControl.ascx", Iata.IS.Model.Enums.InvoiceDownloadOptions.FormCPayables);%>
  </div>
</asp:Content>
