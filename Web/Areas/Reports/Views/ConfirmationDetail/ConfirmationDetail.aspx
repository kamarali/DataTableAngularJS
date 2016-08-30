<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Reports.ConfirmationDetails.ConfirmationDetailModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	SIS :: Report :: Confirmation Details 
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1>
   BVC Details</h1>
  <h2>
    Search Criteria</h2>
  
<% Html.BeginForm("", "", FormMethod.Post, new { id = "ConfirmationDetailReport" }); %>
    <h2>
          <span id="Span1"></span>  </h2>
    <div>
        <% Html.RenderPartial("ConfirmationDetailSearch"); %>
    </div>
    <div/>
    
    <div class="buttonContainer">
        <%--<input type="submit" id="Button1" disabled="disabled" class="primaryButton" value="Generate Report" onclick="CheckValidation(this);" />--%>
         <input type="submit" id="Button2" class="primaryButton" value="Download To CSV" onclick="CheckValidation(this);" />
        
    </div>
     <%Html.EndForm(); %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script type="text/javascript" src="<%:Url.Content("~/Scripts/ConfirmationDetail.js")%>"></script>
    <script type="text/javascript" >
        function CheckValidation(e) {          
            Pax_ConfirmationReport("ConfirmationDetail", e.id, '<%: Url.Action("ExportToCSV", "ConfirmationDetail") %>', '<%:Url.Action("ValidateBillingMonthYearPeriodSearch","ConfirmationDetail")%>');
        }
    </script> 

</asp:Content>
