<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.ReasonCode>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
     SIS :: Master Maintenance :: General :: Reason Code Details
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1>
        Reason Code Setup</h1>
    <h2>
        Reason Code Details</h2>
    <fieldset class="solidBox dataEntry">
        <div>
            <div>
                Reason Code:
                <%: Model.Code%><br />
            </div>
            <div>
                Transaction Type:
                <%: Model.TransactionTypeName %><br />
            </div>
            <div>
                Description: <%: Model.Description %><br />
            </div>
            <div>
                Coupon Awb Breakdown Mandatory:
                <%: Model.CouponAwbBreakdownMandatory %><br />
            </div>
            <div>
                Bilateral Code:
                <%: Model.BilateralCode %><br />
            </div>
            <div>
                Active:
                <%: Model.IsActive %><br />
            </div>
            <div>
                Last Updated By:
                <%: Model.LastUpdatedBy %><br />
            </div>
            <div>
                Last Updated On:<%: String.Format("{0:g}", Model.LastUpdatedOn) %><br />
            </div>
        </div>
         <div class="buttonContainer">
            <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("Index","ReasonCode") %>'" />
        </div>
    </fieldset>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
</asp:Content>
