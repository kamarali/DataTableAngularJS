<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.PaxInvoice>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger ::
  <%: ViewData[ViewDataConstants.BillingType].ToString() %>
  :: <%:ViewData[ViewDataConstants.PageMode] %> Non-Sampling Invoice :: Rejection Memo List
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Rejection Memos</h1>
  <div>
    <%
      Html.RenderPartial(Url.Content("ReadOnlyInvoiceHeaderControl"), Model);%>
  </div>
  <h2>
    Rejection Memo List</h2>
  <div>
    <%
      Html.RenderPartial("GridControl", ViewData[ViewDataConstants.RejectionMemoListGrid]);%>
  </div>
  <div class="buttonContainer">
    <%
  
      if (ViewData[ViewDataConstants.PageMode].ToString() != PageMode.View)
      {
        using (Html.BeginForm("RMCreate", "Invoice", FormMethod.Get))
        {%>
    <input type="submit" value="Add" class="primaryButton" id="btnAdd" />
    <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Edit","Invoice",new{invoiceId = Model.Id})%>';" />
    <%
      }
      }
      else
      {%>
    <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("View","Invoice",new{invoiceId = Model.Id})%>';" />
    <%}%>
  </div>
  <div id="divBillingHistoryInvoice" class="hidden">
    <%
      Html.RenderPartial("BillingHistoryInvoice");%>
  </div>
  <div id="divDuplicateRejections" class="hidden">
    <%
      Html.RenderPartial("DuplicateRejectionControl");%>
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/Billinghistory.js")%>" type="text/javascript"></script>
  <%
    if (ViewData[ViewDataConstants.BillingType] != null && ViewData[ViewDataConstants.BillingType].ToString() == BillingType.Payables && TempData[TempDataConstants.FromCorrespondence] == null)
    {%>
  <%:ScriptHelper.GeneratePaxPayablesGridViewRejectScript(Url, ViewDataConstants.RejectionMemoListGrid, Url.Action("RMView"), "RM", Model.Id.Value(), Url.Action("InitiateRejection", "Invoice", new { area = "Pax" }), Url.Action("InitiateDuplicateRejection", "Invoice", new { area = "Pax" }), Model.BillingCode, Model.BillingYear, Model.BillingMonth, Model.BillingPeriod, Model.SettlementMethodId)%>
  <%} %>
  <%else if (ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
    {%>
    <%if (TempData.ContainsKey(TempDataConstants.FromCorrespondence)){%>
    <% TempData[TempDataConstants.FromCorrespondence] = true;%>
    <%}%>
  <%:ScriptHelper.GenerateGridViewScript(Url, ViewDataConstants.RejectionMemoListGrid, Url.Action("RMView"))%>
  <%}
    else
    {%>    
  <%:ScriptHelper.GenerateGridEditDeleteScript(Url, ViewDataConstants.RejectionMemoListGrid, Url.Action("RMEdit"), Url.Action("RMDelete"))%>
  <%}%>
</asp:Content>
