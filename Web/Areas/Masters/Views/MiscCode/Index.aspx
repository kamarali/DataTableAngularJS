<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.MiscCode>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
     SIS :: Master Maintenance :: General :: Miscellaneous Codes Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
       Miscellaneous Codes Setup
    </h1>
    <% using (Html.BeginForm("Index", "MiscCode", FormMethod.Post, new { @id = "MiscCodeMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <div>
        <% Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchMiscCode.ascx"); %>
    </div>
    <div class="buttonContainer">
    <%
           if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.MiscCodeEditOrDelete))
           {%>
        <input type="button" class="primaryButton" value="Add" id="btnAdd" name="Add" onclick="javascript:location.href = '<%:Url.Action("Create", "MiscCode")%>'" />
        <%
           }%>
        <input type="submit" class="primaryButton" value="Search" id="btnSearch" name="Search" />
    </div>
    <%} %>
     <h2>Search Results</h2>
    <%Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchMiscCodeGrid.ascx", ViewData["MiscCodeGrid"]); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script type="text/javascript" language="javascript">
      $('#btnSearch').click(function () {
        var description = $("#Description").val();
        description = description.replace(/(\'|\")/g,'`');
        $("#Description").val(description);
      });
    $(document).ready(function () {

      $("#MiscCodeMaster").validate({
        rules: {
            Name: {
               maxlength: 50
            },
            Description: {
                maxlength: 1000
            }
        },
        messages: {
            Name: " Transaction Type should be of maximum 50 characters",
            Description: " Description should be of maximum 1000 characters"
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
