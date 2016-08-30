<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SIS.Web.UIModels.Account.CreateUserView>" %>
<%@ Import Namespace="Iata.IS.AdminSystem" %>
<%@ Import Namespace="Iata.IS.Model.Common" %>
<%@ Import Namespace="Iata.IS.Web.UIModel.Account" %>
<script type="text/javascript">
    $(document).ready(function () {
        $('#Salutation').focus();
    });
</script>

<% Html.EnableClientValidation(); %>
<fieldset class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div class="bottomLine">
      <div>
        <label>
          Salutation:</label>
        <%: Html.SalutationDropdownListFor(model => model.Salutation)%>
        <%= Html.Hidden("IsSisOps",Convert.ToBoolean(ViewData["ISOPS"])) %>
      </div>
      <div>
        <label>
          <span>
            <%=Iata.IS.Web.AppSettings.FieldRequiredSymbol  %></span>First Name:</label>
        <%: Html.TextBoxFor(m => m.FirstName, new { maxlength = 100 , Style="width:150px;"})%>
      </div>
      <div>
        <label>
         <%--CMP #665: Sec: 2.3.1, ‘Last Name’ Changes from Optional to Mandatory--%>
          <span>
            <%=Iata.IS.Web.AppSettings.FieldRequiredSymbol  %></span>Last Name:</label>
        <%: Html.TextBoxFor(m => m.LastName, new { maxlength = 100, Style="width:150px;" })%>
      </div>
      <div>
        <label>
          <span>
            <%=Iata.IS.Web.AppSettings.FieldRequiredSymbol  %></span>Email Address:</label>
        <%: Html.TextBoxFor(m => m.EmailAddress, new { maxlength = 250 , Style="width:180px;" })%>
        <%: Html.HiddenFor(m => m.HiddenEmailAddress) %>
      </div>
      <%--CMP#668: Archival of IS-WEB Users and Removal from Screens--%>
      <%if (TempData["isUserCreation"] != null && TempData["isUserCreation"] == "False")
        {%>
            <div>
                <label>Is Archived:</label>
                <%--CMP685: after this CMP IS_ACTIVE flag will be used to activate and deactivate account in place of IS_LOCKED--%>
                <%if (TempData["isUserActive"] != null && !Convert.ToBoolean(TempData["isUserActive"]))
                  {%>
                    <%: Html.CheckBoxFor(m => m.IsArchived, new { @id = "IsArchived" }) %>
                <%}
                  else
                  {%>
                    <%: Html.CheckBoxFor(m => m.IsArchived, new { @id = "IsArchived", @disabled = "disabled", @readonly = "true" })%> 
                <%}%>
            </div>
      <%}%>
    </div>
    <div class="bottomLine">
      <div>
        <label>
          Position Title:</label>
        <%: Html.TextBoxFor(m => m.PositionTitle, new { maxlength = 200, Style="width:150px;" })%>
      </div>
      <div>
        <label>
          Staff ID:</label>
        <%: Html.TextBoxFor(m => m.StaffID, new { maxlength = 15  , Style="width:150px;"})%>
      </div>
      <div>
        <label>
          Division:</label>
        <%: Html.TextBoxFor(m => m.Divison, new { maxlength = 100 , Style="width:150px;" })%>
      </div>
      <div>
        <label>
          Department:</label>
        <%: Html.TextBoxFor(m => m.Department, new { maxlength = 100 , Style="width:150px;" })%>
      </div>
    </div>
    <div class="bottomLine">
      <div>
        <label>
          Telephone
          1:</label>
        <%: Html.TextBoxFor(m => m.Telephone1, new { maxlength = 50, Style="width:150px;" })%>
      </div>
      <div>
        <label>
          Telephone 2:</label>
        <%: Html.TextBoxFor(m => m.Telephone2, new { maxlength = 50, Style="width:150px;" })%>
      </div>
      <div>
        <label>
          Mobile:</label>
        <%: Html.TextBoxFor(m => m.Mobile, new { maxlength = 50, Style="width:150px;" })%>
      </div>
      <div>
        <label>
          Fax:</label>
        <%: Html.TextBoxFor(m => m.Fax, new { maxlength = 50, Style="width:150px;" })%>
      </div>
    </div>
    <div class="bottomLine">
      <div>
        <label>
          SITA Address:</label>
        <%: Html.TextBoxFor(m => m.SITAAddress, new { maxlength = 100, Style="width:200px;" })%>
      </div>
    </div>
    <%if (ViewData.ContainsKey("ISOPS"))
      { %>
    <div class="bottomLine">
      <div>
        <label><%= Iata.IS.Web.AppSettings.UserCategoryText %></label>
        <%= Html.DropDownListFor(m => m.UserCategory, new SelectList((List<UserCategoryListItem>)ViewData["UserCategoryList"], "UserCategoryCode", "UserCategoryName"), new Dictionary<string, object> { { "style", "width:150px;" }})%>
        <%= Html.HiddenFor(m => m.UserCategoryData) %>
      </div>
      <div id="memberdiv">
        <label id="MemebrLabel">
          <%= Iata.IS.Web.AppSettings.MembersText%></label>
                     <!--   CMP#596: Length of Member Accounting Code to be Increased to 12 
                            Desc: Non layout related IS-WEB screen changes.
                            Ref: FRS Section 3.1 Table 2, Section 3.2 Table 8,Section 3.3 Table 12, Section 3.5 Table20 */ -->
                    <%:Html.TextBoxFor(m => m.MembersList, new { @class = "autocComplete textboxWidth" })%>
                        <%:Html.HiddenFor(m => m.MemberData, new { style = "width:200px;" })%>
      </div>
    </div>
    <%} %>

     <%if (SessionUtil.MemberName != "")
      { %>
    <div class="bottomLine">
        <label> <%= Iata.IS.Web.AppSettings.MembersText%></label> <b> <label  id="lblMemberName"></label></b>
     </div>
    <%} %>

    <div>
      <div>
        <label>
          <%= Iata.IS.Web.AppSettings.LocationIDText %></label>
        <% List<LocationListItem> LocationCodes = (List<LocationListItem>)ViewData["LocationCodeList"];
           string selectValue = Iata.IS.Web.AppSettings.CustomLocationIdentifier;
           if (String.IsNullOrEmpty((string)ViewData["selectedLocation"]) == false) { selectValue = (string)ViewData["selectedLocation"]; }
           if (LocationCodes.Count <= 0)
           {
             var CustomListItem = new LocationListItem
                                    {
                                      LocationId = 0,
                                      LocationCode = Iata.IS.Web.AppSettings.CustomLocationIdentifier
                                    };
             LocationCodes.Add(CustomListItem);
           }%>
        <%= Html.DropDownListFor(m => m.LocationID, new SelectList((List<LocationListItem>)ViewData["LocationCodeList"], "LocationId", "LocationCode", selectValue), new Dictionary<string, object> { { "style", "width:150px;" }})%>
      </div>
    </div>
    <div class="bottomLine">
      <div style="width: 30%;">
        <label>
          Address 1:</label>
        <%: Html.TextBoxFor(m => m.Address1, new { maxlength = 70, Style = "width:200px;" })%>
      </div>
      <div style="width: 30%;">
        <label>
          Address 2:</label>
        <%: Html.TextBoxFor(m => m.Address2, new { maxlength = 70, Style = "width:200px;" })%>
      </div>
      <div style="width: 30%;">
        <label>
          Address 3:</label>
        <%: Html.TextBoxFor(m => m.Address3, new { maxlength = 70, Style = "width:200px;" })%>
      </div>
    </div>
    <div class="bottomLine">
      <div>
        <label>
          City Name:</label>
        <%: Html.TextBoxFor(m => m.CityName, new { maxlength = 50 , Style="width:150px;" })%>
      </div>
      <div>
        <label>
          Postal Code:</label>
        <%: Html.TextBoxFor(m => m.PostalCode, new { maxlength = 20 , Style="width:150px;"})%>
      </div>
      <div>
        <label>
          Country Name:</label>
        <%: Html.TextBoxFor(m => m.CountryName, new { @class = "autocComplete" })%>
        <%: Html.HiddenFor(m => m.CountryCode)%>
      </div>
      <div>
        <label id="SubDivisionLabel">
          Subdivision
          Name:</label>
        <%:Html.TextBoxFor(m => m.SubDivisionName, new { @class = "autocComplete" })%>
        <%: Html.HiddenFor(m => m.SubDivisionCode)%>
        <%: Html.Hidden("IsSuperUserCreation", Convert.ToBoolean(ViewData["IsSuperUserCreation"]))%>
        <%: Html.Hidden("SelectedMemberId", Convert.ToBoolean(ViewData["SelectedMemberId"]))%>
        
      </div>
    </div>
         <%if (SystemParameters.Instance.General.IsMultilingualAllowed)
            {%>
            <div class="bottomLine">
            <div>
            <label>Help Text Language:</label>
            <%= Html.LanguageDropdownList("UserLanguageCode", Model==null?string.Empty:Model.UserLanguageCode,false,false)%>
            </div>
              </div>
            <%= Html.HiddenFor(m => m.UserLanguageData) %>
        <%}%>  
       
       <%--CMP-520: Management of Super Users --%> 
           <%--Added new Super User CheckBox--%>  

       <% if (Convert.ToBoolean(ViewData["MakeCheckBoxVisible"])) %>
          <% { %>       
            <div id = "divSuperUser"> 
            <label>Super User:</label>           
            
            <%if (Convert.ToBoolean(ViewData["MakeCheckBoxEditable"]))
              {%>  
                <%: Html.CheckBoxFor(m => m.UserType, new { @id = "SuperUser" })%>                  
             <%
              }%>             
              <%else
{
%>                  
                  <%--<%--SCP# 440291 - SRM: Permissions deleted. Desc: Added a hidden field for Checkbox, this will ensure that apt User Type Value is used.--%>
                  <%= Html.HiddenFor(m => m.UserType)%>

                  <%:Html.CheckBoxFor(m => m.UserType,
                                       new {@id = "SuperUser", @disabled = "disabled", @readonly = "true"})%>
                  <%
}%>
            </div>
          <% } %>
          
                        
    
    <% if (ViewData.ContainsKey("ShouldShowPassPhraseQuestionEdit") && ((bool)ViewData["ShouldShowPassPhraseQuestionEdit"]) == true) %>
    <% { %>
    <div>      
      <% =Html.ActionLink("Change Password", "ChangePassword")%>
      <br /><br />
      <a href="Javascript:ViewOwnPermission()">View Own Permission</a>
      
      <% if(SessionUtil.UserCategory == Iata.IS.Model.MemberProfile.Enums.UserCategory.Member){ %>
      <br /><br />
      <a href="Javascript:OwnLocationAsso()">View Own Location Association</a>
      <% } %>

      </div>

    <div id="dialogOwnPermission">
      <div id="TreeChoosePermission" style="overflow:auto;" />      
    </div>
    
    <div id="dialogOwnLocationAsso">
      <div id="divLocationDetails" style="overflow:auto;" >
            
      </div>
    </div>
    

    <% } %>
    <% if (ViewData.ContainsKey("ShouldShowRoleEdit") && (((bool)ViewData["ShouldShowRoleEdit"]) == true)) %>
    <% { %>
    <% Html.RenderPartial("~/Views/Account/EditRoleControl.ascx"); %>
    <% } %>
  </div>
</fieldset>
<div class="buttonContainer">
  <input id="btnSubmit" type="submit" value="Save User Details" class="primaryButton" />
  <%if (SessionUtil.ManageUserSearchCriteria != null)
    {%>
  <input class="secondaryButton" type="button" value="<%=Iata.IS.Web.AppSettings.BackText %>"
    onclick="javascript:location.href ='<%=Url.Action("SearchOrModify","Account") %>'" />
        <%
    }%>
</div>
