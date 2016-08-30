<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.CouponRejectionBreakdownRecord>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Passenger :: Receivables :: Non-Sampling Invoice :: Create Rejection Memo
    Coupon Breakdown
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script src="<%=Url.Content("~/Scripts/CouponRecord.js")%>" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#txtProrateSlip").bind("keypress", function () { maxLength(this, 4000) });
            $("#txtProrateSlip").bind("paste", function () { maxLengthPaste(this, 4000) });
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        Create Rejection Memo Coupon
    </h1>
    <div>
        <%
            Html.RenderPartial(Url.Content("ReadOnlyInvoiceHeaderControl"), Model.RejectionMemoRecord.Invoice);%>
    </div>
    <div>
        <%
            Html.RenderPartial("RMCouponBreakdownDetailsControl", Model);%>
    </div>
    <div class="buttonContainer">
        <input type="submit" value="Save and Add New" class="primaryButton" id="btnSaveAndAddNew" />
        <input type="submit" value="Save and Duplicate" class="primaryButton" id="btnSaveAndDuplicate" />
        <input type="button" value="Save and Back to Overview" class="primaryButton" id="SaveAndBackToOverview" />
        <input class="secondaryButton" type="button" value="Back" onclick="javascript:history.go(-1);" />
        <div>
            <input type="submit" value="Prorate Slip Details" class="secondaryButton" />
        </div>
        <%= Iata.IS.Web.Util.ScriptHelper.GenerateDialogueHtmlForButton("Prorate Slip Details", "Create RM Coupon Breakdown Prorate Slip", "divProrateSlip", 550, 850)%>
        <div id="divProrateSlip" class="Hidden">
            <%Html.RenderPartial("RMCouponBreakdownProrateSlipControl", Model); %>
        </div>
    </div>
</asp:Content>
