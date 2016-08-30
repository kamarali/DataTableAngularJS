<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.PaxInvoice>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> :: Credit Memo List
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Credit Memo</h1>
  <div>
    <% Html.RenderPartial("ReadOnlyCreditNoteHeaderControl", Model); %>
  </div>
  <h2>
    Credit Memo List
  </h2>
  <div>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.CreditMemoGrid]); %>
  </div>
  <div class="buttonContainer">
    <%
  
      if (ViewData[ViewDataConstants.PageMode].ToString() != PageMode.View)
      {
        using (Html.BeginForm("CreditMemoCreate", "CreditNote", FormMethod.Get))
        {%>
    <input type="submit" value="Create Credit Memo" class="primaryButton" id="btnAdd" />
    <input type="button" value="Back" class="secondaryButton" id="Back" onclick="javascript:location.href = '<%: Url.Action("View", "CreditNote", new {invoiceId = Model.Id.Value()}) %>'" />
    <%
      }
      }
      else
      {%>
        <%: Html.LinkButton("Back", Url.Action("Edit", "CreditNote", new { invoiceId = Model.Id.Value() }))%>
    <%
      }%>
  </div>
  <div id="divBillingHistoryInvoice" class="hidden">
    <%
      Html.RenderPartial("BillingHistoryInvoice");%>
  </div>
</asp:Content>
<asp:Content ID="CreateCreditNoteScriptContent" ContentPlaceHolderID="Script" runat="server">
<%
    if (ViewData[ViewDataConstants.BillingType] != null && ViewData[ViewDataConstants.BillingType].ToString() == BillingType.Payables && TempData[TempDataConstants.FromCorrespondence] == null)
    {%>
  <%:ScriptHelper.GeneratePaxPayablesGridViewRejectScript(Url, ControlIdConstants.CreditMemoGrid, Url.Action("CreditMemoView"), "CM", Model.Id.Value(), Url.Action("InitiateRejection", "CreditNote", new { area = "Pax" }), Url.Action("InitiateDuplicateRejection", "CreditNote", new { area = "Pax" }), Model.BillingCode, Model.BillingYear, Model.BillingMonth, Model.BillingPeriod, Model.SettlementMethodId)%>
  <%} 
    else if (ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
    {
  %>
  <%:ScriptHelper.GenerateGridViewScript(Url, ControlIdConstants.CreditMemoGrid, Url.Action("CreditMemoView"))%>
  <%
    }
    else
    {
  %>
  <%: ScriptHelper.GenerateGridEditDeleteScript(Url, ControlIdConstants.CreditMemoGrid, Url.Action("CreditMemoEdit"), Url.Action("CreditMemoDelete"))%>
  <%
    }
  %>
  <script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/Billinghistory.js")%>" type="text/javascript"></script>
</asp:Content>
