<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Cargo.CargoBillingMemo>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Billing Memo - AirWay Bill Charge Collect Beakdown</h1>
  <div>
    <%
      Html.RenderPartial("ReadOnlyInvoiceHeaderControl", Model.Invoice);%>
  </div>
  <div>
    <h2>
      AWB Charge Collect List
    </h2>
    <%
      Html.RenderPartial("GridControl", ViewData[ViewDataConstants.BillingMemoAwbChargeCollectGrid]);%>
  </div>
 <div class="buttonContainer">
   <input type="submit" value="Add" class="primaryButton ignoredirty" id="btnSaveAndAddNew"
      onclick="javascript:return changeAction('<%: Url.Action("BMAwbChargeCollectCreate","Invoice",new { invoiceId= Model.Invoice.Id.Value(), transaction="BMEdit" }) %>')" />
    <%using (Html.BeginForm("BMList", "Invoice", FormMethod.Get))
      {%>
    <input class="secondaryButton" type="submit" value="Back to View Invoice" />
    <%
      }
    %>
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
   <%
     if (ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
    {%>
  <%: ScriptHelper.GenerateGridViewScript(Url, ControlIdConstants.BillingMemoAwbPrepaidGrid, Url.Action("BMAwbView"))%>
  <%
    }
    else
    {
  %>
  <%: ScriptHelper.GenerateGridEditDeleteScript(Url, ControlIdConstants.BillingMemoAwbPrepaidGrid, "BMAwbEdit", "BMAwbDelete")%>
  <%
    }
  %>
</asp:Content>
