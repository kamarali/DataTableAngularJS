<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.CgoRMReasonAcceptableDiff>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: Cargo Related :: Edit Reason Code - RM Amount Map
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
        
     <h1>
        Reason Code - RM Amount Map
    </h1><h2>
        Edit  Reason Code - RM Amount </h2>
    <% using (Html.BeginForm("Edit", "CgoRMReasonAcceptableDiff", FormMethod.Post, new { @id = "CgoRMReasonAcceptableDiffMaster" }))
       {%>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div>
            <label>
                Transaction Type:</label>
            <%: Html.TransactionTypeDropdownListFor(model => model.TransactionTypeId,2)%>
            <%: Html.ValidationMessageFor(model => model.TransactionTypeId)%>
        </div>
        <div>
            <label>
                Reason Code:</label>
            <%: Html.ReasonCodeDropdownListFor(model => model.ReasonCodeId, Model!=null ?Model.TransactionTypeId:0)%>
            <%: Html.ValidationMessageFor(model => model.ReasonCodeId) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Effective From:
            </label>
           <%:Html.TextBox("EffectiveFrom", Model.EffectiveFrom, new { @class = "datePicker", @id = "EffectiveFrom" })%>
            <%: Html.ValidationMessageFor(model => model.EffectiveFrom) %>
        </div>
        <div class="editor-label">
            <label>
                <span class="required">* </span>Effective To:
            </label>
           <%:Html.TextBox("EffectiveTo", Model.EffectiveTo, new { @class = "datePicker", @id = "EffectiveTo" })%>
            <%: Html.ValidationMessageFor(model => model.EffectiveTo) %>
        </div>
        <div class="editor-label">
            <label>
                Weight Charges Amount:
            </label>
            <%: Html.CheckBoxFor(model => model.WeightChargesAmount) %>
            <%: Html.ValidationMessageFor(model => model.WeightChargesAmount) %>
        </div>
        <div class="editor-label">
            <label>
                Valuation Charges Amount:
            </label>
            <%: Html.CheckBoxFor(model => model.ValuationChargesAmount) %>
            <%: Html.ValidationMessageFor(model => model.ValuationChargesAmount) %>
        </div>
        <div class="editor-label">
            <label>
                ISC Amount:
            </label>
            <%: Html.CheckBoxFor(model => model.IscAmount) %>
            <%: Html.ValidationMessageFor(model => model.IscAmount) %>
        </div>
        <div class="editor-label">
            <label>
                OC Amount:
            </label>
            <%: Html.CheckBoxFor(model => model.OcAmount) %>
            <%: Html.ValidationMessageFor(model => model.OcAmount) %>
        </div>
        <div class="editor-label">
            <label>
                VAT Amount:
            </label>
            <%: Html.CheckBoxFor(model => model.VatAmount) %>
            <%: Html.ValidationMessageFor(model => model.VatAmount) %>
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
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","CgoRMReasonAcceptableDiff") %>'" />
        </div>
    </fieldset>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script src="<%:Url.Content("~/Scripts/Masters/CgoRMReasonAcceptableDiffValidate.js")%>" type="text/javascript"></script>
<script type="text/javascript" language="javascript">
    $(document).ready(function () {
        $('#EffectiveFrom').datepicker({
            dateFormat: "yymmdd"
        });
        $('#EffectiveFrom').watermark("YYYYMMPP");
        $('#EffectiveTo').datepicker({
            dateFormat: "yymmdd"
        });
        $('#EffectiveTo').watermark("YYYYMMPP");
    });
    
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
