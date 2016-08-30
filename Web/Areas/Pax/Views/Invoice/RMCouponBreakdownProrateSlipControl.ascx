<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.RMCoupon>" %>
<div>
    <% Html.RenderPartial(Url.Content("ReadOnlyInvoiceHeaderControl"), Model.RejectionMemoRecord.Invoice); %>
</div>
<h2>
    Rejection Memo Coupon Breakdown Prorate Slip</h2>
<div class="solidBox dataEntry">
    <div class="fieldContainer horizontalFlow">
        <div id="divBlock1">
            <div>
                <%: Html.TextAreaFor(rmCbPs => rmCbPs.ProrateSlipDetails,10,80,null)%>
            </div>
        </div>
    </div>
</div>
