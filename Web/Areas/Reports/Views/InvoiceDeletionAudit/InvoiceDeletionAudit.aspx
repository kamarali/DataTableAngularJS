<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" MasterPageFile="~/Views/Shared/Site.Master" %>
<asp:Content runat="server" ID="Title" ContentPlaceHolderID="TitleContent">
SIS :: Report :: Invoice Deletion Audit Trail Report
</asp:Content>

<asp:Content runat="server" ID="Script" ContentPlaceHolderID="Script">

<script src="<%:Url.Content("~/Scripts/Member/AuditTrail.js")%>" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">

        function CheckValidation() {

            ValidateInvoideDeletionReport("InvoiceDeletion","<%:Url.Action("ValidateBillingMonthYearPeriodSearch","InvoiceDeletionAudit")%>");
        }

        $(document).ready(function () {
            
            $.ajax({
                type: "POST",
                url: '<%:Url.Action("IsUserIdentificationInAuditTrail", "InvoiceDeletionAudit", new { area = "Reports" })%>',
                dataType: "json",
                success: function (result) {
                    if (result) {
                        $("#DeletedBy").attr("disabled", "disabled"); 
                    } else {
                        $("#DeletedBy").removeAttr("disabled"); 
                    }
                }
            });

        });
     
    </script> 

</asp:Content>

<asp:Content runat="server" ID="Main" ContentPlaceHolderID="MainContent">

<h1> Invoice Deletion Audit Trail Report</h1>
  <h2>
    Search Criteria</h2>

<% Html.BeginForm("", "", FormMethod.Post, new { id = "InvoiceDeletion" }); %>
    <h2>
          <span id="headers"></span>  </h2>
    <div>
        <% Html.RenderPartial("InvoiceDeletionAuditSearch"); %>
    </div>
    <div/>
    
    <div class="buttonContainer">
        <input type="button" id="generateButton" class="primaryButton" value="Generate Report" onclick="CheckValidation();" />
        
    </div>
     <%Html.EndForm(); %>
 


</asp:Content>

