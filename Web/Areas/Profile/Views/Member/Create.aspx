<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MemberProfile.Member>" %>
<%@ Import Namespace="Iata.IS.Model.MemberProfile.Enums" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Member Profile
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script src="<%:Url.Content("~/Scripts/Member/MemberProfile.js")%>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script type="text/javascript">
    $(document).ready(function () {
        registerAutocomplete('DisplayCommercialName', 'Id', '<%:Url.Action("GetAllMemberList", "Data", new { area = "" })%>', 0, true, null);
        document.getElementById('DisplayCommercialName').focus();
    });
  </script>
  <script type="text/javascript">

    $(function () {
      $("#tabs").tabs({
        load: function (event, ui) {

          // Format all the date controls on the page.
          formatDateControls();

          // Decorate the '*' on the mandatory fields with red color.
          highlightMandatory();
          
          $(".alphaNumeric").keypress(checkIsAlphaNumeric);
          if (ui.tab.hash == "#ui-tabs-1") {
            initMemberDetailsTabValidations();
            trackFormChanges('MemberDetails');
          }
        }
});

    /*Member Details tab has been remove on create form load*/
    $(".ui-tabs-nav ").remove();

    })

    function CommercialName_SetAutocompleteDisplay(item) {
      var memberCode = item.MemberCodeAlpha + "-" + item.MemberCodeNumeric + "-" + item.CommercialName + "";
      return { label: memberCode, value: memberCode, id: item.Id };
    }
  
  </script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Member Profile</h1>
  <h2>
    Create Member</h2>
  <% Html.RenderPartial("SearchControl", Model); %>
  <div id="tabs">
    <ul class="ui-tabs-nav ui-tabs-hide">
      <%: Html.Tab("Member Details", "MemberDetails", "Member", new { area = "Profile" }, new { id = "memberDetails-tab" }, UserCategory.SisOps, UserCategory.IchOps, UserCategory.AchOps, UserCategory.Member)%>
    </ul>
  </div>
</asp:Content>
