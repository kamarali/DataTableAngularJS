<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RMBMCMDetailsReport.aspx.cs" Inherits="Iata.IS.Web.RMBMCMDetailsReport" %>

<%@ Register TagPrefix="CR" Namespace="CrystalDecisions.Web" %>
<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304"
    Namespace="CrystalDecisions.Web" TagPrefix="CR" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>RM-BM-CM Details Report</title>
    <style type="text/css">
        .crHeader
        {
            background-color: #B3D5F1;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <CR:CrystalReportViewer ID="RMBMCMReport" runat="server" AutoDataBind="True"
            Height="100%" ToolbarImagesFolderUrl="" ToolPanelWidth="100%" Width="100%" DisplayStatusbar="false"
            ToolPanelView="None" HasCrystalLogo="False" />
    </div>
    </form>
</body>
</html>
