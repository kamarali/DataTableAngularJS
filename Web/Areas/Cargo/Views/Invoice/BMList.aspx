<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Cargo.CargoInvoice>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Cargo ::
  <%: ViewData[ViewDataConstants.BillingType].ToString() %>
  :: <%:ViewData[ViewDataConstants.PageMode] %> Invoice :: Billing Memo List
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Billing Memos</h1>
  <div>
    <%
      Html.RenderPartial("ReadOnlyInvoiceHeaderControl", Model);%>
  </div>
  <div>
    <h2>
       Billing Memo List
    </h2>
    <%
      Html.RenderPartial("GridControl", ViewData[ViewDataConstants.BillingMemoGrid]);%>
  </div>
  <div class="buttonContainer">
    <%if (ViewData[ViewDataConstants.PageMode].ToString() != PageMode.View)
      {
        using (Html.BeginForm("BMCreate", "Invoice", FormMethod.Get))
        {%>
    <input type="submit" value="Add" class="primaryButton" id="btnAdd" />
    <input class="secondaryButton" type="button" value="Back to View Invoice" onclick="javascript:location.href = '<%:Url.Action("Edit","Invoice",new{invoiceId = Model.Id})%>';" />
    <%}
      }
      else
      {%>
    <input class="secondaryButton" type="button" value="Back to View Invoice" onclick="javascript:location.href = '<%:Url.Action("View","Invoice",new{invoiceId = Model.Id})%>';" />
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
<script src="<%:Url.Content("~/Scripts/Cargo/Billinghistory.js")%>" type="text/javascript"></script>
   <%
    if (ViewData[ViewDataConstants.BillingType] != null && ViewData[ViewDataConstants.BillingType].ToString() == BillingType.Payables)
    {%>
  <%:ScriptHelper.GenerateCgoPayablesGridViewRejectScript(Url, ControlIdConstants.TransactionGridId, Url.Action("BMView"), "BM", Model.Id.Value(), Url.Action("InitiateRejection", "Invoice", new { area = "Cargo" }), Url.Action("InitiateDuplicateRejection", "Invoice", new { area = "Cargo" }), Model.BillingCode, Model.BillingYear, Model.BillingMonth, Model.BillingPeriod,Model.SettlementMethodId)%>
  <%}
  else if (ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
    {%>
  <%: ScriptHelper.GenerateGridViewScript(Url, ControlIdConstants.TransactionGridId, Url.Action("BMView"))%>
  <%
    }
    else
    {
  %>
  <%: ScriptHelper.GenerateGridEditDeleteScript(Url, ControlIdConstants.TransactionGridId, "BMEdit", "BMDelete")%>
  <%
    }
  %>
</asp:Content>
