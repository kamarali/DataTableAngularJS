<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.InvPaymentStatus>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: MISC Payment Status Related :: MISC Payment Status Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        MISC Payment Status Setup
    </h1>
    <h2>
        Add MISC Payment Status</h2>
    <% using (Html.BeginForm("Create", "InvPaymentStatus", FormMethod.Post, new { @id = "InvPaymentStatusMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>
    <fieldset class="solidBox dataEntry">
        <div>
            <label>
                <span class="required">* </span>Payment Status Description:</label>
            <%: Html.TextBoxFor(invPaymentStatus => invPaymentStatus.Description, new { @Class = "alphaNumericWithSpace", @maxLength = 100 })%>
            <%: Html.ValidationMessageFor(invPaymentStatus => invPaymentStatus.Description)%>
        </div>
         <div>
            <label>
                <span class="required">* </span>Applicable For:</label>
            <%: Html.InvPaymentStatusApplicableForDropdownListFor(model => model.ApplicableFor, "ApplicableFor")%>
            <%: Html.ValidationMessageFor(invPaymentStatus => invPaymentStatus.ApplicableFor)%>
        </div>
        <div>
            <label>
                Active:</label>
            <%: Html.CheckBoxFor(invPaymentStatus => invPaymentStatus.IsActive, new { @checked = "checked" })%>
            <%: Html.ValidationMessageFor(invPaymentStatus => invPaymentStatus.IsActive)%>
        </div>
        <div class="buttonContainer">
            <input type="submit" value="Save" class="primaryButton" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","InvPaymentStatus") %>'" />
        </div>
    </fieldset>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script src="<%:Url.Content("~/Scripts/Masters/InvPaymentStatusValidate.js")%>" type="text/javascript"></script>
</asp:Content>
