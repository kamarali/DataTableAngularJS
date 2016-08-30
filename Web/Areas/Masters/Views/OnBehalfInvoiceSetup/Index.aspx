<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.OnBehalfInvoiceSetup>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
     SIS :: Master Maintenance :: General :: Transmitter Exception Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h1>
       Transmitter Exception Setup
    </h1>
    <% using (Html.BeginForm("Index", "OnBehalfInvoiceSetup", FormMethod.Post))
       {%>
       <%: Html.AntiForgeryToken() %>
    <div>
        <% Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchOnBehalfInvoiceSetup.ascx",Model); %>
    </div>
    <div class="buttonContainer">
    <%if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.OnBehalfInvoiceSetupEditOrDelete))
    {%>
      <input type="button" class="primaryButton" value="Add" id="btnAdd" name="Add" onclick="javascript:location.href = '<%:Url.Action("Create", "OnBehalfInvoiceSetup")%>'" />
    <%}%>
        <input type="submit" class="primaryButton" value="Search" id="btnSearch" name="Search" />
    </div>
    <%} %>
     <h2>Search Results</h2>
    <%Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchOnBehalfInvoiceSetupGrid.ascx", ViewData["OnBehalfInvoiceSetupGrid"]); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script type="text/javascript" language="javascript">

    $(document).ready(function () {
        $("#BillingCategoryId option[value='1']").remove();
        $("#BillingCategoryId option[value='2']").remove();
    });

    $("#BillingCategoryId").change(function () {
        var url = '<%: Url.Content("~/")%>' + "Masters/OnBehalfInvoiceSetup/GetChargeCategoryList?billingCategoryId=" + $("#BillingCategoryId > option:selected").attr("value");
        $.getJSON(url, function (data) {
            var items = "<OPTION value=''>Please Select</OPTION>";
            $.each(data, function (i, ChargeCategory) {
                items += "<OPTION value='" + ChargeCategory.Id + "'>" + ChargeCategory.Name + "</OPTION>";
            });
            $("#ChargeCategoryId").html(items);
            $("#ChargeCodeId").empty();
            $("#ChargeCodeId").append('<option value="">Please Select</option>');
        });
    });

    $("#ChargeCategoryId").change(function () {
        var url = '<%: Url.Content("~/")%>' + "Masters/OnBehalfInvoiceSetup/GetChargeCodeList?ChargeCategoryId=" + $("#ChargeCategoryId > option:selected").attr("value");
        $.getJSON(url, function (data) {
            var items = "<OPTION value=''>Please Select</OPTION>";
            $.each(data, function (i, ChargeCode) {
                items += "<OPTION value='" + ChargeCode.Id + "'>" + ChargeCode.Name + "</OPTION>";
            });
            $("#ChargeCodeId").html(items);
        });
    });   
</script>
</asp:Content>
