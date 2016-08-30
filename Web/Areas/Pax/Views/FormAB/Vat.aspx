<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.PaxInvoice>" %>

<%@ Import Namespace="Iata.IS.Model.Pax" %>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Pax/InvoiceVat.js")%>"></script>
  <![if IE 7]>
  <script type="text/javascript" src="<%: Url.Content("~/Scripts/json2.js")%>"></script>
  <![endif]>
  <script type="text/javascript">
    $(document).ready(function () {
      InitializeViewOnlyInvoiceVat(<%:(ViewData[ViewDataConstants.PageMode] == PageMode.View).ToString().ToLower()%>)
    });
    </script>
    <%: ScriptHelper.GenerateGridDeleteScript(Url, ControlIdConstants.AvailableVatGridId, "VatDelete")%>
    <%: ScriptHelper.GenerateGridDeleteScript(Url, ControlIdConstants.InvoiceVatGridId, "VatDelete")%>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger :: Receivables :: Non-Sampling Invoice :: Invoice VAT
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Invoice VAT
  </h1>
  <div>
    <%
      Html.RenderPartial("ReadOnlyInvoiceHeaderControl", Model);%>
  </div>
  <div>
    <%
        Html.RenderPartial("InvoiceVatDetailsControl", new InvoiceVat() { ParentId = Model.Id });%>
  </div>
  <h2>
    VAT List</h2>
  <div>
    <%
        Html.RenderPartial("GridControl", ViewData[ViewDataConstants.InvoiceVatGrid]);%>
  </div>
  <% if (ViewData[ViewDataConstants.BillingType].ToString() == BillingType.Receivables) {%>
  <div>
    <h2>
      Derived VAT List</h2>
    <!-- TODO : Needs to change  AvailableVat List display functionality -->
    <%
       Html.RenderPartial("GridControl", ViewData[ViewDataConstants.AvailableVatGrid]);%>
  </div>
  <%}%>
  <div class="buttonContainer">
    <input class="primaryButton" type="button" value="Add to Invoice VAT" onclick="copyAvailableVat()" />
  </div>
  <% if (ViewData[ViewDataConstants.BillingType].ToString() == BillingType.Receivables) {%>
  <div>
    <h2>
      Amounts on which VAT Has Not Been Applied</h2>
    <!-- TODO : Functionality to display Amount in which Vat is not applied -->
    <%
       Html.RenderPartial("GridControl", ViewData[ViewDataConstants.UnappliedAmountVatGrid]);%>
  </div>
  <%}%>
  <div class="buttonContainer">   
      <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("View", "FormAB", new { invoiceId = Model.Id.Value() })%>';" />
  </div>
</asp:Content>
