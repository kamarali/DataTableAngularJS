<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Web.Reports.SuspendedInvoice.MemberSuspendedInvoice>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Reports :: Financial Controller :: Suspended Billings
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
       Suspended Billings</h1>
    <% using (Html.BeginForm("MemberSuspendedInvoices", "ManageSuspendedInvoices", FormMethod.Post, new { @id = "viewSuspendedInvoces" }))
       {%>
    <div>
        <% Html.RenderPartial("SearchSuspendedInvoces"); %>
    </div>
    <div class="buttonContainer">
        <input type="submit" id="generateButton" class="primaryButton" value="Generate Report"
            onclick="CheckValidation();" />
    </div>
    <%} %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script type="text/javascript" src="<%:Url.Content("~/Scripts/Reports/ManageSuspendedInvoices.js")%>"></script>
    <script type="text/javascript">
        function CheckValidation() {
            GenerateSuspendedInvoiceReport("viewSuspendedInvoces");
        }
        $(document).ready(function () {

            //registerAutocomplete('BilledEntityName', 'BilledEntityCode', '<%:Url.Action("GetMemberList", "Data",  new  {  area = "" })%>', 0, true, null);
            $("#BilledEntityName").change(function () {
                if ($("#BilledEntityName").val() == '') {
                    $("#BilledEntityCode").val("");
                }
            });

        });
    </script>
</asp:Content>
