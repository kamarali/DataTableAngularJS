<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.BvcAgreement>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: General :: BVC Agreement Master
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        BVC Agreement Setup
    </h1>
    <h2>
        Create BVC Agreement
    </h2>
    <% using (Html.BeginForm("Create", "OneWayBVCAgreement", FormMethod.Post, new { @id = "BVCAgreementMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div>
            <label>
                <span class="required">* </span>Billing Member:</label>
            <%:Html.TextBoxFor(model => model.BillingMemberText, new { @class = "autocComplete",style="50px" })%>
            <%:Html.HiddenFor(model => model.BillingMemberId)%>
            <%: Html.ValidationMessageFor(model => model.BillingMemberId)%>
        </div>
        <div>
            <label>
                <span class="required">* </span>Billed Member:</label>
            <%: Html.TextBoxFor(model => model.BilledMemberText, new { @class = "autocComplete" })%>
            <%:Html.HiddenFor(model => model.BilledMemberId)%>
            <%: Html.ValidationMessageFor(model => model.BilledMemberId)%>
        </div>
        <div>
            <label>
                Active:
            </label>
            <%: Html.CheckBoxFor(model => model.IsActive, new { @checked = "checked" })%>
            <%: Html.ValidationMessageFor(model => model.IsActive)%>
        </div>
        <div class="buttonContainer">
            <input type="submit" value="Save" class="primaryButton" id="btnSave"/>
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","OneWayBVCAgreement") %>'" />
        </div>
    </fieldset>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script src="<%:Url.Content("~/Scripts/Masters/BvcAgreementValidate.js")%>" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
            registerAutocomplete('BillingMemberText', 'BillingMemberId', '<%:Url.Action("GetBVCMemberList", "Data", new { area = "", isBilling = true  })%>', 0, true, null);
            registerAutocomplete('BilledMemberText', 'BilledMemberId', '<%:Url.Action("GetBVCMemberList", "Data", new { area = "" , isBilling = false })%>', 0, true, null);

            $('#BillingMemberText').focus();
        });   

    </script>    
</asp:Content>
