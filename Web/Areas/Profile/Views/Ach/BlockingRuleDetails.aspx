<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MemberProfile.BlockingRule>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  Blocking Rule Details
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1 id="text1">
   Add blocking rules</h1> 
   <h2>
    Blocking Rule Details</h2>
 <div>
    <% Html.RenderPartial("~/Areas/Profile/Views/ACH/BlockingRulesControl.ascx"); %></div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">
      $(document).ready(function () {
          //alert(ViewData["IsInEditMode"].ToString());

          if ('<%=ViewData["IsInEditMode"]%>' == "True") {

              $('#text1').text("Edit blocking rules");
              $('#RuleName').focus();
          }
          else {
              $('#MemberText').focus();
          }


          $('#ClearingHouse').val('ACH');
          initBlockingRuleValidation();
      });

    // Declared Global variable which is used to create "Id" for rows added in BlockedCreditors and BlockedDebtors JqGrid table. 
    var globalRowCount = 0;
    // Variable to set Creditors memberId string
    var creditorMemberIdString = "";
    // Variable to set Debtors memberId string
    var DebtorMemberIdString = "";
    // Variable to set BlockGroups zoneId string
    var BlockByGroupZoneIdString = "";
    // Variable to set ExceptionsId string
    var ExceptionIdString = "";
  </script>
</asp:Content>
