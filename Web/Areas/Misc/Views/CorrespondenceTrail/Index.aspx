<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="Iata.IS.Business.Security.Permissions.Pax" %>
<%@ Import Namespace="Iata.IS.Model.Common" %>
<%@ Import Namespace="Iata.IS.Model.Cargo.BillingHistory" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<%@ Import Namespace="System.Security.Policy" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Miscellaneous :: Correspondence Report
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Misc/CorrespondenceTrail.js")%>"></script>
  <script type="text/javascript"> 
   var loggedInMemberId = <%:SessionUtil.MemberId%>;
    
    $(document).ready(function () {   
     RequestForCorrespondenceTrailReportUrl = '<%:Url.Action("RequestForCorrespondenceTrailReport","CorrespondenceTrail",new {area = "Misc"}) %>';
     RequestForCorrespondenceTrailReportAllUrl = '<%:Url.Action("RequestForCorrespondenceTrailReportAll","CorrespondenceTrail",new {area = "Misc"}) %>'; 
     InitialiseBillingHistory();  
     registerAutocomplete('CorrBilledMemberText', 'CorrBilledMemberId', '<%:Url.Action("GetMemberList", "Data", new { area = "" })%>', 0, true, null);    
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
