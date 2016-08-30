﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%--<%@ Import Namespace="System.Security.Policy" %>--%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Reports :: <%=ViewData["BillingType"]%> :: Cargo Submission Overview 
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

  <% Html.BeginForm("", "", FormMethod.Post, new { id = "CargoSubmissionOverview" }); %>

    <h1>Cargo Submission Overview - Report for <%=ViewData["BillingType"]%></h1>
     
    <div>
        <% Html.RenderPartial("SearchControlForSubmissionOverview",ViewData); %>
    </div>
    <div>
    </div>
    <div class="buttonContainer">
        <input type="submit" id="generateButton" class="primaryButton" value="Generate Report" onclick="GenerateReport()" />
    </div>
     <%Html.EndForm(); %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Reports/SubmissionOverview.js")%>"></script>
      <script type="text/javascript">
          $(document).ready(function () {

              $("#MemberCode").change(function () {
                  if ($("#MemberCode").val() == '') {
                      $("#MemberId").val("");
                  }
              });
              /*CMP #596: Length of Member Accounting Code to be Increased to 12 
              Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
              Ref: FRS Section 3.4 Table 15 Row 43 and 48 */
              registerAutocomplete('MemberCode', 'MemberId', '<%:Url.Action("GetMemberListForPaxCgo", "Data",  new  {  area = "" })%>', 0, true, null);
              
          });

          function GenerateReport() {
          //To identify if form is called for payable or receivable
              ValidateOverviewDocument('CargoSubmissionOverview<%=ViewData["BillingType"]%>');
          }
</script>
</asp:Content>