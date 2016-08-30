<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MemberProfile.BlockingRule>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  ICH Blocking Rules
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1>Manage Blocks</h1>
  <h2>
    ICH Blocking Rules</h2>
  <div>
    Last Updated:
    <%: string.Format("{0}", DateTime.UtcNow.AddDays(-1).AddMinutes(-34)) %>
    by ICH2091</div>
  <% using (Html.BeginForm("IchBlockingRules", "Ich", FormMethod.Post))
     {%>
     <%: Html.AntiForgeryToken() %>
  <div>
    <% Html.RenderPartial("~/Areas/Profile/Views/Shared/SearchBlockingRule.ascx"); %>
  </div>
  <div class="buttonContainer">
  <input type="submit" class="primaryButton" value="Search" id="btnSearch" name="Search" />
    <input type="button" class="primaryButton" value="Add" id="btnAdd" name="Add" onclick="BlockingRulesDetailsRediret();" />
  </div>
  <%} %>
  <div>
    <%Html.RenderPartial("~/Areas/Profile/Views/Shared/BlockingRulesGrid.ascx", ViewData["BlockingRuleGrid"]); %>
  </div>
  <div>
    <input type="button" class="primaryButton" value="Download All Block Information" onclick="javascript:location.href ='<%: Url.Action("GetIchBlockingRules", "Ich") %>'" />
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">
    function BlockingRulesDetailsRediret() {
      window.location.href = 'BlockingRuleDetails';
  }
  $(document).ready(function () {
      $("#helplink").hide();
  });
  </script>
</asp:Content>
