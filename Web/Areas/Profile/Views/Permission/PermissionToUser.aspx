<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MemberProfile.PermissionToUser>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS ::  Profile :: Assign Permission To User
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script src="<%=Url.Content("~/Scripts/jquery.tree.js")%>" type="text/javascript"></script>
  <script src="<%=Url.Content("~/Scripts/plugins/jquery.tree.checkbox.js")%>" type="text/javascript"></script>
  <script src="<%=Url.Content("~/Scripts/PermissionToUser.js")%>" type="text/javascript"></script>
  <script type="text/javascript">
      $('#UserName').focus();
      $('#UserName').flushCache();
      $('#CopyUserName').flushCache();

    registerAutocomplete('UserName', 'UserId', '<%:Url.Action("GetUserList", "Permission", new { area = "" })%>', 0, true, function (selectedId) { UserName_AutoCompleteValueChange(selectedId); });
    registerAutocomplete('CopyUserName', 'CopyUserId', '<%:Url.Action("GetUserListForCopyPermission", "Permission", new { area = "" })%>', 0, true, null);

    $('#UserName').change(function () {
      
        if ($('#UserName').val() == ' ' || $('#UserName').val() == '') {
            
            $("select[id$=TemplateId] > option").remove();
            $('#TreeChoosePermission').empty();
        }


        $('#CopyUserName', '#content').val("");
        $('#CopyUserName').flushCache();
        $('#TemplateId').flushCache();

        $('#SelectedIDs').flushCache();
        $('#UserCategoryId').flushCache();
    });

    // Visible off Submit button in case user field blank
     var AssignedUserID = $("#UserId").val();
     if (AssignedUserID == '') {
       $("#btnSave").hide();
     } else {
       $("#btnSave").show();
     }

    
    SetTemplatePermissionListUrl('<%:Url.Action("GetTemplatePermissionList", "Permission", new { area = "Profile"}) %>');
    SetUserCategoryIdByUserNameUrl('<%:Url.Action("GetUserCategoryIdByUserName", "Permission", new { area = "Profile"}) %>');
    SetTemplateNameByUserCategoryIdUrl('<%:Url.Action("GetTemplateNameByUserCategoryId", "Permission", new { area = "Profile"}) %>');
    SetAssignedPermissionListByUserIdUrl('<%:Url.Action("GetAssignedUserPermissionList", "Permission", new { area = "Profile"}) %>');
    SetPostUrl('<%:Url.Action("GetPermissionListToUser", "Permission", new { area = "Profile"}) %>?userCategory');
  </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Assign Permission To User
  </h1>
  <% using (Html.BeginForm("PermissionToUser", "Permission", FormMethod.Post, new { id = "frmTree" }))
     { %>
     <%: Html.AntiForgeryToken() %>
  <div >
    <div class="searchCriteria">
      <div class="solidBox">
        <div class="fieldContainer horizontalFlow" style="height: 40px;">
          <div class="buttonContainer">
            <div style="float: left; width: 280px;">
              <b>User Name:</b>
              <%:Html.TextBoxFor(m => m.UserName, new { @class = "autocComplete", style = "width:250px;" })%>
              <%= Html.HiddenFor(m => m.UserId)%>
            </div>
            <div style="float: left; width: 170px;">
              <b>Template: </b>
              <br />
              <%: Html.DropDownListFor(m => m.TemplateId, new SelectList(new List<string>()))%>
            </div>
            <div style="float: left; width: 170px;">
              <br />
              <input type="button" id="btnAddToPermissionList" value="Add Permission" style="width: 150px;" class="primaryButton" />
            </div>
            <div style="width: 160px;">
              <br />
              <input type="button" id="btnReplacePermissionList" value="Replace Permission" style="width: 150px;" class="primaryButton" />
            </div>
          </div>
        </div>
      </div>
    </div>
    <h2>
      Copy User Permission
    </h2>
    <div class="searchCriteria">
      <div class="solidBox">
        <div class="fieldContainer horizontalFlow" style="height: 30px;">
          <div style="float: left; width: 550px;" class="buttonContainer">
            <%:Html.TextBoxFor(m => m.CopyUserName, new { @class = "autocComplete", style = "width:250px;" })%>
            <%= Html.HiddenFor(m => m.CopyUserId)%>
            <input type="button" id="btnCopyUserPermission" value="Copy Permission" style="width: 150px; vertical-align:top;" class="primaryButton" />
          </div>
        </div>
      </div>
    </div>
    <br />
    <h3>
      Permission List For Selected User :
    </h3>
    <%= Html.HiddenFor(m => m.SelectedIDs)%>
    <%= Html.HiddenFor(m => m.UserCategoryId)%>
    <div class="solidBox" id="TreeChoosePermission" style="height: 400px; width: 40%; overflow: auto;">
    </div>
    <br />
    <div>
      <input type="button" value="Save" id="btnSave" class="primaryButton" />
      &nbsp;&nbsp;&nbsp;
      <input type="button" value="Cancel" id="btnCancel" class="primaryButton" onclick="javascript:location.href ='<%=Url.Action("Index","Home",new {area=""}) %>'" />
    </div>
  </div>
  <% } %>
</asp:Content>
