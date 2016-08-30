<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<SIS.Web.UIModels.Account.CreateUserView>" %>

<asp:Content ID="registerTitle" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Profile and User Management :: Create Users
</asp:Content>
<asp:Content ID="script" ContentPlaceHolderID="Script" runat="server">
  <script src="<%=Url.Content("~/Scripts/RegisterUser.js")%>" type="text/javascript"></script>
  <script type="text/javascript">
    var SelectedCityChange = false;
    var SelectedSubDivisionChange = false;
    var CountryId = 'BA';

    $(document).ready(function () {

        registerAutocomplete('CountryName', 'CountryCode', '<%:Url.Action("GetCountryList", "Data", new { area = "" })%>', 0, true, null);
        registerAutocomplete('SubDivisionName', 'SubDivisionCode', '<%:Url.Action("GetSubdivisionCodeList", "Data", new { area = "" })%>', 0, true, null, '', '', '#CountryName');

        $('#EmailAddress').blur(function () {
            if (($(this).val() != "") && (($("#HiddenEmailAddress").val()) != ($(this).val())))
                if ($('#UserCategory').val() == 4) {

                    $('#memberdiv').show();
                    $('#MembersList').show();
                    $('#MemebrLabel').show();
                }
                else {
                    $('#memberdiv').hide();
                    $('#MembersList').hide();
                    $('#MembersList').val('');
                    $('#MemberData').val('');
                    $('#LocationID').val("");
                    $('#MemebrLabel').hide();
                }
        });

        $('#CountryName').change(function () {

            $('#SubDivisionName', '#content').val("");
            $('#SubDivisionName').flushCache();
        });

        $('#LocationID').change(function () {

            var selectedItem = $('#LocationID').val();
            GetLocation(selectedItem);
        });

        $('#UserCategory').change(function () {

            var selectedItem = $('#UserCategory').val();
            if (selectedItem != "4") {
                $('#memberdiv').hide();
                $('#MembersList').hide();
                $('#MembersList').val('');
                $('#MemberData').val('');
                $('#LocationID').val("");
                $('#MemebrLabel').hide();
                ClearAllMembers();
                ClearAllLocations();
                GetLocation(0);
                ClearLocationFields();
            }
            else {
                $('#memberdiv').show();
                $('#MembersList').show();
                $('#MemebrLabel').show();
            }
            $('#UserCategoryData').val($('#UserCategory').val());
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

        function GetLocations(selectedItem) {

            ClearAllLocations();
            $.ajaxSetup({ cache: false });
            if ((selectedItem) && (selectedItem != 'undefined')) {
                $.ajax({ url: '<%= Url.Action("GetMemberLocationsByMember","Account")%>',
                    type: "POST",
                    datType: "json",
                    success: function (data) {
                        if (data) {

                            var items = "";
                            var firstItem = "<option value='0'></option>";
                            $.each(data, function (i, data) {
                                if ((data) && (data != ' ')) {
                                    items += "<option value='" + data["LocationId"] + "'>" + data["LocationCode"] + "</option>";
                                }
                            });

                            if (items != "") {
                                items = firstItem + items;
                                $('#LocationID').html(items);
                                $('#LocationID').removeAttr('disabled');
                                $('#LocationID').trigger("change");
                            }
                            else {
                                $('#LocationID').html(firstItem);
                                $('#LocationID').trigger("change");
                            }
                        }
                    },
                    data: selectedItem
                });
            }
        }

        $('#UserCategory:visible').trigger('change');

        var isSuperuserCreation = '<%= ViewData["IsSuperUserCreation"] %>';
        if (isSuperuserCreation) {

            $('#lblMemberName').text('<%= SessionUtil.MemberName %>');
            GetLocations('<%=  ViewData["SelectedMemberId"] %>');

        }

        function ClearAllMembers() { $('#MemberList >option').remove(); }
        function ClearAllLocations() { $('#LocationID >option').remove(); }

        function UserName_AutoCompleteValueChange(selectedId) {

            ClearAllLocations();
            GetLocations(selectedId);
            if ($('#MemberData').val() != selectedId) {
                $('#LocationID').val("");
                $('#LocationID').trigger("change");
            }
        }

        registerAutocomplete('MembersList', 'MemberData', '<%:Url.Action("GetAllMemberList", "Data", new { area = "" })%>', 0, true, function (selectedId) { UserName_AutoCompleteValueChange(selectedId); });

    });

     
  </script>
</asp:Content>
<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
  <h1>Create  Users</h1>
  <p>
    Use the form below to create new User. All required fields are marked (*)
  </p>
  <div style="color: Red">
    <%: Html.ValidationSummary("Account creation was unsuccessful. Please correct the errors and try again.") %>
  </div>
  <% ViewData["ShouldShowPassPhraseQuestionEdit"] = false;%>
  <% ViewData["ShouldShowRoleEdit"] = true;%>  
  <% using (Html.BeginForm("Register", "Account", FormMethod.Post, new { id = "frmRegistration", isSuperUserCreation= ViewData["IsSuperUserCreation"],SelectedMemberId = ViewData["SelectedMemberId"] }))
     { %>     
     <%: Html.AntiForgeryToken() %>
  <% Html.RenderPartial("EditRegistrationControl"); %>
  <% } %>
</asp:Content>
