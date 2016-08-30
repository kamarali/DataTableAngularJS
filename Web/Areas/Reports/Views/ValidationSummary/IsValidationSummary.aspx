<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.Common.ValidationExceptionSummary>" %>



<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
       SIS :: Reports :: <%=ViewData["ValidationCategory"] %> :: IS Validation Summary 
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        $(document).ready(function () {
            var cat = '<%=ViewData["ValidationCategory"].ToString() %>';

            $('#headers').text(cat.concat(": IS Validation Summary"));
        });

</script>

     <% Html.BeginForm("", "", FormMethod.Post, new { id = "IsValidationSummary" }); %>
    <h2>
          <span id="headers"></span>  </h2>
    <div>
        <% Html.RenderPartial("IsValidationSummarySearch"); %>
    </div>
    <div/>
    
    <div class="buttonContainer">
        <input type="submit" id="generateButton" class="primaryButton" value="Generate Report" />
        
    </div>
     <%Html.EndForm(); %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">

 <script type="text/javascript" src="<%:Url.Content("~/Scripts/ValidationSummary.js")%>"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            
            var Category = '<%=ViewData["ValidationCategory"].ToString() %>';
            validateExceptionSummary("IsValidationSummary", Category);
        });
//        function CheckValidation() {
//            var Category = '<%=ViewData["ValidationCategory"].ToString() %>';
//            validateExceptionSummary("IsValidationSummary",Category);
//        }

    </script>

</asp:Content>
   