<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.MinAcceptableAmount>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
     SIS :: Master Maintenance :: General :: Minimum Value Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
       Minimum Value Setup
    </h1>
    <% using (Html.BeginForm("Index", "MinAcceptableAmount", FormMethod.Post, new { id = "MinAcceptableMasterSearch" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <div>
        <% Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchMinAcceptableAmount.ascx"); %>
    </div>
    <div class="buttonContainer">
      <%if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.MinMaxAcceptableAmtEditOrDelete))
{%>
        <input type="button" class="primaryButton" value="Add" id="btnAdd" name="Add" onclick="javascript:location.href = '<%:Url.Action("Create", "MinAcceptableAmount")%>'" />
        <%
}%>
        <input type="submit" class="primaryButton" value="Search" id="btnSearch" name="Search" />
    </div>
    <%} %>
     <h2>Search Results</h2>
    <%Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchMinAcceptableAmountGrid.ascx", ViewData["MinAcceptableAmountGrid"]); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script type="text/javascript" src="<%:Url.Content("~/Scripts/MinAcceptableAmount.js")%>"></script>
    <script type="text/javascript" src="<%:Url.Content("~/Scripts/Masters/MinAcceptableAmountValidate.js")%>" ></script>
    <script type="text/javascript" src="<%:Url.Content("~/Scripts/deleterecord.js")%>"></script>
    <script type="text/javascript">

      $(document).ready(function () {
                 
        BindEventOnCreateMinAccepatableAmount();
        InitialiseInvoiceHeader('<%:Url.Action("GetRejectionReasonForTransactionType", "Data", new { area = "" })%>');
        $("#TransactionTypeId").change();
        rejectionReasonCode = '<%: ViewData["RejectionReasonCode"]%>';

        $('#EffectiveFromPeriod').watermark('PP-MMM-YYYY');
        $('#EffectiveToPeriod').watermark('PP-MMM-YYYY');

        // Validate Search form to check whether Effective from and Effective To period values are in proper format
        $("#MinAcceptableMasterSearch").validate({
          rules: {
            EffectiveFromPeriod: {
              maxlength: 11,
              ValidFromToPeriod: true
            },
            EffectiveToPeriod: {
              maxlength: 11,
              ValidFromToPeriod: true
            }
          },
          messages: {
            EffectiveFromPeriod: "Effective From Period Should Be Valid",
            EffectiveToPeriod: "Effective To Period Should Be Valid"
          },
          invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
          }
        });
        });
    </script>
</asp:Content>