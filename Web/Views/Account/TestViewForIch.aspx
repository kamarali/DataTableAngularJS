<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<form id='test' >
<br />
 UserID = <%: TempData["UserID"] %>   <br />
 MemberID = <%: TempData["MemberID"]%>  <br />  
 ReportOption = <%: TempData["ReportOption"]%> <br />  
 Timestamp = <%: TempData["Timestamp"]%>  <br />

</form>