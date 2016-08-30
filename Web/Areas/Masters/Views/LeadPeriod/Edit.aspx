<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.LeadPeriod>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: General :: Edit Lead period
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        Lead Period Value Setup
    </h1>
    <h2>
        Edit Lead Period</h2>
    <% using (Html.BeginForm("Edit", "LeadPeriod", FormMethod.Post, new { @id = "LeadPeriodMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div class="editText">
            <label>
                <span class="required">* </span>Effective From Period:</label>
            <%: Html.TextBox("EffectiveFromPeriod", Model.EffectiveFromPeriod.ToString(FormatConstants.DateFormatFullYear), new { @class = "datePickerMaster" })%>
            <%: Html.ValidationMessageFor(model => model.EffectiveFromPeriod)%>
        </div>
        <div class="editText">
            <label>
                <span class="required">* </span>Effective To Period:</label>
            <%: Html.TextBox("EffectiveToPeriod", Model.EffectiveToPeriod.ToString(FormatConstants.DateFormatFullYear), new { @class = "datePickerMaster", @maxLength = 11 })%>
            <%: Html.ValidationMessageFor(model => model.EffectiveToPeriod)%>
        </div>
        <div class="editText">
            <label>
                <span class="required">* </span>Lead Period:</label>
            <%: Html.TextBoxFor(model => model.Period, new { @id = "Period" })%>
            <%: Html.ValidationMessageFor(model => model.Period)%>
        </div>
        <div class="editText">
            <label>
                <span class="required">* </span>Billing Category:</label>
            <%: Html.BillingCategoryDropdownListFor(model => model.BillingCategoryId, categoryType: "billingcategory")%>
            <%: Html.ValidationMessageFor(model => model.BillingCategoryId)%>
        </div>
        <div class="editText">
            <label>
                <span class="required">* </span>Clearing House:</label>
            <%: Html.TextBoxFor(model => model.ClearingHouse, new { @class = "alphabet upperCase", @maxLength = 1 })%>
        </div>      
        <div class="editText">
            <label>
                Active:</label>
            <%: Html.CheckBoxFor(model => model.IsActive, new {@Checked="checked" })%>
            <%: Html.ValidationMessageFor(model => model.IsActive) %>
        </div>
        <div class="buttonContainer editText">
            <input type="submit" value="Save" class="primaryButton"/>
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","LeadPeriod") %>'" />
        </div>
    </fieldset>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script type="text/javascript" src="<%:Url.Content("~/Scripts/Masters/LeadPeriodValidate.js")%>" ></script>
    <script type="text/javascript" src="<%:Url.Content("~/Scripts/MinAcceptableAmount.js")%>"></script>
    <script type="text/javascript">
        function GetSamplingValue() {
            var samplingIndicator = $("#IsSamplingIndicator").val();
            if (samplingIndicator == "TRUE" || samplingIndicator == "true") {
                $('#SamplingIndicator').val('Y');
            }
            else {
                $("#SamplingIndicator").val('N');

            }
            return false;
        }
        $(document).ready(function () {
            _dateFormat = 'dd-M-yy';
            BindEventForDate();

        });
    </script>
</asp:Content>
