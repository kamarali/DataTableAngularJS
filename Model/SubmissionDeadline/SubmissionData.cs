using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.SubmissionDeadline
{
   public class SubmissionData
    {
       /// <summary>
       /// Property to get and set Billing category
       /// </summary>
       public string BillingCategory { get; set; }

       /// <summary>
       /// Property to get and set Billing Member Id
       /// </summary>
       public int BillingMemberId { get; set; }

       /// <summary>
       /// Property to get and set Billing Numeric Code
       /// </summary>
       public string BillingNumericCode { get; set; }

       /// <summary>
       /// Property to get and set Billed Member Id
       /// </summary>
       public int BilledMemberId { get; set; }

       /// <summary>
       /// Property to get and set Billed Numeric Code
       /// </summary>
       [DisplayName("Billed Member Code")]
       public string BilledNumericCode { get; set; }

       /// <summary>
       /// Property to get and set Billed Alpha Code
       /// </summary>
       public string BilledAlphaCode { get; set; }

       /// <summary>
       /// Property to get and set Billed Entity Name
       /// </summary>
       public string BilledEntityName { get; set; }

       /// <summary>
       /// Property to get and set invoice No
       /// </summary>
       [DisplayName("Invoice Number")]
       public string InvoiceNo { get; set; }

       /// <summary>
       /// Property to get adn set Invoice Ammount
       /// </summary>
       [DisplayName("Invoice Amount")]
       public string InvoiceAmmount { get; set; }

       /// <summary>
       /// Property to get and set invoice status
       /// </summary>
       [DisplayName("Invoice Status")]
       public string InvoiceType { get; set; }

       /// <summary>
       /// Property to get and set billing Category id
       /// </summary>
       public int BillingCategoryId { get; set; }
    }// End SubmissionData
}// End namespace
