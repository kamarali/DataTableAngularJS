namespace Iata.IS.Model.MemberProfile
{
  public class AchReinstatement
  {
    public string Period { get; set; }

    public string Month { get; set; }

    public string Year { get; set; }

    public Member Member { get; set; }

    public int MemberId { get; set; }
  }
}
