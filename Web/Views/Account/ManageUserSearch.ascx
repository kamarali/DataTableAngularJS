<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.ManageUsers>" %>
<%@ Import Namespace="Iata.IS.Model.MemberProfile.Enums" %>
<% using (Html.BeginForm("ManageUsers", "Account", FormMethod.Post, new { id = "ManageUsers" }))
   { %>
<h2>
  Search Criteria</h2>
<div class="searchCriteriaMedium" style="width:70%;">
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
            Members:<span style="color: Red">*</span>
          </label>
          <%:Html.HiddenFor(m => m.MemberId, new { style = "width:200px;" })%>
          <%:Html.TextBoxFor(m => m.MemberName, new { @class = "autocComplete" })%>
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
    </div>
    <div class="clear">
    </div>
  </div>
</div>
<div class="clear">
</div>
<div>
  <%@ import namespace="Trirand.Web.Mvc" %>
  <%@ import namespace="Iata.IS.Model.MemberProfile.Enums" %>
  <%-- <%= Html.Trirand().JQGrid(Model.OrdersGrid, "SearchGrid") %>--%>
</div>
<script type="text/javascript" src="<%:Url.Content("~/Scripts/User/SearchOrModify.js")%>"></script>
<script type="text/javascript">

  function CheckValidation() {
    ManageUsersValidate("ManageUsers");
  }
           
</script>
<% } %>