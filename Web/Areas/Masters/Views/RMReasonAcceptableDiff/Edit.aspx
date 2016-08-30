<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.Common.RMReasonAcceptableDiff>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: Passenger Related :: Edit Reason Code - RM Amount Map
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">

        $(document).ready(function () {
            $('#EffectiveFrom').watermark("YYYYMMPP");
            $('#EffectiveTo').watermark("YYYYMMPP");
        });
    </script>
    <h1>
        Reason Code - RM Amount Map
    </h1>
    <h2>
        Edit Reason Code - RM Amount</h2>
    <% using (Html.BeginForm("Edit", "RMReasonAcceptableDiff", FormMethod.Post, new { @id = "RMReasonAcceptableDiffMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div>
            <label>
                <span class="required">* </span>Transaction Type:</label>
            <%: Html.TransactionTypeDropdownListFor(model => model.TransactionTypeId,1)%>
            <%: Html.ValidationMessageFor(model => model.TransactionTypeId)%>
        </div>
        <div>
            <label>
                <span class="required">* </span>Reason Code:</label>
            <%: Html.ReasonCodeDropdownListFor(model => model.ReasonCodeId, Model!=null ?Model.TransactionTypeId:0)%>
            <%: Html.ValidationMessageFor(model => model.ReasonCodeId) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Effective From:
            </label>
            <%:Html.TextBox("EffectiveFrom", Model.EffectiveFrom, new { @id = "EffectiveFrom", @maxLength = 8 })%>
            <%: Html.ValidationMessageFor(model => model.EffectiveFrom) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Effective To:
            </label>
            <%:Html.TextBox("EffectiveTo", Model.EffectiveTo, new { @id = "EffectiveTo", @maxLength = 8 })%>
            <%: Html.ValidationMessageFor(model => model.EffectiveTo) %>
        </div>
        <div class="editor-label">
            <label>
                Fare Amount:
            </label>
            <%: Html.CheckBoxFor(model => model.IsFareAmount)%>
            <%: Html.ValidationMessageFor(model => model.IsFareAmount) %>
        </div>
        <div class="editor-label">
            <label>
                Tax Amount:
            </label>
            <%: Html.CheckBoxFor(model => model.IsTaxAmount)%>
            <%: Html.ValidationMessageFor(model => model.IsTaxAmount) %>
        </div>
        <div class="editor-label">
            <label>
                Isc Amount:
            </label>
            <%: Html.CheckBoxFor(model => model.IsIscAmount)%>
            <%: Html.ValidationMessageFor(model => model.IsIscAmount) %>
        </div>
        <div class="editor-label">
            <label>
                Oc Amount:
            </label>
            <%: Html.CheckBoxFor(model => model.IsOcAmount)%>
            <%: Html.ValidationMessageFor(model => model.IsOcAmount) %>
        </div>
        <div class="editor-label">
            <label>
                Uatp Amount:
            </label>
            <%: Html.CheckBoxFor(model => model.IsUatpAmount)%>
            <%: Html.ValidationMessageFor(model => model.IsUatpAmount) %>
        </div>
        <div class="editor-label">
            <label>
                Hf Amount:
            </label>
            <%: Html.CheckBoxFor(model => model.IsHfAmount)%>
            <%: Html.ValidationMessageFor(model => model.IsHfAmount) %>
        </div>
        <div class="editor-label">
            <label>
                Vat Amount:
            </label>
            <%: Html.CheckBoxFor(model => model.IsVatAmount)%>
            <%: Html.ValidationMessageFor(model => model.IsVatAmount) %>
        </div>
        <div class="editor-label">
            <label>
                Active:
            </label>
            <%: Html.CheckBoxFor(model => model.IsActive) %>
            <%: Html.ValidationMessageFor(model => model.IsActive) %>
        </div>
        <div class="buttonContainer">
            <input type="submit" value="Save" class="primaryButton" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","RMReasonAcceptableDiff") %>'" />
        </div>
    </fieldset>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script src="<%:Url.Content("~/Scripts/Masters/RMReasonAcceptableDiffValidate.js")%>"
        type="text/javascript"></script>
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
