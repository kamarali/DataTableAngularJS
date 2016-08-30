using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Master
{
  /// <summary>
  /// This class is used for mapping with procedure and get result based on input parameter.
  /// </summary>
  //CMP #636: Standard Update Mobilization
  public class ChargeCodeSearchData
  {
    public int Id { get; set; }
    public String ChargeCategory { get; set; }
    public String ChargeCode { get; set; }
    public String ChargeCodeTypeRequirement { get; set; }
    //TFS#9976 :Firefox: v47 : Master maintenance:Incorrect Activate ,Deactivate action buttons are displayed for the records in Charge Code Type Requirement Setup and Charge Code Type Name Setup masters
    public String IsActive { get; set; }
    public DateTime LastUpdatedOn { get; set; }
  }
}
