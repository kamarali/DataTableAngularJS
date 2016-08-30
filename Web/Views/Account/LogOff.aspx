<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Anonymous.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Logged Off
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <img alt="Simplified Interline Settlement" src="<%: Url.Content("~/Content/Images/SIS_login_banner_new.png") %>"
        id="banner" />
    <table>        
        <tr>
            <td>
                <div style="width: 100%; float: none;">
                    <table class="basictable" width="600px"  style=" font-size:10pt; text-align:center;  border: 1px solid #000; height:80px;">
                       
                        <tr>
                            <td style=" font-size:10pt;">
                                 You have been successfully logged off <br /> <br />
                                We recommend that you close the browser window
                            </td>
                           
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
    </table>
    <br />
 
</asp:Content>

