<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MemberProfile.SubDivision>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: Area Related :: Area Sub Division Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        Area Sub Division Setup
    </h1>
    <% using (Html.BeginForm("Index", "SubDivision", FormMethod.Post))
       {%>
       <%: Html.AntiForgeryToken() %>
    <div>
        <% Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchSubDivision.ascx"); %>
    </div>
    <div class="buttonContainer">
    <%
           if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.SubDivisionEditOrDelete))
           {%>
        <input type="button" class="primaryButton" value="Add" id="btnAdd" name="Add" onclick="javascript:location.href = '<%:Url.Action("Create", "SubDivision")%>'" />
        <%
           }%>
        <input type="submit" class="primaryButton" value="Search" id="btnSearch" name="Search" />
    </div>
    <%} %>
    <h2>
        Search Results</h2>
    <%Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchSubDivisionGrid.ascx", ViewData["SubDivisionGrid"]); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script type="text/javascript" language="javascript">
  function activatedeactivateRecord(methodName, value, gridId) {
    $('#successMessageContainer').hide();
    if (value == "True") {
      if (confirm("Are you sure you want to deactivate this record?")) {
        $.ajax({
          type: "POST",
          url: methodName,
          success: function (result) {
            $('#errorContainer').hide();
            if (result.IsFailed == false) {
              if (result.isRedirect) {
                location.href = result.RedirectUrl;
              }
              if (result.LineItemDetailExpected) {
                alert("Line item detail expected.");
              }

              //For contacts tab after successful delete remove that contact from other dropdown list.
              if (methodName == "DeleteContact") {
                $("#replaceoldcontact option[value=" + value + "]").remove();
                $("#replacenewcontact option[value=" + value + "]").remove();
                $("#copyoldcontact option[value=" + value + "]").remove();
                $("#copynewcontact option[value=" + value + "]").remove();
              }

              // Toggle message containers.          
              showClientSuccessMessage("Record deactivated successfully.");
            }
            else {
              showClientSuccessMessage("Record deactivated successfully.");
            }
            $(gridId).trigger("reloadGrid");
          }
        });
      }
    }
    else {
      if (confirm("Are you sure you want to activate this record?")) {
        $.ajax({
          type: "POST",
          url: methodName ,
          success: function (result) {
            $('#errorContainer').hide();
            if (result.IsFailed == false) {
              if (result.isRedirect) {
                location.href = result.RedirectUrl;
              }
              if (result.LineItemDetailExpected) {
                alert("Line item detail expected.");
              }

              //For contacts tab after successful delete remove that contact from other dropdown list.
              if (methodName == "DeleteContact") {
                $("#replaceoldcontact option[value=" + value + "]").remove();
                $("#replacenewcontact option[value=" + value + "]").remove();
                $("#copyoldcontact option[value=" + value + "]").remove();
                $("#copynewcontact option[value=" + value + "]").remove();
              }

              // Toggle message containers.          
              showClientSuccessMessage("Record activated successfully.");
            }
            else {
              showClientSuccessMessage("Record activated successfully.");
            }
            $(gridId).trigger("reloadGrid");
          }
        });
      }
    }
    
  }
</script>

</asp:Content>
