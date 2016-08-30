using System;
using System.ComponentModel;
using System.Reflection;
using Iata.IS.Model.Base;
using Iata.IS.Model.Enums;

namespace Iata.IS.Model.Common
{
  /// <summary>
  /// Represents an airport in the system.
  /// </summary>
  
  [Serializable]
    public class InvPaymentStatus : EntityBase<int>
  {
      /// <summary>
      /// Gets or sets the Misc Payment Status Description.
      /// </summary>
      /// <value>The Misc Payment Status Description</value>
      public string Description { get; set; }

      /// <summary>
      /// Gets or sets the Misc Payment Status Applicable For.
      /// </summary>
      /// <value>The Misc Payment Status Applicable For</value>
      public int ApplicableFor { get; set; }
      public string Applicable
      {
          get
          {
              String applicable = "";

              switch ((InvPaymentStatusApplicableFor)ApplicableFor)
              {
                  case InvPaymentStatusApplicableFor.BillingMember:
                      applicable = "Billing Member";
                      break;
                  case InvPaymentStatusApplicableFor.BilledMember:
                      applicable = "Billed Member";
                      break;
                  default:
                      break;

              }

              return applicable;
          }

          //get
          //{
          //    //return ((ApplicableFor)ApplicableFor).ToString();
          //    return EnumDescriptionExtensions.GetDescription((MiscPaymentStatusApplicableFor)ApplicableFor);

          //}
      }

      /// <summary>
      /// Gets or sets the Misc Payment Status IsActive.
      /// </summary>
      /// <value>The Misc Payment Status IsActive</value>
      public bool IsActive { get; set; }

      public bool IsSystemDefined { get; set; }

  }

}