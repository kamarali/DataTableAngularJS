<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<div>
  
   <%Html.RenderPartial("~/Areas/Profile/Views/ACH/BlockedCreditorDebtorGridControl.ascx", ViewData["BlockedCreditorsDebtorsGrid"]); %>
 
    </div>
    <div id="divAddBlocekedDebtors" class="achBlkDebt-dialog">
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
		    resizable: false
		});
      });

      function ShowAddDebtorsdialog() {
          $AddDebtorsdialog.dialog('open');
          return false;
      }
      
</script>