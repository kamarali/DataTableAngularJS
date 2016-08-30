using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.MemberProfile
{
  public class FutureUpdateTemp : EntityBase<int>
  {
   
    public int MemberId { get; set; }

    public string MemberCodeNumeric { get; set; }

    public string MemberCodeAlpha { get; set; }

    public string MemberCommercialName { get; set; }

    public string MembeLegalName { get; set; }

    public int ActionId { get; set; }

    public string TableName { get; set; }

    public string DisplayGroup { get; set; }

    public string ElementName { get; set; }

    public string ActionType { get; set; }

    public string OldValueDisplayName { get; set; }

    public string NewValueDisplayName { get; set; }

    public DateTime ModifiedOn { get; set; }

    public DateTime PeriodDatetime { get; set; }

    public string RelationIdDisplayName { get; set; }

    public string LastUpdatedByName { get; set; }

    //TODO:need to make it more compact and less error prone
    public string ChangeEffectivePeriod
    {
      get
      {
        string period = String.Empty;
        if (PeriodDatetime != null)
        {
          period = Convert.ToString(PeriodDatetime.ToString("dd/MM/yyyy"));
          if (period == "01-01-0001")
          {
            period = String.Empty;
          }
          else
          {
            //period = "P" + period.Substring(1, 1);
            period = period.Substring(3, 3) + " " + period.Substring(7, 4) + " " + "P" + period.Substring(1, 1);
          }
        }

        return period;
      }
    }

  }
}
