using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;
using Iata.IS.Model.Enums;

namespace Iata.IS.Model.Common
{
    public enum BillingCategorys
    {
       Passenger=1,
       Cargo=2,
       Miscellaneous=3,
       Uatp=4
    }

  public class LeadPeriod : MasterBase<int>
  {
    
    /// <summary>
    /// Gets or sets the period.
    /// </summary>
    /// <value>The period.</value>
    public int Period { get; set; }

    /// <summary>
    /// Gets or sets the clearing house.
    /// </summary>
    /// <value>The clearing house.</value>
    public string ClearingHouse { get; set; }

      /// <summary>
      /// Gets or sets the sampling indicator.
      /// </summary>
      /// <value>The sampling indicator.</value>
    public string SamplingIndicator{ get; set; }

    /// <summary>
    /// Gets the value for samplingIndicator
    /// </summary>
    public string  SamplingIndicatorValue
    {
          get
          {
              string samplingIndicator = string.Empty;
              if (!string.IsNullOrEmpty(SamplingIndicator))
              {
                  samplingIndicator = SamplingIndicator.Equals("Y") ? "Yes" : "No";
              }
              return samplingIndicator;
          }
    }

      /// <summary>
      /// Gets or sets  the Issampling indicator.
      /// </summary>
      /// <value>The sampling indicator.</value>
      public bool IsSamplingIndicator
      {
          get
          {
              var isSampling = false;
              if(SamplingIndicator!=null)
              {
                 isSampling=SamplingIndicator.Equals("Y")?true:false;
              }
              return isSampling;
          }
      }

      /// <summary>
      /// Gets or sets the billing category id.
      /// </summary>
      /// <value>The billing category id.</value>
      public BillingCategorys BillingCategory
      {
          get { return ((BillingCategorys) BillingCategoryId); }
      }
    
    /// <summary>
    /// Gets or sets the billing category id.
    /// </summary>
    /// <value>The billing category id.</value>
    public int BillingCategoryId { get; set; }

      /// <summary>
      /// Gets or sets the billing category id.
      /// </summary>
      /// <value>The billing category id.</value>
      public string BillingCategoryValue
      {
          get { return (BillingCategory.ToString()); }
      }

    /// <summary> 
    /// Gets or sets the effective from period.
    /// </summary>
    /// <value>The effective from period.</value>
    public DateTime EffectiveFromPeriod { get; set; }

    /// <summary>
    /// Gets or sets the effective from period.
    /// </summary>
    /// <value>The effective to period.</value>
    public DateTime EffectiveToPeriod { get; set; }
  }
}
