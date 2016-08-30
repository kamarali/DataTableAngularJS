<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConfirmationDetailReport.aspx.cs" Inherits="Iata.IS.Web.ConfirmationDetailReport" %>

<%@ Register TagPrefix="CR" Namespace="CrystalDecisions.Web" %>
<%@ Register assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" namespace="CrystalDecisions.Web" tagprefix="CR" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Passenger IS Data Value Confirmation Details Report</title>
    <style type="text/css">
    .crHeader { background-color:#000000; }
    
    .primaryButton {
     background : url("") no-repeat scroll 0 0 #EF9C00;
     border : 1px solid #CA8402;
     }
    
    </style>
</head>
<body>
    <form id="form1" runat="server">
      
      <div> Go to Page#: &nbsp; 
      <asp:DropDownList ID="ddlPaging" runat="server" OnSelectedIndexChanged="ddlPaging_SelectedIndexChanged" AutoPostBack="true" />
      &nbsp; &nbsp; &nbsp; 
      <asp:Button ID="Button1" runat="server" CssClass="primaryButton"  Text="Export To CSV" onclick="Button1_Click" />

    </div>
    
    <div>
    <CR:CrystalReportViewer ID="CRViewer" runat="server" 
        AutoDataBind="True" Height="100%" 
        ToolbarImagesFolderUrl="" ToolPanelWidth="100%" Width="100%" 
            DisplayStatusbar="false"  ToolPanelView="None" HasCrystalLogo="False" />
    </div>
    </form>
</body>
</html>





