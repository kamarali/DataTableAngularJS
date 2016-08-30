using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
  //This class will be used for denoting additional outputs as billed and billing entity in Pax, Cargo, Misc and UATP tabs
  public class AdditionalOutputs : EntityBase<int>
  {
    //Other outputs as billed entity

    public bool IsPdfAsOtherOutputAsBilledEntity { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public bool? IsPdfAsOtherOutputAsBilledEntityFutureValue { get; set; }

    public string IsPdfAsOtherOutputAsBilledEntityFutureDate { get; set; }
    /*End Addition*/
   
    public bool IsDetailListingAsOtherOutputAsBilledEntity { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public bool? IsDetailListingAsOtherOutputAsBilledEntityFutureValue { get; set; }

    public string IsDetailListingAsOtherOutputAsBilledEntityFutureDate { get; set; }
    /*End Addition*/

    public bool IsDsFileAsOtherOutputAsBilledEntity { get; set; }
  
    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public bool? IsDsFileAsOtherOutputAsBilledEntityFutureValue { get; set; }

    public string IsDsFileAsOtherOutputAsBilledEntityFutureDate { get; set; }
    /*End Addition*/

    public bool IsSuppDocAsOtherOutputAsBilledEntity { get; set; }
    
    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public bool? IsSuppDocAsOtherOutputAsBilledEntityFutureValue { get; set; }

    public string IsSuppDocAsOtherOutputAsBilledEntityFutureDate { get; set; }
    /*End Addition*/

    public bool IsMemoAsOtherOutputAsBilledEntity { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public bool? IsMemoAsOtherOutputAsBilledEntityFutureValue { get; set; }

    public string IsMemoAsOtherOutputAsBilledEntityFutureDate { get; set; }
    /*End Addition*/

    //Other outputs as billing entity

    public bool IsPdfAsOtherOutputAsBillingEntity { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public bool? IsPdfAsOtherOutputAsBillingEntityFutureValue { get; set; }

    public string IsPdfAsOtherOutputAsBillingEntityFutureDate { get; set; }
    /*End Addition*/

    public bool IsDetailListingAsOtherOutputAsBillingEntity { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public bool? IsDetailListingAsOtherOutputAsBillingEntityFutureValue { get; set; }

    public string IsDetailListingAsOtherOutputAsBillingEntityFutureDate { get; set; }
    /*End Addition*/

    public bool IsDsFileAsOtherOutputAsBillingEntity { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public bool? IsDsFileAsOtherOutputAsBillingEntityFutureValue { get; set; }

    public string IsDsFileAsOtherOutputAsBillingEntityFutureDate { get; set; }
    /*End Addition*/

    public bool IsMemoAsOtherOutputAsBillingEntity { get; set; }
    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public bool? IsMemoAsOtherOutputAsBillingEntityFutureValue { get; set; }

    public string IsMemoAsOtherOutputAsBillingEntityFutureDate { get; set; }
    /*End Addition*/

    public int ParentId { get; set; }
  }
}

