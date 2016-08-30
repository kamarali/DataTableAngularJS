<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.PaxInvoice>" %>

<%@ Import Namespace="Iata.IS.Model.Pax.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> :: Sampling Form D
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Sampling Form D List</h1>
  <div>
    <% Html.RenderPartial(Url.Content("SamplingInvoiceHeaderInfoControl"), Model); %>
  </div>
  <div>
    <h2>
      Form D List</h2>
    <% Html.RenderPartial("GridControl", ViewData[ControlIdConstants.FormDGridId]); %>
  </div>
  <div class="clear">
  </div>
  <div class="buttonContainer">
    <%
  
      if ((string)ViewData[ViewDataConstants.PageMode] != PageMode.View)
      {
        using (Html.BeginForm("FormDCreate", "FormDE", FormMethod.Get))
        {%>
    <input class="primaryButton" type="submit" value=" Create " />
    <%
      }
      }
    %>
    <% using (Html.BeginForm())
       { %>
    <%: Html.LinkButton("Back", Url.Action("FormDDetails", "FormDE", new { invoiceId = Model.Id.Value() }))%>
    <%} %>
  </div>
</asp:Content>
<asp:Content ID="CreateFormDScriptContent" ContentPlaceHolderID="Script" runat="server">
  <%
    if ((string)ViewData[ViewDataConstants.PageMode] == PageMode.View)
    {
  %>
  <%:ScriptHelper.GenerateGridViewScript(Url, ControlIdConstants.FormDGridId, Url.Action("FormDView") )%>
  <%
    }
  else
  {
  %>
  <%:ScriptHelper.GenerateGridEditViewDeleteScript(Url, ControlIdConstants.FormDGridId, "FormDEdit", "FormDEdit", "FormDDelete")%>
  <%
    }
  %>
  <script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
  <script type="text/javascript">
    $(document).ready(function () {
      <%if(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
        {%>
          // make header readonly
          SetPageToViewModeEx(true, "#samplingformC,#BillingMemberReference,#BilledMemberReference");
      <%}%>
    });
  </script>
</asp:Content>
