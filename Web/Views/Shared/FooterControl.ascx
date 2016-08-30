<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Iata.IS.Web" %>
<div style="float: left;">
  &copy; <%: ConfigurationManager.AppSettings["CopyRightYear"]%> International Air Transport Association. All rights reserved.
</div>
<div style="float: right;">
  Build:
  <%: ConfigurationManager.AppSettings["VersionNumber"]%>
  | DB:
  <%: ConfigurationManager.AppSettings["DBServerName"]%>
</div>
