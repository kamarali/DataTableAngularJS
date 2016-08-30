<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.Permission>" %>
<%@ Import Namespace="Iata.IS.Model.MemberProfile.Enums" %>
<script type="text/javascript" language="javascript">
    $(document).ready(function () { $('#TemplateName').focus(); });
</script>
<div class="searchCriteria">
  <div class="solidBox">
    <div class="fieldContainer horizontalFlow">
      <div>
        <div style=" float:left; width:250px;" >
          <label>
            Template Name:</label>
                  <%= Html.TextBoxFor(m => m.TemplateName)%>
          
        </div>
      <div>
       <%-- <% if (SessionUtil.UserCategory == UserCategory.SisOps)
           { %>
          <b>User Category </b> <br />
          <%= Html.UserCategoryDropdownListFor(model => model.UserCategoryId, "0", new { style = "width:200px;" })%>
        <% }
           else
           {  %>
            <br /><br />
        <% } %>--%>
        </div>
      </div>
       
    </div>
    <div class="clear">
    </div>
  </div>
</div>
<div class="buttonContainer">
  
  <input class="primaryButton" type="submit" value="Search" />
  
  <input class="primaryButton" type="button" value="Add New Template" onclick="javascript:location.href = '<%:Url.Action("NewPermissionTemplate", "Permission")%>';" />
  <input class="secondaryButton" type="button" onclick="resetForm();" value="Clear" />
</div>
<div class="clear">
</div>

