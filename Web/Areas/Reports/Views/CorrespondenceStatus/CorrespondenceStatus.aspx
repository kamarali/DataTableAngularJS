<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="System.Security.Policy" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	SIS :: Reports :: <%=ViewData["CategoryName"] %> :: Correspondence Status Report 
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">


     <% Html.BeginForm("", "", FormMethod.Post, new { id = "corrSearchCriteria" }); %>
    <h1>
          <span id="headers"></span>  </h1>
    <div>
        <% Html.RenderPartial("CorrespondenceSearch", ViewData); %>
    </div>
    <div/>
    
    <div class="buttonContainer">
        <input type="button" id="generateButton" class="primaryButton" value="Generate Report" onclick = "GenerateReport();" />
        
    </div>
     <%Html.EndForm(); %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">

<script type="text/javascript" src="<%:Url.Content("~/Scripts/Reports/Correspondence.js")%>"></script>
      <script type="text/javascript">
          $(document).ready(function () {
              //CMP526 - Passenger Correspondence Identifiable by Source Code
              registerAutocomplete('SourceCode', 'SourceCode', '<%:Url.Action("GetSourceCodeListForCorrespondence", "Data", new { area = "" })%>', 0, true, null, '<%:Convert.ToInt32(Iata.IS.Model.Enums.BillingCategoryType.Pax)%>');
              $("#CorrespondenceStatusId").bind("change", SetSubStatus);
              var cat = '<%=ViewData["CategoryName"]%>';

              $('#headers').text(cat.concat(": Correspondence status report"));
              /*CMP #596: Length of Member Accounting Code to be Increased to 12 
              Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
              Ref: FRS Section 3.4 Table 15 Row 41 and 53 */
              /* check category to decide whether type B members should be included or not since this page 
                 is also used to display pax and cargo correspondance status*/
              if (cat == 'Miscellaneous') {
                  registerAutocomplete('MemberCode', 'MemberId', '<%:Url.Action("GetMemberList", "Data",  new  {  area = "" })%>', 0, true, null);
              }
              else {
                  registerAutocomplete('MemberCode', 'MemberId', '<%:Url.Action("GetMemberListForPaxCgo", "Data",  new  {  area = "" })%>', 0, true, null);
              }

              $("#MemberCode").change(function () {
                  if ($("#MemberCode").val() == '') {
                      $("#MemberId").val("");
                  }
              });

          });

          function GenerateReport()
          {
           var DateFrom = $("#Fromdate").datepicker("getDate");
           var DateTo = $("#ToDate").datepicker("getDate");

           if (DateFrom.getTime() > DateTo.getTime()) {
               alert("From Date : " + $('#Fromdate').val()+ "\n     To Date : " + $('#ToDate').val() + "\nThe Combination of 'To' Date Should Not Be Earlier Than The Combination of 'From' Date.");
               return false;
           }
           else{
              var cat = '<%=ViewData["Category"].ToString() %>';
              validateCorrespondence("corrSearchCriteria", cat);
              return true;
           }
         };        
</script>
</asp:Content>
