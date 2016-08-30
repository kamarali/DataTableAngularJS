<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.Sampling.ProvisionalInvoiceRecordDetail>" %>

<%@ Import Namespace="Iata.IS.Model.Pax.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> :: Provisional Invoice Records
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Script" runat="server">
  
  <script type="text/javascript" src="<%: Url.Content("~/Scripts/deleterecord.js")%>"></script>
  <![if IE 7]>
  <script type="text/javascript" src="<%: Url.Content("~/Scripts/json2.js")%>"></script>
  <![endif]>
  <% if (ViewData[ViewDataConstants.PageMode].ToString() != PageMode.View)
     {
  %>
  <%:ScriptHelper.GenerateGridDeleteScript(Url, ControlIdConstants.ProvisionalInvoiceGridId, "ProvisionalInvoiceDelete")%>
  <%
    }
     else
     {
  %>
  <script type="text/javascript">
    function ProvisionalInvoiceGrid_DeleteRecord(cellValue, options, rowObject) {
      return '';
    }
  </script>
  <%
    }
  %>
  <script type="text/javascript">
      $(document).ready(function () {   

        <%
        if(ViewData[ViewDataConstants.NotBilateralSettlementMethod] != null && Convert.ToBoolean(ViewData[ViewDataConstants.NotBilateralSettlementMethod]))
        { %>
        SetAutoPopulatedExchangeRateDetails(<%: Model.Invoice.BillingCurrencyId %>, <%: Model.Invoice.ProvisionalBillingYear %>, <%: Model.Invoice.ProvisionalBillingMonth %>, '<%: Url.Action("GetExchangeRate","Data", new { area = "" }) %>');
        <% } %>
        SetPageToViewMode(<%: ((string)ViewData[ViewDataConstants.PageMode] == PageMode.View).ToString().ToLower()%>);
        setGlobalVariables('<%: Model.Invoice.InvoiceSmi %>','<%: ViewData[ViewDataConstants.BilateralSMIs] %>');
        InializeLinking();
      });      
  </script>

  <script type="text/javascript" src="<%: Url.Content("~/Scripts/Pax/ProvisionalInvoice.js")%>"></script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Provisional Invoice Records</h1>
  <% if (ViewData[ViewDataConstants.PageMode].ToString() != PageMode.View && Model.Invoice.IsFormABViaIS != true)
     {
          
  %>
  <div>
    <%
      Html.RenderPartial("ProvisionalInvoiceDetailControl", Model);%>
  </div>
  <%
    }
  %>
  <h2>
    Provisional Invoice List</h2>
  <div>
    <% Html.RenderPartial("GridControl", ViewData[ControlIdConstants.ProvisionalInvoiceGridId]); %>
  </div>
  <div class="clear">
  </div>
  <div class="buttonContainer">
    <% if (ViewData[ViewDataConstants.PageMode].ToString() != PageMode.View)
       {
    %>
    <%: Html.LinkButton("Back", Url.Action("Edit", "FormDE", new { invoiceId = Model.InvoiceId.Value() }))%>
    <%
      }
       else
       {%>
    <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("View",
                                     "FormDEPayables",
                                     new
                                       {
                                         invoiceId = Model.InvoiceId.Value()
                                       })%>';" />
    <%
      }%>
  </div>
</asp:Content>
