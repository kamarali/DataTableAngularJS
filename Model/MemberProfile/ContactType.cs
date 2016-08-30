using Iata.IS.Model.Base;
using Iata.IS.Model.MemberProfile.Enums;
using System;
namespace Iata.IS.Model.MemberProfile
{
  public class ContactType : EntityBase<int>
  {
    public string ContactTypeName { get; set; }

    // Added properties as per the database entities
    public bool Required { get; set; }

    public int OrderNo { get; set; }

    public string DependentField { get; set; }

    public TypeOfContactType TypeOfContactType
    {
      get
      {
        return (TypeOfContactType)TypeId;
      }
      set
      {
        TypeId = Convert.ToInt32(value);
      }
    }

    public string TypeOfContactTypeName
    {
        get
        {
            return ((TypeOfContactType)TypeId).ToString();
        }
    }

    public int TypeId { get; set; }

    public bool IsActive { get; set; }

    public bool Member { get; set; }

    public bool EBilling { get; set; }

    public bool Pax { get; set; }

    public bool Cgo { get; set; }

    public bool Misc { get; set; }

    public bool Uatp { get; set; }

    public bool Ich { get; set; }

    public bool Ach { get; set; }

    public bool Technical { get; set; }

    public bool ReservedGroup1 { get; set; }

    public bool ReservedGroup2 { get; set; }

    public bool ReservedGroup3 { get; set; }

    public int GroupId { get; set; }

    public int SubGroupId { get; set; }

    public ContactTypeGroup ContactTypeGroup { get; set; }

    public ContactTypeSubGroup ContactTypeSubGroup { get; set; }

    public bool AchOpsContactRpt { get; set; }

    public bool AchOpsMemRpt { get; set; }

    public bool IchOpsMemRpt { get; set; }

    public bool IchOpsConctactRpt { get; set; }

    public bool IsAccMemRpt { get; set; }

    public bool IsAccConctactRpt { get; set; }

    public bool IsAccOwnMemRpt { get; set; }

    public bool IsAccOwnContactRpt { get; set; }

    public int? SequenceNo { get; set; }

    public bool ViewableMem { get; set; }

    public bool ViewableSIS { get; set; }

    public bool ViewableICH { get; set; }

    public bool ViewableACH { get; set; }

    public bool EditableMem { get; set; }

    public bool EditableSIS { get; set; }

    public bool EditableICH { get; set; }

    public bool EditableACH { get; set; }

  }
}
