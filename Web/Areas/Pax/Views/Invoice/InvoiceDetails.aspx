<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.PaxInvoice>" %>
<%@ Import Namespace="Iata.IS.Model.Pax.Enums" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> :: Invoice Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    <%:ViewData[ViewDataConstants.PageMode] %> Non-Sampling Invoice</h1>
  <div>
    <%
      Html.RenderPartial("ReadOnlyInvoiceHeaderControl", Model);%>
  </div>
  <div>
    <%
      Html.RenderPartial("InvoiceTotalControl", Model.InvoiceTotalRecord);%>
  </div>
  <h2>
    Actions on this Invoice</h2>
  <div class="buttonContainer">
      <%
      using (Html.BeginForm("PrimeBillingList", "Invoice", FormMethod.Post))
      {%>
      <input class="secondaryButton" type="submit" value="Prime Billing" />
      <%
      }%>
      <%
      using (Html.BeginForm("RMList", "Invoice", FormMethod.Post))
      {%>
      <input class="secondaryButton" type="submit" value="Rejection Memos" />
      <%
      }%>
      <%
      using (Html.BeginForm("BMList", "Invoice", FormMethod.Post))
      {%>
      <input class="secondaryButton" type="submit" value="Billing Memos" />
      <%
      }%>
   </div>
  <div class="clear">
  </div>
  <h2>Source Code List</h2>
  <div id="sourceCodeDetailsDiv">
    <div>
    <%
      Html.RenderPartial("GridControl", ViewData[ViewDataConstants.SourceCodeGrid]);%>
  </div>
  </div>
  <div class="horizontalFlow">
    <h2>
      Submitted Errors</h2>
  </div>
  <div class="buttonContainer">
        <%
          // TODO: Need to figure out which invoice status should be check here
      if (Model.InvoiceStatus == InvoiceStatusType.Open) // || Model.InvoiceStatus == InvoiceStatus.ReadyForValidation)
      {
        using (Html.BeginForm("ValidateInvoice", "Invoice", FormMethod.Post))
        {%>
        <%: Html.AntiForgeryToken() %>
      <input class="primaryButton" type="submit" value="Validate Invoice"/>
          <%
        }
      }
      if (Model.InvoiceStatus == InvoiceStatusType.ReadyForSubmission)
      {
        using (Html.BeginForm("Submit", "Invoice", FormMethod.Post))
        {
%>
        <%: Html.AntiForgeryToken() %>
            <input class="primaryButton" type="submit" value="Submit Invoice"/>
            <%
        }
      }
%>
<%
  if (ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
  {
%>
      <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("View", "Invoice", new
                                                        {
                                                          invoiceId = Model.Id.Value()
                                                        })%>';"  />
                                                      <%
  }
  else
  {
%>
<%: Html.LinkButton("Back", Url.Action("Edit", "Invoice", new { invoiceId = Model.Id.Value() }))%>
<%
  }
%>
  </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script type="text/javascript" src="<%:Url.Content("~/Scripts/ValidateInvoice.js")%>"></script>
</asp:Content>
