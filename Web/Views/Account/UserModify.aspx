<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<SIS.Web.UIModels.Account.CreateUserView>" %>


<asp:Content ID="registerTitle" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Modify
</asp:Content>

<asp:Content ID="script" ContentPlaceHolderID="Script" runat="server">

  <script src="<%=Url.Content("~/Scripts/RegisterUser.js")%>" type="text/javascript"></script>  
  <script src="<%=Url.Content("~/Scripts/jquery.tree.js")%>" type="text/javascript"></script>
  <script src="<%=Url.Content("~/Scripts/plugins/jquery.tree.checkbox.js")%>" type="text/javascript"></script>
  <script src="<%=Url.Content("~/Scripts/PermissionToUser.js")%>" type="text/javascript"></script>

 


<script type="text/javascript">
  var getPermissionListUrl = '<%:Url.Action("GetPermissionListToUser", "Permission", new { area = "Profile"}) %>?userCategory';
  var userId = '<%: SessionUtil.UserId %>';
  var UserCategory = '<%: (int) SessionUtil.UserCategory %>';

  $(document).ready(function () {

      registerAutocomplete('CountryName', 'CountryCode', '<%:Url.Action("GetCountryList", "Data", new { area = "" })%>', 0, true, null);
      registerAutocomplete('SubDivisionName', 'SubDivisionCode', '<%:Url.Action("GetSubdivisionCodeList", "Data", new { area = "" })%>', 0, true, null, '', '', '#CountryName');

      $('#CountryName').change(function () {
          $('#SubDivisionName', '#content').val("");
          $('#SubDivisionName').flushCache();
      });


      setOwnLocationAssociationURL('<%:Url.Action("GetOwnLocationAssociation", "LocationAssociation", new {area = "Profile"})%>');

      //CMP #665: Sec 2.7.1: Conditional Disabling of User Details 
      //Disable User Profile Updates is true, will disable First Name, Last Name, Email Address. otherwise behavior will not change.
      var disableUserProfileUpdates = '<%: ViewData["DisableUserProfileUpdates"] %>';
      if (disableUserProfileUpdates != null && disableUserProfileUpdates.toUpperCase().indexOf('TRUE') >= 0) {
          $('#FirstName').attr('readonly', true);
          $('#LastName').attr('readonly', true);
          $('#EmailAddress').attr('readonly', true);
      }

      if ($('#LocationID').val() != null && $('#LocationID').val() != 0) {

          var selectedItem = $('#LocationID').val();
          GetLocation(selectedItem);
      }

      $('#LocationID').change(function () {

          var selectedItem = $('#LocationID').val();
          GetLocation(selectedItem);
      });

      $('#lblMemberName').text('<%= SessionUtil.MemberName %>');



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

  // Function to validate required First Name & Email Address
$('#btnSubmit').click(ValidateRequiredFields);
function ValidateRequiredFields(event) {
    var fName = $('#FirstName').val();
    var eMailAddress = $('#EmailAddress').val();
    var lName = $('#LastName').val();

    if (fName == "") {
        showClientErrorMessage('First Name Required');
        return false;
    }

    if (!fName == "") {
        var fNameRegExp = new RegExp("^[-a-zA-Z \']*$"); 
        if (!fNameRegExp.test(fName)) {
            showClientErrorMessage('First Name is Invalid "' + fName + '"');
            return false;
        }
    }

    //CMP #665: Sec: 2.3.1, ‘Last Name’ Changes from Optional to Mandatory
    if (lName == "") {
      showClientErrorMessage('Last Name Required');
      return false;
    }

    if (!lName == "") {
      var lNameRegExp = new RegExp("^[-a-zA-Z \']*$");
      if (!lNameRegExp.test(lName)) {
        showClientErrorMessage('Last Name is Invalid "' + lName + '"');
        return false;
      }
    }

    if (eMailAddress == "") {
        showClientErrorMessage('Email Address Required');
        return false;
    }
    if (!eMailAddress == "") {
        //SCP207710 - Change Super User(Allow valid special character).
       if (!/^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i.test(eMailAddress)) {
            showClientErrorMessage('Email Address is Invalid "' + eMailAddress + '"');
            return false;
        }
    }

    // CMP#668: Archival of IS-WEB Users and Removal from Screens
    var isArchivedeCheckbox = document.getElementById('IsArchived');
    isArchivedeCheckbox.disabled = false;

    return true;
}
    </script>


  <style type="text/css">
    table.formattedTable {
      background-color: #edeeee;
      border: 1px solid #666;
      padding: 0px 0px 0px 0px;
      margin: 5px 0px 10px 0px;
      border-collapse: collapse;
    }
    table.formattedTable > thead > tr {
      background-color: #d7e9f8;
      color: #000;
      font-weight: bold;
    }
    table.formattedTable > thead > tr > td {
      vertical-align: top;
      text-align: center;
      border: 1px solid #666;
      padding: 8px 3px 8px 3px;
      width:150px;      
    }
    table.formattedTable > tbody > tr {
      background-color: #fff;
    }
    table.formattedTable > tbody > tr > td {
      padding: 8px 3px 8px 3px;
      font: normal 8pt Arial, Helvetica, sans-serif;
      color: #000000;
      border: 1px solid #666;
    }
  </style>

</asp:Content>


<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
   <% Html.EnableClientValidation(); %>
    <h2><%= Iata.IS.Web.AppSettings.ModifyUserText %></h2>
    <%= Iata.IS.Web.AppSettings.UseTheFormBelowToModifyAccountText2 %>
    <%= Html.ValidationSummary(Iata.IS.Web.AppSettings.AccountUpdateUnsuccessfulMessage) %>
    <% if (ViewData.ContainsKey("ShouldShowRoleEdit") == false)%>
    <% { %>
    <% ViewData["ShouldShowRoleEdit"] = false;%>
    <% ViewData["ShouldShowPassPhraseQuestionEdit"] = true;%>
    <% } %>
    
    <% using (Html.BeginForm("UserModify", "Account", FormMethod.Post, new { id = "UserModify" }))
    { %>
       <%: Html.AntiForgeryToken() %>
    <% Html.RenderPartial("EditRegistrationControl"); %>
   <% } %>
</asp:Content>
