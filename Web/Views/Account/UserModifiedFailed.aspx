<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="ModifyAccount" ContentPlaceHolderID="TitleContent" runat="server">
	Modify Account
</asp:Content>

<asp:Content ID="ModifyAccountFailedContent" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Modify Failed</h2>
    <% 
      if (string.IsNullOrEmpty(TempData["ActivationFailedError"] == null ? "" : TempData["ActivationFailedError"].ToString()))
       {%>
    <p>Account could not updated. An error occured while updating your account.</p>   
    <%
       }
       else
       {%>
    <p>Unable to deactivate the user as the user is the only contact in one of the contact type assigned to it.</p>
    <%
       }%>
</asp:Content>


