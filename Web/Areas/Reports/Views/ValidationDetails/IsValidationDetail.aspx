<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.Common.ValidationExceptionSummary>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	SIS :: Reports :: <%=ViewData["ValidationCategory"] %> :: IS Validation Details 
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

      <script type="text/javascript">
          $(document).ready(function () {
              var cat = '<%=ViewData["ValidationCategory"].ToString() %>';

              $('#headers').text(cat.concat(": IS Validation Detail"));
          });

</script>

     <% Html.BeginForm("", "", FormMethod.Post, new { id = "IsValidationDetail" }); %>
    <h2>
          <span id="headers"></span>  </h2>
    <div>
        <% Html.RenderPartial("IsValidationDetailSearch",ViewData); %>
    </div>
    <div/>
    
    <div class="buttonContainer">
        <input type="submit" id="generateButton" class="primaryButton" value="Generate Report" onclick="CheckValidation();" />
        
    </div>
     <%Html.EndForm(); %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">


 <script type="text/javascript" src="<%:Url.Content("~/Scripts/ValidationDetail.js")%>"></script>
    <script type="text/javascript">

        function CheckValidation() {
            var Category = '<%=ViewData["ValidationCategory"].ToString() %>';
            validateExceptionSummary("IsValidationDetail", Category);
        }

    </script>

</asp:Content>
