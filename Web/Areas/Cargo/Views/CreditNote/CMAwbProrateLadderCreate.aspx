<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Cargo.CMAwbProrateLadderDetail>" %>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">

  <script type="text/javascript">
    pageMode =  <%:((ViewData[ViewDataConstants.PageMode]!=null && ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View) ? true : false).ToString().ToLower() %>;
    registerAutocomplete('CarrierPrefix', 'CarrierPrefix', '<%:Url.Action("GetTicketIssuingAirlineList", "Data", new { area = "" })%>', 0, true, null);
   </script>

  <script src="<%:Url.Content("~/Scripts/Cargo/CMAwbProrateLadder.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
   <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>

  <%: ScriptHelper.GenerateGridDeleteScript(Url, ViewDataConstants.CreditMemoAwbProrateLadderGrid, Url.Action("CMAwbProrateLadderDetailDelete", "CreditNote"))%>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  Billing Memo AWB Prorate Ladder
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Credit Memo AWB Prorate Ladder</h1>
  <%if (Model.IsHeaderAdded == true)
    {%>
  <div>
    <% using (Html.BeginForm("CMAwbProrateLadderCreate", "CreditNote", FormMethod.Post, new { id = "cmAwbPLHeaderForm", @class = "validCharacters" }))
       {  %>
       <%: Html.AntiForgeryToken() %>
    <% Html.RenderPartial("CMAwbProrateLadderControl", Model.ProrateLadder);%>
  </div>
  <%}%>

  <%}%>
  <% using (Html.BeginForm("CMAwbProrateLadderCreate", "CreditNote", FormMethod.Post, new { id = "cmAwbPLForm", @class = "validCharacters" }))
     {  %>
     <%: Html.AntiForgeryToken() %>
  <%if (Model.IsHeaderAdded == false)
    {%>
  <div>
    <% Html.RenderPartial("CMAwbProrateLadderControl", Model.ProrateLadder);%>
  </div>
  <%}%>
  <div>
    <% Html.RenderPartial("CMAwbProrateLadderDetailsControl");%>
  </div>
  <div class="buttonContainer">
    <input class="primaryButton ignoredirty" type="submit" value="Add" />
  </div>
  <%
     }
  %>
  <h2>
    CM AWB Prorate Ladder List</h2>
  <div>
    <%  Html.RenderPartial("GridControl", ViewData[ViewDataConstants.CreditMemoAwbProrateLadderGrid]);%>
  </div>
  <div class="clear">
  </div>
  <div class="buttonContainer">
    <%:Html.LinkButton("Back", Url.RouteUrl("CGOtransactions", new { action = "CMAwbEdit", couponId = Model.ProrateLadder.ParentId }))%>
  </div>
</asp:Content>
