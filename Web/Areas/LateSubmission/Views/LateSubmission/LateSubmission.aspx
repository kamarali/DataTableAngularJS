<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Manage Late Submissions
</asp:Content>
<asp:Content ID="scriptBlock" ContentPlaceHolderID="Script" runat="server">
    <script type="text/javascript" src="<%:Url.Content("~/Scripts/LateSubmission.js")%>"></script>
   
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1>Manage Late Submissions</h1>
<script type="text/javascript" language="javascript">
    $(document).ready(function () {
        $("#helplink").hide();
    });
</script>
    <div class="hidden clientMessage clientSuccessMessage" id="clientSuccessMessageContainer"
        style="display: none;">
        <div style="float: left; width: 20px;">
            <img alt="Success" src="/Content/Images/success_message.png">
        </div>
        <div id="clientSuccessMessage" style="margin-left: 23px;"></div>
    </div>
    
    <h2>
        <%:ViewData["header"]%>
        <input type="hidden" id="lateSubWindowStatus" value="<%:ViewData["lateSubStatus"]%>" />
         <input type="hidden" id="categoryId" value="<%:ViewData["userCategory"]%>" /></h2>
    <div>
        <% Html.RenderPartial("LateSubmissionControl", ViewData["LateSubmissionGrid"]); %>
    </div>
    <div>
        <% Html.RenderPartial("LateSubmissionInvoiceDetailsControl", ViewData["LateSubmissionInvoiceDetailsGrid"]); %>
    </div>
    <div class="buttonContainer">
        <input type="button" class="primaryButton" value="Accept Selected" onclick="AcceptInvoices('<%: Url.Action("AcceptInvoices", "LateSubmission", new { area = "LateSubmission"}) %>')" />
        <input type="button" class="primaryButton" value="Reject Selected" onclick="RejectInvoices('<%: Url.Action("RejectInvoices", "LateSubmission", new { area = "LateSubmission"}) %>')" />
        <input type="button" class="primaryButton" value="Close Late Submission" onclick="LateSubmissionWindowClose('<%: Url.Action("CloseLateSubmissionWindow", "LateSubmission", new { area = "LateSubmission"}) %>')" />
    </div>
</asp:Content>
