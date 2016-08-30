<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.Contact>" %>
<%@ Import Namespace="Iata.IS.Model.MemberProfile" %>
<%@ Import Namespace="Trirand.Web.Mvc" %>
<script type="text/javascript">

  $(document).ready(function () {
    registerAutocomplete('conSubDivisionName', 'conSubDivisionName', '<%:Url.Action("GetSubdivisionNameList", "Data", new { area = "" })%>', 0, true, null, '', '', '#conCountryId');

    $("#LocationId").change(function () {
      viewLocationDetails('<%:Url.Action("GetMemberLocationDetails", "Member", new { area = "Profile"}) %>', 'LocationId');
    });

    $('#searchFirstName').focus();
    $('#divContactAssignmentSearch').bind('dialogclose', function(event) {  
    resetContactAssignSearch();
       
     }); 

//     $('#divSwitchContacts').bind('dialogclose', function(event) {
//     });

    $('#EmailAddress').change(function () {

      if (($(this).val() != "") && (($("#hiddemEmail").val()) != ($(this).val())))

        checkIfUser('<%:Url.Action("GetUserByEmailId", "Member", new {area = "Profile",selectedMemberId =ViewData["SelectedMemberId"] })%>', '<%:Url.Action("GetUserCityNameAndSubDivisionName", "Member", new {area = "Profile"})%>', $(this).val(), null);
    });

    SetPostUrl('<%:Url.Action("GetContactDetails", "Member", new { area = "Profile"}) %>');
    setCheckIfUserUrl('<%:Url.Action("GetUserByEmailId", "Member", new {area = "Profile",selectedMemberId =ViewData["SelectedMemberId"] })%>');
    SetUserCitySubdivisionNameUrl('<%:Url.Action("GetUserCityNameAndSubDivisionName", "Member", new {area = "Profile"})%>');

   <%if (((SessionUtil.UserCategory == Iata.IS.Model.MemberProfile.Enums.UserCategory.IchOps) || (SessionUtil.UserCategory == Iata.IS.Model.MemberProfile.Enums.UserCategory.AchOps)))
      {%>
       $("#ReplaceContacts").attr('disabled', 'disabled');
       $("#CopyContacts").attr('disabled', 'disabled');
    <%
      }%>
      
  });

  $('#conCountryId').change(function () {
    $('#conSubDivisionName').val("");
    $('#conSubDivisionName').flushCache();
  });
</script>
<%var contact = new Contact() { MemberId = (int)ViewData["SelectedMemberId"] }; %>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div class="bottomLine">
      <% Html.RenderPartial("SearchContactControl", contact); %>
    </div>
    <div class="clear">
    </div>
  </div>
  <div>
    <div>
      <div class="fieldContainer verticalFlow ">
        <div class="halfWidthColumn ">
          <%
         var collection = new object[2];
         collection[0] = ViewData["SelectedMemberId"];
         collection[1] = ViewData["ContactsGrid"];
         
         Html.RenderPartial("ContactGridControl", collection); %>
        </div>
        <div style="display: block; width: 100%;">
          <input class="secondaryButton" type="button" id="btnViewAllContactAssignments" value="View All Contact Assignments" />
          <input class="secondaryButton" type="button" id="ReplaceContacts" value="Replace Contact Assignments"
            onclick="showSwitchDialog();" />
          <input class="secondaryButton" type="button" id="CopyContacts" value="Copy Contact Assignments"
            onclick="showCopyDialog();" />
          <input class="primaryButton" type="button" value="Add New Contact" id="AddContact"
            onclick="AddNewContact()" />
          <br />
        </div>
      </div>
      <div class="bottomLine">
        <% Html.RenderPartial("ContactDetails"); %>
      </div>
    </div>
  </div>
</div>
<div id="divSwitchContacts" class="replaceContacts-dialog">
  <% Html.RenderPartial("ReplaceContacts",contact);%></div>
<div id="divContactAssignmentSearch" class="contactAssignment hidden">
  <div>
    <% Html.RenderPartial("SearchContactAssignment");%></div>
  <% Html.RenderPartial("SearchResultControl");%>
</div>
<div id="divCopyContacts">
  <% 
      Html.RenderPartial("CopyContacts", contact);%></div>

<input id="hdnGroupId" type="hidden" value="" />
<input id="hdnSubGroupId" type="hidden" value="" />
<input id="hdnTypeId" type="hidden" value="" />
<script type="text/javascript">
  var $Switchdialog;
  var $Copydialog;
  $(document).ready(function () {
      $("#divCopyContacts").hide();
      $("#divSwitchContacts").hide();

      $('#GroupId,#SubGroupId,#TypeId').change(function () {
          switch ($(this).attr('id')) {
              case 'GroupId':
                  $('#hdnGroupId').val($(this).val());
                  break;
              case 'SubGroupId':
                  $('#hdnSubGroupId').val($(this).val());
                  break;
              case 'TypeId':
                  $('#hdnTypeId').val($(this).val());
                  break;
          }
      });

      $('#btnViewAllContactAssignments').click(function () {

          $("#divContactAssignmentSearch").dialog({
              title: 'All Contact Assignments',
              width: 670,
              modal: false,
              resizable: true,
              overflow: 'hidden',
              open: function () {
                  $("#divContactAssignmentSearch").show();
                  $('#divType').val('divContactAssignmentSearch');
              },
              close: function () {
                  $("#divContactAssignmentSearch").hide();
                  resetContactAssignSearch();
                  //unload grid to load grid for new search criteria. 
                  $.jgrid.gridUnload('#list');
              }
          });

        
          //Get post data.
          var postData = {
              contactTypeCategory: '',
              groupId: $('#hdnGroupId').val(),
              subGroupId: $('#hdnSubGroupId').val(),
              typeId: $('#hdnTypeId').val(),
              columns: '',
              selectedMemberId:<%:ViewData["SelectedMemberId"] %>
          };

          //On search click load grid.
          loadContactGrid('<%: Url.Action("GetMyGridDataJson", "Member", new { area = "Profile"}) %>', postData);
      });

    
  });

  function showSwitchDialog() {

      $.ajax({
          type: "Get",
          cache: false,
          url: '<%: Url.Action("Replacecontacts", "Member", new { area = "Profile"}) %>',
          data: {selectedMemberId:<%:ViewData["SelectedMemberId"] %>},
          success: function (response) {
              $('#replacenewcontact').val('');
            
              $('#divSwitchContacts').val(' ');
              $("#divSwitchContacts").html(response).dialog({

                  title: 'Replace Contact Assignments',
                  width: 210,
                  modal: true,
                  resizable: false,
                  overflow: 'hidden',
                  open: function (e) {
                     
                      $("#divSwitchContacts").show();
                      $('#divType').val('divSwitchContacts');
                  },
                  close: function () {
                      $("#divSwitchContacts").hide();
                      $('#divSwitchContacts').val(' ');
                      $('#replaceoldcontact').val('');
                      $('#replacenewcontact').val('');

                  }
              });

          }
      });
    return false;
  }

  function showCopyDialog() {

      $('#copyoldcontact').val('');
      $('#copynewcontact').val('');

      $.ajax({
          type: "Get",
          cache: false,
          url: '<%: Url.Action("Copycontacts", "Member", new { area = "Profile"}) %>',
          data: {selectedMemberId:<%:ViewData["SelectedMemberId"] %>},
          success: function (response) {
              $('#divCopyContacts').val(' ');
              $("#divCopyContacts").html(response).dialog({

                  title: 'Copy Contact Assignments',
                  width: 210,
                  modal: true,
                  resizable: false,
                  open: function (e) {

                      $("#divCopyContacts").show();
                      $('#divType').val('divCopyContacts');
                  },
                  close: function () {
                      $("#divCopyContacts").hide();
                      $('#divCopyContacts').val(' ');
                      $('#copyoldcontact').val('');
                      $('#copynewcontact').val('');

                  }
              });

          }
      });


    return false;

  }

  function resetContactAssignSearch() {
    $('#hdnGroupId').val("");
    $('#hdnSubGroupId').val("");
    $('#hdnTypeId').val("");
    $("#GroupId option[value='']").attr("selected", "selected");
    $("#SubGroupId option[value='']").attr("selected", "selected");
    $("#TypeId option[value='']").attr("selected", "selected");
  }



      
</script>
