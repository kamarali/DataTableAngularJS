<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SIS.Web.UIModels.Account.SearchBarModel>" %>
   <% using (Html.BeginForm("SearchOrModify", "Account", FormMethod.Post, new { id = "SearchOrModify" }))
   { %>
<h2>
    Search Criteria</h2>
<div class="searchCriteriaMedium">
    <div class="solidBox">
        <div class="fieldContainer horizontalFlow">
            <div>
                <% if (SessionUtil.UserCategory == UserCategory.SisOps)
                   { %>
                <div>
                    <label>
                        User Category:
                    </label>
                    <input id="ddVisible" name="ddVisible" />
                    <%= Html.UserCategoryDropdownListFor(model => model.UserCategoryId, "1", new { style = "width:130px;" })%>
                </div>
                <div id="divMember">
                    <label>
                        Members:
                    </label>
                    <%:Html.HiddenFor(m => m.MemberId, new { style = "width:200px;" })%>
                     <!--   CMP#596: Length of Member Accounting Code to be Increased to 12 
                            Desc: Non layout related IS-WEB screen changes.
                            Ref: FRS Section 3.1 Table 2, Section 3.2 Table 8,Section 3.3 Table 12, Section 3.5 Table20 */ -->
                    <%:Html.TextBoxFor(m => m.MemberName, new { @class = "autocComplete textboxWidth" })%>
                </div>
                <% } %>
            </div>
            <div>
                <div>
                    <label>
                        First Name:
                    </label>
                    <%= Html.TextBoxFor(m => m.FirstName)%>
                </div>
                <div>
                    <label>
                        Last Name:
                    </label>
                    <%= Html.TextBoxFor(m => m.LastName )%>
                </div>
                <div>
                    <label>
                        Email Address:
                    </label>
                    <%= Html.TextBoxFor(m => m.Email)%>
                </div>
                <div>
                    <label>
                        Status:
                    </label>
                    <%= Html.UserStatusDropdownList(model => model.StatusId, "3", new { style = "width:100px;" })%>
                </div>
            </div>
            <div>
                <div class="buttonContainer">
                    <input type="submit" value="Search" onclick="CheckValidation();" class="primaryButton" />
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="clear">
        </div>
    </div>
</div>
<h2>
    Search Results</h2>
<div>
    <%@ import namespace="Trirand.Web.Mvc" %>
    <%@ import namespace="Iata.IS.Model.MemberProfile.Enums" %>
    <%= Html.Trirand().JQGrid(Model.OrdersGrid, "SearchGrid") %>
</div>
<div id="dialogLocationAssociation">

<div class="buttonContainer">
                        <div style="width: 100%; height: 20px;">
                            <label id="lblUserName"></label>
                            <input type="hidden" id= "hdnUserID" />
                            <input type="hidden" id= "hdnEmailId" />
                            <input type="hidden" id= "hdnMemberId" />
                        </div>
                        <br />                       
                       <div  style="width: 380px;  display: inherit; float: none;">
                          
                            <div id="LocAssociationType" >
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
                            <!-- CMP #655: IS-WEB Display per Location ID -->
                            <div id="locationListBox" style="width: 280px; margin-top:10px;">
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
                          <div style="width: 100%; margin-top:10px;">
                            <table style="width: 50%; margin-left:150px;">
                                <tr>
                                    <td >
                                        <input type="button" id="btnSave" value="Save" class="primaryButton"   />
                                    </td>
                                    <td>
                                        <input class="secondaryButton" type="button" id="btnCancel" value="Cancel"  />
                                    </td>
                                </tr>
                            </table>
                        </div>   
      
</div>

<script type="text/javascript" src="<%:Url.Content("~/Scripts/User/SearchOrModify.js")%>"></script>
<script type="text/javascript">

    function formatlink(cellValue, options, rowObject) {
        var cellHtml = cellValue;

        return cellHtml;
    }

    function unformatlink(cellValue, options, cellObject) {
        return $(cellObject.html()).attr("originalValue");
    }

    function CheckValidation() {
        if ($('#MemberName').val() == '') {
            $('#MemberId').val('');
        }
       UserSearchOrModify("SearchModify");
   }
   //CMP #655: IS-WEB Display per Location ID
   $(document).ready(function () {
       SetSaveAssoLocationURL('<%:Url.Action("SaveLocationAssociation", "LocationAssociation", new {area = "Profile"})%>');
   });
           
</script>
<% } %>
