<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MiscUatp.ValidationErrorCorrection>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: UATP :: Receivables :: Validation Error Correction
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <h2>
    Search Criteria
 </h2>

  <div>
    <% using (Html.BeginForm("Index", "MUValidationErrorCorrection", FormMethod.Get, new { id = "frmValidationError" }))
       { %>
    <% Html.RenderPartial("MUValidationErrorCorrectionSeach", Model); %>
    <% } %>
  </div>

  <script type="text/javascript">
    $(document).ready(function () {
      registerAutocomplete('BilledMember', 'BilledMemberId', '<%:Url.Action("GetMemberList", "Data",  new  {  area = "" })%>', 0, true, null);
    });
  </script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
</asp:Content>
