<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MemberProfile.LocationAssociation>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS ::  Profile :: Manage Location Associations
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  
  <script type="text/javascript">
      $('#emailAddress').focus();
      $('#emailAddress').flushCache();
      registerAutocomplete('emailAddress', 'userId', '<%:Url.Action("GetUserContactList", "LocationAssociation", new { area = "" })%>', 0, true, null);
      $("#btnView").bind('click', function () {
          if ($('#userId').val() == 0 || $('#emailAddress').val() == ' ' || $('#emailAddress').val() == '') {
              showClientErrorMessage('Please provide/select an email address to view the Location Association of the User/Contact');
              $('#emailAddress').val('');
              return false;
          }
      });
  </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Manage Location Associations
  </h1>
  <% using (Html.BeginForm("ManageLocationAssociation", "LocationAssociation", FormMethod.Post, new { id = "frmManageLocations" }))
     { %>
     <%: Html.AntiForgeryToken() %>
  <div >
    <div class="searchCriteria">
      <div class="solidBox">
        <div class="fieldContainer horizontalFlow" style="height: 90px;  margin-left : 40px;  ">
          <div class="buttonContainer">
            <div style="width: 280px;">
              <b><span>* </span> User / Contact:</b>
              <%:Html.TextBoxFor(m => m.emailAddress, new { @class = "autocComplete", style = "width:250px;" })%>
              <%= Html.HiddenFor(m => m.userId)%>              
            </div>            
          </div>
          <div style="width: 170px;">
              <br />
              <input type="submit" id="btnView"  value="View" style="width: 150px;" class="primaryButton" />
            </div>  
          
        </div>
      </div>
    </div>
  </div>
  <% } %>
</asp:Content>
