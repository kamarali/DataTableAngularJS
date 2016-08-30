<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Web.UIModel.Account.ProxyViewModel>" %>

<%@ Import Namespace="Iata.IS.Web.UIModel.Account" %>
<asp:Content ID="changePasswordTitle" ContentPlaceHolderID="TitleContent" runat="server">
  <%= Iata.IS.Web.AppSettings.ProxyLogonSearchText %>
</asp:Content>
<asp:Content ID="script" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">
      $(document).ready(function () {

          CheckForTheSession();
          var selectedUserCategory = $("#UserCategoryId").val();

          if (selectedUserCategory == '') {
              $('#divMember').hide();
              $('#divUser').hide();
              $('#btnProxyLogin').hide();
          }

          $('#UserCategoryId').change(function () {
              $('#divMember').show();
              $('#divUser').show();

              var selectedItem = $("#UserCategoryId").val();
              ClearAllEmails();
              $('#MemberName').hide();
              $('#UserEmail').hide();
              $('#lblMember').hide();
              $('#lblUser').hide();

              if (selectedItem != 0 && selectedItem != 4) {
                  GetUsersByCategory(selectedItem);
              } else {
                  $('#MemberName').val('');
                  $('#MemberName').show();
                  $('#lblMember').show();
                  $("#MemberName").focus();
                  $('#btnProxyLogin').hide();
              }

              if (selectedItem == 0) {
                  $('#MemberName').val('');
                  $('#MemberName').hide();
                  $('#lblMember').hide();
                  $('#btnProxyLogin').hide();
              }

              $("#CategoryData").val($("#CategoryName").val());
          });

          registerAutocomplete('MemberName', 'MemberId', '<%:Url.Action("GetAllMemberList", "Data", new { area = "" })%>', 0, true, function (selectedId) { Member_AutoCompleteValueChange(selectedId); });

          function Member_AutoCompleteValueChange(selectedId) {
              ClearAllEmails();
              GetUsers(selectedId);
              return true;
          }

          $('#UserEmail').change(function () {
              $("#UserEmailData").val($("#UserEmail").val());
          });


          function CheckForTheSession() {
              $.ajax({ url: '<%= Url.Action("CheckForSession","Account")%>',
                  type: "POST",
                  datType: "json",
                  success: function (data) {
                      if (data) {

                          window.location.reload(); 
                      }
                  }
              });
          }
          function GetUsers(selectedItem) {
              $.ajaxSetup({ cache: false });
              if ((selectedItem) && (selectedItem != 'undefined')) {
                  $.ajax({ url: '<%= Url.Action("GetUsers","Account")%>',
                      type: "POST",
                      datType: "json",
                      success: function (data) {
                          if (data) {
                              var items = "";
                              var firstItem = "<option value='0'>Select User</option>";
                              var numberOfItems = 0;
                              $.each(data, function (i, data) {
                                  //items += "<option value='" + data["UserEmail"] + "'>" + data["UserEmail"] + "</option>";
                                  items += "<option value=\"" + data["UserEmail"] + "\">" + data['UserEmail'] + "</option>";
                                  numberOfItems += 1;
                              });
                              if (items != "") {
                                  items = firstItem + items;
                                  $("#UserEmail").html(items);
                                  if (numberOfItems > 0) {
                                      $('#UserEmail').show();
                                      $('#lblUser').show();
                                      $('#btnProxyLogin').show();
                                  }
                              } else {
                                  $('#lblUser').show();
                                  $('#UserEmail').show();
                                  var items = "<option value='0'>Select User</option>";
                                  $("#UserEmail").html(items);
                                  $('#btnProxyLogin').hide();
                              }
                          }
                      },
                      error: function (xhr, ajaxOptions, thrownError) {
                          ClearAllEmails();
                          var items = "<option value='0'>Select User</option>";
                          $("#UserEmail").html(items);
                          $('#btnProxyLogin').hide();
                      },
                      data: selectedItem
                  });
              }
          }

          function GetUsersByCategory(selectedItem) {
              $.ajaxSetup({ cache: false });
              if ((selectedItem) && (selectedItem != 'undefined')) {
                  $.ajax({ url: '<%= Url.Action("GetUsersByCategory", "Account")%>',
                      type: "POST",
                      datType: "json",
                      success: function (data) {
                          if (data) {
                              var items = "";
                              var numberOfItems = 0;
                              var firstItem = "<option value='0'>Select</option>";
                              $.each(data, function (i, data) {
                                  items += "<option value=\"" + data["UserEmail"] + "\">" + data['UserEmail'] + "</option>";
                                  numberOfItems += 1;
                              });
                              if (items != "") {
                                  items = firstItem + items;
                                  $("#UserEmail").html(items);
                                  if (numberOfItems > 0) {
                                      $('#lblUser').show();
                                      $('#UserEmail').show();
                                      $('#btnProxyLogin').show();
                                  } else { $('#btnProxyLogin').hide(); }
                              }
                          }
                      },
                      error: function (xhr, ajaxOptions, thrownError) {
                          ClearAllEmails();
                          var items = "<option value='0'>Select</option>";
                          $("#UserEmail").html(items);
                          $('#lblUser').show();
                          $('#UserEmail').show();
                          $('#btnProxyLogin').hide();
                      },
                      data: selectedItem
                  });
              }
          }

          function ClearAllEmails() { $('#UserEmail >option').remove(); }
          $('#CategoryName').trigger('change');
      });
  </script>
</asp:Content>
<asp:Content ID="changePasswordSuccessContent" ContentPlaceHolderID="MainContent"
  runat="server">
  <h1>
  Proxy Login</h1>
  <h2>
    Search Criteria</h2>
  <% using (Html.BeginForm(Html.BeginForm("ProxyLoginSearch", "Account", FormMethod.Post, new { id = "frmProxySearch" })))
     { %>
     <%: Html.AntiForgeryToken() %>
  <div class="solidBox" style="height: 70px;">
    <div class="fieldContainer horizontalFlow">
      <div>
        <div style="float: left; width: 200px;">
          <label id="lblUserCategory">User Category:</label>
          <%= Html.UserCategoryDropdownListFor(model => model.UserCategoryId, "0", new { style = "width:130px;" })%>
        </div>
        <div id="divMember" style="float: left; width: 250px;">
          <label id="lblMember">Member:</label>
          <%:Html.HiddenFor(m => m.MemberId, new { style = "width:200px;" })%>
          <%:Html.TextBoxFor(m => m.MemberName, new { @class = "autocComplete", style = "width:200px;" })%>
        </div>
        <div id="divUser" > 
          <label id="lblUser">Users:</label>
          <%= Html.DropDownListFor(m => m.UserEmail, new SelectList(new List<string>()), new { style = "width:200px;" })%>
          <%= Html.ValidationMessageFor(m => m.UserEmail)%>
          <%= Html.HiddenFor(m => m.UserEmailData) %>
        </div>
      </div>
    </div>
  </div>
  <div id="btnProxyLogin" class="buttonContainer">
    <input type="submit" value="<%= Iata.IS.Web.AppSettings.ProxyAsUserText %>" class="primaryButton" />
  </div>
  <% } %>
</asp:Content>
