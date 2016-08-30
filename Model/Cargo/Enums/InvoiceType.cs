namespace Iata.IS.Model.Cargo.Enums
{
    /// <summary>
    /// Used to distinguish between an invoice and a credit-note.
    /// </summary>
    public enum InvoiceType
    {
        /// <summary>
        /// Invoice.
        /// </summary>
        Invoice = 1,

        /// <summary>
        /// Credit note.
        /// </summary>
        CreditNote = 2,

        /// <summary>
        /// RejectionInvoice
        /// </summary>
        RejectionInvoice = 3,

        /// <summary>
        /// Correspondence Invoice
        /// </summary>
        CorrespondenceInvoice = 4
    }
}
