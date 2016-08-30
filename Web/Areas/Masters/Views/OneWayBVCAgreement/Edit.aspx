<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.BvcAgreement>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: General :: BVC Agreement Master
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h1>BVC Agreement Setup</h1>
 
  
    <h2>Edit BVC Agreement</h2>

    <% using (Html.BeginForm(Html.BeginForm("Edit", "OneWayBVCAgreement", FormMethod.Post, new { @id = "BVCAgreementMaster" })))
       {%>
       <%: Html.AntiForgeryToken() %>
       <%: Html.HiddenFor(model => model.BvcMappingId)%> 
    <fieldset class="solidBox dataEntry">
        <div>
            <label>
                <span class="required">* </span>Billing Member:</label>
            <%: Html.TextBoxFor(model => model.BillingMemberText, new { @class = "autocComplete"})%>
             <%:Html.HiddenFor(model => model.BillingMemberId)%>
              <%: Html.HiddenFor(model => model.BillingMemberText)%>             
        </div>
        <div>
            <label>
               <span class="required">* </span>Billed Member:</label>
            <%: Html.TextBoxFor(model => model.BilledMemberText, new { @class = "autocComplete" })%>
             <%:Html.HiddenFor(model => model.BilledMemberText)%>
            <%:Html.HiddenFor(model => model.BilledMemberId)%>
        </div>
        <div>
            <label>
                Active:
            </label>
            <%: Html.CheckBoxFor(model => model.IsActive)%>
       </div>
        <div class="buttonContainer">
            <input type="submit" value="Save" class="primaryButton" />
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","OneWayBVCAgreement") %>'" />
        </div>
    </fieldset>

    <% } %>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script src="<%:Url.Content("~/Scripts/Masters/BvcAgreementValidate.js")%>" type="text/javascript"></script>
<script type="text/javascript" language="javascript">
    registerAutocomplete('BillingMemberText', 'BillingMemberId', '<%:Url.Action("GetBVCMemberList", "Data", new { area = "", isBilling = true  })%>', 0, true, null);
    registerAutocomplete('BilledMemberText', 'BilledMemberId', '<%:Url.Action("GetBVCMemberList", "Data", new { area = "" , isBilling = false })%>', 0, true, null);
</script>
</asp:Content>