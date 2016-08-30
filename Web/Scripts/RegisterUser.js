/// <reference path="../jquery-1.5.1.min.js" />
/// <reference path="site.js" />

var CheckIfUserUrl = '/Member/GetUserByEmailId';
var selectedItem = '';
var OwnLocationAssociationURL = '/Profile/LocationAssociation/GetOwnLocationAssociation';

function setCheckIfUserUrl(IfUserUrl) {
  CheckIfUserUrl = IfUserUrl;
}

function setOwnLocationAssociationURL(Url) {
    OwnLocationAssociationURL = Url;
}

$(function () {
  $("#frmRegistration").validate({
    rules: {
      FirstName: {
        required: true,
        regex: "^[-a-zA-Z \']*$"
    },
      LastName: {
          //CMP #665: Sec: 2.3.1, ‘Last Name’ Changes from Optional to Mandatory
          required: true,
          regex: "^[-a-zA-Z \']*$"
      },
      EmailAddress:
      {
        required: true,
        email: true
      },

      MembersList: {
        required: function (element) {
          var userCategory = $("#UserCategory :selected").val();
          var IsSisOps = $("#IsSisOps").val();
          if (IsSisOps == "True" && userCategory == "4") {
            return true;
          } else {
            return false;
          }
        }
      }
    },
    messages: {
      FirstName: {
        required: "First Name Required",
        regex: "Invalid data for First Name."
    },
      LastName: {
        //CMP #665: Sec: 2.3.1, ‘Last Name’ Changes from Optional to Mandatory
        required: "Last Name Required",
        regex: "Invalid data for Last Name."
      },
      EmailAddress: {
        required: "Email Address  Required",
        email: "Invalid data for Email Address."

      },
      MembersList: {
        required: "Member Required"
      }
    }
  });
  
  $("#frmChangeSecurityQuestion").validate({
    rules: {
      Question1: { required: true, maxlength: 200 },
      Answer1: { required: true, maxlength: 200 },
      Question2: { required: true, maxlength: 200 },
      Answer2: { required: true, maxlength: 200 },
      Question3: { required: true, maxlength: 200 },
      Answer3: { required: true, maxlength: 200 }

    },
    messages: {
      Question1: "Question1 Required",
      Answer1: "Answer1 Required",
      Question2: "Question2 Required",
      Answer2: "Answer2 Required",
      Question3: "Question3 Required",
      Answer3: "Answer3 Required"
    }
  });


  $("#frmChangePassword").validate({
    rules: {
      currentPassword: { required: true },
      newPassword: { required: true },
      confirmPassword: { required: true }
    },
    messages: {
      currentPassword: "Current Password Required",
      newPassword: "New Password Required",
      confirmPassword: "Confirm New Password Required"
    }
  });
});

function checkContactUser(postUrl, emaild) {
  $.ajax({
    url: postUrl,
    type: "POST",
    async: false,
    data: { emailId: emaild },
    success: function (response) {

      if ((response) && (response.EmailAddress != undefined)) {

        $("#Salutation").val(response.SalutationId);
        $("#FirstName").val(response.FirstName);
        $("#LastName").val(response.LastName);
        $('#EmailAddress').val(response.EmailAddress);
        $('#HiddenEmailAddress').val(response.EmailAddress);
        $("#PositionTitle").val(response.PositionOrTitle);
        $("#StaffID").val(response.StaffId);
        $("#Divison").val(response.Division);
        $("#Department").val(response.Department);
        $("#Telephone1").val(response.PhoneNumber1);
        $("#Telephone2").val(response.PhoneNumber2);
        $("#Mobile").val(response.MobileNumber);
        $("#Fax").val(response.FaxNumber);
        $("#SITAAddress").val(response.SitaAddress);
        $("#LocationID").val(response.LocationId);
        $("#Address1").val(response.AddressLine1);
        $("#Address2").val(response.AddressLine2);
        $("#Address3").val(response.AddressLine3);
        $('#CountryName').val(response.CountryId);
        $('#CountryData').val(response.CountryId);

        $("#CityName").val(response.CityName);
        $("#PostalCode").val(response.PostalCode);

        $('#SubDivisionName').val(response.SubDivisionCode);
        $('#SubDivisionData').val(response.SubDivisionCode);
        $('#UserCategory').val(4);
        $('#MemberData').val(response.MemberId);
      }
    }
  });
}

function GetMemberByUserCategory(postUrl, selectedItem) {
  if ((selectedItem) && (selectedItem != 'undefined')) {
    $.ajax({
      type: "POST",
      async: false,
      url: postUrl,
      dataType: "json",
      success: function (data) {
        if (data) {
          var items = "";
          var firstItem = "<option value=''>Select</option>";
          $.each(data, function (i, data) {
            items += "<option value='" + data["MemberCode"] + "'>" + data["MemberName"] + "</option>";
          });

          if (items != "") {
            items = firstItem + items;
            $('#MembersList').html(items);
            $('#MembersList').removeAttr('disabled');
            $('#MembersList').val($('#MemberData').val());
            $('#MembersList').trigger('change');
          }
          else {
            items = firstItem;
            $('#MembersList').html(items);
          }
        }
        else {
          $('#MembersList').hide();
          $('#MembersList').hide();
        }
      },
      error: function (xhr, ajaxOptions, thrownError) {
        var items = "<option value=''>Select</option>";
        $('#MembersList').html(items);
        $('#LocationID').html(items);
      },
      data: selectedItem
    });
  }
}

function ViewOwnPermission() {
  $("#dialogOwnPermission").dialog({
    autoOpen: true,
    title: 'Own Permission',
    height: 500,
    width: 450,
    modal: true,
    resizable: false
  });

  SetPostUrl(getPermissionListUrl);
  GetTreeviewHierarchyForOwnPermission(UserCategory, userId);
}

//CMP #655: IS-WEB Display per Location ID
function OwnLocationAsso() {
    $("#dialogOwnLocationAsso").dialog({
        autoOpen: true,
        title: 'Own Location Association',
        height: 250,
        width: 830,
        modal: true,
        resizable: false
    });

    $.ajax({
        type: "Get",
        url: OwnLocationAssociationURL,
        dataType: "json",
        cache: false,
        success: function (response) {
            $("#divLocationDetails").html('');
            if (response.length == undefined) {

                if (response == '0') {
                    var inHTML = "<div style='margin-top:70px;text-align:center;'>You are not associated with any Location of your organization.</div>";
                    $("#divLocationDetails").html(inHTML);
                }
                if (response == '1') {
                    var inHTML = "<div style='margin-top:70px;text-align:center;'>You are associated with all Locations of your organization.</div>";
                    $("#divLocationDetails").html(inHTML);
                }
            }

            if (response.length != undefined && response.length > 0) {

                var inHTML = "<div style='margin-top:10px;margin-bottom:10px;'>You are associated with the following Location(s) of your organization.</div>";

                inHTML += "<table class='formattedTable'><thead>  <tr><td>Location ID</td><td>Commercial Name</td><td>Address Line 1</td><td>Address Line 2</td><td>Address Line 3</td><td>City Name</td><td>Postal Code</td><td>Country Name</td></tr></thead>";

                for (var i = 0, l = response.length; i < l; i++) {

                    var addressline2 = '';
                    var addressline3 = '';
                    var postalCode = '';

                    if (response[i].AddressLine2 != null) {
                        addressline2 = response[i].AddressLine2
                    }
                    if (response[i].AddressLine3 != null) {
                        addressline3 = response[i].AddressLine3
                    }
                    if (response[i].PostalCode != null) {
                        postalCode = response[i].PostalCode
                    }

                    inHTML += "<tbody><tr><td>" + response[i].LocationCode + "</td><td>" + response[i].MemberCommerialName + "</td><td>" + response[i].AddressLine1 + "</td><td>" + addressline2 + "</td><td>" + addressline3 + "</td><td>" + response[i].CityName + "</td><td>" + postalCode + "</td><td>" + response[i].CountryName + "</td></tr></thead>";

                }
                inHTML += "</table>"
                $("#divLocationDetails").html(inHTML);
            }

        },
        failure: function (response) {
            showClientErrorMessage(response);
        }
    });



}
// End Code CMP #655: IS-WEB Display per Location ID