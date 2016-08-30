<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MemberProfile.Member>" %>

<%@ Import Namespace="Iata.IS.Model.MemberProfile.Enums" %>

<%@ Import Namespace="Iata.IS.Model.Calendar" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Profile and User Management :: Manage Member Profile
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Member/MemberProfile.js")%>"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script type="text/javascript">
    $(document).ready(function () {
      registerAutocomplete('DisplayCommercialName', 'Id', '<%:Url.Action("GetAllMemberList", "Data", new { area = "" })%>', 0, true, null);
    });
  </script>
  <script type="text/javascript">
      $(function () {
          $("#tabs").tabs({
              load: function (event, ui) {
                  // Format all the date controls on the page.
                  formatDateControls();

                  // Decorate the '*' on the mandatory fields with red color.
                  highlightMandatory();

                  $(".alphaNumeric").keypress(checkIsAlphaNumeric);
                  $(".AllowedFileTypes").keypress(AllowedTypesSupportingDocuments);
                  $(".numeric").keypress(checkIsNumeric);
                  $(".numericOnly").keypress(checkForNumeric);

                  if ($('#tabs .ui-tabs-active').children().attr('id') == "memberDetails-tab") {
                      initMemberDetailsTabValidations();
                      trackFormChanges('MemberDetails');
                  }

                  if ($('#tabs .ui-tabs-active').children().attr('id') == "locations-tab") {
                      initLocationsTabValidations();
                      trackFormChanges('location');
                  }

                  if ($('#tabs .ui-tabs-active').children().attr('id') == "contacts-tab") {
                      initContactsTabValidations();
                      trackFormChanges('contacts');
                  }

                  if ($('#tabs .ui-tabs-active').children().attr('id') == "billing-tab") {
                      initEBillingTabValidations();
                      trackFormChanges('eBilling');
                  }

                  if ($('#tabs .ui-tabs-active').children().attr('id') == "passenger-tab") {
                      initPassengerTabValidations();
                      trackFormChanges('pax');
                  }

                  if ($('#tabs .ui-tabs-active').children().attr('id') == "cargo-tab") {
                      initCgoTabValidations();
                      trackFormChanges('cgo');
                  }

                  if ($('#tabs .ui-tabs-active').children().attr('id') == "misc-tab") {
                      initMiscTabValidations();
                      trackFormChanges('Misc');
                  }

                  if ($('#tabs .ui-tabs-active').children().attr('id') == "uatp-tab") {
                      initUatpTabValidations();
                      trackFormChanges('uatp');
                  }

                  if ($('#tabs .ui-tabs-active').children().attr('id') == "Ich-tab") {
                      initICHTabValidations();
                      trackFormChanges('Ich');
                  }

                  if ($('#tabs .ui-tabs-active').children().attr('id') == "Ach-tab") {
                      initACHTabValidations();
                      trackFormChanges('ach');
                  }

                  if ($('#tabs .ui-tabs-active').children().attr('id') == "technical-tab") {
                      initTechnicalTabValidations();
                      trackFormChanges('technical');
                  }

                  if ($('#tabs .ui-tabs-active').children().attr('id') == "memberControls-tab") {
                      initMemberControlsTabValidations();
                      trackFormChanges('memberControl');
                  }
              },
              beforeActivate: function (event, ui) {
                  event.data = ui.newTab.index();
                  var formName;
                  switch ($('#tabs .ui-tabs-active').children().attr('id')) {//(ui.tab.id) {
                      case 'memberDetails-tab':
                          formName = 'MemberDetails';
                          break;
                      case 'locations-tab':
                          formName = 'location';
                          break;
                      case 'contacts-tab':
                          formName = 'contacts';
                          break;
                      case 'billing-tab':
                          formName = 'eBilling';
                          break;
                      case 'passenger-tab':
                          formName = 'pax';
                          break;
                      case 'cargo-tab':
                          formName = 'cgo';
                          break;
                      case 'misc-tab':
                          formName = 'Misc';
                          break;
                      case 'uatp-tab':
                          formName = 'uatp';
                          break;
                      case 'Ich-tab':
                          formName = 'Ich';
                          break;
                      case 'Ach-tab':
                          formName = 'ach';
                          break;
                      case 'technical-tab':
                          formName = 'technical';
                          break;
                      case 'memberControls-tab':
                          formName = 'memberControl';
                          break;
                  }
                  // Monitor the form for changes.
                  $('form#' + formName).monitorDirty(event);
              }
          }).on('tabsactivate', function (event, ui) {
              // Remove the elements that cause duplicate ids across tabs.
              $('.sn-dialog').remove();
              $('.ui-dialog').remove();
              $('.ml-dialog').remove();
              $('.ag-dialog').remove();
              $('.ichStatus-dialog').remove();
              $('.achStatus-dialog').remove();
              $('.ca-dialog').remove();
              $('.additionaloutput').remove();
              $('.miscuatpControl').remove();
              $('.memberLogo-dialog').remove();
              $('#errorContainer').hide();
              $('#clientErrorMessageContainer').hide();
              $('#clientSuccessMessageContainer').hide();
              $('#successMessageContainer').hide();
              $('.removelist').remove();
              $('.copyContacts-dialog').remove();
              $('.replaceContacts-dialog').remove();
              $('.contactAssignment').remove();
              $('.RejectionOnValidationFailure').remove();
              $('#BillingIsXmlMigrationStatusId').remove();
              $('#BillingIsXmlMigrationDate').remove();
          });
      });

    function CommercialName_SetAutocompleteDisplay(item) {
      var memberCode = item.MemberCodeAlpha + "-" + item.MemberCodeNumeric + "-" + item.CommercialName + "";
      return { label: memberCode, value: memberCode, id: item.Id };
    }
  </script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Member Profile</h1>
  <h2>
    Manage Member</h2>
  <input type="hidden" id="nextPeriod" value='<%: ViewData["NextBillingPeriod"] %>' />
  <input type="hidden" id="currPeriod" value='<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Period.ToString().PadLeft(2, '0')%>' />
  <input type="hidden" id="currMonth" value='<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Month.ToString().PadLeft(2, '0')%>' />
  <input type="hidden" id="currYear" value='<%=((BillingPeriod) ViewData["CurrentBillingPeriod"]).Year.ToString()%>' />
  <% Html.RenderPartial("SearchControl", Model); %>
  <div id="tabs">
    <ul class="ui-tabs-nav">
      <%--CMP #665: User related enhancement [Sec 2.11: Conditional Display of All Tabs in the IS-WEB Member Profile screen]
          This change is applicable for ‘Member User’ and A ‘SIS Ops User’ performs proxy login as a ‘Member User’ 
          This change is not applicable for ‘SIS Ops User’, ‘ICH Ops User’ and ‘ACH Ops User’.--%>
      <%: Html.Tab("Member Details", "MemberDetails", "Member", new { area = "Profile", selectedMemberId = Model.Id }, new { id = "memberDetails-tab" }, UserCategory.SisOps, UserCategory.IchOps, UserCategory.AchOps, UserCategory.Member)%>
      <%: Html.Tab("Locations", "Location", "Member", new { area = "Profile", selectedMemberId = Model.Id }, new { id = "locations-tab" }, UserCategory.SisOps, UserCategory.Member)%>
      <%: Html.Tab("Contacts", "Contacts", "Member", new { area = "Profile", selectedMemberId = Model.Id }, new { id = "contacts-tab" }, UserCategory.SisOps, UserCategory.IchOps, UserCategory.AchOps, UserCategory.Member)%>
      <%: Html.Tab("e-Billing", "EBilling", "Member", new { area = "Profile", selectedMemberId = Model.Id, sisMemberSubStatusId = ViewData["SisMemberSubStatusId"] }, new { id = "billing-tab" }, UserCategory.SisOps, UserCategory.Member)%>
      <%: Html.Tab("Passenger", "Pax", "Member", new { area = "Profile", selectedMemberId = Model.Id, sisMemberSubStatusId = ViewData["SisMemberSubStatusId"] }, new { id = "passenger-tab" }, UserCategory.SisOps, UserCategory.Member)%>
      <%: Html.Tab("Cargo", "Cgo", "Member", new { area = "Profile", selectedMemberId = Model.Id, sisMemberSubStatusId = ViewData["SisMemberSubStatusId"] }, new { id = "cargo-tab" }, UserCategory.SisOps, UserCategory.Member)%>
      <%: Html.Tab("Miscellaneous", "Misc", "Member", new { area = "Profile", selectedMemberId = Model.Id, sisMemberSubStatusId = ViewData["SisMemberSubStatusId"] }, new { id = "misc-tab" }, UserCategory.SisOps, UserCategory.Member)%>
      <%: Html.Tab("UATP", "Uatp", "Member", new { area = "Profile", selectedMemberId = Model.Id, sisMemberSubStatusId = ViewData["SisMemberSubStatusId"] }, new { id = "uatp-tab" }, UserCategory.SisOps, UserCategory.Member)%>
      <%: Html.Tab("ICH", "Ich", "Member", new { area = "Profile", selectedMemberId = Model.Id, sisMemberSubStatusId = ViewData["SisMemberSubStatusId"] }, new { id = "Ich-tab" }, UserCategory.SisOps, UserCategory.IchOps, UserCategory.Member)%>
      <%: Html.Tab("ACH", "Ach", "Member", new { area = "Profile", selectedMemberId = Model.Id, sisMemberSubStatusId = ViewData["SisMemberSubStatusId"] }, new { id = "Ach-tab" }, UserCategory.SisOps, UserCategory.AchOps, UserCategory.Member)%>
      <%: Html.Tab("Technical", "Technical", "Member", new { area = "Profile", selectedMemberId = Model.Id }, new { id = "technical-tab" }, UserCategory.SisOps)%>
      <%: Html.Tab("SIS Ops", "MemberControl", "Member", new { area = "Profile", selectedMemberId = Model.Id }, new { id = "memberControls-tab" }, UserCategory.SisOps)%>
    </ul>
  </div>
  <div class="hidden" id="divBrowse">
    <% Html.RenderPartial("BrowserControl");%>
  </div>
  <div class="hidden">
    <%Html.RenderPartial("FutureDatedValueDialog", Model.Id); %>
  </div>
  <div class="hidden" id="divfutureUpdateInformation">
    <label>
      Updated Value:</label>
    <div id="updatedValue" style="max-width: 130px; max-height: 130px; overflow: auto; font-weight: bold;">
    </div>
    <br />
    <br />
    <label>
      Effective From:</label>
    <label id="effectiveFrom" style="font-weight: bold;">
    </label>
  </div>
</asp:Content>
