<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.OnBehalfInvoiceSetup>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
     SIS :: Master Maintenance :: General :: Add Transmitter Exception
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
       Transmitter Exception Setup
    </h1>
    <h2>
        Add Transmitter Exception</h2>
    <% using (Html.BeginForm("Create", "OnBehalfInvoiceSetup", FormMethod.Post, new { @id = "OnBehalfInvoiceSetupMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div class="editor-label">
            <label>
                <span class="required">* </span>Billing Category:</label>
            <%: Html.BillingCategoryDropdownListFor(model => model.BillingCategoryId) %>
            <%: Html.ValidationMessageFor(model => model.BillingCategoryId) %>
        </div>
        <div class="editor-label">
           <label>
                <span class="required">* </span>Transmitter Code:</label>
            <%: Html.TextBoxFor(model => model.TransmitterCode, new { @class = "alphaNumeric upperCase", @maxLength = 50 })%>
            <%: Html.ValidationMessageFor(model => model.TransmitterCode) %>
        </div>
        <div class="editor-label">
           <label>
                <span class="required">* </span>Charge Category:</label>
            <%: Html.ChargeCategoryDropdownList("ChargeCategoryId",Model!=null ? Model.ChargeCategoryId:0,Model!=null ?Model.BillingCategoryId:0)%>
            <%: Html.ValidationMessageFor(model => model.ChargeCategoryId) %>
        </div>
        <div class="editor-label">
           <label>
                <span class="required">* </span>Charge Code:</label>
                          <%: Html.ChargeCodeDropdownList("ChargeCodeId",Model!=null? Model.ChargeCodeId:0,Model!= null? Model.ChargeCategoryId:0)%>
            <%: Html.ValidationMessageFor(model => model.ChargeCodeId) %>
        </div>
        <div class="editor-label">
            <label>Active:</label>
            <%: Html.CheckBoxFor(model => model.IsActive, new {@Checked="checked" })%>
            <%: Html.ValidationMessageFor(model => model.IsActive) %>
        </div>
        
        <div class="buttonContainer">
            <input type="submit" value="Save" class="primaryButton" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","OnBehalfInvoiceSetup") %>'" />
        </div>
    </fieldset>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script src="<%:Url.Content("~/Scripts/Masters/OnBehalfInvoiceSetupValidate.js")%>"
        type="text/javascript"></script>
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
