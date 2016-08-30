<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.MaxAcceptableAmount>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
     SIS :: Master Maintenance :: General :: Maximum Value Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
       Maximum Value Setup
    </h1>
    <% using (Html.BeginForm("Index", "MaxAcceptableAmount", FormMethod.Post, new {id = "MaxAcceptableMasterSearch"}))
       {%>
       <%: Html.AntiForgeryToken() %>
    <div>
        <% Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchMaxAcceptableAmount.ascx"); %>
    </div>
    <div class="buttonContainer">
      <%if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.MinMaxAcceptableAmtEditOrDelete))
{%>
        <input type="button" class="primaryButton" value="Add" id="btnAdd" name="Add" onclick="javascript:location.href = '<%:Url.Action("Create", "MaxAcceptableAmount")%>'" />
        <%
}%>
        <input type="submit" class="primaryButton" value="Search" id="btnSearch" name="Search" />
    </div>
    <%} %>
     <h2>Search Results</h2>
    <%Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchMaxAcceptableAmountGrid.ascx", ViewData["MaxAcceptableAmountGrid"]); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script type="text/javascript" src="<%:Url.Content("~/Scripts/MinAcceptableAmount.js")%>"></script>
    <script src="<%:Url.Content("~/Scripts/Masters/MaxAcceptableAmountValidate.js")%>" type="text/javascript"></script>
    <script type="text/javascript" src="<%:Url.Content("~/Scripts/deleterecord.js")%>"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#EffectiveFromPeriod').watermark('PP-MMM-YYYY');
            $('#EffectiveToPeriod').watermark('PP-MMM-YYYY');

          $("#MaxAcceptableMasterSearch").validate({
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