<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.LegalArchive.LegalArchiveSearchCriteria>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
   SIS :: Master Maintenance :: General :: Legal Archive Retrieval :: Search and Retrieve
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script type="text/javascript" src="<%:Url.Content("~/Scripts/LegalArchive/SearchLegalArchive.js")%>"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Legal Archive - Search and Retrieve Screen</h1>
  <div>
    <%
      using (Html.BeginForm("Search", "ArchiveRetrieval", FormMethod.Post, new { id = "LegalArchiveSearchForm" }))
      {%>
      <%: Html.AntiForgeryToken() %>
      <%
        Html.RenderPartial("LegalArchiveSearchControl", Model);
      } 
    %>
  </div>
   <h2>Search Results</h2>
   <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.LegalArchiveSearchGrid]); %>
   <div class="buttonContainer">
    <input class="primaryButton" type="submit" onclick="RetriveArchives('#<%:ControlIdConstants.LegalArchiveSearchGrid %>','<%:Url.Action("RetriveArchive","ArchiveRetrieval") %>');" value="Retrieve Selected" />
    <input class="primaryButton" type="submit" onclick="RetriveAll('#<%:ControlIdConstants.LegalArchiveSearchGrid %>','<%:Url.Action("RetriveAll","ArchiveRetrieval") %>');" value="Retrieve All" />
    
    <% if(Html.IsAuthorized(Iata.IS.Business.Security.Permissions.General.LegalArchive.DownloadRetrievedInv))
       {%>
    <%:Html.LinkButton("View Retrieved Invoices", Url.Action("DownloadRetrievedFiles", "ArchiveRetrieval"))%>
    <%
       }%>
</div>
</asp:Content>
