<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Reports ::
    <%=ViewData["CategoryName"] %> :: <%=ViewData["BillingTypeText"] %>
    :: Miscellaneous Invoice Summary
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<%Html.BeginForm("", "", FormMethod.Post, new { id = "ChargeSummaryReportId" }); %>

<h1> <span id="headers" > </span>
     </h1>

      <div>
        <% Html.RenderPartial("~/Areas/Reports/Views/Miscellaneous/ChargeSummary.ascx", ViewData); %>
    </div>

        <div />

    <div class="buttonContainer">
        <input type="submit" id="generateReport" class="primaryButton" value="Generate Report"
            onclick="MiscChargeSummary();" />
    </div>

     <%Html.EndForm(); %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">

 <script type="text/javascript" src="<%:Url.Content("~/Scripts/Reports/MiscellaneousReport.js")%>"></script>
    <script type="text/javascript">

        function MiscChargeSummary() {
            ValidateMiscChargeSummary("ChargeSummaryReportId");
        }
        $(document).ready(function () {
            var cat = '<%=ViewData["BillingTypeText"] %>';

            $('#headers').text(cat.concat(" - Invoice Summary Report"));
            registerAutocomplete('BilledEntityCode', 'BilledEntityCodeId', '<%:Url.Action("GetMemberList", "Data",  new  {  area = "" })%>', 0, true, null);

            $("#BilledEntityCode").change(function () {
                if ($("#BilledEntityCode").val() == '') {
                    $("#BilledEntityCodeId").val("");
                }
            });

            //To display USD as default selected currency in dropdown, as per Shambhu and Robin
            var billingTypeId = <%=ViewData["BillingTypeId"] %>;
            if(billingTypeId == <%:(int)Iata.IS.Model.Enums.BillingType.Receivables%>)
            {
                $('#CurrencyCode').val('840');
            }        

        });
    </script>

</asp:Content>

