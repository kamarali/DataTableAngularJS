<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.CgoRMReasonAcceptableDiff>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: Cargo Related :: Reason Code - RM Amount Map
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        Reason Code - RM Amount Map
    </h1>
    <% using (Html.BeginForm("Index", "CgoRMReasonAcceptableDiff", FormMethod.Post))
       {%>
    <div>
        <% Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchCgoRMReasonAcceptableDiff.ascx"); %>
    </div>
    <div class="buttonContainer">
    <% if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.CgoRMReasonEditOrDelete))
{%>
        <input type="button" class="primaryButton" value="Add" id="btnAdd" name="Add" onclick="javascript:location.href = '<%:Url.Action("Create", "CgoRMReasonAcceptableDiff")%>'" />
        <%
}%>
        <input type="submit" class="primaryButton" value="Search" id="btnSearch" name="Search" />
    </div>
    <%} %>
     <h2>Search Results</h2>
    <%Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchCgoRMReasonAcceptableDiffGrid.ascx", ViewData["CgoRMReasonAcceptableDiffGrid"]); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script type="text/javascript" language="javascript">
        $("#TransactionTypeId").change(function () {
            var url = '<%: Url.Content("~/")%>' + "Masters/CgoRMReasonAcceptableDiff/GetReasonCodeList?TransactionTypeId=" + $("#TransactionTypeId > option:selected").attr("value");
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
