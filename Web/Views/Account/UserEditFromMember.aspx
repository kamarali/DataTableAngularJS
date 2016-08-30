<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<SIS.Web.UIModels.Account.CreateUserView>" %>

<asp:Content ID="registerTitle" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Modify
</asp:Content>
<asp:Content ID="script" ContentPlaceHolderID="Script" runat="server">
  <script src="<%=Url.Content("~/Scripts/RegisterUser.js")%>" type="text/javascript"></script>
  <script type="text/javascript">
    $(document).ready(function () {

      registerAutocomplete('CountryName', 'CountryCode', '<%:Url.Action("GetCountryList", "Data", new { area = "" })%>', 0, true, null);
      registerAutocomplete('SubDivisionName', 'SubDivisionCode', '<%:Url.Action("GetSubdivisionCodeList", "Data", new { area = "" })%>', 0, true, null, '', '', '#CountryName');

      $('#CountryName').change(function () {
        $('#SubDivisionName', '#content').val("");
        $('#SubDivisionName').flushCache();
      });

      $('#EmailAddress').attr('readonly', true);

      //CMP #665: Sec 2.7.1: Conditional Disabling of User Details 
      //Disable User Profile Updates is true, will disable First Name, Last Name, Email Address. otherwise behavior will not change.
      var disableUserProfileUpdates = '<%: ViewData["DisableUserProfileUpdates"] %>';
      if (disableUserProfileUpdates != null && disableUserProfileUpdates.toUpperCase().indexOf('TRUE') >= 0 ) {
          $('#FirstName').attr('readonly', true);
          $('#LastName').attr('readonly', true);
      }

      // SCP121760: SIS Ops User Edit User Screen
      // Read the values assigned form Controller
      var LoggedInUserCategory = '<%: TempData["loggedInUserCategory"] %>';
      var LoggedInUserType = '<%: TempData["loggedInUserType"] %>';

      // if Logged in user is SIS Ops then show 'Save User Details' button.
      if (LoggedInUserCategory == 'True') {
        $('#btnSubmit').attr("style", "display:visible;");
      }
      // if Logged in user is other than SIS Ops and Logged in user is Super user then show 'Save User Details' button.
      else if (LoggedInUserCategory == 'False' && LoggedInUserType == 'True') {
        $('#btnSubmit').attr("style", "display:visible;");
      }
      // if Logged in user is other than SIS Ops and Logged in user is not Super user then hide 'Save User Details' button.
      else {
        $('#btnSubmit').attr("style", "display:none;");
      }

      if ($('#LocationID').val() != null && $('#LocationID').val() != 0) {

        var selectedItem = $('#LocationID').val();
        GetLocation(selectedItem);
      }

      $('#LocationID').change(function () {

        var selectedItem = $('#LocationID').val();
        GetLocation(selectedItem);
      });

      function GetLocation(selectedItem) {

        if (selectedItem > 0) {
          // Disable Field in case Member Location selected
          $('#Address1').attr('disabled', true);
          $('#Address2').attr('disabled', true);
          $('#Address3').attr('disabled', true);
          $('#CityName').attr('disabled', true);
          $('#PostalCode').attr('disabled', true);
          $('#CountryName').attr('disabled', true);
          $('#SubDivisionName').attr('disabled', true);
        }
        else {
          $('#Address1').removeAttr('disabled');
          $('#Address2').removeAttr('disabled');
          $('#Address3').removeAttr('disabled');
          $('#CityName').removeAttr('disabled');
          $('#PostalCode').removeAttr('disabled');
          $('#CountryName').removeAttr('disabled');
          $('#SubDivisionName').removeAttr('disabled');
          $('#CountryName').val("");
        }

        $.ajaxSetup({ cache: false });
        if ((selectedItem)
                && (selectedItem != 'undefined')
                && (selectedItem != ' ')) {

          $.ajax({
            url: '<%= Url.Action("GetMemberLocationByLocationID", "Account")%>',
            type: "POST",
            datType: "json",
            success: function (data) {
              if (data) {

                $.each(data, function (i, data) {


                  var Address1 = data["Address1"];
                  var Address2 = data["Address2"];
                  var Address3 = data["Address3"];
                  var PostalCode = data["PostalCode"];
                  var SelectedCountry = data["CountryCode"];
                  var CountryName = data["CountryName"];
                  var SelectedSubDivision = data["SubDivisionCode"];
                  var SubDivisionName = data["SubDivisionName"];
                  var SelectedCity = data["CityCode"];

                  var CountryValid = ((SelectedCountry) && (SelectedCountry != "undefined") && (SelectedCountry != ' '));
                  var SubDivisionValid = ((SelectedSubDivision) && (SelectedSubDivision != "undefined") && (SelectedSubDivision != ' '));
                  var CityValid = ((SelectedCity) && (SelectedCity != "undefined") && (SelectedCity != ' '));

                  if ((Address1) && (Address1 != 'undefined') && (Address1 != ' ')) {
                    $('#Address1').val(Address1);

                  }
                  else {
                    $('#Address1').val('');
                  }


                  if ((Address2) && (Address2 != 'undefined') && (Address2 != ' ')) {
                    $('#Address2').val(Address2);

                  }
                  else {
                    $('#Address2').val('');
                  }

                  if ((Address3) && (Address3 != 'undefined') && (Address3 != ' ')) {
                    $('#Address3').val(Address3);


                  }
                  else {
                    $('#Address3').val('');

                  }

                  if (CityValid) {
                    $('#CityName').val(SelectedCity);


                  }
                  else {
                    $('#CityName').val('');

                  }
                  if (SubDivisionValid) {
                    $('#SubDivisionName').val(SubDivisionName);
                    $('#SubDivisionCode').val(SelectedSubDivision);

                  }
                  else {
                    SelectedSubDivisionChange = false;
                    $('#SubDivisionName', '#content').val('');
                  }

                  if (CountryValid) {
                    $('#CountryName').val(CountryName)
                    $('#CountryCode').val(SelectedCountry)
                  }

                  if ((PostalCode) && (PostalCode != 'undefined') && (PostalCode != ' ')) {
                    $('#PostalCode').val(PostalCode);

                  }
                  else {
                    $('#PostalCode').val('');

                  }
                });
              }
              else {
                ClearLocationFields();
              }
            },
            error: function (xhr, ajaxOptions, thrownError) {
              ClearLocationFields();
            },
            data: selectedItem
          });
        }
        else {
          ClearLocationFields();
        }
      }

      function ClearLocationFields() {
        $('#Address1').val('');
        $('#Address2').val('');
        $('#Address3').val('');
        $('#PostalCode').val('');
        $('#CityName').val('');
        $('#CountryCode').val('');
        $('#SubDivisionData').val('');
        $('#SubDivisionName').val('');
        $('#CountryName').val('');
        $('#SubDivisionName', '#content').val('');

      }
    });

  // CMP#668: Archival of IS-WEB Users and Removal from Screens
  $('#btnSubmit').click(EnableIsArchivedCheckBox);
  function EnableIsArchivedCheckBox(event) {      
      var isArchivedeCheckbox = document.getElementById('IsArchived');
      isArchivedeCheckbox.disabled = false;
      return true;
  }

  </script>
</asp:Content>
<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Modify User Account</h1>
  <p>
    Use the form below to modify account.
  </p>
  <%: Html.ValidationSummary("Account Update was unsuccessful. Please correct the errors and try again.") %>
  <% ViewData["ShouldShowRoleEdit"] = true;%>  
  <% using (Html.BeginForm("UserEditFromMember", "Account", FormMethod.Post, new { id = "frmRegistration"}))
     { %>     
     <%: Html.AntiForgeryToken() %>
  <% Html.RenderPartial("EditRegistrationControl"); %>
  <% } %>
</asp:Content>
