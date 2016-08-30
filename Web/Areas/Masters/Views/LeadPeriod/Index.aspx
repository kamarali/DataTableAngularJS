<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.LeadPeriod>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	 SIS :: Master Maintenance :: General :: Lead period
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1>Lead Period</h1>
<% using (Html.BeginForm("Index", "LeadPeriod", FormMethod.Post))
       {%>
       <%: Html.AntiForgeryToken() %>
    <div>
        <% Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchLeadPeriod.ascx"); %>
    </div>
    <div class="buttonContainer">
    <%
           if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.LeadPeriodEditOrDelete))
           {%>
        <input type="button" class="primaryButton" value="Add" id="btnAdd" name="Add"  onclick="javascript:location.href = '<%:Url.Action("Create", "LeadPeriod")%>'" />
        <%
           }%>
        <input type="submit" class="primaryButton" value="Search" id="btnSearch" name="Search" />
    </div>
    <%} %>
     <h2>Search Results</h2>
    <%Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchLeadPeriodGrid.ascx", ViewData["LeadPeriodGrid"]); %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script type="text/javascript" src="<%:Url.Content("~/Scripts/Masters/LeadPeriodValidate.js")%>" ></script>
    <script type="text/javascript" src="<%:Url.Content("~/Scripts/MinAcceptableAmount.js")%>"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            BindEventForDate();
        });
    </script>
</asp:Content>
