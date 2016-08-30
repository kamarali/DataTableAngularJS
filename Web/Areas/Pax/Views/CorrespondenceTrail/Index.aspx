<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="Iata.IS.Business.Security.Permissions.Pax" %>
<%@ Import Namespace="Iata.IS.Model.Common" %>
<%@ Import Namespace="Iata.IS.Model.Cargo.BillingHistory" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<%@ Import Namespace="System.Security.Policy" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger :: Correspondence Report
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Pax/CorrespondenceTrail.js")%>"></script>
  <script type="text/javascript"> 
   var loggedInMemberId = <%:SessionUtil.MemberId%>;
    
    $(document).ready(function () {   
     RequestForCorrespondenceTrailReportUrl = '<%:Url.Action("RequestForCorrespondenceTrailReport","CorrespondenceTrail",new {area = "Pax"}) %>';
     RequestForCorrespondenceTrailReportAllUrl = '<%:Url.Action("RequestForCorrespondenceTrailReportAll","CorrespondenceTrail",new {area = "Pax"}) %>'; 
     InitialiseBillingHistory();  
     /*CMP #596: Length of Member Accounting Code to be Increased to 12 
      Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
      Ref: FRS Section 3.4 Table 15 Row 19 */
     registerAutocomplete('CorrBilledMemberText', 'CorrBilledMemberId', '<%:Url.Action("GetMemberListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);    
    });  
 
    
  </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1>Correspondence Reports</h1>
  <%
    using (Html.BeginForm("Index", "CorrespondenceTrail", FormMethod.Post, new { id = "corrSearchCriteria" }))
    {%>
  <div>
    <% Html.RenderPartial("CorrTrailSearchCriteria", ViewData[ViewDataConstants.CorrespondenceTrailSearchCriteria] as CorrespondenceTrailSearchCriteria);%>
  </div>
  <div class="buttonContainer">
    <input type="submit" value="Search" class="primaryButton" id="SearchCorrespondence" onclick="javascript:return(validateMaxDateRange())" />
    <input class="secondaryButton" type="button" onclick="ResetCorrespondence();" value="Clear" />
  </div>
  <%
    }
  %>
  <div id="searchGrid">
    <h2>
      Search Results</h2>
    (Details of the last correspondence)
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.CorrespondenceTrailSearchResultGrid]);%>
  </div>
  <div class="buttonContainer" id="gridButtons">
    <input type="button" value="Download All" class="primaryButton" id="CorrTrailReportRequestAllButton" onclick="CorrTrailReportRequestAll();" />
    <input type="button" value="Download Selected" class="secondaryButton" id="CorrTrailReportRequestButton" onclick="CorrTrailReportRequest();" />
  </div>
</asp:Content>
