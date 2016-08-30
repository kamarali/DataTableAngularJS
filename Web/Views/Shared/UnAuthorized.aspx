<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  Unauthorized Access
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<% if ((string)TempData[ViewDataConstants.InvoiceNumber] != "")
   {%>
  <h1>
    Unauthorized Access</h1>
  <h2>
    Invoice "<%:TempData[ViewDataConstants.InvoiceNumber]%>" cannot be viewed or modified</h2>
  <p>
    <strong>Reason: Invoice status may be one of the following:</strong>
    <ul>
      <li>Ready for Billing</li>
      <li>Processing Complete</li>
      <li>Presented</li>
      <li>Claimed</li>
      <li>Non Correctable Error(s) in the invoice</li>
      <li>Invoice is On Hold</li>
    </ul>
    <strong>Or, you are not authorized to access this invoice (that is you are neither the billing nor the billed member)</strong>
  </p>
  <%
   }else{%>
   <h1>
    Unauthorized Access</h1>
    <p>
    You are not authorized to view this page.</p>
   <%} %>
</asp:Content>
