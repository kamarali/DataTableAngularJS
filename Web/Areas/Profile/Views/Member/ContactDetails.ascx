<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.Contact>" %>
<% Html.BeginForm("Contacts", "Member", FormMethod.Post, new { id = "contacts" }); %>
<%: Html.AntiForgeryToken() %>
<div class="fieldContainer horizontalFlow tempEdit">
  <div class="topLine">
    <%: Html.Hidden("hiddemEmail") %>
    <%: Html.Hidden("UserCategory",SessionUtil.UserCategory)%>
    <%: Html.ProfileFieldFor(contactModel => contactModel.EmailAddress, "Email Address", SessionUtil.UserCategory, new Dictionary<string, object> { { "maxLength", "100" } ,{"class","emailAddress"}})%>
    <%: Html.ProfileFieldFor(contactModel => contactModel.SalutationId, "Salutation", SessionUtil.UserCategory, new Dictionary<string, object> { { "id", "Salutation" } })%>
    <%: Html.ProfileFieldFor(contactModel => contactModel.FirstName, "First Name", SessionUtil.UserCategory, new Dictionary<string, object> { { "maxLength", "100" }})%>
    <%: Html.Hidden("FirstNameHidden") %>
    <%: Html.ProfileFieldFor(contactModel => contactModel.LastName, "Last Name", SessionUtil.UserCategory, new Dictionary<string, object> { { "maxLength", "100" }})%>
    <%: Html.Hidden("LastNameHidden") %>
    <%: Html.ProfileFieldFor(contactModel => contactModel.StaffId, "Staff ID", SessionUtil.UserCategory, new Dictionary<string, object> { { "maxLength", "15" } ,{"class","alphaNumeric"}})%>
  </div>
</div>
<div class="fieldContainer horizontalFlow tempEdit">
  <div>
    <%: Html.ProfileFieldFor(contactModel => contactModel.PositionOrTitle, "Position/Title", SessionUtil.UserCategory, new Dictionary<string, object> { { "maxLength", "100" }})%>
    <%: Html.ProfileFieldFor(contactModel => contactModel.Division, "Division", SessionUtil.UserCategory, new Dictionary<string, object> { { "maxLength", "100" }})%>
    <%: Html.ProfileFieldFor(contactModel => contactModel.Department, "Department", SessionUtil.UserCategory, new Dictionary<string, object> { { "maxLength", "100" }})%>
    <%: Html.TextBoxFor(contactModel => contactModel.Id, new { @id = "contactId", @class = "hidden" })%>
  </div>
</div>
<div class="fieldContainer horizontalFlow tempEdit">
  <div>
    <%: Html.ProfileFieldFor(contactModel => contactModel.LocationId, "Location ID", SessionUtil.UserCategory, profileMemberId: Convert.ToInt32(ViewData["SelectedMemberId"]))%>
    <%: Html.ProfileFieldFor(contactModel => contactModel.AddressLine1, "Address Line 1", SessionUtil.UserCategory, new Dictionary<string, object> { { "maxLength", "70" },{"id","conAddressLine1"}})%>
    <%: Html.ProfileFieldFor(contactModel => contactModel.AddressLine2, "Address Line 2", SessionUtil.UserCategory, new Dictionary<string, object> { { "maxLength", "70"},{"id","conAddressLine2"}})%>
    <%: Html.ProfileFieldFor(contactModel => contactModel.AddressLine3, "Address Line 3", SessionUtil.UserCategory, new Dictionary<string, object> { { "maxLength", "70" },{"id","conAddressLine3"}})%>
    <%: Html.ProfileFieldFor(contactModel => contactModel.CityName, "City Name", SessionUtil.UserCategory, new Dictionary<string, object> { { "maxLength", 50 },{"id","conCityName"}})%>
  </div>
</div>
<div class="fieldContainer horizontalFlow tempEdit">
  <div>
    <%: Html.ProfileFieldFor(contactModel => contactModel.PostalCode, "Postal Code", SessionUtil.UserCategory, new Dictionary<string, object> {{ "maxLength", "50" },{"id","conPostalCode"}})%>
    <%: Html.ProfileFieldFor(contactModel => contactModel.CountryId, "Country Name", SessionUtil.UserCategory, new Dictionary<string, object> {{"id","conCountryId"}})%>
    <%: Html.ProfileFieldFor(contactModel => contactModel.SubDivisionName, "Sub Division Name", SessionUtil.UserCategory, new Dictionary<string, object> {{ "maxLength", "50" },{"id","conSubDivisionName"}})%>
    <%: Html.ProfileFieldFor(contactModel => contactModel.IsActive, "Active", SessionUtil.UserCategory, new Dictionary<string, object> {{"id","conIsActive"}})%>
  </div>
</div>
<div class="fieldContainer horizontalFlow tempEdit">
  <div>
    <%: Html.ProfileFieldFor(contactModel => contactModel.PhoneNumber1, "Phone Number 1", SessionUtil.UserCategory, new Dictionary<string, object> {{ "maxLength", "50" }})%>
    <%: Html.ProfileFieldFor(contactModel => contactModel.PhoneNumber2, "Phone Number 2", SessionUtil.UserCategory, new Dictionary<string, object> {{ "maxLength", "50" }})%>
    <%: Html.ProfileFieldFor(contactModel => contactModel.MobileNumber, "Mobile Number", SessionUtil.UserCategory, new Dictionary<string, object> {{ "maxLength", "50" }})%>
    <%: Html.ProfileFieldFor(contactModel => contactModel.FaxNumber, "Fax Number", SessionUtil.UserCategory, new Dictionary<string, object> {{ "maxLength", "50" }})%>
    <%: Html.ProfileFieldFor(contactModel => contactModel.SitaAddress, "SITA Address", SessionUtil.UserCategory, new Dictionary<string, object> {{ "maxLength", "100" },{"class","alphaNumeric"}})%>

    <%--SCP213365 - CONTACTS TAB--%>
   <%-- <%: Html.Hidden("MemberId", ViewData["SelectedMemberId"])%>--%>
  </div>
</div>
<div class="clear">
</div>
       <% if (ViewData["SelectedMemberId"] == "0")
        {%>
<div class="buttonContainer">
  <input class="primaryButton" id="SaveContacts" type="submit" value="Save Contact"
    disabled="disabled" />
</div>
<%
        }%>
<%
        else
        {%>
        <%--SCP213365 - CONTACTS TAB--%>
        <%: Html.Hidden("MemberId", ViewData["SelectedMemberId"])%>
<div class="buttonContainer">
  <input class="primaryButton ignoredirty" type="submit" value="Save Contact" />
</div>
<%
        }
  Html.EndForm(); %>