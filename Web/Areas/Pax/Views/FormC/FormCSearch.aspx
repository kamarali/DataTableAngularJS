<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.SearchCriteria>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger ::
  <%: ViewData[ViewDataConstants.BillingType].ToString() %>
  :: Manage Sampling Form C
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Script" runat="server">
  <script src="<%:Url.Content("~/Scripts/Pax/FormCSearch.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/deleteFormC.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/ValidateFormC.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/downloadZip.js")%>" type="text/javascript"></script>
  <script type="text/javascript">
      $(document).ready(function () {
          /*CMP #596: Length of Member Accounting Code to be Increased to 12 
          Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
          Ref: FRS Section 3.4 Table 15 Row 4 */
          registerAutocomplete('BilledMemberText', 'BilledMemberId', '<%:Url.Action("GetMemberListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);

          $('#BilledMemberText').blur(function (e) {
              $('#BilledMemberText').val('');
          });
      InitializeSubmitMethod('<%:Url.Action("SubmitFormC", "FormC") %>');
      InitializePresentMethod('<%:Url.Action("PresentFormC", "FormC") %>');
    });
  </script>
  <%: ScriptHelper.GenerateFormCGridEditViewDeleteScript(Url, ControlIdConstants.FormCSearchGrid, Url.RouteUrl("Pax_default", new { action = "FormCEdit", controller = "FormC" }), Url.RouteUrl("Pax_default", new { action = "ViewDetails", controller = "FormC" }), "ValidateFormC", "Delete", Url.Action("DownloadZip", "FormC", new { area = "Pax" }),  (int)ViewData[ViewDataConstants.RejectionOnValidationFlag])%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Sampling Form C Search</h1>
  <% using (Html.BeginForm("Index", "FormC", FormMethod.Get, new { id = "FormCSearchForm" }))
     { %>
  <div>
    <% Html.RenderPartial("FormCSearchControl", Model); %>
  </div>
  <%} %>
  <div>
    <h2>
      Search Results</h2>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.FormCSearchResults]); %>
  </div>
  <div class="buttonContainer">
    <%if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Pax.Receivables.SampleFormC.Submit))
      {%>
    <input class="primaryButton" type="button" value="Submit Selected Form C" id="SubmitFormCButton" />
    <%}%>
    <input class="primaryButton hidden" type="button" value="Present Selected Form C" id="PresentFormCButton" />
  </div>
  <div id="InvoiceDownloadOptions" class="hidden">
    <% Html.RenderPartial("~/Views/Shared/InvoiceDownloadOptionsControl.ascx", Iata.IS.Model.Enums.InvoiceDownloadOptions.FormCReceivables);%>
  </div>
</asp:Content>
