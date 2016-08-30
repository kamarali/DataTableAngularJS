<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<script type="text/javascript" src="<%:Url.Content("~/Scripts/autoCompleteDisplay.js")%>"></script>
<div>
  
   <%Html.RenderPartial("~/Areas/Profile/Views/ICH/BlockedCreditorDebtorGridControl.ascx", ViewData["BlockedCreditorsDebtorsGrid"]); %>
 
    </div>

    <div id="divAddBlocekedCreditors" class="ichBlkCred-dialog">
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
		    resizable: false,
		    close: function (event, ui) {
		        $('#CreditorMemberCode').unautocomplete();
		      }//, // SCP90120: KAL: Issue with Blocking rules master [code deleted]
//             open: function (event, ui) { registerAutocomplete('DebtorMemberCode', 'DebtorMemberId', '<%:Url.Action("GetMemberList", "Data", new { area = "" })%>', 0, true, null); }
		
        });
      });

     function ShowAddCreditorsdialog() {
         $AddCreditorsdialog.dialog('open');
         return false;
     }
           
  </script>
 