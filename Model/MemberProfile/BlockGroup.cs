using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.MemberProfile.Enums;

namespace Iata.IS.Model.MemberProfile
{
  public class BlockGroup : EntityBase<int>
  {
    //False = By True = Against
    public bool IsBlockAgainst { get; set; }

    public int ByAgainst { get; set; }

    // Property to get and set By/Against string name on BlockGroup JQGrid
    public string ByAgainstString { get; set; }

    public int ZoneTypeId { get; set; }

    public IchZoneType ZoneType
    {
      get
      {
        return (IchZoneType)ZoneTypeId;
      }
      set
      {
        ZoneTypeId = Convert.ToInt32(value);
      }
    }

    public string DisplayZoneType{ get; set; }
    //{
    //  get
    //  {
    //    return EnumList.GetIchZoneDisplayValue((IchZone)(ZoneTypeId));
    //  }
    //}

    public bool Pax { get; set; }

    public bool Cargo { get; set; }

    public bool Uatp { get; set; }

    public bool Misc { get; set; }

    public BlockingRule BlockingRule { get; set; }

    public int BlockingRuleId { get; set; }

    // Property to get and set row count of BlockGroup JQgrid.
    public int TempRowCount { get; set; }

    // Property to get and set whether BlockGroup row is deleted on client side. 
    public bool IsDeleted { get; set; }

    public int GroupId { get; set; }

  }
}

