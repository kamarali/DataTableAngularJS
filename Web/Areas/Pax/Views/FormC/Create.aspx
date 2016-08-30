<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.Sampling.SamplingFormC>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> :: Create Sampling Form C
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript" src="<%: Url.Content("~/Scripts/Pax/FormC.js") %>">
  </script>
  <script type="text/javascript">
      $(document).ready(function () {
          /*CMP #596: Length of Member Accounting Code to be Increased to 12 
          Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
          Ref: FRS Section 3.4 Table 15 Row 5 */
          registerAutocomplete('ProvisionalBillingMemberText', 'ProvisionalBillingMemberId', '<%:Url.Action("GetMemberListForPaxCgo", "Data", new { area = "" })%>', 0, true, GetFormABListingCurrency);
      InitializeFormC('<%:Url.Action("GetFormABListingCurrency", "FormC") %>', '<%:SessionUtil.MemberId %>');
    });
</script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Create Sampling Form C</h1>
  <% using (Html.BeginForm("Create", "FormC", FormMethod.Post, new { id = "samplingformC", @class = "validCharacters" }))
     {%>
     <%: Html.AntiForgeryToken() %>
  <div>
    <% Html.RenderPartial("HeaderControl"); %>
  </div>
  <div class="buttonContainer">
    <input class="primaryButton ignoredirty" type="submit" value="Save Form C Header" id="btnSaveHeader" />
  </div>
  <%} %>
  <div class="clear">
  </div>
</asp:Content>
