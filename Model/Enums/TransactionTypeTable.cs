using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Enums
{
    /// <summary>
    /// Table name to find transactions releated invoice id.
    /// </summary>
    public enum TransactionTypeTable
    {
        NONE = 0,
        INVOICE = 1,
        CGO_AIR_WAY_BILL = 3,
        CGO_BM_AWB = 4,
        CGO_BILLING_MEMO = 5,
        CGO_REJECTION_MEMO = 6,
        CGO_RM_AWB = 7,
        CGO_CM_AWB = 8,
        CGO_CREDIT_MEMO = 9,
        PAX_BM_COUPON_BREAKDOWN = 10,
        PAX_BILLING_MEMO = 11,
        PAX_REJECTION_MEMO = 12,
        PAX_RM_COUPON_BREAKDOWN = 13,
        PAX_FORM_E_VAT_BREAKDOWN = 14,
        PAX_FORM_E_PROV_INVOICE = 15,
        PAX_FORM_D_COUPON_RECORD = 16,
        PAX_FORM_C = 17,
        PAX_CM_COUPON_BREAKDOWN = 18,
        PAX_CREDIT_MEMO = 19,
        PAX_COUPON_RECORD = 20,
        MU_LINE_ITEM = 21,
        PAX_FORM_C_DETAIL =22,
        CGO_INVOICE_TOTAL_VAT = 23,
        PAX_INVOICE_TOT_VAT_BREAKDOWN = 24,
        PAX_CORRESPONDENCE = 25,
        CGO_CORRESPONDENCE = 26,
        MU_CORRESPONDENCE = 27,
        VALIDATION_EXCEPTION_SUMMARY = 28
    }

    /// <summary>
    /// Use to indicate invoice can be edit or delete.
    /// </summary>
    public enum InvoiceEditStatus
    {
        NoneEditable = 0,
        Editable = 1,
        InvoiceDeleted = 2
    }
}
