<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.SourceCode>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Source Code
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Manage Source Code</h2>
    <% using (Html.BeginForm("Index", "SourceCode", FormMethod.Post))
       {%>
    <div>
        <% Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchSourceCode.ascx"); %>
    </div>
    <div class="buttonContainer">
        <input type="button" class="primaryButton" value="Add" id="btnAdd" name="Add" onclick="javascript:location.href = '<%:Url.Action("Create","SourceCode") %>'" />
        <input type="submit" class="primaryButton" value="Search" id="btnSearch" name="Search" />
    </div>
    <%} %>
     <h2>Search Results</h2>
    <%Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchSourceCodeGrid.ascx", ViewData["SourceCodeGrid"]); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    
</asp:Content>