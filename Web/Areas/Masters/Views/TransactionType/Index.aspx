<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.TransactionType>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
     SIS :: Master Maintenance :: General :: Transaction Type
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        Transaction Type Setup
    </h1>
    <% using (Html.BeginForm("Index", "TransactionType", FormMethod.Post, new { @id = "TransactionTypeMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <div>
        <% Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchTransactionType.ascx"); %>
    </div>
    <div class="buttonContainer">
    <%if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.TransactionTypeEditOrDelete))
{%>
        <input type="button" class="primaryButton" value="Add" id="btnAdd" name="Add" onclick="javascript:location.href = '<%:Url.Action("Create", "TransactionType")%>'" />
        <%
}%>
        <input type="submit" class="primaryButton" value="Search" id="btnSearch" name="Search" />
    </div>
    <%} %>
     <h2>Search Results</h2>
    <%Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchTransactionTypeGrid.ascx", ViewData["TransactionTypeGrid"]); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script type="text/javascript" language="javascript">
    $(document).ready(function () {

    $("#TransactionTypeMaster").validate({
        rules: {
            Name: {
               maxlength: 20
            },
            Description: {
                maxlength: 255
            }
        },
        messages: {
            Name: " Transaction Type should be of maximum 20 characters",
            Description: " Description should be of maximum 255 characters"
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
