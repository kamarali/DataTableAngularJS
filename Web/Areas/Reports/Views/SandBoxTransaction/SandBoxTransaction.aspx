<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.SandBox.CertificationTransactionDetailsReport>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: SandBoxTransaction
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% Html.BeginForm("", "", FormMethod.Post, new { id = "SandBoxTransaction" }); %>
    <h1>
        Sandbox Testing Report</h1>
    <div>        
        <% Html.RenderPartial("SandBoxTransactionSearch"); %>
    </div>
    <div />    
    <div class="buttonContainer">        
        <input type="submit" class="primaryButton" value="Generate Report" />
    </div>
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />  
    <%Html.EndForm(); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script type="text/javascript" src="<%:Url.Content("~/Scripts/Reports/SandBoxTransaction.js")%>"></script>
    <script type="text/javascript">        
        $(document).ready(function () {            
            SandBoxTransactionValidation("SandBoxTransaction");
        });
    </script>
</asp:Content>
