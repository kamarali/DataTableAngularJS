<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    PaxOldIdec
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Generate Passenger Old IDEC File</h2>
    <% using (Html.BeginForm("Index", "PaxOldIdec", FormMethod.Post, new { @id = "GeneratePaxOldIdec" }))
       {%>
       <%: Html.AntiForgeryToken() %>
       
<div class="solidBox">
    <div class="fieldContainer horizontalFlow">
        <div>
            <div>
                <label>
                    <span class="required">* </span>Billing Year:</label>
                <%: Html.BillingYearDropdownList("BillingYear",0)%>
            </div>
            <div>
                <label>
                    <span class="required">* </span>Billing Month:</label>
                <%: Html.BillingMonthDropdownList("BillingMonth",0)%>
            </div>
        </div>
    </div>
    <div class="clear">
    </div>

    <div class="buttonContainer">
        <input type="submit" class="primaryButton" value="Generate" id="btnSearch" name="Search" />
    </div></div>
    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
</asp:Content>
