<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Cargo.BmTest>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  CargoBMCreate
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h2>
    CargoBMCreate</h2>
  <%--<script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
<script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>--%>
  <%using (Html.BeginForm("CargoBMCreate", "Invoice", FormMethod.Post, new {  id = "cargoBillingMemoForm", @class = "validCharacters" }))
    { %>
  <div class="editor-label">
    <%: Html.LabelFor(model => model.BillingMemoNumber) %>
  </div>
  <div class="editor-field">
    <%: Html.TextBox("BillingMemoNumber") %>
    
  </div>
  <input type="submit" value="Save" class="primaryButton ignoredirty" />
  <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
</asp:Content>
