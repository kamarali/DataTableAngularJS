<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Admin User Search 
</asp:Content>

<asp:Content ID="script" ContentPlaceHolderID="Script" runat="server">
<script type="text/javascript">
    $(document).ready(function () {

       
        $('#searchheader td').each(function () {
          
            $(this).text("Hi");
        }); 

    });
</script>
</asp:Content>
<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Admin User Search</h2>
    <p>
        Use the form below to Search for and modify a selected user.
    </p>

    <% Html.RenderPartial("~/Views/Account/UserSearchControl.ascx"); %>
</asp:Content>

