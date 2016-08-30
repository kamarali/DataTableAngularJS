<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	 SIS :: Report :: Member Location 
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1>
    Member Location Search</h1>
  <h2>
    Search Criteria</h2>

<% Html.BeginForm("", "", FormMethod.Post, new { id = "MemberLocation" }); %>
    <h2>
          <span id="headers"></span>  </h2>
    <div>
        <% Html.RenderPartial("MemberLocationSearch"); %>
    </div>
    <div/>
    
    <div class="buttonContainer">
        <input type="button" id="generateButton" class="primaryButton" value="Generate Report" onclick="CheckValidation();" />
        
    </div>
     <%Html.EndForm(); %>
 
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script src="<%:Url.Content("~/Scripts/Member/MemberLocation.js")%>" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        function CheckValidation() {
            redirectToReport("MemberLocation");
        }
    </script> 

</asp:Content>

