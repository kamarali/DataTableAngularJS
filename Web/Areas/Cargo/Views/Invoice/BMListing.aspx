<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Cargo.RejectionMemo>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Billing Memo Listing
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Billing Memo Listing</h1>
  <div>
    <%
      Html.RenderPartial("ReadOnlyInvoiceHeaderControl", Model.Invoice);%>
  </div>
  <div>
    <h2>
      AirWay Billing Breakdown List
    </h2>
    <%
      Html.RenderPartial("GridControl", ViewData[ViewDataConstants.BillingMemoListingGrid]);%>
  </div>
  <div class="buttonContainer">
    <%using (Html.BeginForm("BMCreate", "Invoice", FormMethod.Get))
      {%>
    <input type="submit" value="Add" class="primaryButton ignoredirty" id="addAwbChargeCollectBrkdwn" />
    <%
      }
    %>
    <%using (Html.BeginForm("BMCreate", "Invoice", FormMethod.Get))
      {%>
    <input class="secondaryButton" type="submit" value="Back" />
    <%
      }
    %>
    <%-- <%: Html.LinkButton("Back", Url.Action("RMCreate", "Invoice"))%>--%>
  </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Script" runat="server">
</asp:Content>
