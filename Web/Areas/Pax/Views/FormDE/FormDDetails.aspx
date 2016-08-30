<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.PaxInvoice>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> :: Form D Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Form D Details</h1>
 <div>
     <% Html.RenderPartial(Url.Content("SamplingInvoiceHeaderInfoControl"), Model); %>
  </div>
  <div class="clear">
  </div>
  <div id="sourceCodeDetailsDiv">
  <h2>Summary List</h2>
  <% Html.RenderPartial("GridControl", ViewData[ControlIdConstants.FormDSourceCodeGridId]); %>
  </div>
  <div class="buttonContainer">
    <%
      using (Html.BeginForm("FormDList", "FormDE", FormMethod.Get))
      {
        if (ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
        {
    %>
    <input class="secondaryButton" type="submit" value="View Form D List" id="ViewDList" />
    <%
        }
        else
        {
    %>
    <input class="secondaryButton" type="submit" value="Add / Edit Form D List" id="EditDlist" />
    <%
        }
      }
%>
    <%: Html.LinkButton("Back", Url.Action("Edit", "FormDE", new { invoiceId = Model.Id.Value() }))%>
  </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script type="text/javascript" src="<%:Url.Content("~/Scripts/Pax/ValidateInvoice.js")%>"></script>
</asp:Content>
