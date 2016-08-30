<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.PaxInvoice>" %>
<%@ Import Namespace="Iata.IS.Model.Pax.Enums" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>

<asp:Content ID = "Content1" ContentPlaceHolderID = "TitleContent" runat = "server">
  SIS :: Passenger :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> :: Sampling Form XF Details
</asp:Content>

<asp:Content ID = "Content2" ContentPlaceHolderID = "MainContent" runat = "server">
  <h1>
    Sampling Form XF Details
  </h1>
  <div>
    <% Html.RenderPartial(Url.Content("~/Areas/Pax/Views/Shared/SamplingInvoiceHeaderInfoControl.ascx"), Model); %>
  </div>
  <div>
    <% Html.RenderPartial("~/Areas/Pax/Views/BaseFXF/InvoiceTotalControl.ascx", Model.InvoiceTotalRecord); %>
  </div>
  <div class="buttonContainer"> 
    <div>
      <% using (Html.BeginForm("RMList", "FormXF", FormMethod.Get))
         {%>

      <input class="secondaryButton" type="submit" value="Rejection Memo" />
      <%} %>
    </div>
  </div>
  <div class="clear">
  </div>
  <h2>Summary List</h2>
  <div id="sourceCodeDetailsDiv">
    <% Html.RenderPartial("~/Areas/Pax/Views/BaseFXF/SummaryGridControl.ascx", ViewData[ViewDataConstants.FormFSummaryListGrid]); %>
  </div>  
  <div class="buttonContainer">
    <%
      // TODO: Need to figure out which invoice status should be check here
      if (Model.InvoiceStatus == InvoiceStatusType.Open || Model.InvoiceStatus == InvoiceStatusType.ValidationError) // || Model.InvoiceStatus == InvoiceStatus.ReadyForValidation || Model.InvoiceStatus == InvoiceStatus.PendingForCorrections)
    {
        using (Html.BeginForm("ValidateInvoice", "FormXF", new { invoiceId = Model.Id.Value() }, FormMethod.Post))
      {
        %>
        <%: Html.AntiForgeryToken() %>
        <input class="primaryButton" type="submit" value="Validate Invoice" />
        <%: Html.LinkButton("Back", Url.Action("Edit", "FormXF", new { invoiceId = Model.Id.Value() }))%>
        <%
      }
    }
    else if(Model.InvoiceStatus == InvoiceStatusType.ReadyForSubmission)
    {
        using (Html.BeginForm("Submit", "FormXF", FormMethod.Post))
      {
      %>
        <%: Html.AntiForgeryToken() %>
        <input class="primaryButton" type="submit" value="Submit Invoice" />
        <%: Html.LinkButton("Back", Url.Action("Edit", "FormXF", new { invoiceId = Model.Id.Value() }))%>
      <%
      }
    }
    else
    {
    %>   
      <%: Html.LinkButton("Back", Url.Action("Edit", "FormXF", new { invoiceId = Model.Id.Value() }))%>
  <%}%>
  </div>  
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Pax/ValidateInvoice.js")%>"></script>
</asp:Content>
