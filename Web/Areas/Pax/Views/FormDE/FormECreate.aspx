<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.Sampling.SamplingFormEDetail>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> :: Create Form E
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Create Form E</h1>
  <div>
    <% Html.RenderPartial("SamplingInvoiceHeaderInfoControl", Model.Invoice); %>
  </div>
  <% using (Html.BeginForm("FormECreate", "FormDE", FormMethod.Post, new { id = "formE", @class = "validCharacters" }))
     { %>
     <%: Html.AntiForgeryToken() %>
  <div>
    <% Html.RenderPartial("FormEControl", Model); %>
  </div>
  <div class="buttonContainer">
    <input class="primaryButton ignoredirty" type="submit" value="Save" id="Save" />
    <%: Html.LinkButton("Back", Url.Action("Edit", "FormDE", new { invoiceId = Model.Invoice.Id }))%>
    <%} %>
  </div>
  <div class="clear">
  </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script src="<%:Url.Content("~/Scripts/Pax/SamplingFormE.js")%>" type="text/javascript"></script>
</asp:Content>
