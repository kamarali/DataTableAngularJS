<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MemberProfile.Permission>" %>
<%@ Import Namespace="Iata.IS.Model.MemberProfile.Enums" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  New Permission Template
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script src="<%=Url.Content("~/Scripts/jquery.tree.js")%>" type="text/javascript"></script>
  <script src="<%=Url.Content("~/Scripts/plugins/jquery.tree.checkbox.js")%>" type="text/javascript"></script>
  <script src="<%=Url.Content("~/Scripts/NewPermissionTemplate.js")%>" type="text/javascript"></script>
  <script type="text/javascript">

    SetPostUrl('<%:Url.Action("GetPermissionListForTreeView", "Permission", new { area = "Profile"}) %>?userCategory')
    SetTemplatePermissionListUrl('<%:Url.Action("GetTemplatePermissionList", "Permission", new { area = "Profile"}) %>')
    SetUserCategory('<%: Convert.ToInt32(SessionUtil.UserCategory) %>');
    $(document).ready(function () {
        $('#UserCategoryId').val('<%= ViewData["UserCategory"].ToString() %>');
        var UserCategory = $('#UserCategoryId').val();
//        if ($.trim(UserCategory) != '') {
//            GetTreeviewHierarchy(UserCategory);
//        }
    });
  </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h1>Manage Permission Template</h1>

    <% using (Html.BeginForm("NewPermissionTemplate", "Permission", FormMethod.Post, new { id = "frmTree" }))
       { %>
    <%: Html.AntiForgeryToken() %>
    <div class="searchCriteria" >

     <div class="solidBox">
        <div class="fieldContainer horizontalFlow" style=" height:40px;">
            <div style="padding-left:50px;">
        <div style="float:left; width:250px;" > 
        <b>Template Name</b> <br />
            <%= Html.TextBoxFor(m => m.TemplateName)%>
            <%= Html.HiddenFor(m => m.TemplateID)%>
        </div>
        
        <div>
        <%--<% if (SessionUtil.UserCategory == UserCategory.SisOps)
           { %>
          <b>User Category </b> <br />
          <%= Html.UserCategoryDropdownListFor(model => model.UserCategoryId, "0", new { style = "width:200px;" })%>
          
        <% }
           else
           {  %>
           
            <br /><br />
        <% } %>--%>
        <%= Html.HiddenFor(model=> model.UserCategoryId) %>
        </div>
         
      
         
         
     
        </div>
        </div>
     </div>

    </div>

    <div style=" padding-left:50px; padding-top:10px; "> 
      
     <h3 id="lblselectLabel"> Select Permission From The List </h3>
        <%= Html.HiddenFor(m => m.SelectedIDs)%>
        
        <%--<div id="TreeChoosePermission" style="height: 400px; width: 100%; overflow:auto;" >
        </div>--%>
        <div class="solidBox" id="TreeChoosePermission" style="height: 400px; width: 40%; overflow: auto;">
    </div>
        <br />
        <div>
        <div class="buttonContainer">
           <input type="button" value="Save" id="btnSubmit" class="primaryButton" onclick="GetButtonClick();" />
            &nbsp;&nbsp;&nbsp;
            <input type="button" value="Back" id="btnCancel" class="secondaryButton" onclick="javascript:location.href ='<%=Url.Action("ManagePermissionTemplate","Permission") %>'" />
            &nbsp;&nbsp;&nbsp;
            <%-- <input class="secondaryButton" type="button" value="<%=Iata.IS.Web.AppSettings.BackText %>"
    onclick="javascript:location.href ='<%=Url.Action("ManagePermissionTemplate","Permission") %>'" />--%>
        </div>            

        </div>
    </div>
    <% } %>

</asp:Content>
