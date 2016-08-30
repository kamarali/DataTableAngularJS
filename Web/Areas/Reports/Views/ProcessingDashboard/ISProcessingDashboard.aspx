<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
 SIS :: Reports :: Processing Dashboard 

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    IS Processing Dashboard</h1>
    <!-- Adding placeholder for Processing Dashboard -->    
    <div id="caltabs" class="solidBox">
      <ul>
        <li>
          <input type="hidden" id="isInvoiceTabClicked" value="false" />
          <%: Html.ActionLink("Invoice Status", "InvoiceStatus", "ProcessingDashboard", new { area = "Reports" }, null)%></li>
        <li>
          <%: Html.ActionLink("File Status", "FileStatus", "ProcessingDashboard", new { area = "Reports"  }, null)%></li>        
      </ul>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  
    <script src="<%:Url.Content("~/Scripts/jquery.json-2.2.js")%>" type="text/javascript"></script>
    <%--CMP #675: Progress Status Bar for Processing of Billing Data Files--%>
    <script src="<%:Url.Content("~/Scripts/ProgressBar.js")%>" type="text/javascript"></script>

  <script type="text/javascript">
      $(function () {
          $('span').removeClass('link');

          $("#caltabs").tabs({
              load: function (event, ui) {
              }
          }).on('tabsactivate', function (event, ui) {
              $('.searchCriteria').remove();
              $('.removelisterror').remove();

          });
      });

    // SCP255637: Sorting Billing Period in the Processing Dashboard does not work properly
    function showInDateFormat(cellValue, options, rowObject) {
        return rowObject.BillingPeriod;
    }

    //280744 - MISC UATP Exchange Rate population/validation during error 
    // Desc: Function to display currency amount as NULL/blank instead of 0.
    function clearanceAmountNullFormatter(cellValue, options, cellObject) {
        if (cellValue == null || cellValue == '' || cellValue == 0 || cellValue == '0.000') {
            return '';
        }
        else {
            var formattedVal = Number(cellValue).toFixed(3);
            var n = formattedVal.toString().split(".");
            //Comma-fies the first part
            n[0] = n[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            //Combines the two sections
            return n.join(".");
        }
    }

  </script>
</asp:Content>
