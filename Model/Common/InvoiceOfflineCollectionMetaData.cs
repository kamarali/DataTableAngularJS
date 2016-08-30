using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.Enums;

namespace Iata.IS.Model.Common
{
  /// <summary>
  /// represents invoice offline collections meta data
  /// </summary>
  public class InvoiceOfflineCollectionMetaData : EntityBase<Guid>
  {
    /// <summary>
    /// Gets or sets the period no.
    /// </summary>
    /// <value>The period no.</value>
    public int PeriodNo { get; set; }
    /// <summary>
    /// Gets or sets the billing year.
    /// </summary>
    /// <value>The billing year.</value>
    public int BillingYear { get; set; }
    /// <summary>
    /// Gets or sets the billing month.
    /// </summary>
    /// <value>The billing month.</value>
    public int BillingMonth { get; set; }
    /// <summary>
    /// Gets or sets the billing member numeric code.
    /// </summary>
    /// <value>The billing member code.</value>
    public string BillingMemberCode { get; set; }
    /// <summary>
    /// Gets or sets the billed member numeric code.
    /// </summary>
    /// <value>The billed member code.</value>
    public string BilledMemberCode { get; set; }
    /// <summary>
    /// Gets or sets the billing category id.
    /// </summary>
    /// <value>The billing category id.</value>
    public int BillingCategoryId { get; set; }
    /// <summary>
    /// Gets the billing category.
    /// </summary>
    /// <value>The billing category.</value>
    public BillingCategoryType BillingCategory { get { return (BillingCategoryType)this.BillingCategoryId; } }
    /// <summary>
    /// Gets or sets the invoice number.
    /// </summary>
    /// <value>The invoice number.</value>
    public string InvoiceNumber { get; set; }
    /// <summary>
    /// Gets or sets the offline collection folder type id.
    /// </summary>
    /// <value>The offline collection folder type id.</value>
    public int OfflineCollectionFolderTypeId { get; set; }
    /// <summary>
    /// Gets or sets the type of the offline collection folder.
    /// </summary>
    /// <value>The type of the offline collection folder.</value>
    public OfflineCollectionFolderType OfflineCollectionFolderType { get; set; }

    /// <summary>
    /// Gets or sets the file path.
    /// </summary>
    /// <value>The file path.</value>
    public string FilePath { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is form C.
    /// </summary>
    /// <value><c>true</c> if this instance is form C; otherwise, <c>false</c>.</value>
    public bool IsFormC { get; set; }

    /// <summary>
    /// Gets or sets the Provisional Billing Month in case of FormC
    /// </summary>
    public int ProvisionalBillingMonth { get; set; }

    /// <summary>
    /// Gets or sets the Provisional Billing Year in case of FormC
    /// </summary>
    public int ProvisionalBillingYear { get; set; }
  }
}