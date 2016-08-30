<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MemberProfile.Permission>" %>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
     
  <script type="text/javascript">
      function resetForm() {
          $("#TemplateName").val("");
          $("#UserCategoryId").val("");
      }

      function deleteRecord(methodName, value, gridId) {
          $('#successMessageContainer').hide();

          if (confirm("Are you sure you want to delete this record?")) {
              $.ajax({
                  type: "POST",
                  url: methodName + "/" + value,
                  success: function (result) {
                      $('#errorContainer').hide();
                      if (result.IsFailed == false) {
                          // Toggle message containers.          
                          showClientSuccessMessage(result.Message);
                      }
                      else {
                          showClientErrorMessage(result.Message);
                      }
                      $(gridId).trigger("reloadGrid");
                  }
              });
          }

      }



  </script>

  <%= ScriptHelper.GenerateGridEditDeleteScript(Url, ViewDataConstants.SearchGrid, Url.Action("EditTemplate", "Permission"), "DeleteTemplate")%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1>Manage Permission Template</h1>
<h2>Search Criteria</h2>
<div>
    <%
        using (Html.BeginForm("ManagePermissionTemplate", "Permission", FormMethod.Get, new { id = "PermissionSearchForm" }))
      {
          Html.RenderPartial("PermissionSearch", Model);
      } 
    %>
  </div>

  <h2>
    Search Results</h2>
<div>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.SearchGrid]); %>
</div>
  
</asp:Content>


<asp:Content ID="registerTitle" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Profile :: Manage User Permissions :: Manage Permission Template 
</asp:Content>