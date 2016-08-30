<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.Sampling.SamplingFormDRecord>"
  MasterPageFile="~/Views/Shared/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> :: Form D/E :: Prorate Slip
</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="MainContent">
  <h2>
    Sampling Form D Prorate Slip
  </h2>
  <div>
    <% Html.RenderPartial("SamplingInvoiceHeaderInfoControl"); %>
  </div>
  <br />
  <% using (Html.BeginForm())
     {%>
  <div>
    <label for="ProrateSlip">
      Prorate Slip:</label>
    <%: Html.TextAreaFor(formD => formD.ProrateSlip.ProrateSlipDetails, 20, 160, ScrollBars.Both)%>
  </div>
  <div class="buttonContainer">
    <input class="primaryButton" type="submit" value="Save" />
    <input class="secondaryButton" type="button" value="Back" onclick="javascript:history.go(-1);" />
  </div>
  <%
    }%>
</asp:Content>
