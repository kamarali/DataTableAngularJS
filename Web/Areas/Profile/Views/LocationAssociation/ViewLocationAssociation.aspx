<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MemberProfile.LocationAssociation>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Profile :: View Location Associations
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {

            // UserType/AssociationType = 0 => None            
                                        //1 => All Location
                                        //2 => Specific Location
                
            $("#btnModify").bind('click', function () {
                var grantingUserType = $('#grantingType').val();
                if (grantingUserType == "0") {
                    showClientErrorMessage('Your Location Association Type is <i>None</i>. You cannot modify Location Association of any User or Contact');
                    return false;
                }
                var targetUserType = $('#targetType').val();
                if (grantingUserType == "2" && targetUserType == "1") {
                    showClientErrorMessage('Your Location Association Type is <i>Specific Location IDs</i>. The User/Contact you are attempting to modify is associated with <i>All Location IDs</i>');
                    return false;
                }

            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        View Location Association
    </h1>
    <% using (Html.BeginForm("ViewLocationAssociation", "LocationAssociation", FormMethod.Post, new { id = "frmViewLocations" }))
       { %>
       <%: Html.AntiForgeryToken() %>
    <div>
        <div class="searchCriteria">
            <div id="divView" class="solidBox">
                <div class="fieldContainer horizontalFlow" style="height: 300px; margin-left: 40px;">
                    <div class="buttonContainer">
                        <div style="width: 280px; height: 15px; display: inherit; float: none;">
                            User / Contact : <b>
                                <%: Html.DisplayFor(m => m.emailAddress)%>
                            </b>
                        </div>
                        <%= Html.HiddenFor(m => m.userId)%>
                        <%= Html.HiddenFor(m => m.emailAddress)%>
                        <%: Html.HiddenFor(m=>m.targetType)%>
                        <%: Html.HiddenFor(m=>m.grantingType)%>
                        <br />
                        <div style="width: 280px; height: 15px; display: inherit; float: none;">
                            Location Association Type : <b>
                                <%if (Convert.ToString(Model.targetType) == "0")
                                  {%>
                                None
                                <%}
                                  else if (Convert.ToString(Model.targetType) == "1")
                                  {%>
                                All Location IDs
                                <%}
                                  else
                                  {%>Specific Location IDs<%} %>
                            </b>
                        </div>
                        <br />
                        <%if (Convert.ToString(Model.targetType) == "2")
                          { %>
                        <div style="width: 280px; height: 200px; display: inherit; float: none;">
                            Associated Location IDs:<br />
                            <%:Html.ListBox("ViewAssociatedLocation", (MultiSelectList)ViewData["ViewAssociatedLocation"], new { @style = "width: 200px;height:150px;" })%>
                        </div>
                        <%} %>
                        <div style="width: 280px; height: 25px;">
                            <table style="width: 80%">
                                <tr>
                                    <td>
                                        <input type="submit" id="btnModify" value="Modify" class="primaryButton" />
                                    </td>
                                    <td>
                                        <input class="secondaryButton" type="button" value="Cancel" style="text-align: right;"
                                            onclick="javascript:location.href ='<%=Url.Action("ManageLocationAssociation","LocationAssociation") %>'" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <% } %>
</asp:Content>
