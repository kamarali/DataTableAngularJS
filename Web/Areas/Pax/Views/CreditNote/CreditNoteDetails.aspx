<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.PaxInvoice>" %>
<%@ Import Namespace="Iata.IS.Model.Pax.Enums" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>

<asp:Content ID="titleBlock" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> :: Credit Note Details
</asp:Content>

<asp:Content ID="contentBlock" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Edit Non-Sampling Credit Note</h1>
  <div>
    <% Html.RenderPartial(Url.Content("ReadOnlyCreditNoteHeaderControl"), Model); %>
  </div>
  <div>
    <% Html.RenderPartial("InvoiceTotalControl", Model.InvoiceTotalRecord); %>
  </div>
  <div class="buttonContainer" id="SubElementsDiv">
    <h2>
      Actions on this Credit Note</h2>
    <div>
      <% using (Html.BeginForm("CreditMemoList", "CreditNote", new { invoiceNumber = Model.Id.Value() }, FormMethod.Get))
         { %>
      <input class="secondaryButton" type="submit" value="Credit Memos" />
      <% } %>
    </div>
  </div>
  <div class="clear">
  </div>
  <h2>Source Code List</h2>
  <div id="sourceCodeDetailsDiv">
      <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.SourceCodeGrid]); %>
  </div>
  <div class="horizontalFlow">
    <h2>
      Submitted Errors</h2>
  </div>
  <div class="buttonContainer">
        <%
      if (Model.InvoiceStatus == InvoiceStatusType.Open)
        // TODO: Need to figure out which invoice status should be check here
        // || Model.InvoiceStatus == InvoiceStatus.ReadyForValidation)
      {
        using (Html.BeginForm("ValidateInvoice", "CreditNote", FormMethod.Post))
        {%>
        <%: Html.AntiForgeryToken() %>
      <input class="primaryButton" type="submit" value="Validate Invoice"/>
          <%
        }
      }
      if (Model.InvoiceStatus == InvoiceStatusType.ReadyForSubmission)
      {
        using (Html.BeginForm("Submit", "CreditNote", FormMethod.Post))
        {
%>
        <%: Html.AntiForgeryToken() %>
            <input class="primaryButton" type="submit" value="Submit Invoice"/>
            <%
        }
      }
%>
      <%: Html.LinkButton("Back", Url.Action("Edit", "CreditNote", new { invoiceId = Model.Id.Value() }))%>
  </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script type="text/javascript" src="<%:Url.Content("~/Scripts/Pax/ValidateInvoice.js")%>"></script>
</asp:Content>
