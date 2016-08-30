<!--CMP527: Control added to show acceptance information on audit trail page.  -->
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<br />
<%if (!string.IsNullOrEmpty(Model.AcceptanceComment)){ %>
 Accepted On:&nbsp;<%:Model.AcceptanceDateTime.ToString("dd-MMM-yy, HH:mm")%> <br />   <br />  
 Accepted By:&nbsp;<%:Model.AcceptanceUserName%><br />   <br />  
 Acceptance Comments:<br />
 <p><%:Model.AcceptanceComment%></p>
<%}%>