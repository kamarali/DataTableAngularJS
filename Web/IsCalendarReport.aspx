<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IsCalendarReport.aspx.cs"
    Inherits="Iata.IS.Web.IsCalendarReport" %>

<%@ Register TagPrefix="CR" Namespace="CrystalDecisions.Web" %>
<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304"
    Namespace="CrystalDecisions.Web" TagPrefix="CR" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>IS and CH Clearing House Calendar Report</title>
    <link href="Content/Site.css" rel="stylesheet" type="text/css" />
    <link href="Content/Forms.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div id="errorDiv" runat="server" visible="false">
        <div class="errorMessage">
            <label>
                <img style="border-style: none; vertical-align: middle" src='/Content/Images/error_icon.png'
                    alt="Error" />No records available to display.</label>
        </div>
    </div>
    <div>
        <CR:CrystalReportViewer ID="CRViewer" runat="server" AutoDataBind="True" Height="100%"
            ToolbarImagesFolderUrl="" ToolPanelWidth="100%" Width="100%" DisplayStatusbar="false"
            ToolPanelView="None" HasCrystalLogo="False" />
    </div>
    </form>
</body>
</html>
