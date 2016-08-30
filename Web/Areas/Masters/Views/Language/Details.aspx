<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.Language>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: General :: Language Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

 <h1>Language Setup</h1>
     <h2>Language Details</h2>

     
    <fieldset class="solidBox dataEntry">
        <div>
            <div>
                Language Code:
                <%: Model.Language_Code%><br />
            </div>
            
            <div>
                Language Description: 
                <%: Model.Language_Desc%><br />
            </div>

            <div>
                Is Required for Help:
                <%: Model.IsReqForHelp %><br />
            </div>
            
             <div>
                Is Required for PDF:
                <%: Model.IsReqForPdf %><br />
            </div>

            <div>
                Last Updated By:
                <%: Model.LastUpdatedBy %><br />
            </div>

            <div>
                Last Updated On:
                <%: String.Format("{0:g}", Model.LastUpdatedOn)%><br />
            </div>
        </div>
         <div class="buttonContainer">
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","Language") %>'" />
        </div>
    </fieldset>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
</asp:Content>