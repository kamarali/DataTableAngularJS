<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.SearchCriteria>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger :: Receivables :: Invoice List
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Invoice Search</h1>
  <h2>
    Search Criteria</h2>
  <div>
    <%
      using (Html.BeginForm("InvoiceSearch", "Invoice", FormMethod.Post, new { style = "display:inline", id = "InvoiceForm" }))
      {
        Html.RenderPartial(Url.Content("~/Areas/Pax/Views/Shared/InvoiceSearchControl.ascx"), Model);
      } 
    %>
  </div>
  <h2>  
  <div>
    <% Html.RenderPartial("InvoiceSearchGridControl", ViewData["SearchResult"]); %>
  </div>
  <div class="buttonContainer">
    <input class="primaryButton" type="button" value="Submit Selected Invoices" />
  </div>
</asp:Content>
