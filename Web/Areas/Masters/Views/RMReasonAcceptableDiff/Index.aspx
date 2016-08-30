<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.Common.RMReasonAcceptableDiff>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Master Maintenance :: Passenger Related :: Reason Code - RM Amount Map
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
       Reason Code - RM Amount Map
    </h1>
    <% using (Html.BeginForm("Index", "RMReasonAcceptableDiff", FormMethod.Post))
       {%>
       <%: Html.AntiForgeryToken() %>
    <div>
        <% Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchRMReasonAcceptableDiff.ascx"); %>
    </div>
    <div class="buttonContainer">
    <%if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.PaxRMReasonEditOrDelete))
{%>
        <input type="button" class="primaryButton" value="Add" id="btnAdd" name="Add" onclick="javascript:location.href = '<%:Url.Action("Create", "RMReasonAcceptableDiff")%>'" />
        <%
}%>
        <input type="submit" class="primaryButton" value="Search" id="btnSearch" name="Search" />
    </div>
    <%} %>
     <h2>Search Results</h2>
    <%Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchRMReasonAcceptableDiffGrid.ascx", ViewData["RMReasonAcceptableDiffGrid"]); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script type="text/javascript" language="javascript">
        $("#TransactionTypeId").change(function () {
            var url = '<%: Url.Content("~/")%>' + "Masters/RMReasonAcceptableDiff/GetReasonCodeList?TransactionTypeId=" + $("#TransactionTypeId > option:selected").attr("value");
            $.getJSON(url, function (data) {
                var items = "<OPTION value=''>Please Select</OPTION>";
                $.each(data, function (i, ReasonCode) {
                    items += "<OPTION value='" + ReasonCode.Id + "'>" + ReasonCode.Code + "</OPTION>";
                });
                $("#ReasonCodeId").html(items);
            });
        });   
</script>
</asp:Content>
