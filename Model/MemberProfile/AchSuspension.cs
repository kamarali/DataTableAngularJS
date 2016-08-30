namespace Iata.IS.Model.MemberProfile
{
  public class AchSuspension
  {
    public int Period { get; set; }

    public int Month { get; set; }

    public int Year { get; set; }

    public int DefaultPeriod { get; set; }

    public int DefaultMonth { get; set; }

    public int DefaultYear { get; set; }

    public Member Member { get; set; }

    public int MemberId { get; set; }
  }

}
