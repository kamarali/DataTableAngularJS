<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<script type="text/javascript" src="<%:Url.Content("~/Scripts/autoCompleteDisplay.js")%>"></script>
<div>
  
   <%Html.RenderPartial("~/Areas/Profile/Views/ICH/BlockedCreditorDebtorGridControl.ascx", ViewData["BlockedCreditorsDebtorsGrid"]); %>
 
    </div>
    <div id="divAddBlocekedDebtors" class="ichBlkDebt-dialog">
    <% Html.RenderPartial("AddBlockedDebtors");%></div>
  
  <div class="buttonContainer">
     <input type="button" class="primaryButton" value="Add"  onclick="ShowAddDebtorsdialog();" />
  
    </div>
   
  <script type="text/javascript">
      var $AddDebtorsdialog;
      $(document).ready(function () {
          $AddDebtorsdialog = $('<div></div>')
		.html($("#divAddBlocekedDebtors"))
		.dialog({
		    autoOpen: false,
		    title: 'Add Blocked Debtor',
		    height: 200,
		    width: 700,
		    modal: true,
		    resizable: false,
             close: function (event, ui) {
                 $('#DebtorMemberCode').unautocomplete();
               } //, // SCP90120: KAL: Issue with Blocking rules master [code deleted]
//             open: function (event, ui) { registerAutocomplete('DebtorMemberCode', 'DebtorMemberId', '<%:Url.Action("GetMemberList", "Data", new { area = "" })%>', 0, true, null); }
		});
      });

function ShowAddDebtorsdialog() {
    $AddDebtorsdialog.dialog('open');
          return false;
      }
      
</script>