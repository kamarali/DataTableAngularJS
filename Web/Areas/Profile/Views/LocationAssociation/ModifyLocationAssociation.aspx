<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MemberProfile.LocationAssociation>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Profile :: Modify Location Associations
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        Modify Location Association
    </h1>
    <% using (Html.BeginForm("ModifyLocationAssociation", "LocationAssociation", FormMethod.Post, new { id = "frmModifyLocations" }))
       { %>
    <div>
        <div class="searchCriteria">
            <div id="divModify" class="solidBox">
                <div class="fieldContainer horizontalFlow" style="height: 380px; margin-left: 40px;">
                    <div class="buttonContainer">
                        <div style="width: 280px; height: 10px; display: inherit; float: none;">
                            User / Contact : <b>
                                <%: Html.DisplayFor(m => m.emailAddress)%>
                            </b>
                            <%: Html.HiddenFor(m=>m.targetType)%>
                            <%: Html.HiddenFor(m=>m.grantingType)%>
                            <%: Html.HiddenFor(m=>m.userId)%>
                            <%: Html.HiddenFor(m=>m.excludedLocations)%>
                            <%: Html.HiddenFor(m => m.emailAddress)%>
                        </div>
                        <br />
                        
                        <div style="width: 380px; display: inherit; float: none;">
                        
                            <div id="LocAssociationType">
                                Location Association Type:<br />
                                <div>
                                    <input type="radio" name="AssociationType" id="radNone" value="0" />
                        
                                    &nbsp;None
                                </div>
                                <br />
                                <div>
                                    <input type="radio" name="AssociationType" id="radAllLocation" value="1" />
                        
                                    &nbsp;All Location IDs
                                </div>
                                <br />
                                <div>
                                    <input type="radio" name="AssociationType" id="radSpecificLocation" value="2" />
                        
                                    &nbsp;Specific Location IDs
                                </div>
                            </div>
                        </div>
                        <div id="locationListBox" style="width: 280px; margin-top: 10px;">
                            <table>
                                <tr>
                                    <td>
                                        Unassociated Location IDs:
                                        <%:Html.ListBox("UnAssociatedLocation", (MultiSelectList)ViewData["UnAssociatedLocation"], new {@style = "width: 200px;height:180px;" })%>
                                    </td>
                                    <td>
                                        <table>
                                            <tr>
                                                <td>
                                                    <input type="button" class="shiftButton" value=">" id="add" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <input type="button" class="shiftButton" value=">>" id="addAll" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <input type="button" class="shiftButton" value="<" id="remove" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <input type="button" class="shiftButton" value="<<" id="removeAll" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td>
                                        Associated Location IDs:
                                        <%:Html.ListBox("AssociatedLocation", (MultiSelectList)ViewData["AssociatedLocation"], new {@style = "width: 200px;height:180px;" })%>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                    <div style="width: 280px; margin-top: 10px;">
                        <table style="width: 80%">
                            <tr>
                                <td>
                                    <input type="button" id="btnSave" value="Save" class="primaryButton" />
                                </td>
                                <td>
                                    <input class="secondaryButton" type="button" value="Cancel" style="text-align: right;"
                                        onclick="javascript:location.href ='<%=Url.Action("ManageLocationAssociation","LocationAssociation") %>'" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
                <div id="divNotes" style="width: 100%; float: none; margin-left: 10px;">
                    <b>Note:</b>
                    <br />
                    <p>
                        The User / Contact whose associations you are modifying may be additionally associated
                        with Location IDs that you are not associated with. Such Location IDs are not displayed
                        in the list boxes above, since you cannot associate or disassociate them.
                    </p>
                    <p>
                        These will not be impacted by any modifications in the associations performed by
                        you.
                    </p>
                    <p>
                        Only Location IDs you are associated with are shown in the list boxes above. You
                        may associate or disassociate such Location IDs with/from this User / Contact.</p>
                </div>
            </div>
        </div>
    </div>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {

            // UserType/AssociationType 
            //0 => None            
            //1 => All Location
            //2 => Specific Location


            var AssociationTypeValue = 0;
            $("#divNotes").hide();
            $('#LocAssociationType').css({ 'display': 'none' });
            $('#locationListBox').css("display", "none");
            //If you want to move selected item from availableFields to selectedFields
            $("#add").click(function () {

                var selected = $("#UnAssociatedLocation").find(':selected').val();
                $("#UnAssociatedLocation option:selected").appendTo("#AssociatedLocation");
                SortListItems("AssociatedLocation");
                $("#AssociatedLocation").val(selected);
            });


            //If you want to move all item from availableFields to selectedFields
            $("#addAll").click(function () {
                $("#UnAssociatedLocation option").appendTo("#AssociatedLocation");
                SortListItems("AssociatedLocation");
            });

            //If you want to remove selected item from selectedFields to availableFields
            $("#remove").click(function () {

                var selected = $("#AssociatedLocation").find(':selected').val();
                $("#AssociatedLocation option:selected").each(function () {
                    $(this).appendTo("#UnAssociatedLocation");
                });

                SortListItems("UnAssociatedLocation");

                $("#UnAssociatedLocation").val(selected);

            });

            //If you want to remove all items from selectedFields to availableFields
            $("#removeAll").click(function () {
                $("#AssociatedLocation option").appendTo("#UnAssociatedLocation");
                SortListItems("UnAssociatedLocation");
            });


            function SortListItems(ListBoxId) {

                var Location = ["MAIN", "UATP"];

                var listIntLocation = $("#" + ListBoxId + " option");
                var listStringLocation = listIntLocation;

                for (var i = listStringLocation.length - 1; i >= 0; --i) {
                    var itemText = listStringLocation[i].innerHTML;
                    itemText = itemText.split('-')[0];

                    var found = $.inArray(itemText.toUpperCase(), Location) > -1;

                    if (!found) {

                        listStringLocation = jQuery.grep(listStringLocation, function (value) {
                            return value != listStringLocation[i];
                        });

                    }
                }

                for (var i = listIntLocation.length - 1; i >= 0; --i) {
                    var itemText = listIntLocation[i].innerHTML;
                    itemText = itemText.split('-')[0];
                    var found = $.inArray(itemText.toUpperCase(), Location) > -1;

                    if (found) {
                        listIntLocation = jQuery.grep(listIntLocation, function (value) {
                            return value != listIntLocation[i];
                        });

                    }
                }

                listStringLocation.sort(function (a, b) {
                    var firstItem = a.innerHTML.split('-')[0];
                    var secondItem = b.innerHTML.split('-')[0];
                    if (firstItem > secondItem) return 1;
                    else if (firstItem < secondItem) return -1;
                });


                listIntLocation.sort(function (a, b) {
                    var firstItem = a.innerHTML.split('-')[0];
                    var secondItem = b.innerHTML.split('-')[0];
                    if (parseInt(firstItem) > parseInt(secondItem)) return 1;
                    else if (parseInt(firstItem) < parseInt(secondItem)) return -1;

                });
                $("#" + ListBoxId).empty().append(listStringLocation);
                $("#" + ListBoxId).append(listIntLocation);
            }


            var grantingUserType = $('#grantingType').val();
            var targetUserType = $('#targetType').val();
            AssociationTypeValue = targetUserType;

            if (grantingUserType == "1") {
                $("#divNotes").hide();
                if (targetUserType == 0) {
                    $("#radNone").attr('checked', 'checked');
                    $('#LocAssociationType').css({ 'display': 'block' });
                    $('#locationListBox').css("display", "none");
                } else if (targetUserType == 1) {
                    $("#radAllLocation").attr('checked', 'checked');
                    $('#LocAssociationType').css({ 'display': 'block' });
                    $('#locationListBox').css("display", "none");
                } else {
                    $("#radSpecificLocation").attr('checked', 'checked');
                    $('#locationListBox').css({ 'display': 'block' });
                    $('#LocAssociationType').css({ 'display': 'block' });
                }

            } else {

                $("#divNotes").show();

                if (targetUserType == 0) {
                    $("#radNone").attr('checked', 'checked');
                    $('#locationListBox').css("display", "block");
                    $('#LocAssociationType').css({ 'display': 'none' });
                } else if (targetUserType == 1) {
                    $("#radAllLocation").attr('checked', 'checked');
                    $('#locationListBox').css("display", "none");
                    $('#LocAssociationType').css({ 'display': 'block' });
                } else {
                    $("#radSpecificLocation").attr('checked', 'checked');
                    $('#locationListBox').css({ 'display': 'block' });
                    $('#LocAssociationType').css({ 'display': 'none' });
                }
            }

            $("input[name='AssociationType']").change(function () {
                AssociationTypeValue = $(this).val();

                if (AssociationTypeValue == "2") {
                    $('#locationListBox').css({ 'display': 'block' });
                } else {
                    $('#locationListBox').css("display", "none");
                }

            });


            $("#btnSave").bind('click', function () {
                var selectedLocationIds = '';
                if (grantingUserType == 2 && AssociationTypeValue == 0) {
                    AssociationTypeValue = 2;
                }

                if (AssociationTypeValue == 2) {

                    $("#AssociatedLocation").each(function () {
                        $('option', this).each(function () {
                            selectedLocationIds = selectedLocationIds + ',' + $(this).val();
                        });
                    });

                }

                if (grantingUserType == 1 && selectedLocationIds == '' && AssociationTypeValue == 2) {
                    showClientErrorMessage('At least one Location ID should be associated with the User / Contact');
                    return false;
                }


                var userId = $("#userId").val();
                var excludedLocIds = $("#excludedLocations").val();
                var emailId = $("#emailAddress").val();

                $.ajax({
                    type: "POST",
                    url: '<%:Url.Action("SaveLocationAssociation", "LocationAssociation", new {area = "Profile"})%>',
                    data: { locationSelectedIds: selectedLocationIds, excludedLocIds: excludedLocIds, userId: userId, associtionType: AssociationTypeValue, emailId: emailId, memberId: "0" },
                    dataType: "json",
                    success: function (response) {
                        if (response.Message) {
                            if (response.IsFailed == false) {
                                showClientSuccessMessage(response.Message);

                            } else {
                                showClientErrorMessage(response.Message);
                                if (typeof result.Message === 'undefined') {
                                    showClientErrorMessage('Session seems to be expired. Please log in again');
                                }
                            }
                        }
                        else {
                            showClientErrorMessage('Session seems to be expired. Please log in again');
                        }
                    },
                    failure: function (response) {
                        showClientErrorMessage(response.Message);
                    }
                });


            });

        });

    </script>
</asp:Content>
