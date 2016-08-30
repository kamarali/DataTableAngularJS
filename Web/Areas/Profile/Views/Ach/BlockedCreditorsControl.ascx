<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<div>
  
   <%Html.RenderPartial("~/Areas/Profile/Views/ACH/BlockedCreditorDebtorGridControl.ascx", ViewData["BlockedCreditorsDebtorsGrid"]); %>
 
    </div>
    <div id="divAddBlocekedCreditors" class="achBlkcred-dialog">
    <% Html.RenderPartial("AddBlockedCreditors");%></div>
  
  <div class="buttonContainer">
     <input type="button" class="primaryButton" value="Add"  onclick="ShowAddCreditorsdialog();" />
    </div>
   
  <script type="text/javascript">
      var $AddCreditorsdialog;
      $(document).ready(function () {
          $AddCreditorsdialog = $('<div></div>')
		.html($("#divAddBlocekedCreditors"))
		.dialog({
		    autoOpen: false,
		    title: 'Add Blocked Creditor',
		    height: 200,
		    width: 700,
		    modal: true,
		    resizable: false
		});
      });

      function ShowAddCreditorsdialog() {
          $AddCreditorsdialog.dialog('open');
          return false;
      }
      
</script>
 