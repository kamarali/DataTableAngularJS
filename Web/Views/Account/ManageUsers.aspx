<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MemberProfile.ManageUsers>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Profile :: Manage Users
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h1>Manage Users</h1>

<div>
    <%
      using (Html.BeginForm("ManageUsers", "Account", FormMethod.Get, new { id = "ManageUsers" }))
      {
        Html.RenderPartial("ManageUserSearch", Model);
      } 
    %>
  </div>

  <h2>
    Search Results</h2>

    <div>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.SearchGrid]); %>
</div>
  

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script type="text/javascript">
   
  $.ajaxSetup({ cache: false });
  function PostData(datatosend, mode) {
    var myForm = document.createElement("form");
    myForm.method = "post";
    myForm.action = "ManageUsers";
    var myInput = document.createElement("input");
    myInput.setAttribute("name", mode);
    myInput.setAttribute("value", datatosend);
    myForm.appendChild(myInput);
    document.body.appendChild(myForm);
    myForm.submit();
    document.body.removeChild(myForm);
  };

  registerAutocomplete('MemberName', 'MemberId', '<%:Url.Action("GetAllMemberList", "Data", new { area = "" })%>', 0, true, null);

  $('#UserCategoryId').change(function () {
    var CategoryId = $("#UserCategoryId").val();
    if (CategoryId != null) {
      if (CategoryId == '4') {
        $('#MemberName').show();
        $('#divMember').show();
        $('#ddVisible').val("1");
        $('#ddVisible').hide();
      }
      else {
        $('#MemberName').hide();
        $('#divMember').hide();
        $('#MemberName').val('');
        $('#MemberId').val('');
        $('#ddVisible').val("0");
        $('#ddVisible').hide();
      }
    }

  });


  $(document).ready(function () {
    $('#UserCategoryId').focus();
    var CategoryId = $("#UserCategoryId").val();
    if (CategoryId != null) {
      if (CategoryId == '4') {
        $('#MemberName').show();
        $('#divMember').show();
        $('#ddVisible').val("1");
        $('#ddVisible').hide();
      } else {
        $('#MemberName').hide();
        $('#divMember').hide();
        $('#MemberName').val('');
        $('#MemberId').val('');
        $('#ddVisible').val("0");
        $('#ddVisible').hide();
      }
    }
  });

   
    </script>
</asp:Content>
