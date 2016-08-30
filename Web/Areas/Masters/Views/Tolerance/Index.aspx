<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.Tolerance>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
     SIS :: Master Maintenance :: General :: Tolerance Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <h1>
        Tolerance Setup
    </h1>
    <% using (Html.BeginForm("Index", "Tolerance", FormMethod.Post, new { id = "SearchTolerance" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <div>
        <% Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchTolerance.ascx"); %>
    </div>
    <div class="buttonContainer">
      <%if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.ToleranceEditOrDelete)) {%>
        <input type="button" class="primaryButton" value="Add" id="btnAdd" name="Add" onclick="javascript:location.href = '<%:Url.Action("Create", "Tolerance")%>'" />
        <% }%>
        <input type="submit" class="primaryButton" value="Search" id="btnSearch" name="Search" />
    </div>
    <%} %>
     <h2>Search Results</h2>
    <%Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchToleranceGrid.ascx", ViewData["ToleranceGrid"]); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script src="<%:Url.Content("~/Scripts/Masters/ToleranceValidate.js")%>" type="text/javascript"></script>
  <script type="text/javascript">
    $(document).ready(function () {
        $('#EffectiveFromPeriod').watermark('PP-MMM-YYYY');
        $('#EffectiveToPeriod').watermark('PP-MMM-YYYY');

      $("#SearchTolerance").validate({
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